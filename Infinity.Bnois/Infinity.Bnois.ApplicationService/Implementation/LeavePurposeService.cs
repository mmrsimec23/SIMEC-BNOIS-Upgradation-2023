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
	public class LeavePurposeService : ILeavePurposeService
	{
		private readonly IBnoisRepository<LeavePurpose> leavePurposeRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public LeavePurposeService(IBnoisRepository<LeavePurpose> leavePurposeRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
		{
			this.leavePurposeRepository = leavePurposeRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }

		public List<LeavePurposeModel> GetPurposes(int ps, int pn, string qs, out int total)
		{
			IQueryable<LeavePurpose> leavePurposes = leavePurposeRepository.FilterWithInclude(x => x.IsActive
			                                                                  && ((x.Name.Contains(qs)
			                                                                       || String.IsNullOrEmpty(qs))));
			total = leavePurposes.Count();
			leavePurposes = leavePurposes.OrderByDescending(x => x.PurposeId).Skip((pn - 1) * ps).Take(ps);
			List<LeavePurposeModel> models = ObjectConverter<LeavePurpose, LeavePurposeModel>.ConvertList(leavePurposes.ToList()).ToList();
			//models = models.Select(x =>
			//{
			//	x.ColorTypeName = Enum.GetName(typeof(ColorType), x.ColorType);
			//	return x;
			//}).ToList();

			return models;
		}

		public async Task<LeavePurposeModel> GetPurpose(int id)
		{
			if (id <= 0)
			{
				return new LeavePurposeModel();
			}
			LeavePurpose leavePurpose = await leavePurposeRepository.FindOneAsync(x => x.PurposeId == id);
			if (leavePurpose == null)
			{
				throw new InfinityNotFoundException("Leave Purpose not found");
			}
			LeavePurposeModel model = ObjectConverter<LeavePurpose, LeavePurposeModel>.Convert(leavePurpose);
			return model;
		}

		public async Task<LeavePurposeModel> SavePurpose(int id, LeavePurposeModel model)
		{
			string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
			if (model == null)
			{
				throw new InfinityArgumentMissingException("Leave Purpose data missing");
			}
			LeavePurpose leavePurpose = ObjectConverter<LeavePurposeModel, LeavePurpose>.Convert(model);
			if (id > 0)
			{
				leavePurpose = await leavePurposeRepository.FindOneAsync(x => x.PurposeId == id);
				if (leavePurpose == null)
				{
					throw new InfinityNotFoundException("Leave Purpose not found !");
				}

				leavePurpose.ModifiedDate = DateTime.Now;
				leavePurpose.ModifiedBy = userId;



                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "LeavePurpose";
                bnLog.TableEntryForm = "Leave Purpose";
                bnLog.PreviousValue = "Id: " + model.PurposeId;
                bnLog.UpdatedValue = "Id: " + model.PurposeId;
                int bnoisUpdateCount = 0;
                
                if (leavePurpose.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + leavePurpose.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                    bnoisUpdateCount += 1;
                }
                if (leavePurpose.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Survey Leave: " + leavePurpose.Remarks;
                    bnLog.UpdatedValue += ", Survey Leave: " + model.Remarks;
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
				leavePurpose.CreatedDate = DateTime.Now;
				leavePurpose.CreatedBy = userId;
				leavePurpose.IsActive = true;
			}
			leavePurpose.Name = model.Name;
			await leavePurposeRepository.SaveAsync(leavePurpose);
			model.PurposeId = leavePurpose.PurposeId;
			return model;
		}

		public async Task<bool> DeletePurpose(int id)
		{
			if (id <= 0)
			{
				throw new InfinityArgumentMissingException("Invalid Request");
			}
			LeavePurpose leavePurpose = await leavePurposeRepository.FindOneAsync(x => x.PurposeId == id);
			if (leavePurpose == null)
			{
				throw new InfinityNotFoundException("Leave Purpose not found");
			}
			else
			{

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "LeavePurpose";
                bnLog.TableEntryForm = "Leave Purpose";
                bnLog.PreviousValue = "Id: " + leavePurpose.PurposeId;
                
				bnLog.PreviousValue += ", Name: " + leavePurpose.Name + ", Survey Leave: " + leavePurpose.Remarks;

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await leavePurposeRepository.DeleteAsync(leavePurpose);
			}
		}

		public async Task<List<SelectModel>> GetLeavePurposeSelectModel()
		{
			ICollection<LeavePurpose> leavePurposes = await leavePurposeRepository.FilterAsync(x => x.IsActive);
			List<SelectModel> selectModels = leavePurposes.Select(x => new SelectModel
			{
				Text = x.Name,
				Value = x.PurposeId
			}).ToList();
			return selectModels;
		}
	}
}
