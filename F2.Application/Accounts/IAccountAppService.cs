using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Accounts
{

    /// <summary>
    /// 
    /// </summary>
    public interface IAccountAppService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PlateNumber"></param>
        /// <returns></returns>
        bool CheckExistsWeixinTuser(string PlateNumber);
    }
}
