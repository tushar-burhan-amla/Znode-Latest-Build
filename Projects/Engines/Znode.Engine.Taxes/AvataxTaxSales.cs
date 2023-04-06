using Avalara.AvaTax.Adapter;
using Avalara.AvaTax.Adapter.AddressService;
using Avalara.AvaTax.Adapter.TaxService;
using System;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Taxes.Helper;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Taxes
{
    /// <summary>
    /// Provides tax calculations through Avatax tax service.
    /// </summary>
	public class AvataxTaxSales : ZnodeTaxesType
    {
        #region Private members
        private AvataxSettings avaSettings;
        #endregion

        #region Constructors
        public AvataxTaxSales()
        {
            Name = "Avatax Sales Tax";
            Description = "Avatax tax connector for sales tax.";
        }
        #endregion

        #region IZnodeTaxTypes Methods
        /// <summary>
        /// Calculate this Sale Tax and update the Shopping Cart with the value in the appropriate place.
        /// </summary>
        public override void Calculate()
        {
            decimal taxSalesTax = GetTaxRates();
            ShoppingCart.SalesTax = taxSalesTax > 0 ? taxSalesTax : 0m;
        } //end calculate

        // Process anything that must be done after the order is submitted.
        public override void PostSubmitOrderProcess(bool isTaxCostUpdated = true)
            => GetTaxRates(true);

        public override void CancelOrderRequest(ShoppingCartModel shoppingCartModel)
        {
            try
            {
                SetAvataxSettings(shoppingCartModel.PortalId);
                TaxSvc taxSv = CreateTaxService();
                string orderNumber = GetOrderNumber(ShoppingCart.OrderId.GetValueOrDefault());
                CancelTaxRequest cancelRequest = new CancelTaxRequest();
                cancelRequest.DocCode = orderNumber;
                cancelRequest.CancelCode = CancelCode.DocVoided;
                cancelRequest.CompanyCode = avaSettings.AvalaraCompanyCode;
                cancelRequest.DocType = DocumentType.SalesInvoice;
                LogErrorMessage(ZnodeTaxHelper.GetLogFor(cancelRequest));
                CancelTaxResult cancelResponce = taxSv.CancelTax(cancelRequest);
                LogErrorMessage(cancelResponce.Messages);
                LogErrorMessage(ZnodeTaxHelper.GetLogFor(cancelResponce));
            }
            catch (Exception ex)
            {
                LogErrorMessage(ex.Message);
            }
        }

        //Return order line item.
        public override void ReturnOrderLineItem(ShoppingCartModel orderModel)
        {
            if (orderModel?.ReturnItemList?.Count > 0)
            {
                try
                {
                    SetAvataxSettings(orderModel.PortalId);
                    TaxSvc taxSv = CreateTaxService();
                    GetTaxHistoryRequest taxReq = new GetTaxHistoryRequest();
                    taxReq.DocCode = GetOrderNumber(orderModel.OmsOrderId ?? 0);
                    taxReq.CompanyCode = avaSettings.AvalaraCompanyCode;
                    taxReq.DetailLevel = DetailLevel.Line;
                    taxReq.DocType = DocumentType.SalesInvoice;

                    GetTaxHistoryResult taxRes = GetHistory(taxSv, taxReq);

                    GetTaxRequest avalaraTaxRequest = new GetTaxRequest();

                    BindReturnLineItem(orderModel, taxRes, avalaraTaxRequest);
                    if (avalaraTaxRequest?.Lines?.Count > 0)
                    {
                        SetReturnTaxRequest(orderModel, avalaraTaxRequest);

                        ReturnRequest(taxSv, avalaraTaxRequest);
                    }
                }
                catch (Exception ex)
                {
                    LogErrorMessage(ex.Message);
                }
            }
        }

        #endregion

        #region Calls to Avalara
        public virtual string TestConnection(TaxPortalModel taxportalModel)
        {
            string result = string.Empty;
            avaSettings = new AvataxSettings();
            avaSettings.AvalaraAccount = taxportalModel.AvalaraAccount;
            avaSettings.AvalaraCompanyCode = taxportalModel.AvalaraCompanyCode;
            avaSettings.AvalaraFreightIdentifier = taxportalModel.AvalaraFreightIdentifier;
            avaSettings.AvalaraLicense = taxportalModel.AvalaraLicense;
            avaSettings.AvalaraServiceURL = taxportalModel.AvataxUrl;
            try
            {
                //setup account info
                TaxSvc taxSv = CreateTaxService();
                PingResult taxRes = taxSv.Ping("");
                if (taxRes.ResultCode == SeverityLevel.Error)
                    result = "Connection Error: " + taxRes.Messages[0].Summary;
                else
                {
                    IsAuthorizedResult isAuthorized = taxSv.IsAuthorized("GetTax, PostTax, CommitTax, CancelTax, AdjustTax, GetTaxHistory, ReconcileTaxHistory");
                    if (isAuthorized.ResultCode == SeverityLevel.Error)
                        result = "Authorization Error: " + isAuthorized.Messages[0].Summary;
                    else
                        result = "Connection success! " + isAuthorized.ResultCode + " " + "#Messages: " + isAuthorized.Messages.Count.ToString() + " " + "Expires: " + isAuthorized.Expires.ToShortDateString() + " " + "Operations: " + isAuthorized.Operations;
                }
                LogErrorMessage(result);
            }
            catch (Exception ex)
            {
                result = "Avatax Connector error: " + ex.Message;
                // Log Activity
                LogErrorMessage(result);
            }
            return result;
        }

        public virtual decimal GetTaxRates(bool isPostSubmitCall = false)
        {
            try
            {
                SetAvataxSettings();
                TaxSvc taxSv = CreateTaxService();

                //get the current store information
                ZnodePortal portal = GetPortalEntity(ZnodeConfigManager.SiteConfig.PortalId);

                Address orgAdr = GetOriginAddress();

                GetTaxRequest taxReq = BuildTaxRequest(orgAdr, DocumentType.SalesOrder, portal, isPostSubmitCall);

                taxReq.DestinationAddress = GetDestinationAddress(); //Znode does not support multiship at this time

                //these are the cart items and add-ons
                taxReq = SetTaxLines(taxReq, isPostSubmitCall);
              
                BindShippingLineItem(taxReq);

                //send the sales order to Avatax
                GetTaxResult taxRes = ReturnRequest(taxSv, taxReq);

                LogErrorMessage(taxRes.Messages);

                //set the shopping cart line item taxes
                int i = 0;
                // subtract one for the shipping item
                ShoppingCart.SalesTax += taxRes.TotalTax;

                decimal lineItemTax = 0;
                foreach (ZnodeShoppingCartItem cartItem in ShoppingCart.ShoppingCartItems)
                {
                    if(HelperUtility.IsNotNull(cartItem))
                    {
                        int? configurableProductCount = cartItem.Product?.ZNodeConfigurableProductCollection?.Count;
                        int? groupProductCount = cartItem.Product?.ZNodeGroupProductCollection?.Count;
                        cartItem.IsTaxCalculated = true;
                        cartItem.Product.SalesTax = taxRes.TaxLines[i].Tax;
                        if (configurableProductCount > 0)
                        {
                            cartItem.Product.SalesTax = 0;
                            foreach (ZnodeProductBaseEntity productItem in cartItem.Product.ZNodeConfigurableProductCollection)
                            {
                                productItem.SalesTax = taxRes.TaxLines[i].Tax;
                            }
                        }
                        if (groupProductCount > 0)
                        {
                            cartItem.Product.SalesTax = 0;
                            foreach (ZnodeProductBaseEntity productItem in cartItem.Product.ZNodeGroupProductCollection)
                            {
                                productItem.SalesTax = taxRes.TaxLines[i].Tax;
                            }
                        }                       
                        cartItem.TaxRuleId = this.TaxBag.TaxRuleId;
                        lineItemTax += taxRes.TaxLines[i].Tax;
                        i++;
                    }
                }                
                ShoppingCart.TaxOnShipping = taxRes.TotalTax - lineItemTax;
                if(taxRes?.TaxLines.Count > 0)
                   ShoppingCart.TaxRate = Convert.ToDecimal(taxRes.TaxLines[0].Rate) * 100;
                return taxRes.TotalTax;
            }
            catch (Exception ex)
            {
                LogErrorMessage(ex.Message);
                return System.Decimal.Zero;
            }
        }

        public virtual GetTaxResult ReturnRequest(TaxSvc taxSv, GetTaxRequest avalaraTaxRequest)
        {
            //Log the request.
            LogErrorMessage(ZnodeTaxHelper.GetLogFor(avalaraTaxRequest));
            //send the sales order to Avatax
            GetTaxResult returedResponce = taxSv.GetTax(avalaraTaxRequest);
            //Log the responce.
            LogErrorMessage(ZnodeTaxHelper.GetLogFor(avalaraTaxRequest));

            LogErrorMessage(returedResponce.Messages);

            return returedResponce;
        }

        public virtual GetTaxHistoryResult GetHistory(TaxSvc taxSv, GetTaxHistoryRequest taxReq)
        {
            //Log the request.
            LogErrorMessage(ZnodeTaxHelper.GetLogFor(taxReq));
            //send the sales order to Avatax
            GetTaxHistoryResult taxRes = taxSv.GetTaxHistory(taxReq);
            //Log the responce.
            LogErrorMessage(ZnodeTaxHelper.GetLogFor(taxRes));

            LogErrorMessage(taxRes.Messages);

            return taxRes;
        }
        #endregion

        #region Helper Methods
        public virtual GetTaxRequest BuildTaxRequest(Address originAddress, DocumentType docType, ZnodePortal currentStore, bool isPostSubmitCall)
        {
            DateTime createdDate = DateTime.Today;
            GetTaxRequest avalaraTaxRequest = new GetTaxRequest();
            avalaraTaxRequest.OriginAddress = originAddress;
            avalaraTaxRequest.CompanyCode = avaSettings.AvalaraCompanyCode;
            avalaraTaxRequest.DetailLevel = DetailLevel.Tax;
            avalaraTaxRequest.DocCode = "DocDate" + System.DateTime.Now.ToString();
            avalaraTaxRequest.DocDate = createdDate;
            string orderNumber = string.Empty;
            string orderState = string.Empty;
            if (ShoppingCart.OrderId > 0)
            {
                avalaraTaxRequest.DocType = DocumentType.SalesInvoice;
                orderState = GetOrderState(ShoppingCart.OrderId.GetValueOrDefault(), out orderNumber, out createdDate);

                avalaraTaxRequest.DocCode = orderNumber;
                avalaraTaxRequest.DocDate = createdDate;
                avalaraTaxRequest.TaxOverride.TaxOverrideType = TaxOverrideType.TaxDate;
                avalaraTaxRequest.TaxOverride.TaxAmount = System.Decimal.Zero;
                avalaraTaxRequest.TaxOverride.TaxDate = createdDate;
                avalaraTaxRequest.TaxOverride.Reason = ZnodeConstant.OrderModified;
            }

            if (ShoppingCart.OrderId > 0 && isPostSubmitCall)
            {             
                //If the order has been shipped then, commit the request in avalara portal.
                if (string.Equals(orderState, ZnodeOrderStatusEnum.SHIPPED.ToString(), StringComparison.InvariantCultureIgnoreCase) && !ZnodeApiSettings.DisableTaxCommit && isPostSubmitCall)               
                    avalaraTaxRequest.Commit = true;                  
            }
            else
                avalaraTaxRequest.DocType = docType;

            avalaraTaxRequest.CurrencyCode = "USD";
            avalaraTaxRequest.Discount = System.Decimal.Zero;

            //get customer profile to determine if tax exempt           
            avalaraTaxRequest.ExemptionNo = (IsTaxExempt(avalaraTaxRequest) || ShoppingCart.CustomTaxCost == 0) ? ZnodeApiSettings.ExemptionNo ?? "EXEMPT" : "";

            //get the currency code
            if (currentStore?.PortalId > 0)
            {
                avalaraTaxRequest.CurrencyCode = GetCurrencyCode(currentStore.PortalId); //ISO code
                //default to USD, must have something
                if (string.IsNullOrEmpty(avalaraTaxRequest.CurrencyCode))
                    avalaraTaxRequest.CurrencyCode = "USD";
            }
            else
                //default to USD, must have something
                avalaraTaxRequest.TaxOverride.TaxOverrideType = TaxOverrideType.None;

            return avalaraTaxRequest;
        }

        public virtual Address GetOriginAddress()
        {
            //get the address of the current portal
            Address orgAddress = new Address();
            IZnodeTaxHelper taxHelper = GetService<IZnodeTaxHelper>();
            AddressModel portalWareHouseAddressModel = taxHelper.GetPortalShippingAddress(ShoppingCart.PortalId.GetValueOrDefault());
            if (!Equals(portalWareHouseAddressModel, null))
            {
                orgAddress.Line1 = portalWareHouseAddressModel.Address1;
                orgAddress.Line2 = portalWareHouseAddressModel.Address2;
                orgAddress.City = portalWareHouseAddressModel.CityName;
                orgAddress.Region = portalWareHouseAddressModel.StateCode;
                orgAddress.PostalCode = portalWareHouseAddressModel.PostalCode;
                orgAddress.Country = portalWareHouseAddressModel.CountryName;
            }
            return orgAddress;
        }

        public virtual Address GetDestinationAddress()
        {
            AddressModel adr = ShoppingCart.Payment?.ShippingAddress ?? new AddressModel();
            return new Address()
            {
                Line1 = adr.Address1,
                Line2 = adr.Address2,
                City = adr.CityName,
                Country = adr.CountryName,
                Region = adr.StateCode,
                PostalCode = adr.PostalCode
            };
        }

        public virtual GetTaxRequest SetTaxLines(GetTaxRequest taxReq, bool isPostSubmitCall)
        {
            int lineNumber = 0;
            foreach (ZnodeShoppingCartItem ci in ShoppingCart.ShoppingCartItems)
            {
                lineNumber = lineNumber + 1;
                Line li = GetLine(ci, lineNumber);
                taxReq.Lines.Add(li);
            }
            if (isPostSubmitCall && ShoppingCart?.ReturnItemList?.Count > 0)
            {
                foreach (ReturnOrderLineItemModel ci in ShoppingCart.ReturnItemList)
                {
                    if (string.Equals(ci.OrderLineItemStatus, ZnodeOrderStatusEnum.RETURNED.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        lineNumber = lineNumber + 1;
                        Line li = GetLine(ci, lineNumber);
                        taxReq.Lines.Add(li);
                    }
                }
            }
            return taxReq;
        }

        public virtual Line GetLine(ZnodeShoppingCartItem ci, int lineNumber)
        {
            decimal cartQuantity = ci.Product.ZNodeGroupProductCollection?.Count > 0 ? ci.Product.ZNodeGroupProductCollection[0].SelectedQuantity : ci.Quantity;
            Line li = new Line();
            li.ItemCode = ci.Product.SKU;
            li.Description = ci.Product.Name;
            li.No = lineNumber.ToString();
            li.Qty = (double)ci.Quantity;
            li.TaxOverride.TaxOverrideType = TaxOverrideType.None;
            li.TaxOverride.TaxAmount = System.Decimal.Zero;
            li.TaxOverride.Reason = string.Empty;
            li.TaxOverride.TaxDate = System.DateTime.Today;
            li.TaxCode = GetProductAttributeValue("AvaTaxCode", ci) ?? ZnodeApiSettings.AvaTaxCode; 
            li.Amount = GetCartItemPrice(ci, cartQuantity);

            return li;
        }

        public virtual Line GetLine(ReturnOrderLineItemModel ci, int lineNumber)
        {
            decimal cartQuantity = ci.Quantity;
            Line li = new Line();
            li.ItemCode = ci.SKU;
            li.Description = ci.ProductName;
            li.No = lineNumber.ToString();
            li.Qty = (double)ci.Quantity;
            li.TaxOverride.TaxOverrideType = TaxOverrideType.None;
            li.TaxOverride.TaxAmount = System.Decimal.Zero;
            li.TaxOverride.Reason = string.Empty;
            li.TaxOverride.TaxDate = System.DateTime.Today;
            li.TaxCode = ZnodeApiSettings.AvaTaxCode;
            li.Amount = IsCalculateTaxAfterDiscount() ? (ci.UnitPrice * cartQuantity) - GetReturnLineItemDiscount(ci) : ci.UnitPrice * cartQuantity; 
            return li;
        }

        private void BindShippingLineItem(GetTaxRequest taxReq)
        {
            //now, add in the shipping line item
            if (IsCalculateTaxOnShipping(ShoppingCart.ShippingCost))
            {
                Line li = new Line();
                li.ItemCode = ZnodeConstant.Shipping;
                li.No = (taxReq.Lines.Count + 1).ToString();
                li.Qty = 1;
                li.Description = ZnodeConstant.Shipping;
                li.TaxOverride.TaxOverrideType = TaxOverrideType.None;
                li.TaxOverride.TaxAmount = System.Decimal.Zero;
                li.TaxOverride.Reason = string.Empty;
                li.TaxOverride.TaxDate = System.DateTime.Today;
                li.Amount = IsCalculateTaxAfterDiscount() ? ShoppingCart.ShippingCost - ShoppingCart.ShippingDiscount : ShoppingCart.ShippingCost;

                //since there is only one freight identifier, will send the identifier of the first item in the cart, of note, this is stored at product for possible future support of multi shipping rules
                li.TaxCode = avaSettings.AvalaraFreightIdentifier;
                taxReq.Lines.Add(li);
            }
        }

        //Gets the order state.
        private string GetOrderState(int orderId, out string orderNumber, out DateTime createdDate)
        {
            orderNumber = string.Empty;
            createdDate = DateTime.Today;
            if (orderId > 0)
            {
                IZnodeRepository<ZnodeOmsOrderDetail> znodeOmsOrderDetails = new ZnodeRepository<ZnodeOmsOrderDetail>();
                IZnodeRepository<ZnodeOmsOrderState> znodeOmsOrderState = new ZnodeRepository<ZnodeOmsOrderState>();
                IZnodeRepository<ZnodeOmsOrder> znodeOmsOrder = new ZnodeRepository<ZnodeOmsOrder>();
                //all client side transactions
                var currency = (from _znodeOmsOrderDetails in znodeOmsOrderDetails.Table
                                join _znodeOmsOrder in znodeOmsOrder.Table on _znodeOmsOrderDetails.OmsOrderId equals _znodeOmsOrder.OmsOrderId
                                join _znodeOmsOrderState in znodeOmsOrderState.Table on _znodeOmsOrderDetails.OmsOrderStateId equals _znodeOmsOrderState.OmsOrderStateId
                                where _znodeOmsOrderDetails.OmsOrderId == orderId && _znodeOmsOrderDetails.IsActive
                                select new
                                {
                                    OrderStateName = _znodeOmsOrderState.OrderStateName,
                                    OrderNumber = _znodeOmsOrder.OrderNumber,
                                    CreatedDate = _znodeOmsOrder.CreatedDate,
                                }

                            )?.FirstOrDefault();

                if (!Equals(currency, null))
                {
                    orderNumber = currency.OrderNumber;
                    createdDate = currency.CreatedDate;
                    return currency.OrderStateName;
                }
            }
            return string.Empty;
        }

        //Get the order number.
        private string GetOrderNumber(int omsOrderId)
        {
            if (omsOrderId > 0)
            {
                IZnodeRepository<ZnodeOmsOrder> znodeOmsOrder = new ZnodeRepository<ZnodeOmsOrder>();
                return znodeOmsOrder.Table.FirstOrDefault(x => x.OmsOrderId == omsOrderId)?.OrderNumber;
            }
            return string.Empty;
        }

        //Get the order number.
        private string GetOrderNumber(int omsOrderId, out DateTime orderDate)
        {
            orderDate = DateTime.Today;
            if (omsOrderId > 0)
            {
                IZnodeRepository<ZnodeOmsOrder> znodeOmsOrder = new ZnodeRepository<ZnodeOmsOrder>();
                ZnodeOmsOrder omsOrder = znodeOmsOrder.Table.FirstOrDefault(x => x.OmsOrderId == omsOrderId);
                orderDate = omsOrder.CreatedDate;
                return omsOrder?.OrderNumber;
            }
            return string.Empty;
        }

        private bool IsTaxExempt(GetTaxRequest avalaraTaxRequest)
        {
            bool profile;
            if (ShoppingCart?.UserId > 0)
            {
                IZnodeRepository<ZnodeUserProfile> userProfile = new ZnodeRepository<ZnodeUserProfile>();
                IZnodeRepository<ZnodeProfile> ps = new ZnodeRepository<ZnodeProfile>();
                //all client side transactions
                profile = (from shippingportal in userProfile.Table
                           join _profile in ps.Table on shippingportal.ProfileId equals _profile.ProfileId
                           where shippingportal.UserId == ShoppingCart.UserId && shippingportal.IsDefault == true
                           select
                               _profile.TaxExempt
                           )?.FirstOrDefault() ?? false;

                avalaraTaxRequest.CustomerCode = ShoppingCart.UserId.ToString();
            }
            else
            {
                profile = false;
                avalaraTaxRequest.CustomerCode = "00001"; //any value will do, it's not a real customer
            }

            return profile;
        }

        private TaxSvc CreateTaxService()
        {
            string client = ZnodeApiSettings.AvaTaxClientHeader;

            if (string.IsNullOrEmpty(client))
                client = "ALP,9.0.0,Znode Avalara Tax Addin,1.0";

            TaxSvc ts = new TaxSvc();
            ts.Configuration.Security.Account = avaSettings.AvalaraAccount;
            ts.Configuration.Security.License = avaSettings.AvalaraLicense;
            ts.Configuration.Url = avaSettings.AvalaraServiceURL;
            ts.Profile.Client = client;
            return ts;
        }

        private void SetAvataxSettings()
        {
            int portalId = ShoppingCart?.PortalId ?? 0;
            if (portalId > 0)
            {
                SetAvalraSetting(portalId);

                //error if configuration is missing
                if ((string.IsNullOrEmpty(avaSettings.AvalaraAccount)) || (string.IsNullOrEmpty(avaSettings.AvalaraLicense)) || (string.IsNullOrEmpty(avaSettings.AvalaraServiceURL)))
                    throw new Exception("Please verify that the account, license, and URL settings have been provided in the configuration file.");
            }
            else
                throw new Exception("PortalId is not set please check.");
        }

        private void SetAvalraSetting(int portalId)
        {
            //getting the settings out of the first tax class.  The system is currently setup to use multiple avatax class identifiers for classes, but should not have separate avatax account settings for each store
            IZnodeRepository<ZnodeTaxPortal> taxPortalRepository = new ZnodeRepository<ZnodeTaxPortal>();
            ZnodeTaxPortal setting = taxPortalRepository.Table.FirstOrDefault(x => x.PortalId == portalId);
            if (!Equals(setting, null))
            {
                avaSettings = new AvataxSettings()
                {
                    AvalaraAccount = setting.AvalaraAccount,
                    AvalaraCompanyCode = setting.AvalaraCompanyCode,
                    AvalaraFreightIdentifier = setting.AvalaraFreightIdentifier,
                    AvalaraLicense = setting.AvalaraLicense,
                    AvalaraServiceURL = setting.AvataxUrl
                };
            }
        }

        private void SetAvataxSettings(int portalId)
        {
            if (portalId > 0)
            {
                SetAvalraSetting(portalId);

                //error if configuration is missing
                if ((string.IsNullOrEmpty(avaSettings.AvalaraAccount)) || (string.IsNullOrEmpty(avaSettings.AvalaraLicense)) || (string.IsNullOrEmpty(avaSettings.AvalaraServiceURL)))
                    throw new Exception("Please verify that the account, license, and URL settings have been provided in the configuration file.");
            }
            else
                throw new Exception("PortalId is not set please check.");
        }

        private void LogErrorMessage(Message m)
        {
            string errorMessage = "Severity: " + m.Severity + "<br />";
            errorMessage += "Name: " + m.Name + "<br />";
            errorMessage += "Details: " + m.Details + "<br />";
            errorMessage += "Summary: " + m.Summary + "<br />";
            LogErrorMessage(errorMessage);
        }

        private void LogErrorMessage(Messages messages)
        {
            //shipping tax, if any is only stored at the order level in Znode, so we won't record it as a line item, and it would be the difference of the line items taxes and the order level tax                
            foreach (Message m in messages)
                LogErrorMessage(m);
        }

        private void LogErrorMessage(string errorMessage)
              // Log Activity            
              => ZnodeLogging.LogMessage(errorMessage, "Avalara");

        //Get Attribute value
        private string GetProductAttributeValue(string attributeCode, ZnodeShoppingCartItem cartItem)
        {
            if (HelperUtility.IsNotNull(cartItem))
            {
                if (cartItem.Product.ZNodeGroupProductCollection.Count > 0)
                    return cartItem?.Product?.ZNodeGroupProductCollection[0]?.Attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.AttributeValue;
                else
                    return cartItem?.Product?.Attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.AttributeValue;
            }
            return string.Empty;
        }

        //Sets the order level discount amount.
        private void SetDiscountAmount(GetTaxRequest taxReq)
        {
            if (!Equals(ShoppingCart, null))
            {
                decimal totalDiscount = ShoppingCart.Discount + ShoppingCart.CSRDiscount;
                //now apply the discounted flag because order level discounts need to be applied as a whole
                if (totalDiscount > 0m)
                {
                    foreach (Line li in taxReq.Lines)
                        li.Discounted = true;
                    taxReq.Discount = totalDiscount;
                }
            }
        }

        //Get the portal entity via portal id.
        private static ZnodePortal GetPortalEntity(int currentPortalID)
        {
            IZnodeRepository<ZnodePortal> ps = new ZnodeRepository<ZnodePortal>();
            return ps.Table.FirstOrDefault(x => x.PortalId == currentPortalID);
        }

        private void BindReturnLineItem(ShoppingCartModel orderModel, GetTaxHistoryResult taxRes, GetTaxRequest avalaraTaxRequest)
        {
            decimal returnShippingCost = 0;
            decimal returnShippingDiscount = 0;
            foreach (var item in orderModel.ReturnItemList)
            {
                for (int i = 0; i < taxRes.GetTaxRequest.Lines.Count; i++)
                {
                    Line result = taxRes.GetTaxRequest.Lines[i];
                    if (!Equals(result, null) && !Equals(item, null) && string.Equals(item.SKU, result.ItemCode, StringComparison.InvariantCultureIgnoreCase) && string.Equals(ZnodeOrderStatusEnum.RETURNED.ToString(), item.OrderLineItemStatus, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if(item.IsShippingReturn)
                        {
                            returnShippingCost = returnShippingCost + item.ShippingCost;
                            returnShippingDiscount = returnShippingDiscount + (item.PerQuantityShippingDiscount * item.Quantity);
                        }
                        result.Amount = -(GetReturnCartItemPrice(item, item.Quantity));
                        result.Qty = (double)item.Quantity;
                        avalaraTaxRequest.Lines.Add(result);
                        break;
                    }
                }
            }
            if( returnShippingCost > 0 && IsCalculateTaxOnShipping(returnShippingCost))
            {
                Line li = new Line();
                li.ItemCode = ZnodeConstant.Shipping;
                li.No = (avalaraTaxRequest.Lines.Count + 1).ToString();
                li.Qty = 1;
                li.Description = ZnodeConstant.Shipping;
                li.Amount = li.Amount - (GetReturnShippingCost(returnShippingCost, returnShippingDiscount));
                li.TaxCode = avaSettings.AvalaraFreightIdentifier;
                avalaraTaxRequest.Lines.Add(li);
            }
        }

        private void SetReturnTaxRequest(ShoppingCartModel orderModel, GetTaxRequest avalaraTaxRequest)
        {
            DateTime orderDate = DateTime.Today;
            //get the current store information
            avalaraTaxRequest.OriginAddress = GetOriginAddress();
            avalaraTaxRequest.OriginAddress = GetDestinationAddress();
            avalaraTaxRequest.CompanyCode = avaSettings.AvalaraCompanyCode;
            avalaraTaxRequest.CurrencyCode = GetCurrencyCode(ZnodeConfigManager.SiteConfig.PortalId); //ISO code

            //default to USD, must have something
            if (string.IsNullOrEmpty(avalaraTaxRequest.CurrencyCode))
                avalaraTaxRequest.CurrencyCode = "USD";

            avalaraTaxRequest.DocDate = System.DateTime.Today;
            avalaraTaxRequest.CustomerCode = orderModel.UserId.ToString();
            avalaraTaxRequest.DocCode = $"{GetOrderNumber(orderModel.OmsOrderId ?? 0, out orderDate)}.{orderModel.ReturnItemList.FirstOrDefault()?.OmsOrderLineItemsId}";
            avalaraTaxRequest.DocType = DocumentType.ReturnInvoice;
            avalaraTaxRequest.TaxOverride.TaxAmount = System.Decimal.Zero;
            avalaraTaxRequest.TaxOverride.TaxOverrideType = TaxOverrideType.TaxDate;
            avalaraTaxRequest.TaxOverride.Reason = orderModel.ReturnItemList?.FirstOrDefault()?.ReasonForReturn;
            avalaraTaxRequest.TaxOverride.TaxDate = orderDate;
            avalaraTaxRequest.Commit = true;
        }
        #endregion       
    } //end class
} //end namespace
