(function () {

    'use strict';
    var controllerId = 'careerForecastsController';
    angular.module('app').controller(controllerId, careerForecastsController);
    careerForecastsController.$inject = ['$stateParams', '$state', 'employeeCareerForecastService', 'notificationService'];

    function careerForecastsController($stateParams, $state, employeeCareerForecastService, notificationService) {
        /* jshint validthis:true */
        var vm = this;
        vm.careerForecastId = 0;
        vm.careerForecasts = [];
        vm.title = 'Career Forecast';
        vm.addCareerForecast = addCareerForecast;
        vm.updateCareerForecast = updateCareerForecast;
        vm.deleteCareerForecast = deleteCareerForecast;

        if ($stateParams.employeeId !== undefined && $stateParams.employeeId !== null) {
            vm.employeeId = $stateParams.employeeId;
        }

        init();
        function init() {
            employeeCareerForecastService.getEmployeeCareerForecastsByEmployee(vm.employeeId).then(function (data) {
                vm.careerForecasts = data.result;
                vm.permission = data.permission;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }


        function addCareerForecast() {
            $state.go('employee-tabs.employee-career-forecast-create', { id: vm.employeeId, careerForecastId: vm.careerForecastId });
        }

        function deleteCareerForecast(careerForecast) {
            employeeCareerForecastService.deleteEmployeeCareerForecast(careerForecast.careerForecastId).then(function (data) {
                employeeCareerForecastService.getEmployeeCareerForecastsByEmployee(vm.employeeId).then(function(data) {
                    vm.careerForecasts = data.result;
                });
                $state.go('employee-tabs.employee-career-forecasts');
            });
        }


        function updateCareerForecast(careerForecast) {
            $state.go('employee-tabs.employee-career-forecast-modify', { id: vm.employeeId, careerForecastId: careerForecast.careerForecastId });
        }


    }

})();
