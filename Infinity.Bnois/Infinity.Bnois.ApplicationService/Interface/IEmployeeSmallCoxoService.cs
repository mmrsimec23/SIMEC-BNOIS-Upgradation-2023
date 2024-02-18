using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Models;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IEmployeeSmallCoxoService
    {
        List<EmployeeCoxoServiceModel> GetEmployeeSmallCoxoServices(int type, int ps, int pn, string qs, out int total);
        Task<EmployeeCoxoServiceModel> GetEmployeeSmallCoxoService(int id);
        Task<EmployeeCoxoServiceModel> SaveEmployeeSmallCoxoService(int v, EmployeeCoxoServiceModel model);
        Task<bool> DeleteEmployeeSmallCoxoService(int id);
        List<SelectModel> GetSmallCoxoTypeSelectModels();
        List<SelectModel> GetSmallCoxoAppoinmentSelectModels(int type);

    }
}