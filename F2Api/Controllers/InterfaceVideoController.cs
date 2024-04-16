using CommonTool;
using F2.Application.Parkade.Dtos;
using F2.Application.VideoEqs;
using F2.Application.VideoEqs.Dtos;
using F2.Common;
using F2.Core.Extensions;
using F2.Core.Extensions.Log;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.UI.WebControls;
using System.Windows.Documents;
using static CommonTool.ApiEnum;
using static CommonTool.Commons;

namespace F2Api.Controllers
{
    /// <summary>
    /// 视频设备接口
    /// </summary>
    public class InterfaceVideoController : ApiController
    {
        #region Define
        private readonly IVideoEqAppService _VideoEqAppService;
        #endregion

        /// <summary>
        /// construct
        /// </summary>
        public InterfaceVideoController()
        {
            _VideoEqAppService = new VideoEqAppService();
        }
        /// <summary>
        /// 高位视频设备状态推送
        /// </summary>
        /// <param name="dto"></param>
        public VideoEqParkHighRepose postStateHighData(Hashtable dto)
        {
            return _VideoEqAppService.PostStateHighData(dto);
        }
        /// <summary>
        /// 高位停车数据接口,byWRS
        /// </summary>
        [HttpPost]
        public VideoEqParkHighRepose parkDataForHigh(VideoEqParkHighRequest dto)
        {
            return _VideoEqAppService.PostVideoEqParkHighData(dto);
        }
        /// <summary>
        /// 高位停车补图片
        /// </summary>
        [HttpPost]
        public VideoEqParkHighRepose pushPieceForHigh(VideoEqParkHighRequest dto)
        {
            return _VideoEqAppService.pushPieceForHigh(dto);
        }
        /// <summary>
        /// 高位报警推送接口,byWRS
        /// </summary>
        [HttpPost]
        public VideoEqParkHighRepose pushFaultDataForHigh(VideoEqParkHighRequest dto)
        {
            return _VideoEqAppService.PushFaultDataForHigh(dto);
        }

        /// <summary>
        /// 高位报警推送接口,byWRS
        /// </summary>
        [HttpPost]
        public VideoEqParkHighRepose pushFixDataForHigh(VideoEqParkHighRequest dto)
        {
            return _VideoEqAppService.pushFixDataForHigh(dto);
        }

        /// <summary>
        /// 臻识设备第三方平台，通用接口对接
        /// </summary>
        [HttpPost]
        public VideoEqParkHighRepose CommonParkData(VideoEqParkHighRequest dto)
        {
            if (dto.evt == 1 || dto.evt == 4)
            {
                VideoEqParkHighRepose res=parkDataForHigh(dto);
                if (dto.evt == 1) {
                    pushPieceForHigh(dto);
                }
                return res;
            }
            else if (dto.evt == 16)
            {
                return pushFaultDataForHigh(dto);
            }
            else if (dto.evt == 512)
            {
                return pushFixDataForHigh(dto);
            }
            else if (dto.evt == 2 || dto.evt == 8 || dto.evt == 128 || dto.evt == 256)
            {
                return pushPieceForHigh(dto);

            }
            else if (dto.evt == 0)
            {
                string input = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
                var param = JsonConvert.DeserializeObject<Hashtable>(input);
                return postStateHighData(param);
            }
            else
            {
                return new VideoEqParkHighRepose();
            }
        }
        /// <summary>
        /// 停易通对接停车数据
        /// </summary>
        [HttpPost]
        public string TytParkDataControl(Hashtable input)
        {
            try
            {
                VideoEqParkHighRequest dto = new VideoEqParkHighRequest();
                if (Convert.ToInt32(input["STATE"]) == 3)
                {
                    dto.evt = 1;
                }
                if (Convert.ToInt32(input["STATE"]) == 1)
                {
                    dto.evt = 4;
                }
                dto.deviceSn = Convert.ToString(input["SN"]);
                dto.parkingActId = Convert.ToString(input["berthCode"]);
                dto.berthCode = Convert.ToString(input["berthCode"]);
                dto.plateNumber = Convert.ToString(input["PLATE"]);
                dto.plateColor = Convert.ToInt32(input["plateColor"]);
                dto.happenTime = (Convert.ToDateTime(input["recTime"]).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                dto.picUrl = "data:image/png;base64," + Convert.ToString(input["ossPath"]);
                if (!string.IsNullOrEmpty(Convert.ToString(input["powerp"])))
                {
                    string sql = "update AbpVideoEquips set Battery='" + Convert.ToInt32(input["powerp"]) + "' where VedioEqNumber = '" + dto.deviceSn + "' and BerthNumber='" + dto.berthCode + "'";
                    SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                }
                dto.Trust = 100;
                dto.actionCredible = 100;
                if (dto.plateNumber == "无牌车")
                {
                    dto.actionCredible = 0;
                }
                Commons com = new Commons();
                bool isFlag = com.IsValidPlateNumber(dto.plateNumber);
                if (!isFlag)
                {
                    dto.actionCredible = 0;
                }
                if (dto.evt == 1 || dto.evt == 4)
                {
                    _VideoEqAppService.PostVideoEqParkHighData(dto);
                }
            }
            catch (Exception ex)
            {
                //记录日志
                CommonTools.WriteLogFile("TytParkDataControl:Error" + ex.ToString());
            }
            return "ok";
        }
        /// <summary>
        /// 停易通对接异常数据
        /// </summary>
        [HttpPost]
        public string TytUnusualDataControl(Hashtable input)
        {
            try
            {
                VideoEqParkHighRequest dto = new VideoEqParkHighRequest();
                dto.evt = 16;
                dto.deviceSn = Convert.ToString(input["sn"]);
                dto.parkingActId = Convert.ToString(input["id"]);
                dto.happenTime = Convert.ToInt64(input["statusTime"]) / 1000;
                dto.picUrl = "data:image/png;base64," + Convert.ToString(input["ossPath"]);
                dto.parkingAbnormalType = Convert.ToInt32(input["status"]);
                var sql = "select BerthNumber from AbpVideoEquips where BerthNumber is not null and VedioEqNumber='" + dto.deviceSn + "'";
                DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                if (tables.Rows.Count > 0)
                {
                    dto.berthCode = Convert.ToString(tables.Rows[0][0]);
                    if (Convert.ToString(input["status"]) != "0")
                    {
                        _VideoEqAppService.PushFaultDataForHigh(dto);
                    }
                }
            }
            catch (Exception ex)
            {
                //记录日志
                CommonTools.WriteLogFile("TytUnusualDataControl:Error" + ex.ToString());
            }
            return "ok";
        }
        /// <summary>
        /// 停易通对接心跳数据
        /// </summary>
        [HttpPost]
        public string TytBeatDataControl(List<Hashtable> input)
        {
            try
            {
                Hashtable param = new Hashtable();
                var frequency = 10;//频率10分钟
                var Now = DateTime.Now.ToLocalTime();
                string deviceStr1 = "";
                string deviceStr2 = "";
                List<string> deList1 = new List<string>();
                List<string> deList2 = new List<string>();
                foreach (Hashtable element in input)
                {
                    var DataTime = Convert.ToDateTime(element["updateTime"]);
                    if ((Now - DataTime).TotalMinutes < 30 * frequency)
                    {
                        if (!deList1.Contains(Convert.ToString(element["SN"])))
                        {
                            deList1.Add(Convert.ToString(element["SN"]));
                            deviceStr1 = deviceStr1 + "'" + Convert.ToString(element["SN"]) + "',";
                        }
                    }
                    if (!deList2.Contains(Convert.ToString(element["SN"])))
                    {
                        deList2.Add(Convert.ToString(element["SN"]));
                        deviceStr2 = deviceStr2 + "'" + Convert.ToString(element["SN"]) + "',";
                    }
                }
                if (!string.IsNullOrEmpty(deviceStr2))
                {
                    deviceStr2 = deviceStr2.Substring(0, deviceStr2.Length - 1);
                    string sql = "update AbpVideoEquips set IsOnlineValue=0 where VedioEqNumber in (" + deviceStr2 + ")";
                    SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                }
                if (!string.IsNullOrEmpty(deviceStr1))
                {
                    deviceStr1 = deviceStr1.Substring(0, deviceStr1.Length - 1);
                    param.Add("evt", 13);
                    param.Add("deviceState", 0);
                    param.Add("deviceSn", deviceStr1);
                    _VideoEqAppService.PostStateHighData(param);
                }
            }
            catch (Exception ex)
            {
                //记录日志
                CommonTools.WriteLogFile("TytBeatDataControl:Error" + ex.ToString());
            }
            return "ok";
        }
        /// <summary>
        /// 对接杭州鼎器数据
        /// </summary>
        [HttpPost]
        public Hashtable HZDQParkDataControl(object data)
        {
            Hashtable res = new Hashtable();
            JObject input = JsonConvert.DeserializeObject<JObject>(data.ToString());
            try
            {
                //停车数据
                if (Convert.ToString(input["cmd"])== "imgreport") {
                    VideoEqParkHighRequest dto = new VideoEqParkHighRequest();
                    if (Convert.ToInt32(input["direction"]) == 1)
                    {
                        dto.evt = 1;
                    }
                    if (Convert.ToInt32(input["direction"]) == 2)
                    {
                        dto.evt = 4;
                    }
                    dto.deviceSn = Convert.ToString(input["sn"]);
                    dto.parkingActId = Convert.ToString(input["id"]);
                    dto.berthCode = Convert.ToString(input["id"]);
                    dto.plateNumber = Convert.ToString(input["license"]);
                    dto.plateColor = 0;
                    if (Convert.ToInt32(input["colortype"]) == 2)
                    {
                        dto.plateColor = 1;
                    }
                    if (Convert.ToInt32(input["colortype"]) == 1)
                    {
                        dto.plateColor = 2;
                    }
                    dto.Trust = 90;
                    dto.happenTime = Convert.ToInt32(input["picdata"][0]["timeStamp"]["sec"]);
                    dto.actionCredible = Convert.ToInt32(input["confidence"]);
                    if (Convert.ToInt32(input["picnum"]) > 0)
                    {
                        dto.picUrl = "data:image/png;base64,"+Convert.ToString(input["picdata"][0]["imageFile"]);
                    }
                    if (dto.evt == 1 || dto.evt == 4)
                    {
                        _VideoEqAppService.PostVideoEqParkHighData(dto);
                    }
                }
                //心跳数据
                if (Convert.ToString(input["cmd"]) == "heartbeat") {
                    Hashtable dto = new Hashtable();
                    dto.Add("evt", 0);
                    dto.Add("deviceSn", input["sn"]);
                    dto.Add("deviceState", 0);
                    _VideoEqAppService.PostStateHighData(dto);
                }
                if (Convert.ToString(input["cmd"]) == "alarmreport")
                {
                    VideoEqParkHighRequest dto = new VideoEqParkHighRequest();
                    dto.evt = 16;
                    dto.deviceSn = Convert.ToString(input["sn"]);
                    dto.berthCode = Convert.ToString(input["id"]);
                    dto.happenTime = Convert.ToInt32(input["picdata"][0]["timeStamp"]["sec"]);
                    dto.parkingActId = Convert.ToString(input["id"]);
                    dto.picUrl = Convert.ToString(input["imageFile"]);
                    switch (Convert.ToInt32(input["alarmtype"]))
                    {
                        case 1:
                            dto.parkingAbnormalType = 9;
                            break;
                        case 3:
                            dto.parkingAbnormalType = 10;
                            break;
                        default: 
                            dto.parkingAbnormalType = 1024;
                            break;
                    }
                    _VideoEqAppService.PushFaultDataForHigh(dto);
                }
            }
            catch (Exception ex)
            {
                //记录日志
                CommonTools.WriteLogFile("HZDQParkDataControl:Error" + ex.ToString());
            }
            res.Add("cmd", input["cmd"]);
            res.Add("devid", input["devid"]);
            res.Add("id", input["id"]);
            res.Add("response", 1);
            return res;
        }
        /// <summary>
        /// 对接路通物联数据
        /// </summary>
        public Hashtable LTWLparkDataControl(object data) {
            Hashtable res = new Hashtable();
            try {
                JObject input = JsonConvert.DeserializeObject<JObject>(data.ToString());
                VideoEqParkHighRequest dto = new VideoEqParkHighRequest();
                //入场
                if (Convert.ToString(input["evt"]) == "evt.car.in")
                {
                    dto.evt = 1;
                    dto.plateNumber = Convert.ToString(input["plateNumber"]);
                    dto.happenTime = (Convert.ToDateTime(input["happenTime"]).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                    dto.berthCode = Convert.ToString(input["berthCode"]);
                    dto.deviceSn = Convert.ToString(input["deviceSn"]);
                    dto.actionCredible = Convert.ToInt32(input["actionCredible"]);
                    dto.parkingActId = Convert.ToString(input["parkingActId"]);
                    dto.plateNumberUrl = Convert.ToString(input["picUrlHphm"]);
                    dto.picUrl = Convert.ToString(input["picUrlIn"]);
                    dto.plateColor = Convert.ToInt32(input["plateColor"]);
                    _VideoEqAppService.PostVideoEqParkHighData(dto);
                }
                //出场
                if (Convert.ToString(input["evt"]) == "evt.car.out")
                {
                    dto.evt = 4;
                    dto.plateNumber = Convert.ToString(input["plateNumber"]);
                    dto.happenTime = (Convert.ToDateTime(input["happenTime"]).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                    dto.berthCode = Convert.ToString(input["berthCode"]);
                    dto.deviceSn = Convert.ToString(input["deviceSn"]);
                    dto.actionCredible = Convert.ToInt32(input["actionCredible"]);
                    dto.parkingActId = Convert.ToString(input["parkingActId"]);
                    dto.plateNumberUrl = Convert.ToString(input["picUrlHphm"]);
                    dto.picUrl = Convert.ToString(input["picUrlOut"]);
                    dto.plateColor = Convert.ToInt32(input["plateColor"]);
                    _VideoEqAppService.PostVideoEqParkHighData(dto);
                }
                //修正
                if (Convert.ToString(input["evt"]) == "evt.car.plateCorrect") {
                    dto.evt = 512;
                    dto.plateNumber = Convert.ToString(input["plateNumber"]);
                    dto.happenTime = (Convert.ToDateTime(input["timeIn"]).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                    dto.deviceSn = Convert.ToString(input["deviceSn"]);
                    dto.parkingActId = Convert.ToString(input["parkingActId"]);
                    dto.plateColor = Convert.ToInt32(input["plateColor"]);
                    string sql = "select BerthNumber from AbpVideoEquips where BerthNumber is not null and VedioEqNumber='" + dto.deviceSn + "'";
                    DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                    if (tables.Rows.Count > 0)
                    {
                        dto.berthCode = Convert.ToString(tables.Rows[0][0]);
                        _VideoEqAppService.pushFixDataForHigh(dto);
                    }
                }
                //状态
                if (Convert.ToString(input["evt"]) == "evt.device.status") {
                    foreach (JToken ele in input["deviceStatusList"])
                    {
                        Hashtable param = new Hashtable();
                        param["deviceSn"] = ele["deviceSn"];
                        param["deviceState"] = ele["deviceStatus"];
                        param["evt"] = 0;
                        _VideoEqAppService.PostStateHighData(param);
                    }
                }
                //泊位异常
                if (Convert.ToString(input["evt"]) == "evt.berth.alarm") {
                    dto.evt = 16;
                    dto.deviceSn = Convert.ToString(input["deviceSn"]);
                    var sql = "select BerthNumber from AbpVideoEquips where BerthNumber is not null and VedioEqNumber='" + dto.deviceSn + "'";
                    DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                    if (tables.Rows.Count > 0) {
                        dto.berthCode = Convert.ToString(tables.Rows[0][0]);
                        dto.parkingAbnormalType = 10;
                        dto.parkingActId = Convert.ToString(input["parkingActId"]);
                        dto.happenTime = (Convert.ToDateTime(input["happenTime"]).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                        _VideoEqAppService.PushFaultDataForHigh(dto);
                    }
                }
                //设备告警
                if (Convert.ToString(input["evt"]) == "evt.warning.device")
                {
                    dto.evt = 16;
                    dto.deviceSn = Convert.ToString(input["deviceSn"]);
                    var sql = "select BerthNumber from AbpVideoEquips where BerthNumber is not null and VedioEqNumber='" + dto.deviceSn + "'";
                    DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                    if (tables.Rows.Count > 0)
                    {
                        dto.berthCode = Convert.ToString(tables.Rows[0][0]);
                        dto.parkingAbnormalType = 11;
                        dto.parkingActId = Convert.ToString(input["evtGuid"]);
                        dto.happenTime = (Convert.ToDateTime(input["time"]).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                        _VideoEqAppService.PushFaultDataForHigh(dto);
                    }
                }
                //重位告警
                if (Convert.ToString(input["evt"]) == "evt.heavy.position")
                {
                    dto.evt = 16;
                    dto.deviceSn = Convert.ToString(input["deviceSn"]);
                    var sql = "select BerthNumber from AbpVideoEquips where BerthNumber is not null and VedioEqNumber='" + dto.deviceSn + "'";
                    DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                    if (tables.Rows.Count > 0)
                    {
                        dto.berthCode = Convert.ToString(tables.Rows[0][0]);
                        dto.parkingAbnormalType = 3;
                        dto.parkingActId = Convert.ToString(input["evtGuid"]);
                        dto.happenTime = (Convert.ToDateTime(input["happenTime"]).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                        _VideoEqAppService.PushFaultDataForHigh(dto);
                    }
                }
                //超长时间占用告警
                if (Convert.ToString(input["evt"]) == "evt.warning.outlimit") {
                    dto.evt = 16;
                    dto.deviceSn = Convert.ToString(input["deviceSn"]);
                    var sql = "select BerthNumber from AbpVideoEquips where BerthNumber is not null and VedioEqNumber='" + dto.deviceSn + "' and BerthNumber='"+ Convert.ToString(input["berthCode"]) + "'";
                    DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                    if (tables.Rows.Count > 0)
                    {
                        foreach (JToken ele in input["picUrlArr"])
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(ele["statusPhoto"]))) {
                                dto.picUrl = Convert.ToString(ele["statusPhoto"]);
                                break;
                            }
                        }
                        dto.berthCode = Convert.ToString(tables.Rows[0][0]);
                        dto.parkingAbnormalType = 12;
                        dto.parkingActId = Convert.ToString(input["parkingActId"]);
                        dto.happenTime = (Convert.ToDateTime(input["happenTime"]).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                        _VideoEqAppService.PushFaultDataForHigh(dto);
                    }
                }
                //车牌遮挡告警
                if (Convert.ToString(input["evt"]) == "evt.plate.shelter")
                {
                    dto.evt = 16;
                    dto.deviceSn = Convert.ToString(input["deviceSn"]);
                    var sql = "select BerthNumber from AbpVideoEquips where BerthNumber is not null and VedioEqNumber='" + dto.deviceSn + "' and BerthNumber='" + Convert.ToString(input["berthCode"]) + "'";
                    DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                    if (tables.Rows.Count > 0)
                    {
                        dto.picUrl = Convert.ToString(input["picUrlOut"]);
                        dto.berthCode = Convert.ToString(tables.Rows[0][0]);
                        dto.parkingAbnormalType = 2;
                        dto.parkingActId = Convert.ToString(input["parkingActId"]);
                        dto.happenTime = (Convert.ToDateTime(input["happenTime"]).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                        dto.picUrl= Convert.ToString(input["picUrlOut"]);
                        _VideoEqAppService.PushFaultDataForHigh(dto);
                    }
                }
                //确定出场告警
                if (Convert.ToString(input["evt"]) == "evt.determine.carout")
                {
                    dto.evt = 16;
                    dto.deviceSn = Convert.ToString(input["deviceSn"]);
                    var sql = "select BerthNumber from AbpVideoEquips where BerthNumber is not null and VedioEqNumber='" + dto.deviceSn + "'";
                    DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                    if (tables.Rows.Count > 0)
                    {
                        dto.berthCode = Convert.ToString(tables.Rows[0][0]);
                        dto.parkingAbnormalType = 13;
                        dto.parkingActId = Convert.ToString(input["parkingActId"]);
                        dto.happenTime = (Convert.ToDateTime(input["happenTime"]).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                        _VideoEqAppService.PushFaultDataForHigh(dto);

                    }
                }
                //疑似识别错误告警
                if (Convert.ToString(input["evt"]) == "evt.platenumber.unsure")
                {
                    dto.evt = 16;
                    dto.deviceSn = Convert.ToString(input["deviceSn"]);
                    var sql = "select BerthNumber from AbpVideoEquips where BerthNumber is not null and VedioEqNumber='" + dto.deviceSn + "'";
                    DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                    if (tables.Rows.Count > 0)
                    {
                        dto.berthCode = Convert.ToString(tables.Rows[0][0]);
                        dto.parkingAbnormalType = 14;
                        dto.parkingActId = Convert.ToString(input["parkingActId"]);
                        dto.happenTime = (Convert.ToDateTime(input["happenTime"]).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                        _VideoEqAppService.PushFaultDataForHigh(dto);
                    }
                }
                //抓拍事件推送
                if (Convert.ToString(input["evt"]) == "evt.scene.capture") {
                    dto.deviceSn = Convert.ToString(input["deviceSn"]);
                    var sql = "select BerthNumber from AbpVideoEquips where BerthNumber is not null and VedioEqNumber='" + dto.deviceSn + "'";
                    DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                    if (tables.Rows.Count > 0) {
                        dto.berthCode = Convert.ToString(tables.Rows[0][0]);
                        dto.picUrl = Convert.ToString(input["captureImgUrl"]);
                        dto.happenTime = (Convert.ToDateTime(input["captureTime"]).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                        _VideoEqAppService.pushCaptureForHigh(dto);
                    }
                }
            }
            catch (Exception ex)
            {
                //记录日志
                CommonTools.WriteLogFile("LTWLparkDataControl:Error" + ex.ToString());
            }
            res.Add("errorcode", "0");
            res.Add("message", "处理成功");
            return res;
        }
        /// <summary>
        /// 对接路通物联抓拍数据查询
        /// </summary>
        public object LTWLcaptureQuery(string deviceCode, string pointTime,string direction,int imgsCount) {
            try {
                Commons com = new Commons();
                Hashtable param = new Hashtable();
                param.Add("deviceCode", deviceCode);
                param.Add("pointTime", pointTime);
                param.Add("direction", direction);
                param.Add("imgsCount", imgsCount);
                string url = ConfigurationManager.AppSettings["LTWLcaptureUrl"].ToString() + "/parking_api_entry/MchApi/query/getImgsWithTimePoint.htm";
                return com.HttpPostNew(url, JsonConvert.SerializeObject(param));
            }
            catch (Exception ex)
            {
                Hashtable res = new Hashtable();
                res.Add("result","false");
                res.Add("msg", ex.ToString());
                return res;
            }
        }   
    }
}