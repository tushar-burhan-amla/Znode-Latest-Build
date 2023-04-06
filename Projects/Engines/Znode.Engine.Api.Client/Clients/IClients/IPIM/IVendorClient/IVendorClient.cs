using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IVendorClient : IBaseClient
    {
        /// <summary>
        /// Gets the list of vendor.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with vendor list.</param>
        /// <param name="filters">Filters to be applied on vendor list.</param>
        /// <param name="sorts">Sorting to be applied on vendor list.</param>
        /// <param name="pageIndex">Start page index of vendor list.</param>
        /// <param name="pageSize">Page size of vendor list.</param>
        /// <returns>Vendor list model.</returns>
        VendorListModel GetVendorList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Creates an vendor.
        /// </summary>
        /// <param name="vendorModel">Vendor Model to be created.</param>
        /// <returns>Created vendor model.</returns>
        VendorModel CreateVendor(VendorModel vendorModel);

        /// <summary>
        /// Gets a vendor by PimVendorId.
        /// </summary>
        /// <param name="pimVendorId">Id of the vendor to be retrieved.</param>
        /// <returns>Vendor model.</returns>
        VendorModel GetVendor(int pimVendorId);

        /// <summary>
        /// Updates a Vendor.
        /// </summary>
        /// <param name="vendorModel">Vendor model to be updated.</param>
        /// <returns>Updated Vendor model.</returns>
        VendorModel UpdateVendor(VendorModel vendorModel);

        /// <summary>
        /// Deletes a Vendor by PimVendorId.
        /// </summary>
        /// <param name="parameterModel">VendorId to delete vendor.</param>
        /// <returns>True/False value according the status of delete operation.</returns>
        bool DeleteVendor(ParameterModel parameterModel);

        /// <summary>
        /// Associate/UnAssociate product to vendor.
        /// </summary>
        /// <param name="vendorProductModel">Vendor Product Model</param>
        /// <returns>Returns status as true/false</returns>
        bool AssociateAndUnAssociateProduct(VendorProductModel vendorProductModel);

        /// <summary>
        /// Get vendor code list.
        /// </summary>
        /// <param name="attributeCode">Attbute code.</param>
        /// <returns>Vendor List Model</returns>
        VendorListModel GetVendorCodeList(string attributeCode);

        /// <summary>
        /// Active/Inactive Vendor
        /// </summary>
        /// <param name="vendorIds"></param>
        /// <param name="isActive"></param>
        /// <returns>Return true/false status</returns>
        bool ActiveInactiveVendor(string vendorIds, bool isActive);
    }
}
