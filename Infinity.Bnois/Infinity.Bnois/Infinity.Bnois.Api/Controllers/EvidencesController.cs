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
using Infinity.Bnois.ApplicationService.Implementation;

namespace Infinity.Bnois.Api.Controllers
{
    [RoutePrefix(BnoisRoutePrefix.Evidences)]
    [EnableCors("*", "*", "*")]
    [ActionAuthorize(Feature = MASTER_SETUP.EVIDENCES)]

    public class EvidencesController : PermissionController
    {
        private readonly IEvidenceService evidenceService;
        public EvidencesController(IEvidenceService evidenceService, IRoleFeatureService roleFeatureService) : base(roleFeatureService)
        {
            this.evidenceService = evidenceService;
        }

        [HttpGet]
        [Route("get-evidences")]
        public IHttpActionResult GetEvidences(int ps, int pn, string qs)
        {
            int total = 0;
            List<EvidenceModel> models = evidenceService.GetEvidences(ps, pn, qs, out total);
            RoleFeature permission = base.GetFeature(MASTER_SETUP.BLOOD_GROUPS);
            return Ok(new ResponseMessage<List<EvidenceModel>>()
            {
                Result = models,
                Total = total, Permission=permission
            });
        }


        [HttpGet]
        [Route("get-evidence")]
        public async Task<IHttpActionResult> GetEvidence(int id)
        {
            EvidenceModel model = await evidenceService.GetEvidence(id);
            return Ok(new ResponseMessage<EvidenceModel>
            {
                Result = model
            });
        }

        [HttpPost]
        [ModelValidation]
        [Route("save-evidence")]
        public async Task<IHttpActionResult> SaveEvidence([FromBody] EvidenceModel model)
        {
            return Ok(new ResponseMessage<EvidenceModel>
            {
                Result = await evidenceService.SaveEvidence(0, model)
            });
        }

        [HttpPut]
        [Route("update-evidence/{id}")]
        public async Task<IHttpActionResult> UpdateEvidence(int id, [FromBody] EvidenceModel model)
        {
            return Ok(new ResponseMessage<EvidenceModel>
            {
                Result = await evidenceService.SaveEvidence(id, model)
            });
        }

        [HttpDelete]
        [Route("delete-evidence/{id}")]
        public async Task<IHttpActionResult> DeleteEvidence(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await evidenceService.DeleteEvidence(id)
            });
        }
    }
}
