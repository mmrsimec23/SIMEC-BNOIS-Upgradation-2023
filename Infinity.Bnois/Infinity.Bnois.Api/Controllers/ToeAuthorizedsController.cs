using Infinity.Bnois.Api.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.Api.Models;
using Infinity.Bnois.Api.Right;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Configuration.Models;
using Infinity.Bnois.Data;

namespace Infinity.Bnois.Api.Controllers
{
    [RoutePrefix(BnoisRoutePrefix.ToeAuthorized)]
    [EnableCors("*","*","*")]
    [ActionAuthorize(Feature = MASTER_SETUP.TOEAUTHORIZED)]
    public class ToeAuthorizedsController : PermissionController
    {
        private readonly IRankService rankService;
        private readonly IBranchService branchService;
        private readonly IOfficeService officeService;
        private readonly IToeAuthorizedService toeAuthorizedService;
        public ToeAuthorizedsController(IRankService rankService, IBranchService branchService, IToeAuthorizedService toeAuthorizedService, IOfficeService officeService, IRoleFeatureService roleFeatureService):base(roleFeatureService)
        {
            this.rankService = rankService;
            this.branchService = branchService;
            this.officeService = officeService;
            this.toeAuthorizedService = toeAuthorizedService;
        }

        [HttpGet]
        [Route("get-toe-authorizeds")]
        public IHttpActionResult GetToeAuthorizeds(int ps, int pn, string qs)
        {
            int total = 0;
            List<ToeAuthorizedModel> toeAuthorizeds = toeAuthorizedService.GetToeAuthorizeds(ps, pn, qs, out total);
            RoleFeature permission = base.GetFeature(MASTER_SETUP.TOEAUTHORIZED);
            return Ok(new ResponseMessage<List<ToeAuthorizedModel>>()
            {
                Result = toeAuthorizeds,
                Total = total, Permission=permission
            });
        }

        [HttpGet]
        [Route("get-toe-authorized")]
        public async Task<IHttpActionResult> GetToeAuthorized(int id)
        {
            ToeAuthorizedViewModel vm = new ToeAuthorizedViewModel();
            vm.ToeAuthorized = await toeAuthorizedService.GetToeAuthorized(id);
            vm.RankList = await rankService.GetConfirmRankSelectModels();
            vm.BranchList = await branchService.GetBranchSelectModels();
            vm.OfficeList = await officeService.GetAdminAuthoritySelectModel();
            return Ok(new ResponseMessage<ToeAuthorizedViewModel>
            {
                Result = vm
            });
        }

        [HttpPost]
        [ModelValidation]
        [Route("save-toe-authorized")]
        public async Task<IHttpActionResult> SaveToeAuthorized([FromBody] ToeAuthorizedModel model)
        {
            return Ok(new ResponseMessage<ToeAuthorizedModel>
            {
                Result = await toeAuthorizedService.SaveToeAuthorized(0, model)
            });
        }

        [HttpPut]
        [Route("update-toe-authorized/{id}")]
        public async Task<IHttpActionResult> UpdateToeAuthorized(int id, [FromBody] ToeAuthorizedModel model)
        {
            return Ok(new ResponseMessage<ToeAuthorizedModel>
            {
                Result = await toeAuthorizedService.SaveToeAuthorized(id, model)
            });
        }

        [HttpDelete]
        [Route("delete-toe-authorized/{id}")]
        public async Task<IHttpActionResult> DeleteToeAuthorized(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await toeAuthorizedService.DeleteToeAuthorized(id)
            });
        }

    }
}