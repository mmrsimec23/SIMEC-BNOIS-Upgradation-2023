﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface ICurrentStatusService
    {
        Object GetGeneralInformation(string pNo);
        List<Object> GetCivilAcademicQualification(string pNo);
        List<Object> GetSecurityClearance(string pNo);
        List<Object> GetCourseAttended(string pNo);
        List<Object> GetForeignCourseAttended(string pNo);
        Object GetForeignCourseVisitGrandTotal(string pNo);
        List<Object> GetExamTestResult(string pNo);
        List<Object> GetCareerForecast(string pNo);
        List<Object> GetCarLoanInfo(string pNo);
        List<Object> GetPunishmentDiscipline(string pNo);
        List<Object> GetCommendationAppreciation(string pNo,int type);
        List<Object> GetAward(string pNo);
        List<Object> GetMedal(string pNo);
        List<Object> GetPublication(string pNo);
        List<Object> GetCleanService(string pNo);
        List<Object> GetChildren(string pNo);
        List<Object> GetSibling(string pNo);
        List<Object> GetNextOfKin(string pNo);
        dynamic GetHeirInfo(string pNo);

        List<Object> GetOprGrading(string pNo);
        List<Object> GetForeignVisit(string pNo);
        List<Object> GetParentInfo(string pNo);
        List<Object> GetSpouseInfo(string pNo);
        List<Object> GetTransferHistory(string pNo);
        List<Object> GetPromotionHistory(string pNo);

        dynamic GetSeaServices(string pNo);
        Object GetSeaServicesGrandTotal(string pNo);

        dynamic GetZoneServices(string pNo);
        dynamic GetZoneCourseMissionServices(string pNo);
        Object GetZoneServicesGrandTotal(string pNo);
        dynamic GetAdditionalSeaServices(string pNo);
        Object GetAdditionalSeaServicesGrandTotal(string pNo);
        
	    List<Object> GetInstructionalServices(string pNo);
        dynamic GetSeaCommandServices(string pNo);
        Object GetSeaCommandServicesGrandTotal(string pNo);
        List<Object> GetInterOrganizationServices(string pNo);
	    List<Object> GetIntelligenceServices(string pNo);

        List<Object> GetNotifications( string userid,string pNo);

	
		Object GetCurrentStatus(string pNo);
        List<object> GetTemporaryTransferHistory(int transferId);
        List<object> GetPersuasion(string pNo);
        List<object> GetRemark(string pNo);
        List<object> GetCourseFuturePlan(string pNo);
        List<object> GetTransferFuturePlan(string pNo);

        Object GetLeaveInfo(string pNo);
        Object GetISSB(string pNo);
        Object GetBatchPosition(string pNo);
        List<Object> GetMissions(string pNo);
        List<Object> GetForeignProjects(string pNo);
        List<Object> GetHODServices(string pNo);
        List<Object> GetDockyardServices(string pNo);
        List<Object> GetSubmarineServices(string pNo);
        List<Object> GetDeputationServices(string pNo);
        List<Object> GetOutsideServices(string pNo);
        List<Object> GetFamilyPermissionRelationCount(string pNo);
        List<Object> GetFamilyPermissions(string pNo, int relationId);
        List<Object> GetMscEducationQualification(string pNo);
        List<object> GetCostGuardHistory(string pNo);
    }
}