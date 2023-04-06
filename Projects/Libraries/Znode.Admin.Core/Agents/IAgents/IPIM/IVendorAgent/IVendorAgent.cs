using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IVendorAgent
    {
        /// <summary>
        /// Gets the list of Vendor.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with vendor list.</param>
        /// <param name="filters">Filters to be applied on vendor list.</param>
        /// <param name="sorts">Sorting to be applied on vendor list.</param>
        /// <param name="pageIndex">Start page index of vendor list.</param>
        /// <param name="pageSize">Page size of vendor list.</param>
        /// <returns>Vendor list model.</returns>
        VendorListViewModel GetVendorList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create new vendor.
        /// </summary>
        /// <param name="vendorViewModel">Vendor view model.</param>
        /// <returns>Returns newly created vendor view model</returns>
        VendorViewModel CreateVendor(VendorViewModel vendorViewModel);

        /// <summary>
        /// Get vendor by PimVendorId.
        /// </summary>
        /// <param name="PimVendorId">Id to get vendor.</param>
        /// <returns>Vendor View Model.</returns>
        VendorViewModel GetVendor(int PimVendorId);

        /// <summary>
        /// Update the Vendor Information.
        /// </summary>
        /// <param name="vendorViewModel">Vendor model.</param>
        /// <returns>Return the Updated Vendor Model.</returns>
        VendorViewModel UpdateVendor(VendorViewModel vendorViewModel);

        /// <summary>
        /// Deletes an existing vendor.
        /// </summary>
        /// <param name="pimVendorId">Ids for the Selected Vendor.</param>
        /// <param name="message">error message</param>
        /// <returns>true / false</returns>
        bool DeleteVendor(string pimVendorId, out string message);

        /// <summary>
        ///  Get the list of associated products.
        /// </summary>
        /// <param name="model">FilterCollectionData model</param>
        /// /// <param name="pimVendorId">vendor code</param>
        /// <param name="vendorCode">vendor code</param>
        /// <returns>Vendor List View Model.</returns>
        VendorListViewModel AssociatedProductList(FilterCollectionDataModel model, int pimVendorId, string vendorCode, string vendorName);

        /// <summary>
        ///  Get the list of unassociated products.
        /// </summary>
        /// <param name="model">FilterCollectionData model</param>
        /// <param name="vendorCode">vendor code</param>
        /// <returns>Vendor List ViewModel.</returns>
        VendorListViewModel UnAssociatedProductList(FilterCollectionDataModel model, string vendorCode);

        /// <summary>
        /// Associate vendor to product.
        /// </summary>
        /// <param name="vendorCode">code of vendor</param>
        /// <param name="productId">productIds</param>
        /// <returns>return status as true/false.</returns>
        bool AssociateVendorProduct(string vendorCode, string productId);

        /// <summary>
        /// Associate vendor to product.
        /// </summary>
        /// <param name="vendorCode">code of vendor</param>
        /// <param name="productId">productId</param>
        /// <returns>return status as true/false.</returns>
        bool UnAssociateVendorProduct(string empty, string productId);

        /// <summary>
        /// Get Vendor list.
        /// </summary>
        /// <returns>Return list of vendor code.</returns>
        List<SelectListItem> GetVendorCodeList();

        /// <summary>
        /// Get country list
        /// </summary>
        /// <returns>Returns country list</returns>
        List<SelectListItem> GetCountries();

        /// <summary>
        /// Check vendor name exist or not.
        /// </summary>
        /// <param name="vendorName"></param>
        /// <param name="vendorId"></param>
        /// <returns>Returns true/false status</returns>
        bool CheckVendorNameExist(string vendorName, int vendorId);

        /// <summary>
        /// Active/Inactive Vendor
        /// </summary>
        /// <param name="vendorIds"></param>
        /// <param name="isActive"></param>
        /// <returns>Return true/false status</returns>
        bool ActiveInactiveVendor(string vendorIds, bool isActive);
    }
}