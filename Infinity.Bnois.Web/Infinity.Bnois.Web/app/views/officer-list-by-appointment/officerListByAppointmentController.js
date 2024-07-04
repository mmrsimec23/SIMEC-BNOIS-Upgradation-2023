(function () {

    'use strict';
    var controllerId = 'officerListByAppointmentController';
    angular.module('app').controller(controllerId, officerListByAppointmentController);
    officerListByAppointmentController.$inject = ['$stateParams', '$state','codeValue','$scope','$rootScope','officeService','notificationService'];

    function officerListByAppointmentController($stateParams, $state, codeValue, $scope, $rootScope, officeService, notificationService) {
        /* jshint validthis:true */
        var vm = this;

        vm.url = codeValue.API_OFFICER_PICTURE_URL;

        vm.officersListByAppt = [];
        vm.back = back;
        vm.officeCurrentStatus = officeCurrentStatus;

        if ($stateParams.officeName !== null) {
            vm.officeName = $stateParams.officeName;
           
        }

        if ($stateParams.appointmentId > 0 && $stateParams.appointmentId !== null) {
            vm.appointmentId = $stateParams.appointmentId;

        }


        init();

        function init() {
            officeService.getOfficerListByAppointment(vm.appointmentId).then(function (data) {
                vm.officersListByAppt = data.result.officersListByAppt;
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
