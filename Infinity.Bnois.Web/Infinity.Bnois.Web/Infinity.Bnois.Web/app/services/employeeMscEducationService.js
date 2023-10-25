(function () {
    'use strict';
    angular.module('app').service('employeeMscEducationService', ['dataConstants', 'apiHttpService', employeeMscEducationService]);

    function employeeMscEducationService(dataConstants, apiHttpService) {
        var service = {
            getEmployeeMscEducations: getEmployeeMscEducations,
            getEmployeeMscEducation: getEmployeeMscEducation,
            saveEmployeeMscEducation: saveEmployeeMscEducation,
            updateEmployeeMscEducation: updateEmployeeMscEducation,
            deleteEmployeeMscEducation: deleteEmployeeMscEducation
        };

        return service;
        function getEmployeeMscEducations(pageSize, pageNumber, searchText) {
            var url = dataConstants.EMPLOYEE_MSC_EDUCATION_URL + 'get-employee-msc-education-list?ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText;
            return apiHttpService.GET(url);
        }

        function getEmployeeMscEducation(id) {
            var url = dataConstants.EMPLOYEE_MSC_EDUCATION_URL + 'get-employee-msc-education?id=' + id;
            return apiHttpService.GET(url);
        }

        function saveEmployeeMscEducation(data) {
            var url = dataConstants.EMPLOYEE_MSC_EDUCATION_URL + 'save-employee-msc-education';
            return apiHttpService.POST(url, data);
        }

        function updateEmployeeMscEducation(id, data) {
            var url = dataConstants.EMPLOYEE_MSC_EDUCATION_URL + 'update-employee-msc-education/' + id;
            return apiHttpService.PUT(url, data);
        }

        function deleteEmployeeMscEducation(id) {
            var url = dataConstants.EMPLOYEE_MSC_EDUCATION_URL + 'delete-employee-msc-education/' + id;
            return apiHttpService.DELETE(url);
        }
    }
})();