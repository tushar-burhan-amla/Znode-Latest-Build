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
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using Znode.Engine.Services.Helper;
using Znode.Libraries.Caching.Events;

namespace Znode.Engine.Services
{
    public partial class ManageMessageService : BaseService, IManageMessageService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeCMSMessage> _cmsMessageRepository;
        private readonly IZnodeRepository<ZnodeCMSPortalMessage> _cmsPortalMessageRepository;
        private readonly IZnodeRepository<ZnodeCMSMessageKey> _cmsMessageKeyRepository;
        private readonly IZnodeRepository<ZnodeCMSArea> _cmsAreaRepository;
        private readonly IZnodeRepository<ZnodePublishPortalLog> _publishPortalLogRepository;

        #endregion

        #region Constructor
        public ManageMessageService()
        {
            _cmsMessageRepository = new ZnodeRepository<ZnodeCMSMessage>();
            _cmsMessageKeyRepository = new ZnodeRepository<ZnodeCMSMessageKey>();
            _cmsPortalMessageRepository = new ZnodeRepository<ZnodeCMSPortalMessage>();
            _cmsAreaRepository = new ZnodeRepository<ZnodeCMSArea>();
            _publishPortalLogRepository = new ZnodeRepository<ZnodePublishPortalLog>();
        }
        #endregion

        #region Manage Messages
        //Get ManageMessage list.
        public virtual ManageMessageListModel GetManageMessages(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel generated to get manage message list: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            //Get value of localeId from filter.         
            int localeId = Convert.ToInt32(string.IsNullOrEmpty(filters.Find(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower())?.Item3) ? GetDefaultLocaleId().ToString() : filters.Find(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower())?.Item3);
            ZnodeLogging.LogMessage("localeId generated from filter: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, localeId);

            if (!Equals(filters.Find(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower())?.Item1, ZnodeLocaleEnum.LocaleId.ToString().ToLower()))
                filters.Add(new FilterTuple("LocaleId", ProcedureFilterOperators.Equals, DefaultGlobalConfigSettingHelper.Locale));

            filters.RemoveAll(x => x.FilterName.Equals(ZnodeLocaleEnum.LocaleId.ToString(), StringComparison.InvariantCultureIgnoreCase));

            //Bind the Filter conditions for the authorized portal access.
            
            //BindUserPortalFilter(ref filters);
            IZnodeViewRepository<ManageMessageModel> objStoredProc = new ZnodeViewRepository<ManageMessageModel>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);

            IList<ManageMessageModel> manageMessageEntityList = objStoredProc.ExecuteStoredProcedureList("Znode_GetManageMessagelist @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("manageMessageEntityList count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, manageMessageEntityList?.Count);

            ManageMessageListModel manageMessageListModel = new ManageMessageListModel { ManageMessages = manageMessageEntityList?.ToList() };

            manageMessageListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return manageMessageListModel;
        }

        //Create new ManageMessage.
        public virtual ManageMessageModel CreateManageMessage(ManageMessageModel manageMessageModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNull(manageMessageModel))
                throw new ZnodeException(ErrorCodes.NullModel, ZnodeConstant.NullModelError);

            ZnodeLogging.LogMessage("Input parameter manageMessageModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, manageMessageModel);

            //If LocaleId is zero then get default locale.
            if (manageMessageModel.LocaleId == 0)
                manageMessageModel.LocaleId = GetDefaultLocaleId();

            //Get comma seprated string of ids for portal and area.
            string portalIds = string.Join(",", manageMessageModel?.PortalIds);

            IZnodeViewRepository<View_ReturnBooleanWithMessage> objStoredProc = new ZnodeViewRepository<View_ReturnBooleanWithMessage>();
            objStoredProc.SetParameter("PortalIds", portalIds, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodeCMSMessageKeyEnum.MessageKey.ToString(), manageMessageModel.MessageKey, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodeLocaleEnum.LocaleId.ToString(), manageMessageModel.LocaleId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter(ZnodeCMSMessageEnum.CMSMessageId.ToString(), 0, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter(ZnodeCMSMessageKeyEnum.CMSMessageKeyId.ToString(), 0, ParameterDirection.Input, DbType.Int32);

            if (HelperUtility.IsNotNull(manageMessageModel.Message))
                objStoredProc.SetParameter("Description", manageMessageModel.Message, ParameterDirection.Input, DbType.String);
            else
                objStoredProc.SetParameter("Description", DBNull.Value, ParameterDirection.Input, DbType.String);

            if (HelperUtility.IsNotNull(manageMessageModel.MessageTag))
                objStoredProc.SetParameter(ZnodeCMSMessageKeyEnum.MessageTag.ToString(), manageMessageModel.MessageTag, ParameterDirection.Input, DbType.String);
            else
                objStoredProc.SetParameter(ZnodeCMSMessageKeyEnum.MessageTag.ToString(), DBNull.Value, ParameterDirection.Input, DbType.String);

            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);

            View_ReturnBooleanWithMessage output = objStoredProc.ExecuteStoredProcedureList("Znode_InsertManageMessages @PortalIds,@MessageKey,@MessageTag,@Description,@LocaleId,@UserId,@CMSMessageId,@CMSMessageKeyId,@Status OUT")?.FirstOrDefault();

            if (Equals(output?.MessageDetails, "Successful"))
            {
                manageMessageModel.CMSMessageId = output.Id;
                manageMessageModel.CMSMessageKeyId = output.Id;
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return manageMessageModel;
            }
            else if (HelperUtility.IsNotNull(output) && output.MessageDetails.Contains("Is Already Exists"))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.MessageKeyAlreadyExist);
            else
            {
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return manageMessageModel;
            }
        }

        //Get ManageMessage details.
        public virtual ManageMessageModel GetManageMessage(ManageMessageMapperModel manageMessageMapperModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter manageMessageMapperModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, manageMessageMapperModel);

            if (manageMessageMapperModel?.CMSMessageKeyId > 0)
            {
                // Get all details of manage messages associated with PortalId and CMSMessageKey.
                ManageMessageModel manageMessageDetails = GetManageMessageForLocaleId(manageMessageMapperModel);

                if (HelperUtility.IsNotNull(manageMessageDetails))
                    return manageMessageDetails;
                else
                {
                    //if message details are not available for locale id passed then get messages for default localeId.
                    int localeId = manageMessageMapperModel.LocaleId;
                    manageMessageMapperModel.LocaleId = GetDefaultLocaleId();
                    manageMessageDetails = GetManageMessageForLocaleId(manageMessageMapperModel);
                    manageMessageDetails.CMSMessageId = !Equals(localeId, manageMessageMapperModel.LocaleId) ? 0 : manageMessageMapperModel.CMSMessageId;
                    if (HelperUtility.IsNotNull(manageMessageDetails))
                    {
                        ZnodeLogging.LogMessage("manageMessageDetails to be returned: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, manageMessageDetails);
                        return manageMessageDetails;
                    }
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return new ManageMessageModel();
        }

        //Update the ManageMessage.
        public virtual bool UpdateManageMessage(ManageMessageModel manageMessageModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(manageMessageModel))
                throw new ZnodeException(ErrorCodes.NullModel, ZnodeConstant.NullModelError);

            ZnodeLogging.LogMessage("Input parameter manageMessageModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, manageMessageModel);
            //If LocaleId is zero then get default locale.
            if (manageMessageModel.LocaleId == 0)
                manageMessageModel.LocaleId = GetDefaultLocaleId();

            IZnodeViewRepository<View_ReturnBooleanWithMessage> objStoredProc = new ZnodeViewRepository<View_ReturnBooleanWithMessage>();
            objStoredProc.SetParameter("PortalIds", manageMessageModel.PortalId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodeCMSMessageKeyEnum.MessageKey.ToString(), manageMessageModel.MessageKey, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(ZnodeLocaleEnum.LocaleId.ToString(), manageMessageModel.LocaleId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter(ZnodeUserEnum.UserId.ToString(), GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter(ZnodeCMSMessageEnum.CMSMessageId.ToString(), manageMessageModel.CMSMessageId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter(ZnodeCMSMessageKeyEnum.CMSMessageKeyId.ToString(), manageMessageModel.CMSMessageKeyId, ParameterDirection.Input, DbType.Int32);
            if (HelperUtility.IsNotNull(manageMessageModel.MessageTag))
                objStoredProc.SetParameter(ZnodeCMSMessageKeyEnum.MessageTag.ToString(), manageMessageModel.MessageTag, ParameterDirection.Input, DbType.String);
            else
                objStoredProc.SetParameter(ZnodeCMSMessageKeyEnum.MessageTag.ToString(), DBNull.Value, ParameterDirection.Input, DbType.String);

            if (HelperUtility.IsNotNull(manageMessageModel.Message))
                objStoredProc.SetParameter("Description", manageMessageModel.Message, ParameterDirection.Input, DbType.String);
            else
                objStoredProc.SetParameter("Description", DBNull.Value, ParameterDirection.Input, DbType.String);

            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);

            View_ReturnBooleanWithMessage output = objStoredProc.ExecuteStoredProcedureList("Znode_InsertManageMessages @PortalIds,@MessageKey,@MessageTag,@Description,@LocaleId,@UserId,@CMSMessageId,@CMSMessageKeyId,@Status OUT")?.FirstOrDefault();

            if (Equals(output?.MessageDetails, "Successful"))
            {
                var portalIds = manageMessageModel.PortalId.ToString().Split(',');
                foreach (var portalId in portalIds)
                {
                    if (!portalId.Equals("0"))
                        BatchUpdateContentBlockPreviewStatus(manageMessageModel.CMSMessageKeyId.Value, Convert.ToInt32(portalId), manageMessageModel.LocaleId.Value, ZnodePublishStatesEnum.DRAFT, true);
                    else
                    {
                        BatchUpdateContentBlockPreviewStatus(manageMessageModel.CMSMessageKeyId.Value, null, manageMessageModel.LocaleId.Value, ZnodePublishStatesEnum.DRAFT, true);
                    }
                }
                return true;
            }
            else
                return false;
        }

        //Delete ManageMessage.
        public virtual bool DeleteManageMessage(ParameterModel cmsManageMessageId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter cmsManageMessageId to delete manage message: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, cmsManageMessageId?.Ids);

            IZnodeViewRepository<View_ReturnBooleanWithMessage> objStoredProc = new ZnodeViewRepository<View_ReturnBooleanWithMessage>();
            objStoredProc.SetParameter(FilterKeys.MessageIds, cmsManageMessageId.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter(FilterKeys.Status, null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            // The Id field of View_ReturnBooleanWithMessage contains PortalId which is used to check whether the Content Block is created for all stores (global) or just for single store (store specific).
            // Messagekey Id is not required.
            var messages = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteManageMessages @MessageIds, @Status OUT", 1, out status);
            return messages.FirstOrDefault().Status.Value;

        }


        //Publish the manage message
        public virtual PublishedModel PublishManageMessage(string cmsMessageKeyId, int portalId, int localeId = 0, string targetPublishState = null, bool takeFromDraftFirst = false)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters cmsMessageKeyId, portalId, localeId, targetPublishState, takeFromDraftFirst: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { cmsMessageKeyId, portalId, localeId, targetPublishState, takeFromDraftFirst });


            if (string.IsNullOrEmpty(cmsMessageKeyId) || string.IsNullOrWhiteSpace(cmsMessageKeyId))
                throw new ZnodeException(ErrorCodes.InvalidEntityPassedDuringPublish, Api_Resources.InvalidEntityMessageDuringPublish);

            ZnodeLogging.LogMessage("Publish content block with the parameters :", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { cmsMessageKeyId, portalId, portalId, targetPublishState });

            bool isDataPublished = ZnodeDependencyResolver.GetService<IPublishPortalDataService>().PublishManageMessage(cmsMessageKeyId, portalId, localeId, targetPublishState, takeFromDraftFirst);

            if (isDataPublished)
            {

                ClearCacheHelper.EnqueueEviction(new StaleApiRoutesEvent()
                {
                    Comment = $"From message service publishing a message.",
                    RouteTemplateKeys = new[] { CachedKeys.MessageKey_ }
                });

                ClearCacheHelper.EnqueueEviction(new StaleWebStoreKeyEvent()
                {
                    Comment = $"From message service publishing a message.",
                    PortalIds = new int[] { portalId },
                    Key = CachedKeys.MessageKey_
                });

            }

            return new PublishedModel { IsPublished = isDataPublished, ErrorMessage = isDataPublished ? String.Empty : Api_Resources.GenericExceptionMessageDuringPublish };
        }

        #endregion

        #region Private Methods
        //if message details are not available for locale id passed then get messages for default localeId.
        private ManageMessageModel GetManageMessageForLocaleId(ManageMessageMapperModel manageMessageMapperModel)
        {
            IZnodeViewRepository<ManageMessageModel> getManageMessageForEdit = new ZnodeViewRepository<ManageMessageModel>();
            getManageMessageForEdit.SetParameter(ZnodeCMSPortalMessageEnum.PortalId.ToString(), manageMessageMapperModel.PortalId, ParameterDirection.Input, DbType.String);
            getManageMessageForEdit.SetParameter(ZnodeCMSMessageKeyEnum.CMSMessageKeyId.ToString(), manageMessageMapperModel.CMSMessageKeyId, ParameterDirection.Input, DbType.Int32);
            getManageMessageForEdit.SetParameter(ZnodeLocaleEnum.LocaleId.ToString(), manageMessageMapperModel.LocaleId, ParameterDirection.Input, DbType.Int32);
            getManageMessageForEdit.SetParameter(ZnodeCMSMessageEnum.CMSMessageId.ToString(), manageMessageMapperModel.CMSMessageId, ParameterDirection.Input, DbType.String);
            ManageMessageModel manageMessageDetails = getManageMessageForEdit.ExecuteStoredProcedureList("Znode_GetManageMessageForEdit @PortalId, @CMSMessageKeyId,@LocaleId,@CMSMessageId")?.FirstOrDefault();
            return manageMessageDetails;
        }

        [Obsolete("To be discontinued in one of the upcoming versions.")]
        //Update the publish status
        private void UpdatePublishContentBlockStatus(int cmsMessageKeyId, string message, int localeId, bool isPortalPublished)
         => _cmsMessageRepository.Update(new ZnodeCMSMessage
         {
             CMSMessageId = cmsMessageKeyId,
             Message = message,
             LocaleId = localeId,
             IsPublished = isPortalPublished
         });

        private void BatchUpdateContentBlockPreviewStatus(int cmsMessageKeyId, int? portalId, int localeId, ZnodePublishStatesEnum targetPublishState, bool isPortalPublished)
        {
            ZnodeLogging.LogMessage("Input parameters cmsMessageKeyId, portalId, localeId, targetPublishState, isPortalPublished: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { cmsMessageKeyId, portalId, localeId, targetPublishState, isPortalPublished });

            List<ZnodeCMSMessage> messages = (from message in _cmsMessageRepository.Table
                                              join portalMessage in _cmsPortalMessageRepository.Table on message.CMSMessageId equals portalMessage.CMSMessageId
                                              join messagekey in _cmsMessageKeyRepository.Table on portalMessage.CMSMessageKeyId equals messagekey.CMSMessageKeyId
                                              where portalMessage.CMSMessageKeyId == cmsMessageKeyId && portalMessage.PortalId == portalId && message.LocaleId == localeId
                                              select message).ToList();
            ZnodeLogging.LogMessage("messages list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, messages?.Count);

            if (HelperUtility.IsNotNull(messages) && messages.Count > 0)
            {
                messages.ForEach(x => x.PublishStateId = isPortalPublished ? (byte)targetPublishState : (byte)ZnodePublishStatesEnum.DRAFT);
                _cmsMessageRepository.BatchUpdate(messages);
            }
        }
        #endregion
    }
}
