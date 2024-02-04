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
    
    public partial class Rank
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Rank()
        {
            this.AgeServicePolicy = new HashSet<AgeServicePolicy>();
            this.EmpRejoin = new HashSet<EmpRejoin>();
            this.MissionAppRank = new HashSet<MissionAppRank>();
            this.OfficeAppRank = new HashSet<OfficeAppRank>();
            this.PromotionPolicy = new HashSet<PromotionPolicy>();
            this.PromotionPolicy1 = new HashSet<PromotionPolicy>();
            this.RankMap = new HashSet<RankMap>();
            this.RankMap1 = new HashSet<RankMap>();
            this.RankMap2 = new HashSet<RankMap>();
            this.RetiredAge = new HashSet<RetiredAge>();
            this.EmployeeServiceExt = new HashSet<EmployeeServiceExt>();
            this.PromotionNominations = new HashSet<PromotionNomination>();
            this.PromotionNominations1 = new HashSet<PromotionNomination>();
            this.EmpRunMissings = new HashSet<EmpRunMissing>();
            this.PreviousTransfers = new HashSet<PreviousTransfer>();
            this.TrainingRanks = new HashSet<TrainingRank>();
            this.PunishmentAccidents = new HashSet<PunishmentAccident>();
            this.PromotionBoards = new HashSet<PromotionBoard>();
            this.PromotionBoards1 = new HashSet<PromotionBoard>();
            this.Transfer = new HashSet<Transfer>();
            this.EmployeeOpr = new HashSet<EmployeeOpr>();
            this.Employee = new HashSet<Employee>();
            this.EmployeeFamilyPermission = new HashSet<EmployeeFamilyPermission>();
            this.EmployeeCarLoan = new HashSet<EmployeeCarLoan>();
            this.EmployeeSecurityClearance = new HashSet<EmployeeSecurityClearance>();
            this.EmployeeMscEducation = new HashSet<EmployeeMscEducation>();
            this.ToeAuthorized = new HashSet<ToeAuthorized>();
            this.DashBoardBranchByAdminAuthority600Entry = new HashSet<DashBoardBranchByAdminAuthority600Entry>();
            this.DashBoardBranchByAdminAuthority700 = new HashSet<DashBoardBranchByAdminAuthority700>();
            this.DashBoardBranch975 = new HashSet<DashBoardBranch975>();
            this.DashBoardBranch980 = new HashSet<DashBoardBranch980>();
            this.DashBoardTrace990 = new HashSet<DashBoardTrace990>();
        }
    
        public int RankId { get; set; }
        public string FullName { get; set; }
        public string FullNameBan { get; set; }
        public string ShortName { get; set; }
        public string ShortNameBan { get; set; }
        public bool IsConfirm { get; set; }
        public Nullable<double> ServiceYear { get; set; }
        public Nullable<int> RankLevel { get; set; }
        public int RankOrder { get; set; }
        public System.DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> Modified { get; set; }
        public string ModifiedBy { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AgeServicePolicy> AgeServicePolicy { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmpRejoin> EmpRejoin { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MissionAppRank> MissionAppRank { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OfficeAppRank> OfficeAppRank { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PromotionPolicy> PromotionPolicy { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PromotionPolicy> PromotionPolicy1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RankMap> RankMap { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RankMap> RankMap1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RankMap> RankMap2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RetiredAge> RetiredAge { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeServiceExt> EmployeeServiceExt { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PromotionNomination> PromotionNominations { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PromotionNomination> PromotionNominations1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmpRunMissing> EmpRunMissings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PreviousTransfer> PreviousTransfers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingRank> TrainingRanks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PunishmentAccident> PunishmentAccidents { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PromotionBoard> PromotionBoards { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PromotionBoard> PromotionBoards1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transfer> Transfer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeOpr> EmployeeOpr { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Employee> Employee { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeFamilyPermission> EmployeeFamilyPermission { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeCarLoan> EmployeeCarLoan { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeSecurityClearance> EmployeeSecurityClearance { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeMscEducation> EmployeeMscEducation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ToeAuthorized> ToeAuthorized { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DashBoardBranchByAdminAuthority600Entry> DashBoardBranchByAdminAuthority600Entry { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DashBoardBranchByAdminAuthority700> DashBoardBranchByAdminAuthority700 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DashBoardBranch975> DashBoardBranch975 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DashBoardBranch980> DashBoardBranch980 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DashBoardTrace990> DashBoardTrace990 { get; set; }
    }
}
