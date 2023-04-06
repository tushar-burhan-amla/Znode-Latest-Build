using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using Znode.Engine.Admin.Models;
using System.Web.Mvc;
using System;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.Admin.Agents
{
    public class EmailTemplateAgent : BaseAgent, IEmailTemplateAgent
    {
        #region Private Variables
        private readonly IEmailTemplateClient _emailTemplateClient;
        private readonly ILocaleAgent _localeAgent;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for Email Template agent.
        /// </summary>
        public EmailTemplateAgent(IEmailTemplateClient emailTemplateClient, IPortalClient portalClient)
        {
            _emailTemplateClient = GetClient<IEmailTemplateClient>(emailTemplateClient);
            _localeAgent = new LocaleAgent(GetClient<LocaleClient>());
        }
        #endregion

        #region public virtual Methods
        //Get list of all email templates.      
        public virtual EmailTemplateListViewModel EmailTemplates(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            EmailTemplateListModel emailTemplateList = _emailTemplateClient.GetTemplates(null, filters, sorts, pageIndex, pageSize);
            EmailTemplateListViewModel listViewModel = new EmailTemplateListViewModel { EmailTemplateList = emailTemplateList?.EmailTemplatesList?.ToViewModel<EmailTemplateViewModel>().ToList() };
            SetListPagingData(listViewModel, emailTemplateList);

            //Set tool option menus for email template grid.
            SetEmailListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return emailTemplateList?.EmailTemplatesList?.Count > 0 ? listViewModel : new EmailTemplateListViewModel() { EmailTemplateList = new List<EmailTemplateViewModel>() };
        }

        //Gets an email template by emailTemplateId.
        public virtual EmailTemplateViewModel GetEmailTemplate(int emailTemplateId, int localeId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (emailTemplateId > 0)
            {
                //Set Filter for Locale Id.
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, localeId == 0 ? DefaultSettingHelper.DefaultLocale : localeId.ToString()));
                ZnodeLogging.LogMessage("filters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters});
                //Get Email Template by emailtemplateId.
                EmailTemplateViewModel emailTemplateViewModel = _emailTemplateClient.GetTemplatePage(emailTemplateId, filters).ToViewModel<EmailTemplateViewModel>();

                if (HelperUtility.IsNotNull(emailTemplateViewModel))
                {
                    emailTemplateViewModel.Locale = _localeAgent.GetLocalesList(emailTemplateViewModel.LocaleId);
                    ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                    return emailTemplateViewModel;
                }
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return new EmailTemplateViewModel { HasError = true };
        }

        //Create email template.
        public virtual EmailTemplateViewModel CreateEmailTemplate(EmailTemplateViewModel emailTemplateViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return _emailTemplateClient.CreateTemplatePage(emailTemplateViewModel?.ToModel<EmailTemplateModel>())?.ToViewModel<EmailTemplateViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (EmailTemplateViewModel)GetViewModelWithErrorMessage(new EmailTemplateViewModel(), Admin_Resources.TemplateNameAlreadyExist);
                    default:
                        return (EmailTemplateViewModel)GetViewModelWithErrorMessage(new EmailTemplateViewModel(), Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (EmailTemplateViewModel)GetViewModelWithErrorMessage(new EmailTemplateViewModel(), Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Update email template.
        public virtual EmailTemplateViewModel UpdateEmailTemplate(EmailTemplateViewModel emailTemplateViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return _emailTemplateClient.UpdateTemplatePage(emailTemplateViewModel?.ToModel<EmailTemplateModel>())?.ToViewModel<EmailTemplateViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (EmailTemplateViewModel)GetViewModelWithErrorMessage(emailTemplateViewModel, Admin_Resources.TemplateNameAlreadyExist);
                    default:
                        return (EmailTemplateViewModel)GetViewModelWithErrorMessage(emailTemplateViewModel, Admin_Resources.UpdateErrorMessage);
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (EmailTemplateViewModel)GetViewModelWithErrorMessage(emailTemplateViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete email template.
        public virtual bool DeleteEmailTemplate(string emailTemplateId)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return _emailTemplateClient.DeleteTemplatePage(new ParameterModel { Ids = emailTemplateId });
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Gets an email template tokens.
        public virtual string GetEmailTemplateTokens()
        {
            EmailTemplateViewModel emailTemplateViewModel = _emailTemplateClient.GetEmailTemplateTokens().ToViewModel<EmailTemplateViewModel>();
            return emailTemplateViewModel.EmailTemplateTokens;
        }

        //Get All Mapped Email Template Area Details.
        public virtual EmailTemplateAreaDataViewModel GetMappedEmailTemplateArea(int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            EmailTemplateAreaDataViewModel model = new EmailTemplateAreaDataViewModel();

            IStoreAgent storeAgent= new StoreAgent(GetClient<PortalClient>(), GetClient<EcommerceCatalogClient>(), GetClient<ThemeClient>(), GetClient<DomainClient>(), GetClient<PriceClient>(), GetClient<OrderStateClient>(),
                GetClient<ProductReviewStateClient>(), GetClient<PortalProfileClient>(), GetClient<WarehouseClient>(), GetClient<CSSClient>(), GetClient<ManageMessageClient>(), GetClient<ContentPageClient>(), GetClient<TaxClassClient>(),
                GetClient<PaymentClient>(), GetClient<ShippingClient>(), GetClient<PortalCountryClient>(),GetClient<TagManagerClient>(), GetClient<GeneralSettingClient>());

            model.Portals = storeAgent.GetPortalSelectList(portalId);
            model.PortalId = portalId > 0 ? portalId : Convert.ToInt32(model.Portals.FirstOrDefault().Value);
            //Get All the Mapped Email Template Areas.
            EmailTemplateAreaMapperListModel mappedTemplateDetails = _emailTemplateClient.GetEmailTemplateAreaMapperList(model.PortalId);
            model.MappedEmailTemplateList = mappedTemplateDetails?.EmailTemplatesAreaMapperList?.ToViewModel<EmailTemplateAreaMapperViewModel>().ToList();
            ZnodeLogging.LogMessage("MappedEmailTemplateList count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { MappedEmailTemplateListCount = model?.MappedEmailTemplateList?.Count });
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return model;
        }

        //Delete the Mapped Configuration for Email Template Area.
        public virtual bool DeleteEmailTemplateAreaMapping(string areaMappingId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (!string.IsNullOrEmpty(areaMappingId))
            {
                try
                {
                    return _emailTemplateClient.DeleteEmailTemplateAreaMapping(new ParameterModel { Ids = areaMappingId });
                }
                catch(Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                    return false;
                }
            }
            return false;
        }

        //Saves the Email Template Area Configuration Details.
        public virtual bool SaveEmailTemplateAreaConfiguration(EmailTemplateAreaMapperViewModel model)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return _emailTemplateClient.SaveEmailTemplateAreaConfiguration(model.ToModel<EmailTemplateAreaMapperModel>());
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Get Available Email Template Area.
        public virtual EmailTemplateAreaMapperViewModel GetAvailableTemplateArea(int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            EmailTemplateAreaMapperViewModel model = new EmailTemplateAreaMapperViewModel();
            model.IsAddMode = true;
            //Get All the Email Template Areas.
            EmailTemplateAreaListModel templateAreas = _emailTemplateClient.GetEmailTemplateAreaList(null, null, null, null);

            //Get All the Mapped Template Areas.
            EmailTemplateAreaMapperListModel mappedTemplateDetails = _emailTemplateClient.GetEmailTemplateAreaMapperList(portalId);

            if (HelperUtility.IsNotNull(templateAreas?.EmailTemplatesAreaList) && templateAreas?.EmailTemplatesAreaList.Count > 0)
            {
                var lstMappedAreaIds = mappedTemplateDetails?.EmailTemplatesAreaMapperList.Select(x => x.EmailTemplateAreasId)?.Distinct().ToList();

                var availableEmailArea = (from item in templateAreas.EmailTemplatesAreaList
                                          where !lstMappedAreaIds.Contains(item.EmailTemplateAreasId)
                                          select new SelectListItem
                                          {
                                              Text = item.Name,
                                              Value = item.EmailTemplateAreasId.ToString()
                                          }).ToList();
                if (availableEmailArea.Count > 0)
                    model.EmailTemplateAreaList = availableEmailArea;
                ZnodeLogging.LogMessage("EmailTemplateAreaList count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { EmailTemplateAreaListCount = model?.EmailTemplateAreaList?.Count });
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return model;
        }

        public virtual List<EmailTemplateViewModel> GetEmailTemplateListByName(string searchTerm)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Set Filter for Email Template Name.
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeEmailTemplateEnum.TemplateName.ToString(), FilterOperators.Like, searchTerm));
            ZnodeLogging.LogMessage("filters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters });
            //Email Template list on the basis of search Term.
            EmailTemplateListModel list = _emailTemplateClient.GetTemplates(null, filters, null, null, null);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return list?.EmailTemplatesList?.Count > 0 ? list.EmailTemplatesList.ToViewModel<EmailTemplateViewModel>().ToList() : new List<EmailTemplateViewModel>();

        }

        //Get Email Template Token.
        public void GetEmailTemplateToken(EmailTemplateViewModel emailTemplateViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            emailTemplateViewModel.EmailTemplateTokens = GetEmailTemplateTokens();
            if (!string.IsNullOrEmpty(emailTemplateViewModel?.EmailTemplateTokens) && emailTemplateViewModel.EmailTemplateTokens.Contains("~"))
            {
                emailTemplateViewModel.EmailTemplateTokensPartOne = emailTemplateViewModel.EmailTemplateTokens.Split('~')[0];
                emailTemplateViewModel.EmailTemplateTokensPartTwo = emailTemplateViewModel.EmailTemplateTokens.Split('~')[1];
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
        }
        #endregion

        #region Private Methods
        //Set tool option menus for email template grid.
        private void SetEmailListToolMenu(EmailTemplateListViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('EmailTemplateDeletePopup')", ControllerName = "EmailTemplate", ActionName = "Delete" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.LinkConfigureEmailNotification, Url = "/EmailTemplate/ManageEmailTemplateArea", ControllerName = "EmailTemplate", ActionName = "ManageEmailTemplateArea" });
            }
        }
        #endregion

    }
}