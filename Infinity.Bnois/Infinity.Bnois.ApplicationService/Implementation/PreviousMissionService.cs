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
    public class PreviousMissionService : IPreviousMissionService
    {
        private readonly IBnoisRepository<PreviousMission> previousMissionRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;


        public PreviousMissionService(IBnoisRepository<PreviousMission> previousMissionRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.previousMissionRepository = previousMissionRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;

        }

        public async Task<PreviousMissionModel> GetPreviousMission(int previousMissionId)
        {
            if (previousMissionId <= 0)
            {
                return new PreviousMissionModel();
            }
            PreviousMission previousMission = await previousMissionRepository.FindOneAsync(x => x.PreviousMissionId == previousMissionId);
            if (previousMission == null)
            {
                return new PreviousMissionModel();
            }

            PreviousMissionModel model = ObjectConverter<PreviousMission, PreviousMissionModel>.Convert(previousMission);
            return model;
        }

        public List<PreviousMissionModel> GetPreviousMissions(int employeeId)
        {
            List<PreviousMission> previousMissions = previousMissionRepository.FilterWithInclude(x => x.EmployeeId == employeeId, "Country").ToList();
            List<PreviousMissionModel> models = ObjectConverter<PreviousMission, PreviousMissionModel>.ConvertList(previousMissions.ToList()).ToList();
            return models;
        }

        public async Task<PreviousMissionModel> SavePreviousMission(int previousMissionId, PreviousMissionModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Officer Mission data missing!");
            }

            PreviousMission previousMission = ObjectConverter<PreviousMissionModel, PreviousMission>.Convert(model);
            string userId= ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            if (previousMissionId > 0)
            {
                previousMission = await previousMissionRepository.FindOneAsync(x => x.PreviousMissionId == previousMissionId);
                if (previousMission == null)
                {
                    throw new InfinityNotFoundException("Mission Not found !");
                }

                previousMission.ModifiedDate = DateTime.Now;
                previousMission.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PreviousMission";
                bnLog.TableEntryForm = "Employee Previous Mission";
                bnLog.PreviousValue = "Id: " + model.PreviousMissionId;
                bnLog.UpdatedValue = "Id: " + model.PreviousMissionId;
                int bnoisUpdateCount = 0;
                if (previousMission.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", previousMission.EmployeeId ?? 0);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId ?? 0);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)emp).PNo;
                    //bnoisUpdateCount += 1;
                }
                if (previousMission.Title != model.Title)
                {
                    bnLog.PreviousValue += ", Title: " + previousMission.Title;
                    bnLog.UpdatedValue += ", Title: " + model.Title;
                    bnoisUpdateCount += 1;
                }
                if (previousMission.CountryId != model.CountryId)
                {
                    if (previousMission.CountryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Country", "CountryId", previousMission.CountryId ?? 0);
                        bnLog.PreviousValue += ", Leave Type: " + ((dynamic)prev).FullName;
                    }
                    if (model.CountryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Country", "CountryId", model.CountryId ?? 0);
                        bnLog.UpdatedValue += ", Leave Type: " + ((dynamic)newv).FullName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (previousMission.FromDate != model.FromDate)
                {
                    bnLog.PreviousValue += ", From Date: " + previousMission.FromDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", From Date: " + model.FromDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (previousMission.ToDate != model.ToDate)
                {
                    bnLog.PreviousValue += ", To Date: " + previousMission.ToDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", To Date: " + model.ToDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (previousMission.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + previousMission.Remarks;
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
                previousMission.EmployeeId = model.EmployeeId;
                previousMission.CreatedBy = userId;
                previousMission.CreatedDate = DateTime.Now;
                previousMission.IsActive = true;
            }

            previousMission.CountryId = model.CountryId;
            previousMission.Title = model.Title;
            previousMission.FromDate = model.FromDate;
            previousMission.ToDate = model.ToDate;
            previousMission.Remarks = model.Remarks;
            await previousMissionRepository.SaveAsync(previousMission);
            model.PreviousMissionId = previousMission.PreviousMissionId;
            return model;
        }


        public async Task<bool> DeletePreviousMission(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            PreviousMission previousMission = await previousMissionRepository.FindOneAsync(x => x.PreviousMissionId == id);
            if (previousMission == null)
            {
                throw new InfinityNotFoundException("Mission not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PreviousMission";
                bnLog.TableEntryForm = "Employee Previous Mission";
                bnLog.PreviousValue = "Id: " + previousMission.PreviousMissionId;

                var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", previousMission.EmployeeId ?? 0);
                bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                bnLog.PreviousValue += ", Title: " + previousMission.Title;
                if (previousMission.CountryId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Country", "CountryId", previousMission.CountryId ?? 0);
                    bnLog.PreviousValue += ", Leave Type: " + ((dynamic)prev).FullName;
                }
                bnLog.PreviousValue += ", From Date: " + previousMission.FromDate?.ToString("dd/MM/yyyy") + ", To Date: " + previousMission.ToDate?.ToString("dd/MM/yyyy") + ", Remarks: " + previousMission.Remarks;

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await previousMissionRepository.DeleteAsync(previousMission);
            }
        }
    }
}
