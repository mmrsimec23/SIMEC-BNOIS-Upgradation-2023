using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Data;
using System.Collections.Generic;

namespace Infinity.Bnois.Api.Models
{
    public class EmployeeCarLoanViewModel
    {
        public EmployeeCarLoanModel EmployeeCarLoan { get; set; }
        public List<SelectModel> CarLoanFiscalYears { get; set; }
        public List<SelectModel> Ranks { get; set; }
    }
}