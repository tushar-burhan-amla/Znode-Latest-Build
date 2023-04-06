using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.ECommerce.ShoppingCart
{
    public class OrderInventoryManageHelper : IOrderInventoryManageHelper
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeOmsOrderLineItem> _orderLineItemRepository;
        private readonly IZnodeRepository<ZnodeOmsSavedCartLineItem> _savedCartLineItemRepository;
        public static Mutex MuTexLock = new Mutex();
        #endregion Private Variables

        #region Constructor

        public OrderInventoryManageHelper()
        {
            _orderLineItemRepository = new ZnodeRepository<ZnodeOmsOrderLineItem>();
            _savedCartLineItemRepository = new ZnodeRepository<ZnodeOmsSavedCartLineItem>();
        }
        #endregion Constructor

        /// <summary>
        /// Set backordering for the shopping cart items (webstore)
        /// </summary>
        /// <param name="ShoppingCartItems"></param>
        public virtual void SetBackOrderingForShoppingCart(ZnodeGenericCollection<ZnodeShoppingCartItem> ShoppingCartItems)
        {
            IEnumerable<ZnodeShoppingCartItem> shoppingCartItems = ShoppingCartItems?.Cast<ZnodeShoppingCartItem>();

            foreach (ZnodeShoppingCartItem item in shoppingCartItems)
            {
                if (IsDisablePurchasing(item.Product.InventoryTracking))
                {
                    item.Product.InventoryTracking = ZnodeConstant.AllowBackOrdering;
                }
                SetBackOrderingForBundleProduct(item);

                SetBackOrderingForAddOnProduct(item);

                SetBackOrderingForConfigurableProduct(item);

                SetBackOrderingForGroupProduct(item);
            }
        }


        //to check dont track inventory set to true
        protected virtual bool IsDisablePurchasing(string inventoryTracking)
        {
            return (inventoryTracking?.ToLower() == ZnodeConstant.DisablePurchasing.ToString().ToLower());
        }


        /// <summary>
        /// Set Back Ordering For Group Product
        /// </summary>
        /// <param name="item"></param>
        protected  void SetBackOrderingForGroupProduct(ZnodeShoppingCartItem item)
        {
            try
            {
                foreach (ZnodeProductBaseEntity group in item.Product.ZNodeGroupProductCollection)
                {
                    if (IsDisablePurchasing(group.InventoryTracking))
                    {
                        group.InventoryTracking = ZnodeConstant.AllowBackOrdering;
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SetBackOrderingForGroupProduct method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// Set Back Ordering For Configurable Product
        /// </summary>
        /// <param name="item"></param>
        protected  void SetBackOrderingForConfigurableProduct(ZnodeShoppingCartItem item)
        {
            try
            {
                foreach (ZnodeProductBaseEntity config in item.Product.ZNodeConfigurableProductCollection)
                {
                    if (IsDisablePurchasing(config.InventoryTracking))
                    {
                        config.InventoryTracking = ZnodeConstant.AllowBackOrdering;
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SetBackOrderingForConfigurableProduct method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// Set Back Ordering For AddOn Product
        /// </summary>
        /// <param name="item"></param>
        protected  void SetBackOrderingForAddOnProduct(ZnodeShoppingCartItem item)
        {
            try
            {
                foreach (ZnodeProductBaseEntity addon in item.Product.ZNodeAddonsProductCollection)
                {
                    if (IsDisablePurchasing(addon.InventoryTracking))
                    {
                        addon.InventoryTracking = ZnodeConstant.AllowBackOrdering;
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SetBackOrderingForAddOnProduct method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// Set Back Ordering For Bundle Product
        /// </summary>
        /// <param name="item"></param>
        protected  void SetBackOrderingForBundleProduct(ZnodeShoppingCartItem item)
        {
            try
            {
                if (item.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Bundles &&  IsDisablePurchasing(item.Product.InventoryTracking))
                {
                    item.Product.InventoryTracking = ZnodeConstant.AllowBackOrdering;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SetBackOrderingForBundleProduct method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// Set backordering for the shopping cart items (admin)
        /// </summary>
        /// <param name="shoppingCartItems"></param>
        public virtual void SetBackOrderingForShoppingCart(List<ShoppingCartItemModel> shoppingCartItems, OrderModel model)
        {
            try
            {
                Parallel.ForEach(shoppingCartItems, item =>
                {
                    item.InventoryTracking = model.OrderLineItems.FirstOrDefault(x => x.OmsOrderLineItemsId == item.OmsOrderLineItemsId)?.InventoryTracking;
                    item.InventoryTracking = model.OrderLineItems.FirstOrDefault(x => x.OmsOrderLineItemsId == item.OmsOrderLineItemsId)?.Attributes.FirstOrDefault(x => x.AttributeCode == "OutOfStockOptions")?.AttributeValueCode;
                    item.TrackInventory = false;
                });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error in SetBackOrderingForShoppingCart method is " + ex.Message, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                throw;
            }
        }


    }
}
