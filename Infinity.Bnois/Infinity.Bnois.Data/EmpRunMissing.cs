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
    
    public partial class EmpRunMissing
    {
        public int EmpRunMissingId { get; set; }
        public int EmployeeId { get; set; }
        public bool IsBackLog { get; set; }
        public Nullable<int> TransferId { get; set; }
        public Nullable<int> RankId { get; set; }
        public Nullable<int> BranchId { get; set; }
        public int Type { get; set; }
        public System.DateTime Date { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> BOffcd { get; set; }
        public Nullable<int> AOffcd { get; set; }
    
        public virtual Branch Branch { get; set; }
        public virtual Rank Rank { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
