using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parking.ZhiBo
{
    public class DelMonthlyRentResponse
    {
        /// <summary>
        /// 1成功0失败
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 错误代码 描述
        /// </summary>
        public string message { get; set; }
    }
}
