using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Admin.Extensions;
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
    public class UrlRedirectAgent : BaseAgent, IUrlRedirectAgent
    {
        #region Private Variables
        private readonly IUrlRedirectClient _urlRedirectClient;
        private readonly ISEOSettingAgent _seoSettingAgent;
        #endregion

        #region Constructor
        public UrlRedirectAgent(IUrlRedirectClient urlRedirectClient)
        {
            _urlRedirectClient = GetClient<IUrlRedirectClient>(urlRedirectClient);
            _seoSettingAgent = new SEOSettingAgent(GetClient<SEOSettingClient>(), GetClient<PortalClient>(), GetClient<PublishProductClient>(), GetClient<PublishCategoryClient>());
        }
        #endregion

        #region public Methods

        //Get Url Redirect list.
        public virtual UrlRedirectListViewModel GetUrlRedirectList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (portalId < 1)
                portalId = Convert.ToInt32(_seoSettingAgent.GetPortalList().FirstOrDefault().Value);

            filters.RemoveAll(x => x.Item1 == ZnodePortalEnum.PortalId.ToString());
            filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()));
            ZnodeLogging.LogMessage("filters and sorts: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            UrlRedirectListModel urlRedirectListModel = _urlRedirectClient.GetUrlRedirectList(filters, sorts, pageIndex, pageSize);

            UrlRedirectListViewModel urlRedirectListViewModel = new UrlRedirectListViewModel { UrlRedirects = urlRedirectListModel?.UrlRedirects?.ToViewModel<UrlRedirectViewModel>().ToList(), PortalList = _seoSettingAgent.GetPortalList(), PortalId = portalId };
            SetListPagingData(urlRedirectListViewModel, urlRedirectListModel);

            foreach (var portalData in urlRedirectListViewModel.PortalList)
            {
                if (Equals(portalData.Value, portalId.ToString()))
                    urlRedirectListViewModel.StoreName = portalData.Text;
            }
            //Set tool options for grid.
            SetUrlRedirectToolMenus(urlRedirectListViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return urlRedirectListViewModel;
        }

        //Get Url Redirect details on the basis of Url Redirect id.
        public virtual UrlRedirectViewModel GetUrlRedirect(int urlRedirectId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (urlRedirectId > 0)
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeCMSUrlRedirectEnum.CMSUrlRedirectId.ToString(), FilterOperators.Equals, urlRedirectId.ToString()));
                UrlRedirectViewModel model = new UrlRedirectViewModel();
                ZnodeLogging.LogMessage("filters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters });
                model = _urlRedirectClient.GetUrlRedirect(filters)?.ToViewModel<UrlRedirectViewModel>();               
                model.StoreName = _seoSettingAgent.GetPortalList().FirstOrDefault(x => x.Value == model.PortalId.ToString()).Text;
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return model;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return new UrlRedirectViewModel() { HasError = true };
        }

        //Create the Url Redirect.
        public virtual UrlRedirectViewModel CreateUrlRedirect(UrlRedirectViewModel urlRedirectViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                //Create Url Redirect and map the model to view model.
                urlRedirectViewModel = _urlRedirectClient.CreateUrlRedirect(urlRedirectViewModel?.ToModel<UrlRedirectModel>())?.ToViewModel<UrlRedirectViewModel>();
                urlRedirectViewModel.StoreName= _seoSettingAgent.GetPortalList().FirstOrDefault(x => x.Value == urlRedirectViewModel.PortalId.ToString()).Text;
                ZnodeLogging.LogMessage("StoreName of urlRedirectViewModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { StoreName = urlRedirectViewModel?.StoreName });
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);        
                return urlRedirectViewModel;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                return SetErrorMessage(ex, urlRedirectViewModel);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                urlRedirectViewModel.StoreName = _seoSettingAgent.GetPortalList().Where(x => x.Value == urlRedirectViewModel.PortalId.ToString()).First().Text;
                return (UrlRedirectViewModel)GetViewModelWithErrorMessage(urlRedirectViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Update the Url Redirect.
        public virtual UrlRedirectViewModel UpdateUrlRedirect(UrlRedirectViewModel urlRedirectViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                //Update Url Redirect and map the model to view model.
                urlRedirectViewModel = _urlRedirectClient.UpdateUrlRedirect(urlRedirectViewModel?.ToModel<UrlRedirectModel>())?.ToViewModel<UrlRedirectViewModel>();
                urlRedirectViewModel.StoreName= _seoSettingAgent.GetPortalList().Where(x => x.Value == urlRedirectViewModel.PortalId.ToString()).First().Text;
                ZnodeLogging.LogMessage("StoreName of urlRedirectViewModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { StoreName = urlRedirectViewModel?.StoreName });
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return urlRedirectViewModel;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                urlRedirectViewModel.StoreName= _seoSettingAgent.GetPortalList().Where(x => x.Value == urlRedirectViewModel.PortalId.ToString()).First().Text;
                return SetErrorMessage(ex, urlRedirectViewModel);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (UrlRedirectViewModel)GetViewModelWithErrorMessage(urlRedirectViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Delete Url Redirect.
        public virtual bool DeleteUrlRedirect(string urlRedirectId)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                //Delete Url Redirect.
                return _urlRedirectClient.DeleteUrlRedirect(new ParameterModel { Ids = urlRedirectId });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }
        #endregion

        #region Private Methods
        private UrlRedirectViewModel SetErrorMessage(ZnodeException exception, UrlRedirectViewModel urlRedirectViewModel)
        {
            switch (exception.ErrorCode)
            {
                case ErrorCodes.InvalidData:
                    return (UrlRedirectViewModel)GetViewModelWithErrorMessage(urlRedirectViewModel, Admin_Resources.ErrorValidURL);
                case ErrorCodes.NotPermitted:
                    return (UrlRedirectViewModel)GetViewModelWithErrorMessage(urlRedirectViewModel, Admin_Resources.ErrorURLRedirectLoop);
                default:
                    return (UrlRedirectViewModel)GetViewModelWithErrorMessage(urlRedirectViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Set the Tool Menus forUrl Redirect List Grid View.
        private void SetUrlRedirectToolMenus(UrlRedirectListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new Models.GridModel();
                model.GridModel.FilterColumn = new Models.FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<Models.ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('UrlRedirectDeletePopup')", ControllerName = "SEO", ActionName = "DeleteUrlRedirect" });
            }
        }
        #endregion
    }
}