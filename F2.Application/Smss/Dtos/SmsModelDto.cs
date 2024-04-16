using System;

namespace F2.Application.Smss.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class SmsModelDto
    {
        /// <summary>
        /// 短信类型
        /// </summary>
        public string ModelType { get; set; }

        /// <summary>
        /// 短信内容
        /// </summary>
        public string SmsContext { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 发送短信路径
        /// </summary>
        public string Url { get; set; }
    }
}
