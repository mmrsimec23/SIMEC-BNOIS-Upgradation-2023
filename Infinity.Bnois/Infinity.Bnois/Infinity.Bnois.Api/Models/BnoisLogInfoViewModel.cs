using Infinity.Bnois.ApplicationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.Api.Models
{
   public class BnoisLogInfoViewModel
    {
        public List<SelectModel> TableList { get; set; }
        public List<SelectModel> LogStatusList { get; set; }
    }
}
