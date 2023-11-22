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
	public class AppointmentCategoryService : IAppointmentCategoryService
	{
		private readonly IBnoisRepository<AptCat> appoitmentCategoryRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public AppointmentCategoryService(IBnoisRepository<AptCat> appoitmentCategoryRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
			this.appoitmentCategoryRepository = appoitmentCategoryRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

		public async Task<bool> DeleteAppointmentCategory(int id)
		{
			if (id < 0)
			{
				throw new InfinityArgumentMissingException("Invalid Request");
			}
			AptCat appointmentCategory = await appoitmentCategoryRepository.FindOneAsync(x => x.AcatId == id);
			if (appointmentCategory == null)
			{
				throw new InfinityNotFoundException("Appointment Category not found");
			}
			else
			{
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "AptCat";
                bnLog.TableEntryForm = "Appointment Category";
                bnLog.PreviousValue = "Id: " + appointmentCategory.AcatId;
                if (appointmentCategory.ANatId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("AptNat", "ANatId", appointmentCategory.ANatId);
                    bnLog.PreviousValue += ", Appointment Nature: " + ((dynamic)prev).ANatNm;
                }
                bnLog.PreviousValue += ", Appointment Category: " + appointmentCategory.AcatNm + ", Appointment Category(Bangla): " + appointmentCategory.AcatNmBng + ", Short Name: " + appointmentCategory.ACatShNm + ", Short Name(Bangla): " + appointmentCategory.ACatShNmBng + ", Go To ACR: " + appointmentCategory.GoAcr;
                
                bnLog.UpdatedValue = "This Record has been Deleted!";
                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);
                return await appoitmentCategoryRepository.DeleteAsync(appointmentCategory);
			}
		}


	    public async Task<AptCatModel> GetAppointmentCategory(int id)
		{
			if (id <= 0)
			{
				return new AptCatModel();
			}
			AptCat AppointmentCategory = await appoitmentCategoryRepository.FindOneAsync(x => x.AcatId == id);
			if (AppointmentCategory == null)
			{
				throw new InfinityNotFoundException("Appointment Category not found");
			}
			AptCatModel model = ObjectConverter<AptCat, AptCatModel>.Convert(AppointmentCategory);
			return model;
		}

		public List<AptCatModel> GetAppointmentCategorys(int ps, int pn, string qs, out int total)
		{
			IQueryable<AptCat> AppointmentCategories = appoitmentCategoryRepository.FilterWithInclude(x => x.IsActive && (x.AcatNm.Contains(qs)|| x.ACatShNm.Contains(qs) || x.AptNat.ANatNm.Contains(qs) || String.IsNullOrEmpty(qs)), "AptNat");
			total = AppointmentCategories.Count();
			AppointmentCategories = AppointmentCategories.OrderByDescending(x => x.AcatId).Skip((pn - 1) * ps).Take(ps);
			List<AptCatModel> models = ObjectConverter<AptCat, AptCatModel>.ConvertList(AppointmentCategories.ToList()).ToList();
			return models;
		}

		public async Task<AptCatModel> SaveAppointmentCategory(int id, AptCatModel model)
		{
			if (model == null)
			{
				throw new InfinityArgumentMissingException("Appointment Category data missing");
			}
			bool isExist = appoitmentCategoryRepository.Exists(x => x.AcatNm == model.AcatNm && x.ANatId==model.ANatId && x.AcatId != id);
			if (isExist)
			{
				throw new InfinityInvalidDataException("Appointment Category exists !");
			}

			string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
			AptCat AppointmentCategory = ObjectConverter<AptCatModel, AptCat>.Convert(model);
			if (id > 0)
			{
				AppointmentCategory = await appoitmentCategoryRepository.FindOneAsync(x => x.AcatId == id);
				if (AppointmentCategory == null)
				{
					throw new InfinityNotFoundException("Appointment Category not found !");
				}

				AppointmentCategory.ModifiedDate = DateTime.Now;
				AppointmentCategory.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "AptCat";
                bnLog.TableEntryForm = "Appointment Category";
                bnLog.PreviousValue = "Id: " + model.AcatId;
                bnLog.UpdatedValue = "Id: " + model.AcatId;
                int bnoisUpdateCount = 0;

                if (AppointmentCategory.ANatId != model.ANatId)
                {
                    if (AppointmentCategory.ANatId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("AptNat", "ANatId", AppointmentCategory.ANatId);
                        bnLog.PreviousValue += ", Appointment Nature: " + ((dynamic)prev).ANatNm;
                    }
                    if (model.ANatId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("AptNat", "ANatId", model.ANatId);
                        bnLog.UpdatedValue += ", Appointment Nature: " + ((dynamic)newv).ANatNm;
                    }
					bnoisUpdateCount += 1;
                }
                if (AppointmentCategory.AcatNm != model.AcatNm)
                {
                    bnLog.PreviousValue += ", Appointment Category: " + AppointmentCategory.AcatNm;
                    bnLog.UpdatedValue += ", Appointment Category: " + model.AcatNm;
                    bnoisUpdateCount += 1;
                }
                if (AppointmentCategory.AcatNmBng != model.AcatNmBng)
                {
                    bnLog.PreviousValue += ", Appointment Category(Bangla): " + AppointmentCategory.AcatNmBng;
                    bnLog.UpdatedValue += ", Appointment Category(Bangla): " + model.AcatNmBng;
                    bnoisUpdateCount += 1;
                }
                if (AppointmentCategory.ACatShNm != model.ACatShNm)
                {
                    bnLog.PreviousValue += ", Short Name: " + AppointmentCategory.ACatShNm;
                    bnLog.UpdatedValue += ", Short Name: " + model.ACatShNm;
                    bnoisUpdateCount += 1;
                }
                if (AppointmentCategory.ACatShNmBng != model.ACatShNmBng)
                {
                    bnLog.PreviousValue += ", Short Name(Bangla): " + AppointmentCategory.ACatShNmBng;
                    bnLog.UpdatedValue += ", Short Name(Bangla): " + model.ACatShNmBng;
                    bnoisUpdateCount += 1;
                }
                if (AppointmentCategory.GoAcr != model.GoAcr)
                {
                    bnLog.PreviousValue += ", Go To ACR: " + AppointmentCategory.GoAcr;
                    bnLog.UpdatedValue += ", Go To ACR: " + model.GoAcr;
                    bnoisUpdateCount += 1;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = model.CreatedBy;
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
				AppointmentCategory.IsActive = true;
				AppointmentCategory.CreatedDate = DateTime.Now;
				AppointmentCategory.CreatedBy = userId;
			}
			AppointmentCategory.ANatId = model.ANatId;
			AppointmentCategory.AcatNm = model.AcatNm;
			AppointmentCategory.AcatNmBng = model.AcatNmBng;
			AppointmentCategory.ACatShNm = model.ACatShNm;
			AppointmentCategory.ACatShNmBng = model.ACatShNmBng;
			AppointmentCategory.GoAcr = model.GoAcr;

			await appoitmentCategoryRepository.SaveAsync(AppointmentCategory);
			model.AcatId = AppointmentCategory.AcatId;
			return model;
		}


	    public async Task<List<SelectModel>> GetCategorySelectListByNature(int id)
	    {
	        ICollection<AptCat> aptCats = await appoitmentCategoryRepository.FilterAsync(x => x.IsActive && x.ANatId== id);
	        List<SelectModel> selectModels = aptCats.OrderBy(x=>x.AcatNm).Select(x => new SelectModel
	        {
	            Text = x.AcatNm,
	            Value = x.AcatId
	        }).ToList();
	        return selectModels;
	    }

	    public async Task<List<SelectModel>> GetCategorySelectModel()
	    {
	        ICollection<AptCat> aptCats = await appoitmentCategoryRepository.FilterAsync(x => x.IsActive &&(x.AptNat.ANatNm.Contains("Inside Navy")|| x.AptNat.ANatNm.Contains("UN Mission")));
	        List<SelectModel> selectModels = aptCats.OrderBy(x=>x.AcatNm).Select(x => new SelectModel
	        {
	            Text = x.AcatNm,
	            Value = x.AcatId
	        }).ToList();
	        return selectModels;
	    }


    }


  

}
