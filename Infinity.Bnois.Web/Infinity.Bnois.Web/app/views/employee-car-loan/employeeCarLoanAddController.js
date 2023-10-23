

(function () {

    'use strict';

    var controllerId = 'employeeCarLoanAdd';

    angular.module('app').controller(controllerId, employeeCarLoanAdd);
    employeeCarLoanAdd.$inject = ['$stateParams', 'employeeService', 'employeeCarLoanService', 'notificationService', '$state'];

    function employeeCarLoanAdd($stateParams, employeeService, employeeCarLoanService, notificationService, $state) {
        var vm = this;
        vm.employeeCarLoanId = 0;
        vm.pNo = '';
        vm.title = 'ADD MODE';
        vm.employeeCarLoan = {};
        vm.carLoanFiscalYears = [];
        vm.ranks = [];
        vm.employeeInfo = {};
        vm.saveButtonText = 'Save';
        vm.save = save;
        vm.close = close;
        vm.getEmployeeInfo = getEmployeeInfo;
        vm.getEmployeeInfoByEmployeeId = getEmployeeInfoByEmployeeId;
        vm.employeeCarLoanForm = {};

        if ($stateParams.id !== undefined && $stateParams.id !== '') {
            vm.employeeCarLoanId = $stateParams.id;
            vm.saveButtonText = 'Update';
            vm.title = 'UPDATE MODE';
        }

        Init();
        function Init() {
            employeeCarLoanService.getEmployeeCarLoan(vm.employeeCarLoanId).then(function (data) {
                vm.employeeCarLoan = data.result.employeeCarLoan;
                if (vm.employeeCarLoan.availDate != null) {
                    vm.employeeCarLoan.availDate = new Date(vm.employeeCarLoan.availDate);
                }
                vm.carLoanFiscalYears = data.result.carLoanFiscalYears;
                vm.ranks = data.result.ranks;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        };
        function getEmployeeInfo() {

            employeeService.getEmployeeByPno(vm.employeeCarLoan.employee.pNo).then(function (data) {
                vm.employeeCarLoan.employee = data.result;
                vm.employeeCarLoan.employeeId = vm.employeeCarLoan.employee.employeeId
                
            }, function (errorMessage) {
                notificationService.displayError(errorMessage.message);
            });
        }
        function getEmployeeInfoByEmployeeId() {

            employeeService.getEmployee(vm.employeeCarLoan.employeeId).then(function (data) {
                vm.employeeInfo = data.result.employee;
                vm.pNo = vm.employeeInfo.pNo;
                vm.employeeCarLoan.employeeId = vm.employeeInfo.employeeId
            }, function (errorMessage) {
                notificationService.displayError(errorMessage.message);
            });
        }
        function save() {

            if (vm.employeeCarLoanId !== 0) {
                updateEmployeeCarLoan();
            } else {
                insertEmployeeCarLoan();
            }
        }

        function insertEmployeeCarLoan() {
            employeeCarLoanService.saveEmployeeCarLoan(vm.employeeCarLoan).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function updateEmployeeCarLoan() {
            employeeCarLoanService.updateEmployeeCarLoan(vm.employeeCarLoanId, vm.employeeCarLoan).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function close() {
            $state.go('employee-car-loan-list');
        }

    }
})();
