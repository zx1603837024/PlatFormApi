using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Caching;
using F2.Common;
using F2.Core.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace CommonTool
{
    public class Commons
    {
        //CONTENT-MD5加密
        public string MD5ToBase64String(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] MD5 = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(str));//MD5(注意UTF8编码)
            string result = Convert.ToBase64String(MD5, 0, MD5.Length);//Base64
            return result;
        }
        /// <summary>
        /// HMACSHA1加密
        /// </summary>
        /// <param name="text">要加密的原串</param>
        ///<param name="key">私钥</param>
        /// <returns></returns>
        public string HMACSHA1Text(string text, string key)
        {
            //HMACSHA1加密
            HMACSHA1 hmacsha1 = new HMACSHA1();
            hmacsha1.Key = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] dataBuffer = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);
            return Convert.ToBase64String(hashBytes);
        }
        //HMAC-SHA256加密鉴权
        public string HmacSha256Authentication(object message, string sign)
        {
            try
            {
                string messageStr = message.ToString();
                JObject input = JsonConvert.DeserializeObject<JObject>(messageStr);
                DateTime timestamp = CommonTools.getDateTime(Convert.ToInt64(input["timestamp"]) * 1000);
                if ((DateTime.Now - timestamp).TotalMinutes < 500)//5分钟超时判断
                {
                    string appKey = Convert.ToString(input["appKey"]);
                    DataTable dtSecret = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select appSecret from AbpAuthentication with(nolock) where appKey = '" + appKey + "'").Tables[0];
                    string appSecret = dtSecret.Rows[0]["appSecret"].ToString();
                    var encoding = new UTF8Encoding();
                    byte[] keyByte = encoding.GetBytes(appSecret);
                    byte[] messageBytes = encoding.GetBytes(messageStr);
                    using (var hmacsha256 = new HMACSHA256(keyByte))
                    {
                        byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                        string HmacSha256 = Convert.ToBase64String(hashmessage);
                        if (HmacSha256 == sign)
                        {
                            input.Remove("appKey");
                            input.Remove("timestamp");
                            input.Remove("url");
                            return input.ToString();
                        }
                        else
                        {
                            return "false";
                        }
                    }
                }
                else {
                    return "false";
                }
            }
            catch (Exception e) {
                return "false";
            }
        }

        //发生post请求
        public string HttpPostNew(string Url, string postDataStr)
        {
            byte[] postBytes = Encoding.GetEncoding("utf-8").GetBytes(postDataStr);
            HttpWebRequest request = WebRequest.Create(Url) as HttpWebRequest;//(HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = postBytes.Length;
            Stream myRequestStream = request.GetRequestStream();
            myRequestStream.Write(postBytes, 0, postBytes.Length);
            myRequestStream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }
        /**
         * 华峰请求接口先调用获取token然后调用接口
         * 华峰业务接口调用前调用该接口appid和appsercet方法内修改
         */
        public string GetTokenByHF(string URL, string DATA,string HfUrl)
        {
            string url = HfUrl + "/api/ev/token";
            string data = "{\"appid\": \"736f6e677368616e68755f4658323032323131303453\",\"secret\": \"5a5d43590ffff772c5500a0ba80fd535\"}";
            string response = HttpPostNew(url, data);
            Cache cache = new Cache();
            var HFToken = cache.Get("HFToken");
            CommonTools.WriteLogFile("HFToken：" + response);
            if (HFToken == null)
            {
                JObject joResponse = (JObject)JsonConvert.DeserializeObject(response);
                if (joResponse["status"].ToString().Equals("0"))
                {
                    JObject hFResponse = (JObject)JsonConvert.DeserializeObject(joResponse["data"].ToString());
                    cache.Insert("HFToken", hFResponse["token"].ToString(), null, System.DateTime.Now.AddSeconds((long)60 * 60 * 2), TimeSpan.Zero);
                    return HttpPostToken(URL, DATA, hFResponse["token"].ToString());
                }
                return response;
            }
            else
            {
                //如果有缓存的token先请求一次对方业务接口看能否请求成功，失败了更新获取token再请求业务接口
                string postResult=HttpPostToken(URL, DATA, HFToken.ToString());
                CommonTools.WriteLogFile("postResult：" + postResult);
                JObject joResponse = (JObject)JsonConvert.DeserializeObject(postResult);
                if (joResponse["errorcode"].ToString().Equals("0"))
                {
                    return postResult;
                }
                else
                {
                    try
                    {
                        string response2 = HttpPostNew(url, data);
                        joResponse = (JObject)JsonConvert.DeserializeObject(response2);
                        if (joResponse["status"].ToString().Equals("0"))
                        {
                            JObject hFResponse = (JObject)JsonConvert.DeserializeObject(joResponse["data"].ToString());
                            cache.Insert("HFToken", hFResponse["token"].ToString(), null, System.DateTime.Now.AddSeconds((long)60 * 60 * 2), TimeSpan.Zero);
                            return HttpPostToken(URL, DATA, hFResponse["token"].ToString());
                        }
                        return response2;
                    }catch (Exception e)
                    {
                        CommonTools.WriteLogFile("华峰请求接口失败：" + joResponse);
                        return "error";
                    }
                }

            }
            
            
        }
        public class HFResponse
        {
            
            public string expired { get; set; }
            public string token { get; set; }
        
        }
        /**
         * token验证
         */
        public string HttpPostToken(string url,string data,string token)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(url);
            }
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(url);
            }
            byte[] postBytes = Encoding.GetEncoding("utf-8").GetBytes(data);
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;//(HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = postBytes.Length;
            request.Headers.Add("Authorization","Bearer "+token); 
            
            Stream myRequestStream = request.GetRequestStream();
            myRequestStream.Write(postBytes, 0, postBytes.Length);
            myRequestStream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }

        //发生get请求
        public string HttpGetNew(string Url, string postDataStr)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }
        //校验车牌号
        public bool IsValidPlateNumber(string plateNumber)
        {
            string pattern = @"^[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领]{1}[A-Z0-9]{5}[A-Z0-9挂学警港澳]{1}$|^[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领]{1}[A-Z0-9]{6}[A-Z0-9挂学警港澳]{1}$";
            return Regex.IsMatch(plateNumber, pattern);
        }
        /// <summary>
        /// 屏幕显示
        /// </summary>
        /// <param name="Content">显示的文字</param>
        /// <returns></returns>
        public byte[] ScreenShow(string[] Content)
        {
            byte[] bytesHead = { 0x00, 0x64, 0xFF, 0xFF, 0x6F, 0xFF};
            byte[] bytesA = {0x00, 0x0F, 0x04, 0x00, 0x15, 0x01, 0x02, 0x00, 0x03, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x1C, 0x60, 0x59, 0x2D, 0x60, 0x4D, 0x2D, 0x60, 0x44, 0x20, 0x60, 0x67, 0xD0, 0xC7, 0xC6, 0xDA, 0x60, 0x56, 0x20, 0x60, 0x72, 0x60, 0x48, 0x3A, 0x60, 0x4E, 0x3A, 0x60, 0x53};
            byte[] bytesB = { 0x0D, 0x01, 0x15, 0x01, 0x02, 0x00, 0x03, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xFF};
            byte[] ContentA = System.Text.Encoding.Default.GetBytes(Content[0]);
            bytesB[12] = Convert.ToByte(ContentA.Length);
            byte[] bytesC = { 0x0D, 0x02, 0x15, 0x01, 0x02, 0x00, 0x03, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xFF};
            byte[] ContentB = System.Text.Encoding.Default.GetBytes(Content[1]);
            bytesC[12] = Convert.ToByte(ContentB.Length);
            byte[] bytesD = { 0x0D, 0x03, 0x15, 0x01, 0x02, 0x00, 0x03, 0x00, 0xFF, 0x00, 0x00, 0x00, 0xFF};
            byte[] ContentC = System.Text.Encoding.Default.GetBytes(Content[2]);
            bytesD[12] = Convert.ToByte(ContentC.Length);
            byte[] bytesE = { 0x00, 0x0A, 0x00, 0x00};
            int DataLength = bytesA.Length + bytesB.Length + ContentA.Length + bytesC.Length + ContentB.Length + bytesD.Length + ContentC.Length + bytesE.Length;
            bytesHead[5] = Convert.ToByte(DataLength);
            byte[] bytesData = new byte[bytesHead.Length+DataLength];
            bytesHead.CopyTo(bytesData, 0);
            bytesA.CopyTo(bytesData, bytesHead.Length);
            bytesB.CopyTo(bytesData, bytesHead.Length+bytesA.Length);
            ContentA.CopyTo(bytesData, bytesHead.Length+bytesA.Length+ bytesB.Length);
            bytesC.CopyTo(bytesData, bytesHead.Length+bytesA.Length + bytesB.Length+ ContentA.Length);
            ContentB.CopyTo(bytesData, bytesHead.Length+bytesA.Length + bytesB.Length + ContentA.Length + bytesC.Length);
            bytesD.CopyTo(bytesData, bytesHead.Length+bytesA.Length + bytesB.Length + ContentA.Length + bytesC.Length+ ContentB.Length);
            ContentC.CopyTo(bytesData, bytesHead.Length+bytesA.Length + bytesB.Length + ContentA.Length + bytesC.Length + ContentB.Length+ bytesD.Length);
            bytesE.CopyTo(bytesData, bytesHead.Length+bytesA.Length + bytesB.Length + ContentA.Length + bytesC.Length + ContentB.Length + bytesD.Length+ ContentC.Length);
            byte[] bytesCRC = { CalculateCRC16(bytesData)[1], CalculateCRC16(bytesData)[0] };
            byte[] bytesAll = new byte[bytesData.Length+bytesCRC.Length];
            bytesData.CopyTo(bytesAll, 0);
            bytesCRC.CopyTo(bytesAll, bytesData.Length);
            return bytesAll;
        }
        /// <summary>
        /// 余位展示
        /// </summary>
        /// <param name="Data">显示的车位数量</param>
        /// <returns></returns>
        public byte[] ShowRemainder(int Data)
        {
            byte[] bytesA = { 0x00, 0x64, 0xFF, 0xFF, 0xE3, 0x09, 0x01, 0x00, 0x00, 0x00, 0x00};
            byte[] bytesData =intToBytes(Data);
            byte[] bytesB = new byte[bytesA.Length + bytesData.Length];
            bytesA.CopyTo(bytesB, 0);
            bytesData.CopyTo(bytesB, bytesA.Length);
            byte[] bytesC = {CalculateCRC16(bytesB)[1],CalculateCRC16(bytesB)[0]};
            byte[] bytesAll = new byte[bytesB.Length+ bytesC.Length];
            bytesB.CopyTo(bytesAll, 0);
            bytesC.CopyTo(bytesAll, bytesB.Length);
            return bytesAll;
        }
        public byte[] intToBytes(int value)
        {
            byte[] src = new byte[4];
            src[3] = (byte)((value >> 24) & 0xFF);
            src[2] = (byte)((value >> 16) & 0xFF);
            src[1] = (byte)((value >> 8) & 0xFF);
            src[0] = (byte)(value & 0xFF);
            return src;
        }
        public byte[] CalculateCRC16(byte[] data)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < data.Length; i++)
            {
                crc ^= data[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x0001) != 0)
                    {
                        crc >>= 1;
                        crc ^= 0xA001;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }
            return BitConverter.GetBytes(crc).Reverse().ToArray();
        }
    }
}
