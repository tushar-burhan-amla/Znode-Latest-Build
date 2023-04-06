using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IBrandClient : IBaseClient
    {
        /// <summary>
        /// Gets the list of brands.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with brand list.</param>
        /// <param name="filters">Filters to be applied on brand list.</param>
        /// <param name="sorts">Sorting to be applied on brand list.</param>
        /// <param name="pageIndex">Start page index of brand list.</param>
        /// <param name="pageSize">Page size of brand list.</param>
        /// <returns>Brand list model.</returns>
        BrandListModel GetBrandList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets a brand by brand Id.
        /// </summary>
        /// <param name="brandId">Id of the brand to be retrieved.</param>
        /// <param name="localeId">Id of the locale.</param>
        /// <returns>Brand model.</returns>
        BrandModel GetBrand(int brandId, int localeId);

        /// <summary>
        /// Creates a brand.
        /// </summary>
        /// <param name="brandModel">Brand model to be created.</param>
        /// <returns>Newly created Brand model.</returns>
        BrandModel CreateBrand(BrandModel brandModel);

        /// <summary>
        /// Updates a Brand.
        /// </summary>
        /// <param name="brandModel">Brand model to be updated.</param>
        /// <returns>Updated Brand model.</returns>
        BrandModel UpdateBrand(BrandModel brandModel);


        /// <summary>
        /// Deletes a Brand by brand ID.
        /// </summary>
        /// <param name="parameterModel">Brand IDs to delete brand.</param>
        /// <returns>True/False value according the status of delete operation.</returns>
        bool DeleteBrand(ParameterModel parameterModel);

        /// <summary>
        /// Get brand code list.
        /// </summary>
        /// <param name="attributeCode">Attbute code.</param>
        /// <returns>Brand List Model</returns>
        BrandListModel GetBrandCodeList(string attributeCode);

        /// <summary>
        /// Associate and UnAssociate product to brand.
        /// </summary>
        /// <param name="brandProductModel">Brand Product Model</param>
        /// <returns>Returns status as true/false</returns>
        bool AssociateAndUnAssociateProduct(BrandProductModel brandProductModel);

        /// <summary>
        /// Active/Inactive Brands
        /// </summary>
        /// <param name="brandIds"></param>
        /// <param name="isActive"></param>
        /// <returns>return status true/false</returns>
        bool ActiveInactiveBrand(string brandIds, bool isActive);

        /// <summary>
        /// Get the list of all portals.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>PortalBrandListModel.</returns>
        PortalBrandListModel GetBrandPortalList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Associate and UnAssociate portal to brand.
        /// </summary>
        /// <param name="brandPortalModel">Brand portal Model</param>
        /// <returns>Returns status as true/false</returns>
        bool AssociateAndUnAssociatePortal(BrandPortalModel brandPortalModel);

        /// <summary>
        /// Check Unique BrandCode
        /// </summary>
        /// <param name="code">BrandCode</param>
        /// <returns>True or false</returns>
        bool CheckUniqueBrandCode(string code);

        /// <summary>
        /// Associate And Unassociate Portal Brands
        /// </summary>
        /// <param name="associatePortalBrandModel"> BrandIds with portal to Associate and UnAssociate</param>
        /// <returns>true/false</returns>
        bool AssociateAndUnAssociatePortalBrand(PortalBrandAssociationModel portalBrandAssociationModel);

        /// <summary>
        /// Get list of portal brands of Associate / UnAssociate with Portal
        /// </summary>
        /// <param name="expands"></param>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns>BrandListModel</returns>
        BrandListModel GetPortalBrandList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Update associate brand data with store.
        /// </summary>
        /// <param name="portalBrandDetailModel">Associated Brand data</param>
        /// <returns>true/false</returns>
        PortalBrandDetailModel UpdateAssociatedPortalBrandDetail(PortalBrandDetailModel portalBrandDetailModel);
    }
}
