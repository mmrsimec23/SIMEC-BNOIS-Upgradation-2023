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
    public class StatusChangeService : IStatusChangeService
    {
        private readonly IBnoisRepository<StatusChange> statusChangeRepository;
        private readonly IBnoisRepository<EmployeeGeneral> employeeGeneralRepository;
        private readonly IBnoisRepository<PhysicalCondition> physicalConditionRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;

        public StatusChangeService(IBnoisRepository<StatusChange> statusChangeRepository, IBnoisRepository<EmployeeGeneral> employeeGeneralRepository,
            IBnoisRepository<PhysicalCondition> physicalConditionRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.statusChangeRepository = statusChangeRepository;
            this.employeeGeneralRepository = employeeGeneralRepository;
            this.physicalConditionRepository = physicalConditionRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;

        }

        public List<StatusChangeModel> GetStatusChanges(int ps, int pn, string qs, out int total)
        {
            IQueryable<StatusChange> statusChanges = statusChangeRepository.FilterWithInclude(x => x.IsActive
                && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee");
            total = statusChanges.Count();
            statusChanges = statusChanges.OrderByDescending(x => x.StatusChangeId).Skip((pn - 1) * ps).Take(ps);
            List<StatusChangeModel> models = ObjectConverter<StatusChange, StatusChangeModel>.ConvertList(statusChanges.ToList()).ToList();


            

            return models;
        }

        public async Task<StatusChangeModel> GetStatusChange(int id)
        {
            if (id <= 0)
            {
                return new StatusChangeModel();
            }
            StatusChange statusChange = await statusChangeRepository.FindOneAsync(x => x.StatusChangeId == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch" });
            if (statusChange == null)
            {
                throw new InfinityNotFoundException("Status not found");
            }
            StatusChangeModel model = ObjectConverter<StatusChange, StatusChangeModel>.Convert(statusChange);
            return model;
        }

       
        public async Task<StatusChangeModel> SaveStatusChange(int id, StatusChangeModel model)
        {
            
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Training Result  data missing");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            StatusChange statusChange = ObjectConverter<StatusChangeModel, StatusChange>.Convert(model);
            if (id > 0)
            {
                statusChange = await statusChangeRepository.FindOneAsync(x => x.StatusChangeId == id);
                if (statusChange == null)
                {
                    throw new InfinityNotFoundException("Status not found !");
                }
                
                statusChange.ModifiedDate = DateTime.Now;
                statusChange.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "StatusChange";
                bnLog.TableEntryForm = "Status Change";
                bnLog.PreviousValue = "Id: " + model.StatusChangeId;
                bnLog.UpdatedValue = "Id: " + model.StatusChangeId;
                if (statusChange.EmployeeId != model.EmployeeId)
                {
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", Name: " + statusChange.Employee.Name + " _ " + statusChange.Employee.PNo;
                    bnLog.UpdatedValue += ", Name: " + ((dynamic)emp).Name + " _ " + ((dynamic)emp).PNo;
                }

                if (statusChange.StatusType != model.StatusType)
                {
                    bnLog.PreviousValue += ", StatusType: " + (statusChange.StatusType == 1 ? "Medical Category" : statusChange.StatusType == 2 ? "Eye Vision" : statusChange.StatusType == 3 ? "Commission Type" : statusChange.StatusType == 4 ? "Branch" : "Religion");
                    bnLog.UpdatedValue += ", StatusType: " + (model.StatusType == 1 ? "Medical Category" : model.StatusType == 2 ? "Eye Vision" : model.StatusType == 3 ? "Commission Type" : model.StatusType == 4 ? "Branch" : "Religion");
                }
                if (statusChange.PreviousId != model.PreviousId)
                {
                    bnLog.PreviousValue += ", Previous: " + statusChange.PreviousId;
                    bnLog.UpdatedValue += ", Previous: " + model.PreviousId;
                }
                if (statusChange.NewId != model.NewId)
                {
                    bnLog.PreviousValue += ", New: " + statusChange.NewId;
                    bnLog.UpdatedValue += ", New: " + model.NewId;
                }
                if (statusChange.MedicalCategoryCause != model.MedicalCategoryCause)
                {
                    bnLog.PreviousValue += ", MedicalCategoryCause: " + statusChange.MedicalCategoryCause;
                    bnLog.UpdatedValue += ", MedicalCategoryCause: " + model.MedicalCategoryCause;
                }
                if (statusChange.MedicalCategoryType != model.MedicalCategoryType)
                {
                    bnLog.PreviousValue += ", MedicalCategoryType: " + statusChange.MedicalCategoryType;
                    bnLog.UpdatedValue += ", MedicalCategoryType: " + model.MedicalCategoryType;
                }
                if (statusChange.Date != model.Date)
                {
                    bnLog.PreviousValue += ", Date: " + statusChange.Date;
                    bnLog.UpdatedValue += ", Date: " + model.Date;
                }
                if (statusChange.DateTo != model.DateTo)
                {
                    bnLog.PreviousValue += ", DateTo: " + statusChange.DateTo;
                    bnLog.UpdatedValue += ", DateTo: " + model.DateTo;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (statusChange.EmployeeId != model.EmployeeId || statusChange.PreviousId != model.PreviousId || statusChange.NewId != model.NewId
                    || statusChange.MedicalCategoryCause != model.MedicalCategoryCause || statusChange.MedicalCategoryType != model.MedicalCategoryType
                    || statusChange.Date != model.Date || statusChange.DateTo != model.DateTo || statusChange.StatusType != model.StatusType)
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

                statusChange.IsActive = true;
                statusChange.CreatedDate = DateTime.Now;
                statusChange.CreatedBy = userId;
            }
            statusChange.EmployeeId = model.EmployeeId;
            statusChange.PreviousId = model.PreviousId;
            statusChange.NewId = model.NewId;
            statusChange.MedicalCategoryCause = model.MedicalCategoryCause;
            statusChange.MedicalCategoryType = model.MedicalCategoryType;
            statusChange.Date = model.Date;
            statusChange.DateTo = model.DateTo;
            statusChange.StatusType = model.StatusType;

            statusChange.Employee = null;
           int result=  await statusChangeRepository.SaveAsync(statusChange);
            model.StatusChangeId = statusChange.StatusChangeId;

            if (result==1)
            {
                if (model.Date <= DateTime.Now)
                {
                    if (model.StatusType == 1)
                    {
                        var physicalCondition = await physicalConditionRepository.FindOneAsync(x => x.EmployeeId == model.EmployeeId);
                        physicalCondition.MedicalCategoryId = model.NewId;
                        physicalCondition.MedicalCategoryCause = model.MedicalCategoryCause;
                        physicalCondition.MedicalCategoryType = model.MedicalCategoryType;
                        physicalCondition.MedicalCategoryDateFrom = model.Date;
                        physicalCondition.MedicalCategoryDateTo = model.DateTo;
                        await physicalConditionRepository.SaveAsync(physicalCondition);
                    }
                    else if (model.StatusType == 2)
                    {
                        var physicalCondition = await physicalConditionRepository.FindOneAsync(x => x.EmployeeId == model.EmployeeId);
                        physicalCondition.EyeVisionId = model.NewId;
                        await physicalConditionRepository.SaveAsync(physicalCondition);


                    }
                    else if (model.StatusType == 3)
                    {
                        var employeeGeneral = await employeeGeneralRepository.FindOneAsync(x => x.EmployeeId == model.EmployeeId);
                        employeeGeneral.CommissionTypeId = model.NewId;
                        await employeeGeneralRepository.SaveAsync(employeeGeneral);

                    }
                    else if (model.StatusType == 4)
                    {
                        var employeeGeneral = await employeeGeneralRepository.FindOneAsync(x => x.EmployeeId == model.EmployeeId);
                        employeeGeneral.BranchId = model.NewId;
                        await employeeGeneralRepository.SaveAsync(employeeGeneral);

                    }
                    else if (model.StatusType == 5)
                    {
                        var employeeGeneral = await employeeGeneralRepository.FindOneAsync(x => x.EmployeeId == model.EmployeeId);
                        employeeGeneral.ReligionId = model.NewId;
                        await employeeGeneralRepository.SaveAsync(employeeGeneral);

                    }

                }
            }

            

            return model;
        }


        public async Task<bool> DeleteStatusChange(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            StatusChange statusChange = await statusChangeRepository.FindOneAsync(x => x.StatusChangeId == id);
            if (statusChange == null)
            {
                throw new InfinityNotFoundException("Status not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "StatusChange";
                bnLog.TableEntryForm = "Status Change";
                var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", statusChange.EmployeeId);
                bnLog.PreviousValue = "Id: " + statusChange.StatusChangeId + ", Name: " + ((dynamic)emp).Name + ", Previous: " + statusChange.PreviousId
                    + ", New: " + statusChange.NewId + ", MedicalCategoryCause: " + statusChange.MedicalCategoryCause + ", MedicalCategoryType: " + statusChange.MedicalCategoryType
                    + ", Date: " + statusChange.Date + ", DateTo: " + statusChange.DateTo + ", StatusType: " + statusChange.StatusType;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                var result = await statusChangeRepository.DeleteAsync(statusChange);
             
                return result;
            }
        }

        public async Task<int> GetEmployeeCommissionType(int employeeId)
        {
           
            EmployeeGeneral employeeGeneral = await employeeGeneralRepository.FindOneAsync(x => x.EmployeeId == employeeId);
            if (employeeGeneral == null)
            {
                throw new InfinityNotFoundException("Employee not found");
            }


            return employeeGeneral.CommissionTypeId;
        }

        public async Task<int> GetEmployeeBranch(int employeeId)
        {

            EmployeeGeneral employeeGeneral = await employeeGeneralRepository.FindOneAsync(x => x.EmployeeId == employeeId);
            if (employeeGeneral == null)
            {
                throw new InfinityNotFoundException("Employee not found");
            }


            return employeeGeneral.BranchId;
        }
        public async Task<int> GetEmployeeReligion(int employeeId)
        {

            EmployeeGeneral employeeGeneral = await employeeGeneralRepository.FindOneAsync(x => x.EmployeeId == employeeId);
            if (employeeGeneral == null)
            {
                throw new InfinityNotFoundException("Employee not found");
            }


            return employeeGeneral.ReligionId ?? 0;
        }

        public async Task<int> GetEmployeeMedicalCategory(int employeeId)
        {

            PhysicalCondition physicalCondition = await physicalConditionRepository.FindOneAsync(x => x.EmployeeId == employeeId);
            if (physicalCondition == null)
            {
                throw new InfinityNotFoundException("Employee not found");
            }


            return physicalCondition.MedicalCategoryId;
        }

        public async Task<int> GetEmployeeEyeVision(int employeeId)
        {

            PhysicalCondition physicalCondition = await physicalConditionRepository.FindOneAsync(x => x.EmployeeId == employeeId);
            if (physicalCondition == null)
            {
                throw new InfinityNotFoundException("Employee not found");
            }


            return physicalCondition.EyeVisionId;
        }

        
    }
}
