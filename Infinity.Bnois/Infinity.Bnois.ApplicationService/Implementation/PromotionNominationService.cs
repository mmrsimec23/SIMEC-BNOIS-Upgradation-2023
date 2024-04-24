using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.Configuration.Data;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class PromotionNominationService : IPromotionNominationService
    {
        private readonly IProcessRepository processRepository;
        private readonly IBnoisRepository<PromotionNomination> promotionNominationRepository;
        private readonly IBnoisRepository<PromotionBoard> promotionBoardRepository;
        private readonly IBnoisRepository<Employee> employeeRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        internal SqlConnection Connection;

        public PromotionNominationService(ConfigurationDbContext context,IProcessRepository processRepository, IBnoisRepository<PromotionNomination> promotionNominationRepository, IBnoisRepository<PromotionBoard> promotionBoardRepository, IBnoisRepository<Employee> employeeRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.promotionNominationRepository = promotionNominationRepository;
            this.promotionBoardRepository = promotionBoardRepository;
            this.employeeRepository = employeeRepository;
            this.processRepository = processRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
            Connection = context.Database.Connection as SqlConnection;
        }


        public async Task<PromotionNominationModel> GetPromotionNomination(int promotionNominationId)
        {
            if (promotionNominationId <= 0)
            {
                return new PromotionNominationModel();
            }
            PromotionNomination promotionNomination = await promotionNominationRepository.FindOneAsync(x => x.PromotionNominationId == promotionNominationId, new List<string> { "Employee", "Employee.Rank", "Employee.Batch" });
            if (promotionNomination == null)
            {
                throw new InfinityNotFoundException("Promotion Nomination not found");
            }
            PromotionNominationModel model = ObjectConverter<PromotionNomination, PromotionNominationModel>.Convert(promotionNomination);
            return model;
        }

        public List<PromotionNominationModel> GetPromotionNominations(int promotionBoardId, int ps, int pn, string qs, out int total,int type)
        {
            IQueryable<PromotionNomination> promotionNominations = promotionNominationRepository.FilterWithInclude(x => x.IsActive && x.PromotionBoardId == promotionBoardId && x.PromotionBoard.Type== type &&(x.Employee.PNo == (qs) || String.IsNullOrEmpty(qs)), "Employee.Rank", "ExecutionRemark", "PromotionBoard.Rank");
            total = promotionNominations.Count();
            promotionNominations = promotionNominations.OrderByDescending(x => x.PromotionNominationId).Skip((pn - 1) * ps).Take(ps);
            List<PromotionNominationModel> models = ObjectConverter<PromotionNomination, PromotionNominationModel>.ConvertList(promotionNominations.ToList()).ToList();
            return models;
        }

        public async Task<PromotionNominationModel> SavePromotionNomination(int promotionNominationId, PromotionNominationModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Promotion Nomination data missing !");
            }

            bool isEmployeeExist = promotionNominationRepository.Exists(x => x.EmployeeId == model.Employee.EmployeeId && x.PromotionBoardId==model.PromotionBoardId && x.PromotionBoard.Type==model.Type && x.FromRankId == model.FromRankId && x.ToRankId == model.ToRankId && x.PromotionNominationId != model.PromotionNominationId);

            if (isEmployeeExist)
            {
                throw new InfinityArgumentMissingException("Officer already added !");
            }
            if (model.PromotionBoardId > 0)
            {
                PromotionBoard promotionBoard = await promotionBoardRepository.FindOneAsync(x => x.PromotionBoardId == model.PromotionBoardId,new List<string> { "Rank","Rank1"});
                if (promotionBoard == null)
                {
                    throw new InfinityArgumentMissingException("Promotion Board not found !");
                }

                Employee employee = await employeeRepository.FindOneAsync(x => x.EmployeeId == model.Employee.EmployeeId,new List<string>{ "Rank" });
                if (employee == null)
                {
                    throw new InfinityNotFoundException("Officer not found !");
                }

                if (model.Type == 1)
                {
                    if (promotionBoard.Rank.RankLevel != employee.Rank.RankLevel)
                    {
                        throw new InfinityArgumentMissingException("Officer is not eligible for this board !");
                    }

                }

                bool isExist = promotionNominationRepository.Exists(x => x.EmployeeId == model.Employee.EmployeeId && x.PromotionBoardId == model.PromotionBoardId && x.PromotionNominationId != model.PromotionNominationId);
                if (isExist)
                {
                    throw new InfinityArgumentMissingException("Promotion Nomination already exists !");
                }

                model.FromRankId = promotionBoard.FromRankId;
                model.ToRankId = promotionBoard.ToRankId;
            }



            

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            PromotionNomination promotionNomination = ObjectConverter<PromotionNominationModel, PromotionNomination>.Convert(model);
            if (promotionNominationId > 0)
            {
                promotionNomination = await promotionNominationRepository.FindOneAsync(x => x.PromotionNominationId == promotionNominationId);
                if (promotionNomination == null)
                {
                    throw new InfinityNotFoundException("Promotion Nomination not found !");
                }
                promotionNomination.ModifiedDate = DateTime.Now;
                promotionNomination.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PromotionNomination";
                bnLog.TableEntryForm = "Promotion Nomination";
                bnLog.PreviousValue = "Id: " + model.PromotionNominationId;
                bnLog.UpdatedValue = "Id: " + model.PromotionNominationId;
                int bnoisUpdateCount = 0;


                if (promotionNomination.EmployeeId > 0)
                {
                    if (promotionNomination.EmployeeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", promotionNomination.EmployeeId);
                        bnLog.PreviousValue += ", P No: " + ((dynamic)prev).PNo;
                    }
                    if (model.EmployeeId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                        bnLog.UpdatedValue += ", P No: " + ((dynamic)newv).PNo;
                    }
                    //bnoisUpdateCount += 1;
                }
                if (promotionNomination.PromotionBoardId != model.PromotionBoardId)
                {
                    if (promotionNomination.PromotionBoardId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("PromotionBoard", "PromotionBoardId", promotionNomination.PromotionBoardId ?? 0);
                        bnLog.PreviousValue += ", Promotion Board: " + ((dynamic)prev).BoardName;
                    }
                    if (model.EmployeeId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("PromotionBoard", "PromotionBoardId", model.PromotionBoardId ?? 0);
                        bnLog.UpdatedValue += ", Promotion Board: " + ((dynamic)newv).BoardName;
                    }
                    bnoisUpdateCount += 1;
                }

                if (promotionNomination.IsBackLog != model.IsBackLog)
                {
                    bnLog.PreviousValue += ", Back Log: " + promotionNomination.IsBackLog;
                    bnLog.UpdatedValue += ", Back Log: " + model.IsBackLog;
                    bnoisUpdateCount += 1;
                }
                if (promotionNomination.TransferId != model.TransferId)
                {
                    if (promotionNomination.TransferId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Transfer", "TransferId", promotionNomination.TransferId ?? 0);
                        bnLog.PreviousValue += ", Transfer: " + ((dynamic)prev).TransferFor + ((dynamic)prev).TransferMode;
                    }
                    if (model.TransferId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Transfer", "TransferId", model.TransferId ?? 0);
                        bnLog.UpdatedValue += ", Transfer: " + ((dynamic)newv).TransferFor + ((dynamic)newv).TransferMode;
                    }
                    bnoisUpdateCount += 1;
                }
                if (promotionNomination.Type != model.Type)
                {
                    bnLog.PreviousValue += ", Type: " + promotionNomination.Type;
                    bnLog.UpdatedValue += ", Type: " + model.Type;
                    bnoisUpdateCount += 1;
                }
                if (promotionNomination.FromRankId != model.FromRankId)
                {
                    if (model.FromRankId > 0)
                    {
                        var rnk = employeeService.GetDynamicTableInfoById("Rank", "RankId", promotionNomination.FromRankId ?? 0);
                        bnLog.PreviousValue += ", Rank: " + ((dynamic)rnk).ShortName;
                    }
                    if (model.FromRankId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.FromRankId ?? 0);
                        bnLog.UpdatedValue += ", From Rank: " + ((dynamic)newv).ShortName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (promotionNomination.ToRankId != model.ToRankId)
                {
                    if (model.ToRankId > 0)
                    {
                        var rnk = employeeService.GetDynamicTableInfoById("Rank", "RankId", promotionNomination.ToRankId ?? 0);
                        bnLog.PreviousValue += ", To Rank: " + ((dynamic)rnk).ShortName;
                    }
                    if (model.ToRankId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.ToRankId ?? 0);
                        bnLog.UpdatedValue += ", To Rank: " + ((dynamic)newv).ShortName;
                    }
                    bnoisUpdateCount += 1;
                }                
                if (promotionNomination.EffectiveDate != model.EffectiveDate)
                {
                    bnLog.PreviousValue += ", Effective Date: " + promotionNomination.EffectiveDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Effective Date: " + model.EffectiveDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }               
                if (promotionNomination.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + promotionNomination.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                    bnoisUpdateCount += 1;
                }
                

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                if (bnoisUpdateCount > 0)
                {
                    await bnoisLogRepository.SaveAsync(bnLog);

                }
                else
                {
                    throw new InfinityNotFoundException("Please Update Any Field!");
                }
                //data log section end

            }
            else
            {
                promotionNomination.IsActive = true;
                promotionNomination.CreatedDate = DateTime.Now;
                promotionNomination.CreatedBy = userId;
            }
            promotionNomination.EmployeeId = model.Employee.EmployeeId;
            promotionNomination.PromotionBoardId = model.PromotionBoardId;
            promotionNomination.Type = model.Type;
            promotionNomination.FromRankId = model.FromRankId;
            promotionNomination.ToRankId = model.ToRankId;
            promotionNomination.Remarks = model.Remarks;
            promotionNomination.ExecutionDate = model.ExecutionDate;
            promotionNomination.EffectiveDate = model.EffectiveDate ?? promotionNomination.EffectiveDate;
            promotionNomination.ExType = model.ExType;
            promotionNomination.ExecutionRemarkId = model.ExecutionRemarkId;

            promotionNomination.TransferId = model.Employee.TransferId;
            if (model.IsBackLog)
            {
                promotionNomination.TransferId = model.TransferId;
            }
           

            promotionNomination.Employee = null;
            await promotionNominationRepository.SaveAsync(promotionNomination);
            model.PromotionNominationId = promotionNomination.PromotionNominationId;

	        await processRepository.UpdateNamingConvention(promotionNomination.EmployeeId);


			return model;
        }

        public async Task<bool> DeletePromotionNomination(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            PromotionNomination promotionNomination = await promotionNominationRepository.FindOneAsync(x => x.PromotionNominationId == id);
            if (promotionNomination == null)
            {
                throw new InfinityNotFoundException("Promotion Nomination not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PromotionNomination";
                bnLog.TableEntryForm = "Promotion Nomination";
                bnLog.PreviousValue = "Id: " + promotionNomination.PromotionNominationId;
                if (promotionNomination.EmployeeId > 0)
                {
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", promotionNomination.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)emp).PNo;
                }
                bnLog.PreviousValue += ", BackLog: " + promotionNomination.IsBackLog;
                if (promotionNomination.FromRankId > 0)
                {
                    var rnk = employeeService.GetDynamicTableInfoById("Rank", "RankId", promotionNomination.FromRankId ?? 0);
                    bnLog.PreviousValue += ", Rank: " + ((dynamic)rnk).ShortName;
                }
                if (promotionNomination.FromRankId > 0)
                {
                    var rnk = employeeService.GetDynamicTableInfoById("Rank", "RankId", promotionNomination.FromRankId ?? 0);
                    bnLog.PreviousValue += ", Rank: " + ((dynamic)rnk).ShortName;
                }

                bnLog.PreviousValue += ", Remarks: " + promotionNomination.Remarks + ", Type: " + promotionNomination.Type;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await promotionNominationRepository.DeleteAsync(promotionNomination);
            }
        }

        public List<PromotionNominationModel> GetPromotionNominations(int promotionBoardId)
        {
            IQueryable<PromotionNomination> promotionNominations = promotionNominationRepository.FilterWithInclude(x => x.IsActive && x.PromotionBoardId == promotionBoardId, "Employee.Rank", "ExecutionRemark", "PromotionBoard.Rank");
            List<PromotionNominationModel> models = ObjectConverter<PromotionNomination, PromotionNominationModel>.ConvertList(promotionNominations.ToList()).ToList();
            return models;
        }

        public async Task<List<PromotionNominationModel>> UpdatePromotionNominations(int promotionBoardId, List<PromotionNominationModel> models)
        {
            if (!models.Any())
            {
                throw new InfinityArgumentMissingException("Promotion Execution List not found !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();

            List<PromotionNomination> promotionNominations = promotionNominationRepository.Where(x => x.PromotionBoardId == promotionBoardId).ToList();
            for (int i = 0; i < promotionNominations.Count; i++)
            {
                promotionNominations[i].ExecutionRemarkId = models[i].ExecutionRemarkId;
                promotionNominations[i].EffectiveDate = models[i].EffectiveDate ?? promotionNominations[i].EffectiveDate;
                promotionNominations[i].Remarks = models[i].Remarks;
                promotionNominations[i].ModifiedBy = userId;
                promotionNominations[i].ModifiedDate = DateTime.Now;
            }
            promotionNominationRepository.UpdateAll(promotionNominations);
            models = ObjectConverter<PromotionNomination, PromotionNominationModel>.ConvertList(promotionNominations).ToList();
            return models;
        }

        public List<PromotionNominationModel> GetPromotionExecutionWithoutBoards(int ps, int pn, string qs, out int total)
        {
            IQueryable<PromotionNomination> promotionNominations = promotionNominationRepository.FilterWithInclude(x => x.IsActive  && x.PromotionBoardId == null && (x.Employee.PNo == (qs) || String.IsNullOrEmpty(qs)), "Employee.Rank", "ExecutionRemark", "Rank", "Rank1");
            total = promotionNominations.Count();
            promotionNominations = promotionNominations.OrderByDescending(x => x.PromotionNominationId).Skip((pn - 1) * ps).Take(ps);
            List<PromotionNominationModel> models = ObjectConverter<PromotionNomination, PromotionNominationModel>.ConvertList(promotionNominations.ToList()).ToList();
            return models;
        }

        public async Task<bool> ExecutePromotion(int promotionBoardId)
        {
            promotionNominationRepository.ExecWithSqlQuery(String.Format("exec [SpUpdateEmployeeCurrentStatus] {0}", promotionBoardId));
            return true;
        }

        public async Task<bool> ExecutePromotionWithOutBoard()
        {
            promotionNominationRepository.ExecWithSqlQuery(String.Format("exec [spPromotionExecutionWithoutBoard]"));
            return true;
        }

        public async Task<bool> ExecuteDatabaseBackup()
        {
            promotionNominationRepository.ExecWithSqlQuery(String.Format("exec [DataBackup]"));
            return true;
        }

        public async Task<bool> ExecuteDataScript()
        {
            //string[] tableNames = { "Commendation", "PunishmentAccident" }; // Add the names of the tables you want to include
            string[] tableNames = {
                "RankCategory"
                ,"OfficerType"
                ,"Batch"
                ,"Gender"
                ,"Rank"
                ,"EmployeeStatus"
                ,"ExecutionRemark"
                ,"Employee"
                ,"EmpRejoin"
                ,"CourseCategory"
                ,"CourseSubCategory"
                ,"Country"
                ,"Course"
                ,"TrainingInstitute"
                ,"TrainingPlan"
                ,"Relation"
                ,"Occupation"
                ,"HeirType"
                ,"HeirNextOfKinInfo"
                ,"PunishmentCategory"
                ,"PunishmentSubCategory"
                ,"PromotionBoard"
                ,"MemberRole"
                ,"Board"
                ,"Institute"
                ,"Medal"
                ,"Award"
                ,"PublicationCategory"
                ,"Publication"
                ,"MedalAward"
                ,"Category"
                ,"SubCategory"
                ,"CommissionType"
                ,"Branch"
                ,"SubBranch"
                ,"Subject"
                ,"MaritalType"
                ,"OfficerStream"
                ,"Nationality"
                ,"Religion"
                ,"ReligionCast"
                ,"EmployeeGeneral"
                ,"Pattern"
                ,"ShipCategory"
                ,"Zone"
                ,"Office"
                ,"OprOccasion"
                ,"RecomandationType"
                ,"AptNat"
                ,"AptCat"
                ,"OfficeAppointment"
                ,"EmployeeOpr"
                ,"SpecialAptType"
                ,"Suitability"
                ,"OprAptSuitability"
                ,"EmployeeTransferFuturePlan"
                ,"Spouse"
                ,"PromotionNomination"
                ,"PreviousMission"
                ,"Division"
                ,"District"
                ,"VisitCategory"
                ,"VisitSubCategory"
                ,"NominationSchedule"
                ,"MissionAppointment"
                ,"Nomination"
                ,"Transfer"
                ,"SecurityClearanceReason"
                ,"EmployeeSecurityClearance"
                ,"LprCalculateInfo"
                ,"LeaveType"
                ,"EmployeeCourseFuturePlan"
                ,"ForeignProject"
                ,"DashBoardMinuite100"
                ,"CareerForecastSetting"
                ,"CareerForecast"
                ,"LeavePurpose"
                ,"EmployeeLeave"
                ,"DashBoardBranchByAdminAuthority600Entry"
                ,"DashBoardBranchByAdminAuthority700"
                ,"TraceSetting"
                ,"BraCtryCoursePoint"
                ,"ToeAuthorized"
                ,"StatusChange"
                ,"EmployeeLeaveYear"
                ,"Photo"
                ,"PftType"
                ,"PftResult"
                ,"EmployeePft"
                ,"Sport"
                ,"EmployeeSport"
                ,"DashBoardBranch975"
                ,"ExamCategory"
                ,"Examination"
                ,"Upazila"
                ,"Address"
                ,"ServiceExamCategory"
                ,"ServiceExam"
                ,"EmployeeServiceExamResult"
                ,"CoXoService"
                ,"ResultType"
                ,"TrainingResult"
                ,"EmployeeLeaveCountry"
                ,"TransferProposal"
                ,"ProposalDetail"
                ,"MscEducationType"
                ,"MscInstitute"
                ,"MscPermissionType"
                ,"EmployeeMscEducation"
                ,"RetiredAge"
                ,"ExamSubject"
                ,"Remark"
                ,"MissionAppBranch"
                ,"RetiredEmployee"
                ,"AgeServicePolicy"
                ,"Certificate"
                ,"EmployeeFamilyPermission"
                ,"EmpRunMissing"
                ,"MissionAppRank"
                ,"ExtracurricularType"
                ,"Extracurricular"
                ,"TrainingRank"
                ,"Result"
                ,"ResultGrade"
                ,"Education"
                ,"EmployeeOther"
                ,"ObservationIntelligent"
                ,"EmployeeHajjDetail"
                ,"TrainingBranch"
                ,"DashBoardMinuite110"
                ,"DashBoardTrace990"
                ,"OfficeAppRank"
                ,"EmployeeChildren"
                ,"CoursePoint"
                ,"Color"
                ,"EyeVision"
                ,"BloodGroup"
                ,"PhysicalStructure"
                ,"MedicalCategory"
                ,"PhysicalCondition"
                ,"Commendation"
                ,"Achievement"
                ,"PreCommissionCourse"
                ,"Sibling"
                ,"PunishmentNature"
                ,"PtDeductPunishment"
                ,"LeavePolicy"
                ,"PunishmentAccident"
                ,"CarLoanFiscalYear"
                ,"EmployeeCarLoan"
                ,"SocialAttribute"
                ,"BonusPtPublic"
                ,"PoorCourseResult"
                ,"BonusPtComApp"
                ,"PreCommissionRank"
                ,"PreviousExperience"
                ,"NominationDetail"
                ,"Parent"
                ,"EmployeeServiceExt"
                ,"DashBoardBranch980"
                ,"ExtraAppointment"
                ,"ProposalCandidate"
                ,"TerminationType"
                ,"EmployeeLpr"
//                ,"BnoisLog"
                ,"DocumentUrl"
                ,"EffectType"
                ,"EmpEducation"
                ,"EmployeeRank"
                ,"EmployeeReport"
                ,"InstituteType"
                ,"MobileNumber"
                ,"OfficeShipment"
                ,"OprGrading"
                ,"PenPicture"
                ,"QuickLink"
                ,"SpecialCourse"
                ,"StoreProcedure"
                ,"TpQualifyingCriteria"
                ,"TraceCoursePoint"
                ,"TraceCourseRpt"
                ,"TraceOprRpt"
                ,"TracePenaltyRpt"
                ,"TracePftRpt"
                ,"YearList"
                ,"ZoneServiceTransfarWareHouse"
            }; // Add the names of the tables you want to include

            using (Connection)
            {
                Connection.Open();
                StringBuilder scriptBuilder = new StringBuilder();
                scriptBuilder.AppendLine("-- Script to insert data into specified tables");
                scriptBuilder.AppendLine("USE [Bnoisdb]");
                scriptBuilder.AppendLine("GO");
                foreach (string tableName in tableNames)
                {
                    // Query data from the current table
                    string query = $"SELECT * FROM [dbo].[{tableName}]";
                    SqlCommand command = new SqlCommand(query, Connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    int rowCount = table.Rows.Count;
                    int rowsProcessed = 0;

                    // Generate script to insert data into the current table
                    scriptBuilder.AppendLine($"-- Insert data into table [dbo].[{tableName}]");
                    if(rowCount > 0)
                    {
                        scriptBuilder.AppendLine($"SET IDENTITY_INSERT [dbo].[{tableName}] ON");
                    }
                    
                    foreach (DataRow row in table.Rows)
                    {
                        StringBuilder columnsBuilder = new StringBuilder();
                        StringBuilder valuesBuilder = new StringBuilder();
                        foreach (DataColumn column in table.Columns)
                        {
                            columnsBuilder.Append($"[{column.ColumnName}], ");
                        }
                        foreach (var item in row.ItemArray)
                        {
                            if (item == DBNull.Value)
                            {
                                valuesBuilder.Append("NULL, ");
                            }
                            else if (item is string)
                            {
                                string stringValue = item.ToString().Replace("'", "''"); // Replace single quotes with double single quotes
                                valuesBuilder.Append("N'");
                                valuesBuilder.Append(stringValue);
                                valuesBuilder.Append("', ");
                            }
                            else if (item is DateTime)
                            {
                                valuesBuilder.Append("N'");
                                valuesBuilder.Append(((DateTime)item).ToString("yyyy-MM-dd HH:mm:ss")); // Assuming SQL Server datetime format
                                valuesBuilder.Append("', ");
                            }
                            else if (item is bool)
                            {
                                valuesBuilder.Append(((bool)item) ? "1, " : "0, ");
                            }
                            else
                            {
                                valuesBuilder.Append($"{item}, ");
                            }
                        }

                        string columns = columnsBuilder.ToString().TrimEnd(' ', ',');
                        string values = valuesBuilder.ToString().TrimEnd(' ', ',');

                        scriptBuilder.AppendLine($"INSERT [dbo].[{tableName}] ({columns}) VALUES ({values});");

                        rowsProcessed++;
                        if (rowsProcessed % 50 == 0 && rowsProcessed < rowCount)
                        {
                            scriptBuilder.AppendLine("GO");
                            //scriptBuilder.AppendLine($"SET IDENTITY_INSERT [dbo].[{tableName}] ON");
                        }
                    }
                    if (rowCount > 0)
                    {
                        scriptBuilder.AppendLine($"SET IDENTITY_INSERT [dbo].[{tableName}] OFF");
                    }
                    
                    scriptBuilder.AppendLine(); // Add a newline between tables
                }

                // Output the generated script
                DateTime time = DateTime.Now; // Replace DateTime.Now with your actual DateTime object

                int year = time.Year;
                int month = time.Month;
                int day = time.Day;
                int hour = time.Hour;
                int minute = time.Minute;
                int second = time.Second;
                //Console.WriteLine(scriptBuilder.ToString());
                using (StreamWriter writer = new StreamWriter(@"E:\bnoip\AutoScript\script" + year + month + day + hour + minute + second + ".sql"))
                {
                    writer.Write(scriptBuilder.ToString());
                }
            }
            return true;

        }
    }
}
