using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;

namespace Infinity.Bnois.ApplicationService.Implementation
{
   public class PunishmentSubCategoryService: IPunishmentSubCategoryService
    {

        private readonly IBnoisRepository<PunishmentSubCategory> punishmentSubCategoryRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public PunishmentSubCategoryService(IBnoisRepository<PunishmentSubCategory> punishmentSubCategoryRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.punishmentSubCategoryRepository = punishmentSubCategoryRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<PunishmentSubCategoryModel> GetPunishmentSubCategories(int ps, int pn, string qs, out int total)
        {
            IQueryable<PunishmentSubCategory> punishmentSubCategories = punishmentSubCategoryRepository.FilterWithInclude(x => x.IsActive
                && ((x.Name.Contains(qs) || String.IsNullOrEmpty(qs)) ||
                (x.PunishmentCategory.Name.Contains(qs) || String.IsNullOrEmpty(qs))), "PunishmentCategory");
            total = punishmentSubCategories.Count();
            punishmentSubCategories = punishmentSubCategories.OrderByDescending(x => x.PunishmentSubCategoryId).Skip((pn - 1) * ps).Take(ps);
            List<PunishmentSubCategoryModel> models = ObjectConverter<PunishmentSubCategory, PunishmentSubCategoryModel>.ConvertList(punishmentSubCategories.ToList()).ToList();
            return models;
        }

        public async Task<PunishmentSubCategoryModel> GetPunishmentSubCategory(int id)
        {
            if (id <= 0)
            {
                return new PunishmentSubCategoryModel();
            }
            PunishmentSubCategory punishmentSubCategory = await punishmentSubCategoryRepository.FindOneAsync(x => x.PunishmentSubCategoryId == id);
            if (punishmentSubCategory == null)
            {
                throw new InfinityNotFoundException("Punishment Sub Category not found");
            }
            PunishmentSubCategoryModel model = ObjectConverter<PunishmentSubCategory, PunishmentSubCategoryModel>.Convert(punishmentSubCategory);
            return model;
        }

    
        public async Task<PunishmentSubCategoryModel> SavePunishmentSubCategory(int id, PunishmentSubCategoryModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("PunishmentSubCategory data missing");
            }

            bool isExistData = punishmentSubCategoryRepository.Exists(x => x.PunishmentCategoryId == model.PunishmentCategoryId && x.Name == model.Name && x.PunishmentSubCategoryId != id);
            if (isExistData)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            PunishmentSubCategory punishmentSubCategory = ObjectConverter<PunishmentSubCategoryModel, PunishmentSubCategory>.Convert(model);
            if (id > 0)
            {
                punishmentSubCategory = await punishmentSubCategoryRepository.FindOneAsync(x => x.PunishmentSubCategoryId == id);
                if (punishmentSubCategory == null)
                {
                    throw new InfinityNotFoundException("Punishment Sub Category not found !");
                }

                punishmentSubCategory.ModifiedDate = DateTime.Now;
                punishmentSubCategory.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PunishmentSubCategory";
                bnLog.TableEntryForm = "Punishment Sub Category";
                bnLog.PreviousValue = "Id: " + model.PunishmentSubCategoryId;
                bnLog.UpdatedValue = "Id: " + model.PunishmentSubCategoryId;
                if (punishmentSubCategory.PunishmentCategoryId != model.PunishmentCategoryId)
                {
                    
                        var prevPublicationCategory = employeeService.GetDynamicTableInfoById("PunishmentCategory", "PunishmentCategoryId", punishmentSubCategory.PunishmentCategoryId);
                        bnLog.PreviousValue += ", Punishment Category: " + ((dynamic)prevPublicationCategory).Name;
                   
                        var newPublicationCategory = employeeService.GetDynamicTableInfoById("PunishmentCategory", "PunishmentCategoryId", model.PunishmentCategoryId);
                        bnLog.UpdatedValue += ", Punishment Category: " + ((dynamic)newPublicationCategory).Name;
                    
                }
                if (punishmentSubCategory.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + punishmentSubCategory.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (punishmentSubCategory.ShortName != model.ShortName)
                {
                    bnLog.PreviousValue += ", Short Name: " + punishmentSubCategory.ShortName;
                    bnLog.UpdatedValue += ", Short Name: " + model.ShortName;
                }
                if (punishmentSubCategory.GotoTrace != model.GotoTrace)
                {
                    bnLog.PreviousValue += ", Go to Trace: " + punishmentSubCategory.GotoTrace;
                    bnLog.UpdatedValue += ", Go to Trace: " + model.GotoTrace;
                }


                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (punishmentSubCategory.PunishmentCategoryId != model.PunishmentCategoryId || punishmentSubCategory.Name != model.Name || punishmentSubCategory.ShortName != model.ShortName || punishmentSubCategory.GotoTrace != model.GotoTrace)
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
                punishmentSubCategory.IsActive = true;
                punishmentSubCategory.CreatedDate = DateTime.Now;
                punishmentSubCategory.CreatedBy = userId;
            }
            punishmentSubCategory.Name = model.Name;
            punishmentSubCategory.ShortName = model.ShortName;
            punishmentSubCategory.GotoTrace = model.GotoTrace;
            punishmentSubCategory.PunishmentCategoryId = model.PunishmentCategoryId;
            await punishmentSubCategoryRepository.SaveAsync(punishmentSubCategory);
            model.PunishmentSubCategoryId = punishmentSubCategory.PunishmentSubCategoryId;
            return model;
        }


        public async Task<bool> DeletePunishmentSubCategory(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            PunishmentSubCategory punishmentSubCategory = await punishmentSubCategoryRepository.FindOneAsync(x => x.PunishmentSubCategoryId == id);
            if (punishmentSubCategory == null)
            {
                throw new InfinityNotFoundException("Punishment Sub Category not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PunishmentSubCategory";
                bnLog.TableEntryForm = "Punishment Sub Category";

                bnLog.PreviousValue = "Id: " + punishmentSubCategory.PunishmentSubCategoryId + ", Name: " + punishmentSubCategory.Name + ", Short Name: " + punishmentSubCategory.ShortName + ", Go To Trace: " + punishmentSubCategory.GotoTrace;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                return await punishmentSubCategoryRepository.DeleteAsync(punishmentSubCategory);
            }
        }


        public async Task<List<SelectModel>> GetPunishmentSubCategorySelectModelsByPunishmentCategory(int id)
        {
            ICollection<PunishmentSubCategory> punishmentSubCategories = await punishmentSubCategoryRepository.FilterAsync(x => x.IsActive && x.PunishmentCategoryId == id);
            List<SelectModel> selectModels = punishmentSubCategories.OrderBy(x=>x.Name).Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.PunishmentSubCategoryId
            }).ToList();
            return selectModels;

        }

        public async Task<List<SelectModel>> GetPunishmentSubCategoryForTrace()
        {
            ICollection<PunishmentSubCategory> punishmentSubCategories = await punishmentSubCategoryRepository.FilterAsync(x => x.IsActive && x.GotoTrace);
            List<SelectModel> selectModels = punishmentSubCategories.OrderBy(x=>x.Name).Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.PunishmentSubCategoryId
            }).ToList();
            return selectModels;
        }
        public async Task<List<SelectModel>> GetPunishmentSubCategorySelectModels()
        {
            ICollection<PunishmentSubCategory> punishmentSubCategories = await punishmentSubCategoryRepository.FilterAsync(x => x.IsActive);
            List<SelectModel> selectModels = punishmentSubCategories.OrderBy(x => x.Name).OrderBy(x=>x.Name).Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.PunishmentSubCategoryId
            }).ToList();
            return selectModels;
        }
    }
}
