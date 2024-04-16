using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Core.Extensions
{
    public static class P2Consts
    {
        
    }

    /// <summary>
    /// 登录枚举
    /// </summary>
    public enum AbpLoginResultType
    {
        Success = 1,

        InvalidUserNameOrEmailAddress,

        InvalidPassword,

        UserIsNotActive,

        EquipmentIsNotActive,

        EmployeeIsCheck,

        InvalidTenancyName,

        TenantIsNotActive,

        UserEmailIsNotConfirmed,

        UnknownExternalLogin,

        PDAbindUser
    }
}
