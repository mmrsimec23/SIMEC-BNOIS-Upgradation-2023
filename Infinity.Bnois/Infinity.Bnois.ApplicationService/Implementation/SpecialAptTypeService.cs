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
	public class SpecialAptTypeService : ISpecialAptTypeService
	{	

		private readonly IBnoisRepository<SpecialAptType> specialAptTypeRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public SpecialAptTypeService(IBnoisRepository<SpecialAptType> specialAptTypeRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
		{
			this.specialAptTypeRepository = specialAptTypeRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }


        public List<SpecialAptTypeModel> GetSpecialAptTypes(int ps, int pn, string qs, out int total)
        {
            IQueryable<SpecialAptType> specialAptTypes = specialAptTypeRepository.FilterWithInclude(x => x.IsActive && (x.SpAptType.Contains(qs) || String.IsNullOrEmpty(qs) ));
            total = specialAptTypes.Count();
            specialAptTypes = specialAptTypes.OrderByDescending(x => x.Id).Skip((pn - 1) * ps).Take(ps);
            List<SpecialAptTypeModel> models = ObjectConverter<SpecialAptType, SpecialAptTypeModel>.ConvertList(specialAptTypes.ToList()).ToList();
            return models;
        }

        public async Task<SpecialAptTypeModel> GetSpecialAptType(int id)
        {
            if (id <= 0)
            {
                return new SpecialAptTypeModel();
            }
            SpecialAptType specialAptType = await specialAptTypeRepository.FindOneAsync(x => x.Id == id);
            if (specialAptType == null)
            {
                throw new InfinityNotFoundException("SpecialAptType not found");
            }
            SpecialAptTypeModel model = ObjectConverter<SpecialAptType, SpecialAptTypeModel>.Convert(specialAptType);
            return model;
        }

        public async Task<SpecialAptTypeModel> SaveSpecialAptType(int id, SpecialAptTypeModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("SpecialAptType data missing");
            }
            bool isExist = specialAptTypeRepository.Exists(x => x.SpAptType == model.SpAptType && x.Id != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            SpecialAptType specialAptType = ObjectConverter<SpecialAptTypeModel, SpecialAptType>.Convert(model);
            if (id > 0)
            {
                specialAptType = await specialAptTypeRepository.FindOneAsync(x => x.Id == id);
                if (specialAptType == null)
                {
                    throw new InfinityNotFoundException("SpecialAptType not found !");
                }
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "SpecialAptType";
                bnLog.TableEntryForm = "Special Appointment";
                bnLog.PreviousValue = "Id: " + model.Id;
                bnLog.UpdatedValue = "Id: " + model.Id;
                if (specialAptType.SpAptType != model.SpAptType)
                {
                    bnLog.PreviousValue += ", SpecialAppointment: " + specialAptType.SpAptType;
                    bnLog.UpdatedValue += ", SpecialAppointment: " + model.SpAptType;
                }
                if (specialAptType.Position != model.Position)
                {
                    bnLog.PreviousValue += ", Position: " + specialAptType.Position;
                    bnLog.UpdatedValue += ", Position: " + model.Position;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (specialAptType.SpAptType != model.SpAptType || specialAptType.Position != model.Position)
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
                specialAptType.IsActive = true;
               
            }
            specialAptType.SpAptType = model.SpAptType;
            specialAptType.Position = model.Position;

            await specialAptTypeRepository.SaveAsync(specialAptType);
            model.Id = specialAptType.Id;
            return model;
        }

        public async Task<bool> DeleteSpecialAptType(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            SpecialAptType specialAptType = await specialAptTypeRepository.FindOneAsync(x => x.Id == id);
            if (specialAptType == null)
            {
                throw new InfinityNotFoundException("SpecialAptType not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "SpecialAptType";
                bnLog.TableEntryForm = "Special Appointment";
                bnLog.PreviousValue = "Id: " + specialAptType.Id + ", SpecialAppointment: " + specialAptType.SpAptType + ", Position: " + specialAptType.Position;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await specialAptTypeRepository.DeleteAsync(specialAptType);
            }
        }



        public async Task<List<SelectModel>> GetSpecialAptTypeSelectModels()
        {
            ICollection<SpecialAptType> branches = await specialAptTypeRepository.FilterAsync(x=>x.IsActive);
            List<SelectModel> selectModels = branches.OrderBy(x => x.SpAptType).Select(x => new SelectModel
            {
                Text = x.SpAptType,
                Value = x.Id
            }).ToList();
            return selectModels;
        }

      
    }
}
