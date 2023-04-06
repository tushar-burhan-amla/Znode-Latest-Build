using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;

using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Helper;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Caching.Events;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Observer;
using Znode.Libraries.Resources;

using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class WebSiteService : BaseService, IWebSiteService
    {
        #region Private Variable
        private readonly IZnodeRepository<ZnodeCMSPortalTheme> _cmsPortalThemeRepository;
        private readonly IZnodeRepository<ZnodeCMSTheme> _cmsThemeRepository;
        private readonly IZnodeRepository<ZnodeCMSPortalProductPage> _cmsPortalProductPageRepository;
        private readonly IZnodeRepository<ZnodeCMSWidget> _widgetRepository;
        private readonly IZnodeRepository<ZnodePimCatalog> _catalogRepository;
        private readonly IZnodeRepository<ZnodePortalCatalog> _portalCatalogRepository;
        private readonly IZnodeRepository<ZnodePortalCustomCss> _portalCustomCssRepository;
        private readonly IZnodeRepository<ZnodeExportProcessLog> _znodeExportProcessLog;
        #endregion

        #region Constructor
        public WebSiteService()
        {
            _cmsPortalThemeRepository = new ZnodeRepository<ZnodeCMSPortalTheme>();
            _cmsThemeRepository = new ZnodeRepository<ZnodeCMSTheme>();
            _cmsPortalProductPageRepository = new ZnodeRepository<ZnodeCMSPortalProductPage>();
            _widgetRepository = new ZnodeRepository<ZnodeCMSWidget>();
            _catalogRepository = new ZnodeRepository<ZnodePimCatalog>();
            _portalCatalogRepository = new ZnodeRepository<ZnodePortalCatalog>();
            _portalCustomCssRepository = new ZnodeRepository<ZnodePortalCustomCss>();
            _znodeExportProcessLog = new ZnodeRepository<ZnodeExportProcessLog>();
        }
        #endregion

        #region Public Methods

        #region Web Site Logo
        //Get the Portal List, for which the Themes are assigned.
        public virtual PortalListModel GetPortalList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameters:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { pageListModel?.ToDebugString() });

            //Method to get locale id from filters.
            int localeId = 0;
            GetLocaleId(filters, ref localeId);

            IZnodeViewRepository<PortalModel> objStoredProc = new ZnodeViewRepository<PortalModel>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_BY", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
            PortalListModel portalListModel = new PortalListModel() { PortalList = objStoredProc.ExecuteStoredProcedureList("Znode_GetCmsWebsiteConfiguration  @WhereClause,@Rows,@PageNo,@Order_BY,@RowsCount OUT,@LocaleId", 4, out pageListModel.TotalRowCount)?.ToList() };
            portalListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return portalListModel;
        }

        //Get Web Site Logo Details by Portal Id.
        public virtual WebSiteLogoModel GetWebSiteLogoDetails(int portalId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (portalId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.PortalNotIdLessThanOne);

            ZnodeLogging.LogMessage("Input parameter portalId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { portalId });

            //Check for User Portal Access.
            CheckUserPortalAccess(portalId);

            WebSiteLogoModel model = new WebSiteLogoModel();
            //Get the Portal Theme Information.
            ZnodeCMSPortalTheme portalTheme = _cmsPortalThemeRepository.Table.FirstOrDefault(x => x.PortalId == portalId);
            if (IsNotNull(portalTheme))
            {
                model = portalTheme.ToModel<WebSiteLogoModel>();
                //Get Theme name from CMSThemeId.               
                FilterCollection filter = new FilterCollection() { new FilterTuple(ZnodeCMSThemeEnum.CMSThemeId.ToString(), FilterOperators.Equals, portalTheme.CMSThemeId.ToString()) };
                ZnodeCMSTheme cmsTheme = _cmsThemeRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause, GetExpandsForParentTheme());
                model.ThemeName = cmsTheme?.Name;
                model.ParentThemeName = cmsTheme?.ZnodeCMSTheme2?.Name;
                ZnodeLogging.LogMessage("MediaId, ThemeName, ParentThemeName :", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { model?.MediaId, model?.ThemeName, model?.ParentThemeName });

                if (model?.MediaId > 0)
                {
                    IMediaManagerServices mediaService = GetService<IMediaManagerServices>();
                    MediaManagerModel mediaData = mediaService.GetMediaByID(Convert.ToInt32(portalTheme.MediaId), null);
                    MediaManagerModel faviconData = model?.FaviconId > 0 ? mediaService.GetMediaByID(Convert.ToInt32(portalTheme.FavIconId), null) : null;
                    model.LogoUrl = IsNotNull(mediaData) ? mediaData.MediaServerThumbnailPath : string.Empty;
                    model.FaviconUrl = IsNotNull(faviconData) ? faviconData?.MediaServerThumbnailPath : string.Empty;
                }
            }
            ZnodePortalCustomCss znodePortalCustomCss = _portalCustomCssRepository.Table.FirstOrDefault(x => x.PortalId == portalId);
            model.DynamicContent = znodePortalCustomCss.ToModel<DynamicContentModel>();

            IZnodeRepository<ZnodePortal> _portalRepository = new ZnodeRepository<ZnodePortal>();
            //Get the Portal Information.
            ZnodePortal portal = _portalRepository.Table.FirstOrDefault(x => x.PortalId == portalId);
            if (IsNotNull(portal))
                model.PortalName = portal.StoreName;
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return model;
        }

        //Save the WebSite Logo Details
        public virtual bool SaveWebSiteLogo(WebSiteLogoModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);

            ZnodeLogging.LogMessage("WebSiteLogoModel with CMSThemeId  :", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { model?.CMSThemeId });

            bool status = false;
            if (model.CMSThemeId > 0)
            {
                //Save the Web Site Logo information.
                status = _cmsPortalThemeRepository.Update(model.ToEntity<ZnodeCMSPortalTheme>());
                if (status)
                    ZnodeLogging.LogMessage(Admin_Resources.SuccessWebSiteLogoSave, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                else
                {
                    throw new ZnodeException(ErrorCodes.ExceptionalError, Admin_Resources.ErrorWhileWebSiteLogoDetailsSave);
                }
            }
            model.DynamicContent.PortalId = model.PortalId;

            SaveUpdateDynamicContent(model.DynamicContent);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return status;
        }

        protected virtual DynamicContentModel SaveUpdateDynamicContent(DynamicContentModel dynamicContentModel)
        {
            ZnodeLogging.LogMessage("SaveDynamicContent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (IsNull(dynamicContentModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorDynamicContentModelNull);

            ZnodePortalCustomCss znodePortalCustomCss = _portalCustomCssRepository.Table.FirstOrDefault(x => x.PortalId == dynamicContentModel.PortalId);

            if (IsNotNull(znodePortalCustomCss))
            {
                dynamicContentModel.PortalCustomCssId = znodePortalCustomCss.PortalCustomCssId;
                _portalCustomCssRepository.Update(dynamicContentModel.ToEntity<ZnodePortalCustomCss>());
                return dynamicContentModel;
            }
            else
            {
                ZnodePortalCustomCss savePortalCustomCssEntity = null;
                if (IsNotNull(dynamicContentModel.DynamicCssStyle) || IsNotNull(dynamicContentModel.WYSIWYGFormatStyle))
                    savePortalCustomCssEntity = _portalCustomCssRepository.Insert(dynamicContentModel.ToEntity<ZnodePortalCustomCss>());
                return IsNotNull(savePortalCustomCssEntity) ? savePortalCustomCssEntity.ToModel<DynamicContentModel>() : dynamicContentModel;
            }
        }

        #endregion

        #region Portal Product Page
        //Get the list of portal page product associated to selected store in website configuration.
        public virtual PortalProductPageModel GetPortalProductPageList(int portalId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter portalId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { portalId });

            //Check for User Portal Access.
            CheckUserPortalAccess(portalId);

            List<ZnodeCMSPortalProductPage> portalProductPageList = (from list in _cmsPortalProductPageRepository.Table
                                                                     where list.PortalId == portalId
                                                                     select list)?.OrderBy(x => x.ProductType)?.ToList();

            ZnodeLogging.LogMessage("portalProductPageList count :", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { portalProductPageList?.Count });

            PortalProductPageModel model = new PortalProductPageModel();
            if (portalProductPageList?.Count > 0)
            {
                model.PortalProductPageList = new List<PortalProductPageModel>();
                foreach (ZnodeCMSPortalProductPage portalProductPage in portalProductPageList)
                    model.PortalProductPageList.Add(new PortalProductPageModel { PortalId = portalId, ProductType = portalProductPage.ProductType, TemplateName = portalProductPage.TemplateName });
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return model;
        }


        //Assign new pdp template to product type.
        public virtual bool UpdatePortalProductPage(PortalProductPageModel portalProductPageModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            bool isUpdated = false;
            if (IsNull(portalProductPageModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);

            if (portalProductPageModel.PortalId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.PortalNotIdLessThanOne);

            ZnodeLogging.LogMessage("portalProductPageModel with PortalId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { portalProductPageModel?.PortalId });

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeCMSPortalProductPageEnum.PortalId.ToString(), FilterOperators.Equals, portalProductPageModel.PortalId.ToString()));

            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClause(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClause for delete:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { whereClause });

            //Check if portal product page details already exists for current portal id.
            if (IsProductPageDetailsAvailable(portalProductPageModel.PortalId) && !_cmsPortalProductPageRepository.Delete(whereClause))
                throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorDataAssociatedToPortalProductPageDelete);
            List<ZnodeCMSPortalProductPage> productPageList = new List<ZnodeCMSPortalProductPage>();

            for (int iLength = 0; iLength < portalProductPageModel.ProductTypeList.Count; iLength++)
                //Bind the newly updated date to entity list object.
                productPageList.Add(new ZnodeCMSPortalProductPage { PortalId = portalProductPageModel.PortalId, ProductType = portalProductPageModel.ProductTypeList[iLength], TemplateName = Convert.ToString(portalProductPageModel.TemplateNameList[iLength]) });

            //Insert new data. 
            IList<ZnodeCMSPortalProductPage> portalProductPageList = _cmsPortalProductPageRepository.Insert(productPageList)?.ToList();
            ZnodeLogging.LogMessage("portalProductPageList count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { portalProductPageList.Count });

            isUpdated = portalProductPageList?.FirstOrDefault().CMSPortalProductPageId > 0;
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return isUpdated;
        }
        #endregion

        #region Publish CMS



        public virtual bool Publish(int portalId, string targetPublishState = null, string publishContent = null, bool takeFromDraftFirst = false)
        {
            ZnodeLogging.LogMessage("Input parameters portalId, targetPublishState :", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { portalId, targetPublishState });

            Guid jobId = Guid.NewGuid();
            bool isDataPublished = GetService<IPublishPortalDataService>().PublishPortal(portalId, jobId, targetPublishState, publishContent, takeFromDraftFirst);



            if (isDataPublished)
            {
                GetService<ICMSPageSearchService>().CreateIndexForPortalCMSPages(portalId, Convert.ToString(targetPublishState), true);
                //Clear Cache of portal after store publish.
                ClearCacheHelper.EnqueueEviction(new PortalPublishEvent()
                {
                    Comment = $"From website service publishing of portal with id '{portalId}'.",
                    PortalIds = new int[] { portalId }
                });
                var znodeEventNotifierCloudflare = new ZnodeEventNotifier<CloudflarePurgeModel>(new CloudflarePurgeModel() { PortalId = new List<int>(portalId) });

            }
            else
                throw new ZnodeException(ErrorCodes.SQLExceptionDuringPublish, Api_Resources.SQLExceptionMessageDuringPublish);


            return isDataPublished;

        }



        public virtual bool PublishAsync(int portalId, string targetPublishState = null, string publishContent = null, bool takeFromDraftFirst = false)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage("Publish portal with the parameters :", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { portalId, targetPublishState, publishContent });

            if (IsExportPublishInProgress())
            {
                throw new ZnodeException(ErrorCodes.NotPermitted, PIM_Resources.ErrorPublishCatalog);
            }

            Guid jobId = Guid.NewGuid();

            ZnodeException _znodeException = null;
            HttpContext httpContext = HttpContext.Current;

            Thread thread = new Thread(new ThreadStart(() =>
            {

                HttpContext.Current = httpContext;

                bool isDataPublished = GetService<IPublishPortalDataService>().PublishPortal(portalId, jobId, targetPublishState, publishContent, takeFromDraftFirst);

                if (isDataPublished)
                {
                    try
                    {
                        GetService<ICMSPageSearchService>().CreateIndexForPortalCMSPages(portalId, Convert.ToString(targetPublishState), true, publishContent);
                        ClearCacheHelper.EnqueueEviction(new PortalPublishEvent()
                        {
                            Comment = "From website service publish async.",
                            PortalIds = new int[] { portalId }
                        });
                        var znodeEventNotifierCloudflare = new ZnodeEventNotifier<CloudflarePurgeModel>(new CloudflarePurgeModel() { PortalId = new List<int>(portalId) });

                        PreviewHelper.LogProgressNotification(jobId, 100, true, false);
                    }
                    catch(Exception ex)
                    {
                        ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                    }
                }
                else
                {
                    _znodeException =  new ZnodeException(ErrorCodes.SQLExceptionDuringPublish, Api_Resources.SQLExceptionMessageDuringPublish);
                }
            }));

            thread.Start();

            thread.Join();

            if (_znodeException != null)
            {
                throw _znodeException;
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return true;
        }

        #endregion

        //Get the widget id by its code.
        public virtual int GetWidgetIdByCode(string widgetCode)
        => _widgetRepository.Table.FirstOrDefault(x => x.Code == widgetCode).CMSWidgetsId;

        public virtual int GetAssociatedCatalogId(int portalId)
        {
            return (from portalCatalog in _portalCatalogRepository.Table
                    where portalCatalog.PortalId == portalId
                    select portalCatalog.PublishCatalogId).FirstOrDefault();
        }

        #endregion

        #region Private Methods 
        //Check if portal product page details already exists for current portal id.
        private bool IsProductPageDetailsAvailable(int portalId)
         => _cmsPortalProductPageRepository.Table.Any(x => x.PortalId == portalId);


        //Method to get locale id from filters.
        private void GetLocaleId(FilterCollection filters, ref int localeId)
        {
            if (filters?.Count > 0)
            {
                localeId = Convert.ToInt32(filters.FirstOrDefault(x => x.FilterName == FilterKeys.LocaleId.ToLower()).FilterValue);
                filters.RemoveAll(x => x.Item1 == ZnodeLocaleEnum.LocaleId.ToString().ToLower());
            }
        }

        //Method to get expands for parent theme navigation property in ZnodeCMSTheme.
        private List<string> GetExpandsForParentTheme()
        {
            List<string> navigationProperties = new List<string>();
            SetExpands(ZnodeCMSThemeEnum.ZnodeCMSTheme2.ToString(), navigationProperties);
            return navigationProperties;
        }

        #endregion

        protected virtual bool IsExportPublishInProgress()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            byte publishStateId = Convert.ToByte(ZnodePublishStatesEnum.PROCESSING);

            bool isExportPublishInProgress = _znodeExportProcessLog.Table.Any(x => x.Status == ZnodeConstant.ExportStatusInprogress || x.Status == ZnodeConstant.SearchIndexStartedStatus);


            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);

            return isExportPublishInProgress;
        }
    }
}
