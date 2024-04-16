using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Rates.Dtos
{
    /// <summary>
    /// 车辆费率
    /// </summary>
    public class CarRateModel
    {
        /// <summary>
        /// 车辆类型
        /// </summary>
        public string CarType { get; set; }

        /// <summary>
        /// 免费时间
        /// </summary>
        public string FreeTime { get; set; }

        public bool ContentFreeTimeFlag { get; set; }
        /// <summary>
        /// 日最大收费金额 0表示无最大收费金额
        /// </summary>
        public decimal DayMaxMoney { get; set; }
        /// <summary>
        /// 次最大收费金额 0表示无最大收费金额
        /// </summary>
        public decimal OnceMaxMoney { get; set; }
        /// <summary>
        /// 时间段列表
        /// </summary>
        public List<CarTimeQuantumModel> CarTimeQuantumList { get; set; }
        /// <summary>
        /// 标准收费列表
        /// </summary>
        public List<CarFeeScaleModel> CarFeeScaleList { get; set; }


        public List<CarFeeModel> CarFeeList { get; set; }


    }
}
