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
    
    public partial class EmployeeRank
    {
        public int EmployeeRankId { get; set; }
        public int EmployeeId { get; set; }
        public int RankId { get; set; }
        public string SourceType { get; set; }
        public System.DateTime EffectiveDate { get; set; }
    }
}