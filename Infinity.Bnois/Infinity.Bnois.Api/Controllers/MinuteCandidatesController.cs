using Infinity.Bnois.Api.Core;
using Infinity.Bnois.ApplicationService.Implementation;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Infinity.Bnois.Api.Controllers
{
    [RoutePrefix(BnoisRoutePrefix.minuiteCandidate)]
    [EnableCors("*", "*", "*")]
    public class MinuteCandidatesController : BaseController
    {
        private readonly IMinuteCandidateService minuteCandidateService;
        public MinuteCandidatesController(IMinuteCandidateService minuteCandidateService)
        {
            this.minuteCandidateService = minuteCandidateService;
        }

        [HttpGet]
        [Route("get-minute-candidates/{minuiteId}")]
        public IHttpActionResult GetMinuteCandidates(int minuiteId)
        {
            int total = 0;
            List<DashBoardMinuite110Model> models = minuteCandidateService.GetMinuteCandidates(minuiteId);
            return Ok(new ResponseMessage<List<DashBoardMinuite110Model>>()
            {
                Result = models
            });
        }

        [HttpGet]
        [Route("get-minute-candidate")]
        public async Task<IHttpActionResult> getMinuteCandidate(int id)
        {
            DashBoardMinuite110Model model = await minuteCandidateService.getMinuteCadidate(id);
            
            return Ok(new ResponseMessage<DashBoardMinuite110Model>
            {
                Result = model
            });
        }

        [HttpPost]
        [ModelValidation]
        [Route("save-minute-candidate/{minuiteId}")]
        public async Task<IHttpActionResult> SaveMinuteCadidate(int minuiteId, [FromBody] DashBoardMinuite110Model model)
        {
            model.MinuiteId = minuiteId;
            return Ok(new ResponseMessage<DashBoardMinuite110Model>
            {
                Result = await minuteCandidateService.SaveMinuteCadidate(0, model)
            });
        }


        [HttpDelete]
        [Route("delete-minute-candidate/{minuiteCandidateId}")]
        public async Task<IHttpActionResult> DeleteMinuteCadidate(int minuiteCandidateId)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await minuteCandidateService.DeleteMinuteCadidate(minuiteCandidateId)
            });
        }
    }
}
