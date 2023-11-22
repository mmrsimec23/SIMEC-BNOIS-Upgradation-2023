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
    public class EmployeeSportService : IEmployeeSportService
    {
        private readonly IBnoisRepository<EmployeeSport> employeeSportRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public EmployeeSportService(IBnoisRepository<EmployeeSport> employeeSportRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.employeeSportRepository = employeeSportRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public async Task<EmployeeSportModel> GetEmployeeSport(int employeeSportId)
        {
            if (employeeSportId <= 0)
            {
                return new EmployeeSportModel();
            }
            EmployeeSport employeeSport = await employeeSportRepository.FindOneAsync(x => x.EmployeeSportId == employeeSportId);
            if (employeeSport == null)
            {
                return new EmployeeSportModel();
            }

            EmployeeSportModel model = ObjectConverter<EmployeeSport, EmployeeSportModel>.Convert(employeeSport);
            return model;
        }

        public List<EmployeeSportModel> GetEmployeeSports(int employeeId)
        {
            List<EmployeeSport> employeeSports = employeeSportRepository.FilterWithInclude(x => x.EmployeeId == employeeId, "Sport").ToList();
            List<EmployeeSportModel> models = ObjectConverter<EmployeeSport, EmployeeSportModel>.ConvertList(employeeSports.ToList()).ToList();
            return models;
        }

        public async Task<EmployeeSportModel> SaveEmployeeSport(int employeeSportId, EmployeeSportModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Officer Sports data missing!");
            }

            EmployeeSport employeeSport = ObjectConverter<EmployeeSportModel, EmployeeSport>.Convert(model);
            string userId= ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            if (employeeSportId > 0)
            {
                employeeSport = await employeeSportRepository.FindOneAsync(x => x.EmployeeSportId == employeeSportId);
                if (employeeSport == null)
                {
                    throw new InfinityNotFoundException("Officer Sports not found !");
                }

                employeeSport.ModifiedDate = DateTime.Now;
                employeeSport.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeSport";
                bnLog.TableEntryForm = "Employee Sports";
                bnLog.PreviousValue = "Id: " + model.EmployeeSportId;
                bnLog.UpdatedValue = "Id: " + model.EmployeeSportId;
                int bnoisUpdateCount = 0;
                if (employeeSport.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeSport.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)emp).PNo;
                    //bnoisUpdateCount += 1;
                }
                if (employeeSport.SportId != model.SportId)
                {
                    if (employeeSport.SportId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Sport", "SportId", employeeSport.SportId);
                        bnLog.PreviousValue += ", Sport: " + ((dynamic)prev).Name;
                    }
                    if (model.SportId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Sport", "SportId", model.SportId);
                        bnLog.UpdatedValue += ", Sport: " + ((dynamic)newv).Name;
                    }
                }
                if (employeeSport.TeamName != model.TeamName)
                {
                    bnLog.PreviousValue += ", Team Name: " + employeeSport.TeamName;
                    bnLog.UpdatedValue += ", Team Name: " + model.TeamName;
                    bnoisUpdateCount += 1;
                }
                if (employeeSport.DateOfParticipation != model.DateOfParticipation)
                {
                    bnLog.PreviousValue += ", Date Of Participation: " + employeeSport.DateOfParticipation?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Date Of Participation: " + model.DateOfParticipation?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeSport.Award != model.Award)
                {
                    bnLog.PreviousValue += ", Award: " + employeeSport.Award;
                    bnLog.UpdatedValue += ", Award: " + model.Award;
                    bnoisUpdateCount += 1;
                }
                if (employeeSport.Hobby != model.Hobby)
                {
                    bnLog.PreviousValue += ", Hobby: " + employeeSport.Hobby;
                    bnLog.UpdatedValue += ", Hobby: " + model.Hobby;
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
                employeeSport.EmployeeId = model.EmployeeId;
                employeeSport.CreatedBy = userId;
                employeeSport.CreatedDate = DateTime.Now;
                employeeSport.IsActive = true;
            }

            employeeSport.SportId = model.SportId;
            employeeSport.TeamName = model.TeamName;
            employeeSport.DateOfParticipation = model.DateOfParticipation;
            employeeSport.Award = model.Award;
            employeeSport.Hobby = "";
            await employeeSportRepository.SaveAsync(employeeSport);
            model.EmployeeSportId = employeeSport.EmployeeSportId;
            return model;
        }


        public async Task<bool> DeleteEmployeeSport(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            EmployeeSport employeeSport = await employeeSportRepository.FindOneAsync(x => x.EmployeeSportId == id);
            if (employeeSport == null)
            {
                throw new InfinityNotFoundException("Sport not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeSport";
                bnLog.TableEntryForm = "Employee Sports";
                bnLog.PreviousValue = "Id: " + employeeSport.EmployeeSportId;

                var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeSport.EmployeeId);
                bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;

                if (employeeSport.SportId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Sport", "SportId", employeeSport.SportId);
                    bnLog.PreviousValue += ", Sport: " + ((dynamic)prev).Name;
                }
                bnLog.PreviousValue += ", Team Name: " + employeeSport.TeamName + ", Participation Date: " + employeeSport.DateOfParticipation?.ToString("dd/MM/yyyy") + ", Award: " + employeeSport.Award;

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await employeeSportRepository.DeleteAsync(employeeSport);
            }
        }
    }
}
