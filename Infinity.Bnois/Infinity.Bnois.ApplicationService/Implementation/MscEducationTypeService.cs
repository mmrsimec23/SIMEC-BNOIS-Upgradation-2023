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
    public class MscEducationTypeService : IMscEducationTypeService
    {
        private readonly IBnoisRepository<MscEducationType> mscEducationTypeRepository;
        public MscEducationTypeService(IBnoisRepository<MscEducationType> mscEducationTypeRepository)
        {
            this.mscEducationTypeRepository = mscEducationTypeRepository;
        }
        
        public async Task<MscEducationTypeModel> GetMscEducationType(int id)
        {
            if (id <= 0)
            {
                return new MscEducationTypeModel();
            }
            MscEducationType MscEducationType = await mscEducationTypeRepository.FindOneAsync(x => x.MscEducationTypeId == id);
            if (MscEducationType == null)
            {
                throw new InfinityNotFoundException("Msc Education Type not found");
            }
            MscEducationTypeModel model = ObjectConverter<MscEducationType, MscEducationTypeModel>.Convert(MscEducationType);
            return model;
        }

        public List<MscEducationTypeModel> GetMscEducationTypes(int ps, int pn, string qs, out int total)
        {
            IQueryable<MscEducationType> MscEducationTypes = mscEducationTypeRepository.FilterWithInclude(x => x.IsActive && (x.Name.Contains(qs) || String.IsNullOrEmpty(qs)));
            total = MscEducationTypes.Count();
            MscEducationTypes = MscEducationTypes.OrderByDescending(x => x.MscEducationTypeId).Skip((pn - 1) * ps).Take(ps);
            List<MscEducationTypeModel> models = ObjectConverter<MscEducationType, MscEducationTypeModel>.ConvertList(MscEducationTypes.ToList()).ToList();
            return models;
        }

        public async Task<MscEducationTypeModel> SaveMscEducationType(int id, MscEducationTypeModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Msc Education Type data missing");
            }
            bool isExist = mscEducationTypeRepository.Exists(x => x.Name == model.Name && x.MscEducationTypeId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            MscEducationType MscEducationType = ObjectConverter<MscEducationTypeModel, MscEducationType>.Convert(model);
            if (id > 0)
            {
                MscEducationType = await mscEducationTypeRepository.FindOneAsync(x => x.MscEducationTypeId == id);
                if (MscEducationType == null)
                {
                    throw new InfinityNotFoundException("Msc Permission Type not found !");
                }

                MscEducationType.ModifiedDate = DateTime.Now;
               MscEducationType.ModifiedBy = userId;
            }
            else
            {
                MscEducationType.IsActive = true;
                MscEducationType.CreatedDate = DateTime.Now;
                MscEducationType.CreatedBy = userId;
            }
            MscEducationType.Name = model.Name;
            MscEducationType.Remarks = model.Remarks;


            await mscEducationTypeRepository.SaveAsync(MscEducationType);
            model.MscEducationTypeId = MscEducationType.MscEducationTypeId;
            return model;
        }
        public async Task<bool> DeleteMscEducationType(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            MscEducationType MscEducationType = await mscEducationTypeRepository.FindOneAsync(x => x.MscEducationTypeId == id);
            if (MscEducationType == null)
            {
                throw new InfinityNotFoundException("Msc Permission Types not found");
            }
            else
            {
                return await mscEducationTypeRepository.DeleteAsync(MscEducationType);
            }
        }

        public async Task<List<SelectModel>> GetMscEducationTypesSelectModels()
        {
            ICollection<MscEducationType> MscEducationTypes = await mscEducationTypeRepository.FilterAsync(x => x.IsActive);
            List<SelectModel> selectModels = MscEducationTypes.OrderBy(x => x.Name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.MscEducationTypeId
            }).ToList();
            return selectModels;
        }
    }
}
