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
    
    public partial class Achievement
    {
        public int AchievementId { get; set; }
        public int EmployeeId { get; set; }
        public bool IsBackLog { get; set; }
        public Nullable<int> RankId { get; set; }
        public Nullable<int> TransferId { get; set; }
        public Nullable<int> GivenEmployeeId { get; set; }
        public Nullable<int> GivenTransferId { get; set; }
        public Nullable<int> CommendationId { get; set; }
        public Nullable<int> PatternId { get; set; }
        public Nullable<int> OfficeId { get; set; }
        public string OfficerName { get; set; }
        public string OfficerDesignation { get; set; }
        public int GivenByType { get; set; }
        public int Type { get; set; }
        public System.DateTime Date { get; set; }
        public string CommAppType { get; set; }
        public string Reason { get; set; }
        public string Remarks { get; set; }
        public string FileName { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> BOffCd { get; set; }
        public Nullable<int> AOffCd { get; set; }
    
        public virtual Commendation Commendation { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Employee Employee1 { get; set; }
        public virtual Office Office { get; set; }
    }
}
