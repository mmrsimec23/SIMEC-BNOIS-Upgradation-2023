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
    public class EmployeeCarLoanService : IEmployeeCarLoanService
    {
        private readonly IBnoisRepository<EmployeeCarLoan> _employeeCarLoanRepository;
        private readonly IBnoisRepository<Employee> _employeeRepository;

        public EmployeeCarLoanService(
            IBnoisRepository<EmployeeCarLoan> employeeCarLoanRepository,
            IBnoisRepository<Employee> employeeRepository
            )
        {
            _employeeCarLoanRepository = employeeCarLoanRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<bool> DeleteEmployeeCarLoan(int id)
        {
            if (id < 0)
            {
                throw new InfinityArgumentMissingException("Invalid Request");
            }
            EmployeeCarLoan employeeCarLoan = await _employeeCarLoanRepository.FindOneAsync(x => x.EmployeeCarLoanId == id);
            if (employeeCarLoan == null)
            {
                throw new InfinityNotFoundException("Employee Car Loan Info not found");
            }
            else
            {
                return await _employeeCarLoanRepository.DeleteAsync(employeeCarLoan);
            }
        }

        public async Task<EmployeeCarLoanModel> getEmployeeCarLoan(int id)
        {
            if (id <= 0)
            {
                return new EmployeeCarLoanModel();
            }
            EmployeeCarLoan employeeCarLoan = await _employeeCarLoanRepository.FindOneAsync(x => x.EmployeeCarLoanId == id, new List<string> { "Employee", "Employee.Rank", "Employee.Batch" });
            if (employeeCarLoan == null)
            {
                throw new InfinityNotFoundException("EmployeeCar Loan Info not found");
            }
            EmployeeCarLoanModel model = ObjectConverter<EmployeeCarLoan, EmployeeCarLoanModel>.Convert(employeeCarLoan);
            return model;
        }

        public List<EmployeeCarLoanModel> GetEmployeeCarLoanList(int ps, int pn, string qs, out int total)
        {
            IQueryable<EmployeeCarLoan> employeeCarLoans = _employeeCarLoanRepository.FilterWithInclude(x => x.Active && (x.Employee.PNo.Contains(qs) || string.IsNullOrEmpty(qs)), "Employee", "CarLoanFiscalYear","Rank");
            total = _employeeCarLoanRepository.Count();
            employeeCarLoans = employeeCarLoans.OrderByDescending(x => x.EmployeeCarLoanId).Skip((pn - 1) * ps).Take(ps);
            List<EmployeeCarLoanModel> models = ObjectConverter<EmployeeCarLoan, EmployeeCarLoanModel>.ConvertList(employeeCarLoans.ToList()).ToList();
            return models;
        }

        public List<EmployeeCarLoanModel> GetEmployeeCarLoansByPno(string PNo)
        {
            int employeeId = _employeeRepository.Where(x => x.PNo == PNo).Select(x => x.EmployeeId).SingleOrDefault();
            List<EmployeeCarLoan> employeeCarLoans = _employeeCarLoanRepository.Where(x => x.EmployeeId == employeeId).ToList();
            List<EmployeeCarLoanModel> models = ObjectConverter<EmployeeCarLoan, EmployeeCarLoanModel>.ConvertList(employeeCarLoans.ToList()).ToList();
            return models;
        }

        public async Task<EmployeeCarLoanModel> SaveEmployeeCarLoan(int id, EmployeeCarLoanModel model)
        {
            if (model == null)
            {
                throw new InfinityArgumentMissingException("Employee Car Loan data missing");
            }
            bool isExistData = _employeeCarLoanRepository.Exists(x => x.EmployeeCarLoanId == model.EmployeeId  && x.EmployeeCarLoanId != id);
            if (isExistData)
            {
                throw new InfinityInvalidDataException("Data already exists !");
            }
            string userId = ConfigurationResolver.Get().LoggedInUser.UserId.ToString();
            EmployeeCarLoan employeeCarLoan = ObjectConverter<EmployeeCarLoanModel, EmployeeCarLoan>.Convert(model);
            if (id > 0)
            {
                employeeCarLoan = await _employeeCarLoanRepository.FindOneAsync(x => x.EmployeeCarLoanId == id);
                if (employeeCarLoan == null)
                {
                    throw new InfinityNotFoundException("Employee Car Loan not found !");
                }

                employeeCarLoan.Modified = DateTime.Now;
                employeeCarLoan.ModifiedBy = userId;
            }
            else
            {
                employeeCarLoan.Active = true;
                employeeCarLoan.Created = DateTime.Now;
                employeeCarLoan.CreatedBy = userId;
            }
            employeeCarLoan.EmployeeId = model.EmployeeId;
            employeeCarLoan.IsBackLog = model.IsBackLog;
            employeeCarLoan.RankId = model.Employee.RankId;
            employeeCarLoan.Status = model.Status;
            employeeCarLoan.CarLoanFiscalYearId = model.CarLoanFiscalYearId;
            employeeCarLoan.AvailDate = model.AvailDate;
            employeeCarLoan.Amount = model.Amount;
            employeeCarLoan.Remarks = model.Remarks;

            if (model.IsBackLog)
            {
                employeeCarLoan.RankId = model.RankId;
            }
            employeeCarLoan.Employee = null;

            try
            {
                await _employeeCarLoanRepository.SaveAsync(employeeCarLoan);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            return model;
        }
    }
}
