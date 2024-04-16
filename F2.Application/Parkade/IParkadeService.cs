using F2.Application.Parkade.Dtos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parkade
{
    public interface IParkadeService
    {
        /// <summary>
        /// 车牌识别结果接收
        /// </summary>
        /// <param name="dto"></param>
        Hashtable PlateResultReceive(ParkadeRequest dto);
        /// <summary>
        /// 心跳数据接收
        /// </summary>
        /// <param name="dto"></param>
        Hashtable HeartBeatReceive(ParkadeRequest dto);
        // <summary>
        /// 开闸服务
        /// </summary>
        /// <param name="dto"></param>
        Hashtable OpenPoleService(Hashtable dto);
        // <summary>
        /// MQ开闸回调
        /// </summary>
        /// <param name="dto"></param>
        Hashtable OpenPoleCallBack(Hashtable dto);
    }
}
