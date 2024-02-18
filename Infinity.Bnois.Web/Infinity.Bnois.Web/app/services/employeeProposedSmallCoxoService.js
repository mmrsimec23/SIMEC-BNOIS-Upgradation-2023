(function () {
    'use strict';
    angular.module('app').service('employeeProposedSmallCoxoService', ['dataConstants', 'apiHttpService', employeeProposedSmallCoxoService]);

    function employeeProposedSmallCoxoService(dataConstants, apiHttpService) {
        var service = {
            getemployeeProposedCoxos: getemployeeProposedCoxos,
            getemployeeProposedCoxo: getemployeeProposedCoxo,
            saveemployeeProposedCoxo: saveemployeeProposedCoxo,
            updateemployeeProposedCoxo: updateemployeeProposedCoxo,
            GetEmployeeCoxoServiceOfficeList: GetEmployeeCoxoServiceOfficeList,
            deleteemployeeProposedCoxo: deleteemployeeProposedCoxo
        };

        return service;
        function getemployeeProposedCoxos(type, pageSize, pageNumber, searchText) {
            var url = dataConstants.BNOIS_SMALL_COXO_SERVICE_URL + 'get-employee-small-coxo-services?type=' + type + '&ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText;
            return apiHttpService.GET(url);
        }

        function getemployeeProposedCoxo(employeeProposedCoxoId, type) {
            var url = dataConstants.BNOIS_SMALL_COXO_SERVICE_URL + 'get-employee-small-coxo-service?id=' + employeeProposedCoxoId + '&type=' + type;
            return apiHttpService.GET(url);
        }

        function saveemployeeProposedCoxo(data) {
            var url = dataConstants.BNOIS_SMALL_COXO_SERVICE_URL + 'save-employee-small-coxo-service';
            return apiHttpService.POST(url, data);
        }

        function updateemployeeProposedCoxo(employeeProposedCoxoId, data) {
            var url = dataConstants.BNOIS_SMALL_COXO_SERVICE_URL + 'update-employee-small-coxo-service/' + employeeProposedCoxoId;
            return apiHttpService.PUT(url, data);
        }

        function deleteemployeeProposedCoxo(employeeProposedCoxoId) {
            var url = dataConstants.BNOIS_SMALL_COXO_SERVICE_URL + 'delete-employee-small-coxo-service/' + employeeProposedCoxoId;
            return apiHttpService.DELETE(url);
        }

        function GetEmployeeCoxoServiceOfficeList(type) {
            var url = dataConstants.BNOIS_SMALL_COXO_SERVICE_URL + 'get-employee-small-coxo-service-office-list?type=' + type;
            return apiHttpService.GET(url);
        }

    }
})();