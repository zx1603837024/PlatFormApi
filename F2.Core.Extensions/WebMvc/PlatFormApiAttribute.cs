using F2.Core.Extensions.Auditing;
using F2.Core.Extensions.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace F2.Core.Extensions.WebMvc
{
    /// <summary>
    /// 
    /// </summary>
    public class PlatFormApiAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly string key = "auditlog";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        private AbpUserLoginToken GetLoginToken(string access_token)
        {
            string sql = "select * from AbpUserLoginToken where Token = @Token";
            if (access_token.Length < 16)
                sql = "select top 1 * from AbpUserLoginToken where EmployeeId = @Token order by id desc ";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@Token", access_token)
            };
            return DataProcessHelper.GetEntityFromTable<AbpUserLoginToken>(SqlHelper.ExecuteDataTable(CommandType.Text, sql, param))[0];
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            if (SkipLogging(actionContext))
            {
                return base.OnActionExecutingAsync(actionContext, cancellationToken);
            }

            AuditLog audit = new AuditLog();
            HttpRequest request = HttpContext.Current.Request;
            audit.ServiceName = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            audit.MethodName = actionContext.ActionDescriptor.ActionName;
            audit.ClientIpAddress = request.UserHostAddress;
            audit.ExecutionTime = DateTime.Now;
            audit.ClientName = request.UserHostName;
            audit.BrowserInfo = request.Browser.Browser + " / " + request.Browser.Version + " / " + request.Browser.Type;
            audit.Parameters = ConvertArgumentsToJson(actionContext.ActionArguments);
            //获取用户id，与用户商户

            object access_token = null;
            actionContext.ActionArguments.TryGetValue("access_token", out access_token);
            
            object obj = null;
            actionContext.ActionArguments.TryGetValue("input", out obj);
            if(obj != null)
            {
                if (obj is EntityDto)
                {
                    EntityDto entity = (EntityDto)obj;
                    access_token = entity.access_token;
                }
            }

            if (access_token != null)
            {
                AbpUserLoginToken loginToken = GetLoginToken(access_token.ToString());
                audit.UserId = loginToken.EmployeeId;
                audit.TenantId = loginToken.TenantId;
            }

            actionContext.Request.Properties[key] = audit;
            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            object audit = null;
            if (actionExecutedContext.Request.Properties.TryGetValue(key, out audit))
            {
                var auditlog = (AuditLog)audit;
                auditlog.ExecutionDuration = Convert.ToInt32((DateTime.Now - auditlog.ExecutionTime).TotalMilliseconds);
                Msmq.MsmqService.SendAuditMsmq(auditlog);
            }
            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }

        /// <summary>
        /// 判断控制器和Action是否要进行拦截（通过判断是否有DisableAuditingAttribute过滤器来验证）
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        private static bool SkipLogging(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<DisableAuditingAttribute>().Count > 0 || actionContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<DisableAuditingAttribute>().Count > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private string ConvertArgumentsToJson(Dictionary<string, object> parameters)
        {
            try
            {
                return JsonConvert.SerializeObject(parameters);
            }
            catch 
            {
                return "{}";
            }
        }
    }
}
