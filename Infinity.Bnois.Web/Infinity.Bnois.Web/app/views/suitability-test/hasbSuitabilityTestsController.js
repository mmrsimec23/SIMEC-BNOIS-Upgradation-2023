

(function () {

    'use strict';
    var controllerId = 'hasbSuitabilityTestsController';
    angular.module('app').controller(controllerId, hasbSuitabilityTestsController);
    hasbSuitabilityTestsController.$inject = ['$state', 'suitabilityTestService', 'notificationService', '$location'];

    function hasbSuitabilityTestsController($state, suitabilityTestService, notificationService, location) {

        /* jshint validthis:true */
        var vm = this;
        vm.suitabilityTests = [];
        vm.courseTypes = [];
        vm.addsuitabilityTest = addsuitabilityTest;
        vm.updatesuitabilityTest = updatesuitabilityTest;
        vm.deletesuitabilityTest = deletesuitabilityTest;
        vm.getListByCourseType = getListByCourseType;
        vm.getDataList = getDataList;
        vm.pageChanged = pageChanged;
        vm.searchText = "";
        vm.onSearch = onSearch;
        vm.pageSize = 50;
        vm.pageNumber = 1;
        vm.total = 0;
        vm.type = 0;

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
            getDataList();
            //getCourseTypeList();
        }

        function getDataList() {
            suitabilityTestService.GetSuitabilityTestListByType(1).then(function (data) {
                vm.suitabilityTests = data.result;
                console.log(vm.suitabilityTests)
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }
        //function getCourseTypeList() {
        //    suitabilityTestService.getCourseTypeList().then(function (data) {
        //        vm.courseTypes = data.result;
        //    });
        //}
        function getListByCourseType() {
            getDataList();
        }
        function addsuitabilityTest() {
            $state.go('suitability-test-officer-create', { type: 1 });
        }

        function updatesuitabilityTest(suitabilityTest) {
            $state.go('suitability-test-modify', { id: suitabilityTest.suitabilityTestId });
        }

        function deletesuitabilityTest(suitabilityTest) {
            suitabilityTestService.deletesuitabilityTest(suitabilityTest.suitabilityTestId).then(function (data) {
                $state.go($state.current, { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
            });
        }

        function pageChanged() {
            $state.go('suitability-tests', { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
        }

        function onSearch() {
            vm.pageNumber = 1;
            pageChanged();
        }
    }

})();

