using Infinity.Bnois.Api.Core;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Infinity.Bnois.Api.Right;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Configuration.Models;
using Infinity.Bnois.Api.Models;

namespace Infinity.Bnois.Api.Controllers
{
    [RoutePrefix(BnoisRoutePrefix.CareerForecastSetting)]
    [EnableCors("*","*","*")]
    [ActionAuthorize(Feature = MASTER_SETUP.CAREER_FORECAST_SETTING)]

    public class CareerForecastSettingController : PermissionController
    {
        private readonly ICareerForecastSettingService careerForecastSettingService;
        private readonly IBranchService branchService;
        public CareerForecastSettingController(ICareerForecastSettingService careerForecastSettingService, IBranchService branchService, IRoleFeatureService roleFeatureService) : base(roleFeatureService)
        {
            this.careerForecastSettingService = careerForecastSettingService;
            this.branchService = branchService;
        }

        [HttpGet]
        [Route("get-career-forecast-setting-list")]
        public IHttpActionResult GetCareerForecastSettings(int ps, int pn, string qs)
        {
            int total = 0;
            List<CareerForecastSettingModel> models = careerForecastSettingService.GetCareerForecastSettings(ps, pn, qs, out total);
            RoleFeature permission = base.GetFeature(MASTER_SETUP.CAREER_FORECAST_SETTING);
            return Ok(new ResponseMessage<List<CareerForecastSettingModel>>()
            {
                Result = models,
                Total = total, Permission=permission
            });
        }

        [HttpGet]
        [Route("get-career-forecast-setting")]
        public async Task<IHttpActionResult> GetCareerForecastSetting(int id)
        {
            CareerForecastSettingViewModel vm = new CareerForecastSettingViewModel();
            vm.CareerForecastSetting = await careerForecastSettingService.GetCareerForecastSetting(id);
            vm.Branches = await branchService.GetBranchSelectModels();
            return Ok(new ResponseMessage<CareerForecastSettingViewModel>
            {
                Result = vm
            });
        }

        [HttpPost]
        [ModelValidation]
        [Route("save-career-forecast-setting")]
        public async Task<IHttpActionResult> SaveCareerForecastSetting([FromBody] CareerForecastSettingModel model)
        {
            return Ok(new ResponseMessage<CareerForecastSettingModel>
            {
                Result = await careerForecastSettingService.SaveCareerForecastSetting(0, model)
            });
        }

        [HttpPut]
        [Route("update-career-forecast-setting/{id}")]
        public async Task<IHttpActionResult> UpdateCareerForecastSetting(int id, [FromBody] CareerForecastSettingModel model)
        {
            return Ok(new ResponseMessage<CareerForecastSettingModel>
            {
                Result = await careerForecastSettingService.SaveCareerForecastSetting(id, model)
            });
        }

        [HttpDelete]
        [Route("delete-career-forecast-setting/{id}")]
        public async Task<IHttpActionResult> DeleteCareerForecastSetting(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await careerForecastSettingService.DeleteCareerForecastSetting(id)
            });
        }
    }
}
