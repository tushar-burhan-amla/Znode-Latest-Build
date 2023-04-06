using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Libraries.Admin
{
    public class ZnodeOrderHelper : IZnodeOrderHelper
    {
        #region Private Variables

        private readonly IZnodeRepository<ZnodeOmsOrder> _orderRepository;
        private readonly IZnodeRepository<ZnodeOmsOrderDetail> _orderDetailRepository;
        private readonly IZnodeRepository<ZnodeOmsOrderLineItem> _orderLineItemRepository;
        private readonly IZnodeRepository<ZnodeOmsOrderShipment> _orderShipmentRepository;
        private readonly IZnodeRepository<ZnodeOmsCookieMapping> _cookieMappingRepository;
        private readonly IZnodeRepository<ZnodeOmsTaxOrderDetail> _taxOrderDetailsRepository;
        private readonly IZnodeRepository<ZnodeOmsTaxOrderLineDetail> _taxOrderLineRepository;
        private readonly IZnodeRepository<ZnodeGiftCard> _giftCardRepository;
        private readonly IZnodeRepository<ZnodeGiftCardHistory> _giftCardHistoryRepository;
        private readonly IZnodeRepository<ZnodePromotion> _promotionRepository;
        private readonly IZnodeRepository<ZnodePromotionCoupon> _promotionCouponRepository;
        private readonly IZnodeRepository<ZnodeOmsOrderDiscount> _omsOrderDiscountRepository;
        private readonly IZnodeRepository<ZnodeOmsDiscountType> _omsDiscountTypeRepository;
        private readonly IZnodeRepository<ZnodeOmsOrderAttribute> _omsOrderAttributeRepository;
        private readonly IZnodeRepository<ZnodePimAttribute> _pimAttribute;
        private readonly IZnodeRepository<ZnodePimAttributeLocale> _pimAttributeLocale;
        private readonly IZnodeRepository<ZnodeLocale> _localeRepository;
        private readonly IZnodeRepository<ZnodeOmsOrderState> _orderStateRepository;
        private readonly IZnodeRepository<ZnodePortal> _portalRepository;
        private readonly IZnodeRepository<ZnodeGlobalSetting> _globalSettingRepository;
        private readonly IZnodeRepository<ZnodePublishProductEntity> _publishProductEntity;
        private readonly IZnodeRepository<ZnodeOmsFailedOrderPayment> _omsFailedOrderPayment;

        #endregion Private Variables

        #region Public Constructor

        public ZnodeOrderHelper(int? webstoreVersionId = null, int? catalogVersionId = null)
        {
            _orderRepository = new ZnodeRepository<ZnodeOmsOrder>();
            _orderDetailRepository = new ZnodeRepository<ZnodeOmsOrderDetail>();
            _orderLineItemRepository = new ZnodeRepository<ZnodeOmsOrderLineItem>();
            _orderShipmentRepository = new ZnodeRepository<ZnodeOmsOrderShipment>();
            _cookieMappingRepository = new ZnodeRepository<ZnodeOmsCookieMapping>();
            _taxOrderDetailsRepository = new ZnodeRepository<ZnodeOmsTaxOrderDetail>();
            _taxOrderLineRepository = new ZnodeRepository<ZnodeOmsTaxOrderLineDetail>();
            _giftCardRepository = new ZnodeRepository<ZnodeGiftCard>();
            _giftCardHistoryRepository = new ZnodeRepository<ZnodeGiftCardHistory>();
            _promotionRepository = new ZnodeRepository<ZnodePromotion>();
            _promotionCouponRepository = new ZnodeRepository<ZnodePromotionCoupon>();
            _omsOrderDiscountRepository = new ZnodeRepository<ZnodeOmsOrderDiscount>();
            _omsDiscountTypeRepository = new ZnodeRepository<ZnodeOmsDiscountType>();
            _omsOrderAttributeRepository = new ZnodeRepository<ZnodeOmsOrderAttribute>();
            _pimAttribute = new ZnodeRepository<ZnodePimAttribute>();
            _pimAttributeLocale = new ZnodeRepository<ZnodePimAttributeLocale>();
            _localeRepository = new ZnodeRepository<ZnodeLocale>();
            _orderStateRepository = new ZnodeRepository<ZnodeOmsOrderState>();
            _portalRepository = new ZnodeRepository<ZnodePortal>();
            _globalSettingRepository = new ZnodeRepository<ZnodeGlobalSetting>();
            _publishProductEntity = new ZnodeRepository<ZnodePublishProductEntity>(HelperMethods.Context);
            _omsFailedOrderPayment = new ZnodeRepository<ZnodeOmsFailedOrderPayment>();


        }

        #endregion Public Constructor

        #region Public Method

        [Obsolete("This method is not in use now and will be discontinued in upcoming versions, as the parent method ZnodeOrderFulfillment.AddOrderToDatabase has been enhanced.")]
        //save order details
        public virtual int SaveOrderDetails(OrderModel order)
        {
            bool saveNotes = true;
            if (order.OmsOrderId == 0)
            {
                int orderId = SaveOrderId(order);
                order.OmsOrderId = orderId;
            }
            else
            {
                saveNotes = false;
                SetOrderData(order);
            }
            order.DiscountAmount += order.CSRDiscountAmount;
            var orderEntity = order.ToEntity<ZnodeOmsOrderDetail>();
            orderEntity = ToOrderDetailEntity(orderEntity, order.BillingAddress);
            ZnodeOmsOrderDetail orderDetail = _orderDetailRepository.Insert(orderEntity);
            order.OmsOrderDetailsId = orderDetail.OmsOrderDetailsId;

            if (saveNotes)
                SaveOrderNotes(order.OmsOrderDetailsId, order.AdditionalInstructions);

            return orderDetail.OmsOrderId;
        }

        [Obsolete("This method is not in use now and will be discontinued in upcoming versions, as the parent method ZnodeOrderFulfillment.AddOrderToDatabase has been enhanced.")]
        //save order line items
        public virtual int SaveOrderLineItem(OrderLineItemModel orderLineItem)
        {
            int userId = HelperMethods.GetLoginAdminUserId();
            orderLineItem.CreatedBy = userId > 0 ? userId : orderLineItem.CreatedBy;
            orderLineItem.ModifiedBy = userId > 0 ? userId : orderLineItem.ModifiedBy;
            ZnodeOmsOrderLineItem lineItems = _orderLineItemRepository.Insert(orderLineItem.ToEntity<ZnodeOmsOrderLineItem>());

            orderLineItem.OmsOrderLineItemsId = lineItems.OmsOrderLineItemsId;

            //to save order line item partial refund amount
            SavePartialDiscount(lineItems);

            //to save order attributes
            SaveOrderAttributes(orderLineItem);

            if (orderLineItem?.OrderLineItemCollection?.Count > 0)
            {
                orderLineItem.OrderLineItemCollection = orderLineItem.OrderLineItemCollection
                                                        .OrderByDescending(m => m.OrderLineItemRelationshipTypeId).ToList();
                foreach (OrderLineItemModel childLineItems in orderLineItem.OrderLineItemCollection)
                {
                    childLineItems.OmsOrderDetailsId = orderLineItem.OmsOrderDetailsId;
                    BindParentOmsOrderLineItemId(orderLineItem, childLineItems);
                    childLineItems.OrderLineItemStateId = orderLineItem.OrderLineItemStateId;
                    childLineItems.Description = childLineItems.Description == string.Empty ? orderLineItem.Description : childLineItems.Description;
                    ZnodeOmsOrderLineItem childLineItem = _orderLineItemRepository.Insert(childLineItems.ToEntity<ZnodeOmsOrderLineItem>());
                    childLineItems.OmsOrderLineItemsId = childLineItem.OmsOrderLineItemsId;
                    //to save order attributes
                    SaveOrderAttributes(childLineItems);
                    //to save order line item partial refund amount
                    SavePartialDiscount(childLineItem);
                }
            }

            //Save tax order line item.
            SaveTaxOrderLine(orderLineItem);

            //Save Personalize attributes
            SavePersonalizeItem(orderLineItem.OmsOrderLineItemsId, orderLineItem);

            List<OrderLineItemAdditionalCostModel> orderLineAdditionalCostList = new List<OrderLineItemAdditionalCostModel>();
            //Save additional cost per order line item
            if (orderLineItem.AdditionalCost?.Count > 0)
            {
                foreach (KeyValuePair<string, decimal> additionalCost in orderLineItem?.AdditionalCost)
                {
                    OrderLineItemAdditionalCostModel orderLineAdditionalCost = new OrderLineItemAdditionalCostModel();
                    orderLineAdditionalCost.OmsOrderLineItemsId = orderLineItem.OmsOrderLineItemsId;
                    orderLineAdditionalCost.KeyName = additionalCost.Key;
                    orderLineAdditionalCost.KeyValue = additionalCost.Value;
                    orderLineAdditionalCostList.Add(orderLineAdditionalCost);
                }
                IZnodeRepository<ZnodeOmsOrderLineItemsAdditionalCost> _znodeOrderLineAdditionalCostRepository = new ZnodeRepository<ZnodeOmsOrderLineItemsAdditionalCost>();
                _znodeOrderLineAdditionalCostRepository.Insert(orderLineAdditionalCostList.ToEntity<ZnodeOmsOrderLineItemsAdditionalCost>());
            }

            return orderLineItem.OmsOrderLineItemsId;
        }

        public virtual void BindParentOmsOrderLineItemId(OrderLineItemModel orderLineItem, OrderLineItemModel childLineItems)
        {
            if (childLineItems.OrderLineItemRelationshipTypeId == Convert.ToInt32(ZnodeCartItemRelationshipTypeEnum.AddOns))
            {
                childLineItems.ParentOmsOrderLineItemsId = orderLineItem.OrderLineItemCollection.Where(m => m.Sku.Equals(childLineItems.ParentProductSKU)).Select(m => m.OmsOrderLineItemsId).FirstOrDefault();
                orderLineItem.Description = childLineItems.Description;
                if (childLineItems.ParentOmsOrderLineItemsId == 0)
                    childLineItems.ParentOmsOrderLineItemsId = orderLineItem.OmsOrderLineItemsId;
            }
            else
            {
                childLineItems.ParentOmsOrderLineItemsId = orderLineItem.OmsOrderLineItemsId;
            }
            childLineItems.ShipDate = orderLineItem.ShipDate;
        }

        [Obsolete("This method is not in use now and will be discontinued in upcoming versions, as the parent method ZnodeOrderFulfillment.AddOrderToDatabase has been enhanced.")]
        //to save order discount
        public virtual bool SaveOrderDiscount(List<OrderDiscountModel> orderDiscount)
        {
            IEnumerable<ZnodeOmsOrderDiscount> omsOrderDiscounts = _omsOrderDiscountRepository.Insert(orderDiscount.ToEntity<ZnodeOmsOrderDiscount>());
            return omsOrderDiscounts?.Count() > 0;
        }

        [Obsolete("This method is not in use now and will be discontinued in upcoming versions, as the parent method ZnodeOrderFulfillment.AddOrderToDatabase has been enhanced.")]
        //to save order line item attributes
        public virtual bool SaveOrderAttributes(OrderLineItemModel orderLineItem)
        {
            bool isSuccess = true;
            List<OrderAttributeModel> orderAttribute = MapLineItemAttribute(orderLineItem);
            if (orderAttribute?.Count > 0)
            {
                IEnumerable<ZnodeOmsOrderAttribute> omsOrderDiscounts = _omsOrderAttributeRepository.Insert(orderAttribute.ToEntity<ZnodeOmsOrderAttribute>());
                isSuccess = omsOrderDiscounts?.Count() > 0;
            }
            return isSuccess;
        }

        [Obsolete("This method is not in use now and will be discontinued in upcoming versions, as these changes have been incorporated in ZnodeOrderFulfillment.AddOrderToDatabase.")]
        //to cancel order by orderId and giftcardNumber
        public virtual bool CancelOrderById(int orderId, string giftcardNumber = "")
        {
            bool status = false;
            ZnodeOmsOrderDetail order = _orderDetailRepository.Table.FirstOrDefault(x => x.OmsOrderId == orderId && x.IsActive);
            order.IsActive = false;


            order.OmsOrderStateId = Convert.ToInt32(_orderStateRepository.Table.FirstOrDefault(x => x.OrderStateName == ZnodeOrderStatusEnum.CANCELED.ToString())?.OmsOrderStateId);
            status = _orderDetailRepository.Update(order);
            if (status)
            {
                List<ZnodeOmsOrderLineItem> orderLineItem = _orderLineItemRepository.Table.Where(x => x.OmsOrderDetailsId == order.OmsOrderDetailsId && x.IsActive).ToList();
                foreach (ZnodeOmsOrderLineItem lineItem in orderLineItem)
                {
                    lineItem.IsActive = false;
                    status = _orderLineItemRepository.Update(lineItem);
                }
            }
            return status;
        }

        //to get order total by Id
        public virtual decimal GetOrderTotalById(int orderId)
         => _orderDetailRepository.Table.Where(x => x.OmsOrderId == orderId && x.IsActive)?.FirstOrDefault().Total ?? 0;

        //Save Order Notes.
        public virtual bool SaveOrderNotes(int omsOrderDetailsId, string note)
        {
            if (!string.IsNullOrEmpty(note))
            {
                IZnodeRepository<ZnodeOmsNote> _omsNoteRepository = new ZnodeRepository<ZnodeOmsNote>();
                ZnodeOmsNote omsNote = new ZnodeOmsNote();
                omsNote.Notes = note;
                omsNote.OmsOrderDetailsId = omsOrderDetailsId;
                ZnodeOmsNote notes = _omsNoteRepository.Insert(omsNote);
                return notes.OmsNotesId > 0;
            }
            return false;
        }

        //to order  payment details
        public virtual bool UpdateOrderPaymentDetails(OrderModel model)
        {
            bool status = false;
            ZnodeOmsOrderDetail order = _orderDetailRepository.GetById(model.OmsOrderDetailsId);
            order.PaymentSettingId = model.PaymentSettingId;
            order.PaymentTypeId = model.PaymentTypeId;
            order.OmsPaymentStateId = model.OmsPaymentStateId;
            order.PaymentTransactionToken = model.PaymentTransactionToken;
            status = _orderDetailRepository.Update(order);
            return status;
        }

        //to order shipping address
        public virtual int SaveShippingAddress(AddressModel model, int? shippingId, string emailId)
        {
            ZnodeOmsOrderShipment orderShipment = ToEntity(model);
            orderShipment.AddressId = model.AddressId;
            orderShipment.ShippingId = shippingId;
            orderShipment.ShipToEmailId = emailId;
            orderShipment = _orderShipmentRepository.Insert(orderShipment);
            return orderShipment.OmsOrderShipmentId;
        }

        // save order shipment details
        public virtual OrderShipmentModel SaveShippingAddressData(AddressModel addressModel, int? shippingId, string emailId)
        {
            try
            {
                ZnodeOmsOrderShipment orderShipment = ToEntity(addressModel);
                orderShipment.AddressId = addressModel.AddressId;
                orderShipment.ShippingId = shippingId;
                orderShipment.ShipToEmailId = emailId;
                orderShipment = _orderShipmentRepository.Insert(orderShipment);
                return orderShipment.ToModel<OrderShipmentModel>();
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage($"Error in SaveShippingAddressData with shippingId:{shippingId} and with exception {ex.Message}", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return null;
            }
        }

        //to get portal details by portalId for order receipt
        public virtual PortalModel GetPortalDetailsByPortalId(int portalId)
        {
            ZnodePortal portal = _portalRepository.GetById(portalId);
            return ToModel(portal);
        }

        public virtual List<ZnodeOmsCookieMapping> GetCookieMappingList(CartParameterModel cartParameterModel)
            => _cookieMappingRepository.Table.Where(x => x.UserId == cartParameterModel.UserId && x.PortalId == cartParameterModel.PortalId).ToList();

        //To get cookieMappingId for login or anonymous user
        public virtual int GetCookieMappingId(int? userId, int portalId)
        {
            //if userId > 0 then check if any cookieMappingId exist for user else create new cookieMappingId and return.
            //Else Create new cookieMappingId for every guest user.
            if (userId > 0)
            {
                ZnodeOmsCookieMapping cookieMappings = _cookieMappingRepository.Table.FirstOrDefault(x => x.UserId == userId && x.PortalId == portalId);

                if (cookieMappings?.OmsCookieMappingId > 0)
                    return cookieMappings.OmsCookieMappingId;
                else
                    return CreateCookieMappingId(userId, portalId);
            }
            else
                return CreateCookieMappingId(userId, portalId);
        }

        //to get savedcartId by cookieMappingId
        public virtual int GetSavedCartId(int? cookieMappingId, int portalId = 0, int? userId = 0)
        {
            int localMappingId = cookieMappingId.GetValueOrDefault();
            return GetSavedCartId(ref localMappingId, portalId, userId);
        }

        //to get savedcartId by cookieMappingId
        public virtual int GetSavedCartId(ref int cookieMappingId, int portalId = 0, int? userId = 0)
        {
            int? localMappingId = cookieMappingId;
            if (cookieMappingId > 0)
            {
                IZnodeRepository<ZnodeOmsSavedCart> _savedCartRepository = new ZnodeRepository<ZnodeOmsSavedCart>();
                ZnodeOmsSavedCart savedCart = _savedCartRepository.Table.FirstOrDefault(x => x.OmsCookieMappingId == localMappingId);
                if (HelperUtility.IsNull(savedCart))
                {

                    ZnodeOmsCookieMapping cookieMapping = _cookieMappingRepository.Table.FirstOrDefault(x => x.OmsCookieMappingId == localMappingId);
                    if (HelperUtility.IsNull(cookieMapping) || cookieMapping.OmsCookieMappingId <= 0)
                    {
                        cookieMappingId = CreateCookieMappingId(userId, portalId);
                    }
                    //savedcartId not exist for cookieMappingId then create new.
                    ZnodeOmsSavedCart newCart = _savedCartRepository.Insert(new ZnodeOmsSavedCart()
                    {
                        OmsCookieMappingId = cookieMappingId,
                        CreatedDate = HelperUtility.GetDateTime(),
                        ModifiedDate = HelperUtility.GetDateTime()
                    });
                    return newCart.OmsSavedCartId;
                }
                return Convert.ToInt32(savedCart.OmsSavedCartId);
            }
            return 0;
        }
        //Get the list of saved cart line item on the basis of savedCartId.
        public virtual List<ZnodeOmsSavedCartLineItem> GetSavedCartLineItem(int savedCartId, int omsOrderId = 0)
        {
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.Required,
                           new System.Transactions.TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
            {
                IZnodeRepository<ZnodeOmsSavedCartLineItem> _savedCartLineItemRepository = new ZnodeRepository<ZnodeOmsSavedCartLineItem>();
                List<ZnodeOmsSavedCartLineItem> savedCartLineItems = _savedCartLineItemRepository.Table.Where(x => x.OmsSavedCartId == savedCartId)?.OrderBy(e=> e.OmsSavedCartLineItemId).ToList() ?? new List<ZnodeOmsSavedCartLineItem>();
                return omsOrderId > 0 ? savedCartLineItems?.Where(x => x.OmsOrderId == omsOrderId)?.ToList() : savedCartLineItems?.Where(x => Equals(x.OmsOrderId, null) || x.OmsOrderId == 0)?.ToList();
            }
        }

        //Get the list of all Parent line items.
        public virtual List<ZnodeOmsSavedCartLineItem> GetParentSavedCartLineItem(List<int> parentSavedCartLineItems, int omsOrderId = 0)
        {
            if (parentSavedCartLineItems?.Count > 0)
            {
                IZnodeRepository<ZnodeOmsSavedCartLineItem> _savedCartLineItemRepository = new ZnodeRepository<ZnodeOmsSavedCartLineItem>();
                return _savedCartLineItemRepository.Table.Where(x => parentSavedCartLineItems.Contains(x.OmsSavedCartLineItemId))?.ToList();
            }
            return null;
        }
        //Get the list of Personalized attribute cart line item on the basis of savedCartLineItemId.
        public virtual Dictionary<string, object> GetPersonalizedValueCartLineItem(int savedCartLineItemId, int localeId)
        {
            IZnodeRepository<ZnodeOmsPersonalizeCartItem> _savedOmsPersonalizeCartItemRepository = new ZnodeRepository<ZnodeOmsPersonalizeCartItem>();
            Dictionary<string, object> personalizeItem = new Dictionary<string, object>();
            foreach (KeyValuePair<string, string> personalizeAttr in _savedOmsPersonalizeCartItemRepository.Table.Where(x => x.OmsSavedCartLineItemId == savedCartLineItemId)?.ToDictionary(x => x.PersonalizeCode, x => x.PersonalizeValue))
                personalizeItem.Add(personalizeAttr.Key, (object)personalizeAttr.Value);
            return personalizeItem;
        }

        //Get the list of Personalized attribute order line item on the basis of orderLineItemId.
        public virtual Dictionary<string, object> GetPersonalizedValueOrderLineItem(int orderLineItemId, bool isReorder, int localeId)
        {
            IZnodeRepository<ZnodeOmsPersonalizeItem> _personalizeItemRepository = new ZnodeRepository<ZnodeOmsPersonalizeItem>();
            Dictionary<string, object> personalizeItem = new Dictionary<string, object>();
            //
            foreach (KeyValuePair<string, string> personalizeAttr in _personalizeItemRepository.Table.Where(x => x.OmsOrderLineItemsId == orderLineItemId)?.ToDictionary(x => x.PersonalizeCode, x => x.PersonalizeValue))
                personalizeItem.Add(personalizeAttr.Key, (object)personalizeAttr.Value);
            return personalizeItem;
        }

        public virtual string SetPortalLogo(int portalId)
        {
            string media = GetPortalLogo(portalId);
            if (string.IsNullOrEmpty(media))
                return string.Empty;
            else
                return $"<img src={getMediaPath() + HttpUtility.UrlPathEncode(media)} class='img-responsive' style='max-width:150px;'></img></br>"; //Store logo image URL Encoded in order to escape any spaces in the image name
        }

        //To manage inventory for order
        public virtual bool ManageOrderInventory(OrderWarehouseModel orderInventory, out List<OrderWarehouseLineItemsModel> productInventoryList)
        {
            string savedCartLineItemXML = HelperUtility.ToXML(orderInventory.LineItems);

            //SP call to Manage Order Inventory.
            IZnodeViewRepository<OrderWarehouseLineItemsModel> objStoredProc = new ZnodeViewRepository<OrderWarehouseLineItemsModel>();
            objStoredProc.SetParameter("SkuXml", savedCartLineItemXML, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodeDomainEnum.PortalId.ToString(), orderInventory.PortalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), orderInventory.UserId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter(View_ReturnBooleanEnum.Status.ToString(), null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            productInventoryList = objStoredProc.ExecuteStoredProcedureList("Znode_UpdateInventory @SkuXml, @PortalId, @UserId ,@Status OUT", 3, out status)?.ToList();

            return status == 1;
        }

        //To save/update SavedCartlineItem data in database
        public virtual bool SaveAllCartLineItems(int savedCartId, ShoppingCartModel shoppingCart)
        {
            if (savedCartId > 0 && !Equals(shoppingCart, null))
            {
                List<SavedCartLineItemModel> savedCartLineItem = new List<SavedCartLineItemModel>();

                //Check if Shopping cart and cart items are null. If not then add it ro SavedCartLineItemModel.
                if ((HelperUtility.IsNotNull(shoppingCart.ShoppingCartItems)))
                {
                    //Bind AddOns, Bundle product skus if associated to cart items.
                    foreach (var item in shoppingCart.ShoppingCartItems)
                        BindShoppingCartItemModel(item, shoppingCart.PublishedCatalogId, shoppingCart.LocaleId);

                    //Merge a specific shopping cart item.
                    if (!shoppingCart.IsSplitCart)
                        MergedShoppingCart(shoppingCart);

                    if (shoppingCart.ShoppingCartItems.Any(x => !x.IsProductEdit))
                        MergeLineItem(shoppingCart);
                    int sequence = 0;
                    foreach (ShoppingCartItemModel cartitem in shoppingCart.ShoppingCartItems)
                    {
                        //to add auto addon sku item in shopping cart
                        if (!string.IsNullOrEmpty(cartitem.AutoAddonSKUs))
                            sequence = AddAutoAddonProductsToCart(sequence, savedCartId, cartitem, shoppingCart, savedCartLineItem);

                        sequence++;
                        savedCartLineItem.Add(BindSavedCartLineItemModel(Convert.ToString(cartitem?.OmsOrderLineItemsId), savedCartId, cartitem, shoppingCart.PublishedCatalogId, shoppingCart.LocaleId));// sequence));
                    }
                }
                ConvertBundleProductToLineItem(savedCartLineItem);

                string savedCartLineItemXML = string.Empty;
                try
                {
                    savedCartLineItemXML = HelperUtility.GetCompressedXml(savedCartLineItem);
                    shoppingCart.UserId = HelperUtility.IsNull(shoppingCart.UserId) ? 0 : shoppingCart.UserId;
                    ZnodeLogging.LogMessage(string.Format(Admin_Resources.ReorderCartXML, savedCartLineItemXML), "GetShippingEstimates", System.Diagnostics.TraceLevel.Info);

                } catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, "Custom - ReorderCartXML", System.Diagnostics.TraceLevel.Error);
                }

                //SP call to save/update savedCartLineItem
                IZnodeViewRepository<SavedCartLineItemModel> objStoredProc = new ZnodeViewRepository<SavedCartLineItemModel>();
                objStoredProc.SetParameter("CartLineItemXML", savedCartLineItemXML, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), shoppingCart.UserId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                int status = 0;
                List<SavedCartLineItemModel> savedCartLineItemList = objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateSaveCartLineItem @CartLineItemXML, @UserId ,@Status OUT", 2, out status).ToList();

                return status == 1;
            }
            return false;
        }

        protected virtual void MergeLineItem(ShoppingCartModel shoppingCart)
        {
            List<ShoppingCartItemModel> updatedLineItemList = new List<ShoppingCartItemModel>();
            foreach (var lineItem in shoppingCart.ShoppingCartItems)
            {
                if (lineItem.GroupProducts?.Count > 0)
                {
                    string sku = lineItem.GroupProducts.First().Sku;
                    decimal? quantity = GetGroupProductQuantity(shoppingCart, lineItem, sku);
                    lineItem.ParentProductSKU = sku;
                    if (quantity.HasValue && quantity != 0)
                        lineItem.GroupProducts.First().Quantity += quantity.GetValueOrDefault();

                    if (!updatedLineItemList.Any(m => m.GroupProducts?.Count > 0 && m.GroupProducts.First().Sku.Equals(sku) && m.AddOnProductSKUs.Equals(lineItem.AddOnProductSKUs)))
                        updatedLineItemList.Add(lineItem);

                }
                else
                    updatedLineItemList.Add(lineItem);

            }
            shoppingCart.ShoppingCartItems = updatedLineItemList;
        }

        protected virtual decimal? GetGroupProductQuantity(ShoppingCartModel shoppingCart, ShoppingCartItemModel lineItem, string sku)
        {
            return shoppingCart.ShoppingCartItems
                                 .FirstOrDefault(m => sku == m.GroupProducts?.FirstOrDefault()?.Sku
                                 && m.AddOnProductSKUs.Equals(lineItem.AddOnProductSKUs)
                                 && ((m.OmsSavedcartLineItemId != lineItem.OmsSavedcartLineItemId)
                                 || (m.OmsOrderLineItemsId != lineItem.OmsOrderLineItemsId))
                                 )?.GroupProducts?.First().Quantity;
        }

        //Convert Bundle product to Separate Line Items
        private void ConvertBundleProductToLineItem(List<SavedCartLineItemModel> savedCartLineItem)
        {
            List<SavedCartLineItemModel> bundleProductList = savedCartLineItem.Where(m => !string.IsNullOrEmpty(m.BundleProducts)
                                                            && m.BundleProducts?.Split(',').ToArray().Length > 1).ToList();
            if (HelperUtility.IsNotNull(bundleProductList) && bundleProductList.Count > 0)
            {
                List<SavedCartLineItemModel> bundleProducts = new List<SavedCartLineItemModel>();
                foreach (SavedCartLineItemModel bundleProduct in bundleProductList)
                {
                    string[] bundleSkus = bundleProduct.BundleProducts.Split(',').ToArray();
                    if (bundleSkus.Length > 1)
                    {
                        foreach (string bundleSku in bundleSkus)
                        {
                            bundleProducts.Add(new SavedCartLineItemModel
                            {
                                SKU = bundleProduct.SKU,
                                Quantity = bundleProduct.Quantity,
                                Description = bundleProduct.Description,
                                BundleProducts = bundleSku,
                                ProductName = bundleProduct.ProductName,
                                OmsSavedCartId = bundleProduct.OmsSavedCartId,
                                AddonProducts = bundleProduct.AddonProducts,
                                AddOnQuantity = bundleProduct.AddOnQuantity,
                                OmsOrderId = bundleProduct.OmsOrderId,
                                PersonaliseValuesDetail = bundleProduct.PersonaliseValuesDetail
                            }); ;
                        }
                    }
                }
                savedCartLineItem.RemoveAll(m => m.BundleProducts?.Split(',').ToArray().Length > 1);
                savedCartLineItem.AddRange(bundleProducts);
            }
        }

        //To save/update SavedCartlineItem data in database
        public virtual bool SaveAllCartLineItemsInDatabase(int savedCartId, AddToCartModel shoppingCart)
        {
            if (savedCartId > 0 && !Equals(shoppingCart, null))
            {
                List<SavedCartLineItemModel> savedCartLineItem = new List<SavedCartLineItemModel>();

                //Check if Shopping cart and cart items are null. If not then add it ro SavedCartLineItemModel.
                if ((HelperUtility.IsNotNull(shoppingCart.ShoppingCartItems)))
                {
                    //Bind AddOns, Bundle product skus if associated rto cart items.
                    foreach (var item in shoppingCart.ShoppingCartItems)
                        BindShoppingCartItemModel(item, shoppingCart.PublishedCatalogId, shoppingCart.LocaleId);

                    int sequence = 0;
                    foreach (ShoppingCartItemModel cartitem in shoppingCart.ShoppingCartItems)
                    {
                        sequence++;
                        savedCartLineItem.Add(BindSaveCartLineItem(Convert.ToString(cartitem?.OmsOrderLineItemsId), savedCartId, cartitem, shoppingCart.PublishedCatalogId, shoppingCart.LocaleId)); //, sequence));
                    }
                }
                string savedCartLineItemXML = HelperUtility.ToXML(savedCartLineItem);

                shoppingCart.UserId = HelperUtility.IsNull(shoppingCart.UserId) ? 0 : shoppingCart.UserId;

                //SP call to save/update savedCartLineItem
                IZnodeViewRepository<SavedCartLineItemModel> objStoredProc = new ZnodeViewRepository<SavedCartLineItemModel>();
                objStoredProc.SetParameter("CartLineItemXML", savedCartLineItemXML, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), shoppingCart.UserId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                int status = 0;
                List<SavedCartLineItemModel> savedCartLineItemList = objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateSaveCartLineItemQuantity @CartLineItemXML, @UserId ,@Status OUT", 2, out status).ToList();

                return status == 1;
            }
            return false;
        }

        //To save/update SavedCartlineItem data in database
        public virtual AddToCartStatusModel SaveAllCartLineItemsInDatabase(AddToCartModel shoppingCart, int cookieMappingId=0)
        {
            if (!Equals(shoppingCart, null))
            {
                List<SavedCartLineItemModel> savedCartLineItem = new List<SavedCartLineItemModel>();

                //Check if Shopping cart and cart items are null. If not then add it ro SavedCartLineItemModel.
                if ((HelperUtility.IsNotNull(shoppingCart.ShoppingCartItems)))
                {
                    //Bind AddOns, Bundle product skus if associated rto cart items.
                    foreach (ShoppingCartItemModel cartitem in shoppingCart.ShoppingCartItems)
                    {
                        BindShoppingCartItemModel(cartitem, shoppingCart.PublishedCatalogId, shoppingCart.LocaleId);
                        savedCartLineItem.Add(BindSaveCartLineItem(Convert.ToString(cartitem?.OmsOrderLineItemsId), 0, cartitem, shoppingCart.PublishedCatalogId, shoppingCart.LocaleId)); //, sequence));
                    }
                }
                string savedCartLineItemXML = HelperUtility.GetCompressedXml(savedCartLineItem);

                shoppingCart.UserId = HelperUtility.IsNull(shoppingCart.UserId) ? 0 : shoppingCart.UserId;

                //SP call to save/update savedCartLineItem
                IZnodeViewRepository<AddToCartStatusModel> objStoredProc = new ZnodeViewRepository<AddToCartStatusModel>();
                objStoredProc.SetParameter("CartLineItemXML", savedCartLineItemXML, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), shoppingCart.UserId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter(ZnodeUserPortalEnum.PortalId.ToString(), shoppingCart.PortalId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter(ZnodeOmsCookieMappingEnum.OmsCookieMappingId.ToString(), cookieMappingId, ParameterDirection.Input, DbType.Int32);
                AddToCartStatusModel addToCartStatusModel = objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateSaveCartLineItemQuantityWrapper @CartLineItemXML, @UserId,@PortalId,@OmsCookieMappingId").FirstOrDefault();

                return addToCartStatusModel;
            }
            return new AddToCartStatusModel();
        }

        // Merge a specific shopping cart item
        public virtual void MergedShoppingCart(ShoppingCartModel shoppingCart)
        {
            List<ShoppingCartItemModel> updatedModel = new List<ShoppingCartItemModel>();

            if (HelperUtility.IsNotNull(shoppingCart))
            {
                bool isProductEdit = shoppingCart?.ShoppingCartItems?.Count(x => x.IsProductEdit) > 0;
                if (shoppingCart.ShoppingCartItems?.Count > 1)
                {
                    //If the product is in edit mode, then we have to replace the quantity of products with new quantities. For this we have made the previous quantities to zero.
                    if (isProductEdit)
                    {
                        shoppingCart.ShoppingCartItems.ForEach(x =>
                        {
                            x.PersonaliseValuesDetail = x.PersonaliseValuesDetail?.Count > 0 ? x.PersonaliseValuesDetail : GetPersonalizedAttributeLineItemDetails(x?.PersonaliseValuesList, shoppingCart.CookieMappingId);
                        });

                        string[] newDesignId = shoppingCart?.ShoppingCartItems?.Where(x => x.OmsSavedcartLineItemId == null).Select(x => x.PersonaliseValuesDetail?.FirstOrDefault()?.DesignId ?? string.Empty).ToArray();
                        foreach (var item in shoppingCart.ShoppingCartItems
                                                         ?.GroupBy(x => x.PersonaliseValuesDetail?.FirstOrDefault()?.DesignId)
                                                         ?.Where(y => y.Count() > 1)
                                                         ?.Select(z => z.Where(o => o.OmsSavedcartLineItemId > 0)))
                        {
                            item.Where(x => newDesignId.Where(y => y != string.Empty)
                                .Contains(x.PersonaliseValuesDetail?.FirstOrDefault().DesignId))
                                .ToList().ForEach(x => x.Quantity = 0);
                        }
                    }
                    //Updates a specific shopping cart quantity of different product types includes simple, bundle, configurable and group product.
                    foreach (ShoppingCartItemModel mainShoppingCartModel in shoppingCart.ShoppingCartItems)
                        updatedModel = MergedOrderedQuantity(shoppingCart.ShoppingCartItems, updatedModel, mainShoppingCartModel, isProductEdit);

                    if (HelperUtility.IsNotNull(updatedModel) && updatedModel.Count > 0)
                    {
                        //Get list of all external ids.
                        IEnumerable<string> externalIds = updatedModel.Select(x => x.ExternalId);

                        //Remove items from existing shopping which having "externalIds".
                        shoppingCart.ShoppingCartItems = shoppingCart.ShoppingCartItems?.Where(x => !externalIds.Contains(x.ExternalId)).ToList();

                        //Concat sorted shopping cart with "updatedModel".
                        shoppingCart.ShoppingCartItems = shoppingCart.ShoppingCartItems.Concat(updatedModel).ToList();
                    }
                }
            }
        }

        //Get Portal Feature Value.
        public virtual bool GetPortalFeatureValue(int portalId, HelperUtility.StoreFeature storeFeature)
        {
            Dictionary<string, bool> portalFeatureValues = ZnodeConfigManager.GetSiteConfigFeatureValueList(portalId);
            return portalFeatureValues.ContainsKey(storeFeature.ToString()) && portalFeatureValues[storeFeature.ToString()];
        }

        //to get order by orderId
        public virtual ZnodeOmsOrderDetail GetOrderById(int orderId)
        => _orderDetailRepository.Table.FirstOrDefault(x => x.OmsOrderId == orderId && x.IsActive);

        //to get order line items by orderDetailsId
        public virtual List<ZnodeOmsOrderLineItem> GetOrderLineItemByOrderId(int orderDetailsId)
        {
            FilterDataCollection filter = new FilterDataCollection();
            filter.Add(new FilterDataTuple(ZnodeOmsOrderDetailEnum.OmsOrderDetailsId.ToString(), FilterOperators.Equals, orderDetailsId.ToString()));
            filter.Add(new FilterDataTuple(ZnodeOmsOrderDetailEnum.IsActive.ToString(), FilterOperators.Equals, Convert.ToString(true)));
            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter)?.WhereClause;
            return _orderLineItemRepository.GetEntityList(whereClause, new List<string> { ZnodeOmsOrderLineItemEnum.ZnodeRmaReasonForReturn.ToString(), ZnodeOmsOrderLineItemEnum.ZnodeOmsOrderState.ToString(), ZnodeOmsOrderLineItemEnum.ZnodeOmsTaxOrderLineDetails.ToString(), ZnodeOmsOrderLineItemEnum.ZnodeOmsPersonalizeItems.ToString(), ZnodeOmsOrderLineItemEnum.ZnodeOmsOrderAttributes.ToString(), ZnodeOmsOrderLineItemEnum.ZnodeOmsOrderWarehouses.ToString(), ZnodeOmsOrderLineItemEnum.ZnodeOmsOrderShipment.ToString() })?.ToList();
        }

        //to get order line items by omsOrderId
        public virtual List<ZnodeOmsOrderLineItem> GetOrderLineItemByOmsOrderId(int omsOrderId)
        {
            ZnodeOmsOrderDetail orderDetail = GetOrderById(omsOrderId);
            FilterDataCollection filter = new FilterDataCollection();
            filter.Add(new FilterDataTuple(ZnodeOmsOrderDetailEnum.OmsOrderDetailsId.ToString(), FilterOperators.Equals, orderDetail.OmsOrderDetailsId.ToString()));
            filter.Add(new FilterDataTuple(ZnodeOmsOrderDetailEnum.IsActive.ToString(), FilterOperators.Equals, Convert.ToString(true)));
            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter)?.WhereClause;
            return _orderLineItemRepository.GetEntityList(whereClause, new List<string> { ZnodeOmsOrderLineItemEnum.ZnodeRmaReasonForReturn.ToString(), ZnodeOmsOrderLineItemEnum.ZnodeOmsOrderState.ToString(), ZnodeOmsOrderLineItemEnum.ZnodeOmsTaxOrderLineDetails.ToString(), ZnodeOmsOrderLineItemEnum.ZnodeOmsPersonalizeItems.ToString(), ZnodeOmsOrderLineItemEnum.ZnodeOmsOrderAttributes.ToString(), ZnodeOmsOrderLineItemEnum.ZnodeOmsOrderWarehouses.ToString(), ZnodeOmsOrderLineItemEnum.ZnodeOmsOrderShipment.ToString() })?.ToList();

        }

        //to get bundle order line items by omsOrderId
        public virtual List<ZnodeOmsOrderLineItem> GetBundleOrderLineItemByOmsOrderId(int omsOrderId)
        {
            ZnodeOmsOrderDetail orderDetail = GetOrderById(omsOrderId);
            FilterDataCollection filter = new FilterDataCollection();
            filter.Add(new FilterDataTuple(ZnodeOmsOrderDetailEnum.OmsOrderDetailsId.ToString(), FilterOperators.Equals, orderDetail.OmsOrderDetailsId.ToString()));
            filter.Add(new FilterDataTuple(ZnodeOmsOrderDetailEnum.IsActive.ToString(), FilterOperators.Equals, Convert.ToString(true)));
            filter.Add(new FilterDataTuple(ZnodeOmsOrderLineItemEnum.OrderLineItemRelationshipTypeId.ToString(), FilterOperators.Equals, Convert.ToString((int)ZnodeCartItemRelationshipTypeEnum.Bundles)));
            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter)?.WhereClause;
            return _orderLineItemRepository.GetEntityList(whereClause, new List<string> {ZnodeOmsOrderLineItemEnum.ZnodeOmsOrderAttributes.ToString()})?.ToList();
        }

        //To get order shipment address.
        public virtual void GetOrderShipmentAddress(int OrderShipmentID, OrderLineItemModel orderLineItem)
        {
            ZnodeOmsOrderShipment orderShipment = _orderShipmentRepository.GetById(OrderShipmentID);
            if (HelperUtility.IsNotNull(orderShipment))
                orderLineItem.ZnodeOmsOrderShipment = orderShipment.ToModel<OrderShipmentModel>();
        }

        //To get the order shipment details for the provided shipment Ids.
        public virtual List<OrderShipmentModel> GetOrderShipmentDetailsByIds(List<int> orderShipmentIds)
        => _orderShipmentRepository.Table.Where(x => orderShipmentIds.Contains(x.OmsOrderShipmentId))?.ToModel<OrderShipmentModel>().ToList();

        //to get giftcard by cardnumber
        public virtual GiftCardModel GetVoucherByCardNumber(string cardNumber, int? orderId = null)
        {
            if (HelperUtility.IsNull(orderId))
                return _giftCardRepository.Table.FirstOrDefault(x => x.CardNumber.ToLower() == cardNumber.ToLower())?.ToModel<GiftCardModel>() ?? new GiftCardModel();
            else
                return GetGiftCardWithOrderAmount(cardNumber, orderId);
        }

        //to order shipping address
        public virtual int AddToGiftCardHistory(int orderId, decimal giftcardAmount, string giftcardNumber, DateTime transactionDate, int? userId = 0)
        {
            int giftCardId = GetVoucherByCardNumber(giftcardNumber, null)?.GiftCardId ?? 0;
            ZnodeGiftCardHistory history = new ZnodeGiftCardHistory();
            if(giftCardId > 0)
            {
                history.TransactionAmount = giftcardAmount;
                history.TransactionDate = transactionDate;
                history.OmsOrderDetailsId = orderId;
                history.GiftCardId = giftCardId;
                history.CreatedBy = (int)userId;
                history.Notes = Admin_Resources.VoucherHistoryNotes;
                history = _giftCardHistoryRepository.Insert(history);
            }
            return history.GiftCardHistoryId;
        }

        //to update giftcard by cardnumber
        public virtual bool UpdateGiftCard(GiftCardModel model, bool isExistingOrder = false)
        {
            ZnodeGiftCard giftCard = _giftCardRepository.Table.FirstOrDefault(x => x.CardNumber == model.CardNumber);
            if (HelperUtility.IsNotNull(giftCard) && giftCard.Amount > 0 && model.Amount.Value > 0)
            {
                if (model.UserId != null && giftCard.UserId == null)
                    giftCard.UserId = model.UserId;
                if(isExistingOrder)
                    giftCard.RemainingAmount = (model.RemainingAmount.Value);
                else
                    giftCard.RemainingAmount = (giftCard.RemainingAmount - model.Amount.Value);
                return _giftCardRepository.Update(giftCard);
            }
            else
                return false;
        }

        //to get coupon available quantity
        public virtual int GetCouponAvailableQuantity(string couponCode)
        {
            return _promotionCouponRepository.Table.Where(x => x.Code == couponCode)?.Select(x=>x.AvailableQuantity)?.FirstOrDefault() ?? 0;
        }

        //to check applied coupon is used in exiting order by OrderId
        public virtual bool IsExistingOrderCoupon(int orderId, string couponcode)
        {
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("@OrderId", orderId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Couponcode", couponcode, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            IList<View_ReturnBoolean> result = objStoredProc.ExecuteStoredProcedureList("Znode_CouponExistInOrder @OrderId, @Couponcode, @Status OUT", 2, out status);

            if (result.FirstOrDefault().Id.Equals(0))
                return true;
            else
                return result.FirstOrDefault().Status.Value;
        }

        //to update Coupon available quantity if it is more than 1
        public virtual bool UpdateCouponQuantity(string couponCode, int orderId = 0, bool isExistingOrder = false)
        {
            ZnodePromotionCoupon coupon = _promotionCouponRepository.Table.FirstOrDefault(x => x.Code == couponCode);
            if (coupon.AvailableQuantity > 0)
            {
                if (orderId > 0 && IsExistingOrderCoupon(orderId, couponCode) && isExistingOrder)
                {
                    return true;
                }
                else
                {
                    coupon.AvailableQuantity -= 1;
                    return _promotionCouponRepository.Update(coupon);
                }
            }
            else
                return false;
        }

        // to save order  the referral commission
        public virtual bool SaveReferralCommission(int? userId, int orderDetailsId, string transactionId, decimal commissionAmount)
        {
            UserModel referralUser = GetUserById(userId);
            if (referralUser != null && referralUser.UserId > 0)
            {
                IZnodeRepository<ZnodeOmsReferralCommission> _referralCommissionRepository = new ZnodeRepository<ZnodeOmsReferralCommission>();
                ReferralCommissionModel model = new ReferralCommissionModel();
                decimal commission = referralUser.ReferralCommission == null ? 0 : Convert.ToDecimal(referralUser.ReferralCommission);
                model.ReferralCommissionTypeId = referralUser.ReferralCommissionTypeId;
                model.ReferralCommission = commission;
                model.UserId = userId.GetValueOrDefault();
                model.OmsOrderDetailsId = orderDetailsId;
                model.TransactionId = transactionId;
                model.OrderCommission = model.ReferralCommissionTypeId == 1 ? (commissionAmount * commission / 100) : commission;
                ZnodeOmsReferralCommission referralCommission = _referralCommissionRepository.Insert(model.ToEntity<ZnodeOmsReferralCommission>());
                return referralCommission.OmsReferralCommissionId > 0;
            }
            return false;
        }

        //to get order csr discount details by omsorderdetailsid
        public virtual decimal GetOrderDiscountAmount(int? omsOrderDetailsId, OrderDiscountTypeEnum discountType)
        {
            int discountTypeId = (_omsDiscountTypeRepository.Table.FirstOrDefault(x => x.Name == discountType.ToString())?.OmsDiscountTypeId).GetValueOrDefault();
            decimal? discountAmount = (from _disc in _omsOrderDiscountRepository.Table
                                       join _typ in _omsDiscountTypeRepository.Table on _disc.OmsDiscountTypeId equals _typ.OmsDiscountTypeId
                                       where _typ.OmsDiscountTypeId == discountTypeId && _disc.OmsOrderDetailsId == omsOrderDetailsId
                                       select _disc.DiscountAmount
                               ).Sum();

            return discountAmount ?? 0;
        }

        //To get order discount details by omsorderdetailsid
        public List<OrderDiscountModel> GetOrderDiscountAmount(int omsOrderDetailsId)
        {
            if (omsOrderDetailsId > 0)
            {
                return (from _disc in _omsOrderDiscountRepository.Table
                        join _typ in _omsDiscountTypeRepository.Table on _disc.OmsDiscountTypeId equals _typ.OmsDiscountTypeId
                        where _disc.OmsOrderDetailsId == omsOrderDetailsId
                        select new OrderDiscountModel
                        {
                            DiscountAmount = _disc.DiscountAmount,
                            DiscountType = _typ.Name,
                            PerQuantityDiscount = (decimal)_disc.PerQuantityDiscount,
                            DiscountCode = _disc.DiscountCode,
                            OmsOrderLineItemId = _disc.OmsOrderLineItemId,
                            DiscountMultiplier=_disc.DiscountMultiplier,
                            ParentOmsOrderLineItemsId=_disc.ParentOmsOrderLineItemsId,
                            DiscountLevelTypeId=_disc.DiscountLevelTypeId,
                            PromotionTypeId = _disc.PromotionTypeId,
                            DiscountAppliedSequence = _disc.DiscountAppliedSequence
                        })?.ToList();
            }
            return null;
        }

        //to get order discount code by omsorderdetailsid
        public virtual string GetDiscountCode(int? omsOrderDetailsId, OrderDiscountTypeEnum discountType)
        {
            int discountTypeId = (_omsDiscountTypeRepository.Table.FirstOrDefault(x => x.Name == discountType.ToString())?.OmsDiscountTypeId).GetValueOrDefault();
            string discountCode = (from _disc in _omsOrderDiscountRepository.Table
                                   join _typ in _omsDiscountTypeRepository.Table on _disc.OmsDiscountTypeId equals _typ.OmsDiscountTypeId
                                   where _typ.OmsDiscountTypeId == discountTypeId && _disc.OmsOrderDetailsId == omsOrderDetailsId
                                   select _disc.DiscountCode
                               ).FirstOrDefault();

            return discountCode;
        }

        //to get group product line item xml data to be stored in savedcartlineitem table with ~ separated quantity
        public virtual string GetGroupProductLineItemXMLData(List<AssociatedProductModel> groupItems)
        {
            //Count check for group products.
            if (groupItems?.Count > 0)
            {
                string productLineItemXML = string.Empty;

                //bind group product sku and quantity.
                foreach (AssociatedProductModel groupItem in groupItems)
                    productLineItemXML = string.Concat(productLineItemXML, groupItem.Sku);

                return productLineItemXML;
            }
            return null;
        }

        //to get group product line item xml data to be stored in savedcartlineitem table with ~ separated quantity for template
        public virtual string GetGroupProductLineItemXMLDataForTemplate(List<AssociatedProductModel> groupItems, string templateType = null)
        {
            //Count check for group products.
            if (groupItems?.Count > 0)
            {
                string productLineItemXML = string.Empty;

                //bind group product sku and quantity.
                foreach (AssociatedProductModel groupItem in groupItems)
                {
                    if(templateType == ZnodeConstant.OrderTemplate)
                         productLineItemXML = string.Concat(productLineItemXML, $"{groupItem.Sku}~{groupItem.Quantity},");
                    else
                        productLineItemXML = string.Concat(productLineItemXML, groupItem.Sku);
                }                 
                return productLineItemXML;
            }
            return null;
        }

        //To get Personalized Attributes item xml data to be stored in savedcartlineitem table with ~ separated quantity
        public virtual string GetPersonalizedAttributeLineItemXMLData(Dictionary<string, object> PersonalizedAttributes, string cookieMappingId)
        {
            //Count check for Personalized Attributes.
            if (PersonalizedAttributes?.Count > 0)
            {
                string personalizedLineItemXML = string.Empty;

                //bind Personalized Attributes via key and value.
                foreach (var PersonalizedAttr in PersonalizedAttributes)
                {
                    if (HelperUtility.IsNotNull(cookieMappingId) || cookieMappingId != "0")
                        personalizedLineItemXML = string.Concat(personalizedLineItemXML, $"{ GetPersonaliseAttributeByLocal(PersonalizedAttr.Key, null)}~{ PersonalizedAttr.Value},");
                    else
                        personalizedLineItemXML = string.Concat(personalizedLineItemXML, $"{PersonalizedAttr.Key}~{ PersonalizedAttr.Value},");
                }

                if (!string.IsNullOrEmpty(personalizedLineItemXML))
                    personalizedLineItemXML = personalizedLineItemXML.Remove(personalizedLineItemXML.Length - 1);
                return personalizedLineItemXML;
            }
            return null;
        }

        public virtual List<PersonaliseValueModel> GetPersonalizedAttributeLineItemDetails(Dictionary<string, object> PersonalizedAttributes, string cookieMappingId)
        {
            //Count check for Personalized Attributes.
            if (PersonalizedAttributes?.Count > 0)
            {
                List<PersonaliseValueModel> personalizedLineItems = new List<PersonaliseValueModel>();

                //bind Personalized Attributes via key and value.
                foreach (var PersonalizedAttr in PersonalizedAttributes)
                {
                    PersonaliseValueModel model = new PersonaliseValueModel();
                    if (HelperUtility.IsNotNull(cookieMappingId) || cookieMappingId != "0")
                    {
                        model.PersonalizeCode = GetPersonaliseAttributeByLocal(PersonalizedAttr.Key, null);
                        model.PersonalizeValue = Convert.ToString(PersonalizedAttr.Value);
                        model.DesignId = GetPersonalizeDesignId(PersonalizedAttr.Value);
                        model.ThumbnailURL = GetPersonalizeThumbnailURL(PersonalizedAttr.Value);
                        model.PersonalizeName = GetPersonaliseAttributeValueByLocal(PersonalizedAttr.Key, null);
                    }
                    else
                    {
                        model.PersonalizeCode = PersonalizedAttr.Key;
                        model.PersonalizeValue = Convert.ToString(PersonalizedAttr.Value);
                        model.DesignId = GetPersonalizeDesignId(PersonalizedAttr.Value);
                        model.ThumbnailURL = GetPersonalizeThumbnailURL(PersonalizedAttr.Value);
                        model.PersonalizeName = GetPersonaliseAttributeValueByLocal(PersonalizedAttr.Key, null);
                    }
                    personalizedLineItems.Add(model);
                }
                return personalizedLineItems;
            }
            return null;
        }

        //Get the list of Personalized attribute cart line item on the basis of savedCartLineItemId.
        public List<PersonaliseValueModel> GetPersonalizedValueCartLineItem(int savedCartLineItemId)
        {
            IZnodeRepository<ZnodeOmsPersonalizeCartItem> _savedOmsPersonalizeCartItemRepository = new ZnodeRepository<ZnodeOmsPersonalizeCartItem>();
            List<PersonaliseValueModel> personalizeItem;

            personalizeItem = (from item in _savedOmsPersonalizeCartItemRepository.Table
                               where item.OmsSavedCartLineItemId == savedCartLineItemId
                               select new PersonaliseValueModel
                               {
                                   PersonalizeCode = item.PersonalizeCode,
                                   PersonalizeValue = item.PersonalizeValue,
                                   DesignId = item.DesignId,
                                   ThumbnailURL = item.ThumbnailURL,
                                   OmsSavedCartLineItemId = item.OmsSavedCartLineItemId ?? 0,
                               }).ToList();
            return personalizeItem;
        }

        public List<PersonaliseValueModel> GetPersonalizedValueCartLineItem(List<int?> savedCartLineItemId)
        {
            IZnodeRepository<ZnodeOmsPersonalizeCartItem> _savedOmsPersonalizeCartItemRepository = new ZnodeRepository<ZnodeOmsPersonalizeCartItem>();
            List<PersonaliseValueModel> personalizeItemList = new List<PersonaliseValueModel>();

            if (savedCartLineItemId.Count > 0)
            {
                personalizeItemList = (from item in _savedOmsPersonalizeCartItemRepository.Table
                                       join itemDetails in _pimAttribute.Table on item.PersonalizeCode equals itemDetails.AttributeCode into itemDetails_join
                                       from itemDetails in itemDetails_join.DefaultIfEmpty()
                                       join itemDetailsLocale in _pimAttributeLocale.Table on new { itemDetails.PimAttributeId } equals new { PimAttributeId = (int)itemDetailsLocale.PimAttributeId } into itemDetailsLocale_join
                                       from itemDetailsLocale in itemDetailsLocale_join.DefaultIfEmpty()
                                       where savedCartLineItemId.Contains(item.OmsSavedCartLineItemId)
                                       select new PersonaliseValueModel
                                       {
                                           PersonalizeCode = item.PersonalizeCode,
                                           PersonalizeValue = item.PersonalizeValue,
                                           DesignId = item.DesignId,
                                           ThumbnailURL = item.ThumbnailURL,
                                           PersonalizeName = itemDetailsLocale.AttributeName,
                                           OmsSavedCartLineItemId = item.OmsSavedCartLineItemId.HasValue ? item.OmsSavedCartLineItemId.Value : 0,
                                       }).ToList();
            }

            return personalizeItemList.GroupBy(x=>new {x.OmsSavedCartLineItemId, x.PersonalizeCode }).Select(y=>y.FirstOrDefault()).Distinct().ToList();
        }

        //Get the list of Personalized attribute quote line item on the basis of quoteCartLineItemId.

        public virtual List<PersonaliseValueModel> GetPersonalizedQuoteValueCartLineItem(int quoteCartLineItemId)
        {
            if (quoteCartLineItemId > 0)
            {
                List<PersonaliseValueModel> personalizeItem;
                IZnodeRepository<ZnodeOmsQuotePersonalizeItem> _savedOmsQuotePersonalizeCartItemRepository = new ZnodeRepository<ZnodeOmsQuotePersonalizeItem>();

                personalizeItem = (from item in _savedOmsQuotePersonalizeCartItemRepository.Table
                                   join itemAttributeDetails in _pimAttribute.Table on item.PersonalizeCode equals itemAttributeDetails.AttributeCode
                                   join itemAttributeLocaleDetails in _pimAttributeLocale.Table on itemAttributeDetails.PimAttributeId equals itemAttributeLocaleDetails.PimAttributeId
                                   where item.OmsQuoteLineItemId == quoteCartLineItemId
                                   select new PersonaliseValueModel
                                   {
                                       PersonalizeCode = item.PersonalizeCode,
                                       PersonalizeValue = item.PersonalizeValue,
                                       DesignId = item.DesignId,
                                       ThumbnailURL = item.ThumbnailURL,
                                       PersonalizeName = itemAttributeLocaleDetails.AttributeName,
                                   }).ToList();

                return personalizeItem;
            }
            return null;
        }

        //Get Thumbnail Url based on the personalize Value.
        protected virtual string GetPersonalizeThumbnailURL(object personalizeValue)
        {
            string url = string.Empty;
            if (HelperUtility.IsNotNull(personalizeValue))
            {
                try
                {
                    dynamic customData = JsonConvert.DeserializeObject(Convert.ToString(personalizeValue));
                    url = customData?.savedDesigns[0]?.Value?.Split(',')[0];
                } catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(Admin_Resources.ErrorThumbnailUrlSet, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning, ex);
                }
            }
            return url;
        }

        //Get the Design id based on the personalize value.
        protected virtual string GetPersonalizeDesignId(object personalizeValue)
        {
            string designId = string.Empty;
            if (HelperUtility.IsNotNull(personalizeValue))
            {
                try
                {
                    dynamic customData = JsonConvert.DeserializeObject(Convert.ToString(personalizeValue));
                    designId = customData?.customizeProductId;
                } catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(Admin_Resources.ErrorDesignIdSet, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning, ex);
                }
            }
            return designId;
        }
        //Get comma seperated skus of add on products.
        public virtual string GetAddOnProducts(int publishProductId, int publishCatalogId, int localeId)
        {
            IZnodeRepository<ZnodePublishAddonEntity> _publishAddOnEntity =  new ZnodeRepository<ZnodePublishAddonEntity>(HelperMethods.Context);
            int currentVersionId = ZnodeDependencyResolver.GetService<IPublishProductHelper>()?.GetCatalogVersionId(publishCatalogId, localeId) ?? 0;
            //Get AddOn Product
            List<ZnodePublishAddonEntity> addOnEntity = _publishAddOnEntity.Table.Where(x => x.ZnodeProductId == publishProductId && x.RequiredType == WebStoreEnum.Required.ToString())?.ToList();

            FilterCollection filters;

            //Check if entity is not null.
            if (addOnEntity?.Count > 0)
            {
                List<int> associatedProductIds = addOnEntity.Select(x => x.AssociatedZnodeProductId)?.ToList();
                //Get associated product list.
                List<PublishProductModel> products = _publishProductEntity.Table.Where(x => x.LocaleId == localeId && associatedProductIds.Contains(x.ZnodeProductId) && x.VersionId == currentVersionId)?.ToModel<PublishProductModel>()?.ToList();
                return string.Join(",", products?.Select(x => x.SKU).Distinct());
            }
            return string.Empty;
        }

        //Get comma seperated skus of bundle products.
        public virtual string GetBundleProducts(int publishProductId, string productType, int publishCatalogId, int localeId)
        {
            if (!string.IsNullOrEmpty(productType) && Equals(productType, ZnodeConstant.BundleProduct))
            {
                IZnodeRepository<ZnodePublishBundleProductEntity> _publishBundleProductEntity = new ZnodeRepository<ZnodePublishBundleProductEntity>(HelperMethods.Context);

                int currentVersionId = ZnodeDependencyResolver.GetService<IPublishProductHelper>()?.GetCatalogVersionId(publishCatalogId, localeId) ?? 0;
                //Get Configurable Product
                List<ZnodePublishBundleProductEntity> bundleEntity = _publishBundleProductEntity.Table.Where(x => x.ZnodeProductId == publishProductId)?.ToList();

                List<int> associatedProductIds = bundleEntity?.Select(x => x.AssociatedZnodeProductId)?.ToList();

                if (bundleEntity?.Count > 0)
                {
                    //Get associated product list.
                    List<ZnodePublishProductEntity> products = _publishProductEntity.Table.Where(x => x.LocaleId == localeId && associatedProductIds.Contains(x.ZnodeProductId) && x.VersionId == currentVersionId)?.ToList();

                    return string.Join(",", products?.Select(x => x.SKU)?.Distinct());
                }
            }
            return string.Empty;
        }

        //Get comma seperated skus of configurable products.
        public virtual string GetConfigurableProducts(int publishProductId, string productType, int publishCatalogId, int localeId, int? catalogVersionId = null)
        {
            if (!string.IsNullOrEmpty(productType) && Equals(productType, ZnodeConstant.ConfigurableProduct))
            {
                IZnodeRepository<ZnodePublishConfigurableProductEntity> _publishConfigureProductEntity = new ZnodeRepository<ZnodePublishConfigurableProductEntity>(HelperMethods.Context);

                //Get Configurable Product
                List<ZnodePublishConfigurableProductEntity> configurableEntity = _publishConfigureProductEntity.Table.Where(x => x.ZnodeProductId == publishProductId)?.ToList();

                if (HelperUtility.IsNull(catalogVersionId))
                { 
                    int currentVersionId = ZnodeDependencyResolver.GetService<IPublishProductHelper>()?.GetCatalogVersionId(publishCatalogId, localeId) ?? 0;
                    catalogVersionId = currentVersionId;
                }
                //Check if entity is not null.
                if (configurableEntity?.Count > 0)
                {
                    int[] associatedProducts = configurableEntity.Select(associatedProduct => associatedProduct.AssociatedZnodeProductId).ToArray();
                    associatedProducts = associatedProducts.Concat(new[] { publishProductId }).ToArray();

                    List<ZnodePublishProductEntity> products = _publishProductEntity.Table.Where(x => x.LocaleId == localeId && associatedProducts.Contains(x.ZnodeProductId) && x.VersionId == catalogVersionId).ToList();

                    //Get associated product list.
                    return products?.FirstOrDefault().SKU;
                }
            }
            return string.Empty;
        }

        //Get comma seperated skus and quantity of group products.
        public virtual string GetGroupProducts(int publishProductId, string productType, int publishCatalogId, int localeId)
        {
            if (!string.IsNullOrEmpty(productType) && Equals(productType, ZnodeConstant.GroupedProduct))
            {
                IZnodeRepository<ZnodePublishGroupProductEntity> _publishGroupProductEntity = new ZnodeRepository<ZnodePublishGroupProductEntity>(HelperMethods.Context);

                //Get Configurable Product
                List<ZnodePublishGroupProductEntity> groupProductEntity = _publishGroupProductEntity.Table.Where(x => x.ZnodeProductId == publishProductId)?.ToList();
                //Check if entity is not null.
                if (HelperUtility.IsNotNull(groupProductEntity))
                {
                    int[] associatedProducts = groupProductEntity.Select(associatedProduct => associatedProduct.AssociatedZnodeProductId).ToArray();
                    associatedProducts = associatedProducts.Concat(new[] { publishProductId }).ToArray();

                    //Get associated product list.
                    List<ZnodePublishProductEntity> productList = _publishProductEntity.Table.Where(x => x.LocaleId == localeId && associatedProducts.Contains(x.ZnodeProductId))?.ToList();

                    List<AssociatedProductModel> groupItems = new List<AssociatedProductModel>();

                    foreach (ZnodePublishProductEntity product in productList)
                        groupItems.Add(new AssociatedProductModel { Sku = product.SKU, Quantity = 1 });

                    return GetGroupProductLineItemXMLData(groupItems);
                }
            }
            return null;
        }

        //Get comma seperated skus and quantity of group products.
        public virtual List<AssociatedProductModel> GetShoppingCartGroupProducts(int publishProductId, string productType, int publishCatalogId, int localeId)
        {
            if (!string.IsNullOrEmpty(productType) && Equals(productType, ZnodeConstant.GroupedProduct))
            {
                IZnodeRepository<ZnodePublishGroupProductEntity> _publishGroupProductEntity = new ZnodeRepository<ZnodePublishGroupProductEntity>(HelperMethods.Context);

                //Get Configurable Product
                List<ZnodePublishGroupProductEntity> groupProductEntity = _publishGroupProductEntity.Table.Where(x => x.ZnodeProductId == publishProductId)?.ToList();

                //Check if entity is not null.
                if (HelperUtility.IsNotNull(groupProductEntity))
                {
                    int[] associatedProducts = groupProductEntity.Select(associatedProduct => associatedProduct.AssociatedZnodeProductId).ToArray();
                    associatedProducts = associatedProducts.Concat(new[] { publishProductId }).ToArray();

                    //Get associated product list.
                    List<ZnodePublishProductEntity> productList = _publishProductEntity.Table.Where(x => x.LocaleId == localeId && associatedProducts.Contains(x.ZnodeProductId)).ToList();

                    List<AssociatedProductModel> groupItems = new List<AssociatedProductModel>();

                    foreach (ZnodePublishProductEntity product in productList)
                        groupItems.Add(new AssociatedProductModel { Sku = product.SKU, Quantity = 1 });

                    return groupItems;
                }
            }
            return null;
        }

        [Obsolete("This method is not in use now and will be discontinued in upcoming versions, as the parent method ZnodeOrderFulfillment.AddOrderToDatabase has been enhanced.")]
        //Set tax order to ZnodeOmsTaxOrderDetail table.
        public virtual void SaveTaxOrder(int orderDetailId, List<OrderLineItemModel> OrderLineItem, int orderId, decimal? taxRate)
        {
            //Map tax order details.
            ZnodeLogging.LogMessage("Input parameter to save tax order ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new Object[] { orderDetailId, OrderLineItem } );
            TaxOrderDetailsModel taxOrderDetailModel = MapTaxOrderDetails(orderDetailId, OrderLineItem);
            ZnodeLogging.LogMessage("Output From savetaxorder method", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose,taxOrderDetailModel);
            //Insert tax details to ZnodeOmsTaxOrderDetail table.
            _taxOrderDetailsRepository.Insert(taxOrderDetailModel.ToEntity<ZnodeOmsTaxOrderDetail>());

            //Insert tax rule details to ZnodeOmsTaxRule table.
            SaveOrderTaxDetails(orderId, OrderLineItem?.FirstOrDefault()?.TaxRuleId, taxRate);
        }

        [Obsolete("This method is not in use now and will be discontinued in upcoming versions, as the parent method ZnodeOrderFulfillment.AddOrderToDatabase has been enhanced.")]
        //Save tax rule for order to ZnodeOmsTaxRule table.
        public virtual bool SaveOrderTaxDetails(int orderId, int? taxRuleId, decimal? taxRate)
        {
            ZnodeLogging.LogMessage("Input parameter to save tax rule for an order ", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new Object[] { orderId, taxRuleId });
            int status = 0;
            if (orderId > 0 && taxRuleId > 0)
            {
                //Insert tax rule details to ZnodeOmsTaxRule table.
                IZnodeViewRepository<TaxOrderDetailsModel> objStoredProc = new ZnodeViewRepository<TaxOrderDetailsModel>();
                objStoredProc.SetParameter("@OmsOrderId", orderId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@TaxRuleId", Convert.ToInt32(taxRuleId), ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@TaxRate", Convert.ToDecimal(taxRate), ParameterDirection.Input, DbType.Decimal);
                objStoredProc.SetParameter("@status", null, ParameterDirection.Output, DbType.Int32);
                objStoredProc.ExecuteStoredProcedureList("Znode_InsertOrderTaxDetails @OmsOrderId, @TaxRuleId, @TaxRate, @status OUT", 2, out status);
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            }
            return status == 1;
        }

        //Save and update Downloadable product key to database.
        public virtual DataTable SaveDownloadableProductKey(DataTable orderDetails, int userId, int omsOrderDetailsId)
        {
            ExecuteSpHelper objStoredProc = new ExecuteSpHelper();
            objStoredProc.SetTableValueParameter("@OMSDownloadableProduct", orderDetails, ParameterDirection.Input, SqlDbType.Structured, "dbo.OMSDownloadableProduct");
            objStoredProc.GetParameter("@UserId", userId, ParameterDirection.Input, SqlDbType.Int);
            objStoredProc.GetParameter("@OmsOrderDetailsId", omsOrderDetailsId, ParameterDirection.Input, SqlDbType.Int);
            
            var savedCartLineItemList = objStoredProc.GetSPResultInDataSet("Znode_InsertUpdateDownloadableProductsOrderDetail");

            var productKeyTable = savedCartLineItemList.Tables[0];

            return productKeyTable;
        }

        //Save Downloadable product key to database.
        public virtual DataTable SaveDownloadableProductKeyWithJSON(DataTable orderDetails, int userId)
        {
            ExecuteSpHelper objStoredProc = new ExecuteSpHelper();
            objStoredProc.GetParameter("@OMSDownloadableProduct", orderDetails?.ToJson(), ParameterDirection.Input, SqlDbType.NVarChar);
            objStoredProc.GetParameter("@UserId", userId, ParameterDirection.Input, SqlDbType.Int);

            var savedCartLineItemList = objStoredProc.GetSPResultInDataSet("Znode_InsertUpdateDownloadableProductsOrderDetailWithJSON");

            var productKeyTable = savedCartLineItemList.Tables[0];

            return productKeyTable;
        }

        public virtual List<ZnodeOmsOrderLineItemRelationshipType> GetOmsOrderLineItemRelationshipTypeList()
        {
            IZnodeRepository<ZnodeOmsOrderLineItemRelationshipType> _omsOrderLineItemRelationshipType = new ZnodeRepository<ZnodeOmsOrderLineItemRelationshipType>();
            return _omsOrderLineItemRelationshipType.GetEntityList(string.Empty)?.ToList();
        }

        //to rollback failure order from database
        public virtual bool RollbackFailedOrder(int orderDetailId)
        {
            try
            {
                IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
                objStoredProc.SetParameter("OrderDetailId", orderDetailId, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                int status = 0;
                IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteOrderById @OrderDetailId, @Status OUT", 1, out status);
                if (deleteResult.FirstOrDefault().Status.Value)
                {
                    ZnodeLogging.LogMessage(Admin_Resources.SuccessOrderDelete);
                    return true;
                }
                else
                {
                    ZnodeLogging.LogMessage(Admin_Resources.ErrorOrderDelete);
                    return false;
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorOrderDelete, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error, new { ex }, "RollbackFailedOrder");
                return false;
            }
        }

        //To manage inventory for order
        public virtual bool ReturnOrderLineItems(ReturnOrderLineItemModel model)
        {
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("OrderLineItemIds", model.OmsOrderLineItemsId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("OmsOrderDetailsId", model.OrderDetailId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("OrderStateName", Convert.ToString(ZnodeOrderStatusEnum.RETURNED), ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("ReasonForReturnId", model.ReasonForReturnId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Quantity", model.GroupProducts?.Count > 0 ? model.GroupProducts?.FirstOrDefault()?.Quantity : model.Quantity, ParameterDirection.Input, DbType.Decimal);
            objStoredProc.SetParameter("IsShippingReturn", model.IsShippingReturn, ParameterDirection.Input, DbType.Boolean);
            objStoredProc.SetParameter("ShippingCost", model.ShippingCost, ParameterDirection.Input, DbType.Decimal);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            IList<View_ReturnBoolean> result = objStoredProc.ExecuteStoredProcedureList("Znode_ReturnOrderLineItem @OrderLineItemIds, @OmsOrderDetailsId, @OrderStateName,@ReasonForReturnId,@Quantity,@IsShippingReturn,@ShippingCost, @Status OUT", 7, out status);
            if (result.FirstOrDefault().Status.Value)
            {
                ReturnTaxCost(model);
                ZnodeLogging.LogMessage(Admin_Resources.SuccessOrderLineItemRuturn);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorOrderLineItemRuturn);
                return false;
            }
        }

        protected virtual void ReturnTaxCost(ReturnOrderLineItemModel model)
        {
            ZnodeOmsTaxOrderLineDetail taxOrderLine = _taxOrderLineRepository.Table.FirstOrDefault(x => x.OmsOrderLineItemsId == model.OmsOrderLineItemsId);
            if (HelperUtility.IsNotNull(taxOrderLine))
            {
                decimal quantity = GetQuantityByLineItemId(taxOrderLine.OmsOrderLineItemsId);
                taxOrderLine.GST = HelperUtility.IsNotNull(taxOrderLine.GST) ? (taxOrderLine.GST / quantity) * model.Quantity : taxOrderLine.GST;
                taxOrderLine.HST = HelperUtility.IsNotNull(taxOrderLine.HST) ? (taxOrderLine.HST / quantity) * model.Quantity : taxOrderLine.HST;
                taxOrderLine.PST = HelperUtility.IsNotNull(taxOrderLine.PST) ? (taxOrderLine.PST / quantity) * model.Quantity : taxOrderLine.PST;
                taxOrderLine.VAT = HelperUtility.IsNotNull(taxOrderLine.VAT) ? (taxOrderLine.VAT / quantity) * model.Quantity : taxOrderLine.VAT;
                taxOrderLine.SalesTax = HelperUtility.IsNotNull(taxOrderLine.SalesTax) ? (taxOrderLine.SalesTax / quantity) * model.Quantity : taxOrderLine.SalesTax;
                taxOrderLine.ImportDuty = HelperUtility.IsNotNull(taxOrderLine?.ImportDuty) ? (taxOrderLine?.ImportDuty / quantity) * model?.Quantity : taxOrderLine?.ImportDuty;                
                _taxOrderLineRepository.Update(taxOrderLine);
            }
        }

        //to get coupons with promotion message
        public virtual List<CouponModel> GetCouponPromotionMessages(string couponCodes)
        {
            List<CouponModel> couponlist = (from _promo in _promotionRepository.Table
                                            join _coupon in _promotionCouponRepository.Table on _promo.PromotionId equals _coupon.PromotionId
                                            where couponCodes.Equals(_coupon.Code, StringComparison.InvariantCultureIgnoreCase)
                                            select new CouponModel
                                            {
                                                Code = couponCodes.Substring(couponCodes.IndexOf(_coupon.Code), _coupon.Code.Length),
                                                PromotionMessage = _promo.PromotionMessage
                                            }).ToList();

            return couponlist ?? new List<CouponModel>();
        }

        //to check this state have option to send email
        public virtual bool IsSendEmail(int stateId)
             => Convert.ToBoolean(_orderStateRepository.Table.FirstOrDefault(x => x.OmsOrderStateId == stateId)?.IsSendEmail);

        //Get orderDefaultStateId by PortalId.
        public virtual int? GetorderDefaultStateId(int portalId)
           => _portalRepository.Table.Where(portal => portal.PortalId == portalId)?.Select(x=>x.DefaultOrderStateID).FirstOrDefault();

        //Get payment status Id by name
        public virtual int? GetPaymentStatusId(string paymentStatus)
        {
            IZnodeRepository<ZnodeOmsPaymentState> _paymentStateRepository = new ZnodeRepository<ZnodeOmsPaymentState>();
            return _paymentStateRepository.Table.Where(x => x.Name == paymentStatus).Select(x => x.OmsPaymentStateId).FirstOrDefault();
        }

        //Get LineItem Shipping Date.
        public virtual List<ZnodeOmsOrderLineItem> GetLineItemShippingDate(List<int?> omsOrderLineItemsIds)
          => _orderLineItemRepository.Table.Where(x => x.OrderLineItemStateId == 20 && omsOrderLineItemsIds.Contains(x.OmsOrderLineItemsId)).ToList();

        public virtual List<OrderLineItemModel> FormalizeOrderLineItems(OrderModel orderModel)
        {
            List<OrderLineItemModel> orderLineItemListModel = new List<OrderLineItemModel>();

            List<OrderLineItemModel> lineItems = orderModel.OrderLineItems.ToList();

            foreach (OrderLineItemModel _lineItem in lineItems)
            {
                OrderLineItemModel parentOrderLineItem = orderModel.OrderLineItems.FirstOrDefault(oli => oli.OmsOrderLineItemsId == _lineItem.ParentOmsOrderLineItemsId);

                int? parentOmsOrderLineItemsId = orderModel.OrderLineItems.Where(oli => oli.ParentOmsOrderLineItemsId == _lineItem.OmsOrderLineItemsId && oli.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Bundles)?.FirstOrDefault()?.ParentOmsOrderLineItemsId;
                if (parentOmsOrderLineItemsId.HasValue)
                {
                    _lineItem.ParentOmsOrderLineItemsId = parentOmsOrderLineItemsId;
                    _lineItem.Total = (_lineItem.Price * _lineItem.Quantity) + lineItems.Where(x => x.ParentOmsOrderLineItemsId == parentOmsOrderLineItemsId && x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns).Sum(y => y.Quantity * y.Price);
                    parentOrderLineItem = orderModel.OrderLineItems.Where(oli => oli.OmsOrderLineItemsId == parentOmsOrderLineItemsId).FirstOrDefault();
                }
                if (parentOrderLineItem != null && _lineItem.OrderLineItemRelationshipTypeId != (int)ZnodeCartItemRelationshipTypeEnum.Bundles && _lineItem.OrderLineItemRelationshipTypeId != null)
                {
                    if (_lineItem.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.Group)
                    {
                        _lineItem.Description = $"{_lineItem.ProductName}{"<br/>"}{ _lineItem.Description}";
                        _lineItem.ProductName = $"{parentOrderLineItem.ProductName}";
                    }

                    _lineItem.Total = (_lineItem.Price*_lineItem.Quantity)+lineItems.Where(x => x.ParentOmsOrderLineItemsId == _lineItem.OmsOrderLineItemsId).Sum(y => y.Quantity * y.Price);
                    _lineItem.PersonaliseValueList = parentOrderLineItem?.PersonaliseValueList;
                    _lineItem.PersonaliseValuesDetail = parentOrderLineItem?.PersonaliseValuesDetail;
                    _lineItem.GroupId = parentOrderLineItem.GroupId;

                    if (HelperUtility.IsNotNull(parentOrderLineItem.PersonaliseValuesDetail))
                    {
                        _lineItem.ProductName = $"<span style='vertical-align:top'>{parentOrderLineItem.ProductName}</span>";
                    }
                    if (_lineItem.OrderLineItemRelationshipTypeId != (int)ZnodeCartItemRelationshipTypeEnum.AddOns)
                    { orderLineItemListModel.Add(_lineItem); }
                    else
                    {
                        OrderLineItemModel parentItem = orderLineItemListModel?.FirstOrDefault(x => x.OmsOrderLineItemsId == _lineItem.ParentOmsOrderLineItemsId);
                        if (HelperUtility.IsNotNull(parentItem))
                        {
                            parentItem.Price =  parentItem.Price + _lineItem.Price;
                        }
                    }
                }
                else
                {
                    if (orderModel.OrderLineItems.Any(oli => oli.OmsOrderLineItemsId == _lineItem.OmsOrderLineItemsId) && _lineItem.OrderLineItemRelationshipTypeId != (int)ZnodeCartItemRelationshipTypeEnum.Bundles && _lineItem.OrderLineItemRelationshipTypeId != (int)ZnodeCartItemRelationshipTypeEnum.AddOns)
                    {
                        _lineItem.Price = _lineItem.Price  + lineItems.Where(x => x.ParentOmsOrderLineItemsId == _lineItem.OmsOrderLineItemsId && x.OrderLineItemRelationshipTypeId == (int)ZnodeCartItemRelationshipTypeEnum.AddOns).Sum(y => y.Price);
                        orderLineItemListModel.Add(_lineItem);
                    }

                }
            }

            return orderLineItemListModel;
        }

        public string GetInventoryRoundOff(String quantity)
        {
            string roundOff = _globalSettingRepository.Table.FirstOrDefault(x => x.FeatureName == GlobalSettingEnum.InventoryRoundOff.ToString())?.FeatureValues;
            roundOff = string.IsNullOrEmpty(roundOff) ? "0" : roundOff;
            decimal size = Convert.ToDecimal(quantity);
            return Convert.ToString(Math.Round((size), Convert.ToInt32(roundOff), MidpointRounding.AwayFromZero));
        }

        public decimal GetPriceRoundOff(decimal price)
        {
            string roundOff = _globalSettingRepository.Table.FirstOrDefault(x => x.FeatureName == GlobalSettingEnum.PriceRoundOff.ToString())?.FeatureValues;
            roundOff = string.IsNullOrEmpty(roundOff) ? "0" : roundOff;
            return Convert.ToDecimal(Math.Round((price), Convert.ToInt32(roundOff), MidpointRounding.AwayFromZero));
        }
      
        // Get valid giftcard by cardnumber
        public virtual GiftCardModel GetByCardNumber(string cardNumber)
        {
            return _giftCardRepository.Table.FirstOrDefault(x => x.CardNumber.ToLower() == cardNumber.ToLower() && x.IsActive == true && x.RemainingAmount > 0 && (DateTime.Today.Date >= x.StartDate && DateTime.Today.Date <= x.ExpirationDate))?.ToModel<GiftCardModel>() ?? new GiftCardModel();
        }

        //Get all active user vouchers.
        public virtual List<GiftCardModel> GetUserVouchers(int userId, int? portalId)
        {
            List<GiftCardModel> giftCardModel = new List<GiftCardModel>();
            if (userId > 0)
            {
                giftCardModel = _giftCardRepository.Table.Where(x => x.UserId == userId && x.PortalId == portalId && x.IsActive == true && x.RemainingAmount > 0 && (DateTime.Today.Date >= x.StartDate && DateTime.Today.Date <= x.ExpirationDate))?.OrderBy(c => c.ExpirationDate).ThenBy(c => c.RemainingAmount).ToModel<GiftCardModel>().ToList() ?? new List<GiftCardModel>();
            }
            return giftCardModel;
        }

        //to get applied voucher details.
        public virtual List<VoucherModel> GetVoucherDetailByCodes(string voucherCodes, int omsOrderId)
        {
            IZnodeViewRepository<VoucherModel> objStoredProc = new ZnodeViewRepository<VoucherModel>();
            List<VoucherModel> voucherlist = new List<VoucherModel>();
            objStoredProc.SetParameter("@VoucherCodes", voucherCodes, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@OmsOrderDetailsId", omsOrderId, ParameterDirection.Input, DbType.Int32);
            voucherlist = objStoredProc.ExecuteStoredProcedureList("Znode_GetGiftCardDetails @VoucherCodes, @OmsOrderDetailsId")?.ToList();
            return voucherlist ?? new List<VoucherModel>();
        }

        //Get Voucher By Voucher Number.
        public virtual GiftCardModel GetVoucherByCardNumber(string voucherNumber)
           => _giftCardRepository.Table.FirstOrDefault(x => x.CardNumber.ToLower() == voucherNumber.ToLower())?.ToModel<GiftCardModel>() ?? new GiftCardModel();


        public virtual void SetPerQuantityDiscountForReturnItem(List<OrderDiscountModel> allLineItemDiscountList, RMAReturnLineItemModel returnLineItem)
        {
            if (HelperUtility.IsNotNull(allLineItemDiscountList))
            {
                returnLineItem.PerQuantityLineItemDiscount = (decimal)allLineItemDiscountList?.Where(x => (x.OmsOrderLineItemId == returnLineItem.OmsOrderLineItemsId || x.OmsOrderLineItemId == returnLineItem.ParentOmsOrderLineItemsId || x.ParentOmsOrderLineItemsId == returnLineItem.OmsOrderLineItemsId) && x.OmsOrderLineItemId != null && (x.OmsDiscountTypeId == (int)OrderDiscountTypeEnum.PROMOCODE || x.OmsDiscountTypeId == (int)OrderDiscountTypeEnum.COUPONCODE) && x.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.LineItemLevel)?.Sum(y => y.PerQuantityDiscount);
                returnLineItem.PerQuantityCSRDiscount = (decimal)allLineItemDiscountList?.Where(x => (x.OmsOrderLineItemId == returnLineItem.OmsOrderLineItemsId || x.OmsOrderLineItemId == returnLineItem.ParentOmsOrderLineItemsId || x.ParentOmsOrderLineItemsId == returnLineItem.OmsOrderLineItemsId) && x.OmsOrderLineItemId != null && x.OmsDiscountTypeId == (int)OrderDiscountTypeEnum.CSRDISCOUNT)?.Sum(y => y.PerQuantityDiscount);
                returnLineItem.PerQuantityShippingDiscount = (decimal)allLineItemDiscountList?.Where(x => (x.OmsOrderLineItemId == returnLineItem.OmsOrderLineItemsId || x.OmsOrderLineItemId == returnLineItem.ParentOmsOrderLineItemsId || x.ParentOmsOrderLineItemsId == returnLineItem.OmsOrderLineItemsId) && x.OmsOrderLineItemId != null && (x.OmsDiscountTypeId == (int)OrderDiscountTypeEnum.PROMOCODE || x.OmsDiscountTypeId == (int)OrderDiscountTypeEnum.COUPONCODE) && x.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.ShippingLevel)?.Sum(y => y.PerQuantityDiscount);
                returnLineItem.PerQuantityOrderLevelDiscountOnLineItem = (decimal)allLineItemDiscountList?.Where(x => (x.OmsOrderLineItemId == returnLineItem.OmsOrderLineItemsId || x.OmsOrderLineItemId == returnLineItem.ParentOmsOrderLineItemsId || x.ParentOmsOrderLineItemsId == returnLineItem.OmsOrderLineItemsId) && x.OmsOrderLineItemId != null && (x.OmsDiscountTypeId == (int)OrderDiscountTypeEnum.PROMOCODE || x.OmsDiscountTypeId == (int)OrderDiscountTypeEnum.COUPONCODE) && x.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.OrderLevel)?.Sum(y => y.PerQuantityDiscount);
                returnLineItem.PerQuantityVoucherAmount = (decimal)allLineItemDiscountList?.Where(x => (x.OmsOrderLineItemId == returnLineItem.OmsOrderLineItemsId || x.OmsOrderLineItemId == returnLineItem.ParentOmsOrderLineItemsId || x.ParentOmsOrderLineItemsId == returnLineItem.OmsOrderLineItemsId) && x.OmsOrderLineItemId != null && (x.OmsDiscountTypeId == (int)OrderDiscountTypeEnum.GIFTCARD) && x.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.VoucherLevel)?.Sum(y => y.PerQuantityDiscount);
            }
        }

        public virtual List<OrderDiscountModel> GetReturnItemsDiscount(int rmaReturnDetailsId)
        {
            List<OrderDiscountModel> omsOrderDiscountList = null;
            if (rmaReturnDetailsId > 0)
            {
                IZnodeViewRepository<OrderDiscountModel> objStoredProc = new ZnodeViewRepository<OrderDiscountModel>();
                objStoredProc.SetParameter("@RmaReturnDetailsId", rmaReturnDetailsId, ParameterDirection.Input, DbType.Int32);
                omsOrderDiscountList = objStoredProc.ExecuteStoredProcedureList("Znode_GetDiscountDetails @RmaReturnDetailsId")?.ToList();
            }
            return omsOrderDiscountList;
        }

        // Save failed order transaction detail
        public virtual bool SaveFailedOrderTransaction(ShoppingCartModel shoppingCartModel)
        {
            try
            {
                ZnodeOmsFailedOrderPayment znodeOmsFailedOrderPayment = _omsFailedOrderPayment.Insert(ToOmsFailedOrderPaymentEntity(shoppingCartModel));
                return znodeOmsFailedOrderPayment.OmsFailedOrderPaymentId > 0 ? true : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(Admin_Resources.AddErrorMessage, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error, ex, "SaveFailedOrderTransaction");
                return false;
            }
        }
        #endregion Public Method

        #region Private Method

        //to extract thumbnail url from personalise value model.

        //To get user by user Id
        protected virtual UserModel GetUserById(int? userId)
        {
            IZnodeRepository<ZnodeUser> _userRepository = new ZnodeRepository<ZnodeUser>();
            UserModel user = _userRepository.Table.Where(x => x.UserId == userId && x.ReferralStatus == "A")?.FirstOrDefault().ToModel<UserModel>();
            return user ?? new UserModel();
        }

        //Map ShoppingCartModel model to ZnodeOmsFailedOrderPayment to save to table.
        protected virtual ZnodeOmsFailedOrderPayment ToOmsFailedOrderPaymentEntity(ShoppingCartModel shoppingCartModel)
        {
            return new ZnodeOmsFailedOrderPayment()
            {
                PaymentCode = shoppingCartModel.Payment.PaymentSetting.PaymentCode,
                PaymentDisplayName = shoppingCartModel.Payment.PaymentDisplayName,
                TransactionToken = shoppingCartModel.Token,
                TotalAmount = shoppingCartModel.Total ?? 0.0M,
                UserName = shoppingCartModel.UserDetails.FirstName + " " + shoppingCartModel.UserDetails.LastName,
                UserId = shoppingCartModel.UserId ?? 0,
                Email = shoppingCartModel.UserDetails.Email,
                PaymentSettingId = shoppingCartModel.Payment.PaymentSetting.PaymentSettingId,
                OrderNumber = shoppingCartModel.OrderNumber,
                OrderDate = shoppingCartModel.OrderDate ?? default(DateTime),
                PaymentStatusId = shoppingCartModel.Payment.PaymentStatusId,
                CreatedBy = shoppingCartModel.UserId ?? 0,
                CreatedDate = HelperUtility.GetDateTime(),
                ModifiedBy = shoppingCartModel.UserId ?? 0,
                ModifiedDate = HelperUtility.GetDateTime()
            };
        }

        //to save data in order table to get orderId
        private int SaveOrderId(OrderModel order)
        {
            int userId = HelperMethods.GetLoginAdminUserId();
            ZnodeOmsOrder newOrder = _orderRepository.Insert(new ZnodeOmsOrder()
            {
                CreatedBy = userId > 0 ? userId : order.UserId,
                ModifiedBy = userId > 0 ? userId : order.UserId,
                OrderNumber = order.OrderNumber,
                CreatedDate = HelperUtility.GetDateTime(),
                ModifiedDate = HelperUtility.GetDateTime(),
                ExternalId = order.OrderNumber,
                PublishStateId = (byte)order.PublishStateId,
            });
            return newOrder.OmsOrderId;
        }

        //to set order number and order create date.
        protected virtual void SetOrderData(OrderModel order)
        {
            if (order.OmsOrderId > 0)
            {
                var orderDetails = (from orderData in _orderRepository.Table
                                    join orderDetail in _orderDetailRepository.Table on orderData.OmsOrderId equals orderDetail.OmsOrderId
                                    where orderData.OmsOrderId == order.OmsOrderId && orderDetail.OmsOrderDetailsId == order.OmsOrderDetailsId
                                    select new
                                    {
                                        CreatedDate = orderData.CreatedDate,
                                        OrderNumber = orderData.OrderNumber,
                                        PaymentDisplayName = orderDetail.PaymentDisplayName,
                                        PaymentExternalId = orderDetail.PaymentExternalId,
                                        PaymentSettingId = orderDetail.PaymentSettingId,
                                    })?.FirstOrDefault();

                if (HelperUtility.IsNotNull(orderDetails))
                {
                    order.OrderNumber = orderDetails.OrderNumber;
                    order.OrderDate = orderDetails.CreatedDate;
                    if (order.PaymentSettingId == orderDetails?.PaymentSettingId)
                    {
                        order.PaymentDisplayName = orderDetails.PaymentDisplayName;
                        order.PaymentExternalId = orderDetails.PaymentExternalId;
                    }

                }
            }
        }

        protected virtual ZnodeOmsOrderShipment ToEntity(AddressModel model)
        {
            if (HelperUtility.IsNull(model))
                return null;

            ZnodeOmsOrderShipment orderShipment = new ZnodeOmsOrderShipment
            {
                ShipName = string.Concat(model.FirstName, " ", model.LastName),
                ShipToFirstName = model.FirstName,
                ShipToLastName = model.LastName,
                ShipToCompanyName = model.CompanyName,
                ShipToStreet1 = model.Address1,
                ShipToStreet2 = model.Address2,
                ShipToCity = model.CityName,
                ShipToStateCode = model.StateName,
                ShipToPostalCode = !string.IsNullOrEmpty(model.PostalCode) ? model.PostalCode.Trim() : model.PostalCode,
                ShipToCountry = model.CountryName,
                ShipToPhoneNumber = model.PhoneNumber,
                CreatedBy = model.CreatedBy,
                CreatedDate = HelperUtility.GetDateTime(),
                ModifiedBy = model.ModifiedBy,
                ModifiedDate = HelperUtility.GetDateTime()
            };
            return orderShipment;
        }

        protected virtual ZnodeOmsOrderDetail ToOrderDetailEntity(ZnodeOmsOrderDetail orderDetails, AddressModel model)
        {
            if (HelperUtility.IsNull(model))
                return orderDetails;
            int userId = HelperMethods.GetLoginAdminUserId();
            orderDetails.BillingFirstName = model.FirstName;
            orderDetails.BillingLastName = model.LastName;
            orderDetails.BillingCountry = model.CountryName;
            orderDetails.BillingStateCode = model.StateCode;
            orderDetails.BillingPostalCode = model.PostalCode;
            orderDetails.BillingPhoneNumber = model.PhoneNumber;
            orderDetails.BillingEmailId = model.EmailAddress;
            orderDetails.BillingStreet1 = model.Address1;
            orderDetails.BillingStreet2 = model.Address2;
            orderDetails.BillingCity = model.CityName;
            orderDetails.BillingCompanyName = model.CompanyName;
            orderDetails.CreatedBy = orderDetails?.CreatedBy > 0 ? orderDetails.CreatedBy : userId;
            orderDetails.ModifiedBy = userId > 0 ? userId : orderDetails.ModifiedBy;
            return orderDetails;
        }

        protected virtual PortalModel ToModel(ZnodePortal entity)
        {
            if (HelperUtility.IsNull(entity))
                return null;

            PortalModel model = new PortalModel
            {
                StoreName = entity.StoreName,
                CustomerServiceEmail = entity.CustomerServiceEmail,
                CustomerServicePhoneNumber = entity.CustomerServicePhoneNumber,
            };
            return model;
        }

        protected virtual SavedCartLineItemModel BindSaveCartLineItem(string cookieMappingId, int savedCartId, ShoppingCartItemModel shoppingCartItem, int publishCatalogId, int localeId)
        {
            SavedCartLineItemModel savedCartLineItemModel = BindSavedCartLineItemModel(cookieMappingId, savedCartId, shoppingCartItem, publishCatalogId, localeId);
            //Specified item sequence 0 for add to cart bundle product 

            BindLineItemProperties(shoppingCartItem, savedCartLineItemModel);
            return savedCartLineItemModel;
        }
        //Bind Saved Cart line item model.
        protected virtual SavedCartLineItemModel BindSavedCartLineItemModel(string cookieMappingId, int savedCartId, ShoppingCartItemModel shoppingCartItem, int publishCatalogId, int localeId)
         => new SavedCartLineItemModel
         {
             OmsSavedCartId = savedCartId,
             SKU = shoppingCartItem.SKU,
             Quantity = shoppingCartItem.GroupProducts?.Count > 0 ? shoppingCartItem.GroupProducts[0].Quantity : shoppingCartItem.Quantity,
             AddOnQuantity = GetAddonQuantity(shoppingCartItem),
             AddonProducts = shoppingCartItem.AddOnProductSKUs,
             BundleProducts = shoppingCartItem.BundleProductSKUs,
             ConfigurableProducts = shoppingCartItem.ConfigurableProductSKUs,
             GroupProducts = GetGroupProductLineItemXMLData(shoppingCartItem?.GroupProducts),
             PersonaliseValuesList = GetPersonalizedAttributeLineItemXMLData(shoppingCartItem?.PersonaliseValuesList, cookieMappingId),
             PersonaliseValuesDetail = (shoppingCartItem?.PersonaliseValuesDetail?.Count() > 0) ? shoppingCartItem.PersonaliseValuesDetail : GetPersonalizedAttributeLineItemDetails(shoppingCartItem?.PersonaliseValuesList, cookieMappingId),
             OmsOrderId = shoppingCartItem.OmsOrderId,
             AutoAddon = shoppingCartItem.AutoAddonSKUs,
             Custom1 = shoppingCartItem.Custom1,
             Custom2 = shoppingCartItem.Custom2,
             Custom3 = shoppingCartItem.Custom3,
             Custom4 = shoppingCartItem.Custom4,
             Custom5 = shoppingCartItem.Custom5,
             Description = shoppingCartItem.Description,
             ProductName = shoppingCartItem.ProductName,
             GroupId = shoppingCartItem.GroupId,
             CustomUnitPrice = shoppingCartItem.CustomUnitPrice
         };

        public virtual decimal GetAddonQuantity(ShoppingCartItemModel shoppingCartItem)
        {
            return  shoppingCartItem.GroupProducts?.Count > 0 ? shoppingCartItem.GroupProducts[0].Quantity : (shoppingCartItem.AssociatedAddOnProducts?.FirstOrDefault(x => x.Sku == shoppingCartItem.AddOnProductSKUs) != null ? shoppingCartItem.AssociatedAddOnProducts.FirstOrDefault(x => x.Sku == shoppingCartItem.AddOnProductSKUs).Quantity : shoppingCartItem.Quantity);
        }

        protected virtual void BindLineItemProperties(ShoppingCartItemModel shoppingCartItem, SavedCartLineItemModel savedCartLineItem)
        {
            savedCartLineItem.ParentOmsSavedCartLineItemId = shoppingCartItem.ParentOmsSavedcartLineItemId.GetValueOrDefault();
            savedCartLineItem.OmsSavedCartLineItemId = shoppingCartItem.OmsSavedcartLineItemId.GetValueOrDefault();
            savedCartLineItem.OrderLineItemRelationshipTypeId = shoppingCartItem.OrderLineItemRelationshipTypeId;
        }

        //Bind Saved Cart line item model.
        protected virtual void BindShoppingCartItemModel(ShoppingCartItemModel shoppingCartItem, int publishCatalogId, int localeId)
        {
            shoppingCartItem.AddOnProductSKUs = !string.IsNullOrEmpty(shoppingCartItem.AddOnProductSKUs) ? shoppingCartItem.AddOnProductSKUs : shoppingCartItem.ParentProductId > 0 ? GetAddOnProducts(shoppingCartItem.ParentProductId, publishCatalogId, localeId) : GetAddOnProducts(shoppingCartItem.ProductId, publishCatalogId, localeId);
            shoppingCartItem.BundleProductSKUs = !string.IsNullOrEmpty(shoppingCartItem.BundleProductSKUs) ? shoppingCartItem.BundleProductSKUs : GetBundleProducts(shoppingCartItem.ProductId, shoppingCartItem.ProductType, publishCatalogId, localeId);
            shoppingCartItem.ConfigurableProductSKUs = !string.IsNullOrEmpty(shoppingCartItem.ConfigurableProductSKUs) ? shoppingCartItem.ConfigurableProductSKUs : GetConfigurableProducts(shoppingCartItem.ProductId, shoppingCartItem.ProductType, publishCatalogId, localeId);
            shoppingCartItem.GroupProducts = (shoppingCartItem?.GroupProducts?.Count > 0) ? shoppingCartItem.GroupProducts : GetShoppingCartGroupProducts(shoppingCartItem.ProductId, shoppingCartItem.ProductType, publishCatalogId, localeId);
        }

        //Get configurable product filter.
        protected static FilterCollection GetAssociatedProductFilter(int localeId, string associatedProductIds)
        {
            FilterCollection filters = new FilterCollection();
            //Associated product ids.
            filters.Add(WebStoreEnum.ZnodeProductId.ToString(), FilterOperators.In, associatedProductIds);
            filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString());
            return filters;
        }

        //create new cookiemappingid for cart
        protected virtual int CreateCookieMappingId(int? userId, int portalId)
        {
            ZnodeOmsCookieMapping cookieMapping = _cookieMappingRepository.Insert(new ZnodeOmsCookieMapping()
            {
                UserId = userId == 0 ? null : userId,
                CreatedDate = HelperUtility.GetDateTime(),
                ModifiedDate = HelperUtility.GetDateTime(),
                PortalId = portalId
            });
            return Convert.ToInt32(cookieMapping?.OmsCookieMappingId);
        }

        //save order line items
        protected virtual void SaveTaxOrderLine(OrderLineItemModel orderLineItem)
        {
            if (TaxCalculated(orderLineItem))
            {
                //Map tax order line details.
                TaxOrderLineDetailsModel taxOrderLineModel = MapTaxOrderLineDetails(orderLineItem);

                //Insert tax order line details to ZnodeOmsTaxOrderLineDetail table.
                if (HelperUtility.IsNotNull(taxOrderLineModel))
                {
                    ZnodeOmsTaxOrderLineDetail taxOrderDetails = _taxOrderLineRepository.Insert(taxOrderLineModel.ToEntity<ZnodeOmsTaxOrderLineDetail>());
                }

                if (orderLineItem?.OrderLineItemCollection?.Count > 0)
                {
                    foreach (OrderLineItemModel childLineItems in orderLineItem.OrderLineItemCollection)
                    {
                        taxOrderLineModel = MapTaxOrderLineDetails(childLineItems);
                        if (HelperUtility.IsNotNull(taxOrderLineModel))
                        {
                            ZnodeOmsTaxOrderLineDetail childLineItem = _taxOrderLineRepository.Insert(taxOrderLineModel.ToEntity<ZnodeOmsTaxOrderLineDetail>());
                            childLineItems.OmsOrderLineItemsId = childLineItem.OmsOrderLineItemsId;
                        }
                    }
                }
            }
        }

        //Map tax order line details.
        protected static TaxOrderLineDetailsModel MapTaxOrderLineDetails(OrderLineItemModel orderLineItem)
        {
            if ((orderLineItem.HST + orderLineItem.VAT + orderLineItem.PST + orderLineItem.GST + orderLineItem.SalesTax) > 0)
            {
                return new TaxOrderLineDetailsModel()
                {
                    HST = orderLineItem.HST,
                    VAT = orderLineItem.VAT,
                    PST = orderLineItem.PST,
                    GST = orderLineItem.GST,
                    SalesTax = orderLineItem.SalesTax,
                    ImportDuty = orderLineItem.ImportDuty,
                    TaxTransactionNumber = orderLineItem.TaxTransactionNumber,
                    TaxRuleId = orderLineItem.TaxRuleId,
                    OmsOrderLineItemsId = orderLineItem.OmsOrderLineItemsId
                };
            }
            else
                return null;
        }

        //Map tax order details.
        protected virtual TaxOrderDetailsModel MapTaxOrderDetails(int orderDetailsId, List<OrderLineItemModel> orderLineItems)
        {
            if (HelperUtility.IsNotNull(orderLineItems) && orderLineItems.Count > 0)
            {
                return new TaxOrderDetailsModel()
                {
                    HST = orderLineItems.Sum(x => x.HST),
                    VAT = orderLineItems.Sum(x => x.VAT),
                    PST = orderLineItems.Sum(x => x.PST),
                    GST = orderLineItems.Sum(x => x.GST),
                    SalesTax = orderLineItems.Sum(x => x.SalesTax),
                    ImportDuty = orderLineItems.Sum(x => x.ImportDuty),
                    OmsOrderDetailsId = orderDetailsId
                };
            }
            else
            {
                return new TaxOrderDetailsModel();
            }
        }

        //Updates a specific shopping cart quantity of different includes simple, bundle, configurable and group product.
        protected virtual List<ShoppingCartItemModel> MergedOrderedQuantity(List<ShoppingCartItemModel> shoppingCart, List<ShoppingCartItemModel> updatedCart, ShoppingCartItemModel model, bool isProductEdit)
        {
            foreach (ShoppingCartItemModel mainShoppingCart in shoppingCart)
            {
                if (string.Equals(string.IsNullOrEmpty(mainShoppingCart.GroupId) ? mainShoppingCart.SKU : mainShoppingCart.GroupId, string.IsNullOrEmpty(model.GroupId) ? model.SKU : model.GroupId, StringComparison.OrdinalIgnoreCase))
                {
                    if (HelperUtility.IsNotNull(mainShoppingCart.GroupProducts) && mainShoppingCart.GroupProducts.Count > 0)
                    {
                        if (!Equals(model.ExternalId, mainShoppingCart.ExternalId))
                        {
                            AssociatedProductModel groupedProduct = mainShoppingCart.GroupProducts[0];

                            string groupProdSKU = groupedProduct.Sku;
                            string mainGroupProdSKU = model.SKU;

                            if (!string.IsNullOrEmpty(groupProdSKU) && !string.IsNullOrEmpty(mainGroupProdSKU)
                                && Equals(groupProdSKU.ToLower(), mainGroupProdSKU.ToLower()))
                            {
                                if (!string.IsNullOrEmpty(mainShoppingCart.AddOnProductSKUs) && !string.IsNullOrEmpty(model.AddOnProductSKUs)
                                    && Equals(mainShoppingCart.AddOnProductSKUs, model.AddOnProductSKUs))
                                {
                                    model.GroupProducts?.Where(x => x.Sku == groupedProduct.Sku).Select(x => x.Quantity = groupedProduct.Quantity).ToList();
                                    break;
                                }
                            }
                            MergeGroupProductQuantity(model, mainShoppingCart);
                        }
                    }
                    else if (!string.IsNullOrEmpty(mainShoppingCart.AddOnProductSKUs) || !string.IsNullOrEmpty(model.AddOnProductSKUs))
                    {
                        if (Equals(mainShoppingCart.AddOnProductSKUs, model.AddOnProductSKUs) && !Equals(model.ExternalId, mainShoppingCart.ExternalId))
                        {
                            if (HelperUtility.IsNotNull(mainShoppingCart.GroupProducts) && mainShoppingCart.GroupProducts.Count > 0)
                                //Merge group product quantity with same product.
                                MergeGroupProductQuantity(model, mainShoppingCart);

                            if (!string.IsNullOrEmpty(mainShoppingCart.ConfigurableProductSKUs))
                            {
                                if (mainShoppingCart.ConfigurableProductSKUs == model.ConfigurableProductSKUs)
                                    MergeCartQuantity(updatedCart, model, mainShoppingCart, isProductEdit);
                                break;
                            }
                            //Merge product quantity and external id.
                            MergeCartQuantity(updatedCart, model, mainShoppingCart, isProductEdit);
                            break;
                        }
                    }
                    else if (!string.IsNullOrEmpty(mainShoppingCart.ConfigurableProductSKUs) || !string.IsNullOrEmpty(model.ConfigurableProductSKUs))
                    {
                        if (Equals(mainShoppingCart.ConfigurableProductSKUs, model.ConfigurableProductSKUs) && !Equals(model.ExternalId, mainShoppingCart.ExternalId))
                        {
                            //Merge product quantity and external id.
                            MergeCartQuantity(updatedCart, model, mainShoppingCart, isProductEdit);
                            break;
                        }
                    }
                    else
                    {
                        if (!Equals(model.ExternalId, mainShoppingCart.ExternalId))
                        {
                            //Merge product quantity and external id.
                            MergeCartQuantity(updatedCart, model, mainShoppingCart, isProductEdit);
                            break;
                        }
                    }
                }
            }

            return updatedCart;
        }

        //Merge product quantity and external id.
        protected virtual void MergeCartQuantity(List<ShoppingCartItemModel> updatedCart, ShoppingCartItemModel model, ShoppingCartItemModel main, bool isProductEdit)
        {
            model.Quantity = isProductEdit ? main.Quantity : (model.Quantity + main.Quantity);
            model.ExternalId = main.ExternalId;

            if (main.PersonaliseValuesList?.Count > 0)
                model.PersonaliseValuesList = main.PersonaliseValuesList;

            updatedCart.Add(model);
        }


        //Merge group product quantity with same product.
        private void MergeGroupProductQuantity(ShoppingCartItemModel model, ShoppingCartItemModel main)
        {
            foreach (AssociatedProductModel groupedProduct in main.GroupProducts)
                model.GroupProducts.Where(x => x.Sku == groupedProduct.Sku).Select(x => x.Quantity == groupedProduct.Quantity).ToList();
        }

        //Insert into ZnodeOmsPersonalizeItem table.
        protected virtual void SavePersonalizeItem(int OmsOrderLineItemsId, OrderLineItemModel orderLineItem)
        {
            IZnodeRepository<ZnodeOmsPersonalizeItem> _PersonalizeItemRepository = new ZnodeRepository<ZnodeOmsPersonalizeItem>();
            List<ZnodeOmsPersonalizeItem> _OmsPersonalizeItem = new List<ZnodeOmsPersonalizeItem>();
            if (orderLineItem?.PersonaliseValueList?.Count > 0)
            {
                foreach (var item in orderLineItem.PersonaliseValueList)
                {
                    ZnodeOmsPersonalizeItem model = new ZnodeOmsPersonalizeItem(); model.OmsOrderLineItemsId = OmsOrderLineItemsId; model.PersonalizeCode = item.Key; model.PersonalizeValue = item.Value?.ToString();
                    _OmsPersonalizeItem.Add(model);
                }
            }
            else if (orderLineItem?.PersonaliseValuesDetail?.Count > 0)
            {
                foreach (var item in orderLineItem.PersonaliseValuesDetail)
                {
                    ZnodeOmsPersonalizeItem model = new ZnodeOmsPersonalizeItem(); model.OmsOrderLineItemsId = OmsOrderLineItemsId; model.PersonalizeCode = item.PersonalizeCode; model.PersonalizeValue = item.PersonalizeValue?.ToString();
                    model.ThumbnailURL = item.ThumbnailURL; model.DesignId = item.DesignId;
                    _OmsPersonalizeItem.Add(model);
                }
            }

            _PersonalizeItemRepository.Insert(_OmsPersonalizeItem);
        }

        //to map line item order attribute data in list of OrderAttributeModel
        protected virtual List<OrderAttributeModel> MapLineItemAttribute(OrderLineItemModel orderLineItem)
        {
            List<OrderAttributeModel> attributes = new List<OrderAttributeModel>();
            if (orderLineItem?.Attributes?.Count > 0)
            {
                orderLineItem.Attributes.ToList().ForEach(item => { attributes.Add(new OrderAttributeModel { OmsOrderLineItemsId = orderLineItem.OmsOrderLineItemsId, AttributeCode = item.AttributeCode, AttributeValue = item.AttributeValue, AttributeValueCode = item.AttributeValueCode }); });
            }
            return attributes;
        }

        //Get locale from code in personalize attributes.
        protected virtual string GetPersonaliseAttributeLocale(string personaliseValue, int? localeId)
        {
            int DefaultLocale = _localeRepository.Table.Where(i => i.IsDefault).Select(x => x.LocaleId).FirstOrDefault();
            if (localeId == null)
                localeId = DefaultLocale;

            string PersonaliseAttributeLocale = (from _pimAttr in _pimAttribute.Table
                                                 join _pimAttLocal in _pimAttributeLocale.Table on _pimAttr.PimAttributeId equals _pimAttLocal.PimAttributeId
                                                 where _pimAttLocal.LocaleId == localeId && _pimAttr.AttributeCode == personaliseValue
                                                 select _pimAttLocal.AttributeName).FirstOrDefault() ?? (from _pimAttr in _pimAttribute.Table
                                                                                                         join _pimAttLocal in _pimAttributeLocale.Table on _pimAttr.PimAttributeId equals _pimAttLocal.PimAttributeId
                                                                                                         where _pimAttLocal.LocaleId == (DefaultLocale) && _pimAttr.AttributeCode == personaliseValue
                                                                                                         select _pimAttLocal.AttributeName).FirstOrDefault();
            return HelperUtility.IsNull(PersonaliseAttributeLocale) ? string.Empty : PersonaliseAttributeLocale;
        }

        //Get Code from locale in personalize attributes.
        protected virtual string GetPersonaliseAttributeByLocal(string personaliseValue, int? localeId)
        {
            int DefaultLocale = _localeRepository.Table.Where(i => i.IsDefault).Select(x => x.LocaleId).FirstOrDefault();
            if (localeId == null)
                localeId = DefaultLocale;

            string PersonaliseAttributeLocale = (from _pimAttr in _pimAttribute.Table
                                                 join _pimAttLocal in _pimAttributeLocale.Table on _pimAttr.PimAttributeId equals _pimAttLocal.PimAttributeId
                                                 where _pimAttLocal.LocaleId == localeId && _pimAttLocal.AttributeName == personaliseValue
                                                 select _pimAttr.AttributeCode).FirstOrDefault() ?? (from _pimAttr in _pimAttribute.Table
                                                                                                     join _pimAttLocal in _pimAttributeLocale.Table on _pimAttr.PimAttributeId equals _pimAttLocal.PimAttributeId
                                                                                                     where _pimAttLocal.LocaleId == (DefaultLocale) && _pimAttLocal.AttributeName == personaliseValue
                                                                                                     select _pimAttr.AttributeCode).FirstOrDefault();
            return HelperUtility.IsNull(PersonaliseAttributeLocale) ? personaliseValue : PersonaliseAttributeLocale;
        }

        //Get Value from locale in personalize attributes.
        protected virtual string GetPersonaliseAttributeValueByLocal(string personaliseValue, int? localeId)
        {
            int DefaultLocale = _localeRepository.Table.Where(i => i.IsDefault).Select(x => x.LocaleId).FirstOrDefault();
            if (localeId == null)
                localeId = DefaultLocale;

            string PersonaliseAttributeLocaleValue = (from _pimAttr in _pimAttribute.Table
                                                      join _pimAttLocal in _pimAttributeLocale.Table on _pimAttr.PimAttributeId equals _pimAttLocal.PimAttributeId
                                                      where _pimAttLocal.LocaleId == localeId && _pimAttr.AttributeCode == personaliseValue
                                                      select _pimAttLocal.AttributeName).FirstOrDefault();
            return HelperUtility.IsNull(PersonaliseAttributeLocaleValue) ? personaliseValue : PersonaliseAttributeLocaleValue;
        }

        //to generate unique order number on basis of current date
        public virtual string GenerateOrderNumber()
        {
            string orderNumber = string.Empty;
            if (!string.IsNullOrEmpty(ZnodeConfigManager.SiteConfig.StoreName))
                orderNumber = ZnodeConfigManager.SiteConfig.StoreName.Trim().Length > 2 ? ZnodeConfigManager.SiteConfig.StoreName.Substring(0, 2) : ZnodeConfigManager.SiteConfig.StoreName.Substring(0, 1);

            DateTime date = DateTime.Now;
            String strDate = date.ToString("yyMMdd-HHmmss-fff");
            orderNumber += $"-{strDate}";
            return orderNumber.ToUpper();
        }

        //Bind Order model for submitting order
        public virtual PlaceOrderModel BindOrderModel(OrderModel orderModel)
        {
            if(HelperUtility.IsNotNull(orderModel))
            {
                return orderModel.ToModel<PlaceOrderModel, OrderModel>();
            }
            return null;
        }

        //Bind Line Item model for submitting order
        public virtual PlaceOrderLineItemModel BindLineItemOrderModel(OrderLineItemModel orderLineItemModel)
        {
            if (HelperUtility.IsNotNull(orderLineItemModel))
            {
                return orderLineItemModel.ToModel<PlaceOrderLineItemModel, OrderLineItemModel>();
            }
            return null;
        }

        //Bind ChildLineItem model for submitting order
        public virtual PlaceOrderlineItemCollection BindChildItemOrderModel(OrderLineItemModel orderLineItemModel)
        {
            if (HelperUtility.IsNotNull(orderLineItemModel))
            {
                return orderLineItemModel.ToModel<PlaceOrderlineItemCollection, OrderLineItemModel>();
            }
            return null;
        }

        //Bind discount model for submitting order
        public virtual List<PlaceOrderDiscountModel> BindDiscountModel(List<OrderDiscountModel> orderDiscountModel)
        {
            if (HelperUtility.IsNotNull(orderDiscountModel))
            {
                return orderDiscountModel.ToModel<PlaceOrderDiscountModel, OrderDiscountModel>().ToList();
            }
            return new List<PlaceOrderDiscountModel>();
        }

        //to check tax calculated for line items
        protected virtual bool TaxCalculated(OrderLineItemModel model)
        {
            bool isCalculated = false;
            if (model.HST > 0
                || model.VAT > 0
                || model.PST > 0
                || model.GST > 0
                || model.SalesTax > 0)
            {
                isCalculated = true;
            }
            else
            {
                isCalculated = Convert.ToBoolean(model?.OrderLineItemCollection.Any(x => (x.HST + x.VAT + x.PST + x.GST + x.SalesTax) > 0));
            }
            return isCalculated;
        }

        //get gift card with order amount
        protected virtual GiftCardModel GetGiftCardWithOrderAmount(string cardNumber, int? orderId = null)
        {
            GiftCardModel model = (from _gc in _giftCardRepository.Table
                                   join _hst in _giftCardHistoryRepository.Table on _gc.GiftCardId equals _hst.GiftCardId
                                   join _details in _orderDetailRepository.Table on _hst.OmsOrderDetailsId equals _details.OmsOrderDetailsId
                                   where _gc.CardNumber.ToLower() == cardNumber.ToLower() && _details.IsActive && _details.OmsOrderId == orderId
                                   select new GiftCardModel
                                   {
                                       GiftCardId = _gc.GiftCardId,
                                       PortalId = _gc.PortalId,
                                       Name = _gc.Name,
                                       CardNumber = _gc.CardNumber,
                                       Amount = _gc.Amount,
                                       RemainingAmount = (_gc.RemainingAmount + _hst.TransactionAmount),
                                       TransactionAmount = _hst.TransactionAmount,
                                       UserId = _gc.UserId,
                                       ExpirationDate = _gc.ExpirationDate,
                                       CreatedBy = _gc.CreatedBy,
                                       CreatedDate = _gc.CreatedDate,
                                       ModifiedBy = _gc.ModifiedBy,
                                       ModifiedDate = _gc.ModifiedDate
                                   }).FirstOrDefault();
            return model ?? _giftCardRepository.Table.FirstOrDefault(x => x.CardNumber.ToLower() == cardNumber.ToLower())?.ToModel<GiftCardModel>() ?? new GiftCardModel();
        }

        //to refund gift card amount
        [Obsolete("Need to implement new refund logic for voucher currently it is out of scope")]
        protected virtual void RefundGiftCardAmount(int orderId, string giftcard)
        {
            GiftCardModel model = GetGiftCardWithOrderAmount(giftcard, orderId);
            if (HelperUtility.IsNotNull(model?.TransactionAmount))
            {
                ZnodeGiftCard giftCard = _giftCardRepository.Table.FirstOrDefault(x => x.CardNumber == giftcard);
                if (HelperUtility.IsNotNull(giftCard))
                {
                    giftCard.Amount = (giftCard.Amount + model.TransactionAmount.Value);
                    _giftCardRepository.Update(giftCard);
                }
            }
        }

        //to add auto addon products to shopping cart
        protected virtual int AddAutoAddonProductsToCart(int sequence, int savedCartId, ShoppingCartItemModel cartItemModel, ShoppingCartModel shoppingCart, List<SavedCartLineItemModel> savedCartLineItem)
        {
            if (HelperUtility.IsNull(cartItemModel))
                return sequence;

            if (!string.IsNullOrEmpty(cartItemModel?.AutoAddonSKUs)
                && string.IsNullOrEmpty(shoppingCart.RemoveAutoAddonSKU)
                && !IsAutoAddonAdded(cartItemModel?.AutoAddonSKUs, shoppingCart.ShoppingCartItems))
            {
                string[] autoAddSKUS = cartItemModel?.AutoAddonSKUs.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string sku in autoAddSKUS)
                {
                    ShoppingCartItemModel autoAddonItem = new ShoppingCartItemModel
                    {
                        SKU = sku,
                        Quantity = 1,
                        AddOnProductSKUs = string.Empty,
                        BundleProductSKUs = string.Empty,
                        ConfigurableProductSKUs = string.Empty,
                        GroupProducts = null,
                        PersonaliseValuesList = null,
                        OmsOrderId = cartItemModel.OmsOrderId,
                        AutoAddonSKUs = sku
                    };
                    sequence++;
                    savedCartLineItem.Add(BindSavedCartLineItemModel(shoppingCart.CookieMappingId, savedCartId, autoAddonItem, shoppingCart.PublishedCatalogId, shoppingCart.LocaleId));//, sequence));
                }
            }
            return sequence;
        }

        //to check auto addon product is added to cart
        protected virtual bool IsAutoAddonAdded(string autoAddonSKUs, List<ShoppingCartItemModel> cartItems)
        {
            bool result = false;
            string[] autoAddSKUS = autoAddonSKUs.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (autoAddSKUS.Length > 0)
            {
                foreach (ShoppingCartItemModel item in cartItems)
                {
                    foreach (string sku in autoAddSKUS)
                    {
                        if ((string.Equals(item.SKU.Trim(), sku.Trim(), StringComparison.OrdinalIgnoreCase)))
                            return true;
                    }
                }
            }
            return result;
        }

        //Get portal logo by portal id
        protected virtual string GetPortalLogo(int portalId)
        {
            IZnodeRepository<ZnodePublishWebstoreEntity> _publishWebstoreEntity = new ZnodeRepository<ZnodePublishWebstoreEntity>(HelperMethods.Context);
            string logo = _publishWebstoreEntity.Table.Where(x => x.PortalId == portalId)?.Select(x => x.WebsiteLogo)?.FirstOrDefault();
            return logo ?? string.Empty;
        }

        //Get media Path
        protected virtual string getMediaPath()
        {
            IZnodeRepository<ZnodeMediaConfiguration> _mediaConfigRepository = new ZnodeRepository<ZnodeMediaConfiguration>();
            return _mediaConfigRepository.Table.Where(x => x.IsActive).Select(x => x.URL)?.FirstOrDefault();
        }

        //Generate filters for Portal Id.
        protected static FilterCollection PortalFilter(int portalId)
           => new FilterCollection() { new FilterTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()) };

        //to save partial discount of line item
        protected virtual void SavePartialDiscount(ZnodeOmsOrderLineItem lineItem)
        {
            if (HelperUtility.IsNotNull(lineItem?.PartialRefundAmount) && lineItem?.Quantity > 0)
            {
                ZnodeOmsOrderDiscount discount = new ZnodeOmsOrderDiscount();
                discount.OmsOrderLineItemId = lineItem.OmsOrderLineItemsId;
                discount.OmsDiscountTypeId = (int)OrderDiscountTypeEnum.PARTIALREFUND;
                discount.DiscountCode = lineItem.ProductName;
                discount.DiscountAmount = lineItem.PartialRefundAmount;
                ZnodeOmsOrderDiscount orderDiscount = _omsOrderDiscountRepository.Insert(discount);
            }
        }

        //to get quantity by lineitem Id
        protected virtual decimal GetQuantityByLineItemId(int lineItemId)
        {
            ZnodeOmsOrderLineItem item = _orderLineItemRepository.GetById(lineItemId);
            if (HelperUtility.IsNotNull(item))
            {
                return item.Quantity.GetValueOrDefault();
            }
            return 0;
        }

        //For Getting personalize attribute.       
        public virtual string GetPersonaliseAttributes(Dictionary<string, object> personaliseValueList)
        {
            personaliseValueList.Remove("AllocatedLineItems");
            string personalizeAttributeHtml = string.Empty;
            if (HelperUtility.IsNotNull(personaliseValueList))
            {
                foreach (var personalizeAttribute in personaliseValueList)
                    personalizeAttributeHtml = string.Concat(personalizeAttributeHtml, $"{"<p>"} { personalizeAttribute.Key}{" : "}{personalizeAttribute.Value}{"</p>"}");

                return personalizeAttributeHtml;
            }
            return string.Empty;
        }

        //Set Personalize Order Details.
        public virtual void SetPersonalizeDetails(OrderLineItemModel lineItem)
        {
            IZnodeRepository<ZnodeOmsPersonalizeItem> _personalizeItemRepository = new ZnodeRepository<ZnodeOmsPersonalizeItem>();
            List<ZnodeOmsPersonalizeItem> lastPersonalizeDetails = _personalizeItemRepository.Table.Where(x => x.OmsOrderLineItemsId == lineItem.OmsOrderLineItemsId).ToList();

            lineItem.PersonaliseValuesDetail = lastPersonalizeDetails.Select(x => (new PersonaliseValueModel() { PersonalizeCode = x.PersonalizeCode, PersonalizeValue = x.PersonalizeValue, DesignId = x.DesignId, ThumbnailURL = x.ThumbnailURL })).ToList();
            lineItem.PersonaliseValueList = lastPersonalizeDetails.ToDictionary(x => x.PersonalizeCode, x => (object)x.PersonalizeValue);
        }

        public virtual List<OrderDiscountModel> GetOmsOrderDiscountList(int omsOrderId)
        {
            List<OrderDiscountModel> omsOrderDiscountList = null;
            if (omsOrderId > 0)
            {
                omsOrderDiscountList = (from _disc in _omsOrderDiscountRepository.Table
                                        join znodeOrderDetail in _orderDetailRepository.Table on _disc.OmsOrderDetailsId equals znodeOrderDetail.OmsOrderDetailsId
                                        where znodeOrderDetail.OmsOrderId == omsOrderId
                                        select new OrderDiscountModel
                                        {
                                            OmsOrderLineItemId = _disc.OmsOrderLineItemId,
                                            OmsOrderDiscountId = _disc.OmsOrderDiscountId,
                                            DiscountCode = _disc.DiscountCode,
                                            DiscountAmount = _disc.DiscountAmount,
                                            Description = _disc.Description,
                                            OmsDiscountTypeId = _disc.OmsDiscountTypeId,
                                            PerQuantityDiscount = (decimal)_disc.PerQuantityDiscount,
                                            ParentOmsOrderLineItemsId = _disc.ParentOmsOrderLineItemsId,
                                            DiscountLevelTypeId = _disc.DiscountLevelTypeId,
                                            PromotionTypeId = _disc.PromotionTypeId,
                                            DiscountAppliedSequence = _disc.DiscountAppliedSequence
                                        }).ToList<OrderDiscountModel>();
            }
            return omsOrderDiscountList;
        }

        public virtual List<OrderDiscountModel> GetReturnItemsDiscountList(int omsOrderId)
        {
            List<OrderDiscountModel> omsOrderDiscountList = null;
            if (omsOrderId > 0)
            {
                omsOrderDiscountList = (from _disc in _omsOrderDiscountRepository.Table
                                        join znodeOrderDetail in _orderDetailRepository.Table
                                        on new { _disc.OmsOrderDetailsId, OmsOrderId=omsOrderId }
                                        equals new { OmsOrderDetailsId =(int?)znodeOrderDetail.OmsOrderDetailsId, znodeOrderDetail.OmsOrderId }
                                        from returnItem in _orderLineItemRepository.Table.Where(x =>
                                        (_disc.OmsOrderLineItemId == x.OmsOrderLineItemsId ||
                                        (_disc.OmsOrderLineItemId == x.ParentOmsOrderLineItemsId && x.OrderLineItemRelationshipTypeId != (int)ZnodeCartItemRelationshipTypeEnum.AddOns))
                                        && x.OrderLineItemRelationshipTypeId != (int)ZnodeCartItemRelationshipTypeEnum.Bundles)
                                        where znodeOrderDetail.OmsOrderId == omsOrderId && returnItem.OrderLineItemStateId == ZnodeConstant.OmsOrderReturnState && _disc.OmsOrderLineItemId != null
                                        select new OrderDiscountModel
                                        {
                                            OmsOrderLineItemId = _disc.OmsOrderLineItemId,
                                            OmsOrderDiscountId = _disc.OmsOrderDiscountId,
                                            DiscountCode = _disc.DiscountCode,
                                            DiscountAmount = _disc.DiscountAmount,
                                            Description = _disc.Description,
                                            OmsDiscountTypeId = _disc.OmsDiscountTypeId,
                                            PerQuantityDiscount = (decimal)_disc.PerQuantityDiscount,
                                            ParentOmsOrderLineItemsId = _disc.ParentOmsOrderLineItemsId,
                                            DiscountLevelTypeId = _disc.DiscountLevelTypeId,
                                            PromotionTypeId= _disc.PromotionTypeId,
                                            DiscountAppliedSequence = _disc.DiscountAppliedSequence
                                        }).ToList<OrderDiscountModel>();

            }
            return omsOrderDiscountList;
        }

        public virtual void CalculateCSRDiscount(ShoppingCartModel shoppingCartModel)
        {
            if (HelperUtility.IsNotNull(shoppingCartModel?.ShoppingCartItems) && shoppingCartModel?.ShoppingCartItems.Count > 0)
            {
                shoppingCartModel.CSRDiscountApplied = true;
                shoppingCartModel.ReturnCSRDiscount = (shoppingCartModel?.ShoppingCartItems.Where(x => x.OrderLineItemStatus.ToUpper() == ZnodeOrderStatusEnum.RETURNED.ToString()).Sum(x => x.PerQuantityCSRDiscount * x.Quantity)).GetValueOrDefault();
                shoppingCartModel.CSRDiscountAmount = (shoppingCartModel?.ShoppingCartItems.Where(x => x.OrderLineItemStatus.ToUpper() != ZnodeOrderStatusEnum.RETURNED.ToString()).Sum(x => x.PerQuantityCSRDiscount * x.Quantity)).GetValueOrDefault();
                shoppingCartModel.CSRDiscountAmount = shoppingCartModel.CSRDiscountAmount - shoppingCartModel.ReturnCSRDiscount;
                shoppingCartModel.ReturnCSRDiscount = shoppingCartModel.ReturnCSRDiscount;
            }
        }



        public virtual void SetPerQuantityDiscount(List<OrderDiscountModel> allLineItemDiscountList, ShoppingCartItemModel shoppingCartItem)
        {
            if (HelperUtility.IsNotNull(allLineItemDiscountList))
            {
                shoppingCartItem.PerQuantityLineItemDiscount = (decimal)allLineItemDiscountList?.Where(x => (x.OmsOrderLineItemId == shoppingCartItem.OmsOrderLineItemsId || x.OmsOrderLineItemId == shoppingCartItem.ParentOmsOrderLineItemsId || x.ParentOmsOrderLineItemsId == shoppingCartItem.OmsOrderLineItemsId) && x.OmsOrderLineItemId != null && (x.OmsDiscountTypeId == (int)OrderDiscountTypeEnum.PROMOCODE || x.OmsDiscountTypeId == (int)OrderDiscountTypeEnum.COUPONCODE) && x.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.LineItemLevel)?.Sum(y => y.PerQuantityDiscount);
                shoppingCartItem.PerQuantityCSRDiscount = (decimal)allLineItemDiscountList?.Where(x => (x.OmsOrderLineItemId == shoppingCartItem.OmsOrderLineItemsId || x.OmsOrderLineItemId == shoppingCartItem.ParentOmsOrderLineItemsId || x.ParentOmsOrderLineItemsId == shoppingCartItem.OmsOrderLineItemsId) && x.OmsOrderLineItemId != null && x.OmsDiscountTypeId == (int)OrderDiscountTypeEnum.CSRDISCOUNT)?.Sum(y => y.PerQuantityDiscount);
                shoppingCartItem.PerQuantityShippingDiscount = (decimal)allLineItemDiscountList?.Where(x => (x.OmsOrderLineItemId == shoppingCartItem.OmsOrderLineItemsId || x.OmsOrderLineItemId == shoppingCartItem.ParentOmsOrderLineItemsId || x.ParentOmsOrderLineItemsId == shoppingCartItem.OmsOrderLineItemsId) && x.OmsOrderLineItemId != null && (x.OmsDiscountTypeId == (int)OrderDiscountTypeEnum.PROMOCODE || x.OmsDiscountTypeId == (int)OrderDiscountTypeEnum.COUPONCODE) && x.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.ShippingLevel)?.Sum(y => y.PerQuantityDiscount);
                shoppingCartItem.PerQuantityOrderLevelDiscountOnLineItem = (decimal)allLineItemDiscountList?.Where(x => (x.OmsOrderLineItemId == shoppingCartItem.OmsOrderLineItemsId || x.OmsOrderLineItemId == shoppingCartItem.ParentOmsOrderLineItemsId || x.ParentOmsOrderLineItemsId == shoppingCartItem.OmsOrderLineItemsId) && x.OmsOrderLineItemId != null && (x.OmsDiscountTypeId == (int)OrderDiscountTypeEnum.PROMOCODE || x.OmsDiscountTypeId == (int)OrderDiscountTypeEnum.COUPONCODE) && x.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.OrderLevel)?.Sum(y => y.PerQuantityDiscount);
                shoppingCartItem.PerQuantityVoucherAmount = (decimal)allLineItemDiscountList?.Where(x => (x.OmsOrderLineItemId == shoppingCartItem.OmsOrderLineItemsId || x.OmsOrderLineItemId == shoppingCartItem.ParentOmsOrderLineItemsId || x.ParentOmsOrderLineItemsId == shoppingCartItem.OmsOrderLineItemsId) && x.OmsOrderLineItemId != null && (x.OmsDiscountTypeId == (int)OrderDiscountTypeEnum.GIFTCARD) && x.DiscountLevelTypeId == (int)DiscountLevelTypeIdEnum.VoucherLevel)?.Sum(y => y.PerQuantityDiscount);
            }
        }

        //To get coupons with promotion message
        public virtual List<CouponModel> GetCouponPromotionMessages(string[] couponCodes, bool isOldOrder, int omsOrderId = 0)
        {
            List<CouponModel> couponList = new List<CouponModel>();
            if (omsOrderId > 0 && !isOldOrder)
            {
                couponList = (from _orderDiscount in _omsOrderDiscountRepository.Table
                              join _orderDetail in _orderDetailRepository.Table on _orderDiscount.OmsOrderDetailsId equals _orderDetail.OmsOrderDetailsId
                              where couponCodes.Any(x => x.Equals(_orderDiscount.DiscountCode, StringComparison.InvariantCultureIgnoreCase))
                              && _orderDetail.OmsOrderId == omsOrderId && _orderDetail.IsActive == true
                              select new CouponModel
                              {
                                  Code = _orderDiscount.DiscountCode,
                                  PromotionMessage = _orderDiscount.PromotionMessage
                              }).Distinct().ToList();
            }
            else
            {
                couponList = (from _promo in _promotionRepository.Table
                              join _coupon in _promotionCouponRepository.Table on _promo.PromotionId equals _coupon.PromotionId
                              where couponCodes.Any(x => x.Equals(_coupon.Code, StringComparison.InvariantCultureIgnoreCase))
                              select new CouponModel
                              {
                                  Code = _coupon.Code,
                                  PromotionMessage = _promo.PromotionMessage
                              }).ToList();
            }
            return couponList ?? new List<CouponModel>();
        }

        #endregion Private Method
    }
}