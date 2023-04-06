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
    public class BrandClient : BaseClient, IBrandClient
    {
        #region Public Methods

        // Gets the list of Brands.
        public virtual BrandListModel GetBrandList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = BrandEndpoint.GetBrandList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            BrandListResponse response = GetResourceFromEndpoint<BrandListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            BrandListModel list = new BrandListModel { Brands = response?.Brands };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Get a brand by brand ID.
        public virtual BrandModel GetBrand(int brandId, int localeId)
        {
            string endpoint = BrandEndpoint.Get(brandId, localeId);

            ApiStatus status = new ApiStatus();
            BrandResponse response = GetResourceFromEndpoint<BrandResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Brand;
        }

        // Creates a brand.
        public virtual BrandModel CreateBrand(BrandModel brandModel)
        {
            //Get Endpoint.
            string endpoint = BrandEndpoint.Create();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            BrandResponse response = PostResourceToEndpoint<BrandResponse>(endpoint, JsonConvert.SerializeObject(brandModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Brand;
        }

        // Updates a brand.
        public virtual BrandModel UpdateBrand(BrandModel brandModel)
        {
            //Get Endpoint
            string endpoint = BrandEndpoint.Update();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            BrandResponse response = PutResourceToEndpoint<BrandResponse>(endpoint, JsonConvert.SerializeObject(brandModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Brand;
        }

        // Deletes a brand.
        public virtual bool DeleteBrand(ParameterModel parameterModel)
        {
            //Get Endpoint.
            string endpoint = BrandEndpoint.Delete();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(parameterModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get brand code list.
        public virtual BrandListModel GetBrandCodeList(string attributeCode)
        {
            //Get Endpoint.
            string endpoint = BrandEndpoint.BrandCodeList(attributeCode);

            //Get response.
            ApiStatus status = new ApiStatus();
            BrandListResponse response = GetResourceFromEndpoint<BrandListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //brand code list.
            BrandListModel list = new BrandListModel { BrandCodes = response?.BrandCodes };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Associate/Unassociate product to brand. 
        public virtual bool AssociateAndUnAssociateProduct(BrandProductModel brandProductModel)
        {
            string endpoint = BrandEndpoint.AssociateAndUnAssociateProduct();
            ApiStatus status = new ApiStatus();

            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(brandProductModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }

        //Active/Inactive Brands
        public virtual bool ActiveInactiveBrand(string brandIds, bool isActive)
        {
            string endpoint = BrandEndpoint.ActiveInactiveBrand();
            ApiStatus status = new ApiStatus();

            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(new ActiveInactiveBrandModel() { BrandId = brandIds, IsActive = isActive }), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get portal List associated with Brand.
        public virtual PortalBrandListModel GetBrandPortalList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Create Endpoint to get the list of all portals.
            string endpoint = BrandEndpoint.GetPortalList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();

            PortalBrandListResponse response = GetResourceFromEndpoint<PortalBrandListResponse>(endpoint, status);

            //Check the status of response of portal list.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PortalBrandListModel list = new PortalBrandListModel { PortalBrandModel = response?.PortalBrand };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Associate/Unassociate portal to brand. 
        public virtual bool AssociateAndUnAssociatePortal(BrandPortalModel brandPortalModel)
        {
            string endpoint = BrandEndpoint.AssociateAndUnAssociatePortal();
            ApiStatus status = new ApiStatus();

            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(brandPortalModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }

        //Check Unique BrandCode. 
        public virtual bool CheckUniqueBrandCode(string code)
        {
            string endpoint = BrandEndpoint.CheckUniqueBrandCode(code);
            ApiStatus status = new ApiStatus();

            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint,status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }

        //Associate /UnAssociate Portal Brands
        public virtual bool AssociateAndUnAssociatePortalBrand(PortalBrandAssociationModel portalBrandAssociationModel)
        {
            string endpoint = BrandEndpoint.AssociateAndUnAssociatePortalBrand();
            ApiStatus status = new ApiStatus();

            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(portalBrandAssociationModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }

        // Get list of Brands Associate And UnAssociate the store.
        public virtual BrandListModel GetPortalBrandList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = BrandEndpoint.GetPortalBrandList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            BrandListResponse response = GetResourceFromEndpoint<BrandListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            BrandListModel list = new BrandListModel { Brands = response?.Brands };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Update Associate brand data for Store.
        public virtual PortalBrandDetailModel UpdateAssociatedPortalBrandDetail(PortalBrandDetailModel portalBrandDetailModel)
        {
            string endpoint = BrandEndpoint.UpdateAssociatedPortalBrandDetail();

            ApiStatus status = new ApiStatus();
            PortalBrandDetailResponse response = PutResourceToEndpoint<PortalBrandDetailResponse>(endpoint, JsonConvert.SerializeObject(portalBrandDetailModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.PortalBrandDetailModel;
        }
        #endregion
    }
}
