using CommonTool;
using F2.Application.PatrolCar;
using F2.Application.PatrolCar.Dtos;
using F2.Application.VideoEqs;
using F2.Application.VideoEqs.Dtos;
using F2.Common;
using F2.Core.Extensions;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
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
    public class InterfacePatrolCarController : ApiController
    {
        #region Define
        private readonly IPatrolCarService _IPatrolCarService;
        #endregion

        public InterfacePatrolCarController()
        {
            _IPatrolCarService = new PatrolCarService();
        }
        //臻识巡检车对接
        [HttpPost]
        public PatrolCarResponse ZhenshiInterface(PatrolCarRequest dto)
        {
            PatrolCarResponse res = new PatrolCarResponse();
            try
            {
                /*string sql = "select Url,Trust,IsPost from AbpParamConfig";
                DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                if (tables.Rows.Count > 0) {
                    dto.Trust = Convert.ToInt32(tables.Rows[0][1]);//置信度
                }*/
                dto.Trust = 0;
                if (dto.event_type == 1 || dto.event_type == 4 || dto.event_type == 512)
                {
                    _IPatrolCarService.PatrolCarStopData(dto);
                }
                if (dto.event_type == 20 || dto.event_type == 21 || dto.event_type == 22 || dto.event_type == 23 || dto.event_type == 24 || dto.event_type == 25 || dto.event_type == 26)
                {
                    _IPatrolCarService.PatrolCarFaultData(dto);
                }
                if (dto.event_type == 0)
                {
                    _IPatrolCarService.PatrolCarStateData(dto);
                }
            }
            catch (Exception ex)
            {
                CommonTools.WriteLogFile("ZhenshiInterface:"+ex.ToString());
            }
            res.errorcode = 0;
            res.message="";
            return res;
        }
        //路通物联出入场数据对接
        [HttpPost]
        public PatrolCarResponse LTWLcarPassDataControl(object data) {
            PatrolCarResponse res = new PatrolCarResponse();
            try {
                JObject input = JsonConvert.DeserializeObject<JObject>(data.ToString());
                if (Convert.ToString(input["deviceType"])=="9") {
                    PatrolCarRequest dto = new PatrolCarRequest();
                    PlateInfo plate = new PlateInfo();
                    List<ImagesInfo> img = new List<ImagesInfo>();
                    dto.guid = Convert.ToString(input["guid"]);
                    dto.berth_confidence = Convert.ToInt32(input["actionCredible"]);
                    plate.confidence= Convert.ToInt32(input["plateCredible"]);
                    dto.occur_time = (Convert.ToDateTime(input["inoutTime"]).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                    dto.sn = Convert.ToString(input["sn"]);
                    dto.berth_name = Convert.ToString(input["deviceBerthCode"]);
                    plate.plate_number = Convert.ToString(input["plateNumber"]);
                    plate.plate_type= Convert.ToInt32(input["plateColor"]);
                    plate.plate_color= Convert.ToInt32(input["plateColor"]);
                    dto.images = img;
                    dto.plate = plate;

                    if (Convert.ToString(input["plateColor"]) == "2")
                    {
                        dto.plate.plate_type = 3;
                    }
                    else {
                        dto.plate.plate_type = 1;
                    }
                    if (Convert.ToString(input["inoutType"])=="1")
                    {
                        dto.event_type = 1;
                    }
                    if (Convert.ToString(input["inoutType"]) == "0")
                    {
                        dto.event_type = 4;
                    }
                    _IPatrolCarService.PatrolCarStopData(dto);
                }

            }
            catch (Exception ex)
            {
                CommonTools.WriteLogFile("LTWLInterface:" + ex.ToString());
            }
            res.errorcode = 0;
            res.message = "";
            return res;
        }
        //路通物联状态数据对接
        [HttpPost]
        public PatrolCarResponse LTWLstatusDataControl(object data)
        {
            PatrolCarResponse res = new PatrolCarResponse();
            try
            {
                JObject input = JsonConvert.DeserializeObject<JObject>(data.ToString());
                if (Convert.ToString(input["deviceType"]) == "9")
                {
                    PatrolCarRequest dto = new PatrolCarRequest();
                    dto.sn = Convert.ToString(input["sn"]);
                    if (Convert.ToInt32(input["deviceStatus"]) == 1)
                    {
                        dto.event_type = 0;
                        dto.device_state = 0;
                        _IPatrolCarService.PatrolCarStateData(dto);
                    }
                    else {
                        dto.event_type = 0;
                        dto.device_state = 1;
                        _IPatrolCarService.PatrolCarStateData(dto);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonTools.WriteLogFile("LTWLInterface:" + ex.ToString());
            }
            res.errorcode = 0;
            res.message = "";
            return res;
        }
        //路通物联位置数据对接
        [HttpPost]
        public PatrolCarResponse LTWLgeoDataControl(object data)
        {
            PatrolCarResponse res = new PatrolCarResponse();
            try
            {
                JObject input = JsonConvert.DeserializeObject<JObject>(data.ToString());
                if (Convert.ToString(input["deviceType"]) == "9")
                {
                    PatrolCarRequest dto = new PatrolCarRequest();
                    dto.sn = Convert.ToString(input["sn"]);
                    dto.X = Convert.ToString(input["latitude"]);
                    dto.Y = Convert.ToString(input["longitude"]);
                    _IPatrolCarService.PatrolCarStationData(dto);
                }
            }
            catch (Exception ex)
            {
                CommonTools.WriteLogFile("LTWLInterface:" + ex.ToString());
            }
            res.errorcode = 0;
            res.message = "";
            return res;
        }
        //路通物联数据对接
        public Hashtable LTWLparkDataControl(object data) {
            Hashtable res = new Hashtable();
            try
            {
                JObject input = JsonConvert.DeserializeObject<JObject>(data.ToString());
                PatrolCarRequest dto = new PatrolCarRequest();
                PlateInfo plate = new PlateInfo();
                List<ImagesInfo> imgList = new List<ImagesInfo>();
                ImagesInfo img = new ImagesInfo();

                //入场
                if (Convert.ToString(input["evt"]) == "evt.car.in")
                {
                    dto.event_type = 1;
                    dto.guid = Convert.ToString(input["parkingActId"]);
                    dto.berth_confidence = Convert.ToInt32(input["actionCredible"]);
                    plate.confidence= Convert.ToInt32(input["plateCredible"]);
                    dto.occur_time = (Convert.ToDateTime(input["happenTime"]).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                    dto.sn = Convert.ToString(input["deviceSn"]);
                    dto.berth_name = Convert.ToString(input["berthCode"]);
                    plate.plate_number = Convert.ToString(input["plateNumber"]);
                    plate.plate_type= Convert.ToInt32(input["plateColor"]);
                    plate.plate_color= Convert.ToInt32(input["plateColor"]);
                    ImagesInfo imgA = new ImagesInfo();
                    ImagesInfo imgB = new ImagesInfo();
                    imgA.image_type = 1;
                    imgA.image_url= Convert.ToString(input["picUrlIn"]);
                    imgList.Add(imgA);
                    imgB.image_type = 2;
                    imgB.image_url = Convert.ToString(input["picUrlHphm"]);
                    imgList.Add(imgB);
                    dto.images = imgList;
                    dto.plate = plate;
                    if (Convert.ToString(input["plateColor"]) == "2")
                    {
                        dto.plate.plate_type = 3;
                    }
                    else {
                        dto.plate.plate_type = 1;
                    }
                    _IPatrolCarService.PatrolCarStopData(dto);
                }
                //出场
                if (Convert.ToString(input["evt"]) == "evt.car.out")
                {
                    dto.event_type = 4;
                    dto.guid = Convert.ToString(input["parkingActId"]);
                    dto.berth_confidence = Convert.ToInt32(input["actionCredible"]);
                    plate.confidence = Convert.ToInt32(input["plateCredible"]);
                    dto.occur_time = (Convert.ToDateTime(input["happenTime"]).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                    dto.sn = Convert.ToString(input["deviceSn"]);
                    dto.berth_name = Convert.ToString(input["berthCode"]);
                    plate.plate_number = Convert.ToString(input["plateNumber"]);
                    plate.plate_type = Convert.ToInt32(input["plateColor"]);
                    plate.plate_color = Convert.ToInt32(input["plateColor"]);
                    ImagesInfo imgA = new ImagesInfo();
                    ImagesInfo imgB = new ImagesInfo();
                    imgA.image_type = 1;
                    imgA.image_url = Convert.ToString(input["picUrlOut"]);
                    imgList.Add(imgA);
                    imgB.image_type = 2;
                    imgB.image_url = Convert.ToString(input["picUrlHphm"]);
                    imgList.Add(imgB);
                    dto.images = imgList;
                    dto.plate = plate;
                    if (Convert.ToString(input["plateColor"]) == "2")
                    {
                        dto.plate.plate_type = 3;
                    }
                    else
                    {
                        dto.plate.plate_type = 1;
                    }
                    _IPatrolCarService.PatrolCarStopData(dto);
                }
                //状态
                if (Convert.ToString(input["evt"]) == "evt.device.status")
                {
                    foreach (JToken ele in input["deviceStatusList"])
                    {
                        Hashtable param = new Hashtable();
                        dto.sn = Convert.ToString(ele["deviceSn"]);
                        dto.event_type = 0;
                        if (Convert.ToString(ele["deviceStatus"]) == "0")
                        {
                            dto.device_state = 0;
                        }
                        else {
                            dto.device_state = 1;
                        }
                        _IPatrolCarService.PatrolCarStateData(dto);
                    }
                }
                //修正
                if (Convert.ToString(input["evt"]) == "evt.car.plateCorrect")
                {
                    dto.event_type = 512;
                    dto.plate.plate_number = Convert.ToString(input["plateNumber"]);
                    dto.sn = Convert.ToString(input["deviceSn"]);
                    dto.guid = Convert.ToString(input["parkingActId"]);
                    _IPatrolCarService.PatrolCarStopData(dto);
                }
                // 泊位异常
                if (Convert.ToString(input["evt"]) == "evt.berth.alarm")
                {
                    dto.event_type = 19;
                    dto.sn = Convert.ToString(input["deviceSn"]);
                    var sql = "select b.BerthNumber from AbpPatrolCarBerths a,AbpBerths b where a.BerthsId=b.Id and a.PatrolCarNumber='" + dto.sn + "'";
                    DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                    if (tables.Rows.Count > 0)
                    {
                        dto.berth_name = Convert.ToString(tables.Rows[0][0]);
                        img.image_type = 0;
                        imgList.Add(img);
                        dto.images = imgList;
                        dto.occur_time = (Convert.ToDateTime(input["happenTime"]).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                        _IPatrolCarService.PatrolCarFaultData(dto);
                    }
                }
                //设备告警
                if (Convert.ToString(input["evt"]) == "evt.warning.device") {
                    dto.event_type = 18;
                    dto.sn = Convert.ToString(input["deviceSn"]);
                    var sql = "select b.BerthNumber from AbpPatrolCarBerths a,AbpBerths b where a.BerthsId=b.Id and a.PatrolCarNumber='" + dto.sn + "'";
                    DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                    if (tables.Rows.Count > 0)
                    {
                        dto.berth_name = Convert.ToString(tables.Rows[0][0]);
                        img.image_type = 0;
                        imgList.Add(img);
                        dto.images = imgList;
                        dto.occur_time = (Convert.ToDateTime(input["happenTime"]).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                        _IPatrolCarService.PatrolCarFaultData(dto);
                    }
                }
                //重位告警
                if (Convert.ToString(input["evt"]) == "evt.heavy.position") {
                    dto.event_type = 25;
                    dto.sn = Convert.ToString(input["deviceSn"]);
                    var sql = "select b.BerthNumber from AbpPatrolCarBerths a,AbpBerths b where a.BerthsId=b.Id and a.PatrolCarNumber='" + dto.sn + "'";
                    DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                    if (tables.Rows.Count > 0)
                    {
                        dto.berth_name = Convert.ToString(tables.Rows[0][0]);
                        img.image_type = 0;
                        imgList.Add(img);
                        dto.images = imgList;
                        dto.occur_time = (Convert.ToDateTime(input["happenTime"]).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                        _IPatrolCarService.PatrolCarFaultData(dto);
                    }
                }
                //超长时间占用告警
                if (Convert.ToString(input["evt"]) == "evt.warning.outlimit") {
                    dto.event_type = 24;
                    dto.sn = Convert.ToString(input["deviceSn"]);
                    var sql = "select b.BerthNumber from AbpPatrolCarBerths a,AbpBerths b where a.BerthsId=b.Id and a.PatrolCarNumber='" + dto.sn + "'";
                    DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                    if (tables.Rows.Count > 0)
                    {
                        dto.berth_name = Convert.ToString(tables.Rows[0][0]);
                        img.image_type = 0;
                        imgList.Add(img);
                        dto.images = imgList;
                        dto.occur_time = (Convert.ToDateTime(input["happenTime"]).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                        _IPatrolCarService.PatrolCarFaultData(dto);
                    }
                }
                //车牌遮挡告警
                if (Convert.ToString(input["evt"]) == "evt.plate.shelter") {
                    dto.event_type = 17;
                    dto.sn = Convert.ToString(input["deviceSn"]);
                    var sql = "select b.BerthNumber from AbpPatrolCarBerths a,AbpBerths b where a.BerthsId=b.Id and a.PatrolCarNumber='" + dto.sn + "'";
                    DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                    if (tables.Rows.Count > 0)
                    {
                        dto.berth_name = Convert.ToString(tables.Rows[0][0]);
                        img.image_type = 0;
                        imgList.Add(img);
                        dto.images = imgList;
                        dto.occur_time = (Convert.ToDateTime(input["happenTime"]).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                        _IPatrolCarService.PatrolCarFaultData(dto);
                    }
                }
                //确定出场告警
                if (Convert.ToString(input["evt"]) == "evt.determine.carout") {
                    dto.event_type = 16;
                    dto.sn = Convert.ToString(input["deviceSn"]);
                    var sql = "select b.BerthNumber from AbpPatrolCarBerths a,AbpBerths b where a.BerthsId=b.Id and a.PatrolCarNumber='" + dto.sn + "'";
                    DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                    if (tables.Rows.Count > 0)
                    {
                        dto.berth_name = Convert.ToString(tables.Rows[0][0]);
                        img.image_type = 0;
                        imgList.Add(img);
                        dto.images = imgList;
                        dto.occur_time = (Convert.ToDateTime(input["happenTime"]).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                        _IPatrolCarService.PatrolCarFaultData(dto);
                    }
                }
                //疑似识别错误告警
                if (Convert.ToString(input["evt"]) == "evt.platenumber.unsure") {
                    dto.event_type = 15;
                    dto.sn = Convert.ToString(input["deviceSn"]);
                    var sql = "select b.BerthNumber from AbpPatrolCarBerths a,AbpBerths b where a.BerthsId=b.Id and a.PatrolCarNumber='" + dto.sn + "'";
                    DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                    if (tables.Rows.Count > 0)
                    {
                        dto.berth_name = Convert.ToString(tables.Rows[0][0]);
                        img.image_type = 0;
                        imgList.Add(img);
                        dto.images = imgList;
                        dto.occur_time = (Convert.ToDateTime(input["happenTime"]).ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                        _IPatrolCarService.PatrolCarFaultData(dto);
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
    }
}