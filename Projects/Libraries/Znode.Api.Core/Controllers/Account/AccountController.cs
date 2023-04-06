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
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Controllers
{
    public class AccountController : BaseController
    {
        #region Private readonly Variables
        private readonly IAccountService _service;
        private readonly IAccountCache _cache;
        #endregion

        #region Public Constructor
        public AccountController(IAccountService service, IAccountQuoteService accountQuoteService)
        {
            _service = service;          
            _cache = new AccountCache(_service, accountQuoteService);
        }
        #endregion

        #region Public Methods

        #region Account
        /// <summary>
        /// Get account list.
        /// </summary>        
        /// <returns>Returns account list.</returns>
        [ResponseType(typeof(AccountListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                //Get account list.
                string data = _cache.GetAccountList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AccountListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }

            return response;
        }


        /// <summary>
        /// Gets an account.
        /// </summary>
        /// <param name="accountId">The Id of the Account.</param>
        /// <returns>return Company Account.</returns>
        [ResponseType(typeof(AccountResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAccount(int accountId)
        {
            HttpResponseMessage response;
            try
            {
                //Get account details by account id.
                string data = _cache.GetAccount(accountId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AccountResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AccountDataResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets an account by its username
        /// </summary>
        /// <param name="accountName">Account Name of the account to be fetched</param>
        /// <param name="portalId">PortalId of the account to be fetched</param>
        /// <returns>returns the account details on the basis of account name</returns>
        [ResponseType(typeof(AccountResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAccountByName(string accountName, int portalId = 0)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAccountByName(accountName, portalId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AccountResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AccountResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Creates an account.
        /// </summary>
        /// <param name="model">model with company details and address</param>
        /// <returns></returns>
        [ResponseType(typeof(AccountDataResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Create([FromBody] AccountDataModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create PIM attribute.
                AccountDataModel companyDetails = _service.CreateAccount(model);

                if (HelperUtility.IsNotNull(companyDetails))
                {
                    response = CreateCreatedResponse(new AccountDataResponse { CompanyDetails = companyDetails });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(companyDetails.CompanyAccount.AccountId)));
                }
                else
                    response = CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AccountDataResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountDataResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update accounts information.
        /// </summary>
        /// <param name="model">Model to update in database.</param>
        /// <returns></returns>
        [ResponseType(typeof(AccountDataResponse))]
        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody] AccountDataModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update PIM Attribute.
                bool attribute = _service.Update(model);
                response = attribute ? CreateCreatedResponse(new AccountDataResponse { CompanyDetails = model, ErrorCode = 0 }) : CreateNoContentResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.CompanyAccount.AccountId)));

            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AccountDataResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);

            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountDataResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete Account by id
        /// </summary>
        /// <param name="accountIds">model with multiple account id</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete(ParameterModel accountIds)
        {
            HttpResponseMessage response;
            try
            {
                //Delete Account based on AccountIds..
                bool deleted = _service.Delete(accountIds);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AssociationDeleteError });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets an account.
        /// </summary>
        /// <param name="accountCode">The Code of the Account.</param>
        /// <returns>return Company Account.</returns>
        [ResponseType(typeof(AccountResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAccountByCode(string accountCode)
        {
            HttpResponseMessage response;
            try
            {
                //Get account details by account code.
                string data = _cache.GetAccountByCode(accountCode, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AccountResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AccountResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete Account.
        /// </summary>
        /// <param name="accountCodes"> Add comma separated account Codes in the field of Ids.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteAccountByCode(ParameterModel accountCodes)
        {
            HttpResponseMessage response;

            try
            {
                bool isDeleted = _service.DeleteAccountByCode(accountCodes);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isDeleted, booleanModel = new BooleanModel { IsSuccess = isDeleted, SuccessMessage = Api_Resources.DeleteAccountSuccessMessage } });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion

        #region Account Notes
        /// <summary>
        /// Create Account Note.
        /// </summary>
        /// <param name="model">NoteModel model.</param>
        /// <returns>Returns created model.</returns>
        [ResponseType(typeof(NoteResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage CreateAccountNote([FromBody] NoteModel model)
        {
            HttpResponseMessage response;

            try
            {
                NoteModel note = _service.CreateAccountNote(model);

                if (HelperUtility.IsNotNull(note))
                {
                    response = CreateCreatedResponse(new NoteResponse { Note = note });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(note?.NoteId)));
                }
                else
                    response = CreateNoContentResponse();
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
        /// Get Note by Note id.
        /// </summary>
        /// <param name="noteId">noteId id to get account Note.</param>
        /// <returns>Returns Note model.</returns>
        [ResponseType(typeof(NoteResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAccountNote(int noteId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAccountNote(noteId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<NoteResponse>(data) : CreateNoContentResponse();
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
        /// Update Account Note.
        /// </summary>
        /// <param name="model">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(NoteResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateAccountNote([FromBody] NoteModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool note = _service.UpdateAccountNote(model);
                response = note ? CreateCreatedResponse(new NoteResponse { Note = model, ErrorCode = 0 }) : CreateNoContentResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.NoteId)));
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
        /// Gets list of Account note.
        /// </summary>
        /// <returns>Returns list of Account note.</returns>
        [ResponseType(typeof(NoteListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAccountNotes()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAccountNotes(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<NoteListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new NoteListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete Account Note.
        /// </summary>
        /// <param name="noteId">note Ids.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteAccountNote([FromBody] ParameterModel noteId)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteAccountNote(noteId) });
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
        #endregion

        #region Account Department
        /// <summary>
        /// Create Account Department.
        /// </summary>
        /// <param name="model">AccountDepartmentModel.</param>
        /// <returns>Returns created model.</returns>
        [ResponseType(typeof(AccountDepartmentResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage CreateAccountDepartment([FromBody] AccountDepartmentModel model)
        {
            HttpResponseMessage response;

            try
            {
                AccountDepartmentModel department = _service.CreateAccountDepartment(model);

                if (!Equals(department, null))
                {
                    response = CreateCreatedResponse(new AccountDepartmentResponse { Department = department });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(department.DepartmentId)));
                }
                else
                    response = CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AccountDepartmentResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountDepartmentResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get Department by Department id.
        /// </summary>
        /// <param name="departmentId">Department id to get account department.</param>
        /// <returns>Returns Department model.</returns>
        [ResponseType(typeof(AccountDepartmentResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAccountDepartment(int departmentId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAccountDepartment(departmentId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AccountDepartmentResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AccountDepartmentResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountDepartmentResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update Account Department.
        /// </summary>
        /// <param name="model">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(AccountDepartmentResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateAccountDepartment([FromBody] AccountDepartmentModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool department = _service.UpdateAccountDepartment(model);
                response = department ? CreateCreatedResponse(new AccountDepartmentResponse { Department = model, ErrorCode = 0 }) : CreateNoContentResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.DepartmentId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AccountDepartmentResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountDepartmentResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets list of Account Department.
        /// </summary>
        /// <returns>Returns list of Account Department.</returns>
        [ResponseType(typeof(AccountDepartmentResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAccountDepartments()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAccountDepartments(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AccountDepartmentListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountDepartmentListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete Account Department.
        /// </summary>
        /// <param name="departmentIds">Department Ids.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteAccountDepartment([FromBody] ParameterModel departmentIds)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteAccountDepartment(departmentIds) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AccountDepartmentResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountDepartmentResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion

        #region Address
        /// <summary>
        /// Gets the address list for account Id.
        /// </summary>
        /// <returns>Returns Address list.</returns>
        [ResponseType(typeof(AccountListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage AddressList()
        {
            HttpResponseMessage response;
            try
            {
                //Get account list.
                string data = _cache.GetAddressList(RouteUri, RouteTemplate);
                // TODO: Add info logs
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AccountListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Create Account Address.
        /// </summary>
        /// <param name="model">AddressModel model.</param>
        /// <returns>Returns created model.</returns>
        [ResponseType(typeof(AccountResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage CreateAccountAddress([FromBody] AddressModel model)
        {
            HttpResponseMessage response;

            try
            {
                AddressModel address = _service.CreateAccountAddress(model);

                if (HelperUtility.IsNotNull(address))
                {
                    response = CreateCreatedResponse(new AccountResponse { AccountAddress = address });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(address?.AccountAddressId)));
                }
                else
                    response = CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AccountResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get account address by filters.
        /// </summary>       
        /// <returns>Returns account's Address.</returns>
        [ResponseType(typeof(AccountResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAccountAddress()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAccountAddress(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AccountResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new AccountResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update Account address.
        /// </summary>
        /// <param name="model">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(AccountResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateAccountAddress([FromBody] AddressModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool note = _service.UpdateAccountAddress(model);
                response = note ? CreateCreatedResponse(new AccountResponse { AccountAddress = model, ErrorCode = 0 }) : CreateNoContentResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.AccountAddressId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new AccountResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AccountResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete Account address.
        /// </summary>
        /// <param name="accountAddressId">account address Ids.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteAccountAddress([FromBody] ParameterModel accountAddressId)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteAccountAddress(accountAddressId) });
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
        #endregion

        #region Associate Price
        /// <summary>
        /// Associate Price List to Account.
        /// </summary>
        /// <param name="priceAccountModel">PriceAccountModel.</param>
        /// <returns>Returns associated price to account.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssociatePriceList([FromBody] PriceAccountModel priceAccountModel)
        {
            HttpResponseMessage response;
            try
            {
                response = _service.AssociatePriceList(priceAccountModel) ? CreateOKResponse(new TrueFalseResponse { IsSuccess = true }) : CreateInternalServerErrorResponse();
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
        /// UnAssociate price lists from account.
        /// </summary>
        /// <param name="priceAccountModel">Model contains data to remove.</param>
        /// <returns>Returns unassociate price list.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage UnAssociatePriceList([FromBody] PriceAccountModel priceAccountModel)
        {
            HttpResponseMessage response;
            try
            {
                response = _service.UnAssociatePriceList(priceAccountModel) ? CreateOKResponse(new TrueFalseResponse { IsSuccess = true }) : CreateInternalServerErrorResponse();
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
        /// To get associated price list precedence value for account.
        /// </summary>
        /// <param name="priceAccountModel">priceAccountModel contains priceListId and accountId to get precedence value.</param>
        /// <returns>PriceListPortalModel.</returns>   
        [ResponseType(typeof(PriceAccountResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GetAssociatedPriceListPrecedence([FromBody] PriceAccountModel priceAccountModel)
        {
            HttpResponseMessage response;
            try
            {
                priceAccountModel = _service.GetAssociatedPriceListPrecedence(priceAccountModel);
                response = HelperUtility.IsNotNull(priceAccountModel) ? CreateOKResponse(new PriceAccountResponse { PriceAccount = priceAccountModel }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PriceAccountResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PriceAccountResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update associated price list precedence value for account.
        /// </summary>
        /// <param name="priceAccountModel">PriceAccountModel.</param>
        /// <returns>Updated associated price list precedence value.</returns>
        [ResponseType(typeof(PriceAccountResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateAssociatedPriceListPrecedence([FromBody] PriceAccountModel priceAccountModel)
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.UpdateAssociatedPriceListPrecedence(priceAccountModel);
                response = data ? CreateCreatedResponse(new PriceAccountResponse { PriceAccount = priceAccountModel }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PriceAccountResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PriceAccountResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        #endregion

        #region Account Order
        /// <summary>
        /// Get the list of all Orders of account.
        /// </summary>
        /// <param name="accountId">The Id of the Account.</param>
        /// <returns>Returns account user order.</returns>
        [ResponseType(typeof(OrderListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAccountUserOrderList(int accountId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAccountUserOrderList(accountId, RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<OrderListResponse>(data);
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new OrderListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion

        #region Account Profile

        /// <summary>
        /// Gets associated/unassociated profiles for account .
        /// </summary>
        /// <returns>Returns list of associated/unassociated profile.</returns>
        [ResponseType(typeof(ProfileListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssociatedUnAssociatedProfile()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAssociatedUnAssociatedProfile(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProfileListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ProfileListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }

            return response;
        }

        /// <summary>
        /// Associate profiles to Account.
        /// </summary>
        /// <param name="profileModel">ProfileModel.</param>
        /// <returns>Returns associated profile to account if true else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssociateProfile([FromBody] ProfileModel profileModel)
        {
            HttpResponseMessage response;
            try
            {
                response = _service.AssociateProfile(profileModel) ? CreateOKResponse(new TrueFalseResponse { IsSuccess = true }) : CreateInternalServerErrorResponse();
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
        /// UnAssociate profiles from account.
        /// </summary>
        /// <param name="profileModel">AccountProfileModel contains data to remove.</param>
        /// <returns>Returns unassociate profile if true else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage UnAssociateProfile([FromBody] AccountProfileModel profileModel)
        {
            HttpResponseMessage response;
            try
            {
                response = _service.UnAssociateProfile(profileModel) ? CreateOKResponse(new TrueFalseResponse { IsSuccess = true }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
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
        /// Get user approver list.
        /// </summary>        
        /// <returns>Returns user approver list.</returns>
        [ResponseType(typeof(UserApproverListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetApproverLevelList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUserApproverList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<UserApproverListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new UserApproverListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual HttpResponseMessage Getlevelslist()
        {
            HttpResponseMessage response;
            try
            {
                //Get level list.
                string data = _cache.Getlevelslist(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ApproverLevelListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ApproverLevelListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Create/Edit approver level
        /// </summary>
        /// <param name="userApprover">UserApproverModel</param>
        /// <returns>UserApproverResponse</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CreateApproverLevel([FromBody]UserApproverModel userApprover)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _service.CreateApproverLevel(userApprover), ErrorCode = 0 });
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
        /// Save Permission Setting
        /// </summary>
        /// <param name="permissionCodeModel">PermissionCodeModel</param>
        /// <returns>PermissionCodeResponse</returns>
        [ResponseType(typeof(PermissionCodeResponse))]
        [HttpPost]
        public virtual HttpResponseMessage SavePermissionSetting([FromBody]PermissionCodeModel permissionCodeModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isSuccess = _service.SavePermissionSetting(permissionCodeModel);
                response = isSuccess ? CreateCreatedResponse(new PermissionCodeResponse { PermissionCode = permissionCodeModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new UserApproverResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new UserApproverResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete User Approver.
        /// </summary>
        /// <param name="userApproverId">User Approver Level Id</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteApproverLevel(ParameterModel userApproverId)
        {
            HttpResponseMessage response;
            try
            {                
                bool deleted = _service.DeleteApproverLevel(userApproverId);
                response = deleted ? CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion

        /// <summary>
        /// Get parent account list
        /// </summary>        
        /// <returns>Returns parent account list.</returns>
        [ResponseType(typeof(ParentAccountListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetParentAccountList()
        {
            HttpResponseMessage response;

            try
            {
                //Get parent account list.
                string data = _cache.GetParentAccountList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ParentAccountListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ParentAccountListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion
    }
}
