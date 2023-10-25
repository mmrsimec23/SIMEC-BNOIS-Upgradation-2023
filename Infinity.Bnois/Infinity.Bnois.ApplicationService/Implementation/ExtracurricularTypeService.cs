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
   public class ExtracurricularTypeService: IExtracurricularTypeService
    {
        private readonly IBnoisRepository<ExtracurricularType> extracurricularTypeService;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public ExtracurricularTypeService(IBnoisRepository<ExtracurricularType> extracurricularTypeService, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.extracurricularTypeService = extracurricularTypeService;
            this.bnoisLogRepository = bnoisLogRepository;
        }

      
        public async Task<ExtracurricularTypeModel> GetExtracurricularType(int id)
        {
            if (id <= 0)
            {
                return new ExtracurricularTypeModel();
            }
            ExtracurricularType extracurricularType = await extracurricularTypeService.FindOneAsync(x => x.ExtracurricularTypeId == id);
            if (extracurricularType == null)
            {
                throw new InfinityNotFoundException("Extracurricular Type not found");
            }
            ExtracurricularTypeModel model = ObjectConverter<ExtracurricularType, ExtracurricularTypeModel>.Convert(extracurricularType);
            return model;
        }

        public List<ExtracurricularTypeModel> GetExtracurricularTypes(int ps, int pn, string qs, out int total)
        {
            IQueryable<ExtracurricularType> extracurricularTypes = extracurricularTypeService.FilterWithInclude(x => x.IsActive && (x.Name.Contains(qs) || String.IsNullOrEmpty(qs)));
            total = extracurricularTypes.Count();
            extracurricularTypes = extracurricularTypes.OrderByDescending(x => x.ExtracurricularTypeId).Skip((pn - 1) * ps).Take(ps);
            List<ExtracurricularTypeModel> models = ObjectConverter<ExtracurricularType, ExtracurricularTypeModel>.ConvertList(extracurricularTypes.ToList()).ToList();
            return models;
        }

        public async Task<ExtracurricularTypeModel> SaveExtracurricularType(int id, ExtracurricularTypeModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Extracurricular Type data missing");
            }

            bool isExistData = extracurricularTypeService.Exists(x => x.Name == model.Name  && x.ExtracurricularTypeId != model.ExtracurricularTypeId);
            if (isExistData)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }
            
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            ExtracurricularType extracurricularType = ObjectConverter<ExtracurricularTypeModel, ExtracurricularType>.Convert(model);
            if (id > 0)
            {
                extracurricularType = await extracurricularTypeService.FindOneAsync(x => x.ExtracurricularTypeId == id);
                if (extracurricularType == null)
                {
                    throw new InfinityNotFoundException("Extracurricular Type not found !");
                }
                extracurricularType.ModifiedDate = DateTime.Now;
                extracurricularType.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ExtracurricularType";
                bnLog.TableEntryForm = "Extracurricular Type";
                bnLog.PreviousValue = "Id: " + model.ExtracurricularTypeId;
                bnLog.UpdatedValue = "Id: " + model.ExtracurricularTypeId;
                if (extracurricularType.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + extracurricularType.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (extracurricularType.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + extracurricularType.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (extracurricularType.Name != model.Name || extracurricularType.Remarks != model.Remarks)
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
                extracurricularType.IsActive = true;
                extracurricularType.CreatedDate = DateTime.Now;
                extracurricularType.CreatedBy = userId;
            }
            extracurricularType.Name = model.Name;
            extracurricularType.Remarks = model.Remarks;
            await extracurricularTypeService.SaveAsync(extracurricularType);
            model.ExtracurricularTypeId = extracurricularType.ExtracurricularTypeId;
            return model;
        }

        public async Task<bool> DeleteExtracurricularType(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            ExtracurricularType extracurricularType = await extracurricularTypeService.FindOneAsync(x => x.ExtracurricularTypeId == id);
            if (extracurricularType == null)
            {
                throw new InfinityNotFoundException("Extracurricular Type not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "ExtracurricularType";
                bnLog.TableEntryForm = "Extracurricular Type";
                bnLog.PreviousValue = "Id: " + extracurricularType.ExtracurricularTypeId + ", Name: " + extracurricularType.Name + ", Remarks: " + extracurricularType.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await extracurricularTypeService.DeleteAsync(extracurricularType);
            }
        }

        public async Task<List<SelectModel>> GetExtracurricularTypeSelectModels()
        {
            ICollection<ExtracurricularType> extracurricularTypes = await extracurricularTypeService.FilterAsync(x => x.IsActive);
            List<SelectModel> selectModels = extracurricularTypes.OrderBy(x => x.Name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.ExtracurricularTypeId
            }).ToList();
            return selectModels;
        }
    }
}
