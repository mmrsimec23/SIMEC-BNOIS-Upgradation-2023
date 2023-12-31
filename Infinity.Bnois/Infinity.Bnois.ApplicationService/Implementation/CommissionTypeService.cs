﻿using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Models
{
    public class CommissionTypeService : ICommissionTypeService
    {
        private readonly IBnoisRepository<CommissionType> commissionTypeRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public CommissionTypeService(IBnoisRepository<CommissionType> commissionTypeRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.commissionTypeRepository = commissionTypeRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }

        public List<CommissionTypeModel> CommissionTypes(int ps, int pn, string qs, out int total)
        {
            IQueryable<CommissionType> commissionTypes = commissionTypeRepository.FilterWithInclude(x => x.IsActive && (x.TypeName.Contains(qs) || String.IsNullOrEmpty(qs)));
            total = commissionTypes.Count();
            commissionTypes = commissionTypes.OrderByDescending(x => x.CommissionTypeId).Skip((pn - 1) * ps).Take(ps);
            List<CommissionTypeModel> models = ObjectConverter<CommissionType, CommissionTypeModel>.ConvertList(commissionTypes.ToList()).ToList();
            return models;
        }
        

        public async Task<CommissionTypeModel> GetCommissionType(int id)
        {
            if (id <= 0)
            {
                return new CommissionTypeModel();
            }
            CommissionType commissionType = await commissionTypeRepository.FindOneAsync(x => x.CommissionTypeId == id);
            if (commissionType == null)
            {
                throw new InfinityNotFoundException("CommissionType not found");
            }
            CommissionTypeModel model = ObjectConverter<CommissionType, CommissionTypeModel>.Convert(commissionType);
            return model;
        }

        public async Task<CommissionTypeModel> SaveCommissionType(int id, CommissionTypeModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("CommissionType data missing");
            }
            bool isExist = commissionTypeRepository.Exists(x => x.TypeName == model.TypeName && x.CommissionTypeId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            CommissionType commissionType = ObjectConverter<CommissionTypeModel, CommissionType>.Convert(model);
            if (id > 0)
            {
                commissionType = await commissionTypeRepository.FindOneAsync(x => x.CommissionTypeId == id);
                if (commissionType == null)
                {
                    throw new InfinityNotFoundException("CommissionType not found !");
                }
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "CommissionType";
                bnLog.TableEntryForm = "Commission Type";
                bnLog.PreviousValue = "Id: " + model.CommissionTypeId;
                bnLog.UpdatedValue = "Id: " + model.CommissionTypeId;
                if (commissionType.TypeName != model.TypeName)
                {
                    bnLog.PreviousValue += ", CommissionType: " + commissionType.TypeName;
                    bnLog.UpdatedValue += ", CommissionType: " + model.TypeName;
                }
                if (commissionType.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + commissionType.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (commissionType.TypeName != model.TypeName || commissionType.Remarks != model.Remarks)
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
                commissionType.IsActive = true;
                commissionType.CreatedDate = DateTime.Now;
                commissionType.CreatedBy = userId;
            }
            commissionType.TypeName = model.TypeName;
            commissionType.ModifiedDate = DateTime.Now;
            commissionType.ModifiedBy = userId;
            commissionType.Remarks = model.Remarks;
            await commissionTypeRepository.SaveAsync(commissionType);
            model.CommissionTypeId = commissionType.CommissionTypeId;
            return model;
        }

        public async Task<bool> DeleteCommissionType(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            CommissionType commissionType = await commissionTypeRepository.FindOneAsync(x => x.CommissionTypeId == id);
            if (commissionType == null)
            {
                throw new InfinityNotFoundException("CommissionType not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "CommissionType";
                bnLog.TableEntryForm = "Commission Type";
                bnLog.PreviousValue = "Id: " + commissionType.CommissionTypeId + ", TypeName: " + commissionType.TypeName + ", Remarks: " + commissionType.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await commissionTypeRepository.DeleteAsync(commissionType);
            }
        }

        public async Task<List<SelectModel>> GetCommissionTypeSelectModels()
        {
            ICollection<CommissionType> commissionTypes = await commissionTypeRepository.FilterAsync(x => x.IsActive);
            List<SelectModel> selectModels = commissionTypes.OrderBy(x => x.TypeName).Select(x => new SelectModel
            {
                Text = x.TypeName,
                Value = x.CommissionTypeId
            }).ToList();
            return selectModels;
        }
    }
}
