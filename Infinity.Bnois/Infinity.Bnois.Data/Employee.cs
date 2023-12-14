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
    
    public partial class Employee
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employee()
        {
            this.Achievement = new HashSet<Achievement>();
            this.Achievement1 = new HashSet<Achievement>();
            this.Address = new HashSet<Address>();
            this.BoardMember = new HashSet<BoardMember>();
            this.Education = new HashSet<Education>();
            this.EmployeeCourseFuturePlan = new HashSet<EmployeeCourseFuturePlan>();
            this.EmployeeDollarSign = new HashSet<EmployeeDollarSign>();
            this.EmployeeLeave = new HashSet<EmployeeLeave>();
            this.EmployeeLpr = new HashSet<EmployeeLpr>();
            this.EmployeeOpr = new HashSet<EmployeeOpr>();
            this.EmployeeOther = new HashSet<EmployeeOther>();
            this.EmployeePft = new HashSet<EmployeePft>();
            this.EmployeeSport = new HashSet<EmployeeSport>();
            this.EmployeeServiceExt = new HashSet<EmployeeServiceExt>();
            this.EmployeeTransferFuturePlan = new HashSet<EmployeeTransferFuturePlan>();
            this.EmpRejoin = new HashSet<EmpRejoin>();
            this.EmpRunMissing = new HashSet<EmpRunMissing>();
            this.ExtraAppointment = new HashSet<ExtraAppointment>();
            this.Extracurricular = new HashSet<Extracurricular>();
            this.ForeignProject = new HashSet<ForeignProject>();
            this.HeirNextOfKinInfo = new HashSet<HeirNextOfKinInfo>();
            this.LprCalculateInfo = new HashSet<LprCalculateInfo>();
            this.MedalAward = new HashSet<MedalAward>();
            this.NominationDetail = new HashSet<NominationDetail>();
            this.ObservationIntelligent = new HashSet<ObservationIntelligent>();
            this.ObservationIntelligent1 = new HashSet<ObservationIntelligent>();
            this.Parent = new HashSet<Parent>();
            this.Photo = new HashSet<Photo>();
            this.PreCommissionCourse = new HashSet<PreCommissionCourse>();
            this.PreviousExperience = new HashSet<PreviousExperience>();
            this.PreviousLeave = new HashSet<PreviousLeave>();
            this.PreviousMission = new HashSet<PreviousMission>();
            this.PreviousPunishment = new HashSet<PreviousPunishment>();
            this.PreviousTransfer = new HashSet<PreviousTransfer>();
            this.PromotionNomination = new HashSet<PromotionNomination>();
            this.PunishmentAccident = new HashSet<PunishmentAccident>();
            this.RetiredEmployee = new HashSet<RetiredEmployee>();
            this.SeaService = new HashSet<SeaService>();
            this.Sibling = new HashSet<Sibling>();
            this.SocialAttribute = new HashSet<SocialAttribute>();
            this.Spouse = new HashSet<Spouse>();
            this.TrainingResult = new HashSet<TrainingResult>();
            this.Transfer = new HashSet<Transfer>();
            this.EmployeeHajjDetail = new HashSet<EmployeeHajjDetail>();
            this.StatusChange = new HashSet<StatusChange>();
            this.PhysicalCondition = new HashSet<PhysicalCondition>();
            this.EmployeeFamilyPermission = new HashSet<EmployeeFamilyPermission>();
            this.EmployeeCarLoan = new HashSet<EmployeeCarLoan>();
            this.EmployeeSecurityClearance = new HashSet<EmployeeSecurityClearance>();
            this.EmployeeChildren = new HashSet<EmployeeChildren>();
            this.CareerForecast = new HashSet<CareerForecast>();
            this.EmployeeGeneral = new HashSet<EmployeeGeneral>();
            this.CoXoService = new HashSet<CoXoService>();
            this.Remark = new HashSet<Remark>();
            this.EmployeeMscEducation = new HashSet<EmployeeMscEducation>();
            this.EmployeeServiceExamResult = new HashSet<EmployeeServiceExamResult>();
            this.DashBoardBranch975 = new HashSet<DashBoardBranch975>();
            this.DashBoardBranch980 = new HashSet<DashBoardBranch980>();
            this.ProposalCandidate = new HashSet<ProposalCandidate>();
        }
    
        public int EmployeeId { get; set; }
        public int RankCategoryId { get; set; }
        public int OfficerTypeId { get; set; }
        public int CountryId { get; set; }
        public string ReferenceId { get; set; }
        public string PNo { get; set; }
        public string BnNo { get; set; }
        public string FullNameEng { get; set; }
        public string Name { get; set; }
        public string FullNameBan { get; set; }
        public Nullable<int> BatchId { get; set; }
        public string BatchPosition { get; set; }
        public int GenderId { get; set; }
        public int RankId { get; set; }
        public int EmployeeStatusId { get; set; }
        public Nullable<int> SLCode { get; set; }
        public bool HasDollarSign { get; set; }
        public string Reason { get; set; }
        public Nullable<System.DateTime> DateOfDollarSign { get; set; }
        public Nullable<int> TransferId { get; set; }
        public Nullable<int> Seniority { get; set; }
        public string NamingConv { get; set; }
        public string Notification { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<int> ExecutionRemarkId { get; set; }
        public Nullable<System.DateTime> BExecutionDate { get; set; }
        public string BSpRemarks { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public bool Active { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Achievement> Achievement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Achievement> Achievement1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Address> Address { get; set; }
        public virtual Batch Batch { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BoardMember> BoardMember { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Education> Education { get; set; }
        public virtual EmployeeStatus EmployeeStatus { get; set; }
        public virtual ExecutionRemark ExecutionRemark { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual OfficerType OfficerType { get; set; }
        public virtual Rank Rank { get; set; }
        public virtual RankCategory RankCategory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeCourseFuturePlan> EmployeeCourseFuturePlan { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeDollarSign> EmployeeDollarSign { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeLeave> EmployeeLeave { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeLpr> EmployeeLpr { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeOpr> EmployeeOpr { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeOther> EmployeeOther { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeePft> EmployeePft { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeSport> EmployeeSport { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeServiceExt> EmployeeServiceExt { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeTransferFuturePlan> EmployeeTransferFuturePlan { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmpRejoin> EmpRejoin { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmpRunMissing> EmpRunMissing { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExtraAppointment> ExtraAppointment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Extracurricular> Extracurricular { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForeignProject> ForeignProject { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HeirNextOfKinInfo> HeirNextOfKinInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LprCalculateInfo> LprCalculateInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MedalAward> MedalAward { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NominationDetail> NominationDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ObservationIntelligent> ObservationIntelligent { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ObservationIntelligent> ObservationIntelligent1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Parent> Parent { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Photo> Photo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PreCommissionCourse> PreCommissionCourse { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PreviousExperience> PreviousExperience { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PreviousLeave> PreviousLeave { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PreviousMission> PreviousMission { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PreviousPunishment> PreviousPunishment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PreviousTransfer> PreviousTransfer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PromotionNomination> PromotionNomination { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PunishmentAccident> PunishmentAccident { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RetiredEmployee> RetiredEmployee { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SeaService> SeaService { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sibling> Sibling { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SocialAttribute> SocialAttribute { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Spouse> Spouse { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrainingResult> TrainingResult { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transfer> Transfer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeHajjDetail> EmployeeHajjDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StatusChange> StatusChange { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PhysicalCondition> PhysicalCondition { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeFamilyPermission> EmployeeFamilyPermission { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeCarLoan> EmployeeCarLoan { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeSecurityClearance> EmployeeSecurityClearance { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeChildren> EmployeeChildren { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CareerForecast> CareerForecast { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeGeneral> EmployeeGeneral { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CoXoService> CoXoService { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Remark> Remark { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeMscEducation> EmployeeMscEducation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EmployeeServiceExamResult> EmployeeServiceExamResult { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DashBoardBranch975> DashBoardBranch975 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DashBoardBranch980> DashBoardBranch980 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProposalCandidate> ProposalCandidate { get; set; }
    }
}
