using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.Api;
using Infinity.Bnois.Configuration;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class PreviousExperienceService : IPreviousExperienceService
    {
        private readonly IBnoisRepository<PreviousExperience> previousExperienceRepository;
        private readonly IBnoisRepository<LprCalculateInfo> lprCalculateInfoRepository;
        private readonly IBnoisRepository<EmployeeGeneral> employeeGeneralRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public PreviousExperienceService(IBnoisRepository<PreviousExperience> previousExperienceRepository, IBnoisRepository<LprCalculateInfo> lprCalculateInfoRepository, IBnoisRepository<EmployeeGeneral> employeeGeneralRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.previousExperienceRepository = previousExperienceRepository;
            this.lprCalculateInfoRepository = lprCalculateInfoRepository;
            this.employeeGeneralRepository = employeeGeneralRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public async Task<PreviousExperienceModel> GetPreviousExperience(int employeeId)
        {
            if (employeeId <= 0)
            {
                return new PreviousExperienceModel();
            }
            PreviousExperience previousExperience = await previousExperienceRepository.FindOneAsync(x => x.EmployeeId == employeeId,new List<string> {"Category","PreCommissionRank"});
            if (previousExperience == null)
            {
                return new PreviousExperienceModel();
            }
            PreviousExperienceModel model = ObjectConverter<PreviousExperience, PreviousExperienceModel>.Convert(previousExperience);
            return model;
        }

        public async Task<PreviousExperienceModel> SavePreviousExperience(int employeeId, PreviousExperienceModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Previous Experience data missing!");
            }

            PreviousExperience previousExperience = ObjectConverter<PreviousExperienceModel, PreviousExperience>.Convert(model);
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            if (previousExperience.PreviousExperienceId > 0)
            {
                previousExperience = await previousExperienceRepository.FindOneAsync(x => x.EmployeeId == employeeId);
                if (previousExperience == null)
                {
                    throw new InfinityNotFoundException("Previous Experience data not found!");
                }
                previousExperience.ModifiedDate = DateTime.Now;
                previousExperience.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PreviousExperience";
                bnLog.TableEntryForm = "Employee Pre Commission Service";
                bnLog.PreviousValue = "Id: " + model.PreviousExperienceId;
                bnLog.UpdatedValue = "Id: " + model.PreviousExperienceId;
                int bnoisUpdateCount = 0;
                if (previousExperience.EmployeeId != model.EmployeeId)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", previousExperience.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                    bnLog.UpdatedValue += ", Name: " + ((dynamic)emp).PNo + "_" + ((dynamic)emp).FullNameEng;
                    bnoisUpdateCount += 1;
                }
                if (previousExperience.PreCommissionRankId != model.PreCommissionRankId)
                {
                    if (previousExperience.PreCommissionRankId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("PreCommissionRank", "PreCommissionRankId", previousExperience.PreCommissionRankId??0);
                        bnLog.PreviousValue += ", Pre Commission Rank: " + ((dynamic)prev).Name;
                    }
                    if (model.PreCommissionRankId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("PreCommissionRankId", "PreCommissionRankId", model.PreCommissionRankId??0);
                        bnLog.UpdatedValue += ", Pre Commission Rank: " + ((dynamic)newv).Name;
                    }
                }
                if (previousExperience.ServiceNo != model.ServiceNo)
                {
                    bnLog.PreviousValue += ", Service No: " + previousExperience.ServiceNo;
                    bnLog.UpdatedValue += ", Service No: " + model.ServiceNo;
                    bnoisUpdateCount += 1;
                }
                if (previousExperience.JoiningDate != model.JoiningDate)
                {
                    bnLog.PreviousValue += ", Joining Date: " + previousExperience.JoiningDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Joining Date: " + model.JoiningDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (previousExperience.CategoryId != model.CategoryId)
                {
                    if (previousExperience.CategoryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Category", "CategoryId", previousExperience.CategoryId ?? 0);
                        bnLog.PreviousValue += ", Category: " + ((dynamic)prev).Name;
                    }
                    if (model.CategoryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("CategoryId", "CategoryId", model.CategoryId ?? 0);
                        bnLog.UpdatedValue += ", Category: " + ((dynamic)newv).Name;
                    }
                }
                if (previousExperience.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Description: " + previousExperience.Remarks;
                    bnLog.UpdatedValue += ", Description: " + model.Remarks;
                    bnoisUpdateCount += 1;
                }
                if (previousExperience.LeavingReason != model.LeavingReason)
                {
                    bnLog.PreviousValue += ", Reason for Leaving Service: " + previousExperience.LeavingReason;
                    bnLog.UpdatedValue += ", Reason for Leaving Service: " + model.LeavingReason;
                    bnoisUpdateCount += 1;
                }
                if (previousExperience.ISSB != model.ISSB)
                {
                    bnLog.PreviousValue += ", ISSB: " + (previousExperience.ISSB == 1 ? "Attained" : previousExperience.ISSB == 2 ? "Not Attained" : "-");
                    bnLog.UpdatedValue += ", ISSB: " + (model.ISSB == 1 ? "Attained" : model.ISSB == 2 ? "Not Attained" : "-");
                    bnoisUpdateCount += 1;
                }
                if (previousExperience.ISSBResult != model.ISSBResult)
                {
                    bnLog.PreviousValue += ", Result: " + (previousExperience.ISSBResult == 1 ? "Qualified" : previousExperience.ISSBResult == 2 ? "Not Qualified" : "");
                    bnLog.UpdatedValue += ", Result: " + (model.ISSBResult == 1 ? "Qualified" : model.ISSBResult == 2 ? "Not Qualified" : "");
                    bnoisUpdateCount += 1;
                }
                if (previousExperience.LeaveMonths != model.LeaveMonths)
                {
                    bnLog.PreviousValue += ", Leave Months: " + previousExperience.LeaveMonths;
                    bnLog.UpdatedValue += ", Leave Months: " + model.LeaveMonths;
                    bnoisUpdateCount += 1;
                }
                if (previousExperience.LeaveDays != model.LeaveDays)
                {
                    bnLog.PreviousValue += ", Leave Days: " + previousExperience.LeaveDays;
                    bnLog.UpdatedValue += ", Leave Days: " + model.LeaveDays;
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
                previousExperience.EmployeeId = employeeId;
                previousExperience.CreatedBy = userId;
                previousExperience.CreatedDate = DateTime.Now;
                previousExperience.Active = true;
            }

            previousExperience.PreCommissionRankId = model.PreCommissionRankId;
            previousExperience.CategoryId = model.CategoryId;
            previousExperience.ServiceNo = model.ServiceNo;
            previousExperience.JoiningDate = model.JoiningDate;
            previousExperience.LeavingReason = model.LeavingReason;
            previousExperience.LeaveMonths = model.LeaveMonths;
            previousExperience.LeaveDays = model.LeaveDays;
            previousExperience.Remarks = model.Remarks;
            previousExperience.ISSB = model.ISSB;
            if (model.ISSB==2)
            {
                previousExperience.ISSBResult = null;
            }
            else
            {
                previousExperience.ISSBResult = model.ISSBResult;
            }
            
            await previousExperienceRepository.SaveAsync(previousExperience);
            model.PreviousExperienceId = previousExperience.PreviousExperienceId;


            var lprCalculateInfo = await lprCalculateInfoRepository.FindOneAsync(x => x.EmployeeId == model.EmployeeId);
            if (lprCalculateInfo != null)
            {
                var employeeGeneral =
                    await employeeGeneralRepository.FindOneAsync(x => x.EmployeeId == lprCalculateInfo.EmployeeId);


                if (employeeGeneral.CategoryId == CodeValue.PromotedOfficer || employeeGeneral.CategoryId ==CodeValue.HonoraryOfficer)
                {
                    lprCalculateInfo.ModifiedDate = DateTime.Now;
                    lprCalculateInfo.ModifiedBy = model.ModifiedBy;

                    lprCalculateInfo.SailorDue = previousExperience.LeaveDays + (previousExperience.LeaveMonths * CodeValue.Days);
                    await lprCalculateInfoRepository.SaveAsync(lprCalculateInfo);
                }
               
            }


            return model;
        }
    }
}
