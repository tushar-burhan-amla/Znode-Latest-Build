using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class ManageMessageAgent : BaseAgent, IManageMessageAgent
    {
        #region Private Variables
        private readonly IManageMessageClient _manageMessageClient;
        private readonly IThemeClient _themeClient;
        private readonly IPortalClient _portalClient;
        private readonly ILocaleAgent _localeAgent;
        private readonly ILocaleClient _localeClient;
        #endregion

        #region Constructor
        public ManageMessageAgent(IManageMessageClient manageMessageClient, IThemeClient themeClient, IPortalClient portalClient, ILocaleClient localeClient)
        {
            _manageMessageClient = GetClient<IManageMessageClient>(manageMessageClient);
            _themeClient = GetClient<IThemeClient>(themeClient);
            _portalClient = GetClient<IPortalClient>(portalClient);
            _localeClient = GetClient<ILocaleClient>(localeClient);
            _localeAgent = new LocaleAgent(GetClient<LocaleClient>());
        }
        #endregion

        //Get ManageMessage list. 
        public virtual ManageMessageListViewModel GetManageMessages(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            int localeId = GetLocaleValue();

            if (filters.Exists(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString()))
            {
                filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString());
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString()));
            }
            else
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId.ToString()));
            ZnodeLogging.LogMessage("Input parameters of method GetManageMessages: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sorts });

            ManageMessageListModel manageMessageList = _manageMessageClient.GetManageMessages(expands, filters, sorts, pageIndex, pageSize);

            ManageMessageListViewModel listViewModel = new ManageMessageListViewModel { ManageMessages = manageMessageList?.ManageMessages?.ToViewModel<ManageMessageViewModel>().ToList() };
            //Set paging parameters.
            SetListPagingData(listViewModel, manageMessageList);

            listViewModel.Locale = GetLocalList();

            //Set tool options for this grid.
            SetManageMessageToolMenus(listViewModel);

            //Set locale value as per selected from locale dropdown
            SetLocaleValueForDropdown(listViewModel.Locale, localeId.ToString());
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return manageMessageList?.ManageMessages?.Count > 0 ? listViewModel : new ManageMessageListViewModel() { ManageMessages = new List<ManageMessageViewModel>(), Locale = listViewModel.Locale };
        }

        //Get ManageMessage details.
        public virtual ManageMessageViewModel GetManageMessage(int cmsMessageKeyId, int portalId, int localeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { cmsMessageKeyId = cmsMessageKeyId, portalId = portalId, localeId = localeId });
            ManageMessageMapperModel manageMessageMapperModel = new ManageMessageMapperModel();
            manageMessageMapperModel.CMSMessageKeyId = cmsMessageKeyId;
            manageMessageMapperModel.PortalId = portalId;
            manageMessageMapperModel.LocaleId = localeId;

            ManageMessageViewModel manageMessageViewModel = cmsMessageKeyId > 0 ? _manageMessageClient.GetManageMessage(manageMessageMapperModel).ToViewModel<ManageMessageViewModel>() : new ManageMessageViewModel();

            //Get the list of area and portal.
            GetManageMessageViewModel(manageMessageViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return manageMessageViewModel;
        }

        //Get Data for manage message view
        public virtual void GetManageMessageViewModel(ManageMessageViewModel viewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Getting portal list.
            viewModel.Portals = ThemeViewModelMap.ToPortalList(_portalClient.GetPortalList(null, null, null, null, null)?.PortalList);

            //If the locale id is 0 set it to default locale.
            if (viewModel.LocaleId == 0)
                viewModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
            ZnodeLogging.LogMessage("LocaleId of ManageMessageViewModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { LocaleId = viewModel?.LocaleId });

            //Binds the locale.
            viewModel.Locales = _localeAgent.GetLocalesList(viewModel.LocaleId);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
        }

        //Create ManageMessage.
        public virtual ManageMessageViewModel CreateManageMessage(ManageMessageViewModel manageMessageViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                ManageMessageModel ManageMessageModel = _manageMessageClient.CreateManageMessage(manageMessageViewModel?.ToModel<ManageMessageModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return IsNotNull(ManageMessageModel) ? ManageMessageModel.ToViewModel<ManageMessageViewModel>() : new ManageMessageViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    //Catch exception when ManageMessage already exists or for an error.
                    case ErrorCodes.AlreadyExist:
                        return (ManageMessageViewModel)GetViewModelWithErrorMessage(new ManageMessageViewModel(), Admin_Resources.MessageKeyAlreadyExist);
                    case ErrorCodes.NullModel:
                        return (ManageMessageViewModel)GetViewModelWithErrorMessage(new ManageMessageViewModel(), Admin_Resources.ErrorModelNull);
                    default:
                        return (ManageMessageViewModel)GetViewModelWithErrorMessage(new ManageMessageViewModel(), Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (ManageMessageViewModel)GetViewModelWithErrorMessage(new ManageMessageViewModel(), Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Update ManageMessage.
        public virtual ManageMessageViewModel UpdateManageMessage(ManageMessageViewModel manageMessageViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return _manageMessageClient.UpdateManageMessage(manageMessageViewModel?.ToModel<ManageMessageModel>())?.ToViewModel<ManageMessageViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (ManageMessageViewModel)GetViewModelWithErrorMessage(new ManageMessageViewModel(), Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete ManageMessage.
        public virtual bool DeleteManageMessage(string cmsManageMessageId)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                //Delete Manage Message.
                return _manageMessageClient.DeleteManageMessage(new ParameterModel { Ids = cmsManageMessageId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        [Obsolete("To be discontinued in one of the upcoming versions.")]
        //Publish the message
        public bool PublishManageMessage(string cmsMessageKeyId, int portalId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            try
            {
                PublishedModel result = _manageMessageClient.PublishManageMessage(new ContentPageParameterModel { Ids = cmsMessageKeyId, portalId = portalId });
                errorMessage = result?.ErrorMessage;                 
                return Convert.ToBoolean(result?.IsPublished);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.InvalidData:
                        errorMessage = ex.Message;
                        return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Publish the message
        public bool PublishManageMessage(string cmsMessageId, int portalId, int localeId, out string errorMessage, string targetPublishState = null, bool takeFromDraftFirst = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            try
            {
                PublishedModel result = _manageMessageClient.PublishManageMessage(new ContentPageParameterModel { Ids = cmsMessageId, portalId = portalId, localeId = localeId, TargetPublishState = targetPublishState, TakeFromDraftFirst = takeFromDraftFirst });
                if (IsNull(result?.IsPublished) || result?.IsPublished == false)
                    errorMessage = string.IsNullOrEmpty(result?.ErrorMessage) ? Admin_Resources.ErrorPublished : result?.ErrorMessage;
                ZnodeLogging.LogMessage("If the Cloudflare is enabled then purge the store URL manually otherwise changes will not reflect on store.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return Convert.ToBoolean(result?.IsPublished);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.InvalidData:
                        errorMessage = ex.Message;
                        return false;
                    case ErrorCodes.StoreNotPublishedForAssociatedEntity:
                        errorMessage = ex.Message;
                        return false;
                    default:
                        errorMessage = Admin_Resources.ErrorPublished;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Get LocaleId
        public virtual int GetLocaleValue()
        {
            int localeId = 0;
            if (CookieHelper.IsCookieExists("_contentCulture"))
            {
                string cookieValue = CookieHelper.GetCookieValue<string>("_contentCulture");
                localeId = string.IsNullOrEmpty(cookieValue) ? Convert.ToInt32(DefaultSettingHelper.DefaultLocale) : Convert.ToInt32(cookieValue);
            }
            else
            {
                localeId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                CookieHelper.SetCookie("_contentCulture", Convert.ToString(localeId));
            }

            return localeId;
        }

        //Set the Tool Menus for Theme List Grid View.
        private void SetManageMessageToolMenus(ManageMessageListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new Models.GridModel();
                model.GridModel.FilterColumn = new Models.FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<Models.ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('CMSManageMessagePopup')", ControllerName = "Content", ActionName = "DeleteManageMessage" });
            }
        }
        public virtual List<SelectListItem> GetLocalList()
        {
            LocaleListModel locales = _localeClient.GetLocaleList(null, new FilterCollection { new FilterTuple(ZnodeLocaleEnum.IsActive.ToString(), FilterOperators.Equals, "true") }, null);

            return (from item in locales.Locales
                    select new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.LocaleId.ToString(),
                        Selected = item.IsDefault
                    }).ToList();
        }
    }
}
