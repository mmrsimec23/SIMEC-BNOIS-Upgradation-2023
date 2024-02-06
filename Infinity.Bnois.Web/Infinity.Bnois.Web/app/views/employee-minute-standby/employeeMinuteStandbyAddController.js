/// <reference path="../../services/employeeMinuteStandbyService.js" />
(function () {

    'use strict';

    var controllerId = 'employeeMinuteStandbyAddController';

    angular.module('app').controller(controllerId, employeeMinuteStandbyAddController);
    employeeMinuteStandbyAddController.$inject = ['$stateParams', 'employeeMinuteStandbyService', 'backLogService','notificationService', '$state'];

    function employeeMinuteStandbyAddController($stateParams, employeeMinuteStandbyService, backLogService, notificationService, $state) {
        var vm = this;
        vm.employeeMinuteStandbyId = 0;
        vm.title = 'ADD MODE';
        vm.employeeMinuteStandby = {};
        
        vm.saveButtonText = 'Save';
        vm.save = save;
        vm.close = close;
        vm.employeeMinuteStandbyForm = {};

        if ($stateParams.id !== undefined && $stateParams.id !== null) {
            vm.employeeMinuteStandbyId = $stateParams.id;
            vm.saveButtonText = 'Update';
            vm.title = 'UPDATE MODE';
        }

        Init();
        function Init() {
            

            employeeMinuteStandbyService.getEmployeeMinuteStandby(vm.employeeMinuteStandbyId).then(function (data) {
                vm.employeeMinuteStandby = data.result;
                if (vm.employeeMinuteStandby.dateFrom != null) {
                    vm.employeeMinuteStandby.dateFrom = new Date(vm.employeeMinuteStandby.dateFrom);
                }
                if (vm.employeeMinuteStandby.dateTo != null) {
                    vm.employeeMinuteStandby.dateTo = new Date(vm.employeeMinuteStandby.dateTo);
                }

            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        };

        function save() {
            if (vm.employeeMinuteStandby.employee.employeeId > 0) {
                vm.employeeMinuteStandby.employeeId = vm.employeeMinuteStandby.employee.employeeId;
            } else {
                notificationService.displayError("Please Search Valid Officer by Pno!!");
            }

            if (vm.employeeMinuteStandbyId !== 0 && vm.employeeMinuteStandbyId !== '') {
                updateEmployeeMinuteStandby();
            } else {
                insertEmployeeMinuteStandby();
            }
        }

        function insertEmployeeMinuteStandby() {
            employeeMinuteStandbyService.saveEmployeeMinuteStandby(vm.employeeMinuteStandby).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }
        function updateEmployeeMinuteStandby() {
            employeeMinuteStandbyService.updateEmployeeMinuteStandby(vm.employeeMinuteStandbyId, vm.employeeMinuteStandby).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function close() {
            $state.go('employee-minute-standby-list');
        }

        //function isBackLogChecked(isBackLog) {
        //    if (isBackLog) {
        //        if (vm.employeeMinuteStandby.employee.employeeId > 0) {
        //            backLogService.getBackLogSelectModels(vm.employeeMinuteStandby.employee.employeeId).then(function (data) {
        //                vm.ranks = data.result.ranks;
        //                vm.transfers = data.result.transfers;
        //            });
        //        }
        //    }
        //}
    }
})();
