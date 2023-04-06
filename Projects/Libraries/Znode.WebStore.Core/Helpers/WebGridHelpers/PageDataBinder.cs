using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.Helpers
{
    public class PageDataBinder : DefaultModelBinder
    {
        Dictionary<string, string> _params = new Dictionary<string, string>();
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

                if (Equals(SessionHelper.GetDataFromSession<int?>("DefaultPageSize"), null))
                    SessionHelper.SaveDataInSession<int>("DefaultPageSize", _recordPerPage);
                else
                    _recordPerPage = SessionHelper.GetDataFromSession<int>("DefaultPageSize");

                //gets the filter collection data model
                return new FilterCollectionDataModel()
                {
                    Page = _page,
                    RecordPerPage = _recordPerPage,
                    SortCollection = sortCollection,
                    Filters = GetFilterCollection(request),
                    Params = _params,
                };
            }
            else
                //if it is not subclass of BaseViewModel then call the base method for bind the model
                return base.BindModel(controllerContext, bindingContext);
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
                    if (HelperUtility.IsNotNull(keys))
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
                            if (HelperUtility.IsNotNull(item.Value) && !Equals(item.Value, string.Empty))
                                _page = int.Parse(item.Value);
                            else
                                _page = 1;//default value
                            break;

                        case DynamicGridConstants.RecordPerPageKey:
                            if (HelperUtility.IsNotNull(item.Value) && !Equals(item.Value, string.Empty))
                            {
                                _recordPerPage = int.Parse(item.Value);
                                SessionHelper.SaveDataInSession<int>("DefaultPageSize", _recordPerPage);
                            }
                            else
                                _recordPerPage = 10;//default value
                            break;
                        case DynamicGridConstants.sortKey:
                            if (HelperUtility.IsNotNull(item.Value) && !Equals(item.Value, string.Empty))
                                _sortKey = item.Value;
                            break;
                        case DynamicGridConstants.sortDirKey:
                            if (HelperUtility.IsNotNull(item.Value) && !Equals(item.Value, string.Empty))
                                _sortDir = item.Value;
                            else
                                _sortDir = DynamicGridConstants.ASCKey;
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
            int count = 0;
            foreach (var _keys in formCollection)
            {
                string keyname = _keys.ToString();
                string keyValue = formCollection[keyname].ToString();

                if (!string.IsNullOrEmpty(keyValue) && !Equals(keyname, "X-Requested-With"))
                {
                    if (HelperUtility.IsNotNull(SessionHelper.GetDataFromSession<List<dynamic>>(DynamicGridConstants.ColumnListSessionKey)))
                    {
                        string _datatype = FilterHelpers.GetDataTypeByKeyName(keyname);

                        if (_datatype.Equals("DateTime") || _datatype.Equals("Date"))
                            keyValue = GetFilterCriteriaForDateTimeColumn(FilterHelpers.GetFilterOperatorByOperatorId(dataOperatorIds[count]), keyValue);
                        else if (_datatype.Equals("String") && keyValue.IndexOf("'") >= 0)
                            keyValue = keyValue.Replace("'", "''");
                        else if (_datatype.Equals("Boolean"))
                        {
                            if (keyValue.Equals("1", StringComparison.OrdinalIgnoreCase))
                                keyValue = "true";
                            else if (keyValue.Equals("0", StringComparison.OrdinalIgnoreCase))
                                keyValue = "false";
                        }
                        if (!Equals(keyname, DynamicGridConstants.DataOperatorId) && !Equals(keyname, DynamicGridConstants.ManageSearch))
                        {
                            filters.Add(new FilterTuple(keyname, FilterHelpers.GetFilterOperatorByOperatorId(dataOperatorIds[count]), keyValue));
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

        private FilterCollection GetModifiedFilterCollection(string requestURL, FilterCollection filters)
        {
            //checks for url is same as previous url 
            string url = SessionHelper.GetDataFromSession<string>(DynamicGridConstants.RelativePathSessionKey) ?? string.Empty;
            if (url.Equals(requestURL))
            {
                //checks for filter session null or count zero
                if (HelperUtility.IsNull(SessionHelper.GetDataFromSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey)) || (SessionHelper.GetDataFromSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey).Count == 0))
                    SessionHelper.SaveDataInSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey, filters);
                else
                {
                    var tempFilterList = SessionHelper.GetDataFromSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey);

                    if (HelperUtility.IsNotNull(tempFilterList))
                    {
                        foreach (var filter in filters)
                        {
                            int index = tempFilterList.FindIndex(x => x.Item1 == filter.Item1);
                            if (index >= 0)
                            {
                                //replace the previous filter with new filter 
                                tempFilterList.RemoveAt(index);
                                tempFilterList.Add(filter);
                            }
                            else
                                tempFilterList.Add(filter);
                        }
                        //assign filter to filters
                        filters = tempFilterList;
                    }
                }
            }
            else
            {
                SessionHelper.SaveDataInSession<string>(DynamicGridConstants.RelativePathSessionKey, requestURL);
                SessionHelper.SaveDataInSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey, filters);
            }
            return filters;
        }


        private void RemoveFilterFromSession(string keyname)
        {
            //Check for null
            if (HelperUtility.IsNotNull(SessionHelper.GetDataFromSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey)))
            {
                //get the index position of filter to remove
                int index = SessionHelper.GetDataFromSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey).FindIndex(x => x.Item1 == keyname);
                //check for greater or equal than zero as index stars from 0
                if (index >= 0)
                    //remove filter from session
                    SessionHelper.GetDataFromSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey).RemoveAt(index);
            }
        }

        //Get filter value for datetime control.
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
    }
}