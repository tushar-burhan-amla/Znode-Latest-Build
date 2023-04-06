using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.WebStore.Core.Agents
{
    public class SavedCartAgent : BaseAgent, ISavedCartAgent
    {

        #region Protected Readonly Variables
        protected readonly ISavedCartClient _savedCardClient;
        protected readonly ICartAgent _cartAgent;
        protected readonly IAccountQuoteClient _accountQuoteClient;
        protected readonly ISaveForLaterAgent _saveForLaterAgent;
        #endregion

        #region Constructor
        public SavedCartAgent(ISavedCartClient saveCartClient, ICartAgent cartAgent, IAccountQuoteClient accountQuoteClient, ISaveForLaterAgent saveForLaterAgent)
        {
            _savedCardClient = GetClient<ISavedCartClient>(saveCartClient);
            _cartAgent = cartAgent;
            _accountQuoteClient = accountQuoteClient;
            _saveForLaterAgent = saveForLaterAgent;
        }
        #endregion

        #region Public Method
        public virtual TemplateViewModel CreateSavedCart(string templateName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                if (!string.IsNullOrEmpty(templateName))
                {
                    List<DropdownListModel> templateListViewModel = GetSavedCart();

                    var match = templateListViewModel.FirstOrDefault(x => x.Name.ToLower() == templateName.ToLower());

                    if (HelperUtility.IsNull(match))
                    {
                        ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);

                        AccountTemplateModel templateModel = new AccountTemplateModel();
                        SetTemplateModelData(templateModel, templateName);

                        templateModel.TemplateCartItems = cartModel.ShoppingCartItems?.ToViewModel<TemplateCartItemModel>().ToList();

                        List<ShoppingCartItemModel> cartItem = cartModel.ShoppingCartItems.Where(x => x.ProductType == ZnodeConstant.BundleProduct).ToList();

                        if (HelperUtility.IsNotNull(cartItem) && cartItem.Count > 0)
                        {
                            foreach (var item in cartItem)
                            {
                                _saveForLaterAgent.SetSelectedBundleProductsForAddToCart(item, templateModel, item.BundleProductSKUs);
                            }
                        }

                        AccountTemplateModel accountTemplateModel = _savedCardClient.CreateSavedCart(templateModel);
                        return HelperUtility.IsNotNull(accountTemplateModel) ? accountTemplateModel.ToViewModel<TemplateViewModel>() : null;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return null;
            }
        }

        public virtual List<DropdownListModel> GetSavedCart()
        {
            TemplateListViewModel templateListViewModel = _cartAgent.GetTemplateList(null, null, null, null, ZnodeConstant.SavedCart);
            List<DropdownListModel> template = templateListViewModel.List.Select(x => new DropdownListModel { Name = x.TemplateName, Id = x.OmsTemplateId }).ToList();
            return template;
        }

        public virtual bool EditSaveCart(int templateId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Info);
            try
            {
                if (HelperUtility.IsNotNull(templateId) && templateId > 0)
                {
                    AccountTemplateModel templateModel = new AccountTemplateModel();

                    ShoppingCartModel cartModel = GetFromSession<ShoppingCartModel>(WebStoreConstants.CartModelSessionKey);

                    templateModel.OmsTemplateId = templateId;
                    templateModel.TemplateType = ZnodeConstant.SavedCart;
                    templateModel.TemplateCartItems = cartModel.ShoppingCartItems?.ToViewModel<TemplateCartItemModel>().ToList();

                    List<ShoppingCartItemModel> cartItem = cartModel.ShoppingCartItems.Where(x => x.ProductType == ZnodeConstant.BundleProduct).ToList();

                    if (HelperUtility.IsNotNull(cartItem) && cartItem.Count > 0)
                    {
                        foreach (var item in cartItem)
                        {
                            _saveForLaterAgent.SetSelectedBundleProductsForAddToCart(item, templateModel, item.BundleProductSKUs);
                        }
                    }
                    bool status = _savedCardClient.EditSaveCart(templateModel);

                    return status;
                }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        public virtual string AddProductToCartForSaveCart(int omsTemplateId)
        {
            int userId = Convert.ToInt32(GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId);
            int portalId = PortalAgent.CurrentPortal.PortalId;
            string errorMessage = string.Empty;
            if (omsTemplateId > 0)
            {
                ExpandCollection expands = new ExpandCollection();
                expands.Add(ZnodeOmsTemplateEnum.ZnodeOmsTemplateLineItems.ToString());

                FilterCollection filters = GetRequiredFilters();
                AccountTemplateModel accountTemplateModel = _accountQuoteClient.GetAccountTemplate(omsTemplateId, expands, filters);
                errorMessage = _cartAgent.CheckTemplateData(accountTemplateModel);

                if (accountTemplateModel?.TemplateCartItems?.Count > 0 && string.IsNullOrEmpty(errorMessage))
                {
                    bool addToCart = _savedCardClient.AddProductToCartForSaveCart(omsTemplateId, userId, portalId);
                    _cartAgent.MergeGuestUserCart();
                    errorMessage = addToCart.ToString();
                }
            }
            return errorMessage;
        }

        //Update quantity of template cart item.
        public virtual TemplateViewModel UpdateSavedItemQuantity(string guid, decimal quantity, int productId)
        {
            // Get shopping cart from session.
            TemplateViewModel templateCart = GetFromSession<TemplateViewModel>(_cartAgent.GetSavedCartModelSessionKey());

            if (HelperUtility.IsNotNull(templateCart))
            {
                //Get shopping cart item on the basis of guid.
                templateCart = _cartAgent.GetTemplateItemByExternalId(templateCart, guid, quantity, productId);

                //Update shopping cart item quantity.
                if (HelperUtility.IsNotNull(templateCart))
                {
                    _cartAgent.ClearCartCountFromSession();
                    return _cartAgent.UpdateTemplateItemQuantity(templateCart, guid, quantity, productId);
                }
            }
            return new TemplateViewModel();
        }

        //Create template.
        public virtual bool EditSavedCartItem(TemplateViewModel templateViewModel)
        {
            AccountTemplateModel accountTemplateModel = templateViewModel.ToModel<AccountTemplateModel>();
            if (HelperUtility.IsNotNull(accountTemplateModel))
            {
                TemplateViewModel cartItems = GetFromSession<TemplateViewModel>(_cartAgent.GetSavedCartModelSessionKey());

                //Validate Template ID.
                if (accountTemplateModel?.OmsTemplateId > 0 && cartItems?.OmsTemplateId > 0)
                {
                    if (!HelperUtility.IsValidIdInQueryString(accountTemplateModel.OmsTemplateId, cartItems.OmsTemplateId))
                    {
                        templateViewModel.HasError = true;
                        templateViewModel.ErrorMessage = WebStore_Resources.HttpCode_401_AccessDeniedMsg;
                        return false;
                    }
                }

                accountTemplateModel.TemplateCartItems = cartItems?.TemplateCartItems.ToModel<TemplateCartItemModel>()?.ToList();
                _cartAgent.SetTemplateModel(accountTemplateModel, cartItems.TemplateType);
                templateViewModel = _accountQuoteClient.CreateTemplate(accountTemplateModel)?.ToViewModel<TemplateViewModel>();
                return HelperUtility.IsNotNull(templateViewModel);
            }
            return false;
        }

        public virtual bool EditSaveCartName(string templateName, int templateId)
        {
            List<DropdownListModel> templateListViewModel = GetSavedCart();

            if (HelperUtility.IsNotNull(templateListViewModel)) {
                var match = templateListViewModel.FirstOrDefault(x => x.Name.ToLower() == templateName.ToLower());

                if (HelperUtility.IsNull(match))
                {
                    bool status = _savedCardClient.EditSaveCartName(templateName, templateId);
                    return status;
                }
                return false; 
            }
            return false;
        }
        #endregion

        #region Protected Method
        protected virtual void SetTemplateModelData(AccountTemplateModel accountTemplateModel,string templateName)
        {
            accountTemplateModel.PortalId = PortalAgent.CurrentPortal.PortalId;
            accountTemplateModel.LocaleId = PortalAgent.LocaleId;
            accountTemplateModel.PublishedCatalogId = PortalAgent.CurrentPortal.PublishCatalogId;
            accountTemplateModel.UserId = Convert.ToInt32(GetFromSession<UserViewModel>(WebStoreConstants.UserAccountKey)?.UserId);
            accountTemplateModel.TemplateType = ZnodeConstant.SavedCart;
            accountTemplateModel.TemplateName = templateName;
        }
        #endregion

    }


}
