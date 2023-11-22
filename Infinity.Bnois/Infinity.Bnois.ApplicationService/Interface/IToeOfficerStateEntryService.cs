using Infinity.Bnois.ApplicationService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IToeOfficerStateEntryService
    {
        List<DashBoardBranchByAdminAuthority700Model> GetToeOfficerStateEntryList(int pageSize, int pageNumber, string searchText, out int total);
        Task<DashBoardBranchByAdminAuthority700Model> GetToeOfficerStateEntry(int id);
        Task<DashBoardBranchByAdminAuthority700Model> SaveToeOfficerStateEntry(int id, DashBoardBranchByAdminAuthority700Model model);
        Task<bool> DeleteToeOfficerStateEntry(int id);
        List<SelectModel> GetOrgTypeSelectModels();
    }

}
