using F2.Core.Extensions.Log;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;


namespace F2Api.WebApi.Filters
{
    /// <summary>
    /// 日志全局过滤 继承自ActionFilterAttribute 只有权限访问的时候才进行访问日志操作
    /// </summary>
    public class LogGlobalAttribute : ActionFilterAttribute
    {
       
        /// <summary>
        /// 全局操作日志
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string actionName = filterContext.ActionDescriptor.ActionName;
            HttpContextBase context = (HttpContextBase)filterContext.Request.Properties["MS_HttpContext"];//获取传统context
            HttpRequestBase request = context.Request;//定义传统request对象 
                                                      //获取当前用户上下文

            //获取action参数
            var actionParamteters = filterContext.ActionArguments;
            string description = JsonConvert.SerializeObject(actionParamteters);
           
            try
            {
                Logger.Log.Info(string.Format("来自{0}的请求,请求地址{1},参数描述{2}", context.Request.UserHostAddress, request.Url.ToString(), description));
            }
            catch
            {
            }
            base.OnActionExecuting(filterContext);
        }



    }

}