using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IRoasterListService
    {
        List<Object> GetRoasterListByShipType(int shipType);
        List<Object> GetLargeShipProposedWaitingCoXoList(int officeId, int appointment);
    }
}