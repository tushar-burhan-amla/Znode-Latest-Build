using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class PortalController : BaseController
    {
        #region Private Variables
        private readonly IPortalCache _portalCache;
        private readonly IPortalService _portalService;
        #endregion

        #region Constructor
        public PortalController(IPortalService service)
        {
            _portalService = service;
            _portalCache = new PortalCache(_portalService);
        }
        #endregion

        /// <summary>
        /// Get the list of all Portals.
        /// </summary>
        /// <returns>Response with All portals List</returns>
        [ResponseType(typeof(PortalListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPortalList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _portalCache.GetPortalList(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<PortalListResponse>(data);
            }
            catch (Exception ex)
            {
                PortalListResponse data = new PortalListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get all portals on Catalog Id.
        /// </summary>
        /// <returns>Response with All portals List</returns>
        [ResponseType(typeof(PortalListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPortalListByCatalogId(int CatalogId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _portalCache.GetPortalListByCatalogId(CatalogId, RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<PortalListResponse>(data);
            }
            catch (Exception ex)
            {
                PortalListResponse data = new PortalListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }


#if DEBUG

        /// <summary>
        /// Get the list of all Portals.
        /// </summary>
        /// <returns>Response with All portals List</returns> 
        [ResponseType(typeof(PortalListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetDevPortalList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _portalCache.GetDevPortalList(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<PortalListResponse>(data);
            }
            catch (Exception ex)
            {
                PortalListResponse data = new PortalListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }
#endif

        /// <summary>
        /// Get the portal details by portal Id.
        /// </summary>
        /// <param name="portalId">Id of portal to get portal details.</param>
        /// <returns>HttpResponseMessage.</returns>
        [ResponseType(typeof(PortalResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPortal(int portalId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _portalCache.GetPortal(portalId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PortalResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                PortalResponse portalResponse = new PortalResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(portalResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get the portal details by Store Code.
        /// </summary>
        /// <param name="storeCode">storeCode to get portal details</param>
        /// <returns>HttpResponseMessage.</returns>
        [ResponseType(typeof(PortalResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPortalByStoreCode(string storeCode)
        {
            HttpResponseMessage response;
            try
            {
                string data = _portalCache.GetPortal(storeCode, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PortalResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                PortalResponse portalResponse = new PortalResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(portalResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }
        /// <summary>
        /// Create new Portal.
        /// </summary>
        /// <param name="portalModel">Portal model.</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(PortalResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage CreatePortal([FromBody] PortalModel portalModel)
        {
            HttpResponseMessage response;
            try
            {
                PortalModel portal = _portalService.CreatePortal(portalModel);
                if (!Equals(portal, null))
                {
                    response = CreateCreatedResponse(new PortalResponse { Portal = portal });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(portal.PortalId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PortalResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning, ex);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PortalResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error, ex);
            }
            return response;
        }

        /// <summary>
        /// Update Portal details.
        /// </summary>
        /// <param name="portalModel">Portal Model.</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(PortalResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdatePortal([FromBody] PortalModel portalModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isUpdated = _portalService.UpdatePortal(portalModel);
                response = isUpdated ? CreateOKResponse(new PortalResponse { Portal = portalModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PortalResponse { HasError = true, ErrorCode = ex.ErrorCode, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning, ex);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PortalResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error, ex);
            }
            return response;
        }

        /// <summary>
        /// Delete a portal by portal Id.
        /// </summary>
        /// <param name="portalIds">Id of portal to delete portal.</param>
        /// <param name="isDeleteByStoreCode">if true then Delete operation will be perform by store code</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage DeletePortal([FromBody] ParameterModel portalIds, bool isDeleteByStoreCode)
        {
            HttpResponseMessage response;
            try
            {
                bool isDeleted = _portalService.DeletePortal(portalIds, isDeleteByStoreCode);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isDeleted });
            }
            catch (ZnodeException ex)
            {
                TrueFalseResponse portalResponse = new TrueFalseResponse { HasError = true, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(portalResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                TrueFalseResponse portalResponse = new TrueFalseResponse { HasError = true };
                response = CreateInternalServerErrorResponse(portalResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        ///Copy and create new store.
        /// </summary>
        /// <param name="portalModel">Portal model.</param>
        /// <returns>Returns created Portal.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CopyStore([FromBody]PortalModel portalModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _portalService.CopyStore(portalModel) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get the list of portal feature list.
        /// </summary>
        /// <returns>Response with list of portal features. </returns>
        [ResponseType(typeof(PortalListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPortalFeatureList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _portalCache.GetPortalFeatures(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<PortalListResponse>(data);
            }
            catch (Exception ex)
            {
                PortalListResponse data = new PortalListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }
        
        /// <summary>
        /// Check portal code already exist or not.
        /// </summary>
        /// <param name="portalCode">portalCode</param>
        /// <returns>Returns true if portal code already exist.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage IsPortalCodeExist(string portalCode)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _portalService.IsCodeExists(new HelperParameterModel() { CodeField = portalCode }) });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        #region Inventory Management

        /// <summary>
        /// Get associated warehouse as per selected portal Id.
        /// </summary>
        /// <param name="portalId">portal Id.</param>
        /// <returns>HttpResponseMessage.</returns>
        [ResponseType(typeof(PortalWarehouseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssociatedWarehouseList(int portalId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _portalCache.GetAssociatedWarehouseList(portalId, RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<PortalWarehouseResponse>(data);
            }
            catch (Exception ex)
            {
                PortalWarehouseResponse data = new PortalWarehouseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Associate Warehouse to portal.
        /// </summary>
        /// <param name="portalWarehouseModel">PortalWarehouseModel to be associated.</param>
        /// <returns>Http response containing boolean value whether warehouse are associated or not.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssociateWarehouseToStore([FromBody] PortalWarehouseModel portalWarehouseModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _portalService.AssociateWarehouseToStore(portalWarehouseModel), ErrorCode = 0 });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        #endregion

        #region Portal Locale
        /// <summary>
        /// Get active Locale list.
        /// </summary>
        /// <returns>Returns list of all Locales.</returns>
        [ResponseType(typeof(LocaleListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage LocaleList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _portalCache.LocaleList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<LocaleListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new LocaleListResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }

        /// <summary>
        /// Updates Locale.
        /// </summary>
        /// <param name="defaultGlobalConfigListModel">DefaultGlobalConfigListModel to be updated</param>
        /// <returns>HttpResponse for Default Global Config List Model.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateLocale([FromBody]DefaultGlobalConfigListModel defaultGlobalConfigListModel)
        {
            HttpResponseMessage response;

            try
            {
                bool isSuccess = _portalService.UpdateLocale(defaultGlobalConfigListModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isSuccess });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion

        #region Shipping association
        /// <summary>
        /// Get portal shipping on the basis of portalId.
        /// </summary>
        /// <param name="portalId">portalId to get portal shipping details.</param>
        /// <returns>Returns portal shipping details.</returns>
        [ResponseType(typeof(PortalShippingResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPortalShippingInformation(int portalId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _portalCache.GetPortalShippingInformation(portalId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PortalShippingResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                PortalShippingResponse data = new PortalShippingResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                PortalShippingResponse data = new PortalShippingResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Update portal shipping details.
        /// </summary>
        /// <param name="portalShippingModel">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(PortalShippingResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdatePortalShipping([FromBody] PortalShippingModel portalShippingModel)
        {
            HttpResponseMessage response;
            try
            {
                //Update portal shipping.
                bool portalShipping = _portalService.UpdatePortalShipping(portalShippingModel);
                response = portalShipping ? CreateCreatedResponse(new PortalShippingResponse { PortalShipping = portalShippingModel, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(portalShippingModel.ShippingPortalId)));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                PortalShippingResponse data = new PortalShippingResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                var data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion

        #region Tax association
        /// <summary>
        /// Get portal tax on the basis of portalId.
        /// </summary>
        /// <param name="portalId">portalId to get portal tax details.</param>
        /// <returns>Returns portal tax details.</returns>
        [ResponseType(typeof(TaxPortalResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetTaxPortalInformation(int portalId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _portalCache.GetTaxPortalInformation(portalId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<TaxPortalResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                TaxPortalResponse data = new TaxPortalResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                TaxPortalResponse data = new TaxPortalResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Update portal tax details.
        /// </summary>
        /// <param name="taxPortalModel">Model to update.</param>
        /// <returns>Returns updated portal tax model.</returns>
        [ResponseType(typeof(TaxPortalResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateTaxPortal([FromBody] TaxPortalModel taxPortalModel)
        {
            HttpResponseMessage response;
            try
            {
                //Update portal tax.
                bool portalTax = _portalService.UpdateTaxPortal(taxPortalModel);
                response = portalTax ? CreateCreatedResponse(new TaxPortalResponse { TaxPortal = taxPortalModel, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(taxPortalModel.TaxPortalId)));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                TaxPortalResponse data = new TaxPortalResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                var data = new TaxPortalResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion        

        #region Portal Tracking Pixel.
        /// <summary>
        /// Get portal tracking pixel by portal id.
        /// </summary>
        /// <param name="portalId">Id of portal to get tracking pixel.</param>
        /// <returns>HttpResponseMessage.</returns>
        [ResponseType(typeof(PortalTrackingPixelResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPortalTrackingPixel(int portalId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _portalCache.GetPortalTrackingPixel(portalId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PortalTrackingPixelResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                PortalTrackingPixelResponse portalResponse = new PortalTrackingPixelResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(portalResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Save portal tracking pixel.
        /// </summary>
        /// <param name="portalTrackingPixelModel">portalTrackingPixelModel.</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage SavePortalTrackingPixel([FromBody] PortalTrackingPixelModel portalTrackingPixelModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _portalService.SavePortalTrackingPixel(portalTrackingPixelModel), ErrorCode = 0 });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        #endregion        

        /// <summary>
        /// Associate/Unassociate tax to portal.
        /// </summary>
        /// <param name="taxClassPortalModel">TaxClassPortalModel</param>
        /// <returns>return status as true/false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssociateAndUnAssociateTaxClass([FromBody] TaxClassPortalModel taxClassPortalModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _portalService.AssociateAndUnAssociateTaxClass(taxClassPortalModel), ErrorCode = 0 });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Set default tax for portal.
        /// </summary>
        /// <param name="taxClassPortalModel">TaxClassPortalModel</param>
        /// <returns>return status as true/false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage SetPortalDefaultTax([FromBody] TaxClassPortalModel taxClassPortalModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _portalService.SetPortalDefaultTax(taxClassPortalModel), ErrorCode = 0 });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets list of publish portals status.
        /// </summary>
        /// <returns>return status as true/false.</returns>
        [ResponseType(typeof(PublishPortalLogListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPortalPublishStatus()
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new PublishPortalLogListResponse { PublishPortalLogs = _portalCache.GetPortalPublishStatus(RouteUri, RouteTemplate) });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishPortalLogListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }

            return response;
        }

        #region Robots.txt
        /// <summary>
        /// Get robots.txt data.
        /// </summary>
        /// <param name="portalId">portal id</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(RobotsTxtResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetRobotsTxt(int portalId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _portalCache.GetRobotsTxt(portalId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<RobotsTxtResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                RobotsTxtResponse portalResponse = new RobotsTxtResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(portalResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Save robots.txt data.
        /// </summary>
        /// <param name="robotsTxtModel"> Robots Txt Model</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage SaveRobotsTxt([FromBody] RobotsTxtModel robotsTxtModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _portalService.SaveRobotsTxt(robotsTxtModel) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion


        #region Apporval Routing
        [ResponseType(typeof(PortalApprovalResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPortalApprovalDetailsById(int portalId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _portalCache.GetPortalApprovalDetails(portalId, RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<PortalApprovalResponse>(data);
            } catch (Exception ex)
            {
                PortalApprovalResponse data = new PortalApprovalResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get Portal Approval type list
        /// </summary>
        /// <returns></returns>
        public virtual HttpResponseMessage GetPortalApprovalTypeList()
        {
            HttpResponseMessage response;
            try
            {
                //Get level list.
                string data = _portalCache.GetPortalApprovalTypeList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PortalApprovalTypeListResponse>(data) : CreateNoContentResponse();
            } catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PortalApprovalTypeListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get Portal Approval level list
        /// </summary>
        /// <returns></returns>
        public virtual HttpResponseMessage GetPortalApprovalLevelList()
        {
            HttpResponseMessage response;
            try
            {
                //Get level list.
                string data = _portalCache.GetPortalApprovalLevelList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PortalApprovalLevelListResponse>(data) : CreateNoContentResponse();
            } catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PortalApprovalLevelListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }

        [ResponseType(typeof(UserApproverListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPortalApproverList(int portalApprovalId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _portalCache.GetPortalApproverList(portalApprovalId, RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<UserApproverListResponse>(data);
            } catch (Exception ex)
            {
                UserApproverListResponse data = new UserApproverListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Save/Update the Portal Approval details.
        /// </summary>
        /// <param name="portalApprovalModel">Portal Approval Model.</param>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage SaveUpdatePortalApprovalDetails([FromBody] PortalApprovalModel portalApprovalModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _portalService.SaveUpdatePortalApprovalDetails(portalApprovalModel) });
            } catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
            } catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;

        }
        #endregion


        #region Portal Search Setting
        /// <summary>
        /// Get the list of Sort Setting.
        /// </summary>
        /// <returns>Returns list of Sort Settings.</returns>
        [ResponseType(typeof(SortSettingListResponse))]
        [HttpGet]
        public HttpResponseMessage Sortlist()
        {
            HttpResponseMessage response;

            try
            {
                string data = _portalCache.GetPortalSortSettings(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SortSettingListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new SortSettingListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SortSettingListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get the list of Page Setting.
        /// </summary>
        /// <returns>Returns list of Page Settings.</returns>
        [ResponseType(typeof(PageSettingListResponse))]
        [HttpGet]
        public HttpResponseMessage Pagelist()
        {
            HttpResponseMessage response;

            try
            {
                string data = _portalCache.GetPortalPageSettings(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PageSettingListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PageSettingListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PageSettingListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// </summary>
        /// <param name="associationModel">Association model.</param>
        /// <returns>Returns true is removed successfully.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage RemoveAssociatedSortSettings(SortSettingAssociationModel associationModel)
        {
            HttpResponseMessage response;

            try
            {
                bool isUnassociated = _portalService.RemoveAssociatedSortSettings(associationModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isUnassociated });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// </summary>
        /// <param name="associationModel">Association model.</param>
        /// <returns>Returns true is removed successfully.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage RemoveAssociatedPageSettings(PageSettingAssociationModel associationModel)
        {
            HttpResponseMessage response;

            try
            {
                bool isUnassociated = _portalService.RemoveAssociatedPageSettings(associationModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isUnassociated });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Associate sort settings.
        /// </summary>
        /// <param name="associationModel">Association model.</param>
        /// <returns>Returns true is associated successfully.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage AssociateSortSettings(SortSettingAssociationModel associationModel)
        {
            HttpResponseMessage response;

            try
            {
                bool isAssociated = _portalService.AssociateSortSettings(associationModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isAssociated });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Associate page settings.
        /// </summary>
        /// <param name="associationModel">Association model.</param>
        /// <returns>Returns true is associated successfully.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage AssociatePageSettings(PageSettingAssociationModel associationModel)
        {
            HttpResponseMessage response;

            try
            {
                bool isAssociated = _portalService.AssociatePageSettings(associationModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isAssociated });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Update portal page setting model
        /// </summary>
        /// <param name="portalPageSettingModel"></param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage UpdatePortalPageSetting(PortalPageSettingModel portalPageSettingModel)
        {
            HttpResponseMessage response;

            try
            {
                bool isAssociated = _portalService.UpdatePortalPageSetting(portalPageSettingModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isAssociated });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }


        #endregion

        #region Portal Barcode setting

        /// <summary>
        /// Get Barcode scanner setting details.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(BarcodeReaderModelResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetBarcodeScanner()
        {
            HttpResponseMessage response;
            try
            {
                string data = _portalCache.GetBarcodeScanner(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<BarcodeReaderModelResponse>(data);
            }
            catch (Exception ex)
            {
                BarcodeReaderModelResponse data = new BarcodeReaderModelResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        #endregion
    }
}
