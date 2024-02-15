using Infinity.Bnois.Api.Core;
using Infinity.Bnois.Api.Models;
using Infinity.Bnois.Api.Right;
using Infinity.Bnois.ApplicationService.Implementation;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Configuration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Infinity.Bnois.Api.Controllers
{
    [RoutePrefix(BnoisRoutePrefix.EmployeeTrace)]
    [EnableCors("*", "*", "*")]
    [ActionAuthorize(Feature = MASTER_SETUP.EMPLOYEE_TRACE)]
    public class EmployeeTraceController: PermissionController
    {
        private readonly IEmployeeTraceService _employeeTraceService;
        private readonly IRankService _rankService;
        public EmployeeTraceController(
            IEmployeeTraceService employeeTraceService, 
            IRankService rankService, 
            IRoleFeatureService roleFeatureService
            ) : base(roleFeatureService)
        {
            _employeeTraceService = employeeTraceService;
            _rankService = rankService;
        }
        [HttpGet]
        [Route("get-employee-trace-list")]
        public IHttpActionResult GetEmployeeTraceList(int ps, int pn, string qs)
        {
            int total = 0;
            List<EmployeeTraceModel> models = _employeeTraceService.GetEmployeeTraceList(ps, pn, qs, out total);
            RoleFeature permission = base.GetFeature(MASTER_SETUP.EMPLOYEE_TRACE);
            return Ok(new ResponseMessage<List<EmployeeTraceModel>>()
            {
                Result = models,
                Total = total,
                Permission = permission
            });
        }
        [HttpGet]
        [Route("get-employee-trace-by-pno")]
        public IHttpActionResult GetEmployeeTracesByPno(string PNo)
        {
            return Ok(new ResponseMessage<List<EmployeeTraceModel>>()
            {
                Result = _employeeTraceService.GetEmployeeTracesByPno(PNo)
        });
        }
        [HttpGet]
        [Route("get-employee-trace")]
        public async Task<IHttpActionResult> getEmployeeTrace(int id)
        {
            EmployeeTraceModel model = await _employeeTraceService.getEmployeeTrace(id);
            //vm.EmployeeTrace = await _employeeTraceService.getEmployeeTrace(id);
            //vm.TraceFiscalYears = await _TraceFiscalYearService.GetTraceFiscalYearsSelectModels();
            //vm.Ranks = await _rankService.GetRanksSelectModel();
            //return Ok(new ResponseMessage<EmployeeTraceModel>
            //{
            //    Result = vm
            //});
            return Ok(new ResponseMessage<EmployeeTraceModel>
            {
                Result = model
            });
        }
        [HttpPost]
        [ModelValidation] 
        [Route("save-employee-trace")]
        public async Task<IHttpActionResult> saveEmployeeTrace([FromBody] EmployeeTraceModel model)
        {
            return Ok(new ResponseMessage<EmployeeTraceModel>
            {
                Result = await _employeeTraceService.SaveEmployeeTrace(0, model)
            });
        }
        [HttpPut]
        [Route("update-employee-trace/{id}")]
        public async Task<IHttpActionResult> updateEmployeeTrace(int id, [FromBody] EmployeeTraceModel model)
        {
            return Ok(new ResponseMessage<EmployeeTraceModel>
            {
                Result = await _employeeTraceService.SaveEmployeeTrace(id, model)

            });
        }

        [HttpDelete]
        [Route("delete-employee-trace/{id}")]
        public async Task<IHttpActionResult> DeleteEmployeeTrace(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await _employeeTraceService.DeleteEmployeeTrace(id)
            });
        }

    }
}
