using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IRoasterListService
    {
        List<Object> GetRoasterListByShipType(int shipType, int coxoStatus);
        List<Object> GetLargeShipProposedWaitingCoXoList(int shipType, int officeId, int appointment);
        List<Object> GetLargeShipCoWaitingList();
        List<Object> GetLargeShipXoWaitingList();
        List<Object> GetMediumShipCoWaitingList();
        List<Object> GetSmallShipCoXoWaitingList(int shipType, int coxoStatus,int viewStatus);
        List<Object> GetSmallShipCoXoPendingList();
    }
}