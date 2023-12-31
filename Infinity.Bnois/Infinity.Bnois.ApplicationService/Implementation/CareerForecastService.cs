﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Data;
using Infinity.Bnois.ExceptionHelper;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class CareerForecastService : ICareerForecastService
    {

        private readonly IBnoisRepository<CareerForecast> careerForecastRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        private readonly IEmployeeService employeeService;
        public CareerForecastService(IBnoisRepository<CareerForecast> careerForecastRepository, IBnoisRepository<BnoisLog> bnoisLogRepository, IEmployeeService employeeService)
        {
            this.careerForecastRepository = careerForecastRepository;
            this.bnoisLogRepository = bnoisLogRepository;
            this.employeeService = employeeService;
        }

  

        public List<CareerForecastModel> GetCareerForecasts(int ps, int pn, string qs, out int total)
        {

            IQueryable<CareerForecast> CareerForecasts = careerForecastRepository.FilterWithInclude(x => x.IsActive && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee");
            total = CareerForecasts.Count();
            CareerForecasts = CareerForecasts.OrderByDescending(x => x.CareerForecastId).Skip((pn - 1) * ps).Take(ps);
            List<CareerForecastModel> models = ObjectConverter<CareerForecast, CareerForecastModel>.ConvertList(CareerForecasts.ToList()).ToList();
            return models;
        }
        public List<CareerForecastModel> GetCareerForecastsByEmployee(int employeeId)
        {
            List<CareerForecast> CareerForecasts = careerForecastRepository.FilterWithInclude(x => x.EmployeeId == employeeId, "Employee", "CareerForecastSetting").OrderBy(x => x.CareerForecastSetting.PositionNo).ToList();
            List<CareerForecastModel> models = ObjectConverter<CareerForecast, CareerForecastModel>.ConvertList(CareerForecasts.ToList()).ToList();
            return models;
        }
        public List<SelectModel> getQexamListSelectModel()
        {
            List<SelectModel> selectModels =
                 Enum.GetValues(typeof(Qexams)).Cast<Qexams>()
                      .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                      .ToList();
            return selectModels;
        }
        public List<SelectModel> getSceeListSelectModel()
        {
            List<SelectModel> selectModels =
                 Enum.GetValues(typeof(Scees)).Cast<Scees>()
                      .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                      .ToList();
            return selectModels;
        }
        public List<SelectModel> getBoardTypeSelectModel()
        {
            List<SelectModel> selectModels =
                 Enum.GetValues(typeof(BoardType)).Cast<BoardType>()
                      .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                      .ToList();
            return selectModels;
        }

        public async Task<CareerForecastModel> GetCareerForecast(int id)
        {
            if (id == 0)
            {
                return new CareerForecastModel();
            }
            CareerForecast CareerForecast = await careerForecastRepository.FindOneAsync(x => x.CareerForecastId == id, new List<string> { "Employee" });
            if (CareerForecast == null)
            {
                throw new InfinityNotFoundException(" Career Forecast not found");
            }
            CareerForecastModel model = ObjectConverter<CareerForecast, CareerForecastModel>.Convert(CareerForecast);
            return model;
        }

        public async Task<CareerForecastModel> SaveCareerForecast(int id, CareerForecastModel model)
        {

            if (model == null)
            {
                throw new InfinityArgumentMissingException("Employee Career Forecast data missing");
            }
            //CareerForecast careerForecast = new CareerForecast();
            CareerForecast careerForecast = ObjectConverter<CareerForecastModel, CareerForecast>.Convert(model);
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();

            //Education education = ObjectConverter<EducationModel, Education>.Convert(model);

            if (id > 0)
            {
                careerForecast = await careerForecastRepository.FindOneAsync(x => x.CareerForecastId == id);
                if (careerForecast == null)
                {
                    throw new InfinityNotFoundException("Education not found!");
                }
                careerForecast.ModifiedBy = userId;
                careerForecast.ModifiedDate = DateTime.Now;
            }
            else
            {
                careerForecast.IsActive = true;
                careerForecast.CreatedDate = DateTime.Now;
                careerForecast.CreatedBy = userId;
            }
            
            careerForecast.EmployeeId = model.EmployeeId;
            careerForecast.CareerForecastSettingId = model.CareerForecastSettingId;
            careerForecast.BranchId = model.BranchId;
            careerForecast.ForecastStatus = model.ForecastStatus;
            

            careerForecast.Employee = null;
            await careerForecastRepository.SaveAsync(careerForecast);
            model.CareerForecastId = careerForecast.CareerForecastId;
            return model;
        }

        public async Task<bool> DeleteCareerForecast(int id)
        {
            if (id <= 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            CareerForecast EmployeeCareerForecast = await careerForecastRepository.FindOneAsync(x => x.CareerForecastId == id);
            if (EmployeeCareerForecast == null)
            {
                throw new InfinityNotFoundException("Employee Family Permission not found");
            }
            else
            {
                bool IsDeleted = false;
                try
                {
                    // data log section start
                    BnoisLog bnLog = new BnoisLog();
                    bnLog.TableName = "CareerForecast";
                    bnLog.TableEntryForm = "Employee Career Forecast";
                    bnLog.PreviousValue = "Id: " + EmployeeCareerForecast.CareerForecastId;
                    var prevemp = employeeService.GetDynamicTableInfoById("Employee", "EmployeeId", EmployeeCareerForecast.EmployeeId);
                    bnLog.PreviousValue += ", Name: " + ((dynamic)prevemp).PNo + "_" + ((dynamic)prevemp).FullNameEng;
                    if (EmployeeCareerForecast.CareerForecastSettingId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("CareerForecastSetting", "CareerForecastSettingId", EmployeeCareerForecast.CareerForecastSettingId ?? 0);
                        bnLog.PreviousValue += ", Exam Title: " + ((dynamic)prev).PositionNo + " - " + ((dynamic)prev).Name;
                    }
                    if (EmployeeCareerForecast.BranchId > 0)
                    {
                        var prev = employeeService.GetDynamicTableInfoById("Branch", "BranchId", EmployeeCareerForecast.BranchId ?? 0);
                        bnLog.PreviousValue += ", Branch: " + ((dynamic)prev).Name;
                    }
                    bnLog.PreviousValue += ", Status: " + (EmployeeCareerForecast.ForecastStatus == 1 ? "Complete" : EmployeeCareerForecast.ForecastStatus == 2 ? "Bypass" : "");
                    
                    bnLog.UpdatedValue = "This Record has been Deleted!";
                    
                    bnLog.LogStatus = 2; // 1 for update, 2 for delete
                    bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                    bnLog.LogCreatedDate = DateTime.Now;


                    await bnoisLogRepository.SaveAsync(bnLog);

                    //data log section end
                    IsDeleted =  await careerForecastRepository.DeleteAsync(EmployeeCareerForecast);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                return IsDeleted;
            }
        }
    }
}