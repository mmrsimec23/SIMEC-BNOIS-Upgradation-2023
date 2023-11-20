(function () {

    'use strict';

    var controllerId = 'overviewOfficersDeploymentAddController';

    angular.module('app').controller(controllerId, overviewOfficersDeploymentAddController);
    overviewOfficersDeploymentAddController.$inject = ['$stateParams', 'overviewOfficersDeploymentService', 'notificationService', '$state'];

    function overviewOfficersDeploymentAddController($stateParams, overviewOfficersDeploymentService, notificationService, $state) {
        var vm = this;
        vm.id = 0;
        vm.title = 'ADD MODE';
        vm.overviewOfficersDeployment = {};
        vm.saveButtonText = 'Save';
        vm.save = save;
        vm.close = close;
        vm.overviewOfficersDeploymentForm = {};

        if ($stateParams.id !== undefined && $stateParams.id !== null) {
            vm.id = $stateParams.id;
            vm.saveButtonText = 'Update';
            vm.title = 'UPDATE MODE';
        }

        Init();
        function Init() {
            overviewOfficersDeploymentService.getOverviewOfficersDeployment(vm.id).then(function (data) {
                vm.overviewOfficersDeployment = data.result.dashBoardBranchByAdminAuthority600Entry;
                vm.rankList = data.result.rankList;
                vm.orgTypeList = data.result.orgTypeList;
                
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        };

        function save() {
            if (vm.id !== 0 && vm.id !== '') {
                updateoverviewOfficersDeployment();
            } else {
                insertoverviewOfficersDeployment();
            }
        }

        function insertoverviewOfficersDeployment() {
            overviewOfficersDeploymentService.saveOverviewOfficersDeployment(vm.overviewOfficersDeployment).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }
        function updateoverviewOfficersDeployment() {
            overviewOfficersDeploymentService.updateOverviewOfficersDeployment(vm.id, vm.overviewOfficersDeployment).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function close() {
            $state.go('overview-officers-deployment-entry-list');
        }
    }
})();
