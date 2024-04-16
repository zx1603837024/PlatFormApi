using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parking.Dtos
{
    public class PushParingRecordInput
    {
        /// <summary>
        /// 车场ID
        /// </summary>
        public string park_id { get; set; }

        /// <summary>
        /// 车场名字
        /// </summary>
        public string park_name { get; set; }

        /// <summary>
        /// 车辆入场时由场库生成的唯一编号
        /// </summary>
        public string record_id { get; set; }

        /// <summary>
        /// 车牌号码
        /// </summary>
        public string plate_number { get; set; }

        /// <summary>
        /// 进场时间 
        /// 时间戳格式 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string entrance_time { get; set; }

        /// <summary>
        /// 出场时间(入场记录没有)
        /// 时间戳格式 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string exit_time { get; set; }

        /// <summary>
        /// 进场通道id
        /// </summary>
        public string entrancegate_id { get; set; }

        /// <summary>
        /// 出场通道id(入场记录没有)
        /// </summary>
        public string exitgate_id { get; set; }

        /// <summary>
        /// 放行类型
        /// </summary>
        public EreleaseType? release_type { get; set; }

        /// <summary>
        /// 车类描述id
        /// </summary>
        public string cartype_id { get; set; }

        /// <summary>
        /// 车型描述id
        /// </summary>
        public string carmodel_id { get; set; }

        /// <summary>
        /// 入场类型
        /// 0正常
        /// 1过期转临停
        /// 2车位占用转临停
        /// </summary>
        public EenterType? enter_type { get; set; }

        /// <summary>
        /// 车牌类型
        /// </summary>
        public EplateColor? plate_color { get; set; }

        /// <summary>
        /// 是否出场
        /// 0是出场1入场
        /// </summary>
        public int is_exit { get; set; }

        public int app_key { get; set; }
    }

    /// <summary>
    /// 放行类型
    /// </summary>
    public enum EreleaseType
    {
        正常放行=0,
        收费放行=1,
        免费放行=2,
        异常放行=3
    }

    /// <summary>
    /// 入场类型
    /// </summary>
    public enum EenterType
    {
        正常=0,
        过期转临停=1,
        车位占用转临停=2
    }

    /// <summary>
    /// 车牌类型
    /// </summary>
    public enum EplateColor
    {
        蓝牌=0,
        黑牌=1,
        黄牌=2,
        新黄牌=3,
        黄色后牌=4,
        警车=5,
        军车=6,
        新黄色后=7,
        武警=8,
        新白牌=9
    }

}
