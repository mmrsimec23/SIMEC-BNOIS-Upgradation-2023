using System.Collections.Generic;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Models;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IEmployeeMinuteStandbyService
    {

        List<DashBoardMinuteStandby975Model> GetEmployeeMinuteStandbys(int ps, int pn, string qs, out int total);
        Task<DashBoardMinuteStandby975Model> GetEmployeeMinuteStandby(int id);
        Task<DashBoardMinuteStandby975Model> SaveEmployeeMinuteStandby(int v, DashBoardMinuteStandby975Model model);
        Task<bool> DeleteEmployeeMinuteStandby(int id);


    }
}