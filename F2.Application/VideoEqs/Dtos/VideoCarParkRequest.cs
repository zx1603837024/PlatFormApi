using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.VideoEqs.Dtos
{
    public class VideoCarParkRequest
    {
        /// <summary>
        /// 一次停车行为所有事件将具有相同的guid
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// 设备序列号
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

        public int parking_id { get; set; }
        public string parking_name { get; set; }
        public int berth_code { get; set; }
        /// <summary>
        /// 泊位代码（客户内部使用的泊位代码，由客户提供）
        /// </summary>
        public string berth_unique { get; set; }
        public string berth_name { get; set; }
        public string berth_confidence { get; set; }
        public Plate plate { get; set; }
        public List<Images> images { get; set; }
        public string occur_time { get; set; }
        public string features { get; set; }
        public string park_state { get; set; }
        public string shot_speed { get; set; }
        public string revise { get; set; }
    }

    public class VideoCarParkRepose
    {
        public string code { get; set; }
        public string message { get; set; }
    }


    public class Plate 
    {
        public string plate_number { get; set; }
        public string plate_type { get; set; }

        public int plate_color { get; set; }
        public string confidence { get; set; }
    }

    public class Images
    {
        public int image_type { get; set; }
        public string image_url { get; set; }
    }

    public class VideoCarParkRequestJSC
    {
        /// <summary>
        /// 泊位编号
        /// </summary>
        public string berthCode { get; set; }

        /// <summary>
        /// 抓拍时间,时间戳
        /// </summary>
        public long capTime { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string deviceCode { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public String carNum { get; set; }

        /// <summary>
        /// 车牌颜色
        /// </summary>
        public int carNumColor { get; set; }

        /// <summary>
        /// 车牌颜色
        /// </summary>
        public int carNumType { get; set; }

        /// <summary>
        /// 全景图片url
        /// </summary>
        public string imageRef { get; set; }

        /// <summary>
        /// 车牌局部图片url
        /// </summary>
        public string objImageRef { get; set; }

        public int event_type { get; set; }
    }
}
