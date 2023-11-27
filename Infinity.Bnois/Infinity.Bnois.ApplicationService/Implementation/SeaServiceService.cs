using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class SeaServiceService : ISeaServiceService
    {
        private readonly IBnoisRepository<SeaService> seaServiceRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public SeaServiceService(IBnoisRepository<SeaService> seaServiceRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.seaServiceRepository = seaServiceRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<SeaServiceModel> GetSeaServices(int ps, int pn, string qs, out int total)
        {
            IQueryable<SeaService> seaServices = seaServiceRepository.FilterWithInclude(x => x.IsActive
                && (x.Employee.PNo.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee", "Country");
            total = seaServices.Count();
	        seaServices = seaServices.OrderByDescending(x => x.SeaServiceId).Skip((pn - 1) * ps).Take(ps);
            List<SeaServiceModel> models = ObjectConverter<SeaService, SeaServiceModel>.ConvertList(seaServices.ToList()).ToList();
            models = models.Select(x =>
            {
                x.ShipTypeName = Enum.GetName(typeof(ShipType), x.ShipType);
                return x;
            }).ToList();
            return models;
        }

        public async Task<SeaServiceModel> GetSeaService(int id)
        {
            if (id <= 0)
            {
                return new SeaServiceModel();
            }
            SeaService seaService = await seaServiceRepository.FindOneAsync(x => x.SeaServiceId == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch" });
            if (seaService == null)
            {
                throw new InfinityNotFoundException("Training Result not found");
            }
            SeaServiceModel model = ObjectConverter<SeaService, SeaServiceModel>.Convert(seaService);
            return model;
        }


        public async Task<SeaServiceModel> SaveSeaService(int id, SeaServiceModel model)
        {
            model.Employee = null;
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Training Result  data missing");
            }



            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            SeaService seaService = ObjectConverter<SeaServiceModel, SeaService>.Convert(model);
            if (id > 0)
            {
	            seaService = await seaServiceRepository.FindOneAsync(x => x.SeaServiceId == id);
                if (seaService == null)
                {
                    throw new InfinityNotFoundException("Training Result not found !");
                }

	            seaService.ModifiedDate = DateTime.Now;
	            seaService.ModifiedBy = userId;


                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "SeaService";
                bnLog.TableEntryForm = "Sea Service";
                bnLog.PreviousValue = "Id: " + model.SeaServiceId;
                bnLog.UpdatedValue = "Id: " + model.SeaServiceId;
                int bnoisUpdateCount = 0;
                if (seaService.EmployeeId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", seaService.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prev).PNo;
                }
                if (model.EmployeeId > 0)
                {
                    var newv = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)newv).PNo;
                }

                if (seaService.CountryId != model.CountryId)
                {
                    if (seaService.CountryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Country", "CountryId", seaService.CountryId??0);
                        bnLog.PreviousValue += ", Country: " + ((dynamic)prev).FullName;
                    }
                    if (model.CountryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Country", "CountryId", model.CountryId ?? 0);
                        bnLog.UpdatedValue += ", Country: " + ((dynamic)newv).FullName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (seaService.ShipType != model.ShipType)
                {
                    bnLog.PreviousValue += ", Ship Type: " + (seaService.ShipType == 1 ? "Small" : seaService.ShipType == 2 ? "Medium" : seaService.ShipType == 3 ? "Large" : "");
                    bnLog.UpdatedValue += ", Ship Type: " + (model.ShipType == 1 ? "Small" : model.ShipType == 2 ? "Medium" : model.ShipType == 3 ? "Large" : "");
                    bnoisUpdateCount += 1;
                }
                
                if (seaService.ShipName != model.ShipName)
                {
                    bnLog.PreviousValue += ",  Ship Name: " + seaService.ShipName;
                    bnLog.UpdatedValue += ",  Ship Name: " + model.ShipName;
                    bnoisUpdateCount += 1;
                }
                if (seaService.OrganizationName != model.OrganizationName)
                {
                    bnLog.PreviousValue += ",  Organization Name: " + seaService.OrganizationName;
                    bnLog.UpdatedValue += ",  Organization Name: " + model.OrganizationName;
                    bnoisUpdateCount += 1;
                }
                if (seaService.AppointmentName != model.AppointmentName)
                {
                    bnLog.PreviousValue += ", Appointment: " + seaService.AppointmentName;
                    bnLog.UpdatedValue += ", Appointment: " + model.AppointmentName;
                    bnoisUpdateCount += 1;
                }
                if (seaService.FromDate != model.FromDate)
                {
                    bnLog.PreviousValue += ", From Date: " + seaService.FromDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", From Date: " + model.FromDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (seaService.ToDate != model.ToDate)
                {
                    bnLog.PreviousValue += ", To Date: " + seaService.ToDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", To Date: " + model.ToDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (seaService.Purpose != model.Purpose)
                {
                    bnLog.PreviousValue += ",  Purpose: " + seaService.Purpose;
                    bnLog.UpdatedValue += ",  Purpose: " + model.Purpose;
                    bnoisUpdateCount += 1;
                }
                if (seaService.Reference != model.Reference)
                {
                    bnLog.PreviousValue += ", Reference: " + seaService.Reference;
                    bnLog.UpdatedValue += ", Reference: " + model.Reference;
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
	            seaService.IsActive = true;
	            seaService.CreatedDate = DateTime.Now;
	            seaService.CreatedBy = userId;
            }
	        seaService.EmployeeId = model.EmployeeId;
	        seaService.CountryId = model.CountryId;
	        seaService.ShipName = model.ShipName;
	        seaService.ShipType = model.ShipType;
	        seaService.OrganizationName = model.OrganizationName;
	        seaService.AppointmentName = model.AppointmentName;
	        seaService.FromDate = model.FromDate ?? seaService.FromDate;
	        seaService.ToDate = model.ToDate?? seaService.ToDate;
	        seaService.Purpose = model.Purpose;
	        seaService.Reference = model.Reference;

            await seaServiceRepository.SaveAsync(seaService);
            model.SeaServiceId = seaService.SeaServiceId;
            return model;
        }


        public async Task<bool> DeleteSeaService(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            SeaService seaService = await seaServiceRepository.FindOneAsync(x => x.SeaServiceId == id);
            if (seaService == null)
            {
                throw new InfinityNotFoundException("Training Result not found");
            }
            else
            {

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "SeaService";
                bnLog.TableEntryForm = "Sea Service";
                bnLog.PreviousValue = "Id: " + seaService.SeaServiceId;
                bnLog.UpdatedValue = "Id: " + seaService.SeaServiceId;
                if (seaService.EmployeeId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", seaService.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prev).PNo;
                }

                if (seaService.CountryId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Country", "CountryId", seaService.CountryId ?? 0);
                    bnLog.PreviousValue += ", Country: " + ((dynamic)prev).FullName;
                }
                bnLog.PreviousValue += ", Ship Type: " + (seaService.ShipType == 1 ? "Small" : seaService.ShipType == 2 ? "Medium" : seaService.ShipType == 3 ? "Large" : "");
                bnLog.PreviousValue += ",  Ship Name: " + seaService.ShipName + ",  Organization Name: " + seaService.OrganizationName + ", Appointment: " + seaService.AppointmentName + ", From Date: " + seaService.FromDate?.ToString("dd/MM/yyyy") + ", To Date: " + seaService.ToDate?.ToString("dd/MM/yyyy") + ",  Purpose: " + seaService.Purpose + ", Reference: " + seaService.Reference;

                bnLog.UpdatedValue += "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await seaServiceRepository.DeleteAsync(seaService);
            }
        }


        public List<SelectModel> GetShipTypeSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(ShipType)).Cast<ShipType>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }


    }
}
