﻿

(function () {

    'use strict';

    var controllerId = 'seaServiceInfoController';

    angular.module('app').controller(controllerId, seaServiceInfoController);
    seaServiceInfoController.$inject = ['$stateParams', 'employeeService','currentStatusService', 'notificationService', '$state'];

    function seaServiceInfoController($stateParams, employeeService, currentStatusService, notificationService, $state) {
        var vm = this;
        vm.pNo = 0;
        vm.additionalSeaServicesGrandTotal = 0;
        vm.additionalSeaServices = [];

        vm.seaServicesGrandTotal = 0;
        vm.shoreCommandServicesGrandTotal = 0;
        vm.seaCommandServicesGrandTotal = 0;
        vm.seaServices = [];
        vm.seaCommandServices = [];
        vm.shoreCommandServices = [];
        vm.adminAuthorityServices = [];
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


            currentStatusService.getSeaServices(vm.pNo).then(function (data) {
                vm.seaServices = data.result.services;
                vm.seaServicesGrandTotal = data.result.grandTotal;

                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });

            currentStatusService.getAdditionalSeaServices(vm.pNo).then(function (data) {
                vm.additionalSeaServices = data.result.services;
                vm.additionalSeaServicesGrandTotal = data.result.grandTotal;
                  
                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });

            currentStatusService.getSeaCommandServices(vm.pNo).then(function (data) {
                vm.seaCommandServices = data.result.services;
                vm.seaCommandServicesGrandTotal = data.result.grandTotal;

            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });

            currentStatusService.getShoreCommandServices(vm.pNo).then(function (data) {
                vm.shoreCommandServices = data.result;

            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });


            currentStatusService.getAdminAuthorityService(vm.pNo).then(function (data) {
                vm.adminAuthorityServices = data.result;

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
