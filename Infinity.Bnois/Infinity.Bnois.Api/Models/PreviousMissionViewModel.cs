﻿using Infinity.Bnois.ApplicationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.Api.Models
{
    public class PreviousMissionViewModel
    {
        public PreviousMissionModel PreviousMission { get; set; }
        public List<SelectModel> Countries { get; set; }
    }
}
