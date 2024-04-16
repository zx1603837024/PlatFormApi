using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace F2.Core.Extensions.WebMvc
{
    /// <summary>
    /// 异常日志拦截
    /// </summary>
    public class ApiExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(HttpActionExecutedContext context)
        {

        }
    }
}
