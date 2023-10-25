//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Infinity.Bnois.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmployeeTransferFuturePlan
    {
        public int EmployeeTransferFuturePlanId { get; set; }
        public int EmployeeId { get; set; }
        public Nullable<int> AptnatId { get; set; }
        public Nullable<int> AptcatId { get; set; }
        public Nullable<int> PatternId { get; set; }
        public int OfficeId { get; set; }
        public Nullable<int> CountryId { get; set; }
        public bool IsMandatory { get; set; }
        public Nullable<System.DateTime> PlanDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    
        public virtual AptCat AptCat { get; set; }
        public virtual AptNat AptNat { get; set; }
        public virtual Country Country { get; set; }
        public virtual Pattern Pattern { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Office Office { get; set; }
    }
}