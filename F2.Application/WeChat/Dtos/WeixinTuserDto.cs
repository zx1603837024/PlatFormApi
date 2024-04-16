using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.WeChat.Dtos
{
    /// <summary>
    /// 微信用户
    /// </summary>
    public class WeixinTuserDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 商户ID
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string nickName { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string tel { get; set; }

        /// <summary>
        /// 微信openid
        /// </summary>
        public string openId { get; set; }

        /// <summary>
        /// 车牌号1
        /// </summary>
        public string CarNumber1 { get; set; }

        /// <summary>
        /// 车牌号2
        /// </summary>
        public string CarNumber2 { get; set; }

        /// <summary>
        /// 车牌号3
        /// </summary>
        public string CarNumber3 { get; set; }
    }
}
