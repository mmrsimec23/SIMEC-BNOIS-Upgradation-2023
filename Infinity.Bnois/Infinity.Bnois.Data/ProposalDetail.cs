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
    
    public partial class ProposalDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProposalDetail()
        {
            this.ProposalCandidate = new HashSet<ProposalCandidate>();
        }
    
        public int ProposalDetailId { get; set; }
        public int TransferProposalId { get; set; }
        public int TransferType { get; set; }
        public int AttachOfficeId { get; set; }
        public Nullable<int> AppointmentId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    
        public virtual TransferProposal TransferProposal { get; set; }
        public virtual Office Office { get; set; }
        public virtual OfficeAppointment OfficeAppointment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProposalCandidate> ProposalCandidate { get; set; }
    }
}
