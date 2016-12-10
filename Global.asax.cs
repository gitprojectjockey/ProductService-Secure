using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace ProductService
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            GlobalConfiguration.Configure(WebApiConfig.Register);


            //My custom helpers
            GlobalConfiguration.Configuration.Services.Add(typeof(IExceptionLogger), new Errors.GExceptionLogger());
            GlobalConfiguration.Configuration.Services.Replace(typeof(IExceptionHandler), new Errors.GExceptionHandler());

            //Call configure on LOG4NET so it pulls in the web config entities.
            log4net.Config.XmlConfigurator.Configure();
        }
    }
}
