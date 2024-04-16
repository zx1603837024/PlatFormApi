using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.VideoEqs.Dtos
{
    public class VideoEqParkRequest
    {
        /// <summary>
        /// 标识
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// 设备Id,用于区分设备
        /// </summary>
        public string SN { get; set; }
        /// <summary>
        /// 在【设备管理平台】中配置日 泊位号
        /// </summary>
        public string berthCode { get; set; }
        /// <summary>
        /// 车位状态。1=空车位，2=驶入，3=有车，4=驶出
        /// </summary>
        public int STATE { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string PLATE { get; set; }
        /// <summary>
        /// 车牌颜色： 0, 未知。1,蓝牌。 2,黄牌。3，白牌。 4,黑牌。5，绿牌。
        /// </summary>
        public int plateColor { get; set; }
        /// <summary>
        /// 识别时间 yyyy-MM-dd HH:mm:ss 
        /// </summary>
        public string recTime { get; set; }
        /// <summary>
        /// 图片转换成的base64编辑字符串
        /// </summary>
        public string ossPath { get; set; }
        /// <summary>
        /// 视频桩类型，内部编号。
        /// </summary>
        public string VMODEL { get; set; }
        /// <summary>
        /// 创建时间，yyyy-MM-dd HH:mm:ss 
        /// </summary>
        public string createTime { get; set; }
        /// <summary>
        /// 电池电量的百分比。
        /// </summary>
        public string powerp { get; set; }
    }
}
