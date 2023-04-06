using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IBrandService
    {
        /// <summary>
        /// Create a brand.
        /// </summary>
        /// <param name="brandModel">Brand Model.</param>
        /// <returns>Returns created brand model.</returns>
        BrandModel CreateBrand(BrandModel brandModel);

        /// <summary>
        /// Get brand on the basis of brand id.
        /// </summary>
        /// <param name="brandId">Brand Id.</param>
        ///  /// <param name="localeId">localeId Id.</param>
        /// <returns>Returns Brand Model.</returns>
        BrandModel GetBrand(int brandId, int localeId);

        /// <summary>
        /// Update brand data.
        /// </summary>
        /// <param name="brandModel">Brand model to update.</param>
        /// <returns>Returns true if brand updated successfully else return false.</returns>
        bool UpdateBrand(BrandModel brandModel);

        /// <summary>
        /// Get list for brand.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Brand list.</param>
        /// <param name="filters">Filters to be applied on Brand list.</param>
        /// <param name="sorts">Sorting to be applied on Brand list.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Returns list of brand.</returns>
        BrandListModel GetBrandList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Delete brand.
        /// </summary>
        /// <param name="brandIds">Brand List Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteBrand(ParameterModel brandIds);

        /// <summary>
        /// Get brand Codes.
        /// </summary>
        /// <param name="attributeCode">Attribute code</param>
        /// <returns>Return list of brand codes.</returns>
        BrandListModel GetBrandCodes(string attributeCode);

        /// <summary>
        /// Associate/UnAssociate product to brand.
        /// </summary>
        /// <param name="brandProductModel">BrandProduct Model</param>
        /// <returns>return status as true/false.</returns>
        bool AssociateAndUnAssociateProduct(BrandProductModel brandProductModel);

        /// <summary>
        /// Get available brand code for creating brands. 
        /// </summary>
        /// <param name="attributeCode">Code of attributes</param>
        /// <returns>Return brand list</returns>
        BrandListModel GetAvailableBrandCodes(string attributeCode);

        /// <summary>
        /// Active/Inactive Brands
        /// </summary>
        /// <param name="model"></param>
        /// <returns>return status as true/false.</returns>
        bool ActiveInactiveBrand(ActiveInactiveBrandModel model);

        /// <summary>
        /// Get the list of all portals.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters.</param>
        /// <returns>PortalBrandListModel Model.</returns>
        PortalBrandListModel GetBrandPortalList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Associate/UnAssociate product to brand.
        /// </summary>
        /// <param name="brandPortalModel">Brand Portal Model</param>
        /// <returns>return status as true/false.</returns>
        bool AssociateAndUnAssociatePortal(BrandPortalModel brandPortalModel);

        /// <summary>
        /// Check if brand already exists or not
        /// </summary>
        /// <param name="code">Brand Code</param>
        /// <returns>Return bool</returns>
        bool CheckBrandCode(string code);

        /// <summary>
        /// Associate And UnAssociate Portal Brands
        /// </summary>
        /// <param name="associatePortalBrandModel"> brand ids with portal to Associate and UnAssociate</param>
        /// <returns>true/false</returns>
        bool AssociateAndUnAssociatePortalBrands(PortalBrandAssociationModel portalBrandAssociationModel);

        /// <summary>
        /// Get list of associated and unassociated brands with store.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Brand list.</param>
        /// <param name="filters">Filters to be applied on Brand list.</param>
        /// <param name="sorts">Sorting to be applied on Brand list.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Returns list of brand.</returns>
        BrandListModel GetPortalBrandList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Update Associated brand data with portal.
        /// </summary>
        /// <param name="portalBrandDetailModel">Associated Brand data</param>
        /// <returns>true/false</returns>
        bool UpdateAssociatedPortalBrandDetail(PortalBrandDetailModel portalBrandDetailModel);
    }
}
