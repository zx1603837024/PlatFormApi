using CommonTool;
using F2.Application.Parkade;
using F2.Application.Parkade.Dtos;
using F2.Application.VideoEqs;
using F2.Application.VideoEqs.Dtos;
using F2.Common;
using F2.Core.Extensions;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.UI.WebControls;
using System.Windows.Documents;

namespace F2Api.Controllers
{
    /// <summary>
    /// 视频设备接口
    /// </summary>
    public class InterfaceParkadeController : ApiController
    {
        #region Define
        private readonly IParkadeService _ParkadeService;
        #endregion

        public InterfaceParkadeController()
        {
            _ParkadeService = new ParkadeService();
        }
        /// <summary>
        /// 车牌识别结果接收
        /// </summary>
        /// <param name="dto"></param>
        [HttpPost]
        public Hashtable PlateResultReceive(object dto)
        {
            Hashtable res = new Hashtable();
            try {
                var json = JsonConvert.SerializeObject(dto);
                CommonTools.WriteLogFile("车牌识别结果接收:" + json.ToString());
                var param = JsonConvert.DeserializeObject<PlateRequest>(json);
                ParkadeRequest req = new ParkadeRequest();
                req.serialno = param.alarmInfoPlate.serialno;
                req.license = param.alarmInfoPlate.result.plateResult.license;
                req.imageFile = "data:image/png;base64," + param.alarmInfoPlate.result.plateResult.imageFile;
                req.imageFragmentFile = "data:image/png;base64," + param.alarmInfoPlate.result.plateResult.imageFragmentFile;
                req.sec = param.alarmInfoPlate.result.plateResult.timeStamp.timeval.sec;
                req.confidence = param.alarmInfoPlate.result.plateResult.confidence;
                req.plateid = param.alarmInfoPlate.result.plateResult.plateid;
                res=_ParkadeService.PlateResultReceive(req);
            }
            catch (Exception ex)
            {
                CommonTools.WriteLogFile("车牌识别结果接收:Error:" + ex.ToString());   
            }
            return res;
        }
        /// <summary>
        /// 手动开闸
        /// </summary>
        /// <param name="dto"></param>
        public Hashtable OpenPole(Hashtable dto) {
            return _ParkadeService.OpenPoleService(dto);
        }
        /// <summary>
        /// MQ开闸回调
        /// </summary>
        /// <param name="dto"></param>
        public Hashtable OpenPoleCallBack(Hashtable dto)
        {
            return _ParkadeService.OpenPoleCallBack(dto);
        }

        public void PostMqttData() {
            /*MqttClientService client = new MqttClientService();
            client.MqttClientStart();
            client.Publish("4566");*/
            Commons com = new Commons();
            String[] show = { "a","b","c" };
            var www=com.ScreenShow(show);
            var sww = Convert.ToBase64String(www);
            var sww2 = www.Length;
        }
    }
}