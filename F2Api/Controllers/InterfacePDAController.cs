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

namespace F2Api.Controllers
{
    /// <summary>
    /// 移动pos接口
    /// </summary>
    public class InterfacePDAController : ApiController
    {
        #region Var
        private readonly IEmployeeAppService _employeeAppService;
        private readonly IBerthsecAppService _berthsecAppService;
        private readonly ITenantAppService _tenantAppService;
        private readonly IPDAAppService _pdaAppService;
        //private readonly IKafkaNetAppService _kafkaNetAppService;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public InterfacePDAController()
        {
            _employeeAppService = new EmployeeAppService();
            _berthsecAppService = new BerthsecAppService();
            _tenantAppService = new TenantAppService();
            _pdaAppService = new PDAAppService();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public object CheckEmployeeLoginByDeviceType([FromUri] EmployeeLoginInput input)
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

            var loginResult = _employeeAppService.EmployeeLogin(input, tenantDto.Id);

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
            var berthsecs = _berthsecAppService.GetBerthsecList(loginResult.User.Id, tenantDto.Id);
            return Json(new AjaxResponse(berthsecs));
        }

        /// <summary>
        /// 用户登录交互token
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetLoginToken([FromBody]LoginTokenInput input)
        {
            return _employeeAppService.LoginToken(input);
        }

        /// <summary>
        /// pda下载参数
        /// </summary>
        /// <param name="access_token">Token认证</param>
        /// <returns></returns>
        [HttpGet]
        public object DownParameter(string access_token)
        {
            return Json(new AjaxResponse(_pdaAppService.DownParameter(access_token)));
        }

        /// <summary>
        /// 车辆进场
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public object InsertCarInParking([FromUri]CarinParkingDto input)
        {
            ErrorInfo error = _pdaAppService.InsertCarInParking(input);
            if (error.Code > 0)
                return Json(new AjaxResponse(error, false));
            return Json(new AjaxResponse(true));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public object InsertCarInParkingRemedy([FromUri]CarinParkingDto input)
        {
            ErrorInfo error = _pdaAppService.InsertCarInParkingRemedy(input);
            if (error.Code > 0)
                return Json(new AjaxResponse(error, false));
            return Json(new AjaxResponse(true));
        }

        /// <summary>
        /// 车辆出场
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public object InsertCarOutParking([FromUri]CaroutParkingDto input)
        {
            ErrorInfo error = _pdaAppService.InsertCarOutParking(input);
            if (error.Code > 0)
                return Json(new AjaxResponse(error, false));
            return Json(new AjaxResponse(true));
        }

        /// <summary>
        /// 车辆出场
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public object InsertCarOutParkingRemedy([FromUri]CaroutParkingDto input)
        {
            ErrorInfo error = _pdaAppService.InsertCarOutParkingRemedy(input);
            if (error.Code > 0)
                return Json(new AjaxResponse(error, false));
            return Json(new AjaxResponse(true));
        }




        /// <summary>
        /// 车辆出场
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public object InsertOtherCarOut([FromUri]CaroutParkingDto input)
        {
            ErrorInfo error = _pdaAppService.InsertCarOutParking(input);
            if (error.Code > 0)//写入出场标记
            {
                return Json(new AjaxResponse(error, false));
            }
            _pdaAppService.InsertRemoteGuid(input.guid, input.BerthsecID, input.access_token);
            return Json(new AjaxResponse(true));
        }

        /// <summary>
        /// 欠费追缴
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public object UpdateFeeBack([FromUri]FeeBackDto input)
        {
            ErrorInfo error = _pdaAppService.UpdateFeeBack(input);
            if (error.Code > 0)
                return Json(new AjaxResponse(error, false));
            return Json(new AjaxResponse(true));
        }

        /// <summary>
        /// 欠费批量追缴
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public object UpdateAllFeeBack([FromUri]FeeBackDto input)
        {
            ErrorInfo error = _pdaAppService.UpdateAllFeeBack(input);
            if (error.Code > 0)
                return Json(new AjaxResponse(error, false));
            return Json(new AjaxResponse(true));
        }


        /// <summary>
        /// 查询在停订单
        /// </summary>
        /// <param name="jsonstr"></param>
        /// <returns></returns>
        [HttpGet]
        public object QueryStopOrder([FromUri] string jsonstr)
        {
            return Json(new AjaxResponse(_berthsecAppService.SearchStopOrder(jsonstr)));
        }



        /// <summary>
        /// 查询停车订单详情
        /// </summary>
        /// <param name="jsonstr"></param>
        /// <returns></returns>
        [HttpGet]
        public object QueryFreeOrder([FromUri] string jsonstr)
        {
            return Json(new AjaxResponse(_berthsecAppService.SearchFreepOrder(jsonstr)));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public object QueryCard([FromUri]QueryCardInput input)
        {
            return Json(new AjaxResponse(_pdaAppService.SearchOtherAccount(input)));
        }
        /// <summary>
        /// 查询欠费数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetEscapeDetailsList([FromUri]EscapeDetailInput input)
        {
            //欠费数据会重复问题修改20220805
            try {
                GetEscapeDetailList EDetailList = _pdaAppService.GetEscapeDetailList(input);
                List<EscapeDetailsDto> EBDetailsDto = new List<EscapeDetailsDto>();
                List<long> IdList = new List<long>();
                foreach (EscapeDetailsDto element in EDetailList.Items)
                {
                    if (!IdList.Contains(element.Id))
                    {
                        IdList.Add(element.Id);
                        EBDetailsDto.Add(element);
                    }
                }
                EDetailList.Items = EBDetailsDto;
                return Json(new AjaxResponse(EDetailList));
            }
            catch 
            {
                return Json(new AjaxResponse(_pdaAppService.GetEscapeDetailList(input)));
            }
            //
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        [HttpGet]
        public object UpdateRemoteGuidStatus([FromUri] string guid, string access_token)
        {
            _pdaAppService.UpdateRemoteGuidStatus(guid, access_token);
            return Json(true);
        }

        /// <summary>
        /// 获取远程出场guid
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        [HttpGet]
        [DisableAuditing]
        public object GetRemoteGuids([FromUri]string access_token)
        {
            return Json(new AjaxResponse(_pdaAppService.GetRemoteGuids(access_token)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SyncTime"></param>
        /// <param name="Berthesclist"></param>
        /// <returns></returns>
        [HttpGet]
        [DisableAuditing]
        public object GetBerthsSyn([FromUri]string SyncTime, string Berthesclist)
        {
            var result = _pdaAppService.GetBerthsSyn(SyncTime, Berthesclist);
            return Json(new AjaxResponse(result));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="access_token">用户在线token</param>
        /// <returns></returns>
        [HttpGet]
        [DisableAuditing]
        public object CheckEquipmentStatus([FromUri]string access_token)
        {
            return Json(new AjaxResponse(_pdaAppService.CheckEquipmentStatus(access_token)));
        }

        /// <summary>
        /// 获取系统最新时间
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DisableAuditing]
        public object GetDateTimeNow()
        {
            return Json(new AjaxResponse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
        }

        /// <summary>
        /// 图片上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [DisableAuditing]
        public object PhotoUpLoadToAndroid()
        {
            //var provider = new MultipartFormDataMemoryStreamProvider();
            //填充provider provider在老版手持机代码里无效，改用context读取
            //Request.Content.ReadAsMultipartAsync(provider);
            //获取传统context
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            //定义传统request对象
            HttpRequestBase request = context.Request;

            //获取表单信息 防止未将对象引用到实例的错误
            string guid = request.Form.GetValues("guid")?.FirstOrDefault()??string.Empty;
            int businessid = int.Parse(request.Form.GetValues("businessid")?.FirstOrDefault()??"0");
            int pictype = int.Parse(request.Form.GetValues("pictype")?.FirstOrDefault() ?? "0");
            string createtime = request.Form.GetValues("createtime")?.FirstOrDefault();
            string access_token = request.Form.GetValues("access_token")?.FirstOrDefault();
            //没有guid，无效的空的请求
            if (string.IsNullOrWhiteSpace(guid))
            {
                return Json(new AjaxResponse("false"));
            }
            //没有token请求进行拦截
            //if (string.IsNullOrWhiteSpace(access_token))
            //{
            //    return Json(new AjaxResponse("false"));
            //}
            //没有检测到文件，判定为无效请求，不做任何处理
            if (request.Files.Count == 0)
            {
                return Json(new AjaxResponse("false"));
            }
            if (_pdaAppService.GetBusinessDetailPicture(guid, businessid) == 1)
                return Json(new AjaxResponse("false"));

            var imageFile = request.Files[0];
            //文件格式，如 image/jpeg
            //var fileNameWithExt = imageFile.Headers.ContentType.MediaType;
            //非图片格式
            //if (fileNameWithExt.ToLower().IndexOf("image/") == -1)
            //{
            //    return Json(new AjaxResponse("false"));
            //}
            var fileExtension =  "jpg";
            var stream = imageFile.InputStream;
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            if (_pdaAppService.PhotoUpLoadToAndroid(guid, businessid, pictype, bytes, fileExtension, createtime, access_token) <= 0)
            {
                return Json(new AjaxResponse("false"));
            }
            return Json(new AjaxResponse("true"));
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
        public object UpdateGps([FromUri]string x, string y, string access_token)
        {
            return Json(new AjaxResponse(_pdaAppService.UpdateGps(x, y, access_token)));
        }

        /// <summary>
        /// 收费员下班接口
        /// </summary>
        /// <param name="berthsecid"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        [HttpGet]
        public object EmployeeOutLineLogout([FromUri]string berthsecid, string access_token)
        {
            return Json(new AjaxResponse(_pdaAppService.EmployeeOutLineLogout(berthsecid, access_token)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="ExceptionMsg"></param>
        /// <returns></returns>
        [HttpPost]
        public object UploadAndroidExceptionLog([FromBody]string guid, string ExceptionMsg)
        {
            return Json(new AjaxResponse(_pdaAppService.UploadAndroidExceptionLog(guid, ExceptionMsg)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetSensorBusinessdetail([FromUri]string guid, string access_token)
        {
            return Json(new AjaxResponse(_pdaAppService.GetSensorBusinessdetail(guid)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        [HttpGet]
        public object NewCardAddTime(string access_token)
        {
            return Json(new AjaxResponse(""));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        [HttpGet]
        public object UpdateEnabled(string access_token)
        {
            return Json(new AjaxResponse(""));
        }


        /// <summary>
        /// 获取再停车辆信息
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="plateNumber"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetBerthInfoByPlateNumber(string access_token, string plateNumber)
        {
            return Json(new AjaxResponse(_pdaAppService.GetBerthInfoByPlateNumber(access_token, plateNumber)));
        }

        

        /// <summary>
        /// 上传异常日志
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [DisableAuditing]
        public object UploadExlogs()
        {
            //获取传统context
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            //定义传统request对象
            HttpRequestBase request = context.Request;

            var EmployeeId = request.Form.GetValues("EmployeeId")[0];
            string access_token = request.Form.GetValues("access_token")[0];
            var stream = request.Files[0].InputStream;
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            if (Directory.Exists(HostingEnvironment.MapPath("~\\exlogs\\" + DateTime.Now.ToString("yyyy-MM-dd"))) == false)
            {
                Directory.CreateDirectory(HostingEnvironment.MapPath("~\\exlogs\\" + DateTime.Now.ToString("yyyy-MM-dd")));
            }
            File.WriteAllBytes(HostingEnvironment.MapPath("~\\exlogs\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\" + EmployeeId + "&" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt"), bytes);
            GC.Collect();
            return Json(new AjaxResponse("true"));
        }

       

        /// <summary>
        /// 上传pda数据库文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [DisableAuditing]
        public object UploadSqliteDB()
        {
            //获取传统context
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
            //定义传统request对象
            HttpRequestBase request = context.Request;
            var EmployeeId = request.Form.GetValues("EmployeeId").FirstOrDefault()??DateTime.Now.ToString("yyyyMMdd");
            string access_token = request.Form.GetValues("access_token").FirstOrDefault() ?? Guid.NewGuid().ToString(); 
            if(request.Files.Count==0)
                return Json(new AjaxResponse("true"));
            var stream = request.Files[0].InputStream;
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);

            if (Directory.Exists(HostingEnvironment.MapPath("~\\sqlitedb\\" + DateTime.Now.ToString("yyyy-MM-dd"))) == false)
            {
                Directory.CreateDirectory(HostingEnvironment.MapPath("~\\sqlitedb\\" + DateTime.Now.ToString("yyyy-MM-dd")));
            }
            File.WriteAllBytes(HostingEnvironment.MapPath("~\\sqlitedb\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\" + EmployeeId + "&" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".db"), bytes);
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
        public object CheckVersion([FromUri]string OldVersion, string PDA, int Type, string access_token)
        {
            return Json(new AjaxResponse(_pdaAppService.CheckVersion(OldVersion, PDA, Type, access_token)));
        }

        /// <summary>
        /// 下载apk升级包
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage DownApk([FromUri]string val)
        {
            HttpResponseMessage result = null;
            string fileName = val + ".zip";
            string absoluFilePath = HostingEnvironment.MapPath("~/apk/" + fileName);
            FileStream fs = new FileStream(absoluFilePath, FileMode.Open);
            result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(fs);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = fileName;
            return result;
        }

        /// <summary>
        /// 下载apk升级包
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage DownApkInsp([FromUri] string val)
        {
            HttpResponseMessage result = null;
            string fileName = val + ".zip";
            string absoluFilePath = HostingEnvironment.MapPath("~/apkInsp/" + fileName);
            FileStream fs = new FileStream(absoluFilePath, FileMode.Open);
            result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(fs);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = fileName;
            return result;
        }

        /// <summary>
        /// 通过设备编号获取服务器信息
        /// </summary>
        /// <param name="DeviceCode"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetServerInfoByDeviceCode([FromUri] string DeviceCode)
        {
            return null;
        }

        /// <summary>
        /// 手持机二维码-微信支付回调通知
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [DisableAuditing]
        public HttpResponseMessage WinxinPayBackMessage()
        {
           // HttpResponseMessage result = null;
            
           // result = new HttpResponseMessage(HttpStatusCode.OK);
           // result.Content = new StreamContent(fs);
           // result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
           // return result;

           // HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
           // HttpRequestBase request = context.Request;
           // string resultFromWx = request.RequestContext.ToString();
           // //设置支付参数
           //// RequestHandler paySignReqHandler = new RequestHandler(null);
           // //WriteLog(" 微支付notice resultFromWx=" + resultFromWx);
           // var res = XDocument.Parse(resultFromWx);
            //通讯成功
            //if (res.Element("xml").Element("return_code").Value == "SUCCESS")
            //{
            //    if (res.Element("xml").Element("result_code").Value == "SUCCESS")
            //    {
            //        //交易成功
            //        paySignReqHandler.SetParameter("return_code", "SUCCESS");
            //        paySignReqHandler.SetParameter("return_msg", "OK");

            //        string ordecode = res.Element("xml").Element("out_trade_no").Value;
            //        //BLL.orders Bll = new BLL.orders();
            //        //try
            //        //{
            //        //    if (Bll.Update(ordecode))
            //        //    {

            //        //        Logger.Log.Info(string.Format("来自{0}的请求,请求地址{1},微支付交易失败=", context.Request.UserHostAddress, request.Url.ToString(), ordecode));
            //        //    }
            //        //    else
            //        //    {
            //        //        Logger.Log.Info(string.Format("来自{0}的请求,请求地址{1},微支付交易失败=", context.Request.UserHostAddress, request.Url.ToString(), ordecode));
            //        //    }
            //        //}
            //        //catch (Exception ex)
            //        //{
            //        //    Logger.Log.Info(string.Format("来自{0}的请求,请求地址{1},微支付交易异常=", context.Request.UserHostAddress, request.Url.ToString(), ordecode));
            //        //}
            //    }
            //    else
            //    {
            //        paySignReqHandler.SetParameter("return_code", "FAIL");
            //        paySignReqHandler.SetParameter("return_msg", "交易失败");
            //    }
            //}
            //else
            //{
            //    paySignReqHandler.SetParameter("return_code", "FAIL");
            //    paySignReqHandler.SetParameter("return_msg", "签名失败");
            //}
            //string data = paySignReqHandler.ParseXML();
            //var result = TenPayV3.Unifiedorder(data);
            //try
            //{
            //    //接收从微信后台POST过来的数据
            //    Stream s = Request.InputStream;
            //    byte[] buffer = new byte[Request.InputStream.Length];
            //    await s.ReadAsync(buffer, 0, buffer.Length); /
            //    string xml = System.Text.Encoding.UTF8.GetString(buffer);
            //    XmlDocument xmlDoc = new XmlDocument();
            //    xmlDoc.LoadXml(xml);
            //    string return_code = xmlDoc.DocumentElement.GetElementsByTagName("return_code")[0].InnerText;
            //    string out_trade_no = xmlDoc.DocumentElement.GetElementsByTagName("out_trade_no")[0].InnerText;//商户订单号
            //    string transaction_id = xmlDoc.DocumentElement.GetElementsByTagName("transaction_id")[0].InnerText;//微信支付订单号
            //    string nonce_str = xmlDoc.DocumentElement.GetElementsByTagName("nonce_str")[0].InnerText;//随机字符串
            //    string total_fee = xmlDoc.DocumentElement.GetElementsByTagName("total_fee")[0].InnerText; //金额
            //    string attach = xmlDoc.DocumentElement.GetElementsByTagName("attach")[0].InnerText;
            //    //业务逻辑 

            //    //微信支付成功回调
            //    if (return_code.ToUpper() == "SUCCESS")
            //    {
            //        return "<xml><return_code><![CDATA[SUCCESS]]></return_code><return_msg><![CDATA[OK]]></return_msg></xml>";
            //    }
            //    else
            //    {
            //        return "<xml><return_code><![CDATA[FAIL]]></return_code><return_msg><![CDATA[ERROR]]></return_msg></xml>"; //回调失败返回给微信
            //    }
            //}
            //catch (Exception)
            //{

            //    return "<xml><return_code><![CDATA[FAIL]]></return_code><return_msg><![CDATA[ERROR]]></return_msg></xml>"; //回调失败返回给微信
            //}
            var xmlstring = @"<xml><return_code><![CDATA[SUCCESS]]></return_code><return_msg><![CDATA[OK]]></return_msg></xml>";
            Logger.Log.Info(string.Format("微信支付交易"));
            return new System.Net.Http.HttpResponseMessage
            {
                Content = new System.Net.Http.StringContent(xmlstring),
                StatusCode = System.Net.HttpStatusCode.OK
            };

            //var xmlstring = "<xml><return_code><![CDATA[SUCCESS]]></return_code><return_msg><![CDATA[OK]]></return_msg></xml>";
            //return xmlstring;
        }

        /// <summary>
        /// 手持端修改拍照
        /// </summary>
        /// <param name="PlateNumber"></param>
        /// <param name="CarType"></param>
        /// <param name="Guid"></param>
        /// <returns></returns>
        [HttpGet]
        public object ModifyCarPlateNumber([FromUri] string PlateNumber,string CarType,string Guid) 
        {
            ErrorInfo error = _pdaAppService.ModifyCarPlateNumber(PlateNumber,CarType,Guid);
            if (error.Code > 0)
                return Json(new AjaxResponse(error, false));
            return Json(new AjaxResponse(true));
        }
    }
}
