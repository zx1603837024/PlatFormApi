using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Smss.Dtos
{
    public class SmsSubmaiMailDto
    {
        /// <summary>
        /// appId
        /// </summary>
        public string appid { get;set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string to { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 密钥
        /// </summary>
        public string signature { get; set; }

    }

}
