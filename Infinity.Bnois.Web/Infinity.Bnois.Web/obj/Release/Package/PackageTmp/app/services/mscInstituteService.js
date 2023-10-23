(function () {
    'use strict';
    angular.module('app').service('mscInstituteService', ['dataConstants', 'apiHttpService', mscInstituteService]);

    function mscInstituteService(dataConstants, apiHttpService) {
        var service = {
            getMscInstituteList: getMscInstituteList,
            getMscInstitute: getMscInstitute,
            saveMscInstitute: saveMscInstitute,
            updateMscInstitute: updateMscInstitute,
            deleteMscInstitute: deleteMscInstitute
        };

        return service;
        function getMscInstituteList(pageSize, pageNumber, searchText) {
            var url = dataConstants.MSC_INSTITUTE_URL + 'get-msc-institute-list?ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText;
            return apiHttpService.GET(url);
        }

        function getMscInstitute(id) {
            var url = dataConstants.MSC_INSTITUTE_URL + 'get-msc-institute?id=' + id;
            return apiHttpService.GET(url);
        }

        function saveMscInstitute(data) {
            var url = dataConstants.MSC_INSTITUTE_URL + 'save-msc-institute';
            return apiHttpService.POST(url, data);
        }

        function updateMscInstitute(id, data) {
            var url = dataConstants.MSC_INSTITUTE_URL + 'update-msc-institute/' + id;
            return apiHttpService.PUT(url, data);
        }

        function deleteMscInstitute(id) {
            var url = dataConstants.MSC_INSTITUTE_URL + 'delete-msc-institute/' + id;
            return apiHttpService.DELETE(url);
        }
    }
})();