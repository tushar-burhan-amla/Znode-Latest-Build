using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class VendorClient : BaseClient, IVendorClient
    {
        // Gets the list of Vendor.
        public virtual VendorListModel GetVendorList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = VendorEndpoint.GetVendorList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            VendorListResponse response = GetResourceFromEndpoint<VendorListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            VendorListModel list = new VendorListModel { Vendors = response?.Vendors };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Create vendor.
        public virtual VendorModel CreateVendor(VendorModel vendorModel)
        {
            string endpoint = VendorEndpoint.CreateVendor();

            ApiStatus status = new ApiStatus();
            VendorResponse response = PostResourceToEndpoint<VendorResponse>(endpoint, JsonConvert.SerializeObject(vendorModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Vendor;
        }

        // Gets a vendor by PimVendorId.
        public virtual VendorModel GetVendor(int pimVendorId)
        {
            string endpoint = VendorEndpoint.Get(pimVendorId);

            ApiStatus status = new ApiStatus();
            VendorResponse response = GetResourceFromEndpoint<VendorResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Vendor;
        }

        // Updates a vendor.
        public virtual VendorModel UpdateVendor(VendorModel vendorModel)
        {
            //Get Endpoint
            string endpoint = VendorEndpoint.UpdateVendor();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            VendorResponse response = PutResourceToEndpoint<VendorResponse>(endpoint, JsonConvert.SerializeObject(vendorModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Vendor;
        }

        // Deletes a vendor.
        public virtual bool DeleteVendor(ParameterModel parameterModel)
        {
            //Get Endpoint.
            string endpoint = VendorEndpoint.DeleteVendor();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(parameterModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Active/Inactive Vendor
        public virtual bool ActiveInactiveVendor(string vendorIds, bool isActive)
        {
            string endpoint = VendorEndpoint.ActiveInactiveVendor();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(new ActiveInactiveVendorModel() { VendorId = vendorIds, IsActive = isActive }), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Associate/Unassociate product to brand. 
        public virtual bool AssociateAndUnAssociateProduct(VendorProductModel vendorProductModel)
        {
            string endpoint = VendorEndpoint.AssociateAndUnAssociateProduct();
            ApiStatus status = new ApiStatus();

            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(vendorProductModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }

        //Get vendor code list.
        public virtual VendorListModel GetVendorCodeList(string attributeCode)
        {
            //Get Endpoint.
            string endpoint = VendorEndpoint.VendorCodeList(attributeCode);

            //Get response.
            ApiStatus status = new ApiStatus();
            VendorListResponse response = GetResourceFromEndpoint<VendorListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //vendor code list.
            VendorListModel list = new VendorListModel { VendorCodes = response?.VendorCodes };
            list.MapPagingDataFromResponse(response);

            return list;
        }
    }
}
