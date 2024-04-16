using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using F2.Application.PDA.Dtos;
using F2.Core.Extensions;
using F2.Core.Extensions.Models;
using F2.Application.WebChat;
using F2.Application.Smss;
using F2.Application.Smss.Dtos;
using System.IO;
using Aop.Api;
using Aop.Api.Request;
using Aop.Api.Response;
using Newtonsoft.Json;
using System.Configuration;
using F2.Common;

namespace F2.Application.PDA
{
    /// <summary>
    /// pda接口程序
    /// </summary>
    public class PDAAppService : IPDAAppService
    {
        #region Var
        private readonly IWebChatAppService _webChatAppService;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public PDAAppService()
        {
            _webChatAppService = new WebChatAppService();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public PdaModel DownParameter(string access_token)
        {
            var loginToken = GetLoginToken(access_token);
            PdaModel pdaModel = new PdaModel();
            pdaModel.PrintList = GetPrintList(loginToken);
            pdaModel.WhiteList = GetWhiteList(loginToken);
            pdaModel.MonthlyCarList = GetMonthlyList(loginToken);
            pdaModel.Berths = GetBerthList(loginToken);
            pdaModel.BerthsecList = GetBerthsecList(loginToken);
            pdaModel.Employee = GetEmployeeInfo(loginToken.EmployeeId);
            var ticketList = GetTicketList(loginToken);
            pdaModel.CarInTicketCss = ticketList.First(entry => entry.Status == "CarIn");
            pdaModel.CarOutTicketCss = ticketList.First(entry => entry.Status == "CarOut");
            pdaModel.OweTicketCSS = ticketList.First(entry => entry.Status == "OweDetail");
            pdaModel.RepayTicketCss = ticketList.First(entry => entry.Status == "Repay");
            pdaModel.DayChargeTicketCss = ticketList.First(entry => entry.Status == "DayCharge");

            #region 参数
            var settings = SettingStoreAppService.GetSettingList(loginToken.TenantId);
            pdaModel.SysInfoPassword = settings.FirstOrDefault(entity => entity.Name == "PDAPassword1").Value;
            pdaModel.CardMenuPassword = settings.FirstOrDefault(entity => entity.Name == "PDAPassword2").Value;
            pdaModel.HrTotalPassword = settings.FirstOrDefault(entity => entity.Name == "PDAPassword3").Value;
            pdaModel.LogoutPassword = settings.FirstOrDefault(entity => entity.Name == "PDAPassword4").Value;
            pdaModel.Password5 = settings.FirstOrDefault(entity => entity.Name == "PDAPassword5").Value;
            pdaModel.PDAChar = settings.FirstOrDefault(entity => entity.Name == "PDAChar").Value;
            pdaModel.PDAInCarEscape = bool.Parse(settings.FirstOrDefault(entity => entity.Name == "PDAInCarEscape").Value);
            pdaModel.PDAInCarPhotoFlag = bool.Parse(settings.FirstOrDefault(entity => entity.Name == "PDAInCarPhotoFlag").Value);
            pdaModel.PDAInCarPhotoNum = settings.FirstOrDefault(entity => entity.Name == "PDAInCarPhotoNum").Value;
            pdaModel.PDAOutCarEscape = bool.Parse(settings.FirstOrDefault(entity => entity.Name == "PDAOutCarEscape").Value);
            pdaModel.PDAOutCarPhotoFlag = bool.Parse(settings.FirstOrDefault(entity => entity.Name == "PDAOutCarPhotoFlag").Value);
            pdaModel.PDAOutCarPhotoNum = settings.FirstOrDefault(entity => entity.Name == "PDAOutCarPhotoNum").Value;
            pdaModel.PDARegion = settings.FirstOrDefault(entity => entity.Name == "PDARegion").Value;
            pdaModel.PDAPrepaidFlag = bool.Parse(settings.FirstOrDefault(entity => entity.Name == "PDAPrepaidFlag").Value);
            pdaModel.PDAPrepaid = settings.FirstOrDefault(entity => entity.Name == "PDAPrepaid").Value;//预缴金额
            pdaModel.SensorTimer = bool.Parse(settings.FirstOrDefault(entity => entity.Name == "SensorTimer").Value);//车检器计时
            pdaModel.EscapeBlack = bool.Parse(settings.FirstOrDefault(entity => entity.Name == "EscapeBlack").Value);
            pdaModel.EscapeBlackMoney = int.Parse(settings.FirstOrDefault(entity => entity.Name == "EscapeBlackMoney").Value);
            pdaModel.EscapePhoto = bool.Parse(settings.FirstOrDefault(entity => entity.Name == "EscapePhoto").Value);
            pdaModel.EscapePhotoMoney = int.Parse(settings.FirstOrDefault(entity => entity.Name == "EscapePhotoMoney").Value);
            pdaModel.PeriodPaid = bool.Parse(settings.FirstOrDefault(entity => entity.Name == "PeriodPaid").Value);
            pdaModel.PeriodTime = settings.FirstOrDefault(entity => entity.Name == "PeriodTime").Value;
            pdaModel.PeriodTime1 = settings.FirstOrDefault(entity => entity.Name == "PeriodTime1").Value;


            pdaModel.VideoRecognition = bool.Parse(settings.FirstOrDefault(entity => entity.Name == "VideoRecognition").Value);
            pdaModel.SensorMorningBegin = int.Parse(settings.FirstOrDefault(entity => entity.Name == "SensorMorningBegin").Value);
            pdaModel.SensorMorningDelay = int.Parse(settings.FirstOrDefault(entity => entity.Name == "SensorMorningDelay").Value);
            pdaModel.SensorMorningEnd = int.Parse(settings.FirstOrDefault(entity => entity.Name == "SensorMorningEnd").Value);
            pdaModel.SensorNightBegin = int.Parse(settings.FirstOrDefault(entity => entity.Name == "SensorNightBegin").Value);
            pdaModel.SensorNightDelay = int.Parse(settings.FirstOrDefault(entity => entity.Name == "SensorNightDelay").Value);
            pdaModel.SensorNightEnd = int.Parse(settings.FirstOrDefault(entity => entity.Name == "SensorNightEnd").Value);
            pdaModel.PDASyncData = bool.Parse(settings.FirstOrDefault(entitiy => entitiy.Name == "PDASyncData").Value);
            pdaModel.PrivilegeCarReceipt = bool.Parse(settings.FirstOrDefault(entitiy => entitiy.Name == "PrivilegeCarReceipt").Value);
            pdaModel.UploadSqlite = bool.Parse(settings.FirstOrDefault(entitiy => entitiy.Name == "UploadSqlite").Value);

            pdaModel.AliPay = bool.Parse(settings.FirstOrDefault(entity => entity.Name == "AliPay").Value);//启用支付宝
            pdaModel.WeixinPay = bool.Parse(settings.FirstOrDefault(entity => entity.Name == "WeixinPay").Value);
            //pdaModel.CCBAggregatePay = bool.Parse(settings.FirstOrDefault(entity => entity.Name == "CCBAggregatePay").Value); 
            pdaModel.WeixinDiscount = settings.FirstOrDefault(entity => entity.Name == "WeixinDiscount").Value;

            pdaModel.AccountPay = bool.Parse(settings.FirstOrDefault(entity => entity.Name == "AccountPay").Value);
            pdaModel.AccountDiscount = settings.FirstOrDefault(entity => entity.Name == "AccountDiscount").Value;

            pdaModel.EscapePrint = bool.Parse(settings.FirstOrDefault(entity => entity.Name == "EscapePrint").Value);
            pdaModel.EscapeXingCode = bool.Parse(settings.FirstOrDefault(entity => entity.Name == "EscapeXingCode").Value);
            pdaModel.IPassCardPay = bool.Parse(settings.FirstOrDefault(entity => entity.Name == "IPassCardPay").Value);
            pdaModel.IPassCardDiscount = settings.FirstOrDefault(entity => entity.Name == "IPassCardDiscount").Value;
            pdaModel.OutCarRecognition = bool.Parse(settings.FirstOrDefault(entity => entity.Name == "OutCarRecognition").Value);
            pdaModel.PayUrl = settings.FirstOrDefault(entity => entity.Name == "OnlinePayUrl").Value;//支付路径配置
            #endregion

            pdaModel.CompanyName = GetCompanyInfo(loginToken);

            //签到操作
            EmployeeCheckIn(access_token);

            return pdaModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        private AbpUserLoginToken GetLoginToken(string access_token)
        {
            if (!string.IsNullOrWhiteSpace(access_token))
            {
                string sql = "select * from AbpUserLoginToken where Token = @Token";
                if (access_token.Length < 16)
                    sql = "select top 1 * from AbpUserLoginToken with(nolock) where EmployeeId = @Token order by id desc ";
                SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@Token", access_token)
            };

                DataTable model = SqlHelper.ExecuteDataTable(CommandType.Text, sql, param);
                if (model.Rows.Count > 0)
                {
                    return DataProcessHelper.GetEntityFromTable<AbpUserLoginToken>(model)[0];
                }
            }
            return new AbpUserLoginToken();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginToken"></param>
        /// <returns></returns>
        private List<PrintAdDto> GetPrintList(AbpUserLoginToken loginToken)
        {
            string sql = "select * from AbpPrintAds where BeginTime < '" + DateTime.Now + "' and EndTime > '" + DateTime.Now + "' and IsActive = 1 and TenantId = " + loginToken.TenantId;
            return DataProcessHelper.GetEntityFromTable<PrintAdDto>(SqlHelper.ExecuteDataTable(CommandType.Text, sql, null));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginToken"></param>
        /// <returns></returns>
        private List<WhiteListsDto> GetWhiteList(AbpUserLoginToken loginToken)
        {
            string sql = "select * from AbpWhiteList where CompanyId = " + loginToken.CompanyId;
            return DataProcessHelper.GetEntityFromTable<WhiteListsDto>(SqlHelper.ExecuteDataTable(CommandType.Text, sql, null));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginToken"></param>
        /// <returns></returns>
        private string GetCompanyInfo(AbpUserLoginToken loginToken)
        {
            string sql = "select CompanyName from AbpOperatorsCompany where Id = " + loginToken.CompanyId;
            return SqlHelper.ExecuteScalar(SqlHelper.connectionString, System.Data.CommandType.Text, sql, null).ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginToken"></param>
        /// <returns></returns>
        private List<TicketStyleDto> GetTicketList(AbpUserLoginToken loginToken)
        {
            string sql = "select * from AbpTicketStyle where CompanyId = " + loginToken.CompanyId;
            return DataProcessHelper.GetEntityFromTable<TicketStyleDto>(SqlHelper.ExecuteDataTable(CommandType.Text, sql, null));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginToken"></param>
        /// <returns></returns>
        private List<MonthlyCarDto> GetMonthlyList(AbpUserLoginToken loginToken)
        {
            List<MonthlyCarDto> monthlyList = new List<MonthlyCarDto>();
            string sql = "select  Id, VehicleOwner, Telphone, PlateNumber, Money, BeginTime, EndTime, TenantId, CompanyId,  CarType,  Version, MonthyType, @parkid as ParkIds from AbpMonthlyCars with(nolock)  where CompanyId =  " + loginToken.CompanyId + "  and BeginTime <= getdate() and EndTime >= getdate() and (charindex(','+@ParkId+ ',', ','+ ParkIds + ',') > 0 or ParkIds = '0') and IsDeleted = 0";
            foreach (var str in loginToken.ParkIds.Split(','))
            {
                if (str.Length > 0)
                    monthlyList.AddRange(DataProcessHelper.GetEntityFromTable<MonthlyCarDto>(SqlHelper.ExecuteDataTable(CommandType.Text, sql, new SqlParameter[] { new SqlParameter("@ParkId", str) })));
            }
            return monthlyList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginToken"></param>
        /// <returns></returns>
        private List<BerthDto> GetBerthList(AbpUserLoginToken loginToken)
        {
            string sql = "select * from AbpBerths with(nolock) where BerthsecId in (" + loginToken.BerthsecIds + ") and IsActive = 1 ";
            return DataProcessHelper.GetEntityFromTable<BerthDto>(SqlHelper.ExecuteDataTable(CommandType.Text, sql, null));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        private EmployeeDto GetEmployeeInfo(long employeeId)
        {
            string sql = "select * from AbpEmployees where Id = " + employeeId;
            return DataProcessHelper.GetEntityFromTable<EmployeeDto>(SqlHelper.ExecuteDataTable(CommandType.Text, sql, null))[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginToken"></param>
        /// <returns></returns>
        public List<BerthsecDto> GetBerthsecList(AbpUserLoginToken loginToken)
        {
            string sql = "select AbpBerthsecs.Id, AbpBerthsecs.BerthsecName, BeginNumeber, EndNumeber, CustomNumeber, CheckInStatus, CheckStatus, CheckOutStatus, CheckInTime, CheckInEmployeeId, CheckOutTime, CheckOutEmployeeId, CheckInDeviceCode, CheckOutDeviceCode, XPoint, YPoint, RegionId, ParkId, AbpBerthsecs.TenantId, AbpBerthsecs.IsActive, UseStatus, AbpBerthsecs.CompanyId,  RateId, A.RatePDA as FeeModel,  RateId1,B.RatePDA as FeeModel1, RateId2, C.RatePDA as FeeModel2, BerthCount, CONVERT(bit, 1) as PushStatus, Lat, Lng, AbpBerthsecs.IsDeleted, AbpBerthsecs.DeleterUserId, AbpBerthsecs.DeletionTime, AbpBerthsecs.LastModificationTime, AbpBerthsecs.LastModifierUserId,  AbpBerthsecs.CreationTime, AbpBerthsecs.CreatorUserId from AbpBerthsecs with(nolock) left join AbpRates as A on A.Id = RateId left join AbpRates as B on B.Id = RateId1 left join AbpRates as C on C.Id = RateId2 where AbpBerthsecs.Id in (0," + loginToken.BerthsecIds + ",0) and AbpBerthsecs.IsDeleted = 0";
            return DataProcessHelper.GetEntityFromTable<BerthsecDto>(SqlHelper.ExecuteDataTable(CommandType.Text, sql, null));
        }

        /// <summary>
        /// 车辆进场方法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ErrorInfo InsertCarInParking(CarinParkingDto input)
        {
            CheckLogintoken(input);

            //判断泊位号是否存在

            BerthDto berthmodel = null;
            var berths = DataProcessHelper.GetEntityFromTable<BerthDto>(SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, "select Id,SensorsInCarTime from AbpBerths where BerthsecId =  " + input.BerthsecId + " and BerthNumber = '" + input.BerthNumber + "'"));

            if (berths != null && berths.Count > 0)
            {
                berthmodel = berths[0];
            }
            else
            {
                return new ErrorInfo(23, "入场失败：泊位号不存在！");//("入场失败：泊位号不存在！", "23");
            }
            DataTable dtBerD = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $"select * from AbpBerthsecs with(nolock) where Id = {input.BerthsecId}").Tables[0];
            if (dtBerD.Rows.Count != 1)
            {
                return new ErrorInfo(23, "入场失败：泊位段不存在！");//("入场失败：泊位号不存在！", "23");
            }


            input.ParkName = Convert.ToString(dtBerD.Rows[0]["BerthsecName"]);

            int count = int.Parse(SqlHelper.ExecuteScalar(SqlHelper.connectionString, CommandType.Text, "select count(1) from AbpBusinessDetail with(nolock) where guid = '" + input.guid + "'").ToString());

            if (count > 0)
            {
                return new ErrorInfo(20, "入场失败：guid已经存在！");//("入场失败：guid已经存在！", "20");
            }

            if (input.StopType == "F4")//白名单
            {
                input.StopType = "7";
            }

            short PrepaidPayStatus;
            if (input.CardNo != "0")
            {
                PrepaidPayStatus = 4;// 卡号不等于0  支付类型为4属账号支付
            }
            else
            {
                PrepaidPayStatus = 1;//卡号等于0  支付类型为1属现金支付
            }

            if (berthmodel.SensorsInCarTime.HasValue)
                input.SensorsInCarTime = berthmodel.SensorsInCarTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
            if (input.SensorsInCarTime == "0")
                input.SensorsInCarTime = null;
            CarInUpdateBerhs(input, PrepaidPayStatus);
            //发送微信消息
            return new ErrorInfo();
        }

        public ErrorInfo InsertCarInParkingRemedy(CarinParkingDto input)
        {
            CheckLogintoken(input);
            BerthDto berthmodel = null;
            var berths = DataProcessHelper.GetEntityFromTable<BerthDto>(SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, "select * from AbpBerths where BerthsecId =  " + input.BerthsecId + " and BerthNumber = '" + input.BerthNumber + "'"));

            if (berths != null && berths.Count > 0)
            {
                berthmodel = berths[0];
            }
            else
            {
                return new ErrorInfo(23, "入场失败：泊位号不存在！");//("入场失败：泊位号不存在！", "23");
            }

            int count = int.Parse(SqlHelper.ExecuteScalar(SqlHelper.connectionString, CommandType.Text, "select count(1) from AbpBusinessDetail with(nolock) where guid = '" + input.guid + "'").ToString());

            if (count > 0)
            {
                return new ErrorInfo(20, "入场失败：guid已经存在！");//("入场失败：guid已经存在！", "20");
            }

            if (input.StopType == "F4")//白名单
            {
                input.StopType = "7";
            }

            short PrepaidPayStatus;
            if (input.CardNo != "0")
            {
                PrepaidPayStatus = 4;// 卡号不等于0  支付类型为4属账号支付
            }
            else
            {
                PrepaidPayStatus = 1;//卡号等于0  支付类型为1属现金支付
            }

            if (input.SensorsInCarTime == "0")
                input.SensorsInCarTime = null;
            CarInUpdateBerhsRemedy(input, PrepaidPayStatus);
            //发送微信消息
            return new ErrorInfo();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        private void CheckLogintoken(CarinParkingDto input)
        {
            string sql = "if not exists( select 1 from AbpUserLoginToken where Token = '" + input.access_token + "') begin   declare @EmployeeId bigint    declare @TenantId int   declare @CompanyId int   declare @RegionIds nvarchar(50)   declare @ParkIds nvarchar(50)   declare @DeviceCode nvarchar(50)   select @EmployeeId = CheckInEmployeeId, @TenantId = TenantId, @CompanyId = CompanyId, @RegionIds = RegionId, @ParkIds = ParkId, @DeviceCode = CheckInDeviceCode from AbpBerthsecs where id = " + input.BerthsecId + "   insert into AbpUserLoginToken(Token, EmployeeId, TenantId, CompanyId, RegionIds, ParkIds, BerthsecIds, DeviceCode, Version)   values('" + input.access_token + "', @EmployeeId, @TenantId, @CompanyId, @RegionIds+',', @ParkIds+',', " + input.BerthsecId + ", @DeviceCode, '88888') end";
            SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.Text, sql);
        }

        /// <summary>
        /// 新增订单数据/推送微信公众号和短信
        /// </summary>
        /// <param name="BerthNumber"></param>
        /// <param name="BerthsecId"></param>
        /// <param name="PlateNumber"></param>
        /// <param name="CarInTime"></param>
        /// <param name="guid"></param>
        /// <param name="CarType"></param>
        /// <param name="CardNo"></param>
        /// <param name="Prepaid"></param>
        private void CarInUpdateBerhs(CarinParkingDto input, short PrepaidPayStatus)
        {
            var loginToken = GetLoginToken(input.access_token);
            string sqlStr = string.Format(@"update AbpBerths set BerthStatus='1',RelateNumber='{0}',InCarTime='{1}',guid='{2}', CarType={3},OutCarTime=null,CardNo='{4}',Prepaid={5}, StopType = {8} where BerthNumber='{6}' and IsActive=1 and BerthsecId={7}",
                   input.PlateNumber, input.CarInTime, input.guid, input.CarType, input.CardNo, input.Prepaid, input.BerthNumber, input.BerthsecId, input.StopType);
            sqlStr += " insert into AbpBusinessDetail(BerthNumber, PlateNumber, CarType, Prepaid, CarInTime, InOperaId, InDeviceCode, guid, StopType, RegionId, ParkId, BerthsecId, Status, PrepaidCarNo, PrepaidPayStatus, Receivable, FactReceive, Arrearage, PaymentType, EscapePayStatus, IsEscapePay, PayStatus, IsPay, FeeType, TenantId, CompanyId, IsLock, IsDeleted, CreationTime, CreatorUserId, InBatchNo, SensorsInCarTime) values(@BerthNumber, @PlateNumber, @CarType, @Prepaid, @CarInTime, @InOperaId, @InDeviceCode, @guid, @StopType, @RegionId, @ParkId, @BerthsecId, @Status, @PrepaidCarNo, @PrepaidPayStatus, 0, 0, 0, 0, 0, 0, 0, 0, 0, @TenantId, @CompanyId, 0, 0, getdate(), @CreatorUserId, @InBatchNo, @SensorsInCarTime) ";

            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@BerthNumber", input.BerthNumber),
                new SqlParameter("@PlateNumber", input.PlateNumber),
                new SqlParameter("@CarType", input.CarType),
                new SqlParameter("@Prepaid", input.Prepaid),
                new SqlParameter("@InDeviceCode",loginToken.DeviceCode ),
                new SqlParameter("@CarInTime", input.CarInTime),
                new SqlParameter("@InOperaId",  loginToken.EmployeeId),
                 new SqlParameter("@CreatorUserId",  loginToken.EmployeeId),
                new SqlParameter("@guid", input.guid),
                new SqlParameter("@StopType", input.StopType),
                new SqlParameter("@RegionId", input.RegionId),
                new SqlParameter("@ParkId", input.ParkId),
                new SqlParameter("@BerthsecId",  input.BerthsecId),
                new SqlParameter("@Status",  1),
                new SqlParameter("@PrepaidCarNo", input.CardNo ),
                new SqlParameter("@PrepaidPayStatus", PrepaidPayStatus),
                new SqlParameter("@TenantId",  loginToken.TenantId),
                new SqlParameter("@CompanyId",  loginToken.CompanyId),
                new SqlParameter("@InBatchNo", input.InBatchNo),
                new SqlParameter("@SensorsInCarTime", input.SensorsInCarTime)
            };
            if (SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.Text, sqlStr, param) > 0)
            {
                //微信推送 首先推送地磁入场时间 如没有则推送 POS 机设备的入场时间    !string.IsNullOrEmpty(input.SensorsInCarTime) ? input.SensorsInCarTime : 
                _webChatAppService.SendCarInMsg(input.PlateNumber, input.CarInTime, input.BerthsecId, input.BerthNumber, loginToken,  input.ParkName);//推送微信信息
                SendSms(input.PlateNumber, loginToken.CompanyId, loginToken.TenantId, "BlackCarInModel", input.CarInTime, input.BerthNumber, new BerthsecAppService().GetBerthsecInfo(input.BerthsecId).BerthsecName);

            }
        }

        private void CarInUpdateBerhsRemedy(CarinParkingDto input, short PrepaidPayStatus)
        {
            var loginToken = GetLoginToken(input.access_token);

            string sqlStr = " insert into AbpBusinessDetail(BerthNumber, PlateNumber, CarType, Prepaid, CarInTime, InOperaId, InDeviceCode, guid, StopType, RegionId, ParkId, BerthsecId, Status, PrepaidCarNo, PrepaidPayStatus, Receivable, FactReceive, Arrearage, PaymentType, EscapePayStatus, IsEscapePay, PayStatus, IsPay, FeeType, TenantId, CompanyId, IsLock, IsDeleted, CreationTime, CreatorUserId, InBatchNo, SensorsInCarTime) values(@BerthNumber, @PlateNumber, @CarType, @Prepaid, @CarInTime, @InOperaId, @InDeviceCode, @guid, @StopType, @RegionId, @ParkId, @BerthsecId, @Status, @PrepaidCarNo, @PrepaidPayStatus, 0, 0, 0, 0, 0, 0, 0, 0, 0, @TenantId, @CompanyId, 0, 0, getdate(), @CreatorUserId, @InBatchNo, @SensorsInCarTime) ";

            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@BerthNumber", input.BerthNumber),
                new SqlParameter("@PlateNumber", input.PlateNumber),
                new SqlParameter("@CarType", input.CarType),
                new SqlParameter("@Prepaid", input.Prepaid),
                new SqlParameter("@InDeviceCode",loginToken.DeviceCode ),
                new SqlParameter("@CarInTime", input.CarInTime),
                new SqlParameter("@InOperaId",  loginToken.EmployeeId),
                 new SqlParameter("@CreatorUserId",  loginToken.EmployeeId),
                new SqlParameter("@guid", input.guid),
                new SqlParameter("@StopType", input.StopType),
                new SqlParameter("@RegionId", input.RegionId),
                new SqlParameter("@ParkId", input.ParkId),
                new SqlParameter("@BerthsecId",  input.BerthsecId),
                new SqlParameter("@Status",  1),
                new SqlParameter("@PrepaidCarNo", input.CardNo ),
                new SqlParameter("@PrepaidPayStatus", PrepaidPayStatus),
                new SqlParameter("@TenantId",  loginToken.TenantId),
                new SqlParameter("@CompanyId",  loginToken.CompanyId),
                new SqlParameter("@InBatchNo", input.InBatchNo),
                new SqlParameter("@SensorsInCarTime", input.SensorsInCarTime)
            };
            if (SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.Text, sqlStr, param) > 0)
            {

            }
        }


        /// <summary>
        /// 发送黑名单短信
        /// </summary>
        /// <param name="RelateNumber"></param>
        /// <param name="companyId"></param>
        /// <param name="tenantId"></param>
        /// <param name="modelType"></param>
        public void SendSms(string RelateNumber, int companyId, int tenantId, string modelType, string CarInTime, string BerthNumber, string berthsecName)
        {
            string sql = "select RelateNumber,CarOwner,IdNumber from AbpBlackList where CompanyId = @CompanyId and TenantId = @TenantId and RelateNumber = @RelateNumber and IsDeleted = 0 and IsActive = 1";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@RelateNumber", RelateNumber),
                new SqlParameter("@CompanyId", companyId),
                new SqlParameter("@TenantId", tenantId)
            };
            DataTable dtLast = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $"select AppName from AbpWeixinConfig  where TenantId = '{tenantId}' ").Tables[0];
            var blacklist = DataProcessHelper.GetEntityFromTable<BlackListDto>(SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql, param).Tables[0]);
            if (blacklist.Count > 0)//发送短信
            {
                var smsModel = GetSmsModel(modelType, tenantId);//BlackCarInModel
                var model = new SmsAccountDto() { Msg ="【"+ Convert.ToString(dtLast.Rows[0]["AppName"]) + "】"+ string.Format(smsModel.SmsContext, blacklist[0].RelateNumber, blacklist[0].CarOwner, CarInTime, berthsecName, BerthNumber), Destnumbers = blacklist[0].IdNumber, TenantId = tenantId };
                //new SmsUtils().SendSmsNoTenant(model);
                new SmsUtils().SendSmsSubmail(model);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="TenantId"></param>
        /// <returns></returns>
        private SmsModelDto GetSmsModel(string modelType, int TenantId)
        {
            string sql = "select * from AbpSmsModel where ModelType = '" + modelType + "' and TenantId = " + TenantId;
            var model = DataProcessHelper.GetEntityFromTable<SmsModelDto>(SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0]);
            return model[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ErrorInfo InsertCarOutParking(CaroutParkingDto input)
        {
            if (input.PayStatus != "4")
                input.CardNo = "0";
            var list = DataProcessHelper.GetEntityFromTable<BusinessDetailDto>(SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text,
                "select Id, guid, Status, BerthNumber, PlateNumber, BerthsecId, Prepaid from AbpBusinessDetail where isdeleted = 0 and guid = '" + input.guid + "'").Tables[0]);
            if (list.Count == 0)
            {
                return new ErrorInfo(22, "出场失败：guid不存在！");
            }
            var entity = list[0];
            if (entity.Status == 2 || entity.Status == 4 || (entity.Status == 3 && !Convert.ToBoolean(input.IsPay)))//服务器数据状态
                return new ErrorInfo(201, "出场失败：该数据已出场！");
            BerthDto berthmodel = DataProcessHelper.GetEntityFromTable<BerthDto>(SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select * from AbpBerths with(nolock) where BerthsecId = " + input.BerthsecID + " and BerthNumber = '" + entity.BerthNumber + "'").Tables[0])[0];

            var loginToken = GetLoginToken(input.access_token);
            BusinessDetailDto detail = new BusinessDetailDto();
            detail.TenantId = loginToken.TenantId;
            detail.CompanyId = loginToken.CompanyId;
            detail.OutOperaId = loginToken.EmployeeId;
            detail.OutDeviceCode = loginToken.DeviceCode;
            detail.BerthsecId = entity.BerthsecId;
            detail.BerthNumber = entity.BerthNumber;
            detail.PlateNumber = entity.PlateNumber;
            if (!Convert.ToBoolean(input.IsPay))//未支付
            {
                detail.Status = 3;
                input.Arrearage = input.Money - entity.Prepaid;//总应收减去预缴金额
                detail.EscapeTime = Convert.ToDateTime(input.CarOutTime);
            }
            else//出场支付
            {
                detail.Status = 2;
                input.Arrearage = 0;
            }
            //是否账号支付
            if (input.CardNo != "0" && Convert.ToBoolean(input.IsPay))
                AccountPay(input.CardNo, detail.PlateNumber, detail.Prepaid, input.Money, loginToken);
            if (!entity.SensorsOutCarTime.HasValue && entity.SensorsInCarTime.HasValue)
            {
                input.SensorsOutCarTime = GetSensorOutTime(berthmodel.SensorNumber, entity.SensorsInCarTime.Value);
            }
            else if (entity.SensorsOutCarTime.HasValue)
            {
                input.SensorsOutCarTime = entity.SensorsOutCarTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                input.SensorsOutCarTime = null;
            }


            DataTable dtBerD = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, $"select * from AbpBerthsecs with(nolock) where Id = {entity.BerthsecId}").Tables[0];
            if (dtBerD.Rows.Count != 1)
            {
                return new ErrorInfo(201, "泊位段不存在，出场失败！");
            }
            input.ParkName = Convert.ToString(dtBerD.Rows[0]["BerthsecName"]);

            CarOutUpdateBerths(detail, input, entity.Id);
            return new ErrorInfo();
        }

        /// <summary>
        /// 后端补救接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ErrorInfo InsertCarOutParkingRemedy(CaroutParkingDto input)
        {
            if (input.PayStatus != "4")
                input.CardNo = "0";
            var list = DataProcessHelper.GetEntityFromTable<BusinessDetailDto>(SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text,
                "select Id, guid, Status, BerthNumber, PlateNumber, BerthsecId, Prepaid from AbpBusinessDetail where isdeleted = 0 and guid = '" + input.guid + "'").Tables[0]);
            if (list.Count == 0)
            {
                return new ErrorInfo(22, "出场失败：guid不存在！");
            }
            var entity = list[0];
            if (entity.Status == 2 || entity.Status == 4 || (entity.Status == 3 && !Convert.ToBoolean(input.IsPay)))//服务器数据状态
                return new ErrorInfo(201, "出场失败：该数据已出场！");
            BerthDto berthmodel = DataProcessHelper.GetEntityFromTable<BerthDto>(SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select * from AbpBerths with(nolock) where BerthsecId = " + input.BerthsecID + " and BerthNumber = '" + entity.BerthNumber + "'").Tables[0])[0];

            var loginToken = GetLoginToken(input.access_token);
            BusinessDetailDto detail = new BusinessDetailDto();
            detail.TenantId = loginToken.TenantId;
            detail.CompanyId = loginToken.CompanyId;
            detail.OutOperaId = loginToken.EmployeeId;
            detail.OutDeviceCode = loginToken.DeviceCode;
            detail.BerthsecId = entity.BerthsecId;
            detail.BerthNumber = entity.BerthNumber;
            detail.PlateNumber = entity.PlateNumber;
            if (!Convert.ToBoolean(input.IsPay))//未支付
            {
                detail.Status = 3;
                input.Arrearage = input.Money - entity.Prepaid;//总应收减去预缴金额
                detail.EscapeTime = Convert.ToDateTime(input.CarOutTime);
            }
            else//出场支付
            {
                detail.Status = 2;
                input.Arrearage = 0;
            }
            //是否账号支付
            if (input.CardNo != "0" && Convert.ToBoolean(input.IsPay))
                AccountPay(input.CardNo, detail.PlateNumber, detail.Prepaid, input.Money, loginToken);
            if (!entity.SensorsOutCarTime.HasValue && entity.SensorsInCarTime.HasValue)
            {
                input.SensorsOutCarTime = GetSensorOutTime(berthmodel.SensorNumber, entity.SensorsInCarTime.Value);
            }
            else if (entity.SensorsOutCarTime.HasValue)
            {
                input.SensorsOutCarTime = entity.SensorsOutCarTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                input.SensorsOutCarTime = null;
            }
            CarOutUpdateBerthsRemedy(detail, input, entity.Id);
            return new ErrorInfo();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="SensorNumber"></param>
        /// <param name="SensorInTime"></param>
        /// <returns></returns>
        private string GetSensorOutTime(string SensorNumber, DateTime SensorInTime)
        {
            string sql = "select CarOutTime from AbpSensorBusinessDetail with(nolock) where CarInTime ='" + SensorInTime + "' and SensorNumber = '" + SensorNumber + "'";
            object obj = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql);
            if (obj == null)
                return null;
            else
            {
                return obj.ToString();
            }
        }

        /// <summary>
        /// 账号扣款
        /// </summary>
        /// <param name="CardNo"></param>
        /// <param name="companyId"></param>
        /// <param name="input"></param>
        private void AccountPay(string CardNo, string PlateNumber, decimal Prepaid, decimal Money, AbpUserLoginToken loginToken)
        {
            string sql = "select * from ExtOtherAccount where CompanyId = @CompanyId and CardNo = @CardNo and IsActive = 1";
            SqlParameter[] param = new SqlParameter[] {
                        new SqlParameter("@CompanyId", loginToken.CompanyId),new SqlParameter("@CardNo", CardNo)
                    };
            var list = DataProcessHelper.GetEntityFromTable<OtherAccountModel>(SqlHelper.ExecuteDataTable(CommandType.Text, sql, param));
            if (list.Count > 0)
            {
                if (Money >= Prepaid)//添加消费记录
                {
                    param = new SqlParameter[] {
                        new SqlParameter("@OtherAccountId", list[0].Id),new SqlParameter("@Money", Money - Prepaid),
                        new SqlParameter("@CardNo", CardNo), new SqlParameter("@EmployeeId", loginToken.EmployeeId),
                        new SqlParameter("@PlateNumber", PlateNumber),new SqlParameter("@TenantId", loginToken.TenantId),
                        new SqlParameter("@CompanyId", loginToken.CompanyId), new SqlParameter("@BeginMoney", list[0].Wallet),
                        new SqlParameter("@EndMoney", list[0].Wallet - (Money - Prepaid))
                    };
                    SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.Text, "insert into AbpDeductionRecords(OtherAccountId, OperType, Money, PayStatus, InTime, Remark, CardNo, EmployeeId, PlateNumber, TenantId, CompanyId, BeginMoney, EndMoney, UserId) values(@OtherAccountId, 2, @Money, 1, getdate(), '消费', @CardNo, @EmployeeId, @PlateNumber, @TenantId, @CompanyId, @BeginMoney, @EndMoney, @EmployeeId) " +
                        " update ExtOtherAccount set  Wallet = @EndMoney where Id = @OtherAccountId "
                        , param);
                }
                else if (Money < Prepaid)//消费金额小于预付金额
                {
                    param = new SqlParameter[] {
                        new SqlParameter("@OtherAccountId", list[0].Id),new SqlParameter("@Money",  Prepaid - Money),
                        new SqlParameter("@CardNo", CardNo), new SqlParameter("@EmployeeId", loginToken.EmployeeId),
                        new SqlParameter("@PlateNumber", PlateNumber),new SqlParameter("@TenantId", loginToken.TenantId),
                        new SqlParameter("@CompanyId", loginToken.CompanyId), new SqlParameter("@BeginMoney", list[0].Wallet),
                        new SqlParameter("@EndMoney", list[0].Wallet - Money + Prepaid)
                    };
                    SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.Text, "insert into AbpDeductionRecords(OtherAccountId, OperType, Money, PayStatus, InTime, Remark, CardNo, EmployeeId, PlateNumber, TenantId, CompanyId, BeginMoney, EndMoney, UserId) values(@OtherAccountId, 4, @Money, 1, getdate(), '返还', @CardNo, @EmployeeId, @PlateNumber, @TenantId, @CompanyId, @BeginMoney, @EndMoney, @EmployeeId) " +
                        " update ExtOtherAccount set  Wallet = @EndMoney where Id = @OtherAccountId "
                        , param);
                }
            }
        }

        /// <summary>
        /// 正常收费出场方法
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="input"></param>
        /// <param name="businessId"></param>
        private void CarOutUpdateBerths(BusinessDetailDto detail, CaroutParkingDto input, long businessId)
        {
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@Status", detail.Status), new SqlParameter("@OutOperaId", detail.OutOperaId),
                new SqlParameter("@OutDeviceCode", detail.OutDeviceCode), new SqlParameter("@Money", input.Money),
                new SqlParameter("@OutBatchNo", input.OutBatchNo), new SqlParameter("@CarOutTime", input.CarOutTime),
                new SqlParameter("@Receivable", input.Receivable),  new SqlParameter("@FactReceive", input.FactReceive),
                new SqlParameter("@CarPayTime", input.CarOutTime), new SqlParameter("@PayStatus", input.PayStatus),
                new SqlParameter("@IsPay", input.IsPay), new SqlParameter("@FeeType", input.FeeType),
                new SqlParameter("@StopTime", input.StopTime.Split('.')[0]), new SqlParameter("@Arrearage", input.Money - input.FactReceive),
                new SqlParameter("@SensorsOutCarTime", input.SensorsOutCarTime)
            };
            string sqlescape = "";
            if (detail.Status == 3)
                sqlescape = ", EscapeTime = @CarOutTime, ChargeDeviceCode = @OutDeviceCode ";
            string sql = "update AbpBusinessDetail set Status = @Status, OutOperaId = @OutOperaId , ChargeOperaId = @OutOperaId, OutDeviceCode = @OutDeviceCode, Money = @Money, OutBatchNo = @OutBatchNo, CarOutTime = @CarOutTime, Receivable = @Receivable,  FactReceive = @FactReceive, Arrearage = @Arrearage, CarPayTime = @CarPayTime, PayStatus = @PayStatus, IsPay = @IsPay, FeeType = @FeeType, SensorsOutCarTime = @SensorsOutCarTime, StopTime = @StopTime " + sqlescape + " where id = " + businessId;

            sql += " update abpberths set BerthStatus = '2', RelateNumber = '" +
                    detail.PlateNumber + "', OutCarTime = @CarOutTime, SensorGuid = '00000000-0000-0000-0000-000000000000' where BerthNumber = '" + detail.BerthNumber + "' and IsActive = 1 and BerthsecId = " + detail.BerthsecId;

            if (SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.Text, sql, param) > 0)
            {
                string SensorsInCarTime = null;
                //查询地磁入场时间
                //var sensorList = DataProcessHelper.GetEntityFromTable<EscapeDetailsDto>(SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text,
                //"select CarInTime from AbpSensorBusinessDetail with(nolock) where guid = '" + input.guid + "'").Tables[0]);

                ////首先推送地磁入场时间 如没有则推送 POS 机设备的入场时间
                //if (sensorList.Count > 0)
                //{
                //    SensorsInCarTime = sensorList[0].CarInTimeString;
                //}
                //else
                //{
                //查询POS机设备入场时间
                var pdaList = DataProcessHelper.GetEntityFromTable<EscapeDetailsDto>(SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text,
                    "select CarInTime from AbpBusinessDetail with(nolock) where guid = '" + input.guid + "'").Tables[0]);
                SensorsInCarTime = pdaList[0].CarInTimeString;
                //}

                //POS机设备出场时间 - 地磁/POS 机入场时间
                var StopTime = (Convert.ToDateTime(input.CarOutTime) - Convert.ToDateTime(SensorsInCarTime)).TotalMinutes;
                _webChatAppService.SendCarOutMsg(detail.TenantId, detail.BerthsecId, detail.PlateNumber, detail.BerthNumber, StopTime, input.Money, input.FactReceive, input.PayStatus, 
                    !string.IsNullOrWhiteSpace(input.SensorsOutCarTime)&& input.SensorsOutCarTime !="1900-01-01 00:00:00" ? input.SensorsOutCarTime : input.CarOutTime,input.ParkName,
                    SensorsInCarTime);

                SendSms(detail.PlateNumber, detail.CompanyId, detail.TenantId, "BlackCarOutModel", input.CarOutTime, detail.BerthNumber, new BerthsecAppService().GetBerthsecInfo(detail.BerthsecId).BerthsecName);
            }
        }

        private void CarOutUpdateBerthsRemedy(BusinessDetailDto detail, CaroutParkingDto input, long businessId)
        {
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@Status", detail.Status), new SqlParameter("@OutOperaId", detail.OutOperaId),
                new SqlParameter("@OutDeviceCode", detail.OutDeviceCode), new SqlParameter("@Money", input.Money),
                new SqlParameter("@OutBatchNo", input.OutBatchNo), new SqlParameter("@CarOutTime", input.CarOutTime),
                new SqlParameter("@Receivable", input.Receivable),  new SqlParameter("@FactReceive", input.FactReceive),
                new SqlParameter("@CarPayTime", input.CarOutTime), new SqlParameter("@PayStatus", input.PayStatus),
                new SqlParameter("@IsPay", input.IsPay), new SqlParameter("@FeeType", input.FeeType),
                new SqlParameter("@StopTime", input.StopTime.Split('.')[0]), new SqlParameter("@Arrearage", input.Money - input.FactReceive),
                new SqlParameter("@SensorsOutCarTime", input.SensorsOutCarTime)
            };
            string sqlescape = "";
            if (detail.Status == 3)
                sqlescape = ", EscapeTime = @CarOutTime, ChargeDeviceCode = @OutDeviceCode ";
            string sql = "update AbpBusinessDetail set Status = @Status, OutOperaId = @OutOperaId , ChargeOperaId = @OutOperaId, OutDeviceCode = @OutDeviceCode, Money = @Money, OutBatchNo = @OutBatchNo, CarOutTime = @CarOutTime, Receivable = @Receivable,  FactReceive = @FactReceive, Arrearage = @Arrearage, CarPayTime = @CarPayTime, PayStatus = @PayStatus, IsPay = @IsPay, FeeType = @FeeType, SensorsOutCarTime = @SensorsOutCarTime, StopTime = @StopTime " + sqlescape + " where id = " + businessId;
            if (SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.Text, sql, param) > 0)
            {

            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="input"></param>
        /// <param name="businessId"></param>
        private void CarOutUpdateBerthByEscape(BusinessDetailDto detail, CaroutParkingDto input, long businessId)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public OtherAccountModel SearchOtherAccount(QueryCardInput input)
        {
            string sql = "select CardNo, TelePhone, PlateNumber, Wallet, IsEnabled from ExtOtherAccount left join ExtOtherPlateNumber on ExtOtherAccount.Id = ExtOtherPlateNumber.AssignedOtherAccountId where (ExtOtherAccount.CardNo = '" + input.cardNo + "' or TelePhone = '" + input.phone + "' or (ExtOtherPlateNumber.PlateNumber = '" + input.plateNumber + "' and ExtOtherPlateNumber.IsDeleted = 0)) and ExtOtherAccount.IsActive = 1 and ExtOtherAccount.IsDeleted = 0 and ExtOtherAccount.IsEnabled = 1 and TenantId = " + GetLoginToken(input.access_token).TenantId;

            var list = DataProcessHelper.GetEntityFromTable<OtherAccountModel>(SqlHelper.ExecuteDataTable(CommandType.Text, sql, null));
            if (list.Count > 0)
            {
                var model = list[0];
                model.msg = "success";
                return model;
            }
            else
                return new OtherAccountModel() { msg = "该卡不存在", CardNo = "", PlateNumber = "", TelePhone = "" };
        }

        /// <summary>
        /// 获取欠费车辆信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public GetEscapeDetailList GetEscapeDetailList(EscapeDetailInput input)
        {
            if (input.plateNumber.Contains("无") || string.IsNullOrWhiteSpace(input.access_token))
            {
                return new GetEscapeDetailList()
                {
                    Items = new List<EscapeDetailsDto>()
                };
            }
            var loginToken = GetLoginToken(input.access_token);
            string sql = "select top 1000 berthsec.BerthsecName , business.CarInTime, business.CarOutTime,sens.CarInTime SensorInTime,sens.CarOutTime SensorOutTime, business.guid, Arrearage, business.Id " +
                     " from AbpBusinessDetail as business with(nolock)" +
                     " left join AbpBerthsecs as berthsec with(nolock) on business.BerthsecId = berthsec.Id" +
                     " left join AbpSensorBusinessDetail as sens with(nolock) on business.guid = sens.guid" +
                     " where   business.PlateNumber = @PlateNumber and business.Status = 3  and business.IsDeleted = 0 ";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@PlateNumber", input.plateNumber)
            };

            //跨商户追缴默认，需要分公司追缴
            if (bool.Parse(SettingStoreAppService.GetSettingOrNull(loginToken.TenantId, null, "TheRecovered").Value))
            {
                sql += " and business.TenantId in (select TenantId from AbpSettings with(nolock) where Name = 'TheRecovered' and Value = 'True') ";
            }
            else if (bool.Parse(SettingStoreAppService.GetSettingOrNull(loginToken.TenantId, null, "TheRecoveredCompany").Value))//分公司追缴
            {
                sql += " and business.TenantId = " + loginToken.TenantId;
            }
            else//默认追缴本分公司数据
            {
                sql += " and business.TenantId = " + loginToken.TenantId + "  and business.CompanyId =  " + loginToken.CompanyId;
            }

            sql += " order by business.CarInTime ASC";
            return new GetEscapeDetailList()
            {
                Items = DataProcessHelper.GetEntityFromTable<EscapeDetailsDto>(SqlHelper.ExecuteDataTable(CommandType.Text, sql, param))
            };

        }

        /// <summary>
        /// 补缴费用
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ErrorInfo UpdateFeeBack(FeeBackDto input)
        {
            var list = DataProcessHelper.GetEntityFromTable<BusinessDetailDto>(SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select * from AbpBusinessDetail with(nolock) where IsDeleted = 0 and Id = " + input.id).Tables[0]);
            if (list.Count > 0)
            {
                var entity = list[0];
                if (entity.Status != 3 && entity.Status != 5)
                {
                    //return new ErrorInfo();
                    return new ErrorInfo(24, "补缴失败：该车没有欠费！");
                }
                var loginToken = GetLoginToken(input.access_token);
                entity.Repayment = entity.Arrearage;//补缴金额
                                                    //entity.Arrearage = entity.Arrearage - Repayment;  //欠费金额等于欠费金额减去补缴金额
                entity.CarRepaymentTime = input.CarRepaymentTime;//补缴时间
                //逃逸追缴支付类型   
                // case 1:"现金";
                // case 2:"刷卡支付";
                // case 3:"微信支付";
                // case 4:"账号支付";
                // case 5:"未付";
                // case 6:"支付宝支付";
                // case 7:"其他";
                // default:"未知";     
                entity.EscapePayStatus = input.EscapePayStatus;
                entity.IsEscapePay = input.IsEscapePay;//逃逸是否支付
                entity.EscapeOperaId = loginToken.EmployeeId;//逃逸追缴收费员ID
                entity.EscapeDeviceCode = loginToken.DeviceCode;//逃逸追缴设备
                entity.EscapeTenantId = loginToken.TenantId;//追缴商户ID
                entity.EscapeCompanyId = loginToken.CompanyId;//追缴分公司
                entity.PaymentType = input.PaymentType;// 追缴类型    1.前端追缴   2.后台追缴   3.微信追缴
                entity.Status = 4;//  2.逃逸已收费
                entity.EscapeCardNo = input.CardNo;
                entity.PaymentBatchNo = input.PaymentBatchNo;

                if (input.CardNo != "0")
                    AccountPay(input.CardNo, entity.PlateNumber, 0, entity.Repayment.Value, loginToken);
                UpdateBackPay(entity);
                return new ErrorInfo();
            }
            return new ErrorInfo(25, "补缴失败：未找到该车记录！");
        }

        /// <summary>
        /// 数据批量追缴
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ErrorInfo UpdateAllFeeBack(FeeBackDto input)
        {
            string[] strs = input.ids.Split(',');
            foreach (var str in strs)
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    input.id = long.Parse(str);
                    UpdateFeeBack(input);
                }
            }
            return new ErrorInfo();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        private void UpdateBackPay(BusinessDetailDto dto)
        {
            string sql = "update AbpBusinessDetail set Repayment = @Repayment,  CarRepaymentTime = @CarRepaymentTime, EscapePayStatus = @EscapePayStatus,  IsEscapePay = @IsEscapePay, " +
                "EscapeOperaId = @EscapeOperaId,  EscapeDeviceCode = @EscapeDeviceCode, EscapeTenantId = @EscapeTenantId, PaymentType = @PaymentType, Status = @Status,  " +
                "EscapeCardNo = @EscapeCardNo, PaymentBatchNo = @PaymentBatchNo where id = " + dto.Id;
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@Repayment",  dto.Repayment),
                new SqlParameter("@CarRepaymentTime",  dto.CarRepaymentTime),
                new SqlParameter("@EscapePayStatus",dto.EscapePayStatus),
                new SqlParameter("@IsEscapePay", dto.IsEscapePay),
                new SqlParameter("@EscapeOperaId", dto.EscapeOperaId),
                new SqlParameter("@EscapeDeviceCode", dto.EscapeDeviceCode),
                new SqlParameter("@EscapeTenantId", dto.EscapeTenantId),
                new SqlParameter("@PaymentType", dto.PaymentType),
                new SqlParameter("@Status", dto.Status),
                new SqlParameter("@EscapeCardNo", dto.EscapeCardNo),
                new SqlParameter("@PaymentBatchNo", dto.PaymentBatchNo)
            };
            SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.Text, sql, param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public int UpdateRemoteGuidStatus(string guid, string access_token)
        {
            var loginToken = GetLoginToken(access_token);
            string sql = "update AbpRemoteGuids set IsActive = 1, DeviceCode = @DeviceCode, EmployeeId =  @EmployeeId,  UpdateTime = getdate()  where BusinessDetailGuid = @BusinessDetailGuid";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@BusinessDetailGuid",  guid),
                new SqlParameter("@DeviceCode", loginToken.DeviceCode),
                new SqlParameter("@EmployeeId", loginToken.EmployeeId)
            };
            return SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.Text, sql, param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="access_token"></param>
        public List<RemoteGuidDto> GetRemoteGuids(string access_token)
        {
            var loginToken = GetLoginToken(access_token);
            if (!string.IsNullOrWhiteSpace(loginToken.BerthsecIds))
            {
                string sql = "select * from AbpRemoteGuids where BerthsecId in (" + loginToken.BerthsecIds + ") and IsActive = 0";

                return DataProcessHelper.GetEntityFromTable<RemoteGuidDto>(SqlHelper.ExecuteDataTable(CommandType.Text, sql, null));
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 同步泊位状态信息
        /// </summary>
        /// <param name="SyncTime"></param>
        /// <param name="Berthesclist"></param>
        /// <returns></returns>
        public List<BerthSensorDto> GetBerthsSyn(string SyncTime, string Berthesclist)
        {
            if (string.IsNullOrWhiteSpace(Berthesclist))
            {
                return new List<BerthSensorDto>();
            }

            var berthsecIds = Berthesclist.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var queryPushBerthsec = "select Id from AbpBerthsecs where PushStatus = 1 and Id in (" + string.Join(",", berthsecIds) + ")";
            var berthsecRows = SqlHelper.ExecuteDataTable(CommandType.Text, queryPushBerthsec);

            if (berthsecRows.Rows.Count == 0)
            {
                return new List<BerthSensorDto>();
            }

            var berthsecPushIds = new List<int>();
            foreach (DataRow dr in berthsecRows.Rows)
            {
                berthsecPushIds.Add((int)dr[0]);
            }



            string queryBerthsec = "select * from AbpBerths where BerthsecId in (" + string.Join(",", berthsecPushIds) + ")";
            if (SyncTime.Length > 10)
            {
                DateTime temp = DateTime.ParseExact(SyncTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture).AddDays(-1);
                queryBerthsec += " and ((SensorBeatTime  > '" + temp.ToString("yyyy-MM-dd HH:mm:ss") + "' and SensorNumber is not null) or IsSourceVideo = 1)";
            }
            return DataProcessHelper.GetEntityFromTable<BerthSensorDto>(SqlHelper.ExecuteDataTable(CommandType.Text, queryBerthsec, null));



            //string tempberthsec = "";
            ////非空验证
            //if (!string.IsNullOrEmpty(Berthesclist))
            //{
            //    foreach (var str in Berthesclist.Split(','))
            //    {
            //        if (!string.IsNullOrWhiteSpace(str))
            //        {
            //            string tempsql = "select Id from AbpBerthsecs where PushStatus = 1 and Id = " + str;
            //            object obj = SqlHelper.ExecuteScalar(SqlHelper.connectionString, CommandType.Text, tempsql);
            //            if (obj != null)
            //            {
            //                tempberthsec = tempberthsec + ",";
            //            }
            //        }
            //    }
            //}
            //if(tempberthsec.Length == 0)
            //{
            //    return new List<BerthSensorDto>();
            //}

            //string sql = "select * from AbpBerths where BerthsecId in (" + Berthesclist + " 0)";
            //if (SyncTime.Length > 10)
            //{
            //    DateTime temp = DateTime.ParseExact(SyncTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture).AddDays(-1);
            //    sql += " and (SensorBeatTime  > '" + temp + "' and SensorNumber is not null)";
            //}
            //return DataProcessHelper.GetEntityFromTable<BerthSensorDto>(SqlHelper.ExecuteDataTable(CommandType.Text, sql, null));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public string CheckEquipmentStatus(string access_token)
        {
            var loginToken = GetLoginToken(access_token);
            object obj = SqlHelper.ExecuteScalar(SqlHelper.connectionString, CommandType.Text, "select 1 from AbpEquipments where PDA = '" + loginToken.DeviceCode + "' and IsActive = 1 and IsDeleted = 0 and TenantId = " + loginToken.TenantId);
            if (obj != null)
                return "1";
            return "0";
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="businessid"></param>
        /// <param name="pic"></param>
        /// <param name="fileType">文件格式</param>
        /// <returns></returns>
        public int PhotoUpLoadToAndroid(string guid, int businessid, int pictype, byte[] pic, string fileType, string createtime, string access_token)
        {
            //先保存图片
            var savePath = SaveUploadImage(pic, fileType, guid, createtime);
            var loginToken = GetLoginToken(access_token);
            string sql = "INSERT INTO AbpBusinessDetailPicture(BusinessDetailId, BusinessDetailGuid, CreationTime, CreatorUserId, PicType, CreateTime) values(@BusinessDetailId, @BusinessDetailGuid, getdate(), @CreatorUserId, @PicType, @CreateTime); select @@identity";
            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@BusinessDetailId", businessid),
                new SqlParameter("@BusinessDetailGuid", guid),
                new SqlParameter("@CreatorUserId", loginToken.EmployeeId),
                new SqlParameter("@PicType", pictype),
                new SqlParameter("@CreateTime", createtime)
            };

            var key = SqlHelper.ExecuteScalar(SqlHelper.connectionString, CommandType.Text, sql, param).ToString();
            InsertPicture(new PicMongoDto()
            {
                BusinessDetailGuid = guid,
                BusinessDetailId = int.Parse(key),
                CreationTime = DateTime.Now,
                CreatorUserId = loginToken.
                EmployeeId,
                Image = pic,
                PicType = pictype,
                FileSavePath = savePath
            });
            return 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="businessid"></param>
        /// <returns></returns>
        public int GetBusinessDetailPicture(string guid, int businessid)
        {
            string sql = "select 1 from AbpBusinessDetailPicture where BusinessDetailGuid = @BusinessDetailGuid and BusinessDetailId = @businessid";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@BusinessDetailGuid", guid),
                new SqlParameter("@businessid", businessid)
            };

            object obj = SqlHelper.ExecuteScalar(SqlHelper.connectionString, CommandType.Text, sql, param);
            if (obj != null)
                return int.Parse(obj.ToString());
            return 0;
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="fileType">文件格式</param>
        private void InsertPicture(PicMongoDto dto)
        {
            string sql = "insert into AbpPictureStores(BusinessDetailGuid, BusinessDetailId, Image,FileSavePath, CreationTime, CreatorUserId, PicType)" +
                " values(@BusinessDetailGuid, @BusinessDetailId, @Image , @FileSavePath , @CreationTime, @CreatorUserId, @PicType)";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@BusinessDetailGuid", dto.BusinessDetailGuid),  new SqlParameter("@BusinessDetailId", dto.BusinessDetailId),
                new SqlParameter("@Image", new byte[0]),  new SqlParameter("@CreationTime", dto.CreationTime),
                new SqlParameter("@CreatorUserId", dto.CreatorUserId),  new SqlParameter("@PicType", dto.PicType),
                new SqlParameter("@FileSavePath", dto.FileSavePath)
            };
            SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.Text, sql, param);
        }

        /// <summary>
        /// 保存图片到本地
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private string SaveUploadImage(byte[] bytes, string fileType, string guid,string createtime)
        {
            try
            {
                DateTime dt = Convert.ToDateTime(createtime);
                string sql = "select top(1) PlateNumber from AbpBusinessDetail with(nolock) where guid = @guid ";
                SqlParameter[] param = new SqlParameter[]
                {
                    new SqlParameter("@guid", guid),
                };
                var PlateNumber = SqlHelper.ExecuteScalar(SqlHelper.connectionString, CommandType.Text, sql, param)?.ToString() ?? Guid.NewGuid().ToString();
                //格式 图片名称
                string imageKey = $"{guid}-{PlateNumber}";

                //获取上传根路径
                string UrlPath = ConfigurationManager.AppSettings["UrlUpload"];
                //建立存储的目录
                string ImageFileUrl = UrlPath + $"\\{dt.Year}\\{dt.Month:d2}\\{dt.Day:d2}\\";
                //判断目录是否存在
                if (!Directory.Exists(ImageFileUrl))
                {
                    //如果不存在，创建
                    Directory.CreateDirectory(ImageFileUrl);
                }
                string NewFilethumbName = $"{imageKey}.{fileType}";  //图片命名
                if (bytes != null)
                {
                    FileStream fileWriter = new FileStream(ImageFileUrl + NewFilethumbName, FileMode.Create);
                    fileWriter.Write(bytes, 0, bytes.Length);
                    fileWriter.Dispose();
                }
                return UrlPath + $"\\{dt.Year}\\{dt.Month:d2}\\{dt.Day:d2}\\" + NewFilethumbName;
            }
            catch (ArgumentException e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 上传设备定位
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public int UpdateGps(string x, string y, string access_token)
        {
            var loginToken = GetLoginToken(access_token);
            string sql = "insert into AbpEquipmentGps(PDA, X, Y, CreationTime, CreatorUserId) values(@PDA, @X, @Y, getdate(), @CreatorUserId)";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@PDA", loginToken.DeviceCode),
                new SqlParameter("@X", x),
                new SqlParameter("@Y", y),
                new SqlParameter("@CreatorUserId", loginToken.EmployeeId)
            };
            return SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.Text, sql, param);
        }

        /// <summary>
        /// 收费员签退
        /// </summary>
        /// <param name="berthsecid"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public int EmployeeOutLineLogout(string berthsecid, string access_token)
        {
            var loginToken = GetLoginToken(access_token);
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@berthsecId", loginToken.BerthsecIds),
                new SqlParameter("@DeviceCode", loginToken.DeviceCode),
                new SqlParameter("@employeeID", loginToken.EmployeeId),
                new SqlParameter("@checkInOrOutTime", DateTime.Now)
            };
            return SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.StoredProcedure, "Pro_Employee_Checkout", param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="ExceptionMsg"></param>
        /// <returns></returns>
        public int UploadAndroidExceptionLog(string guid, string ExceptionMsg)
        {
            string sql = "insert into AbpAndroidLogs(guid, ExceptionMsg, CreationTime) values(@guid, @ExceptionMsg, getdate())";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@guid", guid),
                new SqlParameter("@ExceptionMsg", ExceptionMsg)
            };
            return SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.Text, sql, param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public string GetSensorBusinessdetail(string guid)
        {
            string sql = "select CarOutTime from AbpSensorBusinessDetail with(nolock) right join AbpBerthsecs on berthsecid = AbpBerthsecs.Id and AbpBerthsecs.PushStatus = 1 where guid = @guid and Status = 1";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@guid", guid)
            };
            var obj = SqlHelper.ExecuteScalar(SqlHelper.connectionString, CommandType.Text, sql, param);
            if (obj == null)
                return "";
            return DateTime.Parse(obj.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="OldVersion"></param>
        /// <param name="PDA"></param>
        /// <param name="Type"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public string CheckVersion(string OldVersion, string PDA, int Type, string access_token)
        {
            var loginToken = GetLoginToken(access_token);
            var temp = SettingStoreAppService.GetSettingOrNull(loginToken.TenantId, null, "PDAUpgrate");
            string[] version = OldVersion.Split('.');
            var model = GetPDAVersion(Type, loginToken.TenantId, version[0]);
            var equipment = GetEquipmentVersion(PDA);
            if (bool.Parse(temp.Value) || model.IsUpgrade || equipment.IsUpgrade)
            {
                if (VersionToInt(model.Version) > VersionToInt(OldVersion))
                {
                    return model.Version;
                }
            }
            return "-v";
        }
        /// <summary>
        /// 替换非数字为空 形成全数字
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        private Int64 VersionToInt(string version)
        {
            return long.Parse(version.Replace("A", "").Replace("p", "").Replace(".", "").Replace("V", "").Replace("C", ""));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        private EquipmentVersionDto GetPDAVersion(int Type, int TenantId,string version)
        {
            string sql = "select top 1 * from AbpEquipmentVersions where EqupmentType = " + Type + " and IsActive = 1 and IsDeleted = 0 and TenantId = " + TenantId + "and Version like '" + version + "%' order by id desc";
            return DataProcessHelper.GetEntityFromTable<EquipmentVersionDto>(SqlHelper.ExecuteDataTable(CommandType.Text, sql, null))[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PDA"></param>
        /// <returns></returns>
        private EquipmentDto GetEquipmentVersion(string PDA)
        {
            string sql = "select Version, IsUpgrade from AbpEquipments where PDA = '" + PDA + "' and IsDeleted = 0";
            return DataProcessHelper.GetEntityFromTable<EquipmentDto>(SqlHelper.ExecuteDataTable(CommandType.Text, sql, null))[0];
        }

        /// <summary>
        /// 收费员签到
        /// </summary>
        /// <param name="access_token"></param>
        public void EmployeeCheckIn(string access_token)
        {
            var loginToken = GetLoginToken(access_token);
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@berthsecId", loginToken.BerthsecIds),
                new SqlParameter("@DeviceCode", loginToken.DeviceCode),
                new SqlParameter("@employeeID", loginToken.EmployeeId),
                new SqlParameter("@TenantID", loginToken.TenantId),
                new SqlParameter("@checkInOrOutTime", DateTime.Now),
                new SqlParameter("@CompanyId", loginToken.CompanyId),
                new SqlParameter("@ParkID", loginToken.ParkIds),
                new SqlParameter("@VersionNum", loginToken.Version),
                new SqlParameter("@BatchNo", loginToken.BatchNo)
            };
            SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.StoredProcedure, "Pro_Employee_CheckinV1", param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="plateNumber"></param>
        /// <returns></returns>
        public List<BerthDto> GetBerthInfoByPlateNumber(string access_token, string plateNumber)
        {
            var loginToken = GetLoginToken(access_token);
            string sql = "select top 1 AbpBerths.*, RateId, RateId1, RateId2 from AbpBerths with(nolock) left join AbpBerthsecs on BerthsecId = AbpBerthsecs.Id where AbpBerths.IsActive = 1 and AbpBerths.TenantId = " + loginToken.TenantId + " and AbpBerths.CompanyId = " + loginToken.CompanyId + " and AbpBerths.BerthStatus = 1 and AbpBerths.RelateNumber = '" + plateNumber + "' and AbpBerths.BerthsecId <> -1";
            var berthdtoList = DataProcessHelper.GetEntityFromTable<BerthDto>(SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0]);
            if (berthdtoList.Count > 0)
            {
                List<BerthDto> temp = new List<BerthDto>();
                var model = berthdtoList[0];
                model.JsonRate = GetRate(model.RateId);
                model.JsonRate1 = GetRate(model.RateId1);
                model.JsonRate2 = GetRate(model.RateId2);
                temp.Add(model);
                return temp;
            }
            return null;
        }

        /// <summary>
        /// 获取费率
        /// </summary>
        /// <param name="BerthsecId"></param>
        /// <returns></returns>
        private string GetRate(int BerthsecId)
        {
            string sql = "select RateJson from AbpRates where Id = " + BerthsecId;
            object obj = SqlHelper.ExecuteScalar(SqlHelper.connectionString, CommandType.Text, sql);
            if (obj == null)
                return "";
            else
                return obj.ToString();
        }

        /// <summary>
        /// 远程出场增加出场标记
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="BerthsecId"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public int InsertRemoteGuid(string guid, int BerthsecId, string access_token)
        {
            var loginToken = GetLoginToken(access_token);
            string sql = "Insert Into AbpRemoteGuids([BusinessDetailGuid] , [CreationTime], [CreatorUserId], [IsActive], [BerthsecId]) values('" + guid + "', '" + DateTime.Now + "', " + loginToken.EmployeeId + ", 0, " + BerthsecId + ")";
            return SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.Text, sql);
        }


        /// <summary>
        ///获取二维码 码串
        /// </summary>
        /// <returns></returns>
        public object GetAliPayCode(string total_amount, string subject,string out_trade_no)
        {
            //string method = "alipay.trade.precreate";
            //string format = "JSON";
            //string charset = "utf-8";
            //string sign_type = "RSA2";
            //string sign = "";
            //string timestamp = "";
             string version = "1.0";
            //string biz_content = "";  //公共参数
            //string out_trade_no = "";

            //decimal total_amount = 0;
            //string subject = "";

            //组建标准请求
            IAopClient client = new DefaultAopClient(Config.Gatewayurl, Config.AppId, Config.PrivateKey, "json", version, Config.SignType, Config.AlipayPublicKey, Config.CharSet, false);
            AlipayTradePrecreateRequest request = new AlipayTradePrecreateRequest();
            request.SetNotifyUrl("");
            Dictionary<string, object> bizContent = new Dictionary<string, object>();

            bizContent.Add("out_trade_no", out_trade_no);//商户订单号
            bizContent.Add("total_amount", total_amount);//订单总金额
            bizContent.Add("subject", subject);//订单标题

            string Contentjson = JsonConvert.SerializeObject(bizContent);
            request.BizContent = Contentjson;
            AlipayTradePrecreateResponse response = client.Execute(request);
            Console.WriteLine(response.Body);

            var JsonObj = JsonConvert.DeserializeObject<dynamic>(response.Body);
        
            return JsonObj;

        }



        /// <summary>
        ///统一收单交易创建
        /// </summary>
        /// <returns></returns>
        public object CreatDeal(string total_amount, string subject, string out_trade_no, string buyer_id)
        {
          
            string version = "1.0";

            //组建标准请求
            IAopClient client = new DefaultAopClient(Config.Gatewayurl, Config.AppId, Config.PrivateKey, "json", version, Config.SignType, Config.AlipayPublicKey, Config.CharSet, false);
            AlipayTradeCreateRequest request = new AlipayTradeCreateRequest();
            request.SetNotifyUrl("");
            Dictionary<string, object> bizContent = new Dictionary<string, object>();
            bizContent.Add("out_trade_no", out_trade_no);//商户订单号
            bizContent.Add("total_amount", total_amount);//订单总金额
            bizContent.Add("subject", subject);//订单标题
            bizContent.Add("buyer_id", buyer_id);//买家支付宝用户ID

            string Contentjson = JsonConvert.SerializeObject(bizContent);
            request.BizContent = Contentjson;
            AlipayTradeCreateResponse response = client.Execute(request);
            Console.WriteLine(response.Body);

            var JsonObj = JsonConvert.DeserializeObject<dynamic>(response.Body);

            return JsonObj;

        }

        /// <summary>
        ///统一收单线下交易查询
        /// </summary>
        /// <returns></returns>
        public object QueryAliPay(string out_trade_no)
        {

            string version = "1.0";
            //组建标准请求
            IAopClient client = new DefaultAopClient(Config.Gatewayurl, Config.AppId, Config.PrivateKey, "json", version, Config.SignType, Config.AlipayPublicKey, Config.CharSet, false);

            AlipayTradeQueryRequest request = new AlipayTradeQueryRequest();
            //请求参数 out_trade_no 商户订单号
            request.BizContent = "{" +
            "  \"out_trade_no\":\""+out_trade_no+"\"," +
            "  \"trade_no\":\"\"," +
            "  \"query_options\":[" +
            "    \"trade_settle_info\"" +
            "  ]" +
            "}";
            AlipayTradeQueryResponse response = client.Execute(request);
            Console.WriteLine(response.Body);



            var JsonObj = JsonConvert.DeserializeObject<dynamic>(response.Body);

            return JsonObj;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="PlateNumber"></param>
        /// <param name="CarType"></param>
        /// <returns></returns>
        public ErrorInfo ModifyCarPlateNumber(string PlateNumber,string CarType,string Guid) 
        {
            try 
            {
                string sql = string.Empty;
                sql += $"update AbpBusinessDetail set PlateNumber = '{PlateNumber}',CarType = '{CarType}' where guid = '{Guid}';";
                sql += $"update AbpBerths set RelateNumber = '{PlateNumber}',CarType = '{CarType}' where guid = '{Guid}';";

                #region 事务提交
                SqlConnection conn = new SqlConnection(SqlHelper.connectionString);
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Transaction = tran;
                try
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    CommonTools.WriteLogFile("ErrorSQL:" + sql.ToString() + ex.ToString());
                    tran.Rollback();
                    return new ErrorInfo(2, "错误回滚！");
                }
                #endregion

                return new ErrorInfo();
            } 
            catch (Exception ex) 
            {
                CommonTools.WriteLogFile(ex.ToString());
                return new ErrorInfo(1, "修改车牌号失败！");
            }
        }

    }
}
