using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Znode.Engine.Api.Models.Responses;

namespace Znode.Engine.Api.Helper
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var modelState = actionContext.ModelState;
            if (!modelState.IsValid)
            {
                var errors = modelState
                    .Where(s => s.Value.Errors.Count > 0)
                    .Select(s => new KeyValuePair<string, string>(s.Key, string.IsNullOrEmpty(s.Value.Errors.First().ErrorMessage) ? s.Value.Errors.First().Exception.Message : s.Value.Errors.First().ErrorMessage))
                    .ToDictionary(x => x.Key, x => x.Value);
                if (errors != null)
                {
                    var data = new BaseResponse { HasError = true, ErrorMessage = string.Empty, CustomModelState = errors };
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, data);
                }
            }
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var modelState = actionExecutedContext.ActionContext.ModelState;
            if (!modelState.IsValid)
            {
                var errors = modelState
                    .Where(s => s.Value.Errors.Count > 0)
                    .Select(s => new KeyValuePair<string, string>(s.Key, s.Value.Errors.First().ErrorMessage))
                    .ToDictionary(x => x.Key, x => x.Value);
                if (errors != null)
                {
                    var data = new BaseResponse { HasError = true, ErrorMessage = string.Empty, CustomModelState = errors };
                    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.BadRequest, data);
                }
            }
        }
    }
}