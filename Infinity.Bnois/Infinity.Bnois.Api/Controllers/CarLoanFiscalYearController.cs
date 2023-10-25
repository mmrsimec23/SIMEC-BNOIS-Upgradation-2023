using Infinity.Bnois.Api.Core;
using Infinity.Bnois.Api.Right;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Configuration.Models;

namespace Infinity.Bnois.Api.Controllers
{
    [RoutePrefix(BnoisRoutePrefix.CarLoanFiscalYear)]
    [EnableCors("*", "*", "*")]
    [ActionAuthorize(Feature = MASTER_SETUP.CAR_LOAN_FISCAL_YEAR)]
    public class CarLoanFiscalYearController : PermissionController
    {
        private readonly ICarLoanFiscalYearService carLoanFiscalYearService;
        public CarLoanFiscalYearController(ICarLoanFiscalYearService carLoanFiscalYearService, IRoleFeatureService roleFeatureService) : base(roleFeatureService)
        {
            this.carLoanFiscalYearService = carLoanFiscalYearService;
        }

        [HttpGet]
        [Route("get-car-loan-fiscal-year-list")]
        public IHttpActionResult GetCarLoanFiscalYearList(int ps, int pn, string qs)
        {
            int total = 0;
            List<CarLoanFiscalYearModel> models = carLoanFiscalYearService.GetCarLoanFiscalYears(ps, pn, qs, out total);
            RoleFeature permission = base.GetFeature(MASTER_SETUP.CAR_LOAN_FISCAL_YEAR);
            return Ok(new ResponseMessage<List<CarLoanFiscalYearModel>>()
            {
                Result = models,
                Total = total, Permission=permission
            });
        }

        [HttpGet]
        [Route("get-car-loan-fiscal-year")]
        public async Task<IHttpActionResult> GetCarLoanFiscalYear(int id)
        {
            CarLoanFiscalYearModel model = await carLoanFiscalYearService.GetCarLoanFiscalYear(id);
            return Ok(new ResponseMessage<CarLoanFiscalYearModel>
            {
                Result = model
            });
        }


        [HttpPost]
        [ModelValidation]
        [Route("save-car-loan-fiscal-year")]
        public async Task<IHttpActionResult> SaveCarLoanFiscalYear([FromBody] CarLoanFiscalYearModel model)
        {
            return Ok(new ResponseMessage<CarLoanFiscalYearModel>
            {
                Result = await carLoanFiscalYearService.SaveCarLoanFiscalYear(0, model)
            });
        }



        [HttpPut]
        [Route("update-car-loan-fiscal-year/{id}")]
        public async Task<IHttpActionResult> UpdateCarLoanFiscalYear(int id, [FromBody] CarLoanFiscalYearModel model)
        {
            return Ok(new ResponseMessage<CarLoanFiscalYearModel>
            {
                Result = await carLoanFiscalYearService.SaveCarLoanFiscalYear(id, model)
            });
        }


        [HttpDelete]
        [Route("delete-car-loan-fiscal-year/{id}")]
        public async Task<IHttpActionResult> DeleteSport(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await carLoanFiscalYearService.DeleteCarLoanFiscalYear(id)
            });
        }
    }
}
