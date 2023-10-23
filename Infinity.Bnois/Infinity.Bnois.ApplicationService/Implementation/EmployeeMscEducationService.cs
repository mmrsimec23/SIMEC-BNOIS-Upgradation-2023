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
        public EmployeeMscEducationService(IBnoisRepository<EmployeeMscEducation> employeeMscEducationRepository)
        {
            this.employeeMscEducationRepository = employeeMscEducationRepository;
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
                employeeMscEducation = await employeeMscEducationRepository.FindOneAsync(x => x.EmployeeMscEducationId == id);
                if (employeeMscEducation == null)
                {
                    throw new InfinityNotFoundException("Employee Msc Education not found !");
                }


                employeeMscEducation.ModifiedDate = DateTime.Now;
                employeeMscEducation.ModifiedBy = userId;
            }
            else
            {
                employeeMscEducation.CreatedDate = DateTime.Now;
                employeeMscEducation.CreatedBy = userId;
                employeeMscEducation.IsActive = true;
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

            employeeMscEducation.Employee = null;
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
                return await employeeMscEducationRepository.DeleteAsync(employeeMscEducation);
            }
        }
    }
}