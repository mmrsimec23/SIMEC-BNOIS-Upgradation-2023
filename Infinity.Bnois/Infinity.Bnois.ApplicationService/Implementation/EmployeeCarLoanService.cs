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
    public class EmployeeCarLoanService : IEmployeeCarLoanService
    {
        private readonly IBnoisRepository<EmployeeCarLoan> _employeeCarLoanRepository;
        private readonly IBnoisRepository<Employee> _employeeRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;

        public EmployeeCarLoanService(
            IBnoisRepository<EmployeeCarLoan> employeeCarLoanRepository,
            IBnoisRepository<Employee> employeeRepository,
            IBnoisRepository<BnoisLog> bnoisLogRepository,
            IEmployeeService employeeService
            )
        {
            _employeeCarLoanRepository = employeeCarLoanRepository;
            _employeeRepository = employeeRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public async Task<bool> DeleteEmployeeCarLoan(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            EmployeeCarLoan employeeCarLoan = await _employeeCarLoanRepository.FindOneAsync(x => x.EmployeeCarLoanId == id);
            if (employeeCarLoan == null)
            {
                throw new InfinityNotFoundException("Employee Car Loan Info not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeCarLoan";
                bnLog.TableEntryForm = "Officer Car Loan";
                bnLog.PreviousValue = "Id: " + employeeCarLoan.EmployeeCarLoanId;
                if (employeeCarLoan.EmployeeId > 0)
                {
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeCarLoan.EmployeeId ?? 0);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)emp).PNo;
                }
                bnLog.PreviousValue += ", BackLog: " + employeeCarLoan.IsBackLog;
                if (employeeCarLoan.RankId > 0)
                {
                    var rnk = employeeService.GetDynamicTableInfoById("Rank", "RankId", employeeCarLoan.RankId ?? 0);
                    bnLog.PreviousValue += ", Rank: " + ((dynamic)rnk).ShortName;
                }
                if (employeeCarLoan.CarLoanFiscalYearId > 0)
                {
                    var fiscalYr = employeeService.GetDynamicTableInfoById("CarLoanFiscalYear", "CarLoanFiscalYearId", employeeCarLoan.CarLoanFiscalYearId ?? 0);
                    bnLog.PreviousValue += ", CarLoanFiscalYear: " + ((dynamic)fiscalYr).Name;
                }
                
                bnLog.PreviousValue += ", Remarks: " + employeeCarLoan.Remarks + ", Status: " + employeeCarLoan.Status + ", Avail Date: " + employeeCarLoan.AvailDate?.ToString("dd/MM/yyyy") + ", Amount: " + employeeCarLoan.Amount;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await _employeeCarLoanRepository.DeleteAsync(employeeCarLoan);
            }
        }

        public async Task<EmployeeCarLoanModel> getEmployeeCarLoan(int id)
        {
            if (id <= 0)
            {
                return new EmployeeCarLoanModel();
            }
            EmployeeCarLoan employeeCarLoan = await _employeeCarLoanRepository.FindOneAsync(x => x.EmployeeCarLoanId == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch" });
            if (employeeCarLoan == null)
            {
                throw new InfinityNotFoundException("EmployeeCar Loan Info not found");
            }
            EmployeeCarLoanModel model = ObjectConverter<EmployeeCarLoan, EmployeeCarLoanModel>.Convert(employeeCarLoan);
            return model;
        }

        public List<EmployeeCarLoanModel> GetEmployeeCarLoanList(int ps, int pn, string qs, out int total)
        {
            IQueryable<EmployeeCarLoan> employeeCarLoans = _employeeCarLoanRepository.FilterWithInclude(x => x.Active && (x.Employee.PNo.Contains(qs) || string.IsNullOrEmpty(qs)), "Employee", "CarLoanFiscalYear","Rank");
            total = _employeeCarLoanRepository.Count();
            employeeCarLoans = employeeCarLoans.OrderByDescending(x => x.EmployeeCarLoanId).Skip((pn - 1) * ps).Take(ps);
            List<EmployeeCarLoanModel> models = ObjectConverter<EmployeeCarLoan, EmployeeCarLoanModel>.ConvertList(employeeCarLoans.ToList()).ToList();
            return models;
        }

        public List<EmployeeCarLoanModel> GetEmployeeCarLoansByPno(string PNo)
        {
            int employeeId = _employeeRepository.Where(x => x.PNo == PNo).Select(x => x.EmployeeId).SingleOrDefault();
            List<EmployeeCarLoan> employeeCarLoans = _employeeCarLoanRepository.Where(x => x.EmployeeId == employeeId).ToList();
            List<EmployeeCarLoanModel> models = ObjectConverter<EmployeeCarLoan, EmployeeCarLoanModel>.ConvertList(employeeCarLoans.ToList()).ToList();
            return models;
        }

        public async Task<EmployeeCarLoanModel> SaveEmployeeCarLoan(int id, EmployeeCarLoanModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Employee Car Loan data missing");
            }
            bool isExistData = _employeeCarLoanRepository.Exists(x => x.EmployeeId == model.EmployeeId  && x.EmployeeCarLoanId != id);
            if (isExistData)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            EmployeeCarLoan employeeCarLoan = ObjectConverter<EmployeeCarLoanModel, EmployeeCarLoan>.Convert(model);
            if (id > 0)
            {
                employeeCarLoan = _employeeCarLoanRepository.FindOne(x => x.EmployeeCarLoanId == id);
                if (employeeCarLoan == null)
                {
                    throw new InfinityNotFoundException("Employee Car Loan not found !");
                }

                employeeCarLoan.Modified = DateTime.Now;
                employeeCarLoan.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeCarLoan";
                bnLog.TableEntryForm = "Officer Car Loan";
                bnLog.PreviousValue = "Id: " + model.EmployeeCarLoanId;
                bnLog.UpdatedValue = "Id: " + model.EmployeeCarLoanId;
                if (employeeCarLoan.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeCarLoan.EmployeeId??0);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId??0);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)emp).PNo;
                }
                if (employeeCarLoan.IsBackLog != model.IsBackLog)
                {
                    bnLog.PreviousValue += ", BackLog: " + employeeCarLoan.IsBackLog;
                    bnLog.UpdatedValue += ", BackLog: " + model.IsBackLog;
                }
                if (employeeCarLoan.RankId != model.RankId)
                {
                    if(employeeCarLoan.RankId > 0)
                    {
                        var prevrank = employeeService.GetDynamicTableInfoById("Rank", "RankId", employeeCarLoan.RankId ?? 0);
                        bnLog.PreviousValue += ", Session: " + ((dynamic)prevrank).ShortName;
                    }
                    if (model.RankId > 0)
                    {
                        var rank = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.RankId ?? 0);
                        bnLog.UpdatedValue += ", Session: " + ((dynamic)rank).ShortName;
                    }
                }
                if (employeeCarLoan.Status != model.Status)
                {
                    bnLog.PreviousValue += ", Status: " + employeeCarLoan.Status;
                    bnLog.UpdatedValue += ", Status: " + model.Status;
                }
                if (employeeCarLoan.CarLoanFiscalYearId != model.CarLoanFiscalYearId)
                {
                    if (employeeCarLoan.CarLoanFiscalYearId > 0)
                    {
                        var prevcarLone = employeeService.GetDynamicTableInfoById("CarLoanFiscalYear", "CarLoanFiscalYearId", employeeCarLoan.CarLoanFiscalYearId ?? 0);
                        bnLog.PreviousValue += ", CarLoanFiscalYear: " + ((dynamic)prevcarLone).Name;
                    }
                    if (model.CarLoanFiscalYearId > 0)
                    {
                        var carLone = employeeService.GetDynamicTableInfoById("CarLoanFiscalYear", "CarLoanFiscalYearId", model.CarLoanFiscalYearId ?? 0);
                        bnLog.UpdatedValue += ", CarLoanFiscalYear: " + ((dynamic)carLone).Name;
                    }
                }
                if (employeeCarLoan.AvailDate != model.AvailDate)
                {
                    bnLog.PreviousValue += ", AvailDate: " + employeeCarLoan.AvailDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", AvailDate: " + model.AvailDate?.ToString("dd/MM/yyyy");
                }
                if (employeeCarLoan.Amount != model.Amount)
                {
                    bnLog.PreviousValue += ", Amount: " + employeeCarLoan.Amount;
                    bnLog.UpdatedValue += ", Amount: " + model.Amount;
                }
                if (employeeCarLoan.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + employeeCarLoan.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (employeeCarLoan.EmployeeId != model.EmployeeId || employeeCarLoan.IsBackLog != model.IsBackLog || employeeCarLoan.Remarks != model.Remarks 
                    || employeeCarLoan.RankId != model.Employee.RankId || employeeCarLoan.Status != model.Status || employeeCarLoan.CarLoanFiscalYearId != model.CarLoanFiscalYearId
                    || employeeCarLoan.AvailDate != model.AvailDate || employeeCarLoan.Amount != model.Amount)
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
                employeeCarLoan.Active = true;
                employeeCarLoan.Created = DateTime.Now;
                employeeCarLoan.CreatedBy = userId;
            }
            employeeCarLoan.EmployeeId = model.EmployeeId;
            employeeCarLoan.Employee = null;
            employeeCarLoan.IsBackLog = model.IsBackLog;
            employeeCarLoan.RankId = model.Employee.RankId;
            employeeCarLoan.Status = model.Status;
            employeeCarLoan.CarLoanFiscalYearId = model.CarLoanFiscalYearId;
            employeeCarLoan.AvailDate = model.AvailDate;
            employeeCarLoan.Amount = model.Amount;
            employeeCarLoan.Remarks = model.Remarks;

            if (model.IsBackLog)
            {
                employeeCarLoan.RankId = model.RankId;
            }
            employeeCarLoan.Employee = null;
            employeeCarLoan.CarLoanFiscalYear = null;
            employeeCarLoan.Rank = null;

            try
            {
                await _employeeCarLoanRepository.SaveAsync(employeeCarLoan);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            return model;
        }
    }
}
