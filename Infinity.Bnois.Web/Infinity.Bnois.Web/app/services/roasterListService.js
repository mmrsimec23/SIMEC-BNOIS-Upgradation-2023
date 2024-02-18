(function () {
    'use strict';
    angular.module('app').service('roasterListService', ['dataConstants', 'apiHttpService', roasterListService]);

    function roasterListService(dataConstants, apiHttpService) {
        var service = {
            getRoasterListByShipType: getRoasterListByShipType,
            getRoasterListBySmallShip: getRoasterListBySmallShip,
            getLargeShipCoWaitingList: getLargeShipCoWaitingList,
            getLargeShipXoWaitingList: getLargeShipXoWaitingList,
            getMediumShipCoWaitingList: getMediumShipCoWaitingList,
            getSmallShipCoXoWaitingList: getSmallShipCoXoWaitingList,
            getLargeShipProposedWaitingCoXoList: getLargeShipProposedWaitingCoXoList

        };

        return service;

        function getRoasterListByShipType(shipType, coxoStatus) {
            var url = dataConstants.ROASTER_LIST_URL + 'get-roaster-list-by-ship-type?shipType=' + shipType + '&coxoStatus=' + coxoStatus;
            return apiHttpService.GET(url);
        }
        function getRoasterListBySmallShip(shipType, coxoStatus, viewStatus) {
            var url = dataConstants.ROASTER_LIST_URL + 'get-roaster-list-by-small-ship?shipType=' + shipType + '&coxoStatus=' + coxoStatus+ '&viewStatus=' + viewStatus;
            return apiHttpService.GET(url);
        }

        function getLargeShipProposedWaitingCoXoList(shipType, officeId, appointment) {
            var url = dataConstants.ROASTER_LIST_URL + 'get-proposed-waiting-coxo-list?shipType=' + shipType + '&officeId=' + officeId + '&appointment=' + appointment;
            return apiHttpService.GET(url);
        }
        function getLargeShipCoWaitingList() {
            var url = dataConstants.ROASTER_LIST_URL + 'get-large-ship-co-waiting-list';
            return apiHttpService.GET(url);
        }
        function getLargeShipXoWaitingList() {
            var url = dataConstants.ROASTER_LIST_URL + 'get-large-ship-xo-waiting-list';
            return apiHttpService.GET(url);
        }
        function getMediumShipCoWaitingList() {
            var url = dataConstants.ROASTER_LIST_URL + 'get-medium-ship-co-waiting-list';
            return apiHttpService.GET(url);
        }
        function getSmallShipCoXoWaitingList() {
            var url = dataConstants.ROASTER_LIST_URL + 'get-small-ship-coxo-waiting-list';
            return apiHttpService.GET(url);
        }
    }
})();