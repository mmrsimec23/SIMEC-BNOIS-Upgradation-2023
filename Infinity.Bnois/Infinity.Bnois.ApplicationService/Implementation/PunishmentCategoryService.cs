﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;

namespace Infinity.Bnois.ApplicationService.Implementation
{
   

    public class PunishmentCategoryService : IPunishmentCategoryService
    {
        private readonly IBnoisRepository<PunishmentCategory> punishmentCategoryRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public PunishmentCategoryService(IBnoisRepository<PunishmentCategory> punishmentCategoryRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.punishmentCategoryRepository = punishmentCategoryRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<PunishmentCategoryModel> GetPunishmentCategories(int pageSize, int pageNumber, string searchText, out int total)
        {
            IQueryable<PunishmentCategory> punishmentCategories = punishmentCategoryRepository
                .FilterWithInclude(x => x.IsActive
                && ((x.Name.Contains(searchText)) || String.IsNullOrEmpty(searchText)));
            total = punishmentCategories.Count();
            punishmentCategories = punishmentCategories.OrderByDescending(x => x.PunishmentCategoryId).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            List<PunishmentCategoryModel> models = ObjectConverter<PunishmentCategory, PunishmentCategoryModel>.ConvertList(punishmentCategories.ToList()).ToList();
            return models;
        }

        public async Task<PunishmentCategoryModel> GetPunishmentCategory(int  id)
        {
            if (id <= 0)
            {
                return new PunishmentCategoryModel();
            }
            PunishmentCategory punishmentCategory = await punishmentCategoryRepository.FindOneAsync(x => x.PunishmentCategoryId == id);

            if (punishmentCategory == null)
            {
                throw new InfinityNotFoundException("Punishment Category not found!");
            }
            PunishmentCategoryModel model = ObjectConverter<PunishmentCategory, PunishmentCategoryModel>.Convert(punishmentCategory);
            return model;
        }

        public async Task<PunishmentCategoryModel> SavePunishmentCategory(int id, PunishmentCategoryModel model)
        {
            bool isExist = await punishmentCategoryRepository.ExistsAsync(x => (x.Name == model.Name) && x.PunishmentCategoryId != model.PunishmentCategoryId);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Punishment Category data already exist !");
            }
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Punishment Category data missing!");
            }

            PunishmentCategory punishmentCategory = ObjectConverter<PunishmentCategoryModel, PunishmentCategory>.Convert(model);

            if (id > 0)
            {
                punishmentCategory = await punishmentCategoryRepository.FindOneAsync(x => x.PunishmentCategoryId == id);
                if (punishmentCategory == null)
                {
                    throw new InfinityNotFoundException("Punishment Category not found!");
                }
                punishmentCategory.ModifiedDate = DateTime.Now;
                punishmentCategory.ModifiedBy = model.ModifiedBy;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PunishmentCategory";
                bnLog.TableEntryForm = "Punishment Category";
                bnLog.PreviousValue = "Id: " + model.PunishmentCategoryId;
                bnLog.UpdatedValue = "Id: " + model.PunishmentCategoryId;

                if (punishmentCategory.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + punishmentCategory.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (punishmentCategory.ShortName != model.ShortName)
                {
                    bnLog.PreviousValue += ", Short Name: " + punishmentCategory.ShortName;
                    bnLog.UpdatedValue += ", Short Name: " + model.ShortName;
                }


                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = model.ModifiedBy;
                bnLog.LogCreatedDate = DateTime.Now;

                if (punishmentCategory.Name != model.Name || punishmentCategory.ShortName != model.ShortName)
                {
                    await bnoisLogRepository.SaveAsync(bnLog);

                }
                else
                {
                    throw new InfinityNotFoundException("Please Update Any Field!");
                }
                //data log section end
            }
            else
            {
                punishmentCategory.CreatedBy = model.CreatedBy;
                punishmentCategory.CreatedDate = DateTime.Now;
                punishmentCategory.IsActive = true;
            }
            punishmentCategory.Name = model.Name;
            punishmentCategory.ShortName = model.ShortName;

            await punishmentCategoryRepository.SaveAsync(punishmentCategory);
            model.PunishmentCategoryId = punishmentCategory.PunishmentCategoryId;
            return model;
        }

        public async Task<bool> DeletePunishmentCategory(int id)
        {
            if (id <= 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            PunishmentCategory punishmentCategory = await punishmentCategoryRepository.FindOneAsync(x => x.PunishmentCategoryId == id);
            if (punishmentCategory == null)
            {
                throw new InfinityNotFoundException("Punishment Category not found!");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PunishmentCategory";
                bnLog.TableEntryForm = "Punishment Category";

                bnLog.PreviousValue = "Id: " + punishmentCategory.PunishmentCategoryId + ", Name: " + punishmentCategory.Name + ", Short Name: " + punishmentCategory.ShortName;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                return await punishmentCategoryRepository.DeleteAsync(punishmentCategory);
            }
        }

        public async Task<List<SelectModel>> GetPunishmentCategorySelectModels()
        {
            ICollection<PunishmentCategory> models = await punishmentCategoryRepository.FilterAsync(x => x.IsActive);
            return models.OrderBy(x => x.Name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.PunishmentCategoryId
            }).ToList();
        }

    }
}