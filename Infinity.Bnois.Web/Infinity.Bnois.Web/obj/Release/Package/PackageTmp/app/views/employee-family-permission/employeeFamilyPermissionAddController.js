/// <reference path="../../services/employeeFamilyPermissionService.js" />
(function () {

    'use strict';

    var controllerId = 'employeeFamilyPermissionAddController';

    angular.module('app').controller(controllerId, employeeFamilyPermissionAddController);
    employeeFamilyPermissionAddController.$inject = ['$stateParams', 'employeeFamilyPermissionService', 'backLogService','notificationService', '$state'];

    function employeeFamilyPermissionAddController($stateParams, employeeFamilyPermissionService, backLogService, notificationService, $state) {
        var vm = this;
        vm.employeeFamilyPermissionId = 0;
        vm.title = 'ADD MODE';
        vm.employeeFamilyPermission = {};
        vm.familyPermissionRelationTypes = [];
        vm.familyPermissionCountryList = [];
        vm.ranks = [];
        vm.transfers = [];
        vm.saveButtonText = 'Save';
        vm.save = save;
        vm.close = close;
        vm.employeeFamilyPermissionForm = {};
        vm.isBackLogChecked = isBackLogChecked;

        if ($stateParams.id !== undefined && $stateParams.id !== null) {
            vm.employeeFamilyPermissionId = $stateParams.id;
            vm.saveButtonText = 'Update';
            vm.title = 'UPDATE MODE';
        }

        Init();
        function Init() {
            employeeFamilyPermissionService.getEmployeeFamilyPermission(vm.employeeFamilyPermissionId).then(function (data) {
                vm.employeeFamilyPermission = data.result.employeeFamilyPermission;
               
                if (vm.employeeFamilyPermissionId !== 0 && vm.employeeFamilyPermissionId !== '') {

                    vm.employeeFamilyPermission.fromDate = new Date(data.result.employeeFamilyPermission.fromDate);
                    if (vm.employeeFamilyPermission.toDate != null) {
                        vm.employeeFamilyPermission.toDate = new Date(data.result.employeeFamilyPermission.toDate);
                    }
                }
                vm.familyPermissionRelationTypes = data.result.familyPermissionRelationTypes;
                vm.familyPermissionCountryList = data.result.familyPermissionCountryList;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        };

        function save() {
            if (vm.employeeFamilyPermission.employee.employeeId > 0) {
                vm.employeeFamilyPermission.employeeId = vm.employeeFamilyPermission.employee.employeeId;
            } else {
                notificationService.displayError("Please Search Valid Officer by Pno!!");
            }

            if (vm.employeeFamilyPermissionId !== 0 && vm.employeeFamilyPermissionId !== '') {
                updateemployeeFamilyPermission();
            } else {
                insertemployeeFamilyPermission();
            }
        }

        function insertemployeeFamilyPermission() {
            employeeFamilyPermissionService.saveEmployeeFamilyPermission(vm.employeeFamilyPermission).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }
        function updateemployeeFamilyPermission() {
            employeeFamilyPermissionService.updateEmployeeFamilyPermission(vm.employeeFamilyPermissionId, vm.employeeFamilyPermission).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function close() {
            $state.go('employee-family-permission-list');
        }

        function isBackLogChecked(isBackLog) {
            if (isBackLog) {
                if (vm.employeeFamilyPermission.employee.employeeId > 0) {
                    backLogService.getBackLogSelectModels(vm.employeeFamilyPermission.employee.employeeId).then(function (data) {
                        vm.ranks = data.result.ranks;
                        vm.transfers = data.result.transfers;
                    });
                }
            }
        }
    }
})();
