

(function () {

    'use strict';

    var controllerId = 'toeOfficerStateController';

    angular.module('app').controller(controllerId, toeOfficerStateController);
    toeOfficerStateController.$inject = ['$stateParams','dashboardService', 'notificationService','currentStatusService', '$state'];

    function toeOfficerStateController($stateParams, dashboardService, notificationService, currentStatusService, $state) {
        var vm = this;
        vm.pNo = 0;
        vm.officerStateInside = [];
        vm.officerStateInNavy = [];
        vm.printSection = printSection;
        vm.getOfficerList = getOfficerList;
        vm.getToeOfficerListByTransferType = getToeOfficerListByTransferType;

        if ($stateParams.pno !== undefined && $stateParams.pno !== null) {
            vm.pNo = $stateParams.pno;
        }

        Init();
        function Init() {
            
            
            dashboardService.getToeOfficerStateInside().then(function (data) {
                vm.officerStateInside = data.result;
                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });

            dashboardService.getToeOfficerStateInNavy().then(function (data) {
                vm.officerStateInNavy = data.result;
                console.log(vm.officerStateInNavy)
                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
            
        };
        function getOfficerList(rankId, branch, categoryId, subCategoryId, commissionTypeId) {
            $state.goNewTab('branch-officer', { rankId: rankId, branch: branch, categoryId: categoryId, subCategoryId: subCategoryId, commissionTypeId: commissionTypeId });

        }

        function getToeOfficerListByTransferType(rankId, branch, categoryId, subCategoryId, commissionTypeId, transferType) {
            $state.goNewTab('toe-officer-by-transfer-type', { rankId: rankId, branch: branch, categoryId: categoryId, subCategoryId: subCategoryId, commissionTypeId: commissionTypeId, transferType: transferType });

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
