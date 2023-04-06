using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class AddonGroupCache : BaseCache, IAddonGroupCache
    {
        #region Private Variables

        private readonly IAddonGroupService _service;

        #endregion Private Variables

        public AddonGroupCache(IAddonGroupService _addonGroupService)
        {
            _service = _addonGroupService;
        }

        public virtual string GetAddonGroup(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                AddonGroupModel addonGroup = _service.GetAddonGroup(Filters, Expands);
                if (!Equals(addonGroup, null))
                {
                    //Create response.
                    AddonGroupResponse response = new AddonGroupResponse { AddonGroup = addonGroup };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetAddonGroupList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                AddonGroupListModel addonGroups = _service.GetAddonGroupList(Expands, Filters, Sorts, Page);
                if (!Equals(addonGroups, null))
                {
                    //Create response.
                    AddonGroupListResponse response = new AddonGroupListResponse { AddonGroups = addonGroups.AddonGroups };
                    response.MapPagingDataFromModel(addonGroups);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetUnassociatedAddonGroupProducts(int addonGroupId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                ProductDetailsListModel unassociatedProducts = _service.GetUnassociatedAddonGroupProducts(addonGroupId, Expands, Filters, Sorts, Page);
                if (!Equals(unassociatedProducts, null))
                {
                    //Create response.
                    ProductDetailsListResponse response = new ProductDetailsListResponse
                    {
                        ProductDetailList = unassociatedProducts.ProductDetailList,
                        Locale = unassociatedProducts.Locale,
                        AttributeColumnName = unassociatedProducts.AttributeColumnName,
                        XmlDataList = unassociatedProducts.XmlDataList,
                        ProductDetailsDynamic = unassociatedProducts?.ProductDetailListDynamic,
                        NewAttributeList = unassociatedProducts?.NewAttributeList
                    };
                    response.MapPagingDataFromModel(unassociatedProducts);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetAssociatedAddonGroupProducts(int addonGroupId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                ProductDetailsListModel associatedProducts = _service.GetAssociatedProducts(addonGroupId, Expands, Filters, Sorts, Page);
                if (!Equals(associatedProducts, null))
                {
                    //Create response.
                    ProductDetailsListResponse response = new ProductDetailsListResponse
                    {
                        ProductDetailList = associatedProducts.ProductDetailList,
                        Locale = associatedProducts.Locale,
                        AttributeColumnName = associatedProducts.AttributeColumnName,
                        XmlDataList = associatedProducts.XmlDataList,
                        ProductDetailsDynamic = associatedProducts?.ProductDetailListDynamic,
                        NewAttributeList = associatedProducts?.NewAttributeList
                    };
                    response.MapPagingDataFromModel(associatedProducts);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
    }
}