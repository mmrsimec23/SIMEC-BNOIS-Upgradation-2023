using Infinity.Bnois;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class MaritalTypeService : IMaritalTypeService
    {
        private readonly IBnoisRepository<MaritalType> maritalTypeRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public MaritalTypeService(IBnoisRepository<MaritalType> maritalTypeRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.maritalTypeRepository = maritalTypeRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }
        public List<MaritalTypeModel> GetMaritalTypes(int pageSize, int pageNumber, string searchText, out int total)
        {
            IQueryable<MaritalType> maritalTypes = maritalTypeRepository.FilterWithInclude(x => x.IsActive
               && ((x.Name.Contains(searchText) || String.IsNullOrEmpty(searchText))));
            total = maritalTypes.Count();
            maritalTypes = maritalTypes.OrderByDescending(x => x.MaritalTypeId).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            List<MaritalTypeModel> models = ObjectConverter<MaritalType, MaritalTypeModel>.ConvertList(maritalTypes.ToList()).ToList();
            return models;
        }

        public async Task<MaritalTypeModel> GetMaritalType(int id)
        {
            if (id <= 0)
            {
                return new MaritalTypeModel();
            }
            MaritalType maritalType = await maritalTypeRepository.FindOneAsync(x => x.MaritalTypeId == id);
            if (maritalType == null)
            {
                throw new InfinityNotFoundException("Marital Type not found");
            }
            MaritalTypeModel model = ObjectConverter<MaritalType, MaritalTypeModel>.Convert(maritalType);
            return model;
        }


        public async Task<MaritalTypeModel> SaveMaritalType(int id, MaritalTypeModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Marital Type data missing");
            }
            MaritalType maritalType = ObjectConverter<MaritalTypeModel, MaritalType>.Convert(model);
            if (id > 0)
            {
                maritalType = await maritalTypeRepository.FindOneAsync(x => x.MaritalTypeId == id);
                if (maritalType == null)
                {
                    throw new InfinityNotFoundException("Marital Type not found !");
                }
                maritalType.ModifiedDate = DateTime.Now;
                maritalType.ModifiedBy = model.ModifiedBy;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "MaritalType";
                bnLog.TableEntryForm = "Marital Type";
                bnLog.PreviousValue = "Id: " + model.MaritalTypeId;
                bnLog.UpdatedValue = "Id: " + model.MaritalTypeId;
                if (maritalType.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + maritalType.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (maritalType.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + maritalType.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = model.CreatedBy;
                bnLog.LogCreatedDate = DateTime.Now;

                if (maritalType.Name != model.Name || maritalType.Remarks != model.Remarks)
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
                maritalType.CreatedDate = DateTime.Now;
                maritalType.CreatedBy = model.CreatedBy;
                maritalType.IsActive = true;
            }
            maritalType.Name = model.Name;
            maritalType.Remarks = model.Remarks;
            await maritalTypeRepository.SaveAsync(maritalType);
            model.MaritalTypeId = maritalType.MaritalTypeId;
            return model;
        }
        public async Task<bool> DeleteMaritalType(int id)
        {

            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            MaritalType maritalType = await maritalTypeRepository.FindOneAsync(x => x.MaritalTypeId == id);
            if (maritalType == null)
            {
                throw new InfinityNotFoundException("Marital Type not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "MaritalType";
                bnLog.TableEntryForm = "Marital Type";
                bnLog.PreviousValue = "Id: " + maritalType.MaritalTypeId + ", Name: " + maritalType.Name + ", Remarks: " + maritalType.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await maritalTypeRepository.DeleteAsync(maritalType);
            }
        }

        public async Task<List<SelectModel>> GetMaritalTypeSelectModels()
        {
            ICollection<MaritalType> maritalTypes = await maritalTypeRepository.FilterAsync(x => x.IsActive);
            List<SelectModel> selectModels = maritalTypes.Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.MaritalTypeId
            }).ToList();
            return selectModels;

        }
    }
}
