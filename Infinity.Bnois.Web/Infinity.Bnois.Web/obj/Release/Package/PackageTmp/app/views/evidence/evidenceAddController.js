

(function () {

	'use strict';

	var controllerId = 'evidenceAddController';

	angular.module('app').controller(controllerId, evidenceAddController);
	evidenceAddController.$inject = ['$stateParams', 'evidenceService', 'notificationService', '$state'];

	function evidenceAddController($stateParams, evidenceService, notificationService, $state) {
		var vm = this;
		vm.evidenceId = 0;
        vm.title = 'ADD MODE';
		vm.evidence = {};	
		vm.saveButtonText = 'Save';
		vm.save = save;
		vm.close = close;
		vm.evidenceForm = {};

        if ($stateParams.id !== undefined && $stateParams.id !== '') {
			vm.evidenceId = $stateParams.id;
			vm.saveButtonText = 'Update';
            vm.title = 'UPDATE MODE';
		}

		Init();
		function Init() {
			evidenceService.getEvidence(vm.evidenceId).then(function (data) {
				vm.evidence = data.result;			
			},
				function (errorMessage) {
					notificationService.displayError(errorMessage.message);
				});
		};

		function save() {

			if (vm.evidenceId !== 0) {
				updateEvidence();
			} else {
				insertEvidence();
			}
		}

		function insertEvidence() {
			evidenceService.saveEvidence(vm.evidence).then(function (data) {
				close();
			},
				function (errorMessage) {
					notificationService.displayError(errorMessage.message);
				});
		}

		function updateEvidence() {
			evidenceService.updateEvidence(vm.evidenceId, vm.evidence).then(function (data) {
				close();
			},
				function (errorMessage) {
					notificationService.displayError(errorMessage.message);
				});
		}

		function close() {	
			$state.go('evidences');
		}

	}
})();
