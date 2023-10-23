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
    public class MscPermissionTypeService : IMscPermissionTypeService
    {
        private readonly IBnoisRepository<MscPermissionType> mscPermissionTypeRepository;
        public MscPermissionTypeService(IBnoisRepository<MscPermissionType> mscPermissionTypeRepository)
        {
            this.mscPermissionTypeRepository = mscPermissionTypeRepository;
        }
        
        public async Task<MscPermissionTypeModel> GetMscPermissionType(int id)
        {
            if (id <= 0)
            {
                return new MscPermissionTypeModel();
            }
            MscPermissionType mscPermissionType = await mscPermissionTypeRepository.FindOneAsync(x => x.MscPermissionTypeId == id);
            if (mscPermissionType == null)
            {
                throw new InfinityNotFoundException("Msc Permission Type not found");
            }
            MscPermissionTypeModel model = ObjectConverter<MscPermissionType, MscPermissionTypeModel>.Convert(mscPermissionType);
            return model;
        }

        public List<MscPermissionTypeModel> GetMscPermissionTypes(int ps, int pn, string qs, out int total)
        {
            IQueryable<MscPermissionType> mscPermissionTypes = mscPermissionTypeRepository.FilterWithInclude(x => x.IsActive && (x.Name.Contains(qs) || String.IsNullOrEmpty(qs)));
            total = mscPermissionTypes.Count();
            mscPermissionTypes = mscPermissionTypes.OrderByDescending(x => x.MscPermissionTypeId).Skip((pn - 1) * ps).Take(ps);
            List<MscPermissionTypeModel> models = ObjectConverter<MscPermissionType, MscPermissionTypeModel>.ConvertList(mscPermissionTypes.ToList()).ToList();
            return models;
        }

        public async Task<MscPermissionTypeModel> SaveMscPermissionType(int id, MscPermissionTypeModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Msc Permission Type data missing");
            }
            bool isExist = mscPermissionTypeRepository.Exists(x => x.Name == model.Name && x.MscPermissionTypeId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            MscPermissionType MscPermissionType = ObjectConverter<MscPermissionTypeModel, MscPermissionType>.Convert(model);
            if (id > 0)
            {
                MscPermissionType = await mscPermissionTypeRepository.FindOneAsync(x => x.MscPermissionTypeId == id);
                if (MscPermissionType == null)
                {
                    throw new InfinityNotFoundException("Msc Permission Type not found !");
                }

                MscPermissionType.ModifiedDate = DateTime.Now;
               MscPermissionType.ModifiedBy = userId;
            }
            else
            {
                MscPermissionType.IsActive = true;
                MscPermissionType.CreatedDate = DateTime.Now;
                MscPermissionType.CreatedBy = userId;
            }
            MscPermissionType.Name = model.Name;
            MscPermissionType.Remarks = model.Remarks;


            await mscPermissionTypeRepository.SaveAsync(MscPermissionType);
            model.MscPermissionTypeId = MscPermissionType.MscPermissionTypeId;
            return model;
        }
        public async Task<bool> DeleteMscPermissionType(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            MscPermissionType MscPermissionType = await mscPermissionTypeRepository.FindOneAsync(x => x.MscPermissionTypeId == id);
            if (MscPermissionType == null)
            {
                throw new InfinityNotFoundException("Msc Permission Types not found");
            }
            else
            {
                return await mscPermissionTypeRepository.DeleteAsync(MscPermissionType);
            }
        }

        public async Task<List<SelectModel>> GetMscPermissionTypesSelectModels()
        {
            ICollection<MscPermissionType> MscPermissionTypes = await mscPermissionTypeRepository.FilterAsync(x => x.IsActive);
            List<SelectModel> selectModels = MscPermissionTypes.OrderBy(x => x.Name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.MscPermissionTypeId
            }).ToList();
            return selectModels;
        }
    }
}
