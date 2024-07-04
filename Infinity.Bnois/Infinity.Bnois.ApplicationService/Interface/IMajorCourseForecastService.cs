using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Models;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IMajorCourseForecastService
    {
        List<MajorCourseForecastModel> GetMajorCourseForecasts(int type, int ps, int pn, string qs, out int total);
        Task<MajorCourseForecastModel> GetMajorCourseForecast(int id);
        Task<MajorCourseForecastModel> SaveMajorCourseForecast(int v, MajorCourseForecastModel model);
        Task<bool> DeleteMajorCourseForecast(int id);
        List<SelectModel> GetMajorCourseTypeSelectModels();
        //List<SelectModel> GetCoxoAppoinmentSelectModels(int type);

    }
}