using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace KalaERPApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            // Web API routes
            config.MapHttpAttributeRoutes();
            
            //Cors Enable
           // var cors = new EnableCorsAttribute("*", "*", "*");
           // var cors = new EnableCorsAttribute("http://www.kalapms.com:8282", "*", "*");
           // config.EnableCors(cors);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        //public static void Register(HttpConfiguration config)
        //{
        //    // Web API configuration and services

        //    config.Formatters.Remove(config.Formatters.XmlFormatter);
        //    // Web API routes
        //    config.MapHttpAttributeRoutes();
        //    // Cors Enable
        //    var cors = new EnableCorsAttribute("*", "*", "*");
        //   //  var cors = new EnableCorsAttribute("http://www.kalapms.com:8282", "*", "*");
        //    config.EnableCors(cors);

        //    config.Routes.MapHttpRoute(
        //      name: "DefaultApi",
        //        routeTemplate: "api/{controller}/{id}",
        //        defaults: new { id = RouteParameter.Optional }
        //    );
        //}


        //public static void Register(HttpConfiguration config)
        //{
        //    // New code
        //    var cors = new EnableCorsAttribute("*", "*", "*");
        //    config.EnableCors();
        //    config.Routes.MapHttpRoute(
        //    name: "DefaultApi",
        //    routeTemplate: "api/{controller}/{id}",
        //    defaults: new { id = RouteParameter.Optional }
        //    );
        //}

    }
}

