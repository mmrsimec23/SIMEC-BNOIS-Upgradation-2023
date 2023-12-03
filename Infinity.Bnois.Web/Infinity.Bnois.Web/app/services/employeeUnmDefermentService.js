(function () {
    'use strict';
    angular.module('app').service('employeeUnmDefermentService', ['dataConstants', 'apiHttpService', employeeUnmDefermentService]);

    function employeeUnmDefermentService(dataConstants, apiHttpService) {
        var service = {
            getEmployeeUnmDeferments: getEmployeeUnmDeferments,
            getEmployeeUnmDeferment: getEmployeeUnmDeferment,
            saveEmployeeUnmDeferment: saveEmployeeUnmDeferment,
            updateEmployeeUnmDeferment: updateEmployeeUnmDeferment,
            deleteEmployeeUnmDeferment: deleteEmployeeUnmDeferment
        };

        return service;
        function getEmployeeUnmDeferments(pageSize, pageNumber, searchText) {
            var url = dataConstants.EMPLOYEE_UNM_DEFERMENT_URL + 'get-employee-unm-deferment-list?ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText;
            return apiHttpService.GET(url);
        }

        function getEmployeeUnmDeferment(id) {
            var url = dataConstants.EMPLOYEE_UNM_DEFERMENT_URL + 'get-employee-unm-deferment?id=' + id;
            return apiHttpService.GET(url);
        }

        function saveEmployeeUnmDeferment(data) {
            var url = dataConstants.EMPLOYEE_UNM_DEFERMENT_URL + 'save-employee-unm-deferment';
            return apiHttpService.POST(url, data);
        }

        function updateEmployeeUnmDeferment(id, data) {
            var url = dataConstants.EMPLOYEE_UNM_DEFERMENT_URL + 'update-employee-unm-deferment/' + id;
            return apiHttpService.PUT(url, data);
        }

        function deleteEmployeeUnmDeferment(id) {
            var url = dataConstants.EMPLOYEE_UNM_DEFERMENT_URL + 'delete-employee-unm-deferment/' + id;
            return apiHttpService.DELETE(url);
        }
    }
})();