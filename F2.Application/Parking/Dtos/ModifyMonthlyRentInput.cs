using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parking.Dtos
{
    public class ModifyMonthlyRentInput
    {
        /// <summary>
        /// 微信openid
        /// </summary>
        public string openId { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string plateNumber { get; set; }

        /// <summary>
        /// 商户ID
        /// </summary>
        public int tenantId { get; set; }

        /// <summary>
        /// 停车场ID
        /// </summary>
        public int parkId { get; set; }

        /// <summary>
        /// 开通日期
        /// </summary>
        public DateTime beginDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime endDate { get; set; }


        /// <summary>
        /// 车类型
        /// </summary>
        public SaveMonthlyRentInput.ECarType carType { get; set; }
    }
}
