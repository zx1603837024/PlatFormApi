using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace F2.Core.Extensions.DataExtend
{
    public static class DataHelper
    {
        /// <summary>
        /// 判断是否为空
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        /// <summary>
        /// 指示指定的字符串是 null、空还是仅由空白字符组成。
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        /// <summary>
        /// 转化成json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T entity)
        {
            if (entity == null) return "{}";
            return JsonConvert.SerializeObject(entity);
        }

        /// <summary>
        /// json转化成实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(this string json)
        {
            if (IsNullOrWhiteSpace(json)) return default(T);
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// string.Format
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static string Fmt(this string caller, params object[] paras)
        {
            return string.Format(caller, paras);
        }

        /// <summary>
        /// Guid是否为空
        /// </summary>
        /// <param name="pCaller"></param>
        /// <returns></returns>
        public static bool IsEmpty(this Guid pCaller)
        {
            return Guid.Empty == pCaller;
        }

        /// <summary>
        /// jtoken安全转化成string
        /// </summary>
        /// <param name="jToken"></param>
        /// <returns></returns>
        public static string SafeToString(this JToken jToken)
        {
            if (jToken == null) return null;
            if (jToken is JObject)
            {
                return SafeToString(jToken["#text"]);
            }
            return jToken.ToString();
        }

        /// <summary>
        /// jtoken安全转化成string
        /// </summary>
        /// <param name="jToken"></param>
        /// <returns></returns>
        public static int? SafeToInt(this JToken jToken)
        {
            if (jToken == null) return null;
            var jvalue = SafeToString(jToken);
            int zero = 0;
            if (int.TryParse(jvalue, out zero))
            {
                return zero;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="pInput">输入的字符串</param>
        /// <returns>加密后的结果</returns>
        public static string MD5Encrypt(this string pInput)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] val, hash;
            val = Encoding.UTF8.GetBytes(pInput);
            hash = md5.ComputeHash(val);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }
    }
}
