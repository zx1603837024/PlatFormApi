using F2.Application.Parking.Dtos;
using F2.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using F2.Application.WeChat.Dtos;
using F2.Application.Parking.ZhiBo;
using System.Configuration;
using F2.Core.Extensions.DataExtend;

namespace F2.Application.Parking
{
    public class ParkingService:IParkingService
    {
      

        /// <summary>
        /// 包月
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool SaveMonthlyRent(SaveMonthlyRentInput input)
        {
            var queryWeChatUser = $"SELECT * FROM dbo.WeixinTuser WHERE openId=@openId AND TenantId=@TenantId";
            
            //获取微信用户
            var weChatUser = SQLFactory.GetEntityFromSQL<WeixinTuserDto>(queryWeChatUser, new SqlParameter[]
                        {
                              new SqlParameter("@openId", input.openId),
                              new SqlParameter("@TenantId", input.tenantId),
                        })?.FirstOrDefault();
            if (weChatUser == null)
                throw new Exception("微信用户不存在");
            var parkQuery = $"SELECT * FROM dbo.AbpParks WITH(NOLOCK) WHERE IsDeleted=0 AND Id=" + input.parkId;
            var park = SQLFactory.GetEntityFromSQL<AbpParksDto>(parkQuery)?.FirstOrDefault();
            if (park == null)
            {
                throw new Exception($"{input.parkId}不存在");
            }
            if (string.IsNullOrWhiteSpace(park.ZhiBoParkId))
            {
                throw new Exception($"{input.parkId}不存在");
            }
            DateTime BeginTime = input.beginDate==DateTime.MinValue?DateTime.Today:input.beginDate;
            DateTime EndTime = BeginTime.AddDays(input.days);

            var findSQL = @"SELECT TOP(1) * FROM AbpParkingMonthlyRent WHERE PlateNumber=@PlateNumber AND IsDeleted=0  AND ParkingId=@ParkingId";
            var find = SQLFactory.GetEntityFromSQL<AbpParkingMonthlyRentDto>(findSQL,new SqlParameter[] 
                        {
                              new SqlParameter("@PlateNumber", input.plateNumber),
                              new SqlParameter("@ParkingId", park.ZhiBoParkId),
                        })?.FirstOrDefault();

            if (find != null)
            {
                BeginTime = input.beginDate == DateTime.MinValue ? (find.EndTime>= DateTime.Today ?find.BeginTime:DateTime.Today):input.beginDate;//如果大于今天则说明已过期重新开通，否则是续费
                EndTime = find.EndTime.AddDays(input.days);
            }

            Func<SaveMonthlyRentInput.ECarType, string> ConvertCarModelId = (cartype) =>
            {
                switch (cartype)
                {
                    case SaveMonthlyRentInput.ECarType.小车:
                    default:
                        return ConfigurationManager.AppSettings["ZhiBoSmallCar"];
                    case SaveMonthlyRentInput.ECarType.大车:
                        return ConfigurationManager.AppSettings["ZhiBoBigCar"];
                    case SaveMonthlyRentInput.ECarType.摩托车:
                        return ConfigurationManager.AppSettings["ZhiBoMoto"];
                    case SaveMonthlyRentInput.ECarType.超大车:
                        return ConfigurationManager.AppSettings["ZhiBoLargeCar"];
                    case SaveMonthlyRentInput.ECarType.小型新能源车:
                        return ConfigurationManager.AppSettings["ZhiBoSmallEnergyCar"];
                    case SaveMonthlyRentInput.ECarType.大型新能源车:
                        return ConfigurationManager.AppSettings["ZhiBoBigEnergyCar"];
                }
            };

            var request = new SaveMonthlyRentRequest
            {
                park_id = park.ZhiBoParkId,
                emp_name = string.IsNullOrWhiteSpace(weChatUser.nickName) ? weChatUser.tel : weChatUser.nickName,
                emp_moblie = weChatUser.tel,
                plate_number = input.plateNumber,
                cartypeid = ConfigurationManager.AppSettings["ZhiBoCarTypeId"],
                carmodelid = ConvertCarModelId(input.carType),
                begindate = BeginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                enddate = EndTime.ToString("yyyy-MM-dd HH:mm:ss")
            };


            if (find == null)
            {
                var insertSQL = @"INSERT INTO dbo.AbpParkingMonthlyRent(PlateNumber, OpenId, ParkId, ParkingId, BeginTime, EndTime, CreationTime, IsDeleted,TenantId,CompanyId,CarType)
VALUES(@PlateNumber, @OpenId,@ParkId,@ParkingId,@BeginTime,@EndTime,GETDATE(),0,@TenantId,@CompanyId,@CarType)
SELECT SCOPE_IDENTITY()";
                SqlParameter[] insertParam = new SqlParameter[] {
                    new SqlParameter("@PlateNumber", input.plateNumber),
                    new SqlParameter("@OpenId", input.openId),
                    new SqlParameter("@ParkId", input.parkId),
                    new SqlParameter("@ParkingId", park.ZhiBoParkId),
                    new SqlParameter("@BeginTime", BeginTime),
                    new SqlParameter("@EndTime", EndTime),
                    new SqlParameter("@TenantId", park.TenantId),
                    new SqlParameter("@CompanyId", park.CompanyId),
                    new SqlParameter("@CarType", (int)input.carType),
                };
                var res = WebApiHelper.PostWebApi<SaveMonthlyRentResponse>(ConfigurationManager.AppSettings["ZhiBoHost"] + "zhiboyunting/interface/saveMonthlyRent", request.ToJson());
                if (res.status == 0)
                {
                    throw new Exception(res.message);
                }
                //插入数据库
                var insertId = SQLFactory.ExecuteScalar<int>(insertSQL, insertParam);
                return true;
            }
            else
            {
                var res = WebApiHelper.PostWebApi<SaveMonthlyRentResponse>(ConfigurationManager.AppSettings["ZhiBoHost"] + "zhiboyunting/interface/saveMonthlyRent", request.ToJson());
                if (res.status == 0)
                {
                    throw new Exception(res.message);
                }

                var updateSQL = @"UPDATE dbo.AbpParkingMonthlyRent SET BeginTime=@BeginTime, EndTime= @EndTime WHERE Id=" + find.Id;
                SQLFactory.ExecuteNonQuery(updateSQL,new SqlParameter[] {
                   new SqlParameter("@EndTime", EndTime),
                   new SqlParameter("@BeginTime", BeginTime),
                });
                return true;
            }
        }

        /// <summary>
        /// 编辑月租
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool ModifyMonthlyRent(ModifyMonthlyRentInput input)
        {
            var queryWeChatUser = $"SELECT * FROM dbo.WeixinTuser WHERE openId=@openId AND TenantId=@TenantId";
            //获取微信用户
            var weChatUser = SQLFactory.GetEntityFromSQL<WeixinTuserDto>(queryWeChatUser, new SqlParameter[]
                    {
                              new SqlParameter("@openId", input.openId),
                              new SqlParameter("@TenantId", input.tenantId),
                    })?.FirstOrDefault();
            if (weChatUser == null)
                throw new Exception("微信用户不存在");
            var parkQuery = $"SELECT * FROM dbo.AbpParks WITH(NOLOCK) WHERE IsDeleted=0 AND Id=" + input.parkId;
            var park = SQLFactory.GetEntityFromSQL<AbpParksDto>(parkQuery)?.FirstOrDefault();
            if (park == null)
            {
                throw new Exception($"{input.parkId}不存在");
            }
            if (string.IsNullOrWhiteSpace(park.ZhiBoParkId))
            {
                throw new Exception($"{input.parkId}不存在");
            }
            DateTime BeginTime = input.beginDate;
            DateTime EndTime = input.endDate;

            var findSQL = @"SELECT TOP(1) * FROM AbpParkingMonthlyRent WHERE PlateNumber=@PlateNumber AND IsDeleted=0  AND ParkingId=@ParkingId";
            var find = SQLFactory.GetEntityFromSQL<AbpParkingMonthlyRentDto>(findSQL, new SqlParameter[]
                        {
                              new SqlParameter("@PlateNumber", input.plateNumber),
                              new SqlParameter("@ParkingId", park.ZhiBoParkId),
                        })?.FirstOrDefault();

            Func<SaveMonthlyRentInput.ECarType, string> ConvertCarModelId = (cartype) =>
            {
                switch (cartype)
                {
                    case SaveMonthlyRentInput.ECarType.小车:
                    default:
                        return ConfigurationManager.AppSettings["ZhiBoSmallCar"];
                    case SaveMonthlyRentInput.ECarType.大车:
                        return ConfigurationManager.AppSettings["ZhiBoBigCar"];
                    case SaveMonthlyRentInput.ECarType.摩托车:
                        return ConfigurationManager.AppSettings["ZhiBoMoto"];
                    case SaveMonthlyRentInput.ECarType.超大车:
                        return ConfigurationManager.AppSettings["ZhiBoLargeCar"];
                    case SaveMonthlyRentInput.ECarType.小型新能源车:
                        return ConfigurationManager.AppSettings["ZhiBoSmallEnergyCar"];
                    case SaveMonthlyRentInput.ECarType.大型新能源车:
                        return ConfigurationManager.AppSettings["ZhiBoBigEnergyCar"];
                }
            };

            var request = new SaveMonthlyRentRequest
            {
                park_id = park.ZhiBoParkId,
                emp_name = string.IsNullOrWhiteSpace(weChatUser.nickName) ? weChatUser.tel : weChatUser.nickName,
                emp_moblie = weChatUser.tel,
                plate_number = input.plateNumber,
                cartypeid = ConfigurationManager.AppSettings["ZhiBoCarTypeId"],
                carmodelid = ConvertCarModelId(input.carType),
                begindate = BeginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                enddate = EndTime.ToString("yyyy-MM-dd HH:mm:ss")
            };

            if (find == null)
            {
                var insertSQL = @"INSERT INTO dbo.AbpParkingMonthlyRent(PlateNumber, OpenId, ParkId, ParkingId, BeginTime, EndTime, CreationTime, IsDeleted,TenantId,CompanyId,CarType)
VALUES(@PlateNumber, @OpenId,@ParkId,@ParkingId,@BeginTime,@EndTime,GETDATE(),0,@TenantId,@CompanyId,@CarType)
SELECT SCOPE_IDENTITY()";
                SqlParameter[] insertParam = new SqlParameter[] {
                    new SqlParameter("@PlateNumber", input.plateNumber),
                    new SqlParameter("@OpenId", input.openId),
                    new SqlParameter("@ParkId", input.parkId),
                    new SqlParameter("@ParkingId", park.ZhiBoParkId),
                    new SqlParameter("@BeginTime", BeginTime),
                    new SqlParameter("@EndTime", EndTime),
                    new SqlParameter("@TenantId", park.TenantId),
                    new SqlParameter("@CompanyId", park.CompanyId),
                    new SqlParameter("@CarType", (int)input.carType),
                };
                var res = WebApiHelper.PostWebApi<SaveMonthlyRentResponse>(ConfigurationManager.AppSettings["ZhiBoHost"] + "zhiboyunting/interface/saveMonthlyRent", request.ToJson());
                if (res.status == 0)
                {
                    throw new Exception(res.message);
                }
                //插入数据库
                var insertId = SQLFactory.ExecuteScalar<int>(insertSQL, insertParam);
                return true;
            }
            else
            {
                var res = WebApiHelper.PostWebApi<SaveMonthlyRentResponse>(ConfigurationManager.AppSettings["ZhiBoHost"] + "zhiboyunting/interface/saveMonthlyRent", request.ToJson());
                if (res.status == 0)
                {
                    throw new Exception(res.message);
                }

                var updateSQL = @"UPDATE dbo.AbpParkingMonthlyRent SET BeginTime=@BeginTime, EndTime= @EndTime WHERE Id=" + find.Id;
                SQLFactory.ExecuteNonQuery(updateSQL, new SqlParameter[] {
                   new SqlParameter("@EndTime", EndTime),
                   new SqlParameter("@BeginTime", BeginTime),
                });
                return true;
            }
        }

        /// <summary>
        /// 删除月租
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool DelMonthlyRent(DelMonthlyRentInput input)
        {
            var findSQL = @"SELECT TOP(1) a.* FROM AbpParkingMonthlyRent a INNER JOIN dbo.AbpParks  b ON a.ParkId = b.Id AND b.IsDeleted=0 AND a.IsDeleted=0
                            WHERE a.PlateNumber=@PlateNumber AND a.OpenId=@OpenId AND b.Id=@ParkId";
            var find = SQLFactory.GetEntityFromSQL<AbpParkingMonthlyRentDto>(findSQL, new SqlParameter[]
                        {
                              new SqlParameter("@PlateNumber", input.plateNumber),
                              new SqlParameter("@OpenId", input.openId),
                              new SqlParameter("@ParkId",input.parkId)
                        })?.FirstOrDefault();

            if (find == null)
            {
                return true;
            }
            var parkQuery = $"SELECT * FROM dbo.AbpParks WITH(NOLOCK) WHERE IsDeleted=0 AND Id=" + input.parkId;
            var park = SQLFactory.GetEntityFromSQL<AbpParksDto>(parkQuery)?.FirstOrDefault();
            if (park == null)
            {
                throw new Exception($"{input.parkId}不存在");
            }
            if (string.IsNullOrWhiteSpace(park.ZhiBoParkId))
            {
                throw new Exception($"{input.parkId}不存在");
            }
            if (find == null) { return true; }
            DelMonthlyRentRequest del = new DelMonthlyRentRequest
            {
                plate_number = input.plateNumber,
                park_id = park.ZhiBoParkId
            };
            var delRes = WebApiHelper.PostWebApi<DelMonthlyRentResponse>(ConfigurationManager.AppSettings["ZhiBoHost"] + "zhiboyunting/interface/delMonthlyRent", del.ToJson());
            if (delRes.status == 0)
            {
                throw new Exception(delRes.message);
            }
            var updateSql = "update AbpParkingMonthlyRent SET DeletionTime=GETDATE(),IsDeleted =1 where id= " + find.Id;
            SQLFactory.ExecuteNonQuery(updateSql);
            return true;
        }

        /// <summary>
        /// 扫码入场
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool RemoteOpen(RemoteOpenInput input)
        {
            var request = new NoPlateQRcodeRequest
            {
                park_id = input.parkingId,
                lane_no = input.channelId,
                open_id = input.plateNumber.IsNullOrWhiteSpace() ? input.openId : input.plateNumber,
            };
            var res = WebApiHelper.PostWebApi<NoPlateQRcodeResponse>(ConfigurationManager.AppSettings["ZhiBoHost"] + "zhiboyunting/interface/noPlateQRcode", request.ToJson());
            if (res.status == 0)
            {
                throw new Exception(res.message);
            }
            var insertSql = @"INSERT INTO dbo.AbpRemoteOpenLog(PlateNumber, OpenId, ParkingId, CreationTime,IsCarIn,IsCarOut)
VALUES(@PlateNumber,@OpenId,@ParkingId,GETDATE(),0,0) ";
            SQLFactory.ExecuteNonQuery(insertSql, new SqlParameter[] {
                   new SqlParameter("@PlateNumber", input.plateNumber),
                   new SqlParameter("@OpenId", input.openId),
                   new SqlParameter("@ParkingId", input.parkingId)
                });
            return true;
        }

        /// <summary>
        /// 获取停车订单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public GetParkingBillOutPut GetParkingBill(GetParkingBillInput input)
        {
            //先根据openid查询停车记录，查询到了说明是临停
            var queryRecord = @"SELECT top(1) * FROM dbo.AbpParkingRecord WHERE PlateNumber=@PlateNumber AND tenantId=@tenantId AND ParkingId=@ParkingId AND OrderStatus=1 AND IsDeleted=0  ORDER BY CarInTime DESC";
            var record = DataProcessHelper.GetEntityFromTable<AbpParkingRecordDto>(SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, queryRecord, new SqlParameter[] {
                  new SqlParameter("@PlateNumber", input.openId),
                  new SqlParameter("@ParkingId", input.parkingId),
                  new SqlParameter("@tenantId", input.tenantId),
            }))?.FirstOrDefault();

            GetParkingBillOutPut result = new GetParkingBillOutPut
            {
                parkingId = record.ParkingId,
                recordNo = record.RecordId,
                carInTimeStr = record.CarInTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                plateNumber = record.PlateNumber
            };

            var queryWeChatUser = $"SELECT * FROM dbo.WeixinTuser WHERE openId=@openId AND TenantId=@TenantId";
            //获取微信用户
            var weChatUser = DataProcessHelper.GetEntityFromTable<WeixinTuserDto>(SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, queryWeChatUser, new SqlParameter[]
                        {
                              new SqlParameter("@openId", input.openId),
                              new SqlParameter("@TenantId", input.tenantId),
                        }))?.FirstOrDefault();
            result.nickName = ((weChatUser?.nickName.IsNullOrWhiteSpace() ?? false) ? weChatUser?.tel : weChatUser?.nickName) ?? string.Empty;

            //未找到说明道闸识别进入
            if (record == null)
            {
                var request = new NoPlateQRcodeRequest()
                {
                    open_id = input.openId,
                    lane_no = input.channelId,
                    park_id=input.parkingId
                };
                var res = WebApiHelper.PostWebApi<NoPlateQRcodeResponse>(ConfigurationManager.AppSettings["ZhiBoHost"] + "zhiboyunting/interface/noPlateQRcode", request.ToJson());
                if (res.status == 0)
                {
                    throw new Exception(res.message);
                }
                result.plateNumber = res.JsonData.plate_number;
                result.billNo = res.JsonData.order_no;
                result.payableAmount = Convert.ToDecimal(res.JsonData.upay_fee);
            }
            else
            {
                //扫码记录
                var queryCarIn = @"SELECT TOP(1) * FROM dbo.AbpRemoteOpenLog WHERE OpenId=@OpenId AND ParkingId=@ParkingId AND IsCarIn=1 AND IsCarOut=0 ORDER BY CreationTime DESC";
                var carInLog = DataProcessHelper.GetEntityFromTable<AbpRemoteOpenLogDto>(SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, queryCarIn, new SqlParameter[] {
                                    new SqlParameter("@OpenId", input.openId),
                                    new SqlParameter("@ParkingId", input.parkingId),
                                }))?.FirstOrDefault();
                if (carInLog == null)
                {
                    throw new Exception("未查询到入场信息");
                }
                CalculatingTempCostNoIdsRequest request = new CalculatingTempCostNoIdsRequest { parking_id = input.parkingId, plate_num = record.PlateNumber };

                var res = WebApiHelper.PostWebApi<CalculatingTempCostNoIdsResponse>(ConfigurationManager.AppSettings["ZhiBoHost"] + "zhiboyunting/interface/calculatingTempCostNoIds", request.ToJson());
                if (res.status == 0)
                {
                    throw new Exception(res.message);
                }
                result.billNo = res.JsonData.order_no;
                result.payableAmount = Convert.ToDecimal(res.JsonData.upay_fee);
               
               var updateSql = @"update AbpRemoteOpenLog set RecordBillNo=@RecordBillNo where Id=" + carInLog.Id;
               //更新账单ID
               SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, updateSql, new SqlParameter[] {
                  new SqlParameter("@RecordBillNo", res.JsonData.order_no)
               });
            }
            return result;
        }

        /// <summary>
        /// 支付回调
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool CarOutPayCallBack(CarOutPayCallBackInput input)
        {
            var queryRecord = @"SELECT top(1) * FROM dbo.AbpParkingRecord WHERE RecordId=@RecordId AND IsDeleted=0  ORDER BY CarInTime DESC";
            var record = DataProcessHelper.GetEntityFromTable<AbpParkingRecordDto>(SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, queryRecord, new SqlParameter[] {
                  new SqlParameter("@RecordId", input.recordNo),
            }))?.FirstOrDefault();
            if (record == null)
            {
                throw new Exception("未查询到入场信息");
            }

            //var queryCarInLog = @"SELECT TOP(1) * FROM dbo.AbpRemoteOpenLog WHERE RecordId=@RecordId AND ParkingId=@ParkingId AND IsCarIn=1 AND IsCarOut=0 ORDER BY CreationTime DESC";
            //var carInLog = DataProcessHelper.GetEntityFromTable<AbpRemoteOpenLogDto>(SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, queryCarInLog, new SqlParameter[] {
            //      new SqlParameter("@RecordId", input.recordNo),
            //      new SqlParameter("@ParkingId", input.parkingId),
            //}))?.FirstOrDefault();
            //if (record == null)
            //{
            //    throw new Exception("未查询到入场信息");
            //}

            PayTempCostRequest request = new PayTempCostRequest
            {
                order_id = input.recordNo,
                order_no = input.billNo,
                park_id = input.parkingId,
                pay_fee = input.payFee.ToString(),
                plate_number = record.PlateNumber,
                pay_type = ((int)input.payType).ToString()
            };
            //开闸
            var res = WebApiHelper.PostWebApi<PayTempCostResponse>(ConfigurationManager.AppSettings["ZhiBoHost"] + "zhiboyunting/interface/payTempCost", request.ToJson());
            if (res.status == 0)
            {
                throw new Exception(res.message);
            }
            var updateSql = @"UPDATE dbo.AbpParkingRecord
SET PayableAmount=@PayableAmount, FactReceive=@FactReceive, DiscountMoney=@DiscountMoney, PayType=@PayType, PayTime=GETDATE(), PayStatus=@PayStatus
WHERE RecordId=@RecordId;";
            SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, updateSql, new SqlParameter[] {
                  new SqlParameter("@PayableAmount", input.payFee),
                  new SqlParameter("@FactReceive", input.payFee),
                  new SqlParameter("@DiscountMoney", 0m),
                  new SqlParameter("@PayType", (int)input.payType),
                  new SqlParameter("@PayStatus", 1),
                  new SqlParameter("@RecordId", input.recordNo),
               });

            return true;
        }




        #region 停车场记录推送
        public PushParingRecordOutput pushParingRecord(PushParingRecordInput input)
        {
            var res = new PushParingRecordOutput
            {
                status = EPushParingRecordOutputStatus.成功
            };
            #region 停车场驶入驶出
            try
            {
                //校验appkey

                if (input.is_exit == 1)
                {
                    pushParingRecord_CarIn(input);
                }
                else
                {
                    pushParingRecord_CarOut(input);
                }
            }
            catch(Exception ex)
            {
                res.status = EPushParingRecordOutputStatus.失败;
                res.message = ex.Message;
            }
            #endregion
            return res;
        }

        /// <summary>
        /// 车辆入场
        /// </summary>
        /// <param name="input"></param>
        private void pushParingRecord_CarIn(PushParingRecordInput input)
        {
            if (string.IsNullOrWhiteSpace(input.record_id))
            {
                throw new Exception("record_id为空");
            }
            var sql = $"SELECT TOP(1) 1 FROM AbpParkingRecord WITH(NOLOCK) WHERE RecordId='{input.record_id}'";
            var isExistGuid = SqlHelper.ExecuteScalar(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
            if (isExistGuid!=null && (int)isExistGuid == 1)
            {
                throw new Exception("record_id已存在,重复进场");
            }
            var parkQuery = $"SELECT * FROM dbo.AbpParks WITH(NOLOCK) WHERE IsDeleted=0 AND ZhiBoParkId=" +input.park_id;
            var parks = DataProcessHelper.GetEntityFromTable<AbpParksDto>(SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, parkQuery));
            if (parks == null || parks.Count==0)
            {
                throw new Exception("park_id不存在");
            }

            var channelQuery = $" SELECT * FROM AbpParkChannel WHERE ChannelDirection='1' and IsDeleted=0 AND ZhiBoChannelId =" +input.entrancegate_id;
            var channels = DataProcessHelper.GetEntityFromTable<AbpParkChannerlDto>(SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, channelQuery));
            if (channels == null || channels.Count == 0)
            {
                throw new Exception("entrancegate_id不存在");
            }


            //是否包月
            var isMonthlyCarSql = @"
SELECT TOP(1) 1
FROM dbo.AbpParkingMonthlyRent
WHERE IsDeleted=0 
  AND PlateNumber=@PlateNumber 
  AND BeginTime<=GETDATE()
  AND EndTime>=GETDATE() 
  AND ParkingId=@ParkingId
";
            SqlParameter[] MonthlyParam = new SqlParameter[] {
                new SqlParameter("@PlateNumber", input.plate_number),
                new SqlParameter("@ParkingId", parks[0].ZhiBoParkId),
            };

            ECarType carType = ECarType.临时车;

            var isMonthlyCar= SqlHelper.ExecuteScalar(SqlHelper.connectionString, System.Data.CommandType.Text, isMonthlyCarSql, MonthlyParam);
            if (isMonthlyCar != null && (int)isMonthlyCar == 1)
            {
                carType = ECarType.包月车;
            }

            var insertSql = $@"INSERT INTO dbo.AbpParkingRecord(Guid,RecordId, TenantId, CompanyId, ParkId,ParkingId, PlateNumber, ParkName, OrderStatus, CarInTime,CarInChannelId, CarInChannelName,CarType, PayStatus, IsDeleted , CreationTime,CarInRequest,RegionId)
VALUES(@Guid,@RecordId,@TenantId,@CompanyId,@ParkId,@ParkingId,@PlateNumber,@ParkName,@OrderStatus,@CarInTime,@CarInChannelId,@CarInChannelName,@CarType,@PayStatus,0,GETDATE(),@CarInRequest,@RegionId)";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@Guid", Guid.NewGuid()),
                new SqlParameter("@RecordId", input.record_id),
                new SqlParameter("@TenantId", parks[0].TenantId),
                new SqlParameter("@CompanyId", parks[0].CompanyId),
                new SqlParameter("@ParkId", parks[0].Id),
                new SqlParameter("@ParkingId", parks[0].ZhiBoParkId),
                new SqlParameter("@PlateNumber", input.plate_number),
                new SqlParameter("@ParkName", parks[0].ParkName),
                new SqlParameter("@OrderStatus",(int)EOrderStatus.停车中),
                new SqlParameter("@CarInTime",DateTime.Parse(input.entrance_time)),
                new SqlParameter("@CarInChannelId",channels[0].Id),
                new SqlParameter("@CarInChannelName",channels[0].ChannelName),
                new SqlParameter("@CarType",(int)carType),
                new SqlParameter("@PayStatus",0),
                new SqlParameter("@CarInRequest",JsonConvert.SerializeObject(input)),
                new SqlParameter("@RegionId",parks[0].RegionId)
            };

            SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, insertSql, param);

            var queryCarIn = @"SELECT TOP(1) * FROM dbo.AbpRemoteOpenLog WHERE PlateNumber=@PlateNumber AND ParkingId=@ParkingId AND IsCarIn=0 AND IsCarOut=0 ORDER BY CreationTime DESC";
            var record = DataProcessHelper.GetEntityFromTable<AbpRemoteOpenLogDto>(SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, queryCarIn, new SqlParameter[] {
                  new SqlParameter("@PlateNumber", input.plate_number),
                  new SqlParameter("@ParkingId", input.park_id),
            }))?.FirstOrDefault();
            if (record != null)
            {
                //临时车
                queryCarIn= @"SELECT TOP(1) * FROM dbo.AbpRemoteOpenLog WHERE OpenId=@OpenId AND ParkingId=@ParkingId AND IsCarIn=0 AND IsCarOut=0 ORDER BY CreationTime DESC";
                record = DataProcessHelper.GetEntityFromTable<AbpRemoteOpenLogDto>(SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, queryCarIn, new SqlParameter[] {
                  new SqlParameter("@OpenId", input.plate_number),
                  new SqlParameter("@ParkingId", input.park_id),
                }))?.FirstOrDefault();
                if (record != null)
                {
                    //更新扫码记录
                    var updateReccord = @"update AbpRemoteOpenLog set IsCarIn=1 where id =" + record.Id;
                    SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, updateReccord);
                }
            }

        }


        /// <summary>
        /// 车辆出场
        /// </summary>
        /// <param name="input"></param>
        private void pushParingRecord_CarOut(PushParingRecordInput input)
        {
            if (string.IsNullOrWhiteSpace(input.record_id))
            {
                throw new Exception("record_id为空");
            }
            var sql = $"SELECT TOP(1) ID FROM AbpParkingRecord WITH(NOLOCK) WHERE RecordId='{input.record_id}'";
            var isExistGuid = SqlHelper.ExecuteScalar(SqlHelper.connectionString, System.Data.CommandType.Text, sql);
            if (isExistGuid == null)
            {
                throw new Exception("record_id不存在");
            }
            var channelQuery = $" SELECT * FROM AbpParkChannel WHERE ChannelDirection='2' and IsDeleted=0 AND ZhiBoChannelId =" + input.exitgate_id;
            var channels = DataProcessHelper.GetEntityFromTable<AbpParkChannerlDto>(SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, channelQuery));
            if (channels == null || channels.Count == 0)
            {
                throw new Exception("exitgate_id不存在");
            }

            var updateSql = $@"UPDATE AbpParkingRecord 
                               SET CarOutTime=@CarOutTime,
                                   CarOutChannelId = @CarOutChannelId ,
                                   CarOutChannelName=@CarOutChannelName ,
                                   OrderStatus=@OrderStatus,
                                   StopTime=@StopTime,
                                   CarOutRequest = @CarOutRequest,
                                   LastModificationTime=GETDATE()
                               WHERE RecordId=@RecordId";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@RecordId", input.record_id),
                new SqlParameter("@CarOutTime",DateTime.Parse(input.exit_time)),
                new SqlParameter("@CarOutChannelId",channels[0].Id),
                new SqlParameter("@CarOutChannelName",channels[0].ChannelName),
                new SqlParameter("@OrderStatus",(int)EOrderStatus.已出单),
                new SqlParameter("@StopTime",Convert.ToInt32(Math.Ceiling((DateTime.Parse(input.exit_time)- DateTime.Parse(input.entrance_time)).TotalMinutes))),
                new SqlParameter("@CarOutRequest",JsonConvert.SerializeObject(input)),
            };
            SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, updateSql, param);

            var queryCarIn = @"SELECT TOP(1) * FROM dbo.AbpRemoteOpenLog WHERE PlateNumber=@PlateNumber AND ParkingId=@ParkingId AND IsCarIn=1 AND IsCarOut=0 ORDER BY CreationTime DESC";
            var record = DataProcessHelper.GetEntityFromTable<AbpRemoteOpenLogDto>(SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, queryCarIn, new SqlParameter[] {
                  new SqlParameter("@PlateNumber", input.plate_number),
                  new SqlParameter("@ParkingId", input.park_id),
            }))?.FirstOrDefault();
            if (record != null)
            {
                //更新扫码记录
                var updateReccord = @"update AbpRemoteOpenLog set IsCarOut=1 where id =" + record.Id;
                SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, System.Data.CommandType.Text, updateReccord);
            }

        }
        #endregion
    }
}
