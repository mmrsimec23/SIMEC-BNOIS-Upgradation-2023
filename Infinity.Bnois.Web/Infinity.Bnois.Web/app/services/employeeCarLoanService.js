(function () {
    'use strict';
    angular.module('app').service('employeeCarLoanService', ['dataConstants', 'apiHttpService', employeeCarLoanService]);

    function employeeCarLoanService(dataConstants, apiHttpService) {
        var service = {
            getEmployeeCarLoanList: getEmployeeCarLoanList,
            getEmployeeCarLoan: getEmployeeCarLoan,
            saveEmployeeCarLoan: saveEmployeeCarLoan,
            updateEmployeeCarLoan: updateEmployeeCarLoan,
            deleteEmployeeCarLoan: deleteEmployeeCarLoan,
            GetEmployeeCarLoansByPno: GetEmployeeCarLoansByPno
        };

        return service;
        function getEmployeeCarLoanList(pageSize, pageNumber, searchText) {
            var url = dataConstants.EMPLOYEE_CAR_LOAN_URL + 'get-employee-car-loan-list?ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText;
            return apiHttpService.GET(url);
        }
        function GetEmployeeCarLoansByPno(PNo) {
            var url = dataConstants.EMPLOYEE_CAR_LOAN_URL + 'get-employee-car-loan-by-pno?PNo=' + PNo
            return apiHttpService.GET(url);
        }
        function getEmployeeCarLoan(employeeCarLoanId) {
            var url = dataConstants.EMPLOYEE_CAR_LOAN_URL + 'get-employee-car-loan?id=' + employeeCarLoanId;
            return apiHttpService.GET(url);
        }

        function saveEmployeeCarLoan(data) {
            var url = dataConstants.EMPLOYEE_CAR_LOAN_URL + 'save-employee-car-loan';
            return apiHttpService.POST(url, data);
        }

        function updateEmployeeCarLoan(employeeCarLoanId, data) {
            var url = dataConstants.EMPLOYEE_CAR_LOAN_URL + 'update-employee-car-loan/' + employeeCarLoanId;
            return apiHttpService.PUT(url, data);
        }

        function deleteEmployeeCarLoan(employeeCarLoanId) {
            var url = dataConstants.EMPLOYEE_CAR_LOAN_URL + 'delete-employee-car-loan/' + employeeCarLoanId;
            return apiHttpService.DELETE(url);
        }


    }
})();