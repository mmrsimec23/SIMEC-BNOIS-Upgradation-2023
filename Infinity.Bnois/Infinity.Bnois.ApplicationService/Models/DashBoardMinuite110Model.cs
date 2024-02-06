using Infinity.Bnois.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Models
{
   public class DashBoardMinuite110Model
    {
        public int MinuiteCandidateId { get; set; }
        public int MinuiteId { get; set; }
        public int EmployeeId { get; set; }
        public string ProposedBillet { get; set; }
        public string Remarks1 { get; set; }
        public string Remarks2 { get; set; }
        public string Remarks3 { get; set; }
        public Nullable<int> Remarks4 { get; set; }
        public Nullable<int> Remarks5 { get; set; }
        public Nullable<int> Remarks6 { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual DashBoardMinuite100Model Minuite { get; set; }

        public virtual EmployeeModel Employee { get; set; }
    }
}
