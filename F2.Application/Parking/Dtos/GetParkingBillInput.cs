using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parking.Dtos
{
    /// <summary>
    /// 获取停车账单
    /// </summary>
    public class GetParkingBillInput
    {
        /// <summary>
        /// 停车场ID
        /// </summary>
        public string parkingId { get; set; }

        /// <summary>
        /// 微信OpenID
        /// </summary>
        public string openId { get; set; }

        /// <summary>
        /// 商户ID
        /// </summary>
        public int tenantId { get; set; }

        /// <summary>
        /// 通道ID
        /// </summary>
        public string channelId { get; set; }
    }
}
