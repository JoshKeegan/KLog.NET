/*
 * KLog.NET WebAPI Demo
 * Authors:
 *  Josh Keegan 04/04/2018
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Demo_KLog_WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
