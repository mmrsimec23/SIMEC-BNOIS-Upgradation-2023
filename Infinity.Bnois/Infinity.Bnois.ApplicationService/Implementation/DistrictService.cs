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
   public class DistrictService: IDistrictService
    {

        private readonly IBnoisRepository<District> districtRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public DistrictService(IBnoisRepository<District> districtRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.districtRepository = districtRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }

        public List<DistrictModel> GetDistricts(int ps, int pn, string qs, out int total)
        {
            IQueryable<District> districts = districtRepository.FilterWithInclude(x => x.IsActive
                && ((x.Name.Contains(qs) || String.IsNullOrEmpty(qs)) ||
                (x.Division.Name.Contains(qs) || String.IsNullOrEmpty(qs))), "Division");
            total = districts.Count();
            districts = districts.OrderByDescending(x => x.DistrictId).Skip((pn - 1) * ps).Take(ps);
            List<DistrictModel> models = ObjectConverter<District, DistrictModel>.ConvertList(districts.ToList()).ToList();
            return models;
        }

        public async Task<DistrictModel> GetDistrict(int id)
        {
            if (id <= 0)
            {
                return new DistrictModel();
            }
            District district = await districtRepository.FindOneAsync(x => x.DistrictId == id);
            if (district == null)
            {
                throw new InfinityNotFoundException("District not found");
            }
            DistrictModel model = ObjectConverter<District, DistrictModel>.Convert(district);
            return model;
        }

    
        public async Task<DistrictModel> SaveDistrict(int id, DistrictModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("District data missing");
            }

            bool isExistData = districtRepository.Exists(x => x.DivisionId == model.DivisionId && x.Name == model.Name && x.DistrictId != id);
            if (isExistData)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            District district = ObjectConverter<DistrictModel, District>.Convert(model);
            if (id > 0)
            {
                district = await districtRepository.FindOneAsync(x => x.DistrictId == id);
                if (district == null)
                {
                    throw new InfinityNotFoundException("District not found !");
                }

                district.ModifiedDate = DateTime.Now;
                district.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "District";
                bnLog.TableEntryForm = "District";
                bnLog.PreviousValue = "Id: " + model.DistrictId;
                bnLog.UpdatedValue = "Id: " + model.DistrictId;
                if (district.DivisionId != model.DivisionId)
                {
                    bnLog.PreviousValue += ", Division: " + district.DivisionId;
                    bnLog.UpdatedValue += ", Division: " + model.DivisionId;
                }
                if (district.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + district.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (district.Description != model.Description)
                {
                    bnLog.PreviousValue += ", Description: " + district.Description;
                    bnLog.UpdatedValue += ", Description: " + model.Description;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (district.DivisionId != model.DivisionId || district.Name != model.Name || district.Description != model.Description)
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
                district.IsActive = true;
                district.CreatedDate = DateTime.Now;
                district.CreatedBy = userId;
            }
            district.Name = model.Name;
            district.DivisionId = model.DivisionId;
          
            district.Description = model.Description;
            await districtRepository.SaveAsync(district);
            model.DistrictId = district.DistrictId;
            return model;
        }


        public async Task<bool> DeleteDistrict(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            District district = await districtRepository.FindOneAsync(x => x.DistrictId == id);
            if (district == null)
            {
                throw new InfinityNotFoundException("District not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "District";
                bnLog.TableEntryForm = "District";
                bnLog.PreviousValue = "Id: " + district.DistrictId + ", Division: " + district.DivisionId + ", Name: " + district.Name + ", Description: " + district.Description;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await districtRepository.DeleteAsync(district);
            }
        }


        public async Task<List<SelectModel>> GetDistrictSelectModels()
        {
            ICollection<District> divisions = await districtRepository.FilterAsync(x => x.IsActive);
            List<SelectModel> selectModels = divisions.OrderBy(x => x.Name).Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.DistrictId
            }).ToList();
            return selectModels;

        }

        public async Task<List<SelectModel>> GetDistrictByDivisionSelectModels(int divisionId)
        {
            ICollection<District> models = await districtRepository.FilterAsync(x => x.IsActive && x.DivisionId==divisionId);
            return models.OrderBy(x => x.Name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.DistrictId
            }).ToList();

        }

        public async Task<List<SelectModel>> GetDistrictsSelectModelByDivion(int divisionId)
        {
            IQueryable<District> query = districtRepository.FilterWithInclude(x => x.IsActive && x.DivisionId==divisionId);
            List<District> districts = await query.OrderBy(x => x.Name).ToListAsync();
            List<SelectModel> districtSelectModels = districts.Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.DistrictId
            }).ToList();
            return districtSelectModels;
        }
    }
}
