using Infinity.Bnois.ApplicationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.Api.Models
{
  public  class DashBoardBranchByAdminAuthority600EntryViewModel
    {
        public DashBoardBranchByAdminAuthority600EntryModel DashBoardBranchByAdminAuthority600Entry { get; set; }
        public List<SelectModel> RankList { get; set; }
        public List<SelectModel> OrgTypeList { get; set; }
    }
}
