using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Rates.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class CarFeeScaleModel
    {
        /// <summary>
        /// 收费时间
        /// </summary>
        public string RateTime { get; set; }
        /// <summary>
        /// 收费金额
        /// </summary>
        public decimal RateMoney { get; set; }
        /// <summary>
        /// 收费方式 	0  分钟  1 小时  2	车次
        /// </summary>
        public string RateMethod { get; set; }
    }
}
