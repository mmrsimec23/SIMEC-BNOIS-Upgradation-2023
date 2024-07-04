(function () {

    'use strict';
    var controllerId = 'dashboardTabController';
    angular.module('app').controller(controllerId, dashboardTabController);
    dashboardTabController.$inject = ['$stateParams', '$state', 'codeValue', 'notificationService', 'officeService', 'employeeService','quickLinkService'];

    function dashboardTabController($stateParams, $state, codeValue, notificationService, officeService, employeeService, quickLinkService) {
        /* jshint validthis:true */
        var vm = this;
        vm.URL = codeValue.FILE_URL;
        vm.searchByPno = searchByPno;
        vm.searchByName = searchByName;
        vm.quickLinks = [];
        vm.officeStructure = officeStructure;

        vm.officeList = [];

        vm.localOfficeSearch = localOfficeSearch;
        vm.selected = selected;
        vm.searchByOfficeId = searchByOfficeId;
        vm.searchedOfficeId = 0;

        init();
        function init() {
            $state.go('dashboard.outside-navy');

            quickLinkService.getDashboardQuickLinks().then(function (data) {
                    vm.quickLinks = data.result;

                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });

            officeService.getOfficesForSearch().then(function (data) {
                vm.officeList = data.result;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });

        }


        function searchByPno(pNo) {
            if (pNo != null) {
                employeeService.getEmployeeByPno(pNo).then(function (data) {
                        $state.goNewTab('current-status-tab', { pno: pNo });

                    },
                    function (errorMessage) {
                        notificationService.displayError(errorMessage.message);
                    });
                
            }
        }

        function searchByName(name) {
            if (name != null) {
                $state.goNewTab('officers', { q: name });

            }
        }


        function officeStructure() {
            $state.goNewTab('office-structures');
            
        }

        function localOfficeSearch(str) {
            var matches = [];
            vm.officeList.forEach(function (transfer) {

                if ((transfer.text.toLowerCase().indexOf(str.toString().toLowerCase()) >= 0)) {
                    matches.push(transfer);

                }
            });
            return matches;
        }


        function selected(object) {
            vm.searchedOfficeId = object.originalObject.value;
            console.log(vm.searchedOfficeId)
            if (vm.searchedOfficeId > 0) {
                $state.goNewTab('office-search-result', { officeId: vm.searchedOfficeId });

            }
        }

        function searchByOfficeId() {
            if (vm.searchedOfficeId > 0) {
                $state.goNewTab('office-search-result', { officeId: vm.searchedOfficeId });

            }
        }

    }

})();
