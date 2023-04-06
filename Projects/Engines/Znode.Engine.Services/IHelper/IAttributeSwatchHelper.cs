using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Services
{
    public interface IAttributeSwatchHelper
    {
        void GetAssociatedConfigurableProducts(KeywordSearchModel searchModel, SearchRequestModel model, string attributeCode);
        string GetAttributeSwatchCodeExpands(NameValueCollection expands);
        List<AttributesSelectValuesModel> GetSelectValues(string attributeCode, SearchProductModel searchProduct);
        List<WebStoreAttributeValueSwatchModel> GetSwatchImages(List<AttributesSelectValuesModel> swatches, IImageHelper image);
        bool IsConfigurableProduct(SearchProductModel searchProduct, string productType);
    }
}