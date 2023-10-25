
(function () {
  "use strict";
  var controllerId = 'TopNavigationController';

    angular.module("app").controller(controllerId, ['OidcManager', 'userService','notificationService', TopNavigationController]);

    function TopNavigationController(OidcManager, userService, notificationService) {
    var vm = this;
       vm.accountInfo = {};

      vm.manager = OidcManager.OidcTokenManager();
//    vm.changePassword = changePassword;
    vm.userName = vm.manager.profile.preferred_username;
    vm.logOut = function () {
      vm.manager.removeToken();
      window.location = "/";
    }

    vm.logOutOfIdSrv = function () {
      vm.manager.redirectForLogout();
    }


        vm.changePassword= function (accountInfo) {
           
          userService.changePassword(accountInfo).then(function (data) {
                  notificationService.displaySuccess('Password Changed Successfully.!!');
              },
              function (errorMessage) {
                  notificationService.displayError(errorMessage.message);
              });

      }

    

  }

}());
