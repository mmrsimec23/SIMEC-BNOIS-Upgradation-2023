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
  public  class MinuiteService: IMinuiteService
    {
        private readonly IBnoisRepository<DashBoardMinuite100> minuiteRepository;
        private readonly IProposalDetailService proposalDetailService;
        public MinuiteService(IBnoisRepository<DashBoardMinuite100> minuiteRepository, IProposalDetailService proposalDetailService)
        {
            this.minuiteRepository = minuiteRepository;
            this.proposalDetailService = proposalDetailService;
        }
        public List<DashBoardMinuite100Model> GetMinuites(int ps, int pn, string qs, out int total)
        {
            
                IQueryable<DashBoardMinuite100>  minuites = minuiteRepository.FilterWithInclude(x => x.IsActive && (x.MinuiteName.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee","Country");
                total = minuites.Count();
                minuites = minuites.OrderByDescending(x => x.MinuiteId).Skip((pn - 1) * ps).Take(ps);
                List<DashBoardMinuite100Model> models = ObjectConverter<DashBoardMinuite100, DashBoardMinuite100Model>.ConvertList(minuites.ToList()).ToList();
                return models;
            
        }
        public async Task<DashBoardMinuite100Model> GetMinuite(int id)
        {
            if (id <= 0)
            {
                return new DashBoardMinuite100Model();
            }
            DashBoardMinuite100 minuite = await minuiteRepository.FindOneAsync(x => x.MinuiteId == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch" });
            if (minuite == null)
            {
                throw new InfinityNotFoundException("Transfer Proposal not found");
            }
            DashBoardMinuite100Model model = ObjectConverter<DashBoardMinuite100, DashBoardMinuite100Model>.Convert(minuite);
            return model;
        }

        public async Task<DashBoardMinuite100Model> SaveMinuite(int id, DashBoardMinuite100Model model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Transfer Proposal data missing");
            }
            //bool isExist = transferProposalRepository.Exists(x => x.Name == model.Name && x.TransferProposalId != id);
            //if (isExist)
            //{
            //    throw new InfinityInvalidDataException("Data already exists !");
            //}

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            DashBoardMinuite100 minuite = ObjectConverter<DashBoardMinuite100Model, DashBoardMinuite100>.Convert(model);
            if (id > 0)
            {
                minuite = await minuiteRepository.FindOneAsync(x => x.MinuiteId == id);
                if (minuite == null)
                {
                    throw new InfinityNotFoundException("Transfer Proposal not found !");
                }

                minuite.ModifiedDate = DateTime.Now;
                minuite.ModifiedBy = userId;
            }
            else
            {
                minuite.IsActive = true;
                minuite.CreatedDate = DateTime.Now;
                minuite.CreatedBy = userId;
            }
            minuite.MinuiteNo = model.MinuiteNo;
            minuite.NextMinuiteNo = model.NextMinuiteNo;
            minuite.MinuiteCategory = model.MinuiteCategory;
            minuite.MinuiteName = model.MinuiteName;
            minuite.LetterNo = model.LetterNo;
            minuite.OfferedBy = model.OfferedBy;
            minuite.Vacency = model.Vacency;
            minuite.Nominate = model.Nominate;
            minuite.Purpose = model.Purpose;
            minuite.StartDate = model.StartDate;
            minuite.EndDate = model.EndDate;
            minuite.Location = model.Location;
            minuite.CountryId = model.CountryId;
            minuite.RankName = model.RankName;
            minuite.BranchName = model.BranchName;
            minuite.officerRankof = model.officerRankof;
            minuite.EmployeeId = model.EmployeeId;
            minuite.MinuiteDate = model.MinuiteDate;
            minuite.PreRequQul = model.PreRequQul;
            minuite.Remarks = model.Remarks;
            minuite.ServiceExperiance = model.ServiceExperiance;
            minuite.Competencies = model.Competencies;
            minuite.ExtraRemarks1 = model.ExtraRemarks1;
            minuite.ExtraRemarks2 = model.ExtraRemarks2;
            minuite.ExtraRemarks3 = model.ExtraRemarks3;
            minuite.ExtraRemarks4 = model.ExtraRemarks4;
            minuite.ExtraRemarks5 = model.ExtraRemarks5;
            minuite.ExtraRemarks6 = model.ExtraRemarks6;
            minuite.ExtraRemarks7 = model.ExtraRemarks7;
            minuite.ExtraRemarks8 = model.ExtraRemarks8;
            minuite.ExtraRemarks9 = model.ExtraRemarks9;
            if (model.Employee != null)
            {
                minuite.EmployeeId = model.Employee.EmployeeId;
            }
            


            minuite.Employee = null;

            await minuiteRepository.SaveAsync(minuite);
            model.MinuiteId = minuite.MinuiteId;

            if(model.MinuiteId > 0)
            {
                //ProposalDetailModel pdm = new ProposalDetailModel();
                //pdm.TransferProposalId = model.TransferProposalId;
                //pdm.TransferType = 1;
                //pdm.AttachOfficeId = 1;

                //await proposalDetailService.SaveProposalDetail(0,pdm);

            }
            return model;
        }
        public async Task<bool> DeleteMinuite(int id)
        {

            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            DashBoardMinuite100 minuite = await minuiteRepository.FindOneAsync(x => x.MinuiteId == id);
            if (minuite == null)
            {
                throw new InfinityNotFoundException("Division not found");
            }
            else
            {
                return await minuiteRepository.DeleteAsync(minuite);
            }
        }
        
    }
}
