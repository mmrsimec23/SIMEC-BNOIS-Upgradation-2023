

(function () {

    'use strict';

    var controllerId = 'currentStatusController';

    angular.module('app').controller(controllerId, currentStatusController);
    currentStatusController.$inject = ['$stateParams','employeeService','currentStatusService', 'notificationService','codeValue', '$state'];

    function currentStatusController($stateParams, employeeService, currentStatusService, notificationService, codeValue, $state) {
        var vm = this;
        vm.pNo = 0;
        vm.currentStatus = {};
        vm.notifications = [];
        vm.printSection = printSection;
        vm.specialNotification = true;
        


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

            currentStatusService.getCurrentStatus(vm.pNo).then(function (data) {
                vm.currentStatus = data.result;
                if (vm.currentStatus.notification == null || vm.currentStatus.notification == '') {
                    vm.specialNotification = false;
                }

                vm.currentStatus.photo = codeValue.API_OFFICER_PICTURE_URL + vm.currentStatus.photo;
                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });


            currentStatusService.getNotifications(vm.pNo).then(function (data) {
                vm.notifications = data.result;

//                    vm.notifications[0].notificationText += '. ' + vm.currentStatus.notification;

                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });

            currentStatusService.getRemark(vm.pNo).then(function (data) {
                vm.remarks = data.result;

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

       
    }
})();
