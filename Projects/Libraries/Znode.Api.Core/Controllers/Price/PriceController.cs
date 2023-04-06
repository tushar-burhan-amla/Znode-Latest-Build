using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Framework.Business;
using System.Web.Http.Description;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class PriceController : BaseController
    {
        #region Private Variables
        private readonly IPriceCache _cache;
        private readonly IPriceService _service;
        #endregion

        #region Constructor
        public PriceController(IPriceService service)
        {
            _service = service;
            _cache = new PriceCache(_service);
        }
        #endregion

        #region Price
        /// <summary>
        /// Create Price.
        /// </summary>
        /// <param name="model">PriceModel model.</param>
        /// <returns>Returns created model.</returns>
        [HttpPost, ValidateModel]
        [ResponseType(typeof(PriceResponse))]
        public virtual HttpResponseMessage Create([FromBody] PriceModel model)
        {
            HttpResponseMessage response;

            try
            {
                PriceModel price = _service.CreatePrice(model);

                if (!Equals(price, null))
                {
                    response = CreateCreatedResponse(new PriceResponse { Price = price });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(price.PriceListId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PriceResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PriceResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get Price by Price list id.
        /// </summary>
        /// <param name="priceListId">Price list id to get Price details.</param>
        /// <returns>Returns Price model.</returns>
        [HttpGet]
        [ResponseType(typeof(PriceResponse))]
        public virtual HttpResponseMessage GetPrice(int priceListId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetPrice(priceListId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PriceResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PriceResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Update Price.
        /// </summary>
        /// <param name="model">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [HttpPut]
        [ResponseType(typeof(PriceResponse))]
        public virtual HttpResponseMessage Update([FromBody] PriceModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update Price.
                bool price = _service.UpdatePrice(model);
                response = price ? CreateCreatedResponse(new PriceResponse { Price = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.PriceListId)));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PriceResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PriceResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Gets list of Price.
        /// </summary>
        /// <returns>Returns list of price.</returns>
        [HttpGet]
        [ResponseType(typeof(PriceListResponse))]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetPriceList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PriceListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PriceListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Delete price.
        /// </summary>
        /// <param name="priceListId">Price List Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [HttpPost, ValidateModel]
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage Delete([FromBody] ParameterModel priceListId)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeletePrice(priceListId) });
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
        /// Method to copy Price.
        /// </summary>
        /// <param name="priceModel">Price model.</param>
        /// <returns>Returns created Price.</returns>
        [HttpPost]
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage Copy([FromBody]PriceModel priceModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.CopyPrice(priceModel) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message,ErrorCode=ErrorCodes.AlreadyExist });
            }
            return response;
        }

        #endregion

        #region SKU Price
        /// <summary>
        /// Method to add SKU Price.
        /// </summary>
        /// <param name="priceSKU">PriceSKUDataModel model.</param>
        /// <returns>Returns created SKU Price.</returns>
        [HttpPost, ValidateModel]
        [ResponseType(typeof(PriceSKUResponse))]
        public virtual HttpResponseMessage AddSKUPrice([FromBody] PriceSKUModel priceSKU)
        {
            HttpResponseMessage response;
            try
            {
                priceSKU = _service.AddSKUPrice(priceSKU);
                if (!Equals(priceSKU, null))
                {
                    response = CreateCreatedResponse(new PriceSKUResponse { PriceSKU = priceSKU });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(priceSKU.PriceId)));
                }
                else
                    response = CreateInternalServerErrorResponse();

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PriceSKUResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PriceSKUResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// To get SKU Price by Price id.
        /// </summary>
        /// <param name="priceId">Price Id</param>
        /// <returns>PriceSKUModel model</returns>   
        [HttpGet]
        [ResponseType(typeof(PriceSKUResponse))]
        public virtual HttpResponseMessage GetSKUPrice(int priceId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetSKUPrice(priceId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PriceSKUResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PriceSKUResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PriceSKUResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get List of SKUPrice.
        /// </summary>
        /// <returns>SKU Price List.</returns>
        [HttpGet]
        [ResponseType(typeof(PriceSKUListResponse))]
        public virtual HttpResponseMessage GetSKUPriceList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetSKUPriceList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PriceSKUListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PriceSKUListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Update an Exiting SKU Price.
        /// </summary>
        /// <param name="priceSKUModel">PriceSKUModel.</param>
        /// <returns>Updated PriceSKU model.</returns>
        [HttpPut]
        [ResponseType(typeof(PriceSKUResponse))]
        public virtual HttpResponseMessage UpdateSKUPrice([FromBody] PriceSKUModel priceSKUModel)
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.UpdateSKUPrice(priceSKUModel);
                response = data ? CreateCreatedResponse(new PriceSKUResponse { PriceSKU = priceSKUModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PriceSKUResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PriceSKUResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Delete SKU Price.
        /// </summary>
        /// <param name="skuPriceDeleteModel">PriceId to delete SKU Price.</param>
        /// <returns>true if deleted.</returns>
        [HttpPost]
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage DeleteSKUPrice([FromBody] SKUPriceDeleteModel skuPriceDeleteModel)
        {
            HttpResponseMessage response;

            try
            {
                response = _service.DeleteSKUPrice(skuPriceDeleteModel) ? CreateOKResponse(new TrueFalseResponse { IsSuccess = true }) : CreateInternalServerErrorResponse();
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
        /// To get Price by sku.
        /// </summary>
        /// <returns>PriceSKUModel model</returns>   
        [HttpGet]
        [ResponseType(typeof(PriceSKUResponse))]
        public virtual HttpResponseMessage GetPriceBySku()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetPriceBySku(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PriceSKUResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PriceSKUResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PriceSKUResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// To get product Price with details by product price model.
        /// </summary>
        /// <returns>PriceSKUModel model</returns>   
        [HttpPost]
        [ResponseType(typeof(PriceSKUResponse))]
        public virtual HttpResponseMessage GetProductPricingDetailsBySku([FromBody] ProductPriceListSKUDataModel productPriceListSKUDataModel)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetProductPricingDetailsBySku(productPriceListSKUDataModel);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PriceSKUResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PriceSKUResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get Uom List.
        /// </summary>
        /// <returns>Uom List.</returns>
        [HttpGet]
        [ResponseType(typeof(UomListResponse))]
        public virtual HttpResponseMessage GetUomList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUomList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<UomListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new UomListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get SKU Price List by Catalog Id.
        /// </summary>
        /// <param name="catalogId">Catalog Id</param>
        /// <returns>SKU and its Prices on the basis of CatalogId</returns>
        [HttpGet]
        [ResponseType(typeof(PriceSKUListResponse))]
        public virtual HttpResponseMessage GetPagedPriceSku()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetPagedPriceSKU(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PriceSKUListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PriceSKUListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PriceSKUListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion

        #region Tier Price
        /// <summary>
        /// Add Tier Price.
        /// </summary>
        /// <param name="priceTierListModel">PriceTierListModel listModel.</param>
        /// <returns>Returns created model.</returns>
        [HttpPost]
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage AddTierPrice([FromBody] PriceTierListModel priceTierListModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _service.AddTierPrice(priceTierListModel), ErrorCode = 0 });
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
        /// To get Tier Price by PriceTierId.
        /// </summary>
        /// <param name="priceTierId">PriceTierId</param>
        /// <returns>PriceTierModel model</returns>   
        [HttpGet]
        [ResponseType(typeof(TierPriceResponse))]
        public virtual HttpResponseMessage GetTierPrice(int priceTierId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetTierPrice(priceTierId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<TierPriceResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TierPriceResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get List of Tier Price.
        /// </summary>
        /// <returns>Tier Price List.</returns>
        [HttpGet]
        [ResponseType(typeof(TierPriceListResponse))]
        public virtual HttpResponseMessage GetTierPriceList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetTierPriceList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<TierPriceListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TierPriceListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Update Tier Price.
        /// </summary>
        /// <param name="tierPriceModel">PriceTierModel.</param>
        /// <returns>Updated Tier Price model.</returns>
        [HttpPut]
        [ResponseType(typeof(TierPriceResponse))]
        public virtual HttpResponseMessage UpdateTierPrice([FromBody] PriceTierModel tierPriceModel)
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.UpdateTierPrice(tierPriceModel);
                response = data ? CreateCreatedResponse(new TierPriceResponse { TierPrice = tierPriceModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TierPriceResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TierPriceResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Delete Tier Price.
        /// </summary>
        /// <param name="priceTierId">PriceTierId to delete Tier Price.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [HttpPost]
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage DeleteTierPrice(int priceTierId)
        {
            HttpResponseMessage response;

            try
            {
                bool deleted = _service.DeleteTierPrice(priceTierId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TierPriceResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion

        #region Associate Store
        /// <summary>
        /// Get List of associated store for price.
        /// </summary>
        /// <returns>SKU Price List.</returns>
        [HttpGet]
        [ResponseType(typeof(PricePortalListResponse))]
        public virtual HttpResponseMessage GetAssociatedStoreList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssociatedStoreList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PricePortalListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PricePortalListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Gets list of unassociated stores.
        /// </summary>
        /// <returns>SKU Price List.</returns>
        [HttpGet]
        [ResponseType(typeof(PortalListResponse))]
        public virtual HttpResponseMessage GetUnAssociatedStoreList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUnAssociatedStoreList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PortalListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PortalListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound });
            }
            return response;
        }

        /// <summary>
        /// Associate Store to price.
        /// </summary>
        /// <param name="listModel">PricePortalModel.</param>
        /// <returns>Returns associated store to price.</returns>
        [HttpPost]
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage AssociateStore([FromBody] PricePortalListModel listModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _service.AssociateStore(listModel), ErrorCode = 0 });
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
        /// To get associated stores precedence value.
        /// </summary>
        /// <param name="priceListPortalId">PriceListPortalId.</param>
        /// <returns>PriceListPortalId model.</returns>      
        [HttpGet]
        [ResponseType(typeof(PricePortalResponse))]
        public virtual HttpResponseMessage GetAssociatedStorePrecedence(int priceListPortalId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssociatedStorePrecedence(priceListPortalId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PricePortalResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PricePortalResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PricePortalResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Update associated stores precedence value.
        /// </summary>
        /// <param name="pricePortalModel">PricePortalModel.</param>
        /// <returns>Updated associated stores precedence value.</returns>
        [HttpPut]
        [ResponseType(typeof(PricePortalResponse))]
        public virtual HttpResponseMessage UpdateAssociatedStorePrecedence([FromBody] PricePortalModel pricePortalModel)
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.UpdateAssociatedStorePrecedence(pricePortalModel);
                response = data ? CreateCreatedResponse(new PricePortalResponse { PricePortal = pricePortalModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PricePortalResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PricePortalResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion

        #region Associate Customer
        /// <summary>
        /// Get List of associated customer for price.
        /// </summary>
        /// <returns>Customer Price List.</returns>
        [HttpGet]
        [ResponseType(typeof(PriceUserListResponse))]
        public virtual HttpResponseMessage GetAssociatedCustomerList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssociatedCustomerList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PriceUserListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PriceUserListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Gets list of unassociated customer.
        /// </summary>
        /// <returns>Customer List.</returns>
        [HttpGet]
        [ResponseType(typeof(PriceUserListResponse))]
        public virtual HttpResponseMessage GetUnAssociatedCustomerList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUnAssociatedCustomerList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PriceUserListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PriceUserListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound });
            }
            return response;
        }

        /// <summary>
        /// Associate Customer to price.
        /// </summary>
        /// <param name="listModel">PriceUserListModel.</param>
        /// <returns>Returns associated customer to price.</returns>
        [HttpPost]
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage AssociateCustomer([FromBody] PriceUserListModel listModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isAssociated = _service.AssociateCustomer(listModel);
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = isAssociated, ErrorCode = 0 });
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
        /// Delete Associated Customer.
        /// </summary>
        /// <param name="customerIds">customerIds.</param>
        /// <returns>Returns true if Associated Customer deleted successfully else return false.</returns>
        [HttpPost, ValidateModel]
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage DeleteAssociatedCustomer([FromBody] ParameterModel customerIds)
        {
            HttpResponseMessage response;

            try
            {
                bool deleted = _service.DeleteAssociatedCustomer(customerIds);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
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
        /// To get associated customers precedence value.
        /// </summary>
        /// <param name="priceListUserId">priceListUserId.</param>
        /// <returns>PriceListUserId model.</returns>   
        [HttpGet]
        [ResponseType(typeof(PriceUserResponse))]
        public virtual HttpResponseMessage GetAssociatedCustomerPrecedence(int priceListUserId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssociatedCustomerPrecedence(priceListUserId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PriceUserResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PriceUserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PriceUserResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Update associated customers precedence value.
        /// </summary>
        /// <param name="priceAccountModel">PriceAccountModel.</param>
        /// <returns>Updated associated customers precedence value.</returns>
        [HttpPut]
        [ResponseType(typeof(PriceUserResponse))]
        public virtual HttpResponseMessage UpdateAssociatedCustomerPrecedence([FromBody] PriceUserModel priceAccountModel)
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.UpdateAssociatedCustomerPrecedence(priceAccountModel);
                response = data ? CreateCreatedResponse(new PriceUserResponse { PriceUser = priceAccountModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PriceUserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PriceUserResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Remove associated stores.
        /// </summary>
        /// <param name="model">Model contains data to remove.</param>
        /// <returns>Retrun success response.</returns>
        [HttpPost, ValidateModel]
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage RemoveAssociatedStores([FromBody] RemoveAssociatedStoresModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = _service.RemoveAssociatedStores(model) ? CreateOKResponse(new TrueFalseResponse { IsSuccess = true }) : CreateInternalServerErrorResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion

        #region Associate Account
        /// <summary>
        /// Get List of associated account for price.
        /// </summary>
        /// <returns>Account Price List.</returns>
        [HttpGet]
        [ResponseType(typeof(PriceAccountListResponse))]
        public virtual HttpResponseMessage GetAssociatedAccountList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssociatedAccountList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PriceAccountListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PriceAccountListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Gets list of unassociated account.
        /// </summary>
        /// <returns>Account List.</returns>
        [HttpGet]
        [ResponseType(typeof(PriceAccountListResponse))]
        public virtual HttpResponseMessage GetUnAssociatedAccountList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUnAssociatedAccountList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PriceAccountListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PriceAccountListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound });
            }
            return response;
        }

        /// <summary>
        /// Associate Account to price.
        /// </summary>
        /// <param name="listModel">PriceAccountModel.</param>
        /// <returns>Returns associated account to price.</returns>
        [HttpPost]
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage AssociateAccount([FromBody] PriceAccountListModel listModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isAssociated = _service.AssociateAccount(listModel);
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = isAssociated, ErrorCode = 0 });
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
        /// Remove associated accounts.
        /// </summary>
        /// <param name="model">Model contains data to remove.</param>
        /// <returns></returns>
        [HttpPost, ValidateModel]
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage RemoveAssociatedAccounts([FromBody] RemoveAssociatedAccountsModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = _service.RemoveAssociatedAccounts(model) ? CreateOKResponse(new TrueFalseResponse { IsSuccess = true }) : CreateInternalServerErrorResponse();
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
        /// To get associated account precedence value.
        /// </summary>
        /// <param name="priceListUserId">priceListUserId.</param>
        /// <returns>PriceListUserId model.</returns>   
        [HttpGet]
        [ResponseType(typeof(PriceAccountResponse))]
        public virtual HttpResponseMessage GetAssociatedAccountPrecedence(int priceListUserId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssociatedAccountPrecedence(priceListUserId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PriceAccountResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PriceAccountResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PriceAccountResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Update associated account precedence value.
        /// </summary>
        /// <param name="priceAccountModel">PriceAccountModel.</param>
        /// <returns>Updated associated accounts precedence value.</returns>
        [HttpPut]
        [ResponseType(typeof(PriceAccountResponse))]
        public virtual HttpResponseMessage UpdateAssociatedAccountPrecedence([FromBody] PriceAccountModel priceAccountModel)
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.UpdateAssociatedAccountPrecedence(priceAccountModel);
                response = data ? CreateCreatedResponse(new PriceAccountResponse { PriceAccount = priceAccountModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PriceAccountResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PriceAccountResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion

        #region Associate Profile
        /// <summary>
        /// Get List of associated profile for price.
        /// </summary>
        /// <returns>Returns associated profile list.</returns>
        [HttpGet]
        [ResponseType(typeof(PriceProfileListResponse))]
        public virtual HttpResponseMessage GetAssociatedProfileList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssociatedProfileList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PriceProfileListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PriceProfileListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Gets list of unassociated profiles.
        /// </summary>
        /// <returns>Returns unassociated profile list.</returns>
        [HttpGet]
        [ResponseType(typeof(ProfileListResponse))]
        public virtual HttpResponseMessage GetUnAssociatedProfileList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUnAssociatedProfileList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProfileListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ProfileListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound });
            }
            return response;
        }

        /// <summary>
        /// Associate Profile to price.
        /// </summary>
        /// <param name="listModel">PriceProfileModel.</param>
        /// <returns>Returns associated Profile to price.</returns>
        [HttpPost]
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage AssociateProfile([FromBody] PriceProfileListModel listModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _service.AssociateProfile(listModel), ErrorCode = 0 });
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
        /// Remove associated profiles.
        /// </summary>
        /// <param name="model">Model contains data to remove.</param>
        /// <returns>Return true if removed successfully else false.</returns>
        [HttpPost, ValidateModel]
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage RemoveAssociatedProfiles([FromBody] RemoveAssociatedProfilesModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = _service.RemoveAssociatedProfiles(model) ? CreateOKResponse(new TrueFalseResponse { IsSuccess = true }) : CreateInternalServerErrorResponse();
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
        /// To get associated profile precedence value.
        /// </summary>
        /// <param name="priceListProfileId">priceListProfileId.</param>
        /// <returns>PriceListProfileId model.</returns>   
        [HttpGet]
        [ResponseType(typeof(PriceProfileResponse))]
        public virtual HttpResponseMessage GetAssociatedProfilePrecedence(int priceListProfileId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssociatedProfilePrecedence(priceListProfileId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PriceProfileResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PriceProfileResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PriceProfileResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Update associated profile precedence value.
        /// </summary>
        /// <param name="priceProfileModel">PriceProfileModel.</param>
        /// <returns>Updated associated profile precedence value.</returns>
        [HttpPut]
        [ResponseType(typeof(PriceProfileResponse))]
        public virtual HttpResponseMessage UpdateAssociatedProfilePrecedence([FromBody] PriceProfileModel priceProfileModel)
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.UpdateAssociatedProfilePrecedence(priceProfileModel);
                response = data ? CreateCreatedResponse(new PriceProfileResponse { PriceProfile = priceProfileModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PriceProfileResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PriceProfileResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion

        #region Price Management
        /// <summary>
        /// Get list of unassociated price list.
        /// </summary>
        /// <returns>Returns unassociated price list.</returns>
        [HttpGet]
        [ResponseType(typeof(PriceListResponse))]
        public virtual HttpResponseMessage GetUnAssociatedPriceList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUnAssociatedPriceList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PriceListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PriceListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            return response;
        }

        /// <summary>
        /// Remove associated price lists from store.
        /// </summary>
        /// <param name="model">Model contains data to remove.</param>
        /// <returns></returns>
        [HttpPost, ValidateModel]
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage RemoveAssociatedPriceListToStore([FromBody] ParameterModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.RemoveAssociatedPriceListToStore(model) });
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
        /// Remove associated price lists from profile.
        /// </summary>
        /// <param name="priceListProfileIds">priceListProfileIds to remove.</param>
        /// <returns></returns>
        [HttpPost, ValidateModel]
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage RemoveAssociatedPriceListToProfile([FromBody] ParameterModel priceListProfileIds)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.RemoveAssociatedPriceListToProfile(priceListProfileIds) });
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
        /// To get associated price list precedence value for Store/Profile.
        /// </summary>
        /// <param name="pricePortalModel">pricePortalModel contains priceListId and portalId/profileId to get precedence value.</param>
        /// <returns>PriceListPortalModel.</returns>   
        [HttpPost]
        [ResponseType(typeof(PricePortalResponse))]
        public virtual HttpResponseMessage GetAssociatedPriceListPrecedence([FromBody] PricePortalModel pricePortalModel)
        {
            HttpResponseMessage response;
            try
            {
                PricePortalModel data = _service.GetAssociatedPriceListPrecedence(pricePortalModel);
                response = IsNotNull(data) ? CreateOKResponse(new PricePortalResponse { PricePortal = data }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PricePortalResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PricePortalResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Update associated price list precedence value for Store/Profile.
        /// </summary>
        /// <param name="pricePortalModel">PricePortalModel.</param>
        /// <returns>Updated associated price list precedence value.</returns>
        [HttpPut]
        [ResponseType(typeof(PricePortalResponse))]
        public virtual HttpResponseMessage UpdateAssociatedPriceListPrecedence([FromBody] PricePortalModel pricePortalModel)
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.UpdateAssociatedPriceListPrecedence(pricePortalModel);
                response = data ? CreateCreatedResponse(new PricePortalResponse { PricePortal = pricePortalModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PricePortalResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PricePortalResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion

        /// <summary>
        /// Get the pricing data to export.
        /// </summary>
        /// <param name="priceListIds">ID's of priceList.</param>
        /// <returns>HttpResponseMessage</returns>
        [HttpGet]
        [ResponseType(typeof(ExportPriceListResponse))]
        public virtual HttpResponseMessage GetExportPriceData(string priceListIds)
        {
            HttpResponseMessage response;
            try
            {
                List<ExportPriceModel> priceData = _service.GetExportPriceData(priceListIds);
                response = IsNotNull(priceData) ? CreateOKResponse(new ExportPriceListResponse { ExportPriceList = priceData }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ExportPriceListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
    }
}
