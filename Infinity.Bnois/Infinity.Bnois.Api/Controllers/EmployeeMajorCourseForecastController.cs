
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
    [RoutePrefix(BnoisRoutePrefix.EmployeeMajorCourseForecast)]
    [EnableCors("*", "*", "*")]
    //[ActionAuthorize(Feature = MASTER_SETUP.EMPLOYEE_COXO_SERVICE)]

    public class EmployeeMajorCourseForecastController : PermissionController
    {
        private readonly IMajorCourseForecastService MajorCourseForecastService;



        public EmployeeMajorCourseForecastController(IMajorCourseForecastService MajorCourseForecastService, IRoleFeatureService roleFeatureService) : base(roleFeatureService)
        {
            this.MajorCourseForecastService = MajorCourseForecastService;


        }

        [HttpGet]
        [Route("get-major-course-forecasts")]
        public IHttpActionResult GetMajorCourseForecasts(int type,int ps, int pn, string qs)
        {
            int total = 0;
            List<MajorCourseForecastModel> models = MajorCourseForecastService.GetMajorCourseForecasts(type,ps, pn, qs, out total);
            //RoleFeature permission = base.GetFeature(MASTER_SETUP.EMPLOYEE_COXO_SERVICE);
            return Ok(new ResponseMessage<List<MajorCourseForecastModel>>()
            {
                Result = models,
                Total = total
                //Permission = permission
            });
        }

        [HttpGet]
        [Route("get-major-course-forecast")]
        public async Task<IHttpActionResult> GetMajorCourseForecast(int id)
        {
            MajorCourseForecastViewModel vm = new MajorCourseForecastViewModel();
            vm.MajorCourseForecasts = await MajorCourseForecastService.GetMajorCourseForecast(id);
            vm.CourseTypes = MajorCourseForecastService.GetMajorCourseTypeSelectModels();


            return Ok(new ResponseMessage<MajorCourseForecastViewModel>
            {
                Result = vm
            });
        }


        [HttpGet]
        [Route("get-major-course-type-list")]
        public async Task<IHttpActionResult> GetMajorCourseTypeList()
        {

            return Ok(new ResponseMessage<List<SelectModel>>
            {
                Result =  MajorCourseForecastService.GetMajorCourseTypeSelectModels()
            });
        }


        [HttpPost]
        [ModelValidation]
        [Route("save-major-course-forecast")]
        public async Task<IHttpActionResult> SaveMajorCourseForecast([FromBody] MajorCourseForecastModel model)
        {
            return Ok(new ResponseMessage<MajorCourseForecastModel>
            {
                Result = await MajorCourseForecastService.SaveMajorCourseForecast(0, model)
            });
        }

        [HttpPut]
        [Route("update-major-course-forecast/{id}")]
        public async Task<IHttpActionResult> UpdateMajorCourseForecast(int id, [FromBody] MajorCourseForecastModel model)
        {
            return Ok(new ResponseMessage<MajorCourseForecastModel>
            {
                Result = await MajorCourseForecastService.SaveMajorCourseForecast(id, model)

            });
        }

        [HttpDelete]
        [Route("delete-major-course-forecast/{id}")]
        public async Task<IHttpActionResult> DeleteMajorCourseForecast(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await MajorCourseForecastService.DeleteMajorCourseForecast(id)
            });
        }
    }
}