using System;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Taxes.Helper;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Avalara.AvaTax.RestClient;
using System.Collections.Generic;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Taxes
{
    public class AvataxMapper : ZnodeTaxesType, IAvataxMapper
    {
        public AvataxSettings avataxSettings;
        IAvataxHelper avataxHelper = GetService<IAvataxHelper>();

        public AvataxMapper(int portalId)
        {
            avataxHelper = GetService<IAvataxHelper>();
            avataxSettings = avataxHelper.SetAvataxSettings(portalId);
        }

        // To get the populated AvataxSettings model.
        public virtual AvataxSettings GetAvataxSetting(TaxPortalModel taxportalModel)
        {
            avataxSettings = new AvataxSettings();
            if (IsNotNull(taxportalModel))
            {
                avataxSettings.AvalaraAccount = taxportalModel.AvalaraAccount;
                avataxSettings.AvalaraCompanyCode = taxportalModel.AvalaraCompanyCode;
                avataxSettings.AvalaraFreightIdentifier = taxportalModel.AvalaraFreightIdentifier;
                avataxSettings.AvalaraLicense = taxportalModel.AvalaraLicense;
                avataxSettings.AvalaraServiceURL = taxportalModel.AvataxUrl;
                avataxSettings.AvataxIsTaxIncluded = taxportalModel.AvataxIsTaxIncluded;
            }
            return avataxSettings;
        }

        // This method populates the CreateTransactionModel with valid details for request.
        public virtual CreateTransactionModel BuildTaxRequest(ZnodeShoppingCart shoppingCart, CreateTransactionModel createTransactionModel, DocumentType docType, bool isPostSubmitCall)
        {
            DateTime createdDate = DateTime.Today;
            createTransactionModel.companyCode = avataxSettings.AvalaraCompanyCode;
            createTransactionModel.code = "DocDate" + System.DateTime.Now.ToString();
            createTransactionModel.date = createdDate;
            createTransactionModel.businessIdentificationNo = shoppingCart.BusinessIdentificationNumber;
            string orderNumber = string.Empty;
            string orderState = string.Empty;
            if (shoppingCart.OrderId > 0)
            {
                createTransactionModel.type = DocumentType.SalesInvoice;
                orderState = avataxHelper.GetOrderState(shoppingCart.OrderId.GetValueOrDefault(), out orderNumber, out createdDate);
                createTransactionModel.code = orderNumber;
                createTransactionModel.date = createdDate;

                if (createTransactionModel.taxOverride == null)
                    createTransactionModel.taxOverride = new TaxOverrideModel();

                createTransactionModel.taxOverride.type = TaxOverrideType.TaxDate;
                createTransactionModel.taxOverride.taxAmount = System.Decimal.Zero;
                createTransactionModel.taxOverride.taxDate = createdDate;
                createTransactionModel.taxOverride.reason = ZnodeConstant.OrderModified;
            }

            if (shoppingCart.OrderId > 0 && isPostSubmitCall)
            {
                //If the order has been shipped then, commit the request in avalara portal.
                if (string.Equals(orderState, ZnodeOrderStatusEnum.SHIPPED.ToString(), StringComparison.InvariantCultureIgnoreCase) && !ZnodeApiSettings.DisableTaxCommit && isPostSubmitCall)
                    createTransactionModel.commit = true;
            }
            else
                createTransactionModel.type = docType;

            createTransactionModel.currencyCode = ZnodeConstant.UnitedStatesSuffix;
            createTransactionModel.discount = Decimal.Zero;

            // Get customer profile to determine if tax exempt.         
            createTransactionModel.exemptionNo = (avataxHelper.IsTaxExempt(shoppingCart, createTransactionModel) || shoppingCart.CustomTaxCost == 0) ? ZnodeApiSettings.ExemptionNo ?? "EXEMPT" : "";

            // Get the currency code.
            if (shoppingCart?.PortalId > 0)
            {
                createTransactionModel.currencyCode = avataxHelper.GetCurrencyCode(shoppingCart.PortalId.GetValueOrDefault()); 
                // Default currency code will be set to USD if helper does not returns currency code.
                if (string.IsNullOrEmpty(createTransactionModel.currencyCode))
                    createTransactionModel.currencyCode = ZnodeConstant.UnitedStatesSuffix;
            }
            else
                createTransactionModel.taxOverride.type = TaxOverrideType.None;

            return createTransactionModel;
        }

        // To get the origin address based in portal Id.
        protected virtual AddressLocationInfo GetOriginAddress(ZnodeShoppingCart shoppingCart)
        {
            AddressLocationInfo orgAddress = new AddressLocationInfo();
            ZnodeTaxHelper taxHelper = new ZnodeTaxHelper();
            AddressModel portalWarehouseAddressModel = taxHelper.GetPortalShippingAddress(shoppingCart.PortalId.GetValueOrDefault());
            if (!Equals(portalWarehouseAddressModel, null))
            {
                orgAddress.line1 = portalWarehouseAddressModel.Address1;
                orgAddress.line2 = portalWarehouseAddressModel.Address2;
                orgAddress.city = portalWarehouseAddressModel.CityName;
                orgAddress.region = portalWarehouseAddressModel.StateCode;
                orgAddress.postalCode = portalWarehouseAddressModel.PostalCode;
                orgAddress.country = portalWarehouseAddressModel.CountryName;
            }
            return orgAddress;
        }

        // To get the destination address from shopping cart.
        protected virtual AddressLocationInfo GetDestinationAddress(ZnodeShoppingCart shoppingCart)
        {
            AddressModel addressModel = shoppingCart.Payment?.ShippingAddress ?? new AddressModel();
            return new AddressLocationInfo()
            {
                line1 = addressModel.Address1,
                line2 = addressModel.Address2,
                city = addressModel.CityName,
                country = addressModel.CountryName,
                region = addressModel.StateCode,
                postalCode = addressModel.PostalCode
            };
        }

        // To set line details in CreateTransactionModel.
        protected virtual CreateTransactionModel SetTaxLines(ZnodeShoppingCart shoppingCart, CreateTransactionModel taxRequest, bool isPostSubmitCall)
        {
            int lineNumber = 0;
            foreach (ZnodeShoppingCartItem cartItem in shoppingCart.ShoppingCartItems)
            {
                lineNumber = lineNumber + 1;
                LineItemModel lineItem = new LineItemModel();

                if (cartItem.Product.ZNodeGroupProductCollection?.Count > 0)
                {
                    lineItem = GetLineFromProductCollection(cartItem, lineNumber, shoppingCart);
                }
                else
                {
                    lineItem = GetLine(cartItem, lineNumber, shoppingCart);
                }
                if (taxRequest.lines == null)
                    taxRequest.lines = new List<LineItemModel>();
                taxRequest.lines.Add(lineItem);
            }

            if (isPostSubmitCall && shoppingCart?.ReturnItemList?.Count > 0)
            {
                foreach (ReturnOrderLineItemModel ci in shoppingCart.ReturnItemList)
                {
                    if (string.Equals(ci.OrderLineItemStatus, ZnodeOrderStatusEnum.RETURNED.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        lineNumber = lineNumber + 1;
                        LineItemModel li = GetLine(ci, lineNumber, shoppingCart);
                        if (taxRequest.lines == null)
                            taxRequest.lines = new List<LineItemModel>();
                        taxRequest.lines.Add(li);
                    }
                }
            }
            return taxRequest;
        }

        // This method returns the LineItemModel, which is populated from ZnodeShoppingCartItem model.
        protected virtual LineItemModel GetLine(ZnodeShoppingCartItem shoppingCartItem, int lineNumber, ZnodeShoppingCart shoppingCart)
        {
            decimal cartQuantity = shoppingCartItem.Product.ZNodeGroupProductCollection?.Count > 0 ? shoppingCartItem.Product.ZNodeGroupProductCollection[0].SelectedQuantity : shoppingCartItem.Quantity;
            LineItemModel avataxLineItem = new LineItemModel();
            avataxLineItem.itemCode = shoppingCartItem.Product.SKU;
            avataxLineItem.description = shoppingCartItem.Product.Name;
            avataxLineItem.number = lineNumber.ToString();
            avataxLineItem.quantity = shoppingCartItem.Quantity;
            if (avataxLineItem.taxOverride == null)
                avataxLineItem.taxOverride = new TaxOverrideModel();
            avataxLineItem.taxOverride.type = TaxOverrideType.None;
            avataxLineItem.taxOverride.taxAmount = System.Decimal.Zero;
            avataxLineItem.taxOverride.reason = string.Empty;
            avataxLineItem.taxOverride.taxDate = System.DateTime.Today;
            avataxLineItem.taxCode = GetProductAttributeValue("AvaTaxCode", shoppingCartItem) ?? ZnodeApiSettings.AvaTaxCode;
            avataxLineItem.amount = shoppingCart.IsTaxExempt ? 0 : GetCartItemPrice(shoppingCartItem, cartQuantity, shoppingCart);
            avataxLineItem.taxIncluded = IsPricesInclusiveOfTaxes(shoppingCart);
            return avataxLineItem;
        }

        // This method returns the LineItemModel, which is populated based on details available in ZNodeGroupProductCollection property of ZnodeShoppingCartItem.
        protected virtual LineItemModel GetLineFromProductCollection(ZnodeShoppingCartItem shoppingCartItem, int lineNumber, ZnodeShoppingCart shoppingCart)
        {
            decimal cartQuantity = shoppingCartItem?.Product?.ZNodeGroupProductCollection?.Count > 0 ? shoppingCartItem.Product.ZNodeGroupProductCollection[0].SelectedQuantity : shoppingCartItem.Quantity;
            LineItemModel avataxLineItem = new LineItemModel();
            avataxLineItem.itemCode = shoppingCartItem.Product.ZNodeGroupProductCollection[0]?.SKU;
            avataxLineItem.description = shoppingCartItem.Product.ZNodeGroupProductCollection[0]?.Name;
            avataxLineItem.number = lineNumber.ToString();
            avataxLineItem.quantity = shoppingCartItem.Quantity;
            if (avataxLineItem.taxOverride == null)
                avataxLineItem.taxOverride = new TaxOverrideModel();
            avataxLineItem.taxOverride.type = TaxOverrideType.None;
            avataxLineItem.taxOverride.taxAmount = System.Decimal.Zero;
            avataxLineItem.taxOverride.reason = string.Empty;
            avataxLineItem.taxOverride.taxDate = System.DateTime.Today;
            avataxLineItem.taxCode = GetProductAttributeValue("AvaTaxCode", shoppingCartItem) ?? ZnodeApiSettings.AvaTaxCode;
            decimal unitPrice = GetDisplayedPrice(shoppingCartItem);           
            avataxLineItem.amount = IsCalculateTaxAfterDiscount(shoppingCart) && !shoppingCart.IsQuoteOrder ? (unitPrice * cartQuantity) - GetLineItemDiscount(shoppingCartItem)  : (unitPrice * cartQuantity);
            avataxLineItem.taxIncluded = IsPricesInclusiveOfTaxes(shoppingCart);
            return avataxLineItem;
        }

        // This method returns the LineItemModel, which is populated based on details available in ReturnOrderLineItemModel.
        protected virtual LineItemModel GetLine(ReturnOrderLineItemModel returnOrderLineItem, int lineNumber, ZnodeShoppingCart shoppingCart)
        {
            decimal cartQuantity = returnOrderLineItem.Quantity;
            LineItemModel avataxLineItem = new LineItemModel();
            avataxLineItem.itemCode = returnOrderLineItem.SKU;
            avataxLineItem.description = returnOrderLineItem.ProductName;
            avataxLineItem.number = lineNumber.ToString();
            avataxLineItem.quantity = returnOrderLineItem.Quantity;
            if (avataxLineItem.taxOverride == null)
                avataxLineItem.taxOverride = new TaxOverrideModel();
            avataxLineItem.taxOverride.type = TaxOverrideType.None;
            avataxLineItem.taxOverride.taxAmount = System.Decimal.Zero;
            avataxLineItem.taxOverride.reason = string.Empty;
            avataxLineItem.taxOverride.taxDate = System.DateTime.Today;
            avataxLineItem.taxCode = ZnodeApiSettings.AvaTaxCode;
            avataxLineItem.amount = IsCalculateTaxAfterDiscount(shoppingCart) && !shoppingCart.IsQuoteOrder ? (returnOrderLineItem.UnitPrice * cartQuantity) - GetReturnLineItemDiscount(returnOrderLineItem) : returnOrderLineItem.UnitPrice * cartQuantity;
            avataxLineItem.taxIncluded = IsPricesInclusiveOfTaxes(shoppingCart);

            return avataxLineItem;
        }

        // To bind the shipping line item in tax request.
        protected virtual void BindShippingLineItem(ZnodeShoppingCart shoppingCart, CreateTransactionModel taxRequest, bool shippingTaxInd)
        {
            if (IsCalculateTaxOnShipping(shoppingCart.ShippingCost, shippingTaxInd))
            {
                LineItemModel avataxLineItem = new LineItemModel();
                avataxLineItem.itemCode = ZnodeConstant.Shipping;
                // Check
                avataxLineItem.number = (taxRequest.lines.Count + 1).ToString();
                avataxLineItem.quantity = 1;
                avataxLineItem.description = ZnodeConstant.Shipping;
                if (avataxLineItem.taxOverride == null)
                    avataxLineItem.taxOverride = new TaxOverrideModel();
                avataxLineItem.taxOverride.type = TaxOverrideType.None;
                avataxLineItem.taxOverride.taxAmount = System.Decimal.Zero;
                avataxLineItem.taxOverride.reason = string.Empty;
                avataxLineItem.taxOverride.taxDate = System.DateTime.Today;
                avataxLineItem.amount = IsCalculateTaxAfterDiscount(shoppingCart) && !shoppingCart.IsQuoteOrder ? shoppingCart.ShippingCost - shoppingCart.ShippingDiscount : shoppingCart.ShippingCost;
                avataxLineItem.taxIncluded = IsPricesInclusiveOfTaxes(shoppingCart);

                //since there is only one freight identifier, will send the identifier of the first item in the cart, of note, this is stored at product for possible future support of multi shipping rules
                avataxLineItem.taxCode = avataxSettings.AvalaraFreightIdentifier;
                taxRequest.lines.Add(avataxLineItem);
            }
        }

        // To set ship from and ship to addresses in Avalara AddressesModel. 
        protected virtual AddressesModel SetAddressDetails(ZnodeShoppingCart shoppingCart)
        {
            AddressesModel addressesModel = new AddressesModel();
            addressesModel.shipFrom = GetOriginAddress(shoppingCart);
            addressesModel.shipTo = GetDestinationAddress(shoppingCart);

            return addressesModel;
        }

        // This method sets the order level discount amount in tax request.
        protected virtual void SetDiscountAmount(ZnodeShoppingCart shoppingCart, CreateTransactionModel taxRequest)
        {
            if (!Equals(shoppingCart, null))
            {
                decimal totalDiscount = shoppingCart.Discount + shoppingCart.CSRDiscount + shoppingCart.ShippingDiscount;
                // Now apply the discounted flag because order level discounts need to be applied as a whole
                if (totalDiscount > 0m)
                {
                    foreach (LineItemModel avataxLineItem in taxRequest.lines)
                        avataxLineItem.discounted = true;
                    taxRequest.discount = totalDiscount;
                }
            }
        }

        // To get the product attribute value.
        protected virtual string GetProductAttributeValue(string attributeCode, ZnodeShoppingCartItem shoppingCartItem)
        {
            if (IsNotNull(shoppingCartItem))
            {
                if (shoppingCartItem.Product.ZNodeGroupProductCollection.Count > 0)
                    return shoppingCartItem?.Product?.ZNodeGroupProductCollection[0]?.Attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.AttributeValue;
                else
                    return shoppingCartItem?.Product?.Attributes?.FirstOrDefault(x => x.AttributeCode == attributeCode)?.AttributeValue;
            }
            return string.Empty;
        }

        // To get the populated CreateTransactionModel model for tax request.
        public virtual CreateTransactionModel SetTransationModel(ZnodeShoppingCart shoppingCart, bool shippingTaxInd, bool isPostSubmitCall = false)
        {
            CreateTransactionModel createTransactionModel = new CreateTransactionModel();

            // Get the current portal information.
            ZnodePortal portal = avataxHelper.GetPortalEntity(ZnodeConfigManager.SiteConfig.PortalId);

            if (shoppingCart?.ReturnItemList?.Count > 0)
                isPostSubmitCall = false;

            createTransactionModel = BuildTaxRequest(shoppingCart, createTransactionModel, DocumentType.SalesOrder, isPostSubmitCall);

            createTransactionModel = SetTaxLines(shoppingCart, createTransactionModel, isPostSubmitCall);
            createTransactionModel.addresses = SetAddressDetails(shoppingCart);
            createTransactionModel.isSellerImporterOfRecord = shoppingCart.AvataxIsSellerImporterOfRecord;

            BindShippingLineItem(shoppingCart, createTransactionModel, shippingTaxInd);

            return createTransactionModel;
        }

        // To get the VoidTransactionModel with valid code.
        public virtual VoidTransactionModel GetVoidTransactionModel()
        {
            VoidTransactionModel voidTransactionModel = new VoidTransactionModel()
            {
                code = VoidReasonCode.DocVoided
            };           
            return voidTransactionModel;
        }

        // To map valid details in ZnodeShoppingCart model.
        public virtual void MapDetailsInShoppingCart(ZnodeShoppingCart shoppingCart, TransactionModel taxRequest, int taxRuleId)
        {
            int i = 0;

                if (CheckImportApplicable(taxRequest))
                    shoppingCart.ImportDuty = GetImportDutyValue(taxRequest);

                shoppingCart.SalesTax = Convert.ToDecimal(taxRequest?.totalTax.GetValueOrDefault()) - shoppingCart.ImportDuty;

                // Setting tax rate when shipping address for avalara is of US.
                if (string.Equals(shoppingCart?.Shipping?.ShippingCountryCode, ZnodeConstant.US, StringComparison.InvariantCultureIgnoreCase))
                {
                    shoppingCart.TaxRate = Convert.ToDecimal(taxRequest?.summary?.FirstOrDefault()?.rate) * 100;
                }

                decimal lineItemTax = 0;
                foreach (ZnodeShoppingCartItem shoppingCartItem in shoppingCart.ShoppingCartItems)
                {
                    if (IsNotNull(shoppingCartItem))
                    {
                        int? configurableProductCount = shoppingCartItem.Product?.ZNodeConfigurableProductCollection?.Count;
                        int? groupProductCount = shoppingCartItem.Product?.ZNodeGroupProductCollection?.Count;
                        shoppingCartItem.IsTaxCalculated = true;

                        if (CheckImportApplicable(taxRequest))
                            shoppingCartItem.Product.ImportDuty = GetImportDutyValue(taxRequest, i);

                        if (taxRequest?.lines?.Count >= i)                        
                            shoppingCartItem.Product.SalesTax = Convert.ToDecimal(taxRequest?.lines[i]?.tax.GetValueOrDefault()) - shoppingCartItem.Product.ImportDuty;
                        
                        if (configurableProductCount > 0)
                        {
                            shoppingCartItem.Product.SalesTax = 0;

                            foreach (ZnodeProductBaseEntity productItem in shoppingCartItem.Product.ZNodeConfigurableProductCollection)
                            {
                                if (CheckImportApplicable(taxRequest))
                                    productItem.ImportDuty = GetImportDutyValue(taxRequest, i);

                                if (taxRequest?.lines?.Count >= i)                                
                                    productItem.SalesTax = Convert.ToDecimal(taxRequest?.lines[i]?.tax.GetValueOrDefault()) - productItem.ImportDuty;
                                
                            }
                        }
                        if (groupProductCount > 0)
                        {
                            shoppingCartItem.Product.SalesTax = 0;
                            foreach (ZnodeProductBaseEntity productItem in shoppingCartItem.Product.ZNodeGroupProductCollection)
                            {
                                if (CheckImportApplicable(taxRequest))
                                    productItem.ImportDuty = GetImportDutyValue(taxRequest, i);

                                if (taxRequest?.lines?.Count >= i)                                
                                    productItem.SalesTax = Convert.ToDecimal(taxRequest?.lines[i]?.tax.GetValueOrDefault()) - productItem.ImportDuty;
                                
                            }
                        }
                        shoppingCartItem.TaxRuleId = taxRuleId;

                    if (taxRequest?.lines?.Count >= i)
                    {
                        decimal taxLineItem = Convert.ToDecimal(taxRequest?.lines[i]?.tax.GetValueOrDefault()) - GetImportDutyValue(taxRequest, i);
                        lineItemTax += taxLineItem;
                    }
                        i++;
                    }
                }
                shoppingCart.TaxOnShipping = Convert.ToDecimal(taxRequest?.totalTax.GetValueOrDefault()) - GetImportDutyValue(taxRequest) - lineItemTax;
            }        

        // To map tax summary details in ZnodeShoppingCart.
        public virtual void MapSummaryDetailsInShoppingCart(ZnodeShoppingCart shoppingCart, TransactionModel transactionModel)
        {
            shoppingCart.TaxSummaryList = new List<TaxSummaryModel>();
            // We are not using ToModel method of TranslatorExtension because its reside in Znode.Libraries.Admin project/CL which is not referenced in taxes project. 
            if (HelperUtility.IsNotNull(transactionModel) && transactionModel.summary?.Count > 0 && !transactionModel.summary.Any(x => string.Equals(x.taxType, ZnodeConstant.Sales, StringComparison.InvariantCultureIgnoreCase)))
            {
                foreach (TransactionSummary transactionSummary in transactionModel.summary)
                {
                    if (!string.Equals(transactionSummary.taxSubType, ZnodeConstant.ImportDuty, StringComparison.InvariantCultureIgnoreCase))
                    {
                        shoppingCart.TaxSummaryList.Add(new TaxSummaryModel()
                        {
                            Tax = transactionSummary.tax,
                            Rate = transactionSummary.rate,
                            TaxName = transactionSummary.taxName,
                            TaxTypeName = ZnodeConstant.Avatax
                        });
                    }
                }
            }
        }

        // To map details in ZnodeShoppingCart.
        public virtual void MapMessageDetailsInShoppingCart(ZnodeShoppingCart shoppingCart, TransactionModel transactionModel)
        {
            shoppingCart.TaxMessageList = new List<string>();

            if (HelperUtility.IsNotNull(transactionModel) && transactionModel.messages?.Count > 0 && !transactionModel.summary.Any(x => string.Equals(x.taxType, ZnodeConstant.Sales, StringComparison.InvariantCultureIgnoreCase)))
            {
                foreach (AvaTaxMessage message in transactionModel.messages)
                {
                    if (!message.summary.Equals(ZnodeConstant.MissingHSCodeWarning, StringComparison.InvariantCultureIgnoreCase))
                       shoppingCart.TaxMessageList.Add(message.summary);                  
                }
            }
        }

        // To bind valid values in line items of return request.
        public virtual void BindReturnLineItem(ZnodeShoppingCart shoppingCart, ShoppingCartModel orderModel, AuditTransactionModel auditTransactionResponse, CreateTransactionModel avalaraTaxRequest)
        {
            List<TaxAmountOverrideModel>  taxAmountOverrideList = avataxHelper.GetLineItemTaxOverrideAmount(shoppingCart, orderModel.ReturnItemList);
            avalaraTaxRequest.lines = new List<LineItemModel>();
            decimal returnShippingCost = 0;
            decimal returnShippingDiscount = 0;
            foreach (var item in orderModel.ReturnItemList)
            {
                for (int i = 0; i < auditTransactionResponse.original.request.lines.Count; i++)
                {
                    LineItemModel auditLineItem = auditTransactionResponse.original.request.lines[i];
                    if (!Equals(auditLineItem, null) && !Equals(item, null) && ValidateSku(item, auditLineItem) && string.Equals(ZnodeOrderStatusEnum.RETURNED.ToString(), item.OrderLineItemStatus, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (item.IsShippingReturn)
                        {
                            returnShippingCost = returnShippingCost + item.ShippingCost;
                            returnShippingDiscount = returnShippingDiscount + (item.PerQuantityShippingDiscount * item.Quantity);
                        }
                        auditLineItem.amount = -(item.UnitPrice * item.Quantity);
                        auditLineItem.quantity = (decimal)item.Quantity;
                        if (HelperUtility.IsNull(auditLineItem.taxOverride))
                            auditLineItem.taxOverride = new TaxOverrideModel();
                        auditLineItem.taxOverride.type = TaxOverrideType.TaxAmount;
                        auditLineItem.taxOverride.taxAmount = - taxAmountOverrideList?.FirstOrDefault(x => x.OmsOrderLineItemsId == item.OmsOrderLineItemsId).TaxOverrideAmount;
                        auditLineItem.taxOverride.reason = shoppingCart?.ReturnItemList?.FirstOrDefault()?.ReasonForReturn; ;
                        avalaraTaxRequest.lines.Add(auditLineItem);
                        break;
                    }   
                }
            }
            if (returnShippingCost > 0 && IsCalculateTaxOnShipping(returnShippingCost))
            {
                LineItemModel avataxLineItem = new LineItemModel();

                avataxLineItem.itemCode = ZnodeConstant.Shipping;
                avataxLineItem.number = (avalaraTaxRequest.lines.Count + 1).ToString();
                avataxLineItem.quantity = 1;
                avataxLineItem.description = ZnodeConstant.Shipping;
                avataxLineItem.amount = avataxLineItem.amount - (GetReturnShippingCost(returnShippingCost, returnShippingDiscount));
                AvataxSettings avaSettings = avataxHelper.SetAvataxSettings(shoppingCart.PortalId.GetValueOrDefault());
                avataxLineItem.taxCode = avaSettings.AvalaraFreightIdentifier;
                avalaraTaxRequest.lines.Add(avataxLineItem);
            }
        }

        // To check same SKU is present in both the models or not.
        protected virtual bool ValidateSku(ReturnOrderLineItemModel returnOrderLineItem, LineItemModel auditLineItem)
        {
            if (returnOrderLineItem?.GroupProducts?.Count() > 0)
                return string.Equals(returnOrderLineItem?.GroupProducts[0]?.Sku, auditLineItem?.itemCode, StringComparison.InvariantCultureIgnoreCase);

            return string.Equals(returnOrderLineItem?.SKU, auditLineItem?.itemCode, StringComparison.InvariantCultureIgnoreCase);
        }

        // To bind valid values in line items of cancel request.
        public virtual void BindCancelLineItemDetail(ZnodeShoppingCart shoppingCart, AuditTransactionModel auditTransactionResponse, CreateTransactionModel avalaraTaxRequest)
        {
            List<TaxAmountOverrideModel> taxAmountOverrideList = avataxHelper.GetLineItemTaxOverrideAmount(shoppingCart);
            avalaraTaxRequest.lines = new List<LineItemModel>();
            foreach (ZnodeShoppingCartItem item in shoppingCart.ShoppingCartItems)
            {
                for (int i = 0; i < auditTransactionResponse.original.request.lines.Count; i++)
                {
                    LineItemModel auditLineItem = auditTransactionResponse.original.request.lines[i];
                    if (!Equals(auditLineItem, null) && !Equals(item, null) && string.Equals(item.SKU, auditLineItem.itemCode, StringComparison.InvariantCultureIgnoreCase))
                    {
                        auditLineItem.amount = -(item.UnitPrice * item.Quantity);
                        auditLineItem.quantity = (decimal)item.Quantity;
                        if (HelperUtility.IsNull(auditLineItem.taxOverride))
                            auditLineItem.taxOverride = new TaxOverrideModel();
                        auditLineItem.taxOverride.type = TaxOverrideType.TaxAmount;
                        auditLineItem.taxOverride.taxAmount = -taxAmountOverrideList?.FirstOrDefault(x => x.OmsOrderLineItemsId == item.OmsOrderLineItemId).TaxOverrideAmount;
                        auditLineItem.taxOverride.reason = "Cancel order";
                        avalaraTaxRequest.lines.Add(auditLineItem);
                        break;
                    }
                }
            }
        }

        // To set return tax request.
        public virtual void SetReturnTaxRequest(ZnodeShoppingCart orderModel, CreateTransactionModel avalaraTaxRequest)
        {
            DateTime orderDate = DateTime.Today;
            //get the current store information
            avalaraTaxRequest.addresses = SetAddressDetails(orderModel);

            avalaraTaxRequest.companyCode = avataxSettings.AvalaraCompanyCode;
            avalaraTaxRequest.currencyCode = avataxHelper.GetCurrencyCode(ZnodeConfigManager.SiteConfig.PortalId); //ISO code

            //default to USD, must have something
            if (string.IsNullOrEmpty(avalaraTaxRequest.currencyCode))
                avalaraTaxRequest.currencyCode = ZnodeConstant.UnitedStatesSuffix;

            avalaraTaxRequest.businessIdentificationNo = orderModel.BusinessIdentificationNumber;
            avalaraTaxRequest.date = DateTime.Today;
            avalaraTaxRequest.customerCode = orderModel.UserId.ToString();
            avalaraTaxRequest.code = $"{avataxHelper.GetOrderNumber(orderModel.OrderId ?? 0, out orderDate)}.{orderModel.ReturnItemList.FirstOrDefault()?.OmsOrderLineItemsId}";
            avalaraTaxRequest.type = DocumentType.ReturnInvoice;            
            avalaraTaxRequest.commit = true;
            avalaraTaxRequest.isSellerImporterOfRecord = orderModel.AvataxIsSellerImporterOfRecord;
        }

        // To set cancel tax tax request.
        public virtual void SetCancelTaxRequest(ZnodeShoppingCart shoppingCart, CreateTransactionModel avalaraTaxRequest)
        {
            DateTime orderDate = DateTime.Today;
            //get the current store information
            avalaraTaxRequest.addresses = SetAddressDetails(shoppingCart);

            avalaraTaxRequest.companyCode = avataxSettings.AvalaraCompanyCode;
            avalaraTaxRequest.currencyCode = avataxHelper.GetCurrencyCode(ZnodeConfigManager.SiteConfig.PortalId); //ISO code

            //default to USD, must have something
            if (string.IsNullOrEmpty(avalaraTaxRequest.currencyCode))
                avalaraTaxRequest.currencyCode = ZnodeConstant.UnitedStatesSuffix;

            avalaraTaxRequest.businessIdentificationNo = shoppingCart.BusinessIdentificationNumber;
            avalaraTaxRequest.date = DateTime.Today;
            avalaraTaxRequest.customerCode = shoppingCart.UserId.ToString();
            avalaraTaxRequest.code = $"{avataxHelper.GetOrderNumber(shoppingCart.OrderId ?? 0, out orderDate)}.{shoppingCart.ShoppingCartItems?.FirstOrDefault()?.OmsOrderLineItemId}";
            avalaraTaxRequest.type = DocumentType.ReturnInvoice;
            avalaraTaxRequest.commit = true;
            avalaraTaxRequest.isSellerImporterOfRecord = shoppingCart.AvataxIsSellerImporterOfRecord;
        }

        // To check import duty is applicable based on tax summary details available in TransactionModel. 
        protected virtual bool CheckImportApplicable(TransactionModel taxRes)
        {
            return HelperUtility.IsNotNull(taxRes?.summary?.FirstOrDefault(x => x.taxSubType == "ImportDuty"));
        }

        // To get the import duty value from tax summary details available in TransactionModel. 
        protected virtual decimal GetImportDutyValue(TransactionModel taxRes)
        {
            return (taxRes?.summary?.FirstOrDefault(x => x.taxSubType == "ImportDuty")?.tax).GetValueOrDefault();
        }

        // To get the import duty value of specific line item of TransactionModel. 
        protected virtual decimal GetImportDutyValue(TransactionModel taxRes, int i = 0)
        {
            return (taxRes?.lines[i]?.details?.FirstOrDefault(x => x.taxSubTypeId == "ImportDuty")?.tax).GetValueOrDefault();
        }
    }
}
