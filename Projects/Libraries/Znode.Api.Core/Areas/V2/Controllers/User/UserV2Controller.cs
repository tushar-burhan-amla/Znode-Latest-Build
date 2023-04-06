using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Api.Models.V2;
using Znode.Engine.Api.Models.V2.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class UserV2Controller : BaseController
    {
        #region Private Variables
        private readonly IUserServiceV2 _service;
        #endregion

        #region Public Constructor
        public UserV2Controller(IUserServiceV2 service)
        {
            _service = service;
        }
        #endregion

        #region Public Methods

        [ResponseType(typeof(CreateUserResponseV2))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage CreateUser([FromBody] CreateUserModelV2 model)
        {
            HttpResponseMessage response;
            try
            {
                CreateUserModelV2 accountData = _service.CreateCustomerV2(model);
                
                if (HelperUtility.IsNotNull(accountData))
                {
                    response = CreateCreatedResponse(new CreateUserResponseV2 { User = accountData });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(accountData.UserId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new CreateUserResponseV2 { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new CreateUserResponseV2 { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut, ValidateModel]
        public HttpResponseMessage UpdateUser([FromBody] UpdateUserModelV2 model)
        {
            HttpResponseMessage response;
            try
            {
                bool isRecordUpdated = _service.UpdateUserDataV2(model);

                if (isRecordUpdated)
                {
                    BooleanModel boolModel = new BooleanModel
                    {
                        IsSuccess = isRecordUpdated
                    };

                    TrueFalseResponse resp = new TrueFalseResponse
                    {
                        booleanModel = boolModel,
                        IsSuccess = isRecordUpdated
                    };

                    response = CreateOKResponse(resp);
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message});
            }
            return response;
        }

        [ResponseType(typeof(GuestUserResponseV2))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage CreateGuestCustomerV2([FromBody] GuestUserModelV2 model)
        {
            HttpResponseMessage response;
            try
            {
                GuestUserModelV2 accountData = _service.CreateGuestUserV2(model);

                if (HelperUtility.IsNotNull(accountData))
                {
                    response = CreateCreatedResponse(new GuestUserResponseV2 { GuestUser = accountData });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(accountData.UserId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new CreateUserResponseV2 { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new CreateUserResponseV2 { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        #endregion
    }
}
