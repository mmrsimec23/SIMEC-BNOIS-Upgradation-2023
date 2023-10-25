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
    
    public partial class Award
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Award()
        {
            this.BonusPtAwards = new HashSet<BonusPtAward>();
            this.MedalAwards = new HashSet<MedalAward>();
        }
    
        public int AwardId { get; set; }
        public string NameEng { get; set; }
        public string NameBan { get; set; }
        public string ShortNameEng { get; set; }
        public string ShortNameBan { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public bool GoToOPR { get; set; }
        public bool GoToTrace { get; set; }
        public bool GoToSASB { get; set; }
        public bool ANmCon { get; set; }
        public bool NmRGF { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BonusPtAward> BonusPtAwards { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MedalAward> MedalAwards { get; set; }
    }
}