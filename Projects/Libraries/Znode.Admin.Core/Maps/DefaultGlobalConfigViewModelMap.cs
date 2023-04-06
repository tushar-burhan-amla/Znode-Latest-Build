using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Maps
{
    public class DefaultGlobalConfigViewModelMap
    {
        public static DefaultGlobalConfigViewModel ToViewModel(DefaultGlobalConfigModel model)
        {
            if (Equals(model, null))
                return new DefaultGlobalConfigViewModel();

            return new DefaultGlobalConfigViewModel
            {
                Id = model.ZNodeGlobalSettingId,
                Name = model.FeatureName,
                Value = model.FeatureValues,
                SelectedIds = model.SelectedIds,
                Action = model.Action
            };
        }

        public static DefaultGlobalConfigListViewModel ToListViewModel(DefaultGlobalConfigListModel listModel)
        {
            if (listModel?.DefaultGlobalConfigs?.Count > 0)
            {
                DefaultGlobalConfigListViewModel listViewModel = new DefaultGlobalConfigListViewModel()
                {
                    DefaultGlobalConfigs = listModel.DefaultGlobalConfigs.Select(
                      model => new DefaultGlobalConfigViewModel()
                      {
                          Id = model.ZNodeGlobalSettingId,
                          Name = model.FeatureName,
                          Value = model.FeatureValues,
                          SelectedIds = model.SelectedIds,
                          Action = model.Action
                      }).ToList()
                };
                return listViewModel;
            }
            else
                return new DefaultGlobalConfigListViewModel();
        }

        public static DefaultGlobalConfigListModel ToGlobalConfigurationListModel(DefaultGlobalConfigViewModel viewModel)
        {
            DefaultGlobalConfigListModel listModel = new DefaultGlobalConfigListModel();

            if (!string.IsNullOrEmpty(viewModel?.SelectedIds))
            {
                foreach (string Id in viewModel.SelectedIds.Split(','))
                {
                    listModel.DefaultGlobalConfigs.Add(new DefaultGlobalConfigModel { SelectedIds = Id, Action = viewModel.Action});
                }
            }
            return listModel;
        }
    }
}
