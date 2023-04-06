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
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class CustomerController : BaseController
    {
        #region Private Variables
        private readonly ICustomerCache _cache;
        private readonly ICustomerService _service;
        #endregion

        #region Constructor
        public CustomerController(ICustomerService service)
        {
            _service = service;
            _cache = new CustomerCache(_service);
        }
        #endregion

        #region Public Methods
        #region Profile Association
        /// <summary>
        /// Get list of unassociate profiles.
        /// </summary>
        /// <returns>Unassociate profile list.</returns>
        [ResponseType(typeof(ProfileListResponse))]
        [HttpGet]
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
                response = CreateInternalServerErrorResponse(new ProfileListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get list of associate profiles based on portal.
        /// </summary>
        /// <returns>Return list of associate profiles.</returns>
        [ResponseType(typeof(ProfileListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssociatedProfileList()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAssociatedProfileList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProfileListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ProfileListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Remove associated profiles.
        /// </summary>
        /// <param name="profileIds">profileIds to unassociate profiles.</param>
        /// <param name="userId">user Id of which profiles need to unassociate profiles.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage UnAssociateProfiles([FromBody] ParameterModel profileIds, int userId)
        {
            HttpResponseMessage response;

            try
            {
                //Remove associated profiles.
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.UnAssociateProfiles(profileIds, userId) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Associate profiles.
        /// </summary>
        /// <param name="model">model with userId and profileIds to associate profiles.</param>
        /// <returns>Returns associate profiles if true else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage AssociateProfiles([FromBody] ParameterModelUserProfile model)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.AssociateProfiles(model) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Set default profile for customer.
        /// </summary>
        /// <param name="model">model with userId, profileId, default flag.</param>
        /// <returns>Returns true if set profile successfully else returns  false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage SetDefaultProfile([FromBody] ParameterModelUserProfile model)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.SetDefaultProfile(model) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get list of associate profiles based on portal.
        /// </summary>
        /// <returns>Return list of associate profiles.</returns>
        [ResponseType(typeof(ProfileListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCustomerPortalProfilelist()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetCustomerPortalProfilelist(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProfileListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ProfileListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }

            return response;
        }
        #endregion

        #region Affiliate   
        /// <summary>
        /// Get list of referral commission type.
        /// </summary>
        /// <returns>Returns list of referral commission type.</returns>
        [ResponseType(typeof(ReferralCommissionListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetReferralCommissionTypeList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetReferralCommissionTypeList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ReferralCommissionListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReferralCommissionListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get customer affiliate data.
        /// </summary>
        /// <param name="userId">User id of the customer.</param>
        /// <returns>Returns customer affiliate data.</returns>
        [ResponseType(typeof(ReferralCommissionResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCustomerAffiliate(int userId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetCustomerAffiliate(userId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ReferralCommissionResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ReferralCommissionResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReferralCommissionResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update customer affiliate data.
        /// </summary>
        /// <param name="referralCommissionModel">Referral commission model to update in database.</param>
        /// <returns>Returns customer affiliate data.</returns>
        [ResponseType(typeof(ReferralCommissionResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateCustomerAffiliate([FromBody] ReferralCommissionModel referralCommissionModel)
        {
            HttpResponseMessage response;

            try
            {
                bool user = _service.UpdateCustomerAffiliate(referralCommissionModel);
                response = user ? CreateOKResponse(new ReferralCommissionResponse { ReferralCommission = referralCommissionModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ReferralCommissionResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReferralCommissionResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get list of referral commission for User.
        /// </summary>
        /// <returns>Returns list of referral commission.</returns>
        [ResponseType(typeof(ReferralCommissionListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetReferralCommissionList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetReferralCommissionList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ReferralCommissionListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReferralCommissionListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion

        #region Address
        /// <summary>
        /// Gets the address list for user Id.
        /// </summary>
        /// <returns>Returns Address list.</returns>
        [ResponseType(typeof(AddressListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage AddressList()
        {
            HttpResponseMessage response;
            try
            {
                //Get account list.
                string data = _cache.GetAddressList(RouteUri, RouteTemplate);
                // TODO: Add info logs
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AddressListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AddressListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Create customer Address.
        /// </summary>
        /// <param name="model">AddressModel model.</param>
        /// <returns>Returns created model.</returns>
        [ResponseType(typeof(AddressResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage CreateCustomerAddress([FromBody] AddressModel model)
        {
            HttpResponseMessage response;

            try
            {
                AddressModel address = _service.CreateCustomerAddress(model);

                if (HelperUtility.IsNotNull(address))
                {
                    response = CreateCreatedResponse(new AddressResponse { Address = address });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(address?.UserAddressId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AddressResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AddressResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get customer address by filters.
        /// </summary>       
        /// <returns>Returns customer's Address.</returns>
        [ResponseType(typeof(AddressResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCustomerAddress()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetCustomerAddress(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AddressResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AddressResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AddressResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update customer address.
        /// </summary>
        /// <param name="model">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(AddressResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateCustomerAddress([FromBody] AddressModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool note = _service.UpdateCustomerAddress(model);
                response = note ? CreateCreatedResponse(new AddressResponse { Address = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.UserAddressId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AddressResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AddressResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete customer address.
        /// </summary>
        /// <param name="userAddressId">customer address Ids.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteCustomerAddress([FromBody] ParameterModel userAddressId)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteCustomerAddress(userAddressId) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new NoteResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new NoteResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Get the list of search locations on the basis of search term.
        /// </summary>
        /// <param name="portalId">Portal Id.</param>
        /// <param name="searchTerm">Search term.</param>
        /// <returns>Returns the list of matched search locations.</returns>
        [ResponseType(typeof(AddressListResponse))]
        [HttpGet]
        public HttpResponseMessage GetSearchLocation(int portalId, string searchTerm)
        {
            HttpResponseMessage response;
            try
            {
                //Get list of search locations.
                string data = _cache.GetSearchLocation(portalId, searchTerm, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AddressListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AddressListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AddressListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Update search address.
        /// </summary>
        /// <param name="model">AddressModel.</param>
        /// <returns>Returns updated address model.</returns>
        [ResponseType(typeof(WebStoreAccountResponse))]
        [HttpPut, ValidateModel]
        public HttpResponseMessage UpdateSearchAddress([FromBody] AddressModel model)
        {
            HttpResponseMessage response;
            try
            {
                model = _service.UpdateSearchAddress(model);
                response = HelperUtility.IsNotNull(model) ? CreateOKResponse(new WebStoreAccountResponse { AccountAddress = model }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new WebStoreAccountResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WebStoreAccountResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion

        #region Associate Price
        /// <summary>
        /// Associate Price List to Customer.
        /// </summary>
        /// <param name="priceUserModel">PriceUserModel.</param>
        /// <returns>Returns associated price to account if true else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssociatePriceList([FromBody] PriceUserModel priceUserModel)
        {
            HttpResponseMessage response;
            try
            {
                response = _service.AssociatePriceList(priceUserModel) ? CreateOKResponse(new TrueFalseResponse { IsSuccess = true }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// UnAssociate price lists from customer.
        /// </summary>
        /// <param name="priceUserModel">Model contains data to remove.</param>
        /// <returns>Return list if true else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage UnAssociatePriceList([FromBody] PriceUserModel priceUserModel)
        {
            HttpResponseMessage response;
            try
            {
                response = _service.UnAssociatePriceList(priceUserModel) ? CreateOKResponse(new TrueFalseResponse { IsSuccess = true }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// To get associated price list precedence value for customer.
        /// </summary>
        /// <param name="priceUserModel">priceAccountModel contains priceListId and userId to get precedence value.</param>
        /// <returns>PriceListPortalModel.</returns>   
        [ResponseType(typeof(PriceUserResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GetAssociatedPriceListPrecedence([FromBody] PriceUserModel priceUserModel)
        {
            HttpResponseMessage response;
            try
            {
                priceUserModel = _service.GetAssociatedPriceListPrecedence(priceUserModel);
                response = HelperUtility.IsNotNull(priceUserModel) ? CreateOKResponse(new PriceUserResponse { PriceUser = priceUserModel }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PriceUserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PriceUserResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update associated price list precedence value for account.
        /// </summary>
        /// <param name="priceUserModel">PriceUserModel.</param>
        /// <returns>Updated associated price list precedence value.</returns>
        [ResponseType(typeof(PriceUserResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateAssociatedPriceListPrecedence([FromBody] PriceUserModel priceUserModel)
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.UpdateAssociatedPriceListPrecedence(priceUserModel);
                response = data ? CreateCreatedResponse(new PriceUserResponse { PriceUser = priceUserModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PriceUserResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PriceUserResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        #endregion       
        #endregion
    }
}
