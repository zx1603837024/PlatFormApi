using F2.Application.Smss.Dtos;
using F2.Core.Extensions;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.Net.Http;
using System.Web;

namespace F2.Application.Smss
{
    /// <summary>
    /// 
    /// </summary>
    public class SmsUtils
    {
        /// <summary>
        /// 
        /// </summary>
        HttpClient client = new HttpClient();

        /// <summary>
        /// 对接短信，旧逻辑
        /// </summary>
        /// <param name="input"></param>
        public void SendSmsNoTenant(SmsAccountDto input)
        {
            string isSendSms = SettingStoreAppService.GetSettingOrNull(input.TenantId, null, "SMSChannel").Value;//短信通道，如果启用，可发送短信，不过不启用，不发送短信
            if (isSendSms.ToLower() == "true")
            {
                string smsResult = "Sms接口调用失败！";
                SetUri(input.TenantId);
                input.UserId = SettingStoreAppService.GetSettingOrNull(input.TenantId, null, "Abp.Sms.UserId").Value;
                input.Password = SettingStoreAppService.GetSettingOrNull(input.TenantId, null, "Abp.Sms.Password").Value;
                string sql = "select Sign from AbpTenants where IsDeleted = 0 and IsActive = 1 and Id = " + input.TenantId;
                input.Sign = SqlHelper.ExecuteScalar(SqlHelper.connectionString, System.Data.CommandType.Text, sql).ToString();
                HttpResponseMessage response = null;
                if (SettingStoreAppService.GetSettingOrNull(input.TenantId, null, "Abp.Sms.Method").Value == "Get")
                {
                    response = client.GetAsync(SettingStoreAppService.GetSettingOrNull(input.TenantId, null, "Abp.Sms.AddressUri").Value + input.ToString()).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        smsResult = response.Content.ReadAsStringAsync().Result;
                        sql = "insert into AbpSms(SmsMsg, TelePhones, SmsResult, CreationTime, SmsCount, TenantId) values(@SmsMsg, @TelePhones, @SmsResult, @CreationTime, @SmsCount, @TenantId)";
                        SqlParameter[] param = new SqlParameter[] {
                              new SqlParameter("@SmsMsg",  input.MsgValue + input.SignValue),
                              new SqlParameter("@TelePhones", input.Destnumbers),
                              new SqlParameter("@SmsResult", smsResult),
                              new SqlParameter("@CreationTime", DateTime.Now),
                              new SqlParameter("@SmsCount", 1),
                              new SqlParameter("@TenantId", input.TenantId)
                        };
                        SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql, param);
                    }
                    else
                    {
                        //Logger.Error("短信通道访问异常");
                    }
                }
                else
                {

                }
            }
        }

        /// <summary>
        /// client.BaseAddress 赋值
        /// </summary>
        /// <param name="TenantId"></param>
        private void SetUri(int TenantId)
        {
            client.BaseAddress = new Uri(SettingStoreAppService.GetSettingOrNull(TenantId, null, "Abp.Sms.AddressUrl").Value);
        }



        /// <summary>
        /// 对接Submail短信平台
        /// </summary>
        /// <param name="input"></param>
        public void SendSmsSubmail(SmsAccountDto input)
        {
            string isSendSms = SettingStoreAppService.GetSettingOrNull(input.TenantId, null, "SMSChannel").Value;//短信通道，如果启用，可发送短信，不过不启用，不发送短信
            if (isSendSms.ToLower() == "true")
            {
                string smsResult = "Sms接口调用失败！";
                SmsSubmaiMailDto qm = new SmsSubmaiMailDto();
                qm.appid = "65798";
                qm.content = input.Msg;
                qm.signature = "6d239f8e8ca2c85a351842c077417704";
                qm.to = input.Destnumbers;
                using (HttpClient client1 = new HttpClient()) 
                {
                    HttpResponseMessage response = null;
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(qm));
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    response = client1.PostAsync("https://api-v4.mysubmail.com/sms/send", content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        smsResult = response.Content.ReadAsStringAsync().Result;
                        string sql = "insert into AbpSms(SmsMsg, TelePhones, SmsResult, CreationTime, SmsCount, TenantId) values(@SmsMsg, @TelePhones, @SmsResult, @CreationTime, @SmsCount, @TenantId)";
                        SqlParameter[] param = new SqlParameter[] {
                              new SqlParameter("@SmsMsg",  input.MsgValue + input.SignValue),
                              new SqlParameter("@TelePhones", input.Destnumbers),
                              new SqlParameter("@SmsResult", smsResult),
                              new SqlParameter("@CreationTime", DateTime.Now),
                              new SqlParameter("@SmsCount", 1),
                              new SqlParameter("@TenantId", input.TenantId)
                        };
                        SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql, param);
                    }
                }
            }
        }




    }
}
