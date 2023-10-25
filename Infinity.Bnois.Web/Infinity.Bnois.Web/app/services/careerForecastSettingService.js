(function () {
    'use strict';
    angular.module('app').service('careerForecastSettingService', ['dataConstants', 'apiHttpService', careerForecastSettingService]);

    function careerForecastSettingService(dataConstants, apiHttpService) {
        var service = {
            getCareerForecastSettings: getCareerForecastSettings,
            getCareerForecastSetting: getCareerForecastSetting,
            saveCareerForecastSetting: saveCareerForecastSetting,
            updateCareerForecastSetting: updateCareerForecastSetting,
            deleteCareerForecastSetting: deleteCareerForecastSetting
        };

        return service;
        function getCareerForecastSettings(pageSize, pageNumber, searchText) {
            var url = dataConstants.CAREER_FORECAST_SETTING_URL + 'get-career-forecast-setting-list?ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText;
            return apiHttpService.GET(url);
        }

        function getCareerForecastSetting(id) {
            var url = dataConstants.CAREER_FORECAST_SETTING_URL + 'get-career-forecast-setting?id=' + id;
            return apiHttpService.GET(url);
        }

        function saveCareerForecastSetting(data) {
            var url = dataConstants.CAREER_FORECAST_SETTING_URL + 'save-career-forecast-setting';
            return apiHttpService.POST(url, data);
        }

        function updateCareerForecastSetting(id, data) {
            var url = dataConstants.CAREER_FORECAST_SETTING_URL + 'update-career-forecast-setting/' + id;
            return apiHttpService.PUT(url, data);
        }

        function deleteCareerForecastSetting(id) {
            var url = dataConstants.CAREER_FORECAST_SETTING_URL + 'delete-career-forecast-setting/' + id;
            return apiHttpService.DELETE(url);
        }
        
    }
})();