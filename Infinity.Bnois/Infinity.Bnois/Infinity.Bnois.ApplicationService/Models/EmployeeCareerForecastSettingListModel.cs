using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Models
{
   public class EmployeeCareerForecastSettingListModel
    {
        public int CareerForecastSettingId { get; set; }
        public string Name { get; set; }
        public string Shortname { get; set; }
        public int BranchId { get; set; }
        public int PositionNo { get; set; }
        public bool IsComplete { get; set; }

    }
}
