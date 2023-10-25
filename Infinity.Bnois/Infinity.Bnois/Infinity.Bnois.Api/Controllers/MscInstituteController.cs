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
    [RoutePrefix(BnoisRoutePrefix.MscInstitute)]
    [EnableCors("*", "*", "*")]
    [ActionAuthorize(Feature = MASTER_SETUP.MSC_INSTITUTE)]
    public class MscInstituteController : PermissionController
    {
        private readonly IMscInstituteService mscInstituteService;
        public MscInstituteController(IMscInstituteService mscInstituteService, IRoleFeatureService roleFeatureService) : base(roleFeatureService)
        {
            this.mscInstituteService = mscInstituteService;
        }

        [HttpGet]
        [Route("get-msc-institute-list")]
        public IHttpActionResult GetMscInstituteList(int ps, int pn, string qs)
        {
            int total = 0;
            List<MscInstituteModel> models = mscInstituteService.GetMscInstitutes(ps, pn, qs, out total);
            RoleFeature permission = base.GetFeature(MASTER_SETUP.MSC_INSTITUTE);
            return Ok(new ResponseMessage<List<MscInstituteModel>>()
            {
                Result = models,
                Total = total, Permission=permission
            });
        }

        [HttpGet]
        [Route("get-msc-institute")]
        public async Task<IHttpActionResult> GetMscInstitute(int id)
        {
            MscInstituteModel model = await mscInstituteService.GetMscInstitute(id);
            return Ok(new ResponseMessage<MscInstituteModel>
            {
                Result = model
            });
        }


        [HttpPost]
        [ModelValidation]
        [Route("save-msc-institute")]
        public async Task<IHttpActionResult> SaveMscInstitute([FromBody] MscInstituteModel model)
        {
            return Ok(new ResponseMessage<MscInstituteModel>
            {
                Result = await mscInstituteService.SaveMscInstitute(0, model)
            });
        }



        [HttpPut]
        [Route("update-msc-institute/{id}")]
        public async Task<IHttpActionResult> UpdateMscInstitute(int id, [FromBody] MscInstituteModel model)
        {
            return Ok(new ResponseMessage<MscInstituteModel>
            {
                Result = await mscInstituteService.SaveMscInstitute(id, model)
            });
        }


        [HttpDelete]
        [Route("delete-msc-institute/{id}")]
        public async Task<IHttpActionResult> DeleteMscInstitute(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await mscInstituteService.DeleteMscInstitute(id)
            });
        }
    }
}
