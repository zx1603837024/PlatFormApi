using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parking.Dtos
{
    /// <summary>
    /// 获取停车账单
    /// </summary>
    public class GetParkingBillOutPut
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        public string plateNumber { get; set; }

        /// <summary>
        /// 入场时间
        /// </summary>
        public string carInTimeStr { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string nickName { get; set; }

        /// <summary>
        /// 停车场ID
        /// </summary>
        public string parkingId { get; set; }

        /// <summary>
        /// 支付账单编号
        /// </summary>
        public string billNo { get; set; }

        /// <summary>
        /// 停车订单编号
        /// </summary>
        public string recordNo { get; set; }

        /// <summary>
        /// 应付金额
        /// </summary>
        public decimal payableAmount { get; set; }
    }
}
