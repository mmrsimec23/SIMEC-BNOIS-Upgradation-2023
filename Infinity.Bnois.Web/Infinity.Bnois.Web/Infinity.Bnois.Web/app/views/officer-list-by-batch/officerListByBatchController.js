(function () {

    'use strict';
    var controllerId = 'officerListByBatchController';
    angular.module('app').controller(controllerId, officerListByBatchController);
    officerListByBatchController.$inject = ['$stateParams', '$state','codeValue','$scope','$rootScope','officeService','notificationService'];

    function officerListByBatchController($stateParams, $state, codeValue, $scope, $rootScope, officeService, notificationService) {
        /* jshint validthis:true */
        var vm = this;

        vm.url = codeValue.API_OFFICER_PICTURE_URL;
        
        vm.officersListByBatch = [];
        vm.back = back;
        vm.officeCurrentStatus = officeCurrentStatus;

        if ($stateParams.officeName !== null) {
            vm.officeName = $stateParams.officeName;
           
        }

        if ($stateParams.batchId > 0 && $stateParams.batchId !== null) {
            vm.batchId = $stateParams.batchId;

        }


        init();

        function init() {
            officeService.getOfficerListByBatch(vm.batchId).then(function (data) {
                
                vm.officersListByBatch = data.result.officersListByBatch;
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
