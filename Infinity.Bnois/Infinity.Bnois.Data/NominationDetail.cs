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
    
    public partial class NominationDetail
    {
        public long NominationDetailId { get; set; }
        public int NominationId { get; set; }
        public int EmployeeId { get; set; }
        public bool IsBackLog { get; set; }
        public Nullable<int> RankId { get; set; }
        public Nullable<int> TransferId { get; set; }
        public Nullable<bool> IsApporved { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> TYear { get; set; }
        public Nullable<int> Transcd { get; set; }
    
        public virtual Nomination Nomination { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
