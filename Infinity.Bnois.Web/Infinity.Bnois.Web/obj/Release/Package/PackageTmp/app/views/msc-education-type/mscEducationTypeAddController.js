(function () {

    'use strict';

    var controllerId = 'mscEducationTypeAddController';

    angular.module('app').controller(controllerId, mscEducationTypeAddController);
    mscEducationTypeAddController.$inject = ['$stateParams', 'mscEducationTypeService', 'notificationService', '$state'];

    function mscEducationTypeAddController($stateParams, mscEducationTypeService, notificationService, $state) {
        var vm = this;
        vm.mscEducationTypeId = 0;
        vm.title = 'ADD MODE';
        vm.mscEducationType = {};
        vm.saveButtonText = 'Save';
        vm.save = save
        vm.close = close;
        vm.mscEducationTypeForm = {};

        if ($stateParams.id !== undefined && $stateParams.id !== null) {
            vm.mscEducationTypeId = $stateParams.id;
            vm.saveButtonText = 'Update';
            vm.title = 'UPDATE MODE';
        }

        Init();
        function Init() {
            mscEducationTypeService.getMscEducationType(vm.mscEducationTypeId).then(function (data) {
                vm.mscEducationType = data.result;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        };

        function save() {
            if (vm.mscEducationTypeId !== 0 && vm.mscEducationTypeId !== '') {
                updateMscEducationType();
            } else {
                insertMscEducationType();
            }
        }

        function insertMscEducationType() {
            mscEducationTypeService.saveMscEducationType(vm.mscEducationType).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }
        function updateMscEducationType() {
            mscEducationTypeService.updateMscEducationType(vm.mscEducationTypeId, vm.mscEducationType).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function close() {
            $state.go('msc-education-type-list');
        }
    }
})();
