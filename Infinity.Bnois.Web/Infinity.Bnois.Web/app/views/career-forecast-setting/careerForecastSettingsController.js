(function () {

    'use strict';
    var controllerId = 'careerForecastSettingsController';
    angular.module('app').controller(controllerId, careerForecastSettingsController);
    careerForecastSettingsController.$inject = ['$state', 'careerForecastSettingService', 'notificationService', '$location'];

    function careerForecastSettingsController($state, careerForecastSettingService, notificationService, location) {

        /* jshint validthis:true */
        var vm = this;
        vm.careerForecastSettings = [];
        vm.addCareerForecastSetting = addCareerForecastSetting;
        vm.updateCareerForecastSetting = updateCareerForecastSetting;
        vm.deleteCareerForecastSetting = deleteCareerForecastSetting;
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
            careerForecastSettingService.getCareerForecastSettings(vm.pageSize, vm.pageNumber, vm.searchText).then(function (data) {
                vm.careerForecastSettings = data.result;
                vm.total = data.total; vm.permission = data.permission;
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function addCareerForecastSetting() {
            $state.go('career-forecast-setting-create');
        }

        function updateCareerForecastSetting(careerForecastSetting) {
            $state.go('career-forecast-setting-modify', { id: careerForecastSetting.careerForecastSettingId});
        }

        function deleteCareerForecastSetting(careerForecastSetting) {
            careerForecastSettingService.deleteCareerForecastSetting(careerForecastSetting.careerForecastSettingId).then(function (data) {
                $state.go($state.current, { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });  
            });
        }

        function pageChanged() {
            $state.go('career-forecast-setting-list', { pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
        }

        function onSearch() {
            vm.pageNumber = 1;
            pageChanged();
        }
    }

})();
