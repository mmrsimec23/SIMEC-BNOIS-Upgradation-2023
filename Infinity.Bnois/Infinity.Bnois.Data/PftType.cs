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
    
    public partial class PftType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PftType()
        {
            this.EmployeePfts = new HashSet<EmployeePft>();
        }
    
        public int PftTypeId { get; set; }
        public string PftTitle { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeePft> EmployeePfts { get; set; }
    }
}