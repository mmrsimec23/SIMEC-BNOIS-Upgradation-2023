(function () {

    'use strict';

    var controllerId = 'carLoanFiscalYearAddController';

    angular.module('app').controller(controllerId, carLoanFiscalYearAddController);
    carLoanFiscalYearAddController.$inject = ['$stateParams', 'carLoanFiscalYearService', 'notificationService', '$state'];

    function carLoanFiscalYearAddController($stateParams, carLoanFiscalYearService, notificationService, $state) {
        var vm = this;
        vm.carLoanFiscalYearsId = 0;
        vm.title = 'ADD MODE';
        vm.carLoanFiscalYear = {};
        vm.saveButtonText = 'Save';
        vm.save = save
        vm.close = close;
        vm.carLoanFiscalYearForm = {};

        if ($stateParams.id !== undefined && $stateParams.id !== null) {
            vm.carLoanFiscalYearsId = $stateParams.id;
            vm.saveButtonText = 'Update';
            vm.title = 'UPDATE MODE';
        }

        Init();
        function Init() {
            carLoanFiscalYearService.getCarLoanFiscalYear(vm.carLoanFiscalYearsId).then(function (data) {
                vm.carLoanFiscalYear = data.result;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        };

        function save() {
            if (vm.carLoanFiscalYearsId !== 0 && vm.carLoanFiscalYearsId !== '') {
                updateCarLoanFiscalYear();
            } else {
                insertCarLoanFiscalYear();
            }
        }

        function insertCarLoanFiscalYear() {
            carLoanFiscalYearService.saveCarLoanFiscalYear(vm.carLoanFiscalYear).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }
        function updateCarLoanFiscalYear() {
            carLoanFiscalYearService.updateCarLoanFiscalYear(vm.carLoanFiscalYearsId, vm.carLoanFiscalYear).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function close() {
            $state.go('car-loan-fiscal-year-list');
        }
    }
})();
