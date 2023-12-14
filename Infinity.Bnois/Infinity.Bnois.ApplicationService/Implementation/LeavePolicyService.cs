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
	public class LeavePolicyService : ILeavePolicyService
	{
		private readonly IBnoisRepository<LeavePolicy> leavePolicyRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public LeavePolicyService(IBnoisRepository<LeavePolicy> leavePolicyRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
			this.leavePolicyRepository = leavePolicyRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

		public async Task<bool> DeleteLeavePolicy(int id)
		{
			if (id < 0)
			{
				throw new InfinityArgumentMissingException("Invalid Request");
			}
			LeavePolicy leavePolicy = await leavePolicyRepository.FindOneAsync(x => x.LeavePolicyId == id);
			if (leavePolicy == null)
			{
				throw new InfinityNotFoundException("Leave Policy not found");
			}
			else
			{


                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "LeavePolicy";
                bnLog.TableEntryForm = "Leave Policy";
                bnLog.PreviousValue = "Id: " + leavePolicy.LeavePolicyId;
                

                if (leavePolicy.CommissionTypeId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("CommissionType", "CommissionTypeId", leavePolicy.CommissionTypeId);
                    bnLog.PreviousValue += ", Commission Type: " + ((dynamic)prev).TypeName;
                }
                if (leavePolicy.LeaveTypeId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("LeaveType", "LeaveTypeId", leavePolicy.LeaveTypeId);
                    bnLog.PreviousValue += ", Leave Type: " + ((dynamic)prev).TypeName;
                }
                bnLog.PreviousValue += ", Duration: " + leavePolicy.LeaveDuration + ", Duration Type: " + (leavePolicy.LeaveDurationType == "1" ? "Month" : leavePolicy.LeaveDurationType == "2" ? "Day" : "") + ", Slot: " + leavePolicy.Slot + ", Foreign Duration: " + leavePolicy.ForeignDuration + ", Foreign Duration Type: " + (leavePolicy.ForeignDurationType == "1" ? "Month" : leavePolicy.ForeignDurationType == "2" ? "Day" : "") + ", Foreign Duration Type: " + (leavePolicy.WATP == "Y" ? "Year" : leavePolicy.WATP == "W" ? "Whole Year" : leavePolicy.WATP == "A" ? "After" : "") + ", Tm Year: " + leavePolicy.TmYear;



                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await leavePolicyRepository.DeleteAsync(leavePolicy);
			}
		}

		public async Task<List<SelectModel>> GetDurationType()
		{
			List<SelectModel> selectModels =
				Enum.GetValues(typeof(DurationStatus)).Cast<DurationStatus>()
					.Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v).ToString() })
					.ToList();
			return selectModels;
		}

		public async Task<LeavePolicyModel> GetLeavePolicy(int id)
		{
			if (id <= 0)
			{
				return new LeavePolicyModel();
			}
			LeavePolicy LeavePolicy = await leavePolicyRepository.FindOneAsync(x => x.LeavePolicyId == id);
			if (LeavePolicy == null)
			{
				throw new InfinityNotFoundException("Leave Policy not found");
			}
			LeavePolicyModel model = ObjectConverter<LeavePolicy, LeavePolicyModel>.Convert(LeavePolicy);
			return model;
		}

		public List<LeavePolicyModel> GetLeavePolicyies(int ps, int pn, string qs, out int total)
		{
			IQueryable<LeavePolicy> leavePolicy = leavePolicyRepository.FilterWithInclude(x => x.IsActive && (x.LeaveType.TypeName.Contains(qs) || String.IsNullOrEmpty(qs)
			|| (x.CommissionType.TypeName.Contains(qs) || String.IsNullOrEmpty(qs))), "CommissionType", "LeaveType");
			total = leavePolicy.Count();
			leavePolicy = leavePolicy.OrderByDescending(x => x.LeavePolicyId).Skip((pn - 1) * ps).Take(ps);
			List<LeavePolicyModel> models = ObjectConverter<LeavePolicy, LeavePolicyModel>.ConvertList(leavePolicy.ToList()).ToList();
			return models;
		}



		public async Task<LeavePolicyModel> SaveLeavePolicy(int id, LeavePolicyModel model)
		{
			if (model == null)
			{
				throw new InfinityArgumentMissingException("Leave Policy data missing");
			}
			//bool isExist = leavePolicyRepository.Exists(x => x.TypeName == model.TypeName);
			//if (isExist)
			//{
			//	throw new InfinityInvalidDataException("Data already exists !");
			//}

			string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
			LeavePolicy leavePolicy = ObjectConverter<LeavePolicyModel, LeavePolicy>.Convert(model);
			if (id > 0)
			{
				leavePolicy = await leavePolicyRepository.FindOneAsync(x => x.LeavePolicyId == id);
				if (leavePolicy == null)
				{
					throw new InfinityNotFoundException("Leave Policy not found !");
				}

			    leavePolicy.ModifiedDate = DateTime.Now;
			    leavePolicy.ModifiedBy = userId;


                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "LeavePolicy";
                bnLog.TableEntryForm = "Leave Policy";
                bnLog.PreviousValue = "Id: " + model.LeavePolicyId;
                bnLog.UpdatedValue = "Id: " + model.LeavePolicyId;
                int bnoisUpdateCount = 0;
                if (leavePolicy.CommissionTypeId != model.CommissionTypeId)
                {
                    if (leavePolicy.CommissionTypeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("CommissionType", "CommissionTypeId", leavePolicy.CommissionTypeId);
                        bnLog.PreviousValue += ", Commission Type: " + ((dynamic)prev).TypeName;
                    }
                    if (model.CommissionTypeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("CommissionType", "CommissionTypeId", model.CommissionTypeId);
                        bnLog.UpdatedValue += ", Commission Type: " + ((dynamic)prev).TypeName;
                    }
                }
                if (leavePolicy.LeaveTypeId != model.LeaveTypeId)
                {
                    if (leavePolicy.LeaveTypeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("LeaveType", "LeaveTypeId", leavePolicy.LeaveTypeId);
                        bnLog.PreviousValue += ", Leave Type: " + ((dynamic)prev).TypeName;
                    }
                    if (model.LeaveTypeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("LeaveType", "LeaveTypeId", model.LeaveTypeId);
                        bnLog.UpdatedValue += ", Leave Type: " + ((dynamic)prev).TypeName;
                    }
                }
                if (leavePolicy.LeaveDuration != model.LeaveDuration)
                {
                    bnLog.PreviousValue += ", Duration: " + leavePolicy.LeaveDuration;
                    bnLog.UpdatedValue += ", Duration: " + model.LeaveDuration;
                    bnoisUpdateCount += 1;
                }
                if (leavePolicy.LeaveDurationType != model.LeaveDurationType)
                {
                    bnLog.PreviousValue += ", Duration Type: " + (leavePolicy.LeaveDurationType == "1" ? "Month" : leavePolicy.LeaveDurationType == "2" ? "Day" : "");
                    bnLog.UpdatedValue += ", Duration Type: " + (model.LeaveDurationType == "1" ? "Month" : model.LeaveDurationType == "2" ? "Day" : "");
                    bnoisUpdateCount += 1;
                }
                if (leavePolicy.Slot != model.Slot)
                {
                    bnLog.PreviousValue += ", Slot: " + leavePolicy.Slot;
                    bnLog.UpdatedValue += ", Slot: " + model.Slot;
                    bnoisUpdateCount += 1;
                }
                if (leavePolicy.ForeignDuration != model.ForeignDuration)
                {
                    bnLog.PreviousValue += ", Foreign Duration: " + leavePolicy.ForeignDuration;
                    bnLog.UpdatedValue += ", Foreign Duration: " + model.ForeignDuration;
                    bnoisUpdateCount += 1;
                }
                if (leavePolicy.ForeignDurationType != model.ForeignDurationType)
                {
                    bnLog.PreviousValue += ", Foreign Duration Type: " + (leavePolicy.ForeignDurationType == "1" ? "Month" : leavePolicy.ForeignDurationType == "2" ? "Day" : "");
                    bnLog.UpdatedValue += ", Foreign Duration Type: " + (model.ForeignDurationType == "1" ? "Month" : model.ForeignDurationType == "2" ? "Day" : "");
                    bnoisUpdateCount += 1;
                }
                if (leavePolicy.WATP != model.WATP)
                {
                    bnLog.PreviousValue += ", Foreign Duration Type: " + (leavePolicy.WATP == "Y" ? "Year" : leavePolicy.WATP == "W" ? "Whole Year" : leavePolicy.WATP == "A" ? "After" : "");
                    bnLog.UpdatedValue += ", Foreign Duration Type: " + (model.WATP == "Y" ? "Year" : model.WATP == "W" ? "Whole Year" : model.WATP == "A" ? "After" : "");
                    bnoisUpdateCount += 1;
                }
                if (leavePolicy.TmYear != model.TmYear)
                {
                    bnLog.PreviousValue += ", Tm Year: " + leavePolicy.TmYear;
                    bnLog.UpdatedValue += ", Tm Year: " + model.TmYear;
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
				leavePolicy.IsActive = true;
				leavePolicy.CreatedDate = DateTime.Now;
				leavePolicy.CreatedBy = userId;
			}
			leavePolicy.CommissionTypeId = model.CommissionTypeId;
			leavePolicy.LeaveTypeId = model.LeaveTypeId;
			leavePolicy.LeaveDuration = model.LeaveDuration;
			leavePolicy.LeaveDurationType = model.LeaveDurationType != null ? model.LeaveDurationType.Trim() : model.LeaveDurationType;
			leavePolicy.Slot = model.Slot;
			leavePolicy.ForeignDuration = model.ForeignDuration;
			leavePolicy.ForeignDurationType = model.ForeignDurationType != null ? model.ForeignDurationType.Trim() : model.ForeignDurationType;
			leavePolicy.TmYear = model.WATP == "Y" ? 1 : model.TmYear;
			leavePolicy.WATP = model.WATP;

			await leavePolicyRepository.SaveAsync(leavePolicy);
			model.LeavePolicyId = leavePolicy.LeavePolicyId;
			return model;
		}
	}
}
