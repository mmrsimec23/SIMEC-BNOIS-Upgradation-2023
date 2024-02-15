(function () {
    'use strict';
    angular.module('app').service('minuteCandidateService', ['dataConstants', 'apiHttpService', minuteCandidateService]);

    function minuteCandidateService(dataConstants, apiHttpService) {
        var service = {
            getMinuteCandidates: getMinuteCandidates,
            getMinuteCandidate: getMinuteCandidate,
            saveMinuteCandidate: saveMinuteCandidate,
            deleteMinuteCandidate: deleteMinuteCandidate
        };

        return service;
        function getMinuteCandidates(minuteId) {
            var url = dataConstants.MINUTE_CANDIDATE_URL + 'get-minute-candidates/'+minuteId;
            return apiHttpService.GET(url);
        }
        function getMinuteCandidate(minuteCandidateId) {
            var url = dataConstants.MINUTE_CANDIDATE_URL + 'get-minute-candidate?id=' + minuteCandidateId;
            return apiHttpService.GET(url);
        }
        

        function saveMinuteCandidate(minuteId,data) {
            var url = dataConstants.MINUTE_CANDIDATE_URL + 'save-minute-candidate/' + minuteId;
            return apiHttpService.POST(url, data);
        }

        function deleteMinuteCandidate(minuteCandidateId) {
            var url = dataConstants.MINUTE_CANDIDATE_URL + 'delete-minute-candidate/' + minuteCandidateId;
            return apiHttpService.DELETE(url);
        }
    }
})();