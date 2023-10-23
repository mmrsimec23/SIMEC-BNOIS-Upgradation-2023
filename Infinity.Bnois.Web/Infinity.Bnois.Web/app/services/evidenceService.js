(function () {
    'use strict';
    angular.module('app').service('evidenceService', ['dataConstants', 'apiHttpService', evidenceService]);

    function evidenceService(dataConstants, apiHttpService) {
        var service = {
            getEvidences: getEvidences,
            getEvidence: getEvidence,
            saveEvidence: saveEvidence,
            updateEvidence: updateEvidence,
            deleteEvidence: deleteEvidence
        };

        return service;
        function getEvidences(pageSize, pageNumber, searchText) {
            var url = dataConstants.EVIDENCE_URL + 'get-evidences?ps=' + pageSize + "&pn=" + pageNumber + "&qs=" + searchText;
            return apiHttpService.GET(url);
        }

        function getEvidence(evidenceId) {
            var url = dataConstants.EVIDENCE_URL + 'get-evidence?id=' + evidenceId;
            return apiHttpService.GET(url);
        }

        function saveEvidence(data) {
            var url = dataConstants.EVIDENCE_URL + 'save-evidence';
            return apiHttpService.POST(url, data);
        }

        function updateEvidence(evidenceId, data) {
            var url = dataConstants.EVIDENCE_URL + 'update-evidence/' + evidenceId;
            return apiHttpService.PUT(url, data);
        }

        function deleteEvidence(evidenceId) {
            var url = dataConstants.EVIDENCE_URL + 'delete-evidence/' + evidenceId;
            return apiHttpService.DELETE(url);
        }


    }
})();