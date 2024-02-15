(function () {
    'use strict';
    angular.module('app').service('employeeProposedEolosodloseoService', ['dataConstants', 'apiHttpService', employeeProposedEolosodloseoService]);

    function employeeProposedEolosodloseoService(dataConstants, apiHttpService) {
        var service = {
            getemployeeProposedEolosodloseos: getemployeeProposedEolosodloseos,
            getemployeeProposedEolosodloseo: getemployeeProposedEolosodloseo,
            saveemployeeProposedEolosodloseo: saveemployeeProposedEolosodloseo,
            updateemployeeProposedEolosodloseo: updateemployeeProposedEolosodloseo,
            GetEmployeeEolosodloseoServiceOfficeList: GetEmployeeEolosodloseoServiceOfficeList,
            deleteemployeeProposedEolosodloseo: deleteemployeeProposedEolosodloseo
        };

        return service;
        function getemployeeProposedEolosodloseos(type, pageSize, pageNumber, searchText) {
            var url = dataConstants.BNOIS_EOLOSODLOSEO_SERVICE_URL + 'get-employee-eolosodloseo-services?type=' + type + '&ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText;
            return apiHttpService.GET(url);
        }

        function getemployeeProposedEolosodloseo(employeeProposedCoxoId, type) {
            var url = dataConstants.BNOIS_EOLOSODLOSEO_SERVICE_URL + 'get-employee-eolosodloseo-service?id=' + employeeProposedCoxoId + '&type=' + type;
            return apiHttpService.GET(url);
        }

        function saveemployeeProposedEolosodloseo(data) {
            var url = dataConstants.BNOIS_EOLOSODLOSEO_SERVICE_URL + 'save-employee-eolosodloseo-service';
            return apiHttpService.POST(url, data);
        }

        function updateemployeeProposedEolosodloseo(employeeProposedCoxoId, data) {
            var url = dataConstants.BNOIS_EOLOSODLOSEO_SERVICE_URL + 'update-employee-eolosodloseo-service/' + employeeProposedCoxoId;
            return apiHttpService.PUT(url, data);
        }

        function deleteemployeeProposedEolosodloseo(employeeProposedCoxoId) {
            var url = dataConstants.BNOIS_EOLOSODLOSEO_SERVICE_URL + 'delete-employee-eolosodloseo-service/' + employeeProposedCoxoId;
            return apiHttpService.DELETE(url);
        }

        function GetEmployeeEolosodloseoServiceOfficeList(type) {
            var url = dataConstants.BNOIS_EOLOSODLOSEO_SERVICE_URL + 'get-employee-eolosodloseo-service-office-list?type=' + type;
            return apiHttpService.GET(url);
        }

    }
})();