using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Data.Models;

namespace Infinity.Bnois.ApplicationService.Interface
{
	public interface IEmployeeLeaveService
	{
		List<EmployeeLeaveModel> GetEmployeeLeaves(int ps, int pn, string qs, int? leaveType, out int total);
		Task<EmployeeLeaveModel> GetEmployeeLeave(int id);
		Task<EmployeeLeaveModel> SaveEmployeeLeave(int v, EmployeeLeaveModel model);
		Task<bool> DeleteEmployeeLeave(int id);
        Task<List<EmployeeLeaveBalance>> GetEmployeeLeaveBalance(int employeeId, int leaveType);
		Task<List<LeaveBreakDown>> GetLeaveBreakDowns(int emId);

		Task<List<SpGetEmployeeLeaveInfoByPNo>> GetEmployeeLeaveDetailsByPNo(string employeePNo);
        Task<LeaveSummaryModel> GetLeaveSummary(String pno);
        Task<string> GetEmployeeLeaveDue(int employeeId, int leaveType);
        Task<List<EmployeeLeaveBalance>> GetDefaultEmployeeLeaveBalance(int employeeId, int leaveType,int fromDate);
    }
}
