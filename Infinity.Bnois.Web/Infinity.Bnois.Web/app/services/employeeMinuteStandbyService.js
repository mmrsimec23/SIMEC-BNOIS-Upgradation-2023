(function () {
    'use strict';
    angular.module('app').service('employeeMinuteStandbyService', ['dataConstants', 'apiHttpService', employeeMinuteStandbyService]);

    function employeeMinuteStandbyService(dataConstants, apiHttpService) {
        var service = {
            getEmployeeMinuteStandbys: getEmployeeMinuteStandbys,
            getEmployeeMinuteStandby: getEmployeeMinuteStandby,
            saveEmployeeMinuteStandby: saveEmployeeMinuteStandby,
            updateEmployeeMinuteStandby: updateEmployeeMinuteStandby,
            deleteEmployeeMinuteStandby: deleteEmployeeMinuteStandby
        };

        return service;
        function getEmployeeMinuteStandbys(pageSize, pageNumber, searchText) {
            var url = dataConstants.EMPLOYEE_MINUTE_STANDBY_URL + 'get-employee-minute-standby-list?ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText;
            return apiHttpService.GET(url);
        }

        function getEmployeeMinuteStandby(id) {
            var url = dataConstants.EMPLOYEE_MINUTE_STANDBY_URL + 'get-employee-minute-standby?id=' + id;
            return apiHttpService.GET(url);
        }

        function saveEmployeeMinuteStandby(data) {
            var url = dataConstants.EMPLOYEE_MINUTE_STANDBY_URL + 'save-employee-minute-standby';
            return apiHttpService.POST(url, data);
        }

        function updateEmployeeMinuteStandby(id, data) {
            var url = dataConstants.EMPLOYEE_MINUTE_STANDBY_URL + 'update-employee-minute-standby/' + id;
            return apiHttpService.PUT(url, data);
        }

        function deleteEmployeeMinuteStandby(id) {
            var url = dataConstants.EMPLOYEE_MINUTE_STANDBY_URL + 'delete-employee-minute-standby/' + id;
            return apiHttpService.DELETE(url);
        }
    }
})();