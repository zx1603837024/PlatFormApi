using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parking.Dtos
{
    public class AbpParkChannerlDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 停车场Id
        /// </summary>
        public int ParkId { get; set; }

        /// <summary>
        /// 停车场
        /// </summary>
        public string ParkName { get; set; }

        /// <summary>
        /// 停车场平台通道ID
        /// </summary>
        public string ZhiBoChannelId { get; set; }

        /// <summary>
        /// 停车场类型
        /// </summary>
        public string ChannelCode { get; set; }

        /// <summary>
        /// 通道名称
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// 通道方向
        /// </summary>
        public string ChannelDirection { get; set; }
    }
}
