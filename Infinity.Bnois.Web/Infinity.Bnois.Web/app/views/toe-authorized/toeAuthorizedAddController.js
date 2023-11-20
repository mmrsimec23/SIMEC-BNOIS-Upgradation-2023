(function () {

    'use strict';

    var controllerId = 'toeAuthorizedAddController';

    angular.module('app').controller(controllerId, toeAuthorizedAddController);
    toeAuthorizedAddController.$inject = ['$stateParams', 'toeAuthorizedService', 'notificationService', '$state'];

    function toeAuthorizedAddController($stateParams, toeAuthorizedService, notificationService, $state) {
        var vm = this;
        vm.toeAuthorizedid = 0;
        vm.title = 'ADD MODE';
        vm.toeAuthorized = {};
        vm.saveButtonText = 'Save';
        vm.save = save;
        vm.close = close;
        vm.toeAuthorizedForm = {};

        if ($stateParams.id !== undefined && $stateParams.id !== null) {
            vm.toeAuthorizedid = $stateParams.id;
            vm.saveButtonText = 'Update';
            vm.title = 'UPDATE MODE';
        }

        Init();
        function Init() {
            toeAuthorizedService.getToeAuthorized(vm.toeAuthorizedid).then(function (data) {
                vm.toeAuthorized = data.result.toeAuthorized;
                vm.branchList = data.result.branchList;
                vm.rankList = data.result.rankList;
                vm.officeList = data.result.officeList;
                //if (vm.toeAuthorized.toeAuthorizedid<=0) {
                //    vm.toeAuthorized.isConfirm = true;
                //}
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        };

        function save() {
            if (vm.toeAuthorizedid !== 0 && vm.toeAuthorizedid !== '') {
                updateToeAuthorized();
            } else {
                insertToeAuthorized();
            }
        }

        function insertToeAuthorized() {
            toeAuthorizedService.saveToeAuthorized(vm.toeAuthorized).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }
        function updateToeAuthorized() {
            toeAuthorizedService.updateToeAuthorized(vm.toeAuthorizedid, vm.toeAuthorized).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function close() {
            $state.go('toe-authorizeds');
        }
    }
})();
