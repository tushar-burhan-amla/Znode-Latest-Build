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
    public class BrandController : BaseController
    {
        #region Private Variables
        private readonly IBrandService _service;
        private readonly IBrandCache _cache;
        #endregion

        #region Default Constructor
        public BrandController(IBrandService service)
        {
            _service = service;
            _cache = new BrandCache(_service);
        }
        #endregion

        /// <summary>
        /// Gets list of brands.
        /// </summary>
        /// <returns>It will return the list of brands</returns>
        [ResponseType(typeof(BrandListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetBrands(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<BrandListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                var data = new BrandListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets a brand by brandId.
        /// </summary>
        /// <param name="brandId">ID of brand to be retrieved.</param>
        /// <param name="localeId">ID of local </param>
        /// <returns>It will return brand base on brand Id</returns>
        [ResponseType(typeof(BrandResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(int brandId, int localeId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetBrand(brandId, localeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<BrandResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                BrandResponse data = new BrandResponse { HasError = true, ErrorMessage = ex.Message };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Method to create brand.
        /// </summary>
        /// <param name="model">Brand model.</param>
        /// <returns>Returns created brand.</returns>
        [ResponseType(typeof(BrandResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Create([FromBody]BrandModel model)
        {
            HttpResponseMessage response;
            try
            {
                BrandModel brand = _service.CreateBrand(model);
                if (!Equals(brand, null))
                {
                    response = CreateCreatedResponse(new BrandResponse { Brand = brand });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(brand.BrandId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                BrandResponse data = new BrandResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                BrandResponse data = new BrandResponse { HasError = true, ErrorMessage = ex.Message };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Method to update brand.
        /// </summary>
        /// <param name="model">Brand model to be updated.</param>
        /// <returns>Returns updated brand model.</returns>
        [ResponseType(typeof(BrandResponse))]
        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody]BrandModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool isUpdated = _service.UpdateBrand(model);
                response = isUpdated ? CreateCreatedResponse(new BrandResponse { Brand = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.BrandId)));
            } 
            catch(ZnodeException ex)
            {
                BrandResponse data = new BrandResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };              
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                BrandResponse data = new BrandResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Method to delete brand.
        /// </summary>
        /// <param name="model">Brand Model with Brand Ids and flag to delete Publish Brand.</param>
        /// <returns>Returns status as per delete operation</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete(ParameterModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.DeleteBrand(model);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message};
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets list of brand code.
        /// </summary>
        /// <returns>Returns brand code list.</returns>
        [ResponseType(typeof(BrandListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetBrandCodeList(string attributeCode)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute types.
                string data = _cache.GetBrandCodeList(attributeCode, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<BrandListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                BrandListResponse data = new BrandListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Associate brand product
        /// </summary>
        /// <param name="brandProductModel">BrandProductModel</param>
        /// <returns>Return status true/false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssociateAndUnAssociateProduct([FromBody] BrandProductModel brandProductModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _service.AssociateAndUnAssociateProduct(brandProductModel), ErrorCode = 0 });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AssociationDeleteError });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Active/Inactive Brands
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Return status true/false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage ActiveInactiveBrand(ActiveInactiveBrandModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool status = _service.ActiveInactiveBrand(model);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            } 
            catch(ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get the list of all Portals.
        /// </summary>
        /// <returns>Response with All portals List</returns>
        [ResponseType(typeof(PortalBrandListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetBrandPortalList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetBrandPortalList(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<PortalBrandListResponse>(data);
            }
            catch (Exception ex)
            {
                PortalBrandListResponse data = new PortalBrandListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Associate brand Portal
        /// </summary>
        /// <param name="brandPortalModel">brandPortalModel</param>
        /// <returns>Return status true/false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssociateAndUnAssociatePortal([FromBody] BrandPortalModel brandPortalModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _service.AssociateAndUnAssociatePortal(brandPortalModel), ErrorCode = 0 });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AssociationDeleteError });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Associate brand Portal
        /// </summary>
        /// <param name="brandCode">code</param>
        /// <returns>Return status true/false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage CheckUniqueBrandCode(string brandCode)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _service.CheckBrandCode(brandCode), ErrorCode = 0 });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Associate brands to portal Portal
        /// </summary>
        /// <param name="portalBrandAssociationModel">associatePortalBrandModel</param>
        /// <returns>Return status true/false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssociateAndUnAssociatePortalBrand([FromBody] PortalBrandAssociationModel portalBrandAssociationModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _service.AssociateAndUnAssociatePortalBrands(portalBrandAssociationModel), ErrorCode = 0 });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AssociationDeleteError });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Ge the list of all Brands associated and Unassociated with portal..
        /// </summary>
        /// <returns>Returns Brand list.</returns>
        [ResponseType(typeof(TaxClassListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPortalBrandList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetPortalBrandList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<BrandListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                var data = new BrandListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update Associated portal Brand Detail.
        /// </summary>
        /// <param name="portalBrandDetailModel"></param>
        /// <returns>PortalBrandDetailResponse</returns>
        [HttpPut]
        [ResponseType(typeof(PortalBrandDetailResponse))]
        public virtual HttpResponseMessage UpdateAssociatedPortalBrandDetail([FromBody] PortalBrandDetailModel portalBrandDetailModel)
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.UpdateAssociatedPortalBrandDetail(portalBrandDetailModel);
                response = data ? CreateCreatedResponse(new PortalBrandDetailResponse { PortalBrandDetailModel = portalBrandDetailModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PortalBrandDetailResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PortalBrandDetailResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
    }
}