using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Rates.Dtos
{
    public class RateModel
    {
        public int Id { get; set; }
        /// <summary>
        /// 费率名称
        /// </summary>
        public string RateName { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 车辆费率列表
        /// </summary>
        public List<CarRateModel> CarRateList { get; set; }
        /// <summary>
        /// 收费时间段列表
        /// </summary>
        public List<TimeSettingModel> TimeSettingList { get; set; }
    }
}
