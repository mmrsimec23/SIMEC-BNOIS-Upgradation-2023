using System.Collections.Generic;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Models;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IEmployeeMscEducationService
    {

        List<EmployeeMscEducationModel> GetEmployeeMscEducations(int ps, int pn, string qs, out int total);
        Task<EmployeeMscEducationModel> GetEmployeeMscEducation(int id);
        Task<EmployeeMscEducationModel> SaveEmployeeMscEducation(int v, EmployeeMscEducationModel model);
        Task<bool> DeleteEmployeeMscEducation(int id);


    }
}