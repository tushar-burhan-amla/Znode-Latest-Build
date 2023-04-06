using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using Znode.Admin.Core.Helpers;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using static Znode.Libraries.ECommerce.Utilities.SessionHelper;

namespace Znode.Engine.Admin.Helpers
{
    public static class FilterHelpers
    {
        /// <summary>
        /// Get filter criteria column list
        /// </summary>
        /// <param name="filterCondition">filter collection</param>
        /// <param name="listType">integer listType</param>
        /// <returns></returns>
        public static FilterColumnListModel GetFilterColumnList(FilterCollection filterCondition, string listType, ApplicationSettingDataModel applicationSettingModel, string xmlSetting = "") => GetConvertedFilterColumnModelFromXml(listType, filterCondition, applicationSettingModel, xmlSetting);

        public static Models.ViewModel SaveListView(string viewName, int viewId, bool isPublic, bool isDefault, ref string errorMessage)
        {
            try
            {
                string sortColumn = null;
                string sortType = null;
                string xml = XmlHelper.ToXmlString(GetDataFromSession<List<dynamic>>(DynamicGridConstants.ColumnListSessionKey));
                string xmlSettingName = GetDataFromSession<string>(DynamicGridConstants.listTypeSessionKey) ?? "";
                var filters = GetDataFromSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey);
                if (!Equals(filters, null))
                    filters.RemoveAll(x => x.FilterName.Contains("|"));
                int userId = SessionProxyHelper.GetUserDetails().UserId;
                var _views = GetDataFromSession<List<Api.Models.ViewModel>>(DynamicGridConstants.ListViewsCollection);
                SortCollection sortSessionCollection = GetDataFromSession<SortCollection>(DynamicGridConstants.SortCollectionSessionKey);
                if (HelperUtility.IsNotNull(sortSessionCollection) && sortSessionCollection.Count() > 0)
                {
                    sortColumn = sortSessionCollection.Keys.FirstOrDefault();
                    sortType = sortSessionCollection[sortColumn];
                }
                return GetService<IDependencyHelper>().SaveListView(viewName, viewId, xml, xmlSettingName, filters, sortColumn, sortType, isPublic, isDefault, userId);
            }
            catch (ZnodeException ex)
            {
                errorMessage = ex.ErrorMessage;
                return new Models.ViewModel { HasError = true };
            }
            catch (Exception)
            {
                errorMessage = Admin_Resources.ErrorSaveView;
                return new Models.ViewModel { HasError = true };
            }
        }

        /// <summary>
        /// Get XML configuration
        /// </summary>
        /// <param name="listType">List type</param>
        /// <returns></returns>
        public static string GetXmlConfiguration(string listType, ApplicationSettingDataModel applicationSettingModel) => GetXMLSettingFromDataBase(applicationSettingModel, new FilterCollectionDataModel(), listType);

        /// <summary>
        /// Clear DynamicGrid Session
        /// </summary>
        public static void ClearDynamicGridSession()
        {
            //clears the session values
            RemoveDataFromSession(DynamicGridConstants.XMLStringSessionKey);
            RemoveDataFromSession(DynamicGridConstants.listTypeSessionKey);
            RemoveDataFromSession(DynamicGridConstants.xmlConfigDocSessionKey);
            RemoveDataFromSession(DynamicGridConstants.NewRowColumns);
            RemoveDataFromSession(DynamicGridConstants.AddNewRowData);
        }

        /// <summary>
        /// Get dynamic column and filters criteria from configuration XML
        /// </summary>
        /// <param name="listType">integer list type</param>
        /// <param name="filterKeyValue">filter citeria values</param>
        /// <returns>Returns FilterColumnListModel</returns>
        public static FilterColumnListModel GetConvertedFilterColumnModelFromXml(string listType, FilterCollection filterKeyValue, ApplicationSettingDataModel applicationSettingModel, string xmlSetting = "")
        {
            string XMlstring = GetDataFromSession<string>(DynamicGridConstants.XMLStringSessionKey);
            //check for data present in session or not
            if (Equals(XMlstring, null) || GetDataFromSession<string>(DynamicGridConstants.listTypeSessionKey) != listType.ToString())
            {
                if (string.IsNullOrEmpty(xmlSetting))
                    XMlstring = GetXmlConfiguration(listType, applicationSettingModel);
                else
                    XMlstring = xmlSetting;
                //inserts into session
                SaveDataInSession<string>(DynamicGridConstants.XMLStringSessionKey, XMlstring);
            }

            //parse the xml string into document
            XDocument xmlDoc = XDocument.Parse(XMlstring);

            //Check for custom column
            GetCustomColumn(xmlDoc);

            SaveDataInSession<XDocument>(DynamicGridConstants.xmlConfigDocSessionKey, xmlDoc);

            //get the fields collection
            int[] _fieldscollection = GetDataFromSession<int[]>(DynamicGridConstants.FieldsCollectionSessionKey);

            List<FilterColumnModel> model = (from xml in xmlDoc.Descendants(DynamicGridConstants.columnKey)
                                             where (xml.Element(DynamicGridConstants.isallowsearchKey).Value.Equals(Convert.ToString(DynamicGridConstants.Yes))) && (!Equals(_fieldscollection, null) ? !_fieldscollection.Contains(int.Parse(xml.Element("id").Value)) : (int.Parse(xml.Element("id").Value) == int.Parse(xml.Element("id").Value)))
                                             select new FilterColumnModel
                                             {
                                                 ColumnName = xml.Element(DynamicGridConstants.nameKey).Value,
                                                 HeaderText = xml.Element(DynamicGridConstants.headertextKey).Value,
                                                 IsAllowSearch = xml.Element(DynamicGridConstants.isallowsearchKey).Value,
                                                 SearchControlType = GetXmlElementValue(DynamicGridConstants.SearchControlTypeKey, xml),
                                                 Id = Convert.ToInt32(xml.Element(DynamicGridConstants.IdKey).Value),
                                                 IsSearchable = Equals(Convert.ToString(xml.Element(DynamicGridConstants.isadvancesearch).Value), DynamicGridConstants.Yes) ? true : false,
                                                 SearchControlParameters = GetXmlElementValue(DynamicGridConstants.SearchControlParametersKey, xml),
                                                 DataType = xml.Element(DynamicGridConstants.datatypeKey).Value,
                                                 SelectListOfDatatype = GetOperators(xml.Element(DynamicGridConstants.datatypeKey)?.Value, (Equals(filterKeyValue, null)
                                                    ? 0 : filterKeyValue.Exists(x => x.Item1 == xml.Element(DynamicGridConstants.nameKey).Value)
                                                    ? Convert.ToInt32(GetFilterOperatorId(filterKeyValue.Find(x => x.Item1 == xml.Element(DynamicGridConstants.nameKey).Value).Item2, xml.Element(DynamicGridConstants.datatypeKey).Value)) : 0)),
                                                 Value = Equals(filterKeyValue, null)
                                                    ? string.Empty : filterKeyValue.Exists(x => x.Item1 == xml.Element(DynamicGridConstants.nameKey).Value)
                                                    ? filterKeyValue.Find(x => x.Item1 == xml.Element(DynamicGridConstants.nameKey).Value).Item3 : string.Empty,
                                                 DataOperatorId = Equals(filterKeyValue, null)
                                                    ? 0 : filterKeyValue.Exists(x => x.Item1 == xml.Element(DynamicGridConstants.nameKey)?.Value)
                                                    ? Convert.ToInt32(GetFilterOperatorId(filterKeyValue.Find(x => x.Item1 == xml.Element(DynamicGridConstants.nameKey).Value).Item2, xml.Element(DynamicGridConstants.datatypeKey).Value)) : 0,
                                             }).ToList();


            //If date time range filter is present, adds it in FilterColumnModel
            if (!Equals(filterKeyValue, null))
            {
                FilterTuple _dateTimeRange = HelperMethods.GetDateTimeRangeFilterTuple(filterKeyValue);
                if (!Equals(_dateTimeRange, null))
                    model.Add(new FilterColumnModel() { Value = _dateTimeRange.FilterValue, DataType = "String", ColumnName = _dateTimeRange.FilterName });
            }

            //Adds the filter column model for global search.
            if (!Equals(filterKeyValue, null))
                foreach (FilterTuple item in filterKeyValue)
                {
                    if (!string.IsNullOrEmpty(item.FilterName))
                    {
                        //If filter contains '|' then add that column to mode.
                        if (item.FilterName.Contains("|"))
                            model.Add(new FilterColumnModel() { Value = item.FilterValue, DataType = "String", ColumnName = item.FilterName, IsGlobalSearch = true });
                    }
                }

            List<int> _tempArr = GetDataFromSession<List<int>>(DynamicGridConstants.FilterState);
            List<int> _removedFilters = GetDataFromSession<List<int>>(DynamicGridConstants.RemovedFilters);
            model.ForEach(x =>
            {
                var _flt = filterKeyValue?.FirstOrDefault(y => y.FilterName == x.ColumnName);
                if (!Equals(_flt, null)) { x.IsSearchable = true; }
                if (!Equals(_tempArr, null) && _tempArr.IndexOf(x.Id) != -1) { x.IsSearchable = true; }
                if (!Equals(_removedFilters, null) && _removedFilters.IndexOf(x.Id) != -1) { x.IsSearchable = false; }
            });

            //returns the result
            return new FilterColumnListModel { FilterColumnList = model, GridColumnList = GetDynamicGridView(xmlDoc.ToString(), DynamicGridConstants.columnKey), IsResourceRequired = GetDataFromSession<bool?>(DynamicGridConstants.IsResourceRequired) == true };
        }

        //Get custom column
        private static void GetCustomColumn(XDocument xmlDoc)
        {
            var _customColumns = GetDataFromSession<List<CustomColumnViewModel>>("CustomColumns");

            //Get Last node id
            int count = xmlDoc.Descendants("id").Count();
            var filesCollectionfromXml = xmlDoc.Descendants("name");

            //Add custom column in XML
            if (!Equals(_customColumns, null) && _customColumns.Count > 0)
            {
                _customColumns.ForEach(x =>
                {
                    string _fields = filesCollectionfromXml.FirstOrDefault(y => y?.Value == x?.FieldName)?.Value;

                    if (string.IsNullOrEmpty(_fields))
                        xmlDoc.Element("columns").Add(XmlHelper.WriteXML(x, ++count));
                });

                if (filesCollectionfromXml?.FirstOrDefault(y => y?.Value == "Manage")?.Parent?.Descendants("id")?.FirstOrDefault() != null)
                {
                    filesCollectionfromXml.FirstOrDefault(y => y?.Value == "Manage").Parent.Descendants("id").FirstOrDefault().Value = Convert.ToString(++count);

                    var _xmlElement = filesCollectionfromXml.FirstOrDefault(y => y?.Value == "Manage").Parent;

                    filesCollectionfromXml.FirstOrDefault(y => y?.Value == "Manage").Parent.Remove();

                    xmlDoc.Element("columns").Add(_xmlElement);
                }
            }
        }

        //Get selected column list
        public static string GetVisibleColumn()
        {
            List<string> visibleColumnName = new List<string>();
            var columnList = GetDataFromSession<List<dynamic>>(DynamicGridConstants.ColumnListSessionKey);

            //First time will fetch column from Grid XML. 
            if (columnList == null)
            {
                string xmlSetting = GetDataFromSession<string>(DynamicGridConstants.XMLStringSessionKey);
                if (!string.IsNullOrEmpty(xmlSetting))
                {
                    XDocument xmlDoc = XDocument.Parse(xmlSetting);
                    visibleColumnName = (from xml in xmlDoc.Descendants(DynamicGridConstants.columnKey)
                                         where xml.Element(DynamicGridConstants.isvisibleKey).Value.Equals(Convert.ToString(DynamicGridConstants.Yes))
                                         select xml.Element(DynamicGridConstants.nameKey).Value).ToList();
                }
            }
            else
            {
                visibleColumnName = (from column in columnList
                                     where column.isvisible == Convert.ToString(DynamicGridConstants.Yes)
                                     select Convert.ToString(column.name)).Cast<string>().ToList();
            }
            return visibleColumnName != null ? string.Join(",", visibleColumnName) : string.Empty;
        }


        //returns the value of the element passed
        private static string GetXmlElementValue(string elementKey, dynamic xml)
        {
            try
            {
                return xml.Element(elementKey).Value;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return string.Empty;
            }
        }

        /// <summary>
        /// Get dynamic grid column names
        /// </summary>
        /// <param name="xmlString">xmlString</param>
        /// <param name="tableName">tableName</param>
        /// <returns>Returns dynamic List</returns>
        public static List<dynamic> GetDynamicGridView(string xmlString, string tableName)
        {
            try
            {
                return ConvertXmlToList(xmlString, tableName);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                throw ex;
            }
        }

        /// <summary>
        /// Method is used for convert Xml to dynamic list
        /// </summary>
        /// <param name="xmlString">xml string</param>
        /// <param name="tableName">tableName</param>
        /// <returns>Returns dynamic list</returns>
        public static List<dynamic> ConvertXmlToList(string xmlString, string tableName)
        {
            DataSet dataset = new DataSet();
            try
            {
                //read the xml string
                using (StringReader stringReader = new StringReader(xmlString))
                {
                    dataset.ReadXml(stringReader);
                }
                return DataTableToList(dataset.Tables[tableName]);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                throw ex;
            }
            finally
            {
                dataset.Dispose();
            }
        }

        /// <summary>
        /// Get column table format to dynamic list with dateFormat
        /// </summary>
        /// <param name="dataTable">DataTable</param>F
        /// <returns>Returns dynamic list</returns>
        public static List<dynamic> DataTableToList(DataTable dataTable)
        {
            var result = new List<dynamic>();
            try
            {
                if (!Equals(dataTable, null) && dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        var expandoObj = (IDictionary<string, object>)new ExpandoObject();
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            if (column.ColumnName.Contains(DynamicGridConstants.Date))
                            {
                                DateTime startDate = Convert.ToDateTime(row[column.ColumnName]);
                                expandoObj.Add(column.ColumnName, startDate.ToString(DynamicGridConstants.DateFormat));
                            }
                            else
                                expandoObj.Add(column.ColumnName, row[column.ColumnName]);
                        }
                        result.Add(expandoObj);
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                throw ex;
            }
            finally
            {
                dataTable.Dispose();
            }
            return result;
        }

        /// <summary>
        /// Get dynamic grid model.
        /// function help to generate grid structure as dynamic.
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="model">filter collection model</param>
        /// <param name="dataModel">Generic type list model</param>
        /// <param name="listType">integer list type</param>
        /// <returns>Returns ReportModel</returns>
        public static GridModel GetDynamicGridModel<T>(FilterCollectionDataModel model, List<T> dataModel, string listType, string xmlSetting = "", Dictionary<string, List<SelectListItem>> dropDownValueList = null, bool isBulkAddRequired = false, bool isPermission = true, List<ToolMenuModel> toolMenuList = null, List<CustomColumnViewModel> customColumns = null)
        {
            GridModel gridModel = new GridModel();

            //If the value is null then set to new.
            if (Equals(dataModel, null))
                dataModel = new List<T>();

            int tableWidth = 0;
            //checks for the list name in session
            if (GetDataFromSession<string>(DynamicGridConstants.listTypeSessionKey) != listType.ToString())
            {
                SaveDataInSession<string>(DynamicGridConstants.listTypeSessionKey, listType.ToString());
                RemoveDataFromSession(DynamicGridConstants.ColumnListSessionKey);
                RemoveDataFromSession(DynamicGridConstants.GridViewMode);
                RemoveDataFromSession(DynamicGridConstants.FilterState);
                RemoveDataFromSession(DynamicGridConstants.RemovedFilters);
                RemoveDataFromSession(DynamicGridConstants.XMLStringSessionKey);

                SaveDataInSession<List<CustomColumnViewModel>>("CustomColumns", customColumns);

                gridModel.FilterColumn = GetFilterColumnList(model.Filters, listType, gridModel.GridSettingModel, xmlSetting);
                gridModel.GridColumnList = gridModel.FilterColumn.GridColumnList;
                SaveDataInSession<List<dynamic>>(DynamicGridConstants.ColumnListSessionKey, gridModel.GridColumnList);
            }
            else
            {
                SaveDataInSession<List<CustomColumnViewModel>>("CustomColumns", GetDataFromSession<List<CustomColumnViewModel>>("CustomColumns") ?? customColumns);
                gridModel.FilterColumn = GetFilterColumnList(model.Filters, listType, gridModel.GridSettingModel, xmlSetting);
                gridModel.GridColumnList = GetDataFromSession<List<dynamic>>(DynamicGridConstants.ColumnListSessionKey) ?? gridModel.FilterColumn.GridColumnList;
                if (HelperUtility.IsNull(GetDataFromSession<List<dynamic>>(DynamicGridConstants.ColumnListSessionKey)))
                    SaveDataInSession<List<dynamic>>(DynamicGridConstants.ColumnListSessionKey, gridModel.FilterColumn.GridColumnList);

            }
            //enter list name in session
            SaveDataInSession<string>(DynamicGridConstants.listTypeSessionKey, listType);

            //adds page size
            int pageSize = model.RecordPerPage;
            gridModel.FilterColumn.IsResourceRequired = false;
            //Gets the table width
            tableWidth = GenerateDynamicGridView(gridModel.GridColumnList, gridModel.WebGridColumn, dropDownValueList, isBulkAddRequired, gridModel.FilterColumn.IsResourceRequired, model.LinkPermission, model.ViewMode, model.IsMultiSelectList);

            dataModel = SetSortOrder(model, gridModel, dataModel);
            dataModel.ForEach(lModel => { gridModel.DataTableToList.Add((dynamic)lModel); });

            //sets the grid model properties
            gridModel.TotalRowCount = dataModel.Count;
            gridModel.RowPerPageCount = pageSize;
            gridModel.TableWidth = pageSize == 10 ? tableWidth : tableWidth + 24;
            gridModel.FrontObjectName = listType;
            gridModel.ViewMode = model.ViewMode;
            gridModel.FilterColumn.ToolMenuList = GetAuthorizedToolMenu(toolMenuList);

            GetViewsList(gridModel);
            //Returns the result
            return gridModel;
        }
        //Get Default View of the list
        public static void GetDefaultView(string listName, FilterCollectionDataModel model)
        {
            int userId = SessionProxyHelper.GetUserDetails().UserId;
            var applicationSetting = GetService<IDependencyHelper>().GetFilterConfigurationXML(listName, userId);

            if (GetDataFromSession<string>(DynamicGridConstants.listTypeSessionKey) != listName.ToString() && GetDataFromSession<bool?>(DynamicGridConstants.IsViewRequest) == null)
            {
                SaveDataInSession<string>(DynamicGridConstants.listTypeSessionKey, listName);
                SaveDataInSession<List<Znode.Engine.Api.Models.ViewModel>>(DynamicGridConstants.ListViewsCollection, applicationSetting.Views);
                var defaultView = applicationSetting.Views.FirstOrDefault(x => x.IsDefault && x.CreatedBy == userId);

                if (applicationSetting.Views?.FirstOrDefault(x => x.IsSelected) != null && applicationSetting.Views.FirstOrDefault(x => x.IsSelected).IsSelected)
                {
                    var viewModel = GetService<IDependencyHelper>().GetFilterXML(applicationSetting.Views.FirstOrDefault(x => x.IsSelected).ViewId);
                    SaveDataInSession<string>(DynamicGridConstants.XMLStringSessionKey, viewModel.XmlSetting);
                }
                else
                {
                    SaveDataInSession<string>(DynamicGridConstants.XMLStringSessionKey, defaultView != null ? defaultView.XmlSetting : applicationSetting.Setting);
                }
                if (defaultView != null)
                {
                    applicationSetting.Views.FirstOrDefault(x => x.IsDefault && x.CreatedBy == userId).IsSelected = true;
                    if (HelperUtility.IsNull(Convert.ToString(HttpContext.Current.Request.Cookies[listName])) || HttpContext.Current.Request.Cookies[listName]?.Values?.Count == 0)
                        model.Filters = defaultView.Filters;
                    SaveDataInSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey, model.Filters);
                    if (defaultView.SortColumn != null)
                    {

                        model.SortCollection = new SortCollection();
                        model.SortCollection.Add(defaultView.SortColumn, defaultView.SortType);
                    }
                }
                RemoveDataFromSession(DynamicGridConstants.ColumnListSessionKey);
                RemoveDataFromSession("CustomColumns");

                SaveDataInSession<bool?>(DynamicGridConstants.IsPaging, applicationSetting.IsPaging);
                SaveDataInSession<bool?>(DynamicGridConstants.IsResourceRequired, applicationSetting.IsResourceRequired);
            }
            if (model.SortCollection == null && !string.IsNullOrEmpty(applicationSetting.OrderByFields))
            {
                model.SortCollection = new SortCollection();
                // To sort the list, add the sort filter in OrderByFields column of applicationsetting.
                // If orderby Filter is only one then its key value pair should be separate by '|' like 'OrderDateWithTime|DESC'
                // If orderby Filter is more than one then it should be separate by ',' like 'OrderDateWithTime|DESC,StoreName|AESC'
                string[] sortFilter = applicationSetting.OrderByFields.Split(',');

                foreach (string item in sortFilter)
                {
                    string[] strSplit = item.Split('|');
                    if (strSplit.Length > 0)
                    {
                        model.SortCollection.Add(strSplit[0], strSplit[1]);
                    }
                    else { ZnodeLogging.LogMessage("Error Occurred in OrderByFields column of ApplicationSetting :", listName, TraceLevel.Info, "Sort Filters of List should be separated by ',' and values should be seprated by '|' Like 'OrderDateWithTime|DESC,StoreName|AESC'."); }
                }
            }

        }

        //Get and Set Filters from Cookies if exists.
        public static void GetSetFiltersFromCookies(string listType, FilterCollectionDataModel model)
        {
            if ((HelperUtility.IsNotNull(model.Filters) && model.Filters.Count > 0) || GetDataFromSession<string>(DynamicGridConstants.listTypeSessionKey) == listType)
            {
                string filterCollection = new JavaScriptSerializer().Serialize(model.Filters);
                // Cookies was not updating in Chrome browser. So removing the cookies before setting it.
                CookieHelper.RemoveCookie(listType);
                CookieHelper.SetCookie(listType, filterCollection, (ZnodeConstant.MinutesInAHour * Convert.ToDouble(ZnodeAdminSettings.CookieExpiresValueForFilter)));

            }

            if (model.Filters.Count == 0 && GetDataFromSession<string>(DynamicGridConstants.listTypeSessionKey) != listType && HelperUtility.IsNull(GetDataFromSession<bool?>(DynamicGridConstants.IsViewRequest)))
            {
                if (CookieHelper.IsCookieExists(listType))
                {
                    FilterCollection filterCollection = new FilterCollection();
                    var jsonData = CookieHelper.GetCookieValues(listType);
                    if (jsonData != null && jsonData.Count > 0)
                        filterCollection = JsonConvert.DeserializeObject<FilterCollection>(jsonData[0]);
                    model.Filters = filterCollection;
                    SaveDataInSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey, model.Filters);
                }

            }
        }

        private static void GetViewsList(GridModel model)
        {
            var _views = GetDataFromSession<List<Api.Models.ViewModel>>(DynamicGridConstants.ListViewsCollection);
            _views.ForEach(x =>
            {
                model.Views.Add(new Models.ViewModel { ViewId = x.ViewId, ViewName = x.ViewName, IsSelected = x.IsSelected, CreatedBy = x.CreatedBy, IsDefault = x.IsDefault, IsPublic = x.IsPublic });
            });

        }

        /// <summary>
        /// Set Model recorded ordering ASC/DSC
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="model">Reference of FilterCollectionDataModel</param>
        /// <param name="reportModel"></param>
        /// <param name="dataModel"></param>
        private static List<T> SetSortOrder<T>(FilterCollectionDataModel model, GridModel reportModel, List<T> dataModel)
        {
            if (!Equals(model.SortCollection, null))
            {
                //gets the sort and direction from model

                string sort = string.Empty;
                string sortDir = string.Empty;
                foreach (KeyValuePair<string, string> pair in model.SortCollection)
                {
                    sort = pair.Key;
                    sortDir = pair.Value;
                }

                if (sortDir.Equals(DynamicGridConstants.ASCKey))
                {
                    reportModel.GridSortDirection = SortDirection.Ascending;
                    reportModel.GridSortColumn = sort;
                }
                else
                {
                    reportModel.GridSortDirection = SortDirection.Descending;
                    reportModel.GridSortColumn = sort;
                }
            }
            return dataModel;
        }

        /// <summary>
        /// Generate dynamic grid column
        /// </summary>
        /// <param name="gridList">dynamic grid column list </param>
        /// <param name="tableWidth">table width</param>
        /// <param name="columns">WebGridColumn column object</param>
        /// <returns>returns integer table width</returns>
        private static int GenerateDynamicGridView(List<dynamic> gridList, List<WebGridColumn> columns, Dictionary<string, List<SelectListItem>> dropDownValueList = null, bool isBulkAddRequired = false, bool isResourceRequired = false, List<string> linkPermission = null, string viewMode = null, bool IsMultiSelectList = true)
        {
            if (!Equals(gridList, null) && gridList.Count > 0)
                return GetWebGridColumn(gridList, columns, dropDownValueList, isBulkAddRequired, isResourceRequired, linkPermission, viewMode, IsMultiSelectList);

            return 0;
        }

        public static string SetRuntimeHeaderFormat(dynamic column, bool isResourceRequired)
        {
            string headerFormat = (((IDictionary<String, object>)column).ContainsKey("HeaderFormat")) ? column.HeaderFormat : string.Empty;
            return (string.IsNullOrEmpty(headerFormat)) ? SetDisplayTextFromResource(column.headertext, isResourceRequired) : String.Format(HttpUtility.UrlDecode(headerFormat), GetDisplayTextList(column.headertext, isResourceRequired));
        }

        /// <summary>
        /// Set Display text from resource
        /// </summary>
        /// <param name="resourceKey"></param>
        /// <param name="isResourceRequired"></param>
        /// <returns></returns>
        public static string SetDisplayTextFromResource(string resourceKey, bool isResourceRequired = false)
        {
            try
            {
                //this code is commented as it will be implemented in future
                // if (isResourceRequired && !string.IsNullOrEmpty(resourceKey))
                // return LocalizeHelper.GetLocalizedString(ResourceFile.RDynamicGrid, resourceKey);
                return resourceKey;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                throw ex;
            }
        }

        /// <summary>
        /// Create webgridcolumn with HTML format.
        /// </summary>
        /// <param name="gridList">Grid Column list</param>
        /// <param name="columns">WebGridColumn object</param>
        /// <param name="columnsName">Reference columnsName</param>
        /// <param name="tableWidth">Reference tableWidth</param>
        /// <param name="rowIndex">Reference rowIndex</param>
        /// <param name="dropDownValueList">Dictionary containing dropdown list name and it's  item List</param>
        private static int GetWebGridColumn(List<dynamic> gridList, List<WebGridColumn> columns, Dictionary<string, List<SelectListItem>> dropDownValueList = null, bool isBulkAddRequired = false, bool isResourceRequired = false, List<string> linkPermission = null, string viewMode = null, bool IsMultiSelectList = true)
        {
            string columnsName = string.Empty;
            int tableWidth = 0;
            if (!string.IsNullOrEmpty(viewMode) && !Equals(viewMode, ViewModeTypes.List.ToString()))
                return GetWebGridDetailColumn(gridList, columns, columnsName, linkPermission, viewMode, IsMultiSelectList);
            else
            {
                foreach (var item in gridList)
                {
                    columnsName = Convert.ToString(item.name);
                    bool isSort = false;
                    if (Convert.ToBoolean(item.allowsorting))
                        isSort = true;
                    string headerText = FilterHelpers.SetRuntimeHeaderFormat(item, isResourceRequired);

                    bool _isAttributeColumn = false;

                    if (((IDictionary<string, object>)item).Keys.Contains("isattributecolumn"))
                    {
                        object _value;
                        ((IDictionary<string, object>)item).TryGetValue("isattributecolumn", out _value);
                        _isAttributeColumn = !string.IsNullOrEmpty(Convert.ToString(_value)) ? Convert.ToString(_value) == "n" ? false : true : false;
                    }

                    if (!Equals(Convert.ToString(item.isvisible), DynamicGridConstants.No) && !_isAttributeColumn)
                    {
                        if (Convert.ToString(item.ischeckbox).Equals(DynamicGridConstants.Yes))
                        {
                            columns.Add(new WebGridColumn()
                            {
                                ColumnName = columnsName,
                                Header = headerText,
                                CanSort = isSort,
                                Format = (item1) => new HtmlString(FilterHelpers.SetRuntimeInlineEditFormat(isResourceRequired, item, item1)),
                                Style = DynamicGridConstants.GridCheckbox
                            });
                        }
                        else if (Convert.ToString(item.iscontrol).Equals(DynamicGridConstants.Yes))
                        {
                            if (Equals(item.controltype, DynamicGridConstants.ButtonKey))
                            {
                                columns.Add(new WebGridColumn()
                                {
                                    ColumnName = columnsName,
                                    Header = headerText,
                                    CanSort = isSort,
                                    Format = (item1) => new HtmlString(FilterHelpers.SetRuntimeControl(item1, item, isResourceRequired, linkPermission)),
                                    Style = item.Class
                                });
                            }
                            else if (Equals(item.controltype, DynamicGridConstants.RuntimeHtmlKey))
                            {
                                columns.Add(new WebGridColumn()
                                {
                                    ColumnName = columnsName,
                                    Header = headerText,
                                    CanSort = isSort,
                                    Format = (item1) => new HtmlString(FilterHelpers.SetRuntimeControl(item1, item, isResourceRequired, linkPermission)),
                                    Style = item.Class
                                });
                            }
                            else if (Equals(item.controltype, DynamicGridConstants.RuntimeCodeKey))
                            {
                                columns.Add(new WebGridColumn()
                                {
                                    ColumnName = columnsName,
                                    Header = headerText,
                                    CanSort = isSort,
                                    Format = (item1) => new HtmlString(FilterHelpers.SetRuntimeControl(item1, item, isResourceRequired, linkPermission)),
                                    Style = item.Class
                                });
                            }
                            else
                            {
                                columns.Add(new WebGridColumn()
                                {
                                    ColumnName = columnsName,
                                    Header = headerText,
                                    CanSort = isSort,
                                    Format = (item1) => new HtmlString(FilterHelpers.SetRuntimeInlineEditFormat(isResourceRequired, item, item1)),
                                    Style = item.Class
                                });
                            }
                        }
                        else
                        {
                            switch (columnsName)
                            {
                                case DynamicGridConstants.ManageKey:
                                    columns.Add(new WebGridColumn()
                                    {
                                        ColumnName = columnsName,
                                        Header = headerText,
                                        CanSort = isSort,
                                        Format = (item1) => new HtmlString(FilterHelpers.SetRuntimeControl(item1, item, isResourceRequired, linkPermission)),
                                        Style = DynamicGridConstants.GridAction
                                    });
                                    break;

                                case DynamicGridConstants.ImageKey:
                                    columns.Add(new WebGridColumn()
                                    {
                                        ColumnName = columnsName,
                                        Header = headerText,
                                        CanSort = isSort,
                                        Format = (item1) => new HtmlString(FilterHelpers.SetRuntimeControl(item1, item, isResourceRequired, linkPermission)),
                                        Style = item.Class
                                    });
                                    break;

                                default:
                                    if (Convert.ToString(item.isallowlink).Equals(DynamicGridConstants.Yes))
                                    {
                                        columns.Add(new WebGridColumn()
                                        {
                                            ColumnName = columnsName,
                                            Header = headerText,
                                            CanSort = isSort,
                                            Format = (item1) => new HtmlString(FilterHelpers.SetRuntimeControl(item1, item, isResourceRequired, linkPermission)),
                                            Style = item.Class
                                        });
                                    }
                                    else
                                    {
                                        string datatype = Convert.ToString(item.datatype);
                                        if (datatype.Equals("Boolean"))
                                        {
                                            columns.Add(new WebGridColumn()
                                            {
                                                ColumnName = columnsName,
                                                Header = headerText,
                                                CanSort = isSort,
                                                Format = (item1) => new HtmlString(String.Format("{0}", FilterHelpers.FindRowValues(item1, Convert.ToString(item.name)))),
                                                Style = item.Class
                                            });
                                        }
                                        else if (datatype.Equals(DynamicGridConstants.DateTime) || datatype.Equals(DynamicGridConstants.Date) || datatype.Equals(DynamicGridConstants.Time))
                                        {
                                            columns.Add(new WebGridColumn()
                                            {
                                                ColumnName = columnsName,
                                                Header = headerText,
                                                CanSort = isSort,
                                                Format = (item1) => new HtmlString(String.Format("{0}", FilterHelpers.SetRuntimeDateFormat(item1, Convert.ToString(item.name), datatype, Convert.ToString(item.format)))),
                                            });
                                        }
                                        else
                                        {
                                            columns.Add(new WebGridColumn()
                                            {
                                                ColumnName = columnsName,
                                                Header = headerText,
                                                CanSort = isSort,
                                                Style = item.Class
                                            });
                                        }
                                    }
                                    break;
                            }
                        }
                        tableWidth = tableWidth + 300;
                    }
                }
                SetManageColumn(gridList, columns);
                return tableWidth;
            }
        }

        //Check Whether the Manage Column has any links. In case no links present then remove the Manage Column from list.
        //So that it wont display in the Grid.
        private static void SetManageColumn(List<dynamic> gridList, List<WebGridColumn> columns)
        {
            //Check whether the Manage column exists in the list.
            int columnIndex = columns.FindIndex(x => x.ColumnName == DynamicGridConstants.ManageKey);
            if (columnIndex != -1)
            {
                try
                {
                    var manageColumnList = columns.FindAll(x => x.ColumnName == DynamicGridConstants.ManageKey).ToList();
                    WebGrid grid = new WebGrid();
                    grid.Bind(source: gridList);
                    string manageColumnHtml = grid.Table(columns: manageColumnList).ToHtmlString();
                    grid = null;
                    string searchTerm = "<ul class='action-ui'></ul>";
                    int wordCount = Regex.Matches(manageColumnHtml, searchTerm).Count;
                    if (wordCount > 0)
                        columns.RemoveAt(columnIndex);
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                }
            }
        }

        private static int GetWebGridDetailColumn(List<dynamic> gridList, List<WebGridColumn> columns, string columnsName, List<string> linkPermission = null, string viewMode = null, bool IsMultiSelectList = true)
        {
            string headerText = string.Empty;
            bool isSort = false;
            bool isResourceRequired = true;
            string displayText = string.Empty;
            int tableWidth = 0;
            foreach (var item in gridList)
            {
                columnsName = Convert.ToString(item.name);
                isSort = false;
                if (Convert.ToBoolean(item.allowsorting))
                    isSort = true;
                headerText = FilterHelpers.SetRuntimeHeaderFormat(item, isResourceRequired);

                if (!Equals(Convert.ToString(item.isvisible), DynamicGridConstants.No))
                {
                    if (Convert.ToString(item.allowdetailview).Equals(DynamicGridConstants.Yes) && Equals(viewMode, ViewModeTypes.Detail.ToString()))
                    {
                        columns.Add(new WebGridColumn()
                        {
                            ColumnName = columnsName,
                            Header = headerText,
                            CanSort = isSort,
                            Format = (item1) => String.Format(DynamicGridConstants.DynamicLinkHtml, item.islinkactionurl, SetQueryString(item.islinkparamfield, item.DbParamField, item1), GetParameterValue(item1, item.name)),
                            Style = item.Class
                        });
                    }
                }
                if (Equals(viewMode, ViewModeTypes.Tile.ToString()))
                {
                    if (Equals(columnsName, DynamicGridConstants.ImageKey))
                    {
                        columns.Add(new WebGridColumn()
                        {
                            Format = (item1) => GenerateTile(item1, item, isResourceRequired, linkPermission, IsMultiSelectList),
                            Style = item.Class,
                        });
                    }
                }
                tableWidth = tableWidth + 300;
            }

            return tableWidth;
        }

        /// <summary>
        /// To get filters from cookies.
        /// </summary>
        /// <param name="listType">List type</param>
        /// <param name="model">FilterCollectionDataModel</param>
        public static void GetFiltersFromCookies(string listType, FilterCollectionDataModel model)
        {
            if (CookieHelper.IsCookieExists(listType))
            {
                FilterCollection filterCollection = new FilterCollection();
                var jsonData = CookieHelper.GetCookieValues(listType);
                if (jsonData != null && jsonData.Count > 0)
                    filterCollection = JsonConvert.DeserializeObject<FilterCollection>(jsonData[0]);
                model.Filters = filterCollection;
            }
        }

        #region Generate Dynamic Control

        /// <summary>
        /// Find Row level values for html formating
        /// </summary>
        /// <param name="row">Dynamic grid row</param>
        /// <param name="name">column name</param>
        /// <returns>Returns Html string</returns>
        public static string FindRowValues(object row, string name)
        {
            var val = row.GetDynamicProperty(name, row: row);
            return (!Equals(val, null) && val.ToString().ToLower().Equals("true")) ? DynamicGridConstants.IconTrue : DynamicGridConstants.IconFalse;
        }

        public static string SetRuntimeDateFormat(object row, string name, string type, string format = "")
        {
            string val = Convert.ToString(row.GetDynamicProperty(name, row: row));
            switch (type)
            {
                case DynamicGridConstants.DateTime:
                    string dateTimeStr = String.Empty;

                    if (String.IsNullOrEmpty(format))
                    {
                        dateTimeStr = !string.IsNullOrEmpty(val) ? Convert.ToDateTime(val).ToString(HelperMethods.GetStringDateTimeFormat()) : string.Empty;
                    }
                    else
                    {
                        CultureInfo cultureInfo = new CultureInfo(DefaultSettingHelper.DefaultCulture);
                        DateTimeFormatInfo dateTimeFormatInfo = new DateTimeFormatInfo();
                        dateTimeFormatInfo.FullDateTimePattern = format;
                        cultureInfo.DateTimeFormat = dateTimeFormatInfo;
                        dateTimeStr = !string.IsNullOrEmpty(val) ? Convert.ToDateTime(val, cultureInfo).ToString(HelperMethods.GetStringDateTimeFormat()) : string.Empty;
                    }

                    return dateTimeStr;

                case DynamicGridConstants.Date:
                    return !string.IsNullOrEmpty(val) ? Convert.ToDateTime(val).ToString(HelperMethods.GetStringDateFormat()) : string.Empty;

                case DynamicGridConstants.Time:
                    return !string.IsNullOrEmpty(val) ? Convert.ToDateTime(val).ToString(HelperMethods.GetStringDateFormat()) : string.Empty;

                default:
                    return !string.IsNullOrEmpty(val) ? Convert.ToDateTime(val).ToString(HelperMethods.GetStringDateFormat()) : string.Empty;
            }
        }

        public static string SetRuntimeControl(object row, dynamic columns, bool isResourceRequired, List<string> linkPermission)
        {
            string dynamicHtml = string.Empty;
            string parameterField = string.Empty;
            string parameterValue = string.Empty;
            string actionUrl = string.Empty;
            string displayText = string.Empty;
            string imageSrc = string.Empty;

            if (Equals(columns.name, DynamicGridConstants.ImageKey))
            {
                parameterField = GetImageParameters(row, columns)?[0];
                imageSrc = GetParameterValue(row, parameterField);

                dynamicHtml = FormatHtmltag(imageSrc, row);
                if (!string.IsNullOrEmpty(columns.imageactionurl))
                {
                    parameterField = columns.islinkparamfield;
                    parameterValue = GetParameterValue(row, parameterField);
                    actionUrl = (string.IsNullOrEmpty(parameterValue)) ? columns.imageactionurl : columns.imageactionurl + parameterValue;
                    dynamicHtml = String.Format(DynamicGridConstants.ImageActionLinkHtml, imageSrc, actionUrl);
                }
            }
            else if (Convert.ToString(columns.isallowlink).Equals(DynamicGridConstants.Yes))
            {
                displayText = SetDisplayTextFromResource(columns.displaytext, isResourceRequired);
                displayText = (string.IsNullOrEmpty(displayText)) ? GetParameterValue(row, columns.name) : displayText;

                if (string.IsNullOrEmpty(columns.islinkactionurl))
                    actionUrl = "#";
                else
                    actionUrl = columns.islinkactionurl;

                //Remove area from url for link permission check
                string urlWithoutArea = GetManageActionUrlWithoutArea(columns.islinkactionurl);
                if (!Equals(linkPermission, null))
                {
                    if (SessionProxyHelper.IsAdminUser() || Equals(actionUrl, "#") || linkPermission.Contains(urlWithoutArea?.Substring(1, urlWithoutArea.Length - 1)))
                        dynamicHtml = String.Format(DynamicGridConstants.LinkHtml, actionUrl, SetQueryString(columns.islinkparamfield, columns.DbParamField, row), HttpUtility.UrlDecode(columns.format), displayText);
                    else
                        dynamicHtml = String.Format(displayText);
                }
            }
            else if (columns.name == DynamicGridConstants.ManageKey)
                dynamicHtml = GetManageLink(row, columns, isResourceRequired, linkPermission);
            else if (columns.controltype == DynamicGridConstants.ButtonKey)
            {
                parameterField = columns.controlparamfield;
                parameterValue = GetParameterValue(row, parameterField);
                dynamicHtml = String.Format(DynamicGridConstants.ButtonHtml, HttpUtility.UrlDecode(columns.format), columns.name, columns.displaytext, parameterValue);
            }
            else if (columns.controltype == DynamicGridConstants.RuntimeHtmlKey)
            {
                parameterField = columns.controlparamfield;
                dynamicHtml = (string.IsNullOrEmpty(parameterField)) ? HttpUtility.UrlDecode(columns.format) : String.Format(HttpUtility.UrlDecode(columns.format), GetParameterValueList(row, parameterField));
                if (!string.IsNullOrEmpty(columns.displaytext))
                {
                    dynamicHtml = dynamicHtml.Replace("$#", "{").Replace("#$", "}");
                    dynamicHtml = string.Format(dynamicHtml, GetDisplayTextList(columns.displaytext, isResourceRequired));
                }
            }
            else if (columns.controltype == DynamicGridConstants.RuntimeCodeKey)
            {
                parameterField = columns.controlparamfield;
                object[] paramList = GetParameterValueList(row, parameterField);
                dynamicHtml = ExecuteFunction(HttpUtility.UrlDecode(columns.format), paramList);
                if (!string.IsNullOrEmpty(columns.displaytext))
                {
                    dynamicHtml = dynamicHtml.Replace("$#", "{").Replace("#$", "}");
                    dynamicHtml = string.Format(dynamicHtml, GetDisplayTextList(columns.displaytext, isResourceRequired));
                }
            }

            return dynamicHtml;
        }

        private static string FormatHtmltag(string imageSrc, object row)
        {
            switch (Convert.ToString(row.GetDynamicProperty(ZnodeMediaAttributeFamilyEnum.FamilyCode.ToString(), row: row)).ToLower())
            {
                case DynamicGridConstants.Video:
                    return $"<i class='z-video' title='Video'  src='" + imageSrc + "'></i>";

                case DynamicGridConstants.Audio:
                    return $"<i class='z-audio' title='Audio' src='" + imageSrc + "'></i>";

                case DynamicGridConstants.File:
                    return $"<i class='z-file-text' title='File' src='" + imageSrc + "' ></i>";

                default:
                    return String.Format(DynamicGridConstants.ImageHtml, imageSrc);
            }
        }

        private static string[] GetImageParameters(object row, dynamic column)
        {
            if (!string.IsNullOrEmpty(column.imageparamfield) && column.imageparamfield.Contains(","))
            {
                return column.imageparamfield.Split(',');
            }
            return null;
        }

        //Format Html for Tile View
        private static HtmlString GenerateTile(object row, dynamic column, bool isResourceRequired, List<string> linkPermission, bool IsMultiSelectList = true)
        {
            string imageName = string.Empty, imageSrc = string.Empty, id = string.Empty;
            string[] imageParameters = GetImageParameters(row, column);
            string parameterField = column.imageparamfield;
            if (imageParameters?.Length > 0)
            {
                imageName = GetParameterValue(row, imageParameters[1]);
                imageSrc = GetParameterValue(row, imageParameters[0]);
            }
            if (!string.IsNullOrEmpty(column.checkboxparamfield))
                id = GetParameterValue(row, column.checkboxparamfield);
            string mediaSize = Convert.ToString(row.GetDynamicProperty("Size", row: row));

            switch (Convert.ToString(row.GetDynamicProperty("FamilyCode", row: row)).ToLower())
            {
                case DynamicGridConstants.Video:
                    return MvcHtmlString.Create(string.Format(DynamicGridConstants.VideoTag, id, id, id, imageSrc, id, column.checkboxparamfield, (IsMultiSelectList ? "block" : "none"), (IsMultiSelectList ? "block" : "none"), id, mediaSize, imageName, id, GetManageLink(row, column, isResourceRequired, linkPermission, true)));

                case DynamicGridConstants.Audio:
                    return MvcHtmlString.Create(string.Format(DynamicGridConstants.AudioTag, id, id, id, imageSrc, id, column.checkboxparamfield, (IsMultiSelectList ? "block" : "none"), (IsMultiSelectList ? "block" : "none"), id, mediaSize, imageName, id, GetManageLink(row, column, isResourceRequired, linkPermission, true)));

                case DynamicGridConstants.File:
                    return MvcHtmlString.Create(string.Format(DynamicGridConstants.FileTag, id, id, id, imageSrc, id, column.checkboxparamfield, (IsMultiSelectList ? "block" : "none"), (IsMultiSelectList ? "block" : "none"), id, mediaSize, imageName, id, GetManageLink(row, column, isResourceRequired, linkPermission, true)));

                default:
                    return MvcHtmlString.Create(string.Format(DynamicGridConstants.ImageTag, id, id, id, imageSrc, imageSrc, id, column.checkboxparamfield, (IsMultiSelectList ? "block" : "none"), (IsMultiSelectList ? "block" : "none"), id, mediaSize, imageName, id, GetManageLink(row, column, isResourceRequired, linkPermission, true)));
            }
        }

        private static string GetManageLink(object row, dynamic column, bool isResourceRequired, List<string> linkPermission, bool isTile = false)
        {
            string dynamicHtml = string.Empty;
            int count = 0;
            string manageKey = DynamicGridConstants.ManageKey;
            string[] paramField = column.manageparamfield.Split('|');
            string[] dbParamField = (string.IsNullOrEmpty(column.DbParamField)) ? column.manageparamfield.Split('|') : column.DbParamField.Split('|');
            string[] displayTextList = column.displaytext.Split('|');
            string[] linkClass = HttpUtility.UrlDecode(column.format).Split('|');

            foreach (string manageActionUrl in column.manageactionurl.Split('|'))
            {
                string[] paramValue = GetParameterValueList(row, paramField[count]);
                string queryString = SetQueryString(paramField[count], dbParamField[count], row);
                string displayText = string.Equals(displayTextList[count], AdminConstants.CreateScheduler, StringComparison.InvariantCultureIgnoreCase) ? HelperMethods.GetSchedulerTitle(ConnectorTouchPoints: paramValue[0], schedulerCallFor: paramValue.Count() > 1 ? paramValue[1] : string.Empty) : SetDisplayTextFromResource(displayTextList[count], isResourceRequired);

                if (!Equals(linkPermission, null))
                {
                    //Drop Area from the ActionUrl if exists.
                    string manageActionUrlWithoutArea = GetManageActionUrlWithoutArea(manageActionUrl);

                    string disablePopup = string.Empty;
                    if (Equals(linkClass[count]?.ToLower(), "disable") && (queryString.Contains("IsLock=True") || queryString.Contains("IsActive=True") || queryString.Contains("IsEnabled=True") || queryString.Contains("isDefault=True") || queryString.Contains("IsPause=True")))
                    {
                        disablePopup = $"data-toggle='modal' data-target='#PopUpConfirmEnable' onclick=\"DynamicGrid.prototype.ConfirmEnableDisable('{manageActionUrl}{queryString}')\"";
                        dynamicHtml += string.Format(DynamicGridConstants.ManageChildHtml, "z-" + "enable", string.IsNullOrEmpty(disablePopup) ? "href='" + manageActionUrl + queryString + "'" : string.Empty, queryString, displayTextList[count], "Enable", disablePopup, string.Empty);
                    }
                    else if (Equals(linkClass[count]?.ToLower(), "disable") && (queryString.Contains("IsLock=False") || queryString.Contains("IsActive=False") || queryString.Contains("IsEnabled=False") || queryString.Contains("isDefault=False") || queryString.Contains("IsPause=False")))
                    {
                        disablePopup = $"data-toggle='modal' data-target='#PopUpConfirmDisable' onclick=\"DynamicGrid.prototype.ConfirmEnableDisable('{manageActionUrl}{queryString}')\"";
                        dynamicHtml += string.Format(DynamicGridConstants.ManageChildHtml, "z-" + linkClass[count]?.ToLower(), string.IsNullOrEmpty(disablePopup) ? "href='" + manageActionUrl + queryString + "'" : string.Empty, queryString, displayTextList[count], displayText, disablePopup, string.Empty);
                    }
                    else
                    {
                        string deletePopup = string.Empty;
                        if (Equals(linkClass[count]?.ToLower(), "delete"))
                            deletePopup = $"data-toggle='modal' data-target='#PopUpConfirm' onclick=\"DynamicGrid.prototype.ConfirmDelete('{manageActionUrl}{queryString}',this)\"";
                        if (isTile)
                            dynamicHtml += string.Format(DynamicGridConstants.TileViewManageChildHtml, "z-" + linkClass[count]?.ToLower(), manageActionUrl, queryString, displayTextList[count]);
                        else if (SessionProxyHelper.IsAdminUser() || string.IsNullOrEmpty(manageActionUrl) || string.IsNullOrEmpty(manageActionUrlWithoutArea) || linkPermission.Contains(manageActionUrlWithoutArea.Substring(1, manageActionUrlWithoutArea.Length - 1)))
                        {
                            if (Equals(linkClass[count]?.ToLower(), "download"))
                                dynamicHtml += string.Format(DynamicGridConstants.DownloadChildHtml, "z-" + linkClass[count]?.ToLower(), (string.IsNullOrEmpty(deletePopup) ? "href='" + manageActionUrl + queryString + "'" : string.Empty), queryString, displayTextList[count], displayText);
                            else
                                dynamicHtml += string.Format(DynamicGridConstants.ManageChildHtml, "z-" + linkClass[count]?.ToLower(), (string.IsNullOrEmpty(deletePopup) ? "href='" + manageActionUrl + queryString + "'" : string.Empty), (linkClass[count]?.ToLower() == "edit" ? HttpUtility.UrlEncode(queryString) : queryString), string.Equals(displayTextList[count], AdminConstants.CreateScheduler, StringComparison.InvariantCultureIgnoreCase) ? displayText : displayTextList[count], displayText, deletePopup, (Equals(linkClass[count]?.ToLower(), "edit") ? "oncontextmenu='return false'" : string.Empty));
                        }
                    }
                }
                count++;
            }
            return isTile ?
            String.Format(String.Format(DynamicGridConstants.TileManageHtml, dynamicHtml, SetDisplayTextFromResource(manageKey, isResourceRequired))) :
            String.Format(String.Format(DynamicGridConstants.ManageHtml, dynamicHtml, SetDisplayTextFromResource(manageKey, isResourceRequired)));
        }

        private static string GetManageActionUrlWithoutArea(string manageActionUrl)
        {
            string linkActionUrl = manageActionUrl;
            string manageActionUrlWithoutArea = manageActionUrl;
            //Drop Area from the ActionUrl if exists.
            if (linkActionUrl.Count(x => x == '/') > 2)
            {
                int slashCount = 0;

                foreach (char character in linkActionUrl)
                {
                    if (character == '/')
                        slashCount = slashCount + 1;

                    if (slashCount < 2)
                        manageActionUrlWithoutArea = manageActionUrlWithoutArea.Substring(1, manageActionUrlWithoutArea.Length - 1);
                }
            }
            return manageActionUrlWithoutArea;
        }

        /// <summary>
        /// Get Single Parameter Value
        /// </summary>
        /// <param name="row"></param>
        /// <param name="parameterField"></param>
        /// <returns></returns>
        public static string GetParameterValue(object row, string parameterField)
        {
            try
            {
                return Convert.ToString(row.GetDynamicProperty(parameterField, row: row));
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return "";
            }
        }

        /// <summary>
        /// Set Runtime Inline Edit Format for dynamic grid
        /// </summary>
        /// <param name="isResourceRequired"></param>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public static string SetRuntimeInlineEditFormat(bool isResourceRequired = false, dynamic column = null, object row = null)
        {
            string dynamicHtml = string.Empty;
            string columnName = column.name;
            string parameterField = string.Empty;
            string parameterValue = string.Empty;
            parameterField = column.controlparamfield;
            string _width = column.width;
            parameterValue = GetParameterValue(row, parameterField);

            if (Convert.ToString(column.ischeckbox).Equals(DynamicGridConstants.Yes))
            {
                string _IsNonEditableRow = GetParameterValue(row, "IsNonEditableRow");
                string _disabled = string.Empty;
                string isChecked = string.Empty;
                parameterField = column.checkboxparamfield;

                parameterValue = GetParameterValue(row, parameterField);
                if (Convert.ToString(row.GetDynamicProperty(columnName, row: row)).ToLower() == "true")
                    isChecked = "Checked='Checked'";

                if (Convert.ToInt32(string.IsNullOrEmpty(_IsNonEditableRow) ? "0" : _IsNonEditableRow) == 1)
                    _disabled = "disabled-checkbox";

                dynamicHtml = String.Format(DynamicGridConstants.CheckboxHtml, Convert.ToString(row.GetDynamicProperty(columnName, row: row)), column.name, _disabled, parameterValue, isChecked);
            }

            if (column.controltype == DynamicGridConstants.RadioKey)
            {
                string isChecked = string.Empty;
                if (Convert.ToString(row.GetDynamicProperty(columnName, row: row)).ToLower() == "true")
                    isChecked = "Checked='Checked'";
                dynamicHtml = String.Format(DynamicGridConstants.RadioHtml, Convert.ToString(row.GetDynamicProperty(columnName, row: row)), column.name, parameterValue, isChecked);
            }

            if (column.controltype == DynamicGridConstants.RowWiseRadioKey)
            {
                string isChecked = string.Empty;
                if (Convert.ToString(row.GetDynamicProperty(columnName, row: row)).ToLower() == "true")
                    isChecked = "Checked='Checked'";
                dynamicHtml = String.Format(DynamicGridConstants.RowRadioHtml, Convert.ToString(row.GetDynamicProperty(columnName, row: row)), column.name, parameterValue, isChecked);
            }

            if (column.controltype == DynamicGridConstants.HiddenFieldKey)
                dynamicHtml = String.Format(DynamicGridConstants.HidenfieldHtml, Convert.ToString(row.GetDynamicProperty(columnName, row: row)), column.name);
            else if (column.controltype == DynamicGridConstants.TextKey)
                dynamicHtml = String.Format(DynamicGridConstants.InlineEditHtml, Convert.ToString(row.GetDynamicProperty(columnName, row: row)), column.name, column.maxlength, _width);
            else if (column.controltype == DynamicGridConstants.LabelKey)
                dynamicHtml = String.Format(DynamicGridConstants.LabelHtml, Convert.ToString(row.GetDynamicProperty(columnName, row: row)), column.name);
            else if (column.controltype == DynamicGridConstants.DynamicHtmlKey)
                dynamicHtml = String.Format(DynamicGridConstants.DynamicHtml, Convert.ToString(row.GetDynamicProperty(columnName + DynamicGridConstants.DynamicHtmlKey, row: row)), column.name);
            else if (column.controltype == DynamicGridConstants.MultiCheckboxListKey)
            {
                string selectedString = string.Empty;
                selectedString = Convert.ToString(row.GetDynamicProperty(DynamicGridConstants.SelectedcheckBoxList + columnName, row: row));
                string[] selectedArray = selectedString.Split(',');
                dynamicHtml = "<label data-dgview='show' data-columnname='" + column.name + "'>" + Convert.ToString(row.GetDynamicProperty(columnName, row: row)) + "</label>";
                dynamicHtml = "<div data-dgview='edit' class='dg-select-container' data-columnname='" + column.name + "' style='display:none;'><div class='selectbox'>Select<div class='ico-blue-dropbox'></div></div>";
                dynamicHtml += "<div class='dg-checklistbox' style='display:none;'>";
                dynamicHtml += "<ul data-dgview='edit' style='display:none;' class='mdl-ul' data-columnname='" + column.name + "'>";
                List<SelectListItem> TestList = row.GetDynamicProperty(columnName + DynamicGridConstants.MulticheckBoxListKey, row: row) as List<SelectListItem>;
                if (!Equals(TestList, null))
                {
                    foreach (var item in TestList)
                    {
                        string dynamicId = Guid.NewGuid().ToString();
                        dynamicHtml += "<li><div class=''>";
                        if (selectedArray.Contains(item.Value))
                            dynamicHtml += "<input checked='checked' type='checkbox' class='chk-btn' data-itemid='" + item.Value + "' data-itemvalue='" + item.Text + "' id='chk_" + dynamicId + "'  name='chkBox'/>";
                        else
                            dynamicHtml += "<input type='checkbox' class='chk-btn' data-itemid='" + item.Value + "' data-itemvalue='" + item.Text + "' id='chk_" + dynamicId + "'  name='chkBox'/>";
                        dynamicHtml += "<label class='lbl padding-8' for='chk_" + dynamicId + "' ><span title='" + item.Text + "' class='name-stud'>" + item.Text + "</span></label>";
                        dynamicHtml += " </div></li>";
                    }
                }
                dynamicHtml += "</ul></div></div>";
            }
            else if (column.controltype == DynamicGridConstants.DropDownKey)
            {
                string labelHtml = string.Empty;
                string selectedText = string.Empty;
                dynamicHtml += "<select data-dgview='edit' style='display:none;' class='dropDown' data-columnname='" + column.name + "'  data-columntype='" + Convert.ToString(column.datatype) + "'>";
                List<SelectListItem> TestList = row.GetDynamicProperty(columnName + DynamicGridConstants.DropDownListKey, row: row) as List<SelectListItem>;
                if (!Equals(TestList, null))
                {
                    foreach (var item in TestList)
                    {
                        string rowValue = Convert.ToString(column.datatype).Equals("Boolean") ? Convert.ToString(row.GetDynamicProperty(columnName, row: row))?.ToLower() : Convert.ToString(row.GetDynamicProperty(columnName, row: row));
                        if (Equals(item.Value, rowValue))
                        {
                            selectedText = item.Text;
                            dynamicHtml += "<option selected='selected' value='" + item.Value + "'>" + item.Text + "</option>";
                        }
                        else
                            dynamicHtml += "<option value='" + item.Value + "'>" + item.Text + "</option>";
                    }
                }
                else
                    dynamicHtml += "<option value=''>Select</option>";

                if (Convert.ToString(column.datatype).Equals("Boolean"))
                    labelHtml = "<label data-dgview='show' data-columnname='" + column.name + "'>" + FilterHelpers.FindRowValues(row, Convert.ToString(column.name)) + "</label>";
                else
                    labelHtml = "<label data-dgview='show' data-columnname='" + column.name + "'>" + selectedText + "</label>";

                dynamicHtml += "</select>";
                dynamicHtml = labelHtml + dynamicHtml;
            }
            else
                Convert.ToString(row.GetDynamicProperty(columnName, row: row));
            return dynamicHtml;
        }

        //Check for the Role Permission Access to display only authorized menu, based on login user role Permission.
        private static List<ToolMenuModel> GetAuthorizedToolMenu(List<ToolMenuModel> toolMenuList)
        {
            List<ToolMenuModel> toolList = new List<ToolMenuModel>();
            if (!Equals(toolMenuList, null) && toolMenuList.Count > 0)
            {
                if (SessionProxyHelper.IsAdminUser())
                {
                    toolList = toolMenuList;
                }
                else
                {
                    //Get Login user Access Permission List.
                    List<RolePermissionViewModel> permission = SessionProxyHelper.GetUserPermission();
                    if (!Equals(permission, null) && permission.Count > 0)
                    {
                        foreach (ToolMenuModel toolMenu in toolMenuList)
                        {
                            string url = string.IsNullOrEmpty(toolMenu.AreaName) ? $"{toolMenu.ControllerName}/{toolMenu.ActionName}" : $"{toolMenu.AreaName}/{toolMenu.ControllerName}/{toolMenu.ActionName}";
                            string newurl = url.ToLower();
                            if (permission.FindIndex(w => w.RequestUrlTemplate.Equals(newurl)) != 0)
                            {
                                toolList.Add(toolMenu);
                            }
                        }
                    }
                }
            }
            return toolList;
        }

        #endregion Generate Dynamic Control

        #region Sub grid

        /// <summary>
        /// Get nested grid sub records
        /// </summary>
        /// <param name="row">grid object row</param>
        /// <param name="keyName">field name for get records </param>
        /// <param name="typeName">class name</param>
        /// <param name="methodName">target method name</param>
        /// <returns>returns dynamic list</returns>
        public static List<SelectListItem> GetSubRecords(object row, string keyName, string typeName, string methodName)
        {
            string val = Convert.ToString(row.GetDynamicProperty(keyName, row: row));
            List<SelectListItem> subGridlist = new List<SelectListItem>();
            subGridlist.Add(new SelectListItem() { Text = string.Format(DynamicGridConstants.SubgridHtml, val, typeName, methodName) });
            return subGridlist;
        }

        /// <summary>
        /// Invoke delegets methods from string parameter.
        /// </summary>
        /// <param name="typeName">class name</param>
        /// <param name="methodName">target method name</param>
        /// <param name="stringParam">method parameter</param>
        /// <returns></returns>
        public static List<dynamic> InvokeStringMethod(string typeName, string methodName, string stringParam)
        {
            // Get the Type for the class
            Type calledType = Type.GetType(typeName);

            // Invoke the method itself. The string returned by the method winds up in s.
            // Note that stringParam is passed via the last parameter of InvokeMember,
            // as an array of Objects.
            var result = calledType.InvokeMember(methodName, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new Object[] { stringParam });

            // Return the string that was returned by the called method.
            return (List<dynamic>)result;
        }

        #endregion Sub grid

        #region Method to read XML string from database.

        public static T ConvertFromDBVal<T>(object obj)
        {
            if (Equals(obj, null) || Equals(obj, DBNull.Value))
                return default(T); // returns the default value for the type
            else
                return (T)obj;
        }

        /// <summary>
        /// gets the datatype by key name
        /// </summary>
        /// <param name="keyName">key name</param>
        /// <returns>datatype</returns>
        public static string GetDataTypeByKeyName(string keyName)
        {
            string _datatype = "String";
            //Gets the column list
            List<dynamic> columnList = GetDataFromSession<List<dynamic>>(DynamicGridConstants.ColumnListSessionKey);
            //Gets the column from list
            var data = columnList.FirstOrDefault(x => x.name.ToString().ToLower() == keyName.ToLower());
            if (!Equals(data, null))
                _datatype = data.datatype;
            return _datatype;
        }

        #endregion Method to read XML string from database.

        #region Set Query string and parameter

        /// <summary>
        /// Genrate query string for action url
        /// </summary>
        /// <param name="paramField"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private static string SetQueryString(string paramField, string dbparamField, object row)
        {
            string queryParm = string.Empty;
            if (string.IsNullOrEmpty(paramField))
                return queryParm;
            int index = 0;
            string[] dbfieldName = (string.IsNullOrEmpty(dbparamField)) ? paramField.Split(',') : dbparamField.Split(',');
            foreach (string fieldName in paramField.Split(','))
            {
                //check for the queryParm null or empty to check for first parameter
                if (string.IsNullOrEmpty(queryParm))
                    queryParm = "?" + fieldName + "=" + HttpUtility.UrlEncode(Convert.ToString(row.GetDynamicProperty(fieldName, row: row)));
                else
                    //Upend the query parameter with &
                    queryParm += "&" + fieldName + "=" + HttpUtility.UrlEncode(Convert.ToString(row.GetDynamicProperty(fieldName, row: row)));
                index++;
            }
            return queryParm;
        }

        /// <summary>
        /// Get Parameter Value List
        /// </summary>
        /// <param name="row"></param>
        /// <param name="paramField"></param>
        /// <returns></returns>
        public static string[] GetParameterValueList(object row, string paramField)
        {
            //checks for paramField null or empty
            if (string.IsNullOrEmpty(paramField))
                return null;
            int count = 0;
            //gets the parameter value comma seprated
            string[] paramValue = new string[paramField.Split(',').Count()];
            foreach (string fieldName in paramField.Split(','))
            {
                paramValue[count] = GetParameterValue(row, fieldName);
                count++;
            }
            return paramValue;
        }

        /// <summary>
        /// Get Display Text List
        /// </summary>
        /// <param name="displayText"></param>
        /// <param name="isResourceRequired"></param>
        /// <returns></returns>
        public static string[] GetDisplayTextList(string displayText, bool isResourceRequired = false)
        {
            //creates an array
            string[] displayTextValue = new string[displayText.Split(',').Count()];

            if (string.IsNullOrEmpty(displayText)) return displayTextValue;
            int count = 0;
            foreach (string fieldName in displayText.Split(','))
            {
                displayTextValue[count] = SetDisplayTextFromResource(fieldName, isResourceRequired);
                count++;
            }
            return displayTextValue;
        }

        #endregion Set Query string and parameter

        #region Runtime Execute Code

        /// <summary>
        /// create Method Info
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public static MethodInfo CreateFunction(string function)
        {
            string code = @"
                            using System;
                            namespace UserFunctions
                            {
                                public class BinaryFunction
                                {
                                  func_xy
                                }
                            }
                        ";
            string finalCode = code.Replace("func_xy", function);
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerResults results = provider.CompileAssemblyFromSource(new CompilerParameters(), finalCode);
            Type binaryFunction = results.CompiledAssembly.GetType("UserFunctions.BinaryFunction");
            return binaryFunction.GetMethod("Function");
        }

        /// <summary>
        /// Execute Function
        /// </summary>
        /// <param name="executeCode"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string ExecuteFunction(string executeCode = "", object[] parameters = null)
        {
            try
            {
                MethodInfo function = CreateFunction(executeCode);
                return Convert.ToString(function.Invoke(null, parameters));
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return string.Empty;
            }
        }

        /// <summary>
        /// gets the datatype by operator id
        /// </summary>
        /// <param name="operatorId">operator id</param>
        /// <returns></returns>
        public static string GetFilterOperatorByOperatorId(string operatorId)
        {
            //gets the operator xml
            string operatorXMl = GetXmlConfiguration(DynamicGridConstants.OperatorXML);
            List<dynamic> operatorList = new List<dynamic>();
            //check for not null
            if (!string.IsNullOrEmpty(operatorXMl))
                //gets the operator list
                operatorList = ConvertXmlToList(operatorXMl, DynamicGridConstants.OperatordefinitionKey);
            //get the operator string
            string operatorstr = operatorList.FirstOrDefault(op => op.id.Equals(operatorId))?.displayname;
            //returns the result
            return SetFilterOperators(operatorstr);
        }

        /// <summary>
        /// Get XML configuration
        /// </summary>
        /// <param name="listName">List type</param>
        /// <returns></returns>
        public static string GetXmlConfiguration(string listName)
            => GetService<IDependencyHelper>().GetFilterConfigurationXML(listName, SessionProxyHelper.GetUserDetails().UserId)?.Setting;

        /// <summary>
        /// Get filter collection operator name.
        /// </summary>
        /// <param name="operatorString">Selected Operator String</param>
        /// <returns>Returns filter collection operator name</returns>
        public static string SetFilterOperators(string operatorString)
        {
            if (!string.IsNullOrEmpty(operatorString))
            {
                switch (operatorString.Trim().ToLower())
                {
                    case DynamicGridConstants.IsOperator:
                        return FilterOperators.Is;

                    case DynamicGridConstants.BeginswithOperator:
                        return FilterOperators.StartsWith;

                    case DynamicGridConstants.EndswithOperator:
                        return FilterOperators.EndsWith;

                    case DynamicGridConstants.ContainsOperator:
                        return FilterOperators.Contains;

                    case DynamicGridConstants.EqualsOperator:
                        return FilterOperators.Equals;

                    case DynamicGridConstants.GreaterthanOperator:
                        return FilterOperators.GreaterThan;

                    case DynamicGridConstants.GreaterorequalOperator:
                        return FilterOperators.GreaterThanOrEqual;

                    case DynamicGridConstants.LessthanOperator:
                        return FilterOperators.LessThan;

                    case DynamicGridConstants.LessorequalOperator:
                        return FilterOperators.LessThanOrEqual;

                    case DynamicGridConstants.OnOperator:
                        return FilterOperators.Between;

                    case DynamicGridConstants.OnOrBeforeOperator:
                        return FilterOperators.LessThanOrEqual;

                    case DynamicGridConstants.AfterOperator:
                        return FilterOperators.GreaterThan;

                    case DynamicGridConstants.BeforeOperator:
                        return FilterOperators.LessThan;

                    case DynamicGridConstants.OnOrAfterOperator:
                        return FilterOperators.GreaterThanOrEqual;

                    case DynamicGridConstants.NotOnOperator:
                        return FilterOperators.NotEquals;

                    default:
                        return FilterOperators.Like;
                }
            }
            return FilterOperators.Like;
        }

        // Get filter operator id.
        public static string GetFilterOperatorId(string operatorString, string datatype)
        {
            if (!string.IsNullOrEmpty(operatorString))
            {
                datatype?.Trim().ToLower();
                string operatorXMl = GetDataFromSession<string>(DynamicGridConstants.OperatorXmlSettingSessionKey);
                if (string.IsNullOrEmpty(operatorXMl))
                {
                    operatorXMl = GetXmlConfiguration(DynamicGridConstants.OperatorXML);
                    SaveDataInSession<string>(DynamicGridConstants.OperatorXmlSettingSessionKey, operatorXMl);
                }

                List<dynamic> operatorList = new List<dynamic>();
                //check for not null
                if (!string.IsNullOrEmpty(operatorXMl))
                    //gets the operator list
                    operatorList = ConvertXmlToList(operatorXMl, DynamicGridConstants.OperatordefinitionKey);

                switch (operatorString?.Trim().ToLower())
                {
                    case FilterOperators.StartsWith:
                        return operatorList.FirstOrDefault(op => op.filteroperator.Equals(FilterOperators.StartsWith) && op.datatype.Equals(datatype))?.id;

                    case FilterOperators.EndsWith:
                        return operatorList.FirstOrDefault(op => op.filteroperator.Equals(FilterOperators.EndsWith) && op.datatype.Equals(datatype))?.id;

                    case FilterOperators.Contains:
                        return operatorList.FirstOrDefault(op => op.filteroperator.Equals(FilterOperators.Contains) && op.datatype.Equals(datatype))?.id;

                    case FilterOperators.Equals:
                        return operatorList.FirstOrDefault(op => op.filteroperator.Equals(FilterOperators.Equals) && op.datatype.Equals(datatype))?.id;

                    case FilterOperators.GreaterThan:
                        return operatorList.FirstOrDefault(op => op.filteroperator.Equals(FilterOperators.GreaterThan) && op.datatype.Equals(datatype))?.id;

                    case FilterOperators.GreaterThanOrEqual:
                        return operatorList.FirstOrDefault(op => op.filteroperator.Equals(FilterOperators.GreaterThanOrEqual) && op.datatype.Equals(datatype))?.id;

                    case FilterOperators.LessThan:
                        return operatorList.FirstOrDefault(op => op.filteroperator.Equals(FilterOperators.LessThan) && op.datatype.Equals(datatype))?.id;

                    case FilterOperators.LessThanOrEqual:
                        return operatorList.FirstOrDefault(op => op.filteroperator.Equals(FilterOperators.LessThanOrEqual) && op.datatype.Equals(datatype))?.id;

                    case FilterOperators.NotEquals:
                        return operatorList.FirstOrDefault(op => op.filteroperator.Equals(FilterOperators.NotEquals) && op.datatype.Equals(datatype))?.id;

                    case FilterOperators.Is:
                        return operatorList.FirstOrDefault(op => op.filteroperator.Equals(FilterOperators.Equals) && op.datatype.Equals(datatype))?.id;

                    case FilterOperators.Like:
                        return operatorList.FirstOrDefault(op => op.filteroperator.Equals(FilterOperators.Like) && op.datatype.Equals(datatype))?.id;

                    case FilterOperators.Between:
                        return operatorList.FirstOrDefault(op => op.filteroperator.Equals(FilterOperators.Between) && op.datatype.Equals(datatype))?.id;

                    default:
                        return "0";
                }
            }
            return string.Empty;
        }

        public static string GetXMLSettingFromDataBase(ApplicationSettingDataModel applicationSettingModel, FilterCollectionDataModel filterCollections, string listName = "")
        {
            string xmlString = string.Empty;
            try
            {
                int userId = SessionProxyHelper.GetUserDetails().UserId;
                var applicationsetting = GetService<IDependencyHelper>().GetFilterConfigurationXML(listName, userId);
                if (!Equals(applicationsetting, null))
                {
                    string _selectedView = FindSelectedView(applicationsetting.Views);
                    applicationSettingModel = applicationsetting;

                    xmlString = !string.IsNullOrEmpty(_selectedView) ? _selectedView : applicationsetting.Setting;

                    SaveDataInSession<List<Znode.Engine.Api.Models.ViewModel>>(DynamicGridConstants.ListViewsCollection, applicationsetting.Views);

                    if (!applicationSettingModel.IsPaging)
                        filterCollections.RecordPerPage = 0;
                    applicationSettingModel.OrderByFields = applicationsetting.OrderByFields;
                    if (!string.IsNullOrEmpty(applicationsetting.SelectedColumn))
                        applicationSettingModel.SelectedColumn = applicationsetting.SelectedColumn;
                    applicationSettingModel.Setting = xmlString;
                }

                SaveDataInSession<bool?>(DynamicGridConstants.IsPaging, applicationSettingModel.IsPaging);
                SaveDataInSession<bool?>(DynamicGridConstants.IsResourceRequired, applicationSettingModel.IsResourceRequired);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                throw ex;
            }
            return xmlString;
        }

        private static string FindSelectedView(List<Api.Models.ViewModel> model)
        {
            var _selectedView = model.FirstOrDefault(x => x.IsSelected);

            if (!Equals(_selectedView, null))
            {
                SaveDataInSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey, _selectedView.Filters);
                return _selectedView.XmlSetting;
            }
            return string.Empty;
        }

        /// <summary>
        /// Get Operators list
        /// </summary>
        /// <param name="datatype">data type</param>
        /// <returns>Returns the operator list of data type</returns>
        public static string GetOperators(string datatype, int operatorId)
        {
            string dynamicOperatorString = string.Empty;
            //Check for null and empty
            if (!string.IsNullOrEmpty(datatype))
            {
                //Get the default xml from session, otherwise fetch fresh
                string operatorXMl = GetDataFromSession<string>(DynamicGridConstants.OperatorXmlSettingSessionKey);

                if (Equals(operatorXMl, null))
                {
                    operatorXMl = GetXmlConfiguration(DynamicGridConstants.OperatorXML);
                    SaveDataInSession<string>(DynamicGridConstants.OperatorXmlSettingSessionKey, operatorXMl);
                }

                if (!string.IsNullOrEmpty(operatorXMl))
                {
                    List<dynamic> operatorList = ConvertXmlToList(operatorXMl, DynamicGridConstants.OperatordefinitionKey);
                    if (!Equals(operatorList, null))
                    {
                        //filter the list on datatype
                        operatorList = operatorList.FindAll(x => Equals(x.datatype?.ToString()?.ToLower(), datatype?.ToLower()));
                        foreach (var dr in operatorList)
                        {
                            string selectedOption = (Convert.ToInt32(dr.id) == operatorId) ? "selected" : string.Empty;
                            //create the operator list on datatype
                            dynamicOperatorString += $"<option value='{dr.id}' {selectedOption}>{dr.displayname}</option>";
                        }
                    }
                }
            }
            return dynamicOperatorString;
        }

        /// <summary>
        /// Get Operators list
        /// </summary>
        /// <param name="datatype">data type</param>
        /// <returns>Returns the operator list of data type</returns>
        public static List<dynamic> GetCustomReportOperators(string datatype, int operatorId)
        {
            List<dynamic> operatorList = new List<dynamic>();
            //Check for null and empty
            if (!string.IsNullOrEmpty(datatype))
            {
                //Get the default xml from session, otherwise fetch fresh
                string operatorXMl = GetDataFromSession<string>(DynamicGridConstants.OperatorXmlSettingSessionKey);

                if (Equals(operatorXMl, null))
                {
                    operatorXMl = GetXmlConfiguration(DynamicGridConstants.OperatorXML);
                    SaveDataInSession<string>(DynamicGridConstants.OperatorXmlSettingSessionKey, operatorXMl);
                }

                if (!string.IsNullOrEmpty(operatorXMl))
                {
                    operatorList = ConvertXmlToList(operatorXMl, DynamicGridConstants.OperatordefinitionKey);
                    if (!Equals(operatorList, null))
                        return operatorList.FindAll(x => Equals(x.datatype?.ToString()?.ToLower(), datatype?.ToLower()));
                }
            }
            return operatorList;
        }

        /// <summary>
        /// Set sequence of dynamic grid column
        /// </summary>
        /// <param name="columnList"></param>
        /// <param name="_assignedId"></param>
        /// <returns></returns>
        public static List<dynamic> SetIndexOrder(List<dynamic> columnList, int[] _assignedId)
        {
            List<dynamic> tempList = new List<dynamic>();
            tempList.AddRange(columnList);
            List<dynamic> newList = new List<dynamic>();

            _assignedId.ToList().ForEach(x =>
            {
                var item = columnList.FirstOrDefault(y => y.id == x.ToString());
                newList.Add(item);
                tempList.Remove(item);
            });
            newList.AddRange(tempList);
            return newList;
        }

        #endregion Runtime Execute Code
    }

    public static class XmlHelper
    {
        /// <summary>
        /// Defines the simple types that is directly writeable to XML.
        /// </summary>
        private static readonly Type[] _writeTypes = new[] { typeof(string), typeof(DateTime), typeof(Enum), typeof(decimal), typeof(Guid) };

        /// <summary>
        /// Determines whether [is simple type] [the specified type].
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// 	<c>true</c> if [is simple type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSimpleType(this Type type)
        {
            return type.IsPrimitive || _writeTypes.Contains(type);
        }

        /// <summary>
        /// Converts the specified dynamic object to XML.
        /// </summary>
        /// <param name="dynamicObject">The dynamic object.</param>
        /// <returns>Returns an Xml representation of the dynamic object.</returns>
        public static XElement ConvertToXml(dynamic dynamicObject)
        {
            return ConvertToXml(dynamicObject, null);
        }

        /// <summary>
        /// Converts the specified dynamic object to XML.
        /// </summary>
        /// <param name="dynamicObject">The dynamic object.</param>
        /// /// <param name="element">The element name.</param>
        /// <returns>Returns an Xml representation of the dynamic object.</returns>
        public static XElement ConvertToXml(dynamic dynamicObject, string element)
        {
            var ret = new XElement("column");

            Dictionary<string, object> members = new Dictionary<string, object>(dynamicObject);

            var elements = from prop in members
                           let name = XmlConvert.EncodeName(prop.Key)
                           let val = prop.Value.GetType().IsArray ? "array" : prop.Value
                           let value = prop.Value.GetType().IsArray ? GetArrayElement(prop.Key, (Array)prop.Value) : (prop.Value.GetType().IsSimpleType() ? new XElement(name, val) : val.ToXml(name))
                           where value != null
                           select value;

            ret.Add(elements);

            return ret;
        }

        /// <summary>
        /// Generates an XML string from the dynamic object.
        /// </summary>
        /// <param name="dynamicObject">The dynamic object.</param>
        /// <returns>Returns an XML string.</returns>
        public static string ToXmlString(dynamic dynamicObject)
        {
            // Create the XmlDocument.
            XmlDocument doc = new XmlDocument();
            string xmlData = "<columns></columns>";

            doc.Load(new StringReader(xmlData));

            foreach (var item in dynamicObject)
            {
                XElement xml = XmlHelper.ConvertToXml(item);

                //necessary for crossing XmlDocument contexts
                XmlNode importNode = doc.ImportNode(xml.ToXmlElement(), true);

                doc.DocumentElement.AppendChild(importNode);
            }
            return doc.OuterXml;
        }

        /// <summary>
        /// Converts an anonymous type to an XElement.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Returns the object as it's XML representation in an XElement.</returns>
        public static XElement ToXml(this object input)
        {
            return input.ToXml(null);
        }

        /// <summary>
        /// Converts an anonymous type to an XElement.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="element">The element name.</param>
        /// <returns>Returns the object as it's XML representation in an XElement.</returns>
        public static XElement ToXml(this object input, string element)
        {
            if (input == null)
            {
                return null;
            }

            if (String.IsNullOrWhiteSpace(element))
            {
                element = "object";
            }

            element = XmlConvert.EncodeName(element);
            var ret = new XElement(element);

            if (input != null)
            {
                var type = input.GetType();
                var props = type.GetProperties();

                var elements = from prop in props
                               let name = XmlConvert.EncodeName(prop.Name)
                               let val = prop.PropertyType.IsArray ? "array" : prop.GetValue(input, null)
                               let value = prop.PropertyType.IsArray ? GetArrayElement(prop, (Array)prop.GetValue(input, null)) : (prop.PropertyType.IsSimpleType() ? new XElement(name, val) : val.ToXml(name))
                               where value != null
                               select value;

                ret.Add(elements);
            }

            return ret;
        }

        /// <summary>
        /// Parses the specified XML string to a dynamic.
        /// </summary>
        /// <param name="xmlString">The XML string.</param>
        /// <returns>A dynamic object.</returns>
        public static dynamic ParseDynamic(this string xmlString)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the array element.
        /// </summary>
        /// <param name="info">The property info.</param>
        /// <param name="input">The input object.</param>
        /// <returns>Returns an XElement with the array collection as child elements.</returns>
        private static XElement GetArrayElement(PropertyInfo info, Array input)
        {
            return GetArrayElement(info.Name, input);
        }

        /// <summary>
        /// Gets the array element.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="input">The input object.</param>
        /// <returns>Returns an XElement with the array collection as child elements.</returns>
        private static XElement GetArrayElement(string propertyName, Array input)
        {
            var name = XmlConvert.EncodeName(propertyName);

            XElement rootElement = new XElement(name);

            var arrayCount = input.GetLength(0);

            for (int i = 0; i < arrayCount; i++)
            {
                var val = input.GetValue(i);
                XElement childElement = val.GetType().IsSimpleType() ? new XElement(name + "Child", val) : val.ToXml();

                rootElement.Add(childElement);
            }

            return rootElement;
        }

        public static XmlElement ToXmlElement(this XElement el)
        {
            var doc = new XmlDocument();
            doc.Load(el.CreateReader());
            return doc.DocumentElement;
        }

        // Create XMl string from custom Column list
        public static XElement WriteXML(CustomColumnViewModel column, int count)
        {
            if (!Equals(column, null))
            {
                return new XElement(DynamicGridConstants.columnKey,
                    new XElement(DynamicGridConstants.IdKey, count),
                    new XElement(DynamicGridConstants.nameKey, column.FieldName),
                    new XElement(DynamicGridConstants.headertextKey, column.DisplayName),
                    new XElement(DynamicGridConstants.WidthKey, 40),
                    new XElement(DynamicGridConstants.datatypeKey, "String"),
                    new XElement(DynamicGridConstants.ColumntypeKey, "String"),
                    new XElement(DynamicGridConstants.AllowsortingKey, false),
                    new XElement(DynamicGridConstants.AllowpagingKey, false),
                    new XElement(DynamicGridConstants.FormatKey, string.Empty),
                    new XElement(DynamicGridConstants.isvisibleKey, "y"),
                    new XElement(DynamicGridConstants.MustshowKey, "n"),
                    new XElement(DynamicGridConstants.MusthideKey, "n"),
                    new XElement(DynamicGridConstants.MaxlengthKey, 0),
                    new XElement(DynamicGridConstants.isallowsearchKey, "y"),
                    new XElement(DynamicGridConstants.IsconditionalKey, "y"),
                    new XElement(DynamicGridConstants.isallowlinkKey, "n"),
                    new XElement(DynamicGridConstants.islinkactionurlKey, string.Empty),
                    new XElement(DynamicGridConstants.islinkparamfieldKey, string.Empty),
                    new XElement(DynamicGridConstants.ischeckboxKey, "n"),
                    new XElement(DynamicGridConstants.checkboxparamfieldKey, string.Empty),
                    new XElement(DynamicGridConstants.iscontrolKey, "n"),
                    new XElement(DynamicGridConstants.controltypeKey, string.Empty),
                    new XElement(DynamicGridConstants.controlparamfieldKey, string.Empty),
                    new XElement(DynamicGridConstants.displaytextKey, string.Empty),
                    new XElement(DynamicGridConstants.editactionurlKey, string.Empty),
                    new XElement(DynamicGridConstants.editparamfieldKey, string.Empty),
                    new XElement(DynamicGridConstants.deleteactionurlKey, string.Empty),
                    new XElement(DynamicGridConstants.deleteparamfieldKey, string.Empty),
                    new XElement(DynamicGridConstants.viewactionurlKey, string.Empty),
                    new XElement(DynamicGridConstants.viewparamfieldKey, string.Empty),
                    new XElement(DynamicGridConstants.imageactionurlKey, string.Empty),
                    new XElement(DynamicGridConstants.imageparamfieldKey, string.Empty),
                    new XElement(DynamicGridConstants.manageactionurlKey, string.Empty),
                    new XElement(DynamicGridConstants.manageparamfieldKey, string.Empty),
                    new XElement(DynamicGridConstants.copyactionurlKey, string.Empty),
                    new XElement(DynamicGridConstants.copyparamfieldKey, string.Empty),
                    new XElement(DynamicGridConstants.xaxis, "n"),
                    new XElement(DynamicGridConstants.yaxis, "n"),
                    new XElement(DynamicGridConstants.isadvancesearch, "n"),
                    new XElement(DynamicGridConstants.ClassKey, string.Empty),
                    new XElement(DynamicGridConstants.SearchControlTypeKey, string.Empty),
                    new XElement(DynamicGridConstants.SearchControlParametersKey, string.Empty),
                    new XElement(DynamicGridConstants.DbParamFieldKey, string.Empty),
                    new XElement(DynamicGridConstants.UseMode, "DataBase"),
                    new XElement(DynamicGridConstants.IsGraphKey, "n"),
                     new XElement(DynamicGridConstants.isallowdetailKey, "n"),
                     new XElement("isattributecolumn", "y")
                           );
            }

            return null;
        }
    }
}