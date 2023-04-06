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

namespace Znode.Engine.Admin.Agents
{
    public class ContentContainerAgent : BaseAgent, IContentContainerAgent
    {

        #region Private Variables
        protected readonly IContentContainerClient _contentContainerClient;
        protected readonly IPortalClient _portalClient;
        protected readonly IProfileClient _profileClient;
        protected readonly IGlobalAttributeFamilyClient _globalAttributesFamilyClient;
        protected readonly IContainerTemplateClient _containerTemplateClient;

        #endregion

        #region Constructor
        public ContentContainerAgent(IContentContainerClient contentContainerClient, IPortalClient portalClient, IGlobalAttributeFamilyClient globalAttributeFamilyClient, IProfileClient profileClient, IContainerTemplateClient containerTemplate)
        {
            _contentContainerClient = GetClient<IContentContainerClient>(contentContainerClient);
            _portalClient = GetClient<IPortalClient>(portalClient);
            _globalAttributesFamilyClient = GetClient<IGlobalAttributeFamilyClient>(globalAttributeFamilyClient);
            _profileClient = GetClient<IProfileClient>(profileClient);
            _containerTemplateClient = GetClient<IContainerTemplateClient>(containerTemplate);
        }
        #endregion


        #region Public Methods

        //Get the List of Content Containers
        public virtual ContentContainerListViewModel List(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ContentContainerListModel listModel = _contentContainerClient.List(null, filters, sortCollection, pageIndex, recordPerPage);
            ContentContainerListViewModel viewModel = new ContentContainerListViewModel { ContentContainers = listModel?.ContainerList?.ToViewModel<ContentContainerViewModel>().ToList() };

            //Set Paging Data 
            SetListPagingData(viewModel, listModel);

            //Bind the tools dropdown
            SetToolMenu(viewModel);
            
            //Set scheduler parameter for content container view model
            SetSchedulerParamforContentContainer(viewModel);

            if (listModel?.ContainerList?.Count > 0)
                return viewModel;
            else
                return new ContentContainerListViewModel() { ContentContainers = new List<ContentContainerViewModel>() };
        }

        //Create Content Container
        public virtual ContentContainerViewModel Create(BindDataModel model)
        {
            ContentContainerViewModel contentContainerViewModel = BindContainerViewModel(model);
            contentContainerViewModel.ContainerKey = contentContainerViewModel.ContainerKey.TrimEnd(',');
            RemoveContainerKeysFromBindDataModel(model);
            contentContainerViewModel = _contentContainerClient.Create(contentContainerViewModel.ToModel<ContentContainerCreateModel>()).ToViewModel<ContentContainerViewModel>();
            model.ControlsData["EntityId"] = contentContainerViewModel.ContainerProfileVariantId;
            model.ControlsData["FamilyCode"] = contentContainerViewModel.FamilyCode;
            string errorMessage = string.Empty;
            EntityAttributeViewModel entityAttributeViewModel = new GlobalAttributeEntityAgent(GetClient<GlobalAttributeEntityClient>(), GetClient<ContainerTemplateClient>(), GetClient<ContentContainerClient>()).SaveEntityAttributeDetails(model, out errorMessage);
            return contentContainerViewModel;
        }

        //Edit Content Container
        public virtual ContentContainerViewModel Edit(string containerKey)
        {
            ContentContainerResponseModel containerResponseData = _contentContainerClient.GetContentContainer(containerKey);
            ContentContainerViewModel container = HelperUtility.IsNotNull(containerResponseData) ? containerResponseData.ToViewModel<ContentContainerViewModel>() : new ContentContainerViewModel();
            SaveInSession(AdminConstants.ContainerVariantSessionKey + containerKey, container.ContainerVariants);
            return container;
        }

        //Update Content Container
        public virtual ContentContainerViewModel Update(ContentContainerViewModel model)
            => _contentContainerClient.Update(model.ToModel<ContentContainerUpdateModel>()).ToViewModel<ContentContainerViewModel>();

        //Associate Content Container Variant
        public virtual List<ContainerVariantViewModel> AssociateVariant(ContainerVariantViewModel model, out string message)
        {
            message = string.Empty;
            try
            {
                List<ContainerVariantViewModel> containerVariants = _contentContainerClient.AssociateVariant(model.ToModel<AssociatedVariantModel>()).ToViewModel<ContainerVariantViewModel>().ToList();
                return containerVariants;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                message = ex.ErrorMessage;
                return new List<ContainerVariantViewModel>();
            }
        }

        //Get Unassociated Content Container Profile Variant
        public virtual List<SelectListItem> GetUnassociatedProfiles(string containerKey)
        {
            ProfileListModel profileList = _profileClient.GetProfileList(null, null, null, null);

            List<ContainerVariantViewModel> variants = GetFromSession<List<ContainerVariantViewModel>>(AdminConstants.ContainerVariantSessionKey + containerKey);
            if (HelperUtility.IsNull(variants) || variants?.Count < 1)
                variants = GetVariants(containerKey);

            List<SelectListItem> profiles = new List<SelectListItem>();
            if (profileList.Profiles?.Count > 0)
            {
                profiles.Add(new SelectListItem { Text = "Any User Profile", Value = "0" });
                profileList.Profiles.ForEach(x => { profiles.Add(new SelectListItem { Text = x.ProfileName + AdminConstants.PipeSeperator + x.DefaultExternalAccountNo, Value = x.ProfileId.ToString() }); });
            }
            return profiles;
        }

        //Delete variant associated to Content Container
        public virtual bool DeleteAssociatedVariant(string variantId, string containerKey, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            try
            {
                RemoveInSession(AdminConstants.ContainerVariantSessionKey + containerKey);
                return _contentContainerClient.DeleteAssociatedVariant(new ParameterModel { Ids = variantId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                errorMessage = ex.ErrorMessage;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
        }

        //Delete Content Container
        public virtual bool DeleteContentContainer(string contentContainerIds, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            try
            {
                return _contentContainerClient.DeleteContentContainer(new ParameterModel { Ids = contentContainerIds });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                errorMessage = Admin_Resources.DeleteContentContainer;
                return false;

            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
        }

        public virtual bool AssociateContainerTemplate(int variantId, int containerTemplateId)
           => _contentContainerClient.AssociateContainerTemplate(variantId, containerTemplateId);

        //Verify if the Container Key Exist
        public virtual bool IsContainerExist(string containerKey)
            => !string.IsNullOrEmpty(containerKey) ? _contentContainerClient.IsContainerKeyExist(containerKey) : true;

        //Bind Container model with the available Container Families
        public void BindContentContainerModel(ContentContainerViewModel model)
        {
            model.ContainerFamilies = GetContainerFamilies();
            model.ContainerTemplates = GetContainerTemplates();
            model.Locale = GetAvailableLocales();
        }

        //Get Available Locale
        public virtual List<SelectListItem> GetAvailableLocales()
         =>  new LocaleAgent(GetClient<LocaleClient>()).GetLocalesList();       

        //Get Profile Variant Dropdown
        public virtual List<SelectListItem> GetProfileVariantDropdown(ProfileListModel profileVariants, bool isAllVariants = false)
        {
            List<SelectListItem> profiles = new List<SelectListItem>();
            if (profileVariants.Profiles?.Count > 0)
            {
                if (isAllVariants)
                    profiles.Add(new SelectListItem { Text = Admin_Resources.LabelAllProfiles, Value = "0" });
                profileVariants.Profiles.ForEach(x => { profiles.Add(new SelectListItem { Text = x.ProfileName, Value = x.ProfileId.ToString() }); });
            }
            return profiles;
        }


        //Get Associated variants
        public virtual List<ContainerVariantViewModel> GetVariants(string containerKey)
        {
            List<ContainerVariantViewModel> variants = _contentContainerClient.GetAssociatedVariants(containerKey).ToViewModel<ContainerVariantViewModel>().ToList();

            SaveInSession(AdminConstants.ContainerVariantSessionKey + containerKey, variants);
            return variants;
        }

        //Get the List of Content Containers
        public virtual ContainerVariantListViewModel GetAssociatedVariantList(string containerKey, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            filters.RemoveAll(x => x.Item1.ToLower() == FilterKeys.ContainerKey.ToString().ToLower());
            filters.Add(new FilterTuple(FilterKeys.ContainerKey.ToString(), FilterOperators.Equals, Convert.ToString(containerKey)));

            AssociatedVariantListModel listModel = _contentContainerClient.GetAssociatedVariantList(null, filters, sortCollection, pageIndex, recordPerPage);
            ContainerVariantListViewModel viewModel = new ContainerVariantListViewModel { ContainerVariants = listModel?.AssociatedVariants?.ToViewModel<ContainerVariantViewModel>().ToList() };
            //Set the Status of containerVariant as complementary of IsActive property in order to show the activate/deactivate icon in case of IsActive as false/true respectively. 
            viewModel.ContainerVariants?.ForEach(x => { x.Status = x.IsActive; x.IsActive = !x.Status; });
            //Set Paging Data 
            SetListPagingData(viewModel, listModel);

            //Bind the tools dropdown
            SetVariantListToolMenus(viewModel);

            if (listModel?.AssociatedVariants?.Count > 0)
                return viewModel;
            else
                return new ContainerVariantListViewModel();
        }

        //Save Variants data.
        public virtual bool SaveAssociatedVariantData([ModelBinder(typeof(ControlsModelBinder))] BindDataModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            int localeId = Convert.ToInt32(model.GetValue("LocaleId"));
            int templateId;bool isActive;
            int.TryParse(model.GetValue("ContainerTemplateId").ToString(), out templateId);
            int variantId = Convert.ToInt32(model.GetValue("ProfileVariantId")); 
            //Get the value of IsActive from BindDataModel
            bool.TryParse(model.GetValue("IsActive")?.ToString(), out isActive);

            return _contentContainerClient.SaveVariantData(localeId, templateId, variantId, isActive);            
        }

        //Get Available Portal
        public virtual List<SelectListItem> GetAvailablePortals()
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            int portalId = 0;
            //Get all available portal list.
            PortalListModel portalList = _portalClient.GetPortalList(null, null, null, null, null);

            listItems.Add(new SelectListItem { Text = "Any Store", Value = "0" });

            portalList.PortalList.ForEach(x => { listItems.Add(new SelectListItem { Text = x.StoreName + AdminConstants.PipeSeperator + x.StoreCode, Value = x.PortalId.ToString() }); });

            ZnodeLogging.LogMessage("listItems list count:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, listItems?.Count());
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            return listItems;
        }

        //Delete variant associated to Content Container
        public virtual bool ActivateDeactivateVariant(string containerProfileVariantIds, bool isActivate, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            try
            {
                return _contentContainerClient.ActivateDeactivateVariant(new ParameterModel { Ids = containerProfileVariantIds }, isActivate);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                errorMessage = ex.ErrorMessage;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorFailedToUpdate;
                return false;
            }
        }

        //Publish the content container
        public bool PublishContentContainer(string containerKey, out string errorMessage, string targetPublishState = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            try
            {
                PublishedModel result = _contentContainerClient.PublishContentContainer(containerKey, targetPublishState);
                if (HelperUtility.IsNull(result?.IsPublished) || result?.IsPublished == false)
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

        //Publish the content container Variant
        public bool PublishContainerVariant(string containerKey, int containerProfileVariantId, out string errorMessage, string targetPublishState = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            try
            {
                PublishedModel result = _contentContainerClient.PublishContainerVariant(containerKey, containerProfileVariantId, targetPublishState);
                if (HelperUtility.IsNull(result?.IsPublished) || result?.IsPublished == false)
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
        #endregion


        #region Protected Methods

        //Bind Tools Dropdown
        protected virtual void SetToolMenu(ContentContainerListViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('ContentContainerDeletePopup')", ControllerName = "ContentContainer", ActionName = "Delete" });
            }
        }

        //Get Variant Dropdown
        protected virtual List<SelectListItem> GetVariantDropdown(List<ContainerVariantViewModel> containerVariants)
        {
            List<SelectListItem> variants = new List<SelectListItem>();
            if (containerVariants?.Count > 0)
                containerVariants.ForEach(x => { variants.Add(new SelectListItem { Text = $"Variant For {x.ProfileName} And {x.StoreName}", Value = x.ContainerProfileVariantId.ToString() }); });
            return variants;
        }

        //Get Container Families
        protected virtual List<SelectListItem> GetContainerFamilies()
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(ZnodeGlobalEntityEnum.GlobalEntityId.ToString(), FilterOperators.Equals, ((int)EntityType.Container).ToString());
            List<GlobalAttributeFamilyModel> familyList = _globalAttributesFamilyClient.GetAttributeFamilyList(null, filters, null, null, null).AttributeFamilyList;
            List<SelectListItem> containerFamilies = new List<SelectListItem>();

            familyList?.ForEach(item => { containerFamilies.Add(new SelectListItem() { Text = item.AttributeFamilyName, Value = item.FamilyCode }); });

            return containerFamilies;
        }

        protected virtual List<SelectListItem> GetContainerTemplates()
        {
            List<ContainerTemplateModel> containerTemplateList = _containerTemplateClient.List(null, null, null, null, null).ContainerTemplates;
            List<SelectListItem> containerTemplate = new List<SelectListItem>();

            containerTemplateList?.ForEach(item => { containerTemplate.Add(new SelectListItem() { Text = $"{item.Name} | {item.Code}", Value = item.ContainerTemplateId.ToString() }); });

            return containerTemplate;
        }

        // Removed the keys from model as model passed to save attribute data which does not required following data.
        protected virtual void RemoveContainerKeysFromBindDataModel(BindDataModel model)
        {
            model.ControlsData.Remove("ContainerKey");
            model.ControlsData.Remove("Tags");
            model.ControlsData.Remove("Name");
            model.ControlsData.Remove("ContainerTemplateId");
            model.ControlsData.Remove("ContentContainerId");
            model.ControlsData.Remove("LocaleId");
            model.ControlsData.Remove("IsRedirectToEditVariantScreen");
            //remove the IsActive property from ControlsData since it is not required in further methods.
            model.ControlsData.Remove("IsActive");
        }

        protected virtual ContentContainerViewModel BindContainerViewModel(BindDataModel model)
        {
            int containerTemplateId;
            int.TryParse(model.GetValue("ContainerTemplateId").ToString(), out containerTemplateId);
            return new ContentContainerViewModel()
            {
                ContainerKey = Convert.ToString(model.GetValue("ContainerKey")),
                Tags = Convert.ToString(model.GetValue("Tags")),
                FamilyCode = Convert.ToString(model.GetValue("FamilyCode")),
                Name = Convert.ToString(model.GetValue("Name")),
                ContainerTemplateId = containerTemplateId,
                LocaleId = Convert.ToInt32(model.GetValue("LocaleId")),
                IsActive = Convert.ToBoolean(model.GetValue("IsActive"))
            };
        }

        protected virtual FilterCollection GetPortalFilter(ContentContainerViewModel model)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(FilterKeys.PortalId, FilterOperators.Equals, model.PortalId.ToString());
            return filters;
        }

        //Set scheduler parameter for content container view model
        protected virtual ContentContainerListViewModel SetSchedulerParamforContentContainer(ContentContainerListViewModel viewModel)
        {
            viewModel.ContentContainers?.ForEach(item =>
            {
                item.UrlEncodedContainerKey = HttpUtility.HtmlEncode(item.ContainerKey);
                item.ConnectorTouchPoints = ZnodeConstant.PublishContainer + "_" + item.ContentContainerId + "_" + HttpUtility.UrlEncode(item.ContainerKey);
                item.SchedulerCallFor = ZnodeConstant.PublishContentContainer;
            });
            return viewModel;
        }
        #endregion

        #region private Method
        //Set the Tool Menus for Store List Grid View.
        private void SetVariantListToolMenus(ContainerVariantListViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('DeleteContainerVariant');", ControllerName = "ContentContainer", ActionName = "DeleteAssociatedVariant" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonActivate, JSFunctionName = "EditableText.prototype.DialogDelete('PopUpConfirmActivate');", ControllerName = "ContentContainer", ActionName = "ActivateDeactivateVariant" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDeactivate, JSFunctionName = "EditableText.prototype.DialogDelete('PopUpConfirmDeactivate');", ControllerName = "ContentContainer", ActionName = "ActivateDeactivateVariant" });
            }
        }
        #endregion
    }
}