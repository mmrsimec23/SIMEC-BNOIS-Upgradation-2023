
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
    [RoutePrefix(BnoisRoutePrefix.EmployeeSmallCoxoService)]
    [EnableCors("*", "*", "*")]
    [ActionAuthorize(Feature = MASTER_SETUP.EMPLOYEE_SMALL_COXO_SERVICE)]

    public class EmployeeSmallCoxoServiceController : PermissionController
    {
        private readonly IEmployeeSmallCoxoService EmployeeSmallCoxoService;
        private readonly IOfficeService officeService;
        private readonly IPftResultService pftResultService;



        public EmployeeSmallCoxoServiceController(IEmployeeSmallCoxoService EmployeeSmallCoxoService,
            IOfficeService officeService, IPftResultService pftResultService, IRoleFeatureService roleFeatureService) : base(roleFeatureService)
        {
            this.EmployeeSmallCoxoService = EmployeeSmallCoxoService;
            this.officeService = officeService;
            this.pftResultService = pftResultService;


        }

        [HttpGet]
        [Route("get-employee-small-coxo-services")]
        public IHttpActionResult GetEmployeeSmallCoxoServices(int type,int ps, int pn, string qs)
        {
            int total = 0;
            List<EmployeeCoxoServiceModel> models = EmployeeSmallCoxoService.GetEmployeeSmallCoxoServices(type,ps, pn, qs, out total);
            RoleFeature permission = base.GetFeature(MASTER_SETUP.EMPLOYEE_SMALL_COXO_SERVICE);
            return Ok(new ResponseMessage<List<EmployeeCoxoServiceModel>>()
            {
                Result = models,
                Total = total,
                Permission = permission
            });
        }

        [HttpGet]
        [Route("get-employee-small-coxo-service")]
        public async Task<IHttpActionResult> GetEmployeeSmallCoxoService(int id, int type)
        {
            EmployeeCoxoServiceViewModel vm = new EmployeeCoxoServiceViewModel();
            vm.EmployeeCoxoService = await EmployeeSmallCoxoService.GetEmployeeSmallCoxoService(id);
            vm.CoxoTypes = EmployeeSmallCoxoService.GetSmallCoxoTypeSelectModels();
            vm.CoxoShipTypes = officeService.GetShipTypeSelectModels();
            vm.CoxoAppoinments = EmployeeSmallCoxoService.GetSmallCoxoAppoinmentSelectModels(type);
            vm.Offices = await officeService.GetBornOfficeSelectModel();


            return Ok(new ResponseMessage<EmployeeCoxoServiceViewModel>
            {
                Result = vm
            });
        }


        [HttpGet]
        [Route("get-employee-small-coxo-service-office-list")]
        public async Task<IHttpActionResult> GetEmployeeSmallCoxoServiceOfficeList(int type)
        {

            return Ok(new ResponseMessage<List<SelectModel>>
            {
                Result = await officeService.GetOfficeByShipTypeSelectModel(type)
            });
        }


        [HttpPost]
        [ModelValidation]
        [Route("save-employee-small-coxo-service")]
        public async Task<IHttpActionResult> SaveEmployeeSmallCoxoService([FromBody] EmployeeCoxoServiceModel model)
        {
            return Ok(new ResponseMessage<EmployeeCoxoServiceModel>
            {
                Result = await EmployeeSmallCoxoService.SaveEmployeeSmallCoxoService(0, model)
            });
        }

        [HttpPut]
        [Route("update-employee-small-coxo-service/{id}")]
        public async Task<IHttpActionResult> UpdateEmployeeSmallCoxoService(int id, [FromBody] EmployeeCoxoServiceModel model)
        {
            return Ok(new ResponseMessage<EmployeeCoxoServiceModel>
            {
                Result = await EmployeeSmallCoxoService.SaveEmployeeSmallCoxoService(id, model)

            });
        }

        [HttpDelete]
        [Route("delete-employee-small-coxo-service/{id}")]
        public async Task<IHttpActionResult> DeleteEmployeeSmallCoxoService(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await EmployeeSmallCoxoService.DeleteEmployeeSmallCoxoService(id)
            });
        }
    }
}