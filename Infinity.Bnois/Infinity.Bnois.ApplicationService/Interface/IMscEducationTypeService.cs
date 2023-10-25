using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Models;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IMscEducationTypeService
    {
        List<MscEducationTypeModel> GetMscEducationTypes(int ps, int pn, string qs, out int total);
        Task<MscEducationTypeModel> GetMscEducationType(int id);
        Task<MscEducationTypeModel> SaveMscEducationType(int v, MscEducationTypeModel model);
        Task<bool> DeleteMscEducationType(int id);
        Task<List<SelectModel>> GetMscEducationTypesSelectModels();
    }
}
