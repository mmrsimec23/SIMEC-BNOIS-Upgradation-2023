/// <reference path="../../services/employeeUnmDefermentService.js" />
(function () {

    'use strict';

    var controllerId = 'employeeUnmDefermentAddController';

    angular.module('app').controller(controllerId, employeeUnmDefermentAddController);
    employeeUnmDefermentAddController.$inject = ['$stateParams', 'employeeUnmDefermentService', 'backLogService','notificationService', '$state'];

    function employeeUnmDefermentAddController($stateParams, employeeUnmDefermentService, backLogService, notificationService, $state) {
        var vm = this;
        vm.employeeUnmDefermentId = 0;
        vm.title = 'ADD MODE';
        vm.employeeUnmDeferment = {};
        vm.resons = [
            { 'value': 1, 'text': 'Unwilling' }, { 'value': 2, 'text': 'Punishment' }
        ];
        vm.saveButtonText = 'Save';
        vm.save = save;
        vm.close = close;
        vm.employeeUnmDefermentForm = {};

        if ($stateParams.id !== undefined && $stateParams.id !== null) {
            vm.employeeUnmDefermentId = $stateParams.id;
            vm.saveButtonText = 'Update';
            vm.title = 'UPDATE MODE';
        }

        Init();
        function Init() {
            //employeeUnmDefermentService.getEmployeeUnmDeferment(vm.employeeUnmDefermentId).then(function (data) {
            //    vm.employeeUnmDeferment = data.result;
            //    console.log(vm.employeeUnmDeferment);
               
            //        //if (vm.employeeUnmDeferment.defermentFrom != null) {
            //        //    vm.employeeUnmDeferment.defermentFrom = new Date(data.result.defermentFrom);
            //        //}
                
            //},
            //    function (errorMessage) {
            //        notificationService.displayError(errorMessage.message);
            //    });

            employeeUnmDefermentService.getEmployeeUnmDeferment(vm.employeeUnmDefermentId).then(function (data) {
                vm.employeeUnmDeferment = data.result;
                if (vm.employeeUnmDeferment.defermentFrom != null) {
                    vm.employeeUnmDeferment.defermentFrom = new Date(vm.employeeUnmDeferment.defermentFrom);
                }

            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        };

        function save() {
            if (vm.employeeUnmDeferment.employee.employeeId > 0) {
                vm.employeeUnmDeferment.employeeId = vm.employeeUnmDeferment.employee.employeeId;
            } else {
                notificationService.displayError("Please Search Valid Officer by Pno!!");
            }

            if (vm.employeeUnmDefermentId !== 0 && vm.employeeUnmDefermentId !== '') {
                updateEmployeeUnmDeferment();
            } else {
                insertEmployeeUnmDeferment();
            }
        }

        function insertEmployeeUnmDeferment() {
            employeeUnmDefermentService.saveEmployeeUnmDeferment(vm.employeeUnmDeferment).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }
        function updateEmployeeUnmDeferment() {
            employeeUnmDefermentService.updateEmployeeUnmDeferment(vm.employeeUnmDefermentId, vm.employeeUnmDeferment).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function close() {
            $state.go('employee-unm-deferment-list');
        }

        //function isBackLogChecked(isBackLog) {
        //    if (isBackLog) {
        //        if (vm.employeeUnmDeferment.employee.employeeId > 0) {
        //            backLogService.getBackLogSelectModels(vm.employeeUnmDeferment.employee.employeeId).then(function (data) {
        //                vm.ranks = data.result.ranks;
        //                vm.transfers = data.result.transfers;
        //            });
        //        }
        //    }
        //}
    }
})();
