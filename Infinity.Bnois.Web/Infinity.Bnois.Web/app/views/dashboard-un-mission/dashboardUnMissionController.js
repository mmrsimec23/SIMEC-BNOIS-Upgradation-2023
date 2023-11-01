

(function () {

    'use strict';

    var controllerId = 'dashboardUnMissionController';

    angular.module('app').controller(controllerId, dashboardUnMissionController);
    dashboardUnMissionController.$inject = ['$stateParams','dashboardService' ,'officeService','notificationService', '$state'];

    function dashboardUnMissionController($stateParams, dashboardService, officeService,notificationService, $state) {
        var vm = this;
        
        vm.dashboardOutsideNavies = [];
        vm.ministryOffices = [];
        vm.getEmployeeListByOffice = getEmployeeListByOffice;
        vm.getOfficerList = getOfficerList;
        vm.officeId = -1;

        Init();
        function Init() {

            officeService.getMinistryOffices().then(function (data) {
                vm.ministryOffices = data.result;

            });
            getEmployeeListByOffice(null);
        }

        function getEmployeeListByOffice(officeId) {
            if (officeId == null) {
                officeId = -1;
            }

            dashboardService.getDashboardUnMission(officeId).then(function (data) {
                vm.dashboardOutsideNavies = data.result;

            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function getOfficerList(officeId, rankLevel) {
            $state.goNewTab('outside-navy-officer', { officeId: officeId, rankLevel: rankLevel, parentId: vm.officeId  });
            
        }
       
    }
})();
