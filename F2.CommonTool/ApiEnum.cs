using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTool
{
    public class ApiEnum
    {
        /// <summary>
        /// State字段用的这个枚举，通用
        /// </summary>
        public enum VideoEqParkType
        {
            空车位 = 1,
            驶入 = 2,
            有车 = 3,
            使出 = 4,
        }

        public enum VideoEqParkHighType
        {
            入场 = 1,
            停稳 = 2,
            出场 = 4,
            空场 = 8,
        }

        public enum ParkingAbnormalType
        {
            无报警 = 0,
            视频模糊 = 1,
            车牌遮挡 = 2,
            占用双车位=3,
            覆盖车衣 = 4,
            无牌车 = 7,
            井盖状态 = 8,
            视频遮挡 = 9,
            车位异常=10,
            设备告警 = 11,
            超长时间占用 = 12,
            确定出场报警 = 13,
            疑似识别错误=14,
            无牌车占位报警 = 16,
            非机动车报警 = 32,
            禁止区域停车报警 = 64,
            逆向停车 = 128,
            禁止时段停车 = 256,
            各种压线停车 = 512,
            其他异常 = 1024,
        }
    }
}
