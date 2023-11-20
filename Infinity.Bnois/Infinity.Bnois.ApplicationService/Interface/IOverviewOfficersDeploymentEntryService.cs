using Infinity.Bnois.ApplicationService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IOverviewOfficersDeploymentEntryService
    {
        List<DashBoardBranchByAdminAuthority600EntryModel> GetOverviewOfficersDeploymentEntryList(int pageSize, int pageNumber, string searchText, out int total);
        Task<DashBoardBranchByAdminAuthority600EntryModel> GetOverviewOfficersDeploymentEntry(int id);
        Task<DashBoardBranchByAdminAuthority600EntryModel> SaveOverviewOfficersDeploymentEntry(int id, DashBoardBranchByAdminAuthority600EntryModel model);
        Task<bool> DeleteOverviewOfficersDeploymentEntry(int id);
        List<SelectModel> GetOrgTypeSelectModels();
    }

}
