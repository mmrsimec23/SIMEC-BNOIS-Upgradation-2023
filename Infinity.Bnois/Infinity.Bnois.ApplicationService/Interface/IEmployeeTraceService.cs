using Infinity.Bnois.ApplicationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IEmployeeTraceService
    {
        List<EmployeeTraceModel> GetEmployeeTraceList(int ps, int pn, string qs, out int total);
        List<EmployeeTraceModel> GetEmployeeTracesByPno(string PNo);
        Task<EmployeeTraceModel> getEmployeeTrace(int EmployeeTraceId);
        Task<EmployeeTraceModel> SaveEmployeeTrace(int v, EmployeeTraceModel model);
        Task<bool> DeleteEmployeeTrace(int id);


    }
}
