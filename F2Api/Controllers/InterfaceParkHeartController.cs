using CommonTool;
using F2.Application.Parkade;
using F2.Application.Parkade.Dtos;
using F2.Common;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Http.Results;
using System.Web.Mvc;
using static CommonTool.Commons;

namespace F2Api.Controllers
{
    public class InterfaceParkHeartController : Controller
    {
        #region Define
        private readonly IParkadeService _ParkadeService;
        #endregion

        public InterfaceParkHeartController()
        {
            _ParkadeService = new ParkadeService();
        }
        /// <summary>
        /// 心跳数据接收
        /// </summary>
        /// <param name="dto"></param>
        public ActionResult HeartBeatReceive(FormCollection dto)
        {
            ParkadeRequest req = new ParkadeRequest();
            req.serialno = dto["serialno"].ToString();
            Hashtable result=_ParkadeService.HeartBeatReceive(req);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //
        public ActionResult HeartBeatReceive2(FormCollection dto) {
            Commons com = new Commons();
            string[] show= { "粤B12345", "请您缴费", "100元" };
            var bytes = com.ScreenShow(show);
            byte[] bytes1 = { 0x00, 0x64, 0xFF, 0xFF, 0x0F, 0x06, 0x00, 0x01, 0x00, 0x00, 0x00, 0x05, 0x24, 0xB7 };
            Hashtable result = new Hashtable();
            Hashtable Response = new Hashtable();
            Hashtable serialData = new Hashtable();
            List<Hashtable> LserialData = new List<Hashtable>();
            serialData.Add("serialChannel", 0);
            serialData.Add("data", Convert.ToBase64String(bytes1));
            serialData.Add("dataLen", bytes1.Length);
            LserialData.Add(serialData);
            Response.Add("info", "ok");
            Response.Add("serialData", LserialData);
            result.Add("Response_AlarmInfoPlate", Response);
            CommonTools.WriteLogFile("HeartBeatReceive:Error:" + dto);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}