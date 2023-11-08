using Infinity.Bnois.Api;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IBnoisRepository<Employee> employeeRepository;
        private readonly IBnoisRepository<OfficerType> officerTypeRepository;
        private readonly IBnoisRepository<EmployeeStatus> employeeStatusRepository;
        private readonly IBnoisRepository<EmployeeGeneral> employeeGeneralRepository;
        private readonly IBnoisRepository<AgeServicePolicy> ageServicePolicyRepository;
        private readonly IProcessRepository processRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public EmployeeService(IBnoisRepository<AgeServicePolicy> ageServicePolicyRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IBnoisRepository<EmployeeGeneral> employeeGeneralRepository, IBnoisRepository<EmployeeStatus> employeeStatusRepository, IBnoisRepository<Employee> employeeRepository, IBnoisRepository<OfficerType> officerTypeRepository, IProcessRepository processRepository)
        {
            this.employeeRepository = employeeRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.officerTypeRepository = officerTypeRepository;
            this.employeeStatusRepository = employeeStatusRepository;
            this.processRepository = processRepository;
            this.employeeGeneralRepository = employeeGeneralRepository;
            this.ageServicePolicyRepository = ageServicePolicyRepository;
        }

        public List<EmployeeModel> GetEmployees(int ps, int pn, string qs, out int total)
        {
            IQueryable<Employee> employees = employeeRepository.FilterWithInclude(x => x.Active && (x.PNo == (qs) ||
            x.FullNameEng.Contains(qs) ||
            String.IsNullOrEmpty(qs)), "Batch", "Gender", "Rank");
            total = employees.Count();
            employees = employees.OrderBy(x => x.Seniority).Skip((pn - 1) * ps).Take(ps);
            List<EmployeeModel> models = ObjectConverter<Employee, EmployeeModel>.ConvertList(employees.ToList()).ToList();
            return models;
        }

        public async Task<EmployeeModel> SaveEmployee(int id, EmployeeModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Officer's data missing!");
            }

            bool isExist = await employeeRepository.ExistsAsync(x => (x.PNo == model.PNo) && x.EmployeeId != model.EmployeeId);

            if (isExist)
            {
                throw new InfinityInvalidDataException("Officer's Personal No. data already exist !");
            }


            Employee employee = ObjectConverter<EmployeeModel, Employee>.Convert(model);

            if (id > 0)
            {
                employee = await employeeRepository.FindOneAsync(x => x.EmployeeId == id);
                if (employee == null)
                {
                    throw new InfinityNotFoundException("Officer not found!");
                }
                EmployeeStatus employeeStatus = await employeeStatusRepository.FindOneAsync(x => x.EmployeeStatusId == employee.EmployeeStatusId);

                employee.EmployeeStatusId = employeeStatus.GCode;
                employee.SLCode = employeeStatus.SLCode;
                employee.ModifiedDate = DateTime.Now;
                employee.ModifiedBy = model.ModifiedBy;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Employee";
                bnLog.TableEntryForm = "Employee";
                bnLog.PreviousValue = "Id: " + model.EmployeeId;
                bnLog.UpdatedValue = "Id: " + model.EmployeeId;

                if (employee.PNo != model.PNo)
                {
                    bnLog.PreviousValue += ", Personal Number: " + employee.PNo;
                    bnLog.UpdatedValue += ", Personal Number: " + model.PNo;
                }
                if (employee.OfficerTypeId != model.OfficerTypeId)
                {
                    if (employee.OfficerTypeId > 0)
                    {
                        var prevOfficerType = GetDynamicTableInfoById("OfficerType", "OfficerTypeId", employee.OfficerTypeId);
                        bnLog.PreviousValue += ", Officer Type: " + ((dynamic)prevOfficerType).Name;
                    }
                    if (model.OfficerTypeId > 0)
                    {
                        var newOfficerType = GetDynamicTableInfoById("OfficerType", "OfficerTypeId", model.OfficerTypeId);
                        bnLog.UpdatedValue += ", Officer Type: " + ((dynamic)newOfficerType).Name;
                    }
                }
                if (employee.RankCategoryId != model.RankCategoryId)
                {
                    if (employee.RankCategoryId > 0)
                    {
                        var prevRankCategory = GetDynamicTableInfoById("RankCategory", "RankCategoryId", employee.RankCategoryId);
                        bnLog.PreviousValue += ", Organization: " + ((dynamic)prevRankCategory).Name;
                    }
                    if (model.RankCategoryId > 0)
                    {
                        var newRankCategory = GetDynamicTableInfoById("RankCategory", "RankCategoryId", model.RankCategoryId);
                        bnLog.UpdatedValue += ", Organization: " + ((dynamic)newRankCategory).Name;
                    }
                }
                if (employee.CountryId != model.CountryId)
                {
                    if (employee.CountryId > 0)
                    {
                        var prevCountry = GetDynamicTableInfoById("Country", "CountryId", employee.CountryId);
                        bnLog.PreviousValue += ", Country: " + ((dynamic)prevCountry).FullName;
                    }
                    if (model.CountryId > 0)
                    {
                        var newCountry = GetDynamicTableInfoById("Country", "CountryId", model.CountryId);
                        bnLog.UpdatedValue += ", Country: " + ((dynamic)newCountry).FullName;
                    }
                }
                if (employee.BnNo != model.BnNo)
                {
                    bnLog.PreviousValue += ", BN Number: " + employee.BnNo;
                    bnLog.UpdatedValue += ", BN Number: " + model.BnNo;
                }
                if (employee.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Full Name: " + employee.Name;
                    bnLog.UpdatedValue += ", Full Name: " + model.Name;
                }
                if (employee.FullNameBan != model.FullNameBan)
                {
                    bnLog.PreviousValue += ", Full Name (বাংলা): " + employee.FullNameBan;
                    bnLog.UpdatedValue += ", Full Name (বাংলা): " + model.FullNameBan;
                }

                if (employee.BatchId != model.BatchId)
                {
                    if (employee.BatchId > 0)
                    {
                        var prevBatch = GetDynamicTableInfoById("Batch", "BatchId", employee.BatchId??0);
                        bnLog.PreviousValue += ", Batch: " + ((dynamic)prevBatch).Name;
                    }
                    if (model.BatchId > 0)
                    {
                        var newBatch = GetDynamicTableInfoById("Batch", "BatchId", model.BatchId??0);
                        bnLog.UpdatedValue += ", Batch: " + ((dynamic)newBatch).Name;
                    }
                }
                if (employee.BatchPosition != model.BatchPosition)
                {
                    bnLog.PreviousValue += ", Batch Position: " + employee.BatchPosition;
                    bnLog.UpdatedValue += ", Batch Position: " + model.BatchPosition;
                }
                if (employee.GenderId != model.GenderId)
                {
                    if (employee.GenderId > 0)
                    {
                        var prevGender = GetDynamicTableInfoById("Gender", "GenderId", employee.GenderId);
                        bnLog.PreviousValue += ", Gender: " + ((dynamic)prevGender).Name;
                    }
                    if (model.BatchId > 0)
                    {
                        var newGender = GetDynamicTableInfoById("Gender", "GenderId", model.GenderId);
                        bnLog.UpdatedValue += ", Gender: " + ((dynamic)newGender).Name;
                    }
                }
                if (employee.RankId != model.RankId)
                {
                    if (employee.RankId > 0)
                    {
                        var prevRank = GetDynamicTableInfoById("Rank", "RankId", employee.RankId);
                        bnLog.PreviousValue += ", Rank: " + ((dynamic)prevRank).FullName;
                    }
                    if (model.RankId > 0)
                    {
                        var newRank = GetDynamicTableInfoById("Rank", "RankId", model.RankId);
                        bnLog.UpdatedValue += ", Gender: " + ((dynamic)newRank).FullName;
                    }
                }
                if (employee.EmployeeStatusId != model.EmployeeStatusId)
                {
                    if (employee.EmployeeStatusId > 0)
                    {
                        var prevEmployeeStatus = GetDynamicTableInfoById("EmployeeStatus", "EmployeeStatusId", employee.EmployeeStatusId);
                        bnLog.PreviousValue += ", Status: " + ((dynamic)prevEmployeeStatus).StatusTitle;
                    }
                    if (model.EmployeeStatusId > 0)
                    {
                        var newEmployeeStatus = GetDynamicTableInfoById("EmployeeStatus", "EmployeeStatusId", model.EmployeeStatusId);
                        bnLog.UpdatedValue += ", Status: " + ((dynamic)newEmployeeStatus).StatusTitle;
                    }
                }
                if (employee.Notification != model.Notification)
                {
                    bnLog.PreviousValue += ", Special Notification: " + employee.Notification;
                    bnLog.UpdatedValue += ", Special Notification: " + model.Notification;
                }
                if (employee.ExecutionRemarkId != model.ExecutionRemarkId)
                {
                    if (employee.ExecutionRemarkId > 0)
                    {
                        var prevExecutionRemark = GetDynamicTableInfoById("ExecutionRemark", "ExecutionRemarkId", employee.ExecutionRemarkId??0);
                        bnLog.PreviousValue += ", Board Execution Remark(Promotion): " + ((dynamic)prevExecutionRemark).Name;
                    }
                    if (model.ExecutionRemarkId > 0)
                    {
                        var newExecutionRemark = GetDynamicTableInfoById("ExecutionRemark", "ExecutionRemarkId", model.ExecutionRemarkId??0);
                        bnLog.UpdatedValue += ", Board Execution Remark(Promotion): " + ((dynamic)newExecutionRemark).Name;
                    }
                }
                if (employee.BExecutionDate != model.BExecutionDate)
                {
                    bnLog.PreviousValue += ", Board Execution Date (Promotion): " + employee.BExecutionDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Board Execution Date (Promotion): " + model.BExecutionDate?.ToString("dd/MM/yyyy");
                }
                if (employee.BSpRemarks != model.BSpRemarks)
                {
                    bnLog.PreviousValue += ", Broad Sheet Special Remarks (Nomination): " + employee.BSpRemarks;
                    bnLog.UpdatedValue += ", Broad Sheet Special Remarks (Nomination): " + model.BSpRemarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = model.ModifiedBy;
                bnLog.LogCreatedDate = DateTime.Now;

                if (employee.PNo != model.PNo || employee.OfficerTypeId != model.OfficerTypeId || employee.RankCategoryId != model.RankCategoryId || employee.CountryId != model.CountryId || employee.BnNo != model.BnNo || employee.Name != model.Name || employee.FullNameBan != model.FullNameBan || employee.BatchId != model.BatchId || employee.BatchPosition != model.BatchPosition || employee.GenderId != model.GenderId || employee.RankId != model.RankId || employee.EmployeeStatusId != model.EmployeeStatusId || employee.Notification != model.Notification || employee.ExecutionRemarkId != model.ExecutionRemarkId || employee.BExecutionDate != model.BExecutionDate || employee.BSpRemarks != model.BSpRemarks)
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
                employee.CreatedBy = model.CreatedBy;
                employee.CreatedDate = DateTime.Now;
                employee.Active = true;
                employee.EmployeeStatusId = (int)OfficerCurrentStatus.Active;
                employee.SLCode = (int)OfficerCurrentStatus.Active;
            }
            employee.PNo = model.PNo;
            employee.BnNo = model.BnNo;
            employee.Name = model.Name;
            employee.FullNameEng = model.Name;
            employee.FullNameBan = model.FullNameBan;
            employee.BatchId = model.BatchId;
            employee.BatchPosition = model.BatchPosition;
            employee.GenderId = model.GenderId;
            employee.RankCategoryId = model.RankCategoryId;
            employee.OfficerTypeId = model.OfficerTypeId;
            employee.CountryId = model.CountryId;
            employee.RankId = model.RankId;
            employee.Notification = model.Notification;
            employee.ExecutionRemarkId = model.ExecutionRemarkId;
            employee.BExecutionDate = model.BExecutionDate;
            employee.BSpRemarks = model.BSpRemarks;




        await employeeRepository.SaveAsync(employee);
            model.EmployeeId = employee.EmployeeId;

            await processRepository.UpdateNamingConvention(employee.EmployeeId);
            return model;
        }

        public async Task<bool> DeleteEmployee(int id)
        {
            if (id <= 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Employee employee = await employeeRepository.FindOneAsync(x => x.EmployeeId == id);
            if (employee == null)
            {
                throw new InfinityNotFoundException("Officer not found!");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();

                bnLog.TableName = "Employee";
                bnLog.TableEntryForm = "Employee";
                bnLog.PreviousValue = "Id: " + employee.EmployeeId + ", Personal Number: " + employee.PNo;
                if (employee.OfficerTypeId > 0)
                {
                    var OfficerType = GetDynamicTableInfoById("OfficerType", "OfficerTypeId", employee.OfficerTypeId);
                    bnLog.PreviousValue += ", Officer Type: " + ((dynamic)OfficerType).Name;
                }
                if (employee.RankCategoryId > 0)
                {
                    var prevRankCategory = GetDynamicTableInfoById("RankCategory", "RankCategoryId", employee.RankCategoryId);
                    bnLog.PreviousValue += ", Organization: " + ((dynamic)prevRankCategory).Name;
                }
                if (employee.CountryId > 0)
                {
                    var prevCountry = GetDynamicTableInfoById("Country", "CountryId", employee.CountryId);
                    bnLog.PreviousValue += ", Country: " + ((dynamic)prevCountry).FullName;
                }
                bnLog.PreviousValue += ", BN Number: " + employee.BnNo + ", Full Name: " + employee.Name + ", Full Name (বাংলা): " + employee.FullNameBan;
                if (employee.BatchId > 0)
                {
                    var prevBatch = GetDynamicTableInfoById("Batch", "BatchId", employee.BatchId ?? 0);
                    bnLog.PreviousValue += ", Batch: " + ((dynamic)prevBatch).Name;
                }
                bnLog.PreviousValue += ", Batch Position: " + employee.BatchPosition;
                if (employee.GenderId > 0)
                {
                    var prevGender = GetDynamicTableInfoById("Gender", "GenderId", employee.GenderId);
                    bnLog.PreviousValue += ", Gender: " + ((dynamic)prevGender).Name;
                }
                if (employee.RankId > 0)
                {
                    var prevRank = GetDynamicTableInfoById("Rank", "RankId", employee.RankId);
                    bnLog.PreviousValue += ", Rank: " + ((dynamic)prevRank).FullName;
                }
                if (employee.EmployeeStatusId > 0)
                {
                    var prevEmployeeStatus = GetDynamicTableInfoById("EmployeeStatus", "EmployeeStatusId", employee.EmployeeStatusId);
                    bnLog.PreviousValue += ", Status: " + ((dynamic)prevEmployeeStatus).StatusTitle;
                }
                bnLog.PreviousValue += ", Special Notification: " + employee.Notification;
                if (employee.ExecutionRemarkId > 0)
                {
                    var prevExecutionRemark = GetDynamicTableInfoById("ExecutionRemark", "ExecutionRemarkId", employee.ExecutionRemarkId ?? 0);
                    bnLog.PreviousValue += ", Board Execution Remark(Promotion): " + ((dynamic)prevExecutionRemark).Name;
                }
                bnLog.PreviousValue += ", Board Execution Date (Promotion): " + employee.BExecutionDate?.ToString("dd/MM/yyyy");
                bnLog.PreviousValue += ", Broad Sheet Special Remarks (Nomination): " + employee.BSpRemarks + ", SLCode:" + employee.SLCode + ", HasDollarSign: " + employee.HasDollarSign + ", Reason: " + employee.Reason + ", DateOfDollarSign: " + employee.DateOfDollarSign?.ToString("dd/MM/yyyy") + ", Seniority: " + employee.Seniority;

                if (employee.TransferId > 0)
                {
                    var prevTransfer = GetDynamicTableInfoById("vwTransfer", "TransferId", employee.TransferId ?? 0);
                    bnLog.PreviousValue += ", Born/Attach/Appointment: " + ((dynamic)prevTransfer).BornOffice + '/' + ((dynamic)prevTransfer).CurrentAttach + '/' + ((dynamic)prevTransfer).Appointment;
                    
                }

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                return await employeeRepository.DeleteAsync(employee);
            }
        }

        public async Task<EmployeeModel> GetEmployee(int id)
        {
            if (id <= 0)
            {
                return new EmployeeModel();
            }
            Employee employee = await employeeRepository.FindOneAsync(x => x.EmployeeId == id, new List<string> { "Batch", "Gender", "Rank" });

            if (employee == null)
            {
                throw new InfinityNotFoundException("Officer not found!");
            }
            EmployeeModel model = ObjectConverter<Employee, EmployeeModel>.Convert(employee);
            return model;
        }


        public async Task<EmployeeModel> GetEmployeeByPO(string pno)
        {
            if (String.IsNullOrEmpty(pno))
            {
                return new EmployeeModel();
            }
            Employee employee = await employeeRepository.FindOneAsync(x => x.PNo == pno, new List<string> {  "Rank", "Batch", "EmployeeGeneral", "ExecutionRemark" });
           
            if (employee == null)
            {
                throw new InfinityNotFoundException("Officer not found!");
            }
            return ObjectConverter<Employee, EmployeeModel>.Convert(employee);

        }

        public async Task<List<SelectModel>> GetOfficerTypeSelectModels()
        {
            ICollection<OfficerType> models = await officerTypeRepository.FilterAsync(x => x.OfficerTypeId > 0);
            return models.Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.OfficerTypeId
            }).ToList();

        }

        public async Task<List<SelectModel>> GetEmployeeStatusSelectModels()
        {
            ICollection<EmployeeStatus> models = await employeeStatusRepository.FilterAsync(x => x.EmployeeStatusId > 0);
            return models.Select(x => new SelectModel()
            {
                Text = x.StatusTitle,
                Value = x.EmployeeStatusId
            }).ToList();

        }

        public List<EmployeeModel> GetEmployeesByDollarSign(int ps, int pn, string qs, out int total)
        {
            IQueryable<Employee> employees = employeeRepository.FilterWithInclude(x => x.Active && x.HasDollarSign && (x.PNo.Contains(qs) || String.IsNullOrEmpty(qs)), "Batch", "Gender", "Rank");
            total = employees.Count();
            employees = employees.OrderBy(x => x.PNo).Skip((pn - 1) * ps).Take(ps);
            List<EmployeeModel> models = ObjectConverter<Employee, EmployeeModel>.ConvertList(employees.ToList()).ToList();
            return models;
        }

        public async Task<EmployeeModel> UpdateEmployeeDollarSign(EmployeeModel model)
        {
            bool isExist = await employeeRepository.ExistsAsync(x => x.Active && x.EmployeeId == model.EmployeeId);

            if (!isExist)
            {
                throw new InfinityInvalidDataException("Invalid Office PNo!");
            }
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Officer's data missing!");
            }
            Employee employee = ObjectConverter<EmployeeModel, Employee>.Convert(model);
            employee = await employeeRepository.FindOneAsync(x => x.EmployeeId == model.EmployeeId);
            if (employee == null)
            {
                throw new InfinityNotFoundException("Officer not found!");
            }

            if (model.DateOfDollarSign == null)
            {
                throw new InfinityNotFoundException("Date is Required.");
            }

            employee.ModifiedDate = DateTime.Now;
            employee.ModifiedBy = model.ModifiedBy;
            employee.HasDollarSign = model.HasDollarSign;
            employee.Reason = model.Reason;
            employee.DateOfDollarSign = model.DateOfDollarSign;
            await employeeRepository.SaveAsync(employee);
            model.EmployeeId = employee.EmployeeId;
            return model;
        }

        public async Task<bool> DeleteEmployeeDollarSign(int employeeId)
        {
            bool isExist = await employeeRepository.ExistsAsync(x => x.Active && x.EmployeeId == employeeId);

            if (!isExist)
            {
                throw new InfinityInvalidDataException("Invalid Office PNo!");
            }

            Employee employee = await employeeRepository.FindOneAsync(x => x.EmployeeId == employeeId);
            if (employee == null)
            {
                throw new InfinityNotFoundException("Officer not found!");
            }
            employee.ModifiedDate = DateTime.Now;
            employee.ModifiedBy = ConfigurationResolver.Get().LoggedInUser.UserId;
            employee.HasDollarSign = false;
            var rows = await employeeRepository.SaveAsync(employee);
            return rows > 0;
        }

        public async Task<bool> ExecuteSeniorityProcess()
        {
            employeeRepository.ExecWithSqlQuery(String.Format("exec spUpdateSeniorityPosition"));
            return true;
        }

        public object GetDynamicTableInfoById(string tableName, string columnName, int id)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDynamicRowInfoById] '{0}','{1}',{2} ", tableName, columnName, id));
            return dataTable.ToJson().FirstOrDefault();
        }
        private void GetAgeLimitServiceLimitLprDate(int EmployeeId, int category, int subCategory, int rank, DateTime commissionDate, DateTime dateOfBirth, out DateTime ageLimit, out DateTime serviceLimit, out DateTime LprDate)
        {
            AgeServicePolicy ageServicePolicy = ageServicePolicyRepository.FindOne(x => x.CategoryId == category && x.SubCategoryId == subCategory && x.RankId == rank);
            if (ageServicePolicy == null)
            {
                throw new InfinityNotFoundException("Age Service Policy not Set for EmployeeId=" + EmployeeId.ToString() + "!");
            }
            ageLimit = dateOfBirth.AddYears(ageServicePolicy.AgeLimit);
            serviceLimit = commissionDate.AddYears(ageServicePolicy.ServiceLimit);
            LprDate = ageLimit > serviceLimit ? ageLimit : serviceLimit;

            ageLimit = ageLimit.AddDays(-1);
            serviceLimit = serviceLimit.AddDays(-1);
        }
        public async Task<string> UpdateAgeServicePolicy()
        {
            int count = 0;

            IQueryable<EmployeeGeneral> employeeGenerals = employeeGeneralRepository.FilterWithInclude(x => x.Employee.RankCategoryId == 3, "Employee.RankCategory");
            foreach (var item in employeeGenerals.ToList())
            {
                EmployeeGeneral employeeGeneral = await employeeGeneralRepository.FindOneAsync(x => x.EmployeeGeneralId == item.EmployeeGeneralId);

                GetAgeLimitServiceLimitLprDate(employeeGeneral.EmployeeId, item.CategoryId, item.SubCategoryId ?? 0, item.Employee.RankId, item.CommissionDate ?? DateTime.Today, item.DoB, out DateTime ageLimit, out DateTime serviceLimit, out DateTime LprDate);
                employeeGeneral.AgeLimit = ageLimit;
                employeeGeneral.ServiceLimit = serviceLimit;
                employeeGeneral.LprDate = LprDate;
                await employeeGeneralRepository.SaveAsync(employeeGeneral);
                count = count + 1;
            }
            return "Age Service Policy is updated for " + count.ToString() + " Officers";
        }



        public bool ExecuteNamingConvention()
        {
            employeeRepository.ExecWithSqlQuery(String.Format("exec [SpBuildNamingConvention] -1"));
            return true;
        }

        public List<object> GetEmployeeByName(string qs)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetByName] '{0}' ", qs));

            return dataTable.ToJson().ToList();
        }
    }
}
