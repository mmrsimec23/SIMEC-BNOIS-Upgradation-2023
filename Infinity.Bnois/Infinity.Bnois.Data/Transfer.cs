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
    
    public partial class Transfer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Transfer()
        {
            this.ExtraAppointment = new HashSet<ExtraAppointment>();
            this.EmployeeSecurityClearance = new HashSet<EmployeeSecurityClearance>();
        }
    
        public int TransferId { get; set; }
        public int EmployeeId { get; set; }
        public Nullable<int> DistrictId { get; set; }
        public Nullable<int> RankId { get; set; }
        public Nullable<int> TransferFor { get; set; }
        public int TransferMode { get; set; }
        public int TranferType { get; set; }
        public int TempTransferType { get; set; }
        public System.DateTime FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public Nullable<int> CurrentBornOfficeId { get; set; }
        public Nullable<int> AttachOfficeId { get; set; }
        public Nullable<int> AppointmentId { get; set; }
        public Nullable<int> AppointmentType { get; set; }
        public Nullable<int> NominationId { get; set; }
        public string FileName { get; set; }
        public Nullable<bool> IsExtraAppointment { get; set; }
        public bool IsBackLog { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> Tyear { get; set; }
        public Nullable<int> Transcd { get; set; }
        public Nullable<int> PBOffCd { get; set; }
        public Nullable<int> PAOffCd { get; set; }
        public Nullable<int> OPatCd { get; set; }
        public Nullable<int> OrgCd { get; set; }
        public Nullable<int> AptCd { get; set; }
        public Nullable<int> NOAPCD { get; set; }
    
        public virtual District District { get; set; }
        public virtual Nomination Nomination { get; set; }
        public virtual Rank Rank { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExtraAppointment> ExtraAppointment { get; set; }
        public virtual Employee Employee { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeSecurityClearance> EmployeeSecurityClearance { get; set; }
        public virtual Office Office { get; set; }
        public virtual OfficeAppointment OfficeAppointment { get; set; }
    }
}
