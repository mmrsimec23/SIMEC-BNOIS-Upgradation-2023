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
    public class MedalService : IMedalService
    {

        private readonly IBnoisRepository<Medal> medalRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public MedalService(IBnoisRepository<Medal> medalRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.medalRepository = medalRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }


        public List<MedalModel> GetMedals(int ps, int pn, string qs, out int total)
        {
            IQueryable<Medal> medals = medalRepository.FilterWithInclude(x => x.IsActive && (x.NameEng.Contains(qs) || String.IsNullOrEmpty(qs)));
            total = medals.Count();
            medals = medals.OrderByDescending(x => x.MedalId).Skip((pn - 1) * ps).Take(ps);
            List<MedalModel> models = ObjectConverter<Medal, MedalModel>.ConvertList(medals.ToList()).ToList();

            models = models.Select(x =>
            {
                x.MedalTypeName = Enum.GetName(typeof(MedalType), x.MedalType);
                return x;
            }).ToList();
            return models;
        }

        public async Task<MedalModel> GetMedal(int id)
        {
            if (id <= 0)
            {
                return new MedalModel();
            }
            Medal medal = await medalRepository.FindOneAsync(x => x.MedalId == id);
            if (medal == null)
            {
                throw new InfinityNotFoundException("Medal not found");
            }
            MedalModel model = ObjectConverter<Medal, MedalModel>.Convert(medal);
            return model;
        }

        public async Task<MedalModel> SaveMedal(int id, MedalModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Medal data missing");
            }
            bool isExist = medalRepository.Exists(x => x.NameEng == model.NameEng && x.MedalId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            Medal medal = ObjectConverter<MedalModel, Medal>.Convert(model);
            if (id > 0)
            {
                medal = await medalRepository.FindOneAsync(x => x.MedalId == id);
                if (medal == null)
                {
                    throw new InfinityNotFoundException("Medal not found !");
                }

                medal.ModifiedDate = DateTime.Now;
                medal.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Medal";
                bnLog.TableEntryForm = "Medal";
                bnLog.PreviousValue = "Id: " + model.MedalId;
                bnLog.UpdatedValue = "Id: " + model.MedalId;
                if (medal.MedalType != model.MedalType)
                {
                    bnLog.PreviousValue += ", Medal Type: " + (medal.MedalType== 1 ? "PreCommission" : medal.MedalType == 2 ? "PostCommission" : "");
                    bnLog.UpdatedValue += ", Medal Type: " + (model.MedalType == 1 ? "PreCommission" : model.MedalType == 2 ? "PostCommission" : "");
                }
                if (medal.NameEng != model.NameEng)
                {
                    bnLog.PreviousValue += ", Name Eng: " + medal.NameEng;
                    bnLog.UpdatedValue += ", Name Eng: " + model.NameEng;
                }
                if (medal.NameBan != model.NameBan)
                {
                    bnLog.PreviousValue += ", Name Ban: " + medal.NameBan;
                    bnLog.UpdatedValue += ", Name Ban: " + model.NameBan;
                }
                if (medal.ShortNameEng != model.ShortNameEng)
                {
                    bnLog.PreviousValue += ", Short Name Eng: " + medal.ShortNameEng;
                    bnLog.UpdatedValue += ", Short Name Eng: " + model.ShortNameEng;
                }
                if (medal.ShortNameBan != model.ShortNameBan)
                {
                    bnLog.PreviousValue += ", Short Name Ban: " + medal.ShortNameBan;
                    bnLog.UpdatedValue += ", Short Name Ban: " + model.ShortNameBan;
                }
                if (medal.Description != model.Description)
                {
                    bnLog.PreviousValue += ", Description: " + medal.Description;
                    bnLog.UpdatedValue += ", Description: " + model.Description;
                }
                if (medal.Priority != model.Priority)
                {
                    bnLog.PreviousValue += ", Priority: " + medal.Priority;
                    bnLog.UpdatedValue += ", Priority: " + model.Priority;
                }
                if (medal.GoToTrace != model.GoToTrace)
                {
                    bnLog.PreviousValue += ", Go To Trace: " + medal.GoToTrace;
                    bnLog.UpdatedValue += ", Go To Trace: " + model.GoToTrace;
                }
                if (medal.ANmCon != model.ANmCon)
                {
                    bnLog.PreviousValue += ", ANGF: " + medal.ANmCon;
                    bnLog.UpdatedValue += ", ANGF: " + model.ANmCon;
                }
                if (medal.NmRGF != model.NmRGF)
                {
                    bnLog.PreviousValue += ", NmRGF: " + medal.NmRGF;
                    bnLog.UpdatedValue += ", NmRGF: " + model.NmRGF;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (medal.NameEng != model.NameEng || medal.NameBan != model.NameBan || medal.ShortNameEng != model.ShortNameEng || medal.ShortNameBan != model.ShortNameBan
                    || medal.Description != model.Description || medal.Priority != model.Priority || medal.MedalType != model.MedalType || medal.GoToTrace != model.GoToTrace
                    || medal.ANmCon != model.ANmCon || medal.NmRGF != model.NmRGF)
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
                medal.IsActive = true;
                medal.CreatedDate = DateTime.Now;
                medal.CreatedBy = userId;
            }
            medal.NameEng = model.NameEng;
            medal.NameBan = model.NameBan;
            medal.ShortNameBan = model.ShortNameBan;
            medal.ShortNameEng = model.ShortNameEng;
            medal.Priority = model.Priority;
            medal.MedalType = model.MedalType;
            medal.GoToTrace = model.GoToTrace;
            medal.ANmCon = model.ANmCon;
            medal.NmRGF = model.NmRGF;
            medal.Description = model.Description;


            await medalRepository.SaveAsync(medal);
            model.MedalId = medal.MedalId;
            return model;
        }

        public async Task<bool> DeleteMedal(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Medal medal = await medalRepository.FindOneAsync(x => x.MedalId == id);
            if (medal == null)
            {
                throw new InfinityNotFoundException("Medal not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Medal";
                bnLog.TableEntryForm = "Medal";
                bnLog.PreviousValue = "Id: " + medal.MedalId + ", Name Eng: " + medal.NameEng + ", Name Ban: " + medal.NameBan + ", Short Name Eng: " + medal.ShortNameEng + ", Short Name Ban: " + medal.ShortNameBan
                    + ", Description: " + medal.Description + ", Priority: " + medal.Priority + ", Medal Type: " + (medal.MedalType == 1 ? "PreCommission" : medal.MedalType == 2 ? "PostCommission" : "") 
                    + ", Go To Trace: " + medal.GoToTrace + ", ANGF: " + medal.ANmCon + ", NmRGF: " + medal.NmRGF;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await medalRepository.DeleteAsync(medal);
            }
        }

        public async Task<List<SelectModel>> GetMedalSelectModels(int medalType)
        {
            ICollection<Medal> models = await medalRepository.FilterAsync(x => x.IsActive && x.MedalType == medalType);
            return models.OrderBy(x => x.NameEng).Select(x => new SelectModel()
            {
                Text = x.NameEng,
                Value = x.MedalId
            }).ToList();

        }
        public async Task<List<SelectModel>> GetMedalSelectModels()
        {
            ICollection<Medal> models = await medalRepository.FilterAsync(x => x.IsActive );
            return models.OrderBy(x=>x.NameEng).Select(x => new SelectModel()
            {
                Text = x.NameEng,
                Value = x.MedalId
            }).ToList();

        }

        public List<SelectModel> GetMedalTypeSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(MedalType)).Cast<MedalType>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }

        public async Task<List<SelectModel>> GetTraceMedalSelectModels(int medalType)
        {
            ICollection<Medal> models = await medalRepository.FilterAsync(x => x.IsActive && x.MedalType == medalType && x.GoToTrace);
            return models.Select(x => new SelectModel()
            {
                Text = x.NameEng,
                Value = x.MedalId
            }).ToList();
        }
    }
}
