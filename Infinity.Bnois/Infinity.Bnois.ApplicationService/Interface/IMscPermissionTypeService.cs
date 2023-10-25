using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Models;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IMscPermissionTypeService
    {
        List<MscPermissionTypeModel> GetMscPermissionTypes(int ps, int pn, string qs, out int total);
        Task<MscPermissionTypeModel> GetMscPermissionType(int id);
        Task<MscPermissionTypeModel> SaveMscPermissionType(int v, MscPermissionTypeModel model);
        Task<bool> DeleteMscPermissionType(int id);
        Task<List<SelectModel>> GetMscPermissionTypesSelectModels();
    }
}
