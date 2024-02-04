using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.Data;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class RoasterListEoLoSoService : IRoasterListEoLoSoService
    {
        private readonly IBnoisRepository<Employee> employeeRepository;
        public RoasterListEoLoSoService(IBnoisRepository<Employee> employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }

        public List<object> GetRoasterListForEoLoSoByShipType(int shipType, int coxoStatus)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetCoOpvFfWaitingListForDLOorEOorLOForDLOorEOorLO] {0},{1} ", shipType, coxoStatus));
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

        public List<object> GetLargeShipProposedWaitingCoXoList(int officeId, int appointment)
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetCoXoProposedWaitingOfficerList] {0},{1}", officeId, appointment));
            return dataTable.ToJson().ToList();
        }
        public List<object> GetLargeShipEoSoLoWaitingList()
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetLargeEoSoLoPendingList]"));
            return dataTable.ToJson().ToList();
        }
        public List<object> GetLargeShipSeoDloWaitingList()
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetLargeSeoDloEoSoLoPendingList]"));
            return dataTable.ToJson().ToList();
        }
        public List<object> GetMediumShipEoSoLoWaitingList()
        {
            DataTable dataTable = employeeRepository.ExecWithSqlQuery(String.Format("exec [spGetMediumEoSoLoPendingList]"));
            return dataTable.ToJson().ToList();
        }

    }
}