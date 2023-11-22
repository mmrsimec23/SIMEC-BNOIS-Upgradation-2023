using Infinity.Bnois.ApplicationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.Api.Models
{
  public  class DashBoardBranchByAdminAuthority700ViewModel
    {
        public DashBoardBranchByAdminAuthority700Model DashBoardBranchByAdminAuthority700 { get; set; }
        public List<SelectModel> RankList { get; set; }
        public List<SelectModel> BranchList { get; set; }
    }
}
