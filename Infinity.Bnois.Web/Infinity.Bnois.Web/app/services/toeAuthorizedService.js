(function () {
    'use strict';
    angular.module('app').service('toeAuthorizedService', ['dataConstants', 'apiHttpService', toeAuthorizedService]);

    function toeAuthorizedService(dataConstants, apiHttpService) {
        var service = {
            getToeAuthorizeds: getToeAuthorizeds,
            getToeAuthorized: getToeAuthorized,
            saveToeAuthorized: saveToeAuthorized,
            updateToeAuthorized: updateToeAuthorized,
            deleteToeAuthorized: deleteToeAuthorized
        };

        return service;
        function getToeAuthorizeds(pageSize, pageNumber,searchText) {
            var url = dataConstants.TOE_AUTHORIZED_URL + 'get-toe-authorizeds?ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText;
            return apiHttpService.GET(url);
        }

        function getToeAuthorized(id) {
            var url = dataConstants.TOE_AUTHORIZED_URL + 'get-toe-authorized?id=' + id;
            return apiHttpService.GET(url);
        }

        function saveToeAuthorized(data) {
            var url = dataConstants.TOE_AUTHORIZED_URL + 'save-toe-authorized';
            return apiHttpService.POST(url, data);
        }

        function updateToeAuthorized(id, data) {
            var url = dataConstants.TOE_AUTHORIZED_URL + 'update-toe-authorized/' + id;
            return apiHttpService.PUT(url, data);
        }

        function deleteToeAuthorized(id) {
            var url = dataConstants.TOE_AUTHORIZED_URL + 'delete-toe-authorized/' + id;
            return apiHttpService.DELETE(url);
        }
        
    }
})();