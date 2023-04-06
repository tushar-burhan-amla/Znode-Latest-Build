using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IBrandAgent
    {
        /// <summary>
        /// Gets the list of Brands.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with brand list.</param>
        /// <param name="filters">Filters to be applied on brand list.</param>
        /// <param name="sorts">Sorting to be applied on brand list.</param>
        /// <param name="pageIndex">Start page index of brand list.</param>
        /// <param name="pageSize">Page size of brand list.</param>
        /// <returns>Brand list model.</returns>
        BrandListViewModel GetBrandList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create new brand.
        /// </summary>
        /// <param name="brandViewModel">Brand view model.</param>
        /// <returns>Returns newly created brand view model</returns>
        BrandViewModel CreateBrand(BrandViewModel brandViewModel);

        /// <summary>
        /// Get brand by brand Id.
        /// </summary>
        /// <param name="brandId">Id to get brand.</param>
        /// <returns>Brand View Model.</returns>
        BrandViewModel GetBrand(int brandId, int localeId);

        /// <summary>
        /// Update the Brand Information.
        /// </summary>
        /// <param name="brandViewModel">Brand model.</param>
        /// <returns>Return the Updated Brand Model.</returns>
        BrandViewModel UpdateBrand(BrandViewModel brandViewModel);

        /// <summary>
        /// Deletes an existing brand.
        /// </summary>
        /// <param name="brandId">Ids for the Selected Brand.</param>
        /// <param name="message">error message</param>
        /// <returns>true / false</returns>
        bool DeleteBrand(string brandId, out string message);

        /// <summary>
        /// Check whether the brand name is already exists.
        /// </summary>
        /// <param name="brandName">Name of the brand</param>
        /// <param name="brandId">Id of brand</param>
        /// <returns>return the status in true or false<</returns>
        bool CheckBrandNameExist(string brandName, int brandId);

        ///  <summary>
        ///  Get the list of associated products.
        ///  </summary>
        ///  <param name="model">FilterCollectionData model</param>
        ///  <param name="brandId">Brand ID</param>
        ///  <param name="brandCode">brand code</param>
        ///  <param name="localeId">Locale ID of the Brand.</param>
        ///  <param name="brandName">brand name</param>
        ///  <returns>Brand List View Model.</returns>
        BrandListViewModel AssociatedProductList(FilterCollectionDataModel model, int brandId, string brandCode, int localeId, string brandName);

        /// <summary>
        ///  Get the list of unassociated products.
        /// </summary>
        /// <param name="model">FilterCollectionData model</param>
        /// <param name="brandCode">brand code</param>
        /// <param name="localeId">Locale ID of the Brand</param>
        /// <returns>Brand List ViewModel.</returns>
        BrandListViewModel UnAssociatedProductList(FilterCollectionDataModel model, string brandCode,int localeId);

        /// <summary>
        /// Associate brand to product.
        /// </summary>
        /// <param name="brandCode">code of brand</param>
        /// <param name="productIds">productIds</param>
        /// <returns>return status as true/false.</returns>
        bool AssociateBrandProduct(string brandCode, string productIds);

        /// <summary>
        /// UnAssociate brand to product.
        /// </summary>
        /// <param name="brandCode">code of brand</param>
        /// <param name="productIds">productIds</param>
        /// <returns>return status as true/false.</returns>
        bool UnAssociateBrandProduct(string brandCode, string productIds);

        /// <summary>
        /// Get brand view model.
        /// </summary>
        /// <returns>Return BrandViewModel.</returns>
        BrandViewModel GetBrandViewModel();

        /// <summary>
        /// Active/Inactive Brands
        /// </summary>
        /// <param name="brandIds"></param>
        /// <param name="isActive"></param>
        /// <returns>return status true/false</returns>
        bool ActiveInactiveBrand(string brandIds, bool isActive);

        /// <summary>
        ///  Check brand SEO friendly page name URL.
        /// </summary>
        /// <param name="seoFriendlyPageName">seoFriendlyPageName</param>
        /// <param name="seoDetailId">seoDetailId</param>
        /// <returns>Returns boolean value.</returns>
        bool CheckBrandSEOFriendlyPageNameExist(string seoFriendlyPageName, int seoDetailId);

        ///  <summary>
        ///  Get the list of associated products.
        ///  </summary>
        ///  <param name="model">FilterCollectionData model</param>
        ///  <param name="brandId">Brand ID</param>
        ///  <param name="brandCode">brand code</param>
        ///  <param name="localeId">Locale ID of the Brand.</param>
        ///  <param name="brandName">brand name</param>
        ///  <returns>Brand List View Model.</returns>
        BrandListViewModel AssociatedStoreList(FilterCollectionDataModel model, int brandId, string brandCode, int localeId, string brandName);

        /// <summary>
        ///  Get the list of unassociated products.
        /// </summary>
        /// <param name="model">FilterCollectionData model</param>
        /// <param name="brandCode">brand code</param>
        /// <param name="localeId">Locale ID of the Brand</param>
        /// <returns>Brand List ViewModel.</returns>
        BrandListViewModel UnAssociatedStoreList(FilterCollectionDataModel model, string brandCode, int localeId);

        /// <summary>
        ///  un associate portals.
        /// </summary>
        /// <param name="brandId">brand Id</param>
        /// <param name="portalIds">portal Ids</param>
        /// <returns>Returns boolean value.</returns>
        bool UnAssociateBrandPortal(int brandId, string portalIds);

        /// <summary>
        ///  associate portals.
        /// </summary>
        /// <param name="brandId">brand Id</param>
        /// <param name="portalIds">portal Ids</param>
        /// <returns>Returns boolean value.</returns>
        bool AssociateBrandPortal(int brandId, string portalIds);

        /// <summary>
        /// Check Unique BrandCode
        /// </summary>
        /// <param name="code">BrandCode</param>
        /// <returns>true or false</returns>
        bool CheckUniqueBrandCode(string code);

        /// <summary>
        /// Get Brand Name  
        /// </summary>
        /// <param name="code">Brand Code</param>
        /// <param name="localeId">locale id</param>
        /// <returns>brand name</returns>
        string GetBrandName(string code, int localeId);

        /// <summary>
        /// Associate/UnAssociate portal Brands.
        /// </summary>
        /// <param name="brandIds">brand Ids</param>
        /// <param name="portalId">portal Id</param>
        /// <param name="isAssociated"></param>
        /// <param name="message">error msg string</param>
        /// <returns>true/false.</returns>
        bool AssociateAndUnAssociatePortalBrand(string brandIds, int portalId, bool isAssociated, out string message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="portalId"></param>
        /// <param name="isAssociated"></param>
        /// <returns></returns>
        BrandListViewModel GetPortalBrandList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int portalId, bool isAssociated);

        /// <summary>
        /// Update associated brand data.
        /// </summary>
        /// <param name="data"> brand data</param>
        /// <returns>true/false</returns>
        bool UpdateAssociatedPortalBrandDetail(string data);

    }
}
