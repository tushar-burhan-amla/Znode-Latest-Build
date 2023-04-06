using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Maps
{
    public class ProductViewModelMap
    {
        public static PIMAttributeValueListModel ToAttributeValueListModel(List<string> attributeIds, int pimProductId)
        {
            PIMAttributeValueListModel listModel = new PIMAttributeValueListModel() { AttributeValues = new List<PIMAttributeValueModel>() };
            foreach (string item in attributeIds)
            {
                listModel.AttributeValues.Add(new PIMAttributeValueModel()
                {
                    PimProductId = pimProductId,
                    PimAttributeId = Convert.ToInt32(item),
                });
            }
            return listModel;
        }

        //Get Values From ProductViewModel into ProductModel
        public static ProductModel ToDataModel(ProductViewModel model)
        {
            if (!Equals(model, null))
            {
                return new ProductModel()
                {
                    ProductFamily = model.ProductFamily,
                    ProductType = model.ProductType,
                    ProductId = model.ProductId,
                    LocaleId = model.LocaleId,
                    AssociatedProducts = model.AssociatedProducts,
                    ProductAttributeList = model.ProductAttributeList?.ToModel<ProductAttributeModel>().ToList(),
                    PimCatalogId = model.PimCatalogId,
                    PimCategoryId = model.PimCategoryId,
                    PimCategoryHierarchyId = model.PimCategoryHierarchyId,
                    ConfigureAttributeIds = model.ConfigureAttributeIds,
                    CopyProductId = model.CopyProductId
                };
            }
            return null;
        }

        
    }
}