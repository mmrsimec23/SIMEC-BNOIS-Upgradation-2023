using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Models;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IMinuiteService
    {
        List<DashBoardMinuite100Model> GetMinuites(int ps, int pn, string qs, out int total);
        Task<DashBoardMinuite100Model> GetMinuite(int id);
        Task<DashBoardMinuite100Model> SaveMinuite(int v, DashBoardMinuite100Model model);
        Task<bool> DeleteMinuite(int id);
    }
}
