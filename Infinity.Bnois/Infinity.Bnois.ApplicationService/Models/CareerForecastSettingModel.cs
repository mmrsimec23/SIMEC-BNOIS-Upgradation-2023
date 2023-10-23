using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Models
{
   public class CareerForecastSettingModel
    {
        public int CareerForecastSettingId { get; set; }
        public string Name { get; set; }
        public string Shortname { get; set; }
        public int BranchId { get; set; }
        public int PositionNo { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual BranchModel Branch { get; set; }

    }
}
