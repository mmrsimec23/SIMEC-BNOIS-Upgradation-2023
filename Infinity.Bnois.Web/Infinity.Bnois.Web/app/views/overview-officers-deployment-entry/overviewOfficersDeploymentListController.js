(function () {

    'use strict';
    var controllerId = 'overviewOfficersDeploymentListController';
    angular.module('app').controller(controllerId, overviewOfficersDeploymentListController);
    overviewOfficersDeploymentListController.$inject = ['$state', 'overviewOfficersDeploymentService', 'notificationService', '$location'];

    function overviewOfficersDeploymentListController($state, overviewOfficersDeploymentService, notificationService, location) {

        /* jshint validthis:true */
        var vm = this;
        vm.overviewOfficersDeployments = [];
        vm.permission = {};
        vm.addOverviewOfficersDeployment = addOverviewOfficersDeployment;
        vm.updateOverviewOfficersDeployment = updateOverviewOfficersDeployment;
        vm.deleteOverviewOfficersDeployment = deleteOverviewOfficersDeployment;
        vm.pageChanged = pageChanged;
        vm.searchText = "";
        vm.onSearch = onSearch;
        vm.pageSize = 50;
        vm.pageNumber = 1;
        vm.total = 0;

        if (location.search().ps !== undefined && location.search().ps !== null && location.search().ps !== '') {
            vm.pageSize = location.search().ps;
        }

        if (location.search().pn !== undefined && location.search().pn !== null && location.search().pn !== '') {
            vm.pageNumber = location.search().pn;
        }
        if (location.search().q !== undefined && location.search().q !== null && location.search().q !== '') {
            vm.searchText = location.search().q;
        }
        init();
        function init() {
            overviewOfficersDeploymentService.getOverviewOfficersDeployments(vm.pageSize, vm.pageNumber, vm.searchText).then(function (data) {
                vm.overviewOfficersDeployments = data.result;
                vm.total = data.total; vm.permission = data.permission;
                vm.permission = data.permission;
               
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function addOverviewOfficersDeployment() {
            $state.go('overview-officers-deployment-entry-create');
        }

        function updateOverviewOfficersDeployment(overviewOfficersDeployment) {
            $state.go('overview-officers-deployment-entry-modify', { id: overviewOfficersDeployment.id});
        }

        function deleteOverviewOfficersDeployment(overviewOfficersDeployment) {
            overviewOfficersDeploymentService.deleteOverviewOfficersDeployment(overviewOfficersDeployment.id).then(function (data) {
                $state.go($state.current, { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
            });
        }

        function pageChanged() {
            $state.go('overview-officers-deployment-entry-list', { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
          
        }

        function onSearch() {
            vm.pageNumber = 1;
            pageChanged();
        }
    }

})();
