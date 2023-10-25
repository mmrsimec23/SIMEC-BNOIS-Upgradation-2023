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
	public class LeaveTypeService : ILeaveTypeService
	{
		private readonly IBnoisRepository<LeaveType> leaveTypeRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public LeaveTypeService(IBnoisRepository<LeaveType> leaveTypeRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
		{
			this.leaveTypeRepository = leaveTypeRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }

		public async Task<bool> DeleteLeave(int id)
		{
			if (id < 0)
			{
				throw new InfinityArgumentMissingException("Invalid Request");
			}
			LeaveType batch = await leaveTypeRepository.FindOneAsync(x => x.LeaveTypeId == id);
			if (batch == null)
			{
				throw new InfinityNotFoundException("Leave Type not found");
			}
			else
			{
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "LeaveType";
                bnLog.TableEntryForm = "Leave Type";
                bnLog.PreviousValue = "Id: " + batch.LeaveTypeId + ", Name: " + batch.TypeName + ", ShartName: " + batch.ShartName + ", Description: " + batch.Description;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await leaveTypeRepository.DeleteAsync(batch);
			}
		}

		public async Task<LeaveTypeModel> GetLeaveType(int id)
		{
			if (id <= 0)
			{
				return new LeaveTypeModel();
			}
			LeaveType leaveType = await leaveTypeRepository.FindOneAsync(x => x.LeaveTypeId == id);
			if (leaveType == null)
			{
				throw new InfinityNotFoundException("Leave Type not found");
			}
			LeaveTypeModel model = ObjectConverter<LeaveType, LeaveTypeModel>.Convert(leaveType);
			return model;
		}

		public List<LeaveTypeModel> GetLeaveTypes(int ps, int pn, string qs, out int total)
		{
			IQueryable<LeaveType> leaveTypes = leaveTypeRepository.FilterWithInclude(x => x.IsActive && (x.TypeName.Contains(qs) || String.IsNullOrEmpty(qs) || (x.ShartName.Contains(qs) || String.IsNullOrEmpty(qs))));
			total = leaveTypes.Count();
			leaveTypes = leaveTypes.OrderByDescending(x => x.LeaveTypeId).Skip((pn - 1) * ps).Take(ps);
			List<LeaveTypeModel> models = ObjectConverter<LeaveType, LeaveTypeModel>.ConvertList(leaveTypes.ToList()).ToList();
			return models;
		}

		public async Task<List<SelectModel>> GetLeaveTypeSelectModel()
		{
			ICollection<LeaveType> leaveType = await leaveTypeRepository.FilterAsync(x => x.IsActive);
			List<SelectModel> selectModels = leaveType.Select(x => new SelectModel
			{
				Text = x.TypeName + "__" +x.ShartName,
				Value = x.LeaveTypeId
			}).ToList();
			return selectModels;
		}

		public async Task<LeaveTypeModel> SaveLeaveType(int id, LeaveTypeModel model)
		{
			if (model == null)
			{
				throw new InfinityArgumentMissingException("Leave Type data missing");
			}
		

			string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
			LeaveType leaveType = ObjectConverter<LeaveTypeModel, LeaveType>.Convert(model);
			if (id > 0)
			{
				leaveType = await leaveTypeRepository.FindOneAsync(x => x.LeaveTypeId == id);
				if (leaveType == null)
				{
					throw new InfinityNotFoundException("Leave Type not found !");
				}
				bool isExist = leaveTypeRepository.Exists(x => x.TypeName == model.TypeName);
				if (isExist)
				{
					throw new InfinityInvalidDataException("Data already exists !");
				}

				leaveType.ModifiedDate = DateTime.Now;
			    leaveType.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "LeaveType";
                bnLog.TableEntryForm = "Leave Type";
                bnLog.PreviousValue = "Id: " + model.LeaveTypeId;
                bnLog.UpdatedValue = "Id: " + model.LeaveTypeId;
                if (leaveType.TypeName != model.TypeName)
                {
                    bnLog.PreviousValue += ", Name: " + leaveType.TypeName;
                    bnLog.UpdatedValue += ", Name: " + model.TypeName;
                }
                if (leaveType.ShartName != model.ShartName)
                {
                    bnLog.PreviousValue += ", ShartName: " + leaveType.ShartName;
                    bnLog.UpdatedValue += ", ShartName: " + model.ShartName;
                }
                if (leaveType.Description != model.Description)
                {
                    bnLog.PreviousValue += ", Description: " + leaveType.Description;
                    bnLog.UpdatedValue += ", Description: " + model.Description;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (leaveType.TypeName != model.TypeName || leaveType.ShartName != model.ShartName || leaveType.Description != model.Description)
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
				leaveType.IsActive = true;
				leaveType.CreatedDate = DateTime.Now;
				leaveType.CreatedBy = userId;
			}
			leaveType.TypeName = model.TypeName;
			leaveType.ShartName = model.ShartName;
			leaveType.Description = model.Description;
		
			await leaveTypeRepository.SaveAsync(leaveType);
			model.LeaveTypeId = leaveType.LeaveTypeId;
			return model;
		}
	}
}
