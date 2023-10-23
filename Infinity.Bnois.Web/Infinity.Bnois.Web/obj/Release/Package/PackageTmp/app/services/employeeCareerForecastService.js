(function () {
    'use strict';
    angular.module('app').service('employeeCareerForecastService', ['dataConstants', 'apiHttpService', employeeCareerForecastService]);

    function employeeCareerForecastService(dataConstants, apiHttpService) {
        var service = {
            getEmployeeCareerForecasts: getEmployeeCareerForecasts,
            getEmployeeCareerForecast: getEmployeeCareerForecast,
            getCareerForecastByEmployee: getCareerForecastByEmployee,
            saveEmployeeCareerForecast: saveEmployeeCareerForecast,
            saveCareerForecastByEmployee: saveCareerForecastByEmployee,
            updateEmployeeCareerForecast: updateEmployeeCareerForecast,
            deleteEmployeeCareerForecast: deleteEmployeeCareerForecast,
            getEmployeeCareerForecastsByEmployee: getEmployeeCareerForecastsByEmployee,
            getEmployeeCareerForecastSettingList: getEmployeeCareerForecastSettingList
        };

        return service;
        function getEmployeeCareerForecasts(pageSize, pageNumber, searchText) {
            var url = dataConstants.CAREER_FORECAST_URL + 'get-career-forecast-list?ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText;
            return apiHttpService.GET(url);
        }

        function getEmployeeCareerForecast(id) {
            var url = dataConstants.CAREER_FORECAST_URL + 'get-career-forecast?id=' + id;
            return apiHttpService.GET(url);
        }

        function getCareerForecastByEmployee(employeeId, careerForecastId) {
            var url = dataConstants.CAREER_FORECAST_URL + 'get-career-forecast-by-employee?employeeId=' + employeeId + '&careerForecastId=' + careerForecastId;
            return apiHttpService.GET(url);
        }

        function getEmployeeCareerForecastsByEmployee(employeeId) {
            var url = dataConstants.CAREER_FORECAST_URL + 'get-career-forecasts-by-employee?employeeId=' + employeeId;
            return apiHttpService.GET(url);
        }
        function getEmployeeCareerForecastSettingList(id) {
            var url = dataConstants.CAREER_FORECAST_URL + 'get-career-forecast-setting-list?id=' + id;
            return apiHttpService.GET(url);
        }

        function saveCareerForecastByEmployee(employeeId, data) {
            var url = dataConstants.CAREER_FORECAST_URL + 'save-career-forecast-by-employee/' + employeeId;
            return apiHttpService.POST(url, data);
        }

        function saveEmployeeCareerForecast(data) {
            var url = dataConstants.CAREER_FORECAST_URL + 'save-career-forecast';
            return apiHttpService.POST(url, data);
        }

        function updateEmployeeCareerForecast(id, data) {
            var url = dataConstants.CAREER_FORECAST_URL + 'update-career-forecast/' + id;
            return apiHttpService.PUT(url, data);
        }

        function deleteEmployeeCareerForecast(id) {
            var url = dataConstants.CAREER_FORECAST_URL + 'delete-career-forecast/' + id;
            return apiHttpService.DELETE(url);
        }
    }
})();