using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Models;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IMscInstituteService
    {
        List<MscInstituteModel> GetMscInstitutes(int ps, int pn, string qs, out int total);
        Task<MscInstituteModel> GetMscInstitute(int id);
        Task<MscInstituteModel> SaveMscInstitute(int v, MscInstituteModel model);
        Task<bool> DeleteMscInstitute(int id);
        Task<List<SelectModel>> GetMscInstitutesSelectModels();
    }
}
