(function () {
    'use strict';
    angular.module('app').service('mscEducationTypeService', ['dataConstants', 'apiHttpService', mscEducationTypeService]);

    function mscEducationTypeService(dataConstants, apiHttpService) {
        var service = {
            getMscEducationTypeList: getMscEducationTypeList,
            getMscEducationType: getMscEducationType,
            saveMscEducationType: saveMscEducationType,
            updateMscEducationType: updateMscEducationType,
            deleteMscEducationType: deleteMscEducationType
        };

        return service;
        function getMscEducationTypeList(pageSize, pageNumber, searchText) {
            var url = dataConstants.MSC_EDUCATION_TYPE_URL + 'get-msc-education-type-list?ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText;
            return apiHttpService.GET(url);
        }

        function getMscEducationType(id) {
            var url = dataConstants.MSC_EDUCATION_TYPE_URL + 'get-msc-education-type?id=' + id;
            return apiHttpService.GET(url);
        }

        function saveMscEducationType(data) {
            var url = dataConstants.MSC_EDUCATION_TYPE_URL + 'save-msc-education-type';
            return apiHttpService.POST(url, data);
        }

        function updateMscEducationType(id, data) {
            var url = dataConstants.MSC_EDUCATION_TYPE_URL + 'update-msc-education-type/' + id;
            return apiHttpService.PUT(url, data);
        }

        function deleteMscEducationType(id) {
            var url = dataConstants.MSC_EDUCATION_TYPE_URL + 'delete-msc-education-type/' + id;
            return apiHttpService.DELETE(url);
        }
    }
})();