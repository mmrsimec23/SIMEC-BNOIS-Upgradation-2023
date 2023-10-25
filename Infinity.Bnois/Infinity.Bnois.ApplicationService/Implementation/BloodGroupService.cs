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
    public class BloodGroupService : IBloodGroupService
    {
        private readonly IBnoisRepository<BloodGroup> bloodGroupRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public BloodGroupService(IBnoisRepository<BloodGroup> bloodGroupRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.bloodGroupRepository = bloodGroupRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }

    
        public List<BloodGroupModel> GetBloodGroups(int ps, int pn, string qs, out int total)
        {
            IQueryable<BloodGroup> bloodGroups = bloodGroupRepository.FilterWithInclude(x => x.IsActive && (x.Name.Contains(qs) || String.IsNullOrEmpty(qs)));
            total = bloodGroups.Count();
            bloodGroups = bloodGroups.OrderByDescending(x => x.BloodGroupId).Skip((pn - 1) * ps).Take(ps);
            List<BloodGroupModel> models = ObjectConverter<BloodGroup, BloodGroupModel>.ConvertList(bloodGroups.ToList()).ToList();
            return models;
        }
        public async Task<BloodGroupModel> GetBloodGroup(int id)
        {
            if (id <= 0)
            {
                return new BloodGroupModel();
            }
            BloodGroup bloodGroup = await bloodGroupRepository.FindOneAsync(x => x.BloodGroupId == id);
            if (bloodGroup == null)
            {
                throw new InfinityNotFoundException("Blood Group not found");
            }
            BloodGroupModel model = ObjectConverter<BloodGroup, BloodGroupModel>.Convert(bloodGroup);
            return model;
        }

        public async Task<BloodGroupModel> SaveBloodGroup(int id, BloodGroupModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Blood Group data missing");
            }
            bool isExist = bloodGroupRepository.Exists(x => x.Name == model.Name && x.BloodGroupId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            BloodGroup bloodGroup = ObjectConverter<BloodGroupModel, BloodGroup>.Convert(model);
            if (id > 0)
            {
                bloodGroup = await bloodGroupRepository.FindOneAsync(x => x.BloodGroupId == id);
                if (bloodGroup == null)
                {
                    throw new InfinityNotFoundException("Blood Group not found !");
                }

                bloodGroup.ModifiedDate = DateTime.Now;
                bloodGroup.ModifiedBy = userId;

                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "BloodGroup";
                bnLog.TableEntryForm = "Blood Group";
                bnLog.PreviousValue = "Id: " + model.BloodGroupId;
                bnLog.UpdatedValue = "Id: " + model.BloodGroupId;
                if(bloodGroup.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: "+ bloodGroup.Name;
                    bnLog.UpdatedValue += ", Name: "+ model.Name;
                }
                if(bloodGroup.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + bloodGroup.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if(bloodGroup.Name != model.Name || bloodGroup.Remarks != model.Remarks)
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
                bloodGroup.IsActive = true;
                bloodGroup.CreatedDate = DateTime.Now;
                bloodGroup.CreatedBy = userId;
            }
            bloodGroup.Name = model.Name;
            bloodGroup.Remarks = model.Remarks;
        
            await bloodGroupRepository.SaveAsync(bloodGroup);
            model.BloodGroupId = bloodGroup.BloodGroupId;
            return model;
        }

        public async Task<bool> DeleteBloodGroup(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            BloodGroup bloodGroup = await bloodGroupRepository.FindOneAsync(x => x.BloodGroupId == id);
            if (bloodGroup == null)
            {
                throw new InfinityNotFoundException("Blood Group not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "BloodGroup";
                bnLog.TableEntryForm = "Blood Group";
                bnLog.PreviousValue = "Id: " + bloodGroup.BloodGroupId + ", Name: " + bloodGroup.Name + ", Remarks: " + bloodGroup.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";
                
                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);
               
                //data log section end

                return await bloodGroupRepository.DeleteAsync(bloodGroup);
            }
        }

        public async Task<List<SelectModel>> GetBloodGroupSelectModels()
        {
            ICollection<BloodGroup> bloodGroups = await bloodGroupRepository.FilterAsync(x => x.IsActive);
            return bloodGroups.OrderBy(x => x.Name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.BloodGroupId
            }).ToList();
        }
    }
}
