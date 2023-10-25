

(function () {

	'use strict';

	var controllerId = 'bnoisLogInfoController';

	angular.module('app').controller(controllerId, bnoisLogInfoController);
	bnoisLogInfoController.$inject = ['$stateParams', 'bnoisLogInfoService', 'notificationService', '$state', '$location'];

	function bnoisLogInfoController($stateParams, bnoisLogInfoService, notificationService, $state, location) {
		var vm = this;
		vm.bloodGroupId = 0;
		vm.bnoisLogs = [];
        vm.title = 'ADD MODE';
		vm.bnoisLog = {};	
		vm.saveButtonText = 'Save';
		vm.find = find;
		vm.bnoisLogForm = {};

		vm.tableList = [];

		vm.bnoisLog.tableName = '';
		vm.bnoisLog.logStatus = 0;
		vm.bnoisLog.fromDate = '';
		vm.bnoisLog.toDate = '';
		vm.bnoisLog.startDate = '';
		vm.bnoisLog.endDate = '';

		Init();
		function Init() {
			bnoisLogInfoService.getTableSelectModels().then(function (data) {
				vm.tableList = data.result.tableList;
				vm.logStatusList = data.result.logStatusList;
			},
				function (errorMessage) {
					notificationService.displayError(errorMessage.message);
				});
		};

		function getBnoisLogInfos() {
			bnoisLogInfoService.getBnoisLogInfos(vm.bnoisLog.tableName, vm.bnoisLog.logStatus, vm.bnoisLog.startDate, vm.bnoisLog.endDate).then(function (data) {

				vm.bnoisLogs = data.result;
			},
				function (errorMessage) {
					notificationService.displayError(errorMessage.message);
				});
		}
		function find() {
			
			if (vm.bnoisLog.fromDate != '') {

				vm.bnoisLog.startDate = formatDate(vm.bnoisLog.fromDate);
			}

			if (vm.bnoisLog.toDate != '') {

				vm.bnoisLog.endDate = formatDate(vm.bnoisLog.toDate);
			}

			getBnoisLogInfos();
		}
		function formatDate(date) {
			var day = date.getDate();
			var month = date.getMonth() + 1; // Months are 0-based
			var year = date.getFullYear();

			return (day < 10 ? '0' : '') + day + '/' + (month < 10 ? '0' : '') + month + '/' + year;
		}
	}
})();
