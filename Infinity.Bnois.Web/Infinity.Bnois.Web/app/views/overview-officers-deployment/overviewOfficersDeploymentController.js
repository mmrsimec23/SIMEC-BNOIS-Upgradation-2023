

(function () {

    'use strict';

    var controllerId = 'overviewOfficersDeploymentController';

    angular.module('app').controller(controllerId, overviewOfficersDeploymentController);
    overviewOfficersDeploymentController.$inject = ['$stateParams','dashboardService', 'notificationService','currentStatusService', '$state'];

    function overviewOfficersDeploymentController($stateParams, dashboardService, notificationService, currentStatusService, $state) {
        var vm = this;
        vm.pNo = 0;
        vm.overviewOfficersDeploymentList = [];
        vm.printSection = printSection;
        vm.getOverviewOfficerDeploymentList = getOverviewOfficerDeploymentList;

        if ($stateParams.pno !== undefined && $stateParams.pno !== null) {
            vm.pNo = $stateParams.pno;
        }

        Init();
        function Init() {
            
            
            dashboardService.getBranchAuthorityOfficers600().then(function (data) {
                vm.overviewOfficersDeploymentList = data.result;

                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
            
        };
        function getOverviewOfficerDeploymentList(rankId, officerTypeId, coastGuard, outsideOrg) {
            $state.goNewTab('overview-deployment-officer-list', { rankId: rankId, officerTypeId: officerTypeId, coastGuard: coastGuard, outsideOrg: outsideOrg });

        }

        function printSection(printSectionId) {
            var innerContents = document.getElementById(printSectionId).innerHTML;
            var popupWindow = window.open('', '', 'width=1000,height=700,left=0,top=0,toolbar=0,status=0');
            popupWindow.document.open();
            popupWindow.document.write('<HTML>\n<HEAD>\n');
            popupWindow.document.write('<link rel="stylesheet" type="text/css" href="../../../Content/addmin/PdfReport.css"/>');
            popupWindow.document.write('</HEAD>\n');
            popupWindow.document.write('<BODY  onload="window.print()">\n');
            popupWindow.document.write(innerContents);
            popupWindow.document.write('</BODY>\n');
            popupWindow.document.write('</HTML>\n');
            popupWindow.document.close();
            popupWindow.focus();
        }
       
    }
})();
