(function () {
    'use strict';
    angular.module('app').service('overviewOfficersDeploymentService', ['dataConstants', 'apiHttpService', overviewOfficersDeploymentService]);

    function overviewOfficersDeploymentService(dataConstants, apiHttpService) {
        var service = {
            getOverviewOfficersDeployments: getOverviewOfficersDeployments,
            getOverviewOfficersDeployment: getOverviewOfficersDeployment,
            saveOverviewOfficersDeployment: saveOverviewOfficersDeployment,
            updateOverviewOfficersDeployment: updateOverviewOfficersDeployment,
            deleteOverviewOfficersDeployment: deleteOverviewOfficersDeployment
        };

        return service;
        function getOverviewOfficersDeployments(pageSize, pageNumber,searchText) {
            var url = dataConstants.OODENTRY_URL + 'get-overview-officers-deployment-entry-list?ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText;
            return apiHttpService.GET(url);
        }

        function getOverviewOfficersDeployment(id) {
            var url = dataConstants.OODENTRY_URL + 'get-overview-officers-deployment-entry?id=' + id;
            return apiHttpService.GET(url);
        }

        function saveOverviewOfficersDeployment(data) {
            var url = dataConstants.OODENTRY_URL + 'save-overview-officers-deployment-entry';
            return apiHttpService.POST(url, data);
        }

        function updateOverviewOfficersDeployment(id, data) {
            var url = dataConstants.OODENTRY_URL + 'update-overview-officers-deployment-entry/' + id;
            return apiHttpService.PUT(url, data);
        }

        function deleteOverviewOfficersDeployment(id) {
            var url = dataConstants.OODENTRY_URL + 'delete-overview-officers-deployment-entry/' + id;
            return apiHttpService.DELETE(url);
        }
        
    }
})();