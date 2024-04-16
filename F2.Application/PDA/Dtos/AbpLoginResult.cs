using F2.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.PDA.Dtos
{
    /// <summary>
    /// 
    /// </summary>
   public  class AbpLoginResult
    {
        /// <summary>
        /// 
        /// </summary>
        public AbpLoginResultType Result { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public EmployeeDto User { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public AbpLoginResult(AbpLoginResultType result)
        {
            Result = result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="identity"></param>
        public AbpLoginResult(EmployeeDto user)
            : this(AbpLoginResultType.Success)
        {
            User = user;
        }
    }
}
