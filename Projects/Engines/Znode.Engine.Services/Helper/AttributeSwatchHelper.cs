using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Services.Constants;
using Znode.Libraries.ECommerce.Utilities;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public  class AttributeSwatchHelper : IAttributeSwatchHelper
    {
        public virtual string GetAttributeSwatchCodeExpands(NameValueCollection expands)
        {
            string swatchAttributeCode = "";
            foreach (string key in expands.Keys)
            {
                string value = expands.Get(key);
                if (value.StartsWith(ExpandKeys.Swatch_AttributeCode))
                {
                    //Here we will get the value in swatch_yourattributecode or value we will split it with the _ and get the value.
                    swatchAttributeCode = value.Split('_')[1];
                    return swatchAttributeCode;
                }
                return swatchAttributeCode;
            }
            return swatchAttributeCode;
        }
        public virtual void GetAssociatedConfigurableProducts(KeywordSearchModel searchModel, SearchRequestModel model, string attributeCode)
        {

            IImageHelper imageHelper = GetService<IImageHelper>();

            foreach (SearchProductModel searchProduct in searchModel.Products)
            {
                string productType = ZnodeConstant.ConfigurableProduct;
                if (IsConfigurableProduct(searchProduct, productType))
                {
                    List<AttributesSelectValuesModel> selectValues = GetSelectValues(attributeCode, searchProduct);
                    if (selectValues?.Count > 0)
                    {
                        searchProduct.SwatchAttributesValues = GetSwatchImages(selectValues, imageHelper);
                        searchProduct.ImageSmallPath = imageHelper.GetImageHttpPathSmall(selectValues.First().VariantImagePath);
                    }
                    else
                        searchProduct.ImageSmallPath = imageHelper.GetImageHttpPathSmall(searchProduct.Attributes.Where(y => y.AttributeCode == ZnodeConstant.ProductImage)?.FirstOrDefault()?.AttributeValues);
                }
                else
                    searchProduct.ImageSmallPath = imageHelper.GetImageHttpPathSmall(searchProduct.Attributes.Where(y => y.AttributeCode == ZnodeConstant.ProductImage)?.FirstOrDefault()?.AttributeValues);
            }
        }

        public virtual List<AttributesSelectValuesModel> GetSelectValues(string attributeCode, SearchProductModel searchProduct)
        {
            return searchProduct.Attributes.FirstOrDefault(x => x.AttributeCode.Equals(attributeCode, StringComparison.OrdinalIgnoreCase))?.SelectValues.OrderBy(x => x.VariantDisplayOrder).DistinctBy(z => z.Code).ToList();
        }

        public virtual bool IsConfigurableProduct(SearchProductModel searchProduct, string productType)
        {
            return searchProduct.Attributes.FirstOrDefault(x => x.AttributeCode == ZnodeConstant.ProductType)?.SelectValues.FirstOrDefault().Code == productType;
        }

        public virtual List<WebStoreAttributeValueSwatchModel> GetSwatchImages(List<AttributesSelectValuesModel> swatches, IImageHelper image)
        {
            List<WebStoreAttributeValueSwatchModel> list = new List<WebStoreAttributeValueSwatchModel>();
            foreach (AttributesSelectValuesModel item in swatches)
            {
                WebStoreAttributeValueSwatchModel model = new WebStoreAttributeValueSwatchModel
                {
                    Code = item.Code,
                    Value = item.Value,
                    ImageSmallThumbnailPath = image.GetImageHttpPathSmallThumbnail(item.Path),
                    ImageSmallPath = image.GetImageHttpPathSmall(item.VariantImagePath)
                };
                list.Add(model);
            }
            return list;
        }
    }
}
