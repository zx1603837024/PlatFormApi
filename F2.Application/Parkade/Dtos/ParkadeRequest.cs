using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SKIT.FlurlHttpClient.Wechat.TenpayV3.Models.DepositMarketingMemberCardOpenCardCodesResponse.Types;

namespace F2.Application.Parkade.Dtos
{
    public class ParkadeRequest
    {
        /// <summary>
        /// 设备序列号
        /// </summary>
        public string serialno { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string license { get; set; }
        /// <summary>
        /// 车牌小图片
        /// </summary>
        public string imageFragmentFile { get; set; }
        /// <summary>
        /// 车辆大图片
        /// </summary>
        public string imageFile { get; set; }
        /// <summary>
        /// 车牌颜色
        /// </summary>
        public int colorType { get; set; }
        /// <summary>
        /// 置信度
        /// </summary>
        public int confidence { get; set; }
        /// <summary>
        /// 识别结果车牌 ID
        /// </summary>
        public int plateid { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public long sec { get; set; }
    }
}
