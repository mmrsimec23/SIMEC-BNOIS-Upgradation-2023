

(function () {

    'use strict';

    var controllerId = 'branchAuthorityOfficersController';

    angular.module('app').controller(controllerId, branchAuthorityOfficersController);
    branchAuthorityOfficersController.$inject = ['$stateParams','dashboardService', 'notificationService','currentStatusService', '$state'];

    function branchAuthorityOfficersController($stateParams, dashboardService, notificationService, currentStatusService, $state) {
        var vm = this;
        vm.pNo = 0;
        vm.branchAuthorityComDhaka = [];
        vm.branchAuthorityComChit = [];
        vm.branchAuthorityComBan = [];
        vm.branchAuthorityComKhul = [];
        vm.branchAuthorityCsd = [];
        vm.branchAuthorityComNav = [];
        vm.branchAuthorityComSwads = [];
        vm.branchAuthorityComSub = [];
        vm.branchAuthorityComFlotWest = [];
        vm.branchAuthorityCho = [];
        vm.printSection = printSection;

        if ($stateParams.pno !== undefined && $stateParams.pno !== null) {
            vm.pNo = $stateParams.pno;
        }

        Init();
        function Init() {

            //employeeService.getEmployeeByPno(vm.pNo).then(function (data) {
            //        vm.officerName = data.result.fullNameEng;

            //    },
            //    function (errorMessage) {
            //        notificationService.displayError(errorMessage.message);
            //    });

            
            dashboardService.getBranchAuthorityOfficers9(9).then(function (data) {
                vm.branchAuthorityComDhaka = data.result;
                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
            dashboardService.getBranchAuthorityOfficers10(10).then(function (data) {
                vm.branchAuthorityComChit = data.result;
                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
            dashboardService.getBranchAuthorityOfficers11(11).then(function (data) {
                vm.branchAuthorityComBan = data.result;
                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
            dashboardService.getBranchAuthorityOfficers12(12).then(function (data) {
                vm.branchAuthorityComKhul = data.result;
                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
            dashboardService.getBranchAuthorityOfficers13(13).then(function (data) {
                vm.branchAuthorityCsd = data.result;
                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
            dashboardService.getBranchAuthorityOfficers369(369).then(function (data) {
                vm.branchAuthorityComNav = data.result;
                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
            dashboardService.getBranchAuthorityOfficers383(383).then(function (data) {
                vm.branchAuthorityComSwads = data.result;
                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
            dashboardService.getBranchAuthorityOfficers458(458).then(function (data) {
                vm.branchAuthorityComSub = data.result;
                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
            dashboardService.getBranchAuthorityOfficers513(513).then(function (data) {
                vm.branchAuthorityComFlotWest = data.result;
                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
            dashboardService.getBranchAuthorityOfficers543(543).then(function (data) {
                vm.branchAuthorityCho = data.result;
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
