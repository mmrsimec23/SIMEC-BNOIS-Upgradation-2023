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
    public class HeirTypeService : IHeirTypeService
    {
        private readonly IBnoisRepository<HeirType> heirTypeRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public HeirTypeService(IBnoisRepository<HeirType> heirTypeRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.heirTypeRepository = heirTypeRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }

    
        public List<HeirTypeModel> GetHeirTypes(int ps, int pn, string qs, out int total)
        {
            IQueryable<HeirType> heirTypes = heirTypeRepository.FilterWithInclude(x => x.IsActive && (x.Name.Contains(qs) || String.IsNullOrEmpty(qs)));
            total = heirTypes.Count();
            heirTypes = heirTypes.OrderByDescending(x => x.HeirTypeId).Skip((pn - 1) * ps).Take(ps);
            List<HeirTypeModel> models = ObjectConverter<HeirType, HeirTypeModel>.ConvertList(heirTypes.ToList()).ToList();
            return models;
        }
        public async Task<HeirTypeModel> GetHeirType(int id)
        {
            if (id <= 0)
            {
                return new HeirTypeModel();
            }
            HeirType heirType = await heirTypeRepository.FindOneAsync(x => x.HeirTypeId == id);
            if (heirType == null)
            {
                throw new InfinityNotFoundException("Heir Type not found");
            }
            HeirTypeModel model = ObjectConverter<HeirType, HeirTypeModel>.Convert(heirType);
            return model;
        }

        public async Task<HeirTypeModel> SaveHeirType(int id, HeirTypeModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Heir Type data missing");
            }
            bool isExist = heirTypeRepository.Exists(x => x.Name == model.Name && x.HeirTypeId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            HeirType heirType = ObjectConverter<HeirTypeModel, HeirType>.Convert(model);
            if (id > 0)
            {
                heirType = await heirTypeRepository.FindOneAsync(x => x.HeirTypeId == id);
                if (heirType == null)
                {
                    throw new InfinityNotFoundException("HeirType not found !");
                }

                heirType.ModifiedDate = DateTime.Now;
                heirType.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "HeirType";
                bnLog.TableEntryForm = "Heir Type";
                bnLog.PreviousValue = "Id: " + model.HeirTypeId;
                bnLog.UpdatedValue = "Id: " + model.HeirTypeId;
                if (heirType.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + heirType.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (heirType.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + heirType.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (heirType.Name != model.Name || heirType.Remarks != model.Remarks)
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
                heirType.IsActive = true;
                heirType.CreatedDate = DateTime.Now;
                heirType.CreatedBy = userId;
            }
            heirType.Name = model.Name;
            heirType.Remarks = model.Remarks;
        
            await heirTypeRepository.SaveAsync(heirType);
            model.HeirTypeId = heirType.HeirTypeId;
            return model;
        }

        public async Task<bool> DeleteHeirType(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            HeirType HeirType = await heirTypeRepository.FindOneAsync(x => x.HeirTypeId == id);
            if (HeirType == null)
            {
                throw new InfinityNotFoundException("Heir Type not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "HeirType";
                bnLog.TableEntryForm = "Heir Type";
                bnLog.PreviousValue = "Id: " + HeirType.HeirTypeId + ", Name: " + HeirType.Name + ", Remarks: " + HeirType.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await heirTypeRepository.DeleteAsync(HeirType);
            }
        }

        public async Task<List<SelectModel>> GetHeirTypeSelectModels()
        {
            ICollection<HeirType> heirTypes = await heirTypeRepository.FilterAsync(x => x.IsActive);
            return heirTypes.OrderBy(x => x.Name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.HeirTypeId
            }).ToList();
        }
    }
}
