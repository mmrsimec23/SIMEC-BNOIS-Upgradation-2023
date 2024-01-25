
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
    [RoutePrefix(BnoisRoutePrefix.EmployeeCoXoService)]
    [EnableCors("*", "*", "*")]
    [ActionAuthorize(Feature = MASTER_SETUP.EMPLOYEE_COXO_SERVICE)]

    public class EmployeeCoXoServiceController : PermissionController
    {
        private readonly IEmployeeCoxoService EmployeeCoxoService;
        private readonly IOfficeService officeService;
        private readonly IPftResultService pftResultService;



        public EmployeeCoXoServiceController(IEmployeeCoxoService EmployeeCoxoService,
            IOfficeService officeService, IPftResultService pftResultService, IRoleFeatureService roleFeatureService) : base(roleFeatureService)
        {
            this.EmployeeCoxoService = EmployeeCoxoService;
            this.officeService = officeService;
            this.pftResultService = pftResultService;


        }

        [HttpGet]
        [Route("get-employee-coxo-services")]
        public IHttpActionResult GetEmployeeCoXoService(int type,int ps, int pn, string qs)
        {
            int total = 0;
            List<EmployeeCoxoServiceModel> models = EmployeeCoxoService.GetEmployeeCoxoServices(type,ps, pn, qs, out total);
            RoleFeature permission = base.GetFeature(MASTER_SETUP.EMPLOYEE_COXO_SERVICE);
            return Ok(new ResponseMessage<List<EmployeeCoxoServiceModel>>()
            {
                Result = models,
                Total = total,
                Permission = permission
            });
        }

        [HttpGet]
        [Route("get-employee-coxo-service")]
        public async Task<IHttpActionResult> GetEmployeeCoxoService(int id, int type)
        {
            EmployeeCoxoServiceViewModel vm = new EmployeeCoxoServiceViewModel();
            vm.EmployeeCoxoService = await EmployeeCoxoService.GetEmployeeCoxoService(id);
            vm.CoxoTypes = EmployeeCoxoService.GetCoxoTypeSelectModels();
            vm.CoxoShipTypes = officeService.GetShipTypeSelectModels();
            vm.CoxoAppoinments = EmployeeCoxoService.GetCoxoAppoinmentSelectModels(type);
            vm.Offices = await officeService.GetBornOfficeSelectModel();


            return Ok(new ResponseMessage<EmployeeCoxoServiceViewModel>
            {
                Result = vm
            });
        }


        [HttpGet]
        [Route("get-employee-coxo-service-office-list")]
        public async Task<IHttpActionResult> GetEmployeeCoxoServiceOfficeList(int type)
        {

            return Ok(new ResponseMessage<List<SelectModel>>
            {
                Result = await officeService.GetOfficeByShipTypeSelectModel(type)
            });
        }


        [HttpPost]
        [ModelValidation]
        [Route("save-employee-coxo-service")]
        public async Task<IHttpActionResult> SaveEmployeeCoxoService([FromBody] EmployeeCoxoServiceModel model)
        {
            return Ok(new ResponseMessage<EmployeeCoxoServiceModel>
            {
                Result = await EmployeeCoxoService.SaveEmployeeCoxoService(0, model)
            });
        }

        [HttpPut]
        [Route("update-employee-coxo-service/{id}")]
        public async Task<IHttpActionResult> UpdateEmployeeCoxoService(int id, [FromBody] EmployeeCoxoServiceModel model)
        {
            return Ok(new ResponseMessage<EmployeeCoxoServiceModel>
            {
                Result = await EmployeeCoxoService.SaveEmployeeCoxoService(id, model)

            });
        }

        [HttpDelete]
        [Route("delete-employee-coxo-service/{id}")]
        public async Task<IHttpActionResult> DeleteEmployeeCoxoService(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await EmployeeCoxoService.DeleteEmployeeCoxoService(id)
            });
        }
    }
}