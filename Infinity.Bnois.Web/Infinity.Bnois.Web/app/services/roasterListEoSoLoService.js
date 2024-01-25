(function () {
    'use strict';
    angular.module('app').service('roasterListEoSoLoService', ['dataConstants', 'apiHttpService', roasterListEoSoLoService]);

    function roasterListEoSoLoService(dataConstants, apiHttpService) {
        var service = {
            getRoasterListEoSoLoByShipType: getRoasterListEoSoLoByShipType,
            getLargeShipEosoloWaitingList: getLargeShipEosoloWaitingList,
            getLargeShipSeoDloWaitingList: getLargeShipSeoDloWaitingList,
            getMediumShipEosoloWaitingList: getMediumShipEosoloWaitingList,
            getLargeShipProposedWaitingCoXoList: getLargeShipProposedWaitingCoXoList

        };

        return service;

        function getRoasterListEoSoLoByShipType(shipType, coxoStatus) {
            var url = dataConstants.ROASTER_LIST_EOSOLO_URL + 'get-roaster-list-for-eoloso-by-ship-type?shipType=' + shipType + '&coxoStatus=' + coxoStatus;
            return apiHttpService.GET(url);
        }

        function getLargeShipProposedWaitingCoXoList(officeId, appointment) {
            var url = dataConstants.ROASTER_LIST_URL + 'get-proposed-waiting-coxo-list?officeId=' + officeId + '&appointment=' + appointment;
            return apiHttpService.GET(url);
        }
        function getLargeShipEosoloWaitingList() {
            var url = dataConstants.ROASTER_LIST_EOSOLO_URL + 'get-large-ship-eosolo-waiting-list';
            return apiHttpService.GET(url);
        }
        function getLargeShipSeoDloWaitingList() {
            var url = dataConstants.ROASTER_LIST_EOSOLO_URL + 'get-large-ship-seodlo-waiting-list';
            return apiHttpService.GET(url);
        }
        function getMediumShipEosoloWaitingList() {
            var url = dataConstants.ROASTER_LIST_EOSOLO_URL + 'get-medium-ship-eosolo-waiting-list';
            return apiHttpService.GET(url);
        }
    }
})();