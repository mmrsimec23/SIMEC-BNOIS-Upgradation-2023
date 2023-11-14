using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Configuration.Models;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class SubBranchService : ISubBranchService
    {
        private readonly IBnoisRepository<SubBranch> subBranchRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public SubBranchService(IBnoisRepository<SubBranch> subBranchRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.subBranchRepository = subBranchRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }
        
        public List<SubBranchModel> GetSubBranches(int ps, int pn, string qs, out int total)
        {
            IQueryable<SubBranch> subBraches = subBranchRepository.FilterWithInclude(x => x.IsActive && (x.ShortName.Contains(qs) || String.IsNullOrEmpty(qs)) || (x.FullName.Contains(qs) || String.IsNullOrEmpty(qs)));
            total = subBraches.Count();
            subBraches = subBraches.OrderByDescending(x => x.SubBranchId).Skip((pn - 1) * ps).Take(ps);
            List<SubBranchModel> models = ObjectConverter<SubBranch, SubBranchModel>.ConvertList(subBraches.ToList()).ToList();
            return models;
        }

        public async Task<SubBranchModel> GetSubBranch(int id)
        {
            if (id <= 0)
            {
                return new SubBranchModel();
            }
            SubBranch subBranch = await subBranchRepository.FindOneAsync(x => x.SubBranchId == id);
            if (subBranch == null)
            {
                throw new InfinityNotFoundException("SubBranch not found");
            }
            SubBranchModel model = ObjectConverter<SubBranch, SubBranchModel>.Convert(subBranch);
            return model;
        }

        public async Task<SubBranchModel> SaveSubBranch(int id, SubBranchModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("SubBranch data missing");
            }
            bool isExist = subBranchRepository.Exists(x => x.FullName == model.FullName && x.ShortName == model.ShortName && x.SubBranchId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("SubBranch already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            SubBranch subBranch = ObjectConverter<SubBranchModel, SubBranch>.Convert(model);
            if (id > 0)
            {
                subBranch = await subBranchRepository.FindOneAsync(x => x.SubBranchId == id);
                if (subBranch == null)
                {
                    throw new InfinityNotFoundException("SubBranch not found !");
                }

                subBranch.ModifiedDate = DateTime.Now;
                subBranch.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "SubBranch";
                bnLog.TableEntryForm = "Sub Branch";
                bnLog.PreviousValue = "Id: " + model.SubBranchId;
                bnLog.UpdatedValue = "Id: " + model.SubBranchId;
                int bnoisUpdateCount = 0;
                if (subBranch.FullName != model.FullName)
                {
                    bnLog.PreviousValue += ", FullName: " + subBranch.FullName;
                    bnLog.UpdatedValue += ", FullName: " + model.FullName;
                    bnoisUpdateCount += 1;
                }
                if (subBranch.ShortName != model.ShortName)
                {
                    bnLog.PreviousValue += ", ShortName: " + subBranch.ShortName;
                    bnLog.UpdatedValue += ", ShortName: " + model.ShortName;
                    bnoisUpdateCount += 1;
                }
                if (subBranch.FullNameBan != model.FullNameBan)
                {
                    bnLog.PreviousValue += ", FullNameBan: " + subBranch.FullNameBan;
                    bnLog.UpdatedValue += ", FullNameBan: " + model.FullNameBan;
                    bnoisUpdateCount += 1;
                }
                if (subBranch.ShortNameBan != model.ShortNameBan)
                {
                    bnLog.PreviousValue += ", ShortNameBan: " + subBranch.ShortNameBan;
                    bnLog.UpdatedValue += ", ShortNameBan: " + model.ShortNameBan;
                    bnoisUpdateCount += 1;
                }
                if (subBranch.Description != model.Description)
                {
                    bnLog.PreviousValue += ", Description: " + subBranch.Description;
                    bnLog.UpdatedValue += ", Description: " + model.Description;
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
                subBranch.IsActive = true;
                subBranch.CreatedDate = DateTime.Now;
                subBranch.CreatedBy = userId;
            }
            subBranch.FullName = model.FullName;
            subBranch.ShortName = model.ShortName;
            subBranch.FullNameBan = model.FullNameBan;
            subBranch.ShortNameBan = model.ShortNameBan;
       
            subBranch.Description = model.Description;
            await subBranchRepository.SaveAsync(subBranch);
            model.SubBranchId = subBranch.SubBranchId;
            return model;
        }

        public async Task<bool> DeleteSubBranch(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            SubBranch subBranch = await subBranchRepository.FindOneAsync(x => x.SubBranchId == id);
            if (subBranch == null)
            {
                throw new InfinityNotFoundException("SubBranch not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "SubBranch";
                bnLog.TableEntryForm = "Sub Branch";
                bnLog.PreviousValue = "Id: " + subBranch.SubBranchId + ", FullName: " + subBranch.FullName + ", ShortName: " + subBranch.ShortName + ", FullNameBan: " + subBranch.FullNameBan + ", ShortNameBan: " + subBranch.ShortNameBan + ", Description: " + subBranch.Description;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await subBranchRepository.DeleteAsync(subBranch);
            }
        }

        public async Task<List<SelectModel>> GetSubBranchSelectModels()
        {
            ICollection<SubBranch> subBranches = await subBranchRepository.FilterAsync(x => x.IsActive);
            List<SelectModel> selectModels = subBranches.OrderBy(x=>x.ShortName).Select(x => new SelectModel
            {
                Text = x.ShortName,
                Value = x.SubBranchId
            }).ToList();
            return selectModels;
        }
    }
}
