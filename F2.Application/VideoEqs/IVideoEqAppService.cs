using F2.Application.VideoEqs.Dtos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.VideoEqs
{
    public interface IVideoEqAppService
    {
        /// <summary>
        /// 视频设备状态数据
        /// </summary>
        /// <param name="dto"></param>
        VideoEqParkHighRepose PostStateHighData(Hashtable dto);
        /// <summary>
        /// 视频设备停车数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        VideoEqParkHighRepose PostVideoEqParkHighData(VideoEqParkHighRequest dto);

        /// <summary>
        /// 视频设备报警数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        VideoEqParkHighRepose PushFaultDataForHigh(VideoEqParkHighRequest dto);

        /// <summary>
        /// 视频设备入场修正，通过唯一Id找到车牌号，更新
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        VideoEqParkHighRepose pushFixDataForHigh(VideoEqParkHighRequest dto);

        /// <summary>
        /// 视频设备停车数据补图片
        /// </summary>
        VideoEqParkHighRepose pushPieceForHigh(VideoEqParkHighRequest dto);

        /// <summary>
        /// 视频设备抓拍
        /// </summary>
        VideoEqParkHighRepose pushCaptureForHigh(VideoEqParkHighRequest dto);
    }
}

