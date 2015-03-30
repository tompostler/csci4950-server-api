using Newtonsoft.Json;
using System;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Server_API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // CORS, globally
            EnableCorsAttribute cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            
            // Make JSON the default response type
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter()
                {
                    SerializerSettings =
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }
                });
            config.Formatters.Add(new Formatters.DsonFormatter());

            // April Fools 2015, only during class
            DateTime now = DateTime.UtcNow;
            DateTime start = new DateTime(2015, 4, 1, 10, 0, 0).ToUniversalTime();
            DateTime end = new DateTime(2015, 4, 1, 12, 0, 0).ToUniversalTime();
            if ((now > start) && (now < end))
            {
                config.Formatters.RemoveAt(0);
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
