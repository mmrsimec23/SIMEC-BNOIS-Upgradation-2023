(function () {
	'use strict';
	angular.module('app').service('employeeLeaveService', ['dataConstants', 'apiHttpService', employeeLeaveService]);

	function employeeLeaveService(dataConstants, apiHttpService) {
		var service = {
			getEmployeeLeaves: getEmployeeLeaves,
			getEmployeeLeave: getEmployeeLeave,
			saveEmployeeLeave: saveEmployeeLeave,
			updateEmployeeLeave: updateEmployeeLeave,
			deleteEmployeeLeave: deleteEmployeeLeave,		
			getEmployeeLeaveDurationInfo: getEmployeeLeaveDurationInfo,
			getEmployeeLeaveBreakDown: getEmployeeLeaveBreakDown,
            getEmployeeLeaveAndEmployeeInfo: getEmployeeLeaveAndEmployeeInfo,
            fileUploadUrl: fileUploadUrl,
            getPrivilegeLeaveDurationInfo: getPrivilegeLeaveDurationInfo
		};

        return service;

        function fileUploadUrl() {
            var url = dataConstants.EMPLOYEELEAVE_URL + 'upload-leave-application-file';
            return url;
        }

		function getEmployeeLeaves(pageSize, pageNumber, searchText,leaveTypeId) {
			var url = dataConstants.EMPLOYEELEAVE_URL + 'get-employeeLeaves?ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText + "&leaveType=" + leaveTypeId ;
			return apiHttpService.GET(url);
		}

		function getEmployeeLeave(id) {
			var url = dataConstants.EMPLOYEELEAVE_URL + 'get-employeeLeave?id=' + id;
			return apiHttpService.GET(url);
		}

		function saveEmployeeLeave(data) {
            var url = dataConstants.EMPLOYEELEAVE_URL + 'save-employeeLeave';
			return apiHttpService.POST(url, data);
		}

		function updateEmployeeLeave(id, data) {
			var url = dataConstants.EMPLOYEELEAVE_URL + 'update-employeeLeave/' + id;
			return apiHttpService.PUT(url, data);
		}

		//function deleteEmployeeLeave(id) {
		//	var url = dataConstants.EMPLOYEELEAVE_URL + 'delete-employeeLeave/' + id;
		//	return apiHttpService.DELETE(url);			
		//}
        
        function getPrivilegeLeaveDurationInfo(employeeId, leaveType,fromDate) {

            var url = dataConstants.EMPLOYEELEAVE_URL + 'get-employee-Defaultleaveduration?employeeId=' + employeeId + "&leaveType=" + leaveType + "&fromDate=" + fromDate;
            return apiHttpService.GET(url);
        }
        function getEmployeeLeaveDurationInfo(employeeId, leaveType) {
         
            var url = dataConstants.EMPLOYEELEAVE_URL + 'get-employee-leaveduration?employeeId=' + employeeId + "&leaveType=" + leaveType ;
			return apiHttpService.GET(url);
		}
		function getEmployeeLeaveBreakDown(employeeId) {

			var url = dataConstants.EMPLOYEELEAVE_URL + 'get-leave-break-downs?employeeId=' + employeeId ;
			return apiHttpService.GET(url);
		}
		function getEmployeeLeaveAndEmployeeInfo(pId) {

			var url = dataConstants.EMPLOYEELEAVE_URL + 'get-employee-and-leaveInfo?pId=' + pId;
			return apiHttpService.GET(url);
		}

		function deleteEmployeeLeave(id) {
			var url = dataConstants.EMPLOYEELEAVE_URL + 'delete-employeeLeave/' + id;
			return apiHttpService.DELETE(url);
		}
	
	}
})();