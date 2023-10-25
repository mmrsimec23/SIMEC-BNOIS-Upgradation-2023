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
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public MscPermissionTypeService(IBnoisRepository<MscPermissionType> mscPermissionTypeRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.mscPermissionTypeRepository = mscPermissionTypeRepository;
            this.bnoisLogRepository = bnoisLogRepository;
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
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "MscPermissionType";
                bnLog.TableEntryForm = "Msc Permission Type";
                bnLog.PreviousValue = "Id: " + model.MscPermissionTypeId;
                bnLog.UpdatedValue = "Id: " + model.MscPermissionTypeId;
                if (MscPermissionType.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + MscPermissionType.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (MscPermissionType.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + MscPermissionType.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = model.CreatedBy;
                bnLog.LogCreatedDate = DateTime.Now;

                if (MscPermissionType.Name != model.Name || MscPermissionType.Remarks != model.Remarks)
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
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "MscPermissionType";
                bnLog.TableEntryForm = "Msc Permission Type";
                bnLog.PreviousValue = "Id: " + MscPermissionType.MscPermissionTypeId + ", Name: " + MscPermissionType.Name + ", Remarks: " + MscPermissionType.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
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
