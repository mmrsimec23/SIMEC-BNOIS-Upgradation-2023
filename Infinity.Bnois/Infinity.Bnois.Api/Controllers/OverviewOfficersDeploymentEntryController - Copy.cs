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
    [RoutePrefix(BnoisRoutePrefix.OverviewOfficersDeploymentEntry)]
    [EnableCors("*","*","*")]
    [ActionAuthorize(Feature = MASTER_SETUP.OODENTRY)]
    public class OverviewOfficersDeploymentEntryController : PermissionController
    {
        private readonly IRankService rankService;
        private readonly IOverviewOfficersDeploymentEntryService overviewOfficersDeploymentEntryService;
        public OverviewOfficersDeploymentEntryController(IRankService rankService, IOverviewOfficersDeploymentEntryService overviewOfficersDeploymentEntryService, IRoleFeatureService roleFeatureService) : base(roleFeatureService)
        {
            this.rankService = rankService;
            this.overviewOfficersDeploymentEntryService = overviewOfficersDeploymentEntryService;
        }

        [HttpGet]
        [Route("get-overview-officers-deployment-entry-list")]
        public IHttpActionResult GetOverviewOfficersDeploymentEntry(int ps, int pn, string qs)
        {
            int total = 0;
            List<DashBoardBranchByAdminAuthority600EntryModel> oodeList = overviewOfficersDeploymentEntryService.GetOverviewOfficersDeploymentEntryList(ps, pn, qs, out total);
            RoleFeature permission = base.GetFeature(MASTER_SETUP.OODENTRY);
            return Ok(new ResponseMessage<List<DashBoardBranchByAdminAuthority600EntryModel>>()
            {
                Result = oodeList,
                Total = total,
                Permission = permission
            });
        }

        [HttpGet]
        [Route("get-overview-officers-deployment-entry")]
        public async Task<IHttpActionResult> GetOverviewOfficersDeployment600Entry(int id)
        {
            DashBoardBranchByAdminAuthority600EntryViewModel vm = new DashBoardBranchByAdminAuthority600EntryViewModel();
            vm.DashBoardBranchByAdminAuthority600Entry = await overviewOfficersDeploymentEntryService.GetOverviewOfficersDeploymentEntry(id);
            vm.RankList = await rankService.GetConfirmRankSelectModels();
            vm.OrgTypeList = overviewOfficersDeploymentEntryService.GetOrgTypeSelectModels();
            return Ok(new ResponseMessage<DashBoardBranchByAdminAuthority600EntryViewModel>
            {
                Result = vm
            });
        }

        [HttpPost]
        [ModelValidation]
        [Route("save-overview-officers-deployment-entry")]
        public async Task<IHttpActionResult> SaveOverviewOfficersDeployment600Entry([FromBody] DashBoardBranchByAdminAuthority600EntryModel model)
        {
            return Ok(new ResponseMessage<DashBoardBranchByAdminAuthority600EntryModel>
            {
                Result = await overviewOfficersDeploymentEntryService.SaveOverviewOfficersDeploymentEntry(0, model)
            });
        }

        [HttpPut]
        [Route("update-overview-officers-deployment-entry/{id}")]
        public async Task<IHttpActionResult> UpdateOverviewOfficersDeployment600Entry(int id, [FromBody] DashBoardBranchByAdminAuthority600EntryModel model)
        {
            return Ok(new ResponseMessage<DashBoardBranchByAdminAuthority600EntryModel>
            {
                Result = await overviewOfficersDeploymentEntryService.SaveOverviewOfficersDeploymentEntry(id, model)
            });
        }

        [HttpDelete]
        [Route("delete-overview-officers-deployment-entry/{id}")]
        public async Task<IHttpActionResult> DeleteOverviewOfficersDeployment600Entry(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await overviewOfficersDeploymentEntryService.DeleteOverviewOfficersDeploymentEntry(id)
            });
        }

    }
}