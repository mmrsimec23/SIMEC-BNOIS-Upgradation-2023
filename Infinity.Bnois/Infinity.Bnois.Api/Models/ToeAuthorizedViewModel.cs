using Infinity.Bnois.ApplicationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.Api.Models
{
  public  class ToeAuthorizedViewModel
    {
        public ToeAuthorizedModel ToeAuthorized { get; set; }
        public List<SelectModel> RankList { get; set; }
        public List<SelectModel> BranchList { get; set; }
        public List<SelectModel> OfficeList { get; set; }
    }
}
