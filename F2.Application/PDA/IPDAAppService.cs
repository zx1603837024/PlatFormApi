using F2.Application.PDA.Dtos;
using F2.Core.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.PDA
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPDAAppService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        PdaModel DownParameter(string access_token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginToken"></param>
        /// <returns></returns>
        List<BerthsecDto> GetBerthsecList(AbpUserLoginToken loginToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ErrorInfo InsertCarInParking(CarinParkingDto input);


        ErrorInfo InsertCarInParkingRemedy(CarinParkingDto input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ErrorInfo InsertCarOutParking(CaroutParkingDto input);

        ErrorInfo InsertCarOutParkingRemedy(CaroutParkingDto input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        OtherAccountModel SearchOtherAccount(QueryCardInput input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        GetEscapeDetailList GetEscapeDetailList(EscapeDetailInput input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ErrorInfo UpdateFeeBack(FeeBackDto input);

        /// <summary>
        /// 批量追缴数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        ErrorInfo UpdateAllFeeBack(FeeBackDto input);

        /// <summary>
        /// 更改远程出场标记
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        int UpdateRemoteGuidStatus(string guid, string access_token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="BerthsecId"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        int InsertRemoteGuid(string guid, int BerthsecId, string access_token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        List<RemoteGuidDto> GetRemoteGuids(string access_token);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="SyncTime"></param>
        /// <param name="Berthesclist"></param>
        /// <returns></returns>
        List<BerthSensorDto> GetBerthsSyn(string SyncTime, string Berthesclist);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        string CheckEquipmentStatus(string access_token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="plateNumber"></param>
        /// <returns></returns>
        List<BerthDto> GetBerthInfoByPlateNumber(string access_token, string plateNumber);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="businessid"></param>
        /// <param name="pictype"></param>
        /// <param name="pic"></param>
        /// <param name="fileType">文件扩展名，如jpg，jpeg等</param>
        /// <param name="createtime"></param>
        /// <returns></returns>
        int PhotoUpLoadToAndroid(string guid, int businessid, int pictype, byte[] pic,string fileType, string createtime, string access_token);

        /// <summary>
        /// 判断图片是否上传
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="businessid"></param>
        /// <returns></returns>
        int GetBusinessDetailPicture(string guid, int businessid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        int UpdateGps(string x, string y, string access_token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="berthsecid"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        int EmployeeOutLineLogout(string berthsecid, string access_token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="ExceptionMsg"></param>
        /// <returns></returns>
        int UploadAndroidExceptionLog(string guid, string ExceptionMsg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        string GetSensorBusinessdetail(string guid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="OldVersion"></param>
        /// <param name="PDA"></param>
        /// <param name="Type"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        string CheckVersion(string OldVersion, string PDA, int Type, string access_token);

        /// <summary>
        /// 收费员签到
        /// </summary>
        /// <param name="access_token"></param>
        void EmployeeCheckIn(string access_token);


        /// <summary>
        /// 获取二维码码串
        /// </summary>
        /// <param name="total_amount"></param>
        /// <param name="subject"></param>
        /// <param name="out_trade_no"></param>

        object GetAliPayCode(string total_amount,string subject,string out_trade_no);

        /// <summary>
        /// 统一收单交易创建
        /// </summary>
        /// <param name="total_amount"></param>
        /// <param name="subject"></param>
        /// <param name="total_amount"></param>
        /// <param name="buyer_id"></param>

        object CreatDeal(string total_amount, string subject, string out_trade_no, string buyer_id);


        /// <summary>
        /// 统一收单线下交易查询
        /// </summary>
        /// <param name="out_trade_no"></param>
        object QueryAliPay(string out_trade_no);

        /// <summary>
        /// 手持端修改号牌
        /// </summary>
        /// <param name="PlateNumber"></param>
        /// <param name="CarType"></param>
        /// <returns></returns>
        ErrorInfo ModifyCarPlateNumber(string PlateNumber, string CarType,string Guid);
    }
}
