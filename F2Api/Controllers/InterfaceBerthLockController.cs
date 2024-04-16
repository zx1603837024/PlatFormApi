using F2.Application.BerthLock;
using F2.Application.VideoEqs;
using F2.Application.VideoEqs.Dtos;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using F2.Application.BerthLock.Dtos;
using F2.Common;
using F2.Core.Extensions.Log;
using System.Web.Razor.Parser.SyntaxTree;
using CommonTool;

namespace F2Api.Controllers
{
    public class InterfaceBerthLockController : ApiController
    {
        #region Define
        private readonly IBerthLockService _BerthLockService;
        Commons _com;
        #endregion

        public InterfaceBerthLockController() {
            _BerthLockService = new BerthLockService();
            _com = new Commons();
        }
        [HttpPost]
        public string BerthLockRecControl(object dto)
        {
            try
            {
                BerthLockRequest req = new BerthLockRequest();
                JObject input = JsonConvert.DeserializeObject<JObject>(dto.ToString());
                req.SN = Convert.ToString(input["data"]["dev_id"]);
                TimeSpan mTimeSpan = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0);
                req.happenTime =(long)mTimeSpan.TotalSeconds;
                if (Convert.ToString(input["data"]["hcmdtype"])== "13")//上锁
                {
                    _BerthLockService.LockDataReceive(req);
                }
                if (Convert.ToString(input["data"]["hcmdtype"]) == "11")//下锁
                {
                    _BerthLockService.LockDownDataReceive(req);
                }
                if (Convert.ToString(input["data"]["hcmdtype"]) == "01")//心跳
                {
                    _BerthLockService.LockBeatDataReceive(req);
                }
            }
            catch (Exception ex)
            {
                //记录日志
                Logger.Log.Error("BerthLockRecControl:Error:" + ex.ToString());
            }
            return "ok";
        }
        [HttpPost]
        public string TestKeyControl(object dto)
        {
            var sign = HttpContext.Current.Request.Headers["fx-sign"];
            string payload = _com.HmacSha256Authentication(dto, sign);
            if (payload!="false") {
                JObject input = JsonConvert.DeserializeObject<JObject>(payload);
            }
            return "ok";
        }
    }
}