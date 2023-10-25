(function () {
    'use strict';
    angular.module('app').service('bnoisLogInfoService', ['dataConstants', 'apiHttpService', bnoisLogInfoService]);

    function bnoisLogInfoService(dataConstants, apiHttpService) {
        var service = {
            getBnoisLogInfos: getBnoisLogInfos,
            getTableSelectModels: getTableSelectModels
           
        };

        return service;

        function getTableSelectModels() {
            var url = dataConstants.BNOIS_LOG_INFO_URL + 'get-bnois-log-select-models';
            return apiHttpService.GET(url);
        }
        function getBnoisLogInfos(tableName, logStatus, fromDate, toDate) {
            var url = dataConstants.BNOIS_LOG_INFO_URL + 'get-bnois-log-infos?tableName=' + tableName + '&logStatus=' + logStatus + '&fromDate=' + fromDate + '&toDate=' + toDate;
            return apiHttpService.GET(url);
        }



       

    }
})();