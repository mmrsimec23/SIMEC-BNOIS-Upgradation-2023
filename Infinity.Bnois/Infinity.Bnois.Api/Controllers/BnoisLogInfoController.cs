using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Infinity.Bnois.Api.Core;
using Infinity.Bnois.Api.Models;
using Infinity.Bnois.Api.Right;
using Infinity.Bnois.ApplicationService.Implementation;
using Infinity.Bnois.ApplicationService.Interface;
using Infinity.Bnois.ApplicationService.Models;
using Infinity.Bnois.Data;
using Infinity.Ers.ApplicationService;
using Microsoft.SqlServer.Server;

namespace Infinity.Bnois.Api.Controllers
{
    [RoutePrefix(BnoisRoutePrefix.BnoisLogInfo)]
    [EnableCors("*", "*", "*")]
    [ActionAuthorize(Feature = MASTER_SETUP.BNOIS_LOG_INFO)]

    public class BnoisLogInfoController : BaseController
    {
        private readonly IBnoisLogInfoService bnoisLogInfoService;
       

        public BnoisLogInfoController(IBnoisLogInfoService bnoisLogInfoService)
        {
            this.bnoisLogInfoService = bnoisLogInfoService;
        }
        [HttpGet]
        [Route("get-bnois-log-select-models")]
        public async Task<IHttpActionResult> GetBnoisLogSelectModels()
        {
            BnoisLogInfoViewModel vm = new BnoisLogInfoViewModel();

            vm.TableList = await bnoisLogInfoService.GetTableNameSelectModels();
            vm.LogStatusList = await bnoisLogInfoService.GetStatusSelectModels();


            return Ok(new ResponseMessage<BnoisLogInfoViewModel>()
            {
                Result = vm

            });
        }

        [HttpGet]
        [Route("get-bnois-log-infos")]
        public IHttpActionResult GetBnoisLogInfos(string tableName, int logStatus, string fromDate, string toDate)
        {
            if(tableName == "null")
            {
                tableName = "";
            }
            return Ok(new ResponseMessage<List<object>>()
            {
                Result = bnoisLogInfoService.GetBnoisLogInfos(tableName, logStatus, fromDate, toDate)
            });
        }
        
    }
}