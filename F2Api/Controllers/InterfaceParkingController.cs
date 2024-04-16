using F2.Application.Parking;
using F2.Application.Parking.Dtos;
using F2.Application.PDA;
using F2.Application.PDA.Dtos;
using F2.Core.Extensions.Log;
using F2.Core.Extensions.Models;
using F2.Core.Extensions.WebMvc;
using System;
using System.Web.Http;
using Newtonsoft.Json;
using F2.Core.Extensions.DataExtend;

namespace F2Api.Controllers
{
    /// <summary>
    /// 停车场接口
    /// </summary>
    public class InterfaceParkingController: ApiController
    {
        #region
        private readonly IParkingService _parkingService;
        #endregion

        #region Aop入口
        internal CommonOutput ApiFactoryRun(System.Func<object> func)
        {
            var result = new CommonOutput() { status= EPushParingRecordOutputStatus.成功};
            try
            {
                var res = func.Invoke();
                result.message = res;
                return result;
            }
            catch (Exception ex)
            {
                result.status = EPushParingRecordOutputStatus.失败;
                result.message = ex.Message;
                //记录ex日志
                return result;
            }
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public InterfaceParkingController()
        {
            _parkingService = new ParkingService();
        }

        /// <summary>
        /// 推送停车记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public PushParingRecordOutput PushParingRecord(PushParingRecordInput input)
        {
            Logger.Log.Debug($"PushParingRecord:{input.ToJson()}");
           return  _parkingService.pushParingRecord(input);
        }

        /// <summary>
        /// 保存月租
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public CommonOutput SaveMonthlyRent(SaveMonthlyRentInput input)
        {
            return ApiFactoryRun(() =>
            {
                return _parkingService.SaveMonthlyRent(input);
            });
        }

        /// <summary>
        /// 编辑月租信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public CommonOutput ModifyMonthlyRent(ModifyMonthlyRentInput input)
        {
            return ApiFactoryRun(() =>
            {
                return _parkingService.ModifyMonthlyRent(input);
            });
        }

        /// <summary>
        /// 删除月租信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public CommonOutput DelMonthlyRent(DelMonthlyRentInput input)
        {
            return ApiFactoryRun(() =>
            {
                return _parkingService.DelMonthlyRent(input);
            });
        }

        /// <summary>
        /// 扫码入场
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public CommonOutput RemoteOpen(RemoteOpenInput input)
        {
            return ApiFactoryRun(() =>
            {
                return _parkingService.RemoteOpen(input);
            });
        }

        /// <summary>
        /// 获取账单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public CommonOutput GetParkingBill(GetParkingBillInput input)
        {
            return ApiFactoryRun(() =>
            {
                return _parkingService.GetParkingBill(input);
            });
        }

        /// <summary>
        /// 支付回调
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public CommonOutput CarOutPayCallBack(CarOutPayCallBackInput input)
        {
            return ApiFactoryRun(() =>
            {
                return _parkingService.CarOutPayCallBack(input);
            });
        }
    }
}