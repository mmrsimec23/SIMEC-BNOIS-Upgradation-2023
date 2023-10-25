(function () {

    'use strict';

    var controllerId = 'mscPermissionTypeAddController';

    angular.module('app').controller(controllerId, mscPermissionTypeAddController);
    mscPermissionTypeAddController.$inject = ['$stateParams', 'mscPermissionTypeService', 'notificationService', '$state'];

    function mscPermissionTypeAddController($stateParams, mscPermissionTypeService, notificationService, $state) {
        var vm = this;
        vm.mscPermissionTypeId = 0;
        vm.title = 'ADD MODE';
        vm.mscPermissionType = {};
        vm.saveButtonText = 'Save';
        vm.save = save
        vm.close = close;
        vm.mscPermissionTypeForm = {};

        if ($stateParams.id !== undefined && $stateParams.id !== null) {
            vm.mscPermissionTypeId = $stateParams.id;
            vm.saveButtonText = 'Update';
            vm.title = 'UPDATE MODE';
        }

        Init();
        function Init() {
            mscPermissionTypeService.getMscPermissionType(vm.mscPermissionTypeId).then(function (data) {
                vm.mscPermissionType = data.result;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        };

        function save() {
            if (vm.mscPermissionTypeId !== 0 && vm.mscPermissionTypeId !== '') {
                updateMscPermissionType();
            } else {
                insertMscPermissionType();
            }
        }

        function insertMscPermissionType() {
            mscPermissionTypeService.saveMscPermissionType(vm.mscPermissionType).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }
        function updateMscPermissionType() {
            mscPermissionTypeService.updateMscPermissionType(vm.mscPermissionTypeId, vm.mscPermissionType).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function close() {
            $state.go('msc-permission-type-list');
        }
    }
})();
