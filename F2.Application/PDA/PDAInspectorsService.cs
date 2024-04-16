using F2.Application.Parking.Dtos;
using F2.Application.PDA.Dtos;
using F2.Core.Extensions;
using F2.Core.Extensions.DataExtend;
using F2.Core.Extensions.Models;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using static SKIT.FlurlHttpClient.Wechat.TenpayV3.Models.CreateApplyForSubMerchantApplymentRequest.Types.Business.Types.SaleScene.Types;
using static SKIT.FlurlHttpClient.Wechat.TenpayV3.Models.CreateRefundDomesticRefundRequest.Types.Amount.Types;
using static SKIT.FlurlHttpClient.Wechat.TenpayV3.Models.UpdateMarketingMemberCardOpenCardRightsRequest.Types;

namespace F2.Application.PDA
{
    public class PDAInspectorsService: IPDAInspectorsService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public AbpLoginResult InspectorsLogin(EmployeeLoginInput input, int tenantId)
        {
            SqlHelper.ExecuteScalar(SqlHelper.connectionString, System.Data.CommandType.Text, "update AbpEquipments set DeviceType = " + input.DeviceType + " where PDA = '" + input.DeviceCode + "'");
            string sql = "select top 1 * from AbpInspectors where TenantId = @TenantId and UserName = @UserName and Password = @Password";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@TenantId", tenantId),
                new SqlParameter("@UserName", input.userNameOrEmailAddress),
                new SqlParameter("@Password", input.plainPassword)

            };
            var list = DataProcessHelper.GetEntityFromTable<EmployeeDto>(SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql, param));

            if (list.Count == 0)
            {
                return new AbpLoginResult(AbpLoginResultType.InvalidPassword);
            }
            if (list[0].AccountStatus != "1")//检测用户是否在岗
            {
                return new AbpLoginResult(AbpLoginResultType.UserIsNotActive);
            }
            return new AbpLoginResult(list[0]);
        }
        // <summary>
        /// 获取可签到的泊位段
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public GetAllBerthsecListOutput GetBerthsecList(long inspectorsId, int tenantId)
        {
        
            string sql = "select AbpBerthsecs.Id, BerthsecName,InspCheckStatus as CheckStatus,InspUseStatus as UseStatus from AbpWorkGroupInspectorsBerthsecs left join AbpBerthsecs on BerthsecId = AbpBerthsecs.Id where WorkGroupId in (select Id from AbpWorkGroupsInspectors where id in (select WorkGroupId from AbpWorkGroupInspectors where IsDeleted = 0 and InspectorsId = @InspectorsId) and IsDeleted = 0) and AbpWorkGroupInspectorsBerthsecs.IsDeleted = 0 and AbpBerthsecs.IsDeleted = 0";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@InspectorsId", inspectorsId)
            };
            return new GetAllBerthsecListOutput()
            {
                rows = DataProcessHelper.GetEntityFromTable<BerthsecCheckDto>(SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql, param))
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public LoginModel LoginToken(LoginTokenInput input)
        {
            var Scope = input.scope.Split(' ');
            var tenantId = Scope[0];                //商户代码
            var DeviceCode = Scope[1];              //设备编号
            var Version = Scope[3];                 //版本号 

            AbpUserLoginToken model = new AbpUserLoginToken();
            model.Token = Guid.NewGuid().ToString();
            model.BerthsecIds = Scope[2].Replace(".0", "");

            DataTable berthsecs = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, "SELECT TenantId, CompanyId, ParkId, RegionId FROM AbpBerthsecs where Id in ("+ model.BerthsecIds + ")");

            DataTable inspectors = SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, "select Id,UserName from AbpInspectors where TenantId = '" + berthsecs.Rows[0]["TenantId"] + "' and UserName = '"+ input.userName + "' and Password = '"+ input.password + "'");
            model.InspectorsId = Convert.ToInt32(inspectors.Rows[0]["Id"]);
            model.CompanyId = Convert.ToInt32(berthsecs.Rows[0]["CompanyId"]);
            model.TenantId = Convert.ToInt32(berthsecs.Rows[0]["TenantId"]);
            model.Version = Version;
            for (int i=0; i< berthsecs.Rows.Count;i++) {
                model.ParkIds = model.ParkIds + Convert.ToInt32(berthsecs.Rows[i]["ParkId"]) + ",";
                model.RegionIds = model.RegionIds + Convert.ToInt32(berthsecs.Rows[i]["RegionId"]) + ",";
            }
            model.DeviceCode = DeviceCode;
            string sql = "insert into AbpUserLoginToken(Token, TenantId, CompanyId, RegionIds, ParkIds, DeviceCode, BerthsecIds, InspectorsId, Version, CreateTime) values(@Token, @TenantId, @CompanyId, @RegionIds, @ParkIds, @DeviceCode, @BerthsecIds, @InspectorsId, @Version, getdate()) ";

            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@Token", model.Token),
                new SqlParameter("@BerthsecIds", model.BerthsecIds),
                new SqlParameter("@DeviceCode", model.DeviceCode),
                new SqlParameter("@TenantId", model.TenantId),
                new SqlParameter("@CompanyId", model.CompanyId),
                new SqlParameter("@RegionIds", model.RegionIds),
                new SqlParameter("@ParkIds", model.ParkIds),
                new SqlParameter("@InspectorsId", model.InspectorsId),
                new SqlParameter("@Version", model.Version)
            };
            SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.Text, sql, param);//写入在线登录信息

            return new LoginModel() { access_token = model.Token, expires_in = "30", refresh_token = model.Token, token_type = "app" };
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
        /// <param name=""></param>
        /// <returns></returns>
        private InspectorsDto GetInspectorsInfo(long inspectorsId)
        {
            string sql = "select * from AbpInspectors where Id = " + inspectorsId;
            return DataProcessHelper.GetEntityFromTable<InspectorsDto>(SqlHelper.ExecuteDataTable(CommandType.Text, sql, null))[0];
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
        public List<BerthsecDto> GetBerthsecListIns(AbpUserLoginToken loginToken)
        {
            string sql = "select AbpBerthsecs.Id, AbpBerthsecs.BerthsecName, BeginNumeber, EndNumeber, CustomNumeber, CheckInStatus, CheckStatus, CheckOutStatus, CheckInTime, CheckInEmployeeId, CheckOutTime, CheckOutEmployeeId, CheckInDeviceCode, CheckOutDeviceCode, XPoint, YPoint, RegionId, ParkId, AbpBerthsecs.TenantId, AbpBerthsecs.IsActive, UseStatus, AbpBerthsecs.CompanyId,  RateId, A.RatePDA as FeeModel,  RateId1,B.RatePDA as FeeModel1, RateId2, C.RatePDA as FeeModel2, BerthCount, CONVERT(bit, 1) as PushStatus, Lat, Lng, AbpBerthsecs.IsDeleted, AbpBerthsecs.DeleterUserId, AbpBerthsecs.DeletionTime, AbpBerthsecs.LastModificationTime, AbpBerthsecs.LastModifierUserId,  AbpBerthsecs.CreationTime, AbpBerthsecs.CreatorUserId from AbpBerthsecs with(nolock) left join AbpRates as A on A.Id = RateId left join AbpRates as B on B.Id = RateId1 left join AbpRates as C on C.Id = RateId2 where AbpBerthsecs.Id in (0," + loginToken.BerthsecIds + ",0) and AbpBerthsecs.IsDeleted = 0";

            return DataProcessHelper.GetEntityFromTable<BerthsecDto>(SqlHelper.ExecuteDataTable(CommandType.Text, sql, null));
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
        /// 巡查员签到
        /// </summary>
        /// <param name="access_token"></param>
        public void InspectorsCheckIn(string access_token)
        {
            var loginToken = GetLoginToken(access_token);
            string sql = "update AbpInspectors set CheckIn = 1, CheckOut = 0, CheckInTime = '" + DateTime.Now + "' where Id = '" + loginToken.InspectorsId + "';";
            sql += "update AbpBerthsecs set CheckInInspId = '"+ loginToken.InspectorsId + "', InspCheckInDeviceCode = '"+ loginToken.DeviceCode + "', InspCheckInTime = '"+ DateTime.Now + "', InspCheckStatus = 1, InspUseStatus = 1  where Id in (" + loginToken.BerthsecIds + ");";
            sql += "insert AbpInspectorCheck (InspectorId, ParkId, CheckIn, CheckInTime, CheckOut, DeviceCode, BerthsecId, CompanyId, TenantId) values('" + loginToken.InspectorsId + "', '" + loginToken.ParkIds + "', 1, '" + DateTime.Now + "', 0, '" + loginToken.DeviceCode + "', '" + loginToken.BerthsecIds + "', '" + loginToken.CompanyId + "', '" + loginToken.TenantId + "');";
            SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public PdaModel DownParameterForInspectors(string access_token)
        {
            var loginToken = GetLoginToken(access_token);
            PdaModel pdaModel = new PdaModel();
            pdaModel.PrintList = GetPrintList(loginToken);
            pdaModel.WhiteList = GetWhiteList(loginToken);
            pdaModel.MonthlyCarList = GetMonthlyList(loginToken);
            pdaModel.Berths = GetBerthList(loginToken);
            pdaModel.BerthsecList = GetBerthsecListIns(loginToken);
            pdaModel.Inspectors = GetInspectorsInfo(loginToken.InspectorsId);
            var ticketList = GetTicketList(loginToken);
            pdaModel.CarInTicketCss = ticketList.First(entry => entry.Status == "CarIn");
            pdaModel.CarOutTicketCss = ticketList.First(entry => entry.Status == "CarOut");
            pdaModel.OweTicketCSS = ticketList.First(entry => entry.Status == "OweDetail");
            pdaModel.RepayTicketCss = ticketList.First(entry => entry.Status == "Repay");
            pdaModel.DayChargeTicketCss = ticketList.First(entry => entry.Status == "DayCharge");
            pdaModel.TotalPaidTicketCss = ticketList.First(entry => entry.Status == "TotalPaid");
            pdaModel.InsDayChargeTicketCss = ticketList.First(entry => entry.Status == "InsDayCharge");
            pdaModel.InspRepayTicketCss = ticketList.First(entry => entry.Status == "InspRepay");

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
            InspectorsCheckIn(access_token);
            
            return pdaModel;
        }
        /// <summary>
        /// 巡查员签退
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public int InspectorsOutLineLogout(string berthsecid, string access_token)
        {
            var loginToken = GetLoginToken(access_token);
            string sql = "update AbpInspectors set CheckIn = 0, CheckOut = 1,CheckOuttype=1, CheckOutTime = '" + DateTime.Now + "' where Id = '" + loginToken.InspectorsId + "';";
            sql += "update AbpBerthsecs set CheckOutInspId = '"+ loginToken.InspectorsId + "', InspCheckOutDeviceCode = '"+ loginToken.DeviceCode + "', InspCheckOutTime = '"+ DateTime.Now + "', InspCheckStatus = 0, InspUseStatus = 0  where Id in (" + loginToken.BerthsecIds + ");";
            sql += "update AbpInspectorCheck set CheckIn = 0, CheckOut = 1, CheckOutTime = '"+ DateTime.Now + "' where id in (select Id from AbpInspectorCheck where CheckOutTime Is null and InspectorId = '"+ loginToken.InspectorsId + "');";
            return SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
        }
        /// <summary>
        /// 巡查员获取停车记录
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public Hashtable GetStopCarList(Hashtable input) {
            Hashtable res = new Hashtable();
            var Start = (Convert.ToInt32(input["Page"]) - 1) * Convert.ToInt32(input["PageSize"]);
            var End = Convert.ToInt32(input["PageSize"]);
            DataTable Tocken = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select BerthsecIds from AbpUserLoginToken where Token='" + input["access_token"] + "'").Tables[0];
            string parm = "";
            if (!string.IsNullOrEmpty(Convert.ToString(input["PlateNumber"])))
            {
                if (Convert.ToString(input["PlateNumber"]) == "_无_" || Convert.ToString(input["PlateNumber"]) == "_有_")
                {
                    if (Convert.ToString(input["PlateNumber"]) == "_无_")
                    {
                        parm += " and (PlateNumber is null or PlateNumber like '%无%')";
                    }
                    else {
                        parm += " and PlateNumber is not null and PlateNumber not like '%无%'";
                    }
                }
                else {
                    parm += " and PlateNumber like '%" + Convert.ToString(input["PlateNumber"]) + "%'";
                }
            }            
            string sql = "select CarType,BerthNumber,CONVERT(VARCHAR(20),CarInTime,120) as CarInTime,CONVERT(VARCHAR(20),CarOutTime,120) as CarOutTime,Receivable,Arrearage,PlateNumber from AbpBusinessDetail where BerthsecId in (" + Tocken.Rows[0]["BerthsecIds"] + ")"+ parm + " order by CarInTime desc offset " + Start + " rows fetch next " + End + " rows only";
            DataTable CarList = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0];
            sql = "select count(*) num from AbpBusinessDetail where BerthsecId in (" + Tocken.Rows[0]["BerthsecIds"] + ")" + parm;
            DataTable Num = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0];
            res.Add("List", CarList);
            res.Add("Page", Convert.ToInt32(input["Page"]));
            res.Add("Total", Num.Rows[0]["num"]);
            return res;
        }
        /// <summary>
        /// 巡查员实收合计
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public Hashtable GetAllFeeList(Hashtable input)
        {
            Hashtable res = new Hashtable();
            res.Add("Money", 0);
            res.Add("WXMoney", 0);
            res.Add("ZFBMoney", 0);
            res.Add("FactReceive", 0);
            res.Add("Arrearage", 0);
            res.Add("Repayment", 0);
            DataTable Tocken = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select BerthsecIds from AbpUserLoginToken where Token='" + input["access_token"] + "'").Tables[0];
            string parm = "";
            if (!string.IsNullOrEmpty(Convert.ToString(input["StartTime"])))
            {
                parm += " and CarInTime>='"+ input["StartTime"] + "'";
            }
            if (!string.IsNullOrEmpty(Convert.ToString(input["EndTime"])))
            {
                parm += " and CarInTime<'" + input["EndTime"] + "'";
            }
            string sql = "select sum(FactReceive) FactReceive,sum(Arrearage) Arrearage,sum(Repayment) Repayment from AbpBusinessDetail where BerthsecId in (" + Tocken.Rows[0]["BerthsecIds"] + ")"+ parm;
            DataTable FeeCom = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0];
            sql = "select Sum(FactReceive) FactReceive,PayStatus from AbpBusinessDetail where 1=1"+ parm + " group by PayStatus";
            DataTable FeePay = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0];
            for (int i=0;i< FeePay.Rows.Count; i++) {
                if (Convert.ToString(FeePay.Rows[i]["PayStatus"]) == "1")
                {
                    res["Money"] = FeePay.Rows[i]["FactReceive"];
                }
                if (Convert.ToString(FeePay.Rows[i]["PayStatus"]) == "2") {
                    res["WXMoney"] = FeePay.Rows[i]["FactReceive"];
                }
                if (Convert.ToString(FeePay.Rows[i]["PayStatus"]) == "3")
                {
                    res["ZFBMoney"] = FeePay.Rows[i]["FactReceive"];
                }
            }
            for (int i = 0; i < FeeCom.Rows.Count; i++)
            {
                if (FeeCom.Rows[i]["FactReceive"]!=null) {
                    res["FactReceive"] = FeeCom.Rows[i]["FactReceive"];
                }
                if (FeeCom.Rows[i]["Arrearage"] != null)
                {
                    res["Arrearage"] = FeeCom.Rows[i]["Arrearage"];
                }
                if (FeeCom.Rows[i]["Repayment"] != null)
                {
                    res["Repayment"] = FeeCom.Rows[i]["Repayment"];
                }
            }
            return res;
        }
        /// <summary>
        /// 巡查员欠费记录
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public Hashtable GetArrearageData(Hashtable input) {
            Hashtable res = new Hashtable();
            var Start = (Convert.ToInt32(input["Page"]) - 1) * Convert.ToInt32(input["PageSize"]);
            var End = Convert.ToInt32(input["PageSize"]);
            string parm = "";
            if (!string.IsNullOrEmpty(Convert.ToString(input["PlateNumber"])))
            {
                parm += " and PlateNumber like '%" + Convert.ToString(input["PlateNumber"]) + "%'";
            }
            DataTable Tocken = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select BerthsecIds from AbpUserLoginToken where Token='" + input["access_token"] + "'").Tables[0];
            string sql = "select Arrearage,BerthNumber,PlateNumber,CONVERT(VARCHAR(20),CarInTime,120) as CarInTime,CONVERT(VARCHAR(20),CarOutTime,120) as CarOutTime from AbpBusinessDetail where Arrearage>0 and BerthsecId in (" + Tocken.Rows[0]["BerthsecIds"] + ")"+ parm+ " order by CarInTime desc offset " + Start + " rows fetch next " + End + " rows only";
            DataTable List = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0];
            sql = "select count(*) num from AbpBusinessDetail where Arrearage > 0 and BerthsecId in (" + Tocken.Rows[0]["BerthsecIds"] + ")" + parm;
            DataTable Num = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0];
            res.Add("List", List);
            res.Add("Page", Convert.ToInt32(input["Page"]));
            res.Add("Total", Num.Rows[0]["num"]);
            return res;
        }
        /// <summary>
        /// 巡查员设置信息
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public Hashtable GetInspectorsInfo(Hashtable input) {
            Hashtable res = new Hashtable();
            res.Add("TrueName", "");
            res.Add("UserName","");
            res.Add("FactReceive", 0);
            res.Add("Arrearage", 0);
            res.Add("Repayment", 0);
            res.Add("Receivable", 0);
            res.Add("Num", 0);
            res.Add("CheckInTime", "");
            res.Add("CheckOutTime", "");
            res.Add("BerthsecId", "");
            res.Add("BerthsecName", "");
            DataTable Tocken = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select a.BerthsecIds,b.TrueName,b.UserName,b.CheckInTime,b.CheckOutTime from AbpUserLoginToken a,AbpInspectors b where a.InspectorsId=b.Id and a.Token='" + input["access_token"] + "'").Tables[0];
            string sql = "select sum(FactReceive) FactReceive,sum(Arrearage) Arrearage,sum(Repayment) Repayment,count(*) num,sum(Receivable) Receivable from AbpBusinessDetail where BerthsecId in (" + Tocken.Rows[0]["BerthsecIds"] + ")";
            DataTable List = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0];
            sql = "select Id BerthsecId,BerthsecName from AbpBerthsecs where Id in (" + Tocken.Rows[0]["BerthsecIds"] + ")";
            DataTable BerthsecList = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0];
            for (int i = 0; i < Tocken.Rows.Count; i++)
            {
                res["TrueName"] = Tocken.Rows[i]["TrueName"];
                res["UserName"] = Tocken.Rows[i]["UserName"];
                res["CheckInTime"] = Tocken.Rows[i]["CheckInTime"];
                res["CheckOutTime"] = Tocken.Rows[i]["CheckOutTime"];
            }
            for (int i = 0; i < List.Rows.Count; i++)
            {
                res["FactReceive"] = List.Rows[i]["FactReceive"];
                res["Arrearage"] = List.Rows[i]["Arrearage"];
                res["Repayment"] = List.Rows[i]["Repayment"];
                res["Num"] = List.Rows[i]["num"];
                res["Receivable"] = List.Rows[i]["Receivable"];
            }
            string BerthsecIdStr = "";
            string BerthsecNameStr = "";
            for (int i = 0; i < BerthsecList.Rows.Count; i++)
            {
                BerthsecIdStr+= BerthsecList.Rows[i]["BerthsecId"]+",";
                BerthsecNameStr+= BerthsecList.Rows[i]["BerthsecName"]+",";
            }
            res["BerthsecId"] = BerthsecIdStr.Substring(0, BerthsecIdStr.Length-1);
            res["BerthsecName"] = BerthsecNameStr.Substring(0, BerthsecNameStr.Length - 1);
            return res;
        }
        /// <summary>
        /// 巡查员泊位管理
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public Hashtable GetIBerthInfo(Hashtable input) {
            Hashtable res = new Hashtable();
            var Start = (Convert.ToInt32(input["Page"]) - 1) * Convert.ToInt32(input["PageSize"]);
            var End = Convert.ToInt32(input["PageSize"]);
            string parm = "";
            if (!string.IsNullOrEmpty(Convert.ToString(input["PlateNumber"])))
            {
                parm += " and RelateNumber like '%" + Convert.ToString(input["PlateNumber"]) + "%'";
            }
            if (!string.IsNullOrEmpty(Convert.ToString(input["BerthNumber"])))
            {
                parm += " and BerthNumber like '%" + Convert.ToString(input["BerthNumber"]) + "%'";
            }
            DataTable Tocken = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select BerthsecIds from AbpUserLoginToken where Token='" + input["access_token"] + "'").Tables[0];
            string sql = "select BerthNumber,RelateNumber PlateNumber,ParkStatus from AbpBerths where BerthsecId in (" + Tocken.Rows[0]["BerthsecIds"] + ")" + parm+ " order by BerthNumber asc offset " + Start + " rows fetch next " + End + " rows only";
            DataTable List = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0];
            sql = "select count(*) num from AbpBerths where BerthsecId in (" + Tocken.Rows[0]["BerthsecIds"] + ")" + parm;
            DataTable Num = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0];
            res.Add("List", List);
            res.Add("Page", Convert.ToInt32(input["Page"]));
            res.Add("Total", Num.Rows[0]["num"]);
            return res;
        }
        /// <summary>
        /// 巡查员修改密码
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public Hashtable ModifyPassword(Hashtable input) {
            Hashtable res = new Hashtable();
            DataTable Tocken = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select InspectorsId from AbpUserLoginToken a,AbpInspectors b where a.Token='" + input["access_token"] + "' and a.InspectorsId=b.Id and b.UserName='"+ input["UserName"] + "' and b.Password='"+ input["OldPassword"] + "'").Tables[0];
            if (Tocken.Rows.Count > 0)
            {
                string sql = "update AbpInspectors set Password='"+ input["NewPassword"] + "' where Id='"+ Tocken.Rows[0]["InspectorsId"] + "'";
                SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                res.Add("result", "success");
            }
            else {
                res.Add("result", "false");
                res.Add("msg","用户名或密码错误");
            }
            return res;
        }
        /// <summary>
        /// 保存任务图片到本地
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private string SaveUploadImageTask(byte[] bytes)
        {
            try
            {
                DateTime dt = DateTime.Now;
                string guid = Guid.NewGuid().ToString();
                //格式 图片名称
                string imageKey = $"{guid}";

                //获取上传根路径
                string UrlPath = ConfigurationManager.AppSettings["InspTaskUrlUpload"];
                //建立存储的目录
                string ImageFileUrl = UrlPath + $"\\{dt.Year}\\{dt.Month:d2}\\";
                //判断目录是否存在
                if (!Directory.Exists(ImageFileUrl))
                {
                    //如果不存在，创建
                    Directory.CreateDirectory(ImageFileUrl);
                }
                string NewFilethumbName = $"{imageKey}.jpg";  //图片命名
                if (bytes != null)
                {
                    FileStream fileWriter = new FileStream(ImageFileUrl + NewFilethumbName, FileMode.Create);
                    fileWriter.Write(bytes, 0, bytes.Length);
                    fileWriter.Dispose();
                    return $"\\InspTask\\{dt.Year}\\{dt.Month:d2}\\" + NewFilethumbName;
                }
                else {
                    return "";
                }
                
            }
            catch (ArgumentException e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 巡查员任务上传
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public Hashtable InsertTaskFeedbacks(string access_token, string TaskId, string BerthsecId,string BerthNumber,string Remark, byte[] PicUrl1, byte[] PicUrl2, byte[] PicUrl3)
        {
            Hashtable res = new Hashtable();
            try
            {
                var savePath1 = SaveUploadImageTask(PicUrl1);
                var savePath2 = SaveUploadImageTask(PicUrl2);
                var savePath3 = SaveUploadImageTask(PicUrl3);
                DataTable Tocken = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select BerthsecIds,InspectorsId from AbpUserLoginToken where Token='" + access_token + "'").Tables[0];
                string sql = "insert into AbpInspectorTaskFeedbacks (InspectorTaskId,BerthsecId,BerthNumber,UploadTime,Remark,PicUrl1,PicUrl2,PicUrl3) values ('" + TaskId + "','" + BerthsecId + "','" + BerthNumber + "',getdate(),'" + Remark + "','" + savePath1 + "','" + savePath2 + "','" + savePath3 + "');";
                sql += "update AbpInspectorTasks set Status=2 where Id='"+ TaskId + "';";
                SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                res.Add("result", "success");
            }
            catch (Exception ex) {
                res.Add("result", "false");
                res.Add("msg", ex.ToString());
            }
            return res;
        }
        /// <summary>
        /// 保存事件图片到本地
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private string SaveUploadImageEvent(byte[] bytes)
        {
            try
            {
                DateTime dt = DateTime.Now;
                string guid = Guid.NewGuid().ToString();
                //格式 图片名称
                string imageKey = $"{guid}";

                //获取上传根路径
                string UrlPath = ConfigurationManager.AppSettings["InspEventUrlUpload"];
                //建立存储的目录
                string ImageFileUrl = UrlPath + $"\\{dt.Year}\\{dt.Month:d2}\\";
                //判断目录是否存在
                if (!Directory.Exists(ImageFileUrl))
                {
                    //如果不存在，创建
                    Directory.CreateDirectory(ImageFileUrl);
                }
                string NewFilethumbName = $"{imageKey}.jpg";  //图片命名
                if (bytes != null)
                {
                    FileStream fileWriter = new FileStream(ImageFileUrl + NewFilethumbName, FileMode.Create);
                    fileWriter.Write(bytes, 0, bytes.Length);
                    fileWriter.Dispose();
                    return $"\\InspEvent\\{dt.Year}\\{dt.Month:d2}\\" + NewFilethumbName;
                }
                else
                {
                    return "";
                }

            }
            catch (ArgumentException e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 巡查员事件上传
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public Hashtable InsertInspectorsEvent(string access_token, string BerthsecId, string BerthNumber, string EventContent, byte[] PicUrl)
        {
            Hashtable res = new Hashtable();
            try
            {
                var savePath = SaveUploadImageEvent(PicUrl);

                DataTable Tocken = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select TenantId,CompanyId,BerthsecIds,InspectorsId from AbpUserLoginToken where Token='" + access_token + "'").Tables[0];
                DataTable Berthsecs = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select RegionId,ParkId from AbpBerthsecs where Id='" + BerthsecId + "'").Tables[0];
                string sql = "insert into AbpInspectorsEvent (TenantId,CompanyId,RegionId,ParkId,BerthsecId,BerthNum,EventContent,EventTime,InspectorsId,Status,EventPic,IsDeleted) values ('" + Tocken.Rows[0]["TenantId"] + "','"+ Tocken.Rows[0]["CompanyId"] + "','"+ Berthsecs.Rows[0]["RegionId"] + "','"+ Berthsecs.Rows[0]["ParkId"] + "','"+ BerthsecId + "','"+ BerthNumber + "','"+ EventContent + "',getdate(),'" + Tocken.Rows[0]["InspectorsId"] + "','1','"+ savePath + "','0')";
                SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
                res.Add("result", "success");
            }
            catch (Exception ex)
            {
                res.Add("result", "false");
                res.Add("msg", ex.ToString());
            }
            return res;
        }
        /// <summary>
        /// 巡查员任务表查询
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public Hashtable GetInspectorTasks(Hashtable input)
        {
            Hashtable res = new Hashtable();
            var Start = (Convert.ToInt32(input["Page"]) - 1) * Convert.ToInt32(input["PageSize"]);
            var End = Convert.ToInt32(input["PageSize"]);
            DataTable Tocken = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select InspectorsId from AbpUserLoginToken where Token='" + input["access_token"] + "'").Tables[0];
            string sql = "select a.Remark,CONVERT(VARCHAR(20),a.CreationTime,120) as CreationTime,a.Id,a.TaskName,a.Status,b.PicUrl1,b.PicUrl2,b.PicUrl3 from AbpInspectorTasks a left join AbpInspectorTaskFeedbacks b on a.Id=b.InspectorTaskId where a.IsDeleted=0 and a.InspectorId='" + Tocken.Rows[0]["InspectorsId"] + "' order by a.CreationTime desc offset " + Start + " rows fetch next " + End + " rows only";
            DataTable List = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0];
            sql = "select count(*) num from AbpInspectorTasks a left join AbpInspectorTaskFeedbacks b on a.Id=b.InspectorTaskId where a.IsDeleted=0 and a.InspectorId='" + Tocken.Rows[0]["InspectorsId"] + "'";
            DataTable Num = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0];
            res.Add("List", List);
            res.Add("Page", Convert.ToInt32(input["Page"]));
            res.Add("Total", Num.Rows[0]["num"]);
            return res;
        }
        /// <summary>
        /// 巡查员事件表查询
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public Hashtable GetInspectorEvent(Hashtable input)
        {
            Hashtable res = new Hashtable();
            var Start = (Convert.ToInt32(input["Page"]) - 1) * Convert.ToInt32(input["PageSize"]); 
            var End = Convert.ToInt32(input["PageSize"]);
            DataTable Tocken = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select InspectorsId from AbpUserLoginToken where Token='" + input["access_token"] + "'").Tables[0];
            string sql = "select a.Status,a.EventPic,a.EventContent,a.BerthNum,CONVERT(VARCHAR(20),EventTime,120) as EventTime,b.ParkName,c.BerthsecName from AbpInspectorsEvent a,AbpParks b,AbpBerthsecs c where a.ParkId=b.Id and a.BerthsecId=c.Id and a.IsDeleted=0 and a.InspectorsId='"+ Tocken.Rows[0]["InspectorsId"] + "' order by a.EventTime desc offset " + Start + " rows fetch next " + End + " rows only";
            DataTable List = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0];
            sql = "select count(*) num from AbpInspectorsEvent a,AbpParks b,AbpBerthsecs c where a.ParkId=b.Id and a.BerthsecId=c.Id and a.IsDeleted=0 and a.InspectorsId='" + Tocken.Rows[0]["InspectorsId"] + "'";
            DataTable Num = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0];
            res.Add("List", List);
            res.Add("Page", Convert.ToInt32(input["Page"]));
            res.Add("Total", Num.Rows[0]["num"]);
            return res;
        }
        /// <summary>
        /// 巡查员事件回复表查询
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public Hashtable GetInspectorTaskFeedbacks(Hashtable input)
        {
            Hashtable res = new Hashtable();
            var Start = (Convert.ToInt32(input["Page"]) - 1) * Convert.ToInt32(input["PageSize"]);
            var End = Convert.ToInt32(input["PageSize"]);
            DataTable Tocken = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select InspectorsId from AbpUserLoginToken where Token='" + input["access_token"] + "'").Tables[0];
            string sql = "select b.TaskContent,CONVERT(VARCHAR(20),b.UploadTime,120) as UploadTime,b.BerthNumber,c.ParkName,d.BerthsecName,b.PicUrl1,b.PicUrl2,b.PicUrl3 from AbpInspectorTasks a,AbpInspectorTaskFeedbacks b,AbpParks c,AbpBerthsecs d where a.Id=b.InspectorTaskId and a.ParkId=c.Id and b.BerthsecId=d.Id and a.InspectorId='"+ Tocken.Rows[0]["InspectorsId"] + "' order by b.UploadTime desc offset " + Start + " rows fetch next " + End + " rows only";
            DataTable List = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0];
            sql = "select count(*) num from AbpInspectorTasks a, AbpInspectorTaskFeedbacks b,AbpParks c, AbpBerthsecs d where a.Id = b.InspectorTaskId and a.ParkId = c.Id and b.BerthsecId = d.Id and a.InspectorId = '" + Tocken.Rows[0]["InspectorsId"] + "'";
            DataTable Num = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0];
            res.Add("List", List);
            res.Add("Page", Convert.ToInt32(input["Page"]));
            res.Add("Total", Num.Rows[0]["num"]);
            return res;
        }

        /// <summary>
        /// 上传设备定位
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public int UpdateInspectorGps(string x, string y, string access_token)
        {
            var loginToken = GetLoginToken(access_token);
            string sql = "insert into AbpEquipmentGps(PDA, X, Y, CreationTime, InspectorId) values('"+ loginToken.DeviceCode + "', '"+x+"', '"+ y + "', getdate(), '"+ loginToken.InspectorsId + "')";
            return SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.Text, sql);
        }
        /// <summary>
        /// 巡查员任务条数
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public Hashtable GetInspectorsTaskNum(Hashtable input)
        {
            Hashtable res = new Hashtable();
            DataTable Tocken = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select InspectorsId from AbpUserLoginToken where Token='" + input["access_token"] + "'").Tables[0];
            string sql = "select count(*) num from AbpInspectorTasks where IsDeleted=0 and Status=1 and InspectorId='" + Tocken.Rows[0]["InspectorsId"] + "'";
            DataTable Num = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0];
            res.Add("Num", Num.Rows[0]["num"]);
            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OldVersion"></param>
        /// <param name="PDA"></param>
        /// <param name="Type"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        private EquipmentVersionDto GetPDAVersion(int Type, int TenantId, string version)
        {
            string sql = "select top 1 * from AbpEquipmentInspVersions where EqupmentType = " + Type + " and IsActive = 1 and IsDeleted = 0 and TenantId = " + TenantId + " and Version like '" + version + "%' order by id desc";
            return DataProcessHelper.GetEntityFromTable<EquipmentVersionDto>(SqlHelper.ExecuteDataTable(CommandType.Text, sql, null))[0];
        }
        private EquipmentDto GetEquipmentVersion(string PDA)
        {
            string sql = "select Version, IsUpgrade from AbpEquipmentsInsp where PDA = '" + PDA + "' and IsDeleted = 0";
            return DataProcessHelper.GetEntityFromTable<EquipmentDto>(SqlHelper.ExecuteDataTable(CommandType.Text, sql, null))[0];
        }
        private Int64 VersionToInt(string version)
        {
            return long.Parse(version.Replace("A", "").Replace("p", "").Replace(".", "").Replace("V", "").Replace("C", ""));
        }
        public string CheckInspVersion(string OldVersion, string PDA, int Type, string access_token)
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
        /// 巡查员进场车辆、离场车辆、补交车辆打印查询
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public Hashtable GetTypeCarDataList(Hashtable input)
        {
            Hashtable res = new Hashtable();
            var Start = (Convert.ToInt32(input["Page"]) - 1) * Convert.ToInt32(input["PageSize"]);
            var End = Convert.ToInt32(input["PageSize"]);
            DataTable Tocken = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select a.BerthsecIds,a.InspectorsId,b.TrueName from AbpUserLoginToken a,AbpInspectors b where a.InspectorsId=b.Id and a.Token='" + input["access_token"] + "'").Tables[0];
            string parm = "";
            if (Convert.ToString(input["StopType"])=="1") {
                //进场车辆
                parm += " and a.CarOutTime is null";
            }
            if (Convert.ToString(input["StopType"]) == "2")
            {
                //离场车辆
                parm += " and a.CarOutTime is not null";
            }
            if (Convert.ToString(input["StopType"]) == "3")
            {
               //欠费车辆
                parm += " and a.Arrearage>0 and a.Status=3";
            }
            string sql = "select a.StopType,a.CarType,a.Prepaid,a.Money,a.EscapeOperaId,a.EscapePayStatus,a.CarRepaymentTime,a.BerthNumber,CONVERT(VARCHAR(20),a.CarInTime,120) as CarInTime,CONVERT(VARCHAR(20),a.CarOutTime,120) as CarOutTime,a.Arrearage,a.PlateNumber,b.ParkName from AbpBusinessDetail a,AbpParks b where a.IsDeleted=0 and a.ParkId=b.Id and a.BerthsecId in (" + Tocken.Rows[0]["BerthsecIds"] + ")" + parm + " order by a.CarInTime desc offset " + Start + " rows fetch next " + End + " rows only";
            DataTable CarList = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0];
            sql = "select count(*) num from AbpBusinessDetail a where a.BerthsecId in (" + Tocken.Rows[0]["BerthsecIds"] + ")" + parm;
            DataTable Num = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0];
            res.Add("List", CarList);
            res.Add("Inspectors", Tocken.Rows[0]["TrueName"]);
            res.Add("Page", Convert.ToInt32(input["Page"]));
            res.Add("Total", Num.Rows[0]["num"]);
            return res;
        }
        /// <summary>
        /// 巡查员进场车辆、离场车辆、补交车辆数量查询
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public Hashtable GetTypeCarDataCount(Hashtable input)
        {
            Hashtable res = new Hashtable();
            DataTable Tocken = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select BerthsecIds from AbpUserLoginToken where Token='" + input["access_token"] + "'").Tables[0];
            string sql = "";
            sql = "select count(*) num from AbpBusinessDetail where BerthsecId in (" + Tocken.Rows[0]["BerthsecIds"] + ") and CarOutTime is null";
            DataTable InNum = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0];
            sql = "select count(*) num from AbpBusinessDetail where BerthsecId in (" + Tocken.Rows[0]["BerthsecIds"] + ") and CarOutTime is not null";
            DataTable OutNum = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0];
            sql = "select count(*) num from AbpBusinessDetail where BerthsecId in (" + Tocken.Rows[0]["BerthsecIds"] + ") and Arrearage>0 and Status=3";
            DataTable ArrearageNum = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, sql).Tables[0];
            res.Add("InNum", InNum.Rows[0]["num"]);
            res.Add("OutNum", OutNum.Rows[0]["num"]);
            res.Add("ArrearageNum", ArrearageNum.Rows[0]["num"]);
            return res;
        }
    }
}
