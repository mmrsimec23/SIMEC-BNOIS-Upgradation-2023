(function () {

    'use strict';
    var controllerId = 'careerForecastAddController';
    angular.module('app').controller(controllerId, careerForecastAddController);
    careerForecastAddController.$inject = ['$stateParams', '$state', 'employeeCareerForecastService', 'notificationService'];

    function careerForecastAddController($stateParams, $state, employeeCareerForecastService, notificationService) {
        /* jshint validthis:true */
        var vm = this;
        vm.careerForecastId = 0;
        vm.careerForecast = {};
        vm.examCategory = {};
        vm.careerForecastSettingList = [];
        vm.title = 'ADD MODE';
        //vm.getSubjectsByExamination = getSubjectsByExamination;
        //vm.getExaminationByExamCategory = getExaminationByExamCategory;
        //vm.getInstituteByBoardOrUniversity = getInstituteByBoardOrUniversity

        vm.saveButtonText = 'Save';
        vm.save = save;
        vm.close = close;
        vm.careerForecastForm = {};

        vm.forecastStatusList = [
            { 'value': 1, 'text': 'Complete' }, { 'value': 2, 'text': 'Bypass' }
        ];
        //vm.isShowGPA = true;
        //vm.showGPA = showGPA;
        //function showGPA(resultId) {
        //   // var gpaCodes = bpscDataConstants.gpaCodes;
        //    var gpaResult = vm.results.find(m => m.value == resultId);
        //    if (gpaResult.value == 28 || gpaResult.value == 29) {
        //        vm.isShowGPA = true;
        //    } else {
        //        vm.isShowGPA = false;
        //        vm.education.gpa = 0;
        //    }
        //}
        

        if ($stateParams.id !== undefined && $stateParams.id !== null) {
            vm.employeeId = $stateParams.id;
        }
        if ($stateParams.careerForecastId > 0) {
            vm.careerForecastId = $stateParams.careerForecastId;
            vm.title = 'UPDATE MODE';
        }
        init();
        function init() {
            employeeCareerForecastService.getCareerForecastByEmployee(vm.employeeId, vm.careerForecastId).then(function (data) {
                vm.careerForecast = data.result.careerForecast;

                vm.careerForecastSettingList = data.result.careerForecastSettingList;
                //vm.examinations = data.result.examinations;
                //vm.boards = data.result.boards;
                //vm.institutes = data.result.institutes;
                //vm.results = data.result.results;
                //vm.subjects = data.result.subjects;
                //vm.years = data.result.years;
                //vm.courseDurations = data.result.courseDurations;
                //vm.grades = data.result.grades;
                //vm.isShowDuration = true;
                //if (vm.education.resultId != null) {
                //    showGPA(vm.education.resultId);
                //}
                
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }



        function save() {
            if (vm.careerForecastId > 0 && vm.careerForecastId !== '') {
                updateCareerForecast();
            } else {
                insertCareerForecast();
            }
        }
        function insertCareerForecast() {
            employeeCareerForecastService.saveCareerForecastByEmployee(vm.employeeId, vm.careerForecast).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }
        function updateCareerForecast() {
            employeeCareerForecastService.updateEmployeeCareerForecast(vm.careerForecastId, vm.careerForecast).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function close() {
            $state.go('employee-tabs.employee-career-forecasts');
        }
    }

})();
