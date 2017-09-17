using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Owin;
using System.Net.Http.Formatting;
using Microsoft.Owin.Cors;

namespace BlogService
{
    class Startup
    {
        
        Type valuesControllerType = typeof(global::BlogService.Controllers.BlogController);

        public void Configuration(IAppBuilder appBuilder)
        {
            
            HttpConfiguration config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            appBuilder.UseCors(CorsOptions.AllowAll);
            appBuilder.UseWebApi(config);

            
        }
    }
}
