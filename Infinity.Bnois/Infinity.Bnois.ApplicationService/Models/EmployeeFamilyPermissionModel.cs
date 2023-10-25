using Infinity.Bnois.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Models
{
   public class EmployeeFamilyPermissionModel
  {
        public int EmployeeFamilyPermissionId { get; set; }
        public int EmployeeId { get; set; }
        public Nullable<int> RankId { get; set; }
        public Nullable<int> RelationId { get; set; }
        public Nullable<int> CountryId { get; set; }
        public string RelativeName { get; set; }
        public string VisitPurpose { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public string Remarks { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual CountryModel Country { get; set; }
        public virtual EmployeeModel Employee { get; set; }
        public virtual RankModel Rank { get; set; }
        public virtual RelationModel Relation { get; set; }
    }
}
