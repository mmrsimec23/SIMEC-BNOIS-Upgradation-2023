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
	public class OfficeShipmentService : IOfficeShipmentService
	{
		private readonly IBnoisRepository<OfficeShipment> OfficeShipmentRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public OfficeShipmentService(IBnoisRepository<OfficeShipment> OfficeShipmentRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
		{
			this.OfficeShipmentRepository = OfficeShipmentRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }
      

        public async Task<OfficeShipmentModel> SaveOfficeShipment(OfficeShipmentModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("OfficeShipment data missing");
            }

           
            OfficeShipment OfficeShipment = ObjectConverter<OfficeShipmentModel, OfficeShipment>.Convert(model);
            
	      
	        OfficeShipment.OfficeId = model.OfficeId;
	        OfficeShipment.PreviousParentId = model.PreviousParentId;
	        OfficeShipment.ShipDate = model.ShipDate;
            OfficeShipment.PreZoneId = model.PreZoneId;
            OfficeShipment.PreAdminAuthorityId = model.PreAdminAuthorityId;
            OfficeShipment.PreAddress = model.PreAddress;
            await OfficeShipmentRepository.SaveAsync(OfficeShipment);
            model.OfficeMoveId = OfficeShipment.OfficeMoveId;

            // data log section start
            BnoisLog bnLog = new BnoisLog();
            bnLog.TableName = "OfficeShipment";
            bnLog.TableEntryForm = "Office Shipment";
            bnLog.PreviousValue = "Id: " + OfficeShipment.OfficeMoveId;
            if (OfficeShipment.OfficeId > 0)
            {
                var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", OfficeShipment.OfficeId??0);
                bnLog.PreviousValue += ", Office: " + ((dynamic)prev).ShortName;
            }
            if (OfficeShipment.PreviousParentId > 0)
            {
                var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", OfficeShipment.PreviousParentId ?? 0);
                bnLog.PreviousValue += ", Parent Office: " + ((dynamic)prev).ShortName;
            }
            if (OfficeShipment.PreZoneId > 0)
            {
                var prev = employeeService.GetDynamicTableInfoById("Zone", "ZoneId", OfficeShipment.PreZoneId ?? 0);
                bnLog.PreviousValue += ", Zone: " + ((dynamic)prev).Name;
            }
            if (OfficeShipment.PreAdminAuthorityId > 0)
            {
                var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", OfficeShipment.PreAdminAuthorityId ?? 0);
                bnLog.PreviousValue += ", Admin Authority: " + ((dynamic)prev).ShortName;
            }
            bnLog.PreviousValue += ", Ship Date: " + OfficeShipment.ShipDate?.ToString("dd/MM/yyyy") + ", Pre Address: " + OfficeShipment.PreAddress;

            bnLog.UpdatedValue = "This Office has been Moved!";


            bnLog.LogStatus = 1; // 1 for update, 2 for delete
            bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            bnLog.LogCreatedDate = DateTime.Now;

            await bnoisLogRepository.SaveAsync(bnLog);

            //data log section end
            return model;
        }

        public async Task<bool> DeleteOfficeShipment(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            OfficeShipment OfficeShipment = await OfficeShipmentRepository.FindOneAsync(x => x.OfficeMoveId == id);
            if (OfficeShipment == null)
            {
                throw new InfinityNotFoundException("OfficeShipment not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "OfficeShipment";
                bnLog.TableEntryForm = "Office Shipment";
                bnLog.PreviousValue = "Id: " + OfficeShipment.OfficeMoveId;
                if (OfficeShipment.OfficeId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", OfficeShipment.OfficeId ?? 0);
                    bnLog.PreviousValue += ", Office: " + ((dynamic)prev).ShortName;
                }
                if (OfficeShipment.PreviousParentId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", OfficeShipment.PreviousParentId ?? 0);
                    bnLog.PreviousValue += ", Parent Office: " + ((dynamic)prev).ShortName;
                }
                if (OfficeShipment.PreZoneId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Zone", "ZoneId", OfficeShipment.PreZoneId ?? 0);
                    bnLog.PreviousValue += ", Zone: " + ((dynamic)prev).Name;
                }
                if (OfficeShipment.PreAdminAuthorityId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", OfficeShipment.PreAdminAuthorityId ?? 0);
                    bnLog.PreviousValue += ", Admin Authority: " + ((dynamic)prev).ShortName;
                }
                bnLog.PreviousValue += ", Ship Date: " + OfficeShipment.ShipDate?.ToString("dd/MM/yyyy") + ", Pre Address: " + OfficeShipment.PreAddress;

                bnLog.UpdatedValue = "This Record has been Deleted!";


                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await OfficeShipmentRepository.DeleteAsync(OfficeShipment);
            }
        }

      


    }
}
