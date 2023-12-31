﻿(function () {
    'use strict';
    angular.module('app').service('employeeProposedCoxoService', ['dataConstants', 'apiHttpService', employeeProposedCoxoService]);

    function employeeProposedCoxoService(dataConstants, apiHttpService) {
        var service = {
            getemployeeProposedCoxos: getemployeeProposedCoxos,
            getemployeeProposedCoxo: getemployeeProposedCoxo,
            saveemployeeProposedCoxo: saveemployeeProposedCoxo,
            updateemployeeProposedCoxo: updateemployeeProposedCoxo,
            GetEmployeeCoxoServiceOfficeList: GetEmployeeCoxoServiceOfficeList,
            deleteemployeeProposedCoxo: deleteemployeeProposedCoxo
        };

        return service;
        function getemployeeProposedCoxos(pageSize, pageNumber, searchText) {
            var url = dataConstants.BNOIS_COXO_SERVICE_URL + 'get-employee-coxo-services?ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText;
            return apiHttpService.GET(url);
        }

        function getemployeeProposedCoxo(employeeProposedCoxoId) {
            var url = dataConstants.BNOIS_COXO_SERVICE_URL + 'get-employee-coxo-service?id=' + employeeProposedCoxoId;
            return apiHttpService.GET(url);
        }

        function saveemployeeProposedCoxo(data) {
            var url = dataConstants.BNOIS_COXO_SERVICE_URL + 'save-employee-coxo-service';
            return apiHttpService.POST(url, data);
        }

        function updateemployeeProposedCoxo(employeeProposedCoxoId, data) {
            var url = dataConstants.BNOIS_COXO_SERVICE_URL + 'update-employee-coxo-service/' + employeeProposedCoxoId;
            return apiHttpService.PUT(url, data);
        }

        function deleteemployeeProposedCoxo(employeeProposedCoxoId) {
            var url = dataConstants.BNOIS_COXO_SERVICE_URL + 'delete-employee-coxo-service/' + employeeProposedCoxoId;
            return apiHttpService.DELETE(url);
        }

        function GetEmployeeCoxoServiceOfficeList(type) {
            var url = dataConstants.BNOIS_COXO_SERVICE_URL + 'get-employee-coxo-service-office-list?type=' + type;
            return apiHttpService.GET(url);
        }

    }
})();