(function () {
    'use strict';
    angular.module('app').service('roasterListService', ['dataConstants', 'apiHttpService', roasterListService]);

    function roasterListService(dataConstants, apiHttpService) {
        var service = {
            getRoasterListByShipType: getRoasterListByShipType

        };

        return service;

        function getRoasterListByShipType(shipType) {
            var url = dataConstants.ROASTER_LIST_URL + 'get-roaster-list-by-ship-type?shipType=' + shipType;
            return apiHttpService.GET(url);
        }

    }
})();