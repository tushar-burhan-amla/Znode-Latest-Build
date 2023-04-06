namespace Znode.Engine.Api.Client.Endpoints
{
    public class SearchEndpoint : BaseEndpoint
    {
        //Endpoint to get search result.
        public static string Keyword() => $"{ApiRoot}/search/keyword";

        //Endpoint to get SEO Url details.
        public static string GetSEOUrlDetails(string seoUrl) => $"{ApiRoot}/search/getseourldetails/{seoUrl}";

        //Creates search index.
        public static string InsertCreateIndexData() => $"{ApiRoot}/search/insertcreateindexdata";

        //Saves the boost data for a specific type.(product, category of field)
        public static string SaveBoostValue() => $"{ApiRoot}/search/saveboostvalues";

        //Deletes the boost data for a specific type.(product, category of field)
        public static string DeleteBoostValue() => $"{ApiRoot}/search/deleteboostvalue";

        //Gets list of Global product level boost list.
        public static string GetGlobalProductBoostList() => $"{ApiRoot}/search/getglobalproductboostlist";

        //Get global product-category boost list.
        public static string GetGlobalProductCategoryBoostList() => $"{ApiRoot}/search/getglobalproductcategoryboostlist";

        //Gets portal index data.
        internal static string GetCatalogIndexData() => $"{ApiRoot}/search/getportalindexdata";

        //Gets field level boost data.
        public static string GetFieldBoostList() => $"{ApiRoot}/search/getfieldboostlist";

        //Endpoint to get keyword search suggestion.
        public static string GetKeywordSearchSuggestion() => $"{ApiRoot}/search/getkeywordsearchsuggestion";

        //Endpoint to get create search index with given index name and for given searchIndexMonitorId.
        public static string CreateIndex(string indexName, string revisionType, int catalogId, int searchIndexMonitorId) => $"{ApiRoot}/search/createindex/{indexName}/{revisionType}/{catalogId}/{searchIndexMonitorId}";

        //Endpoint to get list of search index monitor.
        public static string GetSearchIndexMonitorList() => $"{ApiRoot}/search/getsearchindexmonitorlist";

        //Endpoint to get list of search index server status.
        public static string GetSearchIndexServerStatusList() => $"{ApiRoot}/search/getsearchindexserverstatuslist";

        //Get Search Result.
        public static string FullTextSearch() => $"{ApiRoot}/search/fulltextsearch";

        //Get Facet Search text result.
        public static string FacetSearch() => $"{ApiRoot}/search/facetsearch";

        #region Synonyms
        //Create synonyms endpoint.
        public static string CreateSearchSynonyms() => $"{ApiRoot}/search/createsearchsynonyms";

        //Get synonyms on the basis of synonyms id Endpoint.
        public static string GetSearchSynonyms(int searchSynonymsId) => $"{ApiRoot}/search/getsearchsynonyms/{searchSynonymsId}";

        //Update synonyms data for search.
        public static string UpdateSearchSynonyms() => $"{ApiRoot}/search/updatesearchsynonyms";

        //Get Synonyms list.
        public static string GetSearchSynonymsList() => $"{ApiRoot}/search/getsearchsynonymslist";

        //Delete synonyms by id.
        public static string DeleteSearchSynonyms() => $"{ApiRoot}/search/deletesearchsynonyms";
        #endregion

        #region Keywords Redirect
        //Get catalog keywords redirect list.
        public static string GetCatalogKeywordsRedirectList() => $"{ApiRoot}/search/getcatalogkeywordsredirectlist";

        //Create keywords and its redirected url for search.
        public static string CreateSearchKeywordsRedirect() => $"{ApiRoot}/search/createsearchkeywordsredirect";

        //Get keywords on the basis of keywords id Endpoint.
        public static string GetSearchKeywordsRedirect(int searchKeywordsRedirectId) => $"{ApiRoot}/search/getsearchkeywordsredirect/{searchKeywordsRedirectId}";

        //Update keywords data for search.
        public static string UpdateSearchKeywordsRedirect() => $"{ApiRoot}/search/updatesearchkeywordsredirect";

        //Delete keywords by id.
        public static string DeleteSearchKeywordsRedirect() => $"{ApiRoot}/search/deletesearchkeywordsredirect";

        //Write synonyms.txt for search.
        public static string WriteSynonymsFile(int publishCatalogId, bool isSynonymsFile) => $"{ApiRoot}/search/writesynonymsfile/{publishCatalogId}/{isSynonymsFile}";

        //Delete elastic search index
        public static string DeleteIndex(int catalogIndexId)=> $"{ApiRoot}/search/deleteindex/{catalogIndexId}";

        //Endpoint to get product details by sku.
        public static string GetProductDetailsBySKU() => $"{ApiRoot}/search/getproductdetailsbysku";

        #endregion
    }
}
