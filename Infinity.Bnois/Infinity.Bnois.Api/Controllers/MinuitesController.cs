using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.UI.WebControls;
using Infinity.Bnois.Api.Core;
using Infinity.Bnois.Api.Models;
using Infinity.Bnois.Api.Right;
using Infinity.Bnois.ApplicationService.Implementation;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Configuration.Models;

namespace Infinity.Bnois.Api.Controllers
{
    [RoutePrefix(BnoisRoutePrefix.minuites)]
    [EnableCors("*", "*", "*")]
    [ActionAuthorize(Feature = MASTER_SETUP.MINUITE)]

    public class MinuitesController : PermissionController
    {
        private readonly IMinuiteService minuiteService;
        private readonly ICountryService countryService;

        public MinuitesController(IMinuiteService minuiteService, ICountryService countryService, IRoleFeatureService roleFeatureService) : base(roleFeatureService)
        {
            this.minuiteService = minuiteService;
            this.countryService = countryService;
        }

        [HttpGet]
        [Route("get-minuites")]
        public IHttpActionResult GetMinuites(int ps, int pn, string qs)
        {
            int total = 0;
            List<DashBoardMinuite100Model> models = minuiteService.GetMinuites(ps, pn, qs, out total);
            RoleFeature permission = base.GetFeature(MASTER_SETUP.MINUITE);
            return Ok(new ResponseMessage<List<DashBoardMinuite100Model>>()
            {
                Result = models,
                Total = total, Permission=permission
            });
        }

        [HttpGet]
        [Route("get-minuite")]
        public async Task<IHttpActionResult> GetMinuite(int id)
        {
            MinuiteViewModel vm = new MinuiteViewModel();
            vm.Minuite = await minuiteService.GetMinuite(id);
            vm.Countries = await countryService.GetCountriesTypeSelectModel();
            
            return Ok(new ResponseMessage<MinuiteViewModel>
            {
                Result = vm
            });
        }


        [HttpPost]
        [ModelValidation]
        [Route("save-minuite")]
        public async Task<IHttpActionResult> SaveMinuite([FromBody] DashBoardMinuite100Model model)
        {

            if (model.Employee != null)
            {
                model.EmployeeId = model.Employee.EmployeeId;
            }
            return Ok(new ResponseMessage<DashBoardMinuite100Model>
            {
                Result = await minuiteService.SaveMinuite(0, model)
            });
        }



        [HttpPut]
        [Route("update-minuite/{id}")]
        public async Task<IHttpActionResult> UpdateMinuite(int id, [FromBody] DashBoardMinuite100Model model) => Ok(new ResponseMessage<DashBoardMinuite100Model>
        {
            Result = await minuiteService.SaveMinuite(id, model)
        });



        [HttpDelete]
        [Route("delete-minuite/{id}")]
        public async Task<IHttpActionResult> DeleteMinuite(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await minuiteService.DeleteMinuite(id)
            });
        }


    }
}