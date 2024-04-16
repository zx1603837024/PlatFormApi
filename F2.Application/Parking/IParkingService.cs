using F2.Application.Parking.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parking
{
    public interface IParkingService
    {
        /// <summary>
        /// 推送停车记录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        PushParingRecordOutput pushParingRecord(PushParingRecordInput input);

        /// <summary>
        /// 保存月租信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        bool SaveMonthlyRent(SaveMonthlyRentInput input);

        /// <summary>
        /// 保存月租信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        bool ModifyMonthlyRent(ModifyMonthlyRentInput input);

        /// <summary>
        /// 扫码入场
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        bool RemoteOpen(RemoteOpenInput input);

        /// <summary>
        /// 获取停车订单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        GetParkingBillOutPut GetParkingBill(GetParkingBillInput input);

        /// <summary>
        /// 支付回调
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        bool CarOutPayCallBack(CarOutPayCallBackInput input);

        /// <summary>
        /// 删除月租
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        bool DelMonthlyRent(DelMonthlyRentInput input);
    }
}
