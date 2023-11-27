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
    public class ExtraAppointmentService : IExtraAppointmentService
    {
        private readonly IBnoisRepository<ExtraAppointment> ExtraAppointmentRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;

        public ExtraAppointmentService(IBnoisRepository<ExtraAppointment> ExtraAppointmentRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.ExtraAppointmentRepository = ExtraAppointmentRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<ExtraAppointmentModel> GetExtraAppointments(int ps, int pn, string qs, out int total)
        {
            IQueryable<ExtraAppointment> extraAppointments = ExtraAppointmentRepository.FilterWithInclude(x => x.IsActive
                && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee", "Transfer", "Office", "OfficeAppointment");
            total = extraAppointments.Count();
            extraAppointments = extraAppointments.OrderByDescending(x => x.ExtraAppointmentId).Skip((pn - 1) * ps).Take(ps);
            List<ExtraAppointmentModel> models = ObjectConverter<ExtraAppointment, ExtraAppointmentModel>.ConvertList(extraAppointments.ToList()).ToList();
            return models;
        }

        public async Task<ExtraAppointmentModel> GetExtraAppointment(int id)
        {
            if (id <= 0)
            {
                return new ExtraAppointmentModel();
            }
            ExtraAppointment extraAppointment = await ExtraAppointmentRepository.FindOneAsync(x => x.ExtraAppointmentId == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch","Office" });
            if (extraAppointment == null)
            {
                throw new InfinityNotFoundException("Extra Appointment not found");
            }
            ExtraAppointmentModel model = ObjectConverter<ExtraAppointment, ExtraAppointmentModel>.Convert(extraAppointment);
            return model;
        }

       

        public async Task<ExtraAppointmentModel> SaveExtraAppointment(int id, ExtraAppointmentModel model)
        {
           
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Extra Appointment  data missing");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            ExtraAppointment extraAppointment = ObjectConverter<ExtraAppointmentModel, ExtraAppointment>.Convert(model);
            if (id > 0)
            {
                extraAppointment = await ExtraAppointmentRepository.FindOneAsync(x => x.ExtraAppointmentId == id);
                if (extraAppointment == null)
                {
                    throw new InfinityNotFoundException("Extra Appointment not found !");
                }

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ExtraAppointment";
                bnLog.TableEntryForm = "Extra Appointment";
                bnLog.PreviousValue = "Id: " + model.ExtraAppointmentId;
                bnLog.UpdatedValue = "Id: " + model.ExtraAppointmentId;
                int bnoisUpdateCount = 0;

                if (extraAppointment.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", extraAppointment.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)emp).PNo;
                    //bnoisUpdateCount += 1;
                }
                if (extraAppointment.OfficeId != model.OfficeId)
                {
                    if (extraAppointment.OfficeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", extraAppointment.OfficeId);
                        bnLog.PreviousValue += ", Office: " + ((dynamic)prev).ShortName;
                    }
                    if (model.AppointmentId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Office", "OfficeId", model.OfficeId);
                        bnLog.UpdatedValue += ", Office: " + ((dynamic)newv).ShortName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (extraAppointment.AppointmentId != model.AppointmentId)
                {
                    if (extraAppointment.AppointmentId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("OfficeAppointment", "OffAppId", extraAppointment.AppointmentId ?? 0);
                        bnLog.PreviousValue += ", Appointment: " + ((dynamic)prev).Name;
                    }
                    if (model.AppointmentId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("OfficeAppointment", "OffAppId", model.AppointmentId ?? 0);
                        bnLog.UpdatedValue += ", Appointment: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (extraAppointment.AssignDate != model.AssignDate)
                {
                    bnLog.PreviousValue += ", Assign Date: " + extraAppointment.AssignDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Assign Date: " + model.AssignDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (extraAppointment.EndDate != model.EndDate)
                {
                    bnLog.PreviousValue += ", End Date: " + extraAppointment.EndDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", End Date: " + model.EndDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                
                if (extraAppointment.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + extraAppointment.Remarks;
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


                extraAppointment.ModifiedDate = DateTime.Now;
                extraAppointment.ModifiedBy = userId;
            }
            else
            {
                extraAppointment.IsActive = true;
                extraAppointment.CreatedDate = DateTime.Now;
                extraAppointment.CreatedBy = userId;
            }
            extraAppointment.EmployeeId = model.EmployeeId;
            extraAppointment.TransferId = model.Employee.TransferId;
            extraAppointment.OfficeId = model.OfficeId;
            extraAppointment.AppointmentId = model.AppointmentId;
            extraAppointment.AssignDate = model.AssignDate ?? extraAppointment.AssignDate;
            extraAppointment.EndDate = model.EndDate;
            extraAppointment.Remarks = model.Remarks;
            extraAppointment.Employee = null;

            await ExtraAppointmentRepository.SaveAsync(extraAppointment);
            model.ExtraAppointmentId = extraAppointment.ExtraAppointmentId;


            return model;
        }


        public async Task<bool> DeleteExtraAppointment(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            ExtraAppointment extraAppointment = await ExtraAppointmentRepository.FindOneAsync(x => x.ExtraAppointmentId == id);
            if (extraAppointment == null)
            {
                throw new InfinityNotFoundException("Extra Appointment not found");
            }

            // data log section start
            BnoisLog bnLog = new BnoisLog();
            bnLog.TableName = "ExtraAppointment";
            bnLog.TableEntryForm = "Extra Appointment";
            bnLog.PreviousValue = "Id: " + extraAppointment.ExtraAppointmentId;
            if (extraAppointment.EmployeeId > 0)
            {
                var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", extraAppointment.EmployeeId);
                bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
            }
            if (extraAppointment.OfficeId > 0)
            {
                var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", extraAppointment.OfficeId);
                bnLog.PreviousValue += ", Office: " + ((dynamic)prev).ShortName;
            }
            if (extraAppointment.AppointmentId > 0)
            {
                var prev = employeeService.GetDynamicTableInfoById("OfficeAppointment", "OffAppId", extraAppointment.AppointmentId ?? 0);
                bnLog.PreviousValue += ", Appointment: " + ((dynamic)prev).Name;
            }
            bnLog.PreviousValue += ", Assign Date: " + extraAppointment.AssignDate?.ToString("dd/MM/yyyy") + ", End Date: " + extraAppointment.EndDate?.ToString("dd/MM/yyyy") + ", Remarks: " + extraAppointment.Remarks;
            bnLog.UpdatedValue = "This Record has been Deleted!";

            bnLog.LogStatus = 2; // 1 for update, 2 for delete
            bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            bnLog.LogCreatedDate = DateTime.Now;

            await bnoisLogRepository.SaveAsync(bnLog);

            //data log section end

            return await ExtraAppointmentRepository.DeleteAsync(extraAppointment); ;
           
        }


       
    }
}
