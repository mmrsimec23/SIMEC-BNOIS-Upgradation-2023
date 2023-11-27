(function () {
    'use strict';
    angular.module('app').service('dashboardService', ['dataConstants', 'apiHttpService', dashboardService]);

    function dashboardService(dataConstants, apiHttpService) {
        var service = {
            getDashboardOutSideNavy: getDashboardOutSideNavy,
            getDashboardUnMission: getDashboardUnMission,
            getDashboardBranch: getDashboardBranch,
            getDashboardAdminAuthority: getDashboardAdminAuthority,
            getDashboardInsideNavyOrganization: getDashboardInsideNavyOrganization,
            getDashboardBCGOrganization: getDashboardBCGOrganization,
            getDashboardLeave: getDashboardLeave,
            getDashboardExBDLeave: getDashboardExBDLeave,
            getDashboardStream: getDashboardStream,
            getDashboardCategory: getDashboardCategory,
            getDashboardGender: getDashboardGender,
            getDashboardOfficeAppointment: getDashboardOfficeAppointment,
            getDashboardUnderCourse: getDashboardUnderCourse,
            getDashboardUnderMission: getDashboardUnderMission,
            getOutsideNavyOfficer: getOutsideNavyOfficer,
            getAdminAuthorityOfficer: getAdminAuthorityOfficer,
            getLeaveOfficer: getLeaveOfficer,
            getExBDLeaveOfficer: getExBDLeaveOfficer,
            getBranchOfficer: getBranchOfficer,
            getStreamOfficer: getStreamOfficer,
            getCategoryOfficer: getCategoryOfficer,
            getOfficersByOffice: getOfficersByOffice,
            getOfficerByAdminAuthorityWithDynamicQuery: getOfficerByAdminAuthorityWithDynamicQuery,
            getBranchAuthorityOfficers9: getBranchAuthorityOfficers9,
            getBranchAuthorityOfficers10: getBranchAuthorityOfficers10,
            getBranchAuthorityOfficers11: getBranchAuthorityOfficers11,
            getBranchAuthorityOfficers12: getBranchAuthorityOfficers12,
            getBranchAuthorityOfficers13: getBranchAuthorityOfficers13,
            getBranchAuthorityOfficers369: getBranchAuthorityOfficers369,
            getBranchAuthorityOfficers383: getBranchAuthorityOfficers383,
            getBranchAuthorityOfficers458: getBranchAuthorityOfficers458,
            getBranchAuthorityOfficers513: getBranchAuthorityOfficers513,
            getBranchAuthorityOfficers543: getBranchAuthorityOfficers543,
            getBranchAuthorityOfficers600: getBranchAuthorityOfficers600,
            getBNOfficerStates950: getBNOfficerStates950,
            getToeOfficerStateInNavy: getToeOfficerStateInNavy,
            getToeOfficerStateInside: getToeOfficerStateInside,
            getGenderOfficer: getGenderOfficer,
            getGenderOfficerByAuthority: getGenderOfficerByAuthority,
            getToeOfficerListByTransferType: getToeOfficerListByTransferType,
            GetOverviewOfficerDeploymentList: GetOverviewOfficerDeploymentList
           
           
        };

        return service;
     
        function getDashboardOutSideNavy(officeId) {
            var url = dataConstants.DASHBOARD_URL + 'get-dashboard-outside-navy?officeId=' + officeId;
            return apiHttpService.GET(url);
        }


        function getDashboardUnMission(officeId) {
            var url = dataConstants.DASHBOARD_URL + 'get-dashboard-un-mission?officeId=' + officeId;
            return apiHttpService.GET(url);
        }

        function getDashboardAdminAuthority() {
            var url = dataConstants.DASHBOARD_URL + 'get-dashboard-admin-authority';
            return apiHttpService.GET(url);
        }
        function getDashboardInsideNavyOrganization() {
            var url = dataConstants.DASHBOARD_URL + 'get-dashboard-inside-navy-organization';
            return apiHttpService.GET(url);
        }
         function getDashboardBCGOrganization() {
            var url = dataConstants.DASHBOARD_URL + 'get-dashboard-bcg-organization';
            return apiHttpService.GET(url);
        }

       
        function getDashboardBranch() {
            var url = dataConstants.DASHBOARD_URL + 'get-dashboard-branch';
            return apiHttpService.GET(url);
        }

        function getDashboardLeave() {
            var url = dataConstants.DASHBOARD_URL + 'get-dashboard-leave';
            return apiHttpService.GET(url);
        }

       function getDashboardExBDLeave() {
            var url = dataConstants.DASHBOARD_URL + 'get-dashboard-ex-bd-leave';
            return apiHttpService.GET(url);
        }

       
        function getDashboardStream(streamId) {
            var url = dataConstants.DASHBOARD_URL + 'get-dashboard-stream?streamId='+streamId;
            return apiHttpService.GET(url);
        }


        function getDashboardCategory(categoryId) {
            var url = dataConstants.DASHBOARD_URL + 'get-dashboard-category?categoryId=' + categoryId;
            return apiHttpService.GET(url);
        }

        function getDashboardGender(genderId) {
            var url = dataConstants.DASHBOARD_URL + 'get-dashboard-gender?genderId=' + genderId;
            return apiHttpService.GET(url);
        }


        function getDashboardOfficeAppointment(officeType, rankId, displayId) {
            var url = dataConstants.DASHBOARD_URL + 'get-dashboard-office-appointment?officeType=' + officeType + '&rankId=' + rankId + '&displayId=' + displayId;
            return apiHttpService.GET(url);
        }

        function getDashboardUnderMission(rankId) {
            var url = dataConstants.DASHBOARD_URL + 'get-dashboard-under-mission?rankId=' + rankId;
            return apiHttpService.GET(url);
        }

        function getDashboardUnderCourse(rankId) {
            var url = dataConstants.DASHBOARD_URL + 'get-dashboard-under-course?rankId=' + rankId;
            return apiHttpService.GET(url);
        }



        function getOutsideNavyOfficer(officeId, rankLevel, parentId) {
            var url = dataConstants.DASHBOARD_URL + 'get-outside-navy-officer?officeId=' + officeId + '&rankLevel=' + rankLevel + '&parentId=' + parentId;
            return apiHttpService.GET(url);
        }
        function getOfficersByOffice(officeId) {
            var url = dataConstants.DASHBOARD_URL + 'get-office-search-result?officeId=' + officeId;
            return apiHttpService.GET(url);
        }

        function getAdminAuthorityOfficer(officeId, rankLevel,type) {
            var url = dataConstants.DASHBOARD_URL + 'get-admin-authority-officer?officeId=' + officeId+'&rankLevel='+rankLevel+'&type='+type;
            return apiHttpService.GET(url);
        }


        function getLeaveOfficer(leaveType, rankLevel) {
            var url = dataConstants.DASHBOARD_URL + 'get-leave-officer?leaveType=' + leaveType+'&rankLevel='+rankLevel;
            return apiHttpService.GET(url);
        }

        function getExBDLeaveOfficer( rankLevel) {
            var url = dataConstants.DASHBOARD_URL + 'get-ex-bd-leave-officer?rankLevel='+rankLevel;
            return apiHttpService.GET(url);
        }


        function getBranchOfficer(rankId, branch, categoryId, subCategoryId, commissionTypeId) {
            var url = dataConstants.DASHBOARD_URL + 'get-branch-officer?rankId=' + rankId + '&branch=' + branch + '&categoryId=' + categoryId + '&subCategoryId=' + subCategoryId + '&commissionTypeId=' + commissionTypeId;
            return apiHttpService.GET(url);
        }


        function getStreamOfficer(rankId, branch, streamId) {
            var url = dataConstants.DASHBOARD_URL + 'get-stream-officer?rankId=' + rankId + '&branch=' + branch + '&streamId=' + streamId;
            return apiHttpService.GET(url);
        }

        function getCategoryOfficer(rankId, branch, categoryId) {
            var url = dataConstants.DASHBOARD_URL + 'get-category-officer?rankId=' + rankId + '&branch=' + branch + '&categoryId=' + categoryId;
            return apiHttpService.GET(url);
        }

        function getGenderOfficer(rankId, branch, categoryId, subCategoryId, commissionTypeId, genderId) {
            var url = dataConstants.DASHBOARD_URL + 'get-gender-officer?rankId=' + rankId + '&branch=' + branch + '&categoryId=' + categoryId + '&subCategoryId=' + subCategoryId + '&commissionTypeId=' + commissionTypeId + '&genderId=' + genderId;
            return apiHttpService.GET(url);
        }
        function getGenderOfficerByAuthority(adminAuthorityId,rankId, branch, categoryId, subCategoryId, commissionTypeId, genderId) {
            var url = dataConstants.DASHBOARD_URL + 'get-branch-officer-by-admin-authority?adminAuthorityId=' + adminAuthorityId +'&rankId=' + rankId + '&branch=' + branch + '&categoryId=' + categoryId + '&subCategoryId=' + subCategoryId + '&commissionTypeId=' + commissionTypeId + '&genderId=' + genderId;
            return apiHttpService.GET(url);
        }
        function getToeOfficerListByTransferType(rankId, branch, categoryId, subCategoryId, commissionTypeId, transferType) {
            var url = dataConstants.DASHBOARD_URL + 'toe-officer-by-transfer-type?rankId=' + rankId + '&branch=' + branch + '&categoryId=' + categoryId + '&subCategoryId=' + subCategoryId + '&commissionTypeId=' + commissionTypeId + '&transferType=' + transferType;
            return apiHttpService.GET(url);
        }
        function GetOverviewOfficerDeploymentList(rankId, officerTypeId, coastGuard, outsideOrg) {
            var url = dataConstants.DASHBOARD_URL + 'get-overview-officer-deployment-list?rankId=' + rankId + '&officerTypeId=' + officerTypeId + '&coastGuard=' + coastGuard + '&outsideOrg=' + outsideOrg;
            return apiHttpService.GET(url);
        }
        function getOfficerByAdminAuthorityWithDynamicQuery(tableName,branchAuthorityId) {
            var url = dataConstants.DASHBOARD_URL + 'get-officer-by-admin-authority-with-dynamic-query?tableName=' + tableName + '&branchAuthorityId=' + branchAuthorityId;
            return apiHttpService.GET(url);
        }
        function getBranchAuthorityOfficers9(branchAuthorityId) {
            var url = dataConstants.DASHBOARD_URL + 'get-branch-authority-officer-9?branchAuthorityId=' + branchAuthorityId;
            return apiHttpService.GET(url);
        }
        function getBranchAuthorityOfficers10(branchAuthorityId) {
            var url = dataConstants.DASHBOARD_URL + 'get-branch-authority-officer-10?branchAuthorityId=' + branchAuthorityId;
            return apiHttpService.GET(url);
        }
        function getBranchAuthorityOfficers11(branchAuthorityId) {
            var url = dataConstants.DASHBOARD_URL + 'get-branch-authority-officer-11?branchAuthorityId=' + branchAuthorityId;
            return apiHttpService.GET(url);
        }
        function getBranchAuthorityOfficers12(branchAuthorityId) {
            var url = dataConstants.DASHBOARD_URL + 'get-branch-authority-officer-12?branchAuthorityId=' + branchAuthorityId;
            return apiHttpService.GET(url);
        }
        function getBranchAuthorityOfficers13(branchAuthorityId) {
            var url = dataConstants.DASHBOARD_URL + 'get-branch-authority-officer-13?branchAuthorityId=' + branchAuthorityId;
            return apiHttpService.GET(url);
        }
        function getBranchAuthorityOfficers369(branchAuthorityId) {
            var url = dataConstants.DASHBOARD_URL + 'get-branch-authority-officer-369?branchAuthorityId=' + branchAuthorityId;
            return apiHttpService.GET(url);
        }
        function getBranchAuthorityOfficers383(branchAuthorityId) {
            var url = dataConstants.DASHBOARD_URL + 'get-branch-authority-officer-383?branchAuthorityId=' + branchAuthorityId;
            return apiHttpService.GET(url);
        }
        function getBranchAuthorityOfficers458(branchAuthorityId) {
            var url = dataConstants.DASHBOARD_URL + 'get-branch-authority-officer-458?branchAuthorityId=' + branchAuthorityId;
            return apiHttpService.GET(url);
        }
        function getBranchAuthorityOfficers513(branchAuthorityId) {
            var url = dataConstants.DASHBOARD_URL + 'get-branch-authority-officer-513?branchAuthorityId=' + branchAuthorityId;
            return apiHttpService.GET(url);
        }
        function getBranchAuthorityOfficers543(branchAuthorityId) {
            var url = dataConstants.DASHBOARD_URL + 'get-branch-authority-officer-543?branchAuthorityId=' + branchAuthorityId;
            return apiHttpService.GET(url);
        }
        function getBranchAuthorityOfficers600() {
            var url = dataConstants.DASHBOARD_URL + 'get-branch-authority-officer-600';
            return apiHttpService.GET(url);
        }
        function getBNOfficerStates950() {
            var url = dataConstants.DASHBOARD_URL + 'get-bn-officer-states-950';
            return apiHttpService.GET(url);
        }
        function getToeOfficerStateInNavy() {
            var url = dataConstants.DASHBOARD_URL + 'get-toe-officer-state-in-navy';
            return apiHttpService.GET(url);
        }
        function getToeOfficerStateInside() {
            var url = dataConstants.DASHBOARD_URL + 'get-toe-officer-state-inside';
            return apiHttpService.GET(url);
        }
       

    }
})();