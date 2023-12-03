

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
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

    [RoutePrefix(BnoisRoutePrefix.EmployeeUnmDeferment)]
    [EnableCors("*", "*", "*")]
    [ActionAuthorize(Feature = MASTER_SETUP.EMPLOYEE_UNM_DEFERMENT)]

    public class EmployeeUnmDefermentController : PermissionController
    {
        private readonly IEmployeeUnmDefermentService employeeUnmDefermentService;

        public EmployeeUnmDefermentController(IEmployeeUnmDefermentService employeeUnmDefermentService, IRoleFeatureService roleFeatureService) : base(roleFeatureService)
        {
            this.employeeUnmDefermentService = employeeUnmDefermentService;
        }

        [HttpGet]
        [Route("get-employee-unm-deferment-list")]
        public IHttpActionResult GetEmployeeUnmDefermentList(int ps, int pn, string qs)
        {
            int total = 0;
            List<DashBoardBranch975Model> models = employeeUnmDefermentService.GetEmployeeUnmDeferments(ps, pn, qs, out total);
            RoleFeature permission = base.GetFeature(MASTER_SETUP.EMPLOYEE_UNM_DEFERMENT);
            return Ok(new ResponseMessage<List<DashBoardBranch975Model>>()
            {
                Result = models,
                Total = total, Permission=permission
            });
        }




        [HttpGet]
        [Route("get-employee-unm-deferment")]
        public async Task<IHttpActionResult> GetEmployeeUnmDeferment(int id)
        {
            DashBoardBranch975Model model = await employeeUnmDefermentService.GetEmployeeUnmDeferment(id);
            return Ok(new ResponseMessage<DashBoardBranch975Model>
            {
                Result = model
            });
        }

        [HttpPost]
        [ModelValidation]
        [Route("save-employee-unm-deferment")]
        public async Task<IHttpActionResult> SaveEmployeeUnmDeferment([FromBody] DashBoardBranch975Model model)
        {
            
            return Ok(new ResponseMessage<DashBoardBranch975Model>
            {
                Result = await employeeUnmDefermentService.SaveEmployeeUnmDeferment(0, model)
            });
        }

        [HttpPut]
        [Route("update-employee-unm-deferment/{id}")]
        public async Task<IHttpActionResult> UpdateEmployeeUnmDeferment(int id, [FromBody] DashBoardBranch975Model model)
        {
            
            return Ok(new ResponseMessage<DashBoardBranch975Model>
            {
                Result = await employeeUnmDefermentService.SaveEmployeeUnmDeferment(id, model)

            });
        }

        [HttpDelete]
        [Route("delete-employee-unm-deferment/{id}")]
        public async Task<IHttpActionResult> DeleteEmployeeUnmDeferment(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await employeeUnmDefermentService.DeleteEmployeeUnmDeferment(id)
            });
        }

    }
}