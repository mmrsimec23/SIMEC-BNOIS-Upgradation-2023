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
    public class PreCommissionRankService : IPreCommissionRankService
    {
        private readonly IBnoisRepository<PreCommissionRank> preCommissionRankRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public PreCommissionRankService(IBnoisRepository<PreCommissionRank> preCommissionRankRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.preCommissionRankRepository = preCommissionRankRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }

    
        public List<PreCommissionRankModel> GetPreCommissionRanks(int ps, int pn, string qs, out int total)
        {
            IQueryable<PreCommissionRank> preCommissionRanks = preCommissionRankRepository.FilterWithInclude(x => x.IsActive && (x.Name.Contains(qs) || String.IsNullOrEmpty(qs)));
            total = preCommissionRanks.Count();
            preCommissionRanks = preCommissionRanks.OrderByDescending(x => x.PreCommissionRankId).Skip((pn - 1) * ps).Take(ps);
            List<PreCommissionRankModel> models = ObjectConverter<PreCommissionRank, PreCommissionRankModel>.ConvertList(preCommissionRanks.ToList()).ToList();
            return models;
        }
        public async Task<PreCommissionRankModel> GetPreCommissionRank(int id)
        {
            if (id <= 0)
            {
                return new PreCommissionRankModel();
            }
            PreCommissionRank preCommissionRank = await preCommissionRankRepository.FindOneAsync(x => x.PreCommissionRankId == id);
            if (preCommissionRank == null)
            {
                throw new InfinityNotFoundException("Pre Commission Rank not found");
            }
            PreCommissionRankModel model = ObjectConverter<PreCommissionRank, PreCommissionRankModel>.Convert(preCommissionRank);
            return model;
        }

        public async Task<PreCommissionRankModel> SavePreCommissionRank(int id, PreCommissionRankModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Pre Commission Rank data missing");
            }
            bool isExist = preCommissionRankRepository.Exists(x => x.Name == model.Name && x.PreCommissionRankId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            PreCommissionRank preCommissionRank = ObjectConverter<PreCommissionRankModel, PreCommissionRank>.Convert(model);
            if (id > 0)
            {
                preCommissionRank = await preCommissionRankRepository.FindOneAsync(x => x.PreCommissionRankId == id);
                if (preCommissionRank == null)
                {
                    throw new InfinityNotFoundException("Pre Commission Rank not found !");
                }

                preCommissionRank.ModifiedDate = DateTime.Now;
                preCommissionRank.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PreCommissionRank";
                bnLog.TableEntryForm = "Pre Commission Rank";
                bnLog.PreviousValue = "Id: " + model.PreCommissionRankId;
                bnLog.UpdatedValue = "Id: " + model.PreCommissionRankId;
                if (preCommissionRank.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + preCommissionRank.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (preCommissionRank.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + preCommissionRank.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (preCommissionRank.Name != model.Name || preCommissionRank.Remarks != model.Remarks)
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
                preCommissionRank.IsActive = true;
                preCommissionRank.CreatedDate = DateTime.Now;
                preCommissionRank.CreatedBy = userId;
            }
            preCommissionRank.Name = model.Name;
            preCommissionRank.Remarks = model.Remarks;
        
            await preCommissionRankRepository.SaveAsync(preCommissionRank);
            model.PreCommissionRankId = preCommissionRank.PreCommissionRankId;
            return model;
        }

        public async Task<bool> DeletePreCommissionRank(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            PreCommissionRank PreCommissionRank = await preCommissionRankRepository.FindOneAsync(x => x.PreCommissionRankId == id);
            if (PreCommissionRank == null)
            {
                throw new InfinityNotFoundException("Pr eCommission Rank not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PreCommissionRank";
                bnLog.TableEntryForm = "Pre Commission Rank";
                bnLog.PreviousValue = "Id: " + PreCommissionRank.PreCommissionRankId + ", Name: " + PreCommissionRank.Name + ", Remarks: " + PreCommissionRank.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await preCommissionRankRepository.DeleteAsync(PreCommissionRank);
            }
        }

        public async Task<List<SelectModel>> GetPreCommissionRankSelectModels()
        {
            ICollection<PreCommissionRank> PreCommissionRanks = await preCommissionRankRepository.FilterAsync(x => x.IsActive);
            return PreCommissionRanks.OrderBy(x => x.Name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.PreCommissionRankId
            }).ToList();
        }
    }
}
