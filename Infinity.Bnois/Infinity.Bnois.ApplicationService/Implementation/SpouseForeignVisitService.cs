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
    public class SpouseForeignVisitService : ISpouseForeignVisitService
    {
        private readonly IBnoisRepository<SpouseForeignVisit> spouseForignVisitRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public SpouseForeignVisitService(IBnoisRepository<SpouseForeignVisit> spouseForignVisitRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.spouseForignVisitRepository = spouseForignVisitRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }
        public List<SpouseForeignVisitModel> GetSpouseForeignVisits(int spouseId)
        {
            ICollection<SpouseForeignVisit> spouseForeignVisits = spouseForignVisitRepository.FilterWithInclude(x => x.IsActive && x.SpouseId==spouseId,"Spouse","Country").ToList();
            List<SpouseForeignVisitModel> models = ObjectConverter<SpouseForeignVisit, SpouseForeignVisitModel>.ConvertList(spouseForeignVisits).ToList();
            return models;
        }
        public async Task<SpouseForeignVisitModel> GetSpouseForeignVisit(int spouseForeignVisitId)
        {
            if (spouseForeignVisitId <= 0)
            {
                return new SpouseForeignVisitModel();
            }
            SpouseForeignVisit spouseForeignVisit = await spouseForignVisitRepository.FindOneAsync(x => x.SpouseForeignVisitId == spouseForeignVisitId);
            if (spouseForeignVisit == null)
            {
                throw new InfinityNotFoundException("Spouse Forign Visit not found");
            }
            SpouseForeignVisitModel model = ObjectConverter<SpouseForeignVisit, SpouseForeignVisitModel>.Convert(spouseForeignVisit);
            return model;
        }

        public async Task<SpouseForeignVisitModel> SaveSpouseForeignVisit(int spouseForeignVisitId, SpouseForeignVisitModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Division data missing");
            }
            
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            SpouseForeignVisit spouseForeignVisit = ObjectConverter<SpouseForeignVisitModel, SpouseForeignVisit>.Convert(model);
            if (spouseForeignVisitId > 0)
            {
                spouseForeignVisit = await spouseForignVisitRepository.FindOneAsync(x => x.SpouseForeignVisitId == spouseForeignVisitId);
                if (spouseForeignVisit == null)
                {
                    throw new InfinityNotFoundException("Spouse Foreign Visit not found !");
                }

                spouseForeignVisit.ModifiedDate = DateTime.Now;
                spouseForeignVisit.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "SpouseForeignVisit";
                bnLog.TableEntryForm = "Employee's Spouse Foreign Visit Information";
                bnLog.PreviousValue = "Id: " + model.SpouseForeignVisitId;
                bnLog.UpdatedValue = "Id: " + model.SpouseForeignVisitId;
                int bnoisUpdateCount = 0;
                if (spouseForeignVisit.SpouseId != model.SpouseId)
                {
                    bnLog.PreviousValue += ", SpouseId: " + spouseForeignVisit.SpouseId;
                    bnLog.UpdatedValue += ", SpouseId: " + model.SpouseId;
                    bnoisUpdateCount += 1;
                }
                if (spouseForeignVisit.CountryId != model.CountryId)
                {
                    if (spouseForeignVisit.CountryId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Country", "CountryId", spouseForeignVisit.CountryId);
                        bnLog.PreviousValue += ", Country: " + ((dynamic)prev).FullName;
                    }
                    if (model.CountryId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Country", "CountryId", model.CountryId);
                        bnLog.UpdatedValue += ", Country: " + ((dynamic)newv).FullName;
                    }
                    bnoisUpdateCount += 1;
                }
                if (spouseForeignVisit.Purpose != model.Purpose)
                {
                    bnLog.PreviousValue += ", Purpose: " + spouseForeignVisit.Purpose;
                    bnLog.UpdatedValue += ", Purpose: " + model.Purpose;
                    bnoisUpdateCount += 1;
                }
                if (spouseForeignVisit.AccompaniedBy != model.AccompaniedBy)
                {
                    bnLog.PreviousValue += ", Accompanied By: " + spouseForeignVisit.AccompaniedBy;
                    bnLog.UpdatedValue += ", Accompanied By: " + model.AccompaniedBy;
                    bnoisUpdateCount += 1;
                }
                if (spouseForeignVisit.StayFromDate != model.StayFromDate)
                {
                    bnLog.PreviousValue += ", From Date: " + spouseForeignVisit.StayFromDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", From Date: " + model.StayFromDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }
                if (spouseForeignVisit.StayToDate != model.StayToDate)
                {
                    bnLog.PreviousValue += ", To Date: " + spouseForeignVisit.StayToDate?.ToString("dd/MM/yyyy");
                    bnLog.UpdatedValue += ", To Date: " + model.StayToDate?.ToString("dd/MM/yyyy");
                    bnoisUpdateCount += 1;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
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
                spouseForeignVisit.IsActive = true;
                spouseForeignVisit.CreatedDate = DateTime.Now;
                spouseForeignVisit.CreatedBy = userId;
            }
            spouseForeignVisit.SpouseId = model.SpouseId;
            spouseForeignVisit.CountryId = model.CountryId;
            spouseForeignVisit.Purpose = model.Purpose;
            spouseForeignVisit.AccompaniedBy = model.AccompaniedBy;
            spouseForeignVisit.StayFromDate = model.StayFromDate;
            spouseForeignVisit.StayToDate = model.StayToDate;

            await spouseForignVisitRepository.SaveAsync(spouseForeignVisit);
            model.SpouseForeignVisitId = spouseForeignVisit.SpouseForeignVisitId;
            return model;
        }
    }
}
