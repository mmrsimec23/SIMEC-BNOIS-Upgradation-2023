(function () {
    'use strict';
    angular.module('app').service('advanceSearchService', ['dataConstants', 'apiHttpService', advanceSearchService]);

    function advanceSearchService(dataConstants, apiHttpService) {
        var service = {
            getAdvanceSearchSelectModels: getAdvanceSearchSelectModels,
            saveCheckedValue: saveCheckedValue,
            deleteCheckedColumn: deleteCheckedColumn,
            searchOfficers: searchOfficers,
            getCourseSubCategories: getCourseSubCategories,
            getCourseByCategory: getCourseByCategory,
            getCourseBySubCategory: getCourseBySubCategory,
            searchOfficersResult: searchOfficersResult
        };

        return service;
       
        function getAdvanceSearchSelectModels() {
            var url = dataConstants.ADVANCE_SEARCH_URL + 'get-advance-search-select-models';
            return apiHttpService.GET(url);
        }


        function searchOfficers(data) {
            var url = dataConstants.ADVANCE_SEARCH_URL + 'search-officers';
            return apiHttpService.POST(url, data);
        }

        function getCourseSubCategories(categoryId) {
            var url = dataConstants.ADVANCE_SEARCH_URL + 'get-course-sub-categories-by-course?id=' + categoryId;
            return apiHttpService.GET(url);
        }
        function getCourseByCategory(courseCatId) {
            var url = dataConstants.ADVANCE_SEARCH_URL + 'get-course-by-category?id=' + courseCatId;
            return apiHttpService.GET(url);
        }

        function getCourseBySubCategory(courseSubCatId) {
            var url = dataConstants.ADVANCE_SEARCH_URL + 'get-course-by-sub-category?id=' + courseSubCatId;
            return apiHttpService.GET(url);
        }
        function searchOfficersResult() {
            var url = dataConstants.ADVANCE_SEARCH_URL + 'search-officers-result';
            return apiHttpService.GET(url);
        }



        function saveCheckedValue(checked,value) {
            var url = dataConstants.ADVANCE_SEARCH_URL + 'save-checked-column?check='+checked+'&value='+value;
            return apiHttpService.POST(url, {});
        }

        function deleteCheckedColumn() {
            var url = dataConstants.ADVANCE_SEARCH_URL + 'delete-checked-column';
            return apiHttpService.DELETE(url);
        }


    }
})();