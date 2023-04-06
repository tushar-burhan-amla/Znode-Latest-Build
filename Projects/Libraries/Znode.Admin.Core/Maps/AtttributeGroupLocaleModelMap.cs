using Znode.Engine.Api.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Admin.Extensions;

namespace Znode.Engine.Admin.Maps
{
    public class AtttributeGroupLocaleModelMap
    {
        //This method  maps AttributeGroupLocaleViewModel to AttributeGroupLocaleModel and returns AttributeGroupLocaleModel.
        public static AttributeGroupLocaleModel ToModel(AttributeGroupLocaleViewModel viewModel)
        {
            if (!Equals(viewModel, null))
            {
                return new AttributeGroupLocaleModel
                {
                   MediaAttributeGroupLocaleId = viewModel.MediaAttributeGroupLocaleId,
                   MediaAttributeGroupId = viewModel.MediaAttributeGroupId,
                   LocaleId = viewModel.LocaleId,
                   AttributeGroupName = viewModel.AttributeGroupName,
                   Description = viewModel.Description,
                   CreatedBy = viewModel.CreatedBy,
                   ModifiedBy = viewModel.ModifiedBy,
                   MediaCategoryId = viewModel.MediaCategoryId,
                };
            }
            return new AttributeGroupLocaleModel();
        }

        //This method  maps AttributeGroupLocaleModel to AttributeGroupLocaleViewModel and returns AttributeGroupLocaleViewModel.
        public static AttributeGroupLocaleViewModel ToViewModel(AttributeGroupLocaleModel model)
        {
            if (!Equals(model, null))
            {
                return new AttributeGroupLocaleViewModel
                {
                    MediaAttributeGroupLocaleId = model.MediaAttributeGroupLocaleId,
                    MediaAttributeGroupId = model.MediaAttributeGroupId,
                    LocaleId = model.LocaleId,
                    AttributeGroupName = model.AttributeGroupName,
                    Description = model.Description,
                    CreatedBy = model.CreatedBy,
                    CreatedDate = model.CreatedDate.ToDateTimeFormat(),
                    ModifiedBy = model.ModifiedBy,
                    ModifiedDate = model.ModifiedDate.ToDateTimeFormat(),
                    MediaCategoryId = model.MediaCategoryId,
                };
            }
            return new AttributeGroupLocaleViewModel();
        }

        //This method  maps LocaleViewModel to AttributeGroupLocaleViewModel and returns AttributeGroupLocaleViewModel.
        public static AttributeGroupLocaleViewModel ToGroupViewModel(LocaleViewModel viewModel)
        {
            if (!Equals(viewModel, null))
            {
                return new AttributeGroupLocaleViewModel
                {
                    LocaleId = viewModel.LocaleId,
                    AttributeGroupName = viewModel.Name,
                    CreatedBy = viewModel.CreatedBy,
                    CreatedDate = viewModel.CreatedDate,
                    ModifiedBy = viewModel.ModifiedBy,
                    ModifiedDate = viewModel.ModifiedDate,
                };
            }
            return new AttributeGroupLocaleViewModel();
        }
    }
}