using Nest;

using Znode.Libraries.Search;

namespace Znode.Libraries.ElasticSearch
{
    public class ElasticCMSPageSearchService : ElasticSearchBaseService , IElasticCMSPageSearchService
    {
        #region Public method

        //Get cms page search response of cms page request
        public virtual IZnodeCMSPageSearchResponse FullTextContentPageSearch(IZnodeCMSPageSearchRequest request)
        {
            //Checks if the index is present or not.
            CheckIsCMSPageIndexExists(request.IndexName);

            //Get paging params
            request = GetPagingParameters(request);

            SetDefaultFilters(request);

            ISearchResponse<dynamic> searchResponse = GetSearchResponse(MapModel(request));

            return ElasticCMSPageMapper.MapSearchResponseToCMSPageSearchResponse(searchResponse);
        }

        //Get cms page count response of cms page request
        public virtual IZnodeCMSPageSearchResponse GetSearchContentPageCount(IZnodeCMSPageSearchRequest request)
        {
            //Checks if the index is present or not.
            CheckIsCMSPageIndexExists(request.IndexName);

            SetDefaultFilters(request);

            CountResponse countResponse = GetCountSearchResponse(MapModel(request));

            return ElasticCMSPageMapper.MapCountResponseToCMSPageSearchResponse(countResponse); ;
        }

        #endregion Public method

        #region Protected method

        //Map ElasticCMSPageSearchRequest model to ElasticSearchRequest model.
        protected virtual ElasticSearchRequest MapModel(IZnodeCMSPageSearchRequest request)
        {
            ElasticSearchRequest znodeModel = new ElasticSearchRequest();

            znodeModel.PageFrom = request.PageFrom;
            znodeModel.PageSize = request.PageSize;
            znodeModel.StartIndex =request.StartIndex;
            znodeModel.LocaleId = request.LocaleId;
            znodeModel.ProfileId = request.ProfileId;
            znodeModel.SearchText = request.SearchText;
            znodeModel.IndexName = request.IndexName;
            znodeModel.QueryClass = request.QueryClass;
            znodeModel.Operator = request.Operator;
            znodeModel.CatalogIdLocalIdDictionary = request.FilterDictionary;
            znodeModel.FilterValues = request.FilterValues;
            znodeModel.SearchableAttribute = request.SearchableAttribute;
            znodeModel.Source = request.Source;
            return znodeModel;
        }

        #endregion Protected method
    }
}
