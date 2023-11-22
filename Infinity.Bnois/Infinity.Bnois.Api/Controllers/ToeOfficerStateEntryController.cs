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
    [RoutePrefix(BnoisRoutePrefix.ToeOfficerStateEntry)]
    [EnableCors("*","*","*")]
    [ActionAuthorize(Feature = MASTER_SETUP.TOEOSENTRY)]
    public class ToeOfficerStateEntryController : PermissionController
    {
        private readonly IRankService rankService;
        private readonly IBranchService branchService;
        private readonly IToeOfficerStateEntryService toeOfficerStateEntryService;
        public ToeOfficerStateEntryController(IRankService rankService, IBranchService branchService, IToeOfficerStateEntryService toeOfficerStateEntryService, IRoleFeatureService roleFeatureService) : base(roleFeatureService)
        {
            this.rankService = rankService;
            this.branchService = branchService;
            this.toeOfficerStateEntryService = toeOfficerStateEntryService;
        }

        [HttpGet]
        [Route("get-toe-officer-state-entry-list")]
        public IHttpActionResult GetToeOfficerStateEntryList(int ps, int pn, string qs)
        {
            int total = 0;
            List<DashBoardBranchByAdminAuthority700Model> oodeList = toeOfficerStateEntryService.GetToeOfficerStateEntryList(ps, pn, qs, out total);
            RoleFeature permission = base.GetFeature(MASTER_SETUP.TOEOSENTRY);
            return Ok(new ResponseMessage<List<DashBoardBranchByAdminAuthority700Model>>()
            {
                Result = oodeList,
                Total = total,
                Permission = permission
            });
        }

        [HttpGet]
        [Route("get-toe-officer-state-entry")]
        public async Task<IHttpActionResult> GetToeOfficerStateEntry(int id)
        {
            DashBoardBranchByAdminAuthority700ViewModel vm = new DashBoardBranchByAdminAuthority700ViewModel();
            vm.DashBoardBranchByAdminAuthority700 = await toeOfficerStateEntryService.GetToeOfficerStateEntry(id);
            vm.RankList = await rankService.GetConfirmRankSelectModels();
            vm.BranchList = await branchService.GetBranchSelectModels();
            return Ok(new ResponseMessage<DashBoardBranchByAdminAuthority700ViewModel>
            {
                Result = vm
            });
        }

        [HttpPost]
        [ModelValidation]
        [Route("save-toe-officer-state-entry")]
        public async Task<IHttpActionResult> SaveToeOfficerStateEntry([FromBody] DashBoardBranchByAdminAuthority700Model model)
        {
            return Ok(new ResponseMessage<DashBoardBranchByAdminAuthority700Model>
            {
                Result = await toeOfficerStateEntryService.SaveToeOfficerStateEntry(0, model)
            });
        }

        [HttpPut]
        [Route("update-toe-officer-state-entry/{id}")]
        public async Task<IHttpActionResult> UpdateToeOfficerStateEntry(int id, [FromBody] DashBoardBranchByAdminAuthority700Model model)
        {
            return Ok(new ResponseMessage<DashBoardBranchByAdminAuthority700Model>
            {
                Result = await toeOfficerStateEntryService.SaveToeOfficerStateEntry(id, model)
            });
        }

        [HttpDelete]
        [Route("delete-toe-officer-state-entry/{id}")]
        public async Task<IHttpActionResult> DeleteToeOfficerStateEntry(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await toeOfficerStateEntryService.DeleteToeOfficerStateEntry(id)
            });
        }

    }
}