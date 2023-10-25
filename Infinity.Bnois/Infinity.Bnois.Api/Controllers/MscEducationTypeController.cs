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
    [RoutePrefix(BnoisRoutePrefix.MscEducationType)]
    [EnableCors("*", "*", "*")]
    [ActionAuthorize(Feature = MASTER_SETUP.MSC_EDUCATION_TYPE)]
    public class MscEducationTypeController : PermissionController
    {
        private readonly IMscEducationTypeService mscEducationTypeService;
        public MscEducationTypeController(IMscEducationTypeService mscEducationTypeService, IRoleFeatureService roleFeatureService) : base(roleFeatureService)
        {
            this.mscEducationTypeService = mscEducationTypeService;
        }

        [HttpGet]
        [Route("get-msc-education-type-list")]
        public IHttpActionResult GetMscEducationTypeList(int ps, int pn, string qs)
        {
            int total = 0;
            List<MscEducationTypeModel> models = mscEducationTypeService.GetMscEducationTypes(ps, pn, qs, out total);
            RoleFeature permission = base.GetFeature(MASTER_SETUP.MSC_EDUCATION_TYPE);
            return Ok(new ResponseMessage<List<MscEducationTypeModel>>()
            {
                Result = models,
                Total = total, Permission=permission
            });
        }

        [HttpGet]
        [Route("get-msc-education-type")]
        public async Task<IHttpActionResult> GetMscEducationType(int id)
        {
            MscEducationTypeModel model = await mscEducationTypeService.GetMscEducationType(id);
            return Ok(new ResponseMessage<MscEducationTypeModel>
            {
                Result = model
            });
        }


        [HttpPost]
        [ModelValidation]
        [Route("save-msc-education-type")]
        public async Task<IHttpActionResult> SaveMscEducationType([FromBody] MscEducationTypeModel model)
        {
            return Ok(new ResponseMessage<MscEducationTypeModel>
            {
                Result = await mscEducationTypeService.SaveMscEducationType(0, model)
            });
        }



        [HttpPut]
        [Route("update-msc-education-type/{id}")]
        public async Task<IHttpActionResult> UpdateMscEducationType(int id, [FromBody] MscEducationTypeModel model)
        {
            return Ok(new ResponseMessage<MscEducationTypeModel>
            {
                Result = await mscEducationTypeService.SaveMscEducationType(id, model)
            });
        }


        [HttpDelete]
        [Route("delete-msc-education-type/{id}")]
        public async Task<IHttpActionResult> DeleteMscEducationType(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await mscEducationTypeService.DeleteMscEducationType(id)
            });
        }
    }
}
