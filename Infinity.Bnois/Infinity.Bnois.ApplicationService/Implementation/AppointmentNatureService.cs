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
	public class AppointmentNatureService : IAppointmentNatureService
	{
		private readonly IBnoisRepository<AptNat> appoitmentNatureRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public AppointmentNatureService(IBnoisRepository<AptNat> appoitmentNatureRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
			this.appoitmentNatureRepository = appoitmentNatureRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

		public async Task<bool> DeleteAppointmentNature(int id)
		{
			if (id < 0)
			{
				throw new InfinityArgumentMissingException("Invalid Request");
			}
			AptNat appointmentNature = await appoitmentNatureRepository.FindOneAsync(x => x.ANatId == id);
			if (appointmentNature == null)
			{
				throw new InfinityNotFoundException("Appointment Nature not found");
			}
			else
			{
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "AptNat";
                bnLog.TableEntryForm = "Appointment Nature";
                bnLog.PreviousValue = "Id: " + appointmentNature.ANatId;
                bnLog.PreviousValue += ", Appointment Nature: " + appointmentNature.ANatNm + ", Appointment Nature(Bangla): " + appointmentNature.ANatNmBng + ", Short Name: " + appointmentNature.ANatShnm + ", Short Name(Bangla): " + appointmentNature.ANatShnmBng + ",  Outside Navy: " + appointmentNature.AptFr + ", Staff Duties: " + appointmentNature.IsStaffDuty;
                
                bnLog.UpdatedValue = "This Record has been Deleted!";
                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);
                // data log section end
                return await appoitmentNatureRepository.DeleteAsync(appointmentNature);
			}
		}

		public async Task<AptNatModel> GetAppointmentNature(int id)
		{
			if (id <= 0)
			{
				return new AptNatModel();
			}
			AptNat AppointmentNature = await appoitmentNatureRepository.FindOneAsync(x => x.ANatId == id);
			if (AppointmentNature == null)
			{
				throw new InfinityNotFoundException("Appointment Nature not found");
			}
			AptNatModel model = ObjectConverter<AptNat, AptNatModel>.Convert(AppointmentNature);
			return model;
		}

		public List<AptNatModel> GetAppointmentNaturies(int ps, int pn, string qs, out int total)
		{
			IQueryable<AptNat> AppointmentNaturies = appoitmentNatureRepository.FilterWithInclude(x => x.IsActive && (x.ANatNm.Contains(qs)|| x.ANatShnm.Contains(qs) || String.IsNullOrEmpty(qs)));
			total = AppointmentNaturies.Count();
			AppointmentNaturies = AppointmentNaturies.OrderByDescending(x => x.ANatId).Skip((pn - 1) * ps).Take(ps);
			List<AptNatModel> models = ObjectConverter<AptNat, AptNatModel>.ConvertList(AppointmentNaturies.ToList()).ToList();
			return models;
		}

        public async Task<List<SelectModel>> GetNatureSelectList()
        {
            ICollection<AptNat> aptNature = await appoitmentNatureRepository.FilterAsync(x => x.IsActive);
           List<AptNat> query= aptNature.OrderBy(x => x.ANatNm).ToList();
            List<SelectModel> selectModels = query.Select(x => new SelectModel
            {
                Text = x.ANatNm,
                Value = x.ANatId
            }).ToList();
            return selectModels;
        }

        public async Task<AptNatModel> SaveAppointmentNature(int id, AptNatModel model)
		{
			if (model == null)
			{
				throw new InfinityArgumentMissingException("Appointment Nature data missing");
			}
			bool isExist = appoitmentNatureRepository.Exists(x => x.ANatNm == model.ANatNm && x.ANatId != id);
			if (isExist)
			{
				throw new InfinityInvalidDataException("Appointment Nature exists !");
			}

			string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
			AptNat AppointmentNature = ObjectConverter<AptNatModel, AptNat>.Convert(model);
			if (id > 0)
			{
				AppointmentNature = await appoitmentNatureRepository.FindOneAsync(x => x.ANatId == id);
				if (AppointmentNature == null)
				{
					throw new InfinityNotFoundException("Appointment Nature not found !");
				}

				AppointmentNature.ModifiedDate = DateTime.Now;
				AppointmentNature.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "AptNat";
                bnLog.TableEntryForm = "Appointment Nature";
                bnLog.PreviousValue = "Id: " + model.ANatId;
                bnLog.UpdatedValue = "Id: " + model.ANatId;
                int bnoisUpdateCount = 0;

                
                if (AppointmentNature.ANatNm != model.ANatNm)
                {
                    bnLog.PreviousValue += ", Appointment Nature: " + AppointmentNature.ANatNm;
                    bnLog.UpdatedValue += ", Appointment Nature: " + model.ANatNm;
                    bnoisUpdateCount += 1;
                }
                if (AppointmentNature.ANatNmBng != model.ANatNmBng)
                {
                    bnLog.PreviousValue += ", Appointment Nature(Bangla): " + AppointmentNature.ANatNmBng;
                    bnLog.UpdatedValue += ", Appointment Nature(Bangla): " + model.ANatNmBng;
                    bnoisUpdateCount += 1;
                }
                if (AppointmentNature.ANatShnm != model.ANatShnm)
                {
                    bnLog.PreviousValue += ", Short Name: " + AppointmentNature.ANatShnm;
                    bnLog.UpdatedValue += ", Short Name: " + model.ANatShnm;
                    bnoisUpdateCount += 1;
                }
                if (AppointmentNature.ANatShnmBng != model.ANatShnmBng)
                {
                    bnLog.PreviousValue += ", Short Name(Bangla): " + AppointmentNature.ANatShnmBng;
                    bnLog.UpdatedValue += ", Short Name(Bangla): " + model.ANatShnmBng;
                    bnoisUpdateCount += 1;
                }
                if (AppointmentNature.AptFr != model.AptFr)
                {
                    bnLog.PreviousValue += ",  Outside Navy: " + AppointmentNature.AptFr;
                    bnLog.UpdatedValue += ",  Outside Navy: " + model.AptFr;
                    bnoisUpdateCount += 1;
                }
                if (AppointmentNature.IsStaffDuty != model.IsStaffDuty)
                {
                    bnLog.PreviousValue += ", Staff Duties: " + AppointmentNature.IsStaffDuty;
                    bnLog.UpdatedValue += ", Staff Duties: " + model.IsStaffDuty;
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
				AppointmentNature.IsActive = true;
				AppointmentNature.CreatedDate = DateTime.Now;
				AppointmentNature.CreatedBy = userId;
			}
			AppointmentNature.ANatNm = model.ANatNm;
			AppointmentNature.ANatNmBng = model.ANatNmBng;
			AppointmentNature.ANatShnm = model.ANatShnm;
			AppointmentNature.ANatShnmBng = model.ANatShnmBng;
			AppointmentNature.AptFr = model.AptFr;
            AppointmentNature.IsStaffDuty = model.IsStaffDuty;
            await appoitmentNatureRepository.SaveAsync(AppointmentNature);
			model.ANatId = AppointmentNature.ANatId;
			return model;
		}
	}
}
