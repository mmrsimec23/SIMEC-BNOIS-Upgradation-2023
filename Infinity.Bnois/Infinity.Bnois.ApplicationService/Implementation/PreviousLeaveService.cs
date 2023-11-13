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
    public class PreviousLeaveService : IPreviousLeaveService
    {
        private readonly IBnoisRepository<PreviousLeave> PreviousLeaveRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public PreviousLeaveService(IBnoisRepository<PreviousLeave> PreviousLeaveRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.PreviousLeaveRepository = PreviousLeaveRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public async Task<PreviousLeaveModel> GetPreviousLeave(int previousLeaveId)
        {
            if (previousLeaveId <= 0)
            {
                return new PreviousLeaveModel();
            }
            PreviousLeave previousLeave = await PreviousLeaveRepository.FindOneAsync(x => x.PreviousLeaveId == previousLeaveId);
            if (previousLeave == null)
            {
                return new PreviousLeaveModel();
            }

            PreviousLeaveModel model = ObjectConverter<PreviousLeave, PreviousLeaveModel>.Convert(previousLeave);
            return model;
        }

        public List<PreviousLeaveModel> GetPreviousLeaves(int employeeId)
        {
            List<PreviousLeave> previousLeaves = PreviousLeaveRepository.FilterWithInclude(x => x.EmployeeId == employeeId, "LeaveType").ToList();
            List<PreviousLeaveModel> models = ObjectConverter<PreviousLeave, PreviousLeaveModel>.ConvertList(previousLeaves.ToList()).ToList();
            return models;
        }

        public async Task<PreviousLeaveModel> SavePreviousLeave(int previousLeaveId, PreviousLeaveModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Officer Sports data missing!");
            }

            PreviousLeave previousLeave = ObjectConverter<PreviousLeaveModel, PreviousLeave>.Convert(model);
            string userId= ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            if (previousLeaveId > 0)
            {
                previousLeave = await PreviousLeaveRepository.FindOneAsync(x => x.PreviousLeaveId == previousLeaveId);
                if (previousLeave == null)
                {
                    throw new InfinityNotFoundException("Leave Not found !");
                }

                previousLeave.ModifiedDate = DateTime.Now;
                previousLeave.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PreviousLeave";
                bnLog.TableEntryForm = "Employee Previous Leave";
                bnLog.PreviousValue = "Id: " + model.PreviousLeaveId;
                bnLog.UpdatedValue = "Id: " + model.PreviousLeaveId;
                int bnoisUpdateCount = 0;
                if (previousLeave.EmployeeId != model.EmployeeId)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", previousLeave.EmployeeId??0);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId??0);
                    bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                    bnLog.UpdatedValue += ", Name: " + ((dynamic)emp).PNo + "_" + ((dynamic)emp).FullNameEng;
                    bnoisUpdateCount += 1;
                }
                if (previousLeave.Year != model.Year)
                {
                    bnLog.PreviousValue += ", Year: " + previousLeave.Year;
                    bnLog.UpdatedValue += ", Year: " + model.Year;
                    bnoisUpdateCount += 1;
                }
                if (previousLeave.LeaveTypeId != model.LeaveTypeId)
                {
                    if (previousLeave.LeaveTypeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("LeaveType", "LeaveTypeId", previousLeave.LeaveTypeId ?? 0);
                        bnLog.PreviousValue += ", Leave Type: " + ((dynamic)prev).TypeName;
                    }
                    if (model.LeaveTypeId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("LeaveType", "LeaveTypeId", model.LeaveTypeId ?? 0);
                        bnLog.UpdatedValue += ", Leave Type: " + ((dynamic)newv).TypeName;
                    }
                }
                if (previousLeave.FromDate != model.FromDate)
                {
                    bnLog.PreviousValue += ", From Date: " + previousLeave.FromDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", From Date: " + model.FromDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (previousLeave.ToDate != model.ToDate)
                {
                    bnLog.PreviousValue += ", To Date: " + previousLeave.ToDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", To Date: " + model.ToDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (previousLeave.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + previousLeave.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                    bnoisUpdateCount += 1;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
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
                previousLeave.EmployeeId = model.EmployeeId;
                previousLeave.CreatedBy = userId;
                previousLeave.CreatedDate = DateTime.Now;
                previousLeave.IsActive = true;
            }

            previousLeave.LeaveTypeId = model.LeaveTypeId;
            previousLeave.Year = model.Year;
            previousLeave.FromDate = model.FromDate;
            previousLeave.ToDate = model.ToDate;
            previousLeave.Remarks = model.Remarks;
            await PreviousLeaveRepository.SaveAsync(previousLeave);
            model.PreviousLeaveId = previousLeave.PreviousLeaveId;
            return model;
        }


        public async Task<bool> DeletePreviousLeave(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            PreviousLeave previousLeave = await PreviousLeaveRepository.FindOneAsync(x => x.PreviousLeaveId == id);
            if (previousLeave == null)
            {
                throw new InfinityNotFoundException("Leave not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PreviousLeave";
                bnLog.TableEntryForm = "Employee Previous Leave";
                bnLog.PreviousValue = "Id: " + previousLeave.PreviousLeaveId;

                var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", previousLeave.EmployeeId??0);
                bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                bnLog.PreviousValue += ", Year: " + previousLeave.Year;
                if (previousLeave.LeaveTypeId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("LeaveType", "LeaveTypeId", previousLeave.LeaveTypeId??0);
                    bnLog.PreviousValue += ", Leave Type: " + ((dynamic)prev).TypeName;
                }
                bnLog.PreviousValue += ", From Date: " + previousLeave.FromDate?.ToString("dd/MM/yyyy") + ", To Date: " + previousLeave.ToDate?.ToString("dd/MM/yyyy") + ", Remarks: " + previousLeave.Remarks;

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await PreviousLeaveRepository.DeleteAsync(previousLeave);
            }
        }
    }
}
