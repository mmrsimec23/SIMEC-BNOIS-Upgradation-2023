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
    public class EmployeeTraceService : IEmployeeTraceService
    {
        private readonly IBnoisRepository<DashBoardTrace990> _employeeTraceRepository;
        private readonly IBnoisRepository<Employee> _employeeRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;

        public EmployeeTraceService(
            IBnoisRepository<DashBoardTrace990> employeeTraceRepository,
            IBnoisRepository<Employee> employeeRepository,
            IBnoisRepository<BnoisLog> bnoisLogRepository,
            IEmployeeService employeeService
            )
        {
            _employeeTraceRepository = employeeTraceRepository;
            _employeeRepository = employeeRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public async Task<bool> DeleteEmployeeTrace(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            DashBoardTrace990 employeeTrace = await _employeeTraceRepository.FindOneAsync(x => x.Id == id);
            if (employeeTrace == null)
            {
                throw new InfinityNotFoundException("Employee Car Loan Info not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeTrace";
                bnLog.TableEntryForm = "Officer Car Loan";
                bnLog.PreviousValue = "Id: " + employeeTrace.Id;
                if (employeeTrace.EmployeeId > 0)
                {
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeTrace.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)emp).PNo;
                }
                if (employeeTrace.RankId > 0)
                {
                    var rnk = employeeService.GetDynamicTableInfoById("Rank", "RankId", employeeTrace.RankId);
                    bnLog.PreviousValue += ", Rank: " + ((dynamic)rnk).ShortName;
                }
                //if (employeeTrace.TraceFiscalYearId > 0)
                //{
                //    var fiscalYr = employeeService.GetDynamicTableInfoById("TraceFiscalYear", "TraceFiscalYearId", employeeTrace.TraceFiscalYearId ?? 0);
                //    bnLog.PreviousValue += ", TraceFiscalYear: " + ((dynamic)fiscalYr).Name;
                //}
                
                bnLog.PreviousValue += ", Trace Mark: " + employeeTrace.TraceMark;
                bnLog.PreviousValue += ", Remarks: " + employeeTrace.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await _employeeTraceRepository.DeleteAsync(employeeTrace);
            }
        }

        public async Task<EmployeeTraceModel> getEmployeeTrace(int id)
        {
            if (id <= 0)
            {
                return new EmployeeTraceModel();
            }
            DashBoardTrace990 employeeTrace = await _employeeTraceRepository.FindOneAsync(x => x.Id == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch" });
            if (employeeTrace == null)
            {
                throw new InfinityNotFoundException("EmployeeCar Loan Info not found");
            }
            EmployeeTraceModel model = ObjectConverter<DashBoardTrace990, EmployeeTraceModel>.Convert(employeeTrace);
            return model;
        }

        public List<EmployeeTraceModel> GetEmployeeTraceList(int ps, int pn, string qs, out int total)
        {
            IQueryable<DashBoardTrace990> employeeTraces = _employeeTraceRepository.FilterWithInclude(x => (x.Employee.PNo.Contains(qs) || string.IsNullOrEmpty(qs)), "Employee","Rank");
            total = _employeeTraceRepository.Count();
            employeeTraces = employeeTraces.OrderByDescending(x => x.Id).Skip((pn - 1) * ps).Take(ps);
            List<EmployeeTraceModel> models = ObjectConverter<DashBoardTrace990, EmployeeTraceModel>.ConvertList(employeeTraces.ToList()).ToList();
            return models;
        }

        public List<EmployeeTraceModel> GetEmployeeTracesByPno(string PNo)
        {
            int employeeId = _employeeRepository.Where(x => x.PNo == PNo).Select(x => x.EmployeeId).SingleOrDefault();
            List<DashBoardTrace990> employeeTraces = _employeeTraceRepository.Where(x => x.EmployeeId == employeeId).ToList();
            List<EmployeeTraceModel> models = ObjectConverter<DashBoardTrace990, EmployeeTraceModel>.ConvertList(employeeTraces.ToList()).ToList();
            return models;
        }

        public async Task<EmployeeTraceModel> SaveEmployeeTrace(int id, EmployeeTraceModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Employee Car Loan data missing");
            }
            bool isExistData = _employeeTraceRepository.Exists(x => x.EmployeeId == model.EmployeeId  && x.Id != id);
            if (isExistData)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            DashBoardTrace990 employeeTrace = ObjectConverter<EmployeeTraceModel, DashBoardTrace990>.Convert(model);
            if (id > 0)
            {
                employeeTrace = _employeeTraceRepository.FindOne(x => x.Id == id);
                if (employeeTrace == null)
                {
                    throw new InfinityNotFoundException("Employee Car Loan not found !");
                }

                employeeTrace.ModifiedDate = DateTime.Now;
                employeeTrace.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeTrace";
                bnLog.TableEntryForm = "Officer Car Loan";
                bnLog.PreviousValue = "Id: " + model.Id;
                bnLog.UpdatedValue = "Id: " + model.Id;
                if (employeeTrace.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeTrace.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)emp).PNo;
                }
                //if (employeeTrace.IsBackLog != model.IsBackLog)
                //{
                //    bnLog.PreviousValue += ", BackLog: " + employeeTrace.IsBackLog;
                //    bnLog.UpdatedValue += ", BackLog: " + model.IsBackLog;
                //}
                if (employeeTrace.RankId != model.RankId)
                {
                    if(employeeTrace.RankId > 0)
                    {
                        var prevrank = employeeService.GetDynamicTableInfoById("Rank", "RankId", employeeTrace.RankId);
                        bnLog.PreviousValue += ", Session: " + ((dynamic)prevrank).ShortName;
                    }
                    if (model.RankId > 0)
                    {
                        var rank = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.RankId);
                        bnLog.UpdatedValue += ", Session: " + ((dynamic)rank).ShortName;
                    }
                }
                if (employeeTrace.TraceMark != model.TraceMark)
                {
                    bnLog.PreviousValue += ", Status: " + employeeTrace.TraceMark;
                    bnLog.UpdatedValue += ", Status: " + model.TraceMark;
                }
                //if (employeeTrace.TraceFiscalYearId != model.TraceFiscalYearId)
                //{
                //    if (employeeTrace.TraceFiscalYearId > 0)
                //    {
                //        var prevcarLone = employeeService.GetDynamicTableInfoById("TraceFiscalYear", "TraceFiscalYearId", employeeTrace.TraceFiscalYearId ?? 0);
                //        bnLog.PreviousValue += ", TraceFiscalYear: " + ((dynamic)prevcarLone).Name;
                //    }
                //    if (model.TraceFiscalYearId > 0)
                //    {
                //        var carLone = employeeService.GetDynamicTableInfoById("TraceFiscalYear", "TraceFiscalYearId", model.TraceFiscalYearId ?? 0);
                //        bnLog.UpdatedValue += ", TraceFiscalYear: " + ((dynamic)carLone).Name;
                //    }
                //}
                //if (employeeTrace.AvailDate != model.AvailDate)
                //{
                //    bnLog.PreviousValue += ", AvailDate: " + employeeTrace.AvailDate?.ToString("dd/MM/yyyy");
                //    bnLog.UpdatedValue += ", AvailDate: " + model.AvailDate?.ToString("dd/MM/yyyy");
                //}
                //if (employeeTrace.Amount != model.Amount)
                //{
                //    bnLog.PreviousValue += ", Amount: " + employeeTrace.Amount;
                //    bnLog.UpdatedValue += ", Amount: " + model.Amount;
                //}
                if (employeeTrace.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + employeeTrace.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (employeeTrace.EmployeeId != model.EmployeeId || employeeTrace.Remarks != model.Remarks 
                    || employeeTrace.RankId != model.Employee.RankId || employeeTrace.TraceMark != model.TraceMark )
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
                employeeTrace.IsActive = true;
                employeeTrace.CreatedDate = DateTime.Now;
                employeeTrace.CreatedBy = userId;
            }
            employeeTrace.EmployeeId = model.EmployeeId;
            //employeeTrace.Employee = null;
            //employeeTrace.IsBackLog = model.IsBackLog;
            employeeTrace.RankId = model.RankId;
            employeeTrace.TraceMark = model.TraceMark;
            //employeeTrace.TraceFiscalYearId = model.TraceFiscalYearId;
            //employeeTrace.AvailDate = model.AvailDate;
            //employeeTrace.Amount = model.Amount;
            employeeTrace.Remarks = model.Remarks;
            //employeeTrace.RankId = model.RankId;
            employeeTrace.Employee = null;
            //employeeTrace.TraceFiscalYear = null;
            employeeTrace.Rank = null;

            try
            {
                await _employeeTraceRepository.SaveAsync(employeeTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            return model;
        }
    }
}
