using log4net;
using System.Net.Http;
using System.Text;
using System.Web.Http.ExceptionHandling;

namespace ProductService.Errors
{
    public class GExceptionLogger : ExceptionLogger
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(GExceptionLogger));

        public override void Log(ExceptionLoggerContext context)
        {
            if(_log.IsErrorEnabled)
                 _log.ErrorFormat($"{RequestToString(context.Request)} {context.Exception}");
        }

        private static string RequestToString(HttpRequestMessage request)
        {
            var message = new StringBuilder();

            if (request.Method != null)
                message.Append($"Method {request.Method}");

            if (request.RequestUri != null)
                message.Append($"Uri : {request.RequestUri}");

            return message.ToString();
        }
    }
}