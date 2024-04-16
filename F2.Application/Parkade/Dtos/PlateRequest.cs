using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parkade.Dtos
{
    public class PlateRequest
    {
        public AlarmInfoPlate alarmInfoPlate { get; set; }
    }
    public class AlarmInfoPlate {
        public Result result { get; set; }
        /// <summary>
        /// 设备序列号
        /// </summary>
        public string serialno { get; set; }
    }
    public class Result
    {
        public PlateResult plateResult { get; set; }
    }
    public class PlateResult
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        public string license { get; set; }
        /// <summary>
        /// 置信度
        /// </summary>
        public int confidence { get; set; }
        /// <summary>
        /// 识别结果车牌 ID
        /// </summary>
        public int plateid { get; set; }
        /// <summary>
        /// 车牌小图片
        /// </summary>
        public string imageFragmentFile { get; set; }
        /// <summary>
        /// 车辆大图片
        /// </summary>
        public string imageFile { get; set; }

        public TimeStamp timeStamp { get; set; }
    }
    public class TimeStamp
    {
        public Timeval timeval { get; set; }
    }
    public class Timeval
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        public long sec { get; set; }
    }
}
