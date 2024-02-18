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
    public class CoFfRecomService : ICoFfRecomService
    {
        private readonly IBnoisRepository<DashBoardBranch980> coFfRecomRepository;
        public CoFfRecomService(IBnoisRepository<DashBoardBranch980> coFfRecomRepository)
        {
            this.coFfRecomRepository = coFfRecomRepository;
        }

       
        public List<CoFfRecomModel> GetCOFFRecoms(int type)
        {
            IQueryable<DashBoardBranch980> coffrecoms = coFfRecomRepository.FilterWithInclude(x => x.IsActive && x.RecomStatus==type, "Employee");
            coffrecoms = coffrecoms.OrderByDescending(x => x.Id);
            List<CoFfRecomModel> models = ObjectConverter<DashBoardBranch980, CoFfRecomModel>.ConvertList(coffrecoms.ToList()).ToList();
            return models;
        }

        public async Task<CoFfRecomModel> SaveCOFFRecom(int id, CoFfRecomModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("CO FF Recom data missing");
            }

            bool isExistData = coFfRecomRepository.Exists(x => x.EmployeeId == model.EmployeeId && x.Id != id);
            if (isExistData)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            DashBoardBranch980 coffrecoms = ObjectConverter<CoFfRecomModel, DashBoardBranch980>.Convert(model);

            coffrecoms.IsActive = true;
            coffrecoms.CreatedDate = DateTime.Now;
            coffrecoms.CreatedBy = userId;
            coffrecoms.EmployeeId = model.EmployeeId;
            coffrecoms.RankId = model.Employee.RankId;
            coffrecoms.RecomStatus = model.RecomStatus;
            coffrecoms.Remarks = model.Remarks;
            coffrecoms.Employee = null;
            await coFfRecomRepository.SaveAsync(coffrecoms);
            model.Id = coffrecoms.Id;
            return model;
        }

        public async Task<bool> DeleteCOFFRecom(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            DashBoardBranch980 coffrecoms = await coFfRecomRepository.FindOneAsync(x => x.Id == id);
            if (coffrecoms == null)
            {
                throw new InfinityNotFoundException("CO FF Recom not found");
            }
            else
            {
                return await coFfRecomRepository.DeleteAsync(coffrecoms);
            }
        }
    }
}
