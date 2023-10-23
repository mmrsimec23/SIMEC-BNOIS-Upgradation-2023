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
    [RoutePrefix(BnoisRoutePrefix.MscPermissionType)]
    [EnableCors("*", "*", "*")]
    [ActionAuthorize(Feature = MASTER_SETUP.MSC_PERMISSION_TYPE)]
    public class MscPermissionTypeController : PermissionController
    {
        private readonly IMscPermissionTypeService mscPermissionTypeService;
        public MscPermissionTypeController(IMscPermissionTypeService mscPermissionTypeService, IRoleFeatureService roleFeatureService) : base(roleFeatureService)
        {
            this.mscPermissionTypeService = mscPermissionTypeService;
        }

        [HttpGet]
        [Route("get-msc-permission-type-list")]
        public IHttpActionResult GetMscPermissionTypeList(int ps, int pn, string qs)
        {
            int total = 0;
            List<MscPermissionTypeModel> models = mscPermissionTypeService.GetMscPermissionTypes(ps, pn, qs, out total);
            RoleFeature permission = base.GetFeature(MASTER_SETUP.MSC_PERMISSION_TYPE);
            return Ok(new ResponseMessage<List<MscPermissionTypeModel>>()
            {
                Result = models,
                Total = total, Permission=permission
            });
        }

        [HttpGet]
        [Route("get-msc-permission-type")]
        public async Task<IHttpActionResult> GetMscPermissionType(int id)
        {
            MscPermissionTypeModel model = await mscPermissionTypeService.GetMscPermissionType(id);
            return Ok(new ResponseMessage<MscPermissionTypeModel>
            {
                Result = model
            });
        }


        [HttpPost]
        [ModelValidation]
        [Route("save-msc-permission-type")]
        public async Task<IHttpActionResult> SaveMscPermissionType([FromBody] MscPermissionTypeModel model)
        {
            return Ok(new ResponseMessage<MscPermissionTypeModel>
            {
                Result = await mscPermissionTypeService.SaveMscPermissionType(0, model)
            });
        }



        [HttpPut]
        [Route("update-msc-permission-type/{id}")]
        public async Task<IHttpActionResult> UpdateMscPermissionType(int id, [FromBody] MscPermissionTypeModel model)
        {
            return Ok(new ResponseMessage<MscPermissionTypeModel>
            {
                Result = await mscPermissionTypeService.SaveMscPermissionType(id, model)
            });
        }


        [HttpDelete]
        [Route("delete-msc-permission-type/{id}")]
        public async Task<IHttpActionResult> DeleteMscPermissionType(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await mscPermissionTypeService.DeleteMscPermissionType(id)
            });
        }
    }
}
