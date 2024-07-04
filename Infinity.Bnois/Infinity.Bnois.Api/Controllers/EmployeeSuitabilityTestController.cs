
using Infinity.Bnois.Api.Core;
using Infinity.Bnois.ApplicationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.Api.Models;
using Infinity.Bnois.Api.Right;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Configuration.Models;
using Infinity.Ers.ApplicationService;
using Infinity.Bnois.ApplicationService.Implementation;

namespace Infinity.Bnois.Api.Controllers
{
    [RoutePrefix(BnoisRoutePrefix.EmployeeSuitabilityTest)]
    [EnableCors("*", "*", "*")]
    //[ActionAuthorize(Feature = MASTER_SETUP.EMPLOYEE_COXO_SERVICE)]

    public class EmployeeSuitabilityTestController : PermissionController
    {
        private readonly ISuitabilityTestService suitabilityTestService;
        private readonly IBatchService batchService;



        public EmployeeSuitabilityTestController(ISuitabilityTestService suitabilityTestService, IBatchService batchService, IRoleFeatureService roleFeatureService) : base(roleFeatureService)
        {
            this.suitabilityTestService = suitabilityTestService;
            this.batchService = batchService;


        }

        [HttpGet]
        [Route("get-suitability-tests")]
        public IHttpActionResult GetSuitabilityTests(int type,int ps, int pn, string qs)
        {
            int total = 0;
            List<SuitabilityTestModel> models = suitabilityTestService.GetSuitabilityTests(type,ps, pn, qs, out total);
            //RoleFeature permission = base.GetFeature(MASTER_SETUP.EMPLOYEE_COXO_SERVICE);
            return Ok(new ResponseMessage<List<SuitabilityTestModel>>()
            {
                Result = models,
                Total = total
                //Permission = permission
            });
        }

        [HttpGet]
        [Route("get-suitability-test")]
        public async Task<IHttpActionResult> GetSuitabilityTest(int id)
        {
            SuitabilityTestViewModel vm = new SuitabilityTestViewModel();
            vm.SuitabilityTests = await suitabilityTestService.GetSuitabilityTest(id);
            vm.SuitabilityTestTypes = suitabilityTestService.GetMajorSuitabilityTestTypeSelectModels();
            vm.Batches = await batchService.GetBatchSelectModels();


            return Ok(new ResponseMessage<SuitabilityTestViewModel>
            {
                Result = vm
            });
        }


        [HttpGet]
        [Route("get-suitability-test-type-list")]
        public async Task<IHttpActionResult> GetSuitabilityTestTypeList()
        {

            return Ok(new ResponseMessage<List<SelectModel>>
            {
                Result = suitabilityTestService.GetMajorSuitabilityTestTypeSelectModels()
            });
        }


        [HttpPost]
        [ModelValidation]
        [Route("save-suitability-test")]
        public async Task<IHttpActionResult> SaveSuitabilityTest([FromBody] SuitabilityTestModel model)
        {
            return Ok(new ResponseMessage<SuitabilityTestModel>
            {
                Result = await suitabilityTestService.SaveSuitabilityTest(0, model)
            });
        }

        [HttpPut]
        [Route("update-suitability-test/{id}")]
        public async Task<IHttpActionResult> UpdateSuitabilityTest(int id, [FromBody] SuitabilityTestModel model)
        {
            return Ok(new ResponseMessage<SuitabilityTestModel>
            {
                Result = await suitabilityTestService.SaveSuitabilityTest(id, model)

            });
        }

        [HttpDelete]
        [Route("delete-suitability-test/{id}")]
        public async Task<IHttpActionResult> DeleteSuitabilityTest(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await suitabilityTestService.DeleteSuitabilityTest(id)
            });
        }



        [HttpGet]
        [Route("delete-suitability-test-type-list")]
        public async Task<IHttpActionResult> DeleteSuitabilityTestTypeList(int type)
        {

            return Ok(new ResponseMessage<bool>
            {
                Result = await suitabilityTestService.DeleteSuitabilityTestTypeList(type)
            });
        }



        [HttpGet]
        [Route("save-suitability-test-type-list")]
        public async Task<IHttpActionResult> SaveSuitabilityTestTypeList(int type, int batchId)
        {

            return Ok(new ResponseMessage<bool>
            {
                Result = await suitabilityTestService.SaveSuitabilityTestTypeList(type,batchId)
            });
        }



        [HttpGet]
        [Route("get-suitability-test-list-by-type")]
        public IHttpActionResult GetSuitabilityTestListByType(int type)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = suitabilityTestService.GetSuitabilityTestListByType(type)
            });
        }
    }
}