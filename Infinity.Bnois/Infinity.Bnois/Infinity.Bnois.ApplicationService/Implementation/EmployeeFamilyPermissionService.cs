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
        public EmployeeFamilyPermissionService(IBnoisRepository<EmployeeFamilyPermission> employeeFamilyPermissionRepository)
        {
            this.employeeFamilyPermissionRepository = employeeFamilyPermissionRepository;
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
                employeeFamilyPermission = await employeeFamilyPermissionRepository.FindOneAsync(x => x.EmployeeFamilyPermissionId == id);
                if (employeeFamilyPermission == null)
                {
                    throw new InfinityNotFoundException("Employee Family Permission not found !");
                }


                employeeFamilyPermission.ModifiedDate = DateTime.Now;
                employeeFamilyPermission.ModifiedBy = userId;
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
                return await employeeFamilyPermissionRepository.DeleteAsync(employeeFamilyPermission);
            }
        }
    }
}