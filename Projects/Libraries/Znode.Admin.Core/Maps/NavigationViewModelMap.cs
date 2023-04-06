using Znode.Engine.Api.Models;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Maps
{
    public class NavigationViewModelMap
    {
        public static NavigationViewModel ToViewModel(NavigationModel model, string Id, string controller, string areaName, string editAction, string deleteAction, string detailsAction)
        {
            if (!Equals(model, null))
            {
                NavigationViewModel viewModel = new NavigationViewModel()
                {
                    AreaName = !string.IsNullOrEmpty(areaName) ? areaName : string.Empty,
                    Controller = !string.IsNullOrEmpty(controller) ? controller : string.Empty,
                    EditAction = !string.IsNullOrEmpty(editAction) ? editAction : string.Empty,
                    DeleteAction = !string.IsNullOrEmpty(deleteAction) ? deleteAction : string.Empty,
                    DetailAction = !string.IsNullOrEmpty(detailsAction) ? detailsAction : string.Empty,
                    ID = !string.IsNullOrEmpty(Id) ? Id : string.Empty,
                    CurrentIndex = model.CurrentIndex,
                    TotalCount = model.TotalCount.GetValueOrDefault(),
                    NextID = !string.IsNullOrEmpty(model.NextRecordId) ? model.NextRecordId : string.Empty,
                    PreviousID = !string.IsNullOrEmpty(model.PreviousRecordId) ? model.PreviousRecordId : string.Empty,
                };
                return viewModel;
            }
            else
            {
                return new NavigationViewModel();
            }
        }
    }
}