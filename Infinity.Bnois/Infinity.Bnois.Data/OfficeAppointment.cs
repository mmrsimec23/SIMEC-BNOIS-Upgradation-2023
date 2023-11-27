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
    
    public partial class OfficeAppointment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OfficeAppointment()
        {
            this.EmployeeOpr = new HashSet<EmployeeOpr>();
            this.ExtraAppointment = new HashSet<ExtraAppointment>();
            this.OfficeAppBranch = new HashSet<OfficeAppBranch>();
            this.OfficeAppRank = new HashSet<OfficeAppRank>();
            this.ProposalDetail = new HashSet<ProposalDetail>();
            this.Transfer = new HashSet<Transfer>();
        }
    
        public int OffAppId { get; set; }
        public int OfficeId { get; set; }
        public int AptNatId { get; set; }
        public int AptCatId { get; set; }
        public int AppointmentType { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string NameBangla { get; set; }
        public string ShortNameBangla { get; set; }
        public bool GovtApproved { get; set; }
        public bool HeadofDpt { get; set; }
        public bool OfficeHead { get; set; }
        public bool IsInstrServiceCount { get; set; }
        public int ParentOffAppId { get; set; }
        public bool IsAdditional { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
        public string TestRank { get; set; }
        public string TestBranch { get; set; }
        public string OApcd { get; set; }
        public string OPatCd { get; set; }
        public Nullable<bool> OrgCd { get; set; }
        public string AptCd { get; set; }
    
        public virtual AptCat AptCat { get; set; }
        public virtual AptNat AptNat { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeOpr> EmployeeOpr { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExtraAppointment> ExtraAppointment { get; set; }
        public virtual Office Office { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OfficeAppBranch> OfficeAppBranch { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OfficeAppRank> OfficeAppRank { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProposalDetail> ProposalDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transfer> Transfer { get; set; }
    }
}
