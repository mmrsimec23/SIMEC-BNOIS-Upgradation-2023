using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IDashboardService
    {
       
        List<Object> GetDashboardOutSideNavy(int officeId);
        List<Object> GetDashboardUnMission(int officeId);
        List<object> GetDashboardBranch();
        List<object> GetDashboardAdminAuthority();
        List<object> GetDashboardUnderMission(int rankId);
        List<object> GetDashboardUnderCourse(int rankId);
        List<object> GetDashboardInsideNavyOrganization();
        List<object> GetDashboardBCGOrganization();
        List<object> GetDashboardLeave();
        List<object> GetDashboardExBDLeave();
        List<object> GetDashboardStream(int streamId);
        List<object> GetDashboardOfficeAppointment(int officeType, int rankId, int displayId);
        List<object> GetOutsideNavyOfficer(int officeId, int rankLevel, int parentId);
        List<object> GetOfficeSearchResult(int officeId);
        List<object> GetAdminAuthorityOfficer(int officeId, int rankLevel, int type);
        List<object> GetLeaveOfficer(int leaveType, int rankLevel);
        List<object> GetExBDLeaveOfficer( int rankLevel);
        List<object> GetBranchOfficer(int rankId, string branch, int categoryId, int subCategoryId, int commissionTypeId);
        List<object> GetStreamOfficer(int rankId, string branch,int streamId);
        List<object> GetOverviewOfficerDeploymentList(int rankId, int officerTypeId, int coastGuard, int outsideOrg);
        List<object> GetCategoryOfficer(int rankId, string branch,int categoryId);
        List<object> GetGenderOfficer(int rankId, string branch, int categoryId, int subCategoryId, int commissionTypeId, int genderId);
        List<object> GetToeOfficerByTransferType(int rankId, string branch, int categoryId, int subCategoryId, int commissionTypeId, int transferTpye);
        List<object> GetDashboardCategory(int categoryId);
        List<object> GetDashboardGender(int genderId);
        List<object> GetBranchAuthorityOfficer9(int branchAuthorityId);
        List<object> GetBranchAuthorityOfficer10(int branchAuthorityId);
        List<object> GetBranchAuthorityOfficer11(int branchAuthorityId);
        List<object> GetBranchAuthorityOfficer12(int branchAuthorityId);
        List<object> GetBranchAuthorityOfficer13(int branchAuthorityId);
        List<object> GetBranchAuthorityOfficer369(int branchAuthorityId);
        List<object> GetBranchAuthorityOfficer383(int branchAuthorityId);
        List<object> GetBranchAuthorityOfficer458(int branchAuthorityId);
        List<object> GetBranchAuthorityOfficer513(int branchAuthorityId);
        List<object> GetBranchAuthorityOfficer543(int branchAuthorityId);
        List<object> GetBranchAuthorityOfficer600();
        List<object> getToeOfficerStateInNavy();
        List<object> getToeOfficerStateInside();
    }
}