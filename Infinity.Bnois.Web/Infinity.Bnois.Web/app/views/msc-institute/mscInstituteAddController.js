(function () {

    'use strict';

    var controllerId = 'mscInstituteAddController';

    angular.module('app').controller(controllerId, mscInstituteAddController);
    mscInstituteAddController.$inject = ['$stateParams', 'mscInstituteService', 'notificationService', '$state'];

    function mscInstituteAddController($stateParams, mscInstituteService, notificationService, $state) {
        var vm = this;
        vm.mscInstituteId = 0;
        vm.title = 'ADD MODE';
        vm.mscInstitute = {};
        vm.saveButtonText = 'Save';
        vm.save = save
        vm.close = close;
        vm.mscInstituteForm = {};

        if ($stateParams.id !== undefined && $stateParams.id !== null) {
            vm.mscInstituteId = $stateParams.id;
            vm.saveButtonText = 'Update';
            vm.title = 'UPDATE MODE';
        }

        Init();
        function Init() {
            mscInstituteService.getMscInstitute(vm.mscInstituteId).then(function (data) {
                vm.mscInstitute = data.result;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        };

        function save() {
            if (vm.mscInstituteId !== 0 && vm.mscInstituteId !== '') {
                updateMscInstitute();
            } else {
                insertMscInstitute();
            }
        }

        function insertMscInstitute() {
            mscInstituteService.saveMscInstitute(vm.mscInstitute).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }
        function updateMscInstitute() {
            mscInstituteService.updateMscInstitute(vm.mscInstituteId, vm.mscInstitute).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function close() {
            $state.go('msc-institute-list');
        }
    }
})();
