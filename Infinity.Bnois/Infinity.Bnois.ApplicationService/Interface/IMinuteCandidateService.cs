using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Models;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IMinuteCandidateService
    {
        List<DashBoardMinuite110Model> GetMinuteCandidates(int minuiteId);
        Task<DashBoardMinuite110Model> SaveMinuteCadidate(int id, DashBoardMinuite110Model model);
        Task<bool> DeleteMinuteCadidate(int id);
    }
}
