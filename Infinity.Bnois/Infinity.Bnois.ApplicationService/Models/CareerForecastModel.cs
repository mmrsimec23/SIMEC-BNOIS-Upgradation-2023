using Infinity.Bnois.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Models
{
   public class CareerForecastModel
    {
        public int CareerForecastId { get; set; }
        public int EmployeeId { get; set; }
        public Nullable<int> CareerForecastSettingId { get; set; }
        public Nullable<int> BranchId { get; set; }
        public Nullable<int> ForecastStatus { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public List<EmployeeCareerForecastSettingListModel> ForecastSettingList { get; set; }
        public virtual EmployeeModel Employee { get; set; }
        public virtual CareerForecastSettingModel CareerForecastSetting { get; set; }
    }
}
