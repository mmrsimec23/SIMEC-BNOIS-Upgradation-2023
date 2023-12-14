using System;
using System.Collections.Generic;
using System.Data;
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
	public class NominationService : INominationService
	{
		private readonly IBnoisRepository<Nomination> nominationRepository;
	    private readonly IBnoisRepository<vwNomination> vwGetNominationInfoRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public NominationService(IBnoisRepository<Nomination> nominationRepository, IBnoisRepository<vwNomination> vwGetNominationInfoRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
			this.nominationRepository = nominationRepository;
			this.vwGetNominationInfoRepository = vwGetNominationInfoRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }


        public async Task<NominationModel> GetNomination(int id)
        {
            if (id <= 0)
            {
                return new NominationModel();
            }
            Nomination nomination = await nominationRepository.FindOneAsync(x => x.NominationId == id);
            if (nomination == null)
            {
                throw new InfinityNotFoundException("Nomination not found");
            }
            NominationModel model = ObjectConverter<Nomination, NominationModel>.Convert(nomination);
            return model;
        }




	    public  string GetNominationSchedule(int id, int type)
	    {

	        vwNomination nomination =  vwGetNominationInfoRepository.FindOne(x => x.EntityId == id && x.EnitityType==type );
	        if (nomination == null)
	        {
	            throw new InfinityNotFoundException("Nomination not found");
	        }
	        return nomination.Nomination;
	    }


        public IQueryable<vwNomination> GetNominations(int ps, int pn, string qs, out int total,int type)
        {
            IQueryable<vwNomination> nominations = vwGetNominationInfoRepository.FilterWithInclude(x => x.EnitityType==type && (x.Nomination.Contains(qs) || String.IsNullOrEmpty(qs)));
            total = nominations.Count();
            nominations = nominations.OrderByDescending(x => x.NominationId).Skip((pn - 1) * ps).Take(ps);


            return nominations;
            
        }

        public async Task<NominationModel> SaveNomination(int id, NominationModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Nomination data missing");
            }
            //if (model.WithoutAppointment != true && model.MissoinAppointmentId == null)
            //{
            //    throw new InfinityArgumentMissingException("WithoutAppointment or Appoinment must be selected.");
            //}

            bool isExist = nominationRepository.Exists(x =>  x.EnitityType == model.EnitityType && x.EntityId==model.EntityId && x.MissoinAppointmentId==model.MissoinAppointmentId && x.WithoutTransfer==model.WithoutTransfer && x.NominationId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            Nomination nomination = ObjectConverter<NominationModel, Nomination>.Convert(model);
            if (id > 0)
            {
                nomination = await nominationRepository.FindOneAsync(x => x.NominationId == id);
                if (nomination == null)
                {
                    throw new InfinityNotFoundException("Nomination not found !");
                }

                nomination.ModifiedDate = DateTime.Now;
                nomination.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Nomination";
                bnLog.TableEntryForm = "Nomination";
                bnLog.PreviousValue = "Id: " + model.NominationId;
                bnLog.UpdatedValue = "Id: " + model.NominationId;
                int bnoisUpdateCount = 0;

                bnLog.PreviousValue += ", Enitity Type: " + (nomination.EnitityType == 1 ? "Course" : nomination.EnitityType == 2 ? "Mission" : nomination.EnitityType == 3 ? "Foreign Visit" : "Other");
                bnLog.UpdatedValue += ", Enitity Type: " + (model.EnitityType == 1 ? "Course" : model.EnitityType == 2 ? "Mission" : model.EnitityType == 3 ? "Foreign Visit" : "Other");
                
                if (nomination.EntityId != model.EntityId)
                {
                    bnLog.PreviousValue += ", EntityId: " + nomination.EntityId;
                    bnLog.UpdatedValue += ", EntityId: " + model.EntityId;
                    bnoisUpdateCount += 1;
                }
                if (nomination.MissoinAppointmentId != model.MissoinAppointmentId)
                {
                    if (nomination.MissoinAppointmentId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("MissionAppointment", "MissoinAppointmentId", nomination.MissoinAppointmentId??0);
                        bnLog.PreviousValue += ", Missoin Appointment: " + ((dynamic)prev).Name;
                    }
                    if (model.MissoinAppointmentId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("MissoinAppointment", "MissoinAppointmentId", model.MissoinAppointmentId??0);
                        bnLog.UpdatedValue += ", Missoin Appointment: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (nomination.EntryDate != model.EntryDate)
                {
                    bnLog.PreviousValue += ", Entry Date: " + nomination.EntryDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Entry Date: " + model.EntryDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (nomination.WithoutTransfer != model.WithoutTransfer)
                {
                    bnLog.PreviousValue += ",  Without Transfer: " + nomination.WithoutTransfer;
                    bnLog.UpdatedValue += ",  Without Transfer: " + model.WithoutTransfer;
                    bnoisUpdateCount += 1;
                }
                if (nomination.WithoutAppointment != model.WithoutAppointment)
                {
                    bnLog.PreviousValue += ",  Without Appointment: " + nomination.WithoutAppointment;
                    bnLog.UpdatedValue += ",  Without Appointment: " + model.WithoutAppointment;
                    bnoisUpdateCount += 1;
                }
                if (nomination.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + nomination.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
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
                nomination.IsActive = true;
                nomination.CreatedDate = DateTime.Now;
                nomination.CreatedBy = userId;
            }


            nomination.EntryDate = model.EntryDate;
            nomination.EnitityType = model.EnitityType;
            nomination.EntityId = model.EntityId;
            nomination.WithoutTransfer = model.WithoutTransfer;
            nomination.WithoutAppointment = model.WithoutAppointment;
            nomination.MissoinAppointmentId = model.MissoinAppointmentId;
            nomination.Remarks = model.Remarks;
            await nominationRepository.SaveAsync(nomination);
            model.NominationId = nomination.NominationId;
            return model;
        }

        public async Task<bool> DeleteNomination(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Nomination nomination = await nominationRepository.FindOneAsync(x => x.NominationId == id);
            if (nomination == null)
            {
                throw new InfinityNotFoundException("Nomination not found");
            }
            else
            {

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Nomination";
                bnLog.TableEntryForm = "Nomination";
                bnLog.PreviousValue = "Id: " + nomination.NominationId;
                
                bnLog.PreviousValue += ", Enitity Type: " + (nomination.EnitityType == 1 ? "Course" : nomination.EnitityType == 2 ? "Mission" : nomination.EnitityType == 3 ? "Foreign Visit" : "Other") + ", EntityId: " + nomination.EntityId;
                if (nomination.MissoinAppointmentId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("MissionAppointment", "MissoinAppointmentId", nomination.MissoinAppointmentId ?? 0);
                    bnLog.PreviousValue += ", Missoin Appointment: " + ((dynamic)prev).Name;
                }
                bnLog.PreviousValue += ", Entry Date: " + nomination.EntryDate?.ToString("dd/MM/yyyy") + ",  Without Transfer: " + nomination.WithoutTransfer + ",  Without Appointment: " + nomination.WithoutAppointment + ", Remarks: " + nomination.Remarks;

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await nominationRepository.DeleteAsync(nomination);
            }
        }

	    public List<SelectModel> GetNominationTypeSelectModels()
	    {
	        List<SelectModel> selectModels =
	            Enum.GetValues(typeof(NominationType)).Cast<NominationType>()
	                .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt32(v) })
	                .ToList();
	        return selectModels;
        }


	    public async Task<List<SelectModel>> GetNominationSelectModels(int type)
        	    {
        	        ICollection<vwNomination> nominations = await vwGetNominationInfoRepository.FilterWithInclude(x => x.EnitityType==type && x.WithoutTransfer==false ).ToListAsync();
        
        	        if (type == 2)
        	        {
        	            List<SelectModel> selectModels = nominations.OrderByDescending(x=>x.FromDate).Select(x => new SelectModel
        	            {
        	                Text =x.Nomination + " [" + x.MissionAppointment + "]",
        	                Value = x.NominationId
        	            }).ToList();
        
        	            return selectModels;
                    }
        	        else
        	        {
        	            List<SelectModel> selectModels = nominations.OrderByDescending(x => x.FromDate).Select(x => new SelectModel
        	            {
        	                Text = x.Nomination,
	    	                Value = x.NominationId
	    	            }).ToList();
	    	            return selectModels;
	    
	                }
	    
	               
	    	    }



    }
}
