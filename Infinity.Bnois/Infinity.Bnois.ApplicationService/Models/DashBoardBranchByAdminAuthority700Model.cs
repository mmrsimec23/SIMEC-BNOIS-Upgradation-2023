using Infinity.Bnois.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Models
{
	public class DashBoardBranchByAdminAuthority700Model
    {
        public int Id { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> RankId { get; set; }
        public Nullable<int> TransferTypeId { get; set; }
        public Nullable<int> No { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual RankModel Rank { get; set; }
        public virtual BranchModel Branch { get; set; }
    }
}
