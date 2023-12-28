using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.Configuration;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class EmployeeLprService : IEmployeeLprService
    {
        private readonly IBnoisRepository<EmployeeLpr> employeeLprRepository;
        private readonly IBnoisRepository<Employee> employeeRepository;
        private readonly IBnoisRepository<EmployeeGeneral> employeeGeneralRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;

        public EmployeeLprService(IBnoisRepository<EmployeeLpr> employeeLprRepository,
            IBnoisRepository<EmployeeGeneral> employeeGeneralRepository,
            IBnoisRepository<Employee> employeeRepository,
            IBnoisRepository<BnoisLog> bnoisLogRepository, 
            IEmployeeService employeeService)
        {
            this.employeeLprRepository = employeeLprRepository;
            this.employeeGeneralRepository = employeeGeneralRepository;
            this.employeeRepository = employeeRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public async Task<bool> DeleteEmployeeLpr(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            EmployeeLpr employeeLpr = await employeeLprRepository.FindOneAsync(x => x.EmpLprId == id);
            if (employeeLpr == null)
            {
                throw new InfinityNotFoundException("LPR/Retirement/Termination not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeLpr";
                bnLog.TableEntryForm = "Employee LPR";
                bnLog.PreviousValue = "Id: " + employeeLpr.EmpLprId;
                if (employeeLpr.EmployeeId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeLpr.EmployeeId);
                    bnLog.PreviousValue += ", P No: " + ((dynamic)prev).PNo;
                }
                if (employeeLpr.TerminationTypeId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("TerminationType", "TerminationTypeId", employeeLpr.TerminationTypeId);
                    bnLog.PreviousValue += ", Termination Type: " + ((dynamic)prev).Name;
                }
                
                bnLog.PreviousValue += ", Current Status: " + (employeeLpr.CurrentStatus == 6 ? "LPR" : employeeLpr.CurrentStatus == 2 ? "Retired" : employeeLpr.CurrentStatus == 7 ? "Terminated" : "");
               
                bnLog.PreviousValue += ", Lpr Date: " + employeeLpr.LprDate?.ToString("dd/MM/yyyy") + ", Duration Month: " + employeeLpr.DurationMonth + ", Duration Day: " + employeeLpr.DurationDay + ", Retired Date: " + employeeLpr.RetireDate?.ToString("dd/MM/yyyy") + ", Termination Date: " + employeeLpr.TerminationDate?.ToString("dd/MM/yyyy") + ", R Status: " + employeeLpr.RStatus + ", Remarks: " + employeeLpr.Remarks;
                
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await employeeLprRepository.DeleteAsync(employeeLpr);
            }
        }

        public async Task<EmployeeLprModel> GetEmployeeLpr(int id)
        {
            if (id <= 0)
            {
                return new EmployeeLprModel();
            }
            //EmployeeLpr employeeLpr = await employeeLprRepository.FindOneAsync(x => x.EmpLprId == id, new List<string> { "Employee", "Employee1" });
            EmployeeLpr employeeLpr = await employeeLprRepository.FindOneAsync(x => x.EmpLprId == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch" });
            if (employeeLpr == null)
            {
                throw new InfinityNotFoundException("Employee Lpr not found");
            }
            EmployeeLprModel model = ObjectConverter<EmployeeLpr, EmployeeLprModel>.Convert(employeeLpr);
            return model;
        }



        public async Task<List<SelectModel>> GetRetirementStatusSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(RetiredStatus)).Cast<RetiredStatus>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }

        public async Task<List<SelectModel>> GetDurationStatusSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(DurationStatus)).Cast<DurationStatus>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }

        public List<EmployeeLprModel> GetEmployeeLprs(int ps, int pn, string qs, out int total)
        {
            IQueryable<EmployeeLpr> employeeLprs = employeeLprRepository.FilterWithInclude(x => x.IsActive && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee", "Employee.Rank", "Employee.Batch", "TerminationType");
            total = employeeLprs.Count();
            employeeLprs = employeeLprs.OrderByDescending(x => x.EmpLprId).ThenBy(x=>x.Employee.PNo).Skip((pn - 1) * ps).Take(ps);
            List<EmployeeLprModel> models = ObjectConverter<EmployeeLpr, EmployeeLprModel>.ConvertList(employeeLprs.ToList()).ToList();
            return models;
        }

        public async Task<EmployeeLprModel> SaveEmployeeLpr(int id, EmployeeLprModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("LPR/Retirement/Termination  data missing");
            }

            bool isExist = await employeeLprRepository.ExistsAsync(x => x.EmployeeId == model.Employee.EmployeeId && x.EmpLprId != model.EmpLprId);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Officer already exist !");
            }


            EmployeeGeneral employeeGeneral = await employeeGeneralRepository.FindOneAsync(x => x.EmployeeId == model.Employee.EmployeeId);
            if (employeeGeneral == null)
            {
                throw new InfinityNotFoundException("Officer General Information data not found !");
            }

            Employee employee = await employeeRepository.FindOneAsync(x => x.EmployeeId == model.Employee.EmployeeId);
            if (employeeGeneral == null)
            {
                throw new InfinityNotFoundException("Officer Information data not found !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            EmployeeLpr employeeLpr = ObjectConverter<EmployeeLprModel, EmployeeLpr>.Convert(model);
            if (id > 0)
            {
                employeeLpr = await employeeLprRepository.FindOneAsync(x => x.EmpLprId == id);
                if (employeeLpr == null)
                {
                    throw new InfinityNotFoundException("LPR/Retirement/Termination  not found !");
                }
                employeeLpr.ModifiedDate = DateTime.Now;
                employeeLpr.ModifiedBy = userId;



                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeLpr";
                bnLog.TableEntryForm = "Employee LPR";
                bnLog.PreviousValue = "Id: " + model.EmpLprId;
                bnLog.UpdatedValue = "Id: " + model.EmpLprId;
                int bnoisUpdateCount = 0;

                if (employeeLpr.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    if (employeeLpr.EmployeeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeLpr.EmployeeId);
                        bnLog.PreviousValue += ", P No: " + ((dynamic)prev).PNo;
                    }
                    if (model.EmployeeId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                        bnLog.UpdatedValue += ", P No: " + ((dynamic)newv).PNo;
                    }
                }
                if (employeeLpr.CurrentStatus != model.CurrentStatus)
                {
                    bnLog.PreviousValue += ", Current Status: " + (employeeLpr.CurrentStatus == 6 ? "LPR" : employeeLpr.CurrentStatus == 2 ? "Retired" : employeeLpr.CurrentStatus == 7 ? "Terminated" :"");
                    bnLog.UpdatedValue += ", Current Status: " + (model.CurrentStatus == 6 ? "LPR" : model.CurrentStatus == 2 ? "Retired" : model.CurrentStatus == 7 ? "Terminated" : "");
                    bnoisUpdateCount += 1;
                }
                if (employeeLpr.TerminationTypeId != model.TerminationTypeId)
                {
                    if (employeeLpr.TerminationTypeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("TerminationType", "TerminationTypeId", employeeLpr.TerminationTypeId);
                        bnLog.PreviousValue += ", Termination Type: " + ((dynamic)prev).Name;
                    }
                    if (model.TerminationTypeId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("TerminationType", "TerminationTypeId", model.TerminationTypeId);
                        bnLog.UpdatedValue += ", Termination Type: " + ((dynamic)newv).Name;
                    }
                }
                if (employeeLpr.LprDate != model.LprDate)
                {
                    bnLog.PreviousValue += ", Lpr Date: " + employeeLpr.LprDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Lpr Date: " + model.LprDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeLpr.DurationMonth != model.DurationMonth)
                {
                    bnLog.PreviousValue += ", Duration Month: " + employeeLpr.DurationMonth;
                    bnLog.UpdatedValue += ", Duration Month: " + model.DurationMonth;
                    bnoisUpdateCount += 1;
                }
                if (employeeLpr.DurationDay != model.DurationDay)
                {
                    bnLog.PreviousValue += ", Duration Day: " + employeeLpr.DurationDay;
                    bnLog.UpdatedValue += ", Duration Day: " + model.DurationDay;
                    bnoisUpdateCount += 1;
                }
                if (employeeLpr.RetireDate != model.RetireDate)
                {
                    bnLog.PreviousValue += ", Retired Date: " + employeeLpr.RetireDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Retired Date: " + model.RetireDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeLpr.TerminationDate != model.TerminationDate)
                {
                    bnLog.PreviousValue += ", Termination Date: " + employeeLpr.TerminationDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Termination Date: " + model.TerminationDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeLpr.RStatus != model.RStatus)
                {
                    bnLog.PreviousValue += ", R Status: " + employeeLpr.RStatus;
                    bnLog.UpdatedValue += ", R Status: " + model.RStatus;
                    bnoisUpdateCount += 1;
                }
                if (employeeLpr.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + employeeLpr.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                    bnoisUpdateCount += 1;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString(); ;
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
                employeeLpr.IsActive = true;
                employeeLpr.CreatedDate = DateTime.Now;
                employeeLpr.CreatedBy = userId;
            }
            employeeLpr.EmployeeId = model.Employee.EmployeeId;
            employeeLpr.TerminationTypeId = model.TerminationTypeId;
            employeeLpr.CurrentStatus = model.CurrentStatus;
            employeeLpr.LprDate = model.LprDate;
            employeeLpr.DurationMonth = model.DurationMonth;
            employeeLpr.DurationDay = model.DurationDay;
            employeeLpr.RetireDate = model.RetireDate;
            employeeLpr.TerminationDate = model.TerminationDate;
            employeeLpr.RStatus = model.RStatus;
            employeeLpr.Remarks = model.Remarks;
            employeeLpr.Employee = null;
            await employeeLprRepository.SaveAsync(employeeLpr);
            model.EmpLprId = employeeLpr.EmpLprId;
            #region---Update Employee and Employee General Update---
            employeeGeneral.LprDate = employeeLpr.LprDate;
            if (employeeGeneral.LprDate <= DateTime.Today)
            {
                employee.EmployeeStatusId = (int)OfficerCurrentStatus.LPR;
                employee.SLCode = (int)OfficerCurrentStatus.LPR;
            }
            employeeGeneral.RetireDate = employeeLpr.RetireDate;
            if (employeeGeneral.RetireDate <= DateTime.Today)
            {
                employee.EmployeeStatusId = (int)OfficerCurrentStatus.Retired;
                employee.SLCode = (int)OfficerCurrentStatus.Retired;
            }
            employeeGeneral.TerminationDate = employeeLpr.TerminationDate;
            if (employeeGeneral.TerminationDate <= DateTime.Today)
            {
                employee.EmployeeStatusId = (int)OfficerCurrentStatus.Terminated;
                employee.SLCode = (int)OfficerCurrentStatus.Terminated;
            }
            await employeeRepository.SaveAsync(employee);
            await employeeGeneralRepository.SaveAsync(employeeGeneral);
            #endregion
            return model;
        }
      
    }
}
