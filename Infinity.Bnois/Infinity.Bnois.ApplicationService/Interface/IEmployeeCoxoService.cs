using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Models;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IEmployeeCoxoService
    {
        List<EmployeeCoxoServiceModel> GetEmployeeCoxoServices(int ps, int pn, string qs, out int total);
        Task<EmployeeCoxoServiceModel> GetEmployeeCoxoService(int id);
        Task<EmployeeCoxoServiceModel> SaveEmployeeCoxoService(int v, EmployeeCoxoServiceModel model);
        Task<bool> DeleteEmployeeCoxoService(int id);
        List<SelectModel> GetCoxoTypeSelectModels();
        List<SelectModel> GetCoxoAppoinmentSelectModels();

    }
}