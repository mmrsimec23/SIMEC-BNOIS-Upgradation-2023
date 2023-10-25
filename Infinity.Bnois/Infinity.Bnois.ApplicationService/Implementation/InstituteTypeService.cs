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
    public class InstituteTypeService : IInstituteTypeService
    {
        private readonly IBnoisRepository<InstituteType> instituteTypeRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public InstituteTypeService(IBnoisRepository<InstituteType> instituteTypeRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.instituteTypeRepository = instituteTypeRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }

        public List<InstituteTypeModel> InstituteTypes(int ps, int pn, string qs, out int total)
        {
            IQueryable<InstituteType> instituteTypes = instituteTypeRepository.FilterWithInclude(x => x.IsActive
               && (x.Name.Contains(qs) || String.IsNullOrEmpty(qs)));
            total = instituteTypes.Count();
            instituteTypes = instituteTypes.OrderByDescending(x => x.InstituteTypeId).Skip((pn - 1) * ps).Take(ps);
            List<InstituteTypeModel> models = ObjectConverter<InstituteType, InstituteTypeModel>.ConvertList(instituteTypes.ToList()).ToList();
            return models;
        }

        public async Task<InstituteTypeModel> GetInstituteType(int id)
        {
            if (id <= 0)
            {
                return new InstituteTypeModel();
            }
            InstituteType instituteType = await instituteTypeRepository.FindOneAsync(x => x.InstituteTypeId == id);
            if (instituteType == null)
            {
                throw new InfinityNotFoundException("InstituteType not found");
            }
            InstituteTypeModel model = ObjectConverter<InstituteType, InstituteTypeModel>.Convert(instituteType);
            return model;
        }

        public async Task<InstituteTypeModel> SaveInstituteType(int id, InstituteTypeModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("InstituteType data missing");
            }

            bool isExist = instituteTypeRepository.Exists(x => x.Name == model.Name && x.InstituteTypeId != model.InstituteTypeId);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }


            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            InstituteType instituteType = ObjectConverter<InstituteTypeModel, InstituteType>.Convert(model);
            if (id > 0)
            {
                instituteType = await instituteTypeRepository.FindOneAsync(x => x.InstituteTypeId == id);
                if (instituteType == null)
                {
                    throw new InfinityNotFoundException("InstituteType not found !");
                }
                instituteType.ModifiedDate = DateTime.Now;
                instituteType.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "InstituteType";
                bnLog.TableEntryForm = "Institute Type";
                bnLog.PreviousValue = "Id: " + model.InstituteTypeId;
                bnLog.UpdatedValue = "Id: " + model.InstituteTypeId;
                if (instituteType.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + instituteType.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (instituteType.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + instituteType.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (instituteType.Name != model.Name || instituteType.Remarks != model.Remarks)
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
                instituteType.IsActive = true;
                instituteType.CreatedDate = DateTime.Now;
                instituteType.CreatedBy = userId;
            }
            instituteType.Name = model.Name;
            instituteType.Remarks = model.Remarks;
            await instituteTypeRepository.SaveAsync(instituteType);
            model.InstituteTypeId = instituteType.InstituteTypeId;
            return model;
        }

        public async Task<bool> DeleteInstituteType(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            InstituteType instituteType = await instituteTypeRepository.FindOneAsync(x => x.InstituteTypeId == id);
            if (instituteType == null)
            {
                throw new InfinityNotFoundException("InstituteType not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "InstituteType";
                bnLog.TableEntryForm = "Institute Type";
                bnLog.PreviousValue = "Id: " + instituteType.InstituteTypeId + ", Name: " + instituteType.Name + ", Remarks: " + instituteType.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await instituteTypeRepository.DeleteAsync(instituteType);
            }
        }
    }
}
