using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Models
{
  public  class BnoisLogInfoModel
    {
        public string TableName { get; set; }
        public int LogStatus { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}
