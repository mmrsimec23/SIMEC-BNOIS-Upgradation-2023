using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class EmployeeServiceExamResultService : IEmployeeServiceExamResultService
    {
        private readonly IBnoisRepository<EmployeeServiceExamResult> employeeServiceExamResultRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public EmployeeServiceExamResultService(IBnoisRepository<EmployeeServiceExamResult> employeeServiceExamResultRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.employeeServiceExamResultRepository = employeeServiceExamResultRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<EmployeeServiceExamResultModel> GetEmployeeServiceExamResults(int ps, int pn, string qs, out int total)
        {
            IQueryable<EmployeeServiceExamResult> employeeServiceExamResults = employeeServiceExamResultRepository.FilterWithInclude(x => x.IsActive
                && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee", "ServiceExam","ServiceExamCategory");
            total = employeeServiceExamResults.Count();
            employeeServiceExamResults = employeeServiceExamResults.OrderByDescending(x => x.EmployeeServiceExamResultId).Skip((pn - 1) * ps).Take(ps);
            List<EmployeeServiceExamResultModel> models = ObjectConverter<EmployeeServiceExamResult, EmployeeServiceExamResultModel>.ConvertList(employeeServiceExamResults.ToList()).ToList();
            return models;
        }

        public async Task<EmployeeServiceExamResultModel> GetEmployeeServiceExamResult(int id)
        {
            if (id <= 0)
            {
                return new EmployeeServiceExamResultModel();
            }
            EmployeeServiceExamResult employeeServiceExamResult = await employeeServiceExamResultRepository.FindOneAsync(x => x.EmployeeServiceExamResultId == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch" });
            if (employeeServiceExamResult == null)
            {
                throw new InfinityNotFoundException("Employee Service Exam Result not found");
            }
            EmployeeServiceExamResultModel model = ObjectConverter<EmployeeServiceExamResult, EmployeeServiceExamResultModel>.Convert(employeeServiceExamResult);
            return model;
        }



        public async Task<EmployeeServiceExamResultModel> SaveEmployeeServiceExamResult(int id, EmployeeServiceExamResultModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Employee Service Exam Result  data missing");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();


            bool isExist = await employeeServiceExamResultRepository.ExistsAsync(x => x.EmployeeId ==model.EmployeeId && x.ServiceExamCategoryId==model.ServiceExamCategoryId && x.PassFailResult==true && x.EmployeeServiceExamResultId != id);

            if (isExist)
            {
                throw new InfinityInvalidDataException("Officer already qualified. !");
            }


            EmployeeServiceExamResult employeeServiceExamResult = ObjectConverter<EmployeeServiceExamResultModel, EmployeeServiceExamResult>.Convert(model);
            if (id > 0)
            {
                employeeServiceExamResult = employeeServiceExamResultRepository.FindOne(x => x.EmployeeServiceExamResultId == id, new List<string> { "Employee", "Employee.Rank", "ServiceExamCategory", "ServiceExam" });
                if (employeeServiceExamResult == null)
                {
                    throw new InfinityNotFoundException("Employee Service Exam Result not found !");
                }

                employeeServiceExamResult.ModifiedDate = DateTime.Now;
                employeeServiceExamResult.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeServiceExamResult";
                bnLog.TableEntryForm = "Service Exam Result";
                bnLog.PreviousValue = "Id: " + model.EmployeeServiceExamResultId;
                bnLog.UpdatedValue = "Id: " + model.EmployeeServiceExamResultId;
                if (employeeServiceExamResult.EmployeeId != model.EmployeeId)
                {
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId ?? 0);
                    bnLog.PreviousValue += ", Name: " + employeeServiceExamResult.Employee.Name + " _ " + employeeServiceExamResult.Employee.PNo;
                    bnLog.UpdatedValue += ", Name: " + ((dynamic)emp).Name + " _ " + ((dynamic)emp).PNo;
                }
                if (employeeServiceExamResult.ServiceExamCategoryId != model.ServiceExamCategoryId)
                {
                    var sec = employeeService.GetDynamicTableInfoById("ServiceExamCategory", "ServiceExamCategoryId", model.ServiceExamCategoryId);
                    bnLog.PreviousValue += ", Category: " + employeeServiceExamResult.ServiceExamCategory.ExamName;
                    bnLog.UpdatedValue += ", Category: " + ((dynamic)sec).ExamName;
                }
                if (employeeServiceExamResult.ServiceExamId != model.ServiceExamId)
                {
                    var sexam = employeeService.GetDynamicTableInfoById("ServiceExam", "ServiceExamId", model.ServiceExamId);
                    bnLog.PreviousValue += ", ServiceExam: " + employeeServiceExamResult.ServiceExam.Name;
                    bnLog.UpdatedValue += ", ServiceExam: " + ((dynamic)sexam).Name;
                }
                if (employeeServiceExamResult.RankId != model.RankId)
                {
                    var rank = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.RankId ?? 0);
                    bnLog.PreviousValue += ", Rank: " + employeeServiceExamResult.Employee.Rank.ShortName;
                    bnLog.UpdatedValue += ", Rank: " + ((dynamic)rank).ShortName;
                }
                if (employeeServiceExamResult.TransferId != model.TransferId)
                {
                    bnLog.PreviousValue += ", Transfer: " + employeeServiceExamResult.TransferId;
                    bnLog.UpdatedValue += ", Transfer: " + model.TransferId;
                }
                if (employeeServiceExamResult.ExamDate != model.ExamDate)
                {
                    bnLog.PreviousValue += ", ExamDate: " + employeeServiceExamResult.ExamDate;
                    bnLog.UpdatedValue += ", ExamDate: " + model.ExamDate;
                }
                if (employeeServiceExamResult.NumberOfSubject != model.NumberOfSubject)
                {
                    bnLog.PreviousValue += ", NumberOfSubject: " + employeeServiceExamResult.NumberOfSubject;
                    bnLog.UpdatedValue += ", NumberOfSubject: " + model.NumberOfSubject;
                }
                if (employeeServiceExamResult.AttTime != model.AttTime)
                {
                    bnLog.PreviousValue += ", AttTime: " + employeeServiceExamResult.AttTime;
                    bnLog.UpdatedValue += ", AttTime: " + model.AttTime;
                }
                if (employeeServiceExamResult.PassFailResult != model.PassFailResult)
                {
                    bnLog.PreviousValue += ", PassFailResult: " + employeeServiceExamResult.PassFailResult;
                    bnLog.UpdatedValue += ", PassFailResult: " + model.PassFailResult;
                }
                if (employeeServiceExamResult.IsExempted != model.IsExempted)
                {
                    bnLog.PreviousValue += ", IsExempted: " + employeeServiceExamResult.IsExempted;
                    bnLog.UpdatedValue += ", IsExempted: " + model.IsExempted;
                }
                if (employeeServiceExamResult.ExemptedDate != model.ExemptedDate)
                {
                    bnLog.PreviousValue += ", ExemptedDate: " + employeeServiceExamResult.ExemptedDate;
                    bnLog.UpdatedValue += ", ExemptedDate: " + model.ExemptedDate;
                }
                if (employeeServiceExamResult.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + employeeServiceExamResult.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (employeeServiceExamResult.EmployeeId != model.EmployeeId || employeeServiceExamResult.ServiceExamCategoryId != model.ServiceExamCategoryId
                    || employeeServiceExamResult.ServiceExamId != model.ServiceExamId || employeeServiceExamResult.RankId != model.RankId || employeeServiceExamResult.TransferId != model.TransferId
                    || employeeServiceExamResult.ExamDate != model.ExamDate || employeeServiceExamResult.NumberOfSubject != model.NumberOfSubject || employeeServiceExamResult.AttTime != model.AttTime 
                    || employeeServiceExamResult.PassFailResult != model.PassFailResult || employeeServiceExamResult.IsExempted != model.IsExempted || employeeServiceExamResult.ExemptedDate != model.ExemptedDate
                    || employeeServiceExamResult.Remarks != model.Remarks)
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
                employeeServiceExamResult.IsActive = true;
                employeeServiceExamResult.CreatedDate = DateTime.Now;
                employeeServiceExamResult.CreatedBy = userId;
            }
            employeeServiceExamResult.EmployeeId = model.EmployeeId;
            employeeServiceExamResult.ServiceExamCategoryId = model.ServiceExamCategoryId;
            employeeServiceExamResult.ServiceExamId = model.ServiceExamId;
            employeeServiceExamResult.ExamDate = model.ExamDate;
            employeeServiceExamResult.NumberOfSubject = model.NumberOfSubject;
            employeeServiceExamResult.AttTime = model.AttTime;
            employeeServiceExamResult.PassFailResult = model.PassFailResult;
            employeeServiceExamResult.IsExempted = model.IsExempted;
            employeeServiceExamResult.ExemptedDate = model.ExemptedDate;
            employeeServiceExamResult.Remarks = model.Remarks;

            employeeServiceExamResult.RankId = model.Employee.RankId;
            employeeServiceExamResult.TransferId = model.Employee.TransferId;

            if (model.IsBackLog)
            {
                employeeServiceExamResult.RankId = model.RankId;
                employeeServiceExamResult.TransferId = model.TransferId;
            }
            //employeeServiceExamResult.Employee = null;
            await employeeServiceExamResultRepository.SaveAsync(employeeServiceExamResult);
            model.EmployeeServiceExamResultId = employeeServiceExamResult.EmployeeServiceExamResultId;
            return model;
        }


        public async Task<bool> DeleteEmployeeServiceExamResult(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            EmployeeServiceExamResult employeeServiceExamResult = await employeeServiceExamResultRepository.FindOneAsync(x => x.EmployeeServiceExamResultId == id);
            if (employeeServiceExamResult == null)
            {
                throw new InfinityNotFoundException("Employee Service Exam Result not found");
            }
            else

            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeServiceExamResult";
                bnLog.TableEntryForm = "Service Exam Result";
                var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeServiceExamResult.EmployeeId ?? 0);
                var sec = employeeService.GetDynamicTableInfoById("ServiceExamCategory", "ServiceExamCategoryId", employeeServiceExamResult.ServiceExamCategoryId);
                var sexam = employeeService.GetDynamicTableInfoById("ServiceExam", "ServiceExamId", employeeServiceExamResult.ServiceExamId);
                var rank = employeeService.GetDynamicTableInfoById("Rank", "RankId", employeeServiceExamResult.RankId ?? 0);
                bnLog.PreviousValue = "Id: " + employeeServiceExamResult.EmployeeServiceExamResultId + ", Name: " + ((dynamic)emp).Name + ", Category: " + ((dynamic)sec).ExamName
                    + ", ServiceExam: " + ((dynamic)sexam).Name + ", Rank: " + ((dynamic)rank).ShortName + ", Transfer: " + employeeServiceExamResult.TransferId
                    + ", ExamDate: " + employeeServiceExamResult.ExamDate + ", NumberOfSubject: " + employeeServiceExamResult.NumberOfSubject + ", AttTime: " + employeeServiceExamResult.AttTime
                    + ", PassFailResult: " + employeeServiceExamResult.PassFailResult + ", IsExempted: " + employeeServiceExamResult.IsExempted + ", ExemptedDate: " + employeeServiceExamResult.ExemptedDate;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await employeeServiceExamResultRepository.DeleteAsync(employeeServiceExamResult);
            }
        }




    }
}
