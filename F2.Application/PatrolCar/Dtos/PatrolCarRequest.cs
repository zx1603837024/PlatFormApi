using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.PatrolCar.Dtos
{
    public class PatrolCarRequest
    {
        /// <summary>
        /// 一次停车行为所有事件将具有相同的guid
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// 设备号
        /// </summary>
        public string sn { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        public string device_type { get; set; }
        /// <summary>
        /// 事件类型
        /// </summary>
        public int event_type { get; set; }
        /// <summary>
        /// 泊位代码（客户内部使用的泊位代码，由客户提供）
        /// </summary>
        public string berth_unique { get; set; }
        /// <summary>
        /// 泊位名
        /// </summary>
        public string berth_name { get; set; }
        /// <summary>
        /// 泊位置信度
        /// </summary>
        public int? berth_confidence { get; set; }
        /// <summary>
        /// 车牌信息
        /// </summary>
        public PlateInfo plate { get; set; }
        /// <summary>
        /// 车前图片路径
        /// </summary>
        public List<ImagesInfo> images { get; set; }
        /// <summary>
        /// 事件发生时间
        /// </summary>
        public long occur_time { get; set; }
        /// <summary>
        /// 车辆特征值
        /// </summary>
        public int? features { get; set; }
        /// <summary>
        /// 停车状态
        /// </summary>
        public int? park_state { get; set; }
        /// <summary>
        /// 拍摄车速
        /// </summary>
        public int? shot_speed { get; set; }
        /// <summary>
        /// 是否修正前一次上报结果（0：无需修正 1：修正）
        /// </summary>
        public int? revise { get; set; }
        /// <summary>
        /// 设备状态（0在线，1离线）
        /// </summary>
        public int device_state { get; set; }

        /// <summary>
        /// 置信度
        /// </summary>
        public int Trust { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public string X { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public string Y { get; set; }

    }
    public class PlateInfo
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        public string plate_number { get; set; }
        /// <summary>
        /// 车牌类型
        /// </summary>
        public int plate_type { get; set; }
        /// <summary>
        /// 车牌颜色
        /// </summary>
        public int? plate_color { get; set; }
        /// <summary>
        /// 车牌置信度
        /// </summary>
        public int? confidence { get; set; }
    }
    public class ImagesInfo
    {
        /// <summary>
        /// 车前车牌图片路径
        /// </summary>
        public int? image_type { get; set; }
        /// <summary>
        /// 车后图片路径
        /// </summary>
        public string image_url { get; set; }
    }
}
