using F2.Core.Extensions.Log;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace F2Api.WebApi.Filters
{
    /// <summary>
    /// 异常全局处理
    /// </summary>
    public class ExceptionGlobalAtrribute : ExceptionFilterAttribute
    {

        /// <summary>
        /// 异常全局处理
        /// </summary>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception != null)
            {
                string description = actionExecutedContext.Exception.Message.ToString();
                var actionParamteters = actionExecutedContext.ActionContext.ActionArguments;
                string args = JsonConvert.SerializeObject(actionParamteters);
                string controllerName = actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                string actionName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
                var url = actionExecutedContext.Request.RequestUri.AbsoluteUri;
                try
                {
                    Logger.Log.Error(string.Format("请求地址{0},参数描述{1},发生的异常{2}", url, args, description+"\r\n"+ actionExecutedContext.Exception.StackTrace));
                }
                catch
                {
                }

            }

            //throw new HttpResponseException(oHttpResponseMessage);
            base.OnException(actionExecutedContext);

        }
    }

}