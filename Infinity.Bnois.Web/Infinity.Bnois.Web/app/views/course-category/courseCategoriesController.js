﻿(function () {

    'use strict';
    var controllerId = 'courseCategoriesController';
    angular.module('app').controller(controllerId, courseCategoriesController);
    courseCategoriesController.$inject = ['$state', 'courseCategoryService', 'notificationService', '$location'];

    function courseCategoriesController($state, courseCategoryService, notificationService, location) {

        /* jshint validthis:true */
        var vm = this;
        vm.courseCategories = [];
        vm.addCourseCategory = addCourseCategory;
        vm.updateCourseCategory = updateCourseCategory;
        vm.deleteCourseCategory = deleteCourseCategory;
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
            courseCategoryService.getCourseCategories(vm.pageSize, vm.pageNumber, vm.searchText).then(function (data) {
                vm.courseCategories = data.result;
                vm.total = data.total; vm.permission = data.permission;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function addCourseCategory() {
            $state.go('course-category-create');
        }

        function updateCourseCategory(courseCategory) {
            $state.go('course-category-modify', { id: courseCategory.courseCategoryId});
        }

        function deleteCourseCategory(courseCategory) {
            courseCategoryService.deleteCourseCategory(courseCategory.courseCategoryId).then(function (data) {
                $state.go($state.current, { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
            });
        }

        function pageChanged() {
            $state.go('course-categories', { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
        }

        function onSearch() {
            vm.pageNumber = 1;
            pageChanged();
        }
    }

})();
