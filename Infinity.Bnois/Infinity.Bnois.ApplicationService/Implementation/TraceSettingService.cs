using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Implementation
{
   public class TraceSettingService: ITraceSettingService
    {
        private readonly IBnoisRepository<TraceSetting> traceSettingRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public TraceSettingService(IBnoisRepository<TraceSetting> traceSettingRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.traceSettingRepository = traceSettingRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }
        

        public List<TraceSettingModel> GetTraceSettings(int ps, int pn, string qs, out int total)
        {
            IQueryable<TraceSetting> traceSettings = traceSettingRepository.FilterWithInclude(x => x.IsActive);
            total = traceSettings.Count();
            traceSettings = traceSettings.OrderByDescending(x => x.TraceSettingId).Skip((pn - 1) * ps).Take(ps);
            List<TraceSettingModel> models = ObjectConverter<TraceSetting, TraceSettingModel>.ConvertList(traceSettings.ToList()).ToList();
            return models;
        }

        public async Task<TraceSettingModel> GetTraceSetting(int id)
        {
            if (id <= 0)
            {
                return new TraceSettingModel();
            }
            TraceSetting traceSetting = await traceSettingRepository.FindOneAsync(x => x.TraceSettingId == id);
            if (traceSetting == null)
            {
                throw new InfinityNotFoundException("Trace Setting not found");
            }
            TraceSettingModel model = ObjectConverter<TraceSetting, TraceSettingModel>.Convert(traceSetting);
            return model;
        }

        public async Task<TraceSettingModel> SaveTraceSetting(int id, TraceSettingModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Trace Setting data missing");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            TraceSetting traceSetting = ObjectConverter<TraceSettingModel, TraceSetting>.Convert(model);
            if (id > 0)
            {
                traceSetting = await traceSettingRepository.FindOneAsync(x => x.TraceSettingId == id);
                if (traceSetting == null)
                {
                    throw new InfinityNotFoundException("Trace Setting not found !");
                }

                traceSetting.ModifiedDate = DateTime.Now;
                traceSetting.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "TraceSetting";
                bnLog.TableEntryForm = "Trace Setting";
                bnLog.PreviousValue = "Id: " + model.TraceSettingId;
                bnLog.UpdatedValue = "Id: " + model.TraceSettingId;
                int bnoisUpdateCount = 0;
                
                if (traceSetting.CreationDate != model.CreationDate)
                {
                    bnLog.PreviousValue += ", From Date: " + traceSetting.CreationDate.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", From Date: " + model.CreationDate.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (traceSetting.Active != model.Active)
                {
                    bnLog.PreviousValue += ", Active: " + traceSetting.Active;
                    bnLog.UpdatedValue += ", Active: " + model.Active;
                    bnoisUpdateCount += 1;
                }
                if (traceSetting.OPR != model.OPR)
                {
                    bnLog.PreviousValue += ", OPR: " + traceSetting.OPR;
                    bnLog.UpdatedValue += ", OPR: " + model.OPR;
                    bnoisUpdateCount += 1;
                }
                if (traceSetting.Course != model.Course)
                {
                    bnLog.PreviousValue += ", Course: " + traceSetting.Course;
                    bnLog.UpdatedValue += ", Course: " + model.Course;
                    bnoisUpdateCount += 1;
                }
                if (traceSetting.PFT != model.PFT)
                {
                    bnLog.PreviousValue += ", PFT: " + traceSetting.PFT;
                    bnLog.UpdatedValue += ", PFT: " + model.PFT;
                    bnoisUpdateCount += 1;
                }
                if (traceSetting.TotalPoint != model.TotalPoint)
                {
                    bnLog.PreviousValue += ", Total Point: " + traceSetting.TotalPoint;
                    bnLog.UpdatedValue += ", Total Point: " + model.TotalPoint;
                    bnoisUpdateCount += 1;
                }
                if (traceSetting.WeightPreRank != model.WeightPreRank)
                {
                    bnLog.PreviousValue += ", Weightage for Present Rank Service: " + traceSetting.WeightPreRank;
                    bnLog.UpdatedValue += ", Weightage for Present Rank Service: " + model.WeightPreRank;
                    bnoisUpdateCount += 1;
                }
                if (traceSetting.WeightPrevRank != model.WeightPrevRank)
                {
                    bnLog.PreviousValue += ", Weightage for Previous Rank Service: " + traceSetting.WeightPrevRank;
                    bnLog.UpdatedValue += ", Weightage for Previous Rank Service: " + model.WeightPrevRank;
                    bnoisUpdateCount += 1;
                }
                if (traceSetting.OprCount != model.OprCount)
                {
                    bnLog.PreviousValue += ", OPR Count For: " + traceSetting.OprCount;
                    bnLog.UpdatedValue += ", OPR Count For: " + model.OprCount;
                    bnoisUpdateCount += 1;
                }
                if (traceSetting.OprLastYear != model.OprLastYear)
                {
                    bnLog.PreviousValue += ", OPR Last Year: " + traceSetting.OprLastYear;
                    bnLog.UpdatedValue += ", OPR Last Year: " + model.OprLastYear;
                    bnoisUpdateCount += 1;
                }
                if (traceSetting.DivisionalFactor != model.DivisionalFactor)
                {
                    bnLog.PreviousValue += ", Divisional Factor: " + traceSetting.DivisionalFactor;
                    bnLog.UpdatedValue += ", Divisional Factor: " + model.DivisionalFactor;
                    bnoisUpdateCount += 1;
                }
                if (traceSetting.PftCountYear != model.PftCountYear)
                {
                    bnLog.PreviousValue += ", PFT Count For Last: " + traceSetting.PftCountYear;
                    bnLog.UpdatedValue += ", PFT Count For Last: " + model.PftCountYear;
                    bnoisUpdateCount += 1;
                }
                if (traceSetting.DductPtPerPft != model.DductPtPerPft)
                {
                    bnLog.PreviousValue += ", Deduct/Add Point for each PFT: " + traceSetting.DductPtPerPft;
                    bnLog.UpdatedValue += ", Deduct/Add Point for each PFT: " + model.DductPtPerPft;
                    bnoisUpdateCount += 1;
                }
                if (traceSetting.OWPenalCLOpr != model.OWPenalCLOpr)
                {
                    bnLog.PreviousValue += ", Penalty Count for Last (OPR): " + traceSetting.OWPenalCLOpr;
                    bnLog.UpdatedValue += ", Penalty Count for Last (OPR): " + model.OWPenalCLOpr;
                    bnoisUpdateCount += 1;
                }
                if (traceSetting.DductPtPerOWKG != model.DductPtPerOWKG)
                {
                    bnLog.PreviousValue += ", Deduct Point For Each Over Weight in Kgs: " + traceSetting.DductPtPerOWKG;
                    bnLog.UpdatedValue += ", Deduct Point For Each Over Weight in Kgs: " + model.DductPtPerOWKG;
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
                traceSetting.IsActive = true;
                traceSetting.CreatedDate = DateTime.Now;
                traceSetting.CreatedBy = userId;
            }
            traceSetting.Active = model.Active;
            traceSetting.CreationDate = model.CreationDate;
            traceSetting.OPR = model.OPR;
            traceSetting.Course = model.Course;
            traceSetting.PFT = model.PFT;
            traceSetting.TotalPoint = model.TotalPoint;
            traceSetting.WeightPreRank = model.WeightPreRank;
            traceSetting.WeightPrevRank = model.WeightPrevRank;
            traceSetting.OprCount = model.OprCount;
            traceSetting.OprLastYear = model.OprLastYear;
            traceSetting.DivisionalFactor = model.DivisionalFactor;
            traceSetting.PftCountYear = model.PftCountYear;
            traceSetting.DductPtPerPft = model.DductPtPerPft;
            traceSetting.OWPenalCLOpr = model.OWPenalCLOpr;
            traceSetting.DductPtPerOWKG = model.DductPtPerOWKG;
       
            await traceSettingRepository.SaveAsync(traceSetting);
            model.TraceSettingId = traceSetting.TraceSettingId;
            return model;
        }

        public async Task<bool> DeleteTraceSetting(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            TraceSetting traceSetting = await traceSettingRepository.FindOneAsync(x => x.TraceSettingId == id);
            if (traceSetting == null)
            {
                throw new InfinityNotFoundException("Trace Setting not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "TraceSetting";
                bnLog.TableEntryForm = "Trace Setting";
                bnLog.PreviousValue = "Id: " + traceSetting.TraceSettingId;
                int bnoisUpdateCount = 0;

                bnLog.PreviousValue += ", From Date: " + traceSetting.CreationDate.ToString("dd/MM/yyyy") + ", Active: " + traceSetting.Active + ", OPR: " + traceSetting.OPR + ", Course: " + traceSetting.Course + ", PFT: " + traceSetting.PFT + ", Total Point: " + traceSetting.TotalPoint + ", Weightage for Present Rank Service: " + traceSetting.WeightPreRank + ", Weightage for Previous Rank Service: " + traceSetting.WeightPrevRank + ", OPR Count For: " + traceSetting.OprCount + ", OPR Last Year: " + traceSetting.OprLastYear + ", Divisional Factor: " + traceSetting.DivisionalFactor + ", PFT Count For Last: " + traceSetting.PftCountYear + ", Deduct/Add Point for each PFT: " + traceSetting.DductPtPerPft + ", Penalty Count for Last (OPR): " + traceSetting.OWPenalCLOpr + ", Deduct Point For Each Over Weight in Kgs: " + traceSetting.DductPtPerOWKG;

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end

                return await traceSettingRepository.DeleteAsync(traceSetting);
            }
        }
    }
}
