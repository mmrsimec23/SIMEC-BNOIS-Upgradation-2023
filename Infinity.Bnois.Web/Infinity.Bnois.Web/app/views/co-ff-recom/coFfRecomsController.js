(function () {

    'use strict';
    var controllerId = 'coFfRecomsController';
    angular.module('app').controller(controllerId, coFfRecomsController);
    coFfRecomsController.$inject = ['$state', '$stateParams', '$window', 'coFfRecomService', 'notificationService', '$location'];

    function coFfRecomsController($state, $stateParams, $window, coFfRecomService, notificationService, location) {

        /* jshint validthis:true */
        var vm = this;
        vm.id = 0;
        vm.coFfRecoms = [];
        vm.deleteCoFfRecom = deleteCoFfRecom;
        vm.saveButtonText = 'Save';
        vm.save = save;
        vm.back = back;

        if ($stateParams.id !== undefined && $stateParams.id !== null) {
            vm.id = $stateParams.id;
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
            coFfRecomService.getCoFfRecoms().then(function (data) {
                vm.coFfRecoms = data.result;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function save() {
            if (vm.coFfRecom.employee.employeeId > 0) {
                vm.coFfRecom.employeeId = vm.coFfRecom.employee.employeeId;
                vm.coFfRecom.recomStatus = 1;
            } else {
                notificationService.displayError("Please Search Valid Officer by PNo!");
            }
            insertCoFfRecom();
        }

        function insertCoFfRecom() {
            coFfRecomService.saveCoFfRecom(vm.id, vm.coFfRecom).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }
        function close() {
            vm.coFfRecom = null;
            init();
        }
        function deleteCoFfRecom(coFfRecom) {
            coFfRecomService.deleteCoFfRecom(coFfRecom.id).then(function (data) {
                close();
            });


        }


        function back() {
            $state.go('proposal-details');
        }
    }

})();




//(function () {

//    'use strict';
//    var controllerId = 'coFfRecomsController';
//    angular.module('app').controller(controllerId, coFfRecomsController);
//    coFfRecomsController.$inject = ['$state', '$stateParams', 'coFfRecomservice', 'notificationService', '$location'];

//    function coFfRecomsController($state, $stateParams, coFfRecomservice, notificationService, location) {

//        /* jshint validthis:true */
//        var vm = this;
//        vm.coFfRecoms = [];
//        vm.proposalDetailId = 0;
//        vm.addProposalCandidate = addProposalCandidate;
//        vm.updateProposalCandidate = updateProposalCandidate;
//        vm.deleteProposalCandidate = deleteProposalCandidate;
//        vm.back = back;
//        vm.pageChanged = pageChanged;



//        if ($stateParams.proposalDetailId !== undefined && $stateParams.proposalDetailId !== null) {
//            vm.proposalDetailId = $stateParams.proposalDetailId;
//        }

//        if (location.search().ps !== undefined && location.search().ps !== null && location.search().ps !== '') {
//            vm.pageSize = location.search().ps;
//        }

//        if (location.search().pn !== undefined && location.search().pn !== null && location.search().pn !== '') {
//            vm.pageNumber = location.search().pn;
//        }
//        if (location.search().q !== undefined && location.search().q !== null && location.search().q !== '') {
//            vm.searchText = location.search().q;
//        }
        

//        init();
//        function init() {
//            coFfRecomservice.getcoFfRecoms(vm.proposalDetailId,vm.pageSize, vm.pageNumber, vm.searchText).then(function (data) {
//                vm.coFfRecoms = data.result;
//                vm.total = data.total; vm.permission = data.permission;
//            },
//                function (errorMessage) {
//                    notificationService.displayError(errorMessage.message);
//                });
//        }

//        function addProposalCandidate() {
//            $state.go('proposal-detail-create', { proposalDetailId: vm.proposalDetailId });
//        }


//        function deleteProposalCandidate(ProposalCandidate) {
//            coFfRecomservice.deleteProposalCandidate(ProposalCandidate.ProposalCandidateId).then(function (data) {
//                $state.go($state.current, { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
//            });
//        }

//        function back() {
//            $state.go('transfer-proposals');
//        }
       
//    }

//})();
