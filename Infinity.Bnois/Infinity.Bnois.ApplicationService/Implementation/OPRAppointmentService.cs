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
	public class OPRAppointmentService : IOPRAppointmentService
	{
		private readonly IBnoisRepository<OprAptSuitability> OPRAppointmentRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public OPRAppointmentService(IBnoisRepository<OprAptSuitability> OPRAppointmentRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
			this.OPRAppointmentRepository = OPRAppointmentRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }



        public List<OprAptSuitabilityModel> GetOPRAppointments(int id)
        {
            List<OprAptSuitability> OPRAppointments = OPRAppointmentRepository.FilterWithInclude(x => x.EmployeeOprId == id , "SpecialAptType","Suitability").ToList();
            List<OprAptSuitabilityModel> models = ObjectConverter<OprAptSuitability, OprAptSuitabilityModel>.ConvertList(OPRAppointments.ToList()).ToList();
            return models;

        }



        public async Task<OprAptSuitabilityModel> SaveOPRAppointment(int id, OprAptSuitabilityModel model)
        {
           
            if (model == null)
            {
                throw new InfinityArgumentMissingException("OPR Special Appointment data missing");
            }
            bool isExist = OPRAppointmentRepository.Exists(x => x.EmployeeOprId == model.EmployeeOprId && x.SpecialAptTypeId==model.SpecialAptTypeId && x.SuitabilityId == model.SuitabilityId && x.Id != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }


            OprAptSuitability OPRAppointment = ObjectConverter<OprAptSuitabilityModel, OprAptSuitability>.Convert(model);
            if (id > 0)
            {
                OPRAppointment = await OPRAppointmentRepository.FindOneAsync(x => x.Id == id);
                if (OPRAppointment == null)
                {
                    throw new InfinityNotFoundException("OPR Special Appointment not found !");
                }
               
            }
           
            OPRAppointment.EmployeeOprId = model.EmployeeOprId;
            OPRAppointment.SpecialAptTypeId = model.SpecialAptTypeId;
            OPRAppointment.SuitabilityId = model.SuitabilityId;
            
            OPRAppointment.Note = model.Note;
            await OPRAppointmentRepository.SaveAsync(OPRAppointment);
            model.Id = OPRAppointment.Id;
            return model;
        }


	 
        public async Task<bool> DeleteOPRAppointment(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            OprAptSuitability OPRAppointment = await OPRAppointmentRepository.FindOneAsync(x => x.Id == id);
            if (OPRAppointment == null)
            {
                throw new InfinityNotFoundException("OPR Special Appointment not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "OprAptSuitability";
                bnLog.TableEntryForm = "Suitability for Special Type of Appointment (Section-VI)";
                bnLog.PreviousValue = "Id: " + OPRAppointment.Id;

                bnLog.PreviousValue += ", EmployeeOprId: " + OPRAppointment.EmployeeOprId;

                if (OPRAppointment.SpecialAptTypeId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("SpecialAptType", "Id", OPRAppointment.SpecialAptTypeId);
                    bnLog.PreviousValue += ", Special Type of Appointment: " + ((dynamic)prev).SpAptType;
                }
                if (OPRAppointment.SuitabilityId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Suitability", "Id", OPRAppointment.SuitabilityId);
                    bnLog.PreviousValue += ", Suitability: " + ((dynamic)prev).SuitabilityName;
                }
                bnLog.PreviousValue += ", Note: " + OPRAppointment.Note;
                
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await OPRAppointmentRepository.DeleteAsync(OPRAppointment);
            }
        }

    }
}
