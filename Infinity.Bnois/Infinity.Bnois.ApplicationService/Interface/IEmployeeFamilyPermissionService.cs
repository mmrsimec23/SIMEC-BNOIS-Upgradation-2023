using System.Collections.Generic;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Models;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IEmployeeFamilyPermissionService
    {

        List<EmployeeFamilyPermissionModel> GetEmployeeFamilyPermissions(int ps, int pn, string qs, out int total);
        Task<EmployeeFamilyPermissionModel> GetEmployeeFamilyPermission(int id);
        Task<EmployeeFamilyPermissionModel> SaveEmployeeFamilyPermission(int v, EmployeeFamilyPermissionModel model);
        Task<bool> DeleteEmployeeFamilyPermission(int id);


    }
}