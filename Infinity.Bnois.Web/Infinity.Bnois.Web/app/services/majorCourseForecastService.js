(function () {
    'use strict';
    angular.module('app').service('majorCourseForecastService', ['dataConstants', 'apiHttpService', majorCourseForecastService]);

    function majorCourseForecastService(dataConstants, apiHttpService) {
        var service = {
            getMajorCourseForecasts: getMajorCourseForecasts,
            getMajorCourseForecast: getMajorCourseForecast,
            saveMajorCourseForecast: saveMajorCourseForecast,
            updateMajorCourseForecast: updateMajorCourseForecast,
            getCourseTypeList: getCourseTypeList,
            deleteMajorCourseForecast: deleteMajorCourseForecast
        };

        return service;
        function getMajorCourseForecasts(type, pageSize, pageNumber, searchText) {
            var url = dataConstants.MAJOR_COURSE_FORECAST_URL + 'get-major-course-forecasts?type=' + type + '&ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText;
            return apiHttpService.GET(url);
        }

        function getMajorCourseForecast(majorCourseForecastId) {
            var url = dataConstants.MAJOR_COURSE_FORECAST_URL + 'get-major-course-forecast?id=' + majorCourseForecastId;
            return apiHttpService.GET(url);
        }

        function saveMajorCourseForecast(data) {
            var url = dataConstants.MAJOR_COURSE_FORECAST_URL + 'save-major-course-forecast';
            return apiHttpService.POST(url, data);
        }

        function updateMajorCourseForecast(majorCourseForecastId, data) {
            var url = dataConstants.MAJOR_COURSE_FORECAST_URL + 'update-major-course-forecast/' + majorCourseForecastId;
            return apiHttpService.PUT(url, data);
        }

        function deleteMajorCourseForecast(majorCourseForecastId) {
            var url = dataConstants.MAJOR_COURSE_FORECAST_URL + 'delete-major-course-forecast/' + majorCourseForecastId;
            return apiHttpService.DELETE(url);
        }

        function getCourseTypeList() {
            var url = dataConstants.MAJOR_COURSE_FORECAST_URL + 'get-major-course-type-list';
            return apiHttpService.GET(url);
        }

    }
})();