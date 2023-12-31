﻿(function () {
    'use strict';
    angular.module('app').service('shipMovementService', ['dataConstants', 'apiHttpService', shipMovementService]);

    function shipMovementService(dataConstants, apiHttpService) {
        var service = {
            getShipMovementSelectModels: getShipMovementSelectModels,
            getShipMovementHistory: getShipMovementHistory,
            updateShipMovement: updateShipMovement
            
        };

        return service;
    

        function getShipMovementSelectModels() {
            var url = dataConstants.SHIP_MOVEMENT_URL + 'get-ship-movement-select-models';
            return apiHttpService.GET(url);
        }

        function getShipMovementHistory(shipId) {
            var url = dataConstants.SHIP_MOVEMENT_URL + 'get-ship-movement-history?shipId=' + shipId;
            return apiHttpService.GET(url);
        }

       
        function updateShipMovement(officeId, data) {
            var url = dataConstants.SHIP_MOVEMENT_URL + 'update-ship-movement/' + officeId;
            return apiHttpService.PUT(url, data);
        }

       
    }
})();