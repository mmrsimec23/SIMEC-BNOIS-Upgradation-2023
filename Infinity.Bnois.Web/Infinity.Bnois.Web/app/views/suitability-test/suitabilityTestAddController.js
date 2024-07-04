(function () {

    'use strict';

    var controllerId = 'suitabilityTestAddController';

    angular.module('app').controller(controllerId, suitabilityTestAddController);
    suitabilityTestAddController.$inject = ['$stateParams', 'suitabilityTestService',  'notificationService', '$state'];

    function suitabilityTestAddController($stateParams, suitabilityTestService, notificationService, $state) {
        var vm = this;
        vm.suitabilityTestId = 0;
        vm.suitabilityTestType = 0;
        vm.title = 'ADD MODE';
        vm.typeName = '';
        vm.suitabilityTest = {};
        vm.suitabilityTestTypes = [];
        vm.majorCourseForecastList = [];

        vm.batches = [];

        vm.saveOFficerListByBatch = saveOFficerListByBatch;
        vm.updatesuitabilityTest = updatesuitabilityTest;
        vm.deletesuitabilityTest = deletesuitabilityTest;
        vm.deletesuitabilityTestTypeOfficerList = deletesuitabilityTestTypeOfficerList;

        vm.searchText = "";
        vm.pageSize = 50;
        vm.pageNumber = 1;
        vm.total = 0;
        vm.type = 0;

        vm.saveButtonText = 'Save';
        vm.save = save;
        vm.back = back;
        vm.close = close;
        vm.suitabilityTestForm = {};

        vm.offices = [];
        //vm.localSearch = localSearch;
        //vm.selected = selected;

        vm.suitabilityTestTypeList = suitabilityTestTypeList;
        vm.getDataList = getDataList;
        vm.getBatchList = getBatchList;

        if ($stateParams.id !== undefined && $stateParams.id !== null) {
            vm.suitabilityTestId = $stateParams.id;
            vm.saveButtonText = 'Update';
            vm.title = 'UPDATE MODE';
        }

        if ($stateParams.type !== undefined && $stateParams.type !== null) {
            vm.suitabilityTestType = $stateParams.type;
            if (vm.suitabilityTestType == 1) {
                vm.typeName = 'HASB';
            }
            if (vm.suitabilityTestType == 2) {
                vm.typeName = 'SASB';
            }
        }

        Init();
        function Init() {
            suitabilityTestService.getsuitabilityTest(vm.suitabilityTestId).then(function (data) {
                vm.suitabilityTest = data.result.suitabilityTests;

                vm.batches = data.result.batches;
                vm.suitabilityTestTypes = data.result.suitabilityTestTypes;
                if (vm.suitabilityTestId !== 0 && vm.suitabilityTestId !== '') {

                    if (vm.suitabilityTest.expiryDate != null) {
                        vm.suitabilityTest.expiryDate = new Date(data.result.suitabilityTests.expiryDate);
                    }
                }

            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
            getDataList();
        };

        function getDataList() {
            suitabilityTestService.getsuitabilityTests(vm.suitabilityTestType, vm.pageSize, vm.pageNumber, vm.searchText).then(function (data) {
                vm.majorCourseForecastList = data.result;
                vm.total = data.total; vm.permission = data.permission;
                console.log(vm.majorCourseForecastList)
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function saveOFficerListByBatch() {
            console.log(vm.suitabilityTest)
            if (vm.suitabilityTest.batchId > 0) {
                suitabilityTestService.saveSuitabilityTestTypeList(vm.suitabilityTestType, vm.suitabilityTest.batchId).then(function (data) {
                    close();
                },
                    function (errorMessage) {
                        notificationService.displayError(errorMessage.message);
                    });
            } else {
                notificationService.displayError("Please Select Batch!!");
            }
            
        }
        function getBatchList() {
            suitabilityTestService.getsuitabilityTests().then(function (data) {
                vm.majorCourseForecastList = data.result;
                vm.total = data.total; vm.permission = data.permission;
                console.log(vm.majorCourseForecastList)
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function deletesuitabilityTest(suitabilityTest) {
            suitabilityTestService.deletesuitabilityTest(suitabilityTest.suitabilityTestId).then(function (data) {
                $state.go($state.current, { type: vm.suitabilityTestType, pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
            });
        }

        function deletesuitabilityTestTypeOfficerList() {
            suitabilityTestService.deletesuitabilityTestTypeOfficerList(vm.suitabilityTestType).then(function (data) {
                $state.go($state.current, { type: vm.suitabilityTestType, pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
            });
        }
        function back() {
            if (vm.suitabilityTestType == '1') {
                $state.go('hasb-suitability-tests');
            }
            if (vm.suitabilityTestType == '2') {
                $state.go('sasb-suitability-tests');
            }
            
        }
        function save() {
            
            if (vm.suitabilityTest.employee.employeeId > 0) {
                vm.suitabilityTest.employeeId = vm.suitabilityTest.employee.employeeId;
            } else {
                notificationService.displayError("Please Search Valid Officer by Pno!!");
            }

            if (vm.suitabilityTestId !== 0 && vm.suitabilityTestId !== '') {
                updatesuitabilityTest();
            } else {
                insertsuitabilityTest();
            }
        }

        function insertsuitabilityTest() {
            vm.suitabilityTest.suitabilityTestType = vm.suitabilityTestType;
            suitabilityTestService.savesuitabilityTest(vm.suitabilityTest).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }
        function updatesuitabilityTest() {
            suitabilityTestService.updatesuitabilityTest(vm.suitabilityTestId, vm.suitabilityTest).then(function (data) {
                close();
            },
                function (errorMessage) {
                    notificationService.displayError(errorMessage.message);
                });
        }

        function close() {
            $state.go($state.current, { type: vm.suitabilityTestType, pn: vm.pageNumber, ps: vm.pageSize, q: vm.searchText }, { reload: true, inherit: false });
        }
        function suitabilityTestTypeList() {
            suitabilityTestService.suitabilityTestTypeList().then(function (data) {
                vm.suitabilityTestTypes = data.result;
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
        //    vm.suitabilityTest.officeId = object.originalObject.value;

        //}


    }



})();
