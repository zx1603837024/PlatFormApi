using F2.Core.Extensions.Utils;
using System;

namespace F2.Application.Smss.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class SmsAccountDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 采用;隔开
        /// </summary>
        public string Destnumbers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SignValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MsgValue { get; set; }


        private string sign;
        /// <summary>
        /// 公司签名
        /// </summary>
        public string Sign
        {
            get
            {
                if (string.IsNullOrEmpty(sign))
                    sign = "道路停车";
                SignValue = sign;
                return TextUtils.FormatStringToUTF8("【" + sign + "】");
            }
            set
            {
                sign = value;
            }
        }

        private string msg;

        /// <summary>
        /// 
        /// </summary>
        public string Msg
        {
            get
            {
                MsgValue = msg;
                return TextUtils.FormatStringToUTF8(msg);
            }
            set
            {
                msg = value;
            }
        }

        /// <summary>
        /// 定时发送时间
        /// </summary>
        public DateTime? Sendtime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "{\"id\": 1,\"method\":\"send\",\"params\":{\"userid\":\"" + UserId + "\",\"password\":\"" + Password + "\",\"submit\": [{\"content\":\"" + Msg + "\",\"phone\":\"" + Destnumbers + "\"}] }}";
        }
    }
}
