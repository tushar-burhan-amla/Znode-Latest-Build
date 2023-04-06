using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class SearchBoostAndBuryRuleCache : BaseCache, ISearchBoostAndBuryRuleCache
    {
        private readonly ISearchBoostAndBuryRuleService _service;

        public SearchBoostAndBuryRuleCache(ISearchBoostAndBuryRuleService searchBoostAndBuryRuleService)
        {
            _service = searchBoostAndBuryRuleService;
        }

        //get search profile list
        public string GetBoostAndBuryRules(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get Search Profile list.
                SearchBoostAndBuryRuleListModel list = _service.GetBoostAndBuryRules(Expands, Filters, Sorts, Page);
                if (list.SearchBoostAndBuryRuleList?.Count > 0)
                {
                    //Create response.
                    SearchBoostAndBuryRuleListResponse response = new SearchBoostAndBuryRuleListResponse { SearchBoostAndBuryRuleList = list.SearchBoostAndBuryRuleList, PublishCatalogId = list.PublishCatalogId, CatalogName = list.CatalogName };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get boost and bury rule on the basis of searchCatalogRuleId.
        public virtual string GetBoostAndBuryRule(int searchCatalogRuleId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                SearchBoostAndBuryRuleModel searchBoostAndBuryRuleModel = _service.GetBoostAndBuryRule(searchCatalogRuleId);
                if (HelperUtility.IsNotNull(searchBoostAndBuryRuleModel))
                {
                    SearchBoostAndBuryRuleResponse response = new SearchBoostAndBuryRuleResponse { SearchBoostAndBuryRule = searchBoostAndBuryRuleModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get searchable field list.
        public string GetSearchableFieldList(int publishCatalogId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get Search Profile list.
                SearchBoostAndBuryRuleModel searchBoostAndBuryRuleModel = _service.GetSearchableFieldList(publishCatalogId);
                if (searchBoostAndBuryRuleModel.SearchableFieldValueList?.Count > 0)
                {
                    //Create response.
                    SearchBoostAndBuryRuleResponse response = new SearchBoostAndBuryRuleResponse { SearchBoostAndBuryRule = searchBoostAndBuryRuleModel };

                    //apply pagination parameters.
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Auto suggestion for boost and bury.
        public string GetAutoSuggestion(BoostAndBuryParameterModel parameterModel, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get Auto suggestion.
                List<string> searchBoostAndBuryRuleModel = _service.GetAutoSuggestion(parameterModel);
                if (searchBoostAndBuryRuleModel?.Count > 0)
                {
                    //Create response.
                    BoostAndBuryAutocompleteResponse response = new BoostAndBuryAutocompleteResponse { AutoCompleteList = searchBoostAndBuryRuleModel };

                    //apply pagination parameters.
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
    }
}
