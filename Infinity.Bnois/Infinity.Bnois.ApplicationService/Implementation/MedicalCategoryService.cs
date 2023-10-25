using System;
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
   

    public class MedicalCategoryService : IMedicalCategoryService
    {
        private readonly IBnoisRepository<MedicalCategory> medicalCategoryRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public MedicalCategoryService(IBnoisRepository<MedicalCategory> medicalCategoryRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.medicalCategoryRepository = medicalCategoryRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }

        public List<MedicalCategoryModel> GetMedicalCategories(int pageSize, int pageNumber, string searchText, out int total)
        {
            IQueryable<MedicalCategory> medicalCategories = medicalCategoryRepository
                .FilterWithInclude(x => x.IsActive
                && ((x.Name.Contains(searchText)) || String.IsNullOrEmpty(searchText)));
            total = medicalCategories.Count();
            medicalCategories = medicalCategories.OrderByDescending(x => x.MedicalCategoryId).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            List<MedicalCategoryModel> models = ObjectConverter<MedicalCategory, MedicalCategoryModel>.ConvertList(medicalCategories.ToList()).ToList();
            return models;
        }

        public async Task<MedicalCategoryModel> GetMedicalCategory(int  medicalCategoryId)
        {
            if (medicalCategoryId <= 0)
            {
                return new MedicalCategoryModel();
            }
            MedicalCategory medicalCategory = await medicalCategoryRepository.FindOneAsync(x => x.MedicalCategoryId == medicalCategoryId);

            if (medicalCategory == null)
            {
                throw new InfinityNotFoundException("Medical Category not found!");
            }
            MedicalCategoryModel model = ObjectConverter<MedicalCategory, MedicalCategoryModel>.Convert(medicalCategory);
            return model;
        }

        public async Task<MedicalCategoryModel> SaveMedicalCategory(int medicalCategoryId, MedicalCategoryModel model)
        {
            bool isExist = await medicalCategoryRepository.ExistsAsync(x => (x.Name == model.Name) && x.MedicalCategoryId != model.MedicalCategoryId);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Medical Category data already exist !");
            }
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Medical Category data missing!");
            }

            MedicalCategory medicalCategory = ObjectConverter<MedicalCategoryModel, MedicalCategory>.Convert(model);

            if (medicalCategoryId > 0)
            {
                medicalCategory = await medicalCategoryRepository.FindOneAsync(x => x.MedicalCategoryId == medicalCategoryId);
                if (medicalCategory == null)
                {
                    throw new InfinityNotFoundException("Medical Category not found!");
                }
                medicalCategory.ModifiedDate = DateTime.Now;
                medicalCategory.ModifiedBy = model.ModifiedBy;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "MedicalCategory";
                bnLog.TableEntryForm = "Medical Category";
                bnLog.PreviousValue = "Id: " + model.MedicalCategoryId;
                bnLog.UpdatedValue = "Id: " + model.MedicalCategoryId;
                if (medicalCategory.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + medicalCategory.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (medicalCategory.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + medicalCategory.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = model.CreatedBy;
                bnLog.LogCreatedDate = DateTime.Now;

                if (medicalCategory.Name != model.Name || medicalCategory.Remarks != model.Remarks)
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
                medicalCategory.CreatedBy = model.CreatedBy;
                medicalCategory.CreatedDate = DateTime.Now;
                medicalCategory.IsActive = true;
            }
            medicalCategory.Name = model.Name;
            medicalCategory.Remarks = model.Remarks;
            await medicalCategoryRepository.SaveAsync(medicalCategory);
            model.MedicalCategoryId = medicalCategory.MedicalCategoryId;
            return model;
        }

        public async Task<bool> DeleteMedicalCategory(int medicalCategoryId)
        {
            if (medicalCategoryId <= 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            MedicalCategory medicalCategory = await medicalCategoryRepository.FindOneAsync(x => x.MedicalCategoryId == medicalCategoryId);
            if (medicalCategory == null)
            {
                throw new InfinityNotFoundException("Medical Category not found!");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "MedicalCategory";
                bnLog.TableEntryForm = "Medical Category";
                bnLog.PreviousValue = "Id: " + medicalCategory.MedicalCategoryId + ", Name: " + medicalCategory.Name + ", Remarks: " + medicalCategory.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await medicalCategoryRepository.DeleteAsync(medicalCategory);
            }
        }

        public async Task<List<SelectModel>> GetMedicalCategorySelectModels()
        {
            ICollection<MedicalCategory> models = await medicalCategoryRepository.FilterAsync(x => x.IsActive);
            return models.Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.MedicalCategoryId
            }).ToList();
        }

    }
}