﻿using Infinity.Bnois.ApplicationService.Interface;
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
    public class CarLoanFiscalYearService : ICarLoanFiscalYearService
    {
        private readonly IBnoisRepository<CarLoanFiscalYear> carLoanFiscalYearRepository;
        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;
        public CarLoanFiscalYearService(IBnoisRepository<CarLoanFiscalYear> carLoanFiscalYearRepository, IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.carLoanFiscalYearRepository = carLoanFiscalYearRepository;
            this.bnoisLogRepository = bnoisLogRepository;
        }
        
        public async Task<CarLoanFiscalYearModel> GetCarLoanFiscalYear(int id)
        {
            if (id <= 0)
            {
                return new CarLoanFiscalYearModel();
            }
            CarLoanFiscalYear carLoanFiscalYear = await carLoanFiscalYearRepository.FindOneAsync(x => x.CarLoanFiscalYearId == id);
            if (carLoanFiscalYear == null)
            {
                throw new InfinityNotFoundException("Car Loan Fiscal Years not found");
            }
            CarLoanFiscalYearModel model = ObjectConverter<CarLoanFiscalYear, CarLoanFiscalYearModel>.Convert(carLoanFiscalYear);
            return model;
        }

        public List<CarLoanFiscalYearModel> GetCarLoanFiscalYears(int ps, int pn, string qs, out int total)
        {
            IQueryable<CarLoanFiscalYear> carLoanFiscalYears = carLoanFiscalYearRepository.FilterWithInclude(x => x.IsActive && (x.Name.Contains(qs) || String.IsNullOrEmpty(qs)));
            total = carLoanFiscalYears.Count();
            carLoanFiscalYears = carLoanFiscalYears.OrderByDescending(x => x.CarLoanFiscalYearId).Skip((pn - 1) * ps).Take(ps);
            List<CarLoanFiscalYearModel> models = ObjectConverter<CarLoanFiscalYear, CarLoanFiscalYearModel>.ConvertList(carLoanFiscalYears.ToList()).ToList();
            return models;
        }

        public async Task<CarLoanFiscalYearModel> SaveCarLoanFiscalYear(int id, CarLoanFiscalYearModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Car Loan Fiscal Years data missing");
            }
            bool isExist = carLoanFiscalYearRepository.Exists(x => x.Name == model.Name && x.CarLoanFiscalYearId != id);
            if (isExist)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }

            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            CarLoanFiscalYear carLoanFiscalYear = ObjectConverter<CarLoanFiscalYearModel, CarLoanFiscalYear>.Convert(model);
            if (id > 0)
            {
                carLoanFiscalYear = await carLoanFiscalYearRepository.FindOneAsync(x => x.CarLoanFiscalYearId == id);
                if (carLoanFiscalYear == null)
                {
                    throw new InfinityNotFoundException("Car Loan Fiscal Years not found !");
                }

                carLoanFiscalYear.ModifiedDate = DateTime.Now;
               carLoanFiscalYear.ModifiedBy = userId;
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "CarLoanFiscalYear";
                bnLog.TableEntryForm = "Car Loan Fiscal Year";
                bnLog.PreviousValue = "Id: " + model.CarLoanFiscalYearId;
                bnLog.UpdatedValue = "Id: " + model.CarLoanFiscalYearId;
                if (carLoanFiscalYear.Name != model.Name)
                {
                    bnLog.PreviousValue += ", Name: " + carLoanFiscalYear.Name;
                    bnLog.UpdatedValue += ", Name: " + model.Name;
                }
                if (carLoanFiscalYear.Remarks != model.Remarks)
                {
                    bnLog.PreviousValue += ", Remarks: " + carLoanFiscalYear.Remarks;
                    bnLog.UpdatedValue += ", Remarks: " + model.Remarks;
                }

                bnLog.LogStatus = 1; // 1 for update, 2 for delete
                bnLog.UserId = userId;
                bnLog.LogCreatedDate = DateTime.Now;

                if (carLoanFiscalYear.Name != model.Name || carLoanFiscalYear.Remarks != model.Remarks)
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
                carLoanFiscalYear.IsActive = true;
                carLoanFiscalYear.CreatedDate = DateTime.Now;
                carLoanFiscalYear.CreatedBy = userId;
            }
            carLoanFiscalYear.Name = model.Name;
            carLoanFiscalYear.Remarks = model.Remarks;


            await carLoanFiscalYearRepository.SaveAsync(carLoanFiscalYear);
            model.CarLoanFiscalYearId = carLoanFiscalYear.CarLoanFiscalYearId;
            return model;
        }
        public async Task<bool> DeleteCarLoanFiscalYear(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            CarLoanFiscalYear CarLoanFiscalYear = await carLoanFiscalYearRepository.FindOneAsync(x => x.CarLoanFiscalYearId == id);
            if (CarLoanFiscalYear == null)
            {
                throw new InfinityNotFoundException("CarLoanFiscalYears not found");
            }
            else
            {
                // data log section start
                BnoisLog bnLog = new BnoisLog();
                bnLog.TableName = "CarLoanFiscalYear";
                bnLog.TableEntryForm = "Car Loan Fiscal Year";
                bnLog.PreviousValue = "Id: " + CarLoanFiscalYear.CarLoanFiscalYearId + ", Name: " + CarLoanFiscalYear.Name + ", Remarks: " + CarLoanFiscalYear.Remarks;
                bnLog.UpdatedValue = "This Record has been Deleted!";

                bnLog.LogStatus = 2; // 1 for update, 2 for delete
                bnLog.UserId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
                bnLog.LogCreatedDate = DateTime.Now;

                await bnoisLogRepository.SaveAsync(bnLog);

                //data log section end
                return await carLoanFiscalYearRepository.DeleteAsync(CarLoanFiscalYear);
            }
        }

        public async Task<List<SelectModel>> GetCarLoanFiscalYearsSelectModels()
        {
            ICollection<CarLoanFiscalYear> CarLoanFiscalYears = await carLoanFiscalYearRepository.FilterAsync(x => x.IsActive);
            List<SelectModel> selectModels = CarLoanFiscalYears.OrderBy(x => x.Name).Select(x => new SelectModel()
            {
                Text = x.Name,
                Value = x.CarLoanFiscalYearId
            }).ToList();
            return selectModels;
        }
    }
}
