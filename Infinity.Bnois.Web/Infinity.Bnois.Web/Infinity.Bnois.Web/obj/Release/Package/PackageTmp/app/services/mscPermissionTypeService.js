(function () {
    'use strict';
    angular.module('app').service('mscPermissionTypeService', ['dataConstants', 'apiHttpService', mscPermissionTypeService]);

    function mscPermissionTypeService(dataConstants, apiHttpService) {
        var service = {
            getMscPermissionTypeList: getMscPermissionTypeList,
            getMscPermissionType: getMscPermissionType,
            saveMscPermissionType: saveMscPermissionType,
            updateMscPermissionType: updateMscPermissionType,
            deleteMscPermissionType: deleteMscPermissionType
        };

        return service;
        function getMscPermissionTypeList(pageSize, pageNumber, searchText) {
            var url = dataConstants.MSC_PERMISSION_TYPE_URL + 'get-msc-permission-type-list?ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText;
            return apiHttpService.GET(url);
        }

        function getMscPermissionType(id) {
            var url = dataConstants.MSC_PERMISSION_TYPE_URL + 'get-msc-permission-type?id=' + id;
            return apiHttpService.GET(url);
        }

        function saveMscPermissionType(data) {
            var url = dataConstants.MSC_PERMISSION_TYPE_URL + 'save-msc-permission-type';
            return apiHttpService.POST(url, data);
        }

        function updateMscPermissionType(id, data) {
            var url = dataConstants.MSC_PERMISSION_TYPE_URL + 'update-msc-permission-type/' + id;
            return apiHttpService.PUT(url, data);
        }

        function deleteMscPermissionType(id) {
            var url = dataConstants.MSC_PERMISSION_TYPE_URL + 'delete-msc-permission-type/' + id;
            return apiHttpService.DELETE(url);
        }
    }
})();