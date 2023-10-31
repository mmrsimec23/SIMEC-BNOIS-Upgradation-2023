

(function () {

    'use strict';

    var controllerId = 'largeShipCoXoController';

    angular.module('app').controller(controllerId, largeShipCoXoController);
    largeShipCoXoController.$inject = ['$stateParams', 'roasterListService', 'notificationService', '$state'];

    function largeShipCoXoController($stateParams, roasterListService, notificationService, $state) {
        var vm = this;
        vm.shipType = 3;
        vm.coxoStatus = 0;
        vm.largeShipCoXos = [];
        vm.courseAttendeds = [];
        vm.printSection = printSection;
        vm.officeCurrentStatus = officeCurrentStatus;

        if ($stateParams.shipType !== undefined && $stateParams.shipType !== null) {
            vm.shipType = $stateParams.shipType;
        }

        Init();
        function Init() {


            roasterListService.getRoasterListByShipType(vm.shipType, vm.coxoStatus).then(function (data) {
                vm.largeShipCoXos = data.result;
                console.log(vm.largeShipCoXos);
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
