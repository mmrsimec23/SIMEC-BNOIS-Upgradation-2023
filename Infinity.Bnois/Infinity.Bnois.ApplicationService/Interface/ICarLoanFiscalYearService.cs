using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Models;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface ICarLoanFiscalYearService
    {
        List<CarLoanFiscalYearModel> GetCarLoanFiscalYears(int ps, int pn, string qs, out int total);
        Task<CarLoanFiscalYearModel> GetCarLoanFiscalYear(int id);
        Task<CarLoanFiscalYearModel> SaveCarLoanFiscalYear(int v, CarLoanFiscalYearModel model);
        Task<bool> DeleteCarLoanFiscalYear(int id);
        Task<List<SelectModel>> GetCarLoanFiscalYearsSelectModels();
    }
}
