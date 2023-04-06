using Newtonsoft.Json;
using System.Collections.Generic;
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
    public class ContentContainerClient : BaseClient, IContentContainerClient
    {
        // Gets the List of Content Container
        public virtual ContentContainerListModel List(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = ContentContainerEndpoint.List();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            ContentContainerListResponseModel response = GetResourceFromEndpoint<ContentContainerListResponseModel>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ContentContainerListModel list = new ContentContainerListModel { ContainerList = response?.ContainerList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Create Content Container
        public virtual ContentContainerResponseModel Create(ContentContainerCreateModel model)
        {
            //Get Endpoint.
            string endpoint = ContentContainerEndpoint.Create();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            ContentContainerResponse response = PostResourceToEndpoint<ContentContainerResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.ContentContainerModel;
        }

        //Get Content Container
        public virtual ContentContainerResponseModel GetContentContainer(string containerKey)
        {
            //Get Endpoint.
            string endpoint = ContentContainerEndpoint.GetContentContainer(containerKey);

            ApiStatus status = new ApiStatus();
            ContentContainerResponse response = GetResourceFromEndpoint<ContentContainerResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.ContentContainerModel;
        }

        //Update Content Container
        public virtual ContentContainerResponseModel Update(ContentContainerUpdateModel model)
        {
            string endpoint = ContentContainerEndpoint.Update();

            ApiStatus status = new ApiStatus();
            ContentContainerResponse response = PutResourceToEndpoint<ContentContainerResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.ContentContainerModel;
        }

        //Get List Of Associated Variants to a Content Container
        public virtual List<AssociatedVariantModel> GetAssociatedVariants(string containerKey)
        {
            string endpoint = ContentContainerEndpoint.GetAssociatedVariants(containerKey);

            ApiStatus status = new ApiStatus();
            ContentContainerListResponseModel response = GetResourceFromEndpoint<ContentContainerListResponseModel>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);


            return response.AssociatedVariants;
        }

        //Associate Variant to a Content Container
        public virtual List<AssociatedVariantModel> AssociateVariant(AssociatedVariantModel model)
        {
            string endpoint = ContentContainerEndpoint.AssociateVariant();
            ApiStatus status = new ApiStatus();
            ContentContainerListResponseModel response = PostResourceToEndpoint<ContentContainerListResponseModel>(endpoint, JsonConvert.SerializeObject(model), status);


            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response.AssociatedVariants;
        }

        //Delete Associated Variant
        public virtual bool DeleteAssociatedVariant(ParameterModel variantIds)
        {
            //Get Endpoint.
            string endpoint = ContentContainerEndpoint.DeleteAssociateVariant();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(variantIds), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Delete Content Container
        public virtual bool DeleteContentContainer(ParameterModel contentContainerIds)
        {
            //Get Endpoint.
            string endpoint = ContentContainerEndpoint.DeleteContentContainer();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(contentContainerIds), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Associate Container Template to container
        public virtual bool AssociateContainerTemplate(int variantId, int containerTemplateId)
        {
            //Get Endpoint.
            string endpoint = ContentContainerEndpoint.AssociateContainerTemplate(variantId, containerTemplateId);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(variantId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }
        //Verify if the Container Key Exist
        public virtual bool IsContainerKeyExist(string containerKey)
        {
            //Get Endpoint.
            string endpoint = ContentContainerEndpoint.IsContainerKeyExist(containerKey);

            //Get response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }

        // Gets the List of Content Container variants
        public virtual AssociatedVariantListModel GetAssociatedVariantList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = ContentContainerEndpoint.GetAssociatedVariantList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            ContentContainerListResponseModel response = GetResourceFromEndpoint<ContentContainerListResponseModel>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            AssociatedVariantListModel list = new AssociatedVariantListModel { AssociatedVariants = response?.AssociatedVariants };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Save variant data
        public virtual bool SaveVariantData(int localeId, int? templateId, int variantId, bool isActive)
        {
            //Get Endpoint.
            string endpoint = ContentContainerEndpoint.SaveVariantData(localeId, templateId, variantId, isActive);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(variantId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get Content Container Locale data
        public virtual ContentContainerResponseModel GetVariantLocaleData(int variantId)
        {
            //Get Endpoint.
            string endpoint = ContentContainerEndpoint.GetVariantLocaleData(variantId);

            ApiStatus status = new ApiStatus();
            ContentContainerResponse response = GetResourceFromEndpoint<ContentContainerResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.ContentContainerModel;
        }

        //Activate/deactivate Associated Variant
        public virtual bool ActivateDeactivateVariant(ParameterModel variantIds, bool isActivate)
        {
            //Get Endpoint.
            string endpoint = ContentContainerEndpoint.ActivateDeactivateVariant(isActivate);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(variantIds), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Publish the entire content container with associated variants
        public virtual PublishedModel PublishContentContainer(string containerKey, string targetPublishState)
        {
            string endpoint = ContentContainerEndpoint.PublishContentContainer(containerKey, targetPublishState);

            ApiStatus status = new ApiStatus();
            PublishedResponse response = PostResourceToEndpoint<PublishedResponse>(endpoint, JsonConvert.SerializeObject(containerKey), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.PublishedModel;
        }
   
        //Publish the container variant 
        public virtual PublishedModel PublishContainerVariant(string containerKey, int containerProfileVariantId, string targetPublishState)
        {
            string endpoint = ContentContainerEndpoint.PublishContainerVariant(containerKey, containerProfileVariantId, targetPublishState);

            ApiStatus status = new ApiStatus();
            PublishedResponse response = PostResourceToEndpoint<PublishedResponse>(endpoint, JsonConvert.SerializeObject(containerKey), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.PublishedModel;
        }
    }
}
