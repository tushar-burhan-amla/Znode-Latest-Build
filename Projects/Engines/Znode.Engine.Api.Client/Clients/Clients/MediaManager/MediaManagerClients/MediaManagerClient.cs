using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class MediaManagerClient : BaseClient, IMediaManagerClient
    {
        #region Public methods

        //Save metadata files to database.
        public virtual MediaManagerModel SaveMediaFile(MediaManagerModel mediaManagerModel)
        {
            string endpoint = MediaManagerEndpoints.SaveMediaFile();
            ApiStatus status = new ApiStatus();
            MediaManagerResponses response = PostResourceToEndpoint<MediaManagerResponses>(endpoint, JsonConvert.SerializeObject(mediaManagerModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.Media;
        }

        //Updating media by media name.
        public virtual MediaManagerModel UpdateMedia(MediaManagerModel mediaMangerModel)
        {
            string endpoint = MediaManagerEndpoints.Update();
            ApiStatus status = new ApiStatus();
            MediaManagerResponses response = PutResourceToEndpoint<MediaManagerResponses>(endpoint, JsonConvert.SerializeObject(mediaMangerModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.Media;
        }

        //Get all media metadata.
        public virtual MediaManagerListModel GetMedias(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            string endpoint = MediaManagerEndpoints.GetMedias();
            endpoint += BuildEndpointQueryString(null, filters, sortCollection, pageIndex, recordPerPage);
            ApiStatus status = new ApiStatus();
            MediaManagerListResponses response = GetResourceFromEndpoint<MediaManagerListResponses>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.MediaList;
        }

        //Get folder structure for media.
        public virtual MediaManagerTreeModel GetTree()
        {
            string endpoint = MediaManagerEndpoints.GetTree();
            ApiStatus status = new ApiStatus();
            MediaManagerTreeResponse response = GetResourceFromEndpoint<MediaManagerTreeResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.MediaTree;
        }

        //Add new folder to database with parent id.
        public virtual MediaManagerFolderModel AddFolder(MediaManagerFolderModel folderModel)
        {
            string endpoint = MediaManagerEndpoints.AddFolder();
            ApiStatus status = new ApiStatus();
            MediaManagerFolderResponse response = PostResourceToEndpoint<MediaManagerFolderResponse>(endpoint, JsonConvert.SerializeObject(folderModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.MediaManagerFolder;
        }

        //Rename the folder.
        public virtual bool RenameFolder(MediaManagerFolderModel folderModel)
        {
            string endpoint = MediaManagerEndpoints.RenameFolder();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(folderModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Get a media metadata according to media Id.
        public virtual MediaManagerModel GetMedia(int mediaId)
        {
            string endpoint = MediaManagerEndpoints.GetMedia(mediaId);
            ApiStatus status = new ApiStatus();
            MediaManagerResponses response = GetResourceFromEndpoint<MediaManagerResponses>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.Media;
        }

        //Add folder to anther folder.
        public virtual bool MoveFolder(MediaManagerMoveFolderModel mediaModel)
        {
            string endpoint = MediaManagerEndpoints.MoveFolder();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(mediaModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Copy media to specified folder.
        public virtual bool MoveMediaToFolder(AddMediaToFolderModel mediaModel)
        {
            string endpoint = MediaManagerEndpoints.MoveMediaToFolder();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(mediaModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Gets the attributes for the specified media.
        public virtual MediaAttributeValuesListModel GetMediaAttributeValues(int mediaId, ExpandCollection expands)
        {
            string endpoint = MediaManagerEndpoints.GetMediaAttributeValues(mediaId);
            endpoint += BuildEndpointQueryString(expands);
            ApiStatus status = new ApiStatus();
            MediaAttributeValuesListResponse response = GetResourceFromEndpoint<MediaAttributeValuesListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.MediaAttributeValues;
        }

        //Delete medias.
        public virtual string DeleteMedia(DeleteMediaModel deleteMediaModel)
        {
            string endpoint = MediaManagerEndpoints.DeleteMedia();
            ApiStatus status = new ApiStatus();
            StringResponse response = PostResourceToEndpoint<StringResponse>(endpoint, JsonConvert.SerializeObject(deleteMediaModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.Response;
        }

        //Updates media attribute value.
        public virtual bool UpdateMediaAttributeValue(MediaAttributeValuesListModel model)
        {
            string endpoint = MediaManagerEndpoints.UpdateMediaAttributeValue();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response.IsSuccess;
        }

        //Get attribute family id by file extension.
        public virtual MediaAttributeFamily GetAttributeFamilyIdByName(string extension)
        {
            string endpoint = MediaManagerEndpoints.GetAttributeFamilyIdByName(extension);
            ApiStatus status = new ApiStatus();
            MediaAttributeFamilyResponse response = GetResourceFromEndpoint<MediaAttributeFamilyResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.AttributeFamily;
        }

        //Creates a media attribute value.
        public virtual MediaAttributeValuesModel CreateMediaAttributeValue(MediaAttributeValuesModel model)
        {
            string endpoint = MediaManagerEndpoints.CreateMediaAttributeValue();
            ApiStatus status = new ApiStatus();
            MediaAttributeValueResponse response = PostResourceToEndpoint<MediaAttributeValueResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.MediaAttributeValue;
        }

        //Share Media/Folder with users.
        public virtual bool ShareMediaFolder(List<ShareMediaFolderModel> ShareMediaFolderListModel)
        {
            string endpoint = MediaManagerEndpoints.ShareMediaFolder();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(ShareMediaFolderListModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response.IsSuccess;
        }

        //Get allowed extensions.
        public virtual FamilyExtensionListModel GetAllowedExtensions()
        {
            string endpoint = MediaManagerEndpoints.GetAllowedExtensions();
            ApiStatus status = new ApiStatus();
            MediaManagerResponses response = GetResourceFromEndpoint<MediaManagerResponses>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.FamilyExtensionListModel;
        }

        #region Generate Image.
        //Generate specific image on edit.
        public virtual bool GenerateImageOnEdit(string mediaPath)
        {
            GenerateImageModel generateImageModel = new GenerateImageModel();
            generateImageModel.Path = mediaPath;

            string endpoint = MediaManagerEndpoints.GenerateImageOnEdit();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(generateImageModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }
        #endregion

        //Get Media details By Media Id.
        public virtual MediaDetailModel GetMediaDetailsById(int mediaId)
        {
            string endpoint = MediaManagerEndpoints.GetMediaDetailsById(mediaId);
            ApiStatus status = new ApiStatus();
            MediaDetailResponses response = GetResourceFromEndpoint<MediaDetailResponses>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.Media;
        }

        //Get Media details By Media Guid.
        public virtual MediaDetailModel GetMediaDetailsByGuid(string mediaGuid)
        {
            string endpoint = MediaManagerEndpoints.GetMediaDetailsByGuid(mediaGuid);
            ApiStatus status = new ApiStatus();
            MediaDetailResponses response = GetResourceFromEndpoint<MediaDetailResponses>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.Media;
        }
        #endregion
    }
}
