using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Models;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface ISuitabilityTestService
    {
        List<SuitabilityTestModel> GetSuitabilityTests(int type, int ps, int pn, string qs, out int total);
        Task<SuitabilityTestModel> GetSuitabilityTest(int id);
        Task<SuitabilityTestModel> SaveSuitabilityTest(int v, SuitabilityTestModel model);
        Task<bool> DeleteSuitabilityTest(int id);
        Task<bool> SaveSuitabilityTestTypeList(int type, int batchId);
        Task<bool> DeleteSuitabilityTestTypeList(int type);
        List<Object> GetSuitabilityTestListByType(int type);
        List<SelectModel> GetMajorSuitabilityTestTypeSelectModels();
        //List<SelectModel> GetCoxoAppoinmentSelectModels(int type);

    }
}