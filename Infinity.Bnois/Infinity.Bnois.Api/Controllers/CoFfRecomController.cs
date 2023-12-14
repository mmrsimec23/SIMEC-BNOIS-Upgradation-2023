using Infinity.Bnois.Api.Core;
using Infinity.Bnois.Api.Right;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Configuration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Infinity.Bnois.Api.Controllers
{
    [RoutePrefix(BnoisRoutePrefix.COFFRecom)]
    [EnableCors("*", "*", "*")]
    [ActionAuthorize(Feature = MASTER_SETUP.CO_FF_RECOM)]
    public class CoFfRecomController : PermissionController
    {
        private readonly ICoFfRecomService coFfRecomService;
        public CoFfRecomController(ICoFfRecomService coFfRecomService, IRoleFeatureService roleFeatureService) : base(roleFeatureService)
        {
            this.coFfRecomService = coFfRecomService;
        }

        [HttpGet]
        [Route("get-co-ff-recoms")]
        public IHttpActionResult GetCOFFRecoms()
        {
            int total = 0;
            List<CoFfRecomModel> models = coFfRecomService.GetCOFFRecoms();
            RoleFeature permission = base.GetFeature(MASTER_SETUP.CO_FF_RECOM);
            return Ok(new ResponseMessage<List<CoFfRecomModel>>()
            {
                Result = models,
                Permission = permission
            });
        }

        [HttpPost]
        [ModelValidation]
        [Route("save-co-ff-recom/{id}")]
        public async Task<IHttpActionResult> SaveCOFFRecom(int id, [FromBody] CoFfRecomModel model)
        {
            return Ok(new ResponseMessage<CoFfRecomModel>
            {
                Result = await coFfRecomService.SaveCOFFRecom(0, model)
            });
        }


        [HttpDelete]
        [Route("delete-co-ff-recom/{id}")]
        public async Task<IHttpActionResult> DeleteCOFFRecom(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await coFfRecomService.DeleteCOFFRecom(id)
            });
        }
    }
}
