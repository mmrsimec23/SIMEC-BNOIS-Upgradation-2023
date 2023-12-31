﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Models
{
   public class EmployeeDollarSignModel
    {
        public int EmployeeDollarSignId { get; set; }
        public int EmployeeId { get; set; }
        public bool HasDollarSign { get; set; }
        public string Reason { get; set; }
        public System.DateTime DateOfDollarSign { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual EmployeeModel Employee { get; set; }
    }
}
