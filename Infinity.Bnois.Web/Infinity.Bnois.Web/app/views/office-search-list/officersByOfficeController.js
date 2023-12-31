﻿

(function () {

    'use strict';

    var controllerId = 'officersByOfficeController'; 

    angular.module('app').controller(controllerId, officersByOfficeController);
    officersByOfficeController.$inject = ['$stateParams', '$rootScope', 'dashboardService', 'downloadService','reportService', 'notificationService', '$state'];

    function officersByOfficeController($stateParams, $rootScope, dashboardService, downloadService, reportService ,notificationService, $state) {
        var vm = this;
        vm.officerDetails = officerDetails;
        vm.officersByOffice = [];
        vm.officeId = 0;
        vm.printSection = printSection;

        if ($stateParams.officeId !== undefined && $stateParams.officeId !== null) {
            vm.officeId = $stateParams.officeId;
          
        }



        Init();
        function Init() {
            dashboardService.getOfficersByOffice(vm.officeId).then(function (data) {

                    vm.officersByOffice = data.result;
                    vm.totalResult = vm.officersByOffice.length;

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
