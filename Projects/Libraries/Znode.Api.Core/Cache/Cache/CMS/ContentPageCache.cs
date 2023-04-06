using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class ContentPageCache : BaseCache, IContentPageCache
    {
        #region Private Variable
        private readonly IContentPageService _service;
        #endregion

        #region Constructor
        public ContentPageCache(IContentPageService service)
        {
            _service = service;
        }
        #endregion

        #region Public Methods
        #region Content Page
        //Get Content page list.
        public virtual string GetContentPageList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ContentPageListModel list = _service.GetContentPageList(Filters, Expands, Sorts, Page);
                if (list?.ContentPageList?.Count > 0)
                {
                    ContentPageListResponse response = new ContentPageListResponse { ContentPageList = list.ContentPageList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Get Content page by Content page id and localeId.
        public virtual string GetContentPage(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ContentPageModel ContentPage = _service.GetContentPage(Filters);
                if (HelperUtility.IsNotNull(ContentPage))
                    data = InsertIntoCache(routeUri, routeTemplate, new ContentPageResponse { ContentPage = ContentPage });
            }
            return data;
        }

        //Get Content page list for portal.
        public virtual string GetContentPagesList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                WebStoreContentPageListModel list = _service.GetContentPagesList(Filters);
                if (list?.ContentPageList?.Count > 0)
                {
                    WebStoreContentPageListResponse response = new WebStoreContentPageListResponse { ContentPageList = list.ContentPageList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }
        #endregion 

        #region Tree
        //Gets the content page tree.
        public virtual string GetTree(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get the Content page tree.
                ContentPageTreeModel mediatree = _service.GetTreeNode();

                if (HelperUtility.IsNotNull(mediatree))
                {
                    //Get response and insert it into cache.                   
                    ContentPageResponse response = new ContentPageResponse { Tree = mediatree };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
        #endregion
    }
}