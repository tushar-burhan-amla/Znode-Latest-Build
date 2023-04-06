using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class TemplateClient : BaseClient, ITemplateClient
    {
        //Get the list of template.
        public virtual TemplateListModel GetTemplates(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = TemplateEndpoint.List();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            TemplateListResponse response = GetResourceFromEndpoint<TemplateListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            TemplateListModel list = new TemplateListModel { Templates = response?.Templates };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Create template.
        public virtual TemplateModel CreateTemplate(TemplateModel model)
        {
            string endpoint = TemplateEndpoint.Create();

            ApiStatus status = new ApiStatus();
            TemplateListResponse response = PostResourceToEndpoint<TemplateListResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Template;
        }

        //Get template by cmsTemplateId
        public virtual TemplateModel GetTemplate(int cmsTemplateId, ExpandCollection expands)
        {
            string endpoint = TemplateEndpoint.Get(cmsTemplateId);
            endpoint += BuildEndpointQueryString(expands, null, null, null, null);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TemplateListResponse response = GetResourceFromEndpoint<TemplateListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Template;
        }

        //Update template.
        public virtual TemplateModel UpdateTemplate(TemplateModel templateModel)
        {
            string endpoint = TemplateEndpoint.Update();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TemplateListResponse response = PutResourceToEndpoint<TemplateListResponse>(endpoint, JsonConvert.SerializeObject(templateModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.Template;
        }

        //Delete template.
        public virtual bool DeleteTemplate(ParameterModel cmsTemplateIds)
        {
            string endpoint = TemplateEndpoint.Delete();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(cmsTemplateIds), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
    }
}
