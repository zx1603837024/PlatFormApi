using F2.Application.Sensors;
using F2.Application.Sensors.Dtos;
using System.Web.Http;

namespace F2Api.Controllers
{
    /// <summary>
    /// 第三方地磁推送接口
    /// </summary>
    public class InterfaceSensorController : ApiController
    {
        #region Var
        private readonly ISensorAppService _sensorAppService;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public InterfaceSensorController()
        {
            _sensorAppService = new SensorAppService();
        }
        /// <summary>
        /// 地磁接口
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public string SendDeviceByINmotion([FromBody]INmotionDto dto)
        {
            return _sensorAppService.SendDeviceByINmotion(dto);
        }
    }
}
