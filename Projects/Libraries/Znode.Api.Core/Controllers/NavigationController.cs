using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using System.Web.Http.Description;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class NavigationController : BaseController
    {
        private readonly INavigationService _navigationService;

        public NavigationController()
        {
            _navigationService = new NavigationService();
        }

        [ResponseType(typeof(NavigationResponse))]
        [HttpPost]
        public HttpResponseMessage GetNavigationDetails([FromBody] NavigationParamModel model)
        {
            HttpResponseMessage response;

            try
            {
                var navigationModel = _navigationService.GetNavigationDetails(model);
                response = !Equals(navigationModel, null) ? CreateOKResponse(new NavigationResponse { NavigationModel = navigationModel }) : CreateNotFoundResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new NavigationResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }
    }
}
