using Infinity.Bnois.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Models
{
	public class DashBoardBranchByAdminAuthority600EntryModel
    {
        public int Id { get; set; }
        public Nullable<int> RankId { get; set; }
        public Nullable<int> OrgType { get; set; }
        public Nullable<int> No { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual RankModel Rank { get; set; }
    }
}
