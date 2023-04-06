using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.WebStore;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.Helpers;
using Znode.Engine.WebStore.Maps;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.WebStore.Core.Agents
{
    public class FormBuilderAgent : BaseAgent, IFormBuilderAgent
    {
        #region Private Members
        private readonly IFormBuilderClient _formBuilderClient;
        private readonly IGlobalAttributeClient _globalAttributeClient;
        private readonly IGlobalAttributeEntityClient _globalAttributeEntityClient;
        #endregion

        #region Constructor
        public FormBuilderAgent(IFormBuilderClient formBuilderClient, IGlobalAttributeClient globalAttributeClient, IGlobalAttributeEntityClient globalAttributeEntityClient)
        {
            _formBuilderClient = GetClient<IFormBuilderClient>(formBuilderClient);
            _globalAttributeClient = GetClient<IGlobalAttributeClient>(globalAttributeClient);
            _globalAttributeEntityClient = GetClient<IGlobalAttributeEntityClient>(globalAttributeEntityClient);
        }
        #endregion

        #region Public Method

        //Get Entity Associated Attribute Details.
        public virtual FormBuilderAttributeGroupViewModel GetFormTemplate(int formBuilderId, int localeId, int mappingId = 0)
         => (formBuilderId > 0 && localeId > 0) ? GlobalAttributeModelMap.ToGlobalAttributeEntityDetailViewModel(_formBuilderClient.GetFormAttributeGroup(formBuilderId, localeId, mappingId)) : new FormBuilderAttributeGroupViewModel();

        //Save Form Template data.
        public virtual FormSubmitViewModel CreateFormTemplate(BindDataModel bindDataModel)
        {
            RemoveNonAttributeKeys(bindDataModel);
            RemoveAttributeWithEmptyValue(bindDataModel);
            FormSubmitViewModel attributeViewModel = GetFormSubmitViewModel(bindDataModel);
            FormSubmitModel model = _formBuilderClient.CreateFormTemplate(attributeViewModel.ToModel<FormSubmitModel>());
            return model?.ToViewModel<FormSubmitViewModel>();
        }

        //Check value of attribute is already exists or not.
        public virtual string IsFormAttributeValueUnique(GlobalAttributeValueParameterModel model)
        {
            string errorMessage = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(model.AttributeCodeValues))
                {
                    string[] arrayAttributeCodeValues = model.AttributeCodeValues.Split('~');
                    ParameterModel parameterModel = _formBuilderClient.FormAttributeValueUnique(GetGlobalAttributeValueParameterDetails(arrayAttributeCodeValues, model.Id, model.EntityType));
                    if (!string.IsNullOrEmpty(parameterModel?.Ids))
                        errorMessage = string.Format(WebStore_Resources.ErrorAlreadyExistsAttributeCode, parameterModel.Ids);
                }

                return errorMessage;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.IdLessThanOne:
                        errorMessage = WebStore_Resources.InvalidEntityInformation;
                        break;
                    case ErrorCodes.NullModel:
                        errorMessage = ZnodeConstant.NullModelError;
                        break;
                    default:
                        errorMessage = WebStore_Resources.TextInvalidData;
                        break;
                }
                return errorMessage;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                errorMessage = WebStore_Resources.TextInvalidData;
                return errorMessage;
            }
        }

        #endregion

        #region Private Method
        protected virtual FormSubmitViewModel GetFormSubmitViewModel(BindDataModel model)
        {
            FormSubmitViewModel entityAttributeModel = new FormSubmitViewModel();
            model.ControlsData?.ToList().ForEach(item =>
            {
                List<object> itemList = new List<object>();
                itemList.AddRange(item.Key.Split('_'));
                if (itemList.Count() >= 5)
                {
                    entityAttributeModel.Attributes.Add(new FormSubmitAttributeViewModel
                    {
                        AttributeCode = itemList[0].ToString(),
                        GlobalAttributeId = Convert.ToInt32(itemList[1]),
                        GlobalAttributeDefaultValueId = Convert.ToInt32(itemList[2]),
                        GlobalAttributeValueId = Convert.ToInt32(itemList[3]),
                        AttributeValue = item.Value.ToString().Trim(),
                        LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale),
                        FormTemplateId = Convert.ToInt32(itemList[4])
                    });
                }
            });
            entityAttributeModel.FormBuilderId = entityAttributeModel.Attributes?.FirstOrDefault()?.FormTemplateId;
            entityAttributeModel.PortalId = PortalAgent.CurrentPortal.PortalId;
            entityAttributeModel.LocaleId = PortalAgent.LocaleId;
            entityAttributeModel.CustomerEmail = GetCustomerEmail(model);
            return entityAttributeModel;
        }

        //Get customer email details
        protected virtual string GetCustomerEmail(BindDataModel model)
        {
            var controls = model.ControlsData.Where(o => o.Key.Contains("_Email")).Select(x => x.Value).ToList();
            return string.Join(",", controls);
        }

        //Get global attributevalue parameter details.
        private GlobalAttributeValueParameterModel GetGlobalAttributeValueParameterDetails(string[] attributeCodeValues, int id, string entityType)
        {
            List<GlobalAttributeCodeValueModel> globalAttributeCodeValueList = new List<GlobalAttributeCodeValueModel>();
            foreach (var attributeCodeValue in attributeCodeValues)
                globalAttributeCodeValueList.Add(new GlobalAttributeCodeValueModel { AttributeCode = attributeCodeValue.Split('#')[0], AttributeValues = attributeCodeValue.Split('#')[1].Trim() });

            return new GlobalAttributeValueParameterModel()
            {
                Id = id,
                LocaleId = PortalAgent.LocaleId,
                PortalId = PortalAgent.CurrentPortal.PortalId,
                EntityType = entityType,
                GlobalAttributeCodeValueList = globalAttributeCodeValueList
            };
        }

        //Get Entity Associated Attribute Details.
        public virtual GlobalAttributeEntityDetailsViewModel GetEntityAttributeDetails(int entityId, string entityType)
        {
            GlobalAttributeEntityDetailsViewModel entityAttributeModel = new GlobalAttributeEntityDetailsViewModel();
            if (entityId > 0 && !string.IsNullOrEmpty(entityType))
            {
                entityAttributeModel = GlobalAttributeModelMap.ToGlobalAttributeEntityDetailViewModel(_globalAttributeEntityClient.GetEntityAttributeDetails(entityId, entityType));
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

                //Remove unwanted attributes present on form collection
                RemoveNonAttributeKeys(model);
                RemoveAttributeWithEmptyValue(model);
                EntityAttributeViewModel attributeViewModel = GetEntityAttributeViewModel(model);
                attributeViewModel.EntityValueId = entityValue;
                attributeViewModel.EntityType = entityType;
                EntityAttributeModel entityAttribute = _globalAttributeEntityClient.SaveEntityAttributeDetails(attributeViewModel.ToModel<EntityAttributeModel>());
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

        public virtual EntityAttributeViewModel GetEntityAttributeViewModel(BindDataModel model)
        {
            EntityAttributeViewModel entityAttributeModel = new EntityAttributeViewModel();

            model.ControlsData?.ToList().ForEach(item =>
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
                        LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale)
                    });
                }
            });

            return entityAttributeModel;
        }

        #endregion
    }
}
