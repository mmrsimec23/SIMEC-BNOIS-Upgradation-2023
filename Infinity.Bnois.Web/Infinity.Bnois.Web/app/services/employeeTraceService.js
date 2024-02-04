(function () {
    'use strict';
    angular.module('app').service('employeeTraceService', ['dataConstants', 'apiHttpService', employeeTraceService]);

    function employeeTraceService(dataConstants, apiHttpService) {
        var service = {
            getEmployeeTraceList: getEmployeeTraceList,
            getEmployeeTrace: getEmployeeTrace,
            saveEmployeeTrace: saveEmployeeTrace,
            updateEmployeeTrace: updateEmployeeTrace,
            deleteEmployeeTrace: deleteEmployeeTrace,
            GetEmployeeTracesByPno: GetEmployeeTracesByPno
        };

        return service;
        function getEmployeeTraceList(pageSize, pageNumber, searchText) {
            var url = dataConstants.EMPLOYEE_TRACE_URL + 'get-employee-trace-list?ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText;
            return apiHttpService.GET(url);
        }
        function GetEmployeeTracesByPno(PNo) {
            var url = dataConstants.EMPLOYEE_TRACE_URL + 'get-employee-trace-by-pno?PNo=' + PNo
            return apiHttpService.GET(url);
        }
        function getEmployeeTrace(employeeTraceId) {
            var url = dataConstants.EMPLOYEE_TRACE_URL + 'get-employee-trace?id=' + employeeTraceId;
            return apiHttpService.GET(url);
        }

        function saveEmployeeTrace(data) {
            var url = dataConstants.EMPLOYEE_TRACE_URL + 'save-employee-trace';
            return apiHttpService.POST(url, data);
        }

        function updateEmployeeTrace(employeeTraceId, data) {
            var url = dataConstants.EMPLOYEE_TRACE_URL + 'update-employee-trace/' + employeeTraceId;
            return apiHttpService.PUT(url, data);
        }

        function deleteEmployeeTrace(employeeTraceId) {
            var url = dataConstants.EMPLOYEE_TRACE_URL + 'delete-employee-trace/' + employeeTraceId;
            return apiHttpService.DELETE(url);
        }


    }
})();