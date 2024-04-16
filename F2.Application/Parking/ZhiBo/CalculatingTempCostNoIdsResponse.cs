using F2.Core.Extensions.DataExtend;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parking.ZhiBo
{
    /// <summary>
    /// 获取临停缴费接口
    /// </summary>
    public class CalculatingTempCostNoIdsResponse
    {
        /// <summary>
        /// 状态值 0失败 1成功
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// 消息提示
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// json格式的数据
        /// </summary>
        public string data { get; set; }

        /// <summary>
        /// 实体数据
        /// </summary>
        [JsonIgnore]
        public CalculatingTempCostNoIdsData JsonData => data.IsNullOrWhiteSpace() ? null : data.DeserializeObject<CalculatingTempCostNoIdsData>();
       
    }

    public class CalculatingTempCostNoIdsData
    {
        /// <summary>
        /// 车场唯一编号
        /// </summary>
        public string park_id { get; set; }

        /// <summary>
        /// 订单编号
        /// 平台生成的订单号
        /// </summary>
        public string order_no { get; set; }

        /// <summary>
        /// 车牌号码
        /// </summary>
        public string plate_number { get; set; }

        /// <summary>
        /// 折扣金额
        /// 折扣金额已元为单位
        /// </summary>
        public string discount_fee { get; set; }

        /// <summary>
        /// 总金额
        /// 总金额已元为单位
        /// </summary>
        public string total_fee { get; set; }

        /// <summary>
        /// 已缴金额
        /// 已缴金额已元为单位
        /// </summary>
        public string pay_fee { get; set; }

        /// <summary>
        /// 需缴金额
        /// 需缴金额已元为单位
        /// </summary>
        public string upay_fee { get; set; }

        /// <summary>
        /// 入场时间
        /// 格式yyyy-mm-dd hh:mm:ss
        /// </summary>
        public string in_time { get; set; }

        /// <summary>
        /// 出场时间
        /// 格式yyyy-mm-dd hh:mm:ss
        /// </summary>
        public string exit_time { get; set; }
    }
}
