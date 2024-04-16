using F2.Application.Sensors.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Sensors
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISensorAppService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        string SendDeviceByINmotion(INmotionDto dto);
    }
}
