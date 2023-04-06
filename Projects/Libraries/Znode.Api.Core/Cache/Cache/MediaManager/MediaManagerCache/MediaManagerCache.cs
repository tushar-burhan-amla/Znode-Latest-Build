using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class MediaManagerCache : BaseCache, IMediaManagerCache
    {
        #region private Data Member
        private readonly IMediaManagerServices _service;
        #endregion

        #region Public Constructor
        public MediaManagerCache(IMediaManagerServices mediaManagerService)
        {
            _service = mediaManagerService;
        }
        #endregion

        #region Public Methods
        //This method is used to get all media or filter medias by folder id if not present present return all medias.
        public virtual string GetMedias(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get media's from database by call service method
                MediaManagerListModel list = _service.GetMedias(Expands, Filters, Sorts, Page);
                if (HelperUtility.IsNotNull(list))
                {
                    //Get response and insert it into cache.                   
                    MediaManagerListResponses response = new MediaManagerListResponses { MediaList = list };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //This method is used to get all media or filter medias by folder id if not present present return all medias.
        public virtual string GetMedias(FilterCollection paramFilter, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get media's from database by call service method
                MediaManagerListModel list = _service.GetMedias(Expands, paramFilter, Sorts, Page);
                if (HelperUtility.IsNotNull(list))
                {
                    //Get response and insert it into cache.                   
                    MediaManagerListResponses response = new MediaManagerListResponses { MediaList = list };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //This method is used to get a media metadata according to media Id.
        public virtual string GetMediaID(int mediaId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get the media metadata according to media Id.
                MediaManagerModel media = _service.GetMediaByID(mediaId, Expands);
                if (HelperUtility.IsNotNull(media))
                {
                    //Get response and insert it into cache.                   
                    MediaManagerResponses response = new MediaManagerResponses { Media = media };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Gets the attributes for the specified media.
        public virtual string GetMediaAttributeValues(string routeUri, string routeTemplate, int mediaId)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get media's from database by call service method
                MediaAttributeValuesListModel list = _service.GetMediaAttributeValues(mediaId, Expands);
                if (HelperUtility.IsNotNull(list))
                {
                    //Get response and insert it into cache.                   
                    MediaAttributeValuesListResponse response = new MediaAttributeValuesListResponse { MediaAttributeValues = list };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //This method is used to get a media attribute family id by extension.
        public virtual string GetAttributeFamilyIdByName(string routeUri, string routeTemplate, string extension)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get mediapath from database by call service method
                MediaAttributeFamily AttributeFamily = _service.GetAttributeFamilyIdByName(extension);
                if (HelperUtility.IsNotNull(AttributeFamily))
                {
                    //Get response and insert it into cache.                   
                    MediaAttributeFamilyResponse response = new MediaAttributeFamilyResponse { AttributeFamily = AttributeFamily };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Tree Nodes.
        public virtual string GetTree(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get the media metadata according to media Id.
                MediaManagerTreeModel mediatree = _service.GetTreeNode();

                if (HelperUtility.IsNotNull(mediatree))
                {
                    //Get response and insert it into cache.                   
                    MediaManagerTreeResponse response = new MediaManagerTreeResponse { MediaTree = mediatree };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get allowed extensions.
        public virtual string GetAllowedExtensions(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get mediapath from database by call service method
                FamilyExtensionListModel allowedExtensions = _service.GetAllowedExtensions();
                if (allowedExtensions?.FamilyExtensions?.Count > 0)
                {
                    //Get response and insert it into cache.                   
                    MediaManagerResponses response = new MediaManagerResponses { FamilyExtensionListModel = allowedExtensions };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Media Details by ID
        public string GetMediaDetailsById(int mediaId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get the media metadata according to media Id.
                MediaDetailModel media = _service.GetMediaDetailsById(mediaId);
                if (HelperUtility.IsNotNull(media))
                {
                    //Get response and insert it into cache.                   
                    MediaDetailResponses response = new MediaDetailResponses { Media = media };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        // Get MediaDetails By Guid
        public virtual string GetMediaDetailsByGuid(string mediaGuid, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get the media metadata according to media Id.
                MediaDetailModel media = _service.GetMediaDetailsByGuid(mediaGuid);
                if (HelperUtility.IsNotNull(media))
                {
                    //Get response and insert it into cache.                   
                    MediaDetailResponses response = new MediaDetailResponses { Media = media };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}