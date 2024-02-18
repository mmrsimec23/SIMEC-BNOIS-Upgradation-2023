(function () {

    'use strict';

    var controllerId = 'smallProposedCoxoAddController';

    angular.module('app').controller(controllerId, smallProposedCoxoAddController);
    smallProposedCoxoAddController.$inject = ['$stateParams', 'employeeProposedSmallCoxoService', 'notificationService', '$state'];

    function smallProposedCoxoAddController($stateParams, employeeProposedSmallCoxoService, notificationService, $state) {
        var vm = this;
        vm.employeeProposedCoxoId = 0;
        vm.title = 'ADD MODE';
        vm.employeeProposedCoxo = {};
        vm.coxoTypes = [];
        vm.coxoShipTypes = [];
        vm.coxoAppoinments = [];


        vm.saveButtonText = 'Save';
        vm.save = save;
        vm.close = close;
        vm.employeeProposedCoxoForm = {};

        vm.offices = [];
        //vm.localSearch = localSearch;
        //vm.selected = selected;

        vm.getOfficeByShiptype = getOfficeByShiptype;

        if ($stateParams.id !== undefined && $stateParams.id !== null) {
            vm.employeeProposedCoxoId = $stateParams.id;
            vm.saveButtonText = 'Update';
            vm.title = 'UPDATE MODE';
        }

        Init();
        function Init() {
            getOfficeByShiptype(1);
            employeeProposedSmallCoxoService.getemployeeProposedCoxo(vm.employeeProposedCoxoId,3).then(function (data) {
                vm.employeeProposedCoxo = data.result.employeeCoxoService;
                vm.coxoTypes = data.result.coxoTypes;
                //vm.coxoShipTypes = data.result.coxoShipTypes;
                vm.coxoAppoinments = data.result.coxoAppoinments;
                //vm.offices = data.result.offices;
                if (vm.employeeProposedCoxoId !== 0 && vm.employeeProposedCoxoId !== '') {

                    if (vm.employeeProposedCoxo.proposedDate != null) {
                        vm.employeeProposedCoxo.proposedDate = new Date(data.result.employeeCoxoService.proposedDate);
                    }
                    //getOfficeByShiptype(1);
                }

            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        };

        function save() {
            if (vm.employeeProposedCoxo.employee.employeeId > 0) {
                vm.employeeProposedCoxo.employeeId = vm.employeeProposedCoxo.employee.employeeId;
            } else {
                notificationService.displayError("Please Search Valid Officer by Pno!!");
            }

            if (vm.employeeProposedCoxoId !== 0 && vm.employeeProposedCoxoId !== '') {
                updateemployeeProposedCoxo();
            } else {
                insertemployeeProposedCoxo();
            }
        }

        function insertemployeeProposedCoxo() {
            vm.employeeProposedCoxo.type = 3;
            vm.employeeProposedCoxo.shipType = 1;
            employeeProposedSmallCoxoService.saveemployeeProposedCoxo(vm.employeeProposedCoxo).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }
        function updateemployeeProposedCoxo() {
            employeeProposedSmallCoxoService.updateemployeeProposedCoxo(vm.employeeProposedCoxoId, vm.employeeProposedCoxo).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function close() {
            $state.go('small-proposed-coxos');
        }
        function getOfficeByShiptype(type) {
            employeeProposedSmallCoxoService.GetEmployeeCoxoServiceOfficeList(1).then(function (data) {
                vm.offices = data.result;
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
        //    vm.employeeProposedCoxo.officeId = object.originalObject.value;

        //}


    }



})();
