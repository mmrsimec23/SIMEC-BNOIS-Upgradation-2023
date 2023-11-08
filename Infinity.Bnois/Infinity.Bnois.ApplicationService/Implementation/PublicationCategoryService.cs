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
   

    public class PublicationCategoryService : IPublicationCategoryService
    {
        private readonly IBnoisRepository<PublicationCategory> publicationCategoryRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public PublicationCategoryService(IBnoisRepository<PublicationCategory> publicationCategoryRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.publicationCategoryRepository = publicationCategoryRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<PublicationCategoryModel> GetPublicationCategories(int pageSize, int pageNumber, string searchText, out int total)
        {
            IQueryable<PublicationCategory> publicationCategories = publicationCategoryRepository
                .FilterWithInclude(x => x.IsActive
                && ((x.Name.Contains(searchText)) || String.IsNullOrEmpty(searchText)));
            total = publicationCategories.Count();
            publicationCategories = publicationCategories.OrderByDescending(x => x.PublicationCategoryId).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            List<PublicationCategoryModel> models = ObjectConverter<PublicationCategory, PublicationCategoryModel>.ConvertList(publicationCategories.ToList()).ToList();
            return models;
        }

        public async Task<PublicationCategoryModel> GetPublicationCategory(int  id)
        {
            if (id <= 0)
            {
                return new PublicationCategoryModel();
            }
            PublicationCategory publicationCategory = await publicationCategoryRepository.FindOneAsync(x => x.PublicationCategoryId == id);

            if (publicationCategory == null)
            {
                throw new InfinityNotFoundException("Publication Category not found!");
            }
            PublicationCategoryModel model = ObjectConverter<PublicationCategory, PublicationCategoryModel>.Convert(publicationCategory);
            return model;
        }

        public async Task<PublicationCategoryModel> SavePublicationCategory(int id, PublicationCategoryModel model)
        {
            bool isExist = await publicationCategoryRepository.ExistsAsync(x => (x.Name == model.Name) && x.PublicationCategoryId != model.PublicationCategoryId);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Publication Category data already exist !");
            }
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Publication Category data missing!");
            }

            PublicationCategory publicationCategory = ObjectConverter<PublicationCategoryModel, PublicationCategory>.Convert(model);

            if (id > 0)
            {
                publicationCategory = await publicationCategoryRepository.FindOneAsync(x => x.PublicationCategoryId == id);
                if (publicationCategory == null)
                {
                    throw new InfinityNotFoundException("Publication Category not found!");
                }
                publicationCategory.ModifiedDate = DateTime.Now;
                publicationCategory.ModifiedBy = model.ModifiedBy;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PublicationCategory";
                bnLog.TableEntryForm = "Publication Category";
                bnLog.PreviousValue = "Id: " + model.PublicationCategoryId;
                bnLog.UpdatedValue = "Id: " + model.PublicationCategoryId;
                
                if (publicationCategory.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + publicationCategory.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (publicationCategory.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + publicationCategory.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }
                if (publicationCategory.GoToTrace != model.GoToTrace)
                {
                    bnLog.PreviousValue += ", Go To Trace: " + publicationCategory.GoToTrace;
                    bnLog.UpdatedValue += ", Go To Trace: " + model.GoToTrace;
                }


                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = model.ModifiedBy;
                bnLog.LogCreatedDate = DateTime.Now;

                if (publicationCategory.Name != model.Name || publicationCategory.Remarks != model.Remarks || publicationCategory.GoToTrace != model.GoToTrace)
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
                publicationCategory.CreatedBy = model.CreatedBy;
                publicationCategory.CreatedDate = DateTime.Now;
                publicationCategory.IsActive = true;
            }
            publicationCategory.Name = model.Name;
            publicationCategory.Remarks = model.Remarks;
            publicationCategory.GoToTrace = model.GoToTrace;
            await publicationCategoryRepository.SaveAsync(publicationCategory);
            model.PublicationCategoryId = publicationCategory.PublicationCategoryId;
            return model;
        }

        public async Task<bool> DeletePublicationCategory(int id)
        {
            if (id <= 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            PublicationCategory publicationCategory = await publicationCategoryRepository.FindOneAsync(x => x.PublicationCategoryId == id);
            if (publicationCategory == null)
            {
                throw new InfinityNotFoundException("Publication Category not found!");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PublicationCategory";
                bnLog.TableEntryForm = "Publication Category";

                bnLog.PreviousValue = "Id: " + publicationCategory.PublicationCategoryId + ", Name: " + publicationCategory.Name + ", Remarks: " + publicationCategory.Remarks + ", Go To Trace: " + publicationCategory.GoToTrace;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);
                return await publicationCategoryRepository.DeleteAsync(publicationCategory);
            }
        }

        public async Task<List<SelectModel>> GetPublicationCategorySelectModels()
        {
            ICollection<PublicationCategory> models = await publicationCategoryRepository.FilterAsync(x => x.IsActive);
            return models.OrderBy(x => x.Name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.PublicationCategoryId
            }).ToList();
        }

        public async Task<List<SelectModel>> GetTracePublicationCategorySelectModels()
        {
            ICollection<PublicationCategory> models = await publicationCategoryRepository.FilterAsync(x => x.IsActive && x.GoToTrace);
            return models.OrderBy(x => x.Name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.PublicationCategoryId
            }).ToList();
        }
    }
}