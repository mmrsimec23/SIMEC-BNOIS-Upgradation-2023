

(function () {

    'use strict';

    var controllerId = 'bnOfficerStatesController';

    angular.module('app').controller(controllerId, bnOfficerStatesController);
    bnOfficerStatesController.$inject = ['$stateParams','dashboardService', 'notificationService','currentStatusService', '$state'];

    function bnOfficerStatesController($stateParams, dashboardService, notificationService, currentStatusService, $state) {
        var vm = this;
        vm.pNo = 0;
        vm.bnOfficerStatesList = [];
        vm.printSection = printSection;
        vm.getOverviewOfficerDeploymentList = getOverviewOfficerDeploymentList;

        if ($stateParams.pno !== undefined && $stateParams.pno !== null) {
            vm.pNo = $stateParams.pno;
        }

        Init();
        function Init() {
            
            
            dashboardService.getBNOfficerStates950().then(function (data) {
                vm.bnOfficerStatesList = data.result;

                console.log(vm.bnOfficerStatesList)
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
