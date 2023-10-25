

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

    [RoutePrefix(BnoisRoutePrefix.EmployeeFamilyPermission)]
    [EnableCors("*", "*", "*")]
    [ActionAuthorize(Feature = MASTER_SETUP.EMPLOYEE_FAMILY_PERMISSION)]

    public class EmployeeEmployeeFamilyPermissionController : PermissionController
    {
        private readonly IEmployeeFamilyPermissionService employeeFamilyPermissionService;
        private readonly IRelationService relationService;
        private readonly ICountryService countryService;

        public EmployeeEmployeeFamilyPermissionController(IEmployeeFamilyPermissionService employeeFamilyPermissionService,
            IRelationService relationService, ICountryService countryService, IRoleFeatureService roleFeatureService) : base(roleFeatureService)
        {
            this.employeeFamilyPermissionService = employeeFamilyPermissionService;
            this.relationService = relationService;
            this.countryService = countryService;
        }

        [HttpGet]
        [Route("get-employee-family-permission-list")]
        public IHttpActionResult GetEmployeeFamilyPermissionList(int ps, int pn, string qs)
        {
            int total = 0;
            List<EmployeeFamilyPermissionModel> models = employeeFamilyPermissionService.GetEmployeeFamilyPermissions(ps, pn, qs, out total);
            RoleFeature permission = base.GetFeature(MASTER_SETUP.EMPLOYEE_FAMILY_PERMISSION);
            return Ok(new ResponseMessage<List<EmployeeFamilyPermissionModel>>()
            {
                Result = models,
                Total = total, Permission=permission
            });
        }




        [HttpGet]
        [Route("get-employee-family-permission")]
        public async Task<IHttpActionResult> GetEmployeeFamilyPermission(int id)
        {
            EmployeeFamilyPermissionViewModel vm = new EmployeeFamilyPermissionViewModel();
            vm.EmployeeFamilyPermission = await employeeFamilyPermissionService.GetEmployeeFamilyPermission(id);
            vm.FamilyPermissionRelationTypes = await relationService.GetRelationSelectModels();
            vm.FamilyPermissionCountryList = await countryService.GetCountriesTypeSelectModel();
            return Ok(new ResponseMessage<EmployeeFamilyPermissionViewModel>
            {
                Result = vm
            });
        }

        [HttpPost]
        [ModelValidation]
        [Route("save-employee-family-permission")]
        public async Task<IHttpActionResult> SaveEmployeeFamilyPermission([FromBody] EmployeeFamilyPermissionModel model)
        {
            return Ok(new ResponseMessage<EmployeeFamilyPermissionModel>
            {
                Result = await employeeFamilyPermissionService.SaveEmployeeFamilyPermission(0, model)
            });
        }

        [HttpPut]
        [Route("update-employee-family-permission/{id}")]
        public async Task<IHttpActionResult> UpdateEmployeeFamilyPermission(int id, [FromBody] EmployeeFamilyPermissionModel model)
        {
            return Ok(new ResponseMessage<EmployeeFamilyPermissionModel>
            {
                Result = await employeeFamilyPermissionService.SaveEmployeeFamilyPermission(id, model)

            });
        }

        [HttpDelete]
        [Route("delete-employee-family-permission/{id}")]
        public async Task<IHttpActionResult> DeleteEmployeeFamilyPermission(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await employeeFamilyPermissionService.DeleteEmployeeFamilyPermission(id)
            });
        }

    }
}