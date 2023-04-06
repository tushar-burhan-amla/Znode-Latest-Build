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
    public class SearchBoostAndBuryClient : BaseClient, ISearchBoostAndBuryClient
    {
        //Gets boost and bury rule list.
        public SearchBoostAndBuryRuleListModel GetBoostAndBuryRules(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? page, int? recordPerPage)
        {
            //Get Endpoint.
            string endpoint = SearchBoostAndBuryRuleEndpoint.GetBoostAndBuryRules();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, page, recordPerPage);

            //Get response
            ApiStatus status = new ApiStatus();
            SearchBoostAndBuryRuleListResponse response = GetResourceFromEndpoint<SearchBoostAndBuryRuleListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            SearchBoostAndBuryRuleListModel list = new SearchBoostAndBuryRuleListModel { SearchBoostAndBuryRuleList = response?.SearchBoostAndBuryRuleList, PublishCatalogId = response?.PublishCatalogId ?? 0, CatalogName = response?.CatalogName };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Create boost and bury rule.
        public virtual SearchBoostAndBuryRuleModel CreateBoostAndBuryRule(SearchBoostAndBuryRuleModel searchBoostAndBuryRuleModel)
        {
            string endpoint = SearchBoostAndBuryRuleEndpoint.CreateBoostAndBuryRule();

            ApiStatus status = new ApiStatus();
            SearchBoostAndBuryRuleResponse response = PostResourceToEndpoint<SearchBoostAndBuryRuleResponse>(endpoint, JsonConvert.SerializeObject(searchBoostAndBuryRuleModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.SearchBoostAndBuryRule;
        }

        //Get boost and bury on the basis of searchCatalogRuleId.
        public virtual SearchBoostAndBuryRuleModel GetBoostAndBuryRule(int searchCatalogRuleId)
        {
            string endpoint = SearchBoostAndBuryRuleEndpoint.GetBoostAndBuryRule(searchCatalogRuleId);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            SearchBoostAndBuryRuleResponse response = GetResourceFromEndpoint<SearchBoostAndBuryRuleResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.SearchBoostAndBuryRule;
        }

        //Update boost and bury rule.
        public virtual SearchBoostAndBuryRuleModel UpdateBoostAndBuryRule(SearchBoostAndBuryRuleModel searchBoostAndBuryRuleModel)
        {
            string endpoint = SearchBoostAndBuryRuleEndpoint.UpdateBoostAndBuryRule();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            SearchBoostAndBuryRuleResponse response = PutResourceToEndpoint<SearchBoostAndBuryRuleResponse>(endpoint, JsonConvert.SerializeObject(searchBoostAndBuryRuleModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.SearchBoostAndBuryRule;
        }

        //Deletes search Profile
        public bool Delete(ParameterModel parameterModel)
        {
            string endpoint = SearchBoostAndBuryRuleEndpoint.Delete();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(parameterModel), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Deletes search Profile
        public bool PausedSearchRule(ParameterModel parameterModel, bool isPause)
        {
            string endpoint = SearchBoostAndBuryRuleEndpoint.PausedSearchRule(isPause);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(parameterModel), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Get searchable field list.
        public SearchBoostAndBuryRuleModel GetSearchableFieldList(int PublishCatalogId)
        {
            //Get Endpoint.
            string endpoint = SearchBoostAndBuryRuleEndpoint.GetSearchableFieldList(PublishCatalogId);

            //Get response
            ApiStatus status = new ApiStatus();
            SearchBoostAndBuryRuleResponse response = GetResourceFromEndpoint<SearchBoostAndBuryRuleResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            SearchBoostAndBuryRuleModel list = new SearchBoostAndBuryRuleModel { SearchableFieldValueList = response?.SearchBoostAndBuryRule?.SearchableFieldValueList };

            return list;
        }

        //Get Auto suggestion for boost and bury.
        public List<string> GetAutoSuggestion(BoostAndBuryParameterModel boostAndBuryParameterModel)
        {
            string endpoint = SearchBoostAndBuryRuleEndpoint.GetAutoSuggestion();
            ApiStatus status = new ApiStatus();
            BoostAndBuryAutocompleteResponse response = PostResourceToEndpoint<BoostAndBuryAutocompleteResponse>(endpoint, JsonConvert.SerializeObject(boostAndBuryParameterModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.AutoCompleteList;
        }
    }
}
