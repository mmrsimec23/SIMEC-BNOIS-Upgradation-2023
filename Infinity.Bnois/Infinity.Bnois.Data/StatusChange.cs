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
    
    public partial class StatusChange
    {
        public int StatusChangeId { get; set; }
        public int EmployeeId { get; set; }
        public int PreviousId { get; set; }
        public int NewId { get; set; }
        public int StatusType { get; set; }
        public string MedicalCategoryCause { get; set; }
        public Nullable<int> MedicalCategoryType { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<System.DateTime> DateTo { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    
        public virtual Employee Employee { get; set; }
    }
}
