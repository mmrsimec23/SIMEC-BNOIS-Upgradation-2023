

(function () {

    'use strict';
    var controllerId = 'majorCourseForecastsController';
    angular.module('app').controller(controllerId, majorCourseForecastsController);
    majorCourseForecastsController.$inject = ['$state', 'majorCourseForecastService', 'notificationService', '$location'];

    function majorCourseForecastsController($state, majorCourseForecastService, notificationService, location) {

        /* jshint validthis:true */
        var vm = this;
        vm.majorCourseForecasts = [];
        vm.courseTypes = [];
        vm.addmajorCourseForecast = addmajorCourseForecast;
        vm.updatemajorCourseForecast = updatemajorCourseForecast;
        vm.deletemajorCourseForecast = deletemajorCourseForecast;
        vm.getCourseTypeList = getCourseTypeList;
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
            getCourseTypeList();
        }

        function getDataList() {
            majorCourseForecastService.getMajorCourseForecasts(vm.type, vm.pageSize, vm.pageNumber, vm.searchText).then(function (data) {
                vm.majorCourseForecasts = data.result;
                vm.total = data.total; vm.permission = data.permission;
                console.log(vm.majorCourseForecasts)
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }
        function getCourseTypeList() {
            majorCourseForecastService.getCourseTypeList().then(function (data) {
                vm.courseTypes = data.result;
            });
        }
        function getListByCourseType() {
            getDataList();
        }
        function addmajorCourseForecast() {
            $state.go('major-course-forecast-create');
        }

        function updatemajorCourseForecast(majorCourseForecast) {
            $state.go('major-course-forecast-modify', { id: majorCourseForecast.majorCourseForecastId });
        }

        function deletemajorCourseForecast(majorCourseForecast) {
            majorCourseForecastService.deleteMajorCourseForecast(majorCourseForecast.majorCourseForecastId).then(function (data) {
                $state.go($state.current, { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
            });
        }

        function pageChanged() {
            $state.go('major-course-forecasts', { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
        }

        function onSearch() {
            vm.pageNumber = 1;
            pageChanged();
        }
    }

})();

