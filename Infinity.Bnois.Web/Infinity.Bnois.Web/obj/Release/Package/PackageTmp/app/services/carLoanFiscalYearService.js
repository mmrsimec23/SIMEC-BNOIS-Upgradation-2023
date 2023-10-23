(function () {
    'use strict';
    angular.module('app').service('carLoanFiscalYearService', ['dataConstants', 'apiHttpService', carLoanFiscalYearService]);

    function carLoanFiscalYearService(dataConstants, apiHttpService) {
        var service = {
            getCarLoanFiscalYearList: getCarLoanFiscalYearList,
            getCarLoanFiscalYear: getCarLoanFiscalYear,
            saveCarLoanFiscalYear: saveCarLoanFiscalYear,
            updateCarLoanFiscalYear: updateCarLoanFiscalYear,
            deleteCarLoanFiscalYear: deleteCarLoanFiscalYear
        };

        return service;
        function getCarLoanFiscalYearList(pageSize, pageNumber, searchText) {
            var url = dataConstants.CAR_LOAN_FISCAL_YEAR_URL + 'get-car-loan-fiscal-year-list?ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText;
            return apiHttpService.GET(url);
        }

        function getCarLoanFiscalYear(id) {
            var url = dataConstants.CAR_LOAN_FISCAL_YEAR_URL + 'get-car-loan-fiscal-year?id=' + id;
            return apiHttpService.GET(url);
        }

        function saveCarLoanFiscalYear(data) {
            var url = dataConstants.CAR_LOAN_FISCAL_YEAR_URL + 'save-car-loan-fiscal-year';
            return apiHttpService.POST(url, data);
        }

        function updateCarLoanFiscalYear(id, data) {
            var url = dataConstants.CAR_LOAN_FISCAL_YEAR_URL + 'update-car-loan-fiscal-year/' + id;
            return apiHttpService.PUT(url, data);
        }

        function deleteCarLoanFiscalYear(id) {
            var url = dataConstants.CAR_LOAN_FISCAL_YEAR_URL + 'delete-car-loan-fiscal-year/' + id;
            return apiHttpService.DELETE(url);
        }
    }
})();