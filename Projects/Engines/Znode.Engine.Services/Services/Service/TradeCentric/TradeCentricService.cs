using System.Diagnostics;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Resources;
using System.Linq;
using Znode.Engine.Api.Models.Responses;
using System.Net;
using System;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Znode.Engine.Services
{
    public class TradeCentricService : BaseService, ITradeCentricService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeTradeCentricUser> _tradeCentricUser;
        #endregion

        #region Constructor
        public TradeCentricService()
        {
            _tradeCentricUser = new ZnodeRepository<ZnodeTradeCentricUser>();
        }
        #endregion

        // Get TradeCentric user.
        public virtual TradeCentricUserModel GetTradeCentricUser(int userId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            if (userId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.UserIdNotLessThanOne);

            ZnodeTradeCentricUser tradeCentricUser = _tradeCentricUser.Table.FirstOrDefault(x => x.UserId == userId);

            ZnodeLogging.LogMessage("Executed.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            return tradeCentricUser.ToModel<TradeCentricUserModel>();
        }

        // Save TradeCentric user details.
        public virtual bool SaveTradeCentricUser(TradeCentricUserModel tradeCentricUserModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            bool status = false;

            if (IsNull(tradeCentricUserModel))
                throw new ZnodeException(ErrorCodes.NullModel, WebStore_Resources.TradeCentricUserModelNotNull);

            if (tradeCentricUserModel.UserId < 0)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.UserIdNotLessThanOne);


            if (tradeCentricUserModel?.TradeCentricUserId <= 0 && tradeCentricUserModel.UserId > 0)
            {
                ZnodeTradeCentricUser tradeCentricUser = _tradeCentricUser.Insert(tradeCentricUserModel.ToEntity<ZnodeTradeCentricUser>());
                ZnodeLogging.LogMessage("Insert Successfully.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
                status = tradeCentricUser?.TradeCentricUserId > 0;
            }
            else
            {
                status = _tradeCentricUser.Update(tradeCentricUserModel.ToEntity<ZnodeTradeCentricUser>());
            }

            ZnodeLogging.LogMessage("Executed.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            return status;
        }

        // Transfer Cart.
        public virtual TradeCentricCartTransferResponse TransferTradeCentricCart(TradeCentricUserModel tradeCentricUserModel)
        {
            ZnodeLogging.LogMessage("Execution started:", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            if (IsNull(tradeCentricUserModel))
                throw new ZnodeException(ErrorCodes.NullModel, WebStore_Resources.TradeCentricUserModelNotNull);

            return TransferCart(tradeCentricUserModel);
        }

        //Post data to endpoint.
        protected virtual TradeCentricCartTransferResponse TransferCart(TradeCentricUserModel tradeCentricUserModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Execution started.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
                if (tradeCentricUserModel?.CartModel?.Items?.Count > 0)
                {                 
                    byte[] dataBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(tradeCentricUserModel.CartModel));

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(tradeCentricUserModel.ReturnUrl);
                    request.KeepAlive = false;
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Accept = "*/*";
                    request.ContentLength = dataBytes.Length;
                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(dataBytes, 0, dataBytes.Length);
                    }

                    return GetResponseFromEndpoint(request);

                }
                ZnodeLogging.LogMessage("Executed.", ZnodeConstant.TradeCentricComponent, TraceLevel.Info);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeConstant.TradeCentricComponent, TraceLevel.Error);
            }
            return new TradeCentricCartTransferResponse() { RedirectUrl = string.Empty, HasError = true, ErrorCode = ErrorCodes.InvalidData, ErrorMessage = WebStore_Resources.TradeCentricTransferCartErrorMessage };
        }

        // Get response from endpoint.
        protected virtual TradeCentricCartTransferResponse GetResponseFromEndpoint(HttpWebRequest request)
        {
            using (WebResponse response = request.GetResponse())
            {
                using (Stream body = response.GetResponseStream())
                {
                    using (StreamReader stream = new StreamReader(body))
                    {
                        using (JsonTextReader jsonReader = new JsonTextReader(stream))
                        {
                            JsonSerializer jsonSerializer = new JsonSerializer();
                            try
                            {
                                TradeCentricCartTransferResponse responseTradeCentric = jsonSerializer.Deserialize<TradeCentricCartTransferResponse>(jsonReader);
                                if (IsNotNull(responseTradeCentric))
                                {
                                    responseTradeCentric.HasError = false;
                                    responseTradeCentric.ErrorMessage = "";
                                    ZnodeLogging.LogMessage("TransferCart URL: ", ZnodeConstant.TradeCentricComponent, TraceLevel.Verbose, new { Redirect_Url = responseTradeCentric.RedirectUrl });
                                    return responseTradeCentric;
                                }
                            }
                            catch (JsonReaderException ex)
                            {
                                ZnodeLogging.LogMessage(ex.Message, ZnodeConstant.TradeCentricComponent, TraceLevel.Error);
                                throw new ZnodeException(null, ex.Message);
                            }
                            catch (Exception ex)
                            {
                                ZnodeLogging.LogMessage(ex.Message, ZnodeConstant.TradeCentricComponent, TraceLevel.Error);
                            }
                        }
                    }
                }
            }
            return new TradeCentricCartTransferResponse() { RedirectUrl = string.Empty, HasError = true, ErrorCode = ErrorCodes.InvalidData, ErrorMessage = WebStore_Resources.TradeCentricTransferCartErrorMessage };

        }

    }
}
