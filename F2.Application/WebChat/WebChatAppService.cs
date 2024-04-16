using System;
using System.Configuration;
using F2.Core.Extensions.Models;
using F2.Application.Accounts;
using System.Net.Http;
using F2.Application.PDA;
using F2.Application.Accounts.Dtos;
using System.Data.SqlClient;
using F2.Core.Extensions;
using F2.Common;
using Newtonsoft.Json;
using F2.Application.Rates.Dtos;

namespace F2.Application.WebChat
{
    /// <summary>
    /// 
    /// </summary>
    public class WebChatAppService : IWebChatAppService
    {
        #region Var
        private readonly IAccountAppService _accountAppService;
        private readonly IBerthsecAppService _berthsecAppService;

        private static readonly string weixinflag = ConfigurationManager.AppSettings["weixinflag"].ToString();
        private static readonly string weixinverify = ConfigurationManager.AppSettings["weixinverify"].ToString();
        private string weixinurl = "";
        private HttpClient client = new HttpClient();
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public WebChatAppService()
        {
            _accountAppService = new AccountAppService();
            _berthsecAppService = new BerthsecAppService();
        }

        /// <summary>
        /// 进场信息发送
        /// </summary>
        /// <param name="PlateNumber"></param>
        /// <param name="CarInTime"></param>
        /// <param name="loginToken"></param>
        public void SendCarInMsg(string PlateNumber, string CarInTime, int BerthsecId, string Berthnumber, AbpUserLoginToken loginToken, string parkName)
        {
            if (weixinflag == "0")
                return;
            weixinurl = ConfigurationManager.AppSettings[loginToken.TenantId.ToString()].ToString();
            //string url = weixinurl + "ajax/SendMsgStopCarNetByCarNumber?Carnumber=" + PlateNumber + "&CarInTime=" + CarInTime + "&Berthnumber=" + Berthnumber + "&BerthsecName=" + _berthsecAppService.GetBerthsecInfo(BerthsecId).BerthsecName;
            string url = weixinurl + "message/SendInPark" ;

            var qm = new WeixinCarInModel();
            qm.carNumber = PlateNumber;
            qm.carInTime = Convert.ToDateTime(CarInTime).ToString("yyyy-MM-dd HH:mm:ss");
            qm.parkName = parkName;
            HttpResponseMessage response = null;
            HttpContent content = new StringContent(JsonConvert.SerializeObject(qm));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            response = client.PostAsync(url, content).Result;

            //HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                InsertWeixinPushLog(new WeixinPushModel() { CreationTime = DateTime.Now, PlateNumber = PlateNumber, PushContent = url, PushType = "CarInMsg", TenantId = loginToken.TenantId });
            }
        }

        /// <summary>
        /// 出场信息发送
        /// </summary>
        /// <param name="TenantId"></param>
        /// <param name="BerthsecId"></param>
        /// <param name="PlateNumber"></param>
        /// <param name="Berthnumber"></param>
        /// <param name="StopTime"></param>
        /// <param name="Money"></param>
        /// <param name="FactReceive"></param>
        /// <param name="PayStatus"></param>
        /// <param name="CarOutTime"></param>
        public void SendCarOutMsg(int TenantId, int BerthsecId, string PlateNumber, string Berthnumber, double StopTime, decimal Money, decimal FactReceive, string PayStatus, string CarOutTime,string ParkName, string CarInTime)
        {
            if (weixinflag == "0")
                return;
            weixinurl = ConfigurationManager.AppSettings[TenantId.ToString()].ToString();
            //string msg = weixinurl + "ajax/SendMsgOutCarNetByCarNumber?Carnumber=" + PlateNumber + "&Berthnumber=" + Berthnumber + "&money=" + Money + "&stoptime=" + StopTimes(StopTime) + "&CarOutTime=" + CarOutTime + "&PayType=" + ChangePayStatusName(PayStatus, FactReceive);
           // string msg = weixinurl + "message/SendOutPark?carNumber=" + PlateNumber + "&parkName=" + ParkName + "&carOutTime=" + CarOutTime + "&carInTime=" + CarInTime;

            string msg = weixinurl + "message/SendOutPark";

            var qm = new WeixinCarOutModel();
            qm.carNumber = PlateNumber;
            qm.carInTime = Convert.ToDateTime(CarInTime).ToString("yyyy-MM-dd HH:mm:ss");
            qm.parkName = ParkName;
            qm.carOutTime = Convert.ToDateTime(CarOutTime).ToString("yyyy-MM-dd HH:mm:ss");
            HttpResponseMessage response = null;
            HttpContent content = new StringContent(JsonConvert.SerializeObject(qm));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            response = client.PostAsync(msg, content).Result;

            //HttpResponseMessage response = client.GetAsync(msg).Result;
            if (response.IsSuccessStatusCode)
            {
                InsertWeixinPushLog(new WeixinPushModel() { CreationTime = DateTime.Now, PlateNumber = PlateNumber, PushContent = msg, PushType = "CarOutMsg", TenantId = TenantId });
            }
        }

        /// <summary>
        /// 转换支付类型
        /// </summary>
        /// <param name="PayStatus"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        private string ChangePayStatusName(string PayStatus, decimal money)
        {
            if (money == 0)
                return "未付";
            switch (PayStatus)
            {
                case "1":
                    return "现金";
                case "2":
                    return "刷卡支付";
                case "3":
                    return "微信支付";
                case "4":
                    return "账号支付";
                case "5":
                    return "未付";
                case "6":
                    return "支付宝支付";
                case "7":
                    return "其他";
                default:
                    return "未知";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="StopTime"></param>
        /// <returns></returns>
        private string StopTimes(double StopTime)
        {
            int Getstoptime = Convert.ToInt32(StopTime);
            TimeSpan ts = new TimeSpan(0, 0, Getstoptime, 0);
            string dateDiff = "";
            if (Getstoptime >= 1440)//判断是否大于24小时
            {
                dateDiff = ts.Days.ToString() + "天" + ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟";
            }
            else
            {
                dateDiff = ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟";
            }
            return dateDiff;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        private void InsertWeixinPushLog(WeixinPushModel model)
        {
            string sql = "insert into AbpWeixinPushMsg(CreationTime, TenantId, PushType, PushContent, PlateNumber) values(@CreationTime, @TenantId, @PushType, @PushContent, @PlateNumber) SELECT @@IDENTITY as Id";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@CreationTime", model.CreationTime),
                new SqlParameter("@TenantId", model.TenantId),
                new SqlParameter("@PushType", model.PushType),
                new SqlParameter("@PushContent", model.PushContent),
                new SqlParameter("@PlateNumber", model.PlateNumber)
            };
            SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql, param);
        }
    }
}
