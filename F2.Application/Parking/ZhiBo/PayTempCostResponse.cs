using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parking.ZhiBo
{
    /// <summary>
    /// 临停支付通知接口
    /// </summary>
    public class PayTempCostResponse
    {
        /// <summary>
        /// 状态值 0失败 1成功
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 消息提示
        /// </summary>
        public string message { get; set; }
    }
}
