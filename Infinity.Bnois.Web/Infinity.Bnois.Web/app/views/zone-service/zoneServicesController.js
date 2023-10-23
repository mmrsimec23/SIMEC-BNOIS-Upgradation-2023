﻿

(function () {

    'use strict';

    var controllerId = 'zoneServicesController';

    angular.module('app').controller(controllerId, zoneServicesController);
    zoneServicesController.$inject = ['$stateParams','employeeService','currentStatusService', 'notificationService', '$state'];

    function zoneServicesController($stateParams, employeeService, currentStatusService, notificationService, $state) {
        var vm = this;
        vm.pNo = 0;
        vm.additionalzoneServicesGrandTotal = 0;
        vm.additionalzoneServices = [];

        vm.zoneServicesGrandTotal = 0;
        vm.zoneServices = [];

        vm.zoneCourseMissionServicesGrandTotal = 0;
        vm.zoneCourseMissionServices = [];
        vm.printSection = printSection;

        if ($stateParams.pno !== undefined && $stateParams.pno !== null) {
            vm.pNo = $stateParams.pno;
        }

        Init();
        function Init() {


            employeeService.getEmployeeByPno(vm.pNo).then(function (data) {
                    vm.officerName = data.result.fullNameEng;

                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });


            currentStatusService.getZoneServices(vm.pNo).then(function (data) {
                vm.zoneServices = data.result.services;
                vm.zoneServicesGrandTotal = data.result.grandTotal;

                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });


            currentStatusService.getZoneCourseMissionServices(vm.pNo).then(function (data) {
                vm.zoneCourseMissionServices = data.result.services;
                vm.zoneCourseMissionServicesGrandTotal = data.result.grandTotal;

                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });

        };


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
