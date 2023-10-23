﻿(function () {

    'use strict';
    var controllerId = 'employeeMscEducationListController';
    angular.module('app').controller(controllerId, employeeMscEducationListController);
    employeeMscEducationListController.$inject = ['$state', 'employeeMscEducationService', 'notificationService', '$location'];

    function employeeMscEducationListController($state, employeeMscEducationService, notificationService, location) {

        /* jshint validthis:true */
        var vm = this;
        vm.employeeMscEducationList = [];
        vm.addEmployeeMscEducation = addEmployeeMscEducation;
        vm.updateEmployeeMscEducation = updateEmployeeMscEducation;
        vm.deleteEmployeeMscEducation = deleteEmployeeMscEducation;
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
            employeeMscEducationService.getEmployeeMscEducations(vm.pageSize, vm.pageNumber, vm.searchText).then(function (data) {
                vm.employeeMscEducationList = data.result;
                vm.total = data.total; vm.permission = data.permission;
                console.log(vm.employeeMscEducationList);
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function addEmployeeMscEducation() {
            $state.go('employee-msc-education-create');
        }

        function updateEmployeeMscEducation(employeeMscEducation) {
            $state.go('employee-msc-education-modify', { id: employeeMscEducation.employeeMscEducationId });
        }

        function deleteEmployeeMscEducation(employeeMscEducation) {
            employeeMscEducationService.deleteEmployeeMscEducation(employeeMscEducation.employeeMscEducationId).then(function (data) {
                $state.go($state.current, { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
            });
        }

        function pageChanged() {
            $state.go('employee-msc-education-list', { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
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
