using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Rates.Dtos
{
    /// <summary>
    /// 费率计算
    /// </summary>
    public class RateCalculateModel
    {
        /// <summary>
        /// 计算金额
        /// </summary>
        public decimal CalculateMoney { get; set; }
        /// <summary>
        /// 停车时长
        /// </summary>
        public double ParkTime { get; set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public string exceptionMsg { get; set; }
    }
}
