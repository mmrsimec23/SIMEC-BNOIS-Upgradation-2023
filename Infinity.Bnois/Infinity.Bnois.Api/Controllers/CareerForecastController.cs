

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
using Infinity.Bnois.Data;
using Infinity.Ers.ApplicationService;

namespace Infinity.Bnois.Api.Controllers
{

    [RoutePrefix(BnoisRoutePrefix.CareerForecast)]
    [EnableCors("*", "*", "*")]
    [ActionAuthorize(Feature = MASTER_SETUP.EMPLOYEES)]

    public class CareerForecastController : PermissionController
    {
        private readonly ICareerForecastService careerForecastService;
        private readonly IEmployeeGeneralService employeeGeneralService;
        private readonly ICareerForecastSettingService careerForecastSettingService;
        

        public CareerForecastController(ICareerForecastService careerForecastService, ICareerForecastSettingService careerForecastSettingService, IEmployeeGeneralService employeeGeneralService,  IRoleFeatureService roleFeatureService) : base(roleFeatureService)
        {
            this.careerForecastService = careerForecastService;
            this.employeeGeneralService = employeeGeneralService;
            this.careerForecastSettingService = careerForecastSettingService;
        }

        [HttpGet]
        [Route("get-career-forecast-list")]
        public IHttpActionResult GetCareerForecastList(int ps, int pn, string qs)
        {
            int total = 0;
            List<CareerForecastModel> models = careerForecastService.GetCareerForecasts(ps, pn, qs, out total);
            RoleFeature permission = base.GetFeature(MASTER_SETUP.CAREER_FORECAST);
            return Ok(new ResponseMessage<List<CareerForecastModel>>()
            {
                Result = models,
                Total = total, Permission=permission
            });
        }




        [HttpGet]
        [Route("get-career-forecast")]
        public async Task<IHttpActionResult> GetCareerForecast(int id)
        {
            return Ok(new ResponseMessage<CareerForecastModel>
            {
                Result = await careerForecastService.GetCareerForecast(id)
            });
        }


        [HttpGet]
        [Route("get-career-forecast-setting-list")]
        public async Task<IHttpActionResult> GetCareerForecastSettingList(int id)
        {
            var branch = await employeeGeneralService.GetEmployeeGenerals(id);

            List<EmployeeCareerForecastSettingListModel> employeeForecasts = careerForecastSettingService.GetCareerForecastSettingByBranch(branch.BranchId);

            //EmployeeCareerForecastViewModel model = new EmployeeCareerForecastViewModel();
            //model.EmployeeCareerForecast = await employeeCareerForecastService.GetEmployeeCareerForecast(id);
            //model.QexamList = employeeCareerForecastService.getQexamListSelectModel();
            //model.SceeList = employeeCareerForecastService.getSceeListSelectModel();
            
            return Ok(new ResponseMessage<List<EmployeeCareerForecastSettingListModel>>
            {
                Result = employeeForecasts
            });
        }


        [HttpGet]
        [Route("get-career-forecasts-by-employee")]
        public IHttpActionResult GetCareerForecastsByEmployee(int employeeId)
        {
            List<CareerForecastModel> models = careerForecastService.GetCareerForecastsByEmployee(employeeId);
            RoleFeature permission = base.GetFeature(MASTER_SETUP.EMPLOYEES);
            return Ok(new ResponseMessage<List<CareerForecastModel>>()
            {
                Result = models,
                Permission = permission
            });

        }

        [HttpGet]
        [Route("get-career-forecast-by-employee")]
        public async Task<IHttpActionResult> GetCareerForecastByEmployee(int employeeId, int careerForecastId)
        {
            

            CareerForecastViewModel vm = new CareerForecastViewModel();
            vm.CareerForecast = await careerForecastService.GetCareerForecast(careerForecastId);
            
            vm.CareerForecastSettingList = await careerForecastSettingService.GetCareerForecastByBranchSettingSelectModels(employeeId);

            return Ok(new ResponseMessage<CareerForecastViewModel>()
            {
                Result = vm
            });
        }

        [HttpPost]
        [ModelValidation]
        [Route("save-career-forecast-by-employee/{employeeId}")]
        public async Task<IHttpActionResult> SaveCareerForecastByEmployee(int employeeId, [FromBody] CareerForecastModel model)
        {
            model.EmployeeId = employeeId;

            var branch = await employeeGeneralService.GetEmployeeGenerals(employeeId);
            model.BranchId = branch.BranchId;
            //model.ForecastStatus = true;

            return Ok(new ResponseMessage<CareerForecastModel>()
            {
                Result = await careerForecastService.SaveCareerForecast(0, model)
            });
        }

        [HttpPost]
        [ModelValidation]
        [Route("save-career-forecast")]
        public async Task<IHttpActionResult> SaveCareerForecast([FromBody] CareerForecastModel model)
        {
            return Ok(new ResponseMessage<CareerForecastModel>
            {
                Result = await careerForecastService.SaveCareerForecast(0, model)
            });
        }

        [HttpPut]
        [Route("update-career-forecast/{id}")]
        public async Task<IHttpActionResult> UpdateCareerForecast(int id, [FromBody] CareerForecastModel model)
        {
            return Ok(new ResponseMessage<CareerForecastModel>
            {
                Result = await careerForecastService.SaveCareerForecast(id, model)

            });
        }

        [HttpDelete]
        [Route("delete-career-forecast/{id}")]
        public async Task<IHttpActionResult> DeleteCareerForecast(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await careerForecastService.DeleteCareerForecast(id)
            });
        }

    }
}