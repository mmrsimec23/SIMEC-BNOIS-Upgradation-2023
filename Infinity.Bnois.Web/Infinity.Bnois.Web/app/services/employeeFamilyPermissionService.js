(function () {
    'use strict';
    angular.module('app').service('employeeFamilyPermissionService', ['dataConstants', 'apiHttpService', employeeFamilyPermissionService]);

    function employeeFamilyPermissionService(dataConstants, apiHttpService) {
        var service = {
            getEmployeeFamilyPermissions: getEmployeeFamilyPermissions,
            getEmployeeFamilyPermission: getEmployeeFamilyPermission,
            saveEmployeeFamilyPermission: saveEmployeeFamilyPermission,
            updateEmployeeFamilyPermission: updateEmployeeFamilyPermission,
            deleteEmployeeFamilyPermission: deleteEmployeeFamilyPermission
        };

        return service;
        function getEmployeeFamilyPermissions(pageSize, pageNumber, searchText) {
            var url = dataConstants.EMPLOYEE_FAMILY_PERMISSION_URL + 'get-employee-family-permission-list?ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText;
            return apiHttpService.GET(url);
        }

        function getEmployeeFamilyPermission(id) {
            var url = dataConstants.EMPLOYEE_FAMILY_PERMISSION_URL + 'get-employee-family-permission?id=' + id;
            return apiHttpService.GET(url);
        }

        function saveEmployeeFamilyPermission(data) {
            var url = dataConstants.EMPLOYEE_FAMILY_PERMISSION_URL + 'save-employee-family-permission';
            return apiHttpService.POST(url, data);
        }

        function updateEmployeeFamilyPermission(id, data) {
            var url = dataConstants.EMPLOYEE_FAMILY_PERMISSION_URL + 'update-employee-family-permission/' + id;
            return apiHttpService.PUT(url, data);
        }

        function deleteEmployeeFamilyPermission(id) {
            var url = dataConstants.EMPLOYEE_FAMILY_PERMISSION_URL + 'delete-employee-family-permission/' + id;
            return apiHttpService.DELETE(url);
        }
    }
})();