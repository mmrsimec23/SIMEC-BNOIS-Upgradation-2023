(function () {
    'use strict';
    angular.module('app').service('coFfRecomService', ['dataConstants', 'apiHttpService', coFfRecomService]);

    function coFfRecomService(dataConstants, apiHttpService) {
        var service = {
            getCoFfRecoms: getCoFfRecoms,
            saveCoFfRecom: saveCoFfRecom,
            deleteCoFfRecom: deleteCoFfRecom
        };

        return service;
        function getCoFfRecoms() {
            var url = dataConstants.CO_FF_RECOM_URL + 'get-co-ff-recoms/';
            return apiHttpService.GET(url);
        }
        

        function saveCoFfRecom(id,data) {
            var url = dataConstants.CO_FF_RECOM_URL + 'save-co-ff-recom/' + id;
            return apiHttpService.POST(url, data);
        }

        function deleteCoFfRecom(id) {
            var url = dataConstants.CO_FF_RECOM_URL + 'delete-co-ff-recom/' + id;
            return apiHttpService.DELETE(url);
        }
    }
})();