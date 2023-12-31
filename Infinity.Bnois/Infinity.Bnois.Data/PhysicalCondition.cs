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
    
    public partial class PhysicalCondition
    {
        public int PhysicalConditionId { get; set; }
        public int EmployeeId { get; set; }
        public int EyeColorId { get; set; }
        public int SkinColorId { get; set; }
        public int HairColorId { get; set; }
        public int EyeVisionId { get; set; }
        public int BloodGroupId { get; set; }
        public int PhysicalStructureId { get; set; }
        public int MedicalCategoryId { get; set; }
        public string MedicalCategoryCause { get; set; }
        public Nullable<int> MedicalCategoryType { get; set; }
        public Nullable<System.DateTime> MedicalCategoryDateFrom { get; set; }
        public Nullable<System.DateTime> MedicalCategoryDateTo { get; set; }
        public string FileName { get; set; }
        public bool IsPerMedicalCategory { get; set; }
        public double HeightInFeet { get; set; }
        public double HeightInInc { get; set; }
        public double HeightInCM { get; set; }
        public double Weight { get; set; }
        public string IdentificationMark { get; set; }
        public string Remarks { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    
        public virtual BloodGroup BloodGroup { get; set; }
        public virtual Color Color { get; set; }
        public virtual Color Color1 { get; set; }
        public virtual Color Color2 { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual EyeVision EyeVision { get; set; }
        public virtual MedicalCategory MedicalCategory { get; set; }
        public virtual PhysicalStructure PhysicalStructure { get; set; }
    }
}
