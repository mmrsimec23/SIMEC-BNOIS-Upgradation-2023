(function () {

    'use strict';

    var controllerId = 'minuiteAddController';

    angular.module('app').controller(controllerId, minuiteAddController);
    minuiteAddController.$inject = ['$stateParams', 'minuiteService', 'notificationService', '$state'];

    function minuiteAddController($stateParams,  minuiteService, notificationService, $state) {
        var vm = this;
        vm.minuiteId = 0;
        vm.title = 'ADD MODE';
        vm.minuite = {};
        vm.countries = [];
        vm.categories = [{ text: 'UNM', value: 1 }, { text: 'General Course', value: 2 }, { text: 'Long Course', value: 3 }, { text: 'Visit/FAT/Seminar etc', value: 4 }, { text: 'Staff Course', value: 5 }];
        vm.saveButtonText = 'Save';
        vm.save = save;
        vm.close = close;
        vm.minuiteForm = {};

        if ($stateParams.id !== undefined && $stateParams.id !== null) {
            vm.minuiteId = $stateParams.id;
            vm.saveButtonText = 'Update';
            vm.title = 'UPDATE MODE';
        }

        Init();
        function Init() {
             minuiteService.getMinuite(vm.minuiteId).then(function (data) {
                 vm.minuite = data.result.minuite;
                 vm.countries = data.result.countries;

                 if (vm.minuite.startDate !=null) {
                     vm.minuite.startDate = new Date(vm.minuite.startDate);
                 }
                 if (vm.minuite.endDate !=null) {
                     vm.minuite.endDate = new Date(vm.minuite.endDate);
                 }
                 if (vm.minuite.minuiteDate !=null) {
                     vm.minuite.minuiteDate = new Date(vm.minuite.minuiteDate);
                 }
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        };

        function save() {
            if (vm.minuiteId !== 0 && vm.minuiteId !== '') {
                updateMinuite();
            } else {
                insertMinuite();
            }
        }

        function insertMinuite() {
            vm.minuite.ltCdrLevel = 1;
             minuiteService.saveMinuite(vm.minuite).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }
        function updateMinuite() {
             minuiteService.updateMinuite(vm.minuiteId, vm.minuite).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function close() {
            $state.go('minuites');
        }
    }
})();
