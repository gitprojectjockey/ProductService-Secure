using EDataLayer.Core.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ProductService.MyFilters
{
    public class GenericModelListStateValidator : ActionFilterAttribute
    {
        private readonly string _modelListName = string.Empty;
        private readonly Type _type;
        public GenericModelListStateValidator(string modelListName, Type T)
        {
            _modelListName = modelListName;
            _type = T;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            // The MakeGenericType method allows you to write code that assigns specific types to the type parameters of a generic type definition, 
            // thus creating a Type object that represents a particular constructed type.You can use this Type object to create run-time instances of the constructed type.
           var listType = typeof(List<>).MakeGenericType(new Type[] { _type });

            // Activator class contains methods to create types of objects locally or remotely, or obtain references to existing remote objects. This class cannot be inherited.
            // The CreateInstance method creates an instance of a type defined in an assembly by invoking the constructor that best matches the specified arguments.
            // If no arguments are specified, the constructor that takes no parameters, that is, the default constructor, is invoked.
            IList modelList = (IList)Activator.CreateInstance(listType);
            modelList = (IList)actionContext.ActionArguments[_modelListName];

            foreach (var m in modelList)
            {
                if (!actionContext.ModelState.IsValid)
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
                }
            }
        }
    }
}