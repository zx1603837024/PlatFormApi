using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using F2.Application.PDA.Dtos;
using F2.Core.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace F2.Application.PDA
{
    /// <summary>
    /// 
    /// </summary>
    public class BerthsecAppService : IBerthsecAppService
    {
        /// <summary>
        /// 获取可签到的泊位段
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public GetAllBerthsecListOutput GetBerthsecList(long employeeId, int tenantId)
        {
            int Status = 1; 
            if (bool.Parse(SettingStoreAppService.GetSettingOrNull(tenantId, null, "EmployeesLimit").Value))
            {
                Status = 1;
            }
            if (Status != 0)
            {
                Status = 1;//早班
                if (DateTime.Now.Hour >= 22 || DateTime.Now.Hour < 7)//晚班
                    Status = 3;
            }
            string sql = "select AbpBerthsecs.Id, BerthsecName, CheckStatus, UseStatus, PushStatus from AbpWorkGroupBerthsecs left join AbpBerthsecs on BerthsecId = AbpBerthsecs.Id where WorkGroupId in (select Id from AbpWorkGroups where id in (select WorkGroupId from AbpWorkGroupEmployees where IsDeleted = 0 and EmployeeId = @EmployeeId) and IsDeleted = 0) and AbpWorkGroupBerthsecs.IsDeleted = 0 and (Status = @Status) and AbpBerthsecs.IsDeleted = 0";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@EmployeeId", employeeId),
                new SqlParameter("@Status", Status)
            };
            return new GetAllBerthsecListOutput()
            {
                rows = DataProcessHelper.GetEntityFromTable<BerthsecCheckDto>(SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql, param))
            };
        }

        /// <summary>
        /// 获取泊位段信息
        /// </summary>
        /// <param name="BerthsecId"></param>
        /// <returns></returns>
        public BerthsecDto GetBerthsecInfo(int BerthsecId)
        {
            string sql = "select * from abpberthsecs where id = " + BerthsecId;
            return DataProcessHelper.GetEntityFromTable<BerthsecDto>(SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql, null))[0];
        }
        /// <summary>
        /// 获取在停订单信息
        /// </summary>
        /// <param name="jsonstr"></param>
        /// <returns></returns>
        public List<StopOrderDto> SearchStopOrder(string jsonstr)
        {
            List<StopOrderDto> temp = new List<StopOrderDto>();

            JObject jo = (JObject)JsonConvert.DeserializeObject(jsonstr);
            string BerthNumber = (string)jo["BerthNumber"];

            //替换逗号分割
            BerthNumber = BerthNumber.Replace(",","','");
            string sql = "SELECT b.Id berthID ,b.ParkId parkID,b.BerthNumber berthCode,b.RelateNumber carNumber,b.CarType carType,b.StopType stopType,b.InCarTime carInTime,b.prepaid prePay," +
                "sto.InOperaId inOperaId,sto.prepaidCarNo preCardNo,b.BerthStatus berthStatus,b.ParkStatus sensorStatus,b.guid berthGuid,b.SensorGuid sensorGuid,b.SensorBeatTime sensorBeatTime," +
                "b.BerthsecId berthsecID,b.RegionId regionID,c.CarInTime sensorCarInTime,b.IsFaultFlag,b.IsSourceVideo " +
                "from AbpBerths b " +
                "left join  AbpBusinessDetail sto on b.guid = sto.guid left join AbpSensorBusinessDetail c on sto.guid = c.guid " +   
                " where sto.Status = '1' and b.BerthNumber in('" + BerthNumber + "')";

            

            DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
            if (tables.Rows.Count>0)
            {
                var List = DataProcessHelper.GetEntityFromTable<StopOrderDto>(tables);
                for (int i = 0; i < List.Count; i++)
                {
                    var model = List[i];
                    temp.Add(model);
                }
            }
            return temp;
        }


        /// <summary>
        ///根据泊位号获取订单信息
        /// </summary>
        /// <param name="jsonstr"></param>
        /// <returns></returns>
        public List<FreeOrderDetailDto> SearchFreepOrder(string jsonstr)
        {
            List<FreeOrderDetailDto> temp = new List<FreeOrderDetailDto>();

            JObject jo = (JObject)JsonConvert.DeserializeObject(jsonstr);
            string BerthNumber = (string)jo["BerthNumber"];

            //string sql = "select Id, BerthNumber,Status,PlateNumber,guid as Guid from AbpBusinessDetail where Status = '1' and  BerthNumber in(" + BerthNumber + ")";

            string sql = "select * from AbpFreeOrderDetail where berthCode in(" + BerthNumber + ")";


            DataTable tables = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql);
            if (tables.Rows.Count > 0)
            {
                var List = DataProcessHelper.GetEntityFromTable<FreeOrderDetailDto>(tables);
                for (int i = 0; i < List.Count; i++)
                {
                    var model = List[i];
                    temp.Add(model);
                }
            }
            return temp;
        }


        
    }
}
