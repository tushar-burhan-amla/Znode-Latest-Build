using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class ProviderEngineAgent : BaseAgent, IProviderEngineAgent
    {
        #region Private Variables
        private readonly ITaxRuleTypeClient _taxRuleTypeClient;
        private readonly IShippingTypeClient _shippingTypeClient;
        private readonly IPromotionTypeClient _promotionTypeClient;
        #endregion

        #region Constructor
        public ProviderEngineAgent(ITaxRuleTypeClient taxRuleTypeClient, IShippingTypeClient shippingTypeClient, IPromotionTypeClient promotionTypeClient)
        {
            _taxRuleTypeClient = GetClient<ITaxRuleTypeClient>(taxRuleTypeClient);
            _shippingTypeClient = GetClient<IShippingTypeClient>(shippingTypeClient);
            _promotionTypeClient = GetClient<IPromotionTypeClient>(promotionTypeClient);
        }
        #endregion

        #region Public Methods

        #region TaxRule Type
        //Get all saved tax rule types.
        public virtual ProviderEngineListViewModel GetTaxRuleTypeList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters filters and sort collection:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new { Filters = filters, Sorts = sorts });
            ProviderEngineListViewModel providerEngineListViewModel = ProviderEngineViewModelMap.ToTaxRuleListViewModel(_taxRuleTypeClient.GetTaxRuleTypeList(filters, sorts, pageIndex, pageSize));
            SetTaxRuleTypeListToolMenus(providerEngineListViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return providerEngineListViewModel;
        }

        //Get tax rule type on the basis of Id.
        public virtual ProviderEngineViewModel GetTaxRuleType(int taxRuleTypeId)
            => ProviderEngineViewModelMap.ToTaxRuleViewModel(_taxRuleTypeClient.GetTaxRuleType(taxRuleTypeId));

        //Get all the tax rule types from sql cache.
        public virtual List<SelectListItem> GetAllTaxRuleTypesNotInDatabase()
            => ProviderEngineViewModelMap.ToTaxRuleListItems(_taxRuleTypeClient.GetAllTaxRuleTypesNotInDatabase().TaxRuleTypes);

        //Create new Tax Rule type.
        public virtual ProviderEngineViewModel CreateTaxRuleType(ProviderEngineViewModel taxRuleTypeViewModel)
            => _taxRuleTypeClient.CreateTaxRuleType(ProviderEngineViewModelMap.ToTaxRuleModel(taxRuleTypeViewModel))?.ToViewModel<ProviderEngineViewModel>();

        //Update Tax Rule type.
        public virtual ProviderEngineViewModel UpdateTaxRuleType(ProviderEngineViewModel taxRuleTypeViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                return ProviderEngineViewModelMap.ToTaxRuleViewModel(_taxRuleTypeClient.UpdateTaxRuleType(ProviderEngineViewModelMap.ToTaxRuleModel(taxRuleTypeViewModel)));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Warning);
                return (ProviderEngineViewModel)GetViewModelWithErrorMessage(new ProviderEngineViewModel(), ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Error);
                return (ProviderEngineViewModel)GetViewModelWithErrorMessage(new ProviderEngineViewModel(), Admin_Resources.UpdateError);
            }
        }

        //Delete Tax Rule type.
        public virtual bool DeleteTaxRuleType(string taxRuleTypeId, out string message)
        {
            message = Admin_Resources.DeleteErrorMessage;
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                return _taxRuleTypeClient.DeleteTaxRuleType(new ParameterModel { Ids = taxRuleTypeId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Warning);
                if (Equals(ex.ErrorCode, ErrorCodes.AssociationDeleteError))
                    message = Admin_Resources.ErrorDeleteTaxRuleType;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Get tax rule type on the basis of class name.
        public virtual ProviderEngineViewModel GetTaxRuleTypeByClassName(string name)
            => ProviderEngineViewModelMap.ToTaxRuleListViewModel(_taxRuleTypeClient.GetAllTaxRuleTypesNotInDatabase()).ProvideEngineTypes.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());

        //Enable/Disable tax rule types.
        public virtual bool BulkEnableDisableTaxRuleTypes(string taxRuleTypeId, bool isEnable, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            message = "";
            try
            {
                return _taxRuleTypeClient.BulkEnableDisableTaxRuleTypes(new ParameterModel { Ids = taxRuleTypeId }, isEnable);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Warning);
                message = ex.ErrorMessage;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Error);
                return false;
            }
        }
        #endregion

        #region Shipping Type
        //Get all saved shipping types.
        public virtual ProviderEngineListViewModel GetShippingTypeList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ProviderEngineListViewModel providerEngineListViewModel = ProviderEngineViewModelMap.ToShippingListViewModel(_shippingTypeClient.GetShippingTypeList(filters, sorts, pageIndex, pageSize));
            SetShippingTypeListToolMenus(providerEngineListViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return providerEngineListViewModel;
        }

        //Get shipping type on the basis of Id.
        public virtual ProviderEngineViewModel GetShippingType(int shippingTypeId)
            => ProviderEngineViewModelMap.ToShippingTypeViewModel(_shippingTypeClient.GetShippingType(shippingTypeId));

        //Get all the shipping types from sql cache.
        public virtual List<SelectListItem> GetAllShippingTypesNotInDatabase()
            => ProviderEngineViewModelMap.ToShippingListItems(_shippingTypeClient.GetAllShippingTypesNotInDatabase().ShippingTypeList);

        //Create new Shipping type.
        public virtual ProviderEngineViewModel CreateShippingType(ProviderEngineViewModel shippingTypeViewModel)
            => _shippingTypeClient.CreateShippingType(ProviderEngineViewModelMap.ToShippingTypeModel(shippingTypeViewModel))?.ToViewModel<ProviderEngineViewModel>();

        //Update shipping type.
        public virtual ProviderEngineViewModel UpdateShippingType(ProviderEngineViewModel shippingTypeViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                return ProviderEngineViewModelMap.ToShippingTypeViewModel(_shippingTypeClient.UpdateShippingType(ProviderEngineViewModelMap.ToShippingTypeModel(shippingTypeViewModel)));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Warning);
                return (ProviderEngineViewModel)GetViewModelWithErrorMessage(new ProviderEngineViewModel(), ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Error);
                return (ProviderEngineViewModel)GetViewModelWithErrorMessage(new ProviderEngineViewModel(), Admin_Resources.UpdateError);
            }
        }

        //Delete shipping type.
        public virtual bool DeleteShippingType(string shippingTypeId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            message = Admin_Resources.DeleteErrorMessage;
            try
            {
                return _shippingTypeClient.DeleteShippingType(new ParameterModel { Ids = shippingTypeId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Warning);
                if (Equals(ex.ErrorCode, ErrorCodes.AssociationDeleteError))
                    message = Admin_Resources.ErrorDeleteShippingType;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Get shipping type on the basis of class name.
        public virtual ProviderEngineViewModel GetShippingTypeByClassName(string name)
            => ProviderEngineViewModelMap.ToShippingListViewModel(_shippingTypeClient.GetAllShippingTypesNotInDatabase()).ProvideEngineTypes.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());

        //Enable/Disable shipping types.
        public virtual bool BulkEnableDisableShippingTypes(string shippingTypeId, bool isEnable, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            message = "";
            try
            {
                return _shippingTypeClient.BulkEnableDisableShippingTypes(new ParameterModel { Ids = shippingTypeId }, isEnable);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Warning);
                message = ex.ErrorMessage;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Error);
                return false;
            }
        }
        #endregion

        #region Promotion Type
        //Get all saved promotion types.
        public virtual ProviderEngineListViewModel GetPromotionTypeList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ProviderEngineListViewModel providerEngineListViewModel = ProviderEngineViewModelMap.ToPromotionListViewModel(_promotionTypeClient.GetPromotionTypeList(filters, sorts, pageIndex, pageSize));
            SetPromotionTypeToolMenu(providerEngineListViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return providerEngineListViewModel;
        }

        //Get promotion type on the basis of Id.
        public virtual ProviderEngineViewModel GetPromotionType(int promotionTypeId)
            => ProviderEngineViewModelMap.ToPromotionTypeViewModel(_promotionTypeClient.GetPromotionType(promotionTypeId));

        //Get all the promotion types from sql cache.
        public virtual List<SelectListItem> GetAllPromotionTypesNotInDatabase()
            => ProviderEngineViewModelMap.ToPromotionListItems(_promotionTypeClient.GetAllPromotionTypesNotInDatabase().PromotionTypes);

        //Create new Promotion type.
        public virtual ProviderEngineViewModel CreatePromotionType(ProviderEngineViewModel promotionTypeViewModel)
            => _promotionTypeClient.CreatePromotionType(ProviderEngineViewModelMap.ToPromotionTypeModel(promotionTypeViewModel))?.ToViewModel<ProviderEngineViewModel>();

        //Update the Promotion type record.
        public virtual ProviderEngineViewModel UpdatePromotionType(ProviderEngineViewModel promotionTypeViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                return ProviderEngineViewModelMap.ToPromotionTypeViewModel(_promotionTypeClient.UpdatePromotionType(ProviderEngineViewModelMap.ToPromotionTypeModel(promotionTypeViewModel)));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Warning);
                if (Equals(ex.ErrorCode, ErrorCodes.AssociationDeleteError))
                    return (ProviderEngineViewModel)GetViewModelWithErrorMessage(promotionTypeViewModel, Admin_Resources.ErrorDisablePromotionType);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Error);
                return (ProviderEngineViewModel)GetViewModelWithErrorMessage(promotionTypeViewModel, Admin_Resources.UpdateErrorMessage);
            }
            return promotionTypeViewModel;
        }

        //Delete Promotion type.
        public virtual bool DeletePromotionType(string promotionTypeId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            message = Admin_Resources.DeleteErrorMessage;
            try
            {
                return _promotionTypeClient.DeletePromotionType(new ParameterModel { Ids = promotionTypeId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Warning);
                if (Equals(ex.ErrorCode, ErrorCodes.AssociationDeleteError))
                    message = Admin_Resources.ErrorDeletePromotionType;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Get promotion type on the basis of class name.
        public virtual ProviderEngineViewModel GetPromotionTypeByClassName(string name)
            => ProviderEngineViewModelMap.ToPromotionListViewModel(_promotionTypeClient.GetAllPromotionTypesNotInDatabase()).ProvideEngineTypes.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());

        //Enable/Disable promotion types.
        public virtual bool BulkEnableDisablePromotionTypes(string promotionTypeId, bool isEnable, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            message = Admin_Resources.UpdateErrorMessage;
            try
            {
                return _promotionTypeClient.BulkEnableDisablePromotionTypes(new ParameterModel() { Ids = promotionTypeId }, isEnable);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Warning);
                if (Equals(ex.ErrorCode, ErrorCodes.AssociationDeleteError))
                    message = Admin_Resources.ErrorDisablePromotionType;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Error);
                return false;
            }
        }
        #endregion

        #region Private Methods
        private void SetTaxRuleTypeListToolMenus(ProviderEngineListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('TaxRuleDeletePopup')", ControllerName = "ProviderEngine", ActionName = "DeleteTaxRuleType" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.Activate, JSFunctionName = "EditableText.prototype.DialogDelete('taxruleEnable')", ControllerName = "ProviderEngine", ActionName = "BulkEnableDisableTaxRuleTypes" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.DeActivate, JSFunctionName = "EditableText.prototype.DialogDelete('taxruledisable')", ControllerName = "ProviderEngine", ActionName = "BulkEnableDisableTaxRuleTypes" });
            }
        }

        private void SetSupplierTypeListToolMenus(ProviderEngineListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('SupplierTypeDeletePopup')", ControllerName = "ProviderEngine", ActionName = "DeleteSupplierType" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.Activate, JSFunctionName = "EditableText.prototype.DialogDelete('supplierTypeEnable')", ControllerName = "ProviderEngine", ActionName = "BulkEnableDisableSupplierTypes" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.DeActivate, JSFunctionName = "EditableText.prototype.DialogDelete('supplierdisable')", ControllerName = "ProviderEngine", ActionName = "BulkEnableDisableSupplierTypes" });
            }
        }

        private void SetShippingTypeListToolMenus(ProviderEngineListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('ShippingTypeDeletePopup')", ControllerName = "ProviderEngine", ActionName = "DeleteShippingType" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.Activate, JSFunctionName = "EditableText.prototype.DialogDelete('shippingTypeEnable')", ControllerName = "ProviderEngine", ActionName = "BulkEnableDisableShippingTypes" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.DeActivate, JSFunctionName = "EditableText.prototype.DialogDelete('shippingdisable')", ControllerName = "ProviderEngine", ActionName = "BulkEnableDisableShippingTypes" });
            }
        }

        private void SetPromotionTypeToolMenu(ProviderEngineListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('PromotionTypeDeletePopup')", ControllerName = "ProviderEngine", ActionName = "DeletePromotionType" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.Activate, JSFunctionName = "EditableText.prototype.DialogDelete('promotionTypeEnable')", ControllerName = "ProviderEngine", ActionName = "BulkEnableDisablePromotionTypes" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.DeActivate, JSFunctionName = "EditableText.prototype.DialogDelete('promotiondisable')", ControllerName = "ProviderEngine", ActionName = "BulkEnableDisablePromotionTypes" });
            }
        }
        #endregion
        #endregion
    }
}