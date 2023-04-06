﻿using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.Extensions;
using Znode.Engine.WebStore.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.WebStore.Helpers
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
            SessionHelper.RemoveDataFromSession(DynamicGridConstants.XMLStringSesionKey);
            SessionHelper.RemoveDataFromSession(DynamicGridConstants.listTypeSesionKey);
            SessionHelper.RemoveDataFromSession(DynamicGridConstants.xmlConfigDocSessionKey);
            SessionHelper.RemoveDataFromSession(DynamicGridConstants.NewRowColumns);
            SessionHelper.RemoveDataFromSession(DynamicGridConstants.AddNewRowData);
        }

        /// <summary>
        /// Get dynamic column and filters criteria from configuration XML
        /// </summary>
        /// <param name="listType">integer list type</param>
        /// <param name="filterKeyValue">filter citeria values</param>
        /// <returns>Returns FilterColumnListModel</returns>
        private static FilterColumnListModel GetConvertedFilterColumnModelFromXml(string listType, FilterCollection filterKeyValue, ApplicationSettingDataModel applicationSettingModel, string xmlSetting = "")
        {
            string XMlstring = string.Empty;
            //check for data present in session or not
            if (HelperUtility.IsNull(SessionHelper.GetDataFromSession<string>(DynamicGridConstants.XMLStringSesionKey)) || SessionHelper.GetDataFromSession<string>(DynamicGridConstants.listTypeSesionKey) != listType.ToString())
            {
                if (string.IsNullOrEmpty(xmlSetting))
                    XMlstring = GetXmlConfiguration(listType, applicationSettingModel);
                else
                    XMlstring = xmlSetting;
                //inserts into session               
                SessionHelper.SaveDataInSession<string>(DynamicGridConstants.XMLStringSesionKey, XMlstring);
            }
            else
                XMlstring = SessionHelper.GetDataFromSession<string>(DynamicGridConstants.XMLStringSesionKey);
            //parse the xml string into document
            XDocument xmlDoc = XDocument.Parse(XMlstring);
            SessionHelper.SaveDataInSession<XDocument>(DynamicGridConstants.xmlConfigDocSessionKey, xmlDoc);


            //get the feilds collection
            int[] _fieldscollection = SessionHelper.GetDataFromSession<int[]>(DynamicGridConstants.FieldsCollectionSessionKey);

            List<FilterColumnModel> model = (from xml in xmlDoc.Descendants(DynamicGridConstants.columnKey)
                                             where (xml.Element(DynamicGridConstants.isallowsearchKey).Value.Equals(Convert.ToString(DynamicGridConstants.Yes)) || xml.Element(DynamicGridConstants.isallowsearchKey).Value.Equals(Convert.ToString(DynamicGridConstants.Hav))) && (!Equals(_fieldscollection, null) ? !_fieldscollection.Contains(int.Parse(xml.Element("id").Value)) : (int.Parse(xml.Element("id").Value) == int.Parse(xml.Element("id").Value)))
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


            List<int> _tempArr = SessionHelper.GetDataFromSession<List<int>>(DynamicGridConstants.FilterState);
            List<int> _removedFilters = SessionHelper.GetDataFromSession<List<int>>(DynamicGridConstants.RemovedFilters);
            model.ForEach(x =>
            {
                var _flt = filterKeyValue.FirstOrDefault(y => y.FilterName == x.ColumnName);
                if (!Equals(_flt, null)) { x.IsSearchable = true; }
                if (!Equals(_tempArr, null) && _tempArr.IndexOf(x.Id) != -1) { x.IsSearchable = true; }
                if (!Equals(_removedFilters, null) && _removedFilters.IndexOf(x.Id) != -1) { x.IsSearchable = false; }
            });

            //returns the result
            return new FilterColumnListModel { FilterColumnList = model, GridColumnList = GetDynamicGridView(XMlstring, DynamicGridConstants.columnKey), IsResourceRequired = SessionHelper.GetDataFromSession<bool>(DynamicGridConstants.IsResourceRequired) };
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
        /// <param name="dataTable">DataTable</param>
        /// <returns>Returns dynamic list</returns>
        public static List<dynamic> DataTableToList(DataTable dataTable)
        {
            var result = new List<dynamic>();
            try
            {
                if (HelperUtility.IsNotNull(dataTable) && dataTable.Rows.Count > 0)
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
        public static GridModel GetDynamicGridModel<T>(FilterCollectionDataModel model, List<T> dataModel, string listType, string xmlSetting = "", Dictionary<string, List<SelectListItem>> dropDownValueList = null, bool isBulkAddRequired = false, bool isPermission = true, List<ToolMenuModel> toolMenuList = null)
        {
            GridModel gridModel = new GridModel();

            int tableWidth = 0;

            SessionHelper.RemoveDataFromSession(DynamicGridConstants.ColumnListSessionKey);
            SessionHelper.RemoveDataFromSession(DynamicGridConstants.GridViewMode);
            SessionHelper.RemoveDataFromSession(DynamicGridConstants.FilterState);
            SessionHelper.RemoveDataFromSession(DynamicGridConstants.RemovedFilters);

            gridModel.FilterColumn = GetFilterColumnList(model.Filters, listType, gridModel.GridSettingModel, xmlSetting);
            gridModel.GridColumnList = gridModel.FilterColumn.GridColumnList;
            SessionHelper.SaveDataInSession<List<dynamic>>(DynamicGridConstants.ColumnListSessionKey, gridModel.GridColumnList);

            //enter list name in session           
            SessionHelper.SaveDataInSession<string>(DynamicGridConstants.listTypeSesionKey, listType);

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
            gridModel.FilterColumn.ToolMenuList = toolMenuList;
            //Returns the result
            return gridModel;
        }

        /// <summary>
        /// Set Model record ordering ASC/DSC
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="model">Reference of FilterCollectionDataModel</param>
        /// <param name="reportModel"></param>
        /// <param name="dataModel"></param>
        private static List<T> SetSortOrder<T>(FilterCollectionDataModel model, GridModel reportModel, List<T> dataModel)
        {
            if (HelperUtility.IsNotNull(model.SortCollection))
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
        /// <param name="dropDownValueList">Dictionary containing drop down list name and it's item List</param>
        private static int GetWebGridColumn(List<dynamic> gridList, List<WebGridColumn> columns, Dictionary<string, List<SelectListItem>> dropDownValueList = null, bool isBulkAddRequired = false, bool isResourceRequired = false, List<string> linkPermission = null, string viewMode = null, bool IsMultiSelectList = true)
        {
            string columnsName = string.Empty;
            int tableWidth = 0;
            foreach (var item in gridList)
            {
                columnsName = Convert.ToString(item.name);
                bool isSort = false;
                if (Convert.ToBoolean(item.allowsorting))
                    isSort = true;
                string headerText = FilterHelpers.SetRuntimeHeaderFormat(item, isResourceRequired);
                if (!Equals(Convert.ToString(item.isvisible), DynamicGridConstants.No))
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
                                            Format = (item1) => new HtmlString(String.Format("{0}", FilterHelpers.SetRuntimeDateFormat(item1, Convert.ToString(item.name), datatype))),
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
                catch(Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                }
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

        public static string SetRuntimeDateFormat(object row, string name, string type)
        {
            string val = Convert.ToString(row.GetDynamicProperty(name, row: row));
            switch (type)
            {
                case DynamicGridConstants.DateTime:
                    return !string.IsNullOrEmpty(val) ? Convert.ToDateTime(val).ToString(HelperMethods.GetStringDateTimeFormat()) : string.Empty;
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
                dynamicHtml = String.Format(DynamicGridConstants.LinkHtml, actionUrl, SetQueryString(columns.islinkparamfield, columns.DbParamField, row), HttpUtility.UrlDecode(columns.format), displayText);
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
                return column.imageparamfield.Split(',');
            return null;
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

                string queryString = SetQueryString(paramField[count], dbParamField[count], row);
                string displayText = SetDisplayTextFromResource(displayTextList[count], isResourceRequired);

                string disablePopup = string.Empty;
                string resetPopup = string.Empty;
                if (Equals(linkClass[count]?.ToLower(), "disable") && (queryString.Contains("IsLock=True") || queryString.Contains("IsActive=True") || queryString.Contains("IsEnabled=True")))
                {
                    disablePopup = $"data-toggle='modal' href='#' data-target='#PopUpConfirmEnable' onclick=\"DynamicGrid.prototype.ConfirmEnableDisable('{manageActionUrl}{queryString}')\"";
                    dynamicHtml += string.Format(DynamicGridConstants.ManageChildHtml, "zf-" + "enable", string.IsNullOrEmpty(disablePopup) ? "href='" + manageActionUrl + queryString + "'" : string.Empty, queryString, displayTextList[count], "Enable", disablePopup);
                }
                else if (Equals(linkClass[count]?.ToLower(), "disable") && (queryString.Contains("IsLock=False") || queryString.Contains("IsActive=False") || queryString.Contains("IsEnabled=False")))
                {
                    disablePopup = $"data-toggle='modal' href='#' data-target='#PopUpConfirmDisable' onclick=\"DynamicGrid.prototype.ConfirmEnableDisable('{manageActionUrl}{queryString}')\"";
                    dynamicHtml += string.Format(DynamicGridConstants.ManageChildHtml, "zf-" + linkClass[count]?.ToLower(), string.IsNullOrEmpty(disablePopup) ? "href='" + manageActionUrl + queryString + "'" : string.Empty, queryString, displayTextList[count], displayText, disablePopup);
                }
                else if (Equals(linkClass[count]?.ToLower(), "reset"))
                {
                    disablePopup = $"data-toggle='modal' href='#' data-target='#PopUpConfirmResetPassword' onclick=\"DynamicGrid.prototype.ConfirmResetPassword('{manageActionUrl}{queryString}')\"";
                    dynamicHtml += string.Format(DynamicGridConstants.ManageChildHtml, "zf-" + linkClass[count]?.ToLower(), string.IsNullOrEmpty(disablePopup) ? "href='" + manageActionUrl + queryString + "'" : string.Empty, queryString, displayTextList[count], displayText, disablePopup);
                }
                else
                {
                    string deletePopup = string.Empty;
                    if (Equals(linkClass[count]?.ToLower(), "delete"))
                        deletePopup = $"data-toggle='modal' data-target='#PopUpConfirm' onclick=\"DynamicGrid.prototype.ConfirmDelete('{manageActionUrl}{queryString}',this)\"";
                    if (isTile)
                        dynamicHtml += string.Format(DynamicGridConstants.TileViewManageChildHtml, "zf-" + linkClass[count]?.ToLower(), manageActionUrl, queryString, displayTextList[count]);
                    else
                        dynamicHtml += string.Format(DynamicGridConstants.ManageChildHtml, "zf-" + linkClass[count]?.ToLower(), string.IsNullOrEmpty(deletePopup) ? "href='" + manageActionUrl + queryString + "'" : string.Empty, queryString, displayTextList[count], displayText, deletePopup);
                }

                count++;
            }
            return isTile ?
            String.Format(String.Format(DynamicGridConstants.TileManageHtml, dynamicHtml, SetDisplayTextFromResource(manageKey, isResourceRequired))) :
            String.Format(String.Format(DynamicGridConstants.ManageHtml, dynamicHtml, SetDisplayTextFromResource(manageKey, isResourceRequired)));

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
            parameterValue = GetParameterValue(row, parameterField);
            if (Convert.ToString(column.ischeckbox).Equals(DynamicGridConstants.Yes))
            {
                string isChecked = string.Empty;
                parameterField = column.checkboxparamfield;
                parameterValue = GetParameterValue(row, parameterField);
                if (Convert.ToString(row.GetDynamicProperty(columnName, row: row)).ToLower() == "true")
                    isChecked = "Checked='Checked'";
                dynamicHtml = String.Format(DynamicGridConstants.CheckboxHtml, Convert.ToString(row.GetDynamicProperty(columnName, row: row)), column.name, parameterValue, isChecked);
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
                dynamicHtml = String.Format(DynamicGridConstants.InlineEditHtml, Convert.ToString(row.GetDynamicProperty(columnName, row: row)), column.name, column.maxlength);
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
                dynamicHtml += "<select data-dgview='edit' style='display:none;' class='dropDown' data-columnname='" + column.name + "'>";
                List<SelectListItem> TestList = row.GetDynamicProperty(columnName + DynamicGridConstants.DropDownListKey, row: row) as List<SelectListItem>;
                if (!Equals(TestList, null))
                {
                    foreach (var item in TestList)
                    {
                        if (item.Value == Convert.ToString(row.GetDynamicProperty(columnName, row: row)))
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

                labelHtml = "<label data-dgview='show' data-columnname='" + column.name + "'>" + selectedText + "</label>";
                dynamicHtml += "</select>";
                dynamicHtml = labelHtml + dynamicHtml;
            }
            else
                Convert.ToString(row.GetDynamicProperty(columnName, row: row));
            return dynamicHtml;
        }

        #endregion    

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
        #endregion

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
            string _datatype = string.Empty;
            //Gets the column list
            List<dynamic> columnList = SessionHelper.GetDataFromSession<List<dynamic>>(DynamicGridConstants.ColumnListSessionKey);
            //Gets the column from list
            var data = columnList.FirstOrDefault(x => x.name.ToString().ToLower() == keyName.ToLower());
            if (!Equals(data, null))
                _datatype = data.datatype;
            return _datatype;
        }

        #endregion

        #region Set Query string and parameter
        /// <summary>
        /// Genrate query string for action url 
        /// </summary>
        /// <param name="paramField"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private static string SetQueryString(string paramField, string dbParamField, object row)
        {
            string queryParm = string.Empty;
            if (string.IsNullOrEmpty(paramField))
                return queryParm;
            int index = 0;
            string[] dbFieldName = (string.IsNullOrEmpty(dbParamField)) ? paramField.Split(',') : dbParamField.Split(',');
            foreach (string fieldName in paramField.Split(','))
            {
                //check for the queryParm null or empty to check for first parameter
                if (string.IsNullOrEmpty(queryParm))
                    queryParm = "?" + fieldName + "=" + Convert.ToString(row.GetDynamicProperty(fieldName, row: row));
                else
                    //Upend the query parameter with &
                    queryParm += "&" + fieldName + "=" + Convert.ToString(row.GetDynamicProperty(fieldName, row: row));
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
        #endregion

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
        {
            IWebstoreHelper helper = GetService<IWebstoreHelper>();
            return helper.GetFilterConfigurationXML(listName).Setting;
        }

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
                string operatorXMl = SessionHelper.GetDataFromSession<string>(DynamicGridConstants.OperatorXmlSettingSessionKey);
                if (string.IsNullOrEmpty(operatorXMl))
                {
                    operatorXMl = GetXmlConfiguration(DynamicGridConstants.OperatorXML);
                    SessionHelper.SaveDataInSession<string>(DynamicGridConstants.OperatorXmlSettingSessionKey, operatorXMl);
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
                        return operatorList.FirstOrDefault(op => op.filteroperator.Equals(FilterOperators.Is) && op.datatype.Equals(datatype))?.id;
                    case FilterOperators.Like:
                        return operatorList.FirstOrDefault(op => op.filteroperator.Equals(FilterOperators.Like) && op.datatype.Equals(datatype))?.id;
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
                IWebstoreHelper helper = GetService<IWebstoreHelper>();
                ApplicationSettingDataModel applicationsetting = helper.GetFilterConfigurationXML(listName);
                if (!Equals(applicationsetting, null))
                {
                    applicationSettingModel = applicationsetting;
                    xmlString = applicationsetting.Setting;

                    if (!applicationSettingModel.IsPaging)
                        filterCollections.RecordPerPage = 0;
                    applicationSettingModel.OrderByFields = applicationsetting.OrderByFields;
                    if (!string.IsNullOrEmpty(applicationsetting.SelectedColumn))
                        applicationSettingModel.SelectedColumn = applicationsetting.SelectedColumn;
                    applicationSettingModel.Setting = xmlString;
                }
                SessionHelper.SaveDataInSession<bool>(DynamicGridConstants.IsPaging, applicationSettingModel.IsPaging);
                SessionHelper.SaveDataInSession<bool>(DynamicGridConstants.IsResourceRequired, applicationSettingModel.IsResourceRequired);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                throw ex;
            }
            return xmlString;
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
                //Get the default xml
                string operatorXMl = string.Empty;

                if (Equals(SessionHelper.GetDataFromSession<string>(DynamicGridConstants.OperatorXmlSettingSessionKey), null))
                {
                    operatorXMl = GetXmlConfiguration(DynamicGridConstants.OperatorXML);
                    SessionHelper.SaveDataInSession<string>(DynamicGridConstants.OperatorXmlSettingSessionKey, operatorXMl);
                }
                else
                    operatorXMl = SessionHelper.GetDataFromSession<string>(DynamicGridConstants.OperatorXmlSettingSessionKey);

                if (!string.IsNullOrEmpty(operatorXMl))
                {
                    List<dynamic> operatorList = ConvertXmlToList(operatorXMl, DynamicGridConstants.OperatordefinitionKey);
                    if (!Equals(operatorList, null))
                    {
                        //filter the list on datatype
                        operatorList = operatorList.FindAll(x => (x.datatype.Equals(datatype)));
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

        #endregion        
    }
}