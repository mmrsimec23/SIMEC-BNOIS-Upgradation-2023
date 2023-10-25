/// <reference path="../../services/mscInstituteService.js" />

(function () {

    'use strict';
    var controllerId = 'mscInstituteListController';
    angular.module('app').controller(controllerId, mscInstituteListController);
    mscInstituteListController.$inject = ['$state', 'mscInstituteService', 'notificationService', '$location'];

    function mscInstituteListController($state, mscInstituteService, notificationService, location) {

        /* jshint validthis:true */
        var vm = this;
        vm.mscInstituteList = [];
        vm.addMscInstitute = addMscInstitute;
        vm.updateMscInstitute = updateMscInstitute;
        vm.deleteMscInstitute = deleteMscInstitute;
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
            mscInstituteService.getMscInstituteList(vm.pageSize, vm.pageNumber, vm.searchText).then(function (data) {
                vm.mscInstituteList = data.result;
                vm.total = data.total; vm.permission = data.permission;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function addMscInstitute() {
            $state.go('msc-institute-create');
        }

        function updateMscInstitute(mscInstitute) {
            $state.go('msc-institute-modify', { id: mscInstitute.mscInstituteId });
        }

        function deleteMscInstitute(mscInstitute) {
            mscInstituteService.deleteMscInstitute(mscInstitute.mscInstituteId).then(function (data) {
                $state.go($state.current, { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
            });
        }

        function pageChanged() {
            $state.go('msc-institute-list', { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
        }

        function onSearch() {
            vm.pageNumber = 1;
            pageChanged();
        }
    }

})();
