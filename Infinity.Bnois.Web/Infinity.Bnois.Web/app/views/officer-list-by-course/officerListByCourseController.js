(function () {

    'use strict';
    var controllerId = 'officerListByCourseController';
    angular.module('app').controller(controllerId, officerListByCourseController);
    officerListByCourseController.$inject = ['$stateParams', '$state','codeValue','$scope','$rootScope','officeService','notificationService'];

    function officerListByCourseController($stateParams, $state, codeValue, $scope, $rootScope, officeService, notificationService) {
        /* jshint validthis:true */
        var vm = this;

        vm.url = codeValue.API_OFFICER_PICTURE_URL;
        
        vm.officersListByCourse = [];
        vm.back = back;
        vm.officeCurrentStatus = officeCurrentStatus;

        if ($stateParams.officeName !== null) {
            vm.officeName = $stateParams.officeName;
           
        }

        if ($stateParams.coursePlanId > 0 && $stateParams.coursePlanId !== null) {
            vm.coursePlanId = $stateParams.coursePlanId;

        }


        init();

        function init() {
            officeService.getOfficerListByCourse(vm.coursePlanId).then(function (data) {
                
                vm.officersListByCourse = data.result.officersListByCourse;
                console.log(vm.officersListByCourse)
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
