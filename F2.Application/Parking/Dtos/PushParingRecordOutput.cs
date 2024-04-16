using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parking.Dtos
{
    /// <summary>
    /// 车场记录推送
    /// </summary>
    public class PushParingRecordOutput
    {
        /// <summary>
        /// 响应码
        /// </summary>
        public EPushParingRecordOutputStatus status { get; set; }

        /// <summary>
        /// 响应描述
        /// </summary>
        public string message { get; set; }
    }

    /// <summary>
    /// 响应码
    /// </summary>
    public enum EPushParingRecordOutputStatus
    {
        失败=0,
        成功=1,
        自定义失败=999
    }
}
