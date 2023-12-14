using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Models
{
   public class CoFfRecomModel
    {

        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Nullable<int> RankId { get; set; }
        public Nullable<int> RecomStatus { get; set; }
        public string Remarks { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual EmployeeModel Employee { get; set; }
        public virtual RankModel Rank { get; set; }
    }
}
