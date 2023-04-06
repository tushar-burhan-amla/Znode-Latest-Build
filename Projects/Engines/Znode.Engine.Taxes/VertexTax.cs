using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Taxes.CalculateTax70;
using Znode.Engine.Taxes.Helper;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Taxes
{
    public class VertexTax : ZnodeTaxesType
    {
        #region Private Variable
        private CalculateTaxWS70Client _CalculateTaxWS70 { get { return new CalculateTaxWS70Client(); } }
        private VertexSetting _Settings;
        #endregion

        #region Constructors
        public VertexTax()
        {
            Name = "Vertex Tax";
            Description = "Vertex tax connector.";
        }
        #endregion

        #region Public Methods
        //Calculate the tax and set in the shopping cart.
        public override void Calculate()
        {
            try
            {
                SetVertexSetting(ShoppingCart?.PortalId);
                calculateTaxRequest request = GetTaxRequest();
                request.VertexEnvelope.Item = GetQuotationRequest();
                LogErrorMessage(ZnodeTaxHelper.GetLogFor(request.VertexEnvelope));
                _CalculateTaxWS70.calculateTax70(ref request.VertexEnvelope);
                QuotationResponseType response = (QuotationResponseType)request.VertexEnvelope.Item;
                LogErrorMessage(ZnodeTaxHelper.GetLogFor(request.VertexEnvelope));

                int i = 0;
                // subtract one for the shipping item
                ShoppingCart.SalesTax += response.TotalTax;
                decimal lineItemTax = 0;
                foreach (ZnodeShoppingCartItem cartItem in ShoppingCart.ShoppingCartItems)
                {
                    cartItem.IsTaxCalculated = true;
                    int? configurableProductCount = cartItem?.Product?.ZNodeConfigurableProductCollection?.Count;
                    int? groupProductCount = cartItem?.Product?.ZNodeGroupProductCollection?.Count;
                    cartItem.Product.SalesTax = response.LineItem[i].TotalTax;
                    if (configurableProductCount > 0)
                    {
                        cartItem.Product.SalesTax = 0;
                        foreach (ZnodeProductBaseEntity productItem in cartItem.Product.ZNodeConfigurableProductCollection)
                        {
                            productItem.SalesTax = response.LineItem[i].TotalTax;
                        }
                    }
                    if (groupProductCount > 0)
                    {
                        cartItem.Product.SalesTax = 0;
                        foreach (ZnodeProductBaseEntity productItem in cartItem.Product.ZNodeGroupProductCollection)
                        {
                            productItem.SalesTax = response.LineItem[i].TotalTax;
                        }
                    }
                    cartItem.TaxRuleId = this.TaxBag.TaxRuleId;
                    lineItemTax += response.LineItem[i].TotalTax;
                    i++;
                }
                ShoppingCart.TaxOnShipping = response.TotalTax - lineItemTax;
                ShoppingCart.SalesTax = response?.TotalTax ?? 0;
                ShoppingCart.TaxRate = Convert.ToDecimal(response?.LineItem[0]?.Taxes?.Sum(x => x.EffectiveRate) * 100);
            }
            catch (Exception EX)
            {
                ShoppingCart.SalesTax = 0;
                LogErrorMessage(ZnodeTaxHelper.GetLogFor(EX));
            }
        }

        //Calculate the tax and write tax in vertex portal.
        public override void PostSubmitOrderProcess(bool isTaxCostUpdated = true)
        {
            try
            {
                if (isTaxCostUpdated)
                {
                    PostDataToVertex(null);
                }
            }
            catch (Exception EX)
            {
                ShoppingCart.SalesTax = 0;
                LogErrorMessage(ZnodeTaxHelper.GetLogFor(EX));
            }
        }

        //Cancel the order request.
        public override void CancelOrderRequest(ShoppingCartModel shoppingCartModel)
        {
            SetVertexSetting(ShoppingCart?.PortalId);
            calculateTaxRequest request = GetTaxRequest();
            if(!RollbackCurrentTransactionInJournal(request))
                ShoppingCart.SalesTax = 0;
        }

        /// <summary>
        /// To delete the current transaction in Journal.
        /// </summary>
        /// <param name="request">calculateTaxRequest</param>
        /// <returns>Returns true if transaction deleted successfully.</returns>
        protected virtual bool DeleteCurrentTransactionInJournal(calculateTaxRequest request)
        {
            try
            {
                request.VertexEnvelope.Item = GetDeleteRequest();
                request.VertexEnvelope.ApplicationData = new VertexEnvelopeApplicationData() { Sender = _Settings.CompanyCode, MessageLogging = new VertexEnvelopeApplicationDataMessageLogging() { returnLogEntries = true } };
                LogErrorMessage(ZnodeTaxHelper.GetLogFor(request.VertexEnvelope));
                _CalculateTaxWS70.calculateTax70(ref request.VertexEnvelope);
                return true;
            }
            catch (Exception ex)
            {
                LogErrorMessage(ZnodeTaxHelper.GetLogFor(ex));
                return false;
            }
        }

        /// <summary>
        /// To rollback the active transaction in Journal.
        /// </summary>
        /// <param name="request">calculateTaxRequest</param>
        /// <returns>Returns true if transaction rolled back successfully.</returns>
        protected virtual bool RollbackCurrentTransactionInJournal(calculateTaxRequest request)
        {
            try
            {
                request.VertexEnvelope.Item = new RollbackRequestType() { transactionId = GetOrderNumber(ShoppingCart.OrderId) };
                request.VertexEnvelope.ApplicationData = new VertexEnvelopeApplicationData() { Sender = _Settings.CompanyCode, MessageLogging = new VertexEnvelopeApplicationDataMessageLogging() { returnLogEntries = true } };
                LogErrorMessage(ZnodeTaxHelper.GetLogFor(request.VertexEnvelope));
                _CalculateTaxWS70.calculateTax70(ref request.VertexEnvelope);
                return true;
            }
            catch (Exception ex)
            {
                LogErrorMessage(ZnodeTaxHelper.GetLogFor(ex));
                return false;
            }
        }

        //Return order line item.
        public override void ReturnOrderLineItem(ShoppingCartModel orderModel)
        {
            try
            {
                //In case of Vertex tax type, if order state is Cancelled then call CancelOrderRequest method of this class and for vertex no need to consider order line item returns case.
                if (string.Equals(orderModel.OrderStatus, ZnodeOrderStatusEnum.CANCELED.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    CancelOrderRequest(orderModel);
            }
            catch (Exception ex)
            {
                ShoppingCart.SalesTax = 0;
                LogErrorMessage(ZnodeTaxHelper.GetLogFor(ex));
            }
        }

        //Set the request for vertex.
        public virtual void PostDataToVertex(List<ReturnOrderLineItemModel> returnItemList)
        {
            SetVertexSetting(ShoppingCart?.PortalId);
            calculateTaxRequest request = GetTaxRequest();
            RollbackCurrentTransactionInJournal(request);
            request.VertexEnvelope.Item = GetInvoiceRequest(returnItemList);
            LogErrorMessage(ZnodeTaxHelper.GetLogFor(request.VertexEnvelope));
            _CalculateTaxWS70.calculateTax70(ref request.VertexEnvelope);
            InvoiceResponseType response = (InvoiceResponseType)request.VertexEnvelope.Item;
            LogErrorMessage(ZnodeTaxHelper.GetLogFor(response));
            int i = 0;
            // subtract one for the shipping item
            ShoppingCart.SalesTax += response.TotalTax;
            decimal lineItemTax = 0;
            foreach (ZnodeShoppingCartItem cartItem in ShoppingCart.ShoppingCartItems)
            {
                cartItem.IsTaxCalculated = true;
                cartItem.Product.SalesTax = response.LineItem[i].TotalTax;
                cartItem.TaxRuleId = this.TaxBag.TaxRuleId;
                lineItemTax += response.LineItem[i].TotalTax;
                i++;
            }
            ShoppingCart.TaxOnShipping = response.TotalTax - lineItemTax;
            ShoppingCart.SalesTax = response?.TotalTax ?? 0;
        }
        #endregion

        #region Private Methods      
        //Get the invoice request to write the tax on vertex portal.
        private InvoiceRequestType GetInvoiceRequest(List<ReturnOrderLineItemModel> returnItemList)
            => new InvoiceRequestType()
            {
                transactionId = GetOrderNumber(ShoppingCart?.OrderId),
                documentDate = GetOrderDate(),
                documentNumber = ShoppingCart.OrderId?.ToString(),
                postingDate = DateTime.Now,
                transactionType = SaleTransactionType.SALE,
                Currency = GetCurrency(),
                Seller = new SellerType { Company = _Settings.CompanyCode, PhysicalOrigin = GetOriginatorAddress() },
                Customer = new CustomerType { Destination = GetDestinationAddress() },
                LineItem = GetLinesISIType(returnItemList),
            };

        //Get the delete request to delete the tax on vertex portal.
        private DeleteRequestType GetDeleteRequest()
            => new DeleteRequestType() { transactionId = GetOrderNumber(ShoppingCart.OrderId) };

        //Get the order number.
        private string GetOrderNumber(int? omsOrderId)
        {
            if (omsOrderId > 0)
            {
                IZnodeRepository<ZnodeOmsOrder> znodeOmsOrder = new ZnodeRepository<ZnodeOmsOrder>();
                return znodeOmsOrder.Table.FirstOrDefault(x => x.OmsOrderId == omsOrderId)?.OrderNumber;
            }
            return string.Empty;
        }
       
        //Get the quotation request to calculate the tax on vertex portal.
        private QuotationRequestType GetQuotationRequest()
            => new QuotationRequestType()
            {
                documentNumber = ShoppingCart.OrderId?.ToString(),
                documentDate = GetOrderDate(),
                postingDate = DateTime.Now,
                transactionType = SaleTransactionType.SALE,
                transactionId = GetOrderNumber(ShoppingCart.OrderId),
                Currency = GetCurrency(),
                Seller = new SellerType { Company = _Settings.CompanyCode, PhysicalOrigin = GetOriginatorAddress() },
                Customer = new CustomerType { Destination = GetDestinationAddress(), CustomerCode = GetCustomerCode() },
                LineItem = GetLines(),
            };

        private CustomerCodeType GetCustomerCode()
            => new CustomerCodeType { classCode = "tax", Value = ShoppingCart.LoginUserName };

        //get the lines for tax calculation.
        private LineItemQSIType[] GetLines()
        {
            List<LineItemQSIType> lines = new List<LineItemQSIType>();
            int lineNumber = 0;
            foreach (ZnodeShoppingCartItem cartItem in ShoppingCart.ShoppingCartItems)
            {
                LineItemQSIType lineItem = GetLine(cartItem, lineNumber);
                lines.Add(lineItem);               
                lineNumber++;
            }

            //now, add in the shipping line item
            if (IsCalculateTaxOnShipping(ShoppingCart.ShippingCost))
                lines.Add(GetLine());

            return lines.ToArray();
        }

        //Get the tax lines for invoice request.
        private LineItemISIType[] GetLinesISIType(List<ReturnOrderLineItemModel> returnItemList)
        {
            List<LineItemISIType> lines = new List<LineItemISIType>();
            int lineNumber = 0;
            foreach (ZnodeShoppingCartItem cartItem in ShoppingCart.ShoppingCartItems)
            {
                LineItemISIType lineItem = GetLineISIType(cartItem, lineNumber);
                lines.Add(lineItem);
                lineNumber++;
            }
            //Include the return items with negative price.
            if (returnItemList?.Count > 0)
            {
                decimal returnShippingCost = 0;
                decimal returnShippingDiscount = 0;
                foreach (ReturnOrderLineItemModel cartItem in returnItemList)
                {
                    if (cartItem.IsShippingReturn)
                    {
                        returnShippingCost = returnShippingCost + cartItem.ShippingCost;
                        returnShippingDiscount = returnShippingDiscount + (cartItem.PerQuantityShippingDiscount * cartItem.Quantity);
                    }
                    LineItemISIType lineItem = GetLineISIType(cartItem, lineNumber);
                    lines.Add(lineItem);
                    lineNumber++;
                }
                if (returnShippingCost > 0 && IsCalculateTaxOnShipping(returnShippingCost))
                    lines.Add(GetReturnShippingLineISIType(returnShippingCost, returnShippingDiscount));
            }

            //now, add in the shipping line item
            if (IsCalculateTaxOnShipping(ShoppingCart.ShippingCost))
                lines.Add(GetShippingLineISIType());

            return lines.ToArray();
        }

        //Get the lines for quatation request.
        private LineItemQSIType GetLine(ZnodeShoppingCartItem cartItem, int lineNumber)
        {
            decimal cartQuantity = cartItem.Product.ZNodeGroupProductCollection?.Count > 0 ? cartItem.Product.ZNodeGroupProductCollection[0].SelectedQuantity : cartItem.Quantity;

            return new LineItemQSIType()
            {
                lineItemNumber = lineNumber.ToString(),
                UnitPriceSpecified = true,
                ExtendedPriceSpecified = true,
                UnitPrice = IsCalculateTaxAfterDiscount() ? cartItem.PromotionalPrice: cartItem.UnitPrice,
                ExtendedPrice = GetCartItemPrice(cartItem, cartQuantity),
                Quantity = new MeasureType() { Value = cartQuantity },
                Seller = new SellerType() { Company = _Settings.CompanyCode, PhysicalOrigin = GetOriginatorAddress() },
                Customer = new CustomerType { CustomerCode = GetCustomerCode(), Destination = GetDestinationAddress() },
                Product = new Product { productClass = GetTruncatedString(cartItem.SKU), Value = GetTruncatedString(cartItem.Product?.Name) },
                FlexibleFields = GetFlexibleFields(cartItem),
                Discount = GetDiscount(cartItem),             
        };
        }

        //Get the shipping line item.
        private LineItemQSIType GetLine()
            => new LineItemQSIType()
            {
                Product = new Product() { Value = "Shipping" },
                ExtendedPrice = IsCalculateTaxAfterDiscount() ? ShoppingCart.ShippingCost - ShoppingCart.ShippingDiscount : ShoppingCart.ShippingCost ,
                ExtendedPriceSpecified = true
            };

        //Get the lines for invoice request.
        private LineItemISIType GetLineISIType(ZnodeShoppingCartItem cartItem, int lineNumber)
        {
            decimal cartQuantity = cartItem.Product.ZNodeGroupProductCollection?.Count > 0 ? cartItem.Product.ZNodeGroupProductCollection[0].SelectedQuantity : cartItem.Quantity;
            return new LineItemISIType()
            {
                lineItemNumber = lineNumber.ToString(),
                taxDate = GetOrderDate(),
                postingDate = DateTime.Now,
                transactionType = SaleTransactionType.SALE,
                UnitPrice = IsCalculateTaxAfterDiscount() ? cartItem.PromotionalPrice : cartItem.UnitPrice,
                UnitPriceSpecified = true,
                ExtendedPriceSpecified = true,
                ExtendedPrice = GetCartItemPrice(cartItem, cartQuantity),
                Seller = new SellerType() { Company = _Settings.CompanyCode, PhysicalOrigin = GetOriginatorAddress() },
                Customer = new CustomerType { CustomerCode = GetCustomerCode(), Destination = GetDestinationAddress() },
                Product = new Product { productClass = GetTruncatedString(cartItem.SKU), Value = GetTruncatedString(cartItem.Product?.Name) },
                Quantity = new MeasureType() { Value = cartQuantity },
                FlexibleFields = GetFlexibleFields(cartItem),
                Discount = GetDiscount(cartItem),               
            };
        }

        string GetTruncatedString(string productName)
            => productName?.Length > 40 ? productName.Substring(0, 40) : productName;

        //Get the lines for invoice request.
        private LineItemISIType GetLineISIType(ReturnOrderLineItemModel cartItem, int lineNumber)
        {
            decimal cartQuantity = cartItem.GroupProducts?.Count > 0 ? cartItem.GroupProducts[0].Quantity : cartItem.Quantity;
            return new LineItemISIType()
            {
                lineItemNumber = lineNumber.ToString(),
                taxDate = GetOrderDate(),
                postingDate = DateTime.Now,
                transactionType = SaleTransactionType.SALE,
                UnitPrice = -(cartItem.UnitPrice),
                UnitPriceSpecified = true,
                ExtendedPriceSpecified = true,
                ExtendedPrice = -(GetReturnCartItemPrice(cartItem, cartQuantity)),
                Seller = new SellerType() { Company = _Settings.CompanyCode, PhysicalOrigin = GetOriginatorAddress() },
                Customer = new CustomerType { CustomerCode = GetCustomerCode(), Destination = GetDestinationAddress() },
                Product = new Product { productClass = GetTruncatedString(cartItem.SKU), Value = GetTruncatedString(cartItem.ProductName) },
                Quantity = new MeasureType() { Value = cartQuantity },
            };
        }

        //Get the shipping line item.
        private LineItemISIType GetShippingLineISIType()
            => new LineItemISIType()
            {
                Product = new Product() { Value = "Shipping" },
                ExtendedPrice = IsCalculateTaxAfterDiscount() ? ShoppingCart.ShippingCost - ShoppingCart.ShippingDiscount : ShoppingCart.ShippingCost,
                ExtendedPriceSpecified = true,
                taxDate = GetOrderDate(),
            };

        //Get the return shipping line item.
        private LineItemISIType GetReturnShippingLineISIType(decimal ShippingCost, decimal returnShippingDiscount)
           => new LineItemISIType()
           {
               Product = new Product() { Value = "Shipping" },
               ExtendedPrice = -(GetReturnShippingCost(ShippingCost, returnShippingDiscount)),
               ExtendedPriceSpecified = true,
               taxDate = GetOrderDate(),
           };
        //Get the tax request object.
        private calculateTaxRequest GetTaxRequest()
            => new calculateTaxRequest() { VertexEnvelope = new VertexEnvelope() { Login = new LoginType() { UserName = _Settings.UserName, Password = _Settings.Password } } };

        //Get the discount.
        private Discount GetDiscount(ZnodeShoppingCartItem cartItem)
            => new Discount { ItemElementName = ItemChoiceType.DiscountAmount, Item = GetLineItemDiscount(cartItem) };

        //Get the destination address.
        private LocationType GetDestinationAddress()
            => GetLocation(ShoppingCart.Payment?.ShippingAddress ?? new AddressModel());

        //Get originator address.
        private LocationType GetOriginatorAddress()
        {
            IZnodeTaxHelper taxHelper = GetService<IZnodeTaxHelper>();
            AddressModel portalWareHouseAddressModel = taxHelper.GetPortalShippingAddress(ShoppingCart.PortalId.GetValueOrDefault());
            return GetLocation(portalWareHouseAddressModel);
        }

        //Get the address location.
        private LocationType GetLocation(AddressModel address)
        => !Equals(address, null) ?
                 new LocationType
                 {
                     City = address.CityName,
                     Country = address.CountryName,
                     StreetAddress1 = address.Address1,
                     StreetAddress2 = address.Address2,
                     PostalCode = address.PostalCode,
                     MainDivision = address.StateCode
                 } :
             null;

        //Get the currency.
        private CurrencyType GetCurrency()
            => new CurrencyType { isoCurrencyCodeAlpha = GetCurrencyCode(ShoppingCart.PortalId.GetValueOrDefault()) };

        //Set the vertex settings.
        private void SetVertexSetting(int? portalId)
        {
            if (portalId > 0)
            {
                //getting the settings out of the first tax class.  The system is currently setup to use multiple avatax class identifiers for classes, but should not have separate avatax account settings for each store
                IZnodeRepository<ZnodeTaxPortal> taxPortalRepository = new ZnodeRepository<ZnodeTaxPortal>();
                ZnodeTaxPortal setting = taxPortalRepository.Table.FirstOrDefault(x => x.PortalId == portalId);
                if (!Equals(setting, null))
                    _Settings = new VertexSetting()
                    {
                        UserName = setting.Custom1,
                        Password = setting.Custom2
                    };
            }
        }

        private void LogErrorMessage(string errorMessage)
           // Log Activity            
           => ZnodeLogging.LogMessage(errorMessage, "Vertex");

        //Get flexible fields.
        private FlexibleFields GetFlexibleFields(ZnodeShoppingCartItem cartItem)
        {
            List<string> flexFieldsToPassInVertex = ZnodeApiSettings.FlexFieldsToPassInVertex?.Split(',')?.ToList();
            if (flexFieldsToPassInVertex?.Count > 0 && HelperUtility.IsNotNull(cartItem))
            {
                List<FlexibleFieldsFlexibleCodeField> flexibleFields = new List<FlexibleFieldsFlexibleCodeField>();
                OrderAttributeModel orderAttributeModel;
                int index = 0;
                foreach (string field in flexFieldsToPassInVertex)
                {
                    if (cartItem.Product?.ZNodeGroupProductCollection?.Count > 0)
                    {
                        orderAttributeModel = cartItem.Product.ZNodeGroupProductCollection[0].Attributes?.FirstOrDefault(x => x.AttributeCode == field);
                        if (HelperUtility.IsNotNull(orderAttributeModel))
                            flexibleFields.Add(new FlexibleFieldsFlexibleCodeField() { fieldId = index.ToString(), Value = orderAttributeModel.AttributeValue });
                    }
                    else
                    {
                        orderAttributeModel = cartItem.Product?.Attributes?.FirstOrDefault(x => x.AttributeCode == field);
                        if (HelperUtility.IsNotNull(orderAttributeModel))
                            flexibleFields.Add(new FlexibleFieldsFlexibleCodeField() { fieldId = index.ToString(), Value = orderAttributeModel.AttributeValue });
                    }
                    index++;
                }

                if (flexibleFields.Count > 0)
                    return new FlexibleFields() { FlexibleCodeField = flexibleFields.ToArray() };
            }
            return null;
        }

        //Get order Date to set documentDate
        private DateTime GetOrderDate()
        {
            DateTime orderDate = DateTime.Now;
            if (ShoppingCart?.OrderId > 0)
            {
                IZnodeRepository<ZnodeOmsOrder> znodeOmsOrder = new ZnodeRepository<ZnodeOmsOrder>();
                ZnodeOmsOrder omsOrder = znodeOmsOrder.Table.FirstOrDefault(x => x.OmsOrderId == ShoppingCart.OrderId);
                orderDate = omsOrder.CreatedDate;
            }
            return orderDate;
        }
        #endregion
    }

    //Class holds the vertex settings.
    public class VertexSetting
    {
        public string CompanyCode { get; set; }
        public string TrustedId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
