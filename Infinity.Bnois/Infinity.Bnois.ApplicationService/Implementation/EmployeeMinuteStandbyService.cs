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
    public class EmployeeMinuteStandbyService: IEmployeeMinuteStandbyService
    {

        private readonly IBnoisRepository<DashBoardMinuteStandby975> employeeMinuteStandbyRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public EmployeeMinuteStandbyService(IBnoisRepository<DashBoardMinuteStandby975> employeeMinuteStandbyRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.employeeMinuteStandbyRepository = employeeMinuteStandbyRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

  

        public List<DashBoardMinuteStandby975Model> GetEmployeeMinuteStandbys(int ps, int pn, string qs, out int total)
        {

            IQueryable<DashBoardMinuteStandby975> EmployeeMinuteStandbys = employeeMinuteStandbyRepository
                .FilterWithInclude(x => x.IsActive && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee","Rank");
            total = EmployeeMinuteStandbys.Count();
            EmployeeMinuteStandbys = EmployeeMinuteStandbys.OrderByDescending(x => x.Id).Skip((pn - 1) * ps).Take(ps);
            List<DashBoardMinuteStandby975Model> models = ObjectConverter<DashBoardMinuteStandby975, DashBoardMinuteStandby975Model>.ConvertList(EmployeeMinuteStandbys.ToList()).ToList();
            return models;
        }

        public async Task<DashBoardMinuteStandby975Model> GetEmployeeMinuteStandby(int id)
        {
            if (id == 0)
            {
                return new DashBoardMinuteStandby975Model();
            }
            DashBoardMinuteStandby975 EmployeeMinuteStandby = await employeeMinuteStandbyRepository.FindOneAsync(x => x.Id == id, new List<string> { "Employee","Employee.Rank","Employee.Batch" });
            if (EmployeeMinuteStandby == null)
            {
                throw new InfinityNotFoundException("Employee Msc Education not found");
            }
            DashBoardMinuteStandby975Model model = ObjectConverter<DashBoardMinuteStandby975, DashBoardMinuteStandby975Model>.Convert(EmployeeMinuteStandby);
            return model;
        }

        public async Task<DashBoardMinuteStandby975Model> SaveEmployeeMinuteStandby(int id, DashBoardMinuteStandby975Model model)
        {

            if (model == null)
            {
                throw new InfinityArgumentMissingException("Employee Msc Education data missing");
            }
            DashBoardMinuteStandby975 EmployeeMinuteStandby = ObjectConverter<DashBoardMinuteStandby975Model, DashBoardMinuteStandby975>.Convert(model);
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();

            if (id > 0)
            {
                EmployeeMinuteStandby = employeeMinuteStandbyRepository.FindOne(x => x.Id == id);
                if (EmployeeMinuteStandby == null)
                {
                    throw new InfinityNotFoundException("Employee Msc Education not found !");
                }


                EmployeeMinuteStandby.ModifiedDate = DateTime.Now;
                EmployeeMinuteStandby.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "DashBoardMinuteStandby975";
                bnLog.TableEntryForm = "Employee Minute StandBy";
                bnLog.PreviousValue = "Id: " + model.Id;
                bnLog.UpdatedValue = "Id: " + model.Id;
                int bnoisUpdateCount = 0;
                if (EmployeeMinuteStandby.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", EmployeeMinuteStandby.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)emp).PNo;
                }
                
                if (EmployeeMinuteStandby.StandbyRemarks1 != model.StandbyRemarks1)
                {
                    bnLog.PreviousValue += ", Remarks: " + EmployeeMinuteStandby.StandbyRemarks1;
                    bnLog.UpdatedValue += ", Remarks: " + model.StandbyRemarks1;
                    bnoisUpdateCount += 1;
                }
                if (EmployeeMinuteStandby.DateFrom != model.DateFrom)
                {
                    bnLog.PreviousValue += ", Date From: " + EmployeeMinuteStandby.DateFrom.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Date From: " + model.DateFrom.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (EmployeeMinuteStandby.DateTo != model.DateTo)
                {
                    bnLog.PreviousValue += ", Date To: " + EmployeeMinuteStandby.DateTo?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Date To: " + model.DateTo?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (EmployeeMinuteStandby.StandbyRemarks2 != model.StandbyRemarks2)
                {
                    bnLog.PreviousValue += ", StandbyRemarks2: " + EmployeeMinuteStandby.StandbyRemarks2;
                    bnLog.UpdatedValue += ", StandbyRemarks2: " + model.StandbyRemarks2;
                    bnoisUpdateCount += 1;
                }
                if (EmployeeMinuteStandby.StandbyRemarks3 != model.StandbyRemarks3)
                {
                    bnLog.PreviousValue += ", StandbyRemarks3: " + EmployeeMinuteStandby.StandbyRemarks3;
                    bnLog.UpdatedValue += ", StandbyRemarks3: " + model.StandbyRemarks3;
                    bnoisUpdateCount += 1;
                }
                if (EmployeeMinuteStandby.StandbyRemarks4 != model.StandbyRemarks4)
                {
                    bnLog.PreviousValue += ", StandbyRemarks4: " + EmployeeMinuteStandby.StandbyRemarks4;
                    bnLog.UpdatedValue += ", StandbyRemarks4: " + model.StandbyRemarks4;
                    bnoisUpdateCount += 1;
                }
                if (EmployeeMinuteStandby.StandbyRemarks5 != model.StandbyRemarks5)
                {
                    bnLog.PreviousValue += ", StandbyRemarks5: " + EmployeeMinuteStandby.StandbyRemarks5;
                    bnLog.UpdatedValue += ", StandbyRemarks5: " + model.StandbyRemarks5;
                    bnoisUpdateCount += 1;
                }
                if (EmployeeMinuteStandby.StandbyRemarks6 != model.StandbyRemarks6)
                {
                    bnLog.PreviousValue += ", StandbyRemarks6: " + EmployeeMinuteStandby.StandbyRemarks6;
                    bnLog.UpdatedValue += ", StandbyRemarks6: " + model.StandbyRemarks6;
                    bnoisUpdateCount += 1;
                }
                if (EmployeeMinuteStandby.StandbyRemarks7 != model.StandbyRemarks7)
                {
                    bnLog.PreviousValue += ", StandbyRemarks7: " + EmployeeMinuteStandby.StandbyRemarks7;
                    bnLog.UpdatedValue += ", StandbyRemarks7: " + model.StandbyRemarks7;
                    bnoisUpdateCount += 1;
                }
                if (EmployeeMinuteStandby.StandbyRemarks8 != model.StandbyRemarks8)
                {
                    bnLog.PreviousValue += ", StandbyRemarks8: " + EmployeeMinuteStandby.StandbyRemarks8;
                    bnLog.UpdatedValue += ", StandbyRemarks8: " + model.StandbyRemarks8;
                    bnoisUpdateCount += 1;
                }
                if (EmployeeMinuteStandby.StandbyRemarks9 != model.StandbyRemarks9)
                {
                    bnLog.PreviousValue += ", StandbyRemarks9: " + EmployeeMinuteStandby.StandbyRemarks9;
                    bnLog.UpdatedValue += ", StandbyRemarks9: " + model.StandbyRemarks9;
                    bnoisUpdateCount += 1;
                }
                if (EmployeeMinuteStandby.StandbyRemarks10 != model.StandbyRemarks10)
                {
                    bnLog.PreviousValue += ", StandbyRemarks10: " + EmployeeMinuteStandby.StandbyRemarks10;
                    bnLog.UpdatedValue += ", StandbyRemarks10: " + model.StandbyRemarks10;
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
                EmployeeMinuteStandby.CreatedDate = DateTime.Now;
                EmployeeMinuteStandby.CreatedBy = userId;
                EmployeeMinuteStandby.IsActive = true;
            }
            EmployeeMinuteStandby.EmployeeId = model.EmployeeId;
            //EmployeeMinuteStandby.RankId = model.Employee.RankId;
            EmployeeMinuteStandby.StandbyRemarks1 = model.StandbyRemarks1;
            EmployeeMinuteStandby.DateFrom = model.DateFrom;
            EmployeeMinuteStandby.DateTo = model.DateTo;
            EmployeeMinuteStandby.StandbyRemarks2 = model.StandbyRemarks2;
            EmployeeMinuteStandby.StandbyRemarks3 = model.StandbyRemarks3;
            EmployeeMinuteStandby.StandbyRemarks4 = model.StandbyRemarks4;
            EmployeeMinuteStandby.StandbyRemarks5 = model.StandbyRemarks5;
            EmployeeMinuteStandby.StandbyRemarks6 = model.StandbyRemarks6;
            EmployeeMinuteStandby.StandbyRemarks7 = model.StandbyRemarks7;
            EmployeeMinuteStandby.StandbyRemarks8 = model.StandbyRemarks8;
            EmployeeMinuteStandby.StandbyRemarks9 = model.StandbyRemarks9;
            EmployeeMinuteStandby.StandbyRemarks10= model.StandbyRemarks10;

            //if (model.IsBackLog)
            //{
            //    EmployeeMinuteStandby.RankId = model.RankId;
            //    EmployeeMinuteStandby.TransferId = model.TransferId;
            //}


            EmployeeMinuteStandby.Employee = null;

            await employeeMinuteStandbyRepository.SaveAsync(EmployeeMinuteStandby);
            model.Id = EmployeeMinuteStandby.Id;
            return model;
        }

        public async Task<bool> DeleteEmployeeMinuteStandby(int id)
        {
            if (id <= 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            DashBoardMinuteStandby975 EmployeeMinuteStandby = await employeeMinuteStandbyRepository.FindOneAsync(x => x.Id == id);
            if (EmployeeMinuteStandby == null)
            {
                throw new InfinityNotFoundException("Employee Family Permission not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "DashBoardMinuteStandby975";
                bnLog.TableEntryForm = "Employee UNM Deferment";
                bnLog.PreviousValue = "Id: " + EmployeeMinuteStandby.Id;
                if (EmployeeMinuteStandby.EmployeeId > 0)
                {
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", EmployeeMinuteStandby.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)emp).PNo;
                }
                if (EmployeeMinuteStandby.RankId > 0)
                {
                    var prevrank = employeeService.GetDynamicTableInfoById("Rank", "RankId", EmployeeMinuteStandby.RankId ?? 0);
                    bnLog.PreviousValue += ", Rank: " + ((dynamic)prevrank).ShortName;
                }
                bnLog.PreviousValue += ", Remarks: " + EmployeeMinuteStandby.StandbyRemarks1 + EmployeeMinuteStandby.DateFrom.ToString("dd/MM/yyyy") + EmployeeMinuteStandby.DateTo?.ToString("dd/MM/yyyy") + EmployeeMinuteStandby.StandbyRemarks2 + EmployeeMinuteStandby.StandbyRemarks3 + EmployeeMinuteStandby.StandbyRemarks4 + EmployeeMinuteStandby.StandbyRemarks5 + EmployeeMinuteStandby.StandbyRemarks6 + EmployeeMinuteStandby.StandbyRemarks7 + EmployeeMinuteStandby.StandbyRemarks8 + EmployeeMinuteStandby.StandbyRemarks9 + EmployeeMinuteStandby.StandbyRemarks10;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await employeeMinuteStandbyRepository.DeleteAsync(EmployeeMinuteStandby);
            }
        }
    }
}