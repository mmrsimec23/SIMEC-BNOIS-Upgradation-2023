(function () {

    'use strict';
    var controllerId = 'employeeTraceListController';
    angular.module('app').controller(controllerId, employeeTraceListController);
    employeeTraceListController.$inject = ['$state', 'employeeTraceService', 'notificationService', '$location'];

    function employeeTraceListController($state, employeeTraceService, notificationService, location) {

        /* jshint validthis:true */
        var vm = this;
        vm.employeeTraceList = [];
        vm.addEmployeeTrace = addEmployeeTrace;
        vm.updateEmployeeTrace = updateEmployeeTrace;
        vm.deleteEmployeeTrace = deleteEmployeeTrace;
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
            employeeTraceService.getEmployeeTraceList(vm.pageSize, vm.pageNumber, vm.searchText).then(function (data) {

                vm.employeeTraceList = data.result;
                vm.total = data.total;
                vm.permission = data.permission;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function addEmployeeTrace() {
            $state.go('employee-trace-create');
        }

        function updateEmployeeTrace(employeeTrace) {
            $state.go('employee-trace-modify', { id: employeeTrace.id });
        }

        function deleteEmployeeTrace(employeeTrace) {
            employeeTraceService.deleteEmployeeTrace(employeeTrace.id).then(function (data) {
                $state.go($state.current, { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
            });
        }

        function pageChanged() {
            $state.go('employee-trace-list', { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });

        }

        function onSearch() {
            vm.pageNumber = 1;
            pageChanged();
        }
    }

})();
