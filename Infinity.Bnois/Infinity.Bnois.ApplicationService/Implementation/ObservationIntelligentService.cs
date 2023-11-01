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
    public class ObservationIntelligentService : IObservationIntelligentService
    {
        private readonly IBnoisRepository<ObservationIntelligent> observationIntelligentRepository;
        private readonly IEmployeeService employeeService;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public ObservationIntelligentService(IBnoisRepository<ObservationIntelligent> observationIntelligentRepository, IEmployeeService employeeService, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.observationIntelligentRepository = observationIntelligentRepository;
            this.employeeService = employeeService;
            this.bnoisLogRepository = bnoisLogRepository;
        }

        public List<ObservationIntelligentModel> GetObservationIntelligents(int ps, int pn, string qs, out int total)
        {
            IQueryable<ObservationIntelligent> observationIntelligents = observationIntelligentRepository.FilterWithInclude(x => x.IsActive
                && (x.Employee.PNo.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee", "Employee1");
            total = observationIntelligents.Count();
            observationIntelligents = observationIntelligents.OrderByDescending(x => x.ObservationIntelligentId).Skip((pn - 1) * ps).Take(ps);
            List<ObservationIntelligentModel> models = ObjectConverter<ObservationIntelligent, ObservationIntelligentModel>.ConvertList(observationIntelligents.ToList()).ToList();
            models = models.Select(x =>
            {
                x.TypeName = Enum.GetName(typeof(ObservationIntelligentType), x.Type);
                return x;
            }).ToList();
            return models;
        }

        public async Task<ObservationIntelligentModel> GetObservationIntelligent(int id)
        {
            if (id <= 0)
            {
                return new ObservationIntelligentModel();
            }
            ObservationIntelligent observationIntelligent = await observationIntelligentRepository.FindOneAsync(x => x.ObservationIntelligentId == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch","Employee1", "Employee1.Rank", "Employee1.Batch" });
            if (observationIntelligent == null)
            {
                throw new InfinityNotFoundException("Observation Intelligent Report not found");
            }
            ObservationIntelligentModel model = ObjectConverter<ObservationIntelligent, ObservationIntelligentModel>.Convert(observationIntelligent);
            return model;
        }

    
        public async Task<ObservationIntelligentModel> SaveObservationIntelligent(int id, ObservationIntelligentModel model)
        {
          
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Observation Intelligent Report  data missing");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            ObservationIntelligent observationIntelligent = ObjectConverter<ObservationIntelligentModel, ObservationIntelligent>.Convert(model);
            if (id > 0)
            {
                observationIntelligent = await observationIntelligentRepository.FindOneAsync(x => x.ObservationIntelligentId == id);
                if (observationIntelligent == null)
                {
                    throw new InfinityNotFoundException("Observation Intelligent Report not found !");
                }

                observationIntelligent.ModifiedDate = DateTime.Now;
                observationIntelligent.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ObservationIntelligent";
                bnLog.TableEntryForm = "Observation & Intelligent Report";
                bnLog.PreviousValue = "Id: " + model.ObservationIntelligentId;
                bnLog.UpdatedValue = "Id: " + model.ObservationIntelligentId;
                if (observationIntelligent.EmployeeId != model.EmployeeId)
                {
                    var prevEmployee = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", observationIntelligent.EmployeeId);
                    var newEmployee = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", Employee: " + ((dynamic)prevEmployee).FullNameEng;
                    bnLog.UpdatedValue += ", Employee: " + ((dynamic)newEmployee).FullNameEng;
                }
                if (observationIntelligent.IsBackLog != model.IsBackLog)
                {
                    bnLog.PreviousValue += ", Back Log: " + observationIntelligent.IsBackLog;
                    bnLog.UpdatedValue += ", Back Log: " + model.IsBackLog;
                }
                if (observationIntelligent.RankId != model.RankId)
                {
                    var prevRank = employeeService.GetDynamicTableInfoById("Rank", "RankId", observationIntelligent.RankId ?? 0);
                    var newRank = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.RankId ?? 0);
                    bnLog.PreviousValue += ", Rank: " + ((dynamic)prevRank).ShortName;
                    bnLog.UpdatedValue += ", Rank: " + ((dynamic)newRank).ShortName;
                }
                if (observationIntelligent.TransferId != model.TransferId)
                {
                    if (observationIntelligent.TransferId > 0)
                    {
                        var prevTransfer = employeeService.GetDynamicTableInfoById("vwTransfer", "TransferId", observationIntelligent.TransferId ?? 0);
                        bnLog.PreviousValue += ", Born/Attach/Appointment: " + ((dynamic)prevTransfer).BornOffice + '/' + ((dynamic)prevTransfer).CurrentAttach + '/' + ((dynamic)prevTransfer).Appointment;
                    }
                    if (model.TransferId > 0)
                    {
                        var newTransfer = employeeService.GetDynamicTableInfoById("vwTransfer", "TransferId", model.TransferId ?? 0);
                        bnLog.UpdatedValue += ", Born/Attach/Appointment: " + ((dynamic)newTransfer).BornOffice + '/' + ((dynamic)newTransfer).CurrentAttach + '/' + ((dynamic)newTransfer).Appointment;
                    }
                }

                if (observationIntelligent.Type != model.Type)
                {
                    bnLog.PreviousValue += ", Type: " + (observationIntelligent.Type == 1 ? "Observation" : observationIntelligent.Type == 2 ? "Intelligent_Report" : "");
                    bnLog.UpdatedValue += ", Type: " + (observationIntelligent.Type == 1 ? "Observation" : observationIntelligent.Type == 2 ? "Intelligent_Report" : "");
                }

                if (observationIntelligent.Date != model.Date)
                {
                    bnLog.PreviousValue += ", Date: " + observationIntelligent.Date.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Date: " + model.Date?.ToString("dd/MM/yyyy");
                }
                if (observationIntelligent.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + observationIntelligent.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }
                if (observationIntelligent.GivenEmployeeId != model.GivenEmployeeId)
                {
                    if (observationIntelligent.GivenEmployeeId > 0)
                    {
                        var prevEmployee = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", observationIntelligent.GivenEmployeeId ?? 0);
                        bnLog.PreviousValue += ", Employee: " + ((dynamic)prevEmployee).FullNameEng;
                    }
                    if (model.GivenEmployeeId > 0)
                    {
                        var newEmployee = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.GivenEmployeeId ?? 0);
                        bnLog.UpdatedValue += ", Employee: " + ((dynamic)newEmployee).FullNameEng;
                    }
                }
                if (observationIntelligent.GivenTransferId != model.GivenTransferId)
                {
                    if (observationIntelligent.GivenTransferId > 0)
                    {
                        var prevTransfer = employeeService.GetDynamicTableInfoById("vwTransfer", "TransferId", observationIntelligent.GivenTransferId ?? 0);
                        bnLog.PreviousValue += ", Born/Attach/Appointment: " + ((dynamic)prevTransfer).BornOffice + '/' + ((dynamic)prevTransfer).CurrentAttach + '/' + ((dynamic)prevTransfer).Appointment;
                    }
                    if (model.GivenTransferId > 0)
                    {
                        var newTransfer = employeeService.GetDynamicTableInfoById("vwTransfer", "TransferId", model.GivenTransferId ?? 0);
                        bnLog.UpdatedValue += ", Born/Attach/Appointment: " + ((dynamic)newTransfer).BornOffice + '/' + ((dynamic)newTransfer).CurrentAttach + '/' + ((dynamic)newTransfer).Appointment;
                    }
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (observationIntelligent.EmployeeId != model.EmployeeId || observationIntelligent.IsBackLog != model.IsBackLog || observationIntelligent.RankId != model.RankId || observationIntelligent.TransferId != model.TransferId
                    || observationIntelligent.Type != model.Type || observationIntelligent.Date != model.Date || observationIntelligent.Remarks != model.Remarks || observationIntelligent.GivenEmployeeId != model.GivenEmployeeId
                    || observationIntelligent.GivenTransferId != model.GivenTransferId)
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
                observationIntelligent.IsActive = true;
                observationIntelligent.CreatedDate = DateTime.Now;
                observationIntelligent.CreatedBy = userId;
            }
            observationIntelligent.EmployeeId = model.EmployeeId;
            observationIntelligent.GivenEmployeeId = model.GivenEmployeeId;
            observationIntelligent.GivenTransferId = model.GivenTransferId;
            observationIntelligent.Type = model.Type;
            observationIntelligent.Date =model.Date ?? observationIntelligent.Date;
            observationIntelligent.Remarks = model.Remarks;

            observationIntelligent.IsBackLog = model.IsBackLog;
            observationIntelligent.RankId = model.Employee.RankId;
            observationIntelligent.TransferId = model.Employee.TransferId;

            if (model.IsBackLog)
            {

                observationIntelligent.RankId = model.RankId;
                observationIntelligent.TransferId = model.TransferId;
            }

            observationIntelligent.Employee = null;
            observationIntelligent.Employee1 = null;

            await observationIntelligentRepository.SaveAsync(observationIntelligent);
            model.ObservationIntelligentId = observationIntelligent.ObservationIntelligentId;
            return model;
        }


        public async Task<bool> DeleteObservationIntelligent(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            ObservationIntelligent observationIntelligent = observationIntelligentRepository.FindOne(x => x.ObservationIntelligentId == id, new List<string> { "Employee", "Employee1" });
            if (observationIntelligent == null)
            {
                throw new InfinityNotFoundException("Observation Intelligent Report not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ObservationIntelligent";
                bnLog.TableEntryForm = "Observation & Intelligent Report";

                var rank = employeeService.GetDynamicTableInfoById("Rank", "RankId", observationIntelligent.RankId ?? 0);
                



                bnLog.PreviousValue = "Id: " + observationIntelligent.ObservationIntelligentId + ", Employee: " + observationIntelligent.Employee.FullNameEng;
                bnLog.PreviousValue += ", Back Log: " + observationIntelligent.IsBackLog;
                bnLog.PreviousValue += ", Rank: " + ((dynamic)rank).ShortName;
                if (observationIntelligent.TransferId > 0)
                {
                    var transfer = employeeService.GetDynamicTableInfoById("vwTransfer", "TransferId", observationIntelligent.TransferId ?? 0);
                    bnLog.PreviousValue += ", Born/Attach/Appointment: " + ((dynamic)transfer).BornOffice + '/' + ((dynamic)transfer).CurrentAttach + '/' + ((dynamic)transfer).Appointment;

                }
                bnLog.PreviousValue += ", Type: " + (observationIntelligent.Type == 1 ? "Observation" : observationIntelligent.Type == 2 ? "Intelligent_Report" : "");
                bnLog.PreviousValue += ", Date: " + observationIntelligent.Date.ToString("dd/MM/yyyy") + ", Remarks: " + observationIntelligent.Remarks;
                bnLog.PreviousValue += ", Given By: " + observationIntelligent.Employee1.FullNameEng;
                if(observationIntelligent.GivenTransferId > 0)
                {
                    var givenTransfer = employeeService.GetDynamicTableInfoById("vwTransfer", "TransferId", observationIntelligent.GivenTransferId ?? 0);
                    bnLog.PreviousValue += ", Born/Attach/Appointment: " + ((dynamic)givenTransfer).BornOffice + '/' + ((dynamic)givenTransfer).CurrentAttach + '/' + ((dynamic)givenTransfer).Appointment;
                }


                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await observationIntelligentRepository.DeleteAsync(observationIntelligent);
            }
        }

        public List<SelectModel> GetObservationIntelligentTypeSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(ObservationIntelligentType)).Cast<ObservationIntelligentType>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }


      
    }
}
