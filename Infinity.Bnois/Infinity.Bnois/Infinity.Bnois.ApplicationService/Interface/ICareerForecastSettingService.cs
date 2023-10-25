using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Models;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface ICareerForecastSettingService
    {
        List<CareerForecastSettingModel> GetCareerForecastSettings(int ps, int pn, string qs, out int total);
        Task<CareerForecastSettingModel> GetCareerForecastSetting(int id);
        List<EmployeeCareerForecastSettingListModel> GetCareerForecastSettingByBranch(int id);
        Task<CareerForecastSettingModel> SaveCareerForecastSetting(int v, CareerForecastSettingModel model);
        Task<bool> DeleteCareerForecastSetting(int id);
        Task<List<SelectModel>> GetCareerForecastSettingSelectModels();
        Task<List<SelectModel>> GetCareerForecastByBranchSettingSelectModels(int branchId);
    }
}
