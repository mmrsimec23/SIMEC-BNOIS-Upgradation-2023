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
    
    public partial class EmployeeLpr
    {
        public int EmpLprId { get; set; }
        public int EmployeeId { get; set; }
        public int TerminationTypeId { get; set; }
        public Nullable<int> CurrentStatus { get; set; }
        public Nullable<System.DateTime> LprDate { get; set; }
        public Nullable<int> DurationMonth { get; set; }
        public Nullable<int> DurationDay { get; set; }
        public Nullable<System.DateTime> RetireDate { get; set; }
        public Nullable<System.DateTime> TerminationDate { get; set; }
        public int RStatus { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    
        public virtual TerminationType TerminationType { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
