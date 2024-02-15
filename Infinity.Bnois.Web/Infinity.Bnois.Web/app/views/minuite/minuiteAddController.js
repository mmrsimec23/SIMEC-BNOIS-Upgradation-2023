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
        vm.categories = [{ text: 'UNM', value: 1 }, { text: 'General Course', value: 2 }, { text: 'Long Course (E,L,S)', value: 3 }, { text: 'Visit/Seminar etc', value: 4 }, { text: 'Staff Course', value: 5 }, { text: 'FAT/PSI', value: 6 }];
        vm.saveButtonText = 'Save';
        vm.save = save;
        vm.close = close;
        vm.minuiteForm = {};
        vm.minuteFieldVisible = minuteFieldVisible;

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
                 minuteFieldVisible(vm.minuite.minuiteCategory)
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

        function minuteFieldVisible(minuiteCategory) {
            console.log(minuiteCategory);
            vm.unmCat = false;
            vm.gcCat = false;
            vm.lcCat = false;
            vm.vsCat = false;
            vm.scCat = false;
            vm.fpCat = false;
            if (minuiteCategory == 1) {
                vm.unmCat = true;
            }
            else if (minuiteCategory == 2) {
                vm.gcCat = true;
            }
            else if (minuiteCategory == 3) {
                vm.lcCat = true;
            }
            else if (minuiteCategory == 4) {
                vm.vsCat = true;
            }
            else if (minuiteCategory == 5) {
                vm.scCat = true;
            }
            else if (minuiteCategory == 6) {
                vm.fpCat = true;
            }
        }
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
