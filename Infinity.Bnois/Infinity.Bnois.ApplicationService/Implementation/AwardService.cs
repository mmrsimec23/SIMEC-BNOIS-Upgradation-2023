using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;

namespace Infinity.Bnois.ApplicationService.Implementation
{
   public class AwardService : IAwardService
    {

        private readonly IBnoisRepository<Award> awardRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public AwardService(IBnoisRepository<Award> awardRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.awardRepository = awardRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }

        public List<AwardModel> GetAwards(int ps, int pn, string qs, out int total)
        {
            IQueryable<Award> awards = awardRepository.FilterWithInclude(x => x.IsActive && (x.NameEng.Contains(qs) || String.IsNullOrEmpty(qs) ));
            total = awards.Count();
            awards = awards.OrderByDescending(x => x.AwardId).Skip((pn - 1) * ps).Take(ps);
            List<AwardModel> models = ObjectConverter<Award, AwardModel>.ConvertList(awards.ToList()).ToList();
            return models;
        }

        public async Task<AwardModel> GetAward(int id)
        {
            if (id <= 0)
            {
                return new AwardModel();
            }
            Award award = await awardRepository.FindOneAsync(x => x.AwardId == id);
            if (award == null)
            {
                throw new InfinityNotFoundException("Award not found");
            }
            AwardModel model = ObjectConverter<Award, AwardModel>.Convert(award);
            return model;
        }

        public async Task<AwardModel> SaveAward(int id, AwardModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Award data missing");
            }
            bool isExist = awardRepository.Exists(x => x.NameEng == model.NameEng  && x.AwardId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            Award award = ObjectConverter<AwardModel, Award>.Convert(model);
            if (id > 0)
            {
                award = await awardRepository.FindOneAsync(x => x.AwardId == id);
                if (award == null)
                {
                    throw new InfinityNotFoundException("Award not found !");
                }

                award.ModifiedDate = DateTime.Now;
                award.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Award";
                bnLog.TableEntryForm = "Award";
                bnLog.PreviousValue = "Id: " + model.AwardId;
                bnLog.UpdatedValue = "Id: " + model.AwardId;
                if (award.NameEng != model.NameEng)
                {
                    bnLog.PreviousValue += ", Name Eng: " + award.NameEng;
                    bnLog.UpdatedValue += ", Name Eng: " + model.NameEng;
                }
                if (award.NameBan != model.NameBan)
                {
                    bnLog.PreviousValue += ", Name Ban: " + award.NameBan;
                    bnLog.UpdatedValue += ", Name Ban: " + model.NameBan;
                }
                if (award.ShortNameEng != model.ShortNameEng)
                {
                    bnLog.PreviousValue += ", Short Name Eng: " + award.ShortNameEng;
                    bnLog.UpdatedValue += ", Short Name Eng: " + model.ShortNameEng;
                }
                if (award.ShortNameBan != model.ShortNameBan)
                {
                    bnLog.PreviousValue += ", Short Name Ban: " + award.ShortNameBan;
                    bnLog.UpdatedValue += ", Short Name Ban: " + model.ShortNameBan;
                }
                if (award.Description != model.Description)
                {
                    bnLog.PreviousValue += ", Description: " + award.Description;
                    bnLog.UpdatedValue += ", Description: " + model.Description;
                }
                if (award.Priority != model.Priority)
                {
                    bnLog.PreviousValue += ", Priority: " + award.Priority;
                    bnLog.UpdatedValue += ", Priority: " + model.Priority;
                }
                if (award.GoToOPR != model.GoToOPR)
                {
                    bnLog.PreviousValue += ", Go To OPR: " + award.GoToOPR;
                    bnLog.UpdatedValue += ", Go To OPR: " + model.GoToOPR;
                }
                if (award.GoToTrace != model.GoToTrace)
                {
                    bnLog.PreviousValue += ", Go To Trace: " + award.GoToTrace;
                    bnLog.UpdatedValue += ", Go To Trace: " + model.GoToTrace;
                }
                if (award.GoToSASB != model.GoToSASB)
                {
                    bnLog.PreviousValue += ", Go To SASB: " + award.GoToSASB;
                    bnLog.UpdatedValue += ", Go To SASB: " + model.GoToSASB;
                }
                if (award.ANmCon != model.ANmCon)
                {
                    bnLog.PreviousValue += ", ANGF: " + award.ANmCon;
                    bnLog.UpdatedValue += ", ANGF: " + model.ANmCon;
                }
                if (award.NmRGF != model.NmRGF)
                {
                    bnLog.PreviousValue += ", NmRGF: " + award.NmRGF;
                    bnLog.UpdatedValue += ", NmRGF: " + model.NmRGF;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (award.NameEng != model.NameEng || award.NameBan != model.NameBan || award.ShortNameEng != model.ShortNameEng || award.ShortNameBan != model.ShortNameBan
                    || award.Description != model.Description || award.Priority != model.Priority || award.GoToOPR != model.GoToOPR || award.GoToTrace != model.GoToTrace
                    || award.GoToSASB != model.GoToSASB || award.ANmCon != model.ANmCon || award.NmRGF != model.NmRGF)
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
                award.IsActive = true;
                award.CreatedDate = DateTime.Now;
                award.CreatedBy = userId;
            }
            award.NameEng = model.NameEng;
            award.NameBan = model.NameBan;
            award.ShortNameBan = model.ShortNameBan;
            award.ShortNameEng = model.ShortNameEng;
            award.Priority = model.Priority;
            award.GoToOPR = model.GoToOPR;
            award.GoToTrace = model.GoToTrace;
            award.ANmCon = model.ANmCon;
            award.NmRGF = model.NmRGF;
            award.Description = model.Description;
            award.GoToSASB = model.GoToSASB;


            await awardRepository.SaveAsync(award);
            model.AwardId = award.AwardId;
            return model;
        }

        public async Task<bool> DeleteAward(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Award award = await awardRepository.FindOneAsync(x => x.AwardId == id);
            if (award == null)
            {
                throw new InfinityNotFoundException("Award not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Award";
                bnLog.TableEntryForm = "Award";
                bnLog.PreviousValue = "Id: " + award.AwardId + ", Name Eng: " + award.NameEng + ", Name Ban: " + award.NameBan + ", Short Name Eng: " + award.ShortNameEng + ", Short Name Ban: " + award.ShortNameBan
                    + ", Description: " + award.Description + ", Priority: " + award.Priority + ", Go To OPR: " + award.GoToOPR + ", Go To Trace: " + award.GoToTrace + ", Go To SASB: " + award.GoToSASB
                    + ", ANGF: " + award.ANmCon + ", NmRGF: " + award.NmRGF;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await awardRepository.DeleteAsync(award);
            }
        }

        public async Task<List<SelectModel>> GetAwardSelectModels()
        {
            ICollection<Award> models = await awardRepository.FilterAsync(x => x.IsActive);
            return models.OrderBy(x => x.NameEng).Select(x => new SelectModel()
            {
                Text = x.NameEng,
                Value = x.AwardId
            }).ToList();

        }




    }
}
