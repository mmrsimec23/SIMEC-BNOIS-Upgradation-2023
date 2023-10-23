using Infinity.Bnois.Api.Core;
using Infinity.Bnois.Api.Models;
using Infinity.Bnois.Api.Right;
using Infinity.Bnois.ApplicationService.Implementation;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Configuration;
using Infinity.Bnois.Configuration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Infinity.Bnois.Api.Controllers
{
    [RoutePrefix(BnoisRoutePrefix.EmployeeCarLoan)]
    [EnableCors("*", "*", "*")]
    [ActionAuthorize(Feature = MASTER_SETUP.EMPLOYEE_CAR_LOAN)]
    public class EmployeeCarLoanController: PermissionController
    {
        private readonly IEmployeeCarLoanService _employeeCarLoanService;
        private readonly ICarLoanFiscalYearService _carLoanFiscalYearService;
        private readonly IRankService _rankService;
        public EmployeeCarLoanController(
            IEmployeeCarLoanService employeeCarLoanService, 
            ICarLoanFiscalYearService carLoanFiscalYearService, 
            IRankService rankService, 
            IRoleFeatureService roleFeatureService
            ) : base(roleFeatureService)
        {
            _employeeCarLoanService = employeeCarLoanService;
            _carLoanFiscalYearService = carLoanFiscalYearService;
            _rankService = rankService;
        }
        [HttpGet]
        [Route("get-employee-car-loan-list")]
        public IHttpActionResult GetEmployeeCarLoanList(int ps, int pn, string qs)
        {
            int total = 0;
            List<EmployeeCarLoanModel> models = _employeeCarLoanService.GetEmployeeCarLoanList(ps, pn, qs, out total);
            RoleFeature permission = base.GetFeature(MASTER_SETUP.EMPLOYEE_CAR_LOAN);
            return Ok(new ResponseMessage<List<EmployeeCarLoanModel>>()
            {
                Result = models,
                Total = total,
                Permission = permission
            });
        }
        [HttpGet]
        [Route("get-employee-car-loan-by-pno")]
        public IHttpActionResult GetEmployeeCarLoansByPno(string PNo)
        {
            return Ok(new ResponseMessage<List<EmployeeCarLoanModel>>()
            {
                Result = _employeeCarLoanService.GetEmployeeCarLoansByPno(PNo)
        });
        }
        [HttpGet]
        [Route("get-employee-car-loan")]
        public async Task<IHttpActionResult> getEmployeeCarLoan(int id)
        {
            EmployeeCarLoanViewModel vm = new EmployeeCarLoanViewModel(); 
            vm.EmployeeCarLoan = await _employeeCarLoanService.getEmployeeCarLoan(id);
            vm.CarLoanFiscalYears = await _carLoanFiscalYearService.GetCarLoanFiscalYearsSelectModels();
            vm.Ranks = await _rankService.GetRanksSelectModel();
            return Ok(new ResponseMessage<EmployeeCarLoanViewModel>
            {
                Result = vm
            });
        }
        [HttpPost]
        [ModelValidation] 
        [Route("save-employee-car-loan")]
        public async Task<IHttpActionResult> saveEmployeeCarLoan([FromBody] EmployeeCarLoanModel model)
        {
            return Ok(new ResponseMessage<EmployeeCarLoanModel>
            {
                Result = await _employeeCarLoanService.SaveEmployeeCarLoan(0, model)
            });
        }
        [HttpPut]
        [Route("update-employee-car-loan/{id}")]
        public async Task<IHttpActionResult> updateEmployeeCarLoan(int id, [FromBody] EmployeeCarLoanModel model)
        {
            return Ok(new ResponseMessage<EmployeeCarLoanModel>
            {
                Result = await _employeeCarLoanService.SaveEmployeeCarLoan(id, model)

            });
        }

        [HttpDelete]
        [Route("delete-employee-car-loan/{id}")]
        public async Task<IHttpActionResult> DeleteEmployeeCarLoan(int id)
        {
            return Ok(new ResponseMessage<bool>
            {
                Result = await _employeeCarLoanService.DeleteEmployeeCarLoan(id)
            });
        }

    }
}
