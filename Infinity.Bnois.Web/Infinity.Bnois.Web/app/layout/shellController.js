(function () {
    'use strict';

    var controllerId = 'ShellController';
    angular.module('app').controller(controllerId, ['$state','$location', '$rootScope', 'common', 'config', '$routeParams', shell]);

    function shell($state, $location, $rootScope, common,config, $routeParams) {

        var vm = this;

        vm.check = false;

        //vm.showLayout = true;


        if ($location.path() === '/')
        {
            $state.go('dashboard');

        }
        vm.title = 'shell';
        var events = config.events;
        vm.busyMessage = 'Please wait ...';

        vm.spinnerOptions = {
            radius: 30,
            lines: 7,
            length: 0,
            width: 15,
            speed: 1.7,
            corners: 1.0,
            trail: 70,
            color: '#3BBF38'
        };

       
        activate();

        function activate() {
            common.activateController([], controllerId);
        }
       
    }
})();
