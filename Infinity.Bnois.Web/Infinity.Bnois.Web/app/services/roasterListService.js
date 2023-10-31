(function () {
    'use strict';
    angular.module('app').service('roasterListService', ['dataConstants', 'apiHttpService', roasterListService]);

    function roasterListService(dataConstants, apiHttpService) {
        var service = {
            getRoasterListByShipType: getRoasterListByShipType,
            getLargeShipProposedWaitingCoXoList: getLargeShipProposedWaitingCoXoList

        };

        return service;

        function getRoasterListByShipType(shipType, coxoStatus) {
            var url = dataConstants.ROASTER_LIST_URL + 'get-roaster-list-by-ship-type?shipType=' + shipType + '&coxoStatus=' + coxoStatus;
            return apiHttpService.GET(url);
        }

        function getLargeShipProposedWaitingCoXoList(officeId, appointment) {
            var url = dataConstants.ROASTER_LIST_URL + 'get-proposed-waiting-coxo-list?officeId=' + officeId + '&appointment=' + appointment;
            return apiHttpService.GET(url);
        }
    }
})();