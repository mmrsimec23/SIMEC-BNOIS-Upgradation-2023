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
        List<object> GetCategoryOfficer(int rankId, string branch,int categoryId);
        List<object> GetGenderOfficer(int rankId, string branch, int categoryId, int subCategoryId, int commissionTypeId, int genderId);
        List<object> GetDashboardCategory(int categoryId);
        List<object> GetDashboardGender(int genderId);
    }
}