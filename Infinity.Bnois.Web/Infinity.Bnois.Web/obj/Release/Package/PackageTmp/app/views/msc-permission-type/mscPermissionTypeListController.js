/// <reference path="../../services/mscPermissionTypeService.js" />

(function () {

    'use strict';
    var controllerId = 'mscPermissionTypeListController';
    angular.module('app').controller(controllerId, mscPermissionTypeListController);
    mscPermissionTypeListController.$inject = ['$state', 'mscPermissionTypeService', 'notificationService', '$location'];

    function mscPermissionTypeListController($state, mscPermissionTypeService, notificationService, location) {

        /* jshint validthis:true */
        var vm = this;
        vm.mscPermissionTypeList = [];
        vm.addMscPermissionType = addMscPermissionType;
        vm.updateMscPermissionType = updateMscPermissionType;
        vm.deleteMscPermissionType = deleteMscPermissionType;
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
            mscPermissionTypeService.getMscPermissionTypeList(vm.pageSize, vm.pageNumber, vm.searchText).then(function (data) {
                vm.mscPermissionTypeList = data.result;
                vm.total = data.total; vm.permission = data.permission;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function addMscPermissionType() {
            $state.go('msc-permission-type-create');
        }

        function updateMscPermissionType(mscPermissionType) {
            $state.go('msc-permission-type-modify', { id: mscPermissionType.mscPermissionTypeId });
        }

        function deleteMscPermissionType(mscPermissionType) {
            mscPermissionTypeService.deleteMscPermissionType(mscPermissionType.mscPermissionTypeId).then(function (data) {
                $state.go($state.current, { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
            });
        }

        function pageChanged() {
            $state.go('msc-permission-type-list', { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
        }

        function onSearch() {
            vm.pageNumber = 1;
            pageChanged();
        }
    }

})();
