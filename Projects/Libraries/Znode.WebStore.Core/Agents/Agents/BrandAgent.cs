using System.Collections.Generic;
using System.Linq;

using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.Agents
{
    public class BrandAgent : BaseAgent, IBrandAgent
    {
        #region Private Variables
        private readonly IPublishBrandClient _publishBrandClient;
        #endregion

        #region Constructor
        public BrandAgent(IPublishBrandClient publishBrandClient)
        {
            _publishBrandClient = GetClient<IPublishBrandClient>(publishBrandClient);
        }
        #endregion

        //Get Filtered Brand List By Brandname
        public virtual List<BrandViewModel> BrandList(string brandName = null)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add("BrandName", FilterOperators.StartsWith, brandName);
            filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, PortalAgent.LocaleId.ToString());
            filters.Add(FilterKeys.PortalId.ToString(), FilterOperators.Equals, PortalAgent.CurrentPortal?.PortalId.ToString());
            filters.Add(ZnodeBrandDetailEnum.IsActive.ToString(), FilterOperators.Is, ZnodeConstant.TrueValue);

            SortCollection sorts = new SortCollection();
            sorts.Add(SortKeys.DisplayOrder, Helpers.DynamicGridConstants.ASCKey);

            BrandListModel brandList = _publishBrandClient.GetPublishBrandList(null, filters, sorts, null, null);
          
            return brandList?.Brands?.Count > 0 ? brandList?.Brands.ToViewModel<BrandViewModel>().ToList() : new List<BrandViewModel> { };
        }

        //Get brand details by id.
        public virtual BrandViewModel GetBrandDetails(int brandId)
        {
            BrandModel brand = _publishBrandClient.GetPublishBrand(brandId, PortalAgent.LocaleId, PortalAgent.CurrentPortal.PortalId);
            if (HelperUtility.IsNull(brand))
                throw new ZnodeException(ErrorCodes.NotFound, WebStore_Resources.ErrorBrandNotFound);
            return brand.ToViewModel<BrandViewModel>();
        }

        //Set breadcrumb html.
        public virtual void SetBreadCrumbHtml(BrandViewModel brand)=>
            brand.BreadCrumbHtml = $"<a href='/'>Home</a> / <a href='/Brand/List'>{WebStore_Resources.TitleShopByBrand}</a> / {brand.BrandName}";
    }
}