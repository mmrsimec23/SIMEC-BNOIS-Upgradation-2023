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
        public IHttpActionResult GetRoasterListByShipType(int shipType)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = roasterListService.GetRoasterListByShipType(shipType)
            });
        }

    }
}