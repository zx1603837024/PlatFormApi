using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Rates.Dtos
{
    /// <summary>
    /// 具体费率Model
    /// </summary>
    public class CarFeeModel
    {
        /// <summary>
        /// 收费时间
        /// </summary>
        public string RateTime { get; set; }
        /// <summary>
        /// 收费金额
        /// </summary>
        public string RateMoney { get; set; }
        /// <summary>
        /// 收费方式 	0  分钟  1 小时  2	车次
        /// </summary>
        public string RateMethod { get; set; }

        /// <summary>
        /// 0 前 1 每  ps：前10分钟0元 每30分钟2元
        /// </summary>
        public string TimeType { get; set; }

    }
}
