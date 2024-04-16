using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Rates.Dtos
{
    /// <summary>
    /// 停车时间类
    /// </summary>
    public class ParkTime
    {
        /// <summary>
        /// 当天停车开始时间
        /// </summary>
        public DateTime beginTime { get; set; }
        /// <summary>
        /// 当天停车结束时间
        /// </summary>
        public DateTime endTime { get; set; }
        /// <summary>
        /// 当天开始时间段
        /// </summary>
        public double quantumBegin { get; set; }
        /// <summary>
        /// 当天结束时间段
        /// </summary>
        public double quantumEnd { get; set; }
        /// <summary>
        ///当天停车总时长
        /// </summary>
        public double timeTotal { get; set; }
        //停车费用
        public decimal parkMoney { get; set; }
        //当天已收金额
        public decimal alreadyCharge { get; set; }
    }
}
