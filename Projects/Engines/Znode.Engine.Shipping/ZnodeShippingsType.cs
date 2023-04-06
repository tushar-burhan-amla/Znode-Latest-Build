using System;
using System.Collections.Generic;
using System.Diagnostics;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.Framework.Business;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using System.Linq;

namespace Znode.Engine.Shipping
{
    // This is the base class for all shipping types.
    public class ZnodeShippingsType : IZnodeShippingsType
    {
        private string _className;
        private List<ZnodeShippingControl> _controls;
        private ZnodeGenericCollection<ZnodeShoppingCartItem> _shipSeparatelyItems;
        private ZnodeGenericCollection<ZnodeShoppingCartItem> _shipTogetherItems;
        private string _weightUnit;
        public string WeightUnitLbs { get; set; } = "LB";
        public string WeightUnitKgs { get; set; } = "KG";
        public decimal WeightLimit { get; set; }
        public const string PUBLISHSTATUS = "Publish";

        /// <summary>
        /// Get Publish State Id - If PublishStateId is 0 then it will take default publish state.
        /// 
        /// </summary>
        /// <param name="publishStateId">publishstateId</param>
        /// <returns>int- publish state id of store</returns>
        public int GetPublishStateId(int publishStateId)
        {
            if (publishStateId != 0)
                return publishStateId;
            IZnodeRepository<ZnodePublishState> publishStateRepository = new ZnodeRepository<ZnodePublishState>();

            return Convert.ToInt16(publishStateRepository.Table.FirstOrDefault(m => m.StateName == PUBLISHSTATUS)?.PublishStateId);
        }

        public string ClassName
        {
            get
            {
                if (String.IsNullOrEmpty(_className))
                    _className = GetType().Name;

                return _className;
            }
        }

        public string WeightUnitBase
        {
            get
            {
                if (String.IsNullOrEmpty(_weightUnit))
                    _weightUnit = GetType().Name;

                return _weightUnit;
            }

            set { _weightUnit = value; }
        }

        public List<ZnodeShippingControl> Controls
        {
            get { return _controls ?? (_controls = new List<ZnodeShippingControl>()); }
        }

        public ZnodeGenericCollection<ZnodeShoppingCartItem> ShipSeparatelyItems
        {
            get { return _shipSeparatelyItems ?? (_shipSeparatelyItems = new ZnodeGenericCollection<ZnodeShoppingCartItem>()); }
        }

        public ZnodeGenericCollection<ZnodeShoppingCartItem> ShipTogetherItems
        {
            get { return _shipTogetherItems ?? (_shipTogetherItems = new ZnodeGenericCollection<ZnodeShoppingCartItem>()); }
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public int Precedence { get; set; }
        public ZnodeShoppingCart ShoppingCart { get; set; }
        public ZnodeShippingBag ShippingBag { get; set; }

        /// <summary>
        /// Returns a generic shipping error message.
        /// </summary>
        /// <returns>A generic error message used by any shipping type.</returns>
        public virtual string GenericShippingErrorMessage() => "Unable to calculate shipping rates at this time, please try again later.";


        /// <summary>
        /// Binds the shopping cart and shipping data to the shipping type.
        /// </summary>
        /// <param name="shoppingCart">The current shopping cart.</param>
        /// <param name="shippingBag">The shipping properties.</param>
        public virtual void Bind(ZnodeShoppingCart shoppingCart, ZnodeShippingBag shippingBag)
        {
            ShoppingCart = shoppingCart;
            ShippingBag = shippingBag;
        }

        public virtual void ResetShippingItems()
        {
            _shipTogetherItems = new ZnodeGenericCollection<ZnodeShoppingCartItem>();
            _shipSeparatelyItems = new ZnodeGenericCollection<ZnodeShoppingCartItem>();
        }

        // Calculates the shipping rates and updates the shopping cart.
        public virtual void Calculate() { }

        public virtual List<ShippingModel> GetEstimateRate(List<ZnodeShippingBag> shippingbagList) => new List<ShippingModel>();

        /// <summary>
        /// Process anything that must be done before the order is submitted.
        /// </summary>
        /// <returns>True if everything is good for submitting the order; otherwise, false.</returns>
         // Most shipping types don't need any special verification
        public virtual bool PreSubmitOrderProcess() => true;


        /// <summary>
        /// Process anything that must be done after the order is submitted.
        /// </summary>
        public virtual void PostSubmitOrderProcess()
        {
            // Most shipping types don't need any further processing after the order is submitted
        }

        // Helper method to split the ship-separately items from the ship-together items in the shopping cart.
        public virtual void SplitShipSeparatelyFromShipTogether()
        {
            foreach (ZnodeShoppingCartItem cartItem in ShippingBag.ShoppingCart.ShoppingCartItems)
            {
                var hasWeight = false;
                if (cartItem.Product.FreeShippingInd)
                {
                    foreach (ZnodeProductBaseEntity addOn in cartItem.Product.ZNodeAddonsProductCollection)
                    {
                        if (!addOn.FreeShippingInd && addOn.Weight > 0)
                        {
                            hasWeight = true;
                            break;
                        }
                    }
                }

                if (cartItem.Product?.ZNodeGroupProductCollection?.Count < 1 && cartItem.Product?.ZNodeConfigurableProductCollection?.Count < 1)
                    SetProductItemForShipSeparatelyItems(cartItem, hasWeight);

                ShipSeparatelyForGroupAddonConfigureProduct(cartItem.Product.ZNodeGroupProductCollection, hasWeight, cartItem, true);
                ShipSeparatelyForGroupAddonConfigureProduct(cartItem.Product.ZNodeConfigurableProductCollection, hasWeight, cartItem);
                ShipSeparatelyForGroupAddonConfigureProduct(cartItem.Product.ZNodeAddonsProductCollection, hasWeight, cartItem);
            }
        }

        /// <summary>
        /// This function split the PackageWeight in elements according to specified packageWeight limit
        /// </summary>
        /// <param name="packageWeight">packageWeight</param>
        /// <param name="weightLimit">weightLimit</param>
        /// <returns>List of splitted packageWeight</returns>
        public List<decimal> SplitPackageWeight(decimal packageWeight, decimal weightLimit)
        {
            try
            {
                decimal actualWeight = packageWeight;

                int count = 0;
                List<decimal> weightList = new List<decimal>();

                while (actualWeight > weightLimit)
                {
                    decimal extraWeight = 0;
                    extraWeight = actualWeight - weightLimit;
                    actualWeight = actualWeight - extraWeight;
                    weightList.Add(actualWeight);
                    actualWeight = extraWeight;
                    count++;
                }

                if (actualWeight > 0)
                    weightList.Add(actualWeight);

                return weightList;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, "Shipping", TraceLevel.Info);
                throw;
            }
        }

        /// <summary>
        /// This function set the SKU in case of Group addon and configurable product.
        /// </summary>
        /// <param name="productCollection">productCollection</param>
        /// <param name="shipSeparatelyItems">shipSeparatelyItems</param>
        /// <param name="shoppingCartItem">shoppingCartItem</param>
        public virtual void ShipSeparatelyForGroupAddonConfigureProduct(ZnodeGenericCollection<ZnodeProductBaseEntity> productCollection, ZnodeShoppingCartItem shipSeparatelyItems, ZnodeShoppingCartItem shoppingCartItem)
        {
            if (productCollection?.Count > 0)
                foreach (ZnodeProductBaseEntity item in productCollection)
                {
                    if (!string.IsNullOrEmpty(item.SKU) && Equals(item.SKU, shipSeparatelyItems.Product?.SKU))
                        item.ShippingCost = shipSeparatelyItems.ShippingCost;
                }
        }

        /// <summary>        
        /// This function will convert the Package Weight from KG to LBs.
        /// To convert from KG to LBs the formula will be
        /// Lbs = kg * 2.20462262
        /// </summary>
        /// <returns>calculated weight in lbs</returns>
        public decimal ConvertWeightKgToLbs(decimal packageWeight) => (WeightUnitBase.Equals(WeightUnitKgs)) ? (packageWeight * (decimal)2.20462262) : packageWeight;



        // Set items for ship-separately.
        private void SetProductItemForShipSeparatelyItems(ZnodeShoppingCartItem cartItem, bool hasWeight)
        {
            if (cartItem.Product.Weight > 0 )
            {
                if (!cartItem.Product.FreeShippingInd || hasWeight)
                    ShipSeparatelyItems.Add(cartItem);
            }
        }

        // Set group item and addon items for ship-separately.
        private void SetProductItemForShipSeparatelyItems(ZnodeProductBaseEntity product, bool hasWeight, ZnodeShoppingCartItem cartItem, bool isGroupProduct = false)
        {
            ZnodeShoppingCartItem zNodeShoppingCartItem = new ZnodeShoppingCartItem();
            if (product.Weight > 0)
            {
                if (!product.FreeShippingInd || hasWeight)
                {
                    zNodeShoppingCartItem.Product = product;
                    zNodeShoppingCartItem.Quantity = SetQuantityForGroupProduct(isGroupProduct, product, cartItem);
                    ShipSeparatelyItems.Add(zNodeShoppingCartItem);
                }
            }
        }

        // Used for shipseparately for group/addon/configure product.
        private void ShipSeparatelyForGroupAddonConfigureProduct(ZnodeGenericCollection<ZnodeProductBaseEntity> productCollection, bool hasWeight, ZnodeShoppingCartItem cartItem, bool isGroupProduct = false)
        {
            foreach (ZnodeProductBaseEntity groupItem in productCollection)
            {
                SetProductItemForShipSeparatelyItems(groupItem, hasWeight, cartItem, isGroupProduct);
            }
        }

        // Get group product quantity.
        private decimal SetQuantityForGroupProduct(bool isGroupProduct, ZnodeProductBaseEntity product, ZnodeShoppingCartItem cartItem)
        {
            return isGroupProduct ? product.SelectedQuantity : cartItem.Quantity;
        }


    }
}
