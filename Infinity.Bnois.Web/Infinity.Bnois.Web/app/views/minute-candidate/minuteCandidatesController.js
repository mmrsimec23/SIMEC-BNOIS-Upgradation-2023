(function () {

    'use strict';
    var controllerId = 'minuteCandidatesController';
    angular.module('app').controller(controllerId, minuteCandidatesController);
    minuteCandidatesController.$inject = ['$state', '$stateParams', '$window', 'minuiteService','minuteCandidateService', 'notificationService', '$location'];

    function minuteCandidatesController($state, $stateParams, $window, minuiteService, minuteCandidateService, notificationService, location) {

        /* jshint validthis:true */
        var vm = this;
        vm.minuteId = 0;
        vm.minuite = '';
        vm.minuteCandidates = [];
        vm.updateMinuiteCandidate = updateMinuiteCandidate;
        vm.deleteMinuteCandidate = deleteMinuteCandidate;
        vm.saveButtonText = 'Save';
        vm.save = save;
        vm.back = back;

        if ($stateParams.minuiteId !== undefined && $stateParams.minuiteId !== null) {
            vm.minuteId = $stateParams.minuiteId;
        }
        


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

            minuiteService.getMinuite(vm.minuteId).then(function (data) {
                vm.minuiteName = data.result.minuite.minuiteName;  
                console.log(vm.minuite);
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
            minuteCandidateService.getMinuteCandidates(vm.minuteId).then(function (data) {
                vm.minuteCandidates = data.result;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function save() {
            if (vm.minuteCandidate.employee.employeeId > 0) {
                vm.minuteCandidate.employeeId = vm.minuteCandidate.employee.employeeId;
            } else {
                notificationService.displayError("Please Search Valid Officer by PNo!");
            }
            insertMinuteCandidate();
        }

        function insertMinuteCandidate() {
            minuteCandidateService.saveMinuteCandidate(vm.minuteId,vm.minuteCandidate).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function updateMinuiteCandidate(minuteCandidate) {
            vm.saveButtonText = 'Update';
            $window.document.documentElement.scrollTop = 0;
            $window.document.body.scrollTop = 0;
            minuteCandidateService.getMinuteCandidate(minuteCandidate.minuiteCandidateId).then(function (data) {
                vm.minuteCandidate = data.result;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
            
        }
        function close() {
            vm.minuteCandidate = null;
            init();
        }
        function deleteMinuteCandidate(minuteCandidate) {
            minuteCandidateService.deleteMinuteCandidate(minuteCandidate.minuiteCandidateId).then(function (data) {
                close();
            });


        }


        function back() {
            $state.go('minuites');
        }
    }

})();
