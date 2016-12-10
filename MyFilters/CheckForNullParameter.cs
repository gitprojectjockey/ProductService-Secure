using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ProductService.MyFilters
{
    public class CheckForNullParameter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            Func<string, string> convert = s => $"Parameter {s} cannot be null. ";
            string message = "";

            if (actionContext.ActionArguments.Where(a => a.Value == null).Count() > 0)
            {
                string[] argumentNames = string
                    .Join("|", actionContext.ActionArguments
                    .Where(a => a.Value == null)
                    .Select(a => a.Key))
                    .Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var an in argumentNames)
                        message += convert(an);

                actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, message);
            }
        }
    }
}