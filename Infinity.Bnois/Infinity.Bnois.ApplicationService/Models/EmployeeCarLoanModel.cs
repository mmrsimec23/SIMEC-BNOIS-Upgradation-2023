using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Models
{
    public class EmployeeCarLoanModel
    {
        public int EmployeeCarLoanId { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public bool IsBackLog { get; set; }
        public Nullable<int> RankId { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> CarLoanFiscalYearId { get; set; }
        public Nullable<System.DateTime> AvailDate { get; set; }
        public Nullable<double> Amount { get; set; }
        public string Remarks { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> Modified { get; set; }
        public string ModifiedBy { get; set; }
        public bool Active { get; set; }

        public virtual EmployeeModel Employee { get; set; }
        public virtual CarLoanFiscalYearModel CarLoanFiscalYear { get; set; }
        public virtual RankModel Rank { get; set; }
    }
}
