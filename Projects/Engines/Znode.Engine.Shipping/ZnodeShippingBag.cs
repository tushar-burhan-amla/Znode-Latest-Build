using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Shipping
{
    // Property bag of settings used by the shipping types. 
    public class ZnodeShippingBag
    {
        private ZnodeGenericCollection<ZnodeShoppingCartItem> _shipSeparatelyItems;
        private ZnodeGenericCollection<ZnodeShoppingCartItem> _packageItems;

        public decimal HandlingCharge { get; set; }
        public decimal SubTotal { get; set; }
        public string ShippingCode { get; set; }
        public bool ApplyPackageItemHandlingCharge { get; set; }
        public ZnodeShoppingCart ShoppingCart { get; set; }
        public string HandlingBasedOn { get; set; }
        public ShippingRuleListModel ShippingRules { get; set; }

        public ZnodeGenericCollection<ZnodeShoppingCartItem> ShipSeparatelyItems
        {
            get
            {
                if (HelperUtility.IsNull(_shipSeparatelyItems))
                {
                    _shipSeparatelyItems = new ZnodeGenericCollection<ZnodeShoppingCartItem>();

                    foreach (ZnodeShoppingCartItem item in ShoppingCart.ShoppingCartItems)
                    {
                        if (item.Product.ShipSeparately && !item.Product.FreeShippingInd)
                            _shipSeparatelyItems.Add(item);
                    }
                }
                return _shipSeparatelyItems;
            }
        }

        public ZnodeGenericCollection<ZnodeShoppingCartItem> PackageItems
        {
            get
            {
                if (HelperUtility.IsNull(_packageItems))
                {
                    _packageItems = new ZnodeGenericCollection<ZnodeShoppingCartItem>();

                    foreach (ZnodeShoppingCartItem item in ShoppingCart.ShoppingCartItems)
                    {
                        if (!item.Product.FreeShippingInd)
                        {
                            _packageItems.Add(item);
                            ApplyPackageItemHandlingCharge = true;
                        }
                        _packageItems = GetPackageItems(item.Product.ZNodeGroupProductCollection);
                        _packageItems = GetPackageItems(item.Product.ZNodeAddonsProductCollection);
                        _packageItems = GetPackageItems(item.Product.ZNodeConfigurableProductCollection);
                    }
                }
                return _packageItems;
            }
        }

        /// <summary>
        /// Calculate shipping handling charge in percentage.
        /// </summary>
        /// <param name="totalShippingCost"></param>
        /// <returns></returns>
        public decimal CalculateShippingHandlingChargeInPercentage(decimal totalShippingCost, decimal percentage)
            => (totalShippingCost > 0.0M && percentage > 0.0M) ? ((percentage / 100) * totalShippingCost) : 0.0M;

        // Add group/configurable and addons product to packageItems.
        private ZnodeGenericCollection<ZnodeShoppingCartItem> GetPackageItems(ZnodeGenericCollection<ZnodeProductBaseEntity> productCollection)
        {
            if (productCollection?.Count > 0)
            {
                foreach (ZnodeProductBaseEntity itemproduct in productCollection)
                {
                    if (!itemproduct.FreeShippingInd)
                    {
                        _packageItems.Add(new ZnodeShoppingCartItem());
                        ApplyPackageItemHandlingCharge = true;
                    }
                    if (itemproduct.FreeShippingInd)
                        ApplyPackageItemHandlingCharge = false;
                }
            }
            return _packageItems;
        }
    }
}
