using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IRoasterListEoLoSoService
    {
        List<Object> GetRoasterListForEoLoSoByShipType(int shipType, int coxoStatus);
        List<Object> GetLargeShipProposedWaitingCoXoList(int officeId, int appointment);
        List<Object> GetLargeShipEoSoLoWaitingList();
        List<Object> GetLargeShipSeoDloWaitingList();
        List<Object> GetMediumShipEoSoLoWaitingList();
    }
}