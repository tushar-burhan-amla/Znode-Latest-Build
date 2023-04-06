namespace Znode.Engine.Api.Client.Endpoints
{
    public class CMSPageSearchEndpoint : BaseEndpoint
    {
        #region  Cms Page Search Index
        //Endpoint to create search index for CMS pages.
        public static string InsertCreateCmsPageIndexData() => $"{ApiRoot}/cmssearchconfiguration/insertcreatecmspageindexdata";

        //Endpoint to get list of search index monitor of CMS page.
        public static string GetCmsPageSearchIndexMonitorList() => $"{ApiRoot}/cmssearchconfiguration/getcmspagesearchindexmonitorlist";

        //Endpoint to get index data of CMS pages.
        public static string GetCmsPageIndexData() => $"{ApiRoot}/cmssearchconfiguration/getcmspageindexdata";

        #endregion

        #region CMS page search request
        //Endpoint to get CMS Search page base on keyword search.
        public static string FullTextContentPageSearch() => $"{ApiRoot}/cmssearchconfiguration/fulltextcontentpagesearch";

        #endregion
    }
}
