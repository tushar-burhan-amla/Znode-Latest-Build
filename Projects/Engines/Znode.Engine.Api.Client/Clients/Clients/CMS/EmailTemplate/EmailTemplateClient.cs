using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class EmailTemplateClient : BaseClient, IEmailTemplateClient
    {
        //Gets the list of email templates.
        public virtual EmailTemplateListModel GetTemplates(ExpandCollection expands, FilterCollection filters, SortCollection sorts)
            => GetTemplates(expands, filters, sorts, null, null);

        //Gets the list of email templates with paging.
        public virtual EmailTemplateListModel GetTemplates(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = EmailTemplateEndpoint.List();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            EmailTemplateListResponse response = GetResourceFromEndpoint<EmailTemplateListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            EmailTemplateListModel list = new EmailTemplateListModel { EmailTemplatesList = response?.EmailTemplates };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Creates an email template.
        public virtual EmailTemplateModel CreateTemplatePage(EmailTemplateModel emailTemplateModel)
        {
            string endpoint = EmailTemplateEndpoint.Create();

            ApiStatus status = new ApiStatus();
            EmailTemplateResponse response = PostResourceToEndpoint<EmailTemplateResponse>(endpoint, JsonConvert.SerializeObject(emailTemplateModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.EmailTemplate;
        }

        // Gets an email template according to emailTemplateId.
        public virtual EmailTemplateModel GetTemplatePage(int emailTemplateId, FilterCollection filters)
        {
            string endpoint = EmailTemplateEndpoint.Get(emailTemplateId);
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            EmailTemplateResponse response = GetResourceFromEndpoint<EmailTemplateResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
        
            return response?.EmailTemplate;
        }

        // Update an email template.
        public virtual EmailTemplateModel UpdateTemplatePage(EmailTemplateModel emailTemplateModel)
        {
            string endpoint = EmailTemplateEndpoint.Update();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            EmailTemplateResponse response = PutResourceToEndpoint<EmailTemplateResponse>(endpoint, JsonConvert.SerializeObject(emailTemplateModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.EmailTemplate;
        }

        // Delete an email template.
        public virtual bool DeleteTemplatePage(ParameterModel emailTemplateId)
        {
            string endpoint = EmailTemplateEndpoint.Delete();
           
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(emailTemplateId), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        // Gets an email template tokens.
        public virtual EmailTemplateModel GetEmailTemplateTokens()
        {
            string endpoint = EmailTemplateEndpoint.GetEmailTemplateTokens();

            ApiStatus status = new ApiStatus();
            EmailTemplateResponse response = GetResourceFromEndpoint<EmailTemplateResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.EmailTemplate;
        }

        //Get the Email Template Area List
        public virtual EmailTemplateAreaListModel GetEmailTemplateAreaList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = EmailTemplateEndpoint.EmailTemplateAreaList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            EmailTemplateAreaListResponse response = GetResourceFromEndpoint<EmailTemplateAreaListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            EmailTemplateAreaListModel list = new EmailTemplateAreaListModel { EmailTemplatesAreaList = response?.EmailTemplateAreas };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get the Email Template Area Mapper List, contents the mapped email template details.
        public virtual EmailTemplateAreaMapperListModel GetEmailTemplateAreaMapperList(int portalId)
        {
            string endpoint = EmailTemplateEndpoint.EmailTemplateAreaMapperList(portalId);         

            ApiStatus status = new ApiStatus();
            EmailTemplateAreaMapperListResponse response = GetResourceFromEndpoint<EmailTemplateAreaMapperListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            EmailTemplateAreaMapperListModel list = new EmailTemplateAreaMapperListModel { EmailTemplatesAreaMapperList = response?.EmailTemplateAreaMapper };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Delete Email Template Area Configuration.
        public virtual bool DeleteEmailTemplateAreaMapping(ParameterModel areaMappingId)
        {
            string endpoint = EmailTemplateEndpoint.DeleteEmailTemplateAreaConfiguration();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(areaMappingId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Saves the Email Template Area Configuration Details.
        public virtual bool SaveEmailTemplateAreaConfiguration(EmailTemplateAreaMapperModel model)
        {
            string endpoint = EmailTemplateEndpoint.SaveEmailTemplateAreaConfiguration();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.IsSuccess ?? false;
        }
    }
}
