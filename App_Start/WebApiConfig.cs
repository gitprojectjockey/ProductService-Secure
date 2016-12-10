
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Routing;

namespace ProductService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // NOTE: GLOBAL ASAX HAS ALL MY OTHER REGISTATIONS


            // Setup media type formatters
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;

            //I installed Install-Package Microsoft.AspNet.WebApi.Cors and enabled Cors on this service
            //By Installing CORS or Cross - Origin Requests on the web service I am able to call the products api using stright json with ajax
            //from a cross domain client.

            //This enables CORS for all controllers and actions (GLOBAL) you can also do this at the controller of action level
            // var cors = new EnableCorsAttribute("http://localhost:50617", "*", "get,post,put,delete");
            var cors = new EnableCorsAttribute("http://localhost:50617", "*", "*");
            config.EnableCors();

           

            //This Service uses attribute routing only so we need to call config.MapHttpAttributeRoutes() to enable it.      
            config.MapHttpAttributeRoutes();

            // Only allow json formatting by removing the xml formatter
             config.Formatters.Remove(config.Formatters.XmlFormatter);
            // Only allow xml formatting by removing the json formatter
            // config.Formatters.Remove(config.Formatters.JsonFormatter);
        }
    }
}
