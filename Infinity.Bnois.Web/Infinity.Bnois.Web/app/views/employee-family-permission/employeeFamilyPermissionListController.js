(function () {

    'use strict';
    var controllerId = 'employeeFamilyPermissionListController';
    angular.module('app').controller(controllerId, employeeFamilyPermissionListController);
    employeeFamilyPermissionListController.$inject = ['$state', 'employeeFamilyPermissionService', 'notificationService', '$location'];

    function employeeFamilyPermissionListController($state, employeeFamilyPermissionService, notificationService, location) {

        /* jshint validthis:true */
        var vm = this;
        vm.employeeFamilyPermissionList = [];
        vm.addEmployeeFamilyPermission = addEmployeeFamilyPermission;
        vm.updateEmployeeFamilyPermission = updateEmployeeFamilyPermission;
        vm.deleteEmployeeFamilyPermission = deleteEmployeeFamilyPermission;
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
            employeeFamilyPermissionService.getEmployeeFamilyPermissions(vm.pageSize, vm.pageNumber, vm.searchText).then(function (data) {
                vm.employeeFamilyPermissionList = data.result;
                vm.total = data.total; vm.permission = data.permission;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function addEmployeeFamilyPermission() {
            $state.go('employee-family-permission-create');
        }

        function updateEmployeeFamilyPermission(employeeFamilyPermission) {
            $state.go('employee-family-permission-modify', { id: employeeFamilyPermission.employeeFamilyPermissionId });
        }

        function deleteEmployeeFamilyPermission(employeeFamilyPermission) {
            employeeFamilyPermissionService.deleteEmployeeFamilyPermission(employeeFamilyPermission.employeeFamilyPermissionId).then(function (data) {
                $state.go($state.current, { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
            });
        }

        function pageChanged() {
            $state.go('employee-family-permission-list', { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
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
