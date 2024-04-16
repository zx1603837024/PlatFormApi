using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.VideoEqs.Dtos
{
    public class VideoEqParkHighRequest
    {
        /// <summary>
        /// 结果类型
        /// </summary>
        public int evt { get; set; }

        /// <summary>
        /// 停车行为Id,同一次进出完成，将拥有同一个停车行为ID
        /// </summary>
        public string parkingActId { get; set; }

        /// <summary>
        /// 事件发生时间（单位：秒）
        /// </summary>
        public long happenTime { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string plateNumber { get; set; }

        /// <summary>
        /// 车牌颜色：0:未知; 1:蓝色; 2:黄色; 3:白色; 4:黑色; 5:绿色;
        /// </summary>
        public int plateColor { get; set; }

        /// <summary>
        /// 图片路由列表
        /// </summary>
        public string picUrl { get; set; }

        /// <summary>
        /// 车场编号
        /// </summary>
        public int parkingCode { get; set; }

        /// <summary>
        /// 车场名称
        /// </summary>
        public string parkingName { get; set; }

        /// <summary>
        /// 泊位号
        /// </summary>
        public string berthCode { get; set; }

        /// <summary>
        /// 行为可信度（0-100）
        /// </summary>
        public int actionCredible { get; set; }

        /// <summary>
        /// 设备序列号
        /// </summary>
        public string deviceSn { get; set; }

        /// <summary>
        /// 异常停车类型
        /// </summary>
        public int parkingAbnormalType { get; set; }

        /// <summary>
        /// 车牌特写图
        /// </summary>
        public string plateNumberUrl { get; set; }

        /// <summary>
        /// 标识是否为系统自动构造的记录。默认为否(0:否 1:是）
        /// </summary>
        public int autoGenerate { get; set; }

        /// <summary>
        /// 车身颜色
        /// </summary>
        public int carColor { get; set; }

        /// <summary>
        /// 车辆类型
        /// </summary>
        public int carType { get; set; }

        public int Trust { get; set; }
        public int? deviceState { get; set; }
    }

    public class VideoEqParkHighRepose 
    {
        public int? errorcode { get; set; }
        public string message { get; set; }
    }
}
