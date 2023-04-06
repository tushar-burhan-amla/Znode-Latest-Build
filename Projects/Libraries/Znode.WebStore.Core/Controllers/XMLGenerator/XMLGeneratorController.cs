using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Api.Models.Enum;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.Helpers;
using Znode.Engine.WebStore.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
namespace Znode.Engine.WebStore.Controllers.XMLGenerator
{
    public class XMLGeneratorController : BaseController
    {
        #region variables

        private readonly IApplicationSettingsAgent _xmlGeneratorAgent;
        private bool isSuccess = false;
        private string message = string.Empty;
        private string status = string.Empty;
        private const string ApplicationSettingId = "ApplicationSettingId";
        private const int PageNo = 1;
        private const int RecordPerPage = 10;
        #endregion

        #region Constructor
        public XMLGeneratorController(IApplicationSettingsAgent xmlGeneratorAgent)
        {
            _xmlGeneratorAgent = xmlGeneratorAgent;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Create XML 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult CreateXML() => View(new WebGridViewModel());

        /// <summary>
        /// Display List of XML configuration
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            ApplicationSettingListViewModel modelList = _xmlGeneratorAgent.ApplicationsSettingList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            var gridModel = FilterHelpers.GetDynamicGridModel(model, modelList.List, "vw_ZNodeApplicationSetting");

            gridModel.TotalRecordCount = modelList.TotalResults;
            return ActionView(gridModel);
        }

        /// <summary>
        /// Get column list for create XML
        /// </summary>
        /// <param name="entityType">Type of Entity for column list</param>
        /// <param name="entityName">Name of Entity for column list</param>
        /// <param name="columnListJson"></param>
        /// <returns></returns>
        public virtual ActionResult GetColumnList(string entityType = "", string entityName = "", string columnListJson = null) => PartialView(DynamicGridConstants.ListBoxView, GetListViewModel(entityType, entityName, columnListJson));

        /// <summary>
        /// Save XMl configuration
        /// </summary>
        /// <param name="columnCollection">Selected column collection</param>
        /// <param name="viewOptions">View Option Name</param>
        /// <param name="entityType">Type of entity</param>
        /// <param name="entityName">Name of entity</param>
        /// <param name="frontPageName">front Page Name</param>
        /// <param name="frontObjectName">front Object Name</param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual JsonResult SaveColumnXML(string columnCollection, string viewOptions, string entityType, string entityName, string frontPageName, string frontObjectName, int id = 0)
        {
            string xmlSTring = _xmlGeneratorAgent.CreateXMLFile(Newtonsoft.Json.JsonConvert.DeserializeObject<List<WebGridColumnViewModel>>(columnCollection));
            isSuccess = _xmlGeneratorAgent.SaveXmlConfiguration(xmlSTring, viewOptions, entityType, entityName, frontPageName, frontObjectName, id);
            TempData[WebStoreConstants.Notifications] = isSuccess ? GenerateNotificationMessages("Record saved successfully.", NotificationType.success) : GenerateNotificationMessages("Record saved successfully.", NotificationType.error);
            return Json(new { IsSuccess = isSuccess, message = message, status = status }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Edit XMl configuration
        /// </summary>
        /// <param name="columnCollection">Selected column collection</param>
        /// <param name="viewOptions">View Option Name</param>
        /// <param name="entityType">Type of entity</param>
        /// <param name="entityName">Name of entity</param>
        /// <param name="frontPageName">front Page Name</param>
        /// <param name="frontObjectName">front Object Name</param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual ActionResult EditApplicationSetting(string columnCollection, string viewOptions, string entityType, string entityName, string frontPageName, string frontObjectName, int id = 0)
        {
            if (ModelState.IsValid)
            {
                List<WebGridColumnViewModel> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WebGridColumnViewModel>>(columnCollection);
                string xmlSTring = _xmlGeneratorAgent.CreateXMLFile(list);
                isSuccess = _xmlGeneratorAgent.UpdateXmlConfiguration(xmlSTring, viewOptions, entityType, entityName, frontPageName, frontObjectName, id);
                TempData[WebStoreConstants.Notifications] = isSuccess ? GenerateNotificationMessages("Record updated successfully.", NotificationType.success) : GenerateNotificationMessages("Record updated successfully.", NotificationType.error);
            }
            return Json(new { IsSuccess = true, message = message, status = status }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Delete XML Configuration
        /// </summary>
        /// <param name="id">Xml Configuration Id</param>
        /// <returns></returns>
        public virtual ActionResult Delete(int id = 0)
        {
            try
            {
                isSuccess = _xmlGeneratorAgent.DeleteXmlConfiguration(id);
                if (isSuccess)
                {
                    message = DynamicGridConstants.DeleteSuccessMsg;
                    status = DynamicGridConstants.Success;
                }
                else
                {
                    message = DynamicGridConstants.DeleteErrorMsg;
                    status = DynamicGridConstants.Error;
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                //ExceptionHandler(ex);
            }
            return RedirectToAction<XMLGeneratorController>(x => x.List(null));
        }

        /// <summary>
        /// Get Autocomplete Data 
        /// </summary>
        /// <param name="term"></param>
        /// <param name="entityType">Type of entity</param>
        /// <returns></returns>
        public virtual JsonResult AutoCompleteEntityName(string term = "", string entityType = "") => Json(_xmlGeneratorAgent.GetEntityNames(term, entityType), JsonRequestBehavior.AllowGet);

        /// <summary>
        /// Set column setting for create xml
        /// </summary>
        /// <param name="columnNames">Names of columns</param>
        /// <param name="viewMode">View mode </param>
        /// <param name="id">XML config Id</param>
        /// <param name="viewOptions">view Options</param>
        /// <param name="pageName">Page Name</param>
        /// <param name="objectName">Object Name</param>
        /// <param name="columnListJson">column List Json</param>
        /// <returns>Returns Partial View</returns>
        public virtual ActionResult SetColumnSettings(string columnNames = "", string viewMode = "", int id = 0, string viewOptions = "", string pageName = "", string objectName = "", string columnListJson = "")
        {
            WebGridViewModel gridViewModel = null;
            try
            {
                if (!string.IsNullOrEmpty(columnNames))
                {
                    int count = 0;
                    gridViewModel = new WebGridViewModel();

                    List<WebGridColumnViewModel> currentColumnList = null;
                    List<WebGridColumnViewModel> newColumnList = GetWebGridColumnList(columnNames);
                    if (!string.IsNullOrEmpty(columnListJson) && !Equals(columnListJson, "null"))
                    {

                        currentColumnList = new List<WebGridColumnViewModel>();
                        currentColumnList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WebGridColumnViewModel>>(columnListJson);

                        foreach (var _column in newColumnList)
                        {
                            if (!currentColumnList.Any(x => Equals(x.Name, _column.Name)))
                                currentColumnList.Add(_column);
                            int index = currentColumnList.FindIndex(x => Equals(x.Name, _column.Name));
                            var record = currentColumnList[index];
                            currentColumnList.RemoveAt(index);
                            currentColumnList.Insert(count, record);
                            count++;
                        }
                        List<WebGridColumnViewModel> changesColList = new List<WebGridColumnViewModel>();
                        changesColList.AddRange(currentColumnList);
                        foreach (var item in changesColList)
                        {
                            if (!newColumnList.Any(x => Equals(x.Name, item.Name)))
                                currentColumnList.Remove(item);
                        }

                        gridViewModel.WebGridColumnModelList = currentColumnList;
                    }
                    else
                        gridViewModel.WebGridColumnModelList = newColumnList;

                    gridViewModel.ViewMode = viewMode;
                    gridViewModel.Id = id;
                    gridViewModel.ViewOption = viewOptions;
                    gridViewModel.FrontPageName = pageName;
                    gridViewModel.FrontObjectName = objectName;
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                //ExceptionHandler(ex);
            }

            return PartialView(DynamicGridConstants.GridColumnSetting, gridViewModel);

        }

        /// <summary>
        /// View XML 
        /// </summary>
        /// <param name="id">Xml config Id</param>
        /// <returns>String XML</returns>
        public virtual string View(int id)
        {
            FilterCollectionDataModel model = new FilterCollectionDataModel();
            model.Page = PageNo;
            model.RecordPerPage = RecordPerPage;
            model.Filters = new FilterCollection();
            model.Filters.Add(new FilterTuple(ApplicationSettingId, FilterOperators.Equals, id.ToString()));
            ApplicationSettingListViewModel modelList = _xmlGeneratorAgent.ApplicationsSettingList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            string xmlString = modelList?.List[0].Setting.Trim();
            xmlString = xmlString.Replace(" ", "");
            return xmlString;
        }

        /// <summary>
        /// Get Xml data for Edit XML configuration
        /// </summary>
        /// <param name="id">Application Setting Id</param>
        /// <returns>Returns Create/Edit View</returns>
        public virtual ActionResult Edit(int applicationSettingId)
        {
            WebGridViewModel webGridModel = new WebGridViewModel();

            FilterCollectionDataModel filterModel = new FilterCollectionDataModel();
            filterModel.Page = PageNo;
            filterModel.RecordPerPage = RecordPerPage;
            filterModel.Filters = new FilterCollection();
            filterModel.Filters.Add(new FilterTuple(ApplicationSettingId, FilterOperators.Equals, applicationSettingId.ToString()));
            var xmlData = _xmlGeneratorAgent.ApplicationsSettingList(filterModel.Filters, filterModel.SortCollection, filterModel.Page, filterModel.RecordPerPage);
            string xmlString = xmlData.List[0].Setting.Trim();
            List<WebGridColumnViewModel> list = _xmlGeneratorAgent.GetListFromXMLString(xmlString);
            ListViewModel listModel = GetListViewModel(xmlData.List[0].GroupName, xmlData.List[0].ItemName, "", list);
            webGridModel.Id = applicationSettingId;
            webGridModel.WebGridColumnModelList = list;
            webGridModel.ViewMode = ViewMode.Edit.ToString();
            webGridModel.ViewOption = xmlData.List[0].ViewOptions;
            webGridModel.FrontPageName = xmlData.List[0].FrontPageName;
            webGridModel.FrontObjectName = xmlData.List[0].FrontObjectName;
            webGridModel.EntityType = xmlData.List[0].GroupName;
            webGridModel.EntityName = xmlData.List[0].ItemName;
            webGridModel.listViewModel = listModel;

            return View(DynamicGridConstants.CreateXMLView, webGridModel);
        }

        public virtual JsonResult ShowHidecolumn(string id)
        {

            var columnLists = SessionHelper.GetDataFromSession<List<dynamic>>(DynamicGridConstants.ColumnListSessionKey);
            string[] ids = id.Split(',');

            List<dynamic> ColumnListNew = new List<dynamic>();
            List<FilterColumnModel> _model = new List<FilterColumnModel>();
            foreach (var item in columnLists)
            {
                string columnName = item.name;
                item.isvisible = Convert.ToString(item.mustshow) == DynamicGridConstants.No ? (ids.Contains(columnName) ? "y" : "n") : "y";
                ColumnListNew.Add(item);
            }

            SessionHelper.SaveDataInSession<List<dynamic>>(DynamicGridConstants.ColumnListSessionKey, ColumnListNew);
            return Json("true", JsonRequestBehavior.AllowGet);
        }

        public virtual JsonResult SortAction(string id)
        {
            int[] numbers = id.Split(',').Select(int.Parse).ToArray();
            var columnLists = SessionHelper.GetDataFromSession<List<dynamic>>(DynamicGridConstants.ColumnListSessionKey);
            List<dynamic> ColumnListNew = FilterHelpers.SetIndexOrder(columnLists, numbers);
            SessionHelper.SaveDataInSession<List<dynamic>>(DynamicGridConstants.ColumnListSessionKey, ColumnListNew);
            return Json("true", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Get list view model for display selected and unselected columns.
        /// </summary>
        /// <param name="entityType">Type of entity</param>
        /// <param name="entityName">Name of entity</param>
        /// <param name="columnListJson">column List Json</param>
        /// <param name="columnList"> column List</param>
        /// <returns>Returns ListViewModel</returns>
        private ListViewModel GetListViewModel(string entityType = "", string entityName = "", string columnListJson = null, List<WebGridColumnViewModel> columnList = null)
        {
            ListViewModel listModel = new ListViewModel();
            listModel.AssignedList = null;
            listModel.UnAssignedList = null;
            try
            {
                if (!string.IsNullOrEmpty(entityType) && !string.IsNullOrEmpty(entityName))
                    listModel.AssignedList = _xmlGeneratorAgent.GetEntityColumnList(entityType, entityName);
                if (!string.IsNullOrEmpty(columnListJson) && columnListJson != "null")
                {
                    columnList = new List<WebGridColumnViewModel>();
                    columnList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WebGridColumnViewModel>>(columnListJson);
                }
                if (columnList?.Count > 0)
                {
                    IEnumerable<SelectListItem> entityNameList = columnList.Select(item => new SelectListItem()
                    {
                        Value = item.Name,
                        Text = item.Name
                    });
                    listModel.UnAssignedList = entityNameList;

                    if (listModel?.AssignedList?.Count() > 0)
                    {
                        listModel.AssignedList = listModel.AssignedList.ToList().Select(X => X.Value).Except(entityNameList.ToList().Select(X => X.Value)).Select(item => new SelectListItem()
                        {
                            Value = item,
                            Text = item
                        });
                    }
                }
                if (listModel.AssignedList == null)
                    listModel.AssignedList = new List<SelectListItem>();
                if (listModel.UnAssignedList == null)
                    listModel.UnAssignedList = new List<SelectListItem>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return listModel;
        }

        /// <summary>
        /// Create Column list from string
        /// </summary>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        private List<WebGridColumnViewModel> GetWebGridColumnList(string columnNames)
        {
            List<WebGridColumnViewModel> List = new List<WebGridColumnViewModel>();
            string[] columnNameList = columnNames.Split(',');
            foreach (string name in columnNameList)
            {
                WebGridColumnViewModel model = new WebGridColumnViewModel();
                model.Name = name;
                List.Add(model);
            }
            return List;
        }

        public ActionResult CreateFilter(string[] id, bool flag)
        {
            var result = string.Join(",", id);
            int[] numbers = result.Split(',').Select(int.Parse).ToArray();
            List<int> _tempArr = new List<int>();
            List<int> _RemovedFilters = new List<int>();
            if (SessionHelper.GetDataFromSession<List<int>>(DynamicGridConstants.FilterState) != null) _tempArr = SessionHelper.GetDataFromSession<List<int>>(DynamicGridConstants.FilterState);
            if (SessionHelper.GetDataFromSession<List<int>>(DynamicGridConstants.RemovedFilters) != null) _RemovedFilters = SessionHelper.GetDataFromSession<List<int>>(DynamicGridConstants.RemovedFilters);
            var columnLists = SessionHelper.GetDataFromSession<List<dynamic>>(DynamicGridConstants.ColumnListSessionKey);
            List<dynamic> ColumnListNew = new List<dynamic>();
            if (flag)
            {
                List<FilterColumnModel> _model = new List<FilterColumnModel>();
                foreach (var item in columnLists)
                {
                    if (numbers.ToList().IndexOf(Convert.ToInt32(item.id)) != -1)
                    {
                        _model.Add(new FilterColumnModel() { ColumnName = item.name, HeaderText = item.headertext, Id = Convert.ToInt32(item.id), DataType = item.datatype, Value = string.Empty, SelectListOfDatatype = FilterHelpers.GetOperators(item.datatype, 0) });
                        item.isallowsearch = DynamicGridConstants.Yes;
                        if (IsNotNull(_tempArr) && _tempArr.IndexOf(Convert.ToInt32(item.id)) == -1)
                            _tempArr.Add(Convert.ToInt32(item.id));

                        if (IsNotNull(_RemovedFilters) && _RemovedFilters.IndexOf(Convert.ToInt32(item.id)) != -1)
                            _RemovedFilters.Remove(Convert.ToInt32(item.id));

                    }

                    ColumnListNew.Add(item);
                }

                SessionHelper.SaveDataInSession<List<int>>(DynamicGridConstants.FilterState, _tempArr);
                SessionHelper.SaveDataInSession<List<int>>(DynamicGridConstants.RemovedFilters, _RemovedFilters);
                string viewAsString = RenderRazorViewToString("~/Views/DynamicGrid/_FilterControl.cshtml", _model);
                return Json(viewAsString, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var tempFilterList = SessionHelper.GetDataFromSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey);

                foreach (var item in columnLists)
                {
                    if (numbers.ToList().IndexOf(Convert.ToInt32(item.id)) != -1)
                    {
                        item.isallowsearch = DynamicGridConstants.No;
                        if (IsNotNull(_tempArr) && _tempArr.IndexOf(Convert.ToInt32(item.id)) != -1)
                            _tempArr.Remove(Convert.ToInt32(item.id));

                        if (IsNotNull(_RemovedFilters) && _RemovedFilters.IndexOf(Convert.ToInt32(item.id)) == -1)
                            _RemovedFilters.Add(Convert.ToInt32(item.id));

                        int index = tempFilterList.FindIndex(x => x.Item1 == item.name);
                        if (index >= 0)
                            tempFilterList.RemoveAt(index);
                    }
                    ColumnListNew.Add(item);
                }

                SessionHelper.SaveDataInSession<FilterCollection>(DynamicGridConstants.FilterCollectionsSessionKey, tempFilterList);
                SessionHelper.SaveDataInSession<List<int>>(DynamicGridConstants.FilterState, _tempArr);
                SessionHelper.SaveDataInSession<List<int>>(DynamicGridConstants.RemovedFilters, _RemovedFilters);

                return Json(id, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion


    }
}