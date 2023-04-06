using System;
using System.Collections.Generic;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Maps
{
    public static class EcommerceCatalogMap
    {
        //Map PublishCatalogModel to PublishDetailsViewModel
        public static PublishDetailsViewModel ToViewModel(PublishCatalogModel model)
        {
            if (!Equals(model, null))
            {
                PublishDetailsViewModel publishDetailsViewModel = new PublishDetailsViewModel();

                publishDetailsViewModel.Attributes.Add(
                    new Property
                    {
                        ControlType = ControlTypes.Label.ToString(),
                        ControlLabel = Admin_Resources.LabelCatalogId,
                        Value = Convert.ToString(model.PublishCatalogId)
                    });

                publishDetailsViewModel.Attributes.Add(
                    new Property
                    {
                        ControlType = ControlTypes.Label.ToString(),
                        ControlLabel = Admin_Resources.LabelCatalogName,
                        Value = model.CatalogName
                    });
                return publishDetailsViewModel;
            }
            else
                return new PublishDetailsViewModel();
        }


        //Maps PublishCategoryModel to PublishDetailsViewModel
        public static PublishDetailsViewModel ToViewModel(PublishCategoryModel publishCategoryModel)
           => MapsAttributesToModel(publishCategoryModel?.Attributes);


        //Maps PublishProductModel to PublishDetailsViewModel
        public static PublishDetailsViewModel ToViewModel(PublishProductModel publishProductModel)
         => MapsAttributesToModel(publishProductModel?.Attributes);


        //Maps List<Dictionary<string, object>> to PublishDetailsViewModel
        private static PublishDetailsViewModel MapsAttributesToModel(List<PublishAttributeModel> attributes)
        {
            if (attributes?.Count > 0)
            {

                PublishDetailsViewModel publishDetailsViewModel = new PublishDetailsViewModel();

                //Need to change implementation of attributes list which will remove index used below
                foreach (PublishAttributeModel attribute in attributes)
                {
                    string[] galleryImages = null;
                    string attributeValue = attribute.AttributeValues;

                    bool IsImageType = (String.Equals(attribute.AttributeTypeName, ControlTypes.Image.ToString(), StringComparison.OrdinalIgnoreCase));

                    if (IsImageType && IsNotNull(attributeValue)  && attributeValue.Contains(","))
                        galleryImages = attributeValue.Split(',');

                    if (!Equals(galleryImages, null) && galleryImages.Length > 1)
                    {
                        for (int i = 0; i < galleryImages.Length; i++)
                        {
                            publishDetailsViewModel.Attributes.Add(
                               new Property
                               {
                                   ControlType = ControlTypes.Image.ToString(),
                                   ControlLabel = i == 0 ? attribute.AttributeName : string.Empty,
                                   Value = string.IsNullOrEmpty(galleryImages[i]) ? ZnodeAdminSettings.DefaultImagePath : $"{HelperMethods.GetThumbnailImagePath()}{galleryImages[i]}"
                               });
                        }
                    }
                    else
                    {
                        publishDetailsViewModel.Attributes.Add(
                        new Property
                        {
                            ControlType = IsImageType ? ControlTypes.Image.ToString() : ControlTypes.Label.ToString(),
                            ControlLabel = attribute.AttributeName,
                            Value = IsImageType ? string.IsNullOrEmpty(attribute.AttributeValues) ? ZnodeAdminSettings.DefaultImagePath : $"{HelperMethods.GetThumbnailImagePath()}{attribute.AttributeValues}" : attribute.AttributeValues
                        });
                    }
                }

                return publishDetailsViewModel;
            }
            else
                return new PublishDetailsViewModel();
        }
    }
}