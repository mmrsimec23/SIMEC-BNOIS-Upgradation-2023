(function () {
    'use strict';
    angular.module('app').service('minuiteService', ['dataConstants', 'apiHttpService', minuiteService]);

    function minuiteService(dataConstants, apiHttpService) {
        var service = {
            getMinuites: getMinuites,
            getMinuite: getMinuite,
            saveMinuite: saveMinuite,
            updateMinuite: updateMinuite,
            deleteMinuite: deleteMinuite
        };

        return service;
        function getMinuites(pageSize, pageNumber, searchText) {
            var url = dataConstants.MINUITE_URL + 'get-minuites?ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText;
            return apiHttpService.GET(url);
        }

        function getMinuite(id) {
            var url = dataConstants.MINUITE_URL + 'get-minuite?id=' + id;
            return apiHttpService.GET(url);
        }

        function saveMinuite(data) {
            var url = dataConstants.MINUITE_URL + 'save-minuite';
            return apiHttpService.POST(url, data);
        }

        function updateMinuite(id, data) {
            var url = dataConstants.MINUITE_URL + 'update-minuite/' + id;
            return apiHttpService.PUT(url, data);
        }

        function deleteMinuite(id) {
            var url = dataConstants.MINUITE_URL + 'delete-minuite/' + id;
            return apiHttpService.DELETE(url);
        }
    }
})();