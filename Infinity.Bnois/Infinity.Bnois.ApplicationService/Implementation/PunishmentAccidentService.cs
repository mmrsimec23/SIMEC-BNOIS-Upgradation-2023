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
    public class PunishmentAccidentService : IPunishmentAccidentService
    {
        private readonly IBnoisRepository<PunishmentAccident> punishmentAccidentRepository;
        private readonly IBnoisRepository<PtDeductPunishment> ptDeductPunishmentRepository;
        private readonly IBnoisRepository<PunishmentCategory> punishmentCategoryRepository;
        private readonly IBnoisRepository<EmployeeGeneral> employeeGeneralRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public PunishmentAccidentService(IBnoisRepository<EmployeeGeneral> employeeGeneralRepository, IBnoisRepository<PunishmentCategory> punishmentCategoryRepository, IBnoisRepository<PunishmentAccident> punishmentAccidentRepository, IBnoisRepository<PtDeductPunishment> ptDeductPunishmentRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.punishmentAccidentRepository = punishmentAccidentRepository;
            this.ptDeductPunishmentRepository = ptDeductPunishmentRepository;
            this.punishmentCategoryRepository = punishmentCategoryRepository;
            this.employeeGeneralRepository = employeeGeneralRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<PunishmentAccidentModel> GetPunishmentAccidents(int ps, int pn, string qs, out int total)
        {
            IQueryable<PunishmentAccident> punishmentAccidents = punishmentAccidentRepository.FilterWithInclude(x => x.IsActive
                && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee", "PunishmentCategory", "PunishmentSubCategory", "PunishmentNature", "Rank");
            total = punishmentAccidents.Count();
            punishmentAccidents = punishmentAccidents.OrderByDescending(x => x.PunishmentAccidentId).Skip((pn - 1) * ps).Take(ps);
            List<PunishmentAccidentModel> models = ObjectConverter<PunishmentAccident, PunishmentAccidentModel>.ConvertList(punishmentAccidents.ToList()).ToList();
            models = models.Select(x =>
            {
                x.TypeName = Enum.GetName(typeof(PunishmentAccidentType), x.Type);
                return x;
            }).ToList();
            return models;
        }

        public async Task<PunishmentAccidentModel> GetPunishmentAccident(int id)
        {
            if (id <= 0)
            {
                return new PunishmentAccidentModel();
            }
            PunishmentAccident punishmentAccident = await punishmentAccidentRepository.FindOneAsync(x => x.PunishmentAccidentId == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch" });
            if (punishmentAccident == null)
            {
                throw new InfinityNotFoundException("Punishment Accident not found");
            }
            PunishmentAccidentModel model = ObjectConverter<PunishmentAccident, PunishmentAccidentModel>.Convert(punishmentAccident);
            return model;
        }


        public async Task<PunishmentAccidentModel> SavePunishmentAccident(int id, PunishmentAccidentModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("PunishmentAccident  data missing");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            PunishmentAccident punishmentAccident = ObjectConverter<PunishmentAccidentModel, PunishmentAccident>.Convert(model);
            if (id > 0)
            {
                punishmentAccident = await punishmentAccidentRepository.FindOneAsync(x => x.PunishmentAccidentId == id);
                if (punishmentAccident == null)
                {
                    throw new InfinityNotFoundException("Punishment Accident not found !");
                }

                punishmentAccident.ModifiedDate = DateTime.Now;
                punishmentAccident.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PunishmentAccident";
                bnLog.TableEntryForm = "Punishment & Accident";
                bnLog.PreviousValue = "Id: " + model.PunishmentAccidentId;
                bnLog.UpdatedValue = "Id: " + model.PunishmentAccidentId;
                if (punishmentAccident.EmployeeId != model.EmployeeId)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", punishmentAccident.EmployeeId);
                    var newemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", Employee: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                    bnLog.UpdatedValue += ", Employee: " + ((dynamic)newemp).PNo + "_" + ((dynamic)newemp).FullNameEng;
                }
                if (punishmentAccident.RankId != model.RankId)
                {
                    if(punishmentAccident.RankId > 0)
                    {
                        var prevrank = employeeService.GetDynamicTableInfoById("Rank", "RankId", punishmentAccident.RankId ?? 0);
                        bnLog.PreviousValue += ", Rank: " + ((dynamic)prevrank).ShortName;
                    }
                    if (model.RankId > 0)
                    {
                        var rank = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.RankId ?? 0);
                        bnLog.UpdatedValue += ", Rank: " + ((dynamic)rank).ShortName;
                    }
                }
                if (punishmentAccident.IsBackLog != model.IsBackLog)
                {
                    bnLog.PreviousValue += ", Back Log: " + punishmentAccident.IsBackLog;
                    bnLog.UpdatedValue += ", Back Log: " + model.IsBackLog;
                }

                if (punishmentAccident.TransferId != model.TransferId)
                {
                    if(punishmentAccident.TransferId > 0)
                    {
                        var prevTransfer = employeeService.GetDynamicTableInfoById("vwTransfer", "TransferId", punishmentAccident.TransferId ?? 0);
                        bnLog.PreviousValue += ", Born/Attach/Appointment: " + ((dynamic)prevTransfer).BornOffice + '/' + ((dynamic)prevTransfer).CurrentAttach + '/' + ((dynamic)prevTransfer).Appointment;

                    }
                    if (model.TransferId > 0)
                    {
                        var newTransfer = employeeService.GetDynamicTableInfoById("vwTransfer", "TransferId", model.TransferId ?? 0);
                        bnLog.UpdatedValue += ", Born/Attach/Appointment: " + ((dynamic)newTransfer).BornOffice + '/' + ((dynamic)newTransfer).CurrentAttach + '/' + ((dynamic)newTransfer).Appointment;

                    }
                }
                if (punishmentAccident.Type != model.Type)
                {
                    bnLog.PreviousValue += ", Type: " + (punishmentAccident.Type == 1 ? "Punishment" : punishmentAccident.Type == 2 ? "Accident" : "");
                    bnLog.UpdatedValue += ", Type: " + (model.Type == 1 ? "Punishment" : model.Type == 2 ? "Accident" : "");
                }
                if (punishmentAccident.PunishmentCategoryId != model.PunishmentCategoryId)
                {
                    if (punishmentAccident.PunishmentCategoryId > 0)
                    {
                        var prevcat = employeeService.GetDynamicTableInfoById("PunishmentCategory", "PunishmentCategoryId", punishmentAccident.PunishmentCategoryId ?? 0);
                        bnLog.PreviousValue += ", Punishment Category: " + ((dynamic)prevcat).Name;
                    }
                    if (model.PunishmentCategoryId > 0)
                    {
                        var newcat = employeeService.GetDynamicTableInfoById("PunishmentCategory", "PunishmentCategoryId", model.PunishmentCategoryId ?? 0);
                        bnLog.UpdatedValue += ", Punishment Category: " + ((dynamic)newcat).Name;
                    }

                }
                if (punishmentAccident.PunishmentSubCategoryId != model.PunishmentSubCategoryId)
                {
                    if (punishmentAccident.PunishmentSubCategoryId > 0)
                    {
                        var prevsub = employeeService.GetDynamicTableInfoById("PunishmentSubCategory", "PunishmentSubCategoryId", punishmentAccident.PunishmentSubCategoryId ?? 0);
                        bnLog.PreviousValue += ", Punishment Sub Category: " + ((dynamic)prevsub).Name;
                    }
                    if (model.PunishmentSubCategoryId > 0)
                    {
                        var newsub = employeeService.GetDynamicTableInfoById("PunishmentSubCategory", "PunishmentSubCategoryId", model.PunishmentSubCategoryId ?? 0);
                        bnLog.UpdatedValue += ", Punishment Sub Category: " + ((dynamic)newsub).Name;
                    }
                }
                if (punishmentAccident.PunishmentNatureId != model.PunishmentNatureId)
                {
                    if (punishmentAccident.PunishmentNatureId > 0)
                    {
                        var prevPunishmentNature = employeeService.GetDynamicTableInfoById("PunishmentNature", "PunishmentNatureId", punishmentAccident.PunishmentNatureId ?? 0);
                        bnLog.PreviousValue += ", Punishment Nature: " + ((dynamic)prevPunishmentNature).Name;
                    }
                    if (model.PunishmentNatureId > 0)
                    {
                        var newPunishmentNature = employeeService.GetDynamicTableInfoById("PunishmentNature", "PunishmentNatureId", model.PunishmentNatureId ?? 0);
                        bnLog.UpdatedValue += ", Punishment Nature: " + ((dynamic)newPunishmentNature).Name;
                    }
                }
                if (punishmentAccident.DurationMonths != model.DurationMonths)
                {
                    bnLog.PreviousValue += ", Duration Months: " + punishmentAccident.DurationMonths;
                    bnLog.UpdatedValue += ", Duration Months: " + model.DurationMonths;
                }
                if (punishmentAccident.DurationDays != model.DurationDays)
                {
                    bnLog.PreviousValue += ", Duration Days: " + punishmentAccident.DurationDays;
                    bnLog.UpdatedValue += ", Duration Days: " + model.DurationDays;
                }

                if (punishmentAccident.AccedentType != model.AccedentType)
                {
                    bnLog.PreviousValue += ", Accedent Type: " + (punishmentAccident.AccedentType == 1 ? "Major" : punishmentAccident.AccedentType == 2 ? "Minor" : "");
                    bnLog.UpdatedValue += ", Accedent Type: " + (model.AccedentType == 1 ? "Major" : model.AccedentType == 2 ? "Minor" : "");
                }
                if (punishmentAccident.Date != model.Date)
                {
                    bnLog.PreviousValue += ", Date: " + punishmentAccident.Date.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", Date: " + model.Date?.ToString("dd/MM/yyyy");
                }
                if (punishmentAccident.Reason != model.Reason)
                {
                    bnLog.PreviousValue += ", Reason (For Report): " + punishmentAccident.Remarks;
                    bnLog.UpdatedValue += ", Reason (For Report): " + model.Remarks;
                }
                if (punishmentAccident.PunishmentType != model.PunishmentType)
                {
                    bnLog.PreviousValue +=  ", Punishment Type (For Report): " + punishmentAccident.PunishmentType;
                    bnLog.UpdatedValue += ", Punishment Type (For Report): " + model.PunishmentType;
                }
                if (punishmentAccident.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + punishmentAccident.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }
                if (punishmentAccident.FileName != model.FileName)
                {
                    bnLog.PreviousValue += ", File: " + punishmentAccident.FileName;
                    bnLog.UpdatedValue += ", File: " + model.FileName;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (punishmentAccident.FileName != model.FileName || punishmentAccident.Remarks != model.Remarks || punishmentAccident.PunishmentType != model.PunishmentType || punishmentAccident.Reason != model.Reason || punishmentAccident.Date != model.Date || punishmentAccident.AccedentType != model.AccedentType || punishmentAccident.DurationDays != model.DurationDays || punishmentAccident.DurationMonths != model.DurationMonths || punishmentAccident.PunishmentNatureId != model.PunishmentNatureId || punishmentAccident.PunishmentSubCategoryId != model.PunishmentSubCategoryId || punishmentAccident.PunishmentCategoryId != model.PunishmentCategoryId || punishmentAccident.Type != model.Type || punishmentAccident.TransferId != model.TransferId || punishmentAccident.IsBackLog != model.IsBackLog || punishmentAccident.RankId != model.RankId || punishmentAccident.EmployeeId != model.EmployeeId)
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
                punishmentAccident.IsActive = true;
                punishmentAccident.CreatedDate = DateTime.Now;
                punishmentAccident.CreatedBy = userId;
            }

            //if (punishmentAccident.Type == 1)
            //{
            //    PtDeductPunishment ptDeductPunishment = await ptDeductPunishmentRepository.FindOneAsync(x => x.PunishmentSubCategoryId == model.PunishmentSubCategoryId && x.PunishmentNatureId == model.PunishmentNatureId, new List<string> { "PunishmentNature" });
            //    if (ptDeductPunishment != null)
            //    {
            //        if (ptDeductPunishment.PunishmentNature.ShortName == null)
            //            ptDeductPunishment.PunishmentNature.ShortName = String.Empty;

            //        if (ptDeductPunishment.PunishmentNature.ShortName.Equals("NO"))
            //        {
            //            punishmentAccident.PunishmentValue = ptDeductPunishment.PunishmentValue;
            //            punishmentAccident.SkipYear = ptDeductPunishment.SkipYear;
            //            punishmentAccident.DeductPercentage = ptDeductPunishment.DeductPercentage;
            //            punishmentAccident.DeductYear = ptDeductPunishment.DeductionYear;
            //            punishmentAccident.PtAfterDeduct = ptDeductPunishment.PunishmentValue;
            //        }
            //    }

            //    PunishmentCategory punishmentCategory = await punishmentCategoryRepository.FindOneAsync(x => x.PunishmentCategoryId == model.PunishmentCategoryId);
            //    if (punishmentCategory != null)
            //    {
            //        if(punishmentCategory.ShortName == null)
            //            punishmentCategory.ShortName=String.Empty;
                 
            //        if (punishmentCategory.ShortName.Equals("FOS"))
            //        {
            //            EmployeeGeneral employeeGeneral = await employeeGeneralRepository.FindOneAsync(x => x.EmployeeId == model.EmployeeId);
            //            if (employeeGeneral != null)
            //            {
            //                if (employeeGeneral.LieutenantDate != null)
            //                {
            //                    employeeGeneral.PunishmentLtDate = employeeGeneral.LieutenantDate.Value.AddMonths(model.DurationMonths ?? 0);
            //                    employeeGeneral.PunishmentLtDate = employeeGeneral.PunishmentLtDate.Value.AddDays(model.DurationDays ?? 0);
            //                    await employeeGeneralRepository.SaveAsync(employeeGeneral);
            //                }

            //            }
            //        }
            //    }

            //}

            punishmentAccident.EmployeeId = model.EmployeeId;
            punishmentAccident.IsBackLog = model.IsBackLog;
            punishmentAccident.RankId = model.Employee.RankId;
            punishmentAccident.TransferId = model.Employee.TransferId;

            if (model.IsBackLog)
            {

                punishmentAccident.RankId = model.RankId;
                punishmentAccident.TransferId = model.TransferId;
            }


            punishmentAccident.PunishmentCategoryId = model.PunishmentCategoryId;
            punishmentAccident.PunishmentSubCategoryId = model.PunishmentSubCategoryId;
            punishmentAccident.PunishmentNatureId = model.PunishmentNatureId;
            punishmentAccident.AccedentType = model.AccedentType;
            punishmentAccident.Type = model.Type;
            punishmentAccident.DurationMonths = model.DurationMonths;
            punishmentAccident.DurationDays = model.DurationDays;
            punishmentAccident.Date =model.Date ?? punishmentAccident.Date;
            punishmentAccident.PunishmentType = model.PunishmentType;
            punishmentAccident.Remarks = model.Remarks;
            punishmentAccident.Reason = model.Reason;
            punishmentAccident.FileName = model.FileName;
            punishmentAccident.Employee = null;
            await punishmentAccidentRepository.SaveAsync(punishmentAccident);
            model.PunishmentAccidentId = punishmentAccident.PunishmentAccidentId;
            return model;
        }


        public async Task<bool> DeletePunishmentAccident(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            PunishmentAccident punishmentAccident = await punishmentAccidentRepository.FindOneAsync(x => x.PunishmentAccidentId == id);
            if (punishmentAccident == null)
            {
                throw new InfinityNotFoundException("Punishment Accident not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "PunishmentAccident";
                bnLog.TableEntryForm = "Punishment & Accident";
                bnLog.PreviousValue = "Id: " + punishmentAccident.PunishmentAccidentId;

                var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", punishmentAccident.EmployeeId);
                    bnLog.PreviousValue += ", Employee: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                
                if (punishmentAccident.RankId > 0)
                    {
                        var prevrank = employeeService.GetDynamicTableInfoById("Rank", "RankId", punishmentAccident.RankId ?? 0);
                        bnLog.PreviousValue += ", Rank: " + ((dynamic)prevrank).ShortName;
                    }
                    bnLog.PreviousValue += ", Back Log: " + punishmentAccident.IsBackLog;
                    if (punishmentAccident.TransferId > 0)
                    {
                        var prevTransfer = employeeService.GetDynamicTableInfoById("vwTransfer", "TransferId", punishmentAccident.TransferId ?? 0);
                        bnLog.PreviousValue += ", Born/Attach/Appointment: " + ((dynamic)prevTransfer).BornOffice + '/' + ((dynamic)prevTransfer).CurrentAttach + '/' + ((dynamic)prevTransfer).Appointment;

                    }
                    bnLog.PreviousValue += ", Type: " + (punishmentAccident.Type == 1 ? "Punishment" : punishmentAccident.Type == 2 ? "Accident" : "");
                    if (punishmentAccident.PunishmentCategoryId > 0)
                    {
                        var prevcat = employeeService.GetDynamicTableInfoById("PunishmentCategory", "PunishmentCategoryId", punishmentAccident.PunishmentCategoryId ?? 0);
                        bnLog.PreviousValue += ", Punishment Category: " + ((dynamic)prevcat).Name;
                    }
                    if (punishmentAccident.PunishmentSubCategoryId > 0)
                    {
                        var prevsub = employeeService.GetDynamicTableInfoById("PunishmentSubCategory", "PunishmentSubCategoryId", punishmentAccident.PunishmentSubCategoryId ?? 0);
                        bnLog.PreviousValue += ", Punishment Sub Category: " + ((dynamic)prevsub).Name;
                    }
                    if (punishmentAccident.PunishmentNatureId > 0)
                    {
                        var prevPunishmentNature = employeeService.GetDynamicTableInfoById("PunishmentNature", "PunishmentNatureId", punishmentAccident.PunishmentNatureId ?? 0);
                        bnLog.PreviousValue += ", Punishment Nature: " + ((dynamic)prevPunishmentNature).Name;
                    }
                    bnLog.PreviousValue += ", Duration Months: " + punishmentAccident.DurationMonths;
                    bnLog.PreviousValue += ", Duration Days: " + punishmentAccident.DurationDays;
                    bnLog.PreviousValue += ", Accedent Type: " + (punishmentAccident.AccedentType == 1 ? "Major" : punishmentAccident.AccedentType == 2 ? "Minor" : "");
                    bnLog.PreviousValue += ", Date: " + punishmentAccident.Date.ToString("dd/MM/yyyy");
                    bnLog.PreviousValue += ", Reason (For Report): " + punishmentAccident.Remarks;
                    bnLog.PreviousValue += ", Punishment Type (For Report): " + punishmentAccident.PunishmentType;
                    bnLog.PreviousValue += ", Remarks: " + punishmentAccident.Remarks;
                    bnLog.PreviousValue += ", File: " + punishmentAccident.FileName;
                    
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);
                return await punishmentAccidentRepository.DeleteAsync(punishmentAccident);
            }
        }

        public List<SelectModel> GetAccidentTypeSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(AccidentType)).Cast<AccidentType>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }

        public List<SelectModel> GetPunishmentAccidentTypeSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(PunishmentAccidentType)).Cast<PunishmentAccidentType>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }

        public async Task<bool> ExecutePunishmentProcess()
        {
            int result = 0;
            IQueryable<PunishmentAccident> punishments = punishmentAccidentRepository.FilterWithInclude(p => p.Type == 1 && !p.IsProcessed && p.PunishmentNature.ShortName.Equals("NO"), "PunishmentNature");
            foreach (var item in punishments.ToList())
            {
                var totalYears = (DateTime.Today - item.Date).TotalDays / 365.2425;
                if (totalYears <= item.SkipYear)
                {
                    item.PtAfterDeduct = item.PunishmentValue;
                }
                else
                {
                    var years = totalYears - item.SkipYear;

                    for (int i = 1; i <= item.DeductYear; i++)
                    {
                        if (years <= i)
                        {
                            item.YearCount = i;
                            break;
                        }
                    }

                    double r = item.DeductPercentage / 100;
                    double n = Convert.ToDouble(item.YearCount);
                    double point = item.PunishmentValue * Math.Pow((1 - r), n);
                    item.PtAfterDeduct = point;

                }

                result = await punishmentAccidentRepository.SaveAsync(item);
            }


            return result > 0;
        }
    }
}
