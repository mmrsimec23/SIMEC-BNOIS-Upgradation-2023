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
	public class LprCalculateInfoService : ILprCalculateInfoService
	{
		private readonly IBnoisRepository<LprCalculateInfo> lprCalculationRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public LprCalculateInfoService(IBnoisRepository<LprCalculateInfo> lprCalculationRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
			this.lprCalculationRepository = lprCalculationRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

		public async Task<bool> DeleteLprCalculate(int id)
		{
			if (id < 0)
			{
				throw new InfinityArgumentMissingException("Invalid Request");
			}
			LprCalculateInfo lprCalculateInfo = await lprCalculationRepository.FindOneAsync(x => x.LprCalculateId == id);
			if (lprCalculateInfo == null)
			{
				throw new InfinityNotFoundException("LPR Calculate not found");
			}
			else
			{
                // data log section start
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "LprCalculateInfo";
                bnLog.TableEntryForm = "LPR Calculate Info";
                bnLog.PreviousValue = "Id: " + lprCalculateInfo.LprCalculateId;

                if (lprCalculateInfo.EmployeeId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", lprCalculateInfo.EmployeeId);
                    bnLog.PreviousValue += ", P No: " + ((dynamic)prev).PNo;
                }
                
                bnLog.PreviousValue += ", Leave as Sailor: " + lprCalculateInfo.SailorDue + ", Privilege Leave: " + lprCalculateInfo.LPR + ", Furlough Leave: " + lprCalculateInfo.FlLeave + ", Terminal Leave: " + lprCalculateInfo.TerminalLeave + ", Survey Leave: " + lprCalculateInfo.SurveyLeave;
                

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await lprCalculationRepository.DeleteAsync(lprCalculateInfo);
			}
		}

		public async Task<LprCalculateInfoModel> GetLprCalculate(int id)
		{
			if (id <= 0)
			{
				return new LprCalculateInfoModel();
			}
			LprCalculateInfo lprCalculateInfo = await lprCalculationRepository.FindOneAsync(x => x.LprCalculateId == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch" });
			if (lprCalculateInfo == null)
			{
				throw new InfinityNotFoundException("LPR Calculate not found");
			}
			LprCalculateInfoModel model = ObjectConverter<LprCalculateInfo, LprCalculateInfoModel>.Convert(lprCalculateInfo);
			return model;
		}

		public async Task<LprCalculateInfoModel> GetLprCalculateByEmpId(int employeeId)
		{
			if (employeeId <= 0)
			{
				return new LprCalculateInfoModel();
			}
			LprCalculateInfo lprCalculateInfo = await lprCalculationRepository.FindOneAsync(x => x.EmployeeId == employeeId);
			if (lprCalculateInfo == null)
			{
				throw new InfinityNotFoundException("LPR Calculate not found");
			}
			LprCalculateInfoModel model = ObjectConverter<LprCalculateInfo, LprCalculateInfoModel>.Convert(lprCalculateInfo);
			return model;
		}

		public List<LprCalculateInfoModel> GetLprCalculates(int ps, int pn, string qs, out int total)
		{
			IQueryable<LprCalculateInfo> lprCalculateInfo = lprCalculationRepository.FilterWithInclude(x => x.IsActive && (x.Employee.PNo.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee");
			total = lprCalculateInfo.Count();
			lprCalculateInfo = lprCalculateInfo.OrderByDescending(x => x.LprCalculateId).Skip((pn - 1) * ps).Take(ps);
			List<LprCalculateInfoModel> models = ObjectConverter<LprCalculateInfo, LprCalculateInfoModel>.ConvertList(lprCalculateInfo.ToList()).ToList();
			return models;
		}

		public async Task<LprCalculateInfoModel> SaveLprCalculate(int id, LprCalculateInfoModel model)
		{
			if (model == null)
			{
				throw new InfinityArgumentMissingException("LPR Calculate data missing");
			}


		    bool isExist = lprCalculationRepository.Exists(x => x.LprCalculateId != model.LprCalculateId && x.EmployeeId == model.Employee.EmployeeId);
		    if (isExist)
		    {
		        throw new InfinityInvalidDataException("Data already exists !");
		    }



            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
			LprCalculateInfo lprCalculate = ObjectConverter<LprCalculateInfoModel, LprCalculateInfo>.Convert(model);
			if (id > 0)
			{
				lprCalculate = await lprCalculationRepository.FindOneAsync(x => x.LprCalculateId == id);
				if (lprCalculate == null)
				{
					throw new InfinityNotFoundException("LPR Calculate not found !");
				}

				lprCalculate.ModifiedDate = DateTime.Now;
				lprCalculate.ModifiedBy = userId;


                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "LprCalculateInfo";
                bnLog.TableEntryForm = "LPR Calculate Info";
                bnLog.PreviousValue = "Id: " + model.LprCalculateId;
                bnLog.UpdatedValue = "Id: " + model.LprCalculateId;
                int bnoisUpdateCount = 0;
                if (lprCalculate.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    if (lprCalculate.EmployeeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", lprCalculate.EmployeeId);
                        bnLog.PreviousValue += ", P No: " + ((dynamic)prev).PNo;
                    }
                    if (model.EmployeeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                        bnLog.UpdatedValue += ", P No: " + ((dynamic)prev).PNo;
                    }
                }
                if (lprCalculate.SailorDue != model.SailorDue)
                {
                    bnLog.PreviousValue += ", Leave as Sailor: " + lprCalculate.SailorDue;
                    bnLog.UpdatedValue += ", Leave as Sailor: " + model.SailorDue;
                    bnoisUpdateCount += 1;
                }
                if (lprCalculate.LPR != model.LPR)
                {
                    bnLog.PreviousValue += ", Privilege Leave: " + lprCalculate.LPR;
                    bnLog.UpdatedValue += ", Privilege Leave: " + model.LPR;
                    bnoisUpdateCount += 1;
                }
                if (lprCalculate.FlLeave != model.FlLeave)
                {
                    bnLog.PreviousValue += ", Furlough Leave: " + lprCalculate.FlLeave;
                    bnLog.UpdatedValue += ", Furlough Leave: " + model.FlLeave;
                    bnoisUpdateCount += 1;
                }
                if (lprCalculate.TerminalLeave != model.TerminalLeave)
                {
                    bnLog.PreviousValue += ", Terminal Leave: " + lprCalculate.TerminalLeave;
                    bnLog.UpdatedValue += ", Terminal Leave: " + model.TerminalLeave;
                    bnoisUpdateCount += 1;
                }
                if (lprCalculate.SurveyLeave != model.SurveyLeave)
                {
                    bnLog.PreviousValue += ", Survey Leave: " + lprCalculate.SurveyLeave;
                    bnLog.UpdatedValue += ", Survey Leave: " + model.SurveyLeave;
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
				lprCalculate.IsActive = true;
				lprCalculate.CreatedDate = DateTime.Now;
				lprCalculate.CreatedBy = userId;
			}

			lprCalculate.EmployeeId = model.Employee.EmployeeId;
			lprCalculate.LprCalculateId = model.LprCalculateId;
			lprCalculate.SailorDue = model.SailorDue;
			lprCalculate.LPR = model.LPR;
			lprCalculate.FlLeave = model.FlLeave;
			lprCalculate.TerminalLeave = model.TerminalLeave;
			lprCalculate.SurveyLeave = model.SurveyLeave;
			lprCalculate.Employee = null;
			await lprCalculationRepository.SaveAsync(lprCalculate);
			return model;
		}
	}
}
