

(function () {

    'use strict';

    var controllerId = 'employeeTraceAdd';

    angular.module('app').controller(controllerId, employeeTraceAdd);
    employeeTraceAdd.$inject = ['$stateParams', 'employeeService', 'employeeTraceService', 'notificationService', '$state'];

    function employeeTraceAdd($stateParams, employeeService, employeeTraceService, notificationService, $state) {
        var vm = this;
        vm.employeeTraceId = 0;
        vm.pNo = '';
        vm.title = 'ADD MODE';
        vm.employeeTrace = {};
        vm.TraceFiscalYears = [];
        vm.ranks = [];
        vm.employeeInfo = {};
        vm.saveButtonText = 'Save';
        vm.save = save;
        vm.close = close;
        vm.getEmployeeInfo = getEmployeeInfo;
        vm.getEmployeeInfoByEmployeeId = getEmployeeInfoByEmployeeId;
        vm.employeeTraceForm = {};

        if ($stateParams.id !== undefined && $stateParams.id !== '') {
            vm.employeeTraceId = $stateParams.id;
            vm.saveButtonText = 'Update';
            vm.title = 'UPDATE MODE';
        }

        Init();
        function Init() {
            employeeTraceService.getEmployeeTrace(vm.employeeTraceId).then(function (data) {
                vm.employeeTrace = data.result;
                //if (vm.employeeTrace.availDate != null) {
                //    vm.employeeTrace.availDate = new Date(vm.employeeTrace.availDate);
                //}
                //vm.TraceFiscalYears = data.result.TraceFiscalYears;
                //vm.ranks = data.result.ranks;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        };
        function getEmployeeInfo() {

            employeeService.getEmployeeByPno(vm.employeeTrace.employee.pNo).then(function (data) {
                vm.employeeTrace.employee = data.result;
                vm.employeeTrace.employeeId = vm.employeeTrace.employee.employeeId;
                vm.employeeTrace.rankId = vm.employeeTrace.employee.rankId;
                
            }, function (errorMessage) {
                notificationService.displayError(errorMessage.message);
            });
        }
        function getEmployeeInfoByEmployeeId() {

            employeeService.getEmployee(vm.employeeTrace.employeeId).then(function (data) {
                console.log(data.result.employee);
                vm.employeeInfo = data.result.employee;
                vm.pNo = vm.employeeInfo.pNo;
                vm.employeeTrace.rankId = vm.employeeInfo.rankId;
                vm.employeeTrace.employeeId = vm.employeeInfo.employeeId;
            }, function (errorMessage) {
                notificationService.displayError(errorMessage.message);
            });
        }
        function save() {

            if (vm.employeeTraceId !== 0) {
                updateEmployeeTrace();
            } else {
                insertEmployeeTrace();
            }
        }

        function insertEmployeeTrace() {
            
            employeeTraceService.saveEmployeeTrace(vm.employeeTrace).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function updateEmployeeTrace() {
            employeeTraceService.updateEmployeeTrace(vm.employeeTraceId, vm.employeeTrace).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function close() {
            $state.go('employee-trace-list');
        }

    }
})();
