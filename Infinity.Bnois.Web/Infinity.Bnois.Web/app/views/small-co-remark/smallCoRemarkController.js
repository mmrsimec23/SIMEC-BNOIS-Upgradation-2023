(function () {

    'use strict';
    var controllerId = 'smallCoRemarkController';
    angular.module('app').controller(controllerId, smallCoRemarkController);
    smallCoRemarkController.$inject = ['$state', '$stateParams', '$window', 'coFfRecomService', 'notificationService', '$location'];

    function smallCoRemarkController($state, $stateParams, $window, coFfRecomService, notificationService, location) {

        /* jshint validthis:true */
        var vm = this;
        vm.id = 0;
        vm.smallCoRemarks = [];
        vm.deleteSmallCoRemark = deleteSmallCoRemark;
        vm.updateSmallCoRemark = updateSmallCoRemark;
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
            coFfRecomService.getCoFfRecoms(3).then(function (data) {
                vm.smallCoRemarks = data.result;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function save() {
            if (vm.smallCoRemark.employee.employeeId > 0) {
                vm.smallCoRemark.employeeId = vm.smallCoRemark.employee.employeeId;
                vm.smallCoRemark.recomStatus = 3;
            } else {
                notificationService.displayError("Please Search Valid Officer by PNo!");
            }
            insertSmallCoRemark();
        }

        function insertSmallCoRemark() {
            coFfRecomService.saveCoFfRecom(vm.id, vm.smallCoRemark).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }
        function close() {
            vm.smallCoRemark = null;
            init();
        }
        function deleteSmallCoRemark(smallCoRemark) {
            coFfRecomService.deleteCoFfRecom(smallCoRemark.id).then(function (data) {
                close();
            });


        }



        function updateSmallCoRemark(smallCoRemark) {
            vm.saveButtonText = 'Update';
            $window.document.documentElement.scrollTop = 0;
            $window.document.body.scrollTop = 0;
            coFfRecomService.getCoFfRecom(smallCoRemark.id).then(function (data) {
                vm.smallCoRemark = data.result;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });

        }


        function back() {
            $state.go('proposal-details');
        }
    }

})();