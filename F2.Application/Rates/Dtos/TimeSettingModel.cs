using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Rates.Dtos
{
    /// <summary>
    /// 收费时间段
    /// </summary>
    public class TimeSettingModel
    {
        /// <summary>
        /// 车辆类型  	0 所有   1	 大型车   2 小型车  3 	摩托车
        /// </summary>
        public string CarType { get; set; }
        /// <summary>
        ///收费方式（0 按时  1 按次）
        /// </summary>
        public string RateMethod { get; set; }

        public string beginTime { get; set; }
        public string endTime { get; set; }
        /// <summary>
        /// 停车多天计费方式 0  按停车总和  1  分天计算
        /// </summary>
        public string ManyDayMethod { get; set; }

    }
}
