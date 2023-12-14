﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class BnoisDbContext : DbContext
    {
        public BnoisDbContext()
            : base("name=BnoisDbContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<AptCat> AptCats { get; set; }
        public virtual DbSet<AttributeType> AttributeTypes { get; set; }
        public virtual DbSet<BloodGroup> BloodGroups { get; set; }
        public virtual DbSet<Board> Boards { get; set; }
        public virtual DbSet<BoardMember> BoardMembers { get; set; }
        public virtual DbSet<BotherSisterInfo> BotherSisterInfoes { get; set; }
        public virtual DbSet<Color> Colors { get; set; }
        public virtual DbSet<Commendation> Commendations { get; set; }
        public virtual DbSet<CommissionType> CommissionTypes { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<Division> Divisions { get; set; }
        public virtual DbSet<EducationPreviou> EducationPrevious { get; set; }
        public virtual DbSet<EffectType> EffectTypes { get; set; }
        public virtual DbSet<EmployeeDollarSign> EmployeeDollarSigns { get; set; }
        public virtual DbSet<EmployeeLeaveCountry> EmployeeLeaveCountries { get; set; }
        public virtual DbSet<EmployeeLeaveYear> EmployeeLeaveYears { get; set; }
        public virtual DbSet<EmployeePft> EmployeePfts { get; set; }
        public virtual DbSet<EmployeePromotion> EmployeePromotions { get; set; }
        public virtual DbSet<EmployeeRank> EmployeeRanks { get; set; }
        public virtual DbSet<EmployeeSport> EmployeeSports { get; set; }
        public virtual DbSet<EmpRejoin> EmpRejoins { get; set; }
        public virtual DbSet<ExamSubject> ExamSubjects { get; set; }
        public virtual DbSet<Extracurricular> Extracurriculars { get; set; }
        public virtual DbSet<ExtracurricularType> ExtracurricularTypes { get; set; }
        public virtual DbSet<EyeVision> EyeVisions { get; set; }
        public virtual DbSet<Gender> Genders { get; set; }
        public virtual DbSet<HeirType> HeirTypes { get; set; }
        public virtual DbSet<InstituteType> InstituteTypes { get; set; }
        public virtual DbSet<LeavePolicy> LeavePolicies { get; set; }
        public virtual DbSet<LeavePurpose> LeavePurposes { get; set; }
        public virtual DbSet<LeaveType> LeaveTypes { get; set; }
        public virtual DbSet<LprCalculateInfo> LprCalculateInfoes { get; set; }
        public virtual DbSet<MaritalType> MaritalTypes { get; set; }
        public virtual DbSet<MedicalCategory> MedicalCategories { get; set; }
        public virtual DbSet<MemberRole> MemberRoles { get; set; }
        public virtual DbSet<MissionAppBranch> MissionAppBranches { get; set; }
        public virtual DbSet<MissionAppointment> MissionAppointments { get; set; }
        public virtual DbSet<MissionAppRank> MissionAppRanks { get; set; }
        public virtual DbSet<Nationality> Nationalities { get; set; }
        public virtual DbSet<NominationSchedule> NominationSchedules { get; set; }
        public virtual DbSet<Occupation> Occupations { get; set; }
        public virtual DbSet<OfficerStream> OfficerStreams { get; set; }
        public virtual DbSet<OfficerType> OfficerTypes { get; set; }
        public virtual DbSet<OprAptSuitability> OprAptSuitabilities { get; set; }
        public virtual DbSet<OprGrading> OprGradings { get; set; }
        public virtual DbSet<OprOccasion> OprOccasions { get; set; }
        public virtual DbSet<Pattern> Patterns { get; set; }
        public virtual DbSet<PftResult> PftResults { get; set; }
        public virtual DbSet<PftType> PftTypes { get; set; }
        public virtual DbSet<PhysicalStructure> PhysicalStructures { get; set; }
        public virtual DbSet<PreCommissionCourseDetail> PreCommissionCourseDetails { get; set; }
        public virtual DbSet<PreCommissionRank> PreCommissionRanks { get; set; }
        public virtual DbSet<PromotionPolicy> PromotionPolicies { get; set; }
        public virtual DbSet<Publication> Publications { get; set; }
        public virtual DbSet<PublicationCategory> PublicationCategories { get; set; }
        public virtual DbSet<PunishmentCategory> PunishmentCategories { get; set; }
        public virtual DbSet<PunishmentNature> PunishmentNatures { get; set; }
        public virtual DbSet<RankCategory> RankCategories { get; set; }
        public virtual DbSet<RankMap> RankMaps { get; set; }
        public virtual DbSet<Relation> Relations { get; set; }
        public virtual DbSet<Relative> Relatives { get; set; }
        public virtual DbSet<Religion> Religions { get; set; }
        public virtual DbSet<ReligionCast> ReligionCasts { get; set; }
        public virtual DbSet<ResultType> ResultTypes { get; set; }
        public virtual DbSet<RetiredAge> RetiredAges { get; set; }
        public virtual DbSet<SecurityClearanceReason> SecurityClearanceReasons { get; set; }
        public virtual DbSet<ServiceExam> ServiceExams { get; set; }
        public virtual DbSet<ServiceExamCategory> ServiceExamCategories { get; set; }
        public virtual DbSet<ShipCategory> ShipCategories { get; set; }
        public virtual DbSet<SpecialAptType> SpecialAptTypes { get; set; }
        public virtual DbSet<Sport> Sports { get; set; }
        public virtual DbSet<SpouseForeignVisit> SpouseForeignVisits { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }
        public virtual DbSet<Suitability> Suitabilities { get; set; }
        public virtual DbSet<TerminationType> TerminationTypes { get; set; }
        public virtual DbSet<TraceSetting> TraceSettings { get; set; }
        public virtual DbSet<TrainingBranch> TrainingBranches { get; set; }
        public virtual DbSet<TrainingInstitute> TrainingInstitutes { get; set; }
        public virtual DbSet<Upazila> Upazilas { get; set; }
        public virtual DbSet<VisitCategory> VisitCategories { get; set; }
        public virtual DbSet<VisitSubCategory> VisitSubCategories { get; set; }
        public virtual DbSet<YearList> YearLists { get; set; }
        public virtual DbSet<Zone> Zones { get; set; }
        public virtual DbSet<EmployeeTraining> EmployeeTrainings { get; set; }
        public virtual DbSet<RecomandationType> RecomandationType { get; set; }
        public virtual DbSet<BonusPtAward> BonusPtAward { get; set; }
        public virtual DbSet<BonusPtMedal> BonusPtMedal { get; set; }
        public virtual DbSet<AgeServicePolicy> AgeServicePolicy { get; set; }
        public virtual DbSet<BonusPtComApp> BonusPtComApps { get; set; }
        public virtual DbSet<Medal> Medals { get; set; }
        public virtual DbSet<SeaService> SeaServices { get; set; }
        public virtual DbSet<EmployeeStatus> EmployeeStatus { get; set; }
        public virtual DbSet<OfficeAppBranch> OfficeAppBranch { get; set; }
        public virtual DbSet<OfficeAppRank> OfficeAppRank { get; set; }
        public virtual DbSet<CoursePoint> CoursePoint { get; set; }
        public virtual DbSet<Rank> Rank { get; set; }
        public virtual DbSet<SubBranch> SubBranch { get; set; }
        public virtual DbSet<Branch> Branch { get; set; }
        public virtual DbSet<Certificate> Certificate { get; set; }
        public virtual DbSet<RetiredEmpCertificate> RetiredEmpCertificate { get; set; }
        public virtual DbSet<RetiredEmpCountry> RetiredEmpCountry { get; set; }
        public virtual DbSet<RetiredEmployee> RetiredEmployee { get; set; }
        public virtual DbSet<SocialAttribute> SocialAttribute { get; set; }
        public virtual DbSet<SearchColumn> SearchColumn { get; set; }
        public virtual DbSet<EmployeeServiceExt> EmployeeServiceExt { get; set; }
        public virtual DbSet<AptNat> AptNat { get; set; }
        public virtual DbSet<UsedReport> UsedReports { get; set; }
        public virtual DbSet<UsedStoreProcedure> UsedStoreProcedures { get; set; }
        public virtual DbSet<Sibling> Sibling { get; set; }
        public virtual DbSet<ResultGrade> ResultGrade { get; set; }
        public virtual DbSet<Result> Results { get; set; }
        public virtual DbSet<ExamCategory> ExamCategories { get; set; }
        public virtual DbSet<Examination> Examinations { get; set; }
        public virtual DbSet<Batch> Batches { get; set; }
        public virtual DbSet<PunishmentSubCategory> PunishmentSubCategories { get; set; }
        public virtual DbSet<Parent> Parents { get; set; }
        public virtual DbSet<OfficeShipment> OfficeShipment { get; set; }
        public virtual DbSet<PtDeductPunishment> PtDeductPunishment { get; set; }
        public virtual DbSet<PoorCourseResult> PoorCourseResult { get; set; }
        public virtual DbSet<NominationDetail> NominationDetail { get; set; }
        public virtual DbSet<EmployeeLeave> EmployeeLeaves { get; set; }
        public virtual DbSet<PromotionNomination> PromotionNominations { get; set; }
        public virtual DbSet<EmpRunMissing> EmpRunMissings { get; set; }
        public virtual DbSet<HeirNextOfKinInfo> HeirNextOfKinInfo { get; set; }
        public virtual DbSet<Nomination> Nomination { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Photo> Photo { get; set; }
        public virtual DbSet<BonusPtPublic> BonusPtPublic { get; set; }
        public virtual DbSet<TrainingPlan> TrainingPlans { get; set; }
        public virtual DbSet<BraCtryCoursePoint> BraCtryCoursePoints { get; set; }
        public virtual DbSet<PreviousExperience> PreviousExperiences { get; set; }
        public virtual DbSet<ForeignProject> ForeignProjects { get; set; }
        public virtual DbSet<EmployeeOther> EmployeeOthers { get; set; }
        public virtual DbSet<Spouse> Spouses { get; set; }
        public virtual DbSet<PreviousMission> PreviousMissions { get; set; }
        public virtual DbSet<PreviousPunishment> PreviousPunishments { get; set; }
        public virtual DbSet<PreviousLeave> PreviousLeaves { get; set; }
        public virtual DbSet<PreviousTransfer> PreviousTransfers { get; set; }
        public virtual DbSet<TrainingRank> TrainingRanks { get; set; }
        public virtual DbSet<Award> Awards { get; set; }
        public virtual DbSet<CourseSubCategory> CourseSubCategories { get; set; }
        public virtual DbSet<vwNomination> vwNominations { get; set; }
        public virtual DbSet<vwTransfer> vwTransfers { get; set; }
        public virtual DbSet<Subject> Subjects { get; set; }
        public virtual DbSet<MedalAward> MedalAwards { get; set; }
        public virtual DbSet<PunishmentAccident> PunishmentAccidents { get; set; }
        public virtual DbSet<ProposalDetail> ProposalDetails { get; set; }
        public virtual DbSet<CourseCategory> CourseCategories { get; set; }
        public virtual DbSet<QuickLink> QuickLinks { get; set; }
        public virtual DbSet<PromotionBoard> PromotionBoards { get; set; }
        public virtual DbSet<TransferProposal> TransferProposals { get; set; }
        public virtual DbSet<Education> Educations { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<ObservationIntelligent> ObservationIntelligent { get; set; }
        public virtual DbSet<EmployeeCourseFuturePlan> EmployeeCourseFuturePlan { get; set; }
        public virtual DbSet<EmployeeTransferFuturePlan> EmployeeTransferFuturePlan { get; set; }
        public virtual DbSet<Transfer> Transfer { get; set; }
        public virtual DbSet<EmployeeReport> EmployeeReport { get; set; }
        public virtual DbSet<TrainingResult> TrainingResult { get; set; }
        public virtual DbSet<ExtraAppointment> ExtraAppointment { get; set; }
        public virtual DbSet<Achievement> Achievement { get; set; }
        public virtual DbSet<EmployeeLpr> EmployeeLpr { get; set; }
        public virtual DbSet<PreCommissionCourse> PreCommissionCourse { get; set; }
        public virtual DbSet<EmployeeOpr> EmployeeOpr { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<ExecutionRemark> ExecutionRemark { get; set; }
        public virtual DbSet<EmployeeHajjDetail> EmployeeHajjDetail { get; set; }
        public virtual DbSet<StatusChange> StatusChange { get; set; }
        public virtual DbSet<PhysicalCondition> PhysicalCondition { get; set; }
        public virtual DbSet<EmployeeFamilyPermission> EmployeeFamilyPermission { get; set; }
        public virtual DbSet<CarLoanFiscalYear> CarLoanFiscalYear { get; set; }
        public virtual DbSet<EmployeeCarLoan> EmployeeCarLoan { get; set; }
        public virtual DbSet<MscInstitute> MscInstitute { get; set; }
        public virtual DbSet<MscPermissionType> MscPermissionType { get; set; }
        public virtual DbSet<MscEducationType> MscEducationType { get; set; }
        public virtual DbSet<EmployeeSecurityClearance> EmployeeSecurityClearance { get; set; }
        public virtual DbSet<CareerForecastSetting> CareerForecastSetting { get; set; }
        public virtual DbSet<BnoisLog> BnoisLog { get; set; }
        public virtual DbSet<Office> Office { get; set; }
        public virtual DbSet<EmployeeChildren> EmployeeChildren { get; set; }
        public virtual DbSet<CareerForecast> CareerForecast { get; set; }
        public virtual DbSet<EmployeeGeneral> EmployeeGeneral { get; set; }
        public virtual DbSet<CoXoService> CoXoService { get; set; }
        public virtual DbSet<Institute> Institute { get; set; }
        public virtual DbSet<Remark> Remark { get; set; }
        public virtual DbSet<EmployeeMscEducation> EmployeeMscEducation { get; set; }
        public virtual DbSet<EmployeeServiceExamResult> EmployeeServiceExamResult { get; set; }
        public virtual DbSet<ToeAuthorized> ToeAuthorized { get; set; }
        public virtual DbSet<DashBoardBranchByAdminAuthority600Entry> DashBoardBranchByAdminAuthority600Entry { get; set; }
        public virtual DbSet<DashBoardBranchByAdminAuthority700> DashBoardBranchByAdminAuthority700 { get; set; }
        public virtual DbSet<DashBoardBranch975> DashBoardBranch975 { get; set; }
        public virtual DbSet<DashBoardBranch980> DashBoardBranch980 { get; set; }
        public virtual DbSet<OfficeAppointment> OfficeAppointment { get; set; }
        public virtual DbSet<ProposalCandidate> ProposalCandidate { get; set; }
    
        public virtual ObjectResult<GetLeaveInfo_Result> GetLeaveInfo(string employeeID, Nullable<int> idFrom, Nullable<int> idTo)
        {
            var employeeIDParameter = employeeID != null ?
                new ObjectParameter("EmployeeID", employeeID) :
                new ObjectParameter("EmployeeID", typeof(string));
    
            var idFromParameter = idFrom.HasValue ?
                new ObjectParameter("IdFrom", idFrom) :
                new ObjectParameter("IdFrom", typeof(int));
    
            var idToParameter = idTo.HasValue ?
                new ObjectParameter("IdTo", idTo) :
                new ObjectParameter("IdTo", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetLeaveInfo_Result>("GetLeaveInfo", employeeIDParameter, idFromParameter, idToParameter);
        }
    }
}
