using System;
using System.Linq;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using Avalara.AvaTax.RestClient;
using Znode.Libraries.Resources;
using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Taxes
{
    public class AvataxHelper : IAvataxHelper
    {
        private AvataxSettings avaSettings;

        // To get the order state based on order Id.
        public virtual string GetOrderState(int orderId, out string orderNumber, out DateTime createdDate)
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
                                })?.FirstOrDefault();

                if (!Equals(currency, null))
                {
                    orderNumber = currency.OrderNumber;
                    createdDate = currency.CreatedDate;
                    return currency.OrderStateName;
                }
            }
            return string.Empty;
        }

        // To get the order number based on order Id.
        public virtual string GetOrderNumber(int omsOrderId)
        {
            if (omsOrderId > 0)
            {
                IZnodeRepository<ZnodeOmsOrder> _omsOrderRepository = new ZnodeRepository<ZnodeOmsOrder>();
                return _omsOrderRepository.Table.FirstOrDefault(x => x.OmsOrderId == omsOrderId)?.OrderNumber;
            }
            return string.Empty;
        }

        // To get the order number and date based on order Id.
        public virtual string GetOrderNumber(int omsOrderId, out DateTime orderDate)
        {
            orderDate = DateTime.Today;
            if (omsOrderId > 0)
            {
                IZnodeRepository<ZnodeOmsOrder> _omsOrderRepository = new ZnodeRepository<ZnodeOmsOrder>();
                ZnodeOmsOrder omsOrder = _omsOrderRepository.Table.FirstOrDefault(x => x.OmsOrderId == omsOrderId);
                orderDate = omsOrder.CreatedDate;
                return omsOrder?.OrderNumber;
            }
            return string.Empty;
        }

        // To check whether tax is exempt.
        public virtual bool IsTaxExempt(ZnodeShoppingCart shoppingCart, CreateTransactionModel avalaraTaxRequest)
        {
            bool profile;
            if (shoppingCart?.UserId > 0)
            {
                IZnodeRepository<ZnodeUserProfile> userProfile = new ZnodeRepository<ZnodeUserProfile>();
                IZnodeRepository<ZnodeProfile> ps = new ZnodeRepository<ZnodeProfile>();

                profile = (from shippingportal in userProfile.Table
                           join _profile in ps.Table on shippingportal.ProfileId equals _profile.ProfileId
                           where shippingportal.UserId == shoppingCart.UserId && shippingportal.IsDefault == true
                           select _profile.TaxExempt)?.FirstOrDefault() ?? false;

                avalaraTaxRequest.customerCode = shoppingCart.UserId.ToString();
            }
            else
            {
                profile = false;
                avalaraTaxRequest.customerCode = "00001"; //any value will do, it's not a real customer
            }

            return profile;
        }

        // To set avatax settings based on portal Id.
        public virtual AvataxSettings SetAvataxSettings(int portalId = 0)
        {
            if (portalId > 0)
            {
                avaSettings = GetAvataxSetting(portalId);

                //error if configuration is missing
                if ((string.IsNullOrEmpty(avaSettings.AvalaraAccount)) || (string.IsNullOrEmpty(avaSettings.AvalaraLicense)) || (string.IsNullOrEmpty(avaSettings.AvalaraServiceURL)))
                    throw new Exception(Api_Resources.ErrorCheckSettingsInConfigurationFile);
            }
            else
                throw new Exception(Api_Resources.ErrorPortalIdRequired);

            return avaSettings;
        }

        // Get avatax settings based on portal Id.
        public virtual AvataxSettings GetAvataxSetting(int portalId)
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
                    AvalaraServiceURL = setting.AvataxUrl,
                    AvataxIsTaxIncluded = setting.AvataxIsTaxIncluded.GetValueOrDefault()
                };
            }
            return avaSettings;
        }

        // Get the portal entity via portal Id.
        public virtual ZnodePortal GetPortalEntity(int currentPortalId)
        {
            IZnodeRepository<ZnodePortal> _portalRepository = new ZnodeRepository<ZnodePortal>();
            return _portalRepository.Table.FirstOrDefault(x => x.PortalId == currentPortalId);
        }

        // To get the currency code by portal Id.
        public virtual string GetCurrencyCode(int portalId)
        {
            IZnodeRepository<ZnodeCurrency> _currencyRepository = new ZnodeRepository<ZnodeCurrency>();
            IZnodeRepository<ZnodePortalUnit> _portalUnitRepository = new ZnodeRepository<ZnodePortalUnit>();

            string currencyCode = (from _znodePortalUnit in _portalUnitRepository.Table
                                   join _znodeCurrency in _currencyRepository.Table on _znodePortalUnit.CurrencyId equals _znodeCurrency.CurrencyId
                                   where _znodePortalUnit.PortalId == portalId
                                   select _znodeCurrency.CurrencyCode)?.FirstOrDefault();
            return currencyCode;
        }

        // To get the value of IsCalculateTaxAfterDiscount flag.
        public virtual bool IsCalculateTaxAfterDiscount()
        {
            return DefaultGlobalConfigHelper.IsCalculateTaxAfterDiscount;
        }
        
        // To get the order tax summary details 
        protected virtual List<ZnodeOmsTaxOrderSummary> GetSavedTaxSummary(int omsOrderDetailsId)
        {
            IZnodeRepository<ZnodeOmsTaxOrderSummary> _omsTaxOrderSummaryRepository = new ZnodeRepository<ZnodeOmsTaxOrderSummary>();
            return _omsTaxOrderSummaryRepository.Table.Where(x => x.OmsOrderDetailsId == omsOrderDetailsId).ToList();
        }   
        
        // To get the line item tax override amount for returns.
        public virtual List<TaxAmountOverrideModel> GetLineItemTaxOverrideAmount(ZnodeShoppingCart shoppingCart, List<ReturnOrderLineItemModel> returnItemList)
        {
            IZnodeRepository<ZnodeOmsTaxOrderLineDetail> _omsTaxOrderLineDetailReporitory = new ZnodeRepository<ZnodeOmsTaxOrderLineDetail>();

            List<int> returnLineItemsIds = returnItemList?.Select(y => y.OmsOrderLineItemsId).ToList();

            List<ZnodeOmsTaxOrderLineDetail> omsTaxOrderLineDetails = _omsTaxOrderLineDetailReporitory.Table
                .Where(x => returnLineItemsIds.Contains(x.OmsOrderLineItemsId))?.ToList();

            List<TaxAmountOverrideModel> taxAmountOverrideList = new List<TaxAmountOverrideModel>();

            foreach (ReturnOrderLineItemModel returnOrderLineItem in returnItemList)
            {
                TaxAmountOverrideModel taxAmountOverrideModel = new TaxAmountOverrideModel();
                taxAmountOverrideModel.SKU = returnOrderLineItem?.GroupProducts?.Count() > 0 ? returnOrderLineItem?.GroupProducts[0]?.Sku : returnOrderLineItem?.SKU;
                taxAmountOverrideModel.OmsOrderLineItemsId = returnOrderLineItem.OmsOrderLineItemsId;
                ZnodeOmsTaxOrderLineDetail savedItem = omsTaxOrderLineDetails?.FirstOrDefault(x => x.OmsOrderLineItemsId == returnOrderLineItem.OmsOrderLineItemsId);
                ZnodeShoppingCartItem calculatedItem = shoppingCart.ShoppingCartItems?.FirstOrDefault(x => x.OmsOrderLineItemId == returnOrderLineItem.OmsOrderLineItemsId);
                if (returnOrderLineItem?.GroupProducts?.Count() > 0)
                    taxAmountOverrideModel.TaxOverrideAmount = (savedItem?.SalesTax + savedItem?.ImportDuty).GetValueOrDefault() - (calculatedItem?.Product?.ZNodeGroupProductCollection[0]?.SalesTax + calculatedItem?.Product?.ZNodeGroupProductCollection[0]?.ImportDuty).GetValueOrDefault();
                else
                    taxAmountOverrideModel.TaxOverrideAmount = (savedItem?.SalesTax + savedItem?.ImportDuty).GetValueOrDefault() - (calculatedItem?.Product?.SalesTax + calculatedItem?.Product?.ImportDuty).GetValueOrDefault();
                taxAmountOverrideList.Add(taxAmountOverrideModel);
            }
            return taxAmountOverrideList;
        }

        // To get the line item tax override amount for order cancellation.
        public virtual List<TaxAmountOverrideModel> GetLineItemTaxOverrideAmount(ZnodeShoppingCart shoppingCart)
        {
            IZnodeRepository<ZnodeOmsTaxOrderLineDetail> _omsTaxOrderLineDetailReporitory = new ZnodeRepository<ZnodeOmsTaxOrderLineDetail>();

            List<int> omsOrderLineItemIds = shoppingCart?.ShoppingCartItems?.Select(y => y.OmsOrderLineItemId).ToList();

            List<ZnodeOmsTaxOrderLineDetail> znodeOmsTaxOrderLineDetails = _omsTaxOrderLineDetailReporitory.Table
                .Where(x => omsOrderLineItemIds.Contains(x.OmsOrderLineItemsId))?.ToList();

            List<TaxAmountOverrideModel> taxAmountOverrideList = new List<TaxAmountOverrideModel>();

            foreach (ZnodeShoppingCartItem shoppingCartItem in shoppingCart.ShoppingCartItems)
            {
                TaxAmountOverrideModel taxAmountOverrideModel = new TaxAmountOverrideModel();
                taxAmountOverrideModel.SKU = shoppingCartItem.SKU;
                taxAmountOverrideModel.OmsOrderLineItemsId = shoppingCartItem.OmsOrderLineItemId;
                ZnodeOmsTaxOrderLineDetail savedItem = znodeOmsTaxOrderLineDetails?.FirstOrDefault(x => x.OmsOrderLineItemsId == shoppingCartItem.OmsOrderLineItemId);
                taxAmountOverrideModel.TaxOverrideAmount = (savedItem?.SalesTax + savedItem?.ImportDuty).GetValueOrDefault();
                taxAmountOverrideList.Add(taxAmountOverrideModel);
            }
            return taxAmountOverrideList;
        }

        // Get the portal entity via portal Id.
        public virtual bool? IsSellerImporterOfDuty(int omsOrderId)
        {
            IZnodeRepository<ZnodeOmsTaxRule> _znodeOmsTaxRule = new ZnodeRepository<ZnodeOmsTaxRule>();
            return _znodeOmsTaxRule.Table.FirstOrDefault(x => x.OmsOrderId == omsOrderId)?.AvataxIsSellerImporterOfRecord;
        }

        //Update AvataxIsSellerImporterOfRecord If Value Is Null
        public virtual void UpdateIsSellerImporterOfDuty(int omsOrderId, bool isSellerImporterOfDuty)
        {
            IZnodeRepository<ZnodeOmsTaxRule> _znodeOmsTaxRule = new ZnodeRepository<ZnodeOmsTaxRule>();
            ZnodeOmsTaxRule znodeOmsTaxRule =  _znodeOmsTaxRule.Table.FirstOrDefault(x => x.OmsOrderId == omsOrderId);
            if (HelperUtility.IsNull(znodeOmsTaxRule.AvataxIsSellerImporterOfRecord))
            {
                znodeOmsTaxRule.AvataxIsSellerImporterOfRecord = isSellerImporterOfDuty;
                _znodeOmsTaxRule.Update(znodeOmsTaxRule);
            }
        }
    }
}
