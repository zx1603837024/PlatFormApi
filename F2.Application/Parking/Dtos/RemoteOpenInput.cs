using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parking.Dtos
{
    /// <summary>
    /// 远程开闸
    /// </summary>
    public class RemoteOpenInput
    {
        /// <summary>
        /// 微信openid
        /// </summary>
        public string openId { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string plateNumber { get; set; }

        /// <summary>
        /// 停车场ID
        /// </summary>
        public string parkingId { get; set; }

        /// <summary>
        /// 通道ID
        /// </summary>
        public string channelId { get; set; }


    }
}
