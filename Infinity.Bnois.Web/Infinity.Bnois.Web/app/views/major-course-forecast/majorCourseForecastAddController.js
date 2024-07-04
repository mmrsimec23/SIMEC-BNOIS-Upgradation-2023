(function () {

    'use strict';

    var controllerId = 'majorCourseForecastAddController';

    angular.module('app').controller(controllerId, majorCourseForecastAddController);
    majorCourseForecastAddController.$inject = ['$stateParams', 'majorCourseForecastService',  'notificationService', '$state'];

    function majorCourseForecastAddController($stateParams, majorCourseForecastService, notificationService, $state) {
        var vm = this;
        vm.majorCourseForecastId = 0;
        vm.title = 'ADD MODE';
        vm.majorCourseForecast = {};
        vm.courseTypes = [];


        vm.saveButtonText = 'Save';
        vm.save = save;
        vm.close = close;
        vm.majorCourseForecastForm = {};

        vm.offices = [];
        //vm.localSearch = localSearch;
        //vm.selected = selected;

        vm.getCourseTypeList = getCourseTypeList;

        if ($stateParams.id !== undefined && $stateParams.id !== null) {
            vm.majorCourseForecastId = $stateParams.id;
            vm.saveButtonText = 'Update';
            vm.title = 'UPDATE MODE';
        }

        Init();
        function Init() {
            majorCourseForecastService.getMajorCourseForecast(vm.majorCourseForecastId).then(function (data) {
                vm.majorCourseForecast = data.result.majorCourseForecasts;
                vm.courseTypes = data.result.courseTypes;
                if (vm.majorCourseForecastId !== 0 && vm.majorCourseForecastId !== '') {

                    if (vm.majorCourseForecast.expiryDate != null) {
                        vm.majorCourseForecast.expiryDate = new Date(data.result.majorCourseForecasts.expiryDate);
                    }
                }

            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        };

        function save() {
            if (vm.majorCourseForecast.employee.employeeId > 0) {
                vm.majorCourseForecast.employeeId = vm.majorCourseForecast.employee.employeeId;
            } else {
                notificationService.displayError("Please Search Valid Officer by Pno!!");
            }

            if (vm.majorCourseForecastId !== 0 && vm.majorCourseForecastId !== '') {
                updatemajorCourseForecast();
            } else {
                insertmajorCourseForecast();
            }
        }

        function insertmajorCourseForecast() {
            majorCourseForecastService.saveMajorCourseForecast(vm.majorCourseForecast).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }
        function updatemajorCourseForecast() {
            majorCourseForecastService.updateMajorCourseForecast(vm.majorCourseForecastId, vm.majorCourseForecast).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function close() {
            $state.go('major-course-forecasts');
        }
        function getCourseTypeList() {
            majorCourseForecastService.getCourseTypeList().then(function (data) {
                vm.courseTypes = data.result;
            });
        }
        //function localSearch(str) {
        //    var matches = [];
        //    vm.offices.forEach(function (transfer) {

        //        if ((transfer.text.toLowerCase().indexOf(str.toString().toLowerCase()) >= 0)) {
        //            matches.push(transfer);

        //        }
        //    });
        //    return matches;
        //}


        //function selected(object) {
        //    vm.majorCourseForecast.officeId = object.originalObject.value;

        //}


    }



})();
