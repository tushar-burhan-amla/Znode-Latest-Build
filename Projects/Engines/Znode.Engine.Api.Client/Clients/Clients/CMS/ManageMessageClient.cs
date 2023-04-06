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
    public class ManageMessageClient : BaseClient, IManageMessageClient
    {
        #region Manage ManageMessage
        //Get ManageMessage list.
        public virtual ManageMessageListModel GetManageMessages(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ManageMessageEndpoint.GetManageMessages();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ManageMessageListResponse response = GetResourceFromEndpoint<ManageMessageListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ManageMessageListModel list = new ManageMessageListModel { ManageMessages = response?.ManageMessages };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Create ManageMessage.
        public virtual ManageMessageModel CreateManageMessage(ManageMessageModel messageModel)
        {
            string endpoint = ManageMessageEndpoint.CreateManageMessage();

            ApiStatus status = new ApiStatus();
            ManageMessageResponse response = PostResourceToEndpoint<ManageMessageResponse>(endpoint, JsonConvert.SerializeObject(messageModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.ManageMessage;
        }

        //Get ManageMessage details.
        public virtual ManageMessageModel GetManageMessage(ManageMessageMapperModel manageMessageMapperModel)
        {
            string endpoint = ManageMessageEndpoint.GetManageMessage();

            ApiStatus status = new ApiStatus();
            ManageMessageResponse response = PutResourceToEndpoint<ManageMessageResponse>(endpoint, JsonConvert.SerializeObject(manageMessageMapperModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.ManageMessage;
        }

        //Update ManageMessage.
        public virtual ManageMessageModel UpdateManageMessage(ManageMessageModel messageModel)
        {
            string endpoint = ManageMessageEndpoint.UpdateManageMessage();

            ApiStatus status = new ApiStatus();
            ManageMessageResponse response = PutResourceToEndpoint<ManageMessageResponse>(endpoint, JsonConvert.SerializeObject(messageModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.ManageMessage;
        }

        //Delete ManageMessage.
        public virtual bool DeleteManageMessage(ParameterModel cmsManageMessageId)
        {
            string endpoint = ManageMessageEndpoint.DeleteManageMessage();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(cmsManageMessageId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Publish manage  Message
        public virtual PublishedModel PublishManageMessage(ContentPageParameterModel contentPageParameterModel)
        {
            string endpoint = ManageMessageEndpoint.PublishManageMessageWithPreview();

            ApiStatus status = new ApiStatus();
            PublishedResponse response = PostResourceToEndpoint<PublishedResponse>(endpoint, JsonConvert.SerializeObject(contentPageParameterModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.PublishedModel;
        }
        #endregion
    }
}
