using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.VideoEqs.Dtos
{
    public class VideoEqFaultsRequest
    {
        /// <summary>
        /// 标识
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 设备Id,用于区分设备
        /// </summary>
        public string sn { get; set; }

        /// <summary>
        /// 设备状态。0，正常。其它代表异常
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 故障描述
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 故障时间,毫秒数。
        /// </summary>
        public string statusTime { get; set; }

        /// <summary>
        /// 图片转换成的base64编辑字符串
        /// </summary>
        public string ossPath { get; set; }
    }
}
