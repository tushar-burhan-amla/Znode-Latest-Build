using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class CMSPageSearchCache : BaseCache, ICMSPageSearchCache
    {
        private readonly ICMSPageSearchService _cmsPageSearchService;

        public CMSPageSearchCache(ICMSPageSearchService cmsPageSearchService)
        {
            _cmsPageSearchService = cmsPageSearchService;
        }


        #region CMS Index

        //Get CMS Pages search index monitor list.
        public virtual string GetCmsPageSearchIndexMonitorList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CMSSearchIndexMonitorListModel cmsSearchIndexMonitorList = _cmsPageSearchService.GetCmsPageSearchIndexMonitorList(Expands, Filters, Sorts, Page);
                if (IsNotNull(cmsSearchIndexMonitorList))
                {
                    CMSSearchIndexMonitorListResponse response = new CMSSearchIndexMonitorListResponse
                    {
                        CMSSearchIndexMonitorList = cmsSearchIndexMonitorList.CMSSearchIndexMonitorList,
                        PortalId = cmsSearchIndexMonitorList.PortalId,
                        CMSSearchIndexId = cmsSearchIndexMonitorList.CMSSearchIndexId
                    };
                    response.MapPagingDataFromModel(cmsSearchIndexMonitorList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get CMS Pages index data.
        public virtual string GetCmsPageIndexData(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CMSPortalContentPageIndexModel cmsPageIndex = _cmsPageSearchService.GetCmsPagesIndexData(Expands, Filters);
                if (IsNotNull(cmsPageIndex))
                {
                    CMSPortalContentPageIndexResponse response = new CMSPortalContentPageIndexResponse { CMSPortalContentPageIndex = cmsPageIndex };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region CMS page search request

        //Perform CMS full text result from the keyword.
        public virtual string FullTextContentPageSearch(CMSPageSearchRequestModel model, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                CMSKeywordSearchModel cmsKeywordSearchModel = _cmsPageSearchService.FullTextContentPageSearch(model, Filters);
                if (IsNotNull(cmsKeywordSearchModel))
                {
                    CMSKeywordSearchResponse response = new CMSKeywordSearchResponse() { CMSSearch = cmsKeywordSearchModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}
