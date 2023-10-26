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
   

    public class ServiceExamCategoryService : IServiceExamCategoryService
    {
        private readonly IBnoisRepository<ServiceExamCategory> serviceExamCategoryRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public ServiceExamCategoryService(IBnoisRepository<ServiceExamCategory> serviceExamCategoryRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.serviceExamCategoryRepository = serviceExamCategoryRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }

        public List<ServiceExamCategoryModel> GetServiceExamCategories(int pageSize, int pageNumber, string searchText, out int total)
        {
            IQueryable<ServiceExamCategory> serviceExamCategories = serviceExamCategoryRepository
                .FilterWithInclude(x => x.IsActive
                && ((x.ExamName.Contains(searchText)) || String.IsNullOrEmpty(searchText)));
            total = serviceExamCategories.Count();
            serviceExamCategories = serviceExamCategories.OrderByDescending(x => x.ServiceExamCategoryId).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            List<ServiceExamCategoryModel> models = ObjectConverter<ServiceExamCategory, ServiceExamCategoryModel>.ConvertList(serviceExamCategories.ToList()).ToList();
            return models;
        }

        public async Task<ServiceExamCategoryModel> GetServiceExamCategory(int  id)
        {
            if (id <= 0)
            {
                return new ServiceExamCategoryModel();
            }
            ServiceExamCategory serviceExamCategory = await serviceExamCategoryRepository.FindOneAsync(x => x.ServiceExamCategoryId == id);

            if (serviceExamCategory == null)
            {
                throw new InfinityNotFoundException("Service Exam Category not found!");
            }
            ServiceExamCategoryModel model = ObjectConverter<ServiceExamCategory, ServiceExamCategoryModel>.Convert(serviceExamCategory);
            return model;
        }

        public async Task<ServiceExamCategoryModel> SaveServiceExamCategory(int id, ServiceExamCategoryModel model)
        {
            bool isExist = await serviceExamCategoryRepository.ExistsAsync(x => (x.ExamName == model.ExamName) && x.ServiceExamCategoryId != model.ServiceExamCategoryId);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Service Exam Category data already exist !");
            }
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Service Exam Category data missing!");
            }

            ServiceExamCategory serviceExamCategory = ObjectConverter<ServiceExamCategoryModel, ServiceExamCategory>.Convert(model);

            if (id > 0)
            {
                serviceExamCategory = await serviceExamCategoryRepository.FindOneAsync(x => x.ServiceExamCategoryId == id);
                if (serviceExamCategory == null)
                {
                    throw new InfinityNotFoundException("ServiceExam Category not found!");
                }
                serviceExamCategory.ModifiedDate = DateTime.Now;
                serviceExamCategory.ModifiedBy = model.ModifiedBy;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ServiceExamCategory";
                bnLog.TableEntryForm = "Service Exam Category";
                bnLog.PreviousValue = "Id: " + model.ServiceExamCategoryId;
                bnLog.UpdatedValue = "Id: " + model.ServiceExamCategoryId;
                if (serviceExamCategory.ExamName != model.ExamName)
                {
                    bnLog.PreviousValue += ", ExamName: " + serviceExamCategory.ExamName;
                    bnLog.UpdatedValue += ", ExamName: " + model.ExamName;
                }
                if (serviceExamCategory.ShortName != model.ShortName)
                {
                    bnLog.PreviousValue += ", ShortName: " + serviceExamCategory.ShortName;
                    bnLog.UpdatedValue += ", ShortName: " + model.ShortName;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = model.CreatedBy;
                bnLog.LogCreatedDate = DateTime.Now;

                if (serviceExamCategory.ExamName != model.ExamName || serviceExamCategory.ShortName != model.ShortName)
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
                serviceExamCategory.CreatedBy = model.CreatedBy;
                serviceExamCategory.CreatedDate = DateTime.Now;
                serviceExamCategory.IsActive = true;
            }
            serviceExamCategory.ExamName = model.ExamName;
            serviceExamCategory.ShortName = model.ShortName;

            await serviceExamCategoryRepository.SaveAsync(serviceExamCategory);
            model.ServiceExamCategoryId = serviceExamCategory.ServiceExamCategoryId;
            return model;
        }

        public async Task<bool> DeleteServiceExamCategory(int id)
        {
            if (id <= 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            ServiceExamCategory serviceExamCategory = await serviceExamCategoryRepository.FindOneAsync(x => x.ServiceExamCategoryId == id);
            if (serviceExamCategory == null)
            {
                throw new InfinityNotFoundException("Service Exam Category not found!");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ServiceExamCategory";
                bnLog.TableEntryForm = "Service Exam Category";
                bnLog.PreviousValue = "Id: " + serviceExamCategory.ServiceExamCategoryId + ", ExamName: " + serviceExamCategory.ExamName + ", ShortName: " + serviceExamCategory.ShortName;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await serviceExamCategoryRepository.DeleteAsync(serviceExamCategory);
            }
        }

        public async Task<List<SelectModel>> GetServiceExamCategorySelectModels()
        {
            ICollection<ServiceExamCategory> models = await serviceExamCategoryRepository.FilterAsync(x => x.IsActive);
            return models.OrderBy(x => x.ExamName).Select(x => new SelectModel()
            {
                Text = x.ExamName,
                Value = x.ServiceExamCategoryId
            }).ToList();
        }

    }
}