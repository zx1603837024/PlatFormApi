using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F2.Application.Sensors.Dtos;
using System.Data;
using F2.Core.Extensions;
using System.Data.SqlClient;
using F2.Application.Rates;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Cryptography;

namespace F2.Application.Sensors
{
    /// <summary>
    /// 
    /// </summary>
    public class SensorAppService : ISensorAppService
    {
        #region
        private readonly IRateAppService _rateAppService;//车检器设备 
        #endregion
        /// <summary>
        /// 
        /// </summary>
        public SensorAppService()
        {
            _rateAppService = new RateAppService();
        }

        /// <summary>
        /// MD5加密 密码
        /// </summary>
        /// <param name="strtemp"></param>
        /// <returns></returns>
        private string MD5Encrypt(string strtemp)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = Encoding.UTF8.GetBytes(strtemp);//将字符编码为一个字节序列 
            byte[] md5data = md5.ComputeHash(data);//计算data字节数组的哈希值 
            md5.Clear();
            string str = "";
            for (int i = 0; i < md5data.Length; i++)
            {
                str += md5data[i].ToString("x").PadLeft(2, '0');
            }
            return str;
        }

        /// <summary>
        /// 友盟推送
        /// </summary>
        /// <param name="berthsecId">泊位段Id</param>
        /// <param name="parkingStatu">停车状态</param>
        private void UmengPush(string berthsecId, string parkingStatu)
        {
            UmengModel push = new UmengModel();
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            long timeStamp = (long)(DateTime.Now - startTime).TotalMilliseconds; // 相差毫秒数
            push.appkey = "5d40f5f63fc1954966000529";
            push.device_tokens = "Akv0oMQ5aQbC2Da3N3H8p-JS3erbEdPD_cyEwArFXiWx";
            push.type = "unicast";
            push.timestamp = timeStamp.ToString();

            Dictionary<string, string> extra = new Dictionary<string, string>();
            extra.Add("berthsecId", berthsecId);
            extra.Add("parkingStatu", parkingStatu);

            PayloadModel payload = new PayloadModel();
            payload.display_type = "notification";
         
            BodyModel body = new BodyModel();
            body.text = "XXX车位车辆"+ (parkingStatu == "1"?"进场":"出场");
            body.ticker = "地磁进出场推送";
            body.title = "地磁数据推送";
            payload.body = body;
            payload.extra = extra;
            push.payload = payload;
            var str = JsonConvert.SerializeObject(push);
            string url = "http://msg.umeng.com/api/send";
            string sign = MD5Encrypt("POST" + url + str + "hwcjemapzrq1zn3kslsxr3uubew9j2ce");
            HttpContent content = new StringContent(str);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.PostAsync(url + "?sign=" + sign, content).Result;
            var strjson = response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
               
            }
            else
            {
                
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public string SendDeviceByINmotion(INmotionDto dto)
        {
            
            if (dto==null)
            {
                return "{\"code\":100, \"body\":{\"msg\":\"成功\"}} ";

            }
            switch (dto.cmd)
            {
                case "sendParkStatu":
                    if (dto.body.parkingStatu == "1")
                    {
                        InsertCarAdmission(DateTime.ParseExact(dto.body.time, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture), dto.body.sequence, dto.body.deviceID);//车辆进场
                        
                    }
                    else
                    {
                        InsertCarEntrance(DateTime.ParseExact(dto.body.time, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture), dto.body.flag == true ? (int.Parse(dto.body.sequence) - 1).ToString() : dto.body.sequence, dto.body.deviceID);//车辆出场
                       // UmengPush("", dto.body.parkingStatu);
                    }
                    UpdateSensorBattary(dto.body.deviceID, dto.body.battary);
                    break;
                case "sendDeviceHeartbeat":
                    decimal battary = 0;
                    if (!string.IsNullOrWhiteSpace(dto.body.battary))
                    {
                        battary = decimal.Parse(dto.body.battary);
                    }
                    SensorRegister("1", dto.body.deviceID, battary);
                    break;
                case "SendRegister":
                    SensorRegister("1", dto.body.deviceID, 0);
                    break;
                default:
                    return "{\"code\":101, \"body\":{\"msg\":\"失败\"}} ";
            }
            return "{\"code\":100, \"body\":{\"msg\":\"成功\"}} ";
        }

        /// <summary>
        /// 新增车检器
        /// </summary>
        /// <param name="Magnetism"></param>
        /// <param name="Battery"></param>
        /// <param name="TransmitterNumber"></param>
        /// <param name="SensorNumber"></param>
        /// <param name="ParkStatus"></param>
        /// <returns></returns>
        private bool InsertSensor(decimal? Magnetism, decimal? Battery, string TransmitterNumber, string SensorNumber, short ParkStatus)
        {
            SensorRegister(TransmitterNumber, SensorNumber, Battery);
            return true;
        }

        /// <summary>
        /// 新增基站
        /// </summary>
        /// <param name="TransmitterNumber"></param>
        /// <param name="VoltageCaution"></param>
        /// <returns></returns>
        private bool InsertTransmitter(string TransmitterNumber, decimal? VoltageCaution)
        {
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@TransmitterNumber", TransmitterNumber)
            };
            SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.StoredProcedure, "Pro_TransmitterNumberRegister", param);
            return true;
        }

        /// <summary>
        /// 车辆出场
        /// </summary>
        /// <param name="CarOutTime"></param>
        /// <param name="Indicate"></param>
        /// <param name="SensorNumber"></param>
        /// <returns></returns>
        private bool InsertCarEntrance(DateTime CarOutTime, string Indicate, string SensorNumber)
        {
            DataTable dt = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select CarInTime, BerthsecId, PlateNumber, ParkId, CompanyId from AbpSensorBusinessDetail with(nolock) where SensorNumber = '" + SensorNumber + "' and Indicate = '" + Indicate + "' and status = 0").Tables[0];
            int? StopTime = null;
            decimal Receivable = 0;
            if (dt.Rows.Count > 0 && string.IsNullOrWhiteSpace(dt.Rows[0]["CarInTime"].ToString()) == false && string.IsNullOrWhiteSpace(dt.Rows[0]["BerthsecId"].ToString()) == false)
            {
                var model = _rateAppService.RateCalculate(int.Parse(dt.Rows[0]["BerthsecId"].ToString()), DateTime.Parse(dt.Rows[0]["CarInTime"].ToString()), CarOutTime, 2, 0, int.Parse(dt.Rows[0]["ParkId"].ToString()), dt.Rows[0]["PlateNumber"].ToString(), int.Parse(dt.Rows[0]["CompanyId"].ToString()));
                StopTime = (int?)model.ParkTime;
                Receivable = model.CalculateMoney;
            }

            Guid guid = Guid.NewGuid();
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@SensorNumber", SensorNumber),
                new SqlParameter("@Indicate", Indicate),
                new SqlParameter("@guid", guid),
                new SqlParameter("@CarOutTime", CarOutTime),
                new SqlParameter("@StopTime", StopTime),
                new SqlParameter("@Receivable", Receivable)
            };
            DataTable result = SqlHelper.ExecuteDataset(SqlHelper.connectionString, "Pro_Sensor_SensorInsertCarEntrance", param).Tables[0];
            return true;
        }

        /// <summary>
        /// 设备注册
        /// </summary>
        /// <param name="TransmitterNumber"></param>
        /// <param name="SensorNumber"></param>
        /// <returns></returns>
        private bool SensorRegister(string TransmitterNumber, string SensorNumber , decimal? Battery)
        {
            InsertTransmitter(TransmitterNumber, 0);
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@TransmitterNumber", TransmitterNumber),
                new SqlParameter("@SensorNumber", SensorNumber),
                new SqlParameter("@Battery", Battery)
            };
            SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.StoredProcedure, "Pro_SensorRegister", param);
            return true;
        }

        /// <summary>
        /// 更新检测器电量
        /// </summary>
        /// <param name="sensornumber"></param>
        /// <param name="battary"></param>
        /// <returns></returns>
        private bool UpdateSensorBattary(string sensornumber, string battary)
        {
            return true;
        }

        /// <summary>
        /// 车辆进场
        /// </summary>
        /// <param name="CarInTime"></param>
        /// <param name="Indicate"></param>
        /// <param name="SensorNumber"></param>
        /// <returns></returns>
        private bool InsertCarAdmission(DateTime CarInTime, string Indicate, string SensorNumber)
        {
            Guid guid = Guid.NewGuid();
            DataTable dt = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select CarOutTime, BerthsecId, PlateNumber, ParkId, CompanyId from AbpSensorBusinessDetail with(nolock) where SensorNumber = '" + SensorNumber + "' and Indicate = '" + Indicate + "' and status = 0").Tables[0];
            int? StopTime = null;
            decimal Receivable = 0;
            if (dt.Rows.Count > 0 && string.IsNullOrWhiteSpace(dt.Rows[0]["CarOutTime"].ToString()) == false && string.IsNullOrWhiteSpace(dt.Rows[0]["BerthsecId"].ToString()) == false)
            {
                var model = _rateAppService.RateCalculate(int.Parse(dt.Rows[0]["BerthsecId"].ToString()), CarInTime, DateTime.Parse(dt.Rows[0]["CarOutTime"].ToString()), 2, 0, int.Parse(dt.Rows[0]["ParkId"].ToString()), dt.Rows[0]["PlateNumber"].ToString(), int.Parse(dt.Rows[0]["CompanyId"].ToString()));
                StopTime = (int?)model.ParkTime;
                Receivable = model.CalculateMoney;
            }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               

            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@SensorNumber", SensorNumber),
                 new SqlParameter("@Indicate", Indicate),
                 new SqlParameter("@guid", guid),
                 new SqlParameter("@CarInTime", CarInTime),
                 new SqlParameter("@StopTime", StopTime),
                new SqlParameter("@Receivable", Receivable)
            };
            DataTable result = SqlHelper.ExecuteDataset(SqlHelper.connectionString, "Pro_Sensor_SensorInsertCarAdmission", param).Tables[0];

           // UmengPush(dt.Rows[0]["BerthsecId"].ToString(), "1");

            return true;
        }
    }
}
