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
    
    public partial class CourseCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CourseCategory()
        {
            this.BraCtryCoursePoints = new HashSet<BraCtryCoursePoint>();
            this.CoursePoints = new HashSet<CoursePoint>();
            this.CourseSubCategories = new HashSet<CourseSubCategory>();
            this.TrainingPlans = new HashSet<TrainingPlan>();
            this.Courses = new HashSet<Course>();
            this.EmployeeCourseFuturePlan = new HashSet<EmployeeCourseFuturePlan>();
            this.TrainingResult = new HashSet<TrainingResult>();
        }
    
        public int CourseCategoryId { get; set; }
        public string NameBan { get; set; }
        public string Name { get; set; }
        public string ShortNameBan { get; set; }
        public string ShortName { get; set; }
        public Nullable<int> Priority { get; set; }
        public bool Trace { get; set; }
        public bool SASB { get; set; }
        public bool PromotionBoard { get; set; }
        public bool BnList { get; set; }
        public Nullable<int> BnListPriority { get; set; }
        public bool TransferProposal { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BraCtryCoursePoint> BraCtryCoursePoints { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CoursePoint> CoursePoints { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CourseSubCategory> CourseSubCategories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingPlan> TrainingPlans { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Course> Courses { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeCourseFuturePlan> EmployeeCourseFuturePlan { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingResult> TrainingResult { get; set; }
    }
}