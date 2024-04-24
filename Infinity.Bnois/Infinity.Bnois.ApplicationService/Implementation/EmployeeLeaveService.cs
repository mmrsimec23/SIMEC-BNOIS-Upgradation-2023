using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.Api;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Configuration.Models;
using Infinity.Bnois.Data;
using Infinity.Bnois.Data.Models;
using Infinity.Bnois.ExceptionHelper;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class EmployeeLeaveService : IEmployeeLeaveService
    {
        private readonly IEmployeeLeaveRepository employeeLeaveRepository;
        private readonly IBnoisRepository<EmployeeGeneral> employeeGeneralRepository;
        private readonly IBnoisRepository<Employee> employeeRepository;
        private readonly IBnoisRepository<EmployeeLeaveCountry> employeeLeaveCountryRepository;
        private readonly IBnoisRepository<EmployeeLeaveYear> employeeLeaveYearRepository;
        private readonly IBnoisRepository<LeavePolicy> leavePolicyRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public EmployeeLeaveService(IBnoisRepository<Employee> employeeRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService, IEmployeeLeaveRepository employeeLeaveRepository, IBnoisRepository<EmployeeLeaveCountry> employeeLeaveCountryRepository, IBnoisRepository<EmployeeLeaveYear> employeeLeaveYearRepository, IBnoisRepository<EmployeeGeneral> employeeGeneralRepository, IBnoisRepository<LeavePolicy> leavePolicyRepository)
        {
            this.employeeLeaveRepository = employeeLeaveRepository;
            this.employeeLeaveCountryRepository = employeeLeaveCountryRepository;
            this.employeeLeaveYearRepository = employeeLeaveYearRepository;
            this.employeeGeneralRepository = employeeGeneralRepository;
            this.leavePolicyRepository = leavePolicyRepository;
            this.employeeRepository = employeeRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;

        }

        public async Task<bool> DeleteEmployeeLeave(int id)
        {
            if (id <= 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            EmployeeLeave employeeLeave = await employeeLeaveRepository.FindOneAsync(x => x.EmpLeaveId == id);
            List<EmployeeLeaveYear> employeeLeaveYear = employeeLeaveYearRepository.Where(x => x.EmpLeaveId == employeeLeave.EmpLeaveId).ToList();
            List<EmployeeLeaveCountry> employeeLeaveCountry = employeeLeaveCountryRepository.Where(x => x.EmpLeaveId == employeeLeave.EmpLeaveId).ToList();

            if (employeeLeave == null)
            {
                throw new InfinityNotFoundException("Leave Info not found!");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeLeave";
                bnLog.TableEntryForm = "Employee Leave";
                bnLog.PreviousValue = "Id: " + employeeLeave.EmpLeaveId;
                if (employeeLeave.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeLeave.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                }
                if (employeeLeave.LeaveTypeId  > 0)
                {
                     var prev = employeeService.GetDynamicTableInfoById("LeaveType", "LeaveTypeId", employeeLeave.LeaveTypeId);
                     bnLog.PreviousValue += ", Leave Type: " + ((dynamic)prev).TypeName;
                }
                bnLog.PreviousValue += ", Date From: " + employeeLeave.FromDate?.ToString("dd/MM/yyyy") + ", Date To: " + employeeLeave.ToDate?.ToString("dd/MM/yyyy") + ", Duration: " + employeeLeave.Duration + ", Remarks: " + employeeLeave.Remarks + ", Ex Bd Leave: " + employeeLeave.ExBdLeave + ", Accompany By: " + employeeLeave.AccompanyBy;
                if (employeeLeave.Purpose > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("LeavePurpose", "PurposeId", employeeLeave.Purpose ?? 0);
                    bnLog.PreviousValue += ", Purpose: " + ((dynamic)prev).Name;
                }

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                employeeLeaveYearRepository.RemoveRange(employeeLeaveYear);
                employeeLeaveCountryRepository.RemoveRange(employeeLeaveCountry);
                return await employeeLeaveRepository.DeleteAsync(employeeLeave);
            }
        }

        public async Task<EmployeeLeaveModel> GetEmployeeLeave(int id)
        {
            if (id <= 0)
            {

                return new EmployeeLeaveModel() { LeaveBalances = new List<EmployeeLeaveBalance>() };
            }

            EmployeeLeave employeeLeave = await employeeLeaveRepository.FindOneAsync(x => x.EmpLeaveId == id, new List<string> { "Employee", "Employee.EmployeeGeneral", "Employee.Rank", "Employee.Batch", "EmployeeLeaveYears" });

            if (employeeLeave == null)
            {
                throw new InfinityNotFoundException("Employee Leave not found");
            }

            EmployeeLeaveModel model = ObjectConverter<EmployeeLeave, EmployeeLeaveModel>.Convert(employeeLeave);
            if (model.LeaveTypeId == CodeValue.PL_LeaveType)
            {
                model.LeaveBalances = await GetEmployeeLeaveBalanceBySlot(employeeLeave.EmployeeId, model);
            }
            model.CountryIds = employeeLeaveCountryRepository.Where(x => x.EmpLeaveId == employeeLeave.EmpLeaveId).Select(x => x.CountryId).ToArray();
            //var employeeLeaveInfo = employeeLeaveRepository.Where(x =>x.EmployeeId == employeeLeave.EmployeeId && x.LeaveTypeId == employeeLeave.LeaveTypeId && x.EmpLeaveId != employeeLeave.EmpLeaveId).ToList();
            string due = await GetEmployeeLeaveDue(model.EmployeeId, model.LeaveTypeId);
            model.LeaveDueCount = (Convert.ToInt16(due) + Convert.ToInt16(model.Duration)).ToString();

            return model;
        }

        private async Task<List<EmployeeLeaveBalance>> GetEmployeeLeaveBalanceBySlot(int employeeId, EmployeeLeaveModel employeeLeave)
        {
            List<EmployeeLeaveBalance> totalLeaveBalance = await employeeLeaveRepository.GetEmployeeRepository(employeeId, employeeLeave.LeaveTypeId);
            List<EmployeeLeaveBalance> leaveBalancesForEdit = totalLeaveBalance.Where(x => x.LeaveYear >= employeeLeave.EmployeeLeaveYears.Min(y => y.YearText)).ToList();
            int slot = totalLeaveBalance.Select(x => x.Slot).FirstOrDefault();
            List<EmployeeLeaveBalance> filterLeaveBalance = leaveBalancesForEdit.OrderBy(x => x.LeaveYear).Take(slot).ToList();

            var yearr = from leaveYear in employeeLeave.EmployeeLeaveYears
                        group leaveYear by leaveYear.YearText into yearDetails
                        select new
                        {
                            Year = yearDetails.Key,
                            dayCount = yearDetails.Count()
                        };

            foreach (var year in yearr)
            {
                foreach (var leaveBalance in filterLeaveBalance)
                {
                    if (leaveBalance.LeaveYear.ToString().Contains(year.Year.ToString()))
                    {
                        leaveBalance.IsAssigned = true;
                        leaveBalance.TotalConsume = (int)(leaveBalance.TotalConsume - year.dayCount);
                        leaveBalance.Balance = (int)(leaveBalance.Balance + year.dayCount);

                    }
                }
            }
            return filterLeaveBalance.OrderBy(x => x.LeaveYear).ToList();
        }

        public async Task<List<EmployeeLeaveBalance>> GetEmployeeLeaveBalance(int employeeId, int leaveType)
        {
            if (employeeId <= 0)
            {
                throw new InfinityArgumentMissingException("Invalide Officers Pno!");
            }
            List<EmployeeLeaveBalance> totalLeaveBalance = await employeeLeaveRepository.GetEmployeeRepository(employeeId, leaveType);
            int slot = totalLeaveBalance.Select(x => x.Slot).FirstOrDefault();
            List<EmployeeLeaveBalance> filterLeaveBalance = totalLeaveBalance.OrderByDescending(x => x.LeaveYear).Take(slot).ToList();
            return filterLeaveBalance.OrderBy(x => x.LeaveYear).ToList();
        }
        public async Task<List<SpGetEmployeeLeaveInfoByPNo>> GetEmployeeLeaveDetailsByPNo(string employeePNo)
        {
            var leaveBreakDowns = await employeeLeaveRepository.GetLeaveDetailsByPno(employeePNo);
            List<SpGetEmployeeLeaveInfoByPNo> leaveInfo = leaveBreakDowns.GroupBy(x => x.LeaveTypeId).Select(x => x.First()).Select(x => new SpGetEmployeeLeaveInfoByPNo()
            {
                AccompanyBy = x.AccompanyBy,
                EmpLeaveId = x.EmpLeaveId,
                EmployeeId = x.EmployeeId,
                ShartName = x.ShartName,
                FileName = x.FileName,
                LeaveTypeName = x.LeaveTypeName,
                LeaveTypeId = x.LeaveTypeId,
                FromDate = x.FromDate,
                ToDate = x.ToDate,
                Duration = x.Duration,
                Remarks = x.Remarks,
                Slot = x.Slot,
                Country = x.Country,
                CreatedDate = x.CreatedDate

            }).ToList();

            leaveInfo = leaveInfo.Select(x =>
            {
                int id = Convert.ToInt32(x.LeaveTypeId);
                x.SpGetEmployeeLeaveInfoes = leaveBreakDowns.Where(y => y.LeaveTypeId == id).Select(y => new SpGetEmployeeLeaveInfoByPNo()
                {
                    AccompanyBy = y.AccompanyBy,
                    EmpLeaveId = y.EmpLeaveId,
                    EmployeeId = y.EmployeeId,
                    ShartName = y.ShartName,
                    FileName = y.FileName,
                    LeaveTypeName = y.LeaveTypeName,
                    LeaveTypeId = y.LeaveTypeId,
                    FromDate = y.FromDate,
                    ToDate = y.ToDate,
                    Duration = y.Duration,
                    Remarks = y.Remarks,
                    Slot = y.Slot,
                    Country = y.Country,
                    CreatedDate = y.CreatedDate
                }).ToList();
                return x;
            }).ToList();

            return ObjectConverter<SpGetEmployeeLeaveInfoByPNo, SpGetEmployeeLeaveInfoByPNo>.ConvertList(leaveInfo).OrderBy(x => x.LeaveTypeId).ThenBy(x => x.FromDate).ToList();
        }


        public List<EmployeeLeaveModel> GetEmployeeLeaves(int ps, int pn, string qs, int? leaveType, out int total)
        {
            IQueryable<EmployeeLeave> employeeLeave = null;
            if (qs != null || leaveType > 0)
                employeeLeave = employeeLeaveRepository.FilterWithInclude(x => x.Active && x.LeaveTypeId == leaveType || x.Employee.PNo.Contains(qs), "Employee", "LeaveType");
            else
                employeeLeave = employeeLeaveRepository.FilterWithInclude(x => x.Active && (x.Employee.PNo == qs || String.IsNullOrEmpty(qs)), "Employee", "LeaveType");

            total = employeeLeave.Count();
            employeeLeave = employeeLeave.OrderByDescending(x => x.EmployeeId).Skip((pn - 1) * ps).Take(ps);
            List<EmployeeLeaveModel> models = ObjectConverter<EmployeeLeave, EmployeeLeaveModel>.ConvertList(employeeLeave.ToList()).ToList();
            return models;

        }

        public async Task<List<LeaveBreakDown>> GetLeaveBreakDowns(int emId)
        {
            var leaveBreakDows = await employeeLeaveRepository.GetLeaveBreakDown(emId);
            return leaveBreakDows.OrderBy(x => x.YearText).ToList();
        }

        public async Task<EmployeeLeaveModel> SaveEmployeeLeave(int id, EmployeeLeaveModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Leave data missing");
            }

            model.EmployeeId = model.EmployeeId;
            string LeaveDueCount = model.LeaveDueCount;
            // filter Leave select value

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
       
            EmployeeLeave employeeLeave = ObjectConverter<EmployeeLeaveModel, EmployeeLeave>.Convert(model);
            employeeLeave.Employee = null;
            EmployeeLeave virtualEmloyeeInfo = new EmployeeLeave();

        

            string category = model.Employee.EmployeeGeneral.Select(x => x.CategoryId).FirstOrDefault().ToString();
	   

		    if (category == CodeValue.Deputed_Officer_Type || employeeLeave.LeaveTypeId == CodeValue.Medical_LeaveType)
		    {
				DateTime? DateFrom =employeeLeave.FromDate;
				//DateTime ToDate = Convert.ToDateTime(employeeLeave.ToDate);
				List<EmployeeLeaveYear> EmployeeLeaveYears = new List<EmployeeLeaveYear>();
				for (int j = 0; DateFrom <= employeeLeave.ToDate; j++)
				{

					EmployeeLeaveYear leaveYear = new EmployeeLeaveYear();
					leaveYear.YearText = DateTime.Now.Date.Year;
					leaveYear.LeaveDate = (DateTime)DateFrom;
					EmployeeLeaveYears.Add(leaveYear);
					DateFrom = Convert.ToDateTime(DateFrom).Date.AddDays(1);
				}
				virtualEmloyeeInfo.EmployeeLeaveYears = EmployeeLeaveYears;
			}
		    else
		    {
				virtualEmloyeeInfo.EmployeeLeaveYears = await GetEmployeeLeaveYearAsync(LeaveDueCount, id, model.Employee.GenderId, model.EmployeeId, model.LeaveTypeId, employeeLeave.FromDate, employeeLeave.ToDate, model.LeaveBalances);

	        }

			if (model.EmployeeId <= 0)
            {
                throw new InfinityArgumentMissingException("Invalide Officers Pno!");
            }
            if (id > 0)
            {
                employeeLeave = await employeeLeaveRepository.FindOneAsync(x => x.EmpLeaveId == id);
                if (employeeLeave == null)
                {
                    throw new InfinityNotFoundException("Employee Leave not found !");
                }
                employeeLeave.ModifiedDate = DateTime.Now;
                //employeeLeave.CreatedDate = model.CreatedDate;
                employeeLeave.ModifiedBy = userId;
                // All Employee Leave Country Child Delete 
                await employeeLeaveCountryRepository.DeleteAsync(x => x.EmpLeaveId == id);
                // All Employee Leave Year Child Delete 
                await employeeLeaveYearRepository.DeleteAsync(x => x.EmpLeaveId == id);

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeLeave";
                bnLog.TableEntryForm = "Employee Leave";
                bnLog.PreviousValue = "Id: " + model.EmpLeaveId;
                bnLog.UpdatedValue = "Id: " + model.EmpLeaveId;
                int bnoisUpdateCount = 0;
                if (employeeLeave.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeLeave.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)emp).PNo;
                }
                if (employeeLeave.LeaveTypeId != model.LeaveTypeId)
                {
                    if (employeeLeave.LeaveTypeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("LeaveType", "LeaveTypeId", employeeLeave.LeaveTypeId);
                        bnLog.PreviousValue += ", Leave Type: " + ((dynamic)prev).TypeName;
                    }
                    if (model.LeaveTypeId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("LeaveType", "LeaveTypeId", model.LeaveTypeId);
                        bnLog.UpdatedValue += ", Leave Type: " + ((dynamic)newv).TypeName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (employeeLeave.FromDate != model.FromDate)
                {
                    bnLog.PreviousValue += ", Date From: " + employeeLeave.FromDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Date From: " + model.FromDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeLeave.ToDate != model.ToDate)
                {
                    bnLog.PreviousValue += ", Date To: " + employeeLeave.ToDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Date To: " + model.ToDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeLeave.Duration != model.Duration)
                {
                    bnLog.PreviousValue += ", Duration: " + employeeLeave.Duration;
                    bnLog.UpdatedValue += ", Duration: " + model.Duration;
                    bnoisUpdateCount += 1;
                }
                if (employeeLeave.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + employeeLeave.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                    bnoisUpdateCount += 1;
                }
                if (employeeLeave.ExBdLeave != model.ExBdLeave)
                {
                    bnLog.PreviousValue += ", Ex Bd Leave: " + employeeLeave.ExBdLeave;
                    bnLog.UpdatedValue += ", Ex Bd Leave: " + model.ExBdLeave;
                    bnoisUpdateCount += 1;
                }
                if (employeeLeave.AccompanyBy != model.AccompanyBy)
                {
                    bnLog.PreviousValue += ", Accompany By: " + employeeLeave.AccompanyBy;
                    bnLog.UpdatedValue += ", Accompany By: " + model.AccompanyBy;
                    bnoisUpdateCount += 1;
                }
                if (employeeLeave.Purpose != model.Purpose)
                {
                    if (employeeLeave.Purpose > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("LeavePurpose", "PurposeId", employeeLeave.Purpose??0);
                        bnLog.PreviousValue += ", Purpose: " + ((dynamic)prev).Name;
                    }
                    if (model.Purpose > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("LeavePurpose", "PurposeId", model.Purpose??0);
                        bnLog.UpdatedValue += ", Purpose: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                //if (employeeLeave.Purpose != model.Purpose)
                //{
                //    bnLog.PreviousValue += ", Purpose: " + employeeLeave.Purpose;
                //    bnLog.UpdatedValue += ", Purpose: " + model.Purpose;
                //    bnoisUpdateCount += 1;
                //}

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
                //IQueryable<EmployeeLeaveYear> employeeLeaveYears = employeeLeaveRepository.FilterWithInclude(x =>x.EmployeeId == model.EmployeeId );
                IQueryable<EmployeeLeaveYear> employeeLeaves = employeeLeaveYearRepository.FilterWithInclude(x => x.EmployeeLeave.EmployeeId == model.EmployeeId && x.EmployeeLeave.LeaveTypeId == model.LeaveTypeId);
                var isSameDateExist = employeeLeaves.ToList().FirstOrDefault(x => x.LeaveDate.Date == Convert.ToDateTime(model.FromDate).Date || x.LeaveDate.Date == Convert.ToDateTime(model.ToDate).Date);
                if (isSameDateExist != null) throw new InfinityArgumentMissingException("Date Already Exist.");

                employeeLeave.Active = true;
                employeeLeave.CreatedDate = DateTime.Now.Date;
                employeeLeave.CreatedBy = userId;

            }
            employeeLeave.EmployeeLeaveYears = virtualEmloyeeInfo.EmployeeLeaveYears;

            // Employee Leave Country Value added in Employee Country List
            if (model.CountryIds != null) employeeLeave.EmployeeLeaveCountries = model.CountryIds.Select(x => new EmployeeLeaveCountry() { CountryId = x }).ToList();
            employeeLeave.EmployeeId = model.EmployeeId;
            employeeLeave.LeaveTypeId = model.LeaveTypeId;
            employeeLeave.FromDate = model.FromDate;
            employeeLeave.ToDate = model.ToDate;
            employeeLeave.Duration = model.Duration;
            employeeLeave.Remarks = model.Remarks;
            employeeLeave.ExBdLeave = model.ExBdLeave;
            employeeLeave.AccompanyBy = model.AccompanyBy;
            employeeLeave.Purpose = model.Purpose;
            model.FileName = model.FileName;
            model.FromDate = employeeLeave.FromDate;
            model.ToDate = employeeLeave.ToDate;

            employeeLeave.RankId = model.Employee.RankId; ;
            employeeLeave.TransferId = model.Employee.TransferId;

            if (model.IsBackLog)
            {
                employeeLeave.RankId = model.RankId;
                employeeLeave.TransferId = model.TransferId;

            }

            employeeLeave.Employee = null;
            await employeeLeaveRepository.SaveAsync(employeeLeave);
            model.EmpLeaveId = employeeLeave.EmpLeaveId;
            return model;
        }

        private async Task<ICollection<EmployeeLeaveYear>> GetEmployeeLeaveYearAsync(string leaveduecount,int id, int genderid, int employeeId, int leaveTypeId, DateTime? fromDate, DateTime? toDate, List<EmployeeLeaveBalance> leaveBalances)
        {
            List<EmployeeLeaveYear> employeeLeaveYears = new List<EmployeeLeaveYear>();
            int leavedue = Convert.ToInt16(leaveduecount);
            switch (leaveTypeId)
            {
                case CodeValue.PL_LeaveType: // Privileadge Leave Balance Check
                    List<EmployeeLeaveBalance> leaveBalanceList = leaveBalances;
                   
                    leaveBalances = leaveBalances.Where(x => x.IsAssigned == true).ToList();
                    if (!leaveBalances.Any())
                    {
                        throw new InfinityArgumentMissingException("Select Leave Slot");
                    }
                    else
                    {
                        int balanceSum = 0;
                        if (leaveBalanceList != null)
                        {
                            foreach (var item in leaveBalanceList.OrderBy(x => x.LeaveYear))
                            {
                                if (item.IsAssigned != true)
                                    balanceSum += item.Balance;
                                else break;
                            }
                            if (balanceSum != 0)
                            {
                                throw new InfinityArgumentMissingException("Please Select consecutive years");
                            }
                        }
                        // Employee Leave Year Value added in Employee Year List
                        int leaveEntryComplete = 0;

                        DateTime? DateFrom1 = fromDate;

                        foreach (var item in leaveBalances)
                        {
                            int leaveBalanced;
                            int totalLeaveBalanced;
                            int leaveEntryBySlot = 0;
                            int leaveDaysDeclared = (int)(Convert.ToDateTime(toDate).Date - Convert.ToDateTime(fromDate).Date).TotalDays + 1;
                            leaveBalanced = item.Balance;
                            totalLeaveBalanced = (leaveBalances.Sum(x => x.Balance));
                            if (totalLeaveBalanced < leaveDaysDeclared)
                            {
                                throw new InfinityArgumentMissingException("Your Leave Not Available");
                            }
                            for (int i = 0; leaveBalanced > leaveEntryBySlot && toDate >= DateFrom1 && leaveDaysDeclared >= leaveEntryComplete; i++)
                            {

                                EmployeeLeaveYear leaveYear = new EmployeeLeaveYear();
                                leaveYear.YearText = item.LeaveYear;
                                leaveYear.LeaveDate = (DateTime)DateFrom1;
                                employeeLeaveYears.Add(leaveYear);
                                DateFrom1 = Convert.ToDateTime(DateFrom1).Date.AddDays(1);

                                leaveEntryBySlot++;
                                leaveEntryComplete++;
                            }
                        }
                    }
                    break;
                case CodeValue.RL_LeaveType:
                    //DateTime? rlDate = GetRlDate(CodeValue.RL_LeaveType, employeeId, toDate, fromDate);
                    //rlDate = Convert.ToDateTime(rlDate).AddYears(CodeValue.AfterLprYears).Date;
                    DateTime? rlDateFrom = fromDate;
                    int leaveDaysDeclared1 = (int)(Convert.ToDateTime(toDate).Date - Convert.ToDateTime(fromDate).Date).TotalDays + 1;


                    if (leavedue < leaveDaysDeclared1)
                    {
                        throw new InfinityArgumentMissingException("Your Leave Not Available");
                    }
                   for (int i = 0; Convert.ToDateTime(rlDateFrom).Date <= Convert.ToDateTime(toDate).Date; i++)
                    {

                        EmployeeLeaveYear leaveYear = new EmployeeLeaveYear();
                        leaveYear.YearText = DateTime.Now.Date.Year;
                        leaveYear.LeaveDate = (DateTime)rlDateFrom;
                        employeeLeaveYears.Add(leaveYear);
                        rlDateFrom = Convert.ToDateTime(rlDateFrom).Date.AddDays(1);
                    }

                    break;
                case CodeValue.FL_LeaveType:
                    // Furlough Leave Balance Check
                   
                   // List<EmployeeLeaveBalance> leaveDays;
                    DateTime? DateFrom = fromDate;
                    //leaveDays = await employeeLeaveRepository.GetEmployeeRepository(employeeId, leaveTypeId);
                    //int filterBalance = leaveDays.Count > 0 ? (leaveDays.Sum(x => x.Balance)) : 0;
                    int leaveDaysDeclared2 = (int)(Convert.ToDateTime(toDate).Date - Convert.ToDateTime(fromDate).Date).TotalDays + 1;
                    if (leavedue < leaveDaysDeclared2)
                    {
                        throw new InfinityArgumentMissingException("Your Leave Not Available");
                    }                
                    for (int i = 0;Convert.ToDateTime(DateFrom).Date <= Convert.ToDateTime(toDate).Date; i++)
                        {

                            EmployeeLeaveYear leaveYear = new EmployeeLeaveYear();
                            leaveYear.YearText = DateTime.Now.Date.Year;
                            leaveYear.LeaveDate = (DateTime)DateFrom;
                            employeeLeaveYears.Add(leaveYear);
                            DateFrom = Convert.ToDateTime(DateFrom).Date.AddDays(1);                        
                        }
                   
                    break;
                default:
                    if (leaveTypeId == CodeValue.Materny_LeaveType && genderid == CodeValue.Male)
                    {
                        throw new InfinityArgumentMissingException("Your are not able for this leave.");
                    }                 
                    int leaveDaysDeclared3 = (int)(Convert.ToDateTime(toDate).Date - Convert.ToDateTime(fromDate).Date).TotalDays + 1;
                    DateTime? DateFromcc = fromDate;
                  
                        if (leavedue < leaveDaysDeclared3 && leaveTypeId != CodeValue.Ressese_LeaveType)
                        {
                            throw new InfinityArgumentMissingException("Your Leave Not Available");
                        }
                       
                            for (int i = 0;Convert.ToDateTime(DateFromcc).Date <= Convert.ToDateTime(toDate).Date; i++)
                            {
                                EmployeeLeaveYear leaveYear = new EmployeeLeaveYear();
                                leaveYear.YearText = DateTime.Now.Date.Year;
                                leaveYear.LeaveDate = (DateTime)DateFromcc;
                                employeeLeaveYears.Add(leaveYear);
                                DateFromcc = Convert.ToDateTime(DateFromcc).Date.AddDays(1);                            
                            }
                    break;
            }
            return employeeLeaveYears;

        }

        private DateTime? GetRlDate(int rlLeaveType, int EmployeeId, DateTime? employeeLeaveToDate, DateTime? employeeLeaveFromDate)
        {
            var employeeGeneral = employeeGeneralRepository.FindOne(x => x.EmployeeId == EmployeeId);
            LeavePolicy lp = leavePolicyRepository.FindOne(x =>
                x.CommissionTypeId == employeeGeneral.CommissionTypeId && x.LeaveTypeId == rlLeaveType);
            int lpDays = lp.LeaveDurationType == "1" ? lp.LeaveDuration * 30 : lp.LeaveDuration;
            int rlLeaveDays = (int)(Convert.ToDateTime(employeeLeaveToDate).Date - Convert.ToDateTime(employeeLeaveFromDate).Date).TotalDays + 1;
            if (lpDays < rlLeaveDays)
            {
                throw new InfinityArgumentMissingException("Your Leave Not Available");
            }
            var lastRlDate = employeeLeaveRepository.Where(x => x.LeaveTypeId == rlLeaveType && x.EmployeeId == EmployeeId).ToList();

            return lastRlDate.Count() > 0 ? lastRlDate.Max(x => x.ToDate) : employeeGeneral.CategoryId == CodeValue.PromotedOfficer || employeeGeneral.CategoryId == CodeValue.HonoraryOfficer ? employeeGeneral.LastRLAvailedDate
                : employeeGeneral.CommissionDate;
        }
        public async Task<LeaveSummaryModel> GetLeaveSummary(string pno)
        {
            try
            {
                LeaveSummaryModel leaveSummaryModel = new LeaveSummaryModel();
                List<EmployeeLeaveBalance> totaleaveBalance = await employeeLeaveRepository.GetTotalEmployeeRepository(pno);
                if (!totaleaveBalance.Any()) return leaveSummaryModel;
                int maternyLeaveGet = employeeLeaveRepository.Count(x =>x.Employee.PNo == pno && x.LeaveTypeId == CodeValue.Materny_LeaveType);
                int flLeaveGet = employeeLeaveRepository.Where(x =>x.Employee.PNo == pno && x.LeaveTypeId == CodeValue.FL_LeaveType).Sum(x =>x.Duration) ?? 0;
                DateTime commisionDate =(DateTime) totaleaveBalance.Select(x => x.CommissionDate).FirstOrDefault();
                int yy =((int) (DateTime.Now - commisionDate).TotalDays / 365);
                int fltotal = totaleaveBalance.Where(x => x.LeaveTypeId == CodeValue.FL_LeaveType).Sum(x => x.Balance);
                int empid = totaleaveBalance.Select(x =>x.EmployeeId).FirstOrDefault();
                leaveSummaryModel.PlAvailable = totaleaveBalance.Where(x => x.LeaveYear == DateTime.Now.Year && x.LeaveTypeId == CodeValue.PL_LeaveType).Select(x => x.Balance).FirstOrDefault();
                //leaveSummaryModel.TotalPlAvailable = totaleaveBalance.Where(x => x.LeaveTypeId == CodeValue.PL_LeaveType).Sum(x => x.Balance);
                var total = await employeeLeaveRepository.GetEmployeeDueRepository(empid, CodeValue.PL_LeaveType);
                leaveSummaryModel.TotalPlAvailable = total.Sum(x =>x.Balance);
                leaveSummaryModel.LastRlAvailed = GetRlDateForSummery(CodeValue.RL_LeaveType, empid);
                if (leaveSummaryModel.LastRlAvailed != null){
                    leaveSummaryModel.RlDue = Convert.ToDateTime(leaveSummaryModel.LastRlAvailed).AddYears(CodeValue.AfterLprYears);
                    leaveSummaryModel.IsTrue = Convert.ToDateTime(leaveSummaryModel.RlDue).Date < DateTime.Now.Date;
                }
                leaveSummaryModel.RecreationLeaveDue = leaveSummaryModel.RlDue < DateTime.Now.Date ? totaleaveBalance.Where(x =>x.LeaveTypeId == CodeValue.RL_LeaveType).Select(x => x.TotalLeave).LastOrDefault() : 0;

                leaveSummaryModel.CasualLeave = totaleaveBalance.Where(x => x.LeaveYear == DateTime.Now.Year && x.LeaveTypeId == CodeValue.CL_LeaveType).Select(x => x.Balance).FirstOrDefault();
                leaveSummaryModel.SickLeave = totaleaveBalance.Where(x => x.LeaveYear == DateTime.Now.Year && x.LeaveTypeId == CodeValue.Sick_LeaveType).Select(x => x.Balance).FirstOrDefault();
                leaveSummaryModel.MedicalLeave = totaleaveBalance.Where(x => x.LeaveYear == DateTime.Now.Year && x.LeaveTypeId == CodeValue.Medical_LeaveType).Select(x => x.Balance).FirstOrDefault();
                //leaveSummaryModel.TotalFurloughLeave = (yy * 30) > fltotal ? fltotal - flLeaveGet : (yy * 30) - flLeaveGet; 
                leaveSummaryModel.TotalFurloughLeave = (yy * 30) - flLeaveGet; 
                leaveSummaryModel.TerminalLeave = totaleaveBalance.Where(x => x.LeaveTypeId == CodeValue.Terminal_LeaveType).Select(x => x.Balance).FirstOrDefault();
                leaveSummaryModel.MaternyLeave = totaleaveBalance.Where(x => x.LeaveTypeId == CodeValue.Materny_LeaveType).Count() > maternyLeaveGet ? totaleaveBalance.Where(x => x.LeaveTypeId == CodeValue.Materny_LeaveType).Select(x => x.TotalLeave).FirstOrDefault() : 0;
                leaveSummaryModel.SurveyLeave = totaleaveBalance.Where(x => x.LeaveTypeId == CodeValue.Survey_LeaveType).Select(x => x.Balance).FirstOrDefault();
                leaveSummaryModel.WoundLeave = totaleaveBalance.Where(x => x.LeaveTypeId == CodeValue.Wonded_LeaveType).Select(x => x.Balance).FirstOrDefault();
       
                return leaveSummaryModel;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private DateTime? GetRlDateForSummery(int rlLeaveType, int employeeid)
        {
            var employeeGeneral = employeeGeneralRepository.FindOne(x => x.Employee.EmployeeId == employeeid);

            var lastRlDate = employeeLeaveRepository.Where(x => x.LeaveTypeId == rlLeaveType && x.Employee.EmployeeId == employeeid).ToList();

            DateTime? date = lastRlDate.Count() != 0 ? lastRlDate.Max(x => x.FromDate) : employeeGeneral.CategoryId == CodeValue.PromotedOfficer || employeeGeneral.CategoryId == CodeValue.HonoraryOfficer ? employeeGeneral.LastRLAvailedDate
                : employeeGeneral.CommissionDate;
            return date;
        }

        public async Task<string> GetEmployeeLeaveDue(int employeeId, int leaveType)
        {
            if (employeeId <= 0)
            {
                throw new InfinityArgumentMissingException("Invalide Officers Pno!");
            }
            //await employeeLeaveRepository.GetTotalEmployeeRepository(pno);
            Employee employee = employeeRepository.FindOne(x => x.EmployeeId == employeeId);
            EmployeeGeneral empGen = employeeGeneralRepository.FindOne(x=>x.EmployeeId==employeeId);
            if(empGen.CategoryId == 6)
            {
                return null;
            }
            else
            {
                List<EmployeeLeaveBalance> totalLeaveBalance = await employeeLeaveRepository.GetTotalEmployeeRepository(employee.PNo);
                //List<EmployeeLeaveBalance> totalLeaveBalance = await employeeLeaveRepository.GetEmployeeDueRepository(employeeId, leaveType);
                //////////////////
                int maternyLeaveGet = employeeLeaveRepository.Count(x => x.Employee.EmployeeId == employeeId && x.LeaveTypeId == CodeValue.Materny_LeaveType);
                int flLeaveGet = employeeLeaveRepository.Where(x => x.Employee.EmployeeId == employeeId && x.LeaveTypeId == CodeValue.FL_LeaveType).Sum(x => x.Duration) ?? 0;
                DateTime commisionDate = (DateTime)totalLeaveBalance.Select(x => x.CommissionDate).FirstOrDefault();
                int yy = ((int)(DateTime.Now - commisionDate).TotalDays / 365);
                int fltotal = totalLeaveBalance.Where(x => x.LeaveTypeId == CodeValue.FL_LeaveType).Sum(x => x.Balance);


                //leaveSummaryModel.PlAvailable = totalLeaveBalance.Where(x => x.LeaveYear == DateTime.Now.Year && x.LeaveTypeId == CodeValue.PL_LeaveType).Select(x => x.Balance).FirstOrDefault();
                //leaveSummaryModel.TotalPlAvailable = totalLeaveBalance.Where(x => x.LeaveTypeId == CodeValue.PL_LeaveType).Sum(x => x.Balance);
                //leaveSummaryModel.LastRlAvailed = (DateTime)GetRlDateForSummery(CodeValue.RL_LeaveType, pno);
                //leaveSummaryModel.RlDue = leaveSummaryModel.LastRlAvailed.AddYears(CodeValue.AfterLprYears);
                //leaveSummaryModel.RecreationLeaveDue = leaveSummaryModel.RlDue < DateTime.Now.Date ? totaleaveBalance.Where(x => x.LeaveTypeId == CodeValue.RL_LeaveType).Select(x => x.TotalLeave).LastOrDefault() : 0;



                string result = null;
                switch (leaveType)
                {
                    case CodeValue.PL_LeaveType:
                        result = totalLeaveBalance.Where(x => x.LeaveYear == DateTime.Now.Year && x.LeaveTypeId == CodeValue.PL_LeaveType).Select(x => x.Balance).FirstOrDefault().ToString();
                        break;
                    case CodeValue.RL_LeaveType:
                        DateTime rldate = (DateTime)GetRlDateForSummery(CodeValue.RL_LeaveType, employeeId);
                        DateTime rldue = rldate.AddYears(CodeValue.AfterLprYears);
                        result = rldue < DateTime.Now.Date ? totalLeaveBalance.Where(x => x.LeaveTypeId == CodeValue.RL_LeaveType).Select(x => x.TotalLeave).LastOrDefault().ToString() : "0";
                        break;
                    case CodeValue.CL_LeaveType:
                        result = totalLeaveBalance.Where(x => x.LeaveYear == DateTime.Now.Year && x.LeaveTypeId == CodeValue.CL_LeaveType).Select(x => x.Balance).FirstOrDefault().ToString();
                        break;
                    case CodeValue.Sick_LeaveType:
                        result = totalLeaveBalance.Where(x => x.LeaveYear == DateTime.Now.Year && x.LeaveTypeId == CodeValue.Sick_LeaveType).Select(x => x.Balance).FirstOrDefault().ToString();
                        break;
                    case CodeValue.Medical_LeaveType:
                        result = totalLeaveBalance.Where(x => x.LeaveYear == DateTime.Now.Year && x.LeaveTypeId == CodeValue.Medical_LeaveType).Select(x => x.Balance).FirstOrDefault().ToString();
                        break;
                    case CodeValue.FL_LeaveType:
                        result = (yy * 30) > fltotal ? (fltotal - flLeaveGet).ToString() : ((yy * 30) - flLeaveGet).ToString();
                        break;
                    case CodeValue.Terminal_LeaveType:
                        result = totalLeaveBalance.Where(x => x.LeaveYear == DateTime.Now.Year && x.LeaveTypeId == CodeValue.Terminal_LeaveType).Select(x => x.Balance).FirstOrDefault().ToString();
                        break;
                    case CodeValue.Materny_LeaveType:
                        result = totalLeaveBalance.Where(x => x.LeaveTypeId == CodeValue.Materny_LeaveType).Count() > maternyLeaveGet ? totalLeaveBalance.Where(x => x.LeaveTypeId == CodeValue.Materny_LeaveType).Select(x => x.TotalLeave).FirstOrDefault().ToString() : "0";
                        break;
                    case CodeValue.Survey_LeaveType:
                        result = totalLeaveBalance.Where(x => x.LeaveYear == DateTime.Now.Year && x.LeaveTypeId == CodeValue.Survey_LeaveType).Select(x => x.Balance).FirstOrDefault().ToString();
                        break;
                    case CodeValue.Wonded_LeaveType:
                        result = totalLeaveBalance.Where(x => x.LeaveYear == DateTime.Now.Year && x.LeaveTypeId == CodeValue.Wonded_LeaveType).Select(x => x.Balance).FirstOrDefault().ToString();
                        break;
                }
                return result;

                ///////////////
                //totalLeaveBalance = totalLeaveBalance.Where(x => x.LeaveYear <= DateTime.Now.Year).ToList();
                //int maternyLeaveGet = employeeLeaveRepository.Count(x => x.EmployeeId == employeeId && x.LeaveTypeId == CodeValue.Materny_LeaveType);
                //string result = null;
                //switch (leaveType)
                //{
                //    case CodeValue.PL_LeaveType:
                //        result = totalLeaveBalance.Where(x => x.LeaveYear == DateTime.Now.Year).Select(x => x.Balance).FirstOrDefault().ToString();
                //        break;
                //    case CodeValue.CL_LeaveType:
                //        result = totalLeaveBalance.Where(x => x.LeaveYear == DateTime.Now.Year).Select(x => x.Balance).FirstOrDefault().ToString();
                //        break;
                //    case CodeValue.Sick_LeaveType:
                //        result = totalLeaveBalance.Where(x => x.LeaveYear == DateTime.Now.Year).Select(x => x.Balance).FirstOrDefault().ToString();
                //        break;
                //    case CodeValue.Medical_LeaveType:
                //        result = totalLeaveBalance.Where(x => x.LeaveYear == DateTime.Now.Year).Select(x => x.Balance).FirstOrDefault().ToString();
                //        break;
                //    case CodeValue.FL_LeaveType:
                //        result = totalLeaveBalance.Sum(x => x.Balance).ToString();
                //        break;
                //    case CodeValue.Terminal_LeaveType:
                //        result = totalLeaveBalance.Where(x => x.LeaveYear == DateTime.Now.Year).Select(x => x.Balance).FirstOrDefault().ToString();
                //        break;
                //    case CodeValue.Materny_LeaveType:
                //        result = totalLeaveBalance.Count() > maternyLeaveGet ? totalLeaveBalance.Select(x => x.TotalLeave).FirstOrDefault().ToString() : "0";
                //        break;
                //    case CodeValue.Survey_LeaveType:
                //        result = totalLeaveBalance.Where(x => x.LeaveYear == DateTime.Now.Year).Select(x => x.Balance).FirstOrDefault().ToString();
                //        break;
                //    case CodeValue.Wonded_LeaveType:
                //        result = totalLeaveBalance.Where(x => x.LeaveYear == DateTime.Now.Year).Select(x => x.Balance).FirstOrDefault().ToString();
                //        break;
                //}
                //return result;
            }

        }

        public async Task<List<EmployeeLeaveBalance>> GetDefaultEmployeeLeaveBalance(int employeeId, int leaveType, int fromDate)
        {
            if (fromDate < 0)
            {
                throw new InfinityArgumentMissingException("Invalide Date!");
            }
            List<EmployeeLeaveBalance> totalLeaveBalance = await employeeLeaveRepository.GetEmployeeRepository(employeeId, leaveType);
            //int slot = totalLeaveBalance.Select(x => x.Slot).FirstOrDefault();
            //List<EmployeeLeaveBalance> filterLeaveBalance = totalLeaveBalance.OrderByDescending(x => x.LeaveYear).Take(slot).ToList();
            return totalLeaveBalance.Where(x =>x.LeaveYear == (fromDate-2) || x.LeaveYear == (fromDate - 1) || x.LeaveYear == (fromDate) || x.LeaveYear == (fromDate +1) || x.LeaveYear == (fromDate + 2)).OrderBy(x => x.LeaveYear).ToList();
        }
    }
}
