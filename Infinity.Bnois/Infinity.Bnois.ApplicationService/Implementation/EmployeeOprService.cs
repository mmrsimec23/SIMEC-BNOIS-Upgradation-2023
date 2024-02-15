
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
    public class EmployeeOprService : IEmployeeOprService
    {

        private readonly IBnoisRepository<EmployeeOpr> employeeOprRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public EmployeeOprService(IBnoisRepository<EmployeeOpr> employeeOprRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.employeeOprRepository = employeeOprRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }


        public List<EmployeeOprModel> GetEmployeeOprs(int ps, int pn, string qs, out int total)
        {
            IQueryable<EmployeeOpr> oprAptSuitabilities = employeeOprRepository.FilterWithInclude(x => x.IsActive && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs) || x.Rank.ShortName==(qs) || String.IsNullOrEmpty(qs)), "Employee", "Rank", "OprOccasion","Office", "Office1", "OfficeAppointment");
            total = oprAptSuitabilities.Count();
            oprAptSuitabilities = oprAptSuitabilities.OrderByDescending(x => x.Id).Skip((pn - 1) * ps).Take(ps);
            List<EmployeeOprModel> models = ObjectConverter<EmployeeOpr, EmployeeOprModel>.ConvertList(oprAptSuitabilities.ToList()).ToList();
            return models;
        }


        public async Task<EmployeeOprModel> GetEmployeeOpr(int id)
        {
            if (id <= 0)
            {
                return new EmployeeOprModel();
            }
            EmployeeOpr employeeOpr = await employeeOprRepository.FindOneAsync(x => x.Id == id, new List<string> { "Employee", "Office", "Office1", "Employee.Rank", "Employee.Batch" });
            if (employeeOpr == null)
            {
                throw new InfinityNotFoundException("OPR Entry not found");
            }
            EmployeeOprModel model = ObjectConverter<EmployeeOpr, EmployeeOprModel>.Convert(employeeOpr);
            return model;
        }

        public async Task<EmployeeOprModel> SaveEmployeeOpr(int id, EmployeeOprModel model)
        {

            //bool isExist = employeeOprRepository.Exists(x =>x.OccasionId == model.OccasionId && x.EmployeeId == x.EmployeeId && x.OprFromDate==model.OprFromDate && x.OprToDate == model.OprToDate && x.Id != model.Id);
            //if (isExist)
            //{
            //    throw new InfinityInvalidDataException("Data already exists !");
            //}

            model.Employee = null;
            if (model == null)
            {
                throw new InfinityArgumentMissingException("OPR Entry data missing");
            }


            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            EmployeeOpr employeeOpr = ObjectConverter<EmployeeOprModel, EmployeeOpr>.Convert(model);

            if (id > 0)
            {
                employeeOpr = await employeeOprRepository.FindOneAsync(x => x.Id == id);

                if (employeeOpr == null)
                {
                    throw new InfinityNotFoundException("OPR Entry not found !");
                }

                employeeOpr.ModifiedDate = DateTime.Now;
                employeeOpr.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeOpr";
                bnLog.TableEntryForm = "OPR Entry";
                bnLog.PreviousValue = "Id: " + model.Id;
                bnLog.UpdatedValue = "Id: " + model.Id;
                int bnoisUpdateCount = 0;
                if (employeeOpr.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    if (employeeOpr.EmployeeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeOpr.EmployeeId);
                        bnLog.PreviousValue += ", P No: " + ((dynamic)prev).PNo;
                    }
                    if (model.EmployeeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                        bnLog.UpdatedValue += ", P No: " + ((dynamic)prev).PNo;
                    }
                }
                if (employeeOpr.BOffCId != model.BOffCId)
                {
                    if (employeeOpr.BOffCId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", employeeOpr.BOffCId ?? 0);
                        bnLog.PreviousValue += ", Born Office: " + ((dynamic)prev).ShortName;
                    }
                    if (model.BOffCId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", model.BOffCId ?? 0);
                        bnLog.UpdatedValue += ", Born Office: " + ((dynamic)prev).ShortName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (employeeOpr.OfficeId != model.OfficeId)
                {
                    if (employeeOpr.OfficeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", employeeOpr.OfficeId??0);
                        bnLog.PreviousValue += ", Attach Office: " + ((dynamic)prev).ShortName;
                    }
                    if (model.OfficeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", model.OfficeId ?? 0);
                        bnLog.UpdatedValue += ", Attach Office: " + ((dynamic)prev).ShortName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (employeeOpr.AppoinmentId != model.AppoinmentId)
                {
                    if (employeeOpr.AppoinmentId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("OfficeAppointment", "OffAppId", employeeOpr.AppoinmentId ?? 0);
                        bnLog.PreviousValue += ", Appoinment: " + ((dynamic)prev).Name;
                    }
                    if (model.AppoinmentId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("OfficeAppointment", "OffAppId", model.AppoinmentId ?? 0);
                        bnLog.UpdatedValue += ", Appoinment: " + ((dynamic)newv).Name;
                    }
                    bnoisUpdateCount += 1;
                }
                if (employeeOpr.OtherAppointment != model.OtherAppointment)
                {
                    bnLog.PreviousValue += ",  Other Appointment: " + employeeOpr.OtherAppointment;
                    bnLog.UpdatedValue += ",  Other Appointment: " + model.OtherAppointment;
                    bnoisUpdateCount += 1;
                }
                if (employeeOpr.OprRankId != model.OprRankId)
                {
                    if (employeeOpr.OprRankId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Rank", "RankId", employeeOpr.OprRankId);
                        bnLog.PreviousValue += ", OPR Rank: " + ((dynamic)prev).ShortName;
                    }
                    if (model.OprRankId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.OprRankId);
                        bnLog.UpdatedValue += ", OPR Rank: " + ((dynamic)newv).ShortName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (employeeOpr.OccasionId != model.OccasionId)
                {
                    if (employeeOpr.OccasionId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("OprOccasion", "OccasionId", employeeOpr.OccasionId);
                        bnLog.PreviousValue += ", Occasion: " + ((dynamic)prev).Title;
                    }
                    if (model.OccasionId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("OprOccasion", "OccasionId", model.OccasionId);
                        bnLog.UpdatedValue += ", Occasion: " + ((dynamic)newv).Title;
                    }
                    bnoisUpdateCount += 1;
                }
                if (employeeOpr.OprGrade != model.OprGrade)
                {
                    bnLog.PreviousValue += ", OPR Grade: " + employeeOpr.OprGrade;
                    bnLog.UpdatedValue += ", OPR Grade: " + model.OprGrade;
                    bnoisUpdateCount += 1;
                }
                if (employeeOpr.GradingStatus != model.GradingStatus)
                {
                    bnLog.PreviousValue += ",  Grading Status: " + employeeOpr.GradingStatus;
                    bnLog.UpdatedValue += ",  Grading Status: " + model.GradingStatus;
                    bnoisUpdateCount += 1;
                }
                if (employeeOpr.OprFromDate != model.OprFromDate)
                {
                    bnLog.PreviousValue += ", From Date: " + employeeOpr.OprFromDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", From Date: " + model.OprFromDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeOpr.OprToDate != model.OprToDate)
                {
                    bnLog.PreviousValue += ", To Date: " + employeeOpr.OprToDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", To Date: " + model.OprToDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (employeeOpr.RecomandationTypeId != model.RecomandationTypeId)
                {
                    if (employeeOpr.RecomandationTypeId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("RecomandationType", "RecomandationTypeId", employeeOpr.RecomandationTypeId ?? 0);
                        bnLog.PreviousValue += ", Recommendation For Promotion (Section-V): " + ((dynamic)prev).ShortName;
                    }
                    if (model.RecomandationTypeId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("RecomandationType", "RecomandationTypeId", model.RecomandationTypeId ?? 0);
                        bnLog.UpdatedValue += ", Recommendation For Promotion (Section-V): " + ((dynamic)newv).ShortName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (employeeOpr.IsAdverseRemark != model.IsAdverseRemark)
                {
                    bnLog.PreviousValue += ", Is Adverse Remark: " + employeeOpr.IsAdverseRemark;
                    bnLog.UpdatedValue += ", Is Adverse Remark: " + model.IsAdverseRemark;
                    bnoisUpdateCount += 1;
                }
                if (employeeOpr.AdverseRemark != model.AdverseRemark)
                {
                    bnLog.PreviousValue += ", Adverse Remark: " + employeeOpr.AdverseRemark;
                    bnLog.UpdatedValue += ", Adverse Remark: " + model.AdverseRemark;
                    bnoisUpdateCount += 1;
                }
                if (employeeOpr.Overweight != model.Overweight)
                {
                    bnLog.PreviousValue += ", Over Weight KG(s): " + employeeOpr.Overweight;
                    bnLog.UpdatedValue += ", Over Weight KG(s): " + model.Overweight;
                    bnoisUpdateCount += 1;
                }
                if (employeeOpr.Underweight != model.Underweight)
                {
                    bnLog.PreviousValue += ", Under Weight KG(s): " + employeeOpr.Underweight;
                    bnLog.UpdatedValue += ", Under Weight KG(s): " + model.Underweight;
                    bnoisUpdateCount += 1;
                }
                if (employeeOpr.Section2 != model.Section2)
                {
                    bnLog.PreviousValue += ", Section II: " + employeeOpr.Section2;
                    bnLog.UpdatedValue += ", Section II: " + model.Section2;
                    bnoisUpdateCount += 1;
                }
                if (employeeOpr.Section3 != model.Section3)
                {
                    bnLog.PreviousValue += ", Section III: " + employeeOpr.Section3;
                    bnLog.UpdatedValue += ", Section III: " + model.Section3;
                    bnoisUpdateCount += 1;
                }
                if (employeeOpr.Section4 != model.Section4)
                {
                    bnLog.PreviousValue += ", Section IV: " + employeeOpr.Section4;
                    bnLog.UpdatedValue += ", Section IV: " + model.Section4;
                    bnoisUpdateCount += 1;
                }
                if (employeeOpr.ApptRecom != model.ApptRecom)
                {
                    bnLog.PreviousValue += ", Appt Recom: " + employeeOpr.ApptRecom;
                    bnLog.UpdatedValue += ", Appt Recom: " + model.ApptRecom;
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

                employeeOpr.IsActive = true;
                employeeOpr.CreatedDate = DateTime.Now;
                employeeOpr.CreatedBy = userId;
            }
            employeeOpr.EmployeeId = model.EmployeeId;
            employeeOpr.OfficeId = model.OfficeId;
            employeeOpr.OprRankId = model.OprRankId;
            employeeOpr.OccasionId = model.OccasionId;
            employeeOpr.RecomandationTypeId = model.RecomandationTypeId;
            employeeOpr.OprGrade = model.OprGrade;
            employeeOpr.GradingStatus = model.GradingStatus;
            employeeOpr.OprFromDate = model.OprFromDate ?? employeeOpr.OprFromDate;
            employeeOpr.OprToDate = model.OprToDate ?? employeeOpr.OprToDate;
            employeeOpr.FileName = model.FileName;
            employeeOpr.IsAdverseRemark = model.IsAdverseRemark;
            employeeOpr.AdverseRemark = model.AdverseRemark;
            employeeOpr.Section2= model.Section2;
            employeeOpr.Section3 = model.Section3;
            employeeOpr.Section4 = model.Section4;
            employeeOpr.Overweight = model.Overweight??0;
            employeeOpr.Underweight = model.Underweight;
            employeeOpr.ApptRecom = model.ApptRecom;
            employeeOpr.BOffCId = model.BOffCId;
            employeeOpr.AppoinmentId = model.AppoinmentId;
            employeeOpr.OtherAppointment = model.OtherAppointment;
            await employeeOprRepository.SaveAsync(employeeOpr);
            model.Id = employeeOpr.Id;

            return model;
        }

        public async Task<bool> DeleteEmployeeOpr(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            EmployeeOpr employeeOpr = await employeeOprRepository.FindOneAsync(x => x.Id == id);
            if (employeeOpr == null)
            {

                throw new InfinityNotFoundException("OPR Entry not found");
            }
            else
            {


                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeOpr";
                bnLog.TableEntryForm = "OPR Entry";
                bnLog.PreviousValue = "Id: " + employeeOpr.Id;
                
                if (employeeOpr.EmployeeId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeOpr.EmployeeId);
                    bnLog.PreviousValue += ", P No: " + ((dynamic)prev).PNo;
                }
                if (employeeOpr.BOffCId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", employeeOpr.BOffCId ?? 0);
                    bnLog.PreviousValue += ", Born Office: " + ((dynamic)prev).ShortName;
                }
                if (employeeOpr.OfficeId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Office", "OfficeId", employeeOpr.OfficeId ?? 0);
                    bnLog.PreviousValue += ", Attach Office: " + ((dynamic)prev).ShortName;
                }
                
                if (employeeOpr.AppoinmentId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("OfficeAppointment", "OffAppId", employeeOpr.AppoinmentId ?? 0);
                    bnLog.PreviousValue += ", Appoinment: " + ((dynamic)prev).Name;
                }
                bnLog.PreviousValue += ",  Other Appointment: " + employeeOpr.OtherAppointment;
                if (employeeOpr.OprRankId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Rank", "RankId", employeeOpr.OprRankId);
                    bnLog.PreviousValue += ", OPR Rank: " + ((dynamic)prev).ShortName;
                }
                
                if (employeeOpr.OccasionId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("OprOccasion", "OccasionId", employeeOpr.OccasionId);
                    bnLog.PreviousValue += ", Occasion: " + ((dynamic)prev).Title;
                }
                bnLog.PreviousValue += ", OPR Grade: " + employeeOpr.OprGrade + ",  Grading Status: " + employeeOpr.GradingStatus + ", From Date: " + employeeOpr.OprFromDate?.ToString("dd/MM/yyyy") + ", To Date: " + employeeOpr.OprToDate?.ToString("dd/MM/yyyy");
                if (employeeOpr.RecomandationTypeId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("RecomandationType", "RecomandationTypeId", employeeOpr.RecomandationTypeId ?? 0);
                    bnLog.PreviousValue += ", Recommendation For Promotion (Section-V): " + ((dynamic)prev).ShortName;
                }
                bnLog.PreviousValue += ", Is Adverse Remark: " + employeeOpr.IsAdverseRemark + ", Adverse Remark: " + employeeOpr.AdverseRemark + ", Over Weight KG(s): " + employeeOpr.Overweight + ", Under Weight KG(s): " + employeeOpr.Underweight + ", Section II: " + employeeOpr.Section2 + ", Section III: " + employeeOpr.Section3 + ", Section IV: " + employeeOpr.Section4 + ", Appt Recom: " + employeeOpr.ApptRecom;


                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await employeeOprRepository.DeleteAsync(employeeOpr); ;
            }
        }

        public  async Task<EmployeeOprModel> UpdateEmployeeOpr(EmployeeOprModel model)
        {
            string userId = Configuration.ConfigurationResolver.Get().LoggedInUser.UserId;
            EmployeeOpr employeeOpr = ObjectConverter<EmployeeOprModel, EmployeeOpr>.Convert(model);

            employeeOpr = await employeeOprRepository.FindOneAsync(x => x.Id == model.Id);

            if (employeeOpr == null)
            {
                throw new InfinityNotFoundException("OPR Entry not found !");
            }
            if (model.FileName!=null)
            {
                employeeOpr.FileName = model.FileName;
            }
            if (model.ImageSec2 != null)
            {
                employeeOpr.ImageSec2 = model.ImageSec2;
            }
            if (model.ImageSec4 != null)
            {
                employeeOpr.ImageSec4 = model.ImageSec4;
            }
            employeeOpr.ModifiedDate = DateTime.Now;
            employeeOpr.ModifiedBy = userId;
            await employeeOprRepository.SaveAsync(employeeOpr);
            model =  ObjectConverter<EmployeeOpr, EmployeeOprModel>.Convert(employeeOpr);
            return model;
        }
    }
}

