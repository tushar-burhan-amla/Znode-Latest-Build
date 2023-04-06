using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using System.Web.Http.Description;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class VendorController : BaseController
    {
        #region Private Variables
        private readonly IVendorService _service;
        private readonly IVendorCache _cache;
        #endregion

        #region Default Constructor
        public VendorController(IVendorService service)
        {
            _service = service;
            _cache = new VendorCache(_service);
        }
        #endregion

        /// <summary>
        /// Gets list of vendors.
        /// </summary>
        /// <returns>It will return list of vendors.</returns>
        [ResponseType(typeof(VendorListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetVendors(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<VendorListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                var data = new VendorListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Method to create vendor.
        /// </summary>
        /// <param name="model">Vendor model.</param>
        /// <returns>Returns created vendor.</returns>
        [ResponseType(typeof(VendorResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Create([FromBody]VendorModel model)
        {
            HttpResponseMessage response;
            try
            {
                VendorModel vendor = _service.CreateVendor(model);
                if (!Equals(vendor, null))
                {
                    response = CreateCreatedResponse(new VendorResponse { Vendor = vendor });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(vendor.PimVendorId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                VendorResponse data = new VendorResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                VendorResponse data = new VendorResponse { HasError = true, ErrorMessage = ex.Message };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Gets a vendor by PimVendorId.
        /// </summary>
        /// <param name="PimVendorId">ID of vendor to be retrieved.</param>
        /// <returns>Vendor model according to PimVendorId</returns>
        [ResponseType(typeof(VendorResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(int PimVendorId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetVendor(PimVendorId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<VendorResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                VendorResponse data = new VendorResponse { HasError = true, ErrorMessage = ex.Message };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Method to update vendor.
        /// </summary>
        /// <param name="model">Vendor model to be updated.</param>
        /// <returns>Returns updated vendor model.</returns>
        [ResponseType(typeof(VendorResponse))]
        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody]VendorModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool isUpdated = _service.UpdateVendor(model);
                response = isUpdated ? CreateCreatedResponse(new VendorResponse { Vendor = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.PimVendorId)));
            }
            catch (ZnodeException ex)
            {
                VendorResponse data = new VendorResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                VendorResponse data = new VendorResponse { HasError = true, ErrorMessage = ex.Message };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(data);
            }

            return response;

        }

        /// <summary>
        /// Method to delete vendor.
        /// </summary>
        /// <param name="model">Vendor Model with Vendor Ids and flag to delete Publish Vendor.</param>
        /// <returns>Returns status as per delete operation</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete(ParameterModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.DeleteVendor(model);
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
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Active/Inactive Vendor.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns true/false status</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage ActiveInactiveVendor(ActiveInactiveVendorModel model)
        {
            HttpResponseMessage response;

            try
            {
                bool status = _service.ActiveInactiveVendor(model);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
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
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets list of vendor code.
        /// </summary>
        /// <returns>Returns vendor code list.</returns>
        [ResponseType(typeof(VendorListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetVendorCodeList(string attributeCode)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute types.
                string data = _cache.GetVendorCodeList(attributeCode, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<VendorListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                VendorListResponse data = new VendorListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get associate products to vendor.
        /// </summary>
        /// <param name="vendorProductModel">VendorProductModel</param>
        /// <returns>Boolean Response</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssociateAndUnAssociateProduct([FromBody] VendorProductModel vendorProductModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _service.AssociateAndUnAssociateProduct(vendorProductModel), ErrorCode = 0 });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AssociationDeleteError });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }
    }
}