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
    [RoutePrefix(BnoisRoutePrefix.RoasterList)]
    [EnableCors("*", "*", "*")]
    [ActionAuthorize(Feature = MASTER_SETUP.ROASTER_LIST)]

    public class RoasterListController : BaseController
    {
        private readonly IRoasterListService roasterListService;

        public RoasterListController(IRoasterListService roasterListService)
        {
            this.roasterListService = roasterListService;
        }

        [HttpGet]
        [Route("get-roaster-list-by-ship-type")]
        public IHttpActionResult GetRoasterListByShipType(int shipType,int coxoStatus)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = roasterListService.GetRoasterListByShipType(shipType, coxoStatus)
            });
        }

        [HttpGet]
        [Route("get-proposed-waiting-coxo-list")]
        public IHttpActionResult GetLargeShipProposedWaitingCoXoList(int shipType,int officeId, int appointment)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = roasterListService.GetLargeShipProposedWaitingCoXoList(shipType, officeId, appointment)
            });
        }

        [HttpGet]
        [Route("get-large-ship-co-waiting-list")]
        public IHttpActionResult GetLargeShipCoWaitingList()
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = roasterListService.GetLargeShipCoWaitingList()
            });
        }
        [HttpGet]
        [Route("get-large-ship-xo-waiting-list")]
        public IHttpActionResult GetLargeShipXoWaitingList()
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = roasterListService.GetLargeShipXoWaitingList()
            });
        }
        [HttpGet]
        [Route("get-medium-ship-co-waiting-list")]
        public IHttpActionResult GetMediumShipCoWaitingList()
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = roasterListService.GetMediumShipCoWaitingList()
            });
        }


        [HttpGet]
        [Route("get-small-ship-coxo-pending-list")]
        public IHttpActionResult GetSmallShipCoXoPendingList()
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = roasterListService.GetSmallShipCoXoPendingList()
            });
        }

        [HttpGet]
        [Route("get-small-ship-coxo-waiting-list")]
        public IHttpActionResult GetSmallShipCoXoWaitingList()
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = roasterListService.GetSmallShipCoXoPendingList()
            });
        }

        [HttpGet]
        [Route("get-roaster-list-by-small-ship")]
        public IHttpActionResult GetRoasterListBySmallShip(int shipType, int coxoStatus, int viewStatus)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = roasterListService.GetSmallShipCoXoWaitingList(shipType, coxoStatus, viewStatus)
            });
        }
    }
}