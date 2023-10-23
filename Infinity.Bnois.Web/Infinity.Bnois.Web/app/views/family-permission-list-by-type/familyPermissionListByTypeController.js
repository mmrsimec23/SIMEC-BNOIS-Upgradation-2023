(function () {

    'use strict';
    var controllerId = 'familyPermissionListByTypeController';
    angular.module('app').controller(controllerId, familyPermissionListByTypeController);
    familyPermissionListByTypeController.$inject = ['$stateParams', '$state', 'codeValue', '$scope', '$rootScope','currentStatusService','notificationService'];

    function familyPermissionListByTypeController($stateParams, $state, codeValue, $scope, $rootScope, currentStatusService, notificationService) {
        /* jshint validthis:true */
        var vm = this;

        vm.url = codeValue.API_OFFICER_PICTURE_URL;
        
        vm.familyPermissions = [];
        vm.back = back;
        vm.officeCurrentStatus = officeCurrentStatus;

        if ($stateParams.officeName !== null) {
            vm.officeName = $stateParams.officeName;
           
        }

        if ($stateParams.pno > 0 && $stateParams.pno !== null) {
            vm.pno = $stateParams.pno;

        }

        if ($stateParams.relationId > 0 && $stateParams.relationId !== null) {
            vm.relationId = $stateParams.relationId;

        }


        init();

        function init() {
            currentStatusService.GetFamilyPermissions(vm.pno, vm.relationId).then(function (data) {
                
                vm.familyPermissions = data.result;
                },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });


        }

        function back() {

            $state.go('office-structures');
        }

        function officeCurrentStatus(pNo) {

            $state.goNewTab('current-status-tab', { pno: pNo });
        }
    }
})();
