using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.Data;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class RoasterListService : IRoasterListService
    {
        private readonly IBnoisRepository<Employee> employeeRepository;
        public RoasterListService(IBnoisRepository<Employee> employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }

        public List<object> GetRoasterListByShipType(int shipType)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetCoOpvFfWaitingList] {0} ", shipType));
            return dataTable.ToJson().ToList();
        }

    }
}