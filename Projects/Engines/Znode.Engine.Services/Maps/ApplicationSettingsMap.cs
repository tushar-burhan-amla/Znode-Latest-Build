using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services.Maps
{
    public static class ApplicationSettingsMap
    {
        //Convert ZNodeApplicationSetting entity in to ApplicationSettingDataModel model.
        public static ApplicationSettingDataModel ToModel(ZnodeApplicationSetting entity)
        {
            if (!Equals(entity, null))
            {
                return new ApplicationSettingDataModel
                {
                    ApplicationSettingId = entity.ApplicationSettingId,
                    GroupName = entity.GroupName,
                    ItemName = entity.ItemName,
                    Setting = entity.Setting,
                    CreatedDate = entity.CreatedDate,
                    ModifiedDate = entity.ModifiedDate,
                    CreatedBy = entity.CreatedBy,
                    ModifiedBy = entity.ModifiedBy,
                    CreatedByName = entity.CreatedByName,
                    ModifiedByName = entity.ModifiedByName,
                    ViewOptions = entity.ViewOptions,
                    FrontPageName = entity.FrontPageName,
                    FrontObjectName = entity.FrontObjectName,
                    IsCompressed = entity.IsCompressed,
                    OrderByFields=entity.OrderByFields

                };
            }
            else
                return null;
        }

        //Convert IList<ZNodeApplicationSetting> in to ApplicationSettingListModel model.
        public static ApplicationSettingListModel ToListModel(IList<ZnodeApplicationSetting> entity)
        {
            if (!Equals(entity, null))
            {
                ApplicationSettingListModel model = new ApplicationSettingListModel();
                foreach (ZnodeApplicationSetting item in entity)
                {
                    model.ApplicationSettingList.Add(ToModel(item));
                }
                return model;
            }
            else
                return null;
        }

        //Convert ApplicationSettingDataModel model in to ZNodeApplicationSetting entity.
        public static ZnodeApplicationSetting ToEntity(ApplicationSettingDataModel model)
        {
            if (!Equals(model, null))
            {
                return new ZnodeApplicationSetting
                {
                    ApplicationSettingId = (int)model.ApplicationSettingId,
                    ItemName = model.ItemName,
                    GroupName = model.GroupName,
                    Setting = model.Setting,
                    CreatedDate = model.CreatedDate,
                    ModifiedDate = model.ModifiedDate,
                    CreatedBy = model.CreatedBy,
                    ModifiedBy = model.ModifiedBy,
                    CreatedByName = model.CreatedByName,
                    ModifiedByName = model.ModifiedByName,
                    ViewOptions = model.ViewOptions,
                    FrontPageName = model.FrontPageName,
                    FrontObjectName = model.FrontObjectName,
                    IsCompressed = model.IsCompressed,
                    OrderByFields=model.OrderByFields
                };
            }
            else
                return null;
        }

        //Convert View_GetObjectColumnList to EntityColumnModel
        public static EntityColumnModel ToEntityColumnModel(View_GetObjectColumnList entity)
        {
            if (!Equals(entity, null))
            {
                return new EntityColumnModel
                {
                    ColumnId = entity.columnId,
                    ColumnName = entity.ColumnName,
                    DataType = entity.DataType,
                    ObjectName = entity.ObjectName,
                };
            }
            else
                return null;
        }

        //Convert IEnumerable<View_GetObjectColumnList> to ApplicationSettingListModel.
        public static ApplicationSettingListModel ToEntityColumnListModel(IEnumerable<View_GetObjectColumnList> entityList)
        {
            if (!Equals(entityList, null))
            {
                ApplicationSettingListModel model = new ApplicationSettingListModel();
                foreach (View_GetObjectColumnList item in entityList)
                {
                    model.ColumnListList.Add(ToEntityColumnModel(item));
                }
                return model;
            }
            else
                return null;
        }


        public static ZnodeListView ToViewEntity(ViewModel model)
        {
            if (!Equals(model, null))
            {
                return new ZnodeListView
                {
                    ListViewId = model.ViewId,
                    ViewName = model.ViewName,
                    XmlSetting = model.XmlSetting,
                    ApplicationSettingId = model.ApplicationSettingId,
                    IsSelected = model.IsSelected,
                    SortColumn=model.SortColumn,
                    SortType=model.SortType,
                    IsPublic=model.IsPublic,
                    IsDefault=model.IsDefault
                };
            }

            return null;
        }

        public static ZnodeListViewFilter ToViewFilterEntity(FilterTuple Filters, int listViewId)
        {
            if (!Equals(Filters, null))
            {
                return new ZnodeListViewFilter
                {
                    ListViewId = listViewId,
                    FilterName = Filters.FilterName,
                    Operator = Filters.FilterOperator,
                    Value = Filters.FilterValue

                };
            }
            return null;
        }
    }
}
