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
    
    public partial class vwNomination
    {
        public int NominationId { get; set; }
        public int EnitityType { get; set; }
        public string Nomination { get; set; }
        public int EntityId { get; set; }
        public string MissionAppointment { get; set; }
        public Nullable<System.DateTime> EntryDate { get; set; }
        public bool WithoutTransfer { get; set; }
        public string Remarks { get; set; }
        public string Course { get; set; }
        public string Institute { get; set; }
        public string TitleName { get; set; }
        public Nullable<int> AppointmentCategoryId { get; set; }
        public Nullable<int> AppointmentNatureId { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public string Country { get; set; }
        public Nullable<int> CountryId { get; set; }
        public string Title { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public Nullable<int> MissionAppointmentId { get; set; }
    }
}
