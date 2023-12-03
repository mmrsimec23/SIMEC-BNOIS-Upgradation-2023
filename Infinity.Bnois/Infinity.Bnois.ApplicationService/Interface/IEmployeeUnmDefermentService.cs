using System.Collections.Generic;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Models;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IEmployeeUnmDefermentService
    {

        List<DashBoardBranch975Model> GetEmployeeUnmDeferments(int ps, int pn, string qs, out int total);
        Task<DashBoardBranch975Model> GetEmployeeUnmDeferment(int id);
        Task<DashBoardBranch975Model> SaveEmployeeUnmDeferment(int v, DashBoardBranch975Model model);
        Task<bool> DeleteEmployeeUnmDeferment(int id);


    }
}