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
    
    public partial class EmployeeSport
    {
        public long EmployeeSportId { get; set; }
        public int EmployeeId { get; set; }
        public int SportId { get; set; }
        public string TeamName { get; set; }
        public Nullable<System.DateTime> DateOfParticipation { get; set; }
        public string Award { get; set; }
        public string Hobby { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    
        public virtual Sport Sport { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
