using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class EmployeeMscEducationService: IEmployeeMscEducationService
    {

        private readonly IBnoisRepository<EmployeeMscEducation> employeeMscEducationRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public EmployeeMscEducationService(IBnoisRepository<EmployeeMscEducation> employeeMscEducationRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.employeeMscEducationRepository = employeeMscEducationRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<SelectModel> GetMscCompleteTypeSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(MscCompleteType)).Cast<MscCompleteType>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }


        public List<EmployeeMscEducationModel> GetEmployeeMscEducations(int ps, int pn, string qs, out int total)
        {

            IQueryable<EmployeeMscEducation> employeeMscEducations = employeeMscEducationRepository
                .FilterWithInclude(x => x.IsActive && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee", "MscEducationType", "MscInstitute", "MscPermissionType", "Country");
            total = employeeMscEducations.Count();
            employeeMscEducations = employeeMscEducations.OrderByDescending(x => x.EmployeeMscEducationId).Skip((pn - 1) * ps).Take(ps);
            List<EmployeeMscEducationModel> models = ObjectConverter<EmployeeMscEducation, EmployeeMscEducationModel>.ConvertList(employeeMscEducations.ToList()).ToList();
            return models;
        }

        public async Task<EmployeeMscEducationModel> GetEmployeeMscEducation(int id)
        {
            if (id == 0)
            {
                return new EmployeeMscEducationModel();
            }
            EmployeeMscEducation employeeMscEducation = await employeeMscEducationRepository.FindOneAsync(x => x.EmployeeMscEducationId == id, new List<string> { "Employee","Employee.Rank","Employee.Batch" });
            if (employeeMscEducation == null)
            {
                throw new InfinityNotFoundException("Employee Msc Education not found");
            }
            EmployeeMscEducationModel model = ObjectConverter<EmployeeMscEducation, EmployeeMscEducationModel>.Convert(employeeMscEducation);
            return model;
        }

        public async Task<EmployeeMscEducationModel> SaveEmployeeMscEducation(int id, EmployeeMscEducationModel model)
        {

            if (model == null)
            {
                throw new InfinityArgumentMissingException("Employee Msc Education data missing");
            }
            EmployeeMscEducation employeeMscEducation = ObjectConverter<EmployeeMscEducationModel, EmployeeMscEducation>.Convert(model);
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();

            if (id > 0)
            {
                employeeMscEducation = employeeMscEducationRepository.FindOne(x => x.EmployeeMscEducationId == id);
                if (employeeMscEducation == null)
                {
                    throw new InfinityNotFoundException("Employee Msc Education not found !");
                }


                employeeMscEducation.ModifiedDate = DateTime.Now;
                employeeMscEducation.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeMscEducation";
                bnLog.TableEntryForm = "Officer Msc Edu";
                bnLog.PreviousValue = "Id: " + model.EmployeeMscEducationId;
                bnLog.UpdatedValue = "Id: " + model.EmployeeMscEducationId;
                if (employeeMscEducation.EmployeeId > 0 || model.EmployeeId > 0)
                {
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeMscEducation.EmployeeId);
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)prevemp).PNo;
                    bnLog.UpdatedValue += ", PNo: " + ((dynamic)emp).PNo;
                }
                if (employeeMscEducation.MscEducationTypeId != model.MscEducationTypeId)
                {
                    if (employeeMscEducation.MscEducationTypeId > 0)
                    {
                        var prevmsc = employeeService.GetDynamicTableInfoById("MscEducationType", "MscEducationTypeId", employeeMscEducation.MscEducationTypeId ?? 0);
                        bnLog.PreviousValue += ", MscEducationType: " + ((dynamic)prevmsc).Name;
                    }
                    if (model.MscEducationTypeId > 0)
                    {
                        var msc = employeeService.GetDynamicTableInfoById("MscEducationType", "MscEducationTypeId", model.MscEducationTypeId ?? 0);
                        bnLog.UpdatedValue += ", MscEducationType: " + ((dynamic)msc).Name;
                    }
                }
                if (employeeMscEducation.MscInstituteId != model.MscInstituteId)
                {
                    if (employeeMscEducation.MscInstituteId > 0)
                    {
                        var prevmscins = employeeService.GetDynamicTableInfoById("MscInstitute", "MscInstituteId", employeeMscEducation.MscInstituteId ?? 0);
                        bnLog.PreviousValue += ", Msc Institute: " + ((dynamic)prevmscins).Name;
                    }
                    if (model.MscInstituteId > 0)
                    {
                        var mscins = employeeService.GetDynamicTableInfoById("MscInstitute", "MscInstituteId", model.MscInstituteId ?? 0);
                        bnLog.UpdatedValue += ", Msc Institute: " + ((dynamic)mscins).Name;
                    }
                }
                if (employeeMscEducation.MscPermissionTypeId != model.MscPermissionTypeId)
                {
                    if (employeeMscEducation.MscPermissionTypeId > 0)
                    {
                        var prevmscPer = employeeService.GetDynamicTableInfoById("MscPermissionType", "MscPermissionTypeId", employeeMscEducation.MscPermissionTypeId ?? 0);
                        bnLog.PreviousValue += ", Msc Permission Type: " + ((dynamic)prevmscPer).Name;
                    }
                    if (model.MscPermissionTypeId > 0)
                    {
                        var mscPer = employeeService.GetDynamicTableInfoById("MscPermissionType", "MscPermissionTypeId", model.MscPermissionTypeId ?? 0);
                        bnLog.UpdatedValue += ", MscPermissionType: " + ((dynamic)mscPer).Name;
                    }
                }
                if (employeeMscEducation.CountryId != model.CountryId)
                {
                    if (employeeMscEducation.MscPermissionTypeId > 0)
                    {
                        var prevcountry = employeeService.GetDynamicTableInfoById("Country", "CountryId", employeeMscEducation.CountryId ?? 0);
                        bnLog.PreviousValue += ", Country: " + ((dynamic)prevcountry).FullName;
                    }
                    if (model.MscPermissionTypeId > 0)
                    {
                        var country = employeeService.GetDynamicTableInfoById("Country", "CountryId", model.CountryId ?? 0);
                        bnLog.UpdatedValue += ", Country: " + ((dynamic)country).FullName;
                    }
                }
                if (employeeMscEducation.RankId != model.RankId)
                {
                    if (employeeMscEducation.MscPermissionTypeId > 0)
                    {
                        var prevrank = employeeService.GetDynamicTableInfoById("Rank", "RankId", employeeMscEducation.RankId ?? 0);
                        bnLog.PreviousValue += ", Rank: " + ((dynamic)prevrank).ShortName;
                    }
                    if (model.MscPermissionTypeId > 0)
                    {
                        var rank = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.RankId ?? 0);
                        bnLog.UpdatedValue += ", Rank: " + ((dynamic)rank).ShortName;
                    }
                }
                if (employeeMscEducation.PassingYear != model.PassingYear)
                {
                    bnLog.PreviousValue += ", PassingYear: " + employeeMscEducation.PassingYear;
                    bnLog.UpdatedValue += ", PassingYear: " + model.PassingYear;
                }
                if (employeeMscEducation.PermissionYear != model.PermissionYear)
                {
                    bnLog.PreviousValue += ", Permission Year: " + employeeMscEducation.PermissionYear;
                    bnLog.UpdatedValue += ", Permission Year: " + model.PermissionYear;
                }
                if (employeeMscEducation.Results != model.Results)
                {
                    bnLog.PreviousValue += ", Results: " + employeeMscEducation.Results;
                    bnLog.UpdatedValue += ", Results: " + model.Results;
                }
                if (employeeMscEducation.IsComplete != model.IsComplete)
                {
                    bnLog.PreviousValue += ", IsComplete: " + employeeMscEducation.IsComplete;
                    bnLog.UpdatedValue += ", IsComplete: " + model.IsComplete;
                }
                if (employeeMscEducation.FromDate != model.FromDate)
                {
                    bnLog.PreviousValue += ", FromDate: " + employeeMscEducation.FromDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", FromDate: " + model.FromDate?.ToString("dd/MM/yyyy");
                }
                if (employeeMscEducation.ToDate != model.ToDate)
                {
                    bnLog.PreviousValue += ", ToDate: " + employeeMscEducation.ToDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", ToDate: " + model.ToDate?.ToString("dd/MM/yyyy");
                }
                if (employeeMscEducation.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + employeeMscEducation.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }
                if (employeeMscEducation.CompleteStatus != model.CompleteStatus)
                {
                    bnLog.PreviousValue += ", Complete Status: " + employeeMscEducation.CompleteStatus;
                    bnLog.UpdatedValue += ", Complete Status: " + model.CompleteStatus;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (employeeMscEducation.EmployeeId != model.EmployeeId || employeeMscEducation.MscEducationTypeId != model.MscEducationTypeId || employeeMscEducation.Remarks != model.Remarks 
                    || employeeMscEducation.MscInstituteId != model.MscInstituteId || employeeMscEducation.MscPermissionTypeId != model.MscPermissionTypeId || employeeMscEducation.CountryId != model.CountryId
                    || employeeMscEducation.RankId != model.RankId || employeeMscEducation.PassingYear != model.PassingYear || employeeMscEducation.PermissionYear != model.PermissionYear || employeeMscEducation.Results != model.Results
                    || employeeMscEducation.IsComplete != model.IsComplete || employeeMscEducation.FromDate != model.FromDate || employeeMscEducation.ToDate != model.ToDate || employeeMscEducation.CompleteStatus != model.CompleteStatus)
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
                employeeMscEducation.CreatedDate = DateTime.Now;
                employeeMscEducation.CreatedBy = userId;
                employeeMscEducation.IsActive = true;
                employeeMscEducation.Employee = null;
            }
            employeeMscEducation.EmployeeId = model.EmployeeId;
            employeeMscEducation.MscEducationTypeId = model.MscEducationTypeId;
            employeeMscEducation.MscInstituteId = model.MscInstituteId;
            employeeMscEducation.MscPermissionTypeId = model.MscPermissionTypeId;
            employeeMscEducation.CountryId = model.CountryId;
            employeeMscEducation.PassingYear = model.PassingYear;
            employeeMscEducation.PermissionYear = model.PermissionYear;
            employeeMscEducation.Results = model.Results;
            employeeMscEducation.Remarks = model.Remarks;
            employeeMscEducation.IsComplete = model.IsComplete;
            employeeMscEducation.FromDate = model.FromDate;
            employeeMscEducation.ToDate = model.ToDate;
            employeeMscEducation.CompleteStatus = model.CompleteStatus;
            employeeMscEducation.RankId = model.Employee.RankId;

            //if (model.IsBackLog)
            //{
            //    employeeMscEducation.RankId = model.RankId;
            //    employeeMscEducation.TransferId = model.TransferId;
            //}

            //employeeMscEducation.Employee = null;
            
            await employeeMscEducationRepository.SaveAsync(employeeMscEducation);
            model.EmployeeMscEducationId = employeeMscEducation.EmployeeMscEducationId;
            return model;
        }

        public async Task<bool> DeleteEmployeeMscEducation(int id)
        {
            if (id <= 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            EmployeeMscEducation employeeMscEducation = await employeeMscEducationRepository.FindOneAsync(x => x.EmployeeMscEducationId == id);
            if (employeeMscEducation == null)
            {
                throw new InfinityNotFoundException("Employee Family Permission not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "EmployeeMscEducation";
                bnLog.TableEntryForm = "Officer Msc Edu";
                bnLog.PreviousValue = "Id: " + employeeMscEducation.EmployeeMscEducationId;
                if (employeeMscEducation.EmployeeId > 0)
                {
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", employeeMscEducation.EmployeeId);
                    bnLog.PreviousValue += ", PNo: " + ((dynamic)emp).PNo;
                }

                if (employeeMscEducation.MscEducationTypeId > 0)
                {
                    var prevmsc = employeeService.GetDynamicTableInfoById("MscEducationType", "MscEducationTypeId", employeeMscEducation.MscEducationTypeId ?? 0);
                    bnLog.PreviousValue += ", MscEducationType: " + ((dynamic)prevmsc).Name;
                }
                if (employeeMscEducation.MscInstituteId > 0)
                {
                    var prevmscins = employeeService.GetDynamicTableInfoById("MscInstitute", "MscInstituteId", employeeMscEducation.MscInstituteId ?? 0);
                    bnLog.PreviousValue += ", Msc Institute: " + ((dynamic)prevmscins).Name;
                }
                if (employeeMscEducation.MscPermissionTypeId > 0)
                {
                    var prevmscPer = employeeService.GetDynamicTableInfoById("MscPermissionType", "MscPermissionTypeId", employeeMscEducation.MscPermissionTypeId ?? 0);
                    bnLog.PreviousValue += ", Msc Permission Type: " + ((dynamic)prevmscPer).Name;
                }
                if (employeeMscEducation.CountryId > 0)
                {
                    var prevcountry = employeeService.GetDynamicTableInfoById("Country", "CountryId", employeeMscEducation.CountryId ?? 0);
                    bnLog.PreviousValue += ", Country: " + ((dynamic)prevcountry).FullName;
                }
                if (employeeMscEducation.RankId > 0)
                {
                    var prevrank = employeeService.GetDynamicTableInfoById("Rank", "RankId", employeeMscEducation.RankId ?? 0);
                    bnLog.PreviousValue += ", Rank: " + ((dynamic)prevrank).ShortName;
                }
                bnLog.PreviousValue +=  ", PassingYear: " + employeeMscEducation.PassingYear + ", Results: " + employeeMscEducation.Results + ", IsComplete: " + employeeMscEducation.IsComplete + 
                    ", From Date: " + employeeMscEducation.FromDate?.ToString("dd/MM/yyyy") + ", To Date: " + employeeMscEducation.ToDate?.ToString("dd/MM/yyyy") + ", Remarks: " + employeeMscEducation.Remarks + ", Complete Status: " + employeeMscEducation.CompleteStatus;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await employeeMscEducationRepository.DeleteAsync(employeeMscEducation);
            }
        }
    }
}