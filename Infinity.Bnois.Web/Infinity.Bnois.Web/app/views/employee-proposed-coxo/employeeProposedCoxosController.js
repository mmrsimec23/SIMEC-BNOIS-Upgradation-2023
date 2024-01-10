

(function () {

    'use strict';
    var controllerId = 'employeeProposedCoxosController';
    angular.module('app').controller(controllerId, employeeProposedCoxosController);
    employeeProposedCoxosController.$inject = ['$state', 'employeeProposedCoxoService', 'notificationService', '$location'];

    function employeeProposedCoxosController($state, employeeProposedCoxoService, notificationService, location) {

        /* jshint validthis:true */
        var vm = this;
        vm.employeeProposedCoxos = [];
        vm.addEmployeeProposedCoxo = addEmployeeProposedCoxo;
        vm.updateEmployeeProposedCoxo = updateEmployeeProposedCoxo;
        vm.deleteEmployeeProposedCoxo = deleteEmployeeProposedCoxo;
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
            employeeProposedCoxoService.getemployeeProposedCoxos(1,vm.pageSize, vm.pageNumber, vm.searchText).then(function (data) {
                vm.employeeProposedCoxos = data.result;
                vm.total = data.total; vm.permission = data.permission;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function addEmployeeProposedCoxo() {
            $state.go('employee-proposed-coxo-create');
        }

        function updateEmployeeProposedCoxo(EmployeeProposedCoxo) {
            $state.go('employee-proposed-coxo-modify', { id: EmployeeProposedCoxo.coXoServiceId });
        }

        function deleteEmployeeProposedCoxo(EmployeeProposedCoxo) {
            employeeProposedCoxoService.deleteemployeeProposedCoxo(EmployeeProposedCoxo.coXoServiceId).then(function (data) {
                $state.go($state.current, { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
            });
        }

        function pageChanged() {
            $state.go('employee-proposed-coxos', { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
        }

        function onSearch() {
            vm.pageNumber = 1;
            pageChanged();
        }
    }

})();

