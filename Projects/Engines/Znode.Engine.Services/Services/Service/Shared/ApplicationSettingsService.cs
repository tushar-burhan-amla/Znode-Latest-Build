using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class ApplicationSettingsService : BaseService, IApplicationSettingsService
    {
        #region private variables
        private readonly IZnodeRepository<ZnodeApplicationSetting> _applicationSetting;
        private readonly IZnodeRepository<ZnodeListView> _listView;
        private readonly IZnodeRepository<ZnodeListViewFilter> _listViewFilters;

        #endregion

        #region Constructor
        public ApplicationSettingsService()
        {
            _applicationSetting = new ZnodeRepository<ZnodeApplicationSetting>();
            _listView = new ZnodeRepository<ZnodeListView>();
            _listViewFilters = new ZnodeRepository<ZnodeListViewFilter>();
        }
        #endregion

        #region Public Methods
        //Get GetFilterConfiguration XML Data
        public virtual ApplicationSettingDataModel GetFilterConfigurationXML(string itemName, int? userId = null)
        {
            //get the application setting configuration list
            var configData = _applicationSetting.Table.FirstOrDefault(item => item.ItemName.Equals(itemName, StringComparison.InvariantCultureIgnoreCase) || item.ApplicationSettingId.ToString().Equals(itemName));

            if (configData != null)
            {
                var result = ApplicationSettingsMap.ToModel(configData);
                result.Views = GetViews(result.ApplicationSettingId, userId);
                return result;
            }
            return new ApplicationSettingDataModel();
        }

        //Create New view with filters
        public virtual ViewModel CreateNewView(ViewModel model)
        {
            var _id = _applicationSetting.Table.Where(item => item.ItemName.Equals(model.XmlSettingName, System.StringComparison.InvariantCultureIgnoreCase))?.FirstOrDefault()?.ApplicationSettingId;
            model.ApplicationSettingId = Convert.ToInt32(_id);
            model.IsSelected = true;
            UpdateIsSelectedKey(model.ApplicationSettingId);
            if (model.IsDefault)
                SetDefaultFlagsToFalse(model.ApplicationSettingId, model.UserId);

            ZnodeListView existingView = _listView.Table.FirstOrDefault(x => x.ApplicationSettingId == model.ApplicationSettingId && x.ViewName == model.ViewName);

            if (model.ViewId > 0)
            {
                if (HelperUtility.IsNotNull(existingView) && existingView.ListViewId != model.ViewId )
                    throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorViewNameExists);

                _listView.Update(ApplicationSettingsMap.ToViewEntity(model));
                UpdateViewFilters(model.Filters, model.ViewId);
                return model;
            }
            else
            {
                if (HelperUtility.IsNotNull(existingView))
                    throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorViewNameExists);

                var _entity = _listView.Insert(ApplicationSettingsMap.ToViewEntity(model));

                if (!Equals(_entity, null))
                {
                    model.ViewId = _entity.ListViewId;
                    model.CreatedBy = _entity.CreatedBy;
                    SaveViewFilters(model.Filters, _entity.ListViewId);
                    return model;
                }
            }
            return null;
        }

        private void SetDefaultFlagsToFalse(int applicationsettingId, int userId)
        {
            var model = _listView.Table.Where(x => x.ApplicationSettingId == applicationsettingId && x.CreatedBy == userId).ToList();
            if (!Equals(model))
            {
                model.ForEach(x =>
                {
                    x.IsDefault = false;
                    _listView.Update(x);
                });
            }
        }

        //Delete View
        public virtual bool DeleteView(ParameterModel listViewId)
        {
            if (HelperUtility.IsNull(listViewId) || string.IsNullOrEmpty(listViewId.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorViewIdLessThanOne);

            //Generates filter clause for multiple cmsSliderBannerIds.
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeListViewEnum.ListViewId.ToString(), ProcedureFilterOperators.In, listViewId.Ids));

            //Returns true if view deleted successfully else return false.
            bool IsDeleted = _listViewFilters.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);

            IsDeleted = _listView.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);

            ZnodeLogging.LogMessage(IsDeleted ? string.Format(Admin_Resources.SuccessViewDelete, listViewId.Ids) : Admin_Resources.ErrorViewDelete, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return IsDeleted;
        }

        //Get View by itemViewId
        public virtual ViewModel GetView(int itemViewId)
        {
            if (itemViewId > 0)
            {
                var model = (from listView in _listView.Table
                             where listView.ListViewId == itemViewId
                             select new ViewModel()
                             {
                                 ViewName = listView.ViewName,
                                 ViewId = itemViewId,
                                 ApplicationSettingId = listView.ApplicationSettingId.Value,
                                 XmlSetting = listView.XmlSetting,
                                 IsSelected = listView.IsSelected,
                                 SortColumn = listView.SortColumn,
                                 SortType = listView.SortType,
                                 IsPublic = listView.IsPublic,
                                 IsDefault = listView.IsDefault,
                                 CreatedBy = listView.CreatedBy

                             }).FirstOrDefault();
                UpdateIsSelectedKey(model.ApplicationSettingId);
                model.IsSelected = true;
                _listView.Update(ApplicationSettingsMap.ToViewEntity(model));
                model.Filters = GetFilters(model.ViewId);
                return model;
            }
            return new ViewModel();
        }

        public virtual bool UpdateViewSelectedStatus(int applicationSettingId)
        {
            var model = _listView.Table.Where(x => x.ApplicationSettingId == applicationSettingId).ToList();
            if (!Equals(model))
            {
                model.ForEach(x =>
                {
                    x.IsSelected = false;
                    _listView.Update(x);
                });
                return true;
            }
            return false;
        }

        private bool UpdateViewFilters(FilterCollection Filters, int listViewId)
        {
            var _filter = _listViewFilters.Table.Where(x => x.ListViewId == listViewId)?.ToList();

            if (!Equals(_filter, null))
            {
                _listViewFilters.Delete(_filter);
            }

            SaveViewFilters(Filters, listViewId);
            return true;
        }

        private void UpdateIsSelectedKey(int applicationsettingid)
        {
            var model = _listView.Table.Where(x => x.ApplicationSettingId == applicationsettingid).ToList();
            if (!Equals(model))
            {
                model.ForEach(x =>
                {
                    x.IsSelected = false;
                    _listView.Update(x);
                });
            }
        }
        private FilterCollection GetFilters(int listViewId)
        {
            var filterCollection = new FilterCollection();
            var filter = from listViewFilters in _listViewFilters.Table
                         where listViewFilters.ListViewId == listViewId
                         select new
                         {
                             filterName = listViewFilters.FilterName,
                             Operator = listViewFilters.Operator,
                             value = listViewFilters.Value
                         };

            foreach (var item in filter)
            {
                filterCollection.Add(item.filterName.ToString(), item.Operator.ToString(), item.value.ToString());
            }

            return filterCollection;
        }

        private bool SaveViewFilters(FilterCollection Filters, int listViewId)
        {
            if (!Equals(Filters, null))
            {
                Filters.ForEach(x =>
                {
                    _listViewFilters.Insert(ApplicationSettingsMap.ToViewFilterEntity(x, listViewId));
                });
            }

            return true;
        }

        private List<ViewModel> GetViews(long applicationSettingId, int? userId)
        {
            var _views = _listView.Table.Where((x => x.ApplicationSettingId == applicationSettingId && (x.IsPublic || (x.CreatedBy == userId && !x.IsPublic)))).AsEnumerable().OrderByDescending(x => x.IsDefault).ThenByDescending(x => x.CreatedDate);
            if (!Equals(_views, null))
            {
                return (from n in _views
                        select new ViewModel
                        {
                            ViewId = n.ListViewId,
                            ViewName = n.ViewName,
                            CreatedBy = n.CreatedBy,
                            IsDefault = n.IsDefault,
                            IsPublic = n.IsPublic,
                            IsSelected = n.IsSelected,
                            SortColumn = n.IsDefault && n.CreatedBy == userId ? n.SortColumn : null,
                            SortType = n.IsDefault && n.CreatedBy == userId ? n.SortType : null,
                            XmlSetting = n.IsDefault && n.CreatedBy == userId ? n.XmlSetting : string.Empty,
                            Filters = n.IsDefault && n.CreatedBy == userId ? GetFilters(n.ListViewId) : null
                        }).ToList();
            }
            return new List<ViewModel>();
        }
        #endregion

        #region XML Edit

        //Get applicationSetting list  with paging
        public virtual ApplicationSettingListModel GetApplicationSettings(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            //Set paging.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            //Gets the entity list according to where clause, order by clause and pagination and maps the entity list to model
            ApplicationSettingListModel list = ApplicationSettingsMap.ToListModel(_applicationSetting.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount));
            //Set for pagination
            list.BindPageListModel(pageListModel);

            // return list with paging.
            return list;
        }

        //Get column list
        public virtual ApplicationSettingListModel GetColumnList(string entityType, string entityName)
        {
            entityName = entityName.Equals("Empty") ? null : entityName;
            //Get column list from Database by entityType and entityName
            IZnodeViewRepository<View_GetObjectColumnList> objStoredProc = new ZnodeViewRepository<View_GetObjectColumnList>();

            //SP parameters
            objStoredProc.SetParameter("@Type", entityType, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Object_Name", entityName, ParameterDirection.Input, DbType.String);
            var objectColumnList = objStoredProc.ExecuteStoredProcedureList("Znode_GetObjectColumnList @Type, @Object_Name");
            return ApplicationSettingsMap.ToEntityColumnListModel(objectColumnList);
        }

        //Save xml configuration.
        public virtual bool SaveXmlConfiguration(ApplicationSettingDataModel model)
        {
            bool result = false;
            if (!Equals(model, null))
            {
                // Check action mode  "INSERT"  or "UPDATE"
                if (model.ActionMode.Equals("INSERT"))
                {
                    //Save XmlConfiguration
                    if (!Equals(_applicationSetting.Insert(ApplicationSettingsMap.ToEntity(model)), null))

                        result = true;
                    else
                        result = false;

                    ZnodeLogging.LogMessage(result ? Admin_Resources.SuccessXmlSettingInsert : Admin_Resources.ErrorXmlSettingInsert, string.Empty, TraceLevel.Info);
                }
                else if (model.ActionMode.Equals("DELETE"))
                {
                    //Delete XmlConfiguration
                    result = _applicationSetting.Delete(ApplicationSettingsMap.ToEntity(model));
                    ZnodeLogging.LogMessage(result ? string.Format(Admin_Resources.SuccessXmlSettingDelete, model.ApplicationSettingId) : Admin_Resources.ErrorXmlSettingDelete, string.Empty, TraceLevel.Info);
                }
                else if (model.ActionMode.Equals("UPDATE"))
                {
                    //GetAppSetting by Id (this method use for load application setting data to context for Update the records
                    ZnodeApplicationSetting AppSettingData = _applicationSetting.GetById(model.ApplicationSettingId);
                    model.OrderByFields = AppSettingData.OrderByFields;
                    //Map ApplicationSettingModel with ZNodeApplicationSetting Entity and update data
                    result = _applicationSetting.Update(ApplicationSettingsMap.ToEntity(model));
                    ZnodeLogging.LogMessage(result ? string.Format(Admin_Resources.SuccessXmlSettingUpdate, model.ApplicationSettingId) : Admin_Resources.ErrorXmlSettingUpdate, string.Empty, TraceLevel.Info);
                }
            }
            return result;
        }
        #endregion
    }
}
