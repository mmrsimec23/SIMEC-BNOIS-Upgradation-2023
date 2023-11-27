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
    public class ForeignProjectService : IForeignProjectService
    {
        private readonly IBnoisRepository<ForeignProject> foreignProjectRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public ForeignProjectService(IBnoisRepository<ForeignProject> foreignProjectRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.foreignProjectRepository = foreignProjectRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<ForeignProjectModel> GetForeignProjects(int ps, int pn, string qs, out int total)
        {
            IQueryable<ForeignProject> foreignProjects = foreignProjectRepository.FilterWithInclude(x => x.IsActive
                && (x.Employee.PNo.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee", "Country");
            total = foreignProjects.Count();
            foreignProjects = foreignProjects.OrderByDescending(x => x.ForeignProjectId).Skip((pn - 1) * ps).Take(ps);
            List<ForeignProjectModel> models = ObjectConverter<ForeignProject, ForeignProjectModel>.ConvertList(foreignProjects.ToList()).ToList();
            return models;
        }

        public async Task<ForeignProjectModel> GetForeignProject(int id)
        {
            if (id <= 0)
            {
                return new ForeignProjectModel();
            }
            ForeignProject foreignProject = await foreignProjectRepository.FindOneAsync(x => x.ForeignProjectId == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch" });
            if (foreignProject == null)
            {
                throw new InfinityNotFoundException("Foreign Project not found");
            }
            ForeignProjectModel model = ObjectConverter<ForeignProject, ForeignProjectModel>.Convert(foreignProject);
            return model;
        }


        public async Task<ForeignProjectModel> SaveForeignProject(int id, ForeignProjectModel model)
        {
            model.Employee = null;
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Foreign Project  data missing");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            ForeignProject foreignProject = ObjectConverter<ForeignProjectModel, ForeignProject>.Convert(model);
            if (id > 0)
            {
                foreignProject = await foreignProjectRepository.FindOneAsync(x => x.ForeignProjectId == id);
                if (foreignProject == null)
                {
                    throw new InfinityNotFoundException("Foreign Project not found !");
                }


                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ForeignProject";
                bnLog.TableEntryForm = "Foreign Projects";
                bnLog.PreviousValue = "Id: " + model.ForeignProjectId;
                bnLog.UpdatedValue = "Id: " + model.ForeignProjectId;
                int bnoisUpdateCount = 0;

                if (foreignProject.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", foreignProject.EmployeeId??0);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId??0);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)emp).PNo;
                    //bnoisUpdateCount += 1;
                }
                if (foreignProject.CountryId != model.CountryId)
                {
                    if (foreignProject.CountryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Country", "CountryId", foreignProject.CountryId ?? 0);
                        bnLog.PreviousValue += ", Country: " + ((dynamic)prev).FullName;
                    }
                    if (model.CountryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Country", "CountryId", model.CountryId ?? 0);
                        bnLog.UpdatedValue += ", Country: " + ((dynamic)newv).FullName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (foreignProject.ProjectName != model.ProjectName)
                {
                    bnLog.PreviousValue += ", Project Name: " + foreignProject.ProjectName;
                    bnLog.UpdatedValue += ", Project Name: " + model.ProjectName;
                    bnoisUpdateCount += 1;
                }
                if (foreignProject.OrganizationName != model.OrganizationName)
                {
                    bnLog.PreviousValue += ", Organization/Ship Name: " + foreignProject.OrganizationName;
                    bnLog.UpdatedValue += ", Organization/Ship Name: " + model.OrganizationName;
                    bnoisUpdateCount += 1;
                }
                if (foreignProject.AppointmentName != model.AppointmentName)
                {
                    bnLog.PreviousValue += ", Appointment: " + foreignProject.AppointmentName;
                    bnLog.UpdatedValue += ", Appointment: " + model.AppointmentName;
                    bnoisUpdateCount += 1;
                }
                if (foreignProject.FromDate != model.FromDate)
                {
                    bnLog.PreviousValue += ", From Date: " + foreignProject.FromDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", From Date: " + model.FromDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (foreignProject.ToDate != model.ToDate)
                {
                    bnLog.PreviousValue += ", To Date: " + foreignProject.ToDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", To Date: " + model.ToDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }

                if (foreignProject.Purpose != model.Purpose)
                {
                    bnLog.PreviousValue += ", Purpose: " + foreignProject.Purpose;
                    bnLog.UpdatedValue += ", Purpose: " + model.Purpose;
                    bnoisUpdateCount += 1;
                }
                if (foreignProject.Reference != model.Reference)
                {
                    bnLog.PreviousValue += ", Reference: " + foreignProject.Reference;
                    bnLog.UpdatedValue += ", Reference: " + model.Reference;
                    bnoisUpdateCount += 1;
                }
                if (foreignProject.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + foreignProject.Remarks;
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

                foreignProject.ModifiedDate = DateTime.Now;
                foreignProject.ModifiedBy = userId;
            }
            else
            {
                foreignProject.IsActive = true;
                foreignProject.CreatedDate = DateTime.Now;
                foreignProject.CreatedBy = userId;
            }
            foreignProject.EmployeeId = model.EmployeeId;
	        foreignProject.CountryId = model.CountryId;
	        foreignProject.ProjectName = model.ProjectName;
	        foreignProject.OrganizationName = model.OrganizationName;
	        foreignProject.AppointmentName = model.AppointmentName;
	        foreignProject.FromDate = model.FromDate;
	        foreignProject.ToDate = model.ToDate;
	        foreignProject.Purpose = model.Purpose;
	        foreignProject.Reference = model.Reference;
	        foreignProject.Remarks = model.Remarks;

            await foreignProjectRepository.SaveAsync(foreignProject);
            model.ForeignProjectId = foreignProject.ForeignProjectId;
            return model;
        }


        public async Task<bool> DeleteForeignProject(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            ForeignProject foreignProject = await foreignProjectRepository.FindOneAsync(x => x.ForeignProjectId == id);
            if (foreignProject == null)
            {
                throw new InfinityNotFoundException("Foreign Project not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ForeignProject";
                bnLog.TableEntryForm = "Foreign Projects";
                bnLog.PreviousValue = "Id: " + foreignProject.ForeignProjectId;

                if (foreignProject.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", foreignProject.EmployeeId ?? 0);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    //bnoisUpdateCount += 1;
                }
                if (foreignProject.CountryId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Country", "CountryId", foreignProject.CountryId ?? 0);
                    bnLog.PreviousValue += ", Country: " + ((dynamic)prev).FullName;
                }
                bnLog.PreviousValue += ", Project Name: " + foreignProject.ProjectName + ", Organization/Ship Name: " + foreignProject.OrganizationName + ", Appointment: " + foreignProject.AppointmentName + ", From Date: " + foreignProject.FromDate?.ToString("dd/MM/yyyy") + ", To Date: " + foreignProject.ToDate?.ToString("dd/MM/yyyy") + ", Purpose: " + foreignProject.Purpose + ", Reference: " + foreignProject.Reference + ", Remarks: " + foreignProject.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await foreignProjectRepository.DeleteAsync(foreignProject);
            }
        }


      
    }
}
