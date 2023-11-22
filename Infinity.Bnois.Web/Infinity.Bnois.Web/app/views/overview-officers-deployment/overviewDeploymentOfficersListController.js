﻿

(function () {

    'use strict';

    var controllerId = 'overviewDeploymentOfficersListController'; 

    angular.module('app').controller(controllerId, overviewDeploymentOfficersListController);
    overviewDeploymentOfficersListController.$inject = ['$stateParams', '$rootScope', 'dashboardService', 'downloadService','reportService', 'notificationService', '$state'];

    function overviewDeploymentOfficersListController($stateParams, $rootScope, dashboardService, downloadService, reportService ,notificationService, $state) {
        var vm = this;
        vm.officerDetails = officerDetails;
        vm.overviewOfficersDeploymentLists = [];
        vm.rankId = 0;
        vm.officerTypeId = 0;
        vm.coastGuard = 0;
        vm.outsideOrg = 0;
        vm.printSection = printSection;

        if ($stateParams.rankId !== undefined && $stateParams.rankId !== null) {
            vm.rankId = $stateParams.rankId;

        }

        if ($stateParams.officerTypeId !== undefined && $stateParams.officerTypeId !== null) {
            vm.officerTypeId = $stateParams.officerTypeId;

        }

        if ($stateParams.coastGuard !== undefined && $stateParams.coastGuard !== null) {
            vm.coastGuard = $stateParams.coastGuard;

        }

        if ($stateParams.outsideOrg !== undefined && $stateParams.outsideOrg !== null) {
            vm.outsideOrg = $stateParams.outsideOrg;

        }
        


        Init();
        function Init() {
            dashboardService.GetOverviewOfficerDeploymentList(vm.rankId, vm.officerTypeId, vm.coastGuard, vm.outsideOrg).then(function (data) {

                vm.overviewOfficersDeploymentLists = data.result;
                vm.totalResult = vm.overviewOfficersDeploymentLists.length;

                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
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
      

        function officerDetails(pNo) {
            $state.goNewTab('current-status-tab', { pno: pNo });

        }

    }
})();
