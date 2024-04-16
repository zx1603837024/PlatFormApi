using F2.Application.PatrolCar.Dtos;
using F2.Application.VideoEqs.Dtos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.PatrolCar
{
    public interface IPatrolCarService
    {
        /// <summary>
        /// 巡检车停车数据
        /// </summary>
        /// <param name="dto"></param>
        PatrolCarResponse PatrolCarStopData(PatrolCarRequest dto);
        /// <summary>
        /// 巡检车报警数据
        /// </summary>
        /// <param name="dto"></param>
        PatrolCarResponse PatrolCarFaultData(PatrolCarRequest dto);
        /// <summary>
        /// 巡检车状态数据
        /// </summary>
        /// <param name="dto"></param>
        PatrolCarResponse PatrolCarStateData(PatrolCarRequest dto);
        /// <summary>
        /// 巡检车位置信息上报
        /// </summary>
        /// <param name="dto"></param>
        PatrolCarResponse PatrolCarStationData(PatrolCarRequest dto);
    }

}
