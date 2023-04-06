using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class PublishBrandService : BaseService, IPublishBrandService
    {

        #region Private Variables

        #endregion

        #region Constructor
        public PublishBrandService()
        {
        }
        #endregion

        #region public methods
        // Get Published Brand List Associated with store.
        public BrandListModel GetPublishBrandList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started to get Publish Brands.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            int portalId, localId, versionId;
            bool isActive;
            string brandName;
            GetParametersValueForFilters(filters, out portalId, out brandName);

            if (IsNotNull(sorts) && sorts.Count <= 1 && string.IsNullOrEmpty(sorts?.Keys[0]))
                sorts.Add(FilterKeys.DisplayOrder, Constants.SortKeys.Ascending);
                 
            BrandListModel brandListModel = new BrandListModel();
            List<ZnodePublishPortalBrandEntity> portalBrands = null;
            PageListModel pageListModel = new PageListModel(filters, sorts, null);

            try
            {
                if (!string.IsNullOrEmpty(brandName))
                {
                    if (brandName == ZnodeConstant.BrandNameWithNumber)
                    {
                        GetParametersValueForFilters(filters, out localId, out versionId, out isActive);
                        //purpose?
                        List<string> searchTerms = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
                        IZnodeRepository<ZnodePublishPortalBrandEntity> _publishPortalBrandRepository = new ZnodeRepository<ZnodePublishPortalBrandEntity>(HelperMethods.Context);

                        portalBrands = _publishPortalBrandRepository.Table.Where(x =>
                                    searchTerms.Any(y => x.BrandName.StartsWith(y)) && x.LocaleId == localId && x.PortalId == portalId && x.VersionId == versionId && x.IsActive == isActive)?.ToList();

                        brandListModel.Brands = portalBrands?.ToModel<BrandModel>()?.OrderBy(x => Convert.ToInt32(new String(x.BrandName?.TakeWhile(Char.IsDigit).ToArray())))?.ToList();
                    }
                    else
                    {
                        filters.Add(FilterKeys.BrandName, FilterOperators.StartsWith, brandName);
                        portalBrands = GetService<IPublishedPortalDataService>().GetBrandList(pageListModel);
                        brandListModel.Brands = portalBrands?.ToModel<BrandModel>()?.ToList();
                    }
                }
                else
                {
                    portalBrands = GetService<IPublishedPortalDataService>().GetBrandList(pageListModel);
                    brandListModel.Brands = portalBrands?.ToModel<BrandModel>()?.ToList();
                }
                IImageHelper image = GetService<IImageHelper>();
                brandListModel?.Brands?.ForEach(
                    x =>
                    {
                        x.ImageLargePath = image.GetImageHttpPathSmall(x.ImageName);
                        x.ImageSmallPath = image.GetImageHttpPathSmall(x.ImageName);
                        x.ImageMediumPath = image.GetImageHttpPathMedium(x.ImageName);
                        x.ImageThumbNailPath = image.GetImageHttpPathThumbnail(x.ImageName);
                        x.ImageSmallThumbnailPath = image.GetImageHttpPathSmallThumbnail(x.ImageName);
                    });

            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage($"An exception occurred while fetching Publish Brands for portal: {portalId}", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error, ex);
            }

            return brandListModel;
        }

        // Get Published Brand Detail Associated with store.
        public virtual BrandModel GetPublishBrand(int brandId, int localeId, int portalId)
        {
            ZnodeLogging.LogMessage("Execution started to get Publish Brands.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            BrandModel brandModel = null;
            if (HelperUtility.IsNotNull(brandId) && HelperUtility.IsNotNull(localeId))
            {
                ZnodePublishPortalBrandEntity brands = GetService<IPublishedPortalDataService>().GetPublishedBrand(portalId, localeId, brandId);

                brandModel = brands?.ToModel<BrandModel>();
                GetBrandImagePath(portalId, brandModel);
            }

            ZnodeLogging.LogMessage("Execution done for Get Publish Brand.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return brandModel;
        }
        #endregion

        //Get brand image path
        private void GetBrandImagePath(int portalId, BrandModel brand)
        {
            ZnodeLogging.LogMessage("Input parameters portalId:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new object[] { portalId });
            //Get brand image path
            if (HelperUtility.IsNotNull(brand))
            {
                IImageHelper image = GetService<IImageHelper>();
                brand.ImageLargePath = image.GetImageHttpPathLarge(brand.ImageName);
                brand.ImageMediumPath = image.GetImageHttpPathMedium(brand.ImageName);
                brand.ImageThumbNailPath = image.GetImageHttpPathThumbnail(brand.ImageName);
                brand.ImageSmallPath = image.GetImageHttpPathSmall(brand.ImageName);
                brand.OriginalImagepath = image.GetOriginalImagepath(brand.ImageName);
            }
        }

        // Get Parameters Values from Filters.
        protected virtual void GetParametersValueForFilters(FilterCollection filters, out int portalId, out string brandName)
        {
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.PortalId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out portalId);
            brandName = filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.BrandName, StringComparison.InvariantCultureIgnoreCase))?.FilterValue.ToString();
            filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.IsActive, StringComparison.CurrentCultureIgnoreCase));
            filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.BrandName, StringComparison.CurrentCultureIgnoreCase));
            filters.Add(FilterKeys.VersionId, FilterOperators.Equals, WebstoreVersionId.ToString());
            filters.Add(FilterKeys.IsActive, FilterOperators.Equals, ZnodeConstant.TrueValue);
        }

        // Get Parameters Values from Filters for Brand names starts with Numbers.
        protected virtual void GetParametersValueForFilters(FilterCollection filters, out int localId, out int versionId, out bool isActive)
        {
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.LocaleId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out localId);
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.VersionId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out versionId);
            bool.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.IsActive, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out isActive);
        }
    }
}
