using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace ApiSign.Test.MVCWeb
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            //让接口路由支持带action名称的url和不带action的url
            config.Routes.MapHttpRoute("DefaultApiWithId",
               "api/{area}/{controller}/{id}",
               new { id = RouteParameter.Optional },
               new { id = @"\d+" });
            config.Routes.MapHttpRoute("DefaultApiWithAction",
                "api/{area}/{controller}/{action}");

            config.Routes.MapHttpRoute("DefaultApiGet",
                "Api/{area}/{controller}",
                new { action = "Get" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("DefaultApiPost",
                "Api/{area}/{controller}",
                new { action = "Post" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
        }
    }
}
