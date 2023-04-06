using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Engine.Services.Helper;
using Znode.Libraries.Caching.Events;
 using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public partial class ContentPageService : BaseService, IContentPageService
    {
        #region Private Variable
        private readonly IZnodeRepository<ZnodeCMSContentPage> _contentPageRepository;
        private readonly IZnodeRepository<ZnodeCMSContentPageGroup> _contentPageGroup;
        private readonly IZnodeRepository<ZnodeCMSContentPageGroupLocale> _contentPageGroupLocale;
        private readonly IZnodeRepository<ZnodeCMSSEODetail> _cmsSeoDetailRepository;
        private readonly IZnodeRepository<ZnodeCMSSEOType> _cmsSeoTypeRepository;
        private readonly IZnodeRepository<ZnodeCMSSEODetailLocale> _cmsSeoDetailLocaleRepository;
        private readonly IZnodeRepository<View_GetContentPageDetails> _viewGetContentPageDetailsRepository;
        private readonly IZnodeRepository<ZnodeCMSContentPageGroupMapping> _contentPageGroupMapping;
        private readonly IZnodeRepository<ZnodePublishPortalLog> _publishPortalLogRepository;
        private readonly IZnodeRepository<ZnodeCMSContentPagesLocale> _contentPagesLocaleRepository;
        private readonly IZnodeRepository<ZnodeCMSWidgetProduct> _cmsWidgetProductRepository;
        private readonly IZnodeRepository<ZnodeCMSPortalSEOSetting> _portalSEOSettingRepository;




        #endregion

        #region Constructor
        public ContentPageService()
        {
            _contentPageRepository = new ZnodeRepository<ZnodeCMSContentPage>();
            _contentPageGroup = new ZnodeRepository<ZnodeCMSContentPageGroup>();
            _contentPageGroupLocale = new ZnodeRepository<ZnodeCMSContentPageGroupLocale>();
            _cmsSeoDetailRepository = new ZnodeRepository<ZnodeCMSSEODetail>();
            _cmsSeoTypeRepository = new ZnodeRepository<ZnodeCMSSEOType>();
            _cmsSeoDetailLocaleRepository = new ZnodeRepository<ZnodeCMSSEODetailLocale>();
            _viewGetContentPageDetailsRepository = new ZnodeRepository<View_GetContentPageDetails>();
            _contentPageGroupMapping = new ZnodeRepository<ZnodeCMSContentPageGroupMapping>();
            _publishPortalLogRepository = new ZnodeRepository<ZnodePublishPortalLog>();
            _contentPagesLocaleRepository = new ZnodeRepository<ZnodeCMSContentPagesLocale>();
             _cmsWidgetProductRepository = new ZnodeRepository<ZnodeCMSWidgetProduct>();
            _portalSEOSettingRepository = new ZnodeRepository<ZnodeCMSPortalSEOSetting>();

        }
        #endregion

        #region Public Methods
        #region Content Page

        //Create Content page.
        public virtual ContentPageModel CreateContentPage(ContentPageModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel,Admin_Resources.ModelCanNotBeNull);

            //Checks if the content page name already exist for current portal id.
            if (IsPageNameAvailable(model.PageName, model.PortalId))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorPageNameExistsInDatabase);

            ZnodeLogging.LogMessage("ContentPageModel with PageName and PortalId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { model?.PageName, model?.PortalId });

            //Check if seo name already exists.
            if (IsSeoNameAvailable(model))
                throw new ZnodeException(ErrorCodes.SEOUrlAlreadyExists, Admin_Resources.ErrorSEONameExistsInDatabase);

            IZnodeViewRepository<ContentPageModel> objStoredProc = new ZnodeViewRepository<ContentPageModel>();
            objStoredProc.SetParameter("ContentPageXML", ToXML(model), ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateContentPage @ContentPageXML, @UserId, @Status OUT", 2, out status)?.FirstOrDefault();

        }

        //Get list of content pages.
        public virtual ContentPageListModel GetContentPageList(FilterCollection filters, NameValueCollection expands, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            ReplaceSortKeys(ref sorts);

            string localeId = filters.Find(x => string.Equals(x.FilterName, ZnodeCMSContentPageGroupLocaleEnum.LocaleId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3;
            filters.RemoveAll(x => x.FilterName == FilterKeys.LocaleId);

            localeId = string.IsNullOrEmpty(localeId) ? "0" : localeId;

            //Bind the Filter conditions for the authorized portal access.
            BindUserPortalFilter(ref filters);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString()});

            IZnodeViewRepository<ContentPageModel> objStoredProc = new ZnodeViewRepository<ContentPageModel>();
            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", null, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);

            ContentPageListModel listModel = new ContentPageListModel();
            //SP Call
            List<ContentPageModel> contentPageList = objStoredProc.ExecuteStoredProcedureList("Znode_GetCMSContentPagesFolderDetails @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId", 4, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("contentPageList count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { contentPageList?.Count });

            listModel.ContentPageList = contentPageList?.Count > 0 ? contentPageList : new List<ContentPageModel>();

            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Get Content Page by Static Page Id.
        public virtual ContentPageModel GetContentPage(FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            int contentPageID = GetContentPageId(filters);
            ZnodeLogging.LogMessage("contentPageID returned from GetContentPageId method:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { contentPageID });

            if (contentPageID < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorContentPageIdLessThan1);

            int localeId;
            Int32.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(FilterKeys.LocaleId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out localeId);

            //Get all the information related to content page.
            ContentPageModel contentPageModel = GetContentPageInformation(contentPageID, localeId);
            ZnodeLogging.LogMessage("contentPageModel with content page ID returned from GetContentPageInformation:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { contentPageModel?.CMSContentPagesId });

            if (IsNotNull(contentPageModel))
            {
                //Set the template information.
                SetTemplateInformation(contentPageModel);

                IZnodeRepository<ZnodeCMSContentPagesProfile> _cmsContentPagesProfile = new ZnodeRepository<ZnodeCMSContentPagesProfile>();
                contentPageModel.ProfileIds = string.Join(",", _cmsContentPagesProfile?.Table?.Where(x => x.CMSContentPagesId == contentPageID)?.Select(x => x.ProfileId));
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return contentPageModel;
            }
            throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidData);
        }

        //Update Content Page.
        public virtual bool UpdateContentPage(ContentPageModel contentPageModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (IsNull(contentPageModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelCanNotBeNull);

            if (contentPageModel.CMSContentPagesId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorContentPageIdLessThan1);

            ZnodeLogging.LogMessage("contentPageModel with content page ID :", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { contentPageModel?.CMSContentPagesId });

            string contentPageXML = ToXML(contentPageModel);

            IZnodeViewRepository<ContentPageModel> objStoredProc = new ZnodeViewRepository<ContentPageModel>();
            objStoredProc.SetParameter("ContentPageXML", contentPageXML, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("UserId", GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateContentPage @ContentPageXML, @UserId, @Status OUT", 2, out status);

            if (!Equals(contentPageModel.SEOUrl, contentPageModel.OldSEOURL))
            {
                SEODetailsModel seoDetailsModel = new SEODetailsModel() { SEOUrl = contentPageModel.SEOUrl, OldSEOURL = contentPageModel.OldSEOURL, CMSContentPagesId = contentPageModel.CMSContentPagesId, PortalId = contentPageModel.PortalId, CMSSEOTypeId = 3, IsRedirect = contentPageModel.IsRedirect };
                SEORedirectUrlHelper.CreateUrlRedirect(seoDetailsModel);
            }
            UpdateContentPageAfterPublish(contentPageModel.CMSContentPagesId, 0, false);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return status > 0;
        }

        //Delete Content Page.
        public virtual bool DeleteContentPage(ParameterModel contentPageIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (IsNull(contentPageIds) || string.IsNullOrEmpty(contentPageIds.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorContentPageIdLessThan1);

            ZnodeLogging.LogMessage("contentPageIds to be deleted :", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { contentPageIds?.Ids });

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();

            //SP paramters.
            objStoredProc.SetParameter(ZnodeCMSContentPageEnum.CMSContentPagesId.ToString(), contentPageIds.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            int status = 0;
            objStoredProc.ExecuteStoredProcedureList("Znode_DeleteContentPage @CMSContentPagesId,  @Status OUT", 1, out status);
            if (status == 1)
            {
                ZnodeLogging.LogMessage(String.Format(Admin_Resources.SuccessContentPageDelete,contentPageIds.Ids), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, String.Format(Admin_Resources.ErrorContentPageDelete, contentPageIds.Ids));
            }
        }



        public virtual PublishedModel PublishContentPage(int contentPageId, int portalId, int localeId = 0, string targetPublishState = null, bool takeFromDraftFirst = false)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Publish content page with the parameters :", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { contentPageId, portalId, portalId, targetPublishState });

            if (contentPageId < 1)
                throw new ZnodeException(ErrorCodes.InvalidEntityPassedDuringPublish, Api_Resources.InvalidEntityMessageDuringPublish);

            if (portalId <= 0)
            {
                ZnodeCMSContentPage contentPage = _contentPageRepository.Table.FirstOrDefault(o => o.CMSContentPagesId == contentPageId);
                if (contentPage != null)
                {
                    portalId = contentPage.PortalId;
                }
            }

            bool isDataPublished = GetService<IPublishPortalDataService>().PublishContentPage(contentPageId, portalId, localeId, targetPublishState, takeFromDraftFirst);
      
            if(isDataPublished)
            {
                GetService<ICMSPageSearchService>().CreateIndexForPortalCMSPages(portalId, Convert.ToString(targetPublishState));
                ClearCacheHelper.EnqueueEviction(new PortalPublishEvent()
                {
                    Comment = $"From content page publishing.",
                    PortalIds = new int[] { portalId }
                });

                //Clear API cache of 301RedirectUrl
                ClearCacheHelper.EnqueueEviction(new StaleApiRoutesEvent()
                {
                    Comment = $"From content page publishing.",
                    RouteTemplateKeys = new[]
                    {
                        CachedKeys.geturlredirectlist
                    }
                });

                //Clear Web Store cache of 301RedirectUrl
                ClearCacheHelper.EnqueueEviction(new StaleWebStoreKeyEvent()
                {
                    Comment = $"From content page publishing.",
                    PortalIds = new int[] { portalId },
                    Key = CachedKeys.geturlredirects
                });
            }

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return new PublishedModel { IsPublished = isDataPublished, ErrorMessage = isDataPublished ? String.Empty : Api_Resources.GenericExceptionMessageDuringPublish };
        }

        #endregion

        #region Tree
        // Gets content page tree.
        public virtual ContentPageTreeModel GetTreeNode()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            IZnodeViewRepository<View_CMSContentFolder> objStoredProc = new ZnodeViewRepository<View_CMSContentFolder>();

            //Get all path from database.
            List<ContentPageTreeModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetCMSContentFolder")?.Select(x => new ContentPageTreeModel
            {
                Text = x.Name,
                Id = x.CMSContentPageGroupId,
                ParentId = x.ParentCMSContentPageGroupId.GetValueOrDefault()
            }).ToList();

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            //Convert path to parent child pattern.
            return GetAllNode(list).FirstOrDefault();
        }

        // Add new folder.
        public virtual bool AddFolder(ContentPageFolderModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorContentPageFolderModelNull);

            ZnodeCMSContentPageGroup entity = _contentPageGroup.Insert(model.ToEntity<ZnodeCMSContentPageGroup>());
            ZnodeLogging.LogMessage("Inserted ContentPageFolderModel with CMSContentPageGroupId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { entity?.CMSContentPageGroupId });

            if (entity.CMSContentPageGroupId > 0)
            {
                model.CMSContentPageGroupId = entity.CMSContentPageGroupId;
                if (model.LocaleId == 0)
                    model.LocaleId = GetDefaultLocaleId();
                return _contentPageGroupLocale.Insert(model.ToEntity<ZnodeCMSContentPageGroupLocale>())?.CMSContentPageGroupLocaleId > 0;
            }
            return false;
        }

        // Rename the existing folder.
        public virtual bool RenameFolder(ContentPageFolderModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorContentPageFolderModelNull);

            if (model.CMSContentPageGroupId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData,Admin_Resources.ErrorContentPageIdLessThan1);

            ZnodeLogging.LogMessage("ContentPageFolderModel with CMSContentPageGroupId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { model?.CMSContentPageGroupId });

            ZnodeCMSContentPageGroupLocale groupLocale = _contentPageGroupLocale.Table.Where(x => x.CMSContentPageGroupId == model.CMSContentPageGroupId)?.FirstOrDefault();

            if (IsNotNull(groupLocale))
            {
                groupLocale.Name = model.Code;
                _contentPageGroupLocale.Update(groupLocale);
                return true;
            }
            return false;
        }

        // Delete the existing folders.
        public virtual bool DeleteFolder(ParameterModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Ids to be deleted:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { model?.Ids });

            if (string.IsNullOrEmpty(model?.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorContentPageGroupIdLessThan1);

            int status = 0;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodeCMSContentPageGroupEnum.CMSContentPageGroupId.ToString(), model.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteContentPageGroups @CMSContentPageGroupId, @Status OUT", 1, out status);
            ZnodeLogging.LogMessage("deleteResult count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { deleteResult?.Count });

            if (deleteResult.FirstOrDefault().Status.Value)
            {
                ZnodeLogging.LogMessage(Admin_Resources.SuccessContentPageGroupDelete, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorContentPageGroupDelete, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return false;
            }
        }

        //Move Content Page folder.
        public virtual bool MoveContentPagesFolder(ContentPageFolderModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorContentPageFolderModelNull);

            ZnodeLogging.LogMessage("ContentPageFolderModel with CMSContentPageGroupId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { model?.CMSContentPageGroupId });

            bool result = false;
            ZnodeCMSContentPageGroup znodeCMSContentPageGroup = _contentPageGroup.Table.Where(x => x.CMSContentPageGroupId == model.CMSContentPageGroupId)?.FirstOrDefault();
            if(HelperUtility.IsNotNull(znodeCMSContentPageGroup))
            {
                znodeCMSContentPageGroup.ParentCMSContentPageGroupId = model.ParentCMSContentPageGroupId;
                result = _contentPageGroup.Update(znodeCMSContentPageGroup);
            }
            ZnodeLogging.LogMessage(result ? Admin_Resources.SuccessMoveFolder : Admin_Resources.ErrorMoveFolder, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return result;
        }

        //Move pages from one folder to another.
        public virtual bool MovePageToFolder(AddPageToFolderModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorAddPageToFolderModelNull);
            
                //Generate where clause for pages to be moved.
                string whereClauseForPage = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(GetWhereClauseForPageIds(model.PageIds)).WhereClause;
                 ZnodeLogging.LogMessage("Where clause generated for pages to be moved:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { whereClauseForPage });

                //Get list of pages of those page ids.
                List<ZnodeCMSContentPageGroupMapping> pageList = _contentPageGroupMapping.GetEntityList(whereClauseForPage, new List<string>() { "ZnodeCMSContentPage" }).ToList();
                if (IsNull(pageList) || pageList.FindAll(m => m.CMSContentPageGroupId == model.FolderId)?.Count > 0)
                    return false;

                bool result = true;
                string whereClauseForPagePathId = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(GetWhereClauseForPagePathId(model.FolderId)).WhereClause;

                //Get all pages within the folder.
                IList<ZnodeCMSContentPageGroupMapping> pageListInFolder = _contentPageGroupMapping.GetEntityList(whereClauseForPagePathId, new List<string>() { "ZnodeCMSContentPage" });
                ZnodeLogging.LogMessage("Count of pages within the folder :", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { pageListInFolder?.Count });

                 List<string> pageIdsToRemove = new List<string>();
                 ZnodeLogging.LogMessage("pageIdsToRemove count : ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { pageIdsToRemove?.Count });

                //Check if that page is already present in that folder.
                foreach (ZnodeCMSContentPageGroupMapping page in pageList)
                {
                    if (IsNotNull(pageListInFolder.FirstOrDefault(x => x.ZnodeCMSContentPage.PageName == page.ZnodeCMSContentPage.PageName)))
                        pageIdsToRemove.Add(Convert.ToString(page.CMSContentPagesId));
                }

                //Remove pages from that folder and move to assigned folder.
                if (pageIdsToRemove.Count > 0)
                {
                    result = false;
                    List<string> allPageIds = model.PageIds.Split(',').ToList<string>();
                    if (allPageIds.Count == 1 && pageIdsToRemove.Count == 1)
                        return false;
                    else
                        whereClauseForPage = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(GetWhereClauseForPageIds(string.Join(",", allPageIds.Except(pageIdsToRemove).ToList()))).WhereClause;
                    pageList = _contentPageGroupMapping.GetEntityList(whereClauseForPage).ToList();
                }

                pageList.ForEach(x => x.CMSContentPageGroupId = model.FolderId);
                pageList.ForEach(x => _contentPageGroupMapping.Update(x));
                ZnodeLogging.LogMessage(Admin_Resources.SuccessPagesMovedFolder, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return result;
            

        }
        #endregion
        #endregion

        #region Private Methods
                   
        private void UpdateContentPageAfterPublish(int contentPageId, ZnodePublishStatesEnum publishState, bool status)
        {
            var entity = _contentPageRepository.GetById(contentPageId);
            entity.PublishStateId = status ? (byte)publishState : (byte)ZnodePublishStatesEnum.DRAFT;
            _contentPageRepository.Update(entity);
        }


        //Get WhereClause For Media Path Id.
        private FilterDataCollection GetWhereClauseForPagePathId(int folderId)
        {
            FilterCollection filterList = new FilterCollection();
            FilterTuple filterTuple = new FilterTuple(ZnodeCMSContentPageGroupEnum.CMSContentPageGroupId.ToString(), ProcedureFilterOperators.Equals, folderId.ToString());
            filterList.Add(filterTuple);
            return filterList.ToFilterDataCollection();
        }

        //Get WhereClause For MediaIds.
        private FilterDataCollection GetWhereClauseForPageIds(string pageIds)
        {
            FilterCollection filterList = new FilterCollection();
            FilterTuple filterTuple = new FilterTuple(ZnodeCMSContentPageEnum.CMSContentPagesId.ToString(), ProcedureFilterOperators.In, pageIds);
            filterList.Add(filterTuple);
            return filterList.ToFilterDataCollection();
        }

        //Get the tree with its child.
        private List<ContentPageTreeModel> GetAllNode(List<ContentPageTreeModel> mediaPath)
        {
            if (IsNotNull(mediaPath))
            {
                foreach (ContentPageTreeModel item in mediaPath)
                {
                    //find all chid folder and add to list
                    List<ContentPageTreeModel> child = mediaPath.Where(x => x.ParentId == item.Id).ToList();
                    item.Children = new List<ContentPageTreeModel>();
                    item.Children.AddRange(GetAllNode(child));
                }
                ZnodeLogging.LogMessage("mediaPath:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { mediaPath });

                return mediaPath;
            }
            return new List<ContentPageTreeModel>();
        }

        //Check if content page name already exists for current portal id.
        private bool IsPageNameAvailable(string pageName, int portalId)
         => _contentPageRepository.Table.Any(x => x.PageName == pageName.Trim() && x.PortalId == portalId);

        //Check if seo name already exists.
        private bool IsSeoNameAvailable(ContentPageModel model)
        {
            if (!string.IsNullOrEmpty(model?.SEOUrl))
            {
                //Linq for checking the SEO url is present in local table.
                return Convert.ToBoolean((from seoDetail in _cmsSeoDetailRepository.Table
                                          join cmsSeoDetailsLocale in _cmsSeoDetailLocaleRepository.Table on seoDetail.CMSSEODetailId equals cmsSeoDetailsLocale.CMSSEODetailId
                                          where seoDetail.CMSSEOTypeId == (int)SEODetailsEnum.Content_Page && cmsSeoDetailsLocale.LocaleId == model.LocaleId
                                          select new { seoDetail.SEOUrl, seoDetail.PortalId }
                           )?.Any(a => a.SEOUrl == model.SEOUrl.Trim() && a.PortalId == model.PortalId));
            }
            return false;
        }

        //Get all the information related to content page.
        private ContentPageModel GetContentPageInformation(int contentPageId, int localeId)
        {
            ContentPageModel ContentPageModel = new ContentPageModel();
            //Get the information related to content page according to contentPageId and LocaleId.
            ContentPageModel = GetContentPageInformationForLocaleId(contentPageId, localeId);

            return IsNull(ContentPageModel) ? GetContentPageInformationForLocaleId(contentPageId, GetDefaultLocaleId())
                                            : ContentPageModel;
        }

        //Get all the information related to content page from view.
        private ContentPageModel GetContentPageInformationForLocaleId(int contentPageId, int localeId)
        => (_viewGetContentPageDetailsRepository.Table.Where(x => x.CMSContentPagesId == contentPageId && x.LocaleId == localeId))?.FirstOrDefault()?.ToModel<ContentPageModel>();

        //Get  template model by templateId.
        private TemplateModel GetTemplateInformation(int templateId)
        {
            IZnodeRepository<ZnodeCMSTemplate> _znodeCMSTemplate = new ZnodeRepository<ZnodeCMSTemplate>();
            return _znodeCMSTemplate.Table.Where(template => template.CMSTemplateId == templateId)?.FirstOrDefault()?.ToModel<TemplateModel>();
        }

        //Replace the key name of sort to get sorted data  
        private void ReplaceSortKeys(ref NameValueCollection sorts)
        {
            for (int index = 0; index < sorts.Keys.Count; index++)
            {
                if (sorts.Keys.Get(index) == FilterKeys.ItemName) { ReplaceSortKeyName(ref sorts, FilterKeys.ItemName, ZnodeCMSContentPageEnum.PageName.ToString().ToLower()); }
                if (sorts.Keys.Get(index) == FilterKeys.ItemId) { ReplaceSortKeyName(ref sorts, FilterKeys.ItemId, ZnodeCMSContentPageEnum.CMSContentPagesId.ToString().ToLower()); }
            }
        }

        //Set the template information.
        private void SetTemplateInformation(ContentPageModel contentPageModel)
        {
            TemplateModel templateModel = GetTemplateInformation(Convert.ToInt32(contentPageModel.CMSTemplateId));
            if (IsNotNull(templateModel))
            {
                contentPageModel.PageTemplateName = templateModel.Name;
                contentPageModel.PageTemplateFileName = templateModel.FileName;
            }
        }

        //Get the contentPageId from filters.
        private static int GetContentPageId(FilterCollection filters)
        {
            int contentPageId = 0;
            if (filters?.Count > 0)
                Int32.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(ZnodeCMSContentPageEnum.CMSContentPagesId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out contentPageId);
            return contentPageId;
        }

     
        #endregion
    }
}
