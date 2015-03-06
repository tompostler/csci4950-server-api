using System;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Server_API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Make JSON the default response type
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            // April Fools 2015
            if (DateTime.Today.Date.Equals(new DateTime(2015, 4, 1).Date))
            {
                config.Formatters.Clear();
                config.Formatters.Add(new Formatters.DsonFormatter());
            }

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
