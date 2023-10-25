using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;

namespace Infinity.Bnois.ApplicationService.Implementation
{
	public class OccupationService : IOccupationService
	{
		private readonly IBnoisRepository<Occupation> occupationRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public OccupationService(IBnoisRepository<Occupation> occupationRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
		{
			this.occupationRepository = occupationRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }
        public async Task<OccupationModel> GetOccupation(int id)
        {
            if (id <= 0)
            {
                return new OccupationModel();
            }
            Occupation occupation = await occupationRepository.FindOneAsync(x => x.OccupationId == id);
            if (occupation == null)
            {
                throw new InfinityNotFoundException("Occupation not found");
            }
            OccupationModel model = ObjectConverter<Occupation, OccupationModel>.Convert(occupation);
            return model;
        }


		public List<OccupationModel> GetOccupations(int ps, int pn, string qs, out int total)
        {
            IQueryable<Occupation> occupations = occupationRepository.FilterWithInclude(x => x.IsActive
                  && ((x.Name.Contains(qs) || String.IsNullOrEmpty(qs))));
            total = occupations.Count();
	        occupations = occupations.OrderByDescending(x => x.OccupationId).Skip((pn - 1) * ps).Take(ps);
            List<OccupationModel> models = ObjectConverter<Occupation, OccupationModel>.ConvertList(occupations.ToList()).ToList();
            return models;
            
        }



        public async Task<OccupationModel> SaveOccupation(int id, OccupationModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Occupation data missing");
            }

            bool isExist = occupationRepository.Exists(x=> x.Name == model.Name && x.OccupationId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            Occupation occupation = ObjectConverter<OccupationModel, Occupation>.Convert(model);
            if (id > 0)
            {
	            occupation = await occupationRepository.FindOneAsync(x => x.OccupationId == id);
                if (occupation == null)
                {
                    throw new InfinityNotFoundException("Occupation not found !");
                }

	            occupation.ModifiedDate = DateTime.Now;
	            occupation.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Occupation";
                bnLog.TableEntryForm = "Occupation";
                bnLog.PreviousValue = "Id: " + model.OccupationId;
                bnLog.UpdatedValue = "Id: " + model.OccupationId;
                if (occupation.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + occupation.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (occupation.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + occupation.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (occupation.Name != model.Name || occupation.Remarks != model.Remarks)
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
	            occupation.IsActive = true;
	            occupation.CreatedDate = DateTime.Now;
	            occupation.CreatedBy = userId;
			
            }
	        occupation.Name = model.Name;

	        occupation.Remarks = model.Remarks;
            await occupationRepository.SaveAsync(occupation);
            model.OccupationId = occupation.OccupationId;
            return model;
        }

        public async Task<bool> DeleteOccupation(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Occupation occupation = await occupationRepository.FindOneAsync(x => x.OccupationId == id);
            if (occupation == null)
            {
                throw new InfinityNotFoundException("Occupation not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Occupation";
                bnLog.TableEntryForm = "Occupation";
                bnLog.PreviousValue = "Id: " + occupation.OccupationId + ", Name: " + occupation.Name + ", Remarks: " + occupation.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await occupationRepository.DeleteAsync(occupation);
            }
        }

        public async Task<List<SelectModel>> GetOccupationSelectModels()
        {
            ICollection<Occupation> Occupations = await occupationRepository.FilterAsync(x => x.IsActive);
            List<SelectModel> selectModels = Occupations.OrderBy(x=>x.Name).Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.OccupationId
            }).ToList();
            return selectModels;
        }

     
    }
}
