using Infinity.Bnois.ApplicationService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IToeAuthorizedService
    {
        List<ToeAuthorizedModel> GetToeAuthorizeds(int pageSize, int pageNumber, string searchText, out int total);
        Task<ToeAuthorizedModel> GetToeAuthorized(int toeAuthorizedId);
        Task<ToeAuthorizedModel> SaveToeAuthorized(int toeAuthorizedId, ToeAuthorizedModel model);
        Task<bool> DeleteToeAuthorized(int toeAuthorizedId);
    }

}
