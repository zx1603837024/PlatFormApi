using Aop.Api.Domain;
using CommonTool;
using F2.Application.Parkade.Dtos;
using F2.Application.PDA;
using F2.Application.Rates;
using F2.Application.WebChat;
using F2.Common;
using F2.Core.Extensions;
using F2.Core.Extensions.DataExtend;
using F2.Core.Extensions.Models;
using Flurl;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Web.UI.WebControls;
using F2.Core.Extensions.Log;
using static SKIT.FlurlHttpClient.Wechat.TenpayV3.Models.DepositMarketingMemberCardOpenCardCodesResponse.Types;
using static SKIT.FlurlHttpClient.Wechat.TenpayV3.Models.UpdateMarketingMemberCardOpenCardRightsRequest.Types;

namespace F2.Application.Parkade
{
    public class ParkadeService : IParkadeService
    {
        #region Var
        private readonly IWebChatAppService _webChatAppService;
        private readonly IRateAppService _rateAppService;
        #endregion
        public ParkadeService()
        {
            _webChatAppService = new WebChatAppService();
            _rateAppService = new RateAppService();
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
        /// 车牌识别结果对接
        /// </summary>
        /// <param name="dto"></param>
        public Hashtable PlateResultReceive(ParkadeRequest dto) {
            Commons com = new Commons();
            Hashtable res = new Hashtable();
            Hashtable Response = new Hashtable();
            List<Hashtable> LserialData = new List<Hashtable>();
            Hashtable serialData = new Hashtable();
            Response.Add("info", "no");
            serialData.Add("serialChannel", 0);
            serialData.Add("data", "");
            serialData.Add("dataLen", 0);
            try
            {
                string sql = "";
                string sq = "";
                var guid = Guid.NewGuid().ToString();
                string hapenTime = CommonTools.getDateTime(Convert.ToInt64(dto.sec * 1000)).ToString("yyyy-MM-dd HH:mm:ss");
                sq = "select a.EquipmentNumber,a.PassageId,b.PassageType,c.Id BerthsecsId,c.ParkId,c.TenantId,c.CompanyId,c.RegionId from AbpParkadeEquipment a,AbpParkadePassage b,AbpBerthsecs c where a.PassageId=b.Id and b.BerthsecsId=c.Id and a.EquipmentNumber='" + dto.serialno + "'";
                DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sq);
                #region 校验
                if (tables.Rows.Count == 0)
                {
                    Logger.Log.Info("PlateResultReceive:停车场设备信息不存在");
                    LserialData.Add(serialData);
                    Response.Add("serialData", LserialData);
                    res.Add("Response_AlarmInfoPlate", Response);
                    return res;
                }
                TimeSpan ts = DateTime.Now - Convert.ToDateTime(hapenTime);
                if (ts.Days >= 1)
                {
                    Logger.Log.Info("PlateResultReceive:历史数据禁止推送");
                    LserialData.Add(serialData);
                    Response.Add("serialData", LserialData);
                    res.Add("Response_AlarmInfoPlate", Response);
                    return res;
                }
                #endregion
                #region 入场
                if (Convert.ToInt32(tables.Rows[0]["PassageType"]) == 1) {
                    string RelateNumber = "";
                    //剩余车位校验
                    sq = "select Id from AbpMonthlyCars where PlateNumber='" + dto.license + "' and EndTime>=getdate()";
                    DataTable MonthCar = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sq);
                    if (MonthCar.Rows.Count==0) {
                        sq = "select count(*) num from AbpBerths where BerthsecId='" + tables.Rows[0]["BerthsecsId"] + "' and BerthStatus=2";
                        DataTable AllBerth = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sq);
                        sq = "select count(*) num from AbpMonthlyCars where EndTime>=getdate() and ParkIds='" + tables.Rows[0]["ParkId"] + "'";
                        DataTable AllMonthCar = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sq);
                        sq = "select Id from AbpMonthlyCars where PlateNumber='" + dto.license + "' and EndTime>=getdate()";
                        if (Convert.ToInt32(AllBerth.Rows[0]["num"]) - Convert.ToInt32(AllMonthCar.Rows[0]["num"]) <= 0)
                        {
                            Logger.Log.Info("PlateResultReceive:停车场车位已满");
                            sql = $"update AbpParkadeEquipment set State=2,IsOnlineValue=1,BeatDatetime = GETDATE() where EquipmentNumber ='" + dto.serialno + "'";
                            SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                            string[] show = { "非常抱歉", dto.license, "车位已满，请您驶离"};
                            var bytes = com.ScreenShow(show);
                            serialData["data"] = Convert.ToBase64String(bytes);
                            serialData["dataLen"] = bytes.Length;
                            LserialData.Add(serialData);
                            Response.Add("serialData", LserialData);
                            res.Add("Response_AlarmInfoPlate", Response);
                            return res;
                        }
                    }
                    #region 重复入场校验
                    sq = "select Id from AbpParkadeAccessDetail where PlateNumber='" + dto.license + "' and CarInTime>='" + hapenTime + "' and CarInTime<'" + CommonTools.getDateTime(Convert.ToInt64(dto.sec * 1000)).AddSeconds(5).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    DataTable OsOrders = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sq);
                    if (OsOrders.Rows.Count > 0)
                    {
                        Logger.Log.Info("PlateResultReceive:重复入场");
                        LserialData.Add(serialData);
                        Response.Add("serialData", LserialData);
                        res.Add("Response_AlarmInfoPlate", Response);
                        return res;
                    }
                    sq = "select Id from AbpBerths where RelateNumber='" + dto.license + "' and BerthStatus=1";
                    DataTable OsOrder = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sq);
                    if (OsOrder.Rows.Count>0) {
                        Logger.Log.Info("PlateResultReceive:重复入场");
                        string[] show = { "非常抱歉", dto.license, "重复入场，请您驶离" };
                        var bytes = com.ScreenShow(show);
                        serialData["data"] = Convert.ToBase64String(bytes);
                        serialData["dataLen"] = bytes.Length;
                        LserialData.Add(serialData);
                        Response.Add("serialData", LserialData);
                        res.Add("Response_AlarmInfoPlate", Response);
                        return res;
                    }
                    #endregion
                    #region 无牌车
                    if (dto.license == "_无_")
                    {
                        Logger.Log.Info("无牌车入场");
                        RelateNumber = "无牌车";
                        string[] show = { "无牌车请扫码入场" };
                        var bytes = com.ScreenShow(show);
                        serialData["data"] = Convert.ToBase64String(bytes);
                        serialData["dataLen"] = bytes.Length;
                        LserialData.Add(serialData);
                        Response.Add("serialData", LserialData);
                        res.Add("Response_AlarmInfoPlate", Response);
                        return res;
                    }
                    else {
                        RelateNumber = dto.license;
                        sql += "update AbpBerths set RelateNumber = '" + RelateNumber + "',InCarTime = '" + hapenTime + "', OutCarTime = null, guid = '" + guid + "', ParkStatus = 1,BerthStatus='1',CarType='" + getStopType(dto.colorType) + "' where Id=(select top 1 Id from AbpBerths where BerthStatus=2 and ParkId='" + tables.Rows[0]["ParkId"] + "')";
                        sql += "update AbpParks set OccupyCount=isnull(OccupyCount,0)+1 where Id='" + tables.Rows[0]["ParkId"] + "'";
                    }
                    #endregion
                    string OssPathURL = CommonTools.getimages(dto.imageFile, @"\Parkade\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\" + DateTime.Now.ToString("dd"), "In" + DateTime.Now.ToString("yyMMddHHMMssfff"));
                    string DetailOssPathURL = CommonTools.getimages(dto.imageFragmentFile, @"\Parkade\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\" + DateTime.Now.ToString("dd"), "DeIn" + DateTime.Now.ToString("yyMMddHHMMssfff"));
                    sql += "update AbpParkadeEquipment set State=2,IsOnlineValue=1,BeatDatetime = GETDATE() where EquipmentNumber ='"+dto.serialno+"';";
                    sql += "insert into AbpParkadeAccessDetail (ParkId,BerthsecId,InEquipmentNumber,PlateNumber,CarInTime,guid,OssPathURL,DetailOssPathURL,CreationTime,CarType,TenantId,CompanyId) values ('" + tables.Rows[0]["ParkId"] + "','"+ tables.Rows[0]["BerthsecsId"] + "','"+ dto.serialno + "','"+ RelateNumber + "','"+ hapenTime + "','"+ guid + "','"+ OssPathURL + "','"+ DetailOssPathURL + "','"+ DateTime.Now + "','"+ getStopType(dto.colorType) + "','"+ tables.Rows[0]["TenantId"] + "','"+ tables.Rows[0]["CompanyId"] + "')";
                    sql += "insert into AbpBusinessDetail(PlateNumber, CarType, Prepaid, CarInTime, InOperaId, InDeviceCode, guid, StopType, RegionId, ParkId, BerthsecId, Status, PrepaidCarNo, PrepaidPayStatus, Receivable, FactReceive, Arrearage, PaymentType, EscapePayStatus, IsEscapePay, PayStatus, IsPay, FeeType, TenantId, CompanyId, IsLock, IsDeleted, CreationTime, CreatorUserId, InBatchNo, SensorsInCarTime) values ('"+ RelateNumber + "', '"+getStopType(dto.colorType)+"', 0, '"+hapenTime+"',1,'"+dto.serialno+"', '"+guid+"', '1','"+ tables.Rows[0]["RegionId"] + "', '"+ tables.Rows[0]["ParkId"] + "','"+ tables.Rows[0]["BerthsecsId"] + "' ,1, '', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, '"+ tables.Rows[0]["TenantId"] + "', '"+ tables.Rows[0]["CompanyId"] + "', 0, 0, getdate(), NULL, NULL, NULL)";
                    sql += "delete from  AbpParkadePassRecord where PassageId = '" + tables.Rows[0]["PassageId"] + "';";
                    sql += "insert into AbpParkadePassRecord(PassageId, PlateNumber, CreationTime, guid) values ('" + tables.Rows[0]["PassageId"] + "', '" + RelateNumber + "', getdate(), '" + guid + "')";
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
                        #region 无牌车提示
                        if (dto.license == "_无_")
                        {
                            string[] show = { "无牌车", "请您扫码入场", "文明驾驶，秩序停车" };
                            var bytes = com.ScreenShow(show);
                            serialData["data"] = Convert.ToBase64String(bytes);
                            serialData["dataLen"] = bytes.Length;
                        }
                        else
                        {
                            string[] show = { "欢迎光临", dto.license, "文明驾驶，秩序停车" };
                            Response["info"] = "ok";
                            var bytes = com.ScreenShow(show);
                            serialData["data"] = Convert.ToBase64String(bytes);
                            serialData["dataLen"] = bytes.Length;
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        flag = false;
                        Logger.Log.Error("PlateResultReceive:ErrorSQL:" + sql.ToString() + ex.ToString());
                        tran.Rollback();
                    }
                    #endregion
                }
                #endregion
                #region 出场
                if (Convert.ToInt32(tables.Rows[0]["PassageType"]) == 2)
                {
                    if (dto.license == "_无_")
                    {
                        sql += "delete from  AbpParkadePassRecord where PassageId = '" + tables.Rows[0]["PassageId"] + "';";
                        sql += "insert into AbpParkadePassRecord(PassageId, PlateNumber, CreationTime) values ('" + tables.Rows[0]["PassageId"] + "', '无牌车', getdate())";
                        string[] show = { "无牌车", "请您扫码支付", "谢谢配合" };
                        var bytes = com.ScreenShow(show);
                        serialData["data"] = Convert.ToBase64String(bytes);
                        serialData["dataLen"] = bytes.Length;
                    }
                    else
                    {
                        int StopTime = 0;
                        decimal Receivable = 0;
                        sq = "select CarInTime,CarType,guid from AbpParkadeAccessDetail where PlateNumber='" + dto.license + "' and CarOutTime is null";
                        DataTable TabDetail = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sq);
                        if (TabDetail.Rows.Count == 0)
                        {
                            Logger.Log.Info("PlateResultReceive:停车信息不存在");
                            LserialData.Add(serialData);
                            Response.Add("serialData", LserialData);
                            res.Add("Response_AlarmInfoPlate", Response);
                            return res;
                        }
                        else
                        {
                            sq = "select * from AbpBusinessDetail where guid='" + TabDetail.Rows[0]["guid"] + "'";
                            DataTable TabBusiness = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sq);
                            if (TabBusiness.Rows.Count == 0)
                            {
                                Logger.Log.Info("PlateResultReceive:停车信息不存在");
                                LserialData.Add(serialData);
                                Response.Add("serialData", LserialData);
                                res.Add("Response_AlarmInfoPlate", Response);
                                return res;
                            }
                            var model = _rateAppService.RateCalculate(int.Parse(tables.Rows[0]["BerthsecsId"].ToString()), DateTime.Parse(TabDetail.Rows[0]["CarInTime"].ToString()), Convert.ToDateTime(hapenTime), int.Parse(TabDetail.Rows[0]["CarType"].ToString()), 1, int.Parse(tables.Rows[0]["ParkId"].ToString()), dto.license, int.Parse(tables.Rows[0]["CompanyId"].ToString()));
                            StopTime = (int)model.ParkTime;
                            Receivable = model.CalculateMoney;
                            string OutOssPathURL = CommonTools.getimages(dto.imageFile, @"\Parkade\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\" + DateTime.Now.ToString("dd"), "Out" + DateTime.Now.ToString("yyMMddHHMMssfff"));
                            string DetailOutOssPathURL = CommonTools.getimages(dto.imageFragmentFile, @"\Parkade\" + DateTime.Now.ToString("yyyy") + @"\" + DateTime.Now.ToString("MM") + @"\" + DateTime.Now.ToString("dd"), "DeOut" + DateTime.Now.ToString("yyMMddHHMMssfff"));
                            if (Convert.ToInt32(TabBusiness.Rows[0]["Status"]) == 6)
                            {
                                if ((Convert.ToDateTime(hapenTime) - Convert.ToDateTime(TabBusiness.Rows[0]["CarPayTime"].ToString())).Minutes > 15)//超时
                                {
                                    sql += "update AbpParkadeAccessDetail set OutEquipmentNumber='" + dto.serialno + "', OutOssPathURL = '" + OutOssPathURL + "', DetailOutOssPathURL = '" + DetailOutOssPathURL + "' where guid = '" + TabDetail.Rows[0]["guid"] + "';";
                                    sql += "update AbpBusinessDetail set Status = 1 where guid = '" + TabDetail.Rows[0]["guid"] + "';";
                                    sql += "delete from  AbpParkadePassRecord where PassageId = '" + tables.Rows[0]["PassageId"] + "';";
                                    sql += "insert into AbpParkadePassRecord(PassageId, PlateNumber, CreationTime, guid) values ('" + tables.Rows[0]["PassageId"] + "', '" + dto.license + "', getdate(), '" + TabDetail.Rows[0]["guid"] + "')";
                                    string[] show = { dto.license, "请您缴费", Convert.ToString(Receivable) + "元" };
                                    var bytes = com.ScreenShow(show);
                                    serialData["data"] = Convert.ToBase64String(bytes);
                                    serialData["dataLen"] = bytes.Length;
                                }
                                else
                                {
                                    sql += "update AbpParkadeEquipment set IsOnlineValue=1,BeatDatetime = GETDATE() where EquipmentNumber ='" + dto.serialno + "';";
                                    sql += "update AbpBerths set OutCarTime = '" + hapenTime + "',RelateNumber = null, ParkStatus = 0,BerthStatus='2' where guid = '" + TabDetail.Rows[0]["guid"] + "';";
                                    sql += "update AbpParkadeAccessDetail set OutEquipmentNumber='" + dto.serialno + "', CarOutTime = '" + hapenTime + "', OutOssPathURL = '" + OutOssPathURL + "', DetailOutOssPathURL = '" + DetailOutOssPathURL + "' where guid = '" + TabDetail.Rows[0]["guid"] + "';";
                                    sql += "update AbpBusinessDetail set Status = 7,Money = Receivable,FactReceive=Receivable,Arrearage= 0,CarOutTime = '" + hapenTime + "',StopTime = datediff(minute, CarInTime, CarPayTime),IsPay = 1,StopType = 1,PayStatus = 0,OutOperaId = 1,OutDeviceCode = '" + dto.serialno + "' where guid = '" + TabDetail.Rows[0]["guid"] + "';";
                                    sql += "update AbpParks set OccupyCount=OccupyCount-1 where Id='" + tables.Rows[0]["ParkId"] + "'";
                                    Response["info"] = "ok";
                                    string[] show = { "请您通行", dto.license, "小心驾驶，一路顺风" };
                                    var bytes = com.ScreenShow(show);
                                    serialData["data"] = Convert.ToBase64String(bytes);
                                    serialData["dataLen"] = bytes.Length;
                                }
                            }
                            else
                            {
                                if (Receivable == 0)
                                {
                                    sql += "update AbpParkadeEquipment set IsOnlineValue=1,BeatDatetime = GETDATE() where EquipmentNumber ='" + dto.serialno + "';";
                                    sql += "update AbpBerths set OutCarTime = '" + hapenTime + "',RelateNumber = null, ParkStatus = 0,BerthStatus='2' where guid = '" + TabDetail.Rows[0]["guid"] + "';";
                                    sql += "update AbpParkadeAccessDetail set OutEquipmentNumber='" + dto.serialno + "', CarOutTime = '" + hapenTime + "', OutOssPathURL = '" + OutOssPathURL + "', DetailOutOssPathURL = '" + DetailOutOssPathURL + "' where guid = '" + TabDetail.Rows[0]["guid"] + "';";
                                    if (StopTime == 0)
                                    {
                                        sql += $"update AbpBusinessDetail set [Status] = 2,[Receivable] = '{Receivable}' ,[Money] = '{Receivable}',[Arrearage] = '{Receivable}',CarOutTime='{hapenTime}',StopTime =datediff(minute,CarInTime,'{hapenTime}'),IsPay=1,StopType=7,PayStatus=0,OutOperaId=1,CarPayTime='{hapenTime}',OutDeviceCode = '{dto.serialno}' where [guid] = '{Convert.ToString(TabDetail.Rows[0]["guid"])}';";
                                    }
                                    else
                                    {
                                        sql += $"update AbpBusinessDetail set [Status] = 2,[Receivable] = '{Receivable}' ,[Money] = '{Receivable}',[Arrearage] = '{Receivable}',CarOutTime='{hapenTime}',StopTime =datediff(minute,CarInTime,'{hapenTime}'),IsPay=1,StopType=2,PayStatus=0,OutOperaId=1,CarPayTime='{hapenTime}',OutDeviceCode = '{dto.serialno}' where [guid] = '{Convert.ToString(TabDetail.Rows[0]["guid"])}';";
                                    }
                                    sql += "update AbpParks set OccupyCount=OccupyCount-1 where Id='" + tables.Rows[0]["ParkId"] + "'";
                                    Response["info"] = "ok";
                                    string[] show = { "请您通行", dto.license, "小心驾驶，一路顺风" };
                                    var bytes = com.ScreenShow(show);
                                    serialData["data"] = Convert.ToBase64String(bytes);
                                    serialData["dataLen"] = bytes.Length;
                                }
                                else
                                {
                                    sql += "update AbpParkadeAccessDetail set OutEquipmentNumber='" + dto.serialno + "', OutOssPathURL = '" + OutOssPathURL + "', DetailOutOssPathURL = '" + DetailOutOssPathURL + "' where guid = '" + TabDetail.Rows[0]["guid"] + "';";
                                    sql += "insert into AbpParkadePassRecord(PassageId, PlateNumber, CreationTime, guid) values ('" + tables.Rows[0]["PassageId"] + "', '" + dto.license + "', getdate(), '" + TabDetail.Rows[0]["guid"] + "')";
                                    string[] show = { dto.license, "请您缴费", Convert.ToString(Receivable) + "元" };
                                    var bytes = com.ScreenShow(show);
                                    serialData["data"] = Convert.ToBase64String(bytes);
                                    serialData["dataLen"] = bytes.Length;
                                }
                            }
                        }
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
                        Logger.Log.Error("PlateResultReceive:ErrorSQL:" + sql.ToString() + ex.ToString());
                        tran.Rollback();
                    }
                    #endregion
                }
                #endregion
            }
            catch (Exception ex) {
                Logger.Log.Error("PlateResultReceive:Error:" + ex.ToString());
            }
            LserialData.Add(serialData);
            Response.Add("serialData", LserialData);
            res.Add("Response_AlarmInfoPlate", Response);
            return res;
        }
        /// <summary>
        /// 心跳数据对接
        /// </summary>
        /// <param name="dto"></param>
        public Hashtable HeartBeatReceive(ParkadeRequest dto) {
            Hashtable res = new Hashtable();
            try
            {
                var serialno = dto.serialno;
                Commons com = new Commons();
                string sql = "";
                sql = "select State,PassageId from AbpParkadeEquipment with(nolock) where State is not null and EquipmentNumber='" + serialno + "'";
                DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                if (tables.Rows.Count>0) {
                    if (Convert.ToInt32(tables.Rows[0]["State"]) == 1) 
                    { //开闸
                        Hashtable Response = new Hashtable();
                        Response.Add("info", "ok");
                        res.Add("Response_AlarmInfoPlate", Response);
                        sql= "update AbpParkadeEquipment set State=null where EquipmentNumber='" + serialno +"'";
                        SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                    }
                    if (Convert.ToInt32(tables.Rows[0]["State"]) == 2)
                    { //道闸清屏显示
                        sql = "select CONVERT(VARCHAR(20),CarInTime,120) as CarInTime from AbpParkadeAccessDetail with(nolock) where InEquipmentNumber ='" + serialno + "' order by CarInTime desc";
                        DataTable TablesDetail = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                        if (TablesDetail.Rows.Count>0) {
                            if ((DateTime.Now - Convert.ToDateTime(TablesDetail.Rows[0]["CarInTime"])).Seconds>10) {
                                sql = "select b.BerthCount-isnull(b.OccupyCount,0) Bnum from AbpParkadePassage a,AbpParks b where a.ParkId=b.Id and a.Id='"+ (tables.Rows[0]["PassageId"]) + "'";
                                DataTable TablesPark = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                                if (TablesPark.Rows.Count>0) {
                                    var bytes = com.ShowRemainder(Convert.ToInt32(TablesPark.Rows[0]["Bnum"]));
                                    Hashtable Response = new Hashtable();
                                    List<Hashtable> LserialData = new List<Hashtable>();
                                    Hashtable serialData = new Hashtable();
                                    serialData.Add("serialChannel", 0);
                                    serialData.Add("data", Convert.ToBase64String(bytes));
                                    serialData.Add("dataLen", bytes.Length);
                                    LserialData.Add(serialData);
                                    Response.Add("serialData", LserialData);
                                    res.Add("Response_AlarmInfoPlate", Response);
                                    sql = "update AbpParkadeEquipment set State=null where EquipmentNumber='" + serialno + "'";
                                    SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                                }
                            }
                        } 
                    }
                }
                sql = $"update AbpParkadeEquipment set IsOnlineValue=0 where BeatDatetime<'"+DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss") +"'";
                SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("HeartBeatReceive:Error:" + ex.ToString());
                res.Add("Response_AlarmInfoPlate",null);
            }
            return res;
        }
        /// <summary>
        /// 开闸服务
        /// </summary>
        /// <param name="dto"></param>
        public Hashtable OpenPoleService(Hashtable dto) {
            Hashtable res = new Hashtable();
            try
            {
                if (dto.ContainsKey("PassageId"))
                {
                    string sql = "select EquipmentNumber from AbpParkadeEquipment where PassageId='" + dto["PassageId"] + "'";
                    DataTable table = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                    if (table.Rows.Count > 0)
                    {
                        sql = "update AbpParkadeEquipment set State=1 where EquipmentNumber='" + table.Rows[0]["EquipmentNumber"] + "'";
                        SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                        res.Add("result", "success");
                    }
                    else
                    {
                        res.Add("result", "false");
                    }
                }
                else
                {
                    string sql = "select EquipmentNumber from AbpParkadeEquipment where EquipmentNumber='" + dto["EquipmentNumber"] + "'";
                    DataTable table = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
                    if (table.Rows.Count > 0)
                    {
                        sql = "update AbpParkadeEquipment set State=1 where EquipmentNumber='" + dto["EquipmentNumber"] + "'";
                        SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                        res.Add("result", "success");
                    }
                    else
                    {
                        res.Add("result", "false");
                    }
                }
            }
            catch (Exception ex)
            {
                res.Add("result", "false");
                res.Add("msg", ex.ToString());
            }
            return res;
        }
        //MQ开闸回调
        public Hashtable OpenPoleCallBack(Hashtable dto) {
            Hashtable res = new Hashtable();
            try {
                string sql = "update AbpPatrolOpen set UpdateTime=GETDATE(),OptResultMsg='" + dto["OptResultMsg"] + "',OptResultCode='"+ dto["OptResultCode"] + "' where SerNum='" + dto["SerNum"] + "'";
                SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                res.Add("result", "success");
            }
            catch (Exception ex)
            {
                res.Add("result", "false");
                res.Add("msg", ex.ToString());
            }
            return res;
        }
    }
}
