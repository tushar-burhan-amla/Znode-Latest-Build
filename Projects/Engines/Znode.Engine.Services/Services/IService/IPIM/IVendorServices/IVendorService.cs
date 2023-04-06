using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IVendorService
    {
        /// <summary>
        /// Create Vendor.
        /// </summary>
        /// <param name="vendorModel">Vendor Model.</param>
        /// <returns>Returns created Vendor Model.</returns>
        VendorModel CreateVendor(VendorModel vendorModel);

        /// <summary>
        /// Get vendor Codes.
        /// </summary>
        /// <param name="attributeCode">Attribute code</param>
        /// <returns>Return list of vendor codes.</returns>
        VendorListModel GetAvailableVendorCodes(string attributeCode);

        /// <summary>
        /// Get Vendor on the basis of PimVendorId.
        /// </summary>
        /// <param name="pimVendorId">PimVendorId.</param>
        /// <returns>Returns Vendor Model.</returns>
        VendorModel GetVendor(int pimVendorId);

        /// <summary>
        /// Update Vendor data.
        /// </summary>
        /// <param name="vendorModel">Vendor model to update.</param>
        /// <returns>Returns true if vendor updated successfully else return false.</returns>
        bool UpdateVendor(VendorModel vendorModel);

        /// <summary>
        /// Delete vendor.
        /// </summary>
        /// <param name="pimVendorIds">Vendor List Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteVendor(ParameterModel pimVendorIds);

        /// <summary>
        /// Get list for Vendor.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Vendor list.</param>
        /// <param name="filters">Filters to be applied on Vendor list.</param>
        /// <param name="sorts">Sorting to be applied on Vendor list.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Returns list of Vendor.</returns>
        VendorListModel GetVendorList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Associate/UnAssociate product to vendor.
        /// </summary>
        /// <param name="vendorProductModel">VendorProductModel</param>
        /// <returns>return status as true/false.</returns>
        bool AssociateAndUnAssociateProduct(VendorProductModel vendorProductModel);

        /// <summary>
        /// Active/Inactive Vendor
        /// </summary>
        /// <param name="model"></param>
        /// <returns>return status as true/false.</returns>
        bool ActiveInactiveVendor(ActiveInactiveVendorModel model);
    }
}
