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
using F2.Application.Invoice.Dtos;
using Newtonsoft.Json.Linq;
using static CommonTool.Commons;
using static SKIT.FlurlHttpClient.Wechat.TenpayV3.Models.GetMarketingMemberCardOpenCardByCardIdResponse.Types;
using System.Security.Cryptography;
using System.Web;

namespace F2.Application.Invoice
{
    public class InvoiceService : IInvoiceService
    {
        //HMAC-SHA256加密
        public static string ComputeHmacSha256(string secret, string message)
        {
            var encoding = new UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }
        public InvoiceResponse InvoiceCallBack(int timestamp, object dto) {
            
            string appKey = "2cQ8aj7l768M1VLa";
            string appSecret = "6mrc6skf08uq25wb";
            InvoiceResponse res = new InvoiceResponse();
            try
            {
                string data = "appKey =" + appKey + "&algorithm=HMAC-SHA256&timestamp=" + Convert.ToString(timestamp) + "&/api/InterfaceInvoice/InvoiceCallBack&" + dto.ToString();
                var HmacSha256 = ComputeHmacSha256(appSecret, data);
                var sign = HttpContext.Current.Request.Headers["invoice-sign"];
                if (HmacSha256 == sign)
                {
                    var param = JsonConvert.DeserializeObject<JObject>(dto.ToString());
                    if (Convert.ToString(param["callbackType"]) == "开票成功")
                    {
                        string sql = "update AbpInvoiceDetail set Status=1,UpdateTime=GETDATE(),PicThirdInsAddress='" + param["response"]["electronicInvoiceImg"] + "',PDFThirdInsAddress='" + param["response"]["electronicInvoiceUrl"] + "' where InvoiceReqNo='" + param["response"]["outOrderNo"] + "'";
                        SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                        res.code = 1;
                    }
                    else
                    {
                        res.code = 0;
                        Logger.Log.Error("InvoiceCallBack:开票失败;" + dto.ToString());
                    }
                }
                else {
                    res.code = 0;
                    res.message = "鉴权信息错误";
                    Logger.Log.Error("InvoiceCallBack:鉴权信息错误;" + dto.ToString());
                }
            }
            catch (Exception ex)
            {
                res.code = 0;
                res.message = ex.Message;
                Logger.Log.Error("InvoiceCallBack:Error:" + dto.ToString()+";"+ ex.ToString());
            }
            return res;
        }
    }
}
