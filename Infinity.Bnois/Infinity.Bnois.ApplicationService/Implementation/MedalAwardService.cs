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
    public class MedalAwardService : IMedalAwardService
    {
        private readonly IBnoisRepository<MedalAward> medalAwardRepository;
	    private readonly IProcessRepository processRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public MedalAwardService(IBnoisRepository<MedalAward> medalAwardRepository, IProcessRepository processRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.medalAwardRepository = medalAwardRepository;
            this.processRepository = processRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<MedalAwardModel> GetMedalAwards(int ps, int pn, string qs, out int total)
        {
            IQueryable<MedalAward> medalAwards = medalAwardRepository.FilterWithInclude(x => x.IsActive
                && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs) ||x.Medal.NameEng == (qs) || x.Award.NameEng.Contains(qs) || x.Publication.Name == (qs)  || String.IsNullOrEmpty(qs)), "Employee", "Award", "Medal","Publication","PublicationCategory");
            total = medalAwards.Count();
            medalAwards = medalAwards.OrderByDescending(x => x.MedalAwardId).Skip((pn - 1) * ps).Take(ps);
            List<MedalAwardModel> models = ObjectConverter<MedalAward, MedalAwardModel>.ConvertList(medalAwards.ToList()).ToList();

            models = models.Select(x =>
            {
                x.TypeName = Enum.GetName(typeof(MedalAwardType), x.Type);
                return x;
            }).ToList();
            return models;
        }

        public async Task<MedalAwardModel> GetMedalAward(int id)
        {
            if (id <= 0)
            {
                return new MedalAwardModel();
            }
            MedalAward medalAward = await medalAwardRepository.FindOneAsync(x => x.MedalAwardId == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch" });
            if (medalAward == null)
            {
                throw new InfinityNotFoundException("Medal, Award & Publication not found");
            }
            MedalAwardModel model = ObjectConverter<MedalAward, MedalAwardModel>.Convert(medalAward);
            return model;
        }

    
        public async Task<MedalAwardModel> SaveMedalAward(int id, MedalAwardModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Medal, Award & Publication  data missing");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            MedalAward medalAward = ObjectConverter<MedalAwardModel, MedalAward>.Convert(model);
            if (id > 0)
            {
                medalAward = await medalAwardRepository.FindOneAsync(x => x.MedalAwardId == id);
                //medalAward = medalAwardRepository.FindOne(x => x.MedalAwardId == id, new List<string> { "Employee", "Award", "Medal","Publication","PublicationCategory" });
                if (medalAward == null)
                {
                    throw new InfinityNotFoundException("Medal, Award & Publication not found !");
                }

                medalAward.ModifiedDate = DateTime.Now;
                medalAward.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "MedalAward";
                bnLog.TableEntryForm = "Medal Award";
                bnLog.PreviousValue = "Id: " + model.MedalAwardId;
                bnLog.UpdatedValue = "Id: " + model.MedalAwardId;
                if (medalAward.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevEmployee = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", medalAward.EmployeeId);
                    var newEmployee = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevEmployee).PNo;
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)newEmployee).PNo;
                }
                if (medalAward.IsBackLog != model.IsBackLog)
                {
                    bnLog.PreviousValue += ", Back Log: " + medalAward.IsBackLog;
                    bnLog.UpdatedValue += ", Back Log: " + model.IsBackLog;
                }
                if (medalAward.RankId != model.RankId)
                {
                    var prevRank = employeeService.GetDynamicTableInfoById("Rank", "RankId", medalAward.RankId ?? 0);
                    var newRank = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.RankId ?? 0);
                    bnLog.PreviousValue += ", Rank: " + ((dynamic)prevRank).ShortName;
                    bnLog.UpdatedValue += ", Rank: " + ((dynamic)newRank).ShortName;
                }
                if (medalAward.TransferId != model.TransferId)
                {
                    var prevTransfer = employeeService.GetDynamicTableInfoById("vwTransfer", "TransferId", medalAward.TransferId ?? 0);
                    var newTransfer = employeeService.GetDynamicTableInfoById("vwTransfer", "TransferId", model.TransferId ?? 0);
                    bnLog.PreviousValue += ", Born/Attach/Appointment: " + ((dynamic)prevTransfer).BornOffice + '/' + ((dynamic)prevTransfer).CurrentAttach + '/' + ((dynamic)prevTransfer).Appointment;
                    bnLog.UpdatedValue += ", Born/Attach/Appointment: " + ((dynamic)newTransfer).BornOffice + '/' + ((dynamic)newTransfer).CurrentAttach + '/' + ((dynamic)newTransfer).Appointment;
                }

                if (medalAward.Type != model.Type)
                {
                    bnLog.PreviousValue += ", Type: " + (medalAward.Type == 1 ? "Award" : medalAward.Type == 2 ? "Medal" : medalAward.Type == 3 ? "Publication" : "");
                    bnLog.UpdatedValue += ", Type: " + (model.Type == 1 ? "Award" : model.Type == 2 ? "Medal" : model.Type == 3 ? "Publication" : "");
                }
                if (medalAward.MedalId != model.MedalId)
                {
                    if (medalAward.MedalId > 0)
                    {
                        var prevMedal = employeeService.GetDynamicTableInfoById("Medal", "MedalId", medalAward.MedalId ?? 0);
                        bnLog.PreviousValue += ", Medal: " + ((dynamic)prevMedal).NameEng;
                    }
                    if(model.MedalId > 0)
                    {
                        var newMedal = employeeService.GetDynamicTableInfoById("Medal", "MedalId", model.MedalId ?? 0);
                        bnLog.UpdatedValue += ", Medal: " + ((dynamic)newMedal).NameEng;
                    }
                    
                }
                if (medalAward.AwardId != model.AwardId)
                {
                    if (medalAward.AwardId > 0)
                    {
                        var prevAward = employeeService.GetDynamicTableInfoById("Award", "AwardId", medalAward.AwardId ?? 0);
                        bnLog.PreviousValue += ", Award: " + ((dynamic)prevAward).NameEng;
                    }
                    if (model.AwardId > 0)
                    { 
                        var newAward = employeeService.GetDynamicTableInfoById("Award", "AwardId", model.AwardId ?? 0);
                        bnLog.UpdatedValue += ", Award: " + ((dynamic)newAward).NameEng;
                    }
                }
                if (medalAward.PublicationCategoryId != model.PublicationCategoryId)
                {
                    if (medalAward.PublicationCategoryId > 0)
                    {
                        var prevPublicationCategory = employeeService.GetDynamicTableInfoById("PublicationCategory", "PublicationCategoryId", medalAward.PublicationCategoryId ?? 0);
                        bnLog.PreviousValue += ", Publication Category: " + ((dynamic)prevPublicationCategory).Name;
                    }
                    if (model.PublicationCategoryId > 0)
                    {
                        var newPublicationCategory = employeeService.GetDynamicTableInfoById("PublicationCategory", "PublicationCategoryId", model.PublicationCategoryId ?? 0);
                        bnLog.UpdatedValue += ", Publication Category: " + ((dynamic)newPublicationCategory).Name;
                    }
                }
                if (medalAward.PublicationId != model.PublicationId)
                {
                    if (medalAward.PublicationId > 0)
                    {
                        var prevPublication = employeeService.GetDynamicTableInfoById("Publication", "PublicationId", medalAward.PublicationId ?? 0);
                        bnLog.PreviousValue += ", Publication: " + ((dynamic)prevPublication).Name; 
                    }
                    if (model.PublicationId > 0)
                    {
                        var newPublication = employeeService.GetDynamicTableInfoById("Publication", "PublicationId", model.PublicationId ?? 0);
                        bnLog.UpdatedValue += ", Publication: " + ((dynamic)newPublication).Name;
                    }
                }
                
                if (medalAward.Date != model.Date)
                {
                    bnLog.PreviousValue += ", Date: " + medalAward.Date.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Date: " + model.Date?.ToString("dd/MM/yyyy");
                }
                if (medalAward.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + medalAward.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }
                if (medalAward.FileName != model.FileName)
                {
                    bnLog.PreviousValue += ", File: " + medalAward.FileName;
                    bnLog.UpdatedValue += ", File: " + model.FileName;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (medalAward.EmployeeId != model.EmployeeId || medalAward.MedalId != model.MedalId || medalAward.AwardId != model.AwardId || medalAward.PublicationId != model.PublicationId
                    || medalAward.PublicationCategoryId != model.PublicationCategoryId || medalAward.Type != model.Type || medalAward.IsBackLog != model.IsBackLog || medalAward.RankId != model.RankId
                    || medalAward.TransferId != model.TransferId || medalAward.Date != model.Date || medalAward.Remarks != model.Remarks || medalAward.FileName != model.FileName)
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
                medalAward.IsActive = true;
                medalAward.CreatedDate = DateTime.Now;
                medalAward.CreatedBy = userId;
            }
            medalAward.EmployeeId = model.EmployeeId;
            medalAward.AwardId = model.AwardId;
            medalAward.MedalId = model.MedalId;
            medalAward.PublicationId = model.PublicationId;
            medalAward.PublicationCategoryId = model.PublicationCategoryId;
            medalAward.Type = model.Type;
            medalAward.Date =model.Date ?? medalAward.Date;
            medalAward.Remarks = model.Remarks;
            medalAward.Employee = null;
            medalAward.IsBackLog = model.IsBackLog;
            medalAward.FileName = model.FileName;

            medalAward.RankId = model.Employee.RankId; ;
            medalAward.TransferId = model.Employee.TransferId;
            if (model.IsBackLog)
            {
                medalAward.RankId = model.RankId;
                medalAward.TransferId = model.TransferId;
                await medalAwardRepository.SaveAsync(medalAward);
                model.MedalAwardId = medalAward.MedalAwardId;
                return model;
            }

            await medalAwardRepository.SaveAsync(medalAward);
            model.MedalAwardId = medalAward.MedalAwardId;

	        await processRepository.UpdateNamingConvention(medalAward.EmployeeId);
			return model;
        }


        public async Task<bool> DeleteMedalAward(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            MedalAward medalAward = medalAwardRepository.FindOne(x => x.MedalAwardId == id, new List<string> { "Employee", "Award", "Medal", "Publication", "PublicationCategory" });
            if (medalAward == null)
            {
                throw new InfinityNotFoundException("Medal, Award & Publication not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "MedalAward";
                bnLog.TableEntryForm = "Medal Award";

                var rank = employeeService.GetDynamicTableInfoById("Rank", "RankId", medalAward.RankId ?? 0);
                var transfer = employeeService.GetDynamicTableInfoById("vwTransfer", "TransferId", medalAward.TransferId ?? 0);

                
                
                bnLog.PreviousValue = "Id: " + medalAward.MedalAwardId + ", Employee: " + medalAward.Employee.FullNameEng;
                bnLog.PreviousValue += ", Back Log: " + medalAward.IsBackLog;
                bnLog.PreviousValue += ", Rank: " + ((dynamic)rank).ShortName;
                bnLog.PreviousValue += ", Born/Attach/Appointment: " + ((dynamic)transfer).BornOffice + '/' + ((dynamic)transfer).CurrentAttach + '/' + ((dynamic)transfer).Appointment;
                bnLog.PreviousValue += ", Type: " + (medalAward.Type == 1 ? "Award" : medalAward.Type == 2 ? "Medal" : medalAward.Type == 3 ? "Publication" : "");
                bnLog.PreviousValue += medalAward.Type == 1 ? ", Award Name: "+medalAward.Award.NameEng : medalAward.Type == 2 ? ", Medal Name: " + medalAward.Medal.NameEng : medalAward.Type == 3 ? ", Publication Category Name: " + medalAward.PublicationCategory.Name + ", Publication Name: "+ medalAward.Publication.Name : "" ;
                bnLog.PreviousValue += ", Date: " + medalAward.Date.ToString("dd/MM/yyyy") + ", Remarks: " + medalAward.Remarks + ", File: " + medalAward.FileName;

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                var result = await medalAwardRepository.DeleteAsync(medalAward);
				await processRepository.UpdateNamingConvention(medalAward.EmployeeId);
	            return result;

            }
        }

        public List<SelectModel> GetMedalAwardTypeSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(MedalAwardType)).Cast<MedalAwardType>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }


      
    }
}
