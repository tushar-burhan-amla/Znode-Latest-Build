using System;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.Models;
using Znode.Engine.WebStore.ViewModels;

namespace Znode.Engine.WebStore.Maps
{
    public static  class XmlGeneratorModelMap
    {
        public static ApplicationSettingListViewModel ToListModel(ApplicationSettingListModel model)
        {
            if (!Equals(model, null) && !Equals(model.ApplicationSettingList, null))
            {
                var viewModel = new ApplicationSettingListViewModel()
                {
                    List = model.ApplicationSettingList.ToList().Select(
                    x => new ApplicationSettingModel()
                    {
                        ApplicationSettingId = x.ApplicationSettingId,
                        GroupName = x.GroupName,
                        ItemName = x.ItemName,
                        Setting = x.Setting,
                        ViewOptions = x.ViewOptions,
                        FrontPageName = x.FrontPageName,
                        FrontObjectName = x.FrontObjectName,
                        IsCompressed = x.IsCompressed
                    }).ToList()
                };

                viewModel.RecordPerPage = Convert.ToInt32(model.PageSize);
                viewModel.Page = Convert.ToInt32(model.PageIndex);
                viewModel.TotalResults = Convert.ToInt32(model.TotalResults);
                return viewModel;
            }
            return new ApplicationSettingListViewModel();
        }

        public static ApplicationSettingDataModel ToDataModel(ApplicationSettingModel model)
        {
            return new ApplicationSettingDataModel()
            {
                SettingTableName = model.SettingTableName,
                ApplicationSettingId = model.ApplicationSettingId,
                GroupName = model.GroupName,
                ItemName = model.ItemName,
                Setting = model.Setting,
                CreatedBy = model.CreatedBy,
                ModifiedBy = model.ModifiedBy,
                CreatedByName = model.CreatedByName,
                ModifiedByName = model.ModifiedByName,
                ViewOptions = model.ViewOptions,
                FrontPageName = model.FrontPageName,
                FrontObjectName = model.FrontObjectName,
                IsCompressed = model.IsCompressed,
                ActionMode = model.ActionMode
            };
        }

        public static ApplicationSettingModel ToApplicationSettingModel(string xmlSTring, string viewOptions, string entityType, string entityName, string frontPageName, string frontObjectName, int id)
        {
            return new ApplicationSettingModel()
            {
                ApplicationSettingId = id,
                ViewOptions = viewOptions,
                GroupName = entityType,
                ItemName = entityName,
                Setting = xmlSTring,
                FrontPageName = frontPageName,
                FrontObjectName = frontObjectName
            };
        }
    }
}