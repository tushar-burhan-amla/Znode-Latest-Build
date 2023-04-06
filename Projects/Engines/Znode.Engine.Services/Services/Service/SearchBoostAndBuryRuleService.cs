using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;

using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.ElasticSearch;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class SearchBoostAndBuryRuleService : BaseService, ISearchBoostAndBuryRuleService
    {
        #region Constructor

        public SearchBoostAndBuryRuleService()
        {
        }

        #endregion Constructor

        public SearchBoostAndBuryRuleListModel GetBoostAndBuryRules(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            //set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel generated to set SP parameters to get searchboostAndBuryList: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IZnodeViewRepository<SearchBoostAndBuryRuleModel> objStoredProc = new ZnodeViewRepository<SearchBoostAndBuryRuleModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<SearchBoostAndBuryRuleModel> searchboostAndBuryList = objStoredProc.ExecuteStoredProcedureList("Znode_GetSearchRuleDetails @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("searchboostAndBuryList count: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchboostAndBuryList?.Count);

            SearchBoostAndBuryRuleListModel searchBoostAndBuryRuleListModel = new SearchBoostAndBuryRuleListModel();

            searchBoostAndBuryRuleListModel.SearchBoostAndBuryRuleList = searchboostAndBuryList?.Count > 0 ? searchboostAndBuryList?.ToList() : new List<SearchBoostAndBuryRuleModel>();

            searchBoostAndBuryRuleListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return searchBoostAndBuryRuleListModel;
        }

        //Create boost and bury rule.
        public virtual SearchBoostAndBuryRuleModel CreateBoostAndBuryRule(SearchBoostAndBuryRuleModel searchBoostAndBuryRuleModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNull(searchBoostAndBuryRuleModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorSearchBoostAndBuryRuleModelNull);

            //Create or update boost and busy rule.
            int status = CreateUpdateBoostAndBuryRule(searchBoostAndBuryRuleModel);

            if (status == 0)
                throw new ZnodeException(ErrorCodes.CreationFailed, Admin_Resources.ErrorCreateBoostAndBuryRule);
            else
            {
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                return searchBoostAndBuryRuleModel;
            }          
        }

        // Get boost and bury rule on the basis of searchCatalogRuleId.
        public virtual SearchBoostAndBuryRuleModel GetBoostAndBuryRule(int searchCatalogRuleId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter searchCatalogRuleId: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, searchCatalogRuleId);

            if (searchCatalogRuleId <= 0)
                return new SearchBoostAndBuryRuleModel();

            ExecuteSpHelper objStoredProc = new ExecuteSpHelper();
            //SP parameters
            objStoredProc.GetParameter("SearchCatalogRuleId", searchCatalogRuleId, ParameterDirection.Input, SqlDbType.Int);
            DataSet dataSet = objStoredProc.GetSPResultInDataSet("Znode_GetSearchTriggerItemRuleForEdit");

            //Map search boost and bury rule details.
            return MapSearchBoostAndBuryRuleDetails(dataSet);
        }

        //Update boost and bury rule data.
        public virtual bool UpdateBoostAndBuryRule(SearchBoostAndBuryRuleModel searchBoostAndBuryRuleModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNull(searchBoostAndBuryRuleModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorSearchBoostAndBuryRuleModelNull);
            if (searchBoostAndBuryRuleModel.SearchCatalogRuleId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorSearchCatalogRuleIdLessThanOne);

            //Create or update boost and busy rule.
            int status = CreateUpdateBoostAndBuryRule(searchBoostAndBuryRuleModel);

            if (status == 0) return false; else return true;
        }

        //Paused an existing catalog search rule for a while.
        public bool PausedSearchRule(ParameterModel searchCatalogRuleId, bool isPause)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { searchCatalogRuleId = searchCatalogRuleId?.Ids });

            IZnodeRepository<ZnodeSearchCatalogRule> _searchCatalogRuleRepository = new ZnodeRepository<ZnodeSearchCatalogRule>();
            int ruleId = Convert.ToInt32(searchCatalogRuleId.Ids);
            ZnodeSearchCatalogRule searchCatalogRule = _searchCatalogRuleRepository.GetById(ruleId);

            var currentDate = DateTime.UtcNow;

            if (isPause && HelperUtility.IsNotNull(searchCatalogRule.EndDate) && currentDate >= searchCatalogRule.EndDate)
                throw new ZnodeException(ErrorCodes.NotPermitted, Admin_Resources.FutureEndDateToPauseRule);

            if (!isPause && HelperUtility.IsNotNull(searchCatalogRule.EndDate) && currentDate >= searchCatalogRule.EndDate)
                throw new ZnodeException(ErrorCodes.NotPermitted, Admin_Resources.FutureEndDateToStartRule);
            try
            {
                searchCatalogRule.IsPause = isPause;
                return _searchCatalogRuleRepository.Update(searchCatalogRule);
            }
            catch (Exception ex)
            {
                if (isPause)
                    ZnodeLogging.LogMessage(Admin_Resources.ErrorPauseSearchRule + ex.Message, ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                else
                    ZnodeLogging.LogMessage(Admin_Resources.ErrorRestartSearchRule + ex.Message, ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

                return false;
            }
        }

        //Delete an existing search rule.
        public bool Delete(ParameterModel searchCatalogRuleId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNull(searchCatalogRuleId))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorDeleteMediaModelNull);

            if (string.IsNullOrEmpty(searchCatalogRuleId.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorMediaIdsEmpty);
            ZnodeLogging.LogMessage("Input parameter: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { searchCatalogRuleId = searchCatalogRuleId?.Ids });

            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();

            objStoredProc.SetParameter(ZnodeSearchCatalogRuleEnum.SearchCatalogRuleId.ToString(), searchCatalogRuleId.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);

            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteSearchCatalogRule @SearchCatalogRuleId,@Status OUT", 1, out status);
            ZnodeLogging.LogMessage("Deleted result count:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, deleteResult.Count());

            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(Admin_Resources.SuccessDeleteSearchCatalogRule, ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorDeleteSearchCatalogRule, ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                return false;
            }
        }

        //Get searchable field list.
        public SearchBoostAndBuryRuleModel GetSearchableFieldList(int publishCatalogId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            try
            {
                string indexName = GetService<ISearchService>().GetCatalogIndexName(publishCatalogId);
                ZnodeLogging.LogMessage("indexName created:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, indexName);

                if (string.IsNullOrEmpty(indexName))
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorIndexNameBlank);

                string indexPointingToAlias = GetService<IDefaultDataService>().GetIndicesPointingToAlias(indexName)?.FirstOrDefault();

                if (!string.IsNullOrEmpty(indexPointingToAlias))
                    indexName = indexPointingToAlias;

                List<string> fieldList = GetService<IElasticSearchBaseService>().FieldValueList(indexName, "all");
                ZnodeLogging.LogMessage("fieldList count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, fieldList?.Count);

                List<FieldValueModel> fieldValueModelList = new List<FieldValueModel>();
                if (fieldList.Count > 0)
                {


                    List<ZnodePublishCatalogAttributeEntity> attributeList = GetService<IPublishedCatalogDataService>().GetPublishCatalogAttribute(publishCatalogId, GetDefaultLocaleId(), GetCatalogVersionId(publishCatalogId, ZnodePublishStatesEnum.PRODUCTION));
                    ZnodeLogging.LogMessage("attributeList count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, attributeList?.Count);

                    foreach (string item in fieldList)
                    {
                        FieldValueModel fieldValueModel = new FieldValueModel();
                        fieldValueModel.AttributeCode = item;
                        switch (item)
                        {
                            case "ratings":
                                fieldValueModel.AttributeName = "Ratings";
                                fieldValueModel.AttributeType = "number";
                                break;
                            case "totalreviewcount":
                                fieldValueModel.AttributeName = "Total Reviews";
                                fieldValueModel.AttributeType = "number";
                                break;
                            case "categoryname":
                                fieldValueModel.AttributeName = "Category Name";
                                fieldValueModel.AttributeType = "text";
                                break;
                            default:
                                {
                                    ZnodePublishCatalogAttributeEntity attribute = attributeList.FirstOrDefault(x => x.AttributeCode.Equals(item, StringComparison.InvariantCultureIgnoreCase));
                                    fieldValueModel.AttributeName = attribute?.AttributeName;
                                    fieldValueModel.AttributeType = string.Equals(attribute?.AttributeTypeName, "number", StringComparison.InvariantCultureIgnoreCase) ? "number" : "text";

                                }
                                break;
                        }
                        fieldValueModelList.Add(fieldValueModel);
                    }
                    ZnodeLogging.LogMessage("fieldValueModelList count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, fieldList?.Count);
                }
                return new SearchBoostAndBuryRuleModel() { SearchableFieldValueList = fieldValueModelList };
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                if (ex.ErrorMessage == Admin_Resources.SearchIndexNotExist)
                    throw new ZnodeException(ErrorCodes.ExceptionalError, ex.ErrorMessage);
                throw new ZnodeException(ErrorCodes.ExceptionalError, Admin_Resources.ErrorGetSearchableFieldList);
            }
        }

        //Get Auto suggestion for boost and bury.
        public List<string> GetAutoSuggestion(BoostAndBuryParameterModel parameterModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(parameterModel.FieldName))
                throw new ZnodeException(ErrorCodes.InvalidData, "The field name is required to get Suggestion for specified field name.");

            if (string.IsNullOrEmpty(parameterModel.SearchTerm))
                throw new ZnodeException(ErrorCodes.InvalidData, "The search term is required to get suggestion over the field name.");

            if (parameterModel.PublishCatalogId <= 0)
                throw new ZnodeException(ErrorCodes.InvalidData, "CatalogId cannot be null or 0.");

            string indexName = GetService<ISearchService>().GetCatalogIndexName(parameterModel.PublishCatalogId);
            ZnodeLogging.LogMessage("indexName created:", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, indexName);

            if (string.IsNullOrEmpty(indexName))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorIndexNameBlank);

            int localeId = GetDefaultLocaleId();

            int? catalogVersionId = GetCatalogVersionId(parameterModel.PublishCatalogId, localeId);
          
            List<string> fieldList = GetService<IElasticSearchBaseService>().GetBoostAndBuryAutoSuggestion(indexName, parameterModel.PublishCatalogId, parameterModel.FieldName, parameterModel.SearchTerm, catalogVersionId, localeId);
            ZnodeLogging.LogMessage("fieldList count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, fieldList?.Count);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            return fieldList;
        }

        #region Private / Protected methods
        //Set search rule trigger datatable for sp.
        protected virtual DataTable SetSearchRuleTriggerForSP(List<SearchTriggerRuleModel> SearchTriggerRuleModel, int SearchCatalogRuleId)
        {
            DataTable table = new DataTable("SearchRuleTriggerDetail");
            table.Columns.Add("SearchTriggerKeyword", typeof(string));
            table.Columns.Add("SearchTriggerCondition", typeof(string));
            table.Columns.Add("SearchTriggerValue", typeof(string));
            table.Columns.Add("SearchCatalogRuleId", typeof(int));
            table.Columns.Add("SearchTriggerRuleId", typeof(int));

            foreach (SearchTriggerRuleModel item in SearchTriggerRuleModel)
                table.Rows.Add(Convert.ToString(item.SearchTriggerKeyword), Convert.ToString(item.SearchTriggerCondition), Convert.ToString(item.SearchTriggerValue), Convert.ToString(SearchCatalogRuleId), Convert.ToString(item.SearchTriggerRuleId));

            return table;
        }

        //Set search item rule datatable for sp.
        protected virtual DataTable SetSearchItemRuleForSP(List<SearchItemRuleModel> searchItemRuleModel, int SearchCatalogRuleId)
        {
            DataTable table = new DataTable("SearchRuleItemDetail");
            table.Columns.Add("SearchItemKeyword", typeof(string));
            table.Columns.Add("SearchItemCondition", typeof(string));
            table.Columns.Add("SearchItemValue", typeof(string));
            table.Columns.Add("SearchItemBoostValue", typeof(string));
            table.Columns.Add("SearchCatalogRuleId", typeof(int));
            table.Columns.Add("SearchItemRuleId", typeof(int));

            foreach (SearchItemRuleModel item in searchItemRuleModel)
                table.Rows.Add(Convert.ToString(item.SearchItemKeyword), Convert.ToString(item.SearchItemCondition), Convert.ToString(item.SearchItemValue), Convert.ToString(item.SearchItemBoostValue), Convert.ToString(SearchCatalogRuleId), Convert.ToString(item.SearchItemRuleId));

            return table;
        }

        //Map search boost and bury rule details.
        protected virtual SearchBoostAndBuryRuleModel MapSearchBoostAndBuryRuleDetails(DataSet dataSet)
        {
            SearchBoostAndBuryRuleModel searchBoostAndBuryRuleModel = new SearchBoostAndBuryRuleModel();

            if (dataSet.Tables?.Count > 0)
            {
                DataTable table = dataSet.Tables[0];

                foreach (DataRow row in table.Rows)
                {
                    searchBoostAndBuryRuleModel.PublishCatalogId = Convert.ToInt32(row["PublishCatalogId"]);
                    searchBoostAndBuryRuleModel.CatalogName = Convert.ToString(row["CatalogName"]);
                    searchBoostAndBuryRuleModel.SearchCatalogRuleId = Convert.ToInt32(row["SearchCatalogRuleId"]);
                    searchBoostAndBuryRuleModel.RuleName = Convert.ToString(row["RuleName"]);
                    searchBoostAndBuryRuleModel.StartDate = Convert.ToDateTime(row["StartDate"]);
                    searchBoostAndBuryRuleModel.IsGlobalRule = Convert.ToBoolean(row["IsGlobalRule"]);
                    searchBoostAndBuryRuleModel.IsTriggerForAll = Convert.ToBoolean(row["IsTriggerForAll"]);
                    searchBoostAndBuryRuleModel.IsItemForAll = Convert.ToBoolean(row["IsItemForAll"]);
                    searchBoostAndBuryRuleModel.IsPause = Convert.ToBoolean(row["IsPause"]);

                    if (Convert.IsDBNull(row["EndDate"])) searchBoostAndBuryRuleModel.EndDate = null;
                    else searchBoostAndBuryRuleModel.EndDate = Convert.ToDateTime(row["EndDate"]);

                }

                List<SearchTriggerRuleModel> searchTriggerRuleList = dataSet.Tables[1]?.AsEnumerable().Select(m => new SearchTriggerRuleModel()
                {
                    SearchCatalogRuleId = m.Field<int>("SearchCatalogRuleId"),
                    SearchTriggerRuleId = m.Field<int>("SearchTriggerRuleId"),
                    SearchTriggerKeyword = m.Field<string>("SearchTriggerKeyword"),
                    SearchTriggerCondition = m.Field<string>("SearchTriggerCondition"),
                    SearchTriggerValue = m.Field<string>("SearchTriggerValue"),
                }).ToList();

                List<SearchItemRuleModel> SearchItemRuleList = dataSet.Tables[2]?.AsEnumerable().Select(m => new SearchItemRuleModel()
                {
                    SearchItemRuleId = m.Field<int>("SearchItemRuleId"),
                    SearchCatalogRuleId = m.Field<int>("SearchCatalogRuleId"),
                    SearchItemKeyword = m.Field<string>("SearchItemKeyword"),
                    SearchItemCondition = m.Field<string>("SearchItemCondition"),
                    SearchItemValue = m.Field<string>("SearchItemValue"),
                    SearchItemBoostValue = m.Field<decimal?>("SearchItemBoostValue"),
                }).ToList();

                searchBoostAndBuryRuleModel.SearchTriggerRuleList = searchTriggerRuleList;
                searchBoostAndBuryRuleModel.SearchItemRuleList = SearchItemRuleList;
            }
            ZnodeLogging.LogMessage("PublishCatalogId, CatalogName and SearchCatalogRuleId properties of searchBoostAndBuryRuleModel to be returned: ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new { PublishCatalogId = searchBoostAndBuryRuleModel?.PublishCatalogId, CatalogName = searchBoostAndBuryRuleModel?.CatalogName, SearchCatalogRuleId = searchBoostAndBuryRuleModel?.SearchCatalogRuleId });
            return searchBoostAndBuryRuleModel;
        }

        //Create or update boost and busy rule.
        protected virtual int CreateUpdateBoostAndBuryRule(SearchBoostAndBuryRuleModel searchBoostAndBuryRuleModel)
        {
            //Set search rule trigger datatable for sp.
            DataTable searchRuleTriggerDetail = HelperUtility.IsNotNull(searchBoostAndBuryRuleModel.SearchTriggerRuleList) ? SetSearchRuleTriggerForSP(searchBoostAndBuryRuleModel.SearchTriggerRuleList, searchBoostAndBuryRuleModel.SearchCatalogRuleId) : null;

            //Set search item rule datatable for sp.
            DataTable SearchRuleItemDetail = HelperUtility.IsNotNull(searchBoostAndBuryRuleModel.SearchItemRuleList) ? SetSearchItemRuleForSP(searchBoostAndBuryRuleModel.SearchItemRuleList, searchBoostAndBuryRuleModel.SearchCatalogRuleId) : null;

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodeSearchCatalogRuleEnum.SearchCatalogRuleId.ToString(), searchBoostAndBuryRuleModel.SearchCatalogRuleId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodeSearchCatalogRuleEnum.PublishCatalogId.ToString(), searchBoostAndBuryRuleModel.PublishCatalogId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodeSearchCatalogRuleEnum.RuleName.ToString(), searchBoostAndBuryRuleModel.RuleName, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodeSearchCatalogRuleEnum.StartDate.ToString(), searchBoostAndBuryRuleModel.StartDate, ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter(ZnodeSearchCatalogRuleEnum.EndDate.ToString(), HelperUtility.IsNull(searchBoostAndBuryRuleModel.EndDate) ? Convert.ToDateTime("1/1/1754 12:00:00") : searchBoostAndBuryRuleModel.EndDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59), ParameterDirection.Input, DbType.DateTime);
            objStoredProc.SetParameter(ZnodeSearchCatalogRuleEnum.IsGlobalRule.ToString(), searchBoostAndBuryRuleModel.IsGlobalRule, ParameterDirection.Input, DbType.Boolean);
            objStoredProc.SetParameter(ZnodeSearchCatalogRuleEnum.IsTriggerForAll.ToString(), searchBoostAndBuryRuleModel.IsTriggerForAll, ParameterDirection.Input, DbType.Boolean);
            objStoredProc.SetParameter(ZnodeSearchCatalogRuleEnum.IsItemForAll.ToString(), searchBoostAndBuryRuleModel.IsItemForAll, ParameterDirection.Input, DbType.Boolean); 

            int status = 0;

            if (DefaultGlobalConfigSettingHelper.IsColumnEncryptionSettingEnabled)
            {
                objStoredProc.SetParameter("SearchRuleTriggerDetail", searchRuleTriggerDetail?.ToJson(), ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("SearchRuleItemDetail", SearchRuleItemDetail?.ToJson(), ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Boolean);
                objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateSearchCatalogRuleWithJSON @SearchCatalogRuleId,@PublishCatalogId,@RuleName,@StartDate,@EndDate,@IsGlobalRule,@IsTriggerForAll,@SearchRuleTriggerDetail,@IsItemForAll,@SearchRuleItemDetail,@UserId,@Status OUT", 11, out status);
            }
            else
            {
                objStoredProc.SetTableValueParameter("SearchRuleTriggerDetail", searchRuleTriggerDetail, ParameterDirection.Input, SqlDbType.Structured, "dbo.SearchRuleTriggerDetail");
                objStoredProc.SetTableValueParameter("SearchRuleItemDetail", SearchRuleItemDetail, ParameterDirection.Input, SqlDbType.Structured, "dbo.SearchRuleItemDetail");
                objStoredProc.SetParameter("UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Boolean);
                objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateSearchCatalogRule @SearchCatalogRuleId,@PublishCatalogId,@RuleName,@StartDate,@EndDate,@IsGlobalRule,@IsTriggerForAll,@SearchRuleTriggerDetail,@IsItemForAll,@SearchRuleItemDetail,@UserId,@Status OUT", 11, out status);
            }
                return status;
        }
        #endregion

    }
}
