using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Infinity.Bnois.Api.Core;
using Infinity.Bnois.Api.Models;
using Infinity.Bnois.Api.Right;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;

namespace Infinity.Bnois.Api.Controllers
{
    [RoutePrefix(BnoisRoutePrefix.Dashboard)]
    [EnableCors("*", "*", "*")]
    [ActionAuthorize(Feature = MASTER_SETUP.DASHBOARD)]

    public class DashboardController : BaseController
    {
        private readonly IDashboardService dashboardService;
       

        public DashboardController(IDashboardService dashboardService)
        {
            this.dashboardService = dashboardService;
        }
        

        [HttpGet]
        [Route("get-dashboard-outside-navy")]
        public IHttpActionResult GetDashboardOutSideNavy(int officeId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetDashboardOutSideNavy(officeId)
            });
        }
        

        [HttpGet]
        [Route("get-dashboard-un-mission")]
        public IHttpActionResult GetDashboardUnMission(int officeId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetDashboardUnMission(officeId)
            });
        }
        

        [HttpGet]
        [Route("get-dashboard-branch")]
        public IHttpActionResult GetDashboardBranch()
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetDashboardBranch()
            });
        }

        [HttpGet]
        [Route("get-dashboard-admin-authority")]
        public IHttpActionResult GetDashboardAdminAuthority()
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetDashboardAdminAuthority()
            });
        }

        [HttpGet]
        [Route("get-dashboard-under-mission")]
        public IHttpActionResult GetDashboardUnderMission(int rankId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetDashboardUnderMission( rankId)
            });
        }

        [HttpGet]
        [Route("get-dashboard-under-course")]
        public IHttpActionResult GetDashboardUnderCourse(int rankId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetDashboardUnderCourse(rankId)
            });
        }


        [HttpGet]
        [Route("get-dashboard-inside-navy-organization")]
        public IHttpActionResult GetDashboardInsideNavyOrganization()
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetDashboardInsideNavyOrganization()
            });
        }


        [HttpGet]
        [Route("get-dashboard-bcg-organization")]
        public IHttpActionResult GetDashboardBCGOrganization()
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetDashboardBCGOrganization()
            });
        }


        [HttpGet]
        [Route("get-dashboard-leave")]
        public IHttpActionResult GetDashboardleave()
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetDashboardLeave()
            });
        }

        [HttpGet]
        [Route("get-dashboard-ex-bd-leave")]
        public IHttpActionResult GetDashboardExBDLeave()
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetDashboardExBDLeave()
            });
        }


        [HttpGet]
        [Route("get-dashboard-stream")]
        public IHttpActionResult GetDashboardStream(int streamId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetDashboardStream(streamId)
            });
        }

        [HttpGet]
        [Route("get-dashboard-category")]
        public IHttpActionResult GetDashboardCategory(int categoryId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetDashboardCategory(categoryId)
            });
        }
        [HttpGet]
        [Route("get-dashboard-gender")]
        public IHttpActionResult GetDashboardGender(int genderId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetDashboardGender(genderId)
            });
        }


        [HttpGet]
        [Route("get-dashboard-office-appointment")]
        public IHttpActionResult GetDashboardOfficeAppointment(int officeType, int rankId, int displayId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetDashboardOfficeAppointment(officeType,rankId,displayId)
            });
        }



        [HttpGet]
        [Route("get-outside-navy-officer")]
        public IHttpActionResult GetOutsideNavyOfficer(int officeId,int rankLevel, int parentId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetOutsideNavyOfficer(officeId,rankLevel, parentId)
            });
        }
        


        [HttpGet]
        [Route("get-office-search-result")]
        public IHttpActionResult GetOfficeSearchResult(int officeId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetOfficeSearchResult(officeId)
            });
        }


        [HttpGet]
        [Route("get-admin-authority-officer")]
        public IHttpActionResult GetAdminAuthorityOfficer(int officeId,int rankLevel,int type)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetAdminAuthorityOfficer(officeId,rankLevel,type)
            });
        }
        
       
        [HttpGet]
        [Route("get-leave-officer")]
        public IHttpActionResult GetLeaveOfficer(int leaveType,int rankLevel)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetLeaveOfficer(leaveType, rankLevel)
            });
        }

        [HttpGet]
        [Route("get-ex-bd-leave-officer")]
        public IHttpActionResult GetExBDLeaveOfficer( int rankLevel)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetExBDLeaveOfficer(rankLevel)
            });
        }


        [HttpGet]
        [Route("get-branch-officer")]
        public IHttpActionResult GetBranchOfficer(int rankId,string branch, int categoryId,int subCategoryId, int commissionTypeId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetBranchOfficer(rankId, branch, categoryId, subCategoryId, commissionTypeId)
            });
        }

        [HttpGet]
        [Route("get-branch-officer-by-admin-authority")]
        public IHttpActionResult GetBranchOfficerByAdminAuthority(int adminAuthorityId, int rankId,string branch, int categoryId,int subCategoryId, int commissionTypeId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetBranchOfficerByAdminAuthority(adminAuthorityId, rankId, branch, categoryId, subCategoryId, commissionTypeId)
            });
        }

        [HttpGet]
        [Route("get-category-officer")]
        public IHttpActionResult GetCategoryOfficer(int rankId,string branch, int categoryId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetCategoryOfficer(rankId, branch, categoryId)
            });
        }

        [HttpGet]
        [Route("get-gender-officer")]
        public IHttpActionResult GetGenderOfficer(int rankId, string branch, int categoryId, int subCategoryId, int commissionTypeId, int genderId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetGenderOfficer(rankId, branch, categoryId, subCategoryId, commissionTypeId, genderId)
            });
        }
        [HttpGet]
        [Route("toe-officer-by-transfer-type")]
        public IHttpActionResult GetToeOfficerByTransferType(int rankId, string branch, int categoryId, int subCategoryId, int commissionTypeId, int transferType)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetToeOfficerByTransferType(rankId, branch, categoryId, subCategoryId, commissionTypeId, transferType)
            });
        }

        [HttpGet]
        [Route("get-stream-officer")]
        public IHttpActionResult GetStreamOfficer(int rankId, string branch, int streamId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetStreamOfficer(rankId, branch, streamId)
            });
        }

        [HttpGet]
        [Route("get-overview-officer-deployment-list")]
        public IHttpActionResult GetOverviewOfficerDeploymentList(int rankId, int officerTypeId, int coastGuard, int outsideOrg)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetOverviewOfficerDeploymentList(rankId, officerTypeId, coastGuard, outsideOrg)
            });
        }

        [HttpGet]
        [Route("get-officer-by-admin-authority-with-dynamic-query")]
        public IHttpActionResult GetOfficerByAdminAuthorityWithDynamicQuery(string tableName,int branchAuthorityId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetOfficerByAdminAuthorityWithDynamicQuery(tableName,branchAuthorityId)
            });
        }
        [HttpGet]
        [Route("get-branch-authority-officer-9")]
        public IHttpActionResult GetBranchAuthorityOfficer9(int branchAuthorityId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetBranchAuthorityOfficer9(branchAuthorityId)
            });
        }
        [HttpGet]
        [Route("get-branch-authority-officer-10")]
        public IHttpActionResult GetBranchAuthorityOfficer10(int branchAuthorityId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetBranchAuthorityOfficer10(branchAuthorityId)
            });
        }
        [HttpGet]
        [Route("get-branch-authority-officer-11")]
        public IHttpActionResult GetBranchAuthorityOfficer11(int branchAuthorityId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetBranchAuthorityOfficer11(branchAuthorityId)
            });
        }
        [HttpGet]
        [Route("get-branch-authority-officer-12")]
        public IHttpActionResult GetBranchAuthorityOfficer12(int branchAuthorityId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetBranchAuthorityOfficer12(branchAuthorityId)
            });
        }
        [HttpGet]
        [Route("get-branch-authority-officer-13")]
        public IHttpActionResult GetBranchAuthorityOfficer13(int branchAuthorityId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetBranchAuthorityOfficer13(branchAuthorityId)
            });
        }
        [HttpGet]
        [Route("get-branch-authority-officer-369")]
        public IHttpActionResult GetBranchAuthorityOfficer369(int branchAuthorityId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetBranchAuthorityOfficer369(branchAuthorityId)
            });
        }
        [HttpGet]
        [Route("get-branch-authority-officer-383")]
        public IHttpActionResult GetBranchAuthorityOfficer383(int branchAuthorityId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetBranchAuthorityOfficer383(branchAuthorityId)
            });
        }
        [HttpGet]
        [Route("get-branch-authority-officer-458")]
        public IHttpActionResult GetBranchAuthorityOfficer458(int branchAuthorityId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetBranchAuthorityOfficer458(branchAuthorityId)
            });
        }
        [HttpGet]
        [Route("get-branch-authority-officer-513")]
        public IHttpActionResult GetBranchAuthorityOfficer513(int branchAuthorityId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetBranchAuthorityOfficer513(branchAuthorityId)
            });
        }
        [HttpGet]
        [Route("get-branch-authority-officer-543")]
        public IHttpActionResult GetBranchAuthorityOfficer543(int branchAuthorityId)
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetBranchAuthorityOfficer543(branchAuthorityId)
            });
        }
        [HttpGet]
        [Route("get-branch-authority-officer-600")]
        public IHttpActionResult GetBranchAuthorityOfficer600()
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.GetBranchAuthorityOfficer600()
            });
        }
        [HttpGet]
        [Route("get-bn-officer-states-950")]
        public IHttpActionResult getBNOfficerStates950()
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.getBNOfficerStates950()
            });
        }

        [HttpGet]
        [Route("get-toe-officer-state-in-navy")]
        public IHttpActionResult getToeOfficerStateInNavy()
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.getToeOfficerStateInNavy()
            });
        }

        [HttpGet]
        [Route("get-toe-officer-state-inside")]
        public IHttpActionResult getToeOfficerStateInside()
        {
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = dashboardService.getToeOfficerStateInside()
            });
        }



    }
}