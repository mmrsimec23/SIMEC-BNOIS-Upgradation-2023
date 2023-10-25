using Infinity.Bnois.ApplicationService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infinity.Bnois.ApplicationService.Interface
{
    public interface IBnoisLogInfoService
    {
       
        List<Object> GetBnoisLogInfos(string tableName, int logStatus, string fromDate, string toDate);

        Task<List<SelectModel>> GetTableNameSelectModels();
        Task<List<SelectModel>> GetStatusSelectModels();
    }
}