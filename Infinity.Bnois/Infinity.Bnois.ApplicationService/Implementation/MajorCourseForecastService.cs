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
    public class MajorCourseForecastService : IMajorCourseForecastService
    {
        private readonly IBnoisRepository<MajorCourseForecast> majorCourseForecastRepository;
        public MajorCourseForecastService(IBnoisRepository<MajorCourseForecast> majorCourseForecastRepository)
        {
            this.majorCourseForecastRepository = majorCourseForecastRepository;
        }

        public List<MajorCourseForecastModel> GetMajorCourseForecasts(int type, int ps, int pn, string qs, out int total)
        {
            IQueryable<MajorCourseForecast> majorCourseForecasts = majorCourseForecastRepository.FilterWithInclude(x => x.IsActive && (type == 0 || x.Type == type)
                 && (x.Employee.PNo == (qs) || x.Employee.FullNameEng.Contains(qs) || String.IsNullOrEmpty(qs)), "Employee");
            total = majorCourseForecasts.Count();
            majorCourseForecasts = majorCourseForecasts.OrderByDescending(x => x.MajorCourseForecastId).Skip((pn - 1) * ps).Take(ps);
            List<MajorCourseForecastModel> models = ObjectConverter<MajorCourseForecast, MajorCourseForecastModel>.ConvertList(majorCourseForecasts.ToList()).ToList();
            return models;
        }

        public async Task<MajorCourseForecastModel> GetMajorCourseForecast(int id)
        {
            if (id <= 0)
            {
                return new MajorCourseForecastModel();
            }
            MajorCourseForecast majorCourseForecast = await majorCourseForecastRepository.FindOneAsync(x => x.MajorCourseForecastId == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch" });
            if (majorCourseForecast == null)
            {
                throw new InfinityNotFoundException("Employee PFT not found");
            }
            MajorCourseForecastModel model = ObjectConverter<MajorCourseForecast, MajorCourseForecastModel>.Convert(majorCourseForecast);
            return model;
        }



        public async Task<MajorCourseForecastModel> SaveMajorCourseForecast(int id, MajorCourseForecastModel model)
        {
            model.Employee = null;
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Course Forecast data missing");
            }
            //bool isExistData = majorCourseForecastRepository.Exists(x => x.EmployeeId == model.EmployeeId && x.Type == model.ty && x.Type == model.Type && x.CoXoServiceId != id);
            //if (isExistData)
            //{
            //    throw new InfinityInvalidDataException("Data already exists !");
            //}
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            MajorCourseForecast majorCourseForecast = ObjectConverter<MajorCourseForecastModel, MajorCourseForecast>.Convert(model);
            if (id > 0)
            {
                majorCourseForecast = await majorCourseForecastRepository.FindOneAsync(x => x.MajorCourseForecastId == id);
                if (majorCourseForecast == null)
                {
                    throw new InfinityNotFoundException("Course Forecast not found !");
                }

                majorCourseForecast.ModifiedDate = DateTime.Now;
                majorCourseForecast.ModifiedBy = userId;
            }
            else
            {
                majorCourseForecast.IsActive = true;
                majorCourseForecast.CreatedDate = DateTime.Now;
                majorCourseForecast.CreatedBy = userId;
            }
            majorCourseForecast.EmployeeId = model.EmployeeId;
            majorCourseForecast.Type = model.Type;
            majorCourseForecast.Remarks = model.Remarks;
            majorCourseForecast.ExpiryDate = model.ExpiryDate;
            majorCourseForecast.Status = model.Status;
            majorCourseForecast.Remarks = model.Remarks;



            await majorCourseForecastRepository.SaveAsync(majorCourseForecast);
            model.MajorCourseForecastId = majorCourseForecast.MajorCourseForecastId;
            return model;
        }


        public async Task<bool> DeleteMajorCourseForecast(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            MajorCourseForecast majorCourseForecast = await majorCourseForecastRepository.FindOneAsync(x => x.MajorCourseForecastId == id);
            if (majorCourseForecast == null)
            {
                throw new InfinityNotFoundException("Employee PFT not found");
            }
            else
            {
                return await majorCourseForecastRepository.DeleteAsync(majorCourseForecast);
            }
        }

        public List<SelectModel> GetMajorCourseTypeSelectModels()
        {
            List<SelectModel> selectModels =
                Enum.GetValues(typeof(MajorCourseType)).Cast<MajorCourseType>()
                    .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                    .ToList();
            return selectModels;
        }

        //public List<SelectModel> GetCoxoAppoinmentSelectModels(int type)
        //{
        //    if (type == 1 || type == 3)
        //    {
        //        List<SelectModel> selectModels = Enum.GetValues(typeof(CoXoAppointment)).Cast<CoXoAppointment>().Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) }).ToList();
        //        return selectModels;
        //    }
        //    if (type == 2)
        //    {
        //        List<SelectModel> selectModels = Enum.GetValues(typeof(EoLoSoAppointment)).Cast<EoLoSoAppointment>().Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) }).ToList();
        //        return selectModels;
        //    }
        //    return null;
        //}


    }
}