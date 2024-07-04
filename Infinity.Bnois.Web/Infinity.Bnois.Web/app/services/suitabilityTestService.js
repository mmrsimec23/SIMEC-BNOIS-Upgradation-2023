(function () {
    'use strict';
    angular.module('app').service('suitabilityTestService', ['dataConstants', 'apiHttpService', suitabilityTestService]);

    function suitabilityTestService(dataConstants, apiHttpService) {
        var service = {
            getsuitabilityTests: getsuitabilityTests,
            getsuitabilityTest: getsuitabilityTest,
            savesuitabilityTest: savesuitabilityTest,
            updatesuitabilityTest: updatesuitabilityTest,
            suitabilityTestTypeList: suitabilityTestTypeList,
            deletesuitabilityTest: deletesuitabilityTest,
            GetSuitabilityTestListByType: GetSuitabilityTestListByType,
            deletesuitabilityTestTypeOfficerList: deletesuitabilityTestTypeOfficerList,
            saveSuitabilityTestTypeList: saveSuitabilityTestTypeList
        };

        return service;
        function getsuitabilityTests(type, pageSize, pageNumber, searchText) {
            var url = dataConstants.SUITABILITY_TEST_URL + 'get-suitability-tests?type=' + type + '&ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText;
            return apiHttpService.GET(url);
        }

        function getsuitabilityTest(suitabilityTestId) {
            var url = dataConstants.SUITABILITY_TEST_URL + 'get-suitability-test?id=' + suitabilityTestId;
            return apiHttpService.GET(url);
        }

        function savesuitabilityTest(data) {
            var url = dataConstants.SUITABILITY_TEST_URL + 'save-suitability-test';
            return apiHttpService.POST(url, data);
        }

        function updatesuitabilityTest(suitabilityTestId, data) {
            var url = dataConstants.SUITABILITY_TEST_URL + 'update-suitability-test/' + suitabilityTestId;
            return apiHttpService.PUT(url, data);
        }

        function deletesuitabilityTest(suitabilityTestId) {
            var url = dataConstants.SUITABILITY_TEST_URL + 'delete-suitability-test/' + suitabilityTestId;
            return apiHttpService.DELETE(url);
        }

        function deletesuitabilityTestTypeOfficerList(suitabilityTestType) {
            var url = dataConstants.SUITABILITY_TEST_URL + 'delete-suitability-test-type-list?type=' + suitabilityTestType;
            return apiHttpService.GET(url);
        }

        function saveSuitabilityTestTypeList(suitabilityTestType,batchId) {
            var url = dataConstants.SUITABILITY_TEST_URL + 'save-suitability-test-type-list?type=' + suitabilityTestType + '&batchId=' + batchId;
            return apiHttpService.GET(url);
        }

        function GetSuitabilityTestListByType(suitabilityTestType) {
            var url = dataConstants.SUITABILITY_TEST_URL + 'get-suitability-test-list-by-type?type=' + suitabilityTestType;
            return apiHttpService.GET(url);
        }

        function suitabilityTestTypeList() {
            var url = dataConstants.SUITABILITY_TEST_URL + 'get-suitability-test-type-list';
            return apiHttpService.GET(url);
        }

    }
})();