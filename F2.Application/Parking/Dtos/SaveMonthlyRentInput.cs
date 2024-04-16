using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parking.Dtos
{
    /// <summary>
    /// 包月
    /// </summary>
    public class SaveMonthlyRentInput
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
        /// 停车场ID
        /// </summary>
        public  int parkId { get; set; }

        /// <summary>
        /// 商户ID
        /// </summary>
        public int tenantId { get; set; }

        /// <summary>
        /// 包月时长
        /// </summary>
        public int days { get; set; }

        /// <summary>
        /// 车类型
        /// </summary>
        public enum ECarType
        {
            小车 = 1,
            大车=2,
            摩托车=3,
            小型新能源车 =4,
            大型新能源车=5,
            超大车=6
        }

        /// <summary>
        /// 车类型
        /// </summary>
        public ECarType carType { get; set; }

        /// <summary>
        /// 开通日期
        /// </summary>
        public DateTime beginDate { get; set; } = DateTime.Now;

    }
}
