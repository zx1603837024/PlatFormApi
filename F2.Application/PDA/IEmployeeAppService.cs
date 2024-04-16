using F2.Application.PDA.Dtos;
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
    public interface IEmployeeAppService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        AbpLoginResult EmployeeLogin(EmployeeLoginInput input, int tenantId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        LoginModel LoginToken(LoginTokenInput input);
    }
}
