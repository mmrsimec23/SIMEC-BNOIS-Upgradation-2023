

(function () {

    'use strict';
    var controllerId = 'employeeProposedEoSoLosController';
    angular.module('app').controller(controllerId, employeeProposedEoSoLosController);
    employeeProposedEoSoLosController.$inject = ['$state', 'employeeProposedEolosodloseoService', 'notificationService', '$location'];

    function employeeProposedEoSoLosController($state, employeeProposedEolosodloseoService, notificationService, location) {

        /* jshint validthis:true */
        var vm = this;
        vm.employeeProposedEoSoLos = [];
        vm.addEmployeeProposedEoSoLo = addEmployeeProposedEoSoLo;
        vm.updateEmployeeProposedEoSoLo = updateEmployeeProposedEoSoLo;
        vm.deleteEmployeeProposedEoSoLo = deleteEmployeeProposedEoSoLo;
        vm.pageChanged = pageChanged;
        vm.searchText = "";
        vm.onSearch = onSearch;
        vm.pageSize = 50;
        vm.pageNumber = 1;
        vm.total = 0;

        if (location.search().ps !== undefined && location.search().ps !== null && location.search().ps !== '') {
            vm.pageSize = location.search().ps;
        }

        if (location.search().pn !== undefined && location.search().pn !== null && location.search().pn !== '') {
            vm.pageNumber = location.search().pn;
        }
        if (location.search().q !== undefined && location.search().q !== null && location.search().q !== '') {
            vm.searchText = location.search().q;
        }
        init();
        function init() {
            employeeProposedEolosodloseoService.getemployeeProposedEolosodloseos(2,vm.pageSize, vm.pageNumber, vm.searchText).then(function (data) {
                vm.employeeProposedEoSoLos = data.result;
                vm.total = data.total; vm.permission = data.permission;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function addEmployeeProposedEoSoLo() {
            $state.go('employee-proposed-eosolo-create');
        }

        function updateEmployeeProposedEoSoLo(EmployeeProposedEoSoLo) {
            $state.go('employee-proposed-eosolo-modify', { id: EmployeeProposedEoSoLo.coXoServiceId });
        }

        function deleteEmployeeProposedEoSoLo(EmployeeProposedEoSoLo) {
            employeeProposedEolosodloseoService.deleteemployeeProposedEolosodloseo(EmployeeProposedEoSoLo.coXoServiceId).then(function (data) {
                $state.go($state.current, { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
            });
        }

        function pageChanged() {
            $state.go('employee-proposed-eosolos', { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
        }

        function onSearch() {
            vm.pageNumber = 1;
            pageChanged();
        }
    }

})();

