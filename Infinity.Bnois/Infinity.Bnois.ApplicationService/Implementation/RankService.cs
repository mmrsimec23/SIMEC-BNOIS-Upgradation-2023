
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Implementation
{

    public class RankService : IRankService
    {
        private readonly IRankRepository rankRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public RankService(IRankRepository rankRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.rankRepository = rankRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }
        public List<RankModel> 
	        GetRanks(int pageSize, int pageNumber, string searchText, out int total)
        {
            IQueryable<Rank> ranks = rankRepository.FilterWithInclude(x => x.IsActive
                 && ((x.FullName.Contains(searchText) || String.IsNullOrEmpty(searchText)) ||
                 (x.ShortName.Contains(searchText) || String.IsNullOrEmpty(searchText))));
            total = ranks.Count();
            ranks = ranks.OrderByDescending(x => x.RankId).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            List<RankModel> models = ObjectConverter<Rank, RankModel>.ConvertList(ranks.ToList()).ToList();
            return models;
        }

        public async Task<RankModel> GetRank(int id)
        {
            if (id <= 0)
            {
                return new RankModel();
            }
            Rank rank = await rankRepository.FindOneAsync(x => x.RankId == id);
            if (rank == null)
            {
                throw new InfinityNotFoundException("Rank not found");
            }
            RankModel model = ObjectConverter<Rank, RankModel>.Convert(rank);
            return model;
        }

        public async Task<RankModel> SaveRank(int rankId, RankModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Rank data missing");
            }

            bool isExistData = rankRepository.Exists(x => x.FullName == model.FullName && x.ShortName == model.ShortName && x.RankId != model.RankId);
            if (isExistData)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            bool isRankOrderExists = rankRepository.Exists(x =>x.RankOrder == model.RankOrder && x.RankId != model.RankId);
            if (isRankOrderExists)
            {
                throw new InfinityInvalidDataException("Rank serial already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            Rank rank = ObjectConverter<RankModel, Rank>.Convert(model);
            if (rankId > 0)
            {
                rank = await rankRepository.FindOneAsync(x => x.RankId == rankId);
                if (rank == null)
                {
                    throw new InfinityNotFoundException("Rank not found !");
                }
                rank.Modified = DateTime.Now;
                rank.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Rank";
                bnLog.TableEntryForm = "Rank";
                bnLog.PreviousValue = "Id: " + model.RankId;
                bnLog.UpdatedValue = "Id: " + model.RankId;
                if (rank.FullName != model.FullName)
                {
                    bnLog.PreviousValue += ", FullName: " + rank.FullName;
                    bnLog.UpdatedValue += ", FullName: " + model.FullName;
                }
                if (rank.FullNameBan != model.FullNameBan)
                {
                    bnLog.PreviousValue += ", FullNameBan: " + rank.FullNameBan;
                    bnLog.UpdatedValue += ", FullNameBan: " + model.FullNameBan;
                }
                if (rank.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + rank.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }
                if (rank.ShortName != model.ShortName)
                {
                    bnLog.PreviousValue += ", ShortName: " + rank.ShortName;
                    bnLog.UpdatedValue += ", ShortName: " + model.ShortName;
                }
                if (rank.ShortNameBan != model.ShortNameBan)
                {
                    bnLog.PreviousValue += ", ShortNameBan: " + rank.ShortNameBan;
                    bnLog.UpdatedValue += ", ShortNameBan: " + model.ShortNameBan;
                }
                if (rank.RankOrder != model.RankOrder)
                {
                    bnLog.PreviousValue += ", RankOrder: " + rank.RankOrder;
                    bnLog.UpdatedValue += ", RankOrder: " + model.RankOrder;
                }
                if (rank.RankLevel != model.RankLevel)
                {
                    bnLog.PreviousValue += ", RankLevel: " + rank.RankLevel;
                    bnLog.UpdatedValue += ", RankLevel: " + model.RankLevel;
                }
                if (rank.IsConfirm != model.IsConfirm)
                {
                    bnLog.PreviousValue += ", IsConfirm: " + rank.IsConfirm;
                    bnLog.UpdatedValue += ", IsConfirm: " + model.IsConfirm;
                }
                if (rank.ServiceYear != model.ServiceYear)
                {
                    bnLog.PreviousValue += ", ServiceYear: " + rank.ServiceYear;
                    bnLog.UpdatedValue += ", ServiceYear: " + model.ServiceYear;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (rank.FullName != model.FullName || rank.FullNameBan != model.FullNameBan || rank.Remarks != model.Remarks 
                    || rank.ShortName != model.ShortName || rank.ShortNameBan != model.ShortNameBan || rank.RankOrder != model.RankOrder
                    || rank.RankLevel != model.RankLevel || rank.IsConfirm != model.IsConfirm || rank.ServiceYear != model.ServiceYear)
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
                rank.IsActive = true;
                rank.Created = DateTime.Now;
                rank.CreatedBy = userId;
            }
            rank.FullName = model.FullName;
            rank.FullNameBan = model.FullNameBan;
            rank.ShortName = model.ShortName;
            rank.ShortNameBan = model.ShortNameBan;
            rank.RankOrder = model.RankOrder;
            rank.RankLevel = model.RankLevel;
            rank.IsConfirm = model.IsConfirm;
            rank.ServiceYear = model.ServiceYear;
            rank.Remarks = model.Remarks;
            await rankRepository.SaveAsync(rank);
            return model;
        }
        public async Task<bool> DeleteRank(int rankId)
        {
            if (rankId < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Rank rank = await rankRepository.FindOneAsync(x => x.RankId == rankId);
            if (rank == null)
            {
                throw new InfinityNotFoundException("Rank not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Rank";
                bnLog.TableEntryForm = "Rank";
                bnLog.PreviousValue = "Id: " + rank.RankId + ", FullName: " + rank.FullName + ", FullNameBan: " + rank.FullNameBan + ", Remarks: " 
                    + rank.Remarks + ", ShortName: " + rank.ShortName + ", ShortNameBan: " + rank.ShortNameBan + ", RankOrder: " + rank.RankOrder
                    + ", RankLevel: " + rank.RankLevel + ", IsConfirm: " + rank.IsConfirm + ", ServiceYear: " + rank.ServiceYear;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await rankRepository.DeleteAsync(rank);
            }
        }

        public async Task<List<SelectModel>> GetRanksSelectModel()
        {
            List<Rank> ranks = await rankRepository.Where(x => x.RankId > 0).ToListAsync();
            List<Rank> query = ranks.OrderBy(x => x.RankOrder).ToList();
            List<SelectModel> rankSelectModels = query.Select(x => new SelectModel()
            {
                Text = x.ShortName,
                Value = x.RankId
            }).ToList();

            return rankSelectModels;
        }

     
        public async Task<List<SelectModel>> GetRankSelectModels()
        {
            ICollection<Rank> models = await rankRepository.FilterAsync(x => x.IsActive);
            return models.OrderBy(x=>x.RankOrder).Select(x => new SelectModel()
            {
                Text = x.FullName,
                Value = x.RankId
            }).ToList();
        }

        public async Task<List<SelectModel>> GetRankSelectModelsByRankCategory(int rankCategoryId)
        {
            ICollection<Rank> models = await rankRepository.FilterAsync(x => x.IsActive);
            return models.OrderBy(x => x.RankOrder).Select(x => new SelectModel()
            {
                Text = x.FullName,
                Value = x.RankId
            }).ToList();
        }


        public async Task<List<SelectModel>> GetConfirmRankSelectModels()
        {
            ICollection<Rank> models = await rankRepository.FilterAsync(x => x.IsActive && x.IsConfirm);
            return models.OrderBy(x => x.RankOrder).Select(x => new SelectModel()
            {
                Text = x.FullName,
                Value = x.RankId
            }).ToList();
        }

        public async Task<List<SelectModel>> GetActingRankSelectModels()
        {
            ICollection<Rank> models = await rankRepository.FilterAsync(x => x.IsActive && !x.IsConfirm);
            return models.OrderBy(x => x.RankOrder).Select(x => new SelectModel()
            {
                Text = x.FullName,
                Value = x.RankId
            }).ToList();
        }

    }
}
