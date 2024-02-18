using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Models;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IEmployeeEoLoSoDloSeoService
    {
        List<EmployeeCoxoServiceModel> GetEmployeeEoLoSoDloSeoServices(int type, int ps, int pn, string qs, out int total);
        Task<EmployeeCoxoServiceModel> GetEmployeeEoLoSoDloSeoService(int id);
        Task<EmployeeCoxoServiceModel> SaveEmployeeEoLoSoDloSeoService(int v, EmployeeCoxoServiceModel model);
        Task<bool> DeleteEmployeeEoLoSoDloSeoService(int id);
        List<SelectModel> GetEoLoSoDloSeoTypeSelectModels();
        List<SelectModel> GetEoLoSoDloSeoAppoinmentSelectModels(int type);

    }
}