using System;
using F2.Common;
using Newtonsoft.Json;
using F2.Application.PatrolCar.Dtos;
using F2.Core.Extensions;
using System.Data.SqlClient;
using System.Data;
using static CommonTool.ApiEnum;
using static SKIT.FlurlHttpClient.Wechat.TenpayV3.Models.UpdateMarketingMemberCardOpenCardRightsRequest.Types;
using F2.Application.PDA;
using F2.Core.Extensions.Models;
using F2.Application.Rates;
using F2.Application.WebChat;
using System.Net.Http;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Linq.Expressions;
using System.Web.UI.WebControls;
using static SKIT.FlurlHttpClient.Wechat.TenpayV3.Models.CreateEcommerceApplymentRequest.Types;
using Org.BouncyCastle.Utilities;

namespace F2.Application.PatrolCar
{
    public class PatrolCarService : IPatrolCarService
    {
        #region Var
        private readonly IWebChatAppService _webChatAppService;
        private readonly IRateAppService _rateAppService;
        #endregion

        public PatrolCarService()
        {
            _webChatAppService = new WebChatAppService();
            _rateAppService = new RateAppService();
        }
        /// <summary>
        /// 转换为停车类型
        /// </summary>
        /// <param name="ColorType"></param>
        /// <returns></returns>
        public string getStopType(int ColorType)
        {
            if (ColorType == 3 || ColorType == 4)
            {
                //大型车
                return "1";
            }
            else {
                //小型车
                return "2";
            }
        }
        public string getFaultType(int Type)
        {
            switch (Type)
            {
                case 15:
                    return "疑似识别错误告警";
                    break;
                case 16:
                    return "确定出场告警";
                    break;
                case 17:
                    return "车牌遮挡告警";
                    break;
                case 18:
                    return "设备告警";
                    break;
                case 19:
                    return "泊位异常";
                    break;
                case 20:
                    return "泊位外停车告警";
                    break;
                case 21:
                    return "视频遮挡";
                    break;
                case 22:
                    return "非机动车占用";
                    break;
                case 23:
                    return "无牌车占用";
                    break;
                case 24:
                    return "车辆超时占用";
                    break;
                case 25:
                    return "跨泊位停车";
                    break;
                case 26:
                    return "超速";
                    break;
                default:
                    return "";
                    break;
            }
        }
        /// <summary>
        /// 巡检车停车数据
        /// </summary>
        /// <param name="dto"></param>
        public PatrolCarResponse PatrolCarStopData(PatrolCarRequest dto)
        {
            CommonTools.WriteLogFile("巡检车停车数据入库：" + JsonConvert.SerializeObject(dto).ToString());
            PatrolCarResponse result = new PatrolCarResponse();
            try
            {
                result.errorcode = 0;
                string guid = Guid.NewGuid().ToString();
                string hapenTime = CommonTools.getDateTime(Convert.ToInt64(dto.occur_time * 1000)).ToString("yyyy-MM-dd HH:mm:ss.fff");
                string sql = "";
                string sq = "";
                sq = "select a.PatrolCarNumber,a.PatrolType,b.BerthsId,c.Id BerthId,c.BerthNumber,c.TenantId,c.CompanyId,c.RegionId,c.ParkId,c.BerthsecId,d.BerthsecName from AbpPatrolCars a,AbpPatrolCarBerths b,AbpBerths c,AbpBerthsecs d where a.PatrolCarNumber=b.PatrolCarNumber and b.BerthsId=c.Id and c.BerthsecId=d.Id and a.IsUse=1 and a.PatrolCarNumber='" + dto.sn + "' and c.BerthNumber='"+ dto.berth_name+ "'";
                DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sq);
                #region 校验
                if (tables.Rows.Count == 0)
                {
                    CommonTools.WriteLogFile("PatrolCarStopData:巡检车信息与泊位关系不存在");
                    result.errorcode = 1;
                    result.message = "巡检车信息与泊位关系不存在";
                    return result;
                }
                TimeSpan ts = DateTime.Now - Convert.ToDateTime(hapenTime);
                if (ts.Days >= 1)
                {
                    CommonTools.WriteLogFile("PatrolCarStopData:历史数据禁止推送");
                    result.errorcode = 1;
                    result.message = "历史数据禁止推送";
                    return result;
                }
                #endregion
                #region 入场
                if (dto.event_type==1) {
                    DataTable dtSensor = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select CarInTime from AbpSensorBusinessDetail with(nolock) where BerthNumber='" + dto.berth_name + "' and status = 0").Tables[0];//地磁判断
                    if (dtSensor.Rows.Count > 0)
                    {
                        if ((CommonTools.getDateTime(Convert.ToInt64(dto.occur_time * 1000))- Convert.ToDateTime(dtSensor.Rows[0]["CarInTime"])).TotalHours<1) {
                            hapenTime = Convert.ToDateTime(dtSensor.Rows[0]["CarInTime"]).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        }
                        
                    }
                    string InURL = "";
                    string DetailURL = "";
                    int AuditStatus = 0;
                    foreach (ImagesInfo ele in dto.images) {
                        if (ele.image_type==1) {
                            InURL = CommonTools.getimages(ele.image_url, @"\PatrolPic\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\" + DateTime.Now.ToString("dd"), "IN" + DateTime.Now.ToString("yyMMddHHMMssfff"));
                        }
                        if (ele.image_type == 2) {
                            DetailURL = CommonTools.getimages(ele.image_url, @"\PatrolPic\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\" + DateTime.Now.ToString("dd"), "De" + DateTime.Now.ToString("yyMMddHHMMssfff"));
                        }
                    }
                    if (dto.berth_confidence >= dto.Trust && dto.plate.confidence >= dto.Trust) {
                        //有效订单
                        AuditStatus = 1;
                    }
                    else {
                        //待审核订单
                        AuditStatus = 2;
                    }
                    sql += "insert into AbpPatrolCarBusinessDetail (TenantId,CompanyId,RegionId,ParkId,BerthsecId,BerthNumber,PatrolCarNumber,PlateNumber,CarInTime,guid,CreationTime,PlateColor,Confidence,Speed,OssPathURL,DetailOssPathURL,AuditStatus,BConfidence,Keys,BerthId,PatrolType) VALUES ('" + tables.Rows[0]["TenantId"] + "','"+ tables.Rows[0]["CompanyId"] + "','"+ tables.Rows[0]["RegionId"] + "','"+ tables.Rows[0]["ParkId"] + "','"+ tables.Rows[0]["BerthsecId"] + "','"+ tables.Rows[0]["BerthNumber"] + "','"+ tables.Rows[0]["PatrolCarNumber"] + "','"+dto.plate.plate_number+ "','"+ hapenTime + "','"+ guid+ "','"+ DateTime.Now + "','"+dto.plate.plate_color+"','"+dto.plate.confidence+"','"+dto.shot_speed+"','"+ InURL + "','"+ DetailURL + "','"+ AuditStatus + "','"+dto.berth_confidence+"','"+dto.guid+"','"+ tables.Rows[0]["BerthId"] + "','"+ tables.Rows[0]["PatrolType"] + "');";
                    sql += $"update AbpPatrolCars set IsOnlineValue=1, BeatDatetime = '"+ DateTime.Now + "' where PatrolCarNumber = '"+ dto.sn + "';";
                    sql += $"update AbpBerths set StopType = 1,IsFaultFlag=0,RelateNumber = '{dto.plate.plate_number}',InCarTime = '{hapenTime}', OutCarTime = null, ParkStatus = 1,BerthStatus='1',guid='{guid}' where Id = '{tables.Rows[0]["BerthId"]}';";
                    if (AuditStatus==1)
                    {
                        DataTable dtBusiness = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select Id from AbpBusinessDetail with(nolock) where BerthNumber='" + dto.berth_name + "' and CarOutTime is null and IsDeleted=0").Tables[0];
                        if (dtBusiness.Rows.Count==0) {
                            sql += $@" insert into AbpBusinessDetail(BerthNumber, PlateNumber, CarType, Prepaid, CarInTime, InOperaId, InDeviceCode, guid, StopType, RegionId, ParkId, BerthsecId, 
                                    Status, PrepaidCarNo, PrepaidPayStatus, Receivable, FactReceive, Arrearage, PaymentType, EscapePayStatus, IsEscapePay, PayStatus, IsPay, FeeType, TenantId, 
                                    CompanyId, IsLock, IsDeleted, CreationTime, CreatorUserId, InBatchNo, SensorsInCarTime) values
                                    ('{Convert.ToString(tables.Rows[0]["BerthNumber"])}', '{dto.plate.plate_number}', '{getStopType(dto.plate.plate_type)}', 0, '{hapenTime}',1,'{dto.sn}', '{guid}', '1', 
                                    {Convert.ToInt32(tables.Rows[0]["RegionId"])}, {Convert.ToInt32(tables.Rows[0]["ParkId"])}, {Convert.ToInt32(tables.Rows[0]["BerthsecId"])}, 
                                    1, '', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, {Convert.ToInt32(tables.Rows[0]["TenantId"])}, {Convert.ToInt32(tables.Rows[0]["CompanyId"])}, 0, 0, getdate(), NULL, NULL, NULL) ";
                        }
                        /*
                        try {
                            //微信推送
                            var loginToken = new AbpUserLoginToken();
                            loginToken.TenantId = Convert.ToInt32(Convert.ToInt32(tables.Rows[0]["TenantId"]));
                            loginToken.CompanyId = Convert.ToInt32(Convert.ToInt32(tables.Rows[0]["CompanyId"]));
                            _webChatAppService.SendCarInMsg(dto.plate.plate_number, hapenTime, Convert.ToInt32(tables.Rows[0]["BerthsecId"]), Convert.ToString(tables.Rows[0]["BerthNumber"]), loginToken, Convert.ToString(tables.Rows[0]["BerthsecName"]));//推送微信信息
                            PDAAppService _PDAAppService = new PDAAppService();
                            _PDAAppService.SendSms(dto.plate.plate_number, loginToken.CompanyId, loginToken.TenantId, "BlackCarInModel", hapenTime, tables.Rows[0]["BerthNumber"].ToString(), new BerthsecAppService().GetBerthsecInfo(Convert.ToInt32(tables.Rows[0]["BerthsecId"])).BerthsecName);
                        }
                        catch (Exception ex){}
                        */
                    }
                }
                #endregion
                #region 出场
                if (dto.event_type == 4) {
                    DataTable dtSensor = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select CarOutTime from AbpSensorBusinessDetail with(nolock) where BerthNumber='" + dto.berth_name + "' and status = 1 and CarOutTime>'"+ DateTime.Now.AddHours(-1) + "'").Tables[0];//地磁判断
                    if (dtSensor.Rows.Count > 0)
                    {
                        if ((CommonTools.getDateTime(Convert.ToInt64(dto.occur_time * 1000)) - Convert.ToDateTime(dtSensor.Rows[0]["CarOutTime"])).TotalHours < 1) {
                            hapenTime = Convert.ToDateTime(dtSensor.Rows[0]["CarOutTime"]).ToString("yyyy-MM-dd HH:mm:ss.fff");
                        }
                    }
                    string OutURL = "";
                    string DetailURL = "";
                    int? StopTime=0;
                    decimal Receivable = 0;
                    foreach (ImagesInfo ele in dto.images)
                    {
                        if (ele.image_type == 1)
                        {
                            OutURL = CommonTools.getimages(ele.image_url, @"\PatrolPic\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\" + DateTime.Now.ToString("dd"), "Out" + DateTime.Now.ToString("yyMMddHHMMssfff"));
                        }
                        if (ele.image_type == 2)
                        {
                            DetailURL = CommonTools.getimages(ele.image_url, @"\PatrolPic\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\" + DateTime.Now.ToString("dd"), "De" + DateTime.Now.ToString("yyMMddHHMMssfff"));
                        }
                    }
                    
                    sq = "select BerthsecId,ParkId,PlateNumber,CompanyId,CarInTime,guid from AbpPatrolCarBusinessDetail where Keys='"+dto.guid+ "' and PatrolCarNumber='"+dto.sn+"'";
                    DataTable dtTemp = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sq);
                    if (dtTemp.Rows.Count > 0)
                    {
                        var model = _rateAppService.RateCalculate(int.Parse(dtTemp.Rows[0]["BerthsecId"].ToString()), DateTime.Parse(dtTemp.Rows[0]["CarInTime"].ToString()), Convert.ToDateTime(hapenTime), 2, 1, int.Parse(dtTemp.Rows[0]["ParkId"].ToString()), dtTemp.Rows[0]["PlateNumber"].ToString(), int.Parse(dtTemp.Rows[0]["CompanyId"].ToString()));
                        Receivable = model.CalculateMoney;
                        StopTime = (int?)model.ParkTime;
                    }
                    if (string.IsNullOrEmpty(DetailURL))
                    {
                        sql += $"update AbpPatrolCarBusinessDetail set CarOutTime='" + hapenTime + "',StopTime=datediff(minute,CarInTime,'" + hapenTime + "'),Receivable='" + Receivable + "' ,OutOssPathURL = '" + OutURL + "' where Keys = '" + dto.guid + "';";
                    }
                    else {
                        sql += $"update AbpPatrolCarBusinessDetail set CarOutTime='" + hapenTime + "',StopTime=datediff(minute,CarInTime,'"+ hapenTime + "'),Receivable='" + Receivable + "' ,OutOssPathURL = '" + OutURL + "',DetailOssPathURL='"+ DetailURL + "' where Keys = '" + dto.guid + "';";
                    }
                    sql += $"update AbpPatrolCars set IsOnlineValue=1, BeatDatetime = '" + DateTime.Now + "' where PatrolCarNumber = '" + dto.sn + "';";
                    sql += $"update AbpBerths set RelateNumber = '{dto.plate.plate_number}',OutCarTime = '{hapenTime}', ParkStatus = 0,BerthStatus='2',CarType='{getStopType(dto.plate.plate_type)}' where Id = {tables.Rows[0]["BerthId"]};";
                    if (Receivable == 0)
                    {
                        if (StopTime == 0)
                        {
                            sql += $"update AbpBusinessDetail set [Status] = 2,[Receivable] = '{Receivable}' ,[Money] = '{Receivable}',[Arrearage] = '{Receivable}',CarOutTime='{hapenTime}',StopTime =datediff(minute,CarInTime,'{hapenTime}'),IsPay=1,StopType=7,PayStatus=0,OutOperaId=1,CarPayTime='{hapenTime}',OutDeviceCode = '{dto.sn}' where [guid] = '{dtTemp.Rows[0]["guid"]}' and CarOutTime is null;";
                        }
                        else
                        {
                            sql += $"update AbpBusinessDetail set [Status] = 2,[Receivable] = '{Receivable}' ,[Money] = '{Receivable}',[Arrearage] = '{Receivable}',CarOutTime='{hapenTime}',StopTime =datediff(minute,CarInTime,'{hapenTime}'),IsPay=1,StopType=2,PayStatus=0,OutOperaId=1,CarPayTime='{hapenTime}',OutDeviceCode = '{dto.sn}' where [guid] = '{dtTemp.Rows[0]["guid"]}' and CarOutTime is null;";
                        }
                    }
                    else
                    {
                        sql += $"update AbpBusinessDetail set [Status] = 3,[Receivable] = '{Receivable}' ,[Money] = '{Receivable}',[Arrearage] = '{Receivable}',CarOutTime='{hapenTime}',StopTime =datediff(minute,CarInTime,'{hapenTime}'),CarPayTime='{hapenTime}',OutOperaId=1,OutDeviceCode = '{dto.sn}' where [guid] = '{dtTemp.Rows[0]["guid"]}' and CarOutTime is null;";
                    }
                }
                #endregion
                #region 修正
                if (dto.event_type == 512) {
                    string FixURL = "";
                    foreach (ImagesInfo ele in dto.images)
                    {
                        if (ele.image_type == 1)
                        {
                            FixURL = CommonTools.getimages(ele.image_url, @"\PatrolPic\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\" + DateTime.Now.ToString("dd"), "Fix" + DateTime.Now.ToString("yyMMddHHMMssfff"));
                        }
                    }
                    sql += $"update AbpPatrolCarBusinessDetail set FixOssPathURL='" + FixURL + "',PlateNumber='" + dto.plate.plate_number + "' where Keys = '" + dto.guid + "';";

                    string sqs = "select BerthsecId,ParkId,PlateNumber,CompanyId,CarInTime,guid from AbpPatrolCarBusinessDetail where Keys='" + dto.guid + "' and PatrolCarNumber='" + dto.sn + "'";
                    DataTable dtTemps = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sqs);
                    sql += $"update AbpBusinessDetail set PlateNumber='" + dto.plate.plate_number + "' where guid = '" + dtTemps.Rows[0]["guid"] + "';";
                }
                #endregion
                SqlConnection conn = new SqlConnection(SqlHelper.connectionString);
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Transaction = tran;
                try
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    CommonTools.WriteLogFile("PatrolCarStopData:ErrorSQL:" + sql.ToString() + ex.ToString());
                    tran.Rollback();
                    result.errorcode = 1;
                    result.message = "数据库回滚";
                    return result;
                }
                
            }
            catch (Exception ex)
            {
                CommonTools.WriteLogFile("PatrolCarStopData:Error:" + ex.ToString());
                result.errorcode = 1;
                result.message = ex.ToString();
            }
            return result;
        }
        /// <summary>
        /// 巡检车报警数据
        /// </summary>
        /// <param name="dto"></param>
        public PatrolCarResponse PatrolCarFaultData(PatrolCarRequest dto) {
            CommonTools.WriteLogFile("巡检车报警数据入库：" + JsonConvert.SerializeObject(dto).ToString());
            PatrolCarResponse result = new PatrolCarResponse();
            try
            {
                result.errorcode = 0;
                string hapenTime = CommonTools.getDateTime(Convert.ToInt64(dto.occur_time * 1000)).ToString("yyyy-MM-dd HH:mm:ss.fff");
                string sql = "";
                if (dto.event_type == 15 || dto.event_type == 16 || dto.event_type == 17 || dto.event_type == 18 || dto.event_type == 19 || dto.event_type == 20 || dto.event_type==21 || dto.event_type == 22 || dto.event_type == 23 || dto.event_type == 24 || dto.event_type == 25 || dto.event_type == 26) {
                    sql = "select a.PatrolCarNumber,a.PatrolType,b.BerthsId,c.Id BerthId,c.BerthNumber,c.TenantId,c.CompanyId,c.RegionId,c.ParkId,c.BerthsecId,d.BerthsecName from AbpPatrolCars a,AbpPatrolCarBerths b,AbpBerths c,AbpBerthsecs d where a.PatrolCarNumber=b.PatrolCarNumber and b.BerthsId=c.Id and c.BerthsecId=d.Id and a.IsUse=1 and a.PatrolCarNumber='" + dto.sn + "' and c.BerthNumber='" + dto.berth_name + "'";
                    DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                    if (tables.Rows.Count> 0)
                    {
                        string FaultURL = "";
                        foreach (ImagesInfo ele in dto.images)
                        {
                            if (ele.image_type == 1)
                            {
                                FaultURL = CommonTools.getimages(ele.image_url, @"\PatrolPic\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\" + DateTime.Now.ToString("dd"), "Fault" + DateTime.Now.ToString("yyMMddHHMMssfff"));
                            }
                        }
                        sql = "insert into AbpPatrolCarFault (TenantId,CompanyId,RegionId,ParkId,BerthsecId,BerthNumber,PatrolCarNumber,CreationTime,Status,Remark,OssPathURL) VALUES ('" + tables.Rows[0]["TenantId"] + "','" + tables.Rows[0]["CompanyId"] + "','" + tables.Rows[0]["RegionId"] + "','" + tables.Rows[0]["ParkId"] + "','" + tables.Rows[0]["BerthsecId"] + "','" + tables.Rows[0]["BerthNumber"] + "','" + tables.Rows[0]["PatrolCarNumber"] + "','" + hapenTime + "','" + dto.event_type + "','" + getFaultType(dto.event_type) + "','" + FaultURL + "');";
                        SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonTools.WriteLogFile("PatrolCarFaultData:Error:" + ex.ToString());
                result.errorcode = 1;
                result.message = ex.ToString();
            }
            return result;
        }
        /// <summary>
        /// 巡检车状态数据
        /// </summary>
        /// <param name="dto"></param>
        public PatrolCarResponse PatrolCarStateData(PatrolCarRequest dto) {
            CommonTools.WriteLogFile("巡检车状态数据入库：" + JsonConvert.SerializeObject(dto).ToString());
            PatrolCarResponse result = new PatrolCarResponse();
            try {
                result.errorcode = 0;
                string sql = "";
                if (dto.event_type ==0) {
                    if (dto.device_state == 0)
                    {
                        sql = $"update AbpPatrolCars set IsOnlineValue=1, BeatDatetime = '" + DateTime.Now + "' where PatrolCarNumber = '" + dto.sn + "';";
                    }
                    else {
                        sql = $"update AbpPatrolCars set IsOnlineValue=0, BeatDatetime = '" + DateTime.Now + "' where PatrolCarNumber = '" + dto.sn + "';";
                    }
                    SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                }
            }
            catch (Exception ex)
            {
                CommonTools.WriteLogFile("PatrolCarStateData:Error:" + ex.ToString());
                result.errorcode = 1;
                result.message = ex.ToString();
            }
            return result;
        }
        /// <summary>
        /// 巡检车位置信息上报
        /// </summary>
        /// <param name="dto"></param>
        public PatrolCarResponse PatrolCarStationData(PatrolCarRequest dto)
        {
            CommonTools.WriteLogFile("巡检车位置数据入库：" + JsonConvert.SerializeObject(dto).ToString());
            PatrolCarResponse result = new PatrolCarResponse();
            try
            {
                result.errorcode = 0;
                string sql = "insert into AbpEquipmentGps (PDA,X,Y,CreationTime) VALUES ('" + dto.sn + "','"+dto.X+"','"+dto.Y+"',GETDATE())";
                SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                CommonTools.WriteLogFile("PatrolCarStationData:Error:" + ex.ToString());
                result.errorcode = 1;
                result.message = ex.ToString();
            }
            return result;
        }
    }
}
