using F2.Application.PDA;
using F2.Application.PDA.Dtos;
using F2.Core.Extensions;
using F2.Core.Extensions.Models;
using F2.Core.Extensions.WebMvc;
using F2Api.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using System.Web;
using System.Xml.Linq;
using F2.Core.Extensions.Log;
using System.Xml.Serialization;
using System.Text;
using System.Collections;

namespace F2Api.Controllers
{
    /// <summary>
    /// 移动pos接口
    /// </summary>
    public class InterfacePDAInspectorsController : ApiController
    {
        #region Var
        private readonly IBerthsecAppService _berthsecAppService;
        private readonly ITenantAppService _tenantAppService;
        private readonly IPDAAppService _pdaAppService;
        private readonly IPDAInspectorsService _pdaInspectorsService;
        //private readonly IKafkaNetAppService _kafkaNetAppService;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public InterfacePDAInspectorsController()
        {
            _berthsecAppService = new BerthsecAppService();
            _tenantAppService = new TenantAppService();
            _pdaAppService = new PDAAppService();
            _pdaInspectorsService = new PDAInspectorsService();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public object CheckInspectorsLoginByDeviceType([FromUri] EmployeeLoginInput input)
        {
            var tenantDto = _tenantAppService.GetTenantInfo(input.tenancyName);
            if (tenantDto == null)
            {
                return Json(new AjaxResponse(new ErrorInfo() { Code = 7, Message = "未找到此商户: " + input.tenancyName }));
            }
            if (!tenantDto.IsActive)
            {
                return Json(new AjaxResponse(new ErrorInfo() { Code = 8, Message = "商户组织被锁定: " + tenantDto.Name }));
            }

            var loginResult = _pdaInspectorsService.InspectorsLogin(input, tenantDto.Id);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    break;
                case AbpLoginResultType.InvalidUserNameOrEmailAddress:
                case AbpLoginResultType.InvalidPassword:
                    return Json(new AjaxResponse(new ErrorInfo() { Code = 1, Message = "用户名验证错误!" }));
                case AbpLoginResultType.InvalidTenancyName:
                    return Json(new AjaxResponse(new ErrorInfo() { Code = 2, Message = "未找到此商户!" }));
                case AbpLoginResultType.TenantIsNotActive:
                    return Json(new AjaxResponse(new ErrorInfo() { Code = 3, Message = "企业账号被锁定!" }));
                case AbpLoginResultType.UserIsNotActive:
                    return Json(new AjaxResponse(new ErrorInfo() { Code = 4, Message = "用户被锁定!" }));
                case AbpLoginResultType.UserEmailIsNotConfirmed:
                    return Json(new AjaxResponse(new ErrorInfo() { Code = 5, Message = "您的邮件地址为确认!" }));
                case AbpLoginResultType.EmployeeIsCheck:
                    return Json(new AjaxResponse(new ErrorInfo() { Code = 9, Message = "您已经登录过了！" }));
                case AbpLoginResultType.EquipmentIsNotActive:
                    return Json(new AjaxResponse(new ErrorInfo() { Code = 10, Message = input.DeviceCode + "设备未启用！" }));
                case AbpLoginResultType.PDAbindUser:
                    return Json(new AjaxResponse(new ErrorInfo() { Code = 10, Message = input.DeviceCode + "设备与用户未绑定！" }));
                default:
                    return Json(new AjaxResponse(new ErrorInfo() { Code = 6, Message = "未知错误的登录!" }));
            }
            var berthsecs = _pdaInspectorsService.GetBerthsecList(loginResult.User.Id, tenantDto.Id);
            return Json(new AjaxResponse(berthsecs));
        }
        /// <summary>
        /// 用户登录交互token
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetInspectorsLoginToken([FromBody] LoginTokenInput input)
        {
            return _pdaInspectorsService.LoginToken(input);
        }
        /// <summary>
        /// pda下载参数
        /// </summary>
        /// <param name="access_token">Token认证</param>
        /// <returns></returns>
        [HttpGet]
        public object DownParameterForInspectors(string access_token)
        {
            return Json(new AjaxResponse(_pdaInspectorsService.DownParameterForInspectors(access_token)));
        }
        /// <summary>
        /// 巡查员签退
        /// </summary>
        /// <param name="berthsecid"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        [HttpGet]
        public object InspectorsOutLineLogout([FromUri] string berthsecid, string access_token)
        {
            return Json(new AjaxResponse(_pdaInspectorsService.InspectorsOutLineLogout(berthsecid, access_token)));
        }
        /// <summary>
        /// 巡查员获取停车记录
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public object GetStopCarList([FromBody] Hashtable input)
        {
            return Json(new AjaxResponse(_pdaInspectorsService.GetStopCarList(input)));
        }
        /// <summary>
        /// 巡查员实收合计
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public object GetAllFeeList([FromBody] Hashtable input)
        {
            return Json(new AjaxResponse(_pdaInspectorsService.GetAllFeeList(input)));
        }
        /// <summary>
        /// 巡查员欠费记录
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public object GetArrearageData([FromBody] Hashtable input)
        {
            return Json(new AjaxResponse(_pdaInspectorsService.GetArrearageData(input)));
        }
        /// <summary>
        /// 巡查员设置信息
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public object GetInspectorsInfo([FromBody] Hashtable input)
        {
            return Json(new AjaxResponse(_pdaInspectorsService.GetInspectorsInfo(input)));
        }
        /// <summary>
        /// 巡查员泊位管理
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public object GetIBerthInfo([FromBody] Hashtable input)
        {
            return Json(new AjaxResponse(_pdaInspectorsService.GetIBerthInfo(input)));
        }
        /// <summary>
        /// 巡查员修改密码
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public object ModifyPassword([FromBody] Hashtable input)
        {
            return Json(new AjaxResponse(_pdaInspectorsService.ModifyPassword(input)));
        }
        /// <summary>
        /// 巡查员任务上传
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        [DisableAuditing]
        public object InsertTaskFeedbacks()
        {
            
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;
            string BerthsecId = request.Form.GetValues("BerthsecId")?.FirstOrDefault();
            string access_token = request.Form.GetValues("access_token")?.FirstOrDefault();
            string BerthNumber = request.Form.GetValues("BerthNumber")?.FirstOrDefault();
            string Remark = request.Form.GetValues("Remark")?.FirstOrDefault();
            string TaskId = request.Form.GetValues("TaskId")?.FirstOrDefault();
            byte[] PicUrl1 = null;
            byte[] PicUrl2 = null;
            byte[] PicUrl3 = null;
            for (int i=0;i< request.Files.Count;i++) {
                if (i==0) {
                    var imageFile = request.Files[0];
                    var stream = imageFile.InputStream;
                    PicUrl1 = new byte[stream.Length];
                    stream.Read(PicUrl1, 0, PicUrl1.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                }
                if (i == 1)
                {
                    var imageFile = request.Files[1];
                    var stream = imageFile.InputStream;
                    PicUrl2 = new byte[stream.Length];
                    stream.Read(PicUrl2, 0, PicUrl2.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                }
                if (i == 2)
                {
                    var imageFile = request.Files[2];
                    var stream = imageFile.InputStream;
                    PicUrl3 = new byte[stream.Length];
                    stream.Read(PicUrl3, 0, PicUrl3.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                }
            }
            return Json(new AjaxResponse(_pdaInspectorsService.InsertTaskFeedbacks(access_token,TaskId,BerthsecId, BerthNumber, Remark, PicUrl1, PicUrl2, PicUrl3)));
        }
        /// <summary>
        /// 巡查员事件上传
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        [DisableAuditing]
        public object InsertInspectorsEvent()
        {
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            HttpRequestBase request = context.Request;
            string BerthsecId = request.Form.GetValues("BerthsecId")?.FirstOrDefault();
            string access_token = request.Form.GetValues("access_token")?.FirstOrDefault();
            string BerthNumber = request.Form.GetValues("BerthNumber")?.FirstOrDefault();
            string EventContent = request.Form.GetValues("EventContent")?.FirstOrDefault();
            byte[] PicUrl = null;
            for (int i = 0; i < request.Files.Count; i++)
            {
                if (i == 0)
                {
                    var imageFile = request.Files[0];
                    var stream = imageFile.InputStream;
                    PicUrl = new byte[stream.Length];
                    stream.Read(PicUrl, 0, PicUrl.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                }
            }
            return Json(new AjaxResponse(_pdaInspectorsService.InsertInspectorsEvent(access_token, BerthsecId, BerthNumber, EventContent, PicUrl)));
        }
        /// <summary>
        /// 巡查员任务表查询
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public object GetInspectorTasks([FromBody] Hashtable input)
        {
            return Json(new AjaxResponse(_pdaInspectorsService.GetInspectorTasks(input)));
        }
        /// <summary>
        /// 巡查员事件表查询
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public object GetInspectorEvent([FromBody] Hashtable input)
        {
            return Json(new AjaxResponse(_pdaInspectorsService.GetInspectorEvent(input)));
        }
        /// <summary>
        /// 巡查员事件回复表查询
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public object GetInspectorTaskFeedbacks([FromBody] Hashtable input)
        {
            return Json(new AjaxResponse(_pdaInspectorsService.GetInspectorTaskFeedbacks(input)));
        }
        /// <summary>
        /// 上传gps定位位置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        [HttpGet]
        [DisableAuditing]
        public object UpdateInspectorGps([FromUri] string x, string y, string access_token)
        {
            return Json(new AjaxResponse(_pdaInspectorsService.UpdateInspectorGps(x, y, access_token)));
        }
        /// <summary>
        /// 巡查员任务条数
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public object GetInspectorsTaskNum([FromBody] Hashtable input)
        {
            return Json(new AjaxResponse(_pdaInspectorsService.GetInspectorsTaskNum(input)));
        }

        /// <summary>
        /// 上传pda数据库文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [DisableAuditing]
        public object UploadInspSqliteDB()
        {
            //获取传统context
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            //定义传统request对象
            HttpRequestBase request = context.Request;
            var InspectorId = request.Form.GetValues("InspectorId").FirstOrDefault() ?? DateTime.Now.ToString("yyyyMMdd");
            string access_token = request.Form.GetValues("access_token").FirstOrDefault() ?? Guid.NewGuid().ToString();
            if (request.Files.Count == 0)
                return Json(new AjaxResponse("true"));
            var stream = request.Files[0].InputStream;
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);

            if (Directory.Exists(HostingEnvironment.MapPath("~\\sqlitedbInsp\\" + DateTime.Now.ToString("yyyy-MM-dd"))) == false)
            {
                Directory.CreateDirectory(HostingEnvironment.MapPath("~\\sqlitedbInsp\\" + DateTime.Now.ToString("yyyy-MM-dd")));
            }
            File.WriteAllBytes(HostingEnvironment.MapPath("~\\sqlitedbInsp\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\" + InspectorId + "&" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".db"), bytes);
            GC.Collect();
            return Json(new AjaxResponse("true"));
        }
        /// <summary>
        /// 检测软件版本
        /// </summary>
        /// <param name="OldVersion"></param>
        /// <param name="PDA"></param>
        /// <param name="Type"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        [HttpGet]
        public object CheckInspVersion([FromUri] string OldVersion,string PDA,int Type, string access_token)
        {
            return Json(new AjaxResponse(_pdaInspectorsService.CheckInspVersion(OldVersion, PDA, Type, access_token)));
        }
        /// <summary>
        /// 巡查员进场车辆、离场车辆、补交车辆打印查询
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public object GetTypeCarDataList([FromBody] Hashtable input)
        {
            return Json(new AjaxResponse(_pdaInspectorsService.GetTypeCarDataList(input)));
        }
        /// <summary>
        /// 巡查员进场车辆、离场车辆、补交车辆数量查询
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public object GetTypeCarDataCount([FromBody] Hashtable input)
        {
            return Json(new AjaxResponse(_pdaInspectorsService.GetTypeCarDataCount(input)));
        }
    }
}
