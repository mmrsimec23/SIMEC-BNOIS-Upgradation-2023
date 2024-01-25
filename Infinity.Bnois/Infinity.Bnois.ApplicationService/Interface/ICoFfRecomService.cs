using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Models;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface ICoFfRecomService
    {
        List<CoFfRecomModel> GetCOFFRecoms(int type);
        Task<CoFfRecomModel> SaveCOFFRecom(int id, CoFfRecomModel model);
        Task<bool> DeleteCOFFRecom(int id);
    }
}
