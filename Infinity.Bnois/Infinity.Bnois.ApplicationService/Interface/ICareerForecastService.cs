using System.Collections.Generic;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Models;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface ICareerForecastService
    {

        List<CareerForecastModel> GetCareerForecasts(int ps, int pn, string qs, out int total);
        Task<CareerForecastModel> GetCareerForecast(int id);
        Task<CareerForecastModel> SaveCareerForecast(int v, CareerForecastModel model);
        Task<bool> DeleteCareerForecast(int id);
        List<CareerForecastModel> GetCareerForecastsByEmployee(int employeeId);
        //List<SelectModel> getQexamListSelectModel();
        //List<SelectModel> getSceeListSelectModel();

    }
}