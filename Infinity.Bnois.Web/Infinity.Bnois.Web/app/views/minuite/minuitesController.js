(function () {

    'use strict';
    var controllerId = 'minuitesController';
    angular.module('app').controller(controllerId, minuitesController);
    minuitesController.$inject = ['$state', 'minuiteService', 'downloadService','reportService', 'notificationService', '$location'];

    function minuitesController($state, minuiteService, downloadService, reportService, notificationService, location) {

        /* jshint validthis:true */
        var vm = this;
        vm.minuites = [];
        vm.addMinuite = addMinuite;
        vm.updateMinuite = updateMinuite;
        vm.deleteMinuite = deleteMinuite;
        vm.getMinuteCandidates = getMinuteCandidates;
        vm.downlaodMinute = downlaodMinute;
        //vm.downlaodminuiteForXBranch = downlaodminuiteForXBranch;
        //vm.downlaodminuiteWithoutXBranch = downlaodminuiteWithoutXBranch;
        //vm.downlaodminuiteWithPic = downlaodminuiteWithPic;
        vm.pageChanged = pageChanged;
        vm.searchText = "";
        vm.onSearch = onSearch;
        vm.pageSize = 50;
        vm.pageNumber = 1;
        vm.total = 0;


        vm.reportType = 1;
        vm.reportTypes = [{ text: 'PDF', value: 1 }, { text: 'WORD', value: 2 }, { text: 'EXCEL', value: 3 }];

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
            minuiteService.getMinuites(vm.pageSize, vm.pageNumber, vm.searchText).then(function (data) {
                vm.minuites = data.result;
                vm.total = data.total; vm.permission = data.permission;
                console.log(vm.minuites)
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function addMinuite() {
            $state.go('minuite-create');
        }

        function updateMinuite(minuite) {
            $state.go('minuite-modify', { id: minuite.minuiteId });
        }

        function deleteMinuite(minuite) {
            minuiteService.deleteMinuite(minuite.minuiteId).then(function (data) {
                $state.go($state.current, { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
            });
        }

        function getMinuteCandidates(minuite) {
            $state.go('minute-candidates', { minuiteId: minuite.minuiteId });
        }

        
        function pageChanged() {
            $state.go('minuites', { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
        }
        
        function onSearch() {
            vm.pageNumber = 1;
            pageChanged();
        }
        

        function downlaodMinute(minuite) {
            var url = reportService.downlaodMinuiteUrl(minuite.minuiteId, vm.reportType );
            downloadService.downloadFile(url);
        }

        //function downlaodminuiteForXBranch(minuite) {
        //    var url = reportService.downlaodminuiteXBranchUrl(minuite.minuiteId, vm.reportType );
        //    downloadService.downloadFile(url);
        //}
        //function downlaodminuiteWithoutXBranch(minuite) {
        //    var url = reportService.downlaodminuiteWithoutXBranchUrl(minuite.minuiteId, vm.reportType );
        //    downloadService.downloadFile(url);
        //}
        //function downlaodminuiteWithPic(minuite) {
        //    var url = reportService.downlaodminuiteWithPicUrl(minuite.minuiteId, vm.reportType );
        //    downloadService.downloadFile(url);
        //}

    }

})();
