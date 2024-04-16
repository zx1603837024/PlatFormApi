using F2.Core.Extensions.DataExtend;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parking.ZhiBo
{
    /// <summary>
    /// 临停支付通知接口
    /// </summary>
    public class PayTempCostRequest
    {
        /// <summary>
        /// 车场唯一编号
        /// </summary>
        public string park_id { get; set; }

        /// <summary>
        /// 车牌号码
        /// </summary>
        public string plate_number { get; set; }

        /// <summary>
        /// 获取订单接口返回的订单号
        /// </summary>
        public string order_no { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public string pay_fee { get; set; }

        /// <summary>
        /// 支付订单编号
        /// </summary>
        public string order_id { get; set; }

        /// <summary>
        /// 支付方式(1.现金、2.微信、3.支付宝、4.网银、5.电子钱包)
        /// </summary>
        public string pay_type { get; set; }

        /// <summary>
        /// appkey
        /// </summary>
        public string appKey=>ConfigurationManager.AppSettings["ZhiBoAppKey"].MD5Encrypt().ToUpper();

        /// <summary>
        /// 签名
        /// </summary>
        public string sign
        {
            get
            {
                var stringa = $"park_id={park_id}&order_no={order_no}&plate_number={plate_number}";
                var stringsigntemp = stringa + $"&appKey={appKey}";
                return stringsigntemp.MD5Encrypt().ToUpper();
            }
        }
    }
}
