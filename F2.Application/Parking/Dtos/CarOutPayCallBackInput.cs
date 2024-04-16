using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parking.Dtos
{
    /// <summary>
    /// 扫码出场
    /// </summary>
    public class CarOutPayCallBackInput
    {
        /// <summary>
        /// 停车场ID
        /// </summary>
        public string parkingId { get; set; }

        /// <summary>
        /// 停车场平台编号
        /// </summary>
        public string recordNo { get; set; }

        /// <summary>
        /// 账单编号
        /// </summary>
        public string billNo { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal payFee { get; set; }

        public enum EPayType
        {
            现金=1,
            微信=2,
            支付宝=3,
            网银=4,
            电子钱包=5
        } 

        /// <summary>
        /// 支付方式
        /// </summary>
        public EPayType payType { get; set; }
    }
}
