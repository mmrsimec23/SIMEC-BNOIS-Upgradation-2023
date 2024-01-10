using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.Data;

namespace Infinity.Bnois.ApplicationService.Implementation
{
	public class CurrentStatusService : ICurrentStatusService
	{
		private readonly IBnoisRepository<Employee> employeeRepository;
		private readonly IBnoisRepository<EmployeeGeneral> employeeGeneralRepository;
		private readonly IBnoisRepository<HeirType> heirTypeRepository;
		private readonly IBnoisRepository<Zone> zoneRepository;
		public CurrentStatusService(IBnoisRepository<Employee> employeeRepository, IBnoisRepository<HeirType> heirTypeRepository, IBnoisRepository<Zone> zoneRepository, IBnoisRepository<EmployeeGeneral> employeeGeneralRepository)
		{
			this.employeeRepository = employeeRepository;
			this.heirTypeRepository = heirTypeRepository;
			this.zoneRepository = zoneRepository;
			this.employeeGeneralRepository = employeeGeneralRepository;
		}

		public object GetGeneralInformation(string pNo)
		{
			DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetAnnx1OfficerGeneralReport] '{0}' ", pNo));
			return dataTable.ToJson().FirstOrDefault();
		}

		public List<object> GetCivilAcademicQualification(string pNo)
		{
			DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetPersonalityBriefByCivilEducationReport] '{0}' ", pNo));

			return dataTable.ToJson().ToList();
		}
		public List<object> GetSecurityClearance(string pNo)
		{
			DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetOfficerSecurityClearance] '{0}' ", pNo));

			return dataTable.ToJson().ToList();
		}


		public List<object> GetCourseAttended(string pNo)
		{
			DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetAnnx6CourseReport] '{0}' ", pNo));

			return dataTable.ToJson().ToList();
		}

		public List<object> GetForeignCourseAttended(string pNo)
		{
			DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetAnnx6CourseForeignReport] '{0}' ", pNo));

			return dataTable.ToJson().ToList();
		}


        public object GetForeignCourseVisitGrandTotal(string pNo)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetForeignCourseVisitGrandTotal] '{0}' ", pNo));
            return dataTable.ToJson().FirstOrDefault();
        }
        public List<object> GetExamTestResult(string pNo)
		{
			DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetExamTestResult] '{0}' ", pNo));

			return dataTable.ToJson().ToList();
		}
        public List<object> GetUnmDeferment(string pNo)
		{
			DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetUnmDeferment] '{0}' ", pNo));

			return dataTable.ToJson().ToList();
		}


        public List<object> GetCareerForecast(string pNo)
		{
			Employee e = employeeRepository.FindOne(x=> x.PNo == pNo);
			EmployeeGeneral eg = employeeGeneralRepository.FindOne(x => x.EmployeeId == e.EmployeeId);
			DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetCareerForecast] '{0}',{1} ", pNo, eg.BranchId));

			return dataTable.ToJson().ToList();
		}
		
		public List<object> GetCarLoanInfo(string pNo)
		{
			DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetCarLoanInfo] '{0}' ", pNo));

			return dataTable.ToJson().ToList();
		}

		public List<object> GetPunishmentDiscipline(string pNo)
		{
			DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetPunishmentDiscipline] '{0}' ", pNo));

			return dataTable.ToJson().ToList();
		}

		public List<object> GetCommendationAppreciation(string pNo,int type)
		{
			DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetCommendationAppreciation] '{0}','{1}' ", pNo,type));

			return dataTable.ToJson().ToList();
		}

		public List<object> GetAward(string pNo)
		{
			DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetAward] '{0}' ", pNo));

			return dataTable.ToJson().ToList();
		}

		public List<object> GetMedal(string pNo)
		{
			DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetMedal] '{0}' ", pNo));

			return dataTable.ToJson().ToList();
		}

		public List<object> GetPublication(string pNo)
		{
			DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetPublication] '{0}' ", pNo));

			return dataTable.ToJson().ToList();
		}


        public List<object> GetCleanService(string pNo)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetCSA] '{0}' ", pNo));

            return dataTable.ToJson().ToList();
        }



        public List<object> GetChildren(string pNo)
		{
			DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetChildren] '{0}' ", pNo));

			return dataTable.ToJson().ToList();
		}

		public List<object> GetSibling(string pNo)
		{
			DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetSibling] '{0}' ", pNo));

			return dataTable.ToJson().ToList();
		}

		public List<object> GetNextOfKin(string pNo)
		{
			DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetNextOfKin] '{0}' ", pNo));

			return dataTable.ToJson().ToList();
		}


		private List<object> GetHeir(string pNo, int heirTypeId)
		{
			DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetHeir] '{0}',{1} ", pNo, heirTypeId));

			return dataTable.ToJson().ToList();
		}

		public dynamic GetHeirInfo(string pno)
		{
			var heirTypes = heirTypeRepository.Where(x => x.IsActive).ToList();
            Dictionary<string, dynamic> keyValues = new Dictionary<string, dynamic>();
			foreach (var heirType in heirTypes)
			{
				var vaHeirs = GetHeir(pno, heirType.HeirTypeId);

			    if (vaHeirs.Count >0)
			    {
			        keyValues.Add(heirType.Name, vaHeirs);
                }
				
			}
			return keyValues;
		}

	    public List<object> GetOprGrading(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetOprGrading] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
	    }
	    public List<object> GetForeignVisit(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetForeignVisit] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
	    }
	    public List<object> GetParentInfo(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetParentsInfo] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
	    }


	    public List<object> GetSpouseInfo(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetSpouseInfo] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
	    }

	    public List<object> GetTransferHistory(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [SpGetTransferHistory] '{0}'", pNo));

	        return dataTable.ToJson().ToList();
	    }

	    public List<object> GetPromotionHistory(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [SpGetPromotionHistory] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
	    }

	    private List<object> GetAdditionalSeaServicesByType(string pNo, int type)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetAdditionalSeaServices] '{0}',{1} ", pNo, type));

	        return dataTable.ToJson().ToList();
	    }

	    public object GetAdditionalSeaServicesGrandTotal(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetAdditionalSeaServicesGrandTotal] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
	    }


	    public dynamic GetAdditionalSeaServices(string pno)
	    {

	        Dictionary<string, dynamic> keyValues = new Dictionary<string, dynamic>();
	        foreach (var type in Enum.GetValues((typeof(ShipType))))
	        {
	            var results = GetAdditionalSeaServicesByType(pno, (int)type);

	            var shipType = String.Empty;
	            if (results.Count > 0)
	            {
	                if ((int)type == (int)ShipType.Small)
	                {
	                    shipType = "Small";

	                }
	                else if ((int)type == (int)ShipType.Medium)
	                {
	                    shipType = "Medium";
	                }
	                else
	                {
	                    shipType = "Large";
	                }


	                keyValues.Add(shipType, results);
	            }

	        }
	        return keyValues;
	    }


	    private List<object> GetSeaServicesByType(string pNo, int type)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetSeaServices] '{0}',{1} ", pNo, type));

	        return dataTable.ToJson().ToList();
	    }

	    public object GetSeaServicesGrandTotal(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format(@"declare @GrandTotalDuration varchar(100)
exec [spGetSeaServicesGrandTotal] '{0}',@GrandTotalDuration output
select isnull(@GrandTotalDuration,'') as GrandTotalDuration", pNo));

	        return dataTable.ToJson().ToList();
	    }


	    public dynamic GetSeaServices(string pno)
	    {

	        Dictionary<string, dynamic> keyValues = new Dictionary<string, dynamic>();
	        foreach (var type in Enum.GetValues((typeof(ShipType))))
	        {
	            var results = GetSeaServicesByType(pno, (int)type);

	            var shipType = String.Empty;
	            if (results.Count > 0)
	            {
	                if ((int)type == (int)ShipType.Small)
	                {
	                    shipType = "Small";

	                }
	                else if ((int)type == (int)ShipType.Medium)
	                {
	                    shipType = "Medium";
	                }
	                else
	                {
	                    shipType = "Large";
	                }


	                keyValues.Add(shipType, results);
	            }

	        }
	        return keyValues;
	    }


        public List<object> GetInstructionalServices(string pNo)
		{
			DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetInstructionalServices] '{0}' ", pNo));

            return dataTable.ToJson().ToList();
		}


		private List<object> GetSeaCommandServicesByType(string pNo,int type)
		{
			DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetSeaCommandServices] '{0}',{1} ", pNo,type));

			return dataTable.ToJson().ToList();
		}

	    public object GetSeaCommandServicesGrandTotal(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetSeaCommandServicesGrandTotal] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
	    }


        public dynamic GetSeaCommandServices(string pno)
	    {
	       
	        Dictionary<string, dynamic> keyValues = new Dictionary<string, dynamic>();
	        foreach (var type in Enum.GetValues((typeof(ShipType))))
	        {
	            var results = GetSeaCommandServicesByType(pno, (int)type);

	            var shipType = String.Empty;
	            if (results.Count > 0)
	            {
	                if ((int) type == (int)ShipType.Small)
	                {
	                    shipType = "Small";

	                }else if ((int) type == (int) ShipType.Medium)
	                {
	                    shipType = "Medium";
                    }
	                else
	                {
	                    shipType = "Large";
                    }


	                keyValues.Add(shipType, results);
	            }

	        }
	        return keyValues;
	    }



	    private List<object> GetZoneServicesByType(string pNo, int type)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetZoneServices] '{0}',{1} ", pNo, type));

	        return dataTable.ToJson().ToList();
	    }

	    public object GetZoneServicesGrandTotal(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetZoneServicesGrandTotal] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
	    }


	    public dynamic GetZoneServices(string pno)
	    {
	        var zones = zoneRepository.Where(x => x.IsActive).ToList();
            Dictionary<string, dynamic> keyValues = new Dictionary<string, dynamic>();
	        foreach (var item in zones)
	        {
	            var results = GetZoneServicesByType(pno, item.ZoneId);
	         
	            if (results.Count > 0)
	            {
	              

	                keyValues.Add(item.Name, results);
	            }

	        }
	        return keyValues;
	    }


		

	    public dynamic GetZoneCourseMissionServices(string pno)
	    {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetZoneCourseMissionServices] '{0}' ", pno));

            return dataTable.ToJson().ToList();
        }
		
	    public List<object> GetMscEducationQualification(string pno)
	    {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetMscEducationQualification] '{0}' ", pno));

            return dataTable.ToJson().ToList();
        }



        public List<object> GetInterOrganizationServices(string pNo)	
		{
			DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetInterOrganizationServices] '{0}' ", pNo));

			return dataTable.ToJson().ToList();
		}
		public List<object> GetIntelligenceServices(string pNo)
		{
			DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetIntelligenceServices] '{0}' ", pNo));

			return dataTable.ToJson().ToList();
		}


	    public List<object> GetMissions(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetMissions] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
	    }


	    public List<object> GetForeignProjects(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetForeignProjects] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
	    }


	    public List<object> GetHODServices(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetHODServices] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
	    }

	    public List<object> GetDockyardServices(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDockyardServices] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
	    }

	    public List<object> GetNsdServices(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetNsdServices] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
	    }

	    public List<object> GetBsdServices(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetBsdServices] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
	    }

	    public List<object> GetBsoServices(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetBsoServices] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
	    }

	    public List<object> GetSubmarineServices(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetSubmarineServices] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
	    }
		
		

	    public List<object> GetDeputationServices(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDeputationServices] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
	    }
	    public List<object> GetShoreCommandServices(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetShoreCommandServices] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
	    }

	    public List<object> GetOutsideServices(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetOutsideServices] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
	    }
		

	    public List<object> GetFamilyPermissionRelationCount(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetFamilyPermissionRelationCount] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
	    }
		

	    public List<object> GetFamilyPermissions(string pNo, int relationId)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetFamilyPermissions] '{0}',{1} ", pNo, relationId));

	        return dataTable.ToJson().ToList();
	    }



        public List<object> GetNotifications( string userid, string pNo)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetNotification] '{0}','{1}' ", userid, pNo));

            return dataTable.ToJson().ToList();
        }


        public object GetCurrentStatus(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetCurrentStatus] '{0}' ", pNo));
	        return dataTable.ToJson().FirstOrDefault();
	    }


	    public object GetISSB(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetISSB] '{0}' ", pNo));
	        return dataTable.ToJson().FirstOrDefault();
	    }



        public object GetBatchPosition(string pNo)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetBatchPosition] '{0}' ", pNo));

            return dataTable.ToJson().FirstOrDefault();
        }

        public object GetLeaveInfo(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetLeaveInfo] '{0}' ", pNo));
	        return dataTable.ToJson().FirstOrDefault();
	    }


        public List<object> GetTemporaryTransferHistory(int transferId)
	    {

            
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [SpGetTemporaryTransferHistory] {0}", transferId));


	        return dataTable.ToJson().ToList();
        }

	    public List<object> GetPersuasion(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetPersuasion] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
        }

	    public List<object> GetRemark(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetRemark] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
	    }

	    public List<object> GetCourseFuturePlan(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetCourseFuturePlan] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
        }

	    public List<object> GetTransferFuturePlan(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetTransferFuturePlan] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
        }
	    public List<object> GetAdminAuthorityService(string pNo)
	    {
	        DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetAdminAuthorityService] '{0}' ", pNo));

	        return dataTable.ToJson().ToList();
        }

        public List<object> GetCostGuardHistory(string pNo)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetCostGuardHistory] '{0}'", pNo));

            return dataTable.ToJson().ToList();
        }
    }
}