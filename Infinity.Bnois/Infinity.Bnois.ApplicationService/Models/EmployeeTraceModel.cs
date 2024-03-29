﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Models
{
    public class EmployeeTraceModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int RankId { get; set; }
        public Nullable<double> TraceMark { get; set; }
        public Nullable<int> RecomStatus { get; set; }
        public Nullable<int> RecomStatus2 { get; set; }
        public Nullable<int> RecomStatus3 { get; set; }
        public Nullable<double> RecomStatus4 { get; set; }
        public string Remarks { get; set; }
        public string Remarks2 { get; set; }
        public string Remarks3 { get; set; }
        public string Remarks4 { get; set; }
        public string Remarks5 { get; set; }
        public string Remarks6 { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual EmployeeModel Employee { get; set; }
        public virtual RankModel Rank { get; set; }
    }
}
