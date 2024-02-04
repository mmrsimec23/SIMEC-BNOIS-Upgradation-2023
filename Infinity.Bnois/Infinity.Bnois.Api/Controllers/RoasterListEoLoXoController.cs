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
using Infinity.Bnois.Data;

namespace Infinity.Bnois.Api.Controllers
{
    [RoutePrefix(BnoisRoutePrefix.RoasterListEoSoLo)]
    [EnableCors("*", "*", "*")]
    [ActionAuthorize(Feature = MASTER_SETUP.ROASTER_LIST_EOSOLO)]

    public class RoasterListEoLoXoController : BaseController
    {
        private readonly IRoasterListEoLoSoService roasterListService;

        public RoasterListEoLoXoController(IRoasterListEoLoSoService roasterListService)
        {
            this.roasterListService = roasterListService;
        }

        [HttpGet]
        [Route("get-roaster-list-for-eoloso-by-ship-type")]
        public IHttpActionResult GetRoasterListForEoLoSoByShipType(int shipType,int coxoStatus)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = roasterListService.GetRoasterListForEoLoSoByShipType(shipType, coxoStatus)
            });
        }

        [HttpGet]
        [Route("get-proposed-waiting-coxo-list")]
        public IHttpActionResult GetLargeShipProposedWaitingCoXoList(int officeId, int appointment)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = roasterListService.GetLargeShipProposedWaitingCoXoList(officeId, appointment)
            });
        }

        [HttpGet]
        [Route("get-large-ship-eosolo-waiting-list")]
        public IHttpActionResult GetLargeShipEoSoLoWaitingList()
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = roasterListService.GetLargeShipEoSoLoWaitingList()
            });
        }
        [HttpGet]
        [Route("get-large-ship-seodlo-waiting-list")]
        public IHttpActionResult GetLargeShipSeoDloWaitingList()
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = roasterListService.GetLargeShipSeoDloWaitingList()
            });
        }
        [HttpGet]
        [Route("get-medium-ship-eosolo-waiting-list")]
        public IHttpActionResult GetMediumShipEoSoLoWaitingList()
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = roasterListService.GetMediumShipEoSoLoWaitingList()
            });
        }

    }
}