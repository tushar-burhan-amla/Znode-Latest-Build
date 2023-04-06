using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Agents
{
    public class GlobalAttributeEntityAgent : BaseAgent, IGlobalAttributeEntityAgent
    {
        #region Private Members
        private readonly IGlobalAttributeEntityClient _globalAttributesEntityClient;
        private readonly IContainerTemplateClient _widgetTemplateClient;
        protected readonly IContentContainerClient _contentWidgetClient;
        #endregion

        #region Public Constructor
        public GlobalAttributeEntityAgent(IGlobalAttributeEntityClient globalAttributeEntityClient, IContainerTemplateClient widgetTemplateClient, IContentContainerClient contentWidgetClient)
        {
            _globalAttributesEntityClient = GetClient<IGlobalAttributeEntityClient>(globalAttributeEntityClient);
            _widgetTemplateClient = GetClient<IContainerTemplateClient>(widgetTemplateClient);
            _contentWidgetClient = GetClient<IContentContainerClient>(contentWidgetClient);
        }
        #endregion

        public virtual void CreateFiltersForEntityGroup(int entityId)
        {
            Filters = new FilterCollection();
            Filters.Add(new FilterTuple(FilterKeys.GlobalEntityId, FilterOperators.Equals, entityId.ToString()));
        }

        public virtual GlobalAttributeEntityViewModel GetGlobalEntity()
        {
            GlobalEntityListModel globalEntityList = _globalAttributesEntityClient.GetGlobalEntity();

            List<SelectListItem> entityList = new List<SelectListItem>();
            globalEntityList?.GlobalEntityList?.ToList().ForEach(item => { entityList.Add(new SelectListItem() { Text = item.EntityName, Value = Convert.ToString(item.GlobalEntityId) }); });

            GlobalAttributeEntityViewModel attributeEntityViewModel = new GlobalAttributeEntityViewModel() { AttributeEntityList = entityList };

            return attributeEntityViewModel ?? new GlobalAttributeEntityViewModel();
        }

        public virtual AssignedEntityGroupListViewModel GetAssignedEntityAttributeGroups(int entityId)
        {
            CreateFiltersForEntityGroup(entityId);
            CreateExpand();

            GlobalAttributeGroupListModel globalAttributeGroupList = _globalAttributesEntityClient.GetAssignedEntityAttributeGroups(Expands, Filters, new SortCollection(), null, null);

            AssignedEntityGroupListViewModel assignedEntityGroupListViewModel = new AssignedEntityGroupListViewModel();
            assignedEntityGroupListViewModel.AssignedEntityGroupList = globalAttributeGroupList?.AttributeGroupList?.ToViewModel<AssignedEntityGroupViewModel>().ToList();
            assignedEntityGroupListViewModel.EntityId = entityId;
            return assignedEntityGroupListViewModel ?? new AssignedEntityGroupListViewModel();

        }

        public virtual void CreateExpand()
        {
            Expands = new ExpandCollection();
            Expands.Add(ExpandKeys.GlobalAttributeGroup);
        }

        public virtual List<BaseDropDownList> GetUnAssignedEntityAttributeGroups(int entityId)
        {
            CreateFiltersForEntityGroup(entityId);
            return GlobalAttributeEntityViewModelMap.ToBaseDropDownList(_globalAttributesEntityClient.GetUnAssignedEntityAttributeGroups(new ExpandCollection(), Filters, null, null, null));
        }

        public virtual bool AssignAttributeEntityToGroups(string attributeGroupIds, int entityId, out string message)
        {
            message = string.Empty;
            if (!string.IsNullOrEmpty(attributeGroupIds) && entityId > 0)
            {
                try
                {
                    return _globalAttributesEntityClient.AssociateAttributeEntityToGroups(new GlobalAttributeGroupEntityModel { EntityId = entityId, GroupIds = attributeGroupIds });

                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.InvalidData:
                            message = Admin_Resources.TextInvalidData;
                            return false;
                        default:
                            message = Admin_Resources.TextInvalidData;
                            return false;
                    }
                }
                catch(Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    message = Admin_Resources.ErrorFailedToAssign;
                    return false;
                }
            }
            return false;
        }

        public virtual bool UnAssignEntityGroups(int entityId, int groupId, out string message)
        {
            message = string.Empty;
            bool isUnAssign = false;
            try
            {
                if (entityId > 0 && groupId > 0)
                    isUnAssign = _globalAttributesEntityClient.UnAssociateEntityGroups(entityId, groupId);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AssociationDeleteError:
                        message = Admin_Resources.ErrorFailToDeleteGlobalAttributeGroup;
                        return isUnAssign;
                    default:
                        message = Admin_Resources.TextInvalidData;
                        return isUnAssign;
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                message = Admin_Resources.UnassignError;
                return isUnAssign;
            }
            return isUnAssign;
        }

        //Update Attribute Group Display Order
        public virtual GlobalAttributeGroupViewModel UpdateAttributeGroupDisplayOrder(int globalAttributeGroupId, int globalAttributeEntityId, int DisplayOrder)
        {
            try
            {
                GlobalAttributeGroupModel groupModel = new GlobalAttributeGroupModel() { DisplayOrder = DisplayOrder, GlobalAttributeGroupId = globalAttributeGroupId, GlobalEntityId = globalAttributeEntityId };
                GlobalAttributeGroupViewModel attributeDataViewModel = _globalAttributesEntityClient.UpdateAttributeGroupDisplayOrder(groupModel).ToViewModel<GlobalAttributeGroupViewModel>();
                return attributeDataViewModel;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                return (GlobalAttributeGroupViewModel)GetViewModelWithErrorMessage(new GlobalAttributeGroupViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (GlobalAttributeGroupViewModel)GetViewModelWithErrorMessage(new GlobalAttributeGroupViewModel(), Admin_Resources.UpdateErrorMessage);
            }
        }

        //Get Entity Associated Attribute Details.
        public virtual GlobalAttributeEntityDetailsViewModel GetEntityAttributeDetails(int entityId, string entityType)
        {
            GlobalAttributeEntityDetailsViewModel entityAttributeModel = new GlobalAttributeEntityDetailsViewModel();
            if (entityId > 0 && !string.IsNullOrEmpty(entityType))
            {
                entityAttributeModel = GlobalAttributeModelMap.ToGlobalAttributeEntityDetailViewModel(_globalAttributesEntityClient.GetEntityAttributeDetails(entityId, entityType));
            }
            return entityAttributeModel ?? new GlobalAttributeEntityDetailsViewModel();
        }

        public virtual EntityAttributeViewModel SaveEntityAttributeDetails(BindDataModel model, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                int entityValue = Convert.ToInt32(model.GetValue("EntityId"));
                string entityType = Convert.ToString(model.GetValue("EntityType"));
                string familyCode = string.Empty;
                if (model.ControlsData.ContainsKey("FamilyCode"))
                    familyCode = Convert.ToString(model.GetValue("FamilyCode"));
                else
                    familyCode = Convert.ToString(model.GetValue("LocaleFamilyCode"));
                int localeId = Convert.ToInt32(model.GetValue("LocaleId"));
                GlobalAttributeEntityDetailsViewModel entityAttributeModel = GetEntityAttributeDetails(entityValue, entityType);
                //Remove unwanted attributes present on form collection
                RemoveNonAttributeKeys(model);
                RemoveAttributeWithEmptyValue(model, entityAttributeModel.Attributes);
                RemoveAttrAndMceEditorKeyWord(model);
                EntityAttributeViewModel attributeViewModel = GetEntityAttributeViewModel(model, localeId);
                attributeViewModel.EntityValueId = entityValue;
                attributeViewModel.EntityType = entityType;
                attributeViewModel.FamilyCode = familyCode;
                EntityAttributeModel entityAttribute = _globalAttributesEntityClient.SaveEntityAttributeDetails(attributeViewModel.ToModel<EntityAttributeModel>());
                return entityAttribute.ToViewModel<EntityAttributeViewModel>() ?? new EntityAttributeViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        errorMessage = Attributes_Resources.ErrorAttributeAlreadyExists;
                        return new EntityAttributeViewModel { HasError = true };

                    case ErrorCodes.SKUAlreadyExist:
                        errorMessage = PIM_Resources.ErrorSKUAlreadyExists;
                        return new EntityAttributeViewModel { HasError = true };

                    default:
                        errorMessage = Admin_Resources.ErrorFailedToCreate;
                        return new EntityAttributeViewModel { HasError = true };
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return new EntityAttributeViewModel { HasError = true, ErrorMessage = Admin_Resources.ErrorFailedToCreate };
            }
        }

        public virtual EntityAttributeViewModel GetEntityAttributeViewModel(BindDataModel model, int localeId = 0)
        {
            EntityAttributeViewModel entityAttributeModel = new EntityAttributeViewModel();

            model.ControlsData?.ToList().ForEach(item =>
            {
                if (!string.IsNullOrEmpty(item.Value.ToString()))
                {
                    List<object> itemList = new List<object>();
                    itemList.AddRange(item.Key.Split('_'));
                    if (itemList.Count() >= 5)
                    {
                        entityAttributeModel.Attributes.Add(new EntityAttributeDetailsViewModel
                        {
                            AttributeCode = itemList[0].ToString(),
                            GlobalAttributeId = Convert.ToInt32(itemList[1]),
                            GlobalAttributeDefaultValueId = Convert.ToInt32(itemList[2]),
                            GlobalAttributeValueId = Convert.ToInt32(itemList[3]),
                            AttributeValue = item.Value.ToString().Trim(),
                            LocaleId = (localeId > 0) ? localeId : Convert.ToInt32(DefaultSettingHelper.DefaultLocale)
                        });
                    }
                }
            });

            return entityAttributeModel;
        }

        //Create tab structure.
        public virtual TabViewListModel CreateTabStructure(int globalEntityId)
        {
            TabViewListModel TabStructModel = new TabViewListModel();
            TabStructModel.MaintainAllTabData = true;
            TabStructModel.Tabs.Add(new TabViewModel() { Id = 1, Name = Admin_Resources.TitleAttributeGroupsList, IsVisible = true, Url = $"/GlobalAttribute/AssignedEntityAttributeGroupList?id={globalEntityId}", IsSelected = true });
            return TabStructModel;
        }

        //gets the global attributes based on the passed familyCode for setting the values for default container variant.
        public virtual GlobalAttributeEntityDetailsViewModel GetGlobalAttributesForDefaultVariantData(string familyCode, string entityType)
        {
            GlobalAttributeEntityDetailsViewModel entityAttributeModel = new GlobalAttributeEntityDetailsViewModel();
            if (!string.IsNullOrEmpty(familyCode) && !string.IsNullOrEmpty(entityType))
            {
                entityAttributeModel = GlobalAttributeModelMap.ToGlobalAttributeEntityDetailViewModel(_globalAttributesEntityClient.GetGlobalAttributesForDefaultVariantData(familyCode, entityType));
                entityAttributeModel.FamilyCode = familyCode;
            }
            return entityAttributeModel ?? new GlobalAttributeEntityDetailsViewModel();
        }

        // Get Global Attribute details on the basis of Variant id and localeid
        public virtual ContainerVariantDataViewModel GetGlobalAttributesForAssociatedVariant(int variantId, int localeId = 0)
        {
            ContainerVariantDataViewModel variantDataViewModel = new ContainerVariantDataViewModel();
            GlobalAttributeEntityDetailsViewModel entityAttributeModel = new GlobalAttributeEntityDetailsViewModel();
            ContentContainerResponseModel widgetResponseData = _contentWidgetClient.GetVariantLocaleData(variantId) ?? new ContentContainerResponseModel();

            string entityType = Admin_Resources.LabelContentContainers;
            if (variantId > 0)
            {
                entityAttributeModel = GlobalAttributeModelMap.ToGlobalAttributeEntityDetailViewModel(_globalAttributesEntityClient.GetGlobalAttributesForAssociatedVariant(variantId, entityType, localeId));
            }

            variantDataViewModel = widgetResponseData.ToViewModel<ContainerVariantDataViewModel>();
            variantDataViewModel.EntityId = variantId;
            variantDataViewModel.EntityType = entityAttributeModel.EntityType;
            variantDataViewModel.Attributes = entityAttributeModel.Attributes;
            variantDataViewModel.Groups = entityAttributeModel.Groups;
            variantDataViewModel.FamilyCode = entityAttributeModel.FamilyCode;
            variantDataViewModel.ProfileVariantId = variantId;
            variantDataViewModel.Locales = new LocaleAgent(GetClient<LocaleClient>()).GetLocalesList();
            variantDataViewModel.ContainerTemplates = GetWidgetTemplates();
            variantDataViewModel.entityAttributeModel = entityAttributeModel;
            entityAttributeModel.EntityId = variantId;
            //Check if global attributes data is added in particular variant for default locale
            variantDataViewModel.IsDataAddedForDefaultLocale = CheckIfDataExistsForDefaultLocale(variantDataViewModel.LocaleId, entityAttributeModel.Attributes);
            return variantDataViewModel ?? new ContainerVariantDataViewModel();
        }

        //Check if global attributes data is added in particular variant for default locale
        protected virtual bool CheckIfDataExistsForDefaultLocale(int? localeId, List<GlobalAttributeValuesViewModel> attributes)
        {
            if (attributes == null) return false;
            else if (Convert.ToInt32(DefaultSettingHelper.DefaultLocale) == localeId && attributes.All(x => string.IsNullOrEmpty(x.AttributeValue)) && attributes.Any(x => x.IsRequired)) return false;
            else return true;
        }


        protected virtual List<SelectListItem> GetWidgetTemplates()
        {
            List<ContainerTemplateModel> widgetTemplateList = _widgetTemplateClient.List(null, null, null, null, null).ContainerTemplates;
            List<SelectListItem> widgetTemplate = new List<SelectListItem>();

            widgetTemplateList?.ForEach(item => { widgetTemplate.Add(new SelectListItem() { Text = $"{item.Name} | {item.Code}", Value = item.ContainerTemplateId.ToString() }); });

            return widgetTemplate;
        }
    }
}

