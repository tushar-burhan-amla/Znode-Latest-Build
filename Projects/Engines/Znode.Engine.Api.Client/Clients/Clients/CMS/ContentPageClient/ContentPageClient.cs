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
    public class ContentPageClient : BaseClient, IContentPageClient
    {
        #region Public Methods
        #region Content Page
        //Create Content page.
        public virtual ContentPageModel CreateContentPage(ContentPageModel model)
        {
            string endpoint = ContentPageEndpoint.CreateContentPage();

            ApiStatus status = new ApiStatus();
            ContentPageResponse response = PostResourceToEndpoint<ContentPageResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.ContentPage;
        }

        //Get Content page list.
        public virtual ContentPageListModel GetContentPageList(FilterCollection filters, ExpandCollection expands, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ContentPageEndpoint.GetContentPageList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ContentPageListResponse response = GetResourceFromEndpoint<ContentPageListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ContentPageListModel list = new ContentPageListModel { ContentPageList = response?.ContentPageList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get Content Page on the basis of Content page id.
        public virtual ContentPageModel GetContentPage(FilterCollection filters)
        {
            string endpoint = ContentPageEndpoint.GetContentPage();
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            ContentPageResponse response = GetResourceFromEndpoint<ContentPageResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.ContentPage;
        }

        //Update Content Page.
        public virtual ContentPageModel UpdateContentPage(ContentPageModel ContentPageModel)
        {
            string endpoint = ContentPageEndpoint.UpdateContentPage();

            ApiStatus status = new ApiStatus();
            ContentPageResponse response = PutResourceToEndpoint<ContentPageResponse>(endpoint, JsonConvert.SerializeObject(ContentPageModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.ContentPage;
        }

        //Delete Content Page.
        public virtual bool DeleteContentPage(ParameterModel ContentPageId)
        {
            string endpoint = ContentPageEndpoint.DeleteContentPage();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(ContentPageId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        #region Publish Content Page

        //Publish Content Page.
        public virtual PublishedModel PublishContentPage(ContentPageParameterModel parameterModel)
        {
            string endPoint = ContentPageEndpoint.PublishContentPageWithPreview();
            ApiStatus status = new ApiStatus();
            PublishedResponse response = PostResourceToEndpoint<PublishedResponse>(endPoint, JsonConvert.SerializeObject(parameterModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.PublishedModel;
        }

        #endregion Publish Content Page

        //Get Content page list.
        public virtual WebStoreContentPageListModel GetContentPagesList(FilterCollection filters)
        {
            string endpoint = ContentPageEndpoint.GetContentPagesList();
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            WebStoreContentPageListResponse response = GetResourceFromEndpoint<WebStoreContentPageListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            WebStoreContentPageListModel list = new WebStoreContentPageListModel { ContentPageList = response?.ContentPageList };
            list.MapPagingDataFromResponse(response);

            return list;
        }
        #region Content Page Template
        //Get Content page template list.
        public virtual CMSContentPageTemplateListModel GetContentPageTemplateList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ContentPageEndpoint.GetContentPageTemplateList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ContentPageTemplateListResponse response = GetResourceFromEndpoint<ContentPageTemplateListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            CMSContentPageTemplateListModel list = new CMSContentPageTemplateListModel { ContentPageTemplateList = response?.ContentPageTemplateList };
            list.MapPagingDataFromResponse(response);

            return list;
        }
        #endregion
        #endregion

        #region Content Page Tree
        //Gets the tree.
        public virtual ContentPageTreeModel GetTree()
        {
            //creating endpoint here.
            string endpoint = ContentPageEndpoint.GetTree();
            ApiStatus status = new ApiStatus();

            //get response from API
            ContentPageResponse response = GetResourceFromEndpoint<ContentPageResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Tree;
        }

        public virtual bool RenameFolder(ContentPageFolderModel folderModel)
        {
            string endpoint = ContentPageEndpoint.RenameFolder();
            ApiStatus status = new ApiStatus();

            //get response from API
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(folderModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        public virtual ContentPageFolderModel AddFolder(ContentPageFolderModel folderModel)
        {
            string endpoint = ContentPageEndpoint.AddFolder();
            ApiStatus status = new ApiStatus();

            //Get response from API.
            ContentPageResponse response = PostResourceToEndpoint<ContentPageResponse>(endpoint, JsonConvert.SerializeObject(folderModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.ContentPageFolder;
        }

        public virtual bool DeleteFolder(ParameterModel cmsContentPageGroupId)
        {
            string endpoint = ContentPageEndpoint.DeleteFolder();
            ApiStatus status = new ApiStatus();

            //get response from API
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(cmsContentPageGroupId), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Add folder to anther folder.
        public virtual bool MoveContentPagesFolder(ContentPageFolderModel folderModel)
        {
            //Get Endpoint
            string endpoint = ContentPageEndpoint.MoveContentPagesFolder();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(folderModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Move content page to specified folder.
        public virtual bool MovePageToFolder(AddPageToFolderModel model)
        {
            string endpoint = ContentPageEndpoint.MovePageToFolder();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
        #endregion
        #endregion
    }
}
