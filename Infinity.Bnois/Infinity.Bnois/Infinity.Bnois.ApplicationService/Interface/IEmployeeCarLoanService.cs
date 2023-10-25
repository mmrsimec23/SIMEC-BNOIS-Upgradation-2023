using Infinity.Bnois.ApplicationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IEmployeeCarLoanService
    {
        List<EmployeeCarLoanModel> GetEmployeeCarLoanList(int ps, int pn, string qs, out int total);
        List<EmployeeCarLoanModel> GetEmployeeCarLoansByPno(string PNo);
        Task<EmployeeCarLoanModel> getEmployeeCarLoan(int EmployeeCarLoanId);
        Task<EmployeeCarLoanModel> SaveEmployeeCarLoan(int v, EmployeeCarLoanModel model);
        Task<bool> DeleteEmployeeCarLoan(int id);


    }
}
