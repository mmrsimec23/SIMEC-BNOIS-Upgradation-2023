(function () {

	'use strict';
	var controllerId = 'evidencesController';
	angular.module('app').controller(controllerId, evidencesController);
	evidencesController.$inject = ['$state', 'evidenceService', 'notificationService', '$location'];

	function evidencesController($state, evidenceService, notificationService, location) {

		/* jshint validthis:true */
		var vm = this;
		vm.evidences = [];
		vm.addEvidence = addEvidence;
		vm.updateEvidence = updateEvidence;
		vm.deleteEvidence = deleteEvidence;
		vm.pageChanged = pageChanged;
		vm.searchText = "";
		vm.onSearch = onSearch;
		vm.pageSize = 50;
		vm.pageNumber = 1;
		vm.total = 0;

		if (location.search().ps !== undefined && location.search().ps !== null && location.search().ps !== '') {
			vm.pageSize = location.search().ps;
		}

		if (location.search().pn !== undefined && location.search().pn !== null && location.search().pn !== '') {
			vm.pageNumber = location.search().pn;
		}
		if (location.search().q !== undefined && location.search().q !== null && location.search().q !== '') {
			vm.searchText = location.search().q;
		}
		init();
		function init() {
			evidenceService.getEvidences(vm.pageSize, vm.pageNumber, vm.searchText).then(function (data) {

				vm.evidences = data.result;
				vm.total = data.total; vm.permission = data.permission;
			},
				function (errorMessage) {
					notificationService.displayError(errorMessage.message);
				});
		}

		function addEvidence() {
			$state.go('evidence-create');			
		}

		function updateEvidence(evidence) {
			$state.go('evidence-modify', { id: evidence.evidenceId });
		}

		function deleteEvidence(evidence) {	
			evidenceService.deleteEvidence(evidence.evidenceId).then(function (data) {
                $state.go($state.current, { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
            });
		}

        function pageChanged() {
			$state.go('evidences', { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });

		}

		function onSearch() {
			vm.pageNumber = 1;
			pageChanged();
		}
	}

})();
