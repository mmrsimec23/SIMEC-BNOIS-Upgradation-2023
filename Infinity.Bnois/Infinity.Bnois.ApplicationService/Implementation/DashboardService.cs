using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.Data;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class DashboardService : IDashboardService
    {

        private readonly IBnoisRepository<Employee> employeeRepository;

        public DashboardService(IBnoisRepository<Employee> employeeRepository)
        {
            this.employeeRepository = employeeRepository;

        }



        public List<object> GetDashboardOutSideNavy(int officeId)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashboardOutSideNavy] {0} ", officeId));

            return dataTable.ToJson().ToList();
        }

        public List<object> GetDashboardUnMission(int officeId)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashBoardUnMission] {0} ", officeId));

            return dataTable.ToJson().ToList();
        }

        public List<object> GetDashboardBranch()
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashboardBranch]"));

            return dataTable.ToJson().ToList();
        }
        public List<object> GetDashboardAdminAuthority()
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashboardAdminAuthority]"));

            return dataTable.ToJson().ToList();
        }
        public List<object> GetDashboardUnderMission(int rankId)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashboardUnderMission] {0}", rankId));

            return dataTable.ToJson().ToList();
        }
        public List<object> GetDashboardUnderCourse(int rankId)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashboardUnderCourse] {0}", rankId));

            return dataTable.ToJson().ToList();
        }
        public List<object> GetDashboardInsideNavyOrganization()
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashBoardInsideNavyOrganization]"));

            return dataTable.ToJson().ToList();

        }
        public List<object> GetDashboardBCGOrganization()
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashBoardBCGOrganization]"));

            return dataTable.ToJson().ToList();
        }
        public List<object> GetDashboardLeave()
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashboardLeave]"));

            return dataTable.ToJson().ToList();
        }


        public List<object> GetDashboardExBDLeave()
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashboardExBDLeave]"));

            return dataTable.ToJson().ToList();
        }

        public List<object> GetDashboardStream(int streamId)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashboardStream] {0}",streamId));

            return dataTable.ToJson().ToList();
        }

        public List<object> GetDashboardCategory(int categoryId)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashboardCategory] {0}",categoryId));

            return dataTable.ToJson().ToList();
        }

        public List<object> GetDashboardGender(int genderId)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashboardGender] {0}", genderId));

            return dataTable.ToJson().ToList();
        }
        

        public List<object> GetBranchAuthorityOfficer9(int branchAuthorityId)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashBoardBranchByAdminAuthority9] {0}", branchAuthorityId));

            return dataTable.ToJson().ToList();
        }
        public List<object> GetBranchAuthorityOfficer10(int branchAuthorityId)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashBoardBranchByAdminAuthority10] {0}", branchAuthorityId));

            return dataTable.ToJson().ToList();
        }
        public List<object> GetBranchAuthorityOfficer11(int branchAuthorityId)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashBoardBranchByAdminAuthority11] {0}", branchAuthorityId));

            return dataTable.ToJson().ToList();
        }
        public List<object> GetBranchAuthorityOfficer12(int branchAuthorityId)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashBoardBranchByAdminAuthority12] {0}", branchAuthorityId));

            return dataTable.ToJson().ToList();
        }
        public List<object> GetBranchAuthorityOfficer13(int branchAuthorityId)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashBoardBranchByAdminAuthority13] {0}", branchAuthorityId));

            return dataTable.ToJson().ToList();
        }
        public List<object> GetBranchAuthorityOfficer369(int branchAuthorityId)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashBoardBranchByAdminAuthority369] {0}", branchAuthorityId));

            return dataTable.ToJson().ToList();
        }
        public List<object> GetBranchAuthorityOfficer383(int branchAuthorityId)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashBoardBranchByAdminAuthority383] {0}", branchAuthorityId));

            return dataTable.ToJson().ToList();
        }
        
        public List<object> GetBranchAuthorityOfficer458(int branchAuthorityId)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashBoardBranchByAdminAuthority458] {0}", branchAuthorityId));

            return dataTable.ToJson().ToList();
        }
        public List<object> GetBranchAuthorityOfficer513(int branchAuthorityId)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashBoardBranchByAdminAuthority513] {0}", branchAuthorityId));

            return dataTable.ToJson().ToList();
        }
        public List<object> GetBranchAuthorityOfficer543(int branchAuthorityId)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashBoardBranchByAdminAuthority543] {0}", branchAuthorityId));

            return dataTable.ToJson().ToList();
        }
        public List<object> GetOfficerByAdminAuthorityWithDynamicQuery(string tableName,int branchAuthorityId)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetOfficerByAdminAuthorityWithDynamicQuery] '{0}',{1}", tableName, branchAuthorityId));

            return dataTable.ToJson().ToList();
        }
        public List<object> GetBranchAuthorityOfficer600()
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashBoardBranchByAdminAuthority600]"));

            return dataTable.ToJson().ToList();
        }
        public List<object> getBNOfficerStates950()
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashBoardBranch950]"));

            return dataTable.ToJson().ToList();
        }
        public List<object> getToeOfficerStateInside()
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashBoardBranch800]"));

            return dataTable.ToJson().ToList();
        }
        public List<object> getToeOfficerStateInNavy()
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashBoardBranch900]"));

            return dataTable.ToJson().ToList();
        }

        public List<object> GetDashboardOfficeAppointment(int officeType, int rankId, int displayId)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetDashboardOfficeAppointment] {0},{1},{2}", officeType,rankId,displayId));

            return dataTable.ToJson().ToList();
        }

        public List<object> GetOutsideNavyOfficer(int officeId, int rankLevel,int parentId)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetOutsideNavyOfficer] {0},{1},{2}", officeId,rankLevel, parentId));

            return dataTable.ToJson().ToList();
        }
        

        public List<object> GetOfficeSearchResult(int officeId)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetEmployeeListByOffice] {0}", officeId));

            return dataTable.ToJson().ToList();
        }

        public List<object> GetAdminAuthorityOfficer(int officeId, int rankLevel,int type)
        {
            if (type == 1)
            {
                DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetAdminAuthorityOfficer] {0},{1}", officeId, rankLevel));
                return dataTable.ToJson().ToList();
            }
            else if (type == 2)
            {
                DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetInsideNavyOfficer] {0},{1}", officeId, rankLevel));
                return dataTable.ToJson().ToList();
            }
            else
            {
                DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetBCGOfficer] {0},{1}", officeId, rankLevel));
                return dataTable.ToJson().ToList();
            }
           
        }

        public List<object> GetLeaveOfficer(int leaveType, int rankLevel)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetLeaveOfficer] {0},{1}", leaveType, rankLevel));

            return dataTable.ToJson().ToList();
        }

        public List<object> GetExBDLeaveOfficer(int rankLevel)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetExBDLeaveOfficer] {0}", rankLevel));

            return dataTable.ToJson().ToList();
        }

        public List<object> GetBranchOfficer(int rankId, string branch,int categoryId, int subCategoryId, int commissionTypeId)
        {
            string query= String.Format("exec [spGetBranchOfficer] {0},'{1}',{2},{3},{4}", rankId, branch, categoryId, subCategoryId, commissionTypeId);
            if (branch == null)
            {
                query = String.Format("exec [spGetBranchOfficer] {0},null, {1},{2},{3}", rankId, categoryId, subCategoryId, commissionTypeId);
            }

            DataTable dataTable = employeeRepository.ExecWithSqlQuery(query);
            
            return dataTable.ToJson().ToList();
        }

        public List<object> GetBranchOfficerByAdminAuthority(int addAuthId, int rankId, string branch,int categoryId, int subCategoryId, int commissionTypeId)
        {
            string query= String.Format("exec [spGetBranchOfficerByAdminAuthority] {0},{1},'{2}',{3},{4},{5}", addAuthId, rankId, branch, categoryId, subCategoryId, commissionTypeId);
            if (branch == null)
            {
                query = String.Format("exec [spGetBranchOfficerByAdminAuthority] {0}, {1},null,{2},{3},{4}", addAuthId, rankId, categoryId, subCategoryId, commissionTypeId);
            }

            DataTable dataTable = employeeRepository.ExecWithSqlQuery(query);
            
            return dataTable.ToJson().ToList();
        }
        public List<object> GetStreamOfficer(int rankId, string branch,int streamId)
        {
            string query = String.Format("exec [spGetStreamOfficer] {0},'{1}',{2}", rankId, branch, streamId);

            if (branch == null)
            {
                query = String.Format("exec [spGetStreamOfficer] {0},null,{1}", rankId, streamId);
            }
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(query);

            return dataTable.ToJson().ToList();
        }
        public List<object> GetOverviewOfficerDeploymentList(int rankId, int officerTypeId, int coastGuard, int outsideOrg)
        {
            string query = String.Format("exec [spGetOverviewOfOfficerDeploymentList] {0},{1},{2},{3}", rankId, officerTypeId, coastGuard, outsideOrg);

            if (outsideOrg == 100)
            {
                if (coastGuard == 100)
                {
                    if (officerTypeId == 100)
                    {
                        query = String.Format("exec [spGetOverviewOfOfficerDeploymentList] {0},null,null,null", rankId);
                    }
                    else
                    {
                        query = String.Format("exec [spGetOverviewOfOfficerDeploymentList] {0},{1},null,null", rankId, officerTypeId);
                    }
                }
                else
                {
                    query = String.Format("exec [spGetOverviewOfOfficerDeploymentList] {0},{1},{2},null", rankId, officerTypeId, coastGuard);
                } 
            }
            //else
            //{
            //    query = String.Format("exec [spGetOverviewOfOfficerDeploymentList] {0},{1},{2},{3}", rankId, officerTypeId, coastGuard, outsideOrg);
            //}
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(query);

            return dataTable.ToJson().ToList();
        }

        public List<object> GetCategoryOfficer(int rankId, string branch, int categoryId)
        {
            string query = String.Format("exec [spGetCategoryOfficer] {0},'{1}',{2}", rankId, branch, categoryId);

            if (branch == null)
            {
                query = String.Format("exec [spGetCategoryOfficer] {0},null,{1}", rankId, categoryId);
            }
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(query);

            return dataTable.ToJson().ToList();
        }

        public List<object> GetGenderOfficer(int rankId, string branch,int categoryId,int subCategoryId,int commissionTypeId, int genderId)
        {
            string query = String.Format("exec [spGetGenderOfficer] {0},'{1}',{2},{3},{4},{5}", rankId, branch, categoryId, subCategoryId, commissionTypeId, genderId);

            if (branch == null)
            {
                query = String.Format("exec [spGetGenderOfficer] {0},null,{1},{2},{3},{4}", rankId, categoryId, subCategoryId, commissionTypeId, genderId);
            }
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(query);

            return dataTable.ToJson().ToList();
        }

        public List<object> GetToeOfficerByTransferType(int rankId, string branch,int categoryId,int subCategoryId,int commissionTypeId, int transferType)
        {
            string query = String.Format("exec [spGetToeOfficerByTransferType] {0},'{1}',{2},{3},{4},{5}", rankId, branch, categoryId, subCategoryId, commissionTypeId, transferType);

            if (branch == null)
            {
                query = String.Format("exec [spGetToeOfficerByTransferType] {0},null,{1},{2},{3},{4}", rankId, categoryId, subCategoryId, commissionTypeId, transferType);
            }
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(query);

            return dataTable.ToJson().ToList();
        }
    }
}