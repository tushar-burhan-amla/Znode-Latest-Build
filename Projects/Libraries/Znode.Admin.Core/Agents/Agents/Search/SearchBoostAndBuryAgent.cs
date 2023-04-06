using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
namespace Znode.Engine.Admin.Agents
{
    public class SearchBoostAndBuryAgent : BaseAgent, ISearchBoostAndBuryAgent
    {
        private readonly ISearchBoostAndBuryClient _searchBoostAndBuryClient;
        private readonly IEcommerceCatalogClient _ecommerceCatalogClient;
        public SearchBoostAndBuryAgent(ISearchBoostAndBuryClient searchBoostAndBuryClient, IEcommerceCatalogClient ecommerceCatalogClient)
        {
            _searchBoostAndBuryClient = GetClient<ISearchBoostAndBuryClient>(searchBoostAndBuryClient);
            _ecommerceCatalogClient = GetClient<IEcommerceCatalogClient>(ecommerceCatalogClient);
        }

        #region Public Methods
        //To get boost and bury list.
        [Obsolete("This method is deprecated and will be discontinued in upcoming versions." +
        " This method is marked as obsolute because catalogId will be fetched from the filters" +
        " Please use overload of this method which does not have catalogId and catalogName as parameters ")]
        public SearchBoostAndBuryRuleListViewModel GetBoostAndBuryRules(FilterCollectionDataModel model, int catalogId, string catalogName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { catalogId = catalogId, catalogName = catalogName });
            if (catalogId <= 0)
            {
                List<PublishCatalogModel> publishCatalogList = _ecommerceCatalogClient.GetPublishCatalogList(null, null, null, null, null).PublishCatalogs;
                catalogId = (publishCatalogList?.OrderBy(m => m.CatalogName).FirstOrDefault()?.PublishCatalogId).GetValueOrDefault();
                catalogName = (publishCatalogList?.OrderBy(m => m.CatalogName).FirstOrDefault()?.CatalogName);
            }

            model.Filters.RemoveAll(filter => filter.FilterName == ZnodeSearchGlobalProductBoostEnum.PublishCatalogId.ToString());
            model.Filters.Add(new FilterTuple(ZnodeSearchGlobalProductBoostEnum.PublishCatalogId.ToString(), FilterOperators.Equals, catalogId.ToString()));

            SearchBoostAndBuryRuleListModel searchProfileList = _searchBoostAndBuryClient.GetBoostAndBuryRules(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);


            SearchBoostAndBuryRuleListViewModel listViewModel = new SearchBoostAndBuryRuleListViewModel { SearchBoostAndBuryRuleList = searchProfileList?.SearchBoostAndBuryRuleList?.ToViewModel<SearchBoostAndBuryRuleViewModel>().ToList(), PublishCatalogId = catalogId, CatalogName = catalogName };
            SetListPagingData(listViewModel, searchProfileList);

            //Set the Tool Menus for Search Profile List Grid View.
            SetSearchProfileToolMenus(listViewModel);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchProfileList?.SearchBoostAndBuryRuleList?.Count > 0 ? listViewModel : new SearchBoostAndBuryRuleListViewModel() { SearchBoostAndBuryRuleList = new List<SearchBoostAndBuryRuleViewModel>(), PublishCatalogId = catalogId, CatalogName = catalogName };
        }

        //To get boost and bury list.
        public SearchBoostAndBuryRuleListViewModel GetBoostAndBuryRules(FilterCollectionDataModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            
            List<PublishCatalogModel> publishCatalogList = _ecommerceCatalogClient.GetPublishCatalogList(null, null, null, null, null).PublishCatalogs;

            int catalogId = GetCatalogIdFromFilters(model?.Filters);

            if (catalogId == 0)
            {
                catalogId = (publishCatalogList?.OrderBy(m => m.CatalogName).FirstOrDefault()?.PublishCatalogId).GetValueOrDefault();
                UpdateFilters(model?.Filters, FilterKeys.PublishCatalogId, FilterOperators.Equals, catalogId.ToString());
            }
            string catalogName = publishCatalogList?.FirstOrDefault(x => x.PublishCatalogId == catalogId)?.CatalogName;

            SearchBoostAndBuryRuleListModel searchProfileList = _searchBoostAndBuryClient.GetBoostAndBuryRules(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            
            SearchBoostAndBuryRuleListViewModel listViewModel = new SearchBoostAndBuryRuleListViewModel { SearchBoostAndBuryRuleList = searchProfileList?.SearchBoostAndBuryRuleList?.ToViewModel<SearchBoostAndBuryRuleViewModel>().ToList(), PublishCatalogId = catalogId, CatalogName = catalogName };
            SetListPagingData(listViewModel, searchProfileList);

            //Set the Tool Menus for Search Profile List Grid View.
            SetSearchProfileToolMenus(listViewModel);

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchProfileList?.SearchBoostAndBuryRuleList?.Count > 0 ? listViewModel : new SearchBoostAndBuryRuleListViewModel() { SearchBoostAndBuryRuleList = new List<SearchBoostAndBuryRuleViewModel>(), PublishCatalogId = catalogId, CatalogName = catalogName };
        }

        //To get the publish catalog Id from filters.
        protected virtual int GetCatalogIdFromFilters(FilterCollection filters)
        {
            int publishCatalogId = Convert.ToInt32(filters?.FirstOrDefault(filterTuple => filterTuple.Item1 == FilterKeys.PublishCatalogId.ToString().ToLower())?.Item3);
            return publishCatalogId;
        }

        //To update filter with specified details.
        protected virtual void UpdateFilters(FilterCollection filters, string filterName, string filterOperator, string filterValue)
        {
            if (Equals(filters?.Exists(x => x.Item1 == filterName), true))
            {
                filters?.RemoveAll(x => x.Item1 == filterName);
            }
            filters?.Add(new FilterTuple(filterName, filterOperator, filterValue));
        }

        //Create boost and bury rule.
        public virtual SearchBoostAndBuryRuleViewModel CreateBoostAndBuryRule(SearchBoostAndBuryRuleViewModel searchBoostAndBuryRuleViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            try
            {
                return _searchBoostAndBuryClient.CreateBoostAndBuryRule(searchBoostAndBuryRuleViewModel?.ToModel<SearchBoostAndBuryRuleModel>())?.ToViewModel<SearchBoostAndBuryRuleViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    default:
                        return (SearchBoostAndBuryRuleViewModel)GetViewModelWithErrorMessage(searchBoostAndBuryRuleViewModel, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (SearchBoostAndBuryRuleViewModel)GetViewModelWithErrorMessage(searchBoostAndBuryRuleViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Get boost and bury rule.
        public virtual SearchBoostAndBuryRuleViewModel GetBoostAndBuryRule(int searchRuleId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            SearchBoostAndBuryRuleModel webStoreCaseRequestModel = _searchBoostAndBuryClient.GetBoostAndBuryRule(searchRuleId);
            if (webStoreCaseRequestModel?.SearchCatalogRuleId > 0)
                return webStoreCaseRequestModel.ToViewModel<SearchBoostAndBuryRuleViewModel>();

            return null;
        }

        //Update boost and bury rule.
        public virtual SearchBoostAndBuryRuleViewModel UpdateBoostAndBuryRule(SearchBoostAndBuryRuleViewModel searchBoostAndBuryRuleViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            try
            {
                return _searchBoostAndBuryClient.UpdateBoostAndBuryRule(searchBoostAndBuryRuleViewModel?.ToModel<SearchBoostAndBuryRuleModel>())?.ToViewModel<SearchBoostAndBuryRuleViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (SearchBoostAndBuryRuleViewModel)GetViewModelWithErrorMessage(searchBoostAndBuryRuleViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }


        //Delete existing Attribute by attribute Id.
        public virtual bool DeleteCatalogSearchRule(string searchRuleId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(searchRuleId))
            {
                try
                {
                    return _searchBoostAndBuryClient.Delete(new ParameterModel { Ids = searchRuleId });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        default:
                            errorMessage = Admin_Resources.ErrorFailedToDelete;
                            return false;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                    errorMessage = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
            }
            return false;
        }


        public virtual bool PauseCatalogSearchRule(string searchRuleId, bool isPause, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(searchRuleId))
            {
                try
                {
                    return _searchBoostAndBuryClient.PausedSearchRule(new ParameterModel { Ids = searchRuleId }, isPause);
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.NotPermitted:
                            if (isPause)
                                errorMessage = Admin_Resources.PauseSearchRuleNotPermitted;
                            else
                                errorMessage = Admin_Resources.RestartSearchRuleNotPermitted;
                            break;
                        default:
                            errorMessage = Admin_Resources.ErrorMessageFailedToPauseOrRestartRule;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                    errorMessage = Admin_Resources.ErrorMessageFailedToPauseOrRestartRule;
                }
            }
            return false;
        }

        //Check whether the rule name already exists.
        public virtual bool IsRuleNameExist(string ruleName, int publishCatalogId, int searchCatalogRuleId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            if (!string.IsNullOrEmpty(ruleName))
            {
                ruleName = ruleName.Trim();
                FilterCollection filters = new FilterCollection
                {
                    new FilterTuple(ZnodeSearchCatalogRuleEnum.RuleName.ToString(), FilterOperators.Is, ruleName)
                };

                //Get the rule name list based on the rule name filter.
                var ruleList = _searchBoostAndBuryClient.GetBoostAndBuryRules(null, filters, null, null, null);
                if (ruleList?.SearchBoostAndBuryRuleList?.Count > 0)
                {
                    if (searchCatalogRuleId > 0)
                        //Set the status in case the rule name is open in edit mode.
                        ruleList.SearchBoostAndBuryRuleList.RemoveAll(x => x.SearchCatalogRuleId == searchCatalogRuleId);

                    return ruleList.SearchBoostAndBuryRuleList.FindIndex(x => x.RuleName.Equals(ruleName, StringComparison.InvariantCultureIgnoreCase)) != -1;
                }
            }

            return false;
        }

        //Get Auto suggestion for boost and bury.
        public List<string> GetAutoSuggestion(string query, string fieldName, int publishCatalogId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            List<string> autoSuggestions = _searchBoostAndBuryClient.GetAutoSuggestion(new BoostAndBuryParameterModel { PublishCatalogId = publishCatalogId, FieldName = fieldName, SearchTerm = query });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return autoSuggestions;
        }
        #endregion

        #region Private Methods
        private void SetSearchProfileToolMenus(SearchBoostAndBuryRuleListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new Models.GridModel();
                model.GridModel.FilterColumn = new Models.FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<Models.ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('SearchCatalogRuleDeletePopup')", ControllerName = "Search", ActionName = "DeleteCatalogSearchRule" });
            }
        }

        //Bind page drop down details.
        public virtual SearchBoostAndBuryRuleViewModel BindPageDropdown(SearchBoostAndBuryRuleViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            try
            {
                //Get searchable field list.
                model.SearchConditionList = GetSearchableFieldList(model.PublishCatalogId);
                model.IsSearchIndexExists = true;
            }
            catch (ZnodeException ex)
            {
                if (ex.ErrorMessage == "Search index does not exist.")
                    model.IsSearchIndexExists = false;
                model.ErrorMessage = ex.ErrorMessage;
                model.SearchConditionList = new List<FieldValueViewModel>();
            }
            finally
            {
                //Get search trigger operators list.
                model.TriggerOperatorList = GetTriggerOperatorList();

                //Get search item text operators list.
                model.SearchItemTextOperatorList = GetSearchItemTextOperatorList();

                //Get search item numeric operators list.
                model.SearchItmeNumericOperatorList = GetSearchItemNumericOperatorList();
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return model;
        }

        //Get searchable field list.
        protected virtual List<FieldValueViewModel> GetSearchableFieldList(int publishCatalogId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            SearchBoostAndBuryRuleModel searchableFieldListModel = _searchBoostAndBuryClient.GetSearchableFieldList(publishCatalogId);
            List<FieldValueViewModel> searchableFieldList = new List<FieldValueViewModel>();
            if (searchableFieldListModel?.SearchableFieldValueList?.Count > 0)
            {
                List<FieldValueModel> list = searchableFieldListModel.SearchableFieldValueList.Where(s => !string.IsNullOrWhiteSpace(s.AttributeName)).Distinct().OrderBy(x => x.AttributeCode).ToList();
                list?.ForEach(item =>
                {
                    FieldValueViewModel fieldValueViewModel = new FieldValueViewModel
                    {
                        AttributeCode = item.AttributeCode,
                        AttributeName = item.AttributeName,
                        AttributeType = item.AttributeType
                    };
                    searchableFieldList.Add(fieldValueViewModel);
                });
            }
            ZnodeLogging.LogMessage("Searchable field list:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchableFieldList?.Count);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchableFieldList;
        }

        //Get search trigger operators list.
        protected virtual List<SelectListItem> GetTriggerOperatorList()
        {
            List<SelectListItem> triggerOptions = new List<SelectListItem>
            {
                new SelectListItem() { Text = SearchConditionsEnum.Contains.ToString(), Value = SearchConditionsEnum.Contains.ToString() },
                new SelectListItem() { Text = SearchConditionsEnum.Is.ToString(), Value = SearchConditionsEnum.Is.ToString() },
            };
            ZnodeLogging.LogMessage("Trigger operator list count:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, triggerOptions?.Count);
            return triggerOptions;
        }


        //Get search item text operators list.
        protected virtual List<SelectListItem> GetSearchItemTextOperatorList()
        {
            List<SelectListItem> textOperatorList = new List<SelectListItem>
            {
                new SelectListItem() { Text = SearchConditionsEnum.Contains.ToString(), Value = SearchConditionsEnum.Contains.ToString() },
                new SelectListItem() { Text = SearchConditionsEnum.Is.ToString(), Value = SearchConditionsEnum.Is.ToString() },
                new SelectListItem() { Text = "Start With", Value = SearchConditionsEnum.StartWith.ToString() },
                new SelectListItem() { Text = "End With", Value = SearchConditionsEnum.EndWith.ToString() }
            };
            ZnodeLogging.LogMessage("Search item text operator list:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, textOperatorList?.Count);
            return textOperatorList;
        }


        //Get search item numeric operators list.
        protected virtual List<SelectListItem> GetSearchItemNumericOperatorList()
        {
            List<SelectListItem> numericOperatorList = new List<SelectListItem>
            {
                new SelectListItem() { Text = "Equals to (=)", Value = "et" },
                new SelectListItem() { Text = "Greater than (>)", Value = "gt" },
                new SelectListItem() { Text = "Less than (<)", Value = "lt" },
                new SelectListItem() { Text = "Greater than equal to (>=)", Value =  "gte"},
                new SelectListItem() { Text = "Less than equal to (<=)", Value = "lte" }
            };
            ZnodeLogging.LogMessage("Search item numeric operator list:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, numericOperatorList?.Count);
            return numericOperatorList;
        }
        #endregion

    }
}


