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
    public class AchievementService : IAchievementService
    {
        private readonly IBnoisRepository<Achievement> achievementRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public AchievementService(IBnoisRepository<Achievement> achievementRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.achievementRepository = achievementRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<AchievementModel> GetAchievements(int ps, int pn, string qs, out int total)
        {
            IQueryable<Achievement> achievements = achievementRepository.FilterWithInclude(x => x.IsActive
                && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee", "Employee1","Commendation", "Office");
            total = achievements.Count();
            achievements = achievements.OrderByDescending(x => x.AchievementId).Skip((pn - 1) * ps).Take(ps);
            List<AchievementModel> models = ObjectConverter<Achievement, AchievementModel>.ConvertList(achievements.ToList()).ToList();

            models = models.Select(x =>
            {
                x.TypeName = Enum.GetName(typeof(AchievementComType), x.Type);
                x.GivenByTypeName = Enum.GetName(typeof(GivenByType), x.GivenByType);
                return x;
            }).ToList();

            return models;
        }

        public async Task<AchievementModel> GetAchievement(int id)
        {
            if (id <= 0)
            {
                return new AchievementModel();
            }
            Achievement achievement = await achievementRepository.FindOneAsync(x => x.AchievementId == id, new List<string> { "Employee","Employee.Rank","Employee.Batch","Employee1", "Employee1.Rank", "Employee1.Batch" });
            if (achievement == null)
            {
                throw new InfinityNotFoundException("Achievement not found");
            }
            AchievementModel model = ObjectConverter<Achievement, AchievementModel>.Convert(achievement);
            return model;
        }

    
        public async Task<AchievementModel> SaveAchievement(int id, AchievementModel model)
        {

            if (model == null)
            {
                throw new InfinityArgumentMissingException("Achievement  data missing");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            Achievement achievement = ObjectConverter<AchievementModel, Achievement>.Convert(model);
            if (id > 0)
            {
                achievement = await achievementRepository.FindOneAsync(x => x.AchievementId == id);
                if (achievement == null)
                {
                    throw new InfinityNotFoundException("Achievement not found !");
                }

                achievement.ModifiedDate = DateTime.Now;
                achievement.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Achievement";
                bnLog.TableEntryForm = "Personal Achievement";
                bnLog.PreviousValue = "Id: " + model.AchievementId;
                bnLog.UpdatedValue = "Id: " + model.AchievementId;
                if (achievement.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", achievement.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)emp).PNo;
                    
                }
                if (achievement.RankId != model.RankId)
                {
                    if (achievement.RankId > 0)
                    {
                        var prevrank = employeeService.GetDynamicTableInfoById("Rank", "RankId", achievement.RankId ?? 0);
                        bnLog.PreviousValue += ", Rank: " + ((dynamic)prevrank).ShortName;
                    }
                    if (model.RankId > 0)
                    {
                        var rank = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.RankId ?? 0);
                        bnLog.UpdatedValue += ", Rank: " + ((dynamic)rank).ShortName;
                    }
                }
                if (achievement.IsBackLog != model.IsBackLog)
                {
                    bnLog.PreviousValue += ", Is Back Log: " + achievement.IsBackLog;
                    bnLog.UpdatedValue += ", Is Back Log: " + model.IsBackLog;
                }
                if (achievement.TransferId != model.TransferId)
                {
                    if (achievement.TransferId > 0)
                    {
                        var prevTransfer = employeeService.GetDynamicTableInfoById("vwTransfer", "TransferId", achievement.TransferId ?? 0);
                        bnLog.PreviousValue += ", Born/Attach/Appointment: " + ((dynamic)prevTransfer).BornOffice + '/' + ((dynamic)prevTransfer).CurrentAttach + '/' + ((dynamic)prevTransfer).Appointment;
                    }
                    if (model.TransferId > 0)
                    {
                        var newTransfer = employeeService.GetDynamicTableInfoById("vwTransfer", "TransferId", model.TransferId ?? 0);
                        bnLog.UpdatedValue += ", Born/Attach/Appointment: " + ((dynamic)newTransfer).BornOffice + '/' + ((dynamic)newTransfer).CurrentAttach + '/' + ((dynamic)newTransfer).Appointment;
                    }
                }
                
                if (achievement.Type != model.Type)
                {
                    bnLog.PreviousValue += ", Type: " + (achievement.Type == 1 ? "Commendation" : achievement.Type == 2 ? "Appreciation" : achievement.Type == 3 ? "Notable_Achievement" : "" );
                    bnLog.UpdatedValue += ", Type: " + (model.Type == 1 ? "Commendation" : model.Type == 2 ? "Appreciation" : model.Type == 3 ? "Notable_Achievement" : "");
                }

                if (achievement.CommendationId != model.CommendationId)
                {
                    if (achievement.CommendationId > 0)
                    {
                        var prevcomm = employeeService.GetDynamicTableInfoById("Commendation", "CommendationId", achievement.CommendationId ?? 0);
                        bnLog.PreviousValue += achievement.Type == 1 ? ", Commendation Name: " + ((dynamic)prevcomm).Name : achievement.Type == 2 ? ", Appreciation Name: " + ((dynamic)prevcomm).Name : "";
                    }
                    if (model.CommendationId > 0)
                    {
                        var newcomm = employeeService.GetDynamicTableInfoById("Commendation", "CommendationId", model.CommendationId ?? 0);
                        bnLog.UpdatedValue += model.Type == 1 ? ", Commendation Name: " + ((dynamic)newcomm).Name : model.Type == 2 ? ", Appreciation Name: " + ((dynamic)newcomm).Name : "";
                    }
                }
                if (achievement.Date != model.Date)
                {
                    bnLog.PreviousValue += ", Date: " + achievement.Date.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Date: " + model.Date?.ToString("dd/MM/yyyy");
                }
                if (achievement.CommAppType != model.CommAppType)
                {
                    bnLog.PreviousValue += ", Achievement Type (For Report): " + achievement.CommAppType;
                    bnLog.UpdatedValue += ", Achievement Type (For Report): " + model.CommAppType;
                }
                if (achievement.Reason != model.Reason)
                {
                    bnLog.PreviousValue += ", Reason (For Promotion Report): " + achievement.Reason;
                    bnLog.UpdatedValue += ", Reason (For Promotion Report): " + model.Reason;
                }
                if (achievement.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Description: " + achievement.Remarks;
                    bnLog.UpdatedValue += ", Description: " + model.Remarks;
                }

                if (achievement.GivenByType != model.GivenByType)
                {
                    bnLog.PreviousValue += ", Given By: " + (achievement.GivenByType == 1 ? "BN" : achievement.GivenByType == 2 ? "Others" : "");
                    bnLog.UpdatedValue += ", Given By: " + (model.GivenByType == 1 ? "BN" : model.GivenByType == 2 ? "Others" : "");
                }
                if (achievement.GivenEmployeeId != model.GivenEmployeeId)
                {
                    if (achievement.GivenEmployeeId > 0)
                    {
                        var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", achievement.GivenEmployeeId ?? 0);
                        bnLog.PreviousValue += ", Given By Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                    }
                    if (model.GivenEmployeeId > 0)
                    {
                        var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.GivenEmployeeId ?? 0);
                        bnLog.UpdatedValue += ", Given By Name: " + ((dynamic)emp).PNo + "_" + ((dynamic)emp).FullNameEng;
                    }
                }

                if (achievement.GivenTransferId != model.GivenTransferId)
                {
                    if (achievement.GivenTransferId > 0)
                    {
                        var prevTransfer = employeeService.GetDynamicTableInfoById("vwTransfer", "TransferId", achievement.TransferId ?? 0);
                        bnLog.PreviousValue += ", Given By Born/Attach/Appointment: " + ((dynamic)prevTransfer).BornOffice + '/' + ((dynamic)prevTransfer).CurrentAttach + '/' + ((dynamic)prevTransfer).Appointment;
                    }
                    if (achievement.GivenTransferId > 0)
                    {
                        var newTransfer = employeeService.GetDynamicTableInfoById("vwTransfer", "TransferId", model.TransferId ?? 0);
                        bnLog.UpdatedValue += ", Given By Born/Attach/Appointment: " + ((dynamic)newTransfer).BornOffice + '/' + ((dynamic)newTransfer).CurrentAttach + '/' + ((dynamic)newTransfer).Appointment;
                    }
                }
                if (achievement.PatternId != model.PatternId)
                {
                    if (achievement.PatternId > 0)
                    {
                        var prevOffice = employeeService.GetDynamicTableInfoById("Office", "OfficeId", achievement.PatternId ?? 0);
                        bnLog.PreviousValue += ", Pattern: " + ((dynamic)prevOffice).ShortName;
                    }
                    if (model.PatternId > 0)
                    {
                        var newOffice = employeeService.GetDynamicTableInfoById("Office", "OfficeId", model.PatternId ?? 0);
                        bnLog.UpdatedValue += ", Pattern: " + ((dynamic)newOffice).ShortName;
                    }
                }
                if (achievement.OfficeId != model.OfficeId)
                {
                    if (achievement.OfficeId > 0)
                    {
                        var prevOffice = employeeService.GetDynamicTableInfoById("Office", "OfficeId", achievement.OfficeId ?? 0);
                        bnLog.PreviousValue += achievement.GivenByType == 1 ? ", Office: " + ((dynamic)prevOffice).ShortName : achievement.GivenByType == 2 ? ", Org. Name: " + ((dynamic)prevOffice).ShortName : "" ;
                    }
                    if (model.OfficeId > 0)
                    {
                        var newOffice = employeeService.GetDynamicTableInfoById("Office", "OfficeId", model.OfficeId ?? 0);
                        bnLog.UpdatedValue += model.GivenByType == 1 ? ", Office: " + ((dynamic)newOffice).ShortName : model.GivenByType == 2 ? ", Org. Name: " + ((dynamic)newOffice).ShortName : "";
                    }
                }
                
                
                if (achievement.OfficerName != model.OfficerName)
                {
                    bnLog.PreviousValue += ", Officer Name: " + achievement.OfficerName;
                    bnLog.UpdatedValue += ", Officer Name: " + model.OfficerName;

                }
                if (achievement.OfficerDesignation != model.OfficerDesignation)
                {
                    bnLog.PreviousValue += achievement.GivenByType == 1 ? ", Appointment: " + achievement.OfficerDesignation : achievement.GivenByType == 2 ? ", Officer Designation: " + achievement.OfficerDesignation : "";
                    bnLog.UpdatedValue += model.GivenByType == 1 ? ", Appointment: " + model.OfficerDesignation : model.GivenByType == 2 ? ", Officer Designation: " + model.OfficerDesignation : "";

                }
                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (achievement.EmployeeId != model.EmployeeId || achievement.RankId != model.RankId || achievement.OfficerDesignation != model.OfficerDesignation || achievement.OfficerName != model.OfficerName || achievement.PatternId != model.PatternId || achievement.OfficeId != model.OfficeId || achievement.GivenTransferId != model.GivenTransferId || achievement.GivenEmployeeId != model.GivenEmployeeId || achievement.GivenByType != model.GivenByType || achievement.Remarks != model.Remarks || achievement.Reason != model.Reason || achievement.CommAppType != model.CommAppType || achievement.Date != model.Date || achievement.CommendationId != model.CommendationId || achievement.Type != model.Type || achievement.TransferId != model.TransferId || achievement.IsBackLog != model.IsBackLog)
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
                achievement.IsActive = true;
                achievement.CreatedDate = DateTime.Now;
                achievement.CreatedBy = userId;
            }
            achievement.EmployeeId = model.EmployeeId;
            achievement.GivenEmployeeId = model.GivenEmployeeId;
            achievement.GivenTransferId = model.GivenTransferId;
            achievement.CommendationId = model.CommendationId;
            achievement.PatternId = model.PatternId;
            achievement.OfficeId = model.OfficeId;
            achievement.Type = model.Type;
            achievement.Date =model.Date ?? achievement.Date;
            achievement.Remarks = model.Remarks;
            achievement.CommAppType = model.CommAppType;
            achievement.Reason = model.Reason;
            achievement.GivenByType = model.GivenByType;
            achievement.OfficerName = model.OfficerName;
            achievement.OfficerDesignation = model.OfficerDesignation;
            achievement.FileName = model.FileName;

            achievement.IsBackLog = model.IsBackLog;
            achievement.RankId = model.Employee.RankId;
            achievement.TransferId = model.Employee.TransferId;

            if (model.IsBackLog)
            {
                
                achievement.RankId = model.RankId;
                achievement.TransferId = model.TransferId;
            }

            achievement.Employee = null;
            achievement.Employee1 = null;
            await achievementRepository.SaveAsync(achievement);
            model.AchievementId = achievement.AchievementId;
            return model;
        }


        public async Task<bool> DeleteAchievement(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Achievement achievement = await achievementRepository.FindOneAsync(x => x.AchievementId == id);
            if (achievement == null)
            {
                throw new InfinityNotFoundException("Achievement not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Achievement";
                bnLog.TableEntryForm = "Personal Achievement";
                bnLog.PreviousValue = "Id: " + achievement.AchievementId;
                if (achievement.EmployeeId > 0)
                    {
                        var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", achievement.EmployeeId);
                        bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    }
                    
                if (achievement.RankId > 0)
                {
                    var prevrank = employeeService.GetDynamicTableInfoById("Rank", "RankId", achievement.RankId ?? 0);
                    bnLog.PreviousValue += ", Rank: " + ((dynamic)prevrank).ShortName;
                }
                bnLog.PreviousValue += ", Is Back Log: " + achievement.IsBackLog;
                
                if (achievement.TransferId > 0)
                {
                    var prevTransfer = employeeService.GetDynamicTableInfoById("vwTransfer", "TransferId", achievement.TransferId ?? 0);
                    bnLog.PreviousValue += ", Born/Attach/Appointment: " + ((dynamic)prevTransfer).BornOffice + '/' + ((dynamic)prevTransfer).CurrentAttach + '/' + ((dynamic)prevTransfer).Appointment;
                }
                bnLog.PreviousValue += ", Type: " + (achievement.Type == 1 ? "Commendation" : achievement.Type == 2 ? "Appreciation" : achievement.Type == 3 ? "Notable_Achievement" : "");
                   

                if (achievement.CommendationId > 0)
                {
                    var prevcomm = employeeService.GetDynamicTableInfoById("Commendation", "CommendationId", achievement.CommendationId ?? 0);
                    bnLog.PreviousValue += achievement.Type == 1 ? ", Commendation Name: " + ((dynamic)prevcomm).Name : achievement.Type == 2 ? ", Appreciation Name: " + ((dynamic)prevcomm).Name : "";
                }
                bnLog.PreviousValue += ", Date: " + achievement.Date.ToString("dd/MM/yyyy");
                bnLog.PreviousValue += ", Achievement Type (For Report): " + achievement.CommAppType;
                bnLog.PreviousValue += ", Reason (For Promotion Report): " + achievement.Reason;
                bnLog.PreviousValue += ", Description: " + achievement.Remarks;
                bnLog.PreviousValue += ", Given By: " + (achievement.GivenByType == 1 ? "BN" : achievement.GivenByType == 2 ? "Others" : "");
                if (achievement.GivenEmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", achievement.GivenEmployeeId ?? 0);
                    bnLog.PreviousValue += ", Given By Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                }
                if (achievement.GivenTransferId > 0)
                {
                    var prevTransfer = employeeService.GetDynamicTableInfoById("vwTransfer", "TransferId", achievement.TransferId ?? 0);
                    bnLog.PreviousValue += ", Given By Born/Attach/Appointment: " + ((dynamic)prevTransfer).BornOffice + '/' + ((dynamic)prevTransfer).CurrentAttach + '/' + ((dynamic)prevTransfer).Appointment;
                }
                if (achievement.PatternId > 0)
                {
                    var prevOffice = employeeService.GetDynamicTableInfoById("Office", "OfficeId", achievement.PatternId ?? 0);
                    bnLog.PreviousValue += ", Pattern: " + ((dynamic)prevOffice).ShortName;
                }
                if (achievement.OfficeId > 0)
                {
                    var prevOffice = employeeService.GetDynamicTableInfoById("Office", "OfficeId", achievement.OfficeId ?? 0);
                    bnLog.PreviousValue += achievement.GivenByType == 1 ? ", Office: " + ((dynamic)prevOffice).ShortName : achievement.GivenByType == 2 ? ", Org. Name: " + ((dynamic)prevOffice).ShortName : "";
                }
                bnLog.PreviousValue += ", Officer Name: " + achievement.OfficerName;
                bnLog.PreviousValue += achievement.GivenByType == 1 ? ", Appointment: " + achievement.OfficerDesignation : achievement.GivenByType == 2 ? ", Officer Designation: " + achievement.OfficerDesignation : "";

                bnLog.UpdatedValue = "This Record has been Deleted!";
                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                return await achievementRepository.DeleteAsync(achievement);
            }
        }

        public List<SelectModel> GetGivenByTypeSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(GivenByType)).Cast<GivenByType>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }

        public List<SelectModel> GetAchievementComTypeSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(AchievementComType)).Cast<AchievementComType>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }


      
    }
}
