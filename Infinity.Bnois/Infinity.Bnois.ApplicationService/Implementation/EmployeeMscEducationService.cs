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
                employeeMscEducation = employeeMscEducationRepository.FindOne(x => x.EmployeeMscEducationId == id, new List<string> { "Employee", "Employee.Rank", "MscEducationType", "MscInstitute", "MscPermissionType", "Country" });
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
                if (employeeMscEducation.EmployeeId != model.EmployeeId)
                {
                    var emp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", model.EmployeeId);
                    bnLog.PreviousValue += ", Name: " + employeeMscEducation.Employee.Name + " _ " + employeeMscEducation.Employee.PNo;
                    bnLog.UpdatedValue += ", Name: " + ((dynamic)emp).Name + " _ " + ((dynamic)emp).PNo;
                }
                if (employeeMscEducation.MscEducationTypeId != model.MscEducationTypeId)
                {
                    var msc = employeeService.GetDynamicTableInfoById("MscEducationType", "MscEducationTypeId", model.MscEducationTypeId??0);
                    bnLog.PreviousValue += ", MscEducationType: " + employeeMscEducation.MscEducationType.Name;
                    bnLog.UpdatedValue += ", MscEducationType: " + ((dynamic)msc).Name;
                }
                if (employeeMscEducation.MscInstituteId != model.MscInstituteId)
                {
                    var mscins = employeeService.GetDynamicTableInfoById("MscInstitute", "MscInstituteId", model.MscInstituteId ?? 0);
                    bnLog.PreviousValue += ", MscInstitute: " + employeeMscEducation.MscInstitute.Name;
                    bnLog.UpdatedValue += ", MscInstitute: " + ((dynamic)mscins).Name;
                }
                if (employeeMscEducation.MscPermissionTypeId != model.MscPermissionTypeId)
                {
                    var mscPer = employeeService.GetDynamicTableInfoById("MscPermissionType", "MscPermissionTypeId", model.MscPermissionTypeId ?? 0);
                    bnLog.PreviousValue += ", MscPermissionType: " + employeeMscEducation.MscPermissionType.Name;
                    bnLog.UpdatedValue += ", MscPermissionType: " + ((dynamic)mscPer).Name;
                }
                if (employeeMscEducation.CountryId != model.CountryId)
                {
                    var country = employeeService.GetDynamicTableInfoById("Country", "CountryId", model.CountryId ?? 0);
                    bnLog.PreviousValue += ", Country: " + employeeMscEducation.Country.FullName;
                    bnLog.UpdatedValue += ", Country: " + ((dynamic)country).FullName;
                }
                if (employeeMscEducation.RankId != model.RankId)
                {
                    var rank = employeeService.GetDynamicTableInfoById("Rank", "RankId", model.RankId ?? 0);
                    bnLog.PreviousValue += ", Rank: " + employeeMscEducation.Rank.ShortName;
                    bnLog.UpdatedValue += ", Rank: " + ((dynamic)rank).ShortName;
                }
                if (employeeMscEducation.PassingYear != model.PassingYear)
                {
                    bnLog.PreviousValue += ", PassingYear: " + employeeMscEducation.PassingYear;
                    bnLog.UpdatedValue += ", PassingYear: " + model.PassingYear;
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
                    bnLog.PreviousValue += ", FromDate: " + employeeMscEducation.FromDate;
                    bnLog.UpdatedValue += ", FromDate: " + model.FromDate;
                }
                if (employeeMscEducation.ToDate != model.ToDate)
                {
                    bnLog.PreviousValue += ", ToDate: " + employeeMscEducation.ToDate;
                    bnLog.UpdatedValue += ", ToDate: " + model.ToDate;
                }
                if (employeeMscEducation.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + employeeMscEducation.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (employeeMscEducation.EmployeeId != model.EmployeeId || employeeMscEducation.MscEducationTypeId != model.MscEducationTypeId || employeeMscEducation.Remarks != model.Remarks 
                    || employeeMscEducation.MscInstituteId != model.MscInstituteId || employeeMscEducation.MscPermissionTypeId != model.MscPermissionTypeId || employeeMscEducation.CountryId != model.CountryId
                    || employeeMscEducation.RankId != model.RankId || employeeMscEducation.PassingYear != model.PassingYear || employeeMscEducation.Results != model.Results
                    || employeeMscEducation.IsComplete != model.IsComplete || employeeMscEducation.FromDate != model.FromDate || employeeMscEducation.ToDate != model.ToDate)
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
            employeeMscEducation.Results = model.Results;
            employeeMscEducation.Remarks = model.Remarks;
            employeeMscEducation.IsComplete = model.IsComplete;
            employeeMscEducation.FromDate = model.FromDate;
            employeeMscEducation.ToDate = model.ToDate;
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
                bnLog.PreviousValue = "Id: " + employeeMscEducation.EmployeeMscEducationId + ", Name: " + employeeMscEducation.EmployeeId + ", MscEducationType: " + employeeMscEducation.MscEducationTypeId
                    + ", Remarks: " + employeeMscEducation.Remarks + ", MscInstitute: " + employeeMscEducation.MscInstituteId + ", MscPermissionType: " + employeeMscEducation.MscPermissionTypeId
                    + ", Country: " + employeeMscEducation.CountryId + ", Rank: " + employeeMscEducation.RankId + ", PassingYear: " + employeeMscEducation.PassingYear
                    + ", Results: " + employeeMscEducation.Results + ", IsComplete: " + employeeMscEducation.IsComplete + ", FromDate: " + employeeMscEducation.FromDate + ", ToDate: " + employeeMscEducation.ToDate;
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