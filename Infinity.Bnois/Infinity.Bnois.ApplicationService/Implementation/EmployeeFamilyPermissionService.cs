using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class EmployeeFamilyPermissionService: IEmployeeFamilyPermissionService
    {

        private readonly IBnoisRepository<EmployeeFamilyPermission> employeeFamilyPermissionRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public EmployeeFamilyPermissionService(IBnoisRepository<EmployeeFamilyPermission> employeeFamilyPermissionRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.employeeFamilyPermissionRepository = employeeFamilyPermissionRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

  

        public List<EmployeeFamilyPermissionModel> GetEmployeeFamilyPermissions(int ps, int pn, string qs, out int total)
        {

            IQueryable<EmployeeFamilyPermission> employeeFamilyPermissions = employeeFamilyPermissionRepository
                .FilterWithInclude(x => x.IsActive && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee","Relation","Country");
            total = employeeFamilyPermissions.Count();
            employeeFamilyPermissions = employeeFamilyPermissions.OrderByDescending(x => x.EmployeeFamilyPermissionId).Skip((pn - 1) * ps).Take(ps);
            List<EmployeeFamilyPermissionModel> models = ObjectConverter<EmployeeFamilyPermission, EmployeeFamilyPermissionModel>.ConvertList(employeeFamilyPermissions.ToList()).ToList();
            return models;
        }

        public async Task<EmployeeFamilyPermissionModel> GetEmployeeFamilyPermission(int id)
        {
            if (id == 0)
            {
                return new EmployeeFamilyPermissionModel();
            }
            EmployeeFamilyPermission employeeFamilyPermission = await employeeFamilyPermissionRepository.FindOneAsync(x => x.EmployeeFamilyPermissionId == id, new List<string> { "Employee","Employee.Rank","Employee.Batch" });
            if (employeeFamilyPermission == null)
            {
                throw new InfinityNotFoundException("Employee Family Permission not found");
            }
            EmployeeFamilyPermissionModel model = ObjectConverter<EmployeeFamilyPermission, EmployeeFamilyPermissionModel>.Convert(employeeFamilyPermission);
            return model;
        }

        public async Task<EmployeeFamilyPermissionModel> SaveEmployeeFamilyPermission(int id, EmployeeFamilyPermissionModel model)
        {

            if (model == null)
            {
                throw new InfinityArgumentMissingException("Employee Family Permission data missing");
            }
            EmployeeFamilyPermission employeeFamilyPermission = ObjectConverter<EmployeeFamilyPermissionModel, EmployeeFamilyPermission>.Convert(model);
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();

            if (id > 0)
            {
                employeeFamilyPermission = employeeFamilyPermissionRepository.FindOne(x => x.EmployeeFamilyPermissionId == id);
                if (employeeFamilyPermission == null)
                {
                    throw new InfinityNotFoundException("Employee Family Permission not found !");
                }


                employeeFamilyPermission.ModifiedDate = DateTime.Now;
                employeeFamilyPermission.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeFamilyPermission";
                bnLog.TableEntryForm = "Officer Family Permission";
                bnLog.PreviousValue = "Id: " + model.EmployeeFamilyPermissionId;
                bnLog.UpdatedValue = "Id: " + model.EmployeeFamilyPermissionId;
                if (employeeFamilyPermission.EmployeeId != model.EmployeeId)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeFamilyPermission.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                    bnLog.UpdatedValue += ", Name: " + ((dynamic)emp).PNo + "_" + ((dynamic)emp).FullNameEng;
                }
                if (employeeFamilyPermission.RelationId != model.RelationId)
                {
                    var prevrelation = employeeService.GetDynamicTableInfoById("Relation", "RelationId", employeeFamilyPermission.RelationId??0);
                    var relation = employeeService.GetDynamicTableInfoById("Relation", "RelationId", model.RelationId??0);
                    bnLog.PreviousValue += ", Relation: " + ((dynamic)prevrelation).Name;
                    bnLog.UpdatedValue += ", Relation: " + ((dynamic)relation).Name;
                }
                if (employeeFamilyPermission.CountryId != model.CountryId)
                {
                    var prevcoun = employeeService.GetDynamicTableInfoById("Country", "CountryId", employeeFamilyPermission.CountryId??0);
                    var coun = employeeService.GetDynamicTableInfoById("Country", "CountryId", model.CountryId??0);
                    bnLog.PreviousValue += ", Country: " + ((dynamic)prevcoun).FullName;
                    bnLog.UpdatedValue += ", Country: " + ((dynamic)coun).FullName;
                }
                if (employeeFamilyPermission.RankId != model.RankId)
                {
                    var prevrank = employeeService.GetDynamicTableInfoById("Rank", "RankId", employeeFamilyPermission.RankId??0);
                    var rank = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.RankId??0);
                    bnLog.PreviousValue += ", Rank: " + ((dynamic)prevrank).ShortName;
                    bnLog.UpdatedValue += ", Rank: " + ((dynamic)rank).ShortName;
                }
                if (employeeFamilyPermission.RelativeName != model.RelativeName)
                {
                    bnLog.PreviousValue += ", RelativeName: " + employeeFamilyPermission.RelativeName;
                    bnLog.UpdatedValue += ", RelativeName: " + model.RelativeName;
                }
                if (employeeFamilyPermission.VisitPurpose != model.VisitPurpose)
                {
                    bnLog.PreviousValue += ", VisitPurpose: " + employeeFamilyPermission.VisitPurpose;
                    bnLog.UpdatedValue += ", VisitPurpose: " + model.VisitPurpose;
                }
                if (employeeFamilyPermission.FromDate != model.FromDate)
                {
                    bnLog.PreviousValue += ", FromDate: " + employeeFamilyPermission.FromDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", FromDate: " + model.FromDate?.ToString("dd/MM/yyyy");
                }
                if (employeeFamilyPermission.ToDate != model.ToDate)
                {
                    bnLog.PreviousValue += ", ToDate: " + employeeFamilyPermission.ToDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", ToDate: " + model.ToDate?.ToString("dd/MM/yyyy");
                }
                if (employeeFamilyPermission.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + employeeFamilyPermission.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (employeeFamilyPermission.EmployeeId != model.EmployeeId || employeeFamilyPermission.RelationId != model.RelationId || employeeFamilyPermission.Remarks != model.Remarks 
                    || employeeFamilyPermission.CountryId != model.CountryId || employeeFamilyPermission.RankId != model.RankId || employeeFamilyPermission.RelativeName != model.RelativeName
                    || employeeFamilyPermission.VisitPurpose != model.VisitPurpose || employeeFamilyPermission.FromDate != model.FromDate || employeeFamilyPermission.ToDate != model.ToDate)
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
                employeeFamilyPermission.CreatedDate = DateTime.Now;
                employeeFamilyPermission.CreatedBy = userId;
                employeeFamilyPermission.IsActive = true;
            }
            employeeFamilyPermission.EmployeeId = model.EmployeeId;
            employeeFamilyPermission.RelationId = model.RelationId;
            employeeFamilyPermission.CountryId = model.CountryId;
            employeeFamilyPermission.RelativeName = model.RelativeName;
            employeeFamilyPermission.VisitPurpose = model.VisitPurpose;
            employeeFamilyPermission.FromDate = model.FromDate;
            employeeFamilyPermission.ToDate = model.ToDate;
            employeeFamilyPermission.Remarks = model.Remarks;
            employeeFamilyPermission.RankId = model.Employee.RankId;

            //if (model.IsBackLog)
            //{
            //    employeeFamilyPermission.RankId = model.RankId;
            //    employeeFamilyPermission.TransferId = model.TransferId;
            //}

            employeeFamilyPermission.Employee = null;
            //employeeFamilyPermission.Country = null;
            //employeeFamilyPermission.Relation = null;
            //employeeFamilyPermission.Rank = null;
            await employeeFamilyPermissionRepository.SaveAsync(employeeFamilyPermission);
            model.EmployeeFamilyPermissionId = employeeFamilyPermission.EmployeeFamilyPermissionId;
            return model;
        }

        public async Task<bool> DeleteEmployeeFamilyPermission(int id)
        {
            if (id <= 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            EmployeeFamilyPermission employeeFamilyPermission = await employeeFamilyPermissionRepository.FindOneAsync(x => x.EmployeeFamilyPermissionId == id);
            if (employeeFamilyPermission == null)
            {
                throw new InfinityNotFoundException("Employee Family Permission not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeFamilyPermission";
                bnLog.TableEntryForm = "Officer Family Permission";
                var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeFamilyPermission.EmployeeId);
                
                bnLog.PreviousValue = "Id: " + employeeFamilyPermission.EmployeeFamilyPermissionId + ", Name: " + ((dynamic)emp).PNo + "_" + ((dynamic)emp).FullNameEng
                    + ", Relation: " + employeeFamilyPermission.RelationId + ", Remarks: " + employeeFamilyPermission.Remarks 
                    + ", Country: " + employeeFamilyPermission.CountryId + ", Rank: " + employeeFamilyPermission.RankId
                    + ", RelativeName: " + employeeFamilyPermission.RelativeName + ", VisitPurpose: " + employeeFamilyPermission.VisitPurpose
                    + ", FromDate: " + employeeFamilyPermission.FromDate?.ToString("dd/MM/yyyy") + ", ToDate: " + employeeFamilyPermission.ToDate?.ToString("dd/MM/yyyy");
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await employeeFamilyPermissionRepository.DeleteAsync(employeeFamilyPermission);
            }
        }
    }
}