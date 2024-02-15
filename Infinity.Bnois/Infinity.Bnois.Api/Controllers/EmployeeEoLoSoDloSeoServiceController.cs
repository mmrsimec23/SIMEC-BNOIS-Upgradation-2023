
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
    [RoutePrefix(BnoisRoutePrefix.EmployeeEoLoSoDloSeoService)]
    [EnableCors("*", "*", "*")]
    [ActionAuthorize(Feature = MASTER_SETUP.EMPLOYEE_EOLOSODLOSEO_SERVICE)]

    public class EmployeeEoLoSoDloSeoServiceController : PermissionController
    {
        private readonly IEmployeeEoLoSoDloSeoService employeeEoLoSoDloSeoService;
        private readonly IOfficeService officeService;
        private readonly IPftResultService pftResultService;



        public EmployeeEoLoSoDloSeoServiceController(IEmployeeEoLoSoDloSeoService employeeEoLoSoDloSeoService,
            IOfficeService officeService, IPftResultService pftResultService, IRoleFeatureService roleFeatureService) : base(roleFeatureService)
        {
            this.employeeEoLoSoDloSeoService = employeeEoLoSoDloSeoService;
            this.officeService = officeService;
            this.pftResultService = pftResultService;


        }

        [HttpGet]
        [Route("get-employee-eolosodloseo-services")]
        public IHttpActionResult GetEmployeeEoLoSoDloSeoServices(int type,int ps, int pn, string qs)
        {
            int total = 0;
            List<EmployeeCoxoServiceModel> models = employeeEoLoSoDloSeoService.GetEmployeeEoLoSoDloSeoServices(type,ps, pn, qs, out total);
            RoleFeature permission = base.GetFeature(MASTER_SETUP.EMPLOYEE_EOLOSODLOSEO_SERVICE);
            return Ok(new ResponseMessage<List<EmployeeCoxoServiceModel>>()
            {
                Result = models,
                Total = total,
                Permission = permission
            });
        }

        [HttpGet]
        [Route("get-employee-eolosodloseo-service")]
        public async Task<IHttpActionResult> GetEmployeeEoLoSoDloSeoService(int id, int type)
        {
            EmployeeCoxoServiceViewModel vm = new EmployeeCoxoServiceViewModel();
            vm.EmployeeCoxoService = await employeeEoLoSoDloSeoService.GetEmployeeEoLoSoDloSeoService(id);
            vm.CoxoTypes = employeeEoLoSoDloSeoService.GetEoLoSoDloSeoTypeSelectModels();
            vm.CoxoShipTypes = officeService.GetShipTypeSelectModels();
            vm.CoxoAppoinments = employeeEoLoSoDloSeoService.GetEoLoSoDloSeoAppoinmentSelectModels(type);
            vm.Offices = await officeService.GetBornOfficeSelectModel();


            return Ok(new ResponseMessage<EmployeeCoxoServiceViewModel>
            {
                Result = vm
            });
        }


        [HttpGet]
        [Route("get-employee-eolosodloseo-service-office-list")]
        public async Task<IHttpActionResult> GetEmployeeEoLoSoDloSeoServiceOfficeList(int type)
        {

            return Ok(new ResponseMessage<List<SelectModel>>
            {
                Result = await officeService.GetOfficeByShipTypeSelectModel(type)
            });
        }


        [HttpPost]
        [ModelValidation]
        [Route("save-employee-eolosodloseo-service")]
        public async Task<IHttpActionResult> SaveEmployeeEoLoSoDloSeoService([FromBody] EmployeeCoxoServiceModel model)
        {
            return Ok(new ResponseMessage<EmployeeCoxoServiceModel>
            {
                Result = await employeeEoLoSoDloSeoService.SaveEmployeeEoLoSoDloSeoService(0, model)
            });
        }

        [HttpPut]
        [Route("update-employee-eolosodloseo-service/{id}")]
        public async Task<IHttpActionResult> UpdateEmployeeEoLoSoDloSeoService(int id, [FromBody] EmployeeCoxoServiceModel model)
        {
            return Ok(new ResponseMessage<EmployeeCoxoServiceModel>
            {
                Result = await employeeEoLoSoDloSeoService.SaveEmployeeEoLoSoDloSeoService(id, model)

            });
        }

        [HttpDelete]
        [Route("delete-employee-eolosodloseo-service/{id}")]
        public async Task<IHttpActionResult> DeleteEmployeeEoLoSoDloSeoService(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await employeeEoLoSoDloSeoService.DeleteEmployeeEoLoSoDloSeoService(id)
            });
        }
    }
}