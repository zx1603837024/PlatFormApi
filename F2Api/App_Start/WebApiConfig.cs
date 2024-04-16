using F2.Core.Extensions.WebMvc;
using F2Api.WebApi.Filters;
using System.Net.Http.Headers;
using System.Web.Http;

namespace F2Api
{
    /// <summary>
    /// 
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();
            config.Filters.Add(new PlatFormApiAttribute());
            config.Filters.Add(new LogGlobalAttribute());
            config.Filters.Add(new ExceptionGlobalAtrribute());
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}",
                defaults: new { id = RouteParameter.Optional }
            );
            log4net.Config.XmlConfigurator.Configure();
        }
    }
}
