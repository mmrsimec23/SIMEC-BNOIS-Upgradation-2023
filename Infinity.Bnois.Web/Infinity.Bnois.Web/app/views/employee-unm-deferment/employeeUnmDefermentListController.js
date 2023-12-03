(function () {

    'use strict';
    var controllerId = 'employeeUnmDefermentListController';
    angular.module('app').controller(controllerId, employeeUnmDefermentListController);
    employeeUnmDefermentListController.$inject = ['$state', 'employeeUnmDefermentService', 'notificationService', '$location'];

    function employeeUnmDefermentListController($state, employeeUnmDefermentService, notificationService, location) {

        /* jshint validthis:true */
        var vm = this;
        vm.employeeUnmDefermentList = [];
        vm.addEmployeeUnmDeferment = addEmployeeUnmDeferment;
        vm.updateEmployeeUnmDeferment = updateEmployeeUnmDeferment;
        vm.deleteEmployeeUnmDeferment = deleteEmployeeUnmDeferment;
        //vm.reasonList = reasonList;
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
            employeeUnmDefermentService.getEmployeeUnmDeferments(vm.pageSize, vm.pageNumber, vm.searchText).then(function (data) {
                vm.employeeUnmDefermentList = data.result;
                vm.total = data.total; vm.permission = data.permission;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function addEmployeeUnmDeferment() {
            $state.go('employee-unm-deferment-create');
        }

        function updateEmployeeUnmDeferment(employeeUnmDeferment) {
            $state.go('employee-unm-deferment-modify', { id: employeeUnmDeferment.id });
        }

        function deleteEmployeeUnmDeferment(employeeUnmDeferment) {
            employeeUnmDefermentService.deleteEmployeeUnmDeferment(employeeUnmDeferment.id).then(function (data) {
                $state.go($state.current, { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
            });
        }

        function pageChanged() {
            $state.go('employee-unm-deferment-list', { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
        }

        //function reasonList() {
        //    $state.go('security-clearance-reasons');
        //}

        function onSearch() {
            vm.pageNumber = 1;
            pageChanged();
        }
    }

})();
