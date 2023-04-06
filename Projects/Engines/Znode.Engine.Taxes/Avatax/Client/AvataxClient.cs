using System;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Avalara.AvaTax.RestClient;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using System.Diagnostics;
using Znode.Libraries.Resources;
using System.Linq;

namespace Znode.Engine.Taxes
{
    public class AvataxClient : ZnodeTaxesType, IAvataxClient
    {
        IAvataxHelper avataxHelper;
        private AvataxSettings avataxSettings;

        public AvataxClient()
        {
            avataxHelper = GetService<IAvataxHelper>();
        }

        //Calculate Tax
        public override void Calculate()
        {
            decimal taxSalesTax = RESTGetTaxRates();
        }

        // Get Tax for Avalara.
        public string RESTTestConnection(TaxPortalModel taxportalModel)
        {
            string result = string.Empty;
            try
            {
                IAvataxMapper avataxMapper = GetService<IAvataxMapper>(new ZnodeNamedParameter("portalId", taxportalModel.PortalId));
                avataxSettings = avataxMapper.GetAvataxSetting(taxportalModel);

                HttpRequestMessage request = new HttpRequestMessage();
                request = SetRequestHeaders();
                request.RequestUri = new Uri(ZnodeApiSettings.AvalaraRestRootUri + "/utilities/ping");

                PingResultModel pingResultModel = GetResourceFromEndpoint<PingResultModel>(request);

                if (pingResultModel.authenticated.GetValueOrDefault())
                    result = Api_Resources.SuccessAvataxConnection;
            }
            catch (Exception ex)
            {
                result = Api_Resources.ErrorAvataxConnection + ex.Message;                
            }
            return result;
        }

        // Get Tax Rates from Avalara.
        public decimal RESTGetTaxRates(bool isPostSubmitCall = false)
        {
            IAvataxMapper avataxMapper = GetService<IAvataxMapper>(new ZnodeNamedParameter("portalId", ShoppingCart.PortalId.GetValueOrDefault()));
            ShoppingCart.AvataxIsSellerImporterOfRecord = IsSellerImporterOfRecord(ShoppingCart.OrderId);

            CreateOrAdjustTransactionModel createOrAdjustTransactionModel = new CreateOrAdjustTransactionModel()
            {
                createTransactionModel = avataxMapper.SetTransationModel(ShoppingCart, TaxBag.ShippingTaxInd, isPostSubmitCall)
            };

            //send the sales order to Avatax
            TransactionModel taxRes = ReturnRequest(createOrAdjustTransactionModel);
            
            if (ShoppingCart.IsTaxExempt && HelperUtility.IsNotNull(taxRes?.summary))
            {
                foreach (var tax in taxRes?.summary)
                {
                    tax.tax = 0;
                    tax.rate = 0;
                }
            }

            if (!ShoppingCart.IsPricesInclusiveOfTaxes.GetValueOrDefault())
            {
                avataxMapper.MapSummaryDetailsInShoppingCart(ShoppingCart, taxRes);
                avataxMapper.MapMessageDetailsInShoppingCart(ShoppingCart, taxRes);
                avataxMapper.MapDetailsInShoppingCart(ShoppingCart, taxRes, TaxBag.TaxRuleId);
            }
            return taxRes.totalTax.GetValueOrDefault();
        }

        // Process anything that must be done after the order is submitted.
        public override void PostSubmitOrderProcess(bool isTaxCostUpdated = true)
            => RESTGetTaxRates(isTaxCostUpdated);

        // Return Current Shopping Cart Items.
        public TransactionModel ReturnRequest(CreateOrAdjustTransactionModel avalaraTaxRequest)
        {
            TransactionModel transactionModel = null;

            try
            {
                HttpRequestMessage request = new HttpRequestMessage();
                request = SetRequestHeaders();
                request.RequestUri = new Uri(ZnodeApiSettings.AvalaraRestRootUri + "/transactions/createoradjust");

               transactionModel = PostResourceFromEndpoint<TransactionModel, CreateOrAdjustTransactionModel>(request, avalaraTaxRequest);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Avalara.ToString(), TraceLevel.Error);
            }

            return transactionModel;
        }

        // Cancel Current Order.
        public override void CancelOrderRequest(ShoppingCartModel shoppingCartModel)
        {
            try
            {
                IAvataxMapper avataxMapper = GetService<IAvataxMapper>(new ZnodeNamedParameter("portalId", shoppingCartModel.PortalId));

                AuditTransactionModel auditTransactionModel = AuditTransaction(shoppingCartModel.OmsOrderId.GetValueOrDefault());
                CreateTransactionModel avalaraTaxRequest = new CreateTransactionModel();

                avataxMapper.BindCancelLineItemDetail(ShoppingCart, auditTransactionModel, avalaraTaxRequest);

                if (avalaraTaxRequest?.lines?.Count > 0)
                {
                    avalaraTaxRequest.isSellerImporterOfRecord = IsSellerImporterOfRecord(ShoppingCart.OrderId);
                    avataxMapper.SetCancelTaxRequest(ShoppingCart, avalaraTaxRequest);
                    CreateOrAdjustTransactionModel createOrAdjustTransactionModel = new CreateOrAdjustTransactionModel()
                    {
                        createTransactionModel = avalaraTaxRequest
                    };
                    
                    ReturnRequest(createOrAdjustTransactionModel);
                }               
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Avalara.ToString(), TraceLevel.Error);
            }
        }


        //Set Value For IsSellerImporterOfRecord
        protected virtual bool? IsSellerImporterOfRecord(int? omsOrderId)
        {
            bool? isSellerImporterOfRecord = null;
            if (HelperUtility.IsNotNull(omsOrderId) && omsOrderId > 0)
            {
                isSellerImporterOfRecord = avataxHelper.IsSellerImporterOfDuty(omsOrderId.GetValueOrDefault());
                if (HelperUtility.IsNotNull(isSellerImporterOfRecord))
                    return isSellerImporterOfRecord;
            }
            if (HelperUtility.IsNull(isSellerImporterOfRecord))
            {
                AuditTransactionModel auditTransactionModel = AuditTransaction(omsOrderId.GetValueOrDefault());
                isSellerImporterOfRecord = auditTransactionModel?.original?.response?.isSellerImporterOfRecord;
            }
            if (HelperUtility.IsNotNull(omsOrderId) && omsOrderId > 0 && HelperUtility.IsNotNull(isSellerImporterOfRecord))
            {
                avataxHelper.UpdateIsSellerImporterOfDuty(omsOrderId.GetValueOrDefault(), isSellerImporterOfRecord.GetValueOrDefault());
            }
                return isSellerImporterOfRecord;
        }

        //Check Transaction Details In Avalara.
        public virtual AuditTransactionModel AuditTransaction(int omsOrderId)
        {
            AuditTransactionModel transactionModel = null;
            try
            {
                IAvataxMapper avataxMapper = GetService<IAvataxMapper>(new ZnodeNamedParameter("portalId", ShoppingCart.PortalId.GetValueOrDefault()));

                string orderNumber = avataxHelper.GetOrderNumber(omsOrderId);

                HttpRequestMessage request = new HttpRequestMessage();
                request = SetRequestHeaders();
                request.RequestUri = new Uri(ZnodeApiSettings.AvalaraRestRootUri + "/companies/" + avataxSettings.AvalaraCompanyCode + "/transactions/" + orderNumber + "/audit");
                transactionModel = GetResourceFromEndpoint<AuditTransactionModel>(request);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Avalara.ToString(), TraceLevel.Error);
            }
            return transactionModel;
        }

        //Return order line item.
        public override void ReturnOrderLineItem(ShoppingCartModel orderModel)
        {
            IAvataxMapper avataxMapper = GetService<IAvataxMapper>(new ZnodeNamedParameter("portalId", ShoppingCart.PortalId.GetValueOrDefault()));

            avataxSettings = avataxHelper.GetAvataxSetting(ZnodeConfigManager.SiteConfig.PortalId);

            if (orderModel?.ReturnItemList?.Count > 0)
            {
                try
                {
                    AuditTransactionModel auditTransactionModel = AuditTransaction(orderModel.OmsOrderId.GetValueOrDefault());
                    CreateTransactionModel avalaraTaxRequest = new CreateTransactionModel();
                    
                    avataxMapper.BindReturnLineItem(ShoppingCart, orderModel, auditTransactionModel, avalaraTaxRequest);
                    if (avalaraTaxRequest?.lines?.Count > 0)
                    {
                        avalaraTaxRequest.isSellerImporterOfRecord = IsSellerImporterOfRecord(ShoppingCart.OrderId);
                        avataxMapper.SetReturnTaxRequest(ShoppingCart, avalaraTaxRequest);
                        CreateOrAdjustTransactionModel createOrAdjustTransactionModel = new CreateOrAdjustTransactionModel()
                        {
                            createTransactionModel = avalaraTaxRequest
                        };

                        ReturnRequest(createOrAdjustTransactionModel);
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Avalara.ToString(), TraceLevel.Error);
                }
            }
        }

        //Get authorization headers for avalara.
        public string GetAuthorizationHeader()
        {
            if (HelperUtility.IsNull(avataxSettings))
                avataxSettings = avataxHelper.GetAvataxSetting(ZnodeConfigManager.SiteConfig.PortalId);

            var client = String.Format("{0}:{1}", avataxSettings.AvalaraAccount, avataxSettings.AvalaraLicense);
            var bytes = Encoding.UTF8.GetBytes(client);
            string authorization = Convert.ToBase64String(bytes);

            return authorization;
        }

        //Set Avalara request Headers.
        public HttpRequestMessage SetRequestHeaders()
        {
            HttpRequestMessage request = new HttpRequestMessage();
            string authorization = GetAuthorizationHeader();

            request.Headers.Add("Authorization", "Basic " + authorization);
            request.Headers.Add("X-Avalara-Client", GetAvalaraClientHeader());

            return request;
        }

        // To get value for X-Avalara-Client header.
        protected virtual string GetAvalaraClientHeader()
        {
            string version = GetTaxesLibraryVersion();

            return string.Format("{0}; {1}; {2}; {3}; {4}",
                ZnodeApiSettings.AvalaraAppName, version, ZnodeConstant.TaxesLibraryName, version, Environment.MachineName);
        }

        // To get the version info.
        protected virtual string GetTaxesLibraryVersion()
        {
            string assemblyVersion = "";
            try
            {
                var _assembly = AppDomain.CurrentDomain.GetAssemblies().
                           SingleOrDefault(assembly => assembly.GetName().Name == ZnodeConstant.TaxesLibraryName);

                assemblyVersion = FileVersionInfo.GetVersionInfo(_assembly.Location).FileVersion;
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Avalara.ToString(), TraceLevel.Error);
            }

            return string.IsNullOrEmpty(assemblyVersion) ? ZnodeApiSettings.BuildVersion : assemblyVersion;
        }

        //Get Resources from avalara endpoints.
        public T GetResourceFromEndpoint<T>(HttpRequestMessage request)
        {
            request.Method = new HttpMethod("GET");
            T tempModel;
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.SendAsync(request).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        tempModel = JsonConvert.DeserializeObject<T>(content.ReadAsStringAsync().Result);
                    }
                }
            }
            return tempModel;
        }

        //Post Resources to avalara endpoints.
        public T PostResourceFromEndpoint<T, P>(HttpRequestMessage request, P requestTemp)
        {
            request.Method = new HttpMethod("POST");
            request.Content = new StringContent(HelperUtility.ToJSON(requestTemp).ToString());
            T tempModel;
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.SendAsync(request).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        tempModel = JsonConvert.DeserializeObject<T>(content.ReadAsStringAsync().Result);
                    }
                }
            }
            return tempModel;
        }
    }
}