using Infinity.Bnois.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Models
{
   public class EmployeeMscEducationModel
    {
        public int EmployeeMscEducationId { get; set; }
        public int EmployeeId { get; set; }
        public Nullable<int> RankId { get; set; }
        public Nullable<int> MscEducationTypeId { get; set; }
        public Nullable<int> MscInstituteId { get; set; }
        public Nullable<int> MscPermissionTypeId { get; set; }
        public Nullable<int> CountryId { get; set; }
        public string PassingYear { get; set; }
        public string PermissionYear { get; set; }
        public string Results { get; set; }
        public string Remarks { get; set; }

        public Nullable<bool> IsComplete { get; set; }

        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }

        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }

        public virtual CountryModel Country { get; set; }
        public virtual EmployeeModel Employee { get; set; }
        public virtual RankModel Rank { get; set; }
        public virtual MscEducationTypeModel MscEducationType { get; set; }
        public virtual MscInstituteModel MscInstitute { get; set; }
        public virtual MscPermissionTypeModel MscPermissionType { get; set; }
    }
}
