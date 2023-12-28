﻿(function () {
    'use strict';
    angular.module('app').service('currentStatusService', ['dataConstants', 'apiHttpService', currentStatusService]);

    function currentStatusService(dataConstants, apiHttpService) {
        var service = {
            getGeneralInformation: getGeneralInformation,
            getCivilAcademicQualification: getCivilAcademicQualification,
            getSecurityClearance: getSecurityClearance,
            getCourseAttended: getCourseAttended,
            getForeignCourseAttended: getForeignCourseAttended,
            getExamTestResult: getExamTestResult,
            getCareerForecast: getCareerForecast,
            getCarLoanInfo: getCarLoanInfo,
            getPunishmentDiscipline: getPunishmentDiscipline,
            getCommendationAppreciation: getCommendationAppreciation,
            getAward: getAward,
            getMedal: getMedal,
            getCleanService: getCleanService,
            getPublication: getPublication,
            getChildren: getChildren,
            getSibling: getSibling,
            getNextOfKin: getNextOfKin,
            getHeir: getHeir,
            getOprGrading: getOprGrading,
            getForeignVisit: getForeignVisit,
            getForeignCourseVisitGrandTotal: getForeignCourseVisitGrandTotal,
            getParentInfo: getParentInfo,
            getSpouseInfo: getSpouseInfo,
            getTransferHistory: getTransferHistory,
            getCostGuardHistory: getCostGuardHistory,
            getPromotionHistory: getPromotionHistory,
            getAdditionalSeaServices: getAdditionalSeaServices,
            getSeaServices: getSeaServices,
            getInstructionalServices: getInstructionalServices,
            getSeaCommandServices: getSeaCommandServices,
            getInterOrganizationServices: getInterOrganizationServices,
            getIntelligenceServices: getIntelligenceServices,
            getCurrentStatus: getCurrentStatus,
            getNotifications: getNotifications,
            getRemark: getRemark,
            getPersuasion: getPersuasion,
            getCourseFuturePlan: getCourseFuturePlan,
            getTransferFuturePlan: getTransferFuturePlan,
            getTemporaryTransferHistory: getTemporaryTransferHistory,
            getLeaveInfo: getLeaveInfo,
            getAdminAuthorityService: getAdminAuthorityService,
            getISSB: getISSB,
            getForeignProjects: getForeignProjects,
            getMissions: getMissions,
            getHODServices: getHODServices,
            getDockyardServices: getDockyardServices,
            getShoreCommandServices: getShoreCommandServices,
            getSubmarineServices: getSubmarineServices,
            getDeputationServices: getDeputationServices,
            getOutsideServices: getOutsideServices,
            GetFamilyPermissionRelationCount: GetFamilyPermissionRelationCount,
            GetFamilyPermissions: GetFamilyPermissions,
            getZoneServices: getZoneServices,
            getUnmDeferment : getUnmDeferment,
            getZoneCourseMissionServices: getZoneCourseMissionServices,
            getMscEducationQualification: getMscEducationQualification,
            getBatchPosition: getBatchPosition
           
        };

        return service;
     
        function getGeneralInformation(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-general-information?pNo=' + pNo;
            return apiHttpService.GET(url);
        }
        function getCivilAcademicQualification(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-civil-academic-qualification?pNo=' + pNo;
            return apiHttpService.GET(url);
        }
        function getMscEducationQualification(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-msc-education-qualification?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getSecurityClearance(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-security-clearance?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getCourseAttended(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-course-attended?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getUnmDeferment(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-unm-deferment?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getForeignCourseAttended(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-foreign-course-attended?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getExamTestResult(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-exam-test-result?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getCareerForecast(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-career-forecast?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getCarLoanInfo(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-car-loan-info?pNo=' + pNo;
            return apiHttpService.GET(url);
        }
        

        function getBatchPosition(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-batch-position?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getPunishmentDiscipline(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-punishment-discipline?pNo=' + pNo;
            return apiHttpService.GET(url);
        }
        function getCommendationAppreciation(pNo,type) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-commendation-appreciation?pNo=' + pNo+'&type='+type;
            return apiHttpService.GET(url);
        }
        function getAward(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-award?pNo=' + pNo;
            return apiHttpService.GET(url);
        }
        function getMedal(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-medal?pNo=' + pNo;
            return apiHttpService.GET(url);
        }
        function getPublication(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-publication?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getCleanService(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-clean-service?pNo=' + pNo;
            return apiHttpService.GET(url);
        }


        function getChildren(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-current-children?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getSibling(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-current-sibling?pNo=' + pNo;
            return apiHttpService.GET(url);
        }
        function getNextOfKin(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-current-next-of-kin?pNo=' + pNo;
            return apiHttpService.GET(url);
        }
        function getHeir(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-heir-info?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getOprGrading(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-opr-grading?pNo=' + pNo;
            return apiHttpService.GET(url);
        }
        function getForeignVisit(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-foreign-visit?pNo=' + pNo;
            return apiHttpService.GET(url);
        }
        function getForeignCourseVisitGrandTotal(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-foreign-course-visit-grand-total?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getParentInfo(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-parent-info?pNo=' + pNo;
            return apiHttpService.GET(url);
        }
        function getSpouseInfo(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-spouse-info?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getTransferHistory(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-transfer-history?pNo=' + pNo;
            return apiHttpService.GET(url);
        }
        function getCostGuardHistory(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-costguard-history?pNo=' + pNo;
            return apiHttpService.GET(url);
        }
        function getTemporaryTransferHistory(transferId) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-temporary-transfer-history?transferId=' + transferId;
            return apiHttpService.GET(url);
        }

        function getPromotionHistory(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-promotion-history?pNo=' + pNo;
            return apiHttpService.GET(url);
        }
        function getAdditionalSeaServices(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-additional-sea-services?pNo=' + pNo;
            return apiHttpService.GET(url);
        }
        function getSeaServices(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-sea-services?pNo=' + pNo;
            return apiHttpService.GET(url);
        }


        function getZoneServices(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-zone-services?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getZoneCourseMissionServices(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-zone-course-mission-services?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getInstructionalServices(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-instructional-services?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getSeaCommandServices(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-sea-command-services?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getShoreCommandServices(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-shore-command-services?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getInterOrganizationServices(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-inter-organization-services?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getIntelligenceServices(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-intelligence-services?pNo=' + pNo;
            return apiHttpService.GET(url);
        }


        function getHODServices(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-hod-services?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getDockyardServices(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-dockyard-services?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getSubmarineServices(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-submarine-services?pNo=' + pNo;
            return apiHttpService.GET(url);
        }
        

        function getDeputationServices(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-deputation-services?pNo=' + pNo;
            return apiHttpService.GET(url);
        }
        

        function getOutsideServices(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-outside-services?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        

        function GetFamilyPermissionRelationCount(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-family-permission-relation-count?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        

        function GetFamilyPermissions(pNo, relationId) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-family-permissions?pNo=' + pNo + '&relationId=' + relationId;
            return apiHttpService.GET(url);
        }



        function getForeignProjects(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-foreign-projects?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getMissions(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-missions?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getRemark(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-remark?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getPersuasion(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-persuasion?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getCourseFuturePlan(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-course-future-plan?pNo=' + pNo;
            return apiHttpService.GET(url);
        }
        function getTransferFuturePlan(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-transfer-future-plan?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getNotifications(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-notifications?pNo=' + pNo;
            return apiHttpService.GET(url);
        }


        function getCurrentStatus(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-current-status?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getISSB(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-issb?pNo=' + pNo;
            return apiHttpService.GET(url);
        }



        function getLeaveInfo(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-leave-info?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

        function getAdminAuthorityService(pNo) {
            var url = dataConstants.CURRENT_STATUS_URL + 'get-admin-authority-service?pNo=' + pNo;
            return apiHttpService.GET(url);
        }

    }
})();