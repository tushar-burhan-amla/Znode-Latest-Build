using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;
using System.Linq;
using static Znode.Libraries.ECommerce.Utilities.SessionHelper;

namespace Znode.Engine.Admin.Helpers
{
    public class PageDataBinder : DefaultModelBinder
    {
        Dictionary<string, string> _params = new Dictionary<string, string>();
        private string _viewMode = ViewModeTypes.List.ToString();
        private int count { get; set; }
        /// <summary>
        /// Bind Model for filter and sort results
        /// </summary>
        /// <param name="controllerContext">controllerContext</param>
        /// <param name="bindingContext">bindingContext</param>
        /// <returns>Returns FilterCollectionDataModel</returns>
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            //checks for the model is sub class of BaseViewModel
            if (bindingContext.ModelType.IsSubclassOf(typeof(BaseViewModel)))
            {
                HttpRequestBase request = controllerContext.HttpContext.Request;

                int _page = 1;
                int _recordPerPage = HelperMethods.GridPagingStartValue;
                string _sortKey = string.Empty;
                string _sortDir = DynamicGridConstants.ASCKey;

                SortCollection sortCollection = GetSortCollection(request, ref _page, ref _recordPerPage, ref _sortKey, ref _sortDir);
                if (HelperUtility.IsNotNull(sortCollection) || HelperUtility.IsNotNull(GetDataFromSession<SortCollection>(DynamicGridConstants.SortCollectionSessionKey)))
                    sortCollection = GetSortCollection(sortCollection);

                if (Equals(GetDataFromSession<int?>("DefaultPageSize"), null))
                    SaveDataInSession<int?>("DefaultPageSize", (int?)_recordPerPage);
                else
                  _recordPerPage = Convert.ToInt32(GetDataFromSession<int?>("DefaultPageSize"));


                
                //gets the filter collection data model
                return new FilterCollectionDataModel()
                {
                    Page = _page,
                    RecordPerPage = _recordPerPage,
                    SortCollection = sortCollection,
                    Filters = GetFilterCollection(request),
                    Params = _params,
                    ViewMode = _viewMode,
                    LinkPermission = GetAuthorizedLinkPermission()
                };
            }
            else
                //if it is not subclass of BaseViewModel then call the base method for bind the model
                return base.BindModel(controllerContext, bindingContext);
        }

        /// <summary>
        /// Get sort collection from session or sortCollection
        /// </summary>
        /// <param name="sortCollection">sortCollection</param>
        /// <returns></returns>
        private SortCollection GetSortCollection(SortCollection sortCollection)
        {
            SortCollection sortSessionCollection = GetDataFromSession<SortCollection>(DynamicGridConstants.SortCollectionSessionKey);
            if (HelperUtility.IsNotNull(sortSessionCollection) && sortSessionCollection.Count > 0)
            {
                if (GetDataFromSession<bool?>(DynamicGridConstants.IsViewRequest) == true)
                    sortCollection = sortSessionCollection;
                else
                    SaveDataInSession<SortCollection>(DynamicGridConstants.SortCollectionSessionKey, sortCollection);
            }
            else
            {
                if (GetDataFromSession<bool?>(DynamicGridConstants.IsViewRequest) != true)
                    SaveDataInSession<SortCollection>(DynamicGridConstants.SortCollectionSessionKey, sortCollection);
                else
                    sortCollection = null;
            }
            return sortCollection;
        }

        /// <summary>
        /// Get sort collection from request
        /// </summary>
        /// <param name="request">Http Request</param>
        /// <param name="_page">ref _page</param>
        /// <param name="_recordPerPage">ref _recordPerPage</param>
        /// <param name="_sortKey">ref _sortKey</param>
        /// <param name="_sortDir">ref _sortDir</param>
        /// <returns>Returns SortCollection</returns>
        private SortCollection GetSortCollection(HttpRequestBase request, ref int _page, ref int _recordPerPage, ref string _sortKey, ref string _sortDir)
        {
            SortCollection sortCollection = new SortCollection();
            Dictionary<string, string> _paginationcollection = new Dictionary<string, string>();

            //collect the query string 
            var queryStringCollection = request.QueryString;

            if (queryStringCollection.Count > 0)
            {
                foreach (var keys in queryStringCollection)
                {
                    if (!Equals(keys, null))
                    {
                        var kayname = keys.ToString();
                        var kayValue = queryStringCollection[kayname].ToString();
                        _paginationcollection.Add(kayname, kayValue);//add the values in pagination collection
                    }
                }
            }

            if (!Equals(_paginationcollection, null) && !Equals(_paginationcollection.Count, 0))
            {
                foreach (var item in _paginationcollection)
                {
                    switch (item.Key.ToLower())
                    {
                        case DynamicGridConstants.PageKey:
                            if (!Equals(item.Value, null) && !Equals(item.Value, string.Empty))
                                _page = int.Parse(item.Value);
                            else
                                _page = 1;//default value
                            break;

                        case DynamicGridConstants.RecordPerPageKey:
                            if (!Equals(item.Value, null) && !Equals(item.Value, string.Empty))
                            {
                                _recordPerPage = int.Parse(item.Value);
                                SaveDataInSession<int?>("DefaultPageSize", (int?)_recordPerPage);
              }
                            else
                                _recordPerPage = 10;//default value
                            break;
                        case DynamicGridConstants.sortKey:
                            if (!Equals(item.Value, null) && !Equals(item.Value, string.Empty))
                                _sortKey = item.Value;
                            break;
                        case DynamicGridConstants.sortDirKey:
                            if (!Equals(item.Value, null) && !Equals(item.Value, string.Empty))
                                _sortDir = item.Value;
                            else
                                _sortDir = DynamicGridConstants.ASCKey;
                            break;
                        case DynamicGridConstants.ViewMode:
                            if (!Equals(item.Value, null) && !Equals(item.Value, string.Empty))
                            {
                                SaveDataInSession<string>(DynamicGridConstants.GridViewMode, item.Value);
                                _viewMode = item.Value;
                            }
                            else
                                _viewMode = GetDataFromSession<string>(DynamicGridConstants.GridViewMode) ?? (ViewModeTypes.List).ToString();
                            break;
                    }

                    if (item.Key != DynamicGridConstants.PageKey && item.Key != DynamicGridConstants.RecordPerPageKey && item.Key != DynamicGridConstants.sortKey && item.Key != DynamicGridConstants.sortDirKey)
                        _params.Add(item.Key, item.Value);
                }
            }

            if (string.IsNullOrEmpty(_sortKey))
                return null;
            else
            {
                sortCollection.Add(_sortKey, _sortDir);
            }
            return sortCollection;
        }

        /// <summary>
        /// Get Filter Collection
        /// </summary>
        /// <param name="request">Http request</param>
        /// <returns>Returns FilterCollection</returns>
        private FilterCollection GetFilterCollection(HttpRequestBase request)
        {
            FilterCollection filters = new FilterCollection();
            var formCollection = request.Form;
            string[] dataOperatorIds = Convert.ToString(formCollection[DynamicGridConstants.DataOperatorId])?.Split(',');
            count = 0;
            foreach (var _keys in formCollection)
            {
                string keyname = _keys.ToString();
                string keyValue = formCollection[keyname].ToString();

                if (!string.IsNullOrEmpty(keyValue) && !Equals(keyname, "X-Requested-With") && !Equals(keyname, DynamicGridConstants.DataOperatorId))
                {
                    if (HelperUtility.IsNotNull(GetDataFromSession<List<dynamic>>(DynamicGridConstants.ColumnListSessionKey)))
                    {
                        string _datatype = FilterHelpers.GetDataTypeByKeyName(keyname);
                        string filterOperator = FilterHelpers.GetFilterOperatorByOperatorId(dataOperatorIds[count]);

                        if (_datatype.Equals("DateTime") || _datatype.Equals("Date"))
                            keyValue = GetFilterCriteriaForDateTimeColumn(filterOperator, keyValue);
                        else if (_datatype.Equals("String") && keyValue.IndexOf("'") >= 0)
                            keyValue = keyValue.Replace("'", "''");
                        else if (_datatype.Equals("Boolean"))
                        {
                            if (keyValue.Contains(','))
                                keyValue = keyValue.Replace(',', ' '); //Replace ',' from bool string
                            if (keyValue.Equals("1", StringComparison.OrdinalIgnoreCase))
                                keyValue = "true";
                            else if (keyValue.Equals("0", StringComparison.OrdinalIgnoreCase))
                                keyValue = "false";
                        }
                        else
                            keyValue = keyValue.Replace(",", "");

                        if (!Equals(keyname, DynamicGridConstants.DataOperatorId) && !Equals(keyname, DynamicGridConstants.ManageSearch))
                        {
                            filters.Add(new FilterTuple(keyname, filterOperator, keyValue));
                            count++;
                        }
                    }
                    else
                    {
                        if (!Equals(keyname, DynamicGridConstants.DataOperatorId))
                            filters.Add(new FilterTuple(keyname, FilterOperators.Contains, keyValue));
                    }
                }
                else
                    RemoveFilterFromSession(keyname);
            }
            return GetModifiedFilterCollection(request.AppRelativeCurrentExecutionFilePath, filters);
        }

        //Get filter value for datetime control
        private string GetFilterCriteriaForDateTimeColumn(string selectedOperator, string controlValue)
        {
            string filter = string.Empty;
            DateTime dt;
            DateTime.TryParse(Convert.ToString(controlValue), out dt);

            switch (selectedOperator)
            {
                case FilterOperators.Between: //On    BETWEEN '{1}' AND '{2}'
                    filter = "'" + dt.ToString() + "' AND '" + dt.AddDays(1).AddSeconds(-1).ToString() + "'";
                    break;
                case FilterOperators.GreaterThan:  //After  &gt; '{1} 23:59:59'
                    filter = "'" + dt.AddDays(1).AddSeconds(-1).ToString() + "'";
                    break;
                case FilterOperators.GreaterThanOrEqual: //OnOrAfter  &gt;= '{1}'
                case FilterOperators.LessThanOrEqual: //OnOrBefore &lt;= '{1}'
                case FilterOperators.LessThan:  //Before &lt; '{1} 23:59:59'
                    filter = "'" + dt.ToString() + "'";
                    break;
                case FilterOperators.NotEquals: //NotOn  != '{1}'
                    filter = dt.ToString();
                    break;
            }
            return filter;
        }

        private FilterCollection GetModifiedFilterCollection(string requestURL, FilterCollection filters)
        {
            string _path = GetDataFromSession<string>(DynamicGridConstants.RelativePathSessionKey);

            //checks for url is same as previous url 
            if (!string.IsNullOrEmpty(_path) && _path.Equals(requestURL))
            {
                FilterCollection tempFilterList = GetDataFromSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey);

                //checks for filter session null or count zero
                if (Equals(tempFilterList, null) || tempFilterList.Count == 0)
                {
                    if (GetDataFromSession<bool?>(DynamicGridConstants.IsViewRequest) != null)
                        filters = new FilterCollection();
                    SaveDataInSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey, filters);
                }
                else
                {
                    Boolean isViewRequest = Convert.ToBoolean(GetDataFromSession<bool?>(DynamicGridConstants.IsViewRequest));
                    //check if it get view request.
                    if (isViewRequest)
                        filters = tempFilterList;
                    else if (filters.Count < 1)
                    {
                        filters = tempFilterList;
                        SaveDataInSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey, filters);
                    }
                        
                    else if (!Equals(tempFilterList, null))
                    {
                        if (!Equals(HelperMethods.GetDateTimeRangeFilterTuple(filters), null))
                            SaveDataInSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey, filters);
                        else
                        {
                            FilterTuple _dateTimeRangeInSession = HelperMethods.GetDateTimeRangeFilterTuple(tempFilterList);
                            //If date time range filter is present in the session, adds it in filter collection
                            if (!Equals(_dateTimeRangeInSession, null))
                                filters.Add(_dateTimeRangeInSession);
                            SaveDataInSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey, filters);
                        }
                    }
                }
            }
            else
            {
                SaveDataInSession<string>(DynamicGridConstants.RelativePathSessionKey, requestURL);
                SaveDataInSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey, filters);
            }
            RemoveDataFromSession(DynamicGridConstants.IsViewRequest);
            return filters;
        }

        //Get Login user Access Permission List.
        private List<string> GetAuthorizedLinkPermission()
        {
            List<string> lstPermission = new List<string>();
            var permission = SessionProxyHelper.GetUserPermission();
            if (!Equals(permission, null) && permission.Count > 0)
            {
                lstPermission = permission.Select(x => x.RequestUrlTemplate).ToList();
            }
            return lstPermission;
        }
        private void RemoveFilterFromSession(string keyname)
        {
            FilterCollection filterList = GetDataFromSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey);

            //Check for null
            if (HelperUtility.IsNotNull(filterList))
            {
                //get the index position of filter to remove
                int index = filterList.FindIndex(x => x.Item1.Contains(keyname));
                //check for greater or equal than zero as index stars from 0
                if (index >= 0)
                {
                    //remove filter from session
                    filterList.RemoveAt(index);
                    //override remaining filters in session
                    SaveDataInSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey, filterList);
                    count++;
                }
            }
        }
    }
}