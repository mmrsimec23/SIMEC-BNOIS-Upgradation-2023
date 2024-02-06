using Infinity.Bnois.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Models
{
   public class DashBoardMinuteStandby975Model
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Nullable<int> RankId { get; set; }
        public System.DateTime DateFrom { get; set; }
        public Nullable<System.DateTime> DateTo { get; set; }
        public string StandbyRemarks1 { get; set; }
        public string StandbyRemarks2 { get; set; }
        public string StandbyRemarks3 { get; set; }
        public string StandbyRemarks4 { get; set; }
        public string StandbyRemarks5 { get; set; }
        public Nullable<int> StandbyRemarks6 { get; set; }
        public Nullable<int> StandbyRemarks7 { get; set; }
        public Nullable<int> StandbyRemarks8 { get; set; }
        public Nullable<int> StandbyRemarks9 { get; set; }
        public Nullable<int> StandbyRemarks10 { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual DashBoardMinuteStandby975Model StandbyInfo { get; set; }

        public virtual EmployeeModel Employee { get; set; }
        public virtual RankModel Rank { get; set; }
    }
}
