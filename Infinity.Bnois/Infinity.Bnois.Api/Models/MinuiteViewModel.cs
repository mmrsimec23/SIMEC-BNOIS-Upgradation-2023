using System.Collections.Generic;
using Infinity.Bnois.ApplicationService.Models;


namespace Infinity.Bnois.Api.Models
{
    public class MinuiteViewModel
    {
        public DashBoardMinuite100Model Minuite { get; set; }
        public List<SelectModel> Countries { get; set; }
    }
}