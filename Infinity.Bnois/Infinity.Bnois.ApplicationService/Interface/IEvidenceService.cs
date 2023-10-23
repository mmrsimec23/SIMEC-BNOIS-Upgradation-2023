using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Models;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IEvidenceService
    {
        List<EvidenceModel> GetEvidences(int ps, int pn, string qs, out int total);
        Task<EvidenceModel> GetEvidence(int id);
        Task<EvidenceModel> SaveEvidence(int v, EvidenceModel model);
        Task<bool> DeleteEvidence(int id);
        Task<List<SelectModel>> GetEvidenceSelectModels();
    }
}
