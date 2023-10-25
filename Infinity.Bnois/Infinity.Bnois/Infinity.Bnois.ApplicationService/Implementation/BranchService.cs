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
	public class BranchService : IBranchService
	{	

		private readonly IBnoisRepository<Branch> branchRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public BranchService(IBnoisRepository<Branch> branchRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
		{
			this.branchRepository = branchRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }

		public async Task<bool> DeleteBranch(int id)
		{
			if (id <= 0)
			{
				throw new InfinityArgumentMissingException("Invalid Request");
			}
			Branch branch = await branchRepository.FindOneAsync(x => x.BranchId == id);
			if (branch == null)
			{
				throw new InfinityNotFoundException("Branch not found");
			}
			else
			{
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Branch";
                bnLog.TableEntryForm = "Branch";
                bnLog.PreviousValue = "Id: " + branch.BranchId + ", Name: " + branch.Name + ", NameBan: " + branch.NameBan + ", ShortName: " + branch.ShortName + ", ShortNameBan: " + branch.ShortNameBan + ", Priority: " + branch.Priority + ", Description: " + branch.Description;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await branchRepository.DeleteAsync(branch);
			}
		}

		public async Task<BranchModel> GetBranch(int id)
		{
			if (id <= 0)
			{
				return new BranchModel();
			}
			Branch branch = await branchRepository.FindOneAsync(x => x.BranchId == id);
			if (branch == null)
			{
				throw new InfinityNotFoundException("Branch not found");
			}
			BranchModel model = ObjectConverter<Branch, BranchModel>.Convert(branch);
			return model;
		}

		public List<BranchModel> GetBranchs(int ps, int pn, string qs, out int total)
		{
	

			IQueryable<Branch> branchs = branchRepository.FilterWithInclude(x => x.Active == true
				  && ((x.Name.Contains(qs) || String.IsNullOrEmpty(qs)) ||
				  (x.Description.Contains(qs) || String.IsNullOrEmpty(qs))));

			total = branchs.Count();
			branchs = branchs.OrderByDescending(x => x.BranchId).Skip((pn - 1) * ps).Take(ps);
			List<BranchModel> models = ObjectConverter<Branch, BranchModel>.ConvertList(branchs.ToList()).ToList();
			return models;
		}

     
        public async Task<BranchModel> SaveBranch(int id, BranchModel model)
		{
			string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
			if (model == null)
			{
				throw new InfinityArgumentMissingException("Branch data missing");
			}
			Branch branch = ObjectConverter<BranchModel, Branch>.Convert(model);
			if (id > 0)
			{
				branch = await branchRepository.FindOneAsync(x => x.BranchId == id);
				if (branch == null)
				{
					throw new InfinityNotFoundException("Branch not found !");
				}

			    branch.Modified = DateTime.Now;
			    branch.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Branch";
                bnLog.TableEntryForm = "Branch";
                bnLog.PreviousValue = "Id: " + model.BranchId;
                bnLog.UpdatedValue = "Id: " + model.BranchId;
                if (branch.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + branch.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (branch.NameBan != model.NameBan)
                {
                    bnLog.PreviousValue += ", NameBan: " + branch.NameBan;
                    bnLog.UpdatedValue += ", NameBan: " + model.NameBan;
                }
                if (branch.ShortName != model.ShortName)
                {
                    bnLog.PreviousValue += ", ShortName: " + branch.ShortName;
                    bnLog.UpdatedValue += ", ShortName: " + model.ShortName;
                }
                if (branch.ShortNameBan != model.ShortNameBan)
                {
                    bnLog.PreviousValue += ", ShortNameBan: " + branch.ShortNameBan;
                    bnLog.UpdatedValue += ", ShortNameBan: " + model.ShortNameBan;
                }
                if (branch.Priority != model.Priority)
                {
                    bnLog.PreviousValue += ", Priority: " + branch.Priority;
                    bnLog.UpdatedValue += ", Priority: " + model.Priority;
                }
                if (branch.Description != model.Description)
                {
                    bnLog.PreviousValue += ", Description: " + branch.Description;
                    bnLog.UpdatedValue += ", Description: " + model.Description;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (branch.Name != model.Name || branch.NameBan != model.NameBan || branch.ShortName != model.ShortName || branch.ShortNameBan != model.ShortNameBan || branch.Priority != model.Priority || branch.Description != model.Description)
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
				branch.Created = DateTime.Now;
				branch.CreatedBy = userId;
			}
			branch.Name = model.Name;
			branch.NameBan = model.NameBan;
			branch.ShortName = model.ShortName;
			branch.ShortNameBan = model.ShortNameBan;
			branch.Priority = model.Priority;
			branch.Description = model.Description;
		
			branch.Active = true;
			await branchRepository.SaveAsync(branch);
			model.BranchId = branch.BranchId;
			return model;
		}

        public async Task<List<SelectModel>> GetBranchSelectModels()
        {
            ICollection<Branch> branches = await branchRepository.FilterAsync(x => x.Active==true);
            List<SelectModel> selectModels = branches.OrderBy(x=>x.Priority).Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.BranchId
            }).ToList();
            return selectModels;
        }

    }
}
