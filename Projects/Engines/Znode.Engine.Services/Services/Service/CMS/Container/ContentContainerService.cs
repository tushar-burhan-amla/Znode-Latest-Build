using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class ContentContainerService : BaseService, IContentContainerService
    {
        #region Private variables
        protected readonly IZnodeRepository<ZnodeCMSContentContainer> _contentContainerRepository;
        protected readonly IZnodeRepository<ZnodeCMSContainerProfileVariant> _containerProfileVariantRepository;
        protected readonly IZnodeRepository<ZnodePortal> _portalRepository;
        protected readonly IZnodeRepository<ZnodeProfile> _profileRepository;
        protected readonly IZnodeRepository<ZnodeGlobalEntityFamilyMapper> _entityFamilyMapperRepository;
        protected readonly IZnodeRepository<ZnodeCMSContainerProfileVariantLocale> _containerProfileVariantLocaleRepository;
        protected readonly IZnodeRepository<ZnodeGlobalAttributeFamily> _globalAttributeFamily;

        #endregion

        #region Public Constructor
        public ContentContainerService()
        {
            _containerProfileVariantRepository = new ZnodeRepository<ZnodeCMSContainerProfileVariant>();
            _contentContainerRepository = new ZnodeRepository<ZnodeCMSContentContainer>();
            _portalRepository = new ZnodeRepository<ZnodePortal>();
            _profileRepository = new ZnodeRepository<ZnodeProfile>();
            _entityFamilyMapperRepository = new ZnodeRepository<ZnodeGlobalEntityFamilyMapper>();
            _containerProfileVariantLocaleRepository = new ZnodeRepository<ZnodeCMSContainerProfileVariantLocale>();
            _globalAttributeFamily = new ZnodeRepository<ZnodeGlobalAttributeFamily>();
        }
        #endregion

        // Get the List of Content Container
        public virtual ContentContainerListModel List(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            //set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);

            IZnodeViewRepository<ContentContainerResponseModel> objStoredProc = new ZnodeViewRepository<ContentContainerResponseModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<ContentContainerResponseModel> contentContainerListResponse = objStoredProc.ExecuteStoredProcedureList("Znode_GetCMSContainerlist @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);

            ContentContainerListModel contentContainerListModel = new ContentContainerListModel();
            contentContainerListModel.ContainerList = contentContainerListResponse?.Count > 0 ? contentContainerListResponse?.ToList() : null;

            contentContainerListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return contentContainerListModel;
        }

        //Create Content Container
        public virtual ContentContainerResponseModel Create(ContentContainerCreateModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            if (IsContainerKeyExists(model.ContainerKey))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ContainerKeyExist);

            model.FamilyId = GetFamilyId(model.FamilyCode);
            // while creating new container default state set as draft.
            model.PublishStateId = Convert.ToInt32(ZnodePublishStatesEnum.DRAFT);
            //Insert Content Container Details
            ContentContainerResponseModel contentContainer = _contentContainerRepository.Insert(model.ToEntity<ZnodeCMSContentContainer>()).ToModel<ContentContainerResponseModel>();
            contentContainer.GlobalEntityId = (int)GlobalAttributeEntityType.Container;
            contentContainer.FamilyCode = model.FamilyCode;
            contentContainer.ContainerTemplateId = model.ContainerTemplateId;

            //Insert data into Entity Family Mapper
            ZnodeGlobalEntityFamilyMapper entityFamilyMapper = contentContainer.ToEntity<ZnodeGlobalEntityFamilyMapper>();
            _entityFamilyMapperRepository.Insert(entityFamilyMapper);

            AssociatedVariantModel defaultVariant = new AssociatedVariantModel()
            {
                ContentContainerId = contentContainer.ContentContainerId,
                LocaleId = Convert.ToInt32(DefaultGlobalConfigSettingHelper.DefaultLocale),
                ContainerTemplateId = model.ContainerTemplateId > 0 ? model.ContainerTemplateId : null,
                IsActive = model.IsActive ,//bind the IsActive property to insert in the database
                PublishStateId = Convert.ToInt32(ZnodePublishStatesEnum.DRAFT) //while creating container variant publish status is set as draft
            };
            //Associate Default Variant while creating content container
            contentContainer.ContainerProfileVariantId = AssociateDefaultVariant(defaultVariant);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return contentContainer;
        }

        //Update Content Container
        public virtual ContentContainerResponseModel Update(ContentContainerUpdateModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);

            ZnodeCMSContentContainer contentContainer = _contentContainerRepository.Table.FirstOrDefault(x => x.ContainerKey.ToLower() == model.ContainerKey.ToLower());

            if (Equals(contentContainer, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidContainerKey);

            contentContainer.Tags = model.Tags;
            contentContainer.Name = model.Name;
            // while updating existing container publish state set as draft.
            contentContainer.PublishStateId = Convert.ToByte(ZnodePublishStatesEnum.DRAFT);

            bool IsUpdated = _contentContainerRepository.Update(contentContainer);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return contentContainer.ToModel<ContentContainerResponseModel>();
        }

        //Delete Content Container
        public virtual bool DeleteContentContainer(ParameterModel contentContainerIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return DeleteContainerByIds(contentContainerIds.Ids);
        }

        //Delete Content Container by Container Key
        public virtual bool DeleteContentContainerByContainerKey(string containerKey)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(containerKey))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidContainerKey);

            int containerId = GetContainerId(containerKey);

            return DeleteContainerByIds(containerId.ToString());
        }

        //Verify if the Container Key Exists
        public virtual bool IsContainerKeyExists(string containerKey)
        {
            if (string.IsNullOrEmpty(containerKey))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidContainerKey);

            return _contentContainerRepository.Table.Any(x => x.ContainerKey.ToLower() == containerKey.ToLower());
        }


        //Get Content Container
        public virtual ContentContainerResponseModel GetContentContainer(string containerKey)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(containerKey))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidContainerKey);

            ContentContainerResponseModel contentContainer = GetContentContainerDetails(containerKey);

            contentContainer.StoreName = string.IsNullOrEmpty(contentContainer.StoreName) ? Admin_Resources.LabelAllStores : (from portalData in _portalRepository.Table.Where(x => x.PortalId == PortalId) select portalData.StoreName).FirstOrDefault();

            contentContainer.ContainerVariants = GetAssociatedVariants(containerKey);

            contentContainer.IsGlobalContentWidget = HelperUtility.IsNull(contentContainer.PortalId) ? ZnodeConstant.True : ZnodeConstant.False;

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return contentContainer;
        }


        //Get Associated variants
        public virtual List<AssociatedVariantModel> GetAssociatedVariants(string containerKey)
        {
            if (string.IsNullOrEmpty(containerKey))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidContainerKey);

            int containerId = GetContainerId(containerKey);
            return GetAssociations(containerId);
        }

        //Delete Variant
        public virtual bool DeleteAssociatedVariant(ParameterModel variantIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            try
            {
                int status = 0;
                IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
                objStoredProc.SetParameter("ContainerProfileVariantId", variantIds.Ids, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteAssociatedVariants @ContainerProfileVariantId,@Status OUT", 1, out status);

                if (deleteResult.FirstOrDefault().Status.Value)
                {
                    ZnodeLogging.LogMessage(Admin_Resources.DeleteMessage, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                    return true;
                }
                else
                {
                    ZnodeLogging.LogMessage(Admin_Resources.ErrorFailedToDelete, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                    throw new ZnodeException(ErrorCodes.AssociationDeleteError, Api_Resources.ErrorDeleteDefaultContainerVariant);
                }
            }
            catch (Exception ex )
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Api_Resources.ErrorDeleteDefaultContainerVariant);
            }
        }

        //Associate Variant
        public virtual List<AssociatedVariantModel>  AssociateVariant(AssociatedVariantModel variant)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            variant.PortalId = (variant.PortalId == 0 || variant.PortalId == null) ? null : variant.PortalId;
            variant.ProfileId = (variant.ProfileId == 0 ||variant.ProfileId  == null ) ? null : variant.ProfileId;

            if (_containerProfileVariantRepository.Table.Any(x => x.CMSContentContainerId == variant.ContentContainerId && x.ProfileId == variant.ProfileId && x.PortalId == variant.PortalId))
                throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.ErrorContainerVariantAlreadyExists);
            
            // while associating(updating existing) container publish state set as draft.
            variant.PublishStateId = Convert.ToInt32(ZnodePublishStatesEnum.DRAFT);

            //while associating(updating existing) container publish state set as draft.
            variant.PublishStateId = Convert.ToInt32(ZnodePublishStatesEnum.DRAFT);
            ZnodeCMSContainerProfileVariant profileVariant = _containerProfileVariantRepository.Insert(variant.ToEntity<ZnodeCMSContainerProfileVariant>());
            variant.ContainerProfileVariantId = profileVariant.CMSContainerProfileVariantId;
            //While associating variant for the first time, we need a default locale to be inserted. 
            variant.LocaleId = Convert.ToInt32(DefaultGlobalConfigSettingHelper.DefaultLocale);
          
            //When a new variant is added, the variant by default should be set as active. 
            variant.IsActive = true;
            ZnodeCMSContainerProfileVariantLocale profileVariantLocale = _containerProfileVariantLocaleRepository.Insert(variant.ToEntity<ZnodeCMSContainerProfileVariantLocale>());
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return GetAssociations(profileVariant.CMSContentContainerId);
        }

        //Associate Default Variant while content container creation
        public virtual int AssociateDefaultVariant(AssociatedVariantModel variant)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (_containerProfileVariantRepository.Table.Any(x => x.CMSContentContainerId == variant.ContentContainerId && x.ProfileId == variant.ProfileId))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidData);

            ZnodeCMSContainerProfileVariant profileVariant = _containerProfileVariantRepository.Insert(variant.ToEntity<ZnodeCMSContainerProfileVariant>());
            variant.ContainerProfileVariantId = profileVariant.CMSContainerProfileVariantId;
            // while associating variants to new container publish state set as draft.
            variant.PublishStateId = Convert.ToInt32(ZnodePublishStatesEnum.DRAFT);
            ZnodeCMSContainerProfileVariantLocale profileVariantLocale = _containerProfileVariantLocaleRepository.Insert(variant.ToEntity<ZnodeCMSContainerProfileVariantLocale>());
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return variant.ContainerProfileVariantId;
        }

        public virtual bool AssociateContainerTemplate(int variantId, int containerTemplateId)
        {
            ZnodeCMSContainerProfileVariant model = _containerProfileVariantRepository.Table.FirstOrDefault(x => x.CMSContainerProfileVariantId == variantId);

            if (Equals(model, null))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelCanNotBeNull);

            if (containerTemplateId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.IdCanNotBeLessThanOne);

            bool isUpdated = _containerProfileVariantRepository.Update(model);

            return isUpdated;

        }

        //Delete containers
        protected virtual bool DeleteContainerByIds(string contentContainerIds)
        {
            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("ContentContainerIds", contentContainerIds, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteContentContainer @ContentContainerIds,@Status OUT", 1, out status);

            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(Admin_Resources.DeleteMessage, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorFailedToDelete);
            }
        }

        // Get the List of Associated Variants
        public virtual AssociatedVariantListModel GetAssociatedVariantList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            AssociatedVariantListModel associatedVariantListModel = new AssociatedVariantListModel();

            string containerKey = (filters.FirstOrDefault(x => x.FilterName.Equals("ContainerKey", StringComparison.InvariantCultureIgnoreCase))?.FilterValue);
            filters.RemoveAll(x => x.Item1.ToLower() == FilterKeys.ContainerKey.ToString().ToLower());

            SetIsActiveFilter(filters);
           
            //set paging parameters.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            try
            {
                IZnodeViewRepository<AssociatedVariantModel> objStoredProc = new ZnodeViewRepository<AssociatedVariantModel>();
                objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@ContainerKey", containerKey, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
                IList<AssociatedVariantModel> variantsResponse = objStoredProc.ExecuteStoredProcedureList("Znode_GetCMSContainerProfileVariantlist @WhereClause,@ContainerKey, @Rows,@PageNo,@Order_By,@RowsCount OUT", 5, out pageListModel.TotalRowCount);

                associatedVariantListModel.AssociatedVariants = variantsResponse?.Count > 0 ? variantsResponse?.ToList() : null;

                associatedVariantListModel.BindPageListModel(pageListModel);
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return associatedVariantListModel;
        }

        // Save LocaleId, templateId and IsActive status against profilevariantId
        public virtual bool SaveVariantData(int localeId, int? templateId, int variantId, bool isActive)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            try
            {
                int status = 0;
                IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
                objStoredProc.SetParameter("LocaleId", localeId, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("CMSContainerTemplateId", templateId, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("CMSContainerProfileVariantId", variantId, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("IsActive", isActive, ParameterDirection.Input, DbType.Boolean);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                IList<View_ReturnBoolean> result = objStoredProc.ExecuteStoredProcedureList("Znode_SaveVariantsData @LocaleId, @CMSContainerTemplateId, @CMSContainerProfileVariantId, @UserId, @IsActive, @Status OUT", 5, out status);


                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return true;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Get Content Container Attribute Values based on container variants.
        public virtual ContentContainerDataModel GetContentContainerData(string containerKey, int localeId, int portalId = 0, int profileId = 0)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (string.IsNullOrEmpty(containerKey))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidContainerKey);
            if (localeId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorLocaleIdLessThanZero);

            ContentContainerDataModel contentContainerDataModel = new ContentContainerDataModel();

            string entityType = ZnodeConstant.ContentContainers;
            try
            {
                ExecuteSpHelper executeSpHelper = new ExecuteSpHelper();
                executeSpHelper.GetParameter("@EntityName", entityType, ParameterDirection.Input, SqlDbType.VarChar);
                executeSpHelper.GetParameter("@ContainerKey", containerKey, ParameterDirection.Input, SqlDbType.VarChar);
                executeSpHelper.GetParameter("@LocalId", localeId, ParameterDirection.Input, SqlDbType.Int);
                executeSpHelper.GetParameter("@PortalId", portalId, ParameterDirection.Input, SqlDbType.Int);
                executeSpHelper.GetParameter("@ProfileId", profileId, ParameterDirection.Input, SqlDbType.Int);

                DataSet containerData = executeSpHelper.GetSPResultInDataSet("Znode_GetPublishContentContainerAttributeValue");

                //Binds the dataset table data to the properties in ContainerDataModel
                contentContainerDataModel = BindDataSetToContainerDataModel(contentContainerDataModel, containerData);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return contentContainerDataModel;
        }



        // Get profile variant locale data based on variant id
        public virtual ContentContainerResponseModel GetVariantLocaleData(int variantId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            try
            {
                ContentContainerResponseModel contentContainer = (from _profileVariantLocale in _containerProfileVariantLocaleRepository.Table
                                                                  join _ProfileVariant in _containerProfileVariantRepository.Table on
                                                                  _profileVariantLocale.CMSContainerProfileVariantId equals _ProfileVariant.CMSContainerProfileVariantId
                                                                  join _contentContainer in _contentContainerRepository.Table on _ProfileVariant.CMSContentContainerId equals _contentContainer.CMSContentContainerId
                                                                  where _ProfileVariant.CMSContainerProfileVariantId == variantId
                                                                  select new ContentContainerResponseModel
                                                                  {
                                                                      ContainerTemplateId = _profileVariantLocale.CMSContainerTemplateId,
                                                                      LocaleId = _profileVariantLocale.LocaleId,
                                                                      PortalId = _ProfileVariant.PortalId,
                                                                      ProfileId = _ProfileVariant.ProfileId,
                                                                      ContainerKey = _contentContainer.ContainerKey,
                                                                      ContainerName = _contentContainer.Name,
                                                                      IsActive = _profileVariantLocale.IsActive
                                                                  }).FirstOrDefault();
                contentContainer.StoreName = (contentContainer.PortalId == null) ? Admin_Resources.LabelAnyStore : _portalRepository.Table.FirstOrDefault(x => x.PortalId == contentContainer.PortalId)?.StoreName;
                contentContainer.ProfileName = (contentContainer.ProfileId == null) ? Admin_Resources.LabelAnyUserProfile : _profileRepository.Table.FirstOrDefault(x => x.ProfileId == contentContainer.ProfileId)?.ProfileName;
                return contentContainer;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return new ContentContainerResponseModel();
            }
        }

        //Activate/Deactivate associated Variant
        public virtual bool ActivateDeactivateVariant(ParameterModel variantIds, bool isActivate)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            try
            {
                int status = 0;
                IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
                objStoredProc.SetParameter("containerProfileVariantIds", variantIds.Ids, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("IsActivate", isActivate, ParameterDirection.Input, DbType.Boolean);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                IList<View_ReturnBoolean> activateDeactivateResult = objStoredProc.ExecuteStoredProcedureList("Znode_ActivateDeactivateAssociatedVariants @ContainerProfileVariantIds,@IsActivate,@Status OUT", 1, out status);

                if (activateDeactivateResult.FirstOrDefault().Status.Value)
                {
                    ZnodeLogging.LogMessage(isActivate ? Api_Resources.VariantActivatedSuccessMessage : Api_Resources.VariantDeactivatedSuccessMessage, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                    return true;
                }
                else
                {
                    if (isActivate)
                    {
                        ZnodeLogging.LogMessage(Api_Resources.ErrorFailedToActivateVariant, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                        throw new ZnodeException(ErrorCodes.InternalItemNotUpdated, Api_Resources.ErrorFailedToActivateVariant);
                    }
                    else
                    {
                        ZnodeLogging.LogMessage(Api_Resources.ErrorDefaultVariantDeactivateMessage, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                        throw new ZnodeException(ErrorCodes.SetDefaultDataError, Api_Resources.ErrorDefaultVariantDeactivateMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, ex.Message);
            }
        }

        #region Protected Methods
        //Get Details of Content Container
        protected virtual ContentContainerResponseModel GetContentContainerDetails(string containerKey)
        {
            IZnodeRepository<ZnodeGlobalAttributeFamilyLocale> _globalAttributeFamilyRepository = new ZnodeRepository<ZnodeGlobalAttributeFamilyLocale>();
            ContentContainerResponseModel contentContainer = (from _contentContainer in _contentContainerRepository.Table
                                                              join _attributeFamily in _globalAttributeFamilyRepository.Table on _contentContainer.FamilyId equals _attributeFamily.GlobalAttributeFamilyId
                                                              where _contentContainer.ContainerKey.ToLower() == containerKey.ToLower()
                                                              select new ContentContainerResponseModel
                                                              {
                                                                  Name = _contentContainer.Name,
                                                                  ContainerKey = containerKey,
                                                                  FamilyName = _attributeFamily.AttributeFamilyName,
                                                                  Tags = _contentContainer.Tags,
                                                                  ContentContainerId = _contentContainer.CMSContentContainerId
                                                              }
                                                     ).FirstOrDefault();
            return contentContainer;

        }

        //Get Associated Variants
        protected virtual List<AssociatedVariantModel> GetAssociations(int containerId)
        {
            return (from variantRepository in _containerProfileVariantRepository.Table
                    join contentContainers in _contentContainerRepository.Table on variantRepository.CMSContentContainerId equals contentContainers.CMSContentContainerId
                    join profileRepository in _profileRepository.Table on variantRepository.ProfileId equals profileRepository.ProfileId
                    join portal in _portalRepository.Table on variantRepository.PortalId equals portal.PortalId
                    into data
                    from portal  in data.DefaultIfEmpty()
                    where variantRepository.CMSContentContainerId == containerId
                    select new AssociatedVariantModel
                    {
                        ProfileId = variantRepository.ProfileId,
                        PortalId =  variantRepository.PortalId,
                        ContentContainerId = variantRepository.CMSContentContainerId,
                        ProfileName = string.IsNullOrEmpty(profileRepository.ProfileName) ? Admin_Resources.LabelAllProfiles : profileRepository.ProfileName,
                        StoreName = string.IsNullOrEmpty(portal.StoreName) ? Admin_Resources.LabelAllStores : portal.StoreName,
                        ContainerKey = contentContainers.ContainerKey,
                        ContainerProfileVariantId = variantRepository.CMSContainerProfileVariantId,
                    }).ToList();
        }

        //Get PortalId
        protected virtual int? GetPortalId(string storeCode)
        {
            int? portalId = (from portalRepository in _portalRepository.Table.Where(x => x.StoreCode.ToLower() == storeCode.ToLower()) select portalRepository.PortalId).FirstOrDefault();

            if (portalId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidData);

            return portalId;

        }

        //Get Family Id
        protected virtual int GetFamilyId(string familyCode)
        {
            IZnodeRepository<ZnodeGlobalAttributeFamily> _globalAttributeFamilyRepository = new ZnodeRepository<ZnodeGlobalAttributeFamily>();
            int familyId = (from globalFamilyRepository in _globalAttributeFamilyRepository.Table.Where(x => x.FamilyCode.ToLower() == familyCode.ToLower()) select globalFamilyRepository.GlobalAttributeFamilyId).FirstOrDefault();

            if (familyId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidFamilyCode);

            return familyId;
        }

        //Get Container Id
        protected virtual int GetContainerId(string containerKey)
        {
            int containerId = (from containerRepository in _contentContainerRepository.Table.Where(x => x.ContainerKey.ToLower() == containerKey.ToLower()) select containerRepository.CMSContentContainerId).FirstOrDefault();

            if (containerId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidContainerKey);

            return containerId;

        }

        //Binds the dataset table data to the properties in ContainerDataModel
        protected virtual ContentContainerDataModel BindDataSetToContainerDataModel(ContentContainerDataModel contentContainerDataModel, DataSet containerData)
        {
            ConvertDataTableToList dt = new ConvertDataTableToList();
            contentContainerDataModel = containerData?.Tables.Count > 0 ? dt.ConvertDataTable<ContentContainerDataModel>(containerData?.Tables[0]).FirstOrDefault() : new ContentContainerDataModel();

            contentContainerDataModel.Attributes = JsonConvert.DeserializeObject<List<GlobalAttributeValuesModel>>(containerData?.Tables[0].Rows.OfType<DataRow>()
                            .Select(dr => dr.Field<string>("GlobalAttributes")).FirstOrDefault());

            return contentContainerDataModel;
        }

        //Set Isactive/Status filter in filterCollection
        protected virtual void SetIsActiveFilter(FilterCollection filters)
        {
            string status = (filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.Status, StringComparison.InvariantCultureIgnoreCase))?.FilterValue);
            if (!string.IsNullOrEmpty(status)){
                filters.RemoveAll(x => x.Item1.Equals(FilterKeys.Status, StringComparison.InvariantCultureIgnoreCase));

                filters.Add(FilterKeys.Status, FilterOperators.Equals, (status == FilterKeys.ActiveTrueValue) ? FilterKeys.ActiveTrue : FilterKeys.ActiveFalse);
            }

        }
        #endregion
    }
}
