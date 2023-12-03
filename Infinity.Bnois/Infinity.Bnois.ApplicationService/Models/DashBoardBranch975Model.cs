using Infinity.Bnois.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Models
{
	public class DashBoardBranch975Model
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Nullable<int> RankId { get; set; }
        public System.DateTime DefermentFrom { get; set; }
        public Nullable<int> DurationYear { get; set; }
        public Nullable<System.DateTime> DefermentTo { get; set; }
        public Nullable<int> UnmReason { get; set; }
        public string UnwillingMissionRemarks { get; set; }
        public string UnmReference { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual EmployeeModel Employee { get; set; }
        public virtual RankModel Rank { get; set; }
    }
}
