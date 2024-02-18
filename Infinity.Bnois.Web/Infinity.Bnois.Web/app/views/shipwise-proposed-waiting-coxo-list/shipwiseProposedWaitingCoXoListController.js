

(function () {

    'use strict';

    var controllerId = 'shipwiseProposedWaitingCoXoListController';

    angular.module('app').controller(controllerId, shipwiseProposedWaitingCoXoListController);
    shipwiseProposedWaitingCoXoListController.$inject = ['$stateParams', 'roasterListService', 'notificationService', '$state'];

    function shipwiseProposedWaitingCoXoListController($stateParams, roasterListService, notificationService, $state) {
        var vm = this;
        vm.officeId = 0;
        vm.shipType = 0;
        vm.appointment = 1;
        vm.largeShipCoXos = [];
        vm.courseAttendeds = [];
        vm.printSection = printSection;
        vm.shipName = '';
        vm.officeCurrentStatus = officeCurrentStatus;

        if ($stateParams.shipType !== undefined && $stateParams.shipType !== null) {
            vm.shipType = $stateParams.shipType;
        }
        if ($stateParams.officeId !== undefined && $stateParams.officeId !== null) {
            vm.officeId = $stateParams.officeId;
        }
        if ($stateParams.appointment !== undefined && $stateParams.appointment !== null) {
            vm.appointment = $stateParams.appointment;
        }

        Init();
        function Init() {


            roasterListService.getLargeShipProposedWaitingCoXoList(vm.shipType,vm.officeId,vm.appointment).then(function (data) {
                vm.largeShipCoXos = data.result;
                vm.shipName = vm.largeShipCoXos[0].shortName;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });

        };
        function officeCurrentStatus(pNo) {

            $state.goNewTab('current-status-tab', { pno: pNo });
        }
        function printSection(printSectionId) {
            var innerContents = document.getElementById(printSectionId).innerHTML;
            var popupWindow = window.open('', '', 'width=810,height=520,left=0,top=0,toolbar=0,status=0');
            popupWindow.document.open();
            popupWindow.document.write('<HTML>\n<HEAD>\n');
            popupWindow.document.write('<link rel="stylesheet" type="text/css" href="../../../Content/addmin//PdfReport.css"/>');
            popupWindow.document.write('</HEAD>\n');
            popupWindow.document.write('<BODY >\n');
            popupWindow.document.write(innerContents);
            popupWindow.document.write('</BODY>\n');
            popupWindow.document.write('</HTML>\n');
            popupWindow.document.close();
            popupWindow.focus();
        }

    }
})();
