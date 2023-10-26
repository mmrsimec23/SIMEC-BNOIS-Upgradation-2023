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
    public class EmployeeSecurityClearanceService: IEmployeeSecurityClearanceService
    {

        private readonly IBnoisRepository<EmployeeSecurityClearance> employeeSecurityClearanceRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public EmployeeSecurityClearanceService(IBnoisRepository<EmployeeSecurityClearance> employeeSecurityClearanceRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.employeeSecurityClearanceRepository = employeeSecurityClearanceRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

  

        public List<EmployeeSecurityClearanceModel> GetEmployeeSecurityClearances(int ps, int pn, string qs, out int total)
        {

            IQueryable<EmployeeSecurityClearance> employeeSecurityClearances = employeeSecurityClearanceRepository
                .FilterWithInclude(x => x.IsActive && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee", "SecurityClearanceReason");
            total = employeeSecurityClearances.Count();
            employeeSecurityClearances = employeeSecurityClearances.OrderByDescending(x => x.EmployeeSecurityClearanceId).Skip((pn - 1) * ps).Take(ps);
            List<EmployeeSecurityClearanceModel> models = ObjectConverter<EmployeeSecurityClearance, EmployeeSecurityClearanceModel>.ConvertList(employeeSecurityClearances.ToList()).ToList();
            return models;
        }

        public async Task<EmployeeSecurityClearanceModel> GetEmployeeSecurityClearance(int id)
        {
            if (id == 0)
            {
                return new EmployeeSecurityClearanceModel();
            }
            EmployeeSecurityClearance employeeSecurityClearance = await employeeSecurityClearanceRepository.FindOneAsync(x => x.EmployeeSecurityClearanceId == id, new List<string> { "Employee","Employee.Rank","Employee.Batch" });
            if (employeeSecurityClearance == null)
            {
                throw new InfinityNotFoundException("Employee Security Clearance not found");
            }
            EmployeeSecurityClearanceModel model = ObjectConverter<EmployeeSecurityClearance, EmployeeSecurityClearanceModel>.Convert(employeeSecurityClearance);
            return model;
        }

        public async Task<EmployeeSecurityClearanceModel> SaveEmployeeSecurityClearance(int id, EmployeeSecurityClearanceModel model)
        {

            if (model == null)
            {
                throw new InfinityArgumentMissingException("Employee Security Clearance data missing");
            }
            EmployeeSecurityClearance employeeSecurityClearance = ObjectConverter<EmployeeSecurityClearanceModel, EmployeeSecurityClearance>.Convert(model);
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();

            if (id > 0)
            {
                employeeSecurityClearance = employeeSecurityClearanceRepository.FindOne(x => x.EmployeeSecurityClearanceId == id, new List<string> { "Employee", "Employee.Rank", "SecurityClearanceReason" });
                if (employeeSecurityClearance == null)
                {
                    throw new InfinityNotFoundException("Employee Security Clearance not found !");
                }


                employeeSecurityClearance.ModifiedDate = DateTime.Now;
                employeeSecurityClearance.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeSecurityClearance";
                bnLog.TableEntryForm = "Officer Security Clearance";
                bnLog.PreviousValue = "Id: " + model.EmployeeSecurityClearanceId;
                bnLog.UpdatedValue = "Id: " + model.EmployeeSecurityClearanceId;
                if (employeeSecurityClearance.EmployeeId != model.EmployeeId)
                {
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", Name: " + employeeSecurityClearance.Employee.Name + " _ " + employeeSecurityClearance.Employee.PNo;
                    bnLog.UpdatedValue += ", Name: " + ((dynamic)emp).Name + " _ " + ((dynamic)emp).PNo;
                }
                if (employeeSecurityClearance.RankId != model.RankId)
                {
                    var rank = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.RankId ?? 0);
                    bnLog.PreviousValue += ", Rank: " + employeeSecurityClearance.Rank.ShortName;
                    bnLog.UpdatedValue += ", Rank: " + ((dynamic)rank).ShortName;
                }
                if (employeeSecurityClearance.SecurityClearanceReasonId != model.SecurityClearanceReasonId)
                {
                    var reason = employeeService.GetDynamicTableInfoById("SecurityClearanceReason", "SecurityClearanceReasonId", model.SecurityClearanceReasonId);
                    bnLog.PreviousValue += ", Reason: " + employeeSecurityClearance.SecurityClearanceReason.Reason;
                    bnLog.UpdatedValue += ", Reason: " + ((dynamic)reason).Reason;
                }
                if (employeeSecurityClearance.TransferId != model.TransferId)
                {
                    bnLog.PreviousValue += ", Transfer: " + employeeSecurityClearance.TransferId;
                    bnLog.UpdatedValue += ", Transfer: " + model.TransferId;
                }
                if (employeeSecurityClearance.IsCleared != model.IsCleared)
                {
                    bnLog.PreviousValue += ", IsCleared: " + employeeSecurityClearance.IsCleared;
                    bnLog.UpdatedValue += ", IsCleared: " + model.IsCleared;
                }
                if (employeeSecurityClearance.NotClearReason != model.NotClearReason)
                {
                    bnLog.PreviousValue += ", NotClearReason: " + employeeSecurityClearance.NotClearReason;
                    bnLog.UpdatedValue += ", NotClearReason: " + model.NotClearReason;
                }
                if (employeeSecurityClearance.ClearanceDate != model.ClearanceDate)
                {
                    bnLog.PreviousValue += ", ClearanceDate: " + employeeSecurityClearance.ClearanceDate;
                    bnLog.UpdatedValue += ", ClearanceDate: " + model.ClearanceDate;
                }
                if (employeeSecurityClearance.Expirydate != model.Expirydate)
                {
                    bnLog.PreviousValue += ", Expirydate: " + employeeSecurityClearance.Expirydate;
                    bnLog.UpdatedValue += ", Expirydate: " + model.Expirydate;
                }
                if (employeeSecurityClearance.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + employeeSecurityClearance.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (employeeSecurityClearance.EmployeeId != model.EmployeeId || employeeSecurityClearance.RankId != model.RankId || employeeSecurityClearance.Remarks != model.Remarks 
                    || employeeSecurityClearance.SecurityClearanceReasonId != model.SecurityClearanceReasonId || employeeSecurityClearance.TransferId != model.TransferId || employeeSecurityClearance.IsCleared != model.IsCleared
                    || employeeSecurityClearance.NotClearReason != model.NotClearReason || employeeSecurityClearance.ClearanceDate != model.ClearanceDate || employeeSecurityClearance.Expirydate != model.Expirydate)
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
                employeeSecurityClearance.CreatedDate = DateTime.Now;
                employeeSecurityClearance.CreatedBy = userId;
                employeeSecurityClearance.IsActive = true;
                employeeSecurityClearance.Employee = null;
            }
            employeeSecurityClearance.EmployeeId = model.EmployeeId;
            employeeSecurityClearance.SecurityClearanceReasonId = model.SecurityClearanceReasonId;
            employeeSecurityClearance.Remarks = model.Remarks;
            employeeSecurityClearance.IsCleared = model.IsCleared;
            employeeSecurityClearance.NotClearReason = model.NotClearReason;
            employeeSecurityClearance.ClearanceDate =(DateTime) model.ClearanceDate;
            employeeSecurityClearance.Expirydate = model.Expirydate;
            employeeSecurityClearance.RankId = model.Employee.RankId;
            employeeSecurityClearance.TransferId = model.Employee.TransferId;

            if (model.IsBackLog)
            {
                employeeSecurityClearance.RankId = model.RankId;
                employeeSecurityClearance.TransferId = model.TransferId;
            }

            //employeeSecurityClearance.Employee = null;
            //await employeeSecurityClearanceRepository.SaveAsync(employeeSecurityClearance);
            //model.EmployeeSecurityClearanceId = employeeSecurityClearance.EmployeeSecurityClearanceId;
            try
            {
                await employeeSecurityClearanceRepository.SaveAsync(employeeSecurityClearance);
                model.EmployeeSecurityClearanceId = employeeSecurityClearance.EmployeeSecurityClearanceId;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
            return model;
        }

        public async Task<bool> DeleteEmployeeSecurityClearance(int id)
        {
            if (id <= 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            EmployeeSecurityClearance employeeSecurityClearance = await employeeSecurityClearanceRepository.FindOneAsync(x => x.EmployeeSecurityClearanceId == id);
            if (employeeSecurityClearance == null)
            {
                throw new InfinityNotFoundException("Employee Security Clearance not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeSecurityClearance";
                bnLog.TableEntryForm = "Officer Security Clearance";
                var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeSecurityClearance.EmployeeId);
                var rank = employeeService.GetDynamicTableInfoById("Rank", "RankId", employeeSecurityClearance.RankId ?? 0);
                var reason = employeeService.GetDynamicTableInfoById("SecurityClearanceReason", "SecurityClearanceReasonId", employeeSecurityClearance.SecurityClearanceReasonId);
                var transfer = employeeService.GetDynamicTableInfoById("Transfer", "TransferId", employeeSecurityClearance.TransferId ?? 0);
                bnLog.PreviousValue = "Id: " + employeeSecurityClearance.EmployeeSecurityClearanceId + ", Name: " + ((dynamic)emp).Name + ", Rank: " + ((dynamic)rank).ShortName
                    + ", Remarks: " + employeeSecurityClearance.Remarks + ", Reason: " + ((dynamic)reason).Reason + ", Transfer: " + ((dynamic)emp).FromDate
                    + ", IsCleared: " + employeeSecurityClearance.IsCleared + ", NotClearReason: " + employeeSecurityClearance.NotClearReason + ", ClearanceDate: " + employeeSecurityClearance.ClearanceDate + ", Expirydate: " + employeeSecurityClearance.Expirydate;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await employeeSecurityClearanceRepository.DeleteAsync(employeeSecurityClearance);
            }
        }
    }
}