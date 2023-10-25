/// <reference path="../../services/mscEducationTypeService.js" />

(function () {

    'use strict';
    var controllerId = 'mscEducationTypeListController';
    angular.module('app').controller(controllerId, mscEducationTypeListController);
    mscEducationTypeListController.$inject = ['$state', 'mscEducationTypeService', 'notificationService', '$location'];

    function mscEducationTypeListController($state, mscEducationTypeService, notificationService, location) {

        /* jshint validthis:true */
        var vm = this;
        vm.mscEducationTypeList = [];
        vm.addMscEducationType = addMscEducationType;
        vm.updateMscEducationType = updateMscEducationType;
        vm.deleteMscEducationType = deleteMscEducationType;
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
            mscEducationTypeService.getMscEducationTypeList(vm.pageSize, vm.pageNumber, vm.searchText).then(function (data) {
                vm.mscEducationTypeList = data.result;
                vm.total = data.total; vm.permission = data.permission;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function addMscEducationType() {
            $state.go('msc-education-type-create');
        }

        function updateMscEducationType(mscEducationType) {
            $state.go('msc-education-type-modify', { id: mscEducationType.mscEducationTypeId });
        }

        function deleteMscEducationType(mscEducationType) {
            mscEducationTypeService.deleteMscEducationType(mscEducationType.mscEducationTypeId).then(function (data) {
                $state.go($state.current, { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
            });
        }

        function pageChanged() {
            $state.go('msc-education-type-list', { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
        }

        function onSearch() {
            vm.pageNumber = 1;
            pageChanged();
        }
    }

})();
