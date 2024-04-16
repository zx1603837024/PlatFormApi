using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Rates.Dtos
{
    /// <summary>
    /// 时间段设置  0-30 分钟 2 元
    /// </summary>
    public class CarTimeQuantumModel
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public double beginTime { get; set; }
        /// <summary>
        /// 
        /// 结束时间
        /// </summary>
        public double endTime { get; set; }
        /// <summary>
        /// 收费方式
        /// </summary>
        public string RateMethod { get; set; }
        /// <summary>
        /// 收费金额
        /// </summary>
        public string TimeQuantumMoney { get; set; }
    }
}
