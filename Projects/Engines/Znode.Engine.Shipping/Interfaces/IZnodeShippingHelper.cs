using System;
using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;

namespace Znode.Engine.Shipping
{
    public interface IZnodeShippingHelper
    {
        /// <summary>
        /// Get shipping rule list on basis on rulename and country code.        
        /// </summary>
        List<ShippingRuleModel> GetShippingRuleList(string ruleName, string countryCode, int portalId = 0, int shippingId = 0);

        /// <summary>
        /// Get order level shipping handling charge.        
        /// </summary>
        decimal GetOrderLevelShippingHandlingCharge(ZnodeShippingBag shippingBag, ZnodeShoppingCart shoppingCart);

        /// <summary>
        /// Get Item handling charge.    
        /// </summary>
        decimal GetItemHandlingCharge(decimal itemShippingCost, ZnodeShippingBag shippingBag);

        /// <summary>
        /// Get portal shipping on basis on portal id.      
        /// </summary>
        PortalShippingModel GetPortalShipping(int portalId, int publishStateId);

        /// <summary>
        /// Get item ship together handling charge.    
        /// </summary>
        decimal GetShipTogetherItemsHandlingCharge(ZnodeShippingBag shippingBag, decimal itemShippingRate);

        /// <summary>
        /// Get item ship separately handling charge    
        /// </summary>
        decimal GetShipSeparatelyItemsHandlingCharge(ZnodeShippingBag shippingBag, ZnodeShoppingCartItem separateItem, decimal itemShippingRate);

        /// <summary>
        /// Get string value of enum.   
        /// </summary>
        string GetShippingRuleTypesEnumValue(string enumString);

        string GetUPSLTLServiceCodeEnumValue(string enumString);

        /// <summary>
        /// Get ware house default address on the basis of portal id.    
        /// </summary>
        AddressModel GetPortalWareHouseAddress(int portalId);

        /// <summary>
        /// Get shipping origin address on the basis of portal id.  
        /// </summary>
        AddressModel GetPortalShippingOriginAddress(int portalId);

        /// <summary>
        /// Get portal shipping address.  
        /// </summary>
        AddressModel GetPortalShippingAddress(int portalId);

        /// <summary>
        /// Get country name by country code.
        /// </summary>
        string GetCountryByCountryCode(string countryCode);

        /// <summary>
        /// Get state code by stateName
        /// </summary>
        string GetStateCode(string stateName);

        /// <summary>
        /// Set error message for shipping.
        /// </summary>
        ZnodeShoppingCart SetShippingErrorMessage(ZnodeShoppingCart shoppingCart);

        /// <summary>
        /// Get shipping rule list.
        /// </summary>
        List<ShippingRuleModel> GetPortalProfileShippingRuleList(string shippingCountryCode, int shippingId, int? portalId = 0, int? profileId = 0, int? userId = 0);

        /// <summary>
        /// Get shipping details.
        /// </summary>
        ShippingModel GetPortalProfileShipping(string shippingCountryCode, int shippingID, int? portalId = 0, int? profileId = 0, int? userId = 0);


        PortalModel GetPortal(int portalId);

        DateTime GetPickUpDate();

        DateTime GetUPSPickUpDate(DateTime pickupDate);
    }
}
