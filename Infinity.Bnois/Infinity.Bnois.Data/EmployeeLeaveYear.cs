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
    
    public partial class EmployeeLeaveYear
    {
        public int EmpLeaveYearId { get; set; }
        public int EmpLeaveId { get; set; }
        public Nullable<int> YearText { get; set; }
        public System.DateTime LeaveDate { get; set; }
    
        public virtual EmployeeLeave EmployeeLeave { get; set; }
    }
}
