using F2.Core.Extensions.DataExtend;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parking.ZhiBo
{
    public class NoPlateQRcodeResponse
    {
        /// <summary>
        /// 1成功0失败
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 错误代码 描述
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// json格式的数据
        /// </summary>
        public string data { get; set; }

        /// <summary>
        /// 实体信息
        /// </summary>
        [JsonIgnore]
        public NoPlateQRcode JsonData => data.IsNullOrEmpty() ? null : data.DeserializeObject<NoPlateQRcode>();

        
    }
    public class NoPlateQRcode
    {
        /// <summary>
        /// 扫码进或出的时间
        /// </summary>
        public string in_time { get; set; }

        /// <summary>
        /// 车牌号码
        /// 1.无牌车扫码进出该值为传入的虚拟车牌
        /// 2.有牌车扫码出由车场返回当前通道的车牌号码
        /// </summary>
        public string plate_number { get; set; }

        /// <summary>
        /// 车场生成的订单号
        /// </summary>
        public string order_no { get; set; }

        /// <summary>
        /// 本次停车应收总费用，单位元
        /// </summary>
        public string upay_fee { get; set; }

        /// <summary>
        /// 本次停车实收总费用，单位元
        /// </summary>
        public string total_fee { get; set; }

        /// <summary>
        /// 优惠总金额，单位元
        /// </summary>
        public string discount_fee { get; set; }

        /// <summary>
        /// 优惠总时长，单位分钟 
        /// </summary>
        public string discount_time { get; set; }

        /// <summary>
        /// 已缴金额
        /// </summary>
        public string pay_fee { get; set; }
    }
}
