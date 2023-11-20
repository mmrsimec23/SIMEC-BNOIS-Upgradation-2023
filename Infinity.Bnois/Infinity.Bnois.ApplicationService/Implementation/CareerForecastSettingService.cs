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
    public class CareerForecastSettingService : ICareerForecastSettingService
    {
        private readonly IBnoisRepository<CareerForecastSetting> careerForecastSettingRepository;
        private readonly IBnoisRepository<EmployeeGeneral> employeeGeneralRepository;
        private readonly IBnoisRepository<CareerForecast> careerForecastRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public CareerForecastSettingService(IBnoisRepository<CareerForecastSetting> careerForecastSettingRepository, IEmployeeService employeeService, IBnoisRepository<EmployeeGeneral> employeeGeneralRepository, IBnoisRepository<CareerForecast> careerForecastRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.careerForecastSettingRepository = careerForecastSettingRepository;
            this.employeeGeneralRepository = employeeGeneralRepository;
            this.careerForecastRepository = careerForecastRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }
        

        public List<CareerForecastSettingModel> GetCareerForecastSettings(int ps, int pn, string qs, out int total)
        {
            IQueryable<CareerForecastSetting> careerForecastSettings = careerForecastSettingRepository.FilterWithInclude(x => x.IsActive && (x.Name.Contains(qs) || String.IsNullOrEmpty(qs)) || (x.Shortname.Contains(qs) || String.IsNullOrEmpty(qs)), "Branch");
            total = careerForecastSettings.Count();
            careerForecastSettings = careerForecastSettings.OrderByDescending(x => x.CareerForecastSettingId).Skip((pn - 1) * ps).Take(ps);
            List<CareerForecastSettingModel> models = ObjectConverter<CareerForecastSetting, CareerForecastSettingModel>.ConvertList(careerForecastSettings.ToList()).ToList();
            return models;
        }

        public List<EmployeeCareerForecastSettingListModel> GetCareerForecastSettingByBranch(int id)
        {
            List<EmployeeCareerForecastSettingListModel> forecasts = careerForecastSettingRepository.FilterWithInclude(x=> x.BranchId==id).Select(v => 
            new EmployeeCareerForecastSettingListModel
            { 
                CareerForecastSettingId = v.CareerForecastSettingId, 
                Name = v.Name,
                Shortname = v.Shortname,
                BranchId = v.BranchId,
                PositionNo = v.PositionNo
            }).OrderBy(x=> x.PositionNo).ToList();

            return forecasts;
        }

        public async Task<CareerForecastSettingModel> GetCareerForecastSetting(int id)
        {
            if (id <= 0)
            {
                return new CareerForecastSettingModel();
            }
            CareerForecastSetting careerForecastSettings = await careerForecastSettingRepository.FindOneAsync(x => x.CareerForecastSettingId == id);
            if (careerForecastSettings == null)
            {
                throw new InfinityNotFoundException("Batch not found");
            }
            CareerForecastSettingModel model = ObjectConverter<CareerForecastSetting, CareerForecastSettingModel>.Convert(careerForecastSettings);
            return model;
        }

        public async Task<CareerForecastSettingModel> SaveCareerForecastSetting(int id, CareerForecastSettingModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Career Forecast Setting data missing");
            }
            bool isExist = careerForecastSettingRepository.Exists(x => x.Name == model.Name && x.BranchId == model.BranchId && x.CareerForecastSettingId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            CareerForecastSetting careerForecastSettings = ObjectConverter<CareerForecastSettingModel, CareerForecastSetting>.Convert(model);
            if (id > 0)
            {
                careerForecastSettings = await careerForecastSettingRepository.FindOneAsync(x => x.CareerForecastSettingId == id);
                if (careerForecastSettings == null)
                {
                    throw new InfinityNotFoundException("Career Forecast Setting not found !");
                }

                careerForecastSettings.ModifiedDate = DateTime.Now;
                careerForecastSettings.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "CareerForecastSetting";
                bnLog.TableEntryForm = "Career Forecast Setting";
                bnLog.PreviousValue = "Id: " + model.CareerForecastSettingId;
                bnLog.UpdatedValue = "Id: " + model.CareerForecastSettingId;
                if (careerForecastSettings.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + careerForecastSettings.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (careerForecastSettings.Shortname != model.Shortname)
                {
                    bnLog.PreviousValue += ", Shortname: " + careerForecastSettings.Shortname;
                    bnLog.UpdatedValue += ", Shortname: " + model.Shortname;
                }
                if (careerForecastSettings.BranchId != model.BranchId)
                {
                    if (careerForecastSettings.BranchId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Branch", "BranchId", careerForecastSettings.BranchId);
                        bnLog.PreviousValue += ", Branch: " + ((dynamic)prev).Name;
                    }
                    if (model.BranchId > 0)
                    {
                        var newv = employeeService.GetDynamicTableInfoById("Branch", "BranchId", model.BranchId);
                        bnLog.UpdatedValue += ", Branch: " + ((dynamic)newv).Name;
                    }
                }
                if (careerForecastSettings.PositionNo != model.PositionNo)
                {
                    bnLog.PreviousValue += ", PositionNo: " + careerForecastSettings.PositionNo;
                    bnLog.UpdatedValue += ", PositionNo: " + model.PositionNo;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (careerForecastSettings.Name != model.Name || careerForecastSettings.Shortname != model.Shortname || careerForecastSettings.BranchId != model.BranchId || careerForecastSettings.PositionNo != model.PositionNo)
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
                careerForecastSettings.IsActive = true;
                careerForecastSettings.CreatedDate = DateTime.Now;
                careerForecastSettings.CreatedBy = userId;
            }
            careerForecastSettings.Name = model.Name;
            careerForecastSettings.Shortname = model.Shortname;
            careerForecastSettings.BranchId = model.BranchId;
            careerForecastSettings.PositionNo = model.PositionNo;
            await careerForecastSettingRepository.SaveAsync(careerForecastSettings);
            model.CareerForecastSettingId = careerForecastSettings.CareerForecastSettingId;
            return model;
        }

        public async Task<bool> DeleteCareerForecastSetting(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            CareerForecastSetting careerForecastSettings = await careerForecastSettingRepository.FindOneAsync(x => x.CareerForecastSettingId == id);
            if (careerForecastSettings == null)
            {
                throw new InfinityNotFoundException("Career Forecast Setting not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "careerForecastSettings";
                bnLog.TableEntryForm = "career Forecast Settings";
                bnLog.PreviousValue = "Id: " + careerForecastSettings.CareerForecastSettingId + ", Name: " + careerForecastSettings.Name + ", Shortname: " + careerForecastSettings.Shortname;
                
                if (careerForecastSettings.BranchId > 0)
                {
                    var prev = employeeService.GetDynamicTableInfoById("Branch", "BranchId", careerForecastSettings.BranchId);
                    bnLog.PreviousValue += ", Branch: " + ((dynamic)prev).Name;
                }
                bnLog.PreviousValue += ", PositionNo: " + careerForecastSettings.PositionNo;

                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await careerForecastSettingRepository.DeleteAsync(careerForecastSettings);
            }
        }

        public async Task<List<SelectModel>> GetCareerForecastSettingSelectModels()
        {
            ICollection<CareerForecastSetting> models = await careerForecastSettingRepository.FilterAsync(x => x.IsActive);
            return models.OrderBy(x => x.Name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.CareerForecastSettingId
            }).ToList();

        }

        public async Task<List<SelectModel>> GetCareerForecastByBranchSettingSelectModels(int employeeId)
        {

            var branch = await employeeGeneralRepository.FindOneAsync(x=> x.EmployeeId == employeeId);
            List<CareerForecast> careerForecasts = careerForecastRepository.FilterWithInclude(x=> x.EmployeeId == employeeId && x.BranchId == branch.BranchId).ToList();
            List<CareerForecastSetting> models = careerForecastSettingRepository.FilterWithInclude(x => x.BranchId == branch.BranchId).ToList();
            
            List<CareerForecastSetting> final = models.Where(p => careerForecasts.All(p2 => p2.CareerForecastSettingId != p.CareerForecastSettingId)).ToList();
            
            return final.OrderBy(x => x.PositionNo).Select(x => new SelectModel()
            {
                Text = x.PositionNo +" - "+ x.Name,
                Value = x.CareerForecastSettingId
            }).ToList();

        }
    }
}
