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

        public List<object> GetRoasterListByShipType(int shipType, int coxoStatus)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetCoOpvFfWaitingList] {0},{1} ", shipType, coxoStatus));
            return dataTable.ToJson().ToList();
        }

        //public dynamic GetSeaServices(string pno)
        //{

        //    Dictionary<string, dynamic> keyValues = new Dictionary<string, dynamic>();

        //    foreach (var type in Enum.GetValues((typeof(ShipType))))
        //    {
        //        var results = GetSeaServicesByType(pno, (int)type);

        //        var shipType = String.Empty;
        //        if (results.Count > 0)
        //        {
        //            if ((int)type == (int)ShipType.Small)
        //            {
        //                shipType = "Small";

        //            }
        //            else if ((int)type == (int)ShipType.Medium)
        //            {
        //                shipType = "Medium";
        //            }
        //            else
        //            {
        //                shipType = "Large";
        //            }


        //            keyValues.Add(shipType, results);
        //        }

        //    }
        //    return keyValues;
        //}

        public List<object> GetLargeShipProposedWaitingCoXoList(int shipType, int officeId, int appointment)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetCoXoProposedWaitingOfficerList] {0},{1}, {2}", shipType, officeId, appointment));
            return dataTable.ToJson().ToList();
        }
        public List<object> GetLargeShipCoWaitingList()
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetLargeCoPendingList]"));
            return dataTable.ToJson().ToList();
        }
        public List<object> GetLargeShipXoWaitingList()
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetLargeXoPendingList]"));
            return dataTable.ToJson().ToList();
        }
        public List<object> GetMediumShipCoWaitingList()
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetMediumCoPendingList]"));
            return dataTable.ToJson().ToList();
        }
        
        public List<object> GetSmallShipCoXoWaitingList(int shipType, int coxoStatus, int viewStatus)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetSmallCoOpvFfWaitingList] {0},{1},{2} ", shipType, coxoStatus, viewStatus));
            return dataTable.ToJson().ToList();
        }
        public List<object> GetSmallShipCoXoPendingList()
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetSmallCoXoPendingList]"));
            return dataTable.ToJson().ToList();
        }

    }
}