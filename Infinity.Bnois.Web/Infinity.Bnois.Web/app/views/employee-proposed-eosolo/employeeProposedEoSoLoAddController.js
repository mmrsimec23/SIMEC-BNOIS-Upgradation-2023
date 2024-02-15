(function () {

    'use strict';

    var controllerId = 'employeeProposedEoSoLoAddController';

    angular.module('app').controller(controllerId, employeeProposedEoSoLoAddController);
    employeeProposedEoSoLoAddController.$inject = ['$stateParams', 'employeeProposedEolosodloseoService', 'serviceExamService', 'notificationService', '$state'];

    function employeeProposedEoSoLoAddController($stateParams, employeeProposedEolosodloseoService, serviceExamService, notificationService, $state) {
        var vm = this;
        vm.employeeProposedCoxoId = 0;
        vm.title = 'ADD MODE';
        vm.employeeProposedEoSoLo = {};
        vm.coxoTypes = [];
        vm.coxoShipTypes = [];
        vm.coxoAppoinments = [];


        vm.saveButtonText = 'Save';
        vm.save = save;
        vm.close = close;
        vm.employeeProposedEoSoLoForm = {};

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
            employeeProposedEolosodloseoService.getemployeeProposedEolosodloseo(vm.employeeProposedCoxoId, 2).then(function (data) {
                vm.employeeProposedEoSoLo = data.result.employeeCoxoService;
                vm.coxoTypes = data.result.coxoTypes;
                vm.coxoShipTypes = data.result.coxoShipTypes;
                vm.coxoAppoinments = data.result.coxoAppoinments;
                vm.offices = data.result.offices;
                if (vm.employeeProposedCoxoId !== 0 && vm.employeeProposedCoxoId !== '') {

                    if (vm.employeeProposedEoSoLo.proposedDate != null) {
                        vm.employeeProposedEoSoLo.proposedDate = new Date(data.result.employeeCoxoService.proposedDate);
                    }
                    getOfficeByShiptype(data.result.employeeCoxoService.shipType);
                }

            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        };

        function save() {
            if (vm.employeeProposedEoSoLo.employee.employeeId > 0) {
                vm.employeeProposedEoSoLo.employeeId = vm.employeeProposedEoSoLo.employee.employeeId;
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
            vm.employeeProposedEoSoLo.type = 2;
            employeeProposedEolosodloseoService.saveemployeeProposedEolosodloseo(vm.employeeProposedEoSoLo).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }
        function updateemployeeProposedCoxo() {
            employeeProposedEolosodloseoService.updateemployeeProposedEolosodloseo(vm.employeeProposedCoxoId, vm.employeeProposedEoSoLo).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function close() {
            $state.go('employee-proposed-eosolos');
        }
        function getOfficeByShiptype(type) {
            employeeProposedEolosodloseoService.GetEmployeeEolosodloseoServiceOfficeList(type).then(function (data) {
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
        //    vm.employeeProposedEoSoLo.officeId = object.originalObject.value;

        //}


    }



})();
