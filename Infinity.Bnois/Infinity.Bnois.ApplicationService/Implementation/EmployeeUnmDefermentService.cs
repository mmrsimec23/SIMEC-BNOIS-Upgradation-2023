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
    public class EmployeeUnmDefermentService: IEmployeeUnmDefermentService
    {

        private readonly IBnoisRepository<DashBoardBranch975> employeeUnmDefermentRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public EmployeeUnmDefermentService(IBnoisRepository<DashBoardBranch975> employeeUnmDefermentRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.employeeUnmDefermentRepository = employeeUnmDefermentRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

  

        public List<DashBoardBranch975Model> GetEmployeeUnmDeferments(int ps, int pn, string qs, out int total)
        {

            IQueryable<DashBoardBranch975> EmployeeUnmDeferments = employeeUnmDefermentRepository
                .FilterWithInclude(x => x.IsActive && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee","Rank");
            total = EmployeeUnmDeferments.Count();
            EmployeeUnmDeferments = EmployeeUnmDeferments.OrderByDescending(x => x.Id).Skip((pn - 1) * ps).Take(ps);
            List<DashBoardBranch975Model> models = ObjectConverter<DashBoardBranch975, DashBoardBranch975Model>.ConvertList(EmployeeUnmDeferments.ToList()).ToList();
            return models;
        }

        public async Task<DashBoardBranch975Model> GetEmployeeUnmDeferment(int id)
        {
            if (id == 0)
            {
                return new DashBoardBranch975Model();
            }
            DashBoardBranch975 EmployeeUnmDeferment = await employeeUnmDefermentRepository.FindOneAsync(x => x.Id == id, new List<string> { "Employee","Employee.Rank","Employee.Batch" });
            if (EmployeeUnmDeferment == null)
            {
                throw new InfinityNotFoundException("Employee Msc Education not found");
            }
            DashBoardBranch975Model model = ObjectConverter<DashBoardBranch975, DashBoardBranch975Model>.Convert(EmployeeUnmDeferment);
            return model;
        }

        public async Task<DashBoardBranch975Model> SaveEmployeeUnmDeferment(int id, DashBoardBranch975Model model)
        {

            if (model == null)
            {
                throw new InfinityArgumentMissingException("Employee Msc Education data missing");
            }
            if (model.DefermentFrom != null && model.DurationYear > 0)
            {
                int days = 365 * model.DurationYear ?? 0;
                model.DefermentTo = model.DefermentFrom.AddDays(days);
            }
            DashBoardBranch975 EmployeeUnmDeferment = ObjectConverter<DashBoardBranch975Model, DashBoardBranch975>.Convert(model);
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();

            if (id > 0)
            {
                EmployeeUnmDeferment = employeeUnmDefermentRepository.FindOne(x => x.Id == id);
                if (EmployeeUnmDeferment == null)
                {
                    throw new InfinityNotFoundException("Employee Msc Education not found !");
                }


                EmployeeUnmDeferment.ModifiedDate = DateTime.Now;
                EmployeeUnmDeferment.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "DashBoardBranch975";
                bnLog.TableEntryForm = "Employee UNM Deferment";
                bnLog.PreviousValue = "Id: " + model.Id;
                bnLog.UpdatedValue = "Id: " + model.Id;
                int bnoisUpdateCount = 0;
                if (EmployeeUnmDeferment.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", EmployeeUnmDeferment.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)emp).PNo;
                }
                
                if (EmployeeUnmDeferment.UnmReason != model.UnmReason)
                {
                    bnLog.PreviousValue += ", Deferment Reason: " + (EmployeeUnmDeferment.UnmReason == 1 ? "Unwilling": EmployeeUnmDeferment.UnmReason == 2 ? "Punishment" : "");
                    bnLog.UpdatedValue += ", Deferment Reason: " + (model.UnmReason == 1 ? "Unwilling" : model.UnmReason == 2 ? "Punishment" : "");
                    bnoisUpdateCount += 1;
                }
                if (EmployeeUnmDeferment.DefermentFrom != model.DefermentFrom)
                {
                    bnLog.PreviousValue += ", Deferment From: " + EmployeeUnmDeferment.DefermentFrom.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Deferment From: " + model.DefermentFrom.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (EmployeeUnmDeferment.DurationYear != model.DurationYear)
                {
                    bnLog.PreviousValue += ", Duration Year: " + EmployeeUnmDeferment.DurationYear;
                    bnLog.UpdatedValue += ", Duration Year: " + model.DurationYear;
                    bnoisUpdateCount += 1;
                }
                if (EmployeeUnmDeferment.DefermentTo != model.DefermentTo)
                {
                    bnLog.PreviousValue += ", Deferment To: " + EmployeeUnmDeferment.DefermentTo?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Deferment To: " + model.DefermentTo?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (EmployeeUnmDeferment.UnmReference != model.UnmReference)
                {
                    bnLog.PreviousValue += ", Authority Ltr: " + EmployeeUnmDeferment.UnmReference;
                    bnLog.UpdatedValue += ", Authority Ltr: " + model.UnmReference;
                    bnoisUpdateCount += 1;
                }
                if (EmployeeUnmDeferment.UnwillingMissionRemarks != model.UnwillingMissionRemarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + EmployeeUnmDeferment.UnwillingMissionRemarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.UnwillingMissionRemarks;
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
                EmployeeUnmDeferment.CreatedDate = DateTime.Now;
                EmployeeUnmDeferment.CreatedBy = userId;
                EmployeeUnmDeferment.IsActive = true;
            }
            EmployeeUnmDeferment.EmployeeId = model.EmployeeId;
            //EmployeeUnmDeferment.RankId = model.Employee.RankId;
            EmployeeUnmDeferment.DefermentFrom = model.DefermentFrom;
            EmployeeUnmDeferment.DurationYear = model.DurationYear;
            EmployeeUnmDeferment.DefermentTo = model.DefermentTo;
            EmployeeUnmDeferment.UnmReason = model.UnmReason;
            EmployeeUnmDeferment.UnwillingMissionRemarks = model.UnwillingMissionRemarks;
            EmployeeUnmDeferment.UnmReference = model.UnmReference;

            //if (model.IsBackLog)
            //{
            //    EmployeeUnmDeferment.RankId = model.RankId;
            //    EmployeeUnmDeferment.TransferId = model.TransferId;
            //}


            EmployeeUnmDeferment.Employee = null;

            await employeeUnmDefermentRepository.SaveAsync(EmployeeUnmDeferment);
            model.Id = EmployeeUnmDeferment.Id;
            return model;
        }

        public async Task<bool> DeleteEmployeeUnmDeferment(int id)
        {
            if (id <= 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            DashBoardBranch975 EmployeeUnmDeferment = await employeeUnmDefermentRepository.FindOneAsync(x => x.Id == id);
            if (EmployeeUnmDeferment == null)
            {
                throw new InfinityNotFoundException("Employee Family Permission not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "DashBoardBranch975";
                bnLog.TableEntryForm = "Employee UNM Deferment";
                bnLog.PreviousValue = "Id: " + EmployeeUnmDeferment.Id;
                if (EmployeeUnmDeferment.EmployeeId > 0)
                {
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", EmployeeUnmDeferment.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)emp).PNo;
                }
                if (EmployeeUnmDeferment.RankId > 0)
                {
                    var prevrank = employeeService.GetDynamicTableInfoById("Rank", "RankId", EmployeeUnmDeferment.RankId ?? 0);
                    bnLog.PreviousValue += ", Rank: " + ((dynamic)prevrank).ShortName;
                }
                bnLog.PreviousValue +=  ", Reason: " + (EmployeeUnmDeferment.UnmReason == 1 ? "Unwiling" : EmployeeUnmDeferment.UnmReason == 2 ? "Punishment" : "") + ", Remarks: " + EmployeeUnmDeferment.UnwillingMissionRemarks + ", Duration Year: " + EmployeeUnmDeferment.DurationYear +
                    ", Deferment From: " + EmployeeUnmDeferment.DefermentFrom.ToString("dd/MM/yyyy") + ", Deferment To: " + EmployeeUnmDeferment.DefermentTo?.ToString("dd/MM/yyyy") + ", Authority: " + EmployeeUnmDeferment.UnmReference;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await employeeUnmDefermentRepository.DeleteAsync(EmployeeUnmDeferment);
            }
        }
    }
}