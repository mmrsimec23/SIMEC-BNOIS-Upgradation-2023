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
        public CareerForecastSettingService(IBnoisRepository<CareerForecastSetting> careerForecastSettingRepository, IBnoisRepository<EmployeeGeneral> employeeGeneralRepository, IBnoisRepository<CareerForecast> careerForecastRepository)
        {
            this.careerForecastSettingRepository = careerForecastSettingRepository;
            this.employeeGeneralRepository = employeeGeneralRepository;
            this.careerForecastRepository = careerForecastRepository;
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
