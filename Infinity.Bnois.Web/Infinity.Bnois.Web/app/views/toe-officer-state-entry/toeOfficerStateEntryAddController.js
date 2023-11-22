(function () {

    'use strict';

    var controllerId = 'toeOfficerStateEntryAddController';

    angular.module('app').controller(controllerId, toeOfficerStateEntryAddController);
    toeOfficerStateEntryAddController.$inject = ['$stateParams', 'toeOfficerStateEntryService', 'notificationService', '$state'];

    function toeOfficerStateEntryAddController($stateParams, toeOfficerStateEntryService, notificationService, $state) {
        var vm = this;
        vm.id = 0;
        vm.title = 'ADD MODE';
        vm.toeOfficerStateEntry = {};
        vm.saveButtonText = 'Save';
        vm.save = save;
        vm.close = close;
        vm.toeOfficerStateEntryForm = {};

        if ($stateParams.id !== undefined && $stateParams.id !== null) {
            vm.id = $stateParams.id;
            vm.saveButtonText = 'Update';
            vm.title = 'UPDATE MODE';
        }
        vm.transferTypes = [
            { 'value': 1, 'text': 'Inside BN' }, { 'value': 2, 'text': 'In BN' }
        ];
        Init();
        function Init() {
            toeOfficerStateEntryService.getToeOfficerStateEntry(vm.id).then(function (data) {
                vm.toeOfficerStateEntry = data.result.dashBoardBranchByAdminAuthority700;
                vm.rankList = data.result.rankList;
                vm.branchList = data.result.branchList;
                
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        };

        function save() {
            if (vm.id !== 0 && vm.id !== '') {
                updateToeOfficerStateEntry();
            } else {
                insertToeOfficerStateEntry();
            }
        }

        function insertToeOfficerStateEntry() {
            toeOfficerStateEntryService.saveToeOfficerStateEntry(vm.toeOfficerStateEntry).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }
        function updateToeOfficerStateEntry() {
            toeOfficerStateEntryService.updateToeOfficerStateEntry(vm.id, vm.toeOfficerStateEntry).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function close() {
            $state.go('toe-officer-state-entry-list');
        }
    }
})();
