(function () {
    'use strict';
    angular.module('app').service('toeOfficerStateEntryService', ['dataConstants', 'apiHttpService', toeOfficerStateEntryService]);

    function toeOfficerStateEntryService(dataConstants, apiHttpService) {
        var service = {
            getToeOfficerStateEntrys: getToeOfficerStateEntrys,
            getToeOfficerStateEntry: getToeOfficerStateEntry,
            saveToeOfficerStateEntry: saveToeOfficerStateEntry,
            updateToeOfficerStateEntry: updateToeOfficerStateEntry,
            deleteToeOfficerStateEntry: deleteToeOfficerStateEntry
        };

        return service;
        function getToeOfficerStateEntrys(pageSize, pageNumber,searchText) {
            var url = dataConstants.TOEOSENTRY_URL + 'get-toe-officer-state-entry-list?ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText;
            return apiHttpService.GET(url);
        }

        function getToeOfficerStateEntry(id) {
            var url = dataConstants.TOEOSENTRY_URL + 'get-toe-officer-state-entry?id=' + id;
            return apiHttpService.GET(url);
        }

        function saveToeOfficerStateEntry(data) {
            var url = dataConstants.TOEOSENTRY_URL + 'save-toe-officer-state-entry';
            return apiHttpService.POST(url, data);
        }

        function updateToeOfficerStateEntry(id, data) {
            var url = dataConstants.TOEOSENTRY_URL + 'update-toe-officer-state-entry/' + id;
            return apiHttpService.PUT(url, data);
        }

        function deleteToeOfficerStateEntry(id) {
            var url = dataConstants.TOEOSENTRY_URL + 'delete-toe-officer-state-entry/' + id;
            return apiHttpService.DELETE(url);
        }
        
    }
})();