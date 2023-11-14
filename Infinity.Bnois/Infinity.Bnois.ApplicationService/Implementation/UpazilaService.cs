using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Configuration.Models;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class UpazilaService: IUpazilaService
    {

        private readonly IBnoisRepository<Upazila> upazilaRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public UpazilaService(IBnoisRepository<Upazila> upazilaRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.upazilaRepository = upazilaRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

        public List<UpazilaModel> GetUpazilas(int ps, int pn, string qs, out int total)
        {
            IQueryable<Upazila> upazilas = upazilaRepository.FilterWithInclude(x => x.IsActive
                && ((x.Name.Contains(qs) || String.IsNullOrEmpty(qs)) ||
                (x.District.Name.Contains(qs) || String.IsNullOrEmpty(qs))), "District","Division");
            total = upazilas.Count();
            upazilas = upazilas.OrderByDescending(x => x.DistrictId).Skip((pn - 1) * ps).Take(ps);
            List<UpazilaModel> models = ObjectConverter<Upazila, UpazilaModel>.ConvertList(upazilas.ToList()).ToList();
            return models;
        }

        public async Task<UpazilaModel> GetUpazila(int id)
        {
            if (id <= 0)
            {
                return new UpazilaModel();
            }
            Upazila upazila = await upazilaRepository.FindOneAsync(x => x.UpazilaId == id);
            if (upazila == null)
            {
                throw new InfinityNotFoundException("Upazila not found");
            }
            UpazilaModel model = ObjectConverter<Upazila, UpazilaModel>.Convert(upazila);
            return model;
        }


        public async Task<UpazilaModel> SaveUpazila(int id, UpazilaModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Upazila data missing");
            }

            bool isExistData = upazilaRepository.Exists(x => x.DistrictId == model.DistrictId && x.Name == model.Name && x.UpazilaId != id);
            if (isExistData)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            Upazila upazila = ObjectConverter<UpazilaModel, Upazila>.Convert(model);
            if (id > 0)
            {
                upazila = await upazilaRepository.FindOneAsync(x => x.UpazilaId == id, new List<string>() { "District", "Division" });
                if (upazila == null)
                {
                    throw new InfinityNotFoundException("Upazila not found !");
                }
                upazila.ModifiedDate = DateTime.Now;
                upazila.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Upazila";
                bnLog.TableEntryForm = "Upazila";
                bnLog.PreviousValue = "Id: " + model.UpazilaId;
                bnLog.UpdatedValue = "Id: " + model.UpazilaId;
                int bnoisUpdateCount = 0;
                if (upazila.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + upazila.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                    bnoisUpdateCount += 1;
                }
                if (upazila.DistrictId != model.DistrictId)
                {
                    var dis = employeeService.GetDynamicTableInfoById("District", "DistrictId", model.DistrictId);
                    bnLog.PreviousValue += ", District: " + upazila.District.Name;
                    bnLog.UpdatedValue += ", District: " + ((dynamic)dis).Name;
                    bnoisUpdateCount += 1;
                }
                if (upazila.DivisionId != model.DivisionId)
                {
                    var div = employeeService.GetDynamicTableInfoById("Division", "DivisionId", model.DivisionId??0);
                    bnLog.PreviousValue += ", Division: " + upazila.Division.Name;
                    bnLog.UpdatedValue += ", Division: " + ((dynamic)div).Name;
                    bnoisUpdateCount += 1;
                }
                if (upazila.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + upazila.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                    bnoisUpdateCount += 1;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (bnoisUpdateCount > 0)
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
                upazila.IsActive = true;
                upazila.CreatedDate = DateTime.Now;
                upazila.CreatedBy = userId;
            }
            upazila.Name = model.Name;
            upazila.DistrictId = model.DistrictId;
            upazila.DivisionId = model.DivisionId;
            upazila.Remarks = model.Remarks;
            await upazilaRepository.SaveAsync(upazila);
            model.UpazilaId = upazila.UpazilaId;
            return model;
        }


        public async Task<bool> DeleteUpazila(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            Upazila upazila = await upazilaRepository.FindOneAsync(x => x.UpazilaId == id);
            if (upazila == null)
            {
                throw new InfinityNotFoundException("Upazila not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "Upazila";
                bnLog.TableEntryForm = "Upazila";
                bnLog.PreviousValue = "Id: " + upazila.UpazilaId + ", Name: " + upazila.Name + ", District: " + upazila.DistrictId + ", Division: " + upazila.DivisionId + ", Remarks: " + upazila.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await upazilaRepository.DeleteAsync(upazila);
            }
        }


        public async Task<List<SelectModel>> GetUpazilaSelectModels()
        {
            ICollection<Upazila> districts = await upazilaRepository.FilterAsync(x => x.IsActive);
            List<SelectModel> selectModels = districts.OrderBy(x => x.Name).Select(x => new SelectModel
            {
                Text = x.Name,
                Value = x.DistrictId
            }).ToList();
            return selectModels;

        }

        public async Task<List<SelectModel>> GetUpazilaByDistrictSelectModels(int districtId)
        {
            ICollection<Upazila> models = await upazilaRepository.FilterAsync(x => x.IsActive && x.DistrictId == districtId);
            return models.OrderBy(x => x.Name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.UpazilaId
            }).ToList();
        }

        public async Task<List<SelectModel>> GetUpazilasSelectModelByDistrict(int districtId)
        {
            IQueryable<Upazila> query = upazilaRepository.FilterWithInclude(x => x.IsActive && x.DistrictId == districtId);
            List<Upazila> upazilas = await query.OrderBy(x => x.Name).ToListAsync();
            List<SelectModel> districtSelectModels = upazilas.Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.UpazilaId
            }).ToList();
            return districtSelectModels;
        }
    }
}