using F2.Application.VideoEqs.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using F2.Core.Extensions;
using F2.Application.WebChat;
using System.Data.SqlClient;
using F2.Application.PDA;
using F2.Common;
using System.IO;
using F2.Core.Extensions.Models;
using System.Configuration;
using System.Net.Http;
using System.Collections;
using F2.Application.Rates;
using static CommonTool.ApiEnum;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using CommonTool;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Web.UI.WebControls;
using Org.BouncyCastle.Utilities.Collections;
using static SKIT.FlurlHttpClient.Wechat.TenpayV3.Models.CreateApplyForSubMerchantApplymentRequest.Types.Subject.Types;
using static SKIT.FlurlHttpClient.Wechat.TenpayV3.Models.CreateEcommerceApplymentRequest.Types;
using System.Linq.Expressions;
using System.Web.Caching;
using Org.BouncyCastle.Crypto;
using System.ComponentModel;
using Aop.Api.Domain;
using F2.Application.Parking.Dtos;

namespace F2.Application.VideoEqs
{
    public class VideoEqAppService : IVideoEqAppService
    {
        #region Var
        private readonly IWebChatAppService _webChatAppService;
        private readonly IRateAppService _rateAppService;
        private HttpClient client = new HttpClient();
        #endregion

        public VideoEqAppService()
        {
            _webChatAppService = new WebChatAppService();
            _rateAppService = new RateAppService();
        }
        /// <summary>
        /// 视频设备状态数据
        /// </summary>
        /// <param name="dto"></param>
        public VideoEqParkHighRepose PostStateHighData(Hashtable dto)
        {
            CommonTools.WriteLogFile("视频设备状态数据入库：" + JsonConvert.SerializeObject(dto).ToString());
            VideoEqParkHighRepose result = new VideoEqParkHighRepose();
            try
            {
                result.errorcode = 1;
                var deviceSn = Convert.ToString(dto["deviceSn"]);
                var deviceState = Convert.ToInt32(dto["deviceState"]);
                var IsOnlineValue = 1;
                if (deviceState == 1)
                {
                    IsOnlineValue = 0;
                }
                string sql = "";
                if (Convert.ToString(dto["evt"]) == "0")
                {
                    sql = "update AbpVideoEquips set IsOnlineValue=" + IsOnlineValue + " where VedioEqNumber='" + deviceSn + "'";
                    SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                    result.errorcode = 0;
                    result.message = "";
                }
                if (Convert.ToString(dto["evt"]) == "13")
                {
                    sql = "update AbpVideoEquips set IsOnlineValue=0";
                    SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                    sql = "update AbpVideoEquips set IsOnlineValue=" + IsOnlineValue + " where VedioEqNumber in (" + deviceSn + ")";
                    SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                    result.errorcode = 0;
                    result.message = "";
                }
            }
            catch (Exception ex)
            {
                //记录日志
                CommonTools.WriteLogFile(ex.ToString());
                result.errorcode = 1;
                result.message = ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 通过拍照颜色转换为停车类型
        /// </summary>
        /// <param name="ColorType"></param>
        /// <returns></returns>
        public string getStopType(int ColorType)
        {
            if (ColorType == 0 || ColorType == 1 || ColorType == 5)
            {
                //未知，蓝，绿
                return "2";
            }
            else if (ColorType == 2)
            {
                //黄牌大车
                return "1";
            }
            else
            {
                //黑，白
                return "2";
            }
        }

        /// <summary>
        /// 视频设备停车数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public VideoEqParkHighRepose PostVideoEqParkHighData(VideoEqParkHighRequest dto)
        {
            CommonTools.WriteLogFile("视频设备停车数据入库：" + JsonConvert.SerializeObject(dto).ToString());
            VideoEqParkHighRepose result = new VideoEqParkHighRepose();
            try
            {
                result.errorcode = 1;
                string sql = string.Empty;
                string guid = Guid.NewGuid().ToString();
                string hapenTime = CommonTools.getDateTime(Convert.ToInt64(dto.happenTime * 1000)).ToString("yyyy-MM-dd HH:mm:ss.fff");
                Commons com = new Commons();

                #region 校验入参
                TimeSpan ts = DateTime.Now - Convert.ToDateTime(hapenTime);
                if (ts.Days > 1)
                {
                    result.errorcode = 1;
                    result.message = "历史数据禁止推送";
                    return result;
                }
                #endregion

                #region 校验停车数据表
                DataTable dtV = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $"select Id,State from AbpVideoEquipBusinessDetail with(nolock) where VID = '{dto.parkingActId}' and VedioEqNumber='{dto.deviceSn}'").Tables[0];

                if (dtV.Rows.Count > 1)
                {
                    CommonTools.WriteLogFile("数据重复");
                    result.errorcode = 1;
                    result.message = "数据重复";
                    return result;
                }
                else if (dtV.Rows.Count == 1 && dtV.Select("State = '3 '").Count() != 1)
                {
                    CommonTools.WriteLogFile("数据重复");
                    result.errorcode = 1;
                    result.message = "数据重复";
                    return result;
                }
                #endregion

                #region 校验视频设备表
                //通过设备号查询视频设备表,设备号唯一
                DataTable dt = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $"select * from AbpVideoEquips with(nolock) where IsUse=1 and VedioEqNumber = '{dto.deviceSn}' and BerthNumber ='{dto.berthCode}'").Tables[0];

                if (dt.Rows.Count < 1)
                {
                    CommonTools.WriteLogFile("视频设备未注册");
                    result.errorcode = 1;
                    result.message = "视频设备未注册";
                    return result;
                }

                #endregion
                string ErrorMsg = "";
                if (dto.evt == (int)VideoEqParkHighType.入场)
                {
                    int AuditStatus = 1;
                    #region 车辆入场，插入视频设备停车数据表（需支付金额为空，出场时间为空，停车时间为空）
                    string InURL = CommonTools.getimages(dto.picUrl, @"\VideoPic\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\" + DateTime.Now.ToString("dd"), "IN" + Guid.NewGuid().ToString());
                    string DetailURL = CommonTools.getimages(dto.plateNumberUrl, @"\VideoPic\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\" + DateTime.Now.ToString("dd"), "De" + Guid.NewGuid().ToString());


                    #endregion
                    #region 更新视频设备配置表
                    sql += $"update AbpVideoEquips set [Guid] = '{guid}', ParkStatus = 1,IsOnlineValue=1,BeatDatetime = GETDATE() where BerthNumber = '{dto.berthCode}';";
                    #endregion

                    #region 更新泊位表
                    //判断泊位号是否存在
                    DataTable dtBer = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $"select * from AbpBerths with(nolock) where Id = {Convert.ToInt32(dt.Rows[0]["BerthId"])}").Tables[0];
                    if (dtBer.Rows.Count != 1)
                    {
                        CommonTools.WriteLogFile("泊位号不存在，入场失败");
                        result.errorcode = 1;
                        result.message = "泊位号不存在，入场失败";
                        return result;
                    }
                    DataTable dtBerC = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $"select * from AbpBerths with(nolock) where Id = {Convert.ToInt32(dt.Rows[0]["BerthId"])} and RelateNumber='{dto.plateNumber}' and ParkStatus=1").Tables[0];
                    if (dtBerC.Rows.Count>0) {
                        ErrorMsg = "重复订单";
                        CommonTools.WriteLogFile("该车辆重复在停");
                        result.errorcode = 1;
                        result.message = "该车辆重复在停";
                        return result;
                    }
                    DataTable dtBerD = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $"select * from AbpBerthsecs with(nolock) where Id = {Convert.ToInt32(dtBer.Rows[0]["BerthsecId"])}").Tables[0];
                    if (dtBerD.Rows.Count != 1)
                    {
                        CommonTools.WriteLogFile("泊位段不存在，入场失败");
                        result.errorcode = 1;
                        result.message = "泊位段不存在，入场失败";
                        return result;
                    }
                    DataTable dtBerE = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $"select * from AbpBerths with(nolock) where Id = {Convert.ToInt32(dt.Rows[0]["BerthId"])} and ParkStatus=1").Tables[0];
                    if (dtBerE.Rows.Count > 0)
                    {
                        sql += $"update AbpVideoEquipBusinessDetail set AuditStatus = 2 where guid ='" + Convert.ToString(dtBerE.Rows[0]["guid"]) + "';";
                    }
                    #region 车牌校验
                    if (!com.IsValidPlateNumber(dto.plateNumber))
                    {
                        AuditStatus = 2;
                        ErrorMsg += "车牌错误;";
                    }
                    #endregion
                    //CarType={3},CardNo='{4}',Prepaid={5}, 
                    sql += $"update AbpBerths set IsSourceVideo = 1,StopType = 1,RelateNumber = '{dto.plateNumber}',InCarTime = '{hapenTime}', OutCarTime = null, [guid] = '{guid}', ParkStatus = 1,BerthStatus='1',CarType='{getStopType(dto.plateColor)}' where Id = {Convert.ToInt32(dt.Rows[0]["BerthId"])};";
                    #endregion
                    if (dto.actionCredible >= dto.Trust || dto.Trust == 101)
                    {
                        //置信度
                        sql += $@"INSERT INTO [dbo].[AbpVideoEquipBusinessDetail]
                                ([TenantId],[CompanyId],[RegionId],[ParkId],[BerthsecId],[BerthNumber],[BerthId],[VedioEqNumber],[Receivable],
                                [PlateNumber],[CarInTime],[CarOutTime],[StopTime],[guid],[Indicate],[CreationTime],[Status],[State],
                                [PlateColor],[OssPathURL],[VModel],[Powerp],[VID],[OutOssPathURL],DetailOssPathURL,FixOssPathURL,[Trust],[AuditStatus],ErrorMsg)
                                VALUES
                                ({Convert.ToInt32(dt.Rows[0]["TenantId"])},{Convert.ToInt32(dt.Rows[0]["CompanyId"])},{Convert.ToInt32(dt.Rows[0]["RegionId"])},
                                    {Convert.ToInt32(dt.Rows[0]["ParkId"])},{Convert.ToInt32(dt.Rows[0]["BerthsecId"])},'{Convert.ToString(dt.Rows[0]["BerthNumber"])}','{Convert.ToString(dt.Rows[0]["BerthId"])}',
                                '{dto.deviceSn}',NULL,'{dto.plateNumber}','{hapenTime}',NULL,NULL,'{guid}',NULL,'{DateTime.Now}',0,{(int)VideoEqParkType.有车},{dto.plateColor},'{InURL}','','','H{dto.parkingActId}',NULL,'{DetailURL}',NULL,{dto.actionCredible},{AuditStatus},'{ErrorMsg}');";

                        #region 更新收费明细表
                        //判断收费明细是否存在
                        DataTable dtBusD = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $"select count(1) from AbpBusinessDetail with(nolock) where guid = '{guid}'").Tables[0];
                        if (dtBusD.Rows.Count != 1)
                        {
                            CommonTools.WriteLogFile("收费信息已存在，入场失败");
                            result.errorcode = 1;
                            result.message = "收费信息已存在，入场失败";
                            return result;
                        }
                        //黑名单白名单问题

                        //更新收费明细
                        //CarType默认1；InOperaId默认1
                        sql += $@" insert into AbpBusinessDetail(BerthNumber, PlateNumber, CarType, Prepaid, CarInTime, InOperaId, InDeviceCode, guid, StopType, RegionId, ParkId, BerthsecId, 
                                    Status, PrepaidCarNo, PrepaidPayStatus, Receivable, FactReceive, Arrearage, PaymentType, EscapePayStatus, IsEscapePay, PayStatus, IsPay, FeeType, TenantId, 
                                    CompanyId, IsLock, IsDeleted, CreationTime, CreatorUserId, InBatchNo, SensorsInCarTime) values
                                    ('{Convert.ToString(dtBer.Rows[0]["BerthNumber"])}', '{dto.plateNumber}', '{getStopType(dto.plateColor)}', 0, '{hapenTime}',1,'{dto.deviceSn}', '{guid}', '1', 
                                    {Convert.ToInt32(dtBer.Rows[0]["RegionId"])}, {Convert.ToInt32(dtBer.Rows[0]["ParkId"])}, {Convert.ToInt32(dtBer.Rows[0]["BerthsecId"])}, 
                                    1, '', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, {Convert.ToInt32(dtBer.Rows[0]["TenantId"])}, {Convert.ToInt32(dtBer.Rows[0]["CompanyId"])}, 0, 0, getdate(), NULL, NULL, NULL) ";
                        #endregion
                    }
                    else
                    {
                        sql += $@"INSERT INTO [dbo].[AbpVideoEquipBusinessDetail]
                                ([TenantId],[CompanyId],[RegionId],[ParkId],[BerthsecId],[BerthNumber],[BerthId],[VedioEqNumber],[Receivable],
                                [PlateNumber],[CarInTime],[CarOutTime],[StopTime],[guid],[Indicate],[CreationTime],[Status],[State],
                                [PlateColor],[OssPathURL],[VModel],[Powerp],[VID],[OutOssPathURL],DetailOssPathURL,FixOssPathURL,[Trust],[AuditStatus],ErrorMsg)
                                VALUES
                                ({Convert.ToInt32(dt.Rows[0]["TenantId"])},{Convert.ToInt32(dt.Rows[0]["CompanyId"])},{Convert.ToInt32(dt.Rows[0]["RegionId"])},
                                    {Convert.ToInt32(dt.Rows[0]["ParkId"])},{Convert.ToInt32(dt.Rows[0]["BerthsecId"])},'{Convert.ToString(dt.Rows[0]["BerthNumber"])}','{Convert.ToString(dt.Rows[0]["BerthId"])}',
                                '{dto.deviceSn}',NULL,'{dto.plateNumber}','{hapenTime}',NULL,NULL,'{guid}',NULL,'{DateTime.Now}',0,{(int)VideoEqParkType.有车},{dto.plateColor},'{InURL}','','','H{dto.parkingActId}',NULL,'{DetailURL}',NULL,{dto.actionCredible},2,'{ErrorMsg}');";
                    }
                    #region 事务提交
                    bool flag = true;
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
                        flag = false;
                        CommonTools.WriteLogFile("ErrorSQL:" + sql.ToString() + ex.ToString());
                        tran.Rollback();
                        if (ex.ToString().Contains("0x80131904"))
                        {
                            string SqlFalseCount = "select Id from AbpVideoFaultsData with(nolock) where Evt='" + dto.evt + "' and ParkingActId='" + dto.parkingActId + "' and PlateNumber='" + dto.plateNumber + "' and IsDelete is null";
                            DataTable tablesFalseCount = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, SqlFalseCount);
                            if (tablesFalseCount.Rows.Count == 0)
                            {
                                string SqlFalse = "insert into AbpVideoFaultsData (Evt,ParkingActId,PlateNumber,CreateTime,ParamStr) values ('" + dto.evt + "','" + dto.parkingActId + "','" + dto.plateNumber + "',getdate(),'" + JsonConvert.SerializeObject(dto).ToString() + "')";
                                SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, SqlFalse);
                            }
                        }
                        result.errorcode = 1;
                        result.message = "数据库回滚";
                        return result;
                    }
                    #endregion
                    result.errorcode = 0;
                    result.message = "";
                }
                else if (dto.evt == (int)VideoEqParkHighType.出场)
                {
                    //通过上次存的Guid，处理数据
                    DataTable dtLast = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $"select * from AbpVideoEquipBusinessDetail with(nolock) where State = '3' and Vid ='H{dto.parkingActId}' and VedioEqNumber='{dto.deviceSn}'").Tables[0];
                    string Guids = "";
                    if (dtLast.Rows.Count == 0)
                    {
                        DataTable dtLasts = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $"select * from AbpVideoEquipBusinessDetail with(nolock) where BerthNumber ='{dto.berthCode}' and PlateNumber='{dto.plateNumber}'").Tables[0];
                        if (dtLasts.Rows.Count > 0)
                        {
                            Guids = Convert.ToString(dtLasts.Rows[0]["guid"]);
                        }
                        else
                        {
                            CommonTools.WriteLogFile("无停车:" + dto.plateNumber);
                            result.errorcode = 1;
                            result.message = "无停车数据";
                            return result;
                        }
                    }
                    else {
                        Guids = Convert.ToString(dtLast.Rows[0]["guid"]);
                    }
                    
                    string OutURL = CommonTools.getimages(dto.picUrl, @"\VideoPic\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\" + DateTime.Now.ToString("dd"), "Out" + Guid.NewGuid().ToString());
                    string DetailURL = CommonTools.getimages(dto.plateNumberUrl, @"\VideoPic\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\" + DateTime.Now.ToString("dd"), "De" + Guid.NewGuid().ToString());

                    #region 车辆出场，更新出场时间，停车时间，金额

                    //以下代码复制过来
                    DataTable dtTemp = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $@"select Id,CarInTime, BerthsecId, PlateNumber, ParkId, CompanyId from AbpVideoEquipBusinessDetail with(nolock) where [guid] = '{Guids}'").Tables[0];
                    int? StopTime = null;
                    decimal Receivable = 0;
                    if (dtTemp.Rows.Count > 0 && string.IsNullOrWhiteSpace(dtTemp.Rows[0]["CarInTime"].ToString()) == false && string.IsNullOrWhiteSpace(dtTemp.Rows[0]["BerthsecId"].ToString()) == false)
                    {
                        var model = _rateAppService.RateCalculate(int.Parse(dtTemp.Rows[0]["BerthsecId"].ToString()), DateTime.Parse(dtTemp.Rows[0]["CarInTime"].ToString()), Convert.ToDateTime(hapenTime), 2, 0, int.Parse(dtTemp.Rows[0]["ParkId"].ToString()), dtTemp.Rows[0]["PlateNumber"].ToString(), int.Parse(dtTemp.Rows[0]["CompanyId"].ToString()));
                        StopTime = (int?)model.ParkTime;
                        Receivable = model.CalculateMoney;
                    }
                    int AuditStatus = 1;
                    if (Convert.ToString(dtTemp.Rows[0]["PlateNumber"])!= dto.plateNumber) {
                        sql += $"update AbpVideoEquipBusinessDetail set AuditStatus=2 where [guid] = '{Guids}';";
                        ErrorMsg += "出入场车牌不一致";
                    }
                    if (string.IsNullOrEmpty(DetailURL))
                    {
                        sql += $"update AbpVideoEquipBusinessDetail set [Receivable] = '{Receivable}' ,State = 1, PlateNumberOut= '{dto.plateNumber}',CarOutTime='{hapenTime}',StopTime =datediff(minute,CarInTime,'{hapenTime}'), OutOssPathURL='{OutURL}',ErrorMsg='{ErrorMsg}' where [guid] = '{Guids}';";
                    }
                    else
                    {
                        sql += $"update AbpVideoEquipBusinessDetail set [Receivable] = '{Receivable}' ,State = 1, PlateNumberOut= '{dto.plateNumber}',CarOutTime='{hapenTime}',DetailOssPathURL='{DetailURL}',StopTime =datediff(minute,CarInTime,'{hapenTime}'), OutOssPathURL='{OutURL}',ErrorMsg='{ErrorMsg}' where [guid] = '{Guids}';";
                    }
                    #endregion

                    #region 更新视频设备配置表
                    sql += $"update AbpVideoEquips set  ParkStatus = 0,IsOnlineValue=1,BeatDatetime = GETDATE() where BerthNumber = '{dto.berthCode}';";
                    #endregion

                    #region 更新泊位表
                    //判断泊位号是否存在
                    DataTable dtBer = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $"select * from AbpBerths with(nolock) where Id = {Convert.ToInt32(dt.Rows[0]["BerthId"])}").Tables[0];
                    if (dtBer.Rows.Count != 1)
                    {
                        CommonTools.WriteLogFile("泊位号不存在，出场失败");
                        result.errorcode = 1;
                        result.message = "泊位号不存在，出场失败";
                        return result;
                    }
                    DataTable dtBerD = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $"select * from AbpBerthsecs with(nolock) where Id = {Convert.ToInt32(dtBer.Rows[0]["BerthsecId"])}").Tables[0];
                    if (dtBerD.Rows.Count != 1)
                    {
                        CommonTools.WriteLogFile("泊位段不存在，出场失败");
                        result.errorcode = 1;
                        result.message = "泊位段不存在，出场失败";
                        return result;
                    }
                    sql += $"update AbpBerths set IsSourceVideo = 0,RelateNumber = '{dto.plateNumber}',OutCarTime = '{hapenTime}', ParkStatus = 0,BerthStatus='2',CarType='{getStopType(dto.plateColor)}' where Id = {dt.Rows[0]["BerthId"]};";
                    #endregion

                    #region 更新收费明细表
                    //判断收费明细是否存在
                    DataTable dtBusD = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $"select Id,[Status] from AbpBusinessDetail with(nolock) where guid = '{Convert.ToString(dtLast.Rows[0]["guid"])}'").Tables[0];
                    if (dtBusD.Rows.Count != 1)
                    {
                        CommonTools.WriteLogFile("收费信息不存在，出场失败");
                    }
                    if (Receivable == 0)
                    {
                        if (StopTime == 0)
                        {
                            sql += $"update AbpBusinessDetail set [Status] = 2,[Receivable] = '{Receivable}' ,[Money] = '{Receivable}',[Arrearage] = '{Receivable}',CarOutTime='{hapenTime}',StopTime =datediff(minute,CarInTime,'{hapenTime}'),IsPay=1,StopType=7,PayStatus=0,OutOperaId=1,CarPayTime='{hapenTime}',OutDeviceCode = '{dto.deviceSn}' where [guid] = '{Convert.ToString(dtLast.Rows[0]["guid"])}';";
                        }
                        else
                        {
                            sql += $"update AbpBusinessDetail set [Status] = 2,[Receivable] = '{Receivable}' ,[Money] = '{Receivable}',[Arrearage] = '{Receivable}',CarOutTime='{hapenTime}',StopTime =datediff(minute,CarInTime,'{hapenTime}'),IsPay=1,StopType=2,PayStatus=0,OutOperaId=1,CarPayTime='{hapenTime}',OutDeviceCode = '{dto.deviceSn}' where [guid] = '{Convert.ToString(dtLast.Rows[0]["guid"])}';";
                        }
                    }
                    else
                    {
                        sql += $"update AbpBusinessDetail set [Status] = 3,[Receivable] = '{Receivable}' ,[Money] = '{Receivable}',[Arrearage] = '{Receivable}',CarOutTime='{hapenTime}',StopTime =datediff(minute,CarInTime,'{hapenTime}'),CarPayTime='{hapenTime}',OutOperaId=1,OutDeviceCode = '{dto.deviceSn}' where [guid] = '{Convert.ToString(dtLast.Rows[0]["guid"])}';";
                    }
                    #endregion
                    if (dto.actionCredible < dto.Trust) {
                        sql += $"update AbpBusinessDetail set IsDeleted=0 where [guid] = '{Convert.ToString(dtLast.Rows[0]["guid"])}';";//业务平台
                    }

                    #region 事务提交
                    bool flag = true;
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
                        flag = false;
                        CommonTools.WriteLogFile("ErrorSQL:" + sql.ToString() + ex.ToString());
                        tran.Rollback();
                        if (ex.ToString().Contains("0x80131904"))
                        {
                            string SqlFalseCount = "select Id from AbpVideoFaultsData with(nolock) where Evt='" + dto.evt + "' and ParkingActId='" + dto.parkingActId + "' and PlateNumber='" + dto.plateNumber + "' and IsDelete is null";
                            DataTable tablesFalseCount = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, SqlFalseCount);
                            if (tablesFalseCount.Rows.Count == 0)
                            {
                                string SqlFalse = "insert into AbpVideoFaultsData (Evt,ParkingActId,PlateNumber,CreateTime,ParamStr) values ('" + dto.evt + "','" + dto.parkingActId + "','" + dto.plateNumber + "',getdate(),'" + JsonConvert.SerializeObject(dto).ToString() + "')";
                                SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, SqlFalse);
                            }
                        }
                        result.errorcode = 1;
                        result.message = "数据库回滚";
                        return result;
                    }
                    #endregion
                    #region 微信推送
                    /*if (flag)
                    {
                        //微信推送 
                        _webChatAppService.SendCarOutMsg(Convert.ToInt32(dtLast.Rows[0]["TenantId"]), Convert.ToInt32(dtLast.Rows[0]["BerthsecId"]),
                            Convert.ToString(dtLast.Rows[0]["PlateNumber"]), Convert.ToString(dtLast.Rows[0]["BerthNumber"]), Convert.ToDouble(StopTime), Receivable, 0, "",
                            hapenTime, Convert.ToString(dtBerD.Rows[0]["BerthsecName"]), dtTemp.Rows[0]["CarInTime"].ToString());
                        PDAAppService _PDAAppService = new PDAAppService();
                        _PDAAppService.SendSms(Convert.ToString(dtLast.Rows[0]["PlateNumber"]), Convert.ToInt32(dtLast.Rows[0]["CompanyId"]), Convert.ToInt32(dtLast.Rows[0]["TenantId"]), "BlackCarOutModel",
                            hapenTime, Convert.ToString(dtLast.Rows[0]["BerthNumber"]), new BerthsecAppService().GetBerthsecInfo(Convert.ToInt32(dtLast.Rows[0]["BerthsecId"])).BerthsecName);
                    }*/
                    #endregion
                    result.errorcode = 0;
                    result.message = "";
                }
                return result;
            }
            catch (Exception ex)
            {
                //记录日志
                JavaScriptSerializer serialize = new JavaScriptSerializer();
                CommonTools.WriteLogFile("入参：" /*+ serialize.Serialize(dto)*/ + "，Error" + ex.ToString());
                result.errorcode = 1;
                result.message = "接口异常，请联系管理员";
                return result;
            }
        }

        /// <summary>
        /// 视频设备报警数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public VideoEqParkHighRepose PushFaultDataForHigh(VideoEqParkHighRequest dto)
        {
            CommonTools.WriteLogFile("视频设备报警数据入库：" + JsonConvert.SerializeObject(dto).ToString());
            VideoEqParkHighRepose result = new VideoEqParkHighRepose();
            try
            {
                result.errorcode = 1;
                string sql = string.Empty;
                string guid = Guid.NewGuid().ToString();
                string hapenTime = CommonTools.getDateTime(Convert.ToInt64(dto.happenTime * 1000)).ToString("yyyy-MM-dd HH:mm:ss.fff");

                #region 校验入参
                TimeSpan ts = DateTime.Now - Convert.ToDateTime(hapenTime);
                if (ts.Days > 1)
                {
                    result.errorcode = 1;
                    result.message = "历史数据禁止推送";
                    return result;
                }
                #endregion

                #region 校验视频设备表
                DataTable dt1 = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $"select * from AbpVideoEquips with(nolock) where VedioEqNumber = '{dto.deviceSn}' and BerthNumber ='{dto.berthCode}'").Tables[0];

                if (dt1.Rows.Count < 1)
                {
                    CommonTools.WriteLogFile("泊位号和系统泊位号不匹配");
                    result.errorcode = 1;
                    result.message = "泊位号和系统泊位号不匹配";
                    return result;
                }

                #endregion

                if (dto.evt == 16)
                {
                    //异常推送
                    #region 插入异常数据表
                    string URL = CommonTools.getimages(dto.picUrl, @"\VideoPic\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\" + DateTime.Now.ToString("dd"), "Fa" + Guid.NewGuid().ToString());

                    sql += $@"INSERT INTO [dbo].[AbpVideoEquipFaults]
                            ([TenantId],[CompanyId],[RegionId],[ParkId],[BerthsecId],[BerthNumber],[BerthId],
                            [VedioEqNumber],[CreationTime],[VID] ,[Status],[Remark],[StatusTime],[OssPathURL])
                            VALUES
                           ({Convert.ToInt32(dt1.Rows[0]["TenantId"])},{Convert.ToInt32(dt1.Rows[0]["CompanyId"])},{Convert.ToInt32(dt1.Rows[0]["RegionId"])},
                            {Convert.ToInt32(dt1.Rows[0]["ParkId"])},{Convert.ToInt32(dt1.Rows[0]["BerthsecId"])},'{Convert.ToString(dt1.Rows[0]["BerthNumber"])}','{Convert.ToInt32(dt1.Rows[0]["BerthId"])}',
                           '{dto.deviceSn}','{DateTime.Now}','H{dto.parkingActId}','{dto.parkingAbnormalType}','{Convert.ToString((ParkingAbnormalType)dto.parkingAbnormalType)}','{hapenTime}','{URL}');";
                    #endregion

                    #region 更新泊位号状态为-1
                    //判断泊位号是否存在
                    DataTable dtBer = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $"select * from AbpBerths with(nolock) where Id = {Convert.ToInt32(dt1.Rows[0]["BerthId"])}").Tables[0];
                    if (dtBer.Rows.Count != 1)
                    {
                        CommonTools.WriteLogFile("泊位号不存在，入场失败");
                        result.errorcode = 1;
                        result.message = "泊位号不存在，入场失败";
                        return result;
                    }

                    sql += $"update AbpBerths set IsFaultFlag=1 where Id = {Convert.ToInt32(dt1.Rows[0]["BerthId"])};";
                    #endregion

                    #region 事务提交
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
                        CommonTools.WriteLogFile("ErrorSQL:" + sql.ToString() + ex.ToString());
                        tran.Rollback();
                        result.errorcode = 1;
                        result.message = "数据库回滚";
                        return result;
                    }
                    #endregion
                    result.errorcode = 0;
                    result.message = "";
                }
                return result;
            }
            catch (Exception ex)
            {
                //记录日志
                JavaScriptSerializer serialize = new JavaScriptSerializer();
                CommonTools.WriteLogFile("入参：" /*+ serialize.Serialize(dto)*/ + "，Error" + ex.ToString());
                result.errorcode = 1;
                result.message = "接口异常，请联系管理员";
                return result;
            }
        }

        /// <summary>
        /// 视频设备入场修正，通过唯一Id找到车牌号，更新
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public VideoEqParkHighRepose pushFixDataForHigh(VideoEqParkHighRequest dto)
        {
            CommonTools.WriteLogFile("视频设备修正数据入库：" + JsonConvert.SerializeObject(dto).ToString());
            VideoEqParkHighRepose result = new VideoEqParkHighRepose();
            try
            {
                result.errorcode = 1;
                string sql = string.Empty;
                string guid = Guid.NewGuid().ToString();


                #region 校验入参

                #endregion

                #region 校验停车数据表
                DataTable dtV = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $"select * from AbpVideoEquipBusinessDetail with(nolock) where VID ='H{dto.parkingActId}' and VedioEqNumber='{dto.deviceSn}'").Tables[0];

                if (dtV.Rows.Count < 1)
                {
                    CommonTools.WriteLogFile("没有停车数据");
                    result.errorcode = 1;
                    result.message = "没有停车数据";
                    return result;
                }
                #endregion

                #region 校验视频设备表
                //通过设备号查询视频设备表,设备号唯一
                DataTable dt = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $"select * from AbpVideoEquips with(nolock) where VedioEqNumber = '{dto.deviceSn}'").Tables[0];

                if (dt.Rows.Count < 1)
                {
                    CommonTools.WriteLogFile("视频设备未注册");
                    result.errorcode = 1;
                    result.message = "视频设备未注册";
                    return result;
                }

                DataTable dt1 = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $"select * from AbpVideoEquips with(nolock) where VedioEqNumber = '{dto.deviceSn}' and BerthNumber ='{dto.berthCode}'").Tables[0];

                if (dt1.Rows.Count < 1)
                {
                    CommonTools.WriteLogFile("泊位号和系统泊位号不匹配");
                    result.errorcode = 1;
                    result.message = "泊位号和系统泊位号不匹配";
                    return result;
                }
                #endregion

                if (dto.evt == 512)
                {
                    string FixURL = CommonTools.getimages(dto.picUrl, @"\VideoPic\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\" + DateTime.Now.ToString("dd"), "Fix" + Guid.NewGuid().ToString());
                    string DetailURL = CommonTools.getimages(dto.plateNumberUrl, @"\VideoPic\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\" + DateTime.Now.ToString("dd"), "De" + Guid.NewGuid().ToString());
                    if (string.IsNullOrEmpty(DetailURL))
                    {
                        sql += $"update AbpVideoEquipBusinessDetail set FixOssPathURL='{FixURL}',PlateNumber='{dto.plateNumber}',AuditStatus=2 where [guid] = '{Convert.ToString(dtV.Rows[0]["guid"])}';";
                    }
                    else
                    {
                        sql += $"update AbpVideoEquipBusinessDetail set FixOssPathURL='{FixURL}',DetailOssPathURL='{DetailURL}',PlateNumber='{dto.plateNumber}',AuditStatus=2 where [guid] = '{Convert.ToString(dtV.Rows[0]["guid"])}';";
                    }


                    #region 更新泊位表
                    //判断泊位号是否存在
                    DataTable dtBer = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $"select * from AbpBerths with(nolock) where Id = {Convert.ToInt32(dt1.Rows[0]["BerthId"])}").Tables[0];
                    if (dtBer.Rows.Count != 1)
                    {
                        CommonTools.WriteLogFile("泊位号不存在，出场失败");
                        result.errorcode = 1;
                        result.message = "泊位号不存在，出场失败";
                        return result;
                    }
                    sql += $"update AbpBerths set IsFaultFlag = 0 ,RelateNumber = '{dto.plateNumber}',CarType='{getStopType(dto.plateColor)}' where Id = {dt1.Rows[0]["BerthId"]};";
                    #endregion

                    #region 更新停车数据表
                    //判断收费明细是否存在
                    /*DataTable dtBusD = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $"select Id,[Status] from AbpBusinessDetail with(nolock) where guid = '{Convert.ToString(dtV.Rows[0]["guid"])}'").Tables[0];
                    if (dtBusD.Rows.Count != 1)
                    {
                        CommonTools.WriteLogFile("收费信息不存在，出场失败");
                        result.errorcode = 1;
                        result.message = "收费信息不存在，出场失败";
                        return result;
                    }
                    if (Convert.ToInt32(dtBusD.Rows[0]["Status"]) == 2 || Convert.ToInt32(dtBusD.Rows[0]["Status"]) == 4)//服务器数据状态
                    {
                        CommonTools.WriteLogFile("出场失败：该数据已出场！");
                        result.errorcode = 1;
                        result.message = "出场失败：该数据已出场！";
                        return result;
                    }
                    sql += $"update AbpBusinessDetail set PlateNumber ='{dto.plateNumber}' where [guid] = '{Convert.ToString(dtV.Rows[0]["guid"])}';";
                    */
                    #endregion

                    #region 事务提交
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
                        CommonTools.WriteLogFile("ErrorSQL:" + sql.ToString() + ex.ToString());
                        tran.Rollback();
                        return result;
                    }
                    #endregion
                    result.errorcode = 0;
                    result.message = "";
                }
                return result;
            }
            catch (Exception ex)
            {
                //记录日志
                JavaScriptSerializer serialize = new JavaScriptSerializer();
                CommonTools.WriteLogFile("入参：" /*+ serialize.Serialize(dto)*/ + "，Error" + ex.ToString());
                result.errorcode = 1;
                result.message = "请联系管理员！";
                return result;
            }
        }
        /// <summary>
        /// 视频设备停车数据补图片
        /// </summary>
        public VideoEqParkHighRepose pushPieceForHigh(VideoEqParkHighRequest dto)
        {
            VideoEqParkHighRepose result = new VideoEqParkHighRepose();
            CommonTools.WriteLogFile("视频设备补图片数据入库：" + JsonConvert.SerializeObject(dto).ToString());
            try
            {
                string sql = "";
                Cache cache = new Cache();
                var Times = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                foreach (DictionaryEntry ele in cache)
                {
                    try
                    {
                        var key = Convert.ToString(ele.Key);
                        var valArr = Convert.ToString(cache.Get(dto.deviceSn + dto.parkingActId)).Split(',');
                        if (Times - Convert.ToInt32(valArr[3]) > 1800)
                        {
                            //半小时过期
                            cache.Remove(key);
                        }
                    }
                    catch (Exception ex) { var e = ex; }
                }
                var val = cache.Get(dto.deviceSn + dto.parkingActId);
                if (val != null)
                {
                    var valArr = Convert.ToString(cache.Get(dto.deviceSn + dto.parkingActId)).Split(',');
                    sql = "select Id from AbpVideoEquipBusinessDetail where VID='H" + valArr[1] + "' and VedioEqNumber='" + valArr[0] + "'";
                    DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                    if (tables.Rows.Count > 0)
                    {
                        sql = "update AbpVideoEquipBusinessDetail set DetailOssPathURL='" + valArr[2] + "' where VID='H" + valArr[1] + "' and VedioEqNumber='" + valArr[0] + "'";
                        SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                        cache.Remove(dto.deviceSn + dto.parkingActId);
                    }
                }
                string DetailURL = CommonTools.getimages(dto.plateNumberUrl, @"\VideoPic\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\" + DateTime.Now.ToString("dd"), "De" + Guid.NewGuid().ToString());
                switch (dto.evt)
                {
                    case 2:
                        sql = "select Id from AbpVideoEquipBusinessDetail where VID='H" + dto.parkingActId + "' and VedioEqNumber='" + dto.deviceSn + "'";
                        DataTable TableDetail = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                        if (TableDetail.Rows.Count>0) {
                            string StopURL = CommonTools.getimages(dto.picUrl, @"\VideoPic\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\" + DateTime.Now.ToString("dd"), "Stop" + Guid.NewGuid().ToString());
                            if (string.IsNullOrEmpty(DetailURL))
                            {
                                sql = "update AbpVideoEquipBusinessDetail set StopPathURL='" + StopURL + "' where VID='H" + dto.parkingActId + "' and VedioEqNumber='" + dto.deviceSn + "'";
                            }
                            else
                            {
                                sql = "update AbpVideoEquipBusinessDetail set StopPathURL='" + StopURL + "',DetailOssPathURL='" + DetailURL + "' where VID='H" + dto.parkingActId + "' and VedioEqNumber='" + dto.deviceSn + "'";
                            }
                            SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                            sql = "select StopPathURL from AbpVideoEquipBusinessDetail where VID='H" + dto.parkingActId + "' and VedioEqNumber='" + dto.deviceSn + "'";
                            DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                            if (tables.Rows.Count > 0)
                            {
                                if (string.IsNullOrEmpty(Convert.ToString(tables.Rows[0][0])))
                                {
                                    CommonTools.WriteLogFile("pushPieceForHigh高位停车补图片：StopPathURL无数据H" + dto.parkingActId + "VedioEqNumber" + dto.deviceSn);
                                    result.errorcode = 1;
                                }
                                else
                                {
                                    result.errorcode = 0;
                                    result.message = "";
                                }
                            }
                            else
                            {
                                CommonTools.WriteLogFile("pushPieceForHigh高位停车补图片：StopPathURL无数据H" + dto.parkingActId + "VedioEqNumber" + dto.deviceSn);
                                result.errorcode = 1;
                            }
                        }
                        break;
                    case 8:
                        sql = "select Id from AbpVideoEquipBusinessDetail where VID='H" + dto.parkingActId + "' and VedioEqNumber='" + dto.deviceSn + "'";
                        DataTable TableDetails = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                        if (TableDetails.Rows.Count>0) {
                            string NoneURL = CommonTools.getimages(dto.picUrl, @"\VideoPic\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\" + DateTime.Now.ToString("dd"), "None" + Guid.NewGuid().ToString());
                            if (string.IsNullOrEmpty(DetailURL))
                            {
                                sql = "update AbpVideoEquipBusinessDetail set NonePathURL='" + NoneURL + "' where VID='H" + dto.parkingActId + "' and VedioEqNumber='" + dto.deviceSn + "'";
                            }
                            else
                            {
                                sql = "update AbpVideoEquipBusinessDetail set NonePathURL='" + NoneURL + "',DetailOssPathURL='" + DetailURL + "' where VID='H" + dto.parkingActId + "' and VedioEqNumber='" + dto.deviceSn + "'";
                            }
                            SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                            sql = "select NonePathURL from AbpVideoEquipBusinessDetail where VID='H" + dto.parkingActId + "' and VedioEqNumber='" + dto.deviceSn + "'";
                            DataTable tables2 = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                            if (tables2.Rows.Count > 0)
                            {
                                if (string.IsNullOrEmpty(Convert.ToString(tables2.Rows[0][0])))
                                {
                                    CommonTools.WriteLogFile("pushPieceForHigh高位停车补图片：NonePathURL无数据H" + dto.parkingActId + "VedioEqNumber" + dto.deviceSn);
                                    result.errorcode = 1;
                                }
                                else
                                {
                                    result.errorcode = 0;
                                    result.message = "";
                                }
                            }
                            else
                            {
                                CommonTools.WriteLogFile("pushPieceForHigh高位停车补图片：NonePathURL无数据H" + dto.parkingActId + "VedioEqNumber" + dto.deviceSn);
                                result.errorcode = 1;
                            }
                        }
                        break;
                    case 256:
                        sql = "select Id from AbpVideoEquipBusinessDetail where VID='H" + dto.parkingActId + "' and VedioEqNumber='" + dto.deviceSn + "'";
                        DataTable TableDetailXZ = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                        if (TableDetailXZ.Rows.Count > 0) {
                            if (!string.IsNullOrEmpty(DetailURL))
                            {
                                sql = "update AbpVideoEquipBusinessDetail set DetailOssPathURL='" + DetailURL + "' where VID='H" + dto.parkingActId + "' and VedioEqNumber='" + dto.deviceSn + "'";
                                SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                            }
                        }
                        sql = "select Id from AbpVideoEquipBusinessDetail where VID='H" + dto.parkingActId + "' and VedioEqNumber='" + dto.deviceSn + "'";
                        DataTable tables3 = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                        if (tables3.Rows.Count > 0)
                        {
                            if (string.IsNullOrEmpty(Convert.ToString(tables3.Rows[0][0])))
                            {
                                CommonTools.WriteLogFile("pushPieceForHigh高位停车补图片：256无数据H" + dto.parkingActId + "VedioEqNumber" + dto.deviceSn);
                                result.errorcode = 1;
                            }
                            else
                            {
                                result.errorcode = 0;
                                result.message = "";
                            }
                        }
                        else
                        {
                            CommonTools.WriteLogFile("pushPieceForHigh高位停车补图片：256无数据H" + dto.parkingActId + "VedioEqNumber" + dto.deviceSn);
                            result.errorcode = 1;
                        }

                        break;
                    case 128:
                        if (!string.IsNullOrEmpty(DetailURL))
                        {
                            cache.Insert(dto.deviceSn + dto.parkingActId, dto.deviceSn + "," + dto.parkingActId + "," + DetailURL + "," + Times);
                        }
                        break;
                }
                result.errorcode = 0;
                result.message = "";
            }
            catch (Exception ex)
            {
                //记录日志
                CommonTools.WriteLogFile("pushPieceForHigh：Error" + ex.ToString());
                result.errorcode = 1;
            }
            return result;
        }
        /// <summary>
        /// 视频设备抓拍
        /// </summary>
        public VideoEqParkHighRepose pushCaptureForHigh(VideoEqParkHighRequest dto)
        {
            CommonTools.WriteLogFile("视频设备抓拍数据入库：" + JsonConvert.SerializeObject(dto).ToString());
            VideoEqParkHighRepose result = new VideoEqParkHighRepose();
            try
            {
                string hapenTime = CommonTools.getDateTime(Convert.ToInt64(dto.happenTime * 1000)).ToString("yyyy-MM-dd HH:mm:ss.fff");
                result.errorcode = 0;
                result.message = "";
                string sql = "";
                sql = "select ParkId,TenantId,CompanyId,RegionId,BerthsecId,BerthId from AbpVideoEquips where BerthNumber='" + dto.berthCode + "' and VedioEqNumber='" + dto.deviceSn + "'";
                DataTable table2 = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                if (table2.Rows.Count>0) {
                    string LocalUrl = CommonTools.getimages(Convert.ToString(dto.picUrl), @"\VideoPic\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\" + DateTime.Now.ToString("dd"), dto.deviceSn + "-" + Guid.NewGuid().ToString().Replace("-", ""));
                    sql = "insert into AbpVideoCapture (ParkId,TenantId,CompanyId,RegionId,BerthsecId,BerthId,HappenTime,PicUrl,BerthNumber,VedioEqNumber,LocalUrl) values ('" + table2.Rows[0][0] + "','" + table2.Rows[0][1] + "','" + table2.Rows[0][2] + "','" + table2.Rows[0][3] + "','" + table2.Rows[0][4] + "','" + table2.Rows[0][5] + "','" + hapenTime + "','" + dto.picUrl + "','" + dto.berthCode + "','" + dto.deviceSn + "','" + LocalUrl + "')";
                    SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                }   
                return result;
            }
            catch (Exception ex)
            {
                //记录日志
                JavaScriptSerializer serialize = new JavaScriptSerializer();
                CommonTools.WriteLogFile("入参：" /*+ serialize.Serialize(dto)*/ + "，Error" + ex.ToString());
                result.errorcode = 1;
                result.message = "请联系管理员！";
                return result;
            }
        }
    }
}

