using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using F2.Application.Rates.Dtos;
using F2.Core.Extensions;
using System.Data;
using System.Data.SqlClient;
using F2.Application.PDA.Dtos;
using F2.Application.PDA;

namespace F2.Application.Rates
{
    /// <summary>
    /// 
    /// </summary>
    public class RateAppService : IRateAppService
    {
        #region  Var
        private int freecount = 0;
        private readonly IBerthsecAppService _berthsecAppService;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public RateAppService()
        {
            freecount = 0;
            _berthsecAppService = new BerthsecAppService();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RateId"></param>
        /// <returns></returns>
        private string GetRateInfo(int RateId)
        {
            string sql = "select RateJson from AbpRates where id = " + RateId;
            return SqlHelper.ExecuteScalar(SqlHelper.connectionString, CommandType.Text, sql).ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="berthsecID"></param>
        /// <param name="inCarTime"></param>
        /// <param name="outCarTime"></param>
        /// <param name="carType"></param>
        /// <param name="RateId"></param>
        /// <param name="parkId"></param>
        /// <param name="plateNumber"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public RateCalculateModel RateCalculate(int berthsecID, DateTime inCarTime, DateTime outCarTime, int cartype, int RateId, int parkId, string plateNumber, int companyId)
        {
            RateCalculateModel rateclaculatemodel = new RateCalculateModel();

            RateMode ratemode = new RateMode();

            string MonthyType = "";

            BerthsecDto berthsec = _berthsecAppService.GetBerthsecInfo(berthsecID); //_abpBerthsecRepository.Get(berthsecID);
            if (!string.IsNullOrWhiteSpace(plateNumber))
            {
                string sql = "select count(1) as count from AbpWhiteList where IsDeleted = 0 and IsActive = 1 and RelateNumber = '" + plateNumber + "' and CompanyId = " + companyId;

                int count = int.Parse(SqlHelper.ExecuteScalar(SqlHelper.connectionString, CommandType.Text, sql, null).ToString());
                if (count > 0)
                {
                    return rateclaculatemodel;
                }

                DataTable dt = SqlHelper.ExecuteDataset(SqlHelper.connectionString, CommandType.Text, "select * from AbpMonthlyCars with(nolock) where  PlateNumber = '" + plateNumber + "' and IsDeleted = 0 and (case when CompanyId is null then AbpMonthlyCars.tenantid else " + berthsec.TenantId + "  end)  = " + berthsec.TenantId + " and (case when CompanyId is not null then AbpMonthlyCars.CompanyId else " + berthsec.CompanyId + " end) = " + berthsec.CompanyId + " and BeginTime <= getdate() and EndTime >= getdate() and (charindex(@parkid, ','+ ParkIds + ',') > 0 or ParkIds = '0')", new SqlParameter[] { new SqlParameter("@parkid", "," + berthsec.ParkId + ",") }).Tables[0];
                if (dt.Rows.Count > 0)
                    MonthyType = dt.Rows[0]["MonthyType"].ToString();
            }

            ratemode.rateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<RateModel>(GetRateInfo(berthsec.RateId));//早班费率

            if (inCarTime > outCarTime)
            {
                rateclaculatemodel.exceptionMsg = "入场时间大于出场时间";
                return rateclaculatemodel;
            }

            if (berthsec.RateId1.HasValue && berthsec.RateId1.Value != 0)
                ratemode.rateModel1 = Newtonsoft.Json.JsonConvert.DeserializeObject<RateModel>(GetRateInfo(berthsec.RateId1.Value));//中班费率

            if (berthsec.RateId2.HasValue && berthsec.RateId2.Value != 0)
                ratemode.rateModel2 = Newtonsoft.Json.JsonConvert.DeserializeObject<RateModel>(GetRateInfo(berthsec.RateId2.Value));//晚班费率

            if (ratemode.rateModel.IsActive == false)
            {
                rateclaculatemodel.exceptionMsg = "费率未启用";
                return rateclaculatemodel;
            }

            DateTime rateInCarTime = inCarTime.AddHours(-int.Parse(ratemode.rateModel.TimeSettingList[0].beginTime.Split(':')[0])).
                AddMinutes(-int.Parse(ratemode.rateModel.TimeSettingList[0].beginTime.Split(':')[1]));

            DateTime rateOutCarTime = outCarTime.AddHours(-int.Parse(ratemode.rateModel.TimeSettingList[0].beginTime.Split(':')[0])).
                AddMinutes(-int.Parse(ratemode.rateModel.TimeSettingList[0].beginTime.Split(':')[1]));

            TimeSpan ts = rateOutCarTime.Date.Subtract(rateInCarTime.Date);
            int days = ts.Days - 1;
            if (days > 0)
                rateclaculatemodel.CalculateMoney = ProcessDayMaxMoney(cartype, days, ratemode, MonthyType);
            rateclaculatemodel.ParkTime = (outCarTime - inCarTime).TotalMinutes;    //停车时长

            if (MonthyType == "MonthyAll")
            {
                rateclaculatemodel.CalculateMoney = 0;
                return rateclaculatemodel;
            }

            if (MonthyType != "MonthyMorning")
                rateclaculatemodel.CalculateMoney += ProcessCarTime(ratemode.rateModel, cartype, 0, rateInCarTime, rateOutCarTime, days, ratemode.rateModel.TimeSettingList[0].beginTime);   //早班

            if (MonthyType != "MonthyMiddle")
                rateclaculatemodel.CalculateMoney += ProcessCarTime(ratemode.rateModel1, cartype, 0, rateInCarTime, rateOutCarTime, days, ratemode.rateModel.TimeSettingList[0].beginTime);  //中班

            if (MonthyType != "MonthyNight")
                rateclaculatemodel.CalculateMoney += ProcessCarTime(ratemode.rateModel2, cartype, 0, rateInCarTime, rateOutCarTime, days, ratemode.rateModel.TimeSettingList[0].beginTime);  //晚班

            if (rateOutCarTime.Date > rateInCarTime.Date)
            {
                if (MonthyType != "MonthyMorning")
                    rateclaculatemodel.CalculateMoney += ProcessCarTime(ratemode.rateModel, cartype, 1, rateInCarTime, rateOutCarTime, days, ratemode.rateModel.TimeSettingList[0].beginTime);
                if (MonthyType != "MonthyMiddle")
                    rateclaculatemodel.CalculateMoney += ProcessCarTime(ratemode.rateModel1, cartype, 1, rateInCarTime, rateOutCarTime, days, ratemode.rateModel.TimeSettingList[0].beginTime);
                if (MonthyType != "MonthyNight")
                    rateclaculatemodel.CalculateMoney += ProcessCarTime(ratemode.rateModel2, cartype, 1, rateInCarTime, rateOutCarTime, days, ratemode.rateModel.TimeSettingList[0].beginTime);
            }
            return rateclaculatemodel;
        }

        /// <summary>
        /// 计算天最大金额
        /// </summary>
        /// <param name="CarType"></param>
        /// <param name="Days"></param>
        /// <param name="ratemode"></param>
        /// <param name="MonthyType"></param>
        /// <returns></returns>
        private decimal ProcessDayMaxMoney(int CarType, int Days, RateMode ratemode, string MonthyType)
        {
            decimal DayMaxMoney = 0;

            if (MonthyType == "MonthyAll")
                return DayMaxMoney;

            if (MonthyType != "MonthyMorning")
            {
                foreach (CarRateModel carfee in ratemode.rateModel.CarRateList)         //早班费率
                {
                    if (CarType == int.Parse(carfee.CarType))
                    {
                        if (ratemode.rateModel.TimeSettingList[0].RateMethod == "1")    // 按次
                        {
                            DayMaxMoney += (int)carfee.OnceMaxMoney;
                            break;
                        }
                        if (ratemode.rateModel.TimeSettingList[0].RateMethod == "0")    // 按时间
                        {
                            DayMaxMoney += (int)carfee.DayMaxMoney;
                            break;
                        }
                    }
                }
            }

            if (MonthyType != "MonthyMiddle")
            {
                if (ratemode.rateModel1 != null && ratemode.rateModel1.CarRateList != null && ratemode.rateModel1.IsActive)//中班费率
                {
                    foreach (CarRateModel carfee in ratemode.rateModel1.CarRateList)
                    {
                        if (CarType == int.Parse(carfee.CarType))
                        {
                            if (ratemode.rateModel1.TimeSettingList[0].RateMethod == "1")   //按次
                            {
                                DayMaxMoney += (int)carfee.OnceMaxMoney;
                                break;
                            }
                            if (ratemode.rateModel1.TimeSettingList[0].RateMethod == "0")   //按时间
                            {
                                DayMaxMoney += (int)carfee.DayMaxMoney;
                                break;
                            }
                        }
                    }
                }
            }

            if (MonthyType != "MonthyNight")
            {
                if (ratemode.rateModel2 != null && ratemode.rateModel2.CarRateList != null && ratemode.rateModel2.IsActive)    // 晚班费率
                {
                    foreach (CarRateModel carfee in ratemode.rateModel2.CarRateList)
                    {
                        if (CarType == int.Parse(carfee.CarType))
                        {
                            if (ratemode.rateModel2.TimeSettingList[0].RateMethod == "1")       // 按次
                            {
                                DayMaxMoney += (int)carfee.OnceMaxMoney;
                                break;
                            }
                            if (ratemode.rateModel2.TimeSettingList[0].RateMethod == "0")  // 按时间
                            {
                                DayMaxMoney += (int)carfee.DayMaxMoney;
                                break;
                            }
                        }
                    }
                }
            }
            return DayMaxMoney * Days;
        }

        /// <summary>
        /// 处理时间
        /// </summary>
        /// <param name="rateModel"></param>
        /// <param name="CarType"></param>
        /// <param name="Type"></param>
        /// <param name="rateInCarTime"></param>
        /// <param name="rateOutCarTime"></param>
        /// <param name="days"></param>
        /// <param name="temprate"></param>
        /// <returns></returns>
        private decimal ProcessCarTime(RateModel rateModel, int CarType, int Type, DateTime rateInCarTime, DateTime rateOutCarTime, int days, string temprate)
        {

            DateTime endTime = rateOutCarTime;
            DateTime beginTime = rateInCarTime;
            DateTime tempTime = rateInCarTime;

            if (rateModel == null)
                return 0;

            if (rateModel.CarRateList == null)
                return 0;
            if (Type == 1 && days < 0)
            {
                return 0;
            }

            ParkTime parktime = new ParkTime();


            if (days >= 0)
            {
                if (Type == 0)
                { // 进场时间
                    endTime = DateTime.Parse(beginTime.ToString("yyyy-MM-dd 23:59:59"));
                    tempTime = beginTime;
                }
                else
                { // 出场时间
                    beginTime = DateTime.Parse(endTime.ToString("yyyy-MM-dd 00:00:00"));
                    tempTime = endTime;
                }
            }


            parktime.beginTime = DateTime.Parse(tempTime.ToString("yyyy-MM-dd " + rateModel.TimeSettingList[0].beginTime + ":00"));
            parktime.beginTime = parktime.beginTime.
                AddHours(-int.Parse(temprate.Split(':')[0])).
                AddMinutes(-int.Parse(temprate.Split(':')[1]));

            parktime.endTime = DateTime.Parse(tempTime.ToString("yyyy-MM-dd " + rateModel.TimeSettingList[0].endTime + ":00"));
            parktime.endTime = parktime.endTime.
                AddHours(-int.Parse(temprate.Split(':')[0])).
                AddMinutes(-int.Parse(temprate.Split(':')[1]));




            if (parktime.endTime < parktime.beginTime)
            {
                parktime.endTime = parktime.endTime.AddDays(1);
            }

            if (endTime <= parktime.beginTime)// 如果出场时间小于费率开始时间
                return 0;


            if (parktime.endTime <= beginTime && parktime.endTime > parktime.beginTime)
            {
                return 0;
            }


            if (beginTime > parktime.beginTime)// 如果停车时间大于费率开始时间
            {
                parktime.beginTime = beginTime;
            }

            if (endTime < parktime.endTime)
            {
                parktime.endTime = endTime;
            }

            parktime.timeTotal = (parktime.endTime - parktime.beginTime).TotalMinutes;


            if (rateModel.TimeSettingList[0].RateMethod == "1")// 按次收
            {
                foreach (CarRateModel carfee in rateModel.CarRateList)
                {
                    if (CarType == int.Parse(carfee.CarType))
                    {
                        if (int.Parse(carfee.FreeTime) >= parktime.timeTotal)
                            parktime.parkMoney = 0;
                        else
                            parktime.parkMoney = carfee.CarFeeScaleList[0].RateMoney;
                        break;
                    }
                }
                return parktime.parkMoney;
            }

            foreach (CarRateModel carfee in rateModel.CarRateList)
            {
                if (CarType == int.Parse(carfee.CarType))//车辆类型
                {
                    var CarTimeQuantumListOrder = carfee.CarTimeQuantumList.OrderBy(a => a.beginTime);
                    double freeTime = double.Parse(carfee.FreeTime);
                    if (parktime.timeTotal <= freeTime)// 如果收费时间总和小于免费时间 收费金额为0
                    {
                        parktime.parkMoney = 0;
                        return parktime.parkMoney;
                    }

                    if (carfee.ContentFreeTimeFlag == false && parktime.timeTotal > freeTime && freecount == 0)// 免费时间是否包含在收费时间段内
                    {
                        parktime.timeTotal -= freeTime;
                        freecount++;
                    }


                    for (int i = 0; i < carfee.CarTimeQuantumList.Count; i++)
                    {// 获取时间段的长度

                        if (carfee.CarTimeQuantumList[i].RateMethod == "0")
                        {// 判断时间段收费方式是分钟还是小时:
                         // 0分钟，1小时
                            if (parktime.timeTotal >= carfee.CarTimeQuantumList[i].beginTime
                                    && parktime.timeTotal <= carfee.CarTimeQuantumList[i].endTime)
                            {// 获取时间段的结束时间
                                for (int m = 0; m <= i; m++)
                                {
                                    parktime.parkMoney += decimal.Parse(carfee.CarTimeQuantumList[m].TimeQuantumMoney);
                                }
                            }
                            else
                            {
                                // 超过时间段，xx元一分钟
                                decimal parktimeminutes = (decimal)(parktime.timeTotal
                                        - carfee.CarTimeQuantumList[carfee.CarTimeQuantumList.Count - 1].endTime);
                                for (int m = 0; m < carfee.CarTimeQuantumList.Count; m++)
                                {
                                    parktime.parkMoney += decimal.Parse(carfee.CarTimeQuantumList[m].TimeQuantumMoney);
                                }

                                if (carfee.CarFeeScaleList.Count > 0)
                                {
                                    parktime.parkMoney += Math.Ceiling(parktimeminutes / (int.Parse(carfee.CarFeeScaleList[0].RateTime)))
                                            * carfee.CarFeeScaleList[0].RateMoney;
                                }
                            }
                        }
                        else
                        {

                            if (parktime.timeTotal / 60 >= carfee.CarTimeQuantumList[i].beginTime / 60
                                    && parktime.timeTotal / 60 <= carfee.CarTimeQuantumList[i].endTime / 60)
                            {
                                for (int m = 0; m <= i; m++)
                                {
                                    parktime.parkMoney += decimal.Parse(carfee.CarTimeQuantumList[m].TimeQuantumMoney);
                                }
                            }
                            else
                            {
                                // 超过时间段，xx元一小时
                                double parktimeminutes = (parktime.timeTotal
                                        - carfee.CarTimeQuantumList[carfee.CarTimeQuantumList.Count - 1].endTime);
                                double hours = parktimeminutes % 60 == 0 ? parktimeminutes / 60
                                        : Math.Ceiling(parktimeminutes / 60);
                                for (int m = 0; m < carfee.CarTimeQuantumList.Count; m++)
                                {
                                    parktime.parkMoney += decimal.Parse(carfee.CarTimeQuantumList[m].TimeQuantumMoney);
                                }
                                if (carfee.CarTimeQuantumList[0].TimeQuantumMoney != null)
                                {
                                    parktime.parkMoney += (decimal)hours
                                            * decimal.Parse(carfee.CarTimeQuantumList[0].TimeQuantumMoney);
                                }
                            }
                        }
                    }
                }
            }
            decimal DayMaxMoney = 0;
            foreach (CarRateModel carfee in rateModel.CarRateList)
            {
                if (CarType == int.Parse(carfee.CarType))
                {
                    if (rateModel.TimeSettingList[0].RateMethod == "1")// 按次
                    {
                        DayMaxMoney += (int)carfee.OnceMaxMoney;
                        break;
                    }
                    if (rateModel.TimeSettingList[0].RateMethod == "0")// 按时间
                    {
                        DayMaxMoney += (int)carfee.DayMaxMoney;
                        break;
                    }
                }
            }

            if (parktime.parkMoney > DayMaxMoney)
                return DayMaxMoney;
            return parktime.parkMoney;
        }
    }
}
