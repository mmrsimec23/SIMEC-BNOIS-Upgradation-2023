/// <reference path="../../services/employeeMscEducationService.js" />
(function () {

    'use strict';

    var controllerId = 'employeeMscEducationAddController';

    angular.module('app').controller(controllerId, employeeMscEducationAddController);
    employeeMscEducationAddController.$inject = ['$stateParams', 'employeeMscEducationService', 'backLogService','notificationService', '$state'];

    function employeeMscEducationAddController($stateParams, employeeMscEducationService, backLogService, notificationService, $state) {
        var vm = this;
        vm.employeeMscEducationId = 0;
        vm.title = 'ADD MODE';
        vm.employeeMscEducation = {};
        vm.mscEducationTypeList = [];
        vm.mscInstituteList = [];
        vm.mscPermissionTypeList = [];
        vm.countryList = [];
        vm.mscCompleteTypes = [];
        vm.ranks = [];
        vm.transfers = [];
        vm.saveButtonText = 'Save';
        vm.save = save;
        vm.close = close;
        vm.employeeMscEducationForm = {};

        if ($stateParams.id !== undefined && $stateParams.id !== null) {
            vm.employeeMscEducationId = $stateParams.id;
            vm.saveButtonText = 'Update';
            vm.title = 'UPDATE MODE';
        }

        Init();
        function Init() {
            employeeMscEducationService.getEmployeeMscEducation(vm.employeeMscEducationId).then(function (data) {
                vm.employeeMscEducation = data.result.employeeMscEducation;
               
                if (vm.employeeMscEducationId !== 0 && vm.employeeMscEducationId !== '') {

                    //vm.employeeMscEducation.completeStatus = data.result.employeeMscEducation.completeStatus;
                    if (vm.employeeMscEducation.fromDate != null) {
                        vm.employeeMscEducation.fromDate = new Date(data.result.employeeMscEducation.fromDate);
                    }
                    if (vm.employeeMscEducation.toDate != null) {
                        vm.employeeMscEducation.toDate = new Date(data.result.employeeMscEducation.toDate);
                    }
                }
                vm.mscEducationTypeList = data.result.mscEducationTypeList;
                vm.mscInstituteList = data.result.mscInstituteList;
                vm.mscPermissionTypeList = data.result.mscPermissionTypeList;
                vm.countryList = data.result.countryList;
                vm.mscCompleteTypes = data.result.mscCompleteTypes;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        };

        function save() {
            if (vm.employeeMscEducation.employee.employeeId > 0) {
                vm.employeeMscEducation.employeeId = vm.employeeMscEducation.employee.employeeId;
            } else {
                notificationService.displayError("Please Search Valid Officer by Pno!!");
            }

            if (vm.employeeMscEducationId !== 0 && vm.employeeMscEducationId !== '') {
                updateEmployeeMscEducation();
            } else {
                insertEmployeeMscEducation();
            }
        }

        function insertEmployeeMscEducation() {
            employeeMscEducationService.saveEmployeeMscEducation(vm.employeeMscEducation).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }
        function updateEmployeeMscEducation() {
            employeeMscEducationService.updateEmployeeMscEducation(vm.employeeMscEducationId, vm.employeeMscEducation).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function close() {
            $state.go('employee-msc-education-list');
        }

        //function isBackLogChecked(isBackLog) {
        //    if (isBackLog) {
        //        if (vm.employeeMscEducation.employee.employeeId > 0) {
        //            backLogService.getBackLogSelectModels(vm.employeeMscEducation.employee.employeeId).then(function (data) {
        //                vm.ranks = data.result.ranks;
        //                vm.transfers = data.result.transfers;
        //            });
        //        }
        //    }
        //}
    }
})();
