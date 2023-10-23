(function () {
    'use strict';
    angular.module('app').service('officeService', ['dataConstants', 'apiHttpService', officeService]);

    function officeService(dataConstants, apiHttpService) {
        var service = {
            getOffices: getOffices,
            getOfficesForSearch: getOfficesForSearch,
            getOfficeStructures: getOfficeStructures,
            getOffice: getOffice,
            getOfficeSelectModelByType: getOfficeSelectModelByType,
            saveOffice: saveOffice,
            updateOffice: updateOffice,
            deleteOffice: deleteOffice,
            getMinistryOffices: getMinistryOffices,
            getChildOfficeSelectModels: getChildOfficeSelectModels,
            getOfficeAppointmentDetails: getOfficeAppointmentDetails,
            getOfficerListByBatch: getOfficerListByBatch,
            getOfficeSelectModelByShip: getOfficeSelectModelByShip


        };

        return service;
        function getOffices() {
            var url = dataConstants.OFFICE_URL + 'get-offices';
            return apiHttpService.GET(url);
        }

        function getOfficesForSearch() {
            var url = dataConstants.OFFICE_URL + 'get-office-for-office-search';
            return apiHttpService.GET(url);
        }
        function getOfficeStructures() {
            var url = dataConstants.OFFICE_URL + 'get-office-structures';
            return apiHttpService.GET(url);
        }

        function getOffice(id) {
            var url = dataConstants.OFFICE_URL + 'get-office?id=' + id;
            return apiHttpService.GET(url);
        }
        function getOfficeSelectModelByType(type) {
            var url = dataConstants.OFFICE_URL + 'get-office-select-model-by-type?type=' + type;
            return apiHttpService.GET(url);
        }


        function getMinistryOffices() {
            var url = dataConstants.OFFICE_URL + 'get-ministry-office-select-models';
            return apiHttpService.GET(url);
        }

        function getOfficeSelectModelByShip(ship) {
            var url = dataConstants.OFFICE_URL + 'get-office-select-model-by-ship?ship='+ship;
            return apiHttpService.GET(url);
        }

     


        function getChildOfficeSelectModels(parentId) {
            var url = dataConstants.OFFICE_URL + 'get-child-office-select-models?parentId='+parentId;
            return apiHttpService.GET(url);
        }

        function getOfficeAppointmentDetails(officeId) {
            var url = dataConstants.OFFICE_URL + 'get-office-appointment-details?officeId=' + officeId;
            return apiHttpService.GET(url);
        }

        function getOfficerListByBatch(batchId) {
            var url = dataConstants.OFFICE_URL + 'officers-result-by-batch?batchId=' + batchId;
            return apiHttpService.GET(url);
        }


        function saveOffice(data) {
            var url = dataConstants.OFFICE_URL + 'save-office';
            return apiHttpService.POST(url, data);
        }

        function updateOffice(id, data) {
            var url = dataConstants.OFFICE_URL + 'update-office/' + id;
            return apiHttpService.PUT(url, data);
        }

        function deleteOffice(id) {
            var url = dataConstants.OFFICE_URL + 'delete-office/' + id;
            return apiHttpService.DELETE(url);
        }
    }
})();