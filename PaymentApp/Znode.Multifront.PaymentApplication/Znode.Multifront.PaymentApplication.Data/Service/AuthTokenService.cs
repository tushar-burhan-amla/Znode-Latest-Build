using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Multifront.PaymentApplication.Data.Interface;
using Znode.Multifront.PaymentApplication.Data.Repository;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;
using Znode.Multifront.PaymentApplication.Models.Models;

namespace Znode.Multifront.PaymentApplication.Data.Service
{
    public class AuthTokenService : BaseService
    {
        #region Create auth token
        public virtual string CreateToken(string userOrSessionId, bool fromAdminApp)
        {
            try
            {
                if (!String.IsNullOrEmpty(userOrSessionId))
                {
                    IZnodePaymentViewRepository<ZnodeAuthTokenModel> objStoredProc = new ZnodePaymentViewRepository<ZnodeAuthTokenModel>();
                    objStoredProc.SetParameter("@UserOrSessionId", userOrSessionId, ParameterDirection.Input, DbType.String);
                    objStoredProc.SetParameter("@IsFromAdminApp", fromAdminApp, ParameterDirection.Input, DbType.Boolean);
                    IList<ZnodeAuthTokenModel> tokenModels = objStoredProc.ExecuteStoredProcedureList("Znode_InsertAuthToken @UserOrSessionId,@IsFromAdminApp");
                    return Encrypt(tokenModels.FirstOrDefault().AuthToken);
                }
                else
                {
                    Logging.LogMessage("Method :CreateToken  Message: Incorrect UserOrSessionId failed to create token: UserOrSessionId :" + userOrSessionId + " and FromAdminApp:"+ fromAdminApp.ToString(), Logging.Components.Payment.ToString(), TraceLevel.Error);
                    return String.Empty;
                }
            }
            catch (Exception exception)
            {
                Logging.LogMessage("Method :CreateToken  Message: Failed to create token: UserOrSessionId :" + userOrSessionId + " and FromAdminApp:" + fromAdminApp.ToString() +"  Error: " + exception.Message, Logging.Components.Payment.ToString(), TraceLevel.Error);
                throw;
            }
        }
        #endregion

        #region check valid Auth Token
        public virtual bool ValidateAuthToken(string token, bool DoNotCount = false)
        {
            try
            {
                IZnodePaymentViewRepository<ZnodeAuthTokenModel> objStoredProc = new ZnodePaymentViewRepository<ZnodeAuthTokenModel>();
                objStoredProc.SetParameter("@AuthToken",Decrypt(token), ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@DoNotCount", DoNotCount, ParameterDirection.Input, DbType.Boolean);
                objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Int32);
                int status = 0;
                objStoredProc.ExecuteStoredProcedureList("Znode_ValidateAuthToken @AuthToken,@DoNotCount,  @Status OUT", 2, out status);

                return status.Equals(1);
            }
            catch (Exception exception)
            {
                Logging.LogMessage("Method :ValidateAuthToken  Message: Token validation failed Error: " + exception.Message, Logging.Components.Payment.ToString(), TraceLevel.Error);
                throw;
            }

        }
        #endregion

        #region Delete used auth token
        public virtual bool DeleteAuthToken(string token)
        {
            try
            {
                IZnodePaymentViewRepository<ZnodeAuthTokenModel> objStoredProc = new ZnodePaymentViewRepository<ZnodeAuthTokenModel>();
                objStoredProc.SetParameter("@AuthToken", token, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Int32);
                int status = 0;
                objStoredProc.ExecuteStoredProcedureList("Znode_DeleteAuthToken @AuthToken,  @Status OUT", 1, out status);

                return status.Equals(1);
            }
            catch (Exception ex)
            {
                Logging.LogMessage("Method :DeleteAuthToken  Message: Token Deletion failed  Error: " + ex.Message, Logging.Components.Payment.ToString(), TraceLevel.Error);
                return false;
            }
            
        }
        #endregion

        #region Delete old used auth token
        public virtual bool DeleteExpiredAuthToken()
        {
            try
            {
                IZnodePaymentViewRepository<BooleanModel> objStoredProc = new ZnodePaymentViewRepository<BooleanModel>();
                objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Int32);
                int status = 0;
                objStoredProc.ExecuteStoredProcedureList("Znode_DeleteOldAuthTokens @Status OUT", 0, out status);
                return status.Equals(1);
            }
            catch (Exception ex)
            {
                Logging.LogMessage("Method :DeleteExpiredAuthToken  Message: Token Deletion failed  Error: " + ex.Message, Logging.Components.Payment.ToString(), TraceLevel.Error);
                return false;
            }

        }
        #endregion
    }
}
