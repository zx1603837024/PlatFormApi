using System.Text;

namespace F2.Core.Extensions.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class TextUtils
    {
        /// <summary>
        /// 格式化字符串utf-8
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string FormatStringToUTF8(string val)
        {
            string temp = string.Empty;
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] encodedBytes = utf8.GetBytes(val);
            foreach (byte b in encodedBytes)
            {
                temp += "%" + b.ToString("X");
            }
            return temp;
        }
    }
}
