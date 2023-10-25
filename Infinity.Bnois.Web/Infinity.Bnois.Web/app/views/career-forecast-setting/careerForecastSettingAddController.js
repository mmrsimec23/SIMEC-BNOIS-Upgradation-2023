(function () {

    'use strict';

    var controllerId = 'careerForecastSettingAddController';

    angular.module('app').controller(controllerId, careerForecastSettingAddController);
    careerForecastSettingAddController.$inject = ['$stateParams', 'careerForecastSettingService', 'notificationService', '$state'];

    function careerForecastSettingAddController($stateParams, careerForecastSettingService, notificationService, $state) {
        var vm = this;
        vm.careerForecastSettingId = 0;
        vm.title = 'ADD MODE';
        vm.careerForecastSetting = {};
        vm.saveButtonText = 'Save';
        vm.save = save;
        vm.close = close;
        vm.careerForecastSettingForm = {};

        vm.branches = [];

        if ($stateParams.id !== undefined && $stateParams.id !== null) {
            vm.careerForecastSettingId = $stateParams.id;
            vm.saveButtonText = 'Update';
            vm.title = 'UPDATE MODE';
        }

        Init();
        function Init() {
            careerForecastSettingService.getCareerForecastSetting(vm.careerForecastSettingId).then(function (data) {
                vm.careerForecastSetting = data.result.careerForecastSetting;
                vm.branches = data.result.branches;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        };

        function save() {
            if (vm.careerForecastSettingId !== 0 && vm.careerForecastSettingId !== '') {
                updateCareerForecastSetting();
            } else {  
                insertCareerForecastSetting();
            }
        }

        function insertCareerForecastSetting() {
            careerForecastSettingService.saveCareerForecastSetting(vm.careerForecastSetting).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }
        function updateCareerForecastSetting() {
            careerForecastSettingService.updateCareerForecastSetting(vm.careerForecastSettingId, vm.careerForecastSetting).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function close() {
            $state.go('career-forecast-setting-list');
        }
    }
})();
