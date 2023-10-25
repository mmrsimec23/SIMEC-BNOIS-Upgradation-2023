﻿

(function () {

    'use strict';

    var controllerId = 'foreignVisitController';

    angular.module('app').controller(controllerId, foreignVisitController);
    foreignVisitController.$inject = ['$stateParams','employeeService','currentStatusService', 'notificationService', '$state'];

    function foreignVisitController($stateParams, employeeService,currentStatusService, notificationService, $state) {
        var vm = this;
        vm.pNo = 0;
        vm.foreignVisits = [];
        vm.courseAttendeds = [];
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


            currentStatusService.getForeignVisit(vm.pNo).then(function (data) {
                vm.foreignVisits = data.result;

                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });

            currentStatusService.getForeignCourseAttended(vm.pNo).then(function (data) {
                vm.courseAttendeds = data.result;

            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        };

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
