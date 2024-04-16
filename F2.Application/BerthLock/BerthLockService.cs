using CommonTool;
using F2.Application.Parkade.Dtos;
using F2.Common;
using F2.Core.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F2.Core.Extensions.Log;
using F2.Application.BerthLock.Dtos;
using static SKIT.FlurlHttpClient.Wechat.TenpayV3.Models.DepositMarketingMemberCardOpenCardCodesResponse.Types;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace F2.Application.BerthLock
{
    public class BerthLockService: IBerthLockService
    {
        //上锁数据对接
        public BerthLockRespose LockDataReceive(BerthLockRequest dto) {
            Commons com = new Commons();
            string guid = Guid.NewGuid().ToString();
            BerthLockRespose response = new BerthLockRespose();
            response.result = true;
            response.message = "";
            try
            {
                string sql = "";
                string sq = "";
                string hapenTime = CommonTools.getDateTime(Convert.ToInt64(dto.happenTime * 1000)).ToString("yyyy-MM-dd HH:mm:ss");
                sq = "select a.TenantId,a.CompanyId,a.ParkId,a.BerthsecId,b.RegionId,a.BerthNumber,a.BerthLockEqNumber,a.BerthId from AbpBerthLockEquips a,AbpBerths b where a.BerthId=b.Id and a.BerthLockEqNumber='" + dto.SN + "'";
                DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sq);
                #region 校验
                if (tables.Rows.Count == 0)
                {
                    Logger.Log.Info("LockDataReceive:车位锁设备信息不存在");
                    response.result = false;
                    return response;
                }
                TimeSpan ts = DateTime.Now - Convert.ToDateTime(hapenTime);
                if (ts.Days >= 1)
                {
                    Logger.Log.Info("LockDataReceive:历史数据禁止推送");
                    response.result = false;
                    return response;
                }
                sql += $"update AbpBerths set StopType = 1,RelateNumber = '',InCarTime = '{hapenTime}', OutCarTime = null, [guid] = '{guid}', ParkStatus = 1,BerthStatus='1',CarType='' where Id = {Convert.ToInt32(tables.Rows[0]["BerthId"])};";
                sql += "insert into AbpBerthLockEquipsBusinessDetail (TenantId,CompanyId,BerthId,BerthLockEqNumber,BerthNumber,CarInTime,ParkId,BerthsecId,guid,RegionId,CreationTime) values ('" + tables.Rows[0]["TenantId"] + "','" + tables.Rows[0]["CompanyId"] + "','" + tables.Rows[0]["BerthId"] + "','" + tables.Rows[0]["BerthLockEqNumber"] + "','" + tables.Rows[0]["BerthNumber"] + "','" + hapenTime + "','" + tables.Rows[0]["ParkId"] + "','" + tables.Rows[0]["BerthsecId"] + "','" + guid + "','"+ tables.Rows[0]["RegionId"] + "',getdate());";
                sql += $@" insert into AbpBusinessDetail(BerthNumber, PlateNumber, CarType, Prepaid, CarInTime, InOperaId, InDeviceCode, guid, StopType, RegionId, ParkId, BerthsecId, 
                                    Status, PrepaidCarNo, PrepaidPayStatus, Receivable, FactReceive, Arrearage, PaymentType, EscapePayStatus, IsEscapePay, PayStatus, IsPay, FeeType, TenantId, 
                                    CompanyId, IsLock, IsDeleted, CreationTime, CreatorUserId, InBatchNo, SensorsInCarTime) values
                                    ('{Convert.ToString(tables.Rows[0]["BerthNumber"])}', '', '2', 0, '{hapenTime}',1,'{dto.SN}', '{guid}', '1', 
                                    {Convert.ToInt32(tables.Rows[0]["RegionId"])}, {Convert.ToInt32(tables.Rows[0]["ParkId"])}, {Convert.ToInt32(tables.Rows[0]["BerthsecId"])}, 
                                    1, '', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, {Convert.ToInt32(tables.Rows[0]["TenantId"])}, {Convert.ToInt32(tables.Rows[0]["CompanyId"])}, 0, 0, getdate(), NULL, NULL, NULL) ";
                //CarTyp默认2
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
                    response.result = false;
                    Logger.Log.Error("LockDataReceive:ErrorSQL:" + sql.ToString() + ex.ToString());
                    tran.Rollback();
                }
                #endregion
            }
            catch (Exception ex)
            {
                Logger.Log.Error("LockDataReceive:Error:" + ex.ToString());
            }
            return response;
        }
        //下锁数据对接
        public BerthLockRespose LockDownDataReceive(BerthLockRequest dto)
        {
            Commons com = new Commons();
            string guid = Guid.NewGuid().ToString();
            BerthLockRespose response = new BerthLockRespose();
            response.result = true;
            response.message = "";
            try
            {
                string sql = "";
                string sq = "";
                string hapenTime = CommonTools.getDateTime(Convert.ToInt64(dto.happenTime * 1000)).ToString("yyyy-MM-dd HH:mm:ss");
                sq = "select a.TenantId,a.CompanyId,a.ParkId,a.BerthsecId,b.RegionId,b.guid,a.BerthNumber,a.BerthLockEqNumber,a.BerthId from AbpBerthLockEquips a,AbpBerths b where a.BerthId=b.Id and a.BerthLockEqNumber='" + dto.SN + "'";
                DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sq);
                #region 校验
                if (tables.Rows.Count == 0)
                {
                    Logger.Log.Info("LockDownDataReceive:车位锁设备信息不存在");
                    response.result = false;
                    return response;
                }
                TimeSpan ts = DateTime.Now - Convert.ToDateTime(hapenTime);
                if (ts.Days >= 1)
                {
                    Logger.Log.Info("LockDownDataReceive:历史数据禁止推送");
                    response.result = false;
                    return response;
                }
                sql += $"update AbpBerths set RelateNumber = null,OutCarTime = '{hapenTime}', ParkStatus = 0,BerthStatus='2',CarType='' where Id = {tables.Rows[0]["BerthId"]};";
                sql += $"update AbpBerthLockEquipsBusinessDetail set CarOutTime = '{hapenTime}' where guid = '"+
                    tables.Rows[0]["guid"] + "';";
                sql += $"update AbpBusinessDetail set Status=2,CarOutTime = '{hapenTime}',StopTime = datediff(minute, CarInTime, '{hapenTime}'),OutOperaId = 1,OutDeviceCode = '{dto.SN}' where[guid] = '{Convert.ToString(tables.Rows[0]["guid"])}'";
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
                    response.result = false;
                    Logger.Log.Error("LockDownDataReceive:ErrorSQL:" + sql.ToString() + ex.ToString());
                    tran.Rollback();
                }
                #endregion
            }
            catch (Exception ex)
            {
                Logger.Log.Error("LockDownDataReceive:Error:" + ex.ToString());
            }
            return response;
        }
        //心跳数据对接
        public BerthLockRespose LockBeatDataReceive(BerthLockRequest dto) {
            BerthLockRespose response = new BerthLockRespose();
            response.result = true;
            response.message = "";
            try
            {
                string sql = "update AbpBerthLockEquips set IsOnlineValue=0 where BeatDatetime<'" + DateTime.Now.AddHours(-1).ToString() + "'";
                SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                sql = "update AbpBerthLockEquips set IsOnlineValue=1,BeatDatetime=getdate() where BerthLockEqNumber='" + dto.SN + "'";
                SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("LockDownDataReceive:Error:" + ex.ToString());
            }
            return response;
        }
    }
}
