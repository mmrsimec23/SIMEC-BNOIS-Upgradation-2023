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
   public class ServiceExamService: IServiceExamService
    {

        private readonly IBnoisRepository<ServiceExam> serviceExamRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public ServiceExamService(IBnoisRepository<ServiceExam> serviceExamRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.serviceExamRepository = serviceExamRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<ServiceExamModel> GetServiceExams(int ps, int pn, string qs, out int total)
        {
            IQueryable<ServiceExam> serviceExams = serviceExamRepository.FilterWithInclude(x => x.IsActive
                && ((x.Name.Contains(qs) || String.IsNullOrEmpty(qs)) ||
                (x.ServiceExamCategory.ExamName.Contains(qs) || String.IsNullOrEmpty(qs))), "ServiceExamCategory","Branch");
            total = serviceExams.Count();
            serviceExams = serviceExams.OrderByDescending(x => x.ServiceExamId).Skip((pn - 1) * ps).Take(ps);
            List<ServiceExamModel> models = ObjectConverter<ServiceExam, ServiceExamModel>.ConvertList(serviceExams.ToList()).ToList();
            return models;
        }

        public async Task<ServiceExamModel> GetServiceExam(int id)
        {
            if (id <= 0)
            {
                return new ServiceExamModel();
            }
            ServiceExam serviceExam = await serviceExamRepository.FindOneAsync(x => x.ServiceExamId == id);
            if (serviceExam == null)
            {
                throw new InfinityNotFoundException("Service Exam not found");
            }
            ServiceExamModel model = ObjectConverter<ServiceExam, ServiceExamModel>.Convert(serviceExam);
            return model;
        }

    
        public async Task<ServiceExamModel> SaveServiceExam(int id, ServiceExamModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Service Exam data missing");
            }

            bool isExistData = serviceExamRepository.Exists(x => x.ServiceExamCategoryId == model.ServiceExamCategoryId && x.Name == model.Name && x.ServiceExamId != id);
            if (isExistData)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            ServiceExam serviceExam = ObjectConverter<ServiceExamModel, ServiceExam>.Convert(model);
            if (id > 0)
            {
                serviceExam = await serviceExamRepository.FindOneAsync(x => x.ServiceExamId == id, new List<string>() { "ServiceExamCategory", "Branch" });
                if (serviceExam == null)
                {
                    throw new InfinityNotFoundException("Service Exam not found !");
                }

                serviceExam.ModifiedDate = DateTime.Now;
                serviceExam.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ServiceExam";
                bnLog.TableEntryForm = "Service Exam";
                bnLog.PreviousValue = "Id: " + model.ServiceExamId;
                bnLog.UpdatedValue = "Id: " + model.ServiceExamId;
                int bnoisUpdateCount = 0;
                if (serviceExam.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + serviceExam.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                    bnoisUpdateCount += 1;
                }
                if (serviceExam.ShortName != model.ShortName)
                {
                    bnLog.PreviousValue += ", ShortName: " + serviceExam.ShortName;
                    bnLog.UpdatedValue += ", ShortName: " + model.ShortName;
                    bnoisUpdateCount += 1;
                }
                if (serviceExam.ServiceExamCategoryId != model.ServiceExamCategoryId)
                {
                    var sec = employeeService.GetDynamicTableInfoById("ServiceExamCategory", "ServiceExamCategoryId", model.ServiceExamCategoryId);
                    bnLog.PreviousValue += ", ServiceExamCategory: " + serviceExam.ServiceExamCategory.ExamName;
                    bnLog.UpdatedValue += ", ServiceExamCategory: " + ((dynamic)sec).ExamName;
                    bnoisUpdateCount += 1;
                }
                if (serviceExam.BranchId != model.BranchId)
                {
                    var branch = employeeService.GetDynamicTableInfoById("Branch", "BranchId", model.BranchId);
                    bnLog.PreviousValue += ", Branch: " + serviceExam.Branch.Name;
                    bnLog.UpdatedValue += ", Branch: " + ((dynamic)branch).Name;
                    bnoisUpdateCount += 1;
                }
                if (serviceExam.NOS != model.NOS)
                {
                    bnLog.PreviousValue += ", NOS: " + serviceExam.NOS;
                    bnLog.UpdatedValue += ", NOS: " + model.NOS;
                    bnoisUpdateCount += 1;
                }
                if (serviceExam.AttTime != model.AttTime)
                {
                    bnLog.PreviousValue += ", AttTime: " + serviceExam.AttTime;
                    bnLog.UpdatedValue += ", AttTime: " + model.AttTime;
                    bnoisUpdateCount += 1;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (bnoisUpdateCount > 0)
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
                serviceExam.IsActive = true;
                serviceExam.CreatedDate = DateTime.Now;
                serviceExam.CreatedBy = userId;
            }
            serviceExam.Name = model.Name;
            serviceExam.ShortName = model.ShortName;
            serviceExam.ServiceExamCategoryId = model.ServiceExamCategoryId;
            serviceExam.ServiceExamCategory = null;
            serviceExam.BranchId = model.BranchId;
            serviceExam.Branch = null;
            serviceExam.NOS = model.NOS;
            serviceExam.AttTime = model.AttTime;
            await serviceExamRepository.SaveAsync(serviceExam);
            model.ServiceExamId = serviceExam.ServiceExamId;
            return model;
        }


        public async Task<bool> DeleteServiceExam(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            ServiceExam serviceExam = await serviceExamRepository.FindOneAsync(x => x.ServiceExamId == id);
            if (serviceExam == null)
            {
                throw new InfinityNotFoundException("Service Exam not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ServiceExam";
                bnLog.TableEntryForm = "Service Exam";
                bnLog.PreviousValue = "Id: " + serviceExam.ServiceExamId + ", Name: " + serviceExam.Name + ", ShortName: " + serviceExam.ShortName
                    + ", ServiceExamCategory: " + serviceExam.ServiceExamCategoryId + ", Branch: " + serviceExam.BranchId + ", NOS: " + serviceExam.NOS + ", AttTime: " + serviceExam.AttTime;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await serviceExamRepository.DeleteAsync(serviceExam);
            }
        }


        public async Task<List<SelectModel>> GetServiceExamSelectModelsByServiceExamCategory(int id)
        {
            ICollection<ServiceExam> serviceExams = await serviceExamRepository.FilterAsync(x => x.IsActive && x.ServiceExamCategoryId == id);
            List<SelectModel> selectModels = serviceExams.OrderBy(x => x.Name).Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.ServiceExamId
            }).ToList();
            return selectModels;

        }

    }
}
