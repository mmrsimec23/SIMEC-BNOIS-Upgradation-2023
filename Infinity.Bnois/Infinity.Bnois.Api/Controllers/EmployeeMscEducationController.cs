

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

    [RoutePrefix(BnoisRoutePrefix.EmployeeMscEducation)]
    [EnableCors("*", "*", "*")]
    [ActionAuthorize(Feature = MASTER_SETUP.EMPLOYEE_MSC_EDUCATION)]

    public class EmployeeEmployeeMscEducationController : PermissionController
    {
        private readonly IEmployeeMscEducationService employeeMscEducationService;
        private readonly IMscEducationTypeService mscEducationTypeService;
        private readonly IMscInstituteService mscInstituteService;
        private readonly IMscPermissionTypeService mscPermissionTypeService;
        private readonly ICountryService countryService;

        public EmployeeEmployeeMscEducationController(IEmployeeMscEducationService employeeMscEducationService, IMscPermissionTypeService mscPermissionTypeService, IMscEducationTypeService mscEducationTypeService,
            IMscInstituteService mscInstituteService, ICountryService countryService, IRoleFeatureService roleFeatureService) : base(roleFeatureService)
        {
            this.employeeMscEducationService = employeeMscEducationService;
            this.mscEducationTypeService = mscEducationTypeService;
            this.mscInstituteService = mscInstituteService;
            this.mscPermissionTypeService = mscPermissionTypeService;
            this.countryService = countryService;
        }

        [HttpGet]
        [Route("get-employee-msc-education-list")]
        public IHttpActionResult GetEmployeeMscEducationList(int ps, int pn, string qs)
        {
            int total = 0;
            List<EmployeeMscEducationModel> models = employeeMscEducationService.GetEmployeeMscEducations(ps, pn, qs, out total);
            RoleFeature permission = base.GetFeature(MASTER_SETUP.EMPLOYEE_MSC_EDUCATION);
            return Ok(new ResponseMessage<List<EmployeeMscEducationModel>>()
            {
                Result = models,
                Total = total, Permission=permission
            });
        }




        [HttpGet]
        [Route("get-employee-msc-education")]
        public async Task<IHttpActionResult> GetEmployeeMscEducation(int id)
        {
            EmployeeMscEducationViewModel vm = new EmployeeMscEducationViewModel();
            vm.EmployeeMscEducation = await employeeMscEducationService.GetEmployeeMscEducation(id);
            vm.MscEducationTypeList = await mscEducationTypeService.GetMscEducationTypesSelectModels();
            vm.MscInstituteList = await mscInstituteService.GetMscInstitutesSelectModels();
            vm.MscPermissionTypeList = await mscPermissionTypeService.GetMscPermissionTypesSelectModels();
            vm.CountryList = await countryService.GetCountriesTypeSelectModel();
            return Ok(new ResponseMessage<EmployeeMscEducationViewModel>
            {
                Result = vm
            });
        }

        [HttpPost]
        [ModelValidation]
        [Route("save-employee-msc-education")]
        public async Task<IHttpActionResult> SaveEmployeeMscEducation([FromBody] EmployeeMscEducationModel model)
        {
            return Ok(new ResponseMessage<EmployeeMscEducationModel>
            {
                Result = await employeeMscEducationService.SaveEmployeeMscEducation(0, model)
            });
        }

        [HttpPut]
        [Route("update-employee-msc-education/{id}")]
        public async Task<IHttpActionResult> UpdateEmployeeMscEducation(int id, [FromBody] EmployeeMscEducationModel model)
        {
            return Ok(new ResponseMessage<EmployeeMscEducationModel>
            {
                Result = await employeeMscEducationService.SaveEmployeeMscEducation(id, model)

            });
        }

        [HttpDelete]
        [Route("delete-employee-msc-education/{id}")]
        public async Task<IHttpActionResult> DeleteEmployeeMscEducation(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await employeeMscEducationService.DeleteEmployeeMscEducation(id)
            });
        }

    }
}