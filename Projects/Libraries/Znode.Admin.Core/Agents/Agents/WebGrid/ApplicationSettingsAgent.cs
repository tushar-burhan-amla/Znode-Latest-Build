using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Enum;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using Znode.Engine.Exceptions;
using System.Diagnostics;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Admin.Agents
{
    public class ApplicationSettingsAgent : BaseAgent, IApplicationSettingsAgent
    {
        #region Private Variables
        private readonly IApplicationSettingsClient _applicationSettings;
        private const string ActionModeInsert = "INSERT";
        private const string ActionModeUpdate = "UPDATE";
        private const string ActionModeDelete = "DELETE";
        private const string EntityTypeTable = "Table";
        private const string EntityTypeView = "View";
        private const string EntityTypeProc = "StoredProcedure";
        private const string ObjectName = "ObjectName";
        private const string All = "All";
        private const string Columns = "columns";
        private const string EmptyString = "Empty";
        private const string ColumnName = "ColumnName";
        #endregion

        #region Constructor
        public ApplicationSettingsAgent(IApplicationSettingsClient applicationSettings)
        {
            _applicationSettings = GetClient<IApplicationSettingsClient>(applicationSettings);
        }
        #endregion

        #region Public  Methods
        public virtual ApplicationSettingDataModel GetFilterConfigurationXML(string nameOfXML, int? userId = null) => !string.IsNullOrEmpty(nameOfXML) ? _applicationSettings.GetFilterConfigurationXML(nameOfXML, userId) : new ApplicationSettingDataModel();

        public virtual Models.ViewModel CreateNewView(Models.ViewModel model)
        {
            var result = _applicationSettings.CreateNewView(new Api.Models.ViewModel { ViewId = model.ViewId, ViewName = model.ViewName, XmlSetting = model.XmlSetting, Filters = model.Filters, XmlSettingName = model.XmlSettingName, SortColumn = model.SortColumn, SortType = model.SortType, IsPublic = model.IsPublic, IsDefault = model.IsDefault, UserId = model.UserId, CreatedBy = model.CreatedBy });
            return XmlGeneratorModelMap.ToListViewModel(result);
        }

        public virtual bool DeleteView(string itemViewId)
        {
            string message = Admin_Resources.ErrorFailedToDelete;
            if (!string.IsNullOrEmpty(itemViewId))
            {
                try
                {
                    return _applicationSettings.DeleteView(new ParameterModel { Ids = itemViewId });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                    message = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    message = Admin_Resources.ErrorFailedToDelete;
                    return false;
                }
            }
            return false;
        }

        //Return listviewmodel
        public virtual Models.ViewModel GetView(int itemViewId)
        {
            var result = _applicationSettings.GetView(itemViewId);
            return XmlGeneratorModelMap.ToListViewModel(result);
        }

        #region XML Editor public virtual Methods

        //Returns  ApplicationSettingListViewModel list with filtered data
        public virtual ApplicationSettingListViewModel ApplicationsSettingList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            try
            {
                ApplicationSettingListModel model = _applicationSettings.GetApplicationSettings(filters, sortCollection, pageIndex, recordPerPage);
                return XmlGeneratorModelMap.ToListModel(model);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                throw;
            }

        }

        //Save XmlConfiguration 
        public virtual bool SaveXmlConfiguration(string xmlString, string viewOptions, string entityType, string entityName, string frontPageName, string frontObjectName, int id)
        {
            //Convert to ApplicationSettingModel 
            ApplicationSettingModel model = XmlGeneratorModelMap.ToApplicationSettingModel(xmlString, viewOptions, entityType, entityName, frontPageName, frontObjectName, id);

            //Set action mode for save xml configuration
            model.ActionMode = ActionModeInsert;
            return _applicationSettings.SaveXmlConfiguration(XmlGeneratorModelMap.ToDataModel(model));
        }

        //Update XmlConfiguration
        public virtual bool UpdateXmlConfiguration(string xmlString, string viewOptions, string entityType, string entityName, string frontPageName, string frontObjectName, int id)
        {
            //Convert to ApplicationSettingModel 
            ApplicationSettingModel model = XmlGeneratorModelMap.ToApplicationSettingModel(xmlString, viewOptions, entityType, entityName, frontPageName, frontObjectName, id);

            //Set action mode for save xml configuration
            model.ActionMode = ActionModeUpdate;
            return _applicationSettings.SaveXmlConfiguration(XmlGeneratorModelMap.ToDataModel(model));
        }

        // Delete XmlConfiguration
        public virtual bool DeleteXmlConfiguration(int id)
            => _applicationSettings.SaveXmlConfiguration(new ApplicationSettingDataModel() { ApplicationSettingId = id, ActionMode = ActionModeDelete });

        //Returns entity columns DataSet by entityType and entityname
        public virtual List<EntityColumnModel> GetObjectColumnList(string entityType, string entityName)
            => _applicationSettings.GetColumnList(entityType, string.IsNullOrEmpty(entityName) ? EmptyString : entityName);

        //Returns column list by entityType and entityname
        public virtual List<SelectListItem> GetEntityColumnList(string entityType, string entityName)
        {
            List<SelectListItem> columnList = new List<SelectListItem>();

            if (!string.IsNullOrEmpty(entityType) && !string.IsNullOrEmpty(entityName))
            {
                if (Equals(entityType, EntityTypeTable))
                    entityType = GetObjectColumnListParameter.U.ToString();
                else if (Equals(entityType, EntityTypeView))
                    entityType = GetObjectColumnListParameter.V.ToString();
                List<EntityColumnModel> entityColumnList = new List<EntityColumnModel>();
                if (Equals(entityType, EntityTypeProc))
                    entityColumnList = GetObjectColumnList(GetObjectColumnListParameter.P.ToString(), entityName);
                else
                    entityColumnList = GetObjectColumnList(entityType, entityName);
                if (entityColumnList?.Count > 0)
                    foreach (EntityColumnModel _dataRow in entityColumnList)
                    {
                        columnList.Add(new SelectListItem() { Value = Convert.ToString(_dataRow.ColumnName), Text = Convert.ToString(_dataRow.ColumnName) });
                    }

                // set default columns 
                columnList.Add(new SelectListItem() { Text = DynamicGridConstants.EditKey, Value = DynamicGridConstants.EditKey });
                columnList.Add(new SelectListItem() { Text = DynamicGridConstants.DeleteKey, Value = DynamicGridConstants.DeleteKey });
                columnList.Add(new SelectListItem() { Text = DynamicGridConstants.ManageKey, Value = DynamicGridConstants.ManageKey });
                columnList.Add(new SelectListItem() { Text = DynamicGridConstants.CopyKey, Value = DynamicGridConstants.CopyKey });
                columnList.Add(new SelectListItem() { Text = DynamicGridConstants.ViewKey, Value = DynamicGridConstants.ViewKey });
                columnList.Add(new SelectListItem() { Text = DynamicGridConstants.ImageKey, Value = DynamicGridConstants.ImageKey });
            }
            return columnList;
        }
        //Convert WebGridColumn list to xml format and returns  as  string  
        public virtual string CreateXMLFile(List<WebGridColumnViewModel> list)
        {
            string xmlFile = string.Empty;
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            writer.WriteStartDocument();
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            WriteXMLString(list, writer);
            writer.WriteEndDocument();
            writer.Flush();
            xmlFile = stringWriter.ToString();
            writer.Close();
            return xmlFile;
        }
        //Generate WebGridColumnModel from string and returns List of WebGridColumnModel
        public virtual List<WebGridColumnViewModel> GetListFromXMLString(string xmlString)
        {
            List<WebGridColumnViewModel> list = new List<WebGridColumnViewModel>();
            XDocument xmlDoc = XDocument.Parse(xmlString);
            list = (from xml in xmlDoc.Descendants(DynamicGridConstants.columnKey)
                    select new WebGridColumnViewModel()
                    {
                        Id = Convert.ToInt32(xml.TryGetElementValue(DynamicGridConstants.IdKey)),
                        Name = xml.TryGetElementValue(DynamicGridConstants.nameKey),
                        Headertext = xml.TryGetElementValue(DynamicGridConstants.headertextKey),
                        Width = Convert.ToInt32(xml.TryGetElementValue(DynamicGridConstants.WidthKey)),
                        Datatype = xml.TryGetElementValue(DynamicGridConstants.datatypeKey),
                        Columntype = xml.TryGetElementValue(DynamicGridConstants.ColumntypeKey),
                        Allowsorting = Convert.ToBoolean(xml.TryGetElementValue(DynamicGridConstants.AllowsortingKey)),
                        Allowpaging = Convert.ToBoolean(xml.TryGetElementValue(DynamicGridConstants.AllowpagingKey)),
                        Format = xml.TryGetElementValue(DynamicGridConstants.FormatKey),
                        Isvisible = Convert.ToChar(xml.TryGetElementValue(DynamicGridConstants.isvisibleKey)),
                        Mustshow = Convert.ToChar(xml.TryGetElementValue(DynamicGridConstants.MustshowKey)),
                        Musthide = Convert.ToChar(xml.TryGetElementValue(DynamicGridConstants.MusthideKey)),
                        Maxlength = Convert.ToInt32(xml.TryGetElementValue(DynamicGridConstants.MaxlengthKey, null, "Int")),
                        Isallowsearch = Convert.ToChar(xml.TryGetElementValue(DynamicGridConstants.isallowsearchKey)),
                        Isconditional = Convert.ToChar(xml.TryGetElementValue(DynamicGridConstants.IsconditionalKey)),

                        Isallowlink = Convert.ToChar(xml.TryGetElementValue(DynamicGridConstants.isallowlinkKey)),
                        Islinkactionurl = xml.TryGetElementValue(DynamicGridConstants.islinkactionurlKey),
                        Islinkparamfield = xml.TryGetElementValue(DynamicGridConstants.islinkparamfieldKey),
                        Ischeckbox = Convert.ToChar(xml.TryGetElementValue(DynamicGridConstants.ischeckboxKey, null, "Char")),
                        Checkboxparamfield = xml.TryGetElementValue(DynamicGridConstants.checkboxparamfieldKey),
                        Iscontrol = Convert.ToChar(xml.TryGetElementValue(DynamicGridConstants.iscontrolKey, null, "Char")),
                        Controltype = xml.TryGetElementValue(DynamicGridConstants.controltypeKey),
                        Controlparamfield = xml.TryGetElementValue(DynamicGridConstants.controlparamfieldKey),
                        Displaytext = xml.TryGetElementValue(DynamicGridConstants.displaytextKey),
                        Editactionurl = xml.TryGetElementValue(DynamicGridConstants.editactionurlKey),
                        Editparamfield = xml.TryGetElementValue(DynamicGridConstants.editparamfieldKey),
                        Deleteactionurl = xml.TryGetElementValue(DynamicGridConstants.deleteactionurlKey),
                        Deleteparamfield = xml.TryGetElementValue(DynamicGridConstants.deleteparamfieldKey),
                        Viewactionurl = xml.TryGetElementValue(DynamicGridConstants.viewactionurlKey),
                        Viewparamfield = xml.TryGetElementValue(DynamicGridConstants.viewparamfieldKey),
                        Imageactionurl = xml.TryGetElementValue(DynamicGridConstants.imageactionurlKey),
                        Imageparamfield = xml.TryGetElementValue(DynamicGridConstants.imageparamfieldKey),
                        Manageactionurl = xml.TryGetElementValue(DynamicGridConstants.manageactionurlKey),
                        Manageparamfield = xml.TryGetElementValue(DynamicGridConstants.manageparamfieldKey),
                        Copyactionurl = xml.TryGetElementValue(DynamicGridConstants.copyactionurlKey),
                        Copyparamfield = xml.TryGetElementValue(DynamicGridConstants.copyparamfieldKey),
                        XAxis = Convert.ToChar(xml.TryGetElementValue(DynamicGridConstants.xaxis, null, "Char")),
                        YAxis = Convert.ToChar(xml.TryGetElementValue(DynamicGridConstants.yaxis, null, "Char")),
                        IsAdvanceSearch = Convert.ToChar(xml.TryGetElementValue(DynamicGridConstants.isadvancesearch, null, "Char")),
                        Class = xml.TryGetElementValue(DynamicGridConstants.ClassKey),
                        SearchControlType = xml.TryGetElementValue(DynamicGridConstants.SearchControlTypeKey),
                        SearchControlParameters = xml.TryGetElementValue(DynamicGridConstants.SearchControlParametersKey),
                        DbParamField = xml.TryGetElementValue(DynamicGridConstants.DbParamFieldKey),
                        UseMode = xml.TryGetElementValue(DynamicGridConstants.UseMode),
                        IsGraph = xml.TryGetElementValue(DynamicGridConstants.IsGraphKey),
                        AllowDetailView = Convert.ToChar(xml.TryGetElementValue(DynamicGridConstants.isallowdetailKey)),
                    }).ToList();
            return list;
        }

        public virtual IEnumerable<SelectListItem> GetEntityNames(string term = "", string entityType = "")
        {
            IEnumerable<SelectListItem> entityNameList = null;

            List<EntityColumnModel> entityColumnList = GetObjectName(entityType, All);

            if (entityColumnList?.Count > 0)
            {
                entityNameList = (from data in entityColumnList
                                  select new SelectListItem
                                  {
                                      Value = data.ObjectName,
                                      Text = data.ObjectName
                                  }).Distinct();
                if (entityNameList.Count() > 1)
                {
                    var DistinctObjectList = entityNameList.Select(c => c.Value).Where(c => c.Contains(term)).Distinct();

                    entityNameList = (from obj in DistinctObjectList
                                      select new SelectListItem
                                      {
                                          Value = obj,
                                          Text = obj
                                      });
                }

            }
            return entityNameList;
        }

        public bool UpdateViewSelectedStatus(int applicationSettingId)
        {
            return _applicationSettings.UpdateViewSelectedStatus(applicationSettingId);
        }

        #endregion
        #endregion

        #region XML Editor Private Methods

        // Get Names of tables/views/procedures
        private List<EntityColumnModel> GetObjectName(string entityType, string entityName)
        {
            List<EntityColumnModel> entityColumnList = new List<EntityColumnModel>();
            if (!string.IsNullOrEmpty(entityType) && !string.IsNullOrEmpty(entityName))
            {
                if (Equals(entityName, All))
                    entityName = null;
                if (Equals(entityType, EntityTypeTable))
                    entityType = GetObjectColumnListParameter.U.ToString();
                else if (Equals(entityType, EntityTypeView))
                    entityType = GetObjectColumnListParameter.V.ToString();
                else
                    entityType = GetObjectColumnListParameter.P.ToString();

                List<EntityColumnModel> dsColumn = GetObjectColumnList(entityType, entityName);
                if (dsColumn?.Count > 0)
                    entityColumnList = dsColumn;
            }
            return entityColumnList;
        }

        // Create XMl string from WebGridColumnModel list
        private void WriteXMLString(List<WebGridColumnViewModel> List, XmlTextWriter writer)
        {
            if (!Equals(List, null))
            {
                var xmlElement = new XElement(Columns,
                      from column in List
                      select new XElement(DynamicGridConstants.columnKey,
                          new XElement(DynamicGridConstants.IdKey, column.Id),
                          new XElement(DynamicGridConstants.nameKey, column.Name),
                          new XElement(DynamicGridConstants.headertextKey, column.Headertext),
                          new XElement(DynamicGridConstants.WidthKey, column.Width),
                          new XElement(DynamicGridConstants.datatypeKey, column.Datatype),
                          new XElement(DynamicGridConstants.ColumntypeKey, column.Columntype),
                          new XElement(DynamicGridConstants.AllowsortingKey, column.Allowsorting),
                          new XElement(DynamicGridConstants.AllowpagingKey, column.Allowpaging),
                          new XElement(DynamicGridConstants.FormatKey, column.Format),
                          new XElement(DynamicGridConstants.isvisibleKey, column.Isvisible),
                          new XElement(DynamicGridConstants.MustshowKey, column.Mustshow),
                          new XElement(DynamicGridConstants.MusthideKey, column.Musthide),
                          new XElement(DynamicGridConstants.MaxlengthKey, column.Maxlength),
                          new XElement(DynamicGridConstants.isallowsearchKey, column.Isallowsearch),
                          new XElement(DynamicGridConstants.IsconditionalKey, column.Isconditional),
                          new XElement(DynamicGridConstants.isallowlinkKey, column.Isallowlink),
                          new XElement(DynamicGridConstants.islinkactionurlKey, column.Islinkactionurl),
                          new XElement(DynamicGridConstants.islinkparamfieldKey, column.Islinkparamfield),
                          new XElement(DynamicGridConstants.ischeckboxKey, column.Ischeckbox),
                          new XElement(DynamicGridConstants.checkboxparamfieldKey, column.Checkboxparamfield),
                          new XElement(DynamicGridConstants.iscontrolKey, column.Iscontrol),
                          new XElement(DynamicGridConstants.controltypeKey, column.Controltype),
                          new XElement(DynamicGridConstants.controlparamfieldKey, column.Controlparamfield),//
                          new XElement(DynamicGridConstants.displaytextKey, column.Displaytext),
                          new XElement(DynamicGridConstants.editactionurlKey, column.Editactionurl),
                          new XElement(DynamicGridConstants.editparamfieldKey, column.Editparamfield),
                          new XElement(DynamicGridConstants.deleteactionurlKey, column.Deleteactionurl),
                          new XElement(DynamicGridConstants.deleteparamfieldKey, column.Deleteparamfield),
                          new XElement(DynamicGridConstants.viewactionurlKey, column.Viewactionurl),
                          new XElement(DynamicGridConstants.viewparamfieldKey, column.Viewparamfield),
                          new XElement(DynamicGridConstants.imageactionurlKey, column.Imageactionurl),
                          new XElement(DynamicGridConstants.imageparamfieldKey, column.Imageparamfield),
                          new XElement(DynamicGridConstants.manageactionurlKey, column.Manageactionurl),
                          new XElement(DynamicGridConstants.manageparamfieldKey, column.Manageparamfield),
                          new XElement(DynamicGridConstants.copyactionurlKey, column.Copyactionurl),
                          new XElement(DynamicGridConstants.copyparamfieldKey, column.Copyparamfield),
                          new XElement(DynamicGridConstants.xaxis, column.XAxis),
                          new XElement(DynamicGridConstants.yaxis, column.YAxis),
                          new XElement(DynamicGridConstants.isadvancesearch, column.IsAdvanceSearch),
                          new XElement(DynamicGridConstants.ClassKey, column.Class),
                          new XElement(DynamicGridConstants.SearchControlTypeKey, column.SearchControlType),
                          new XElement(DynamicGridConstants.SearchControlParametersKey, column.SearchControlParameters),
                          new XElement(DynamicGridConstants.DbParamFieldKey, column.DbParamField),
                          new XElement(DynamicGridConstants.UseMode, column.UseMode),
                          new XElement(DynamicGridConstants.IsGraphKey, column.IsGraph),
                           new XElement(DynamicGridConstants.isallowdetailKey, column.AllowDetailView)
                                 ));

                xmlElement.WriteTo(writer);

            }
        }
        #endregion


        #region View

        #endregion
    }
}