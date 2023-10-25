using Infinity.Bnois.ApplicationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.Api.Models
{
    public class EmployeeCoxoServiceViewModel
    {
        public EmployeeCoxoServiceModel EmployeeCoxoService { get; set; }
        public List<SelectModel> CoxoTypes { get; set; }
        public List<SelectModel> CoxoShipTypes { get; set; }
        public List<SelectModel> CoxoAppoinments { get; set; }
        public List<SelectModel> Offices { get; set; }

    }
}