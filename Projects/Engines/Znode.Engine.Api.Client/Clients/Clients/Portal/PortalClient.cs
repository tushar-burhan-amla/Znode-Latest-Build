using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class PortalClient : BaseClient, IPortalClient
    {
        public virtual PortalListModel GetPortalList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Create Endpoint to get the list of all portals.
            string endpoint = PortalEndpoint.GetPortalList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();

            PortalListResponse response = GetResourceFromEndpoint<PortalListResponse>(endpoint, status);

            //Check the status of response of portal list.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PortalListModel list = new PortalListModel { PortalList = response?.PortalList };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        // Get All Sort Settings
        public virtual PortalSortSettingListModel GetPortalSortSettings(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PortalEndpoint.SortList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            SortSettingListResponse response = GetResourceFromEndpoint<SortSettingListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PortalSortSettingListModel list = new PortalSortSettingListModel { SortSettings = response?.SortSettings };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Get All Page Settings
        public virtual PortalPageSettingListModel GetPortalPageSettings(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PortalEndpoint.PageList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PageSettingListResponse response = GetResourceFromEndpoint<PageSettingListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PortalPageSettingListModel list = new PortalPageSettingListModel { PageSettings = response?.PageSettings };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Update portal Page settings
        public virtual bool UpdatePortalPageSetting(PortalPageSettingModel domainViewModel)
        {
            string endpoint = PortalEndpoint.UpdatePortalPageSetting();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(domainViewModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.IsSuccess ?? false;
        }        

        //Remove associated payment settings to portal.
        public virtual bool RemoveAssociatedSortSettings(SortSettingAssociationModel model)
        {
            string endpoint = PortalEndpoint.RemoveAssociatedSortSettings();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.IsSuccess ?? false;
        }

        //Remove associated payment settings to portal.
        public virtual bool RemoveAssociatedPageSettings(PageSettingAssociationModel model)
        {
            string endpoint = PortalEndpoint.RemoveAssociatedPageSettings();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.IsSuccess ?? false;
        }

        //Associate sort settings to portal.
        public virtual bool AssociateSortSettings(SortSettingAssociationModel model)
        {
            string endpoint = PortalEndpoint.AssociateSortSettings();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.IsSuccess ?? false;
        }

        //Associate page settings to portal.
        public virtual bool AssociatePageSettings(PageSettingAssociationModel model)
        {
            string endpoint = PortalEndpoint.AssociatePageSettings();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.IsSuccess ?? false;
        }

        public virtual PortalListModel GetPortalListByCatalogId(int catalogId)
        {
            //Create Endpoint to get the list of all portals.
            string endpoint = PortalEndpoint.GetPortalListByCatalogId(catalogId);
            endpoint += BuildEndpointQueryString(null, null, null, null, null);

            ApiStatus status = new ApiStatus();

            PortalListResponse response = GetResourceFromEndpoint<PortalListResponse>(endpoint, status);

            //Check the status of response of portal list.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PortalListModel list = new PortalListModel { PortalList = response?.PortalList };
            list.MapPagingDataFromResponse(response);
            return list;
        }
#if DEBUG

        public virtual PortalListModel GetDevPortalList()
        {
            //Create Endpoint to get the list of all portals.
            string endpoint = PortalEndpoint.GetDevPortalList();
            endpoint += BuildEndpointQueryString(null, null, null, null, null);

            ApiStatus status = new ApiStatus();

            PortalListResponse response = GetResourceFromEndpoint<PortalListResponse>(endpoint, status);

            //Check the status of response of portal list.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PortalListModel list = new PortalListModel { PortalList = response?.PortalList };
            list.MapPagingDataFromResponse(response);
            return list;
        }
#endif

        public virtual PortalModel GetPortal(int portalId, ExpandCollection expands)
        {
            //Create Endpoint to get the list of all portals.
            string endpoint = PortalEndpoint.GetPortal(portalId);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            PortalResponse response = GetResourceFromEndpoint<PortalResponse>(endpoint, status);

            //Check the status of response of portal list.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Portal;
        }

        //Get portal by store code. 
        public virtual PortalModel GetPortal(string storeCode, ExpandCollection expands)
        {
            //Create Endpoint to get the list of all portals.
            string endpoint = PortalEndpoint.GetPortalByStoreCode(storeCode);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            PortalResponse response = GetResourceFromEndpoint<PortalResponse>(endpoint, status);

            //Check the status of response of portal list.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Portal;
        }

        public virtual PortalModel CreatePortal(PortalModel portalModel)
        {
            //Create Endpoint to create new portal..
            string endpoint = PortalEndpoint.CreatePortal();

            ApiStatus status = new ApiStatus();
            PortalResponse response = PostResourceToEndpoint<PortalResponse>(endpoint, JsonConvert.SerializeObject(portalModel), status);

            //Check the status of response of portal.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.Portal;
        }

        public virtual PortalModel UpdatePortal(PortalModel portalModel)
        {
            //Create Endpoint to create new portal..
            string endpoint = PortalEndpoint.UpdatePortal();

            ApiStatus status = new ApiStatus();
            PortalResponse response = PutResourceToEndpoint<PortalResponse>(endpoint, JsonConvert.SerializeObject(portalModel), status);

            //Check the status of response of portal.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.Portal;
        }

        public virtual bool DeletePortal(ParameterModel portalIds, bool isDeleteByStoreCode)
        {
            //Create Endpoint to create new portal..
            string endpoint = PortalEndpoint.DeletePortal(isDeleteByStoreCode);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(portalIds), status);

            //Check the status of response of portal.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        // Copy a store.
        public virtual bool CopyStore(PortalModel portalModel)
        {
            string endpoint = PortalEndpoint.CopyStore();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(portalModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        public virtual List<PortalFeatureModel> GetPortalFeatureList()
        {
            //Create Endpoint to get the list of all portals.
            string endpoint = PortalEndpoint.GetPortalFeatureList();

            ApiStatus status = new ApiStatus();

            PortalListResponse response = GetResourceFromEndpoint<PortalListResponse>(endpoint, status);

            //Check the status of response of portal list.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PortalFeatureList;
        }
        
        //Check portal code already exist or not.
        public virtual bool IsPortalCodeExist(string portalCode)
        {
            //Get Endpoint.
            string endpoint = PortalEndpoint.IsPortalCodeExist(portalCode);

            //Get response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }
        #region Inventory Warehouse Association

        public virtual PortalWarehouseModel GetAssociatedWarehouseList(int portalId, FilterCollection filters, ExpandCollection expands)
        {
            string endpoint = PortalEndpoint.GetAssociatedWarehouseList(portalId);
            endpoint += BuildEndpointQueryString(expands, filters, null, 0, 0);

            ApiStatus status = new ApiStatus();
            PortalWarehouseResponse response = GetResourceFromEndpoint<PortalWarehouseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PortalWarehouse;
        }

        public virtual bool AssociateWarehouseToStore(PortalWarehouseModel portalWarehouseModel)
        {
            string endpoint = PortalEndpoint.AssociateWarehouseToStore();
            ApiStatus status = new ApiStatus();

            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(portalWarehouseModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }

        #endregion

        #region Portal Locale
        //Gets active Locales.
        public virtual LocaleListModel GetLocaleList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PortalEndpoint.GetLocaleList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            LocaleListResponse response = GetResourceFromEndpoint<LocaleListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            LocaleListModel list = new LocaleListModel { Locales = response?.Locales };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Update Locales.
        public virtual bool UpdateLocale(DefaultGlobalConfigListModel globalConfigurationListModel)
        {
            //Get Endpoint
            string endpoint = PortalEndpoint.UpdateLocale();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(globalConfigurationListModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }
        #endregion

        #region Portal Shipping
        //Get portal shipping data on the basis of portalId.
        public virtual PortalShippingModel GetPortalShippingInformation(int portalId,FilterCollection filters)
        {
            string endpoint = PortalEndpoint.GetPortalShippingInformation(portalId);
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PortalShippingResponse response = GetResourceFromEndpoint<PortalShippingResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PortalShipping;
        }

        //Update portal shipping data.
        public virtual PortalShippingModel UpdatePortalShipping(PortalShippingModel portalShippingModel)
        {
            string endpoint = PortalEndpoint.UpdatePortalShipping();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PortalShippingResponse response = PutResourceToEndpoint<PortalShippingResponse>(endpoint, JsonConvert.SerializeObject(portalShippingModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.PortalShipping;
        }
        #endregion

        #region Portal Tax
        //Get portal tax data on the basis of portalId.
        public virtual TaxPortalModel GetTaxPortalInformation(int portalId, ExpandCollection expands)
        {
            string endpoint = PortalEndpoint.GetTaxPortalInformation(portalId);
            endpoint += BuildEndpointQueryString(expands);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TaxPortalResponse response = GetResourceFromEndpoint<TaxPortalResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.TaxPortal;
        }

        //Update portal tax data.
        public virtual TaxPortalModel UpdateTaxPortal(TaxPortalModel taxPortalModel)
        {
            string endpoint = PortalEndpoint.UpdateTaxPortal();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TaxPortalResponse response = PutResourceToEndpoint<TaxPortalResponse>(endpoint, JsonConvert.SerializeObject(taxPortalModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.TaxPortal;
        }
        #endregion       

        #region Tax
        //Associate/Unassociate taxclass to portal. 
        public virtual bool AssociateAndUnAssociateTaxClass(TaxClassPortalModel taxClassPortalModel)
        {
            string endpoint = PortalEndpoint.AssociateAndUnAssociateTaxClass();
            ApiStatus status = new ApiStatus();

            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(taxClassPortalModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }

        //Set portal default tax.
        public virtual bool SetPortalDefaultTax(TaxClassPortalModel taxClassPortalModel)
        {
            string endpoint = PortalEndpoint.SetPortalDefaultTax();
            ApiStatus status = new ApiStatus();

            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(taxClassPortalModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }
        #endregion

        #region Portal Tracking Pixels
        //Get Tracking Pixels. 
        public virtual PortalTrackingPixelModel GetPortalTrackingPixel(int portalId, ExpandCollection expands)
        {
            string endpoint = PortalEndpoint.GetPortalTrackingPixel(portalId);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            PortalTrackingPixelResponse response = GetResourceFromEndpoint<PortalTrackingPixelResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.PortalTrackingPixel;
        }

        //Save Portal Tracking Pixel.
        public virtual bool SavePortalTrackingPixel(PortalTrackingPixelModel portalTrackingPixel)
        {
            string endpoint = PortalEndpoint.SavePortalTrackingPixel();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(portalTrackingPixel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response.IsSuccess;
        }
        #endregion

        // Get Portal Publish Status.
        public virtual PublishPortalLogListModel GetPortalPublishStatus(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = PortalEndpoint.GetPortalPublishStatus();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            PublishPortalLogListResponse response = GetResourceFromEndpoint<PublishPortalLogListResponse>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.PublishPortalLogs;
        }

        #region Robots.txt
        //Get robots.txt data.
        public virtual RobotsTxtModel GetRobotsTxt(int portalId, ExpandCollection expands)
        {
            string endpoint = PortalEndpoint.GetRobotsTxt(portalId);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            RobotsTxtResponse response = GetResourceFromEndpoint<RobotsTxtResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.RobotsTxt;
        }

        //Save Robots.txt data.
        public virtual bool SaveRobotsTxt(RobotsTxtModel model)
        {
            string endpoint = PortalEndpoint.SaveRobotsTxt();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return (response?.IsSuccess).GetValueOrDefault();
        }
        #endregion

        public virtual PortalApprovalModel GetPortalApproverDetailsById(int portalId)
        {
            string endpoint = PortalEndpoint.GetPortalApproverDetailsById(portalId);
            ApiStatus status = new ApiStatus();
            PortalApprovalResponse response = GetResourceFromEndpoint<PortalApprovalResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.PortalApprovalModel;
        }

        //Delete approval level by id
        public bool DeletePortalApproverUserById(ParameterModel userApproverId)
        {
            string endpoint = PortalEndpoint.DeletePortalApproverUserById();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(userApproverId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Save/Update the Portal Approval details.
        public virtual bool SaveUpdatePortalApprovalDetails(PortalApprovalModel portalApprovalModel)
        {
            string endpoint = PortalEndpoint.SaveUpdatePortalApprovalDetails();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(portalApprovalModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return (response?.IsSuccess).GetValueOrDefault();
        }

        //Get barcode setting details
        public virtual BarcodeReaderModel GetBarcodeScannerDetail()
        {
            string endpoint = PortalEndpoint.GetBarcodeScannerDetail();

            ApiStatus status = new ApiStatus();
            BarcodeReaderModelResponse response = GetResourceFromEndpoint<BarcodeReaderModelResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.BarcodeReader;
        }
    }
}
