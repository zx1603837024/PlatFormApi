using F2.Application.PDA.Dtos;
using F2.Core.Extensions;
using F2.Core.Extensions.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace F2.Application.PDA
{
    /// <summary>
    /// 
    /// </summary>
    public class EmployeeAppService : IEmployeeAppService
    {
        #region Var
        private readonly ITenantAppService _tenantAppService;
        private readonly IPDAAppService _pdaAppService;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public EmployeeAppService()
        {
            _tenantAppService = new TenantAppService();
            _pdaAppService = new PDAAppService();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public AbpLoginResult EmployeeLogin(EmployeeLoginInput input, int tenantId)
        {
            if (SqlHelper.ExecuteScalar(SqlHelper.connectionString, System.Data.CommandType.Text, "if exists (select IsActive from AbpEquipments with(nolock) where IsDeleted = 0 and PDA = '" + input.DeviceCode + "' and IsActive = 1) begin update AbpEquipments set DeviceType = " + input.DeviceType + " where PDA = '" + input.DeviceCode + "' select 1 end else begin if not exists(select 1 from AbpEquipments with(nolock) where PDA = '" + input.DeviceCode + "') begin insert into AbpEquipments(PDA, Type, DeviceType, UseStatus, IsActive, IsDeleted, CreationTime, tenantId, IsUpgrade, SD, Printers, Standard, GPS, Scan, CompanyId, Remark) values ('" + input.DeviceCode + "', 1, " + input.DeviceType + ", 0, 0, 0, GETDATE(), " + tenantId + ", 0,  0, 0, 0, 1, 0, 0, '系统自动注册设备') end  else  begin   update AbpEquipments set DeviceType = " + input.DeviceType + " where PDA = '" + input.DeviceCode + "'  end end") == null)
                return new AbpLoginResult(AbpLoginResultType.EquipmentIsNotActive);

            string sql = "select top 1 * from AbpEmployees where TenantId = @TenantId and UserName = @UserName and Password = @Password";
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

            if (SettingStoreAppService.GetSettingOrNull(tenantId, null, "PDABindUser").Value == "True")
            {
                if (SqlHelper.ExecuteScalar(SqlHelper.connectionString, System.Data.CommandType.Text, "select 1 from AbpEquipments with(nolock) where PDA = '" + input.DeviceCode + "' and EmployeeId = " + list[0].Id) == null)
                    return new AbpLoginResult(AbpLoginResultType.PDAbindUser);
            }
            return new AbpLoginResult(list[0]);
        }

        private EmployeeDto GetEmployeeInfo(string username, string password, int tenantId)
        {
            string sql = "select top 1 * from AbpEmployees where TenantId = @TenantId and UserName = @UserName and Password = @Password";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@TenantId", tenantId),
                new SqlParameter("@UserName", username),
                new SqlParameter("@Password", password)
            };
            return DataProcessHelper.GetEntityFromTable<EmployeeDto>(SqlHelper.ExecuteDataTable(System.Data.CommandType.Text, sql, param))[0];
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

            var berthsecs = _pdaAppService.GetBerthsecList(model);

            var employee = GetEmployeeInfo(input.userName, input.password, berthsecs[0].TenantId);
            model.EmployeeId = employee.Id;
            model.CompanyId = berthsecs[0].CompanyId;
            model.TenantId = berthsecs[0].TenantId;
            model.Version = Version;
            foreach (var v in berthsecs)
            {
                model.ParkIds = model.ParkIds + v.ParkId + ",";
                model.RegionIds = model.RegionIds + v.RegionId + ",";
            }
            model.DeviceCode = DeviceCode;
            string sql = "insert into AbpUserLoginToken(Token, TenantId, CompanyId, RegionIds, ParkIds, DeviceCode, BerthsecIds, EmployeeId, Version, CreateTime) values(@Token, @TenantId, @CompanyId, @RegionIds, @ParkIds, @DeviceCode, @BerthsecIds, @EmployeeId, @Version, getdate()) ";

            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@Token", model.Token),
                new SqlParameter("@BerthsecIds", model.BerthsecIds),
                new SqlParameter("@DeviceCode", model.DeviceCode),
                new SqlParameter("@TenantId", model.TenantId),
                new SqlParameter("@CompanyId", model.CompanyId),
                new SqlParameter("@RegionIds", model.RegionIds),
                new SqlParameter("@ParkIds", model.ParkIds),
                new SqlParameter("@EmployeeId", model.EmployeeId),
                new SqlParameter("@Version", model.Version)
            };
            SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.Text, sql, param);//写入在线登录信息

            string temp = DateTime.Now.ToString("yyyyMMdd") + employee.UserName;
            if (!string.IsNullOrWhiteSpace(employee.BatchNo) && employee.BatchNo.Contains(temp))
            {
                employee.BatchNo = temp + (int.Parse(employee.BatchNo.Replace(temp, "")) + 1).ToString().PadLeft(2, '0');
            }
            else
            {
                employee.BatchNo = temp + "01";
            }

            param = new SqlParameter[] {
                new SqlParameter("@berthsecId", model.BerthsecIds),
                new SqlParameter("@DeviceCode", DeviceCode),
                new SqlParameter("@employeeID", model.EmployeeId),
                new SqlParameter("@TenantID", model.TenantId),
                new SqlParameter("@checkInOrOutTime", DateTime.Now),
                new SqlParameter("@CompanyId", model.CompanyId),
                new SqlParameter("@ParkID", model.ParkIds),
                new SqlParameter("@VersionNum", model.Version),
                new SqlParameter("@BatchNo",employee.BatchNo)
            };
            SqlHelper.ExecuteNonQuery(SqlHelper.connectionString, CommandType.StoredProcedure, "Pro_Employee_CheckinV1", param);//收费员
            return new LoginModel() { access_token = model.Token, expires_in = "30", refresh_token = model.Token, token_type = "app" };
        }
    }
}
