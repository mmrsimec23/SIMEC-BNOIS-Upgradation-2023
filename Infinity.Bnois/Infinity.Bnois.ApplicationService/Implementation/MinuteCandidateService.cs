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
    public class MinuteCandidateService : IMinuteCandidateService
    {
        private readonly IBnoisRepository<DashBoardMinuite110> minuteCandidateRepository;
        public MinuteCandidateService(IBnoisRepository<DashBoardMinuite110> minuteCandidateRepository)
        {
            this.minuteCandidateRepository = minuteCandidateRepository;
        }

       
        public List<DashBoardMinuite110Model> GetMinuteCandidates(int minuiteId)
        {
            IQueryable<DashBoardMinuite110> minuteCandidates = minuteCandidateRepository.FilterWithInclude(x => x.IsActive && x.MinuiteId == minuiteId, "Employee");
            minuteCandidates = minuteCandidates.OrderByDescending(x => x.MinuiteCandidateId);
            List<DashBoardMinuite110Model> models = ObjectConverter<DashBoardMinuite110, DashBoardMinuite110Model>.ConvertList(minuteCandidates.ToList()).ToList();
            return models;
        }

        public async Task<DashBoardMinuite110Model> SaveMinuteCadidate(int id, DashBoardMinuite110Model model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Minute Candidate Officer data missing");
            }

            bool isExistData = minuteCandidateRepository.Exists(x => x.MinuiteId == model.MinuiteId && x.EmployeeId == model.EmployeeId && x.MinuiteCandidateId != id);
            if (isExistData)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            DashBoardMinuite110 minuteCadidate = ObjectConverter<DashBoardMinuite110Model, DashBoardMinuite110>.Convert(model);

            minuteCadidate.IsActive = true;
            minuteCadidate.CreatedDate = DateTime.Now;
            minuteCadidate.CreatedBy = userId;
            minuteCadidate.MinuiteId = model.MinuiteId;
            minuteCadidate.ProposedBillet = model.ProposedBillet;
            minuteCadidate.Remarks1 = model.Remarks1;
            minuteCadidate.Remarks2 = model.Remarks2;
            minuteCadidate.Remarks3 = model.Remarks3;
            minuteCadidate.Remarks4 = model.Remarks4;
            minuteCadidate.Remarks5 = model.Remarks5;
            minuteCadidate.Remarks6 = model.Remarks6;
            minuteCadidate.EmployeeId = model.EmployeeId;
            minuteCadidate.Employee = null;
            await minuteCandidateRepository.SaveAsync(minuteCadidate);
            model.MinuiteCandidateId = minuteCadidate.MinuiteCandidateId;
            return model;
        }

        public async Task<bool> DeleteMinuteCadidate(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            DashBoardMinuite110 cadidate = await minuteCandidateRepository.FindOneAsync(x => x.MinuiteCandidateId == id);
            if (cadidate == null)
            {
                throw new InfinityNotFoundException("Proposal Cadidate not found");
            }
            else
            {
                return await minuteCandidateRepository.DeleteAsync(cadidate);
            }
        }
    }
}
