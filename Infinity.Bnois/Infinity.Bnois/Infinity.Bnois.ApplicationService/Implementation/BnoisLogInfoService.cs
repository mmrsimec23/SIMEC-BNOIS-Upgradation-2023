using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Data;

namespace Infinity.Bnois.ApplicationService.Implementation
{
    public class BnoisLogInfoService : IBnoisLogInfoService
    {

        private readonly IBnoisRepository<BnoisLog> bnoisLogRepository;

        public BnoisLogInfoService( IBnoisRepository<BnoisLog> bnoisLogRepository)
        {
            this.bnoisLogRepository = bnoisLogRepository;

        }



        public List<object> GetBnoisLogInfos(string tableName, int logStatus, string fromDate, string toDate)
        {
            //model.TableName = "";
            
            DataTable dataTable = bnoisLogRepository.ExecWithSqlQuery(String.Format("exec [spGetBnoisLogInfo] '{0}',{1},'{2}','{3}'", tableName, logStatus, fromDate, toDate));

            return dataTable.ToJson().ToList();
        }

        public async Task<List<SelectModel>> GetTableNameSelectModels()
        {
            DataTable dataTable = bnoisLogRepository.ExecWithSqlQuery(String.Format("exec [spGetBnoisLogTableList]"));
            List<dynamic> models = dataTable.ToJson().ToList();
            return models.OrderBy(x => x.TableEntryForm).Select(x => new SelectModel()
            {
                Text = x.TableEntryForm,
                Value = x.TableName
            }).ToList();

        }

        public async Task<List<SelectModel>> GetStatusSelectModels()
        {
            List<SelectModel> selectModels =
              Enum.GetValues(typeof(LogStatusType)).Cast<LogStatusType>()
                   .Select(v => new SelectModel { Text = v.ToString(), Value = Convert.ToInt16(v) })
                   .ToList();
            return selectModels;
        }
    }
}