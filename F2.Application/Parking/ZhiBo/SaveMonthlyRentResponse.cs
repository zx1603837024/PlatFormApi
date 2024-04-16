using F2.Core.Extensions.DataExtend;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parking.ZhiBo
{
    public class SaveMonthlyRentResponse
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
