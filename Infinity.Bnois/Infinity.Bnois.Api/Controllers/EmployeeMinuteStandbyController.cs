

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

    [RoutePrefix(BnoisRoutePrefix.EmployeeMinuteStandby)]
    [EnableCors("*", "*", "*")]
    [ActionAuthorize(Feature = MASTER_SETUP.EMPLOYEE_MINUTE_STANDBY)]

    public class EmployeeMinuteStandbyController : PermissionController
    {
        private readonly IEmployeeMinuteStandbyService employeeMinuteStandbyService;

        public EmployeeMinuteStandbyController(IEmployeeMinuteStandbyService employeeMinuteStandbyService, IRoleFeatureService roleFeatureService) : base(roleFeatureService)
        {
            this.employeeMinuteStandbyService = employeeMinuteStandbyService;
        }

        [HttpGet]
        [Route("get-employee-minute-standby-list")]
        public IHttpActionResult GetEmployeeMinuteStandbyList(int ps, int pn, string qs)
        {
            int total = 0;
            List<DashBoardMinuteStandby975Model> models = employeeMinuteStandbyService.GetEmployeeMinuteStandbys(ps, pn, qs, out total);
            RoleFeature permission = base.GetFeature(MASTER_SETUP.EMPLOYEE_MINUTE_STANDBY);
            return Ok(new ResponseMessage<List<DashBoardMinuteStandby975Model>>()
            {
                Result = models,
                Total = total, Permission=permission
            });
        }




        [HttpGet]
        [Route("get-employee-minute-standby")]
        public async Task<IHttpActionResult> GetEmployeeMinuteStandby(int id)
        {
            DashBoardMinuteStandby975Model model = await employeeMinuteStandbyService.GetEmployeeMinuteStandby(id);
            return Ok(new ResponseMessage<DashBoardMinuteStandby975Model>
            {
                Result = model
            });
        }

        [HttpPost]
        [ModelValidation]
        [Route("save-employee-minute-standby")]
        public async Task<IHttpActionResult> SaveEmployeeMinuteStandby([FromBody] DashBoardMinuteStandby975Model model)
        {
            
            return Ok(new ResponseMessage<DashBoardMinuteStandby975Model>
            {
                Result = await employeeMinuteStandbyService.SaveEmployeeMinuteStandby(0, model)
            });
        }

        [HttpPut]
        [Route("update-employee-minute-standby/{id}")]
        public async Task<IHttpActionResult> UpdateEmployeeMinuteStandby(int id, [FromBody] DashBoardMinuteStandby975Model model)
        {
            
            return Ok(new ResponseMessage<DashBoardMinuteStandby975Model>
            {
                Result = await employeeMinuteStandbyService.SaveEmployeeMinuteStandby(id, model)

            });
        }

        [HttpDelete]
        [Route("delete-employee-minute-standby/{id}")]
        public async Task<IHttpActionResult> DeleteEmployeeMinuteStandby(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await employeeMinuteStandbyService.DeleteEmployeeMinuteStandby(id)
            });
        }

    }
}