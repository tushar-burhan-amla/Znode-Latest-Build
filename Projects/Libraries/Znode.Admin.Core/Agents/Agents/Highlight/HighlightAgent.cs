using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
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
    public class HighlightAgent : BaseAgent, IHighlightAgent
    {
        #region Private Variables

        private readonly IHighlightClient _highlightClient;
        private readonly ILocaleAgent _localeAgent;
        private readonly IProductsClient _productClient;

        #endregion Private Variables

        #region Constructor

        /// <summary>
        /// Constructor for Highlight agent.
        /// </summary>
        public HighlightAgent(IHighlightClient highlightClient, IProductsClient productClient)
        {
            _highlightClient = GetClient<IHighlightClient>(highlightClient);
            _localeAgent = new LocaleAgent(GetClient<LocaleClient>());
            _productClient = GetClient<IProductsClient>(productClient);
        }

        #endregion Constructor

        #region Public Methods

        //Get list of all Highlights.
        public virtual HighlightListViewModel Highlights(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null, int localeId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters filters and sortCollection.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] {  filters, sorts });
            localeId = localeId < 1 ? GetLocaleValue() : localeId;

            // Based on this filter Znode_GetHighlightDetail SP will return highlight image instead of swatch image for all applicable records.
            filters.Add(new FilterTuple(FilterKeys.IsFromAdmin, FilterOperators.Equals, FilterKeys.ActiveTrue));
            //Checking For LocaleId already Exists in Filters Or Not
            if (filters.Exists(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString()))
            {
                filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString());
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString()));
            }
            else
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString()));
                        
            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(ZnodeHighlightEnum.DisplayOrder.ToString(), DynamicGridConstants.ASCKey);
            }
            HighlightListModel highlightList = _highlightClient.GetHighlight(null, filters, sorts, pageIndex, pageSize);
            // Removing these filters as these are not required in cookie.
            filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.IsFromAdmin.ToString(), StringComparison.InvariantCultureIgnoreCase));
            HighlightListViewModel listViewModel = new HighlightListViewModel { HighlightList = highlightList?.HighlightList?.ToViewModel<HighlightViewModel>().ToList() };
            SetListPagingData(listViewModel, highlightList);
            listViewModel.HighlightList?.ForEach(x => x.LocaleId = localeId);
            //Bind Locale List
            listViewModel.Locale = _localeAgent.GetLocalesList();

            //Set locale value as per selected from locale dropdown
            SetLocaleValueForDropdown(listViewModel.Locale, localeId.ToString());

            //Set tool option menus for highlight grid.
            SetHighlightListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return highlightList?.HighlightList?.Count > 0 ? listViewModel : new HighlightListViewModel() { HighlightList = new List<HighlightViewModel>(), Locale = _localeAgent.GetLocalesList() };
        }

        //Gets highlight by highlightId and localeId.
        public virtual HighlightViewModel GetHighlight(int highlightId, int localeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters localeId.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { localeId });

            if (highlightId > 0)
            {
                //Set Filter for Locale Id.
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, (localeId > 0) ? localeId.ToString() : GetLocaleValue().ToString()));
                // Based on this filter Znode_GetHighlightDetail SP will return highlight image instead of swatch image for all applicable records.
                filters.Add(new FilterTuple(FilterKeys.IsFromAdmin, FilterOperators.Equals, FilterKeys.ActiveTrue));

                //Get Highlight by highlightId.
                HighlightViewModel highlightViewModel = _highlightClient.GetHighlight(highlightId, filters).ToViewModel<HighlightViewModel>();

                if (IsNotNull(highlightViewModel))
                {
                    GetHighlightTypeList(highlightViewModel);
                    highlightViewModel.LocaleId = localeId > 0 ? localeId : GetLocaleValue();
                    highlightViewModel.Locale = _localeAgent.GetLocalesList(GetLocaleValue());
                    highlightViewModel.HighlightCode = highlightViewModel.HighlightCode;
                   
                    highlightViewModel.HighlightCodeList = GetHighlightCodeList();
                    highlightViewModel.IsHyperlink = highlightViewModel.DisplayPopup ? true : false;
                    highlightViewModel.Hyperlink = highlightViewModel.DisplayPopup ? highlightViewModel.Hyperlink : string.Empty;
                    ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
                    return highlightViewModel;
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            return new HighlightViewModel { HasError = true };
        }

        //Create highlight.
        public virtual HighlightViewModel CreateHighlight(HighlightViewModel highlightViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
                return _highlightClient.CreateHighlight(highlightViewModel?.ToModel<HighlightModel>())?.ToViewModel<HighlightViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex,ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (HighlightViewModel)GetViewModelWithErrorMessage(new HighlightViewModel(), Admin_Resources.HighlightNameAlreadyExist);

                    default:
                        return (HighlightViewModel)GetViewModelWithErrorMessage(new HighlightViewModel(), Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex,ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
                return (HighlightViewModel)GetViewModelWithErrorMessage(new HighlightViewModel(), Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Update highlight.
        public virtual HighlightViewModel UpdateHighlight(HighlightViewModel highlightViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
                return _highlightClient.UpdateHighlight(highlightViewModel?.ToModel<HighlightModel>())?.ToViewModel<HighlightViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex,ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (HighlightViewModel)GetViewModelWithErrorMessage(highlightViewModel, Admin_Resources.HighlightNameAlreadyExist);

                    default:
                        return (HighlightViewModel)GetViewModelWithErrorMessage(highlightViewModel, Admin_Resources.UpdateErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex,ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
                return (HighlightViewModel)GetViewModelWithErrorMessage(highlightViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete highlight.
        public virtual bool DeleteHighlight(string highlightId)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
                return _highlightClient.DeleteHighlight(new ParameterModel { Ids = highlightId });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex,ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Bind dropdowns.
        public virtual void BindDropdownValues(HighlightViewModel viewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            GetHighlightTypeList(viewModel);
            viewModel.Locale = _localeAgent.GetLocalesList();//Set Filter for Locale Id.
            viewModel.HighlightCodeList = GetHighlightCodeList();
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

        }

        //Bind default values of model.
        public virtual void BindDefaultValues(HighlightViewModel viewModel)
        {
            viewModel.IsActive = true;
            viewModel.IsHyperlink = true;
            viewModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
        }

        //Get Highlight type list and attributes token.
        public virtual void GetHighlightTypeList(HighlightViewModel viewModel)
        {
            HighlightTypeListModel highlightTypeList = _highlightClient.GetHighlightTypeList();
            List<SelectListItem> selectedHighlightTypeList = new List<SelectListItem>();
            if (highlightTypeList?.HighlightTypes?.Count > 0)
            {
                highlightTypeList.HighlightTypes.ToList().ForEach(item => { selectedHighlightTypeList.Add(new SelectListItem() { Text = item.Name, Value = item.HighlightTypeId.ToString() }); });
            }
            ZnodeLogging.LogMessage("selectedHighlightTypeList count.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { selectedHighlightTypeList?.Count });
            GetTemplateTokens(viewModel, highlightTypeList);
            viewModel.HighlightTypeList = selectedHighlightTypeList;
        }

        //Get highlightProduct list.
        public virtual HighlightListViewModel GetHighlightProductList(FilterCollectionDataModel model, int localeId, int highlightId, string highlightCode)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters localeId, highlightId, highlightCode.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { localeId, highlightId, highlightCode });

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeHighlightProduct.ToString(), model);
            //Checking For highlight already Exists in Filters Or Not
            SetFilterforHighlight(model, highlightCode, localeId);

            HighlightListViewModel highlightProducts = new HighlightListViewModel();
            ProductDetailsListModel ProductList = _productClient.GetProducts(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            ProductList.ProductDetailList.ForEach(item => item.AttributeValue = highlightCode);
            ProductList?.AttributeColumnName?.Remove(AdminConstants.Highlights);
            ProductList?.AttributeColumnName?.Remove(AdminConstants.ProductImage);
            ProductList?.AttributeColumnName?.Remove(AdminConstants.Assortment);
            highlightProducts.GridModel = FilterHelpers.GetDynamicGridModel(model, ProductList?.XmlDataList ?? new List<dynamic>(), GridListType.ZnodeHighlightProduct.ToString(), string.Empty, null, true, true, null, CustomAttributeColumn(ProductList.AttributeColumnName));
            SetAssociatedHighlightProductListToolMenu(highlightProducts);
            SetListPagingData(highlightProducts, ProductList);
            highlightProducts.HighlightCode = highlightCode;
            highlightProducts.HighlightId = highlightId;
            highlightProducts.HighlightName = GetHighlight(highlightId, GetLocaleValue()).HighlightName;
            highlightProducts.LocaleId = localeId;
            highlightProducts.GridModel.TotalRecordCount = highlightProducts.TotalResults;

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return highlightProducts;
        }

        //Get unassociated product list .
        public virtual HighlightListViewModel GetUnAssociatedProductList(FilterCollectionDataModel model, int localeId, string highlightCode)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters localeId, highlightCode.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { localeId, highlightCode });

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.AssociatedProductList.ToString(), model);
            SetFilterforNotEqualHighlight(model, highlightCode, localeId);

            HighlightListViewModel highlightProducts = new HighlightListViewModel();
            ProductDetailsListModel ProductList = _productClient.GetProducts(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            ProductList?.AttributeColumnName?.Remove(AdminConstants.ProductImage);
            ProductList?.AttributeColumnName?.Remove(AdminConstants.Highlights);
            ProductList?.AttributeColumnName?.Remove(AdminConstants.Assortment);
            highlightProducts.GridModel = FilterHelpers.GetDynamicGridModel(model, ProductList?.XmlDataList ?? new List<dynamic>(), GridListType.AssociatedProductList.ToString(), string.Empty, null, true, true, null, CustomAttributeColumn(ProductList.AttributeColumnName));
            SetListPagingData(highlightProducts, ProductList);
            highlightProducts.HighlightCode = highlightCode;
            highlightProducts.LocaleId = localeId;
            highlightProducts.GridModel.TotalRecordCount = highlightProducts.TotalResults;
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);

            return highlightProducts;
        }

        //Associate Highlight Product.
        public virtual bool AssociateHighlightProducts(string highlightCode, string productIds)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
                return !string.IsNullOrEmpty(productIds) ?
                   _highlightClient.AssociateAndUnAssociateProduct(GetHighlightProductModel(AdminConstants.Highlights, highlightCode, productIds, false)) : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex,ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //UnAssociate Highlight to product.
        public virtual bool UnAssociateHighlightProduct(string highlightCode, string productIds)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Info);
                return !string.IsNullOrEmpty(productIds) ?
                    _highlightClient.AssociateAndUnAssociateProduct(GetHighlightProductModel(AdminConstants.Highlights, highlightCode, productIds, true)) : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex,ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Get highlight code list.
        public virtual List<SelectListItem> GetHighlightCodeList()
        {
            HighlightListModel highlightCodeList = _highlightClient.GetHighlightCodeList(AdminConstants.Highlights);
            List<SelectListItem> selectedHighlightCodeList = new List<SelectListItem>();
            if (highlightCodeList?.HighlightCodes?.Count > 0)
                highlightCodeList.HighlightCodes.ToList().ForEach(item =>
                {
                    string defaultValueLocale = item.ValueLocales.Where(x => x.LocaleId?.ToString() == DefaultSettingHelper.DefaultLocale).FirstOrDefault()?.DefaultAttributeValue;
                    selectedHighlightCodeList.Add(new SelectListItem() { Text = string.IsNullOrEmpty(defaultValueLocale) ? item.AttributeDefaultValueCode : defaultValueLocale, Value = item.AttributeDefaultValueCode });
                });
            ZnodeLogging.LogMessage("selectedHighlightCodeList count.", ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Verbose, new object[] { selectedHighlightCodeList?.Count });

            return selectedHighlightCodeList;
        }

        //Get LocaleId
        public virtual int GetLocaleValue()
        {
            int localeId = 0;
            if (CookieHelper.IsCookieExists("_highLightCulture"))
            {
                string cookieValue = CookieHelper.GetCookieValue<string>("_highLightCulture");
                localeId = string.IsNullOrEmpty(cookieValue) ? Convert.ToInt32(DefaultSettingHelper.DefaultLocale) : Convert.ToInt32(cookieValue);
            }
            else
            {
                localeId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                CookieHelper.SetCookie("_highLightCulture", Convert.ToString(localeId));
            }

            return localeId;
        }

        #endregion Public Methods

        #region Private Methods.

        //Set tool option menus for highlight grid.
        private void SetHighlightListToolMenu(HighlightListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('HighlightDeletePopup')", ControllerName = "Highlight", ActionName = "Delete" });
            }
        }

        //Set filter for equal clause.
        private void SetFilterforHighlight(FilterCollectionDataModel model, string highlightCode, int localeId)
        {
            if (model.Filters.Exists(x => x.Item1 == AdminConstants.Highlights))
            {
                model.Filters.RemoveAll(x => x.Item1 == AdminConstants.Highlights);
                model.Filters.Insert(0, new FilterTuple(AdminConstants.Highlights, FilterOperators.Like, highlightCode));
            }
            else
                model.Filters.Insert(0, new FilterTuple(AdminConstants.Highlights, FilterOperators.Like, highlightCode));

            if (model.Filters.Exists(x => x.Item1 == FilterKeys.LocaleId))
            {
                model.Filters.RemoveAll(x => x.Item1 == FilterKeys.LocaleId);
                model.Filters.Insert(0, new FilterTuple(FilterKeys.LocaleId, FilterOperators.Equals, localeId.ToString()));
            }
            else
                model.Filters.Insert(0, new FilterTuple(FilterKeys.LocaleId, FilterOperators.Equals, localeId.ToString()));

            if (!model.Filters.Exists(x => x.Item1 == AdminConstants.IsCallForAttribute))
                model.Filters.Add(new FilterTuple(AdminConstants.IsCallForAttribute, FilterOperators.Equals, "True"));
        }

        //Set filter for not equal clause.
        private void SetFilterforNotEqualHighlight(FilterCollectionDataModel model, string highlightCode, int localeId)
        {
            //If null set it to new.
            if (IsNull(model.Filters))
                model.Filters = new FilterCollection();

            //Remove HighlightCode and IsProductNotIn filter.
            model.Filters.RemoveAll(x => x.Item1.Equals(AdminConstants.Highlights, StringComparison.InvariantCultureIgnoreCase));
            model.Filters.RemoveAll(x => x.Item1.Equals(FilterKeys.IsProductNotIn, StringComparison.InvariantCultureIgnoreCase));
            model.Filters.RemoveAll(x => x.Item1.Equals(AdminConstants.IsCallForAttribute, StringComparison.InvariantCultureIgnoreCase));
            model.Filters.RemoveAll(x => x.Item1.Equals(FilterKeys.LocaleId, StringComparison.InvariantCultureIgnoreCase));

            //Add HighlightCode and IsProductNotIn with new values.
            model.Filters.Add(new FilterTuple(AdminConstants.IsCallForAttribute, FilterOperators.Equals, "True"));
            model.Filters.Add(new FilterTuple(FilterKeys.IsProductNotIn, FilterOperators.Equals, "True"));
            model.Filters.Add(new FilterTuple(FilterKeys.LocaleId, FilterOperators.Equals, localeId.ToString()));
            model.Filters.Add(new FilterTuple(AdminConstants.Highlights, FilterOperators.Is, highlightCode));
            model.Filters.Reverse();
        }

        //Set tool menu for associated highlight product list.
        private void SetAssociatedHighlightProductListToolMenu(HighlightListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('HighlightProductDeletePopup')", ControllerName = "Highlight", ActionName = "UnAssociateHighlightProducts" });
            }
        }

        //Get highlight product model.
        private HighlightProductModel GetHighlightProductModel(string attributeCode, string attributeValue, string productIds, bool isUnAssociated = true)
        {
            return new HighlightProductModel()
            {
                AttributeCode = attributeCode,
                AttributeValue = attributeValue,
                ProductIds = productIds,
                IsUnAssociated = isUnAssociated
            };
        }

        //Get attributes tokens.
        private void GetTemplateTokens(HighlightViewModel viewModel, HighlightTypeListModel highlightTypeList)
        {
            viewModel.TemplateTokens = highlightTypeList.TemplateTokens;
            string[] tokens = viewModel.TemplateTokens.Split('~');
            if(IsNotNull(tokens))
            {
                viewModel.TemplateTokensPartOne = tokens[0].Remove(tokens[0].LastIndexOf(','), 1);
                viewModel.TemplateTokensPartTwo = tokens.Length > 1 ? tokens[1].Remove(tokens[1].LastIndexOf(','), 1) : string.Empty;
            }
        }
        #endregion Private Methods.
    }
}