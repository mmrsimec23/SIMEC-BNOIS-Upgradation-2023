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
    
    public partial class NominationSchedule
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NominationSchedule()
        {
            this.MissionAppointments = new HashSet<MissionAppointment>();
        }
    
        public int NominationScheduleId { get; set; }
        public int NominationScheduleType { get; set; }
        public Nullable<int> VisitCategoryId { get; set; }
        public Nullable<int> VisitSubCategoryId { get; set; }
        public string TitleName { get; set; }
        public Nullable<int> CountryId { get; set; }
        public string Purpose { get; set; }
        public string Location { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public Nullable<int> NumberOfPost { get; set; }
        public Nullable<int> AssignPost { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    
        public virtual Country Country { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MissionAppointment> MissionAppointments { get; set; }
        public virtual VisitCategory VisitCategory { get; set; }
        public virtual VisitSubCategory VisitSubCategory { get; set; }
    }
}
