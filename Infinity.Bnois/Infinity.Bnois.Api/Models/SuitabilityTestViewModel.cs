using Infinity.Bnois.ApplicationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.Api.Models
{
    public class SuitabilityTestViewModel
    {
        public SuitabilityTestModel SuitabilityTests { get; set; }
        public List<SelectModel> SuitabilityTestTypes { get; set; }
        public List<SelectModel> Batches { get; set; }

    }
}