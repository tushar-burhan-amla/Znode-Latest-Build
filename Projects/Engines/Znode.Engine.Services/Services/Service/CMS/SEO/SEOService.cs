using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
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
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Utilities = Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public partial class SEOService : BaseService, ISEOService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeCMSPortalSEOSetting> _portalSEOSettingRepository;
        private readonly IZnodeRepository<ZnodeCMSSEODetail> _seoDetailRepository;
        private readonly IZnodeRepository<ZnodeCMSSEODetailLocale> _seoDetailLocaleRepository;
        private readonly IZnodeRepository<ZnodeCMSSEOType> _seoTypeRepository;
        private readonly IZnodeRepository<ZnodePublishPortalLog> _publishPortalLogRepository;
        private readonly IZnodeRepository<ZnodePortalCatalog> _portalCatalogRepository;  
        private readonly ProductAssociationHelper productAssociationHelper;
        #endregion

        #region Public Constructor
        public SEOService()
        {
            _portalSEOSettingRepository = new ZnodeRepository<ZnodeCMSPortalSEOSetting>();
            _seoDetailRepository = new ZnodeRepository<ZnodeCMSSEODetail>();
            _seoTypeRepository = new ZnodeRepository<ZnodeCMSSEOType>();
            _seoDetailLocaleRepository = new ZnodeRepository<ZnodeCMSSEODetailLocale>();
            _publishPortalLogRepository = new ZnodeRepository<ZnodePublishPortalLog>();
            _portalCatalogRepository = new ZnodeRepository<ZnodePortalCatalog>();
            productAssociationHelper = new ProductAssociationHelper();
        }
        #endregion

        #region Public Methods

        #region Default SEO Settings
        //Get the portal seo setting.
        public virtual PortalSEOSettingModel GetPortalSEOSetting(int portalId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (portalId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.PortalIdNotLessThanOne);

            FilterCollection filters = new FilterCollection { new FilterTuple(ZnodeCMSPortalSEOSettingEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()) };

            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            ZnodeLogging.LogMessage("whereClauseModel generated: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, whereClauseModel);
            ZnodeCMSPortalSEOSetting portalSeoSetting = _portalSEOSettingRepository.GetEntity(whereClauseModel.WhereClause);

            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return portalSeoSetting?.ToModel<PortalSEOSettingModel>();
        }

        //Create portal seo setting.
        public virtual PortalSEOSettingModel CreatePortalSEOSetting(PortalSEOSettingModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorPortalSEOModelNull);

            ZnodeCMSPortalSEOSetting portalSeoSetting = _portalSEOSettingRepository.Insert(model.ToEntity<ZnodeCMSPortalSEOSetting>());
            ZnodeLogging.LogMessage("Inserted PortalSEOSetting with PortalId: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, portalSeoSetting.PortalId);
            if (portalSeoSetting.CMSPortalSEOSettingId > 0)
            {
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessPortalSEOSettingInsert, portalSeoSetting.CMSPortalSEOSettingId), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return portalSeoSetting.ToModel<PortalSEOSettingModel>();
            }
            ZnodeLogging.LogMessage(string.Format(Admin_Resources.ErrorPortalSEOSettingInsert, portalSeoSetting.CMSPortalSEOSettingId), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return null;
        }

        //Update portal seo setting.
        public virtual bool UpdatePortalSEOSetting(PortalSEOSettingModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorPortalSEOModelNull);

            bool isUpdated = _portalSEOSettingRepository.Update(model.ToEntity<ZnodeCMSPortalSEOSetting>());
            if (isUpdated)
            {
                ZnodeLogging.LogMessage(String.Format(Admin_Resources.SuccessPortalSEOSettingInsert, model.CMSPortalSEOSettingId), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return true;
            }
            ZnodeLogging.LogMessage(String.Format(Admin_Resources.ErrorPortalSEOSettingInsert, model.CMSPortalSEOSettingId), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return false;
        }
        #endregion

        #region SEO Details
        //Gets the list of SEO details.
        public virtual SEODetailsListModel GetSEODetailsList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            //Bind the Filter, Sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate seoDetailsList list ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //Get Expands.

            List<string> navigationProperties = GetExpands(expands);

            //Maps the entity list to model.
            IList<ZnodeCMSSEODetail> seoDetailsList = _seoDetailRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, navigationProperties, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("seoDetailsList list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, seoDetailsList?.Count());
            SEODetailsListModel listModel = new SEODetailsListModel();
            listModel.SEODetailsList = seoDetailsList?.Count > 0 ? seoDetailsList.ToModel<SEODetailsModel>().ToList() : new List<SEODetailsModel>();

            //Set for Pagination.
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Get the SEO details.
        [Obsolete]
        public virtual SEODetailsModel GetSEODetails(int itemId, int seoTypeId, int localeId, int portalId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter itemId, seoTypeId, localeId,portalId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { itemId, seoTypeId, localeId, portalId });

            //Check for User Portal Access.
            CheckUserPortalAccess(portalId);

            if (itemId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorItemIdLessThan1);

            //If LocaleId is less than 1 get default locale.
            if (localeId < 1)
                localeId = GetDefaultLocaleId();
            ZnodeLogging.LogMessage("localeId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info, localeId);
            //Get the SEO details form SEO detail and SEO detail locale table.
            SEODetailsModel model = GetSEOAndLocaleDetails(itemId, seoTypeId, localeId, portalId);

            //Get SEO details for default Locale if model returns null for locale Id.
            model = GetSEODetailsIfNull(itemId, seoTypeId, portalId, model);

            //Maps the field.
            model.SEOTypeName = _seoTypeRepository.Table.Where(x => x.CMSSEOTypeId == seoTypeId).Select(x => x.Name).FirstOrDefault();
            model.CMSSEOTypeId = seoTypeId;
            model.ItemName = GetSeoTypeItemName(itemId, seoTypeId, localeId, portalId);
            model.LocaleId = (localeId > 0) ? localeId : model.LocaleId;
            model.OldSEOURL = model.SEOUrl;
            model.PimProductId = model.SEOId.GetValueOrDefault();
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return model;
        }

        //Get the SEO details.
        public virtual SEODetailsModel GetSEODetailsBySEOCode(string seoCode, int seoTypeId, int localeId, int portalId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter seoCode, seoTypeId, localeId,portalId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { seoCode, seoTypeId, localeId, portalId });

            //Check for User Portal Access.
            CheckUserPortalAccess(portalId);

            if (string.IsNullOrEmpty(seoCode))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorItemIdLessThan1);

            //If LocaleId is less than 1 get default locale.
            if (localeId < 1)
                localeId = GetDefaultLocaleId();

            //Get the SEO details form SEO detail and SEO detail locale table.
            SEODetailsModel model = GetSEOAndLocaleDetailInformation(seoCode, seoTypeId, localeId, portalId);

            //Get SEO details for default Locale if model returns null for locale Id.
            model = GetSEODetailsIfNullforSEOCode(seoCode, seoTypeId, portalId, model);

            //Maps the field.
            model.SEOTypeName = _seoTypeRepository.Table.Where(x => x.CMSSEOTypeId == seoTypeId).Select(x => x.Name).FirstOrDefault();
            model.CMSSEOTypeId = seoTypeId;
            model.ItemName = GetSeoTypeItemName(seoCode, seoTypeId, localeId, portalId);
            model.LocaleId = (localeId > 0) ? localeId : model.LocaleId;
            model.OldSEOURL = model.SEOUrl;
            model.PimProductId = model.SEOId.GetValueOrDefault();
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return model;
        }

        //Get the SEO details.
        public virtual SEODetailsModel GetDefaultSEODetails(string seoCode, int seoTypeId, int localeId, int portalId, int itemId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter seoCode, seoTypeId, localeId,portalId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { seoCode, seoTypeId, localeId, portalId });
            string seoType = ((SEODetailsEnum)seoTypeId).ToString();
            //Check for User Portal Access.
            CheckUserPortalAccess(portalId);

            if (localeId < 1)
                localeId = GetDefaultLocaleId();
            IZnodeViewRepository<SEODetailsModel> objStoredProc = new ZnodeViewRepository<SEODetailsModel>();
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@SEOType", seoType, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Id", itemId, ParameterDirection.Input, DbType.Int32);

            IList<SEODetailsModel> seoEntity = objStoredProc.ExecuteStoredProcedureList("Znode_GetSEODefaultSetting @PortalId,@SEOType,@Id");
            //Get the SEO details form SEO detail and SEO detail locale table.
            SEODetailsModel model = GetSEOAndLocaleDetailInformation(seoCode, seoTypeId, localeId, portalId);
            //Get SEO details for default Locale if model returns null for locale Id.
            model = GetSEODetailsIfNullforSEOCode(seoCode, seoTypeId, portalId, model);
            if (seoEntity?.Count > 0)
            {
                model.SEOTitle = seoEntity?.FirstOrDefault()?.SEOTitle;
                model.SEOKeywords = seoEntity?.FirstOrDefault()?.SEOKeywords;
                model.SEODescription = seoEntity?.FirstOrDefault()?.SEODescription;
                model.CMSSEOTypeId = seoTypeId;
            }
            return model;
        }

        public virtual SEODetailsModel GetPublishSEODetails(int seoDetailId, string seoType, int localeId, int portalId, string seoCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter seoDetailId, seoType, localeId, portalId, seoCode:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { seoDetailId, seoType, localeId, portalId, seoCode });

            if (seoCode == "NaN")
                seoCode = string.Empty;

            if (seoType.Equals(ZnodeConstant.Product, StringComparison.InvariantCultureIgnoreCase))
                seoType = ZnodeConstant.Product;

            ZnodePublishSeoEntity publishSEOList = GetService<IPublishedPortalDataService>().GetSEOEntityByCode(seoCode, seoType, portalId, localeId);

            SEODetailsModel model = IsNotNull(publishSEOList) ? publishSEOList.ToModel<SEODetailsModel>() : new SEODetailsModel();

            return model;
        }


        //Get product seo details.
        public virtual SEODetailsModel GetProductSeoData(int localeId, int seoId, int portalId, string seoTypeName, string seoCode)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter localeId,seoId,portalId,seoTypeName,seoCode:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { localeId, seoId, portalId, seoTypeName, seoCode });

            return (from seoDetail in _seoDetailRepository.Table
                    join seoType in _seoTypeRepository.Table on seoDetail.CMSSEOTypeId equals seoType.CMSSEOTypeId
                    from seoDetailLocale in _seoDetailLocaleRepository.Table.Where(seoDetailLocale => seoDetail.CMSSEODetailId == seoDetailLocale.CMSSEODetailId && seoDetailLocale.LocaleId == localeId).DefaultIfEmpty()
                    where seoDetail.SEOCode == seoCode && seoDetail.PortalId == portalId && seoType.Name == seoTypeName
                    select new SEODetailsModel
                    {
                        SEODescription = seoDetailLocale.SEODescription,
                        SEOTitle = seoDetailLocale.SEOTitle,
                        SEOUrl = seoDetail.SEOUrl,
                        SEOKeywords = seoDetailLocale.SEOKeywords
                    })?.FirstOrDefault();
        }

        //Get category list for SEO
        public virtual PublishCategoryListModel GetCategoryListForSEO(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            int portalId, localeId;
            GetParametersValueForFilters(filters, out portalId, out localeId);

            //Remove portal id filter.
            filters.RemoveAll(x => x.FilterName == Utilities.FilterKeys.PortalId);
            filters.RemoveAll(x => x.FilterName == Utilities.FilterKeys.LocaleId);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate categories list ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IZnodeViewRepository<PublishCategoryModel> objStoredProc = new ZnodeViewRepository<PublishCategoryModel>();
            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", null, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);

            //SP Call- Get publish categories
            List<PublishCategoryModel> categories = objStoredProc.ExecuteStoredProcedureList("Znode_GetCatalogCategorySEODetail @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId,@PortalId", 4, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("categories list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, categories?.Count());
            //Filter list by expiration date and activation date.
            categories = GetFilterDateReult(categories);

            PublishCategoryListModel publishCategoryListModel = new PublishCategoryListModel() { PublishCategories = categories };

            //Map pagination parameters
            publishCategoryListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return publishCategoryListModel;
        }

        //Get parameter values from filters.
        private static void GetParametersValueForFilters(FilterCollection filters, out int portalId, out int localeId)
        {
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(Utilities.FilterKeys.PortalId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out portalId);
            int.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(Utilities.FilterKeys.LocaleId, StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out localeId);
        }

        //Filter list by expiration date and activation date.
        private List<PublishCategoryModel> GetFilterDateReult(List<PublishCategoryModel> list) =>
         list.Where(x => (x.ActivationDate == null || x.ActivationDate.GetValueOrDefault().Date <= HelperUtility.GetDate()) && (x.ExpirationDate == null || x.ExpirationDate.GetValueOrDefault().Date >= HelperUtility.GetDate())).ToList();

        //Get SEO details for default Locale if model returns null for locale Id.
        [Obsolete]
        private SEODetailsModel GetSEODetailsIfNull(int itemId, int seoTypeId, int portalId, SEODetailsModel model)
        {
            ZnodeLogging.LogMessage("Input Parameter itemId,seoTypeId,portalId", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { itemId, seoTypeId, portalId });
            if (IsNull(model))
                model = new SEODetailsModel();
            //Get SEO details for default Locale if model returns null for locale Id.
            else if (string.IsNullOrEmpty(model.SEOUrl) && (string.IsNullOrEmpty(model.SEOTitle) && string.IsNullOrEmpty(model.SEOKeywords) && string.IsNullOrEmpty(model.SEODescription)))
            {
                ZnodeLogging.LogMessage("Parameter GetSEOAndLocaleDetailInformation", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { itemId, seoTypeId, "Method: GetDefaultLocaleId()", portalId });
                model = GetSEOAndLocaleDetails(itemId, seoTypeId, GetDefaultLocaleId(), portalId);
            }
            return model;
        }

        //Get SEO details for default Locale if model returns null for locale Id.
        private SEODetailsModel GetSEODetailsIfNullforSEOCode(string seoCode, int seoTypeId, int portalId, SEODetailsModel model)
        {
            ZnodeLogging.LogMessage("Input Parameter seoCode,seoTypeId,portalId", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { seoCode, seoTypeId, portalId });

            if (IsNull(model))
                model = new SEODetailsModel();
            //Get SEO details for default Locale if model returns null for locale Id.
            else if (string.IsNullOrEmpty(model.SEOUrl) && (string.IsNullOrEmpty(model.SEOTitle) && string.IsNullOrEmpty(model.SEOKeywords) && string.IsNullOrEmpty(model.SEODescription)))
            {
                ZnodeLogging.LogMessage("Parameter GetSEOAndLocaleDetailInformation", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { seoCode, seoTypeId, "Method: GetDefaultLocaleId()", portalId });
                model = GetSEOAndLocaleDetailInformation(seoCode, seoTypeId, GetDefaultLocaleId(), portalId);
            }
            return model;
        }

        //Get seo and locale details
        [Obsolete]
        private SEODetailsModel GetSEOAndLocaleDetails(int itemId, int seoTypeId, int localeId, int portalId)
        {
            ZnodeLogging.LogMessage("Input Parameter itemId,seoTypeId,localeId,portalId", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { itemId, seoTypeId, localeId, portalId });
            if (portalId < 1)
            {
                var _seoDetail = _seoDetailRepository.Table.FirstOrDefault(a => a.SEOId == itemId && a.CMSSEOTypeId == seoTypeId);
                portalId = (IsNotNull(_seoDetail) ? Convert.ToInt32(_seoDetail.PortalId) : portalId);
                ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { seoDetail = _seoDetail, portalId = portalId });
            }

            return (from seoDetail in _seoDetailRepository.Table
                    from seoDetailLocale in _seoDetailLocaleRepository.Table
                    .Where(seoDetailLocale => seoDetail.CMSSEODetailId == seoDetailLocale.CMSSEODetailId && seoDetailLocale.LocaleId == localeId).DefaultIfEmpty()
                    where seoDetail.SEOId == itemId && seoDetail.PortalId == portalId && seoDetail.CMSSEOTypeId == seoTypeId
                    select new SEODetailsModel()
                    {
                        CMSSEODetailId = seoDetail.CMSSEODetailId,
                        IsRedirect = (seoDetail.IsRedirect == null) ? false : seoDetail.IsRedirect,
                        CMSSEOTypeId = seoDetail.CMSSEOTypeId,
                        LocaleId = localeId,
                        MetaInformation = seoDetail.MetaInformation,
                        SEOId = seoDetail.SEOId,
                        SEODescription = !Equals(seoDetailLocale, null) ? seoDetailLocale.SEODescription : string.Empty,
                        SEOKeywords = !Equals(seoDetailLocale, null) ? seoDetailLocale.SEOKeywords : string.Empty,
                        SEOTitle = !Equals(seoDetailLocale, null) ? seoDetailLocale.SEOTitle : string.Empty,
                        SEOUrl = !Equals(seoDetailLocale, null) ? seoDetail.SEOUrl : string.Empty,
                        PortalId = portalId
                    })?.FirstOrDefault();
        }

        //Get seo and locale details
        private SEODetailsModel GetSEOAndLocaleDetailInformation(string seoCode, int seoTypeId, int localeId, int portalId)
        {
            ZnodeLogging.LogMessage("Input Parameter seoCode,seoTypeId,localeId,portalId", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { seoCode, seoTypeId, localeId, portalId });
            if (portalId < 1)
            {
                var _seoDetail = _seoDetailRepository.Table.FirstOrDefault(a => a.SEOCode == seoCode && a.CMSSEOTypeId == seoTypeId);
                ZnodeLogging.LogMessage("seoDetail:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { _seoDetail });
                portalId = (IsNotNull(_seoDetail) ? Convert.ToInt32(_seoDetail.PortalId) : portalId);
                ZnodeLogging.LogMessage("portalId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { portalId });

            }

            return (from seoDetail in _seoDetailRepository.Table
                    from seoDetailLocale in _seoDetailLocaleRepository.Table
                    .Where(seoDetailLocale => seoDetail.CMSSEODetailId == seoDetailLocale.CMSSEODetailId && seoDetailLocale.LocaleId == localeId).DefaultIfEmpty()
                    where seoDetail.SEOCode == seoCode && seoDetail.PortalId == portalId && seoDetail.CMSSEOTypeId == seoTypeId
                    select new SEODetailsModel()
                    {
                        CMSSEODetailId = seoDetail.CMSSEODetailId,
                        IsRedirect = (seoDetail.IsRedirect == null) ? false : seoDetail.IsRedirect,
                        CMSSEOTypeId = seoDetail.CMSSEOTypeId,
                        LocaleId = localeId,
                        MetaInformation = seoDetail.MetaInformation,
                        SEOId = seoDetail.SEOId,
                        SEODescription = !Equals(seoDetailLocale, null) ? seoDetailLocale.SEODescription : string.Empty,
                        SEOKeywords = !Equals(seoDetailLocale, null) ? seoDetailLocale.SEOKeywords : string.Empty,
                        SEOTitle = !Equals(seoDetailLocale, null) ? seoDetailLocale.SEOTitle : string.Empty,
                        SEOUrl = !Equals(seoDetailLocale, null) ? seoDetail.SEOUrl : string.Empty,
                        CanonicalURL = !Equals(seoDetailLocale, null) ? seoDetailLocale.CanonicalURL : string.Empty,
                        RobotTag = !Equals(seoDetailLocale, null) ? seoDetailLocale.RobotTag : string.Empty,
                        PortalId = portalId
                    })?.FirstOrDefault();
        }

        //Create seo details.
        public virtual SEODetailsModel CreateSEODetails(SEODetailsModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter SEODetailsModel having originalPortalId, seoTitle, seoDescription, seoKeywords, seoTypeName", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { model.PortalId, model.SEOTitle, model.SEODescription, model.SEOKeywords, model.SEOTypeName });

            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorSEODetailModelNull);

            //Check if seoUrl for Portal already exists.
            if (IsSeoUrlExists(model.SEOUrl, model.PortalId))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorSEOURLExists);

            int originalPortalId = model.PortalId;
            int pimProductId = model.PimProductId;
            string seoTitle = model.SEOTitle;
            string seoDescription = model.SEODescription;
            string seoKeywords = model.SEOKeywords;
            string seoTypeName = model.SEOTypeName;
            ZnodeCMSSEODetail seoDetail;
            try
            {
                model.SEOId = null;
                seoDetail = model.IsAllStore ? SaveSEOForAllStore(model) : AddSEODetails(model);
                model.CMSSEODetailId = seoDetail.CMSSEODetailId;
                model.PortalId = originalPortalId;
                model = seoDetail.ToModel<SEODetailsModel>();
                model.PimProductId = model.SEOId.GetValueOrDefault();
                model.SEOTitle = seoTitle;
                model.SEOKeywords = seoKeywords;
                model.SEODescription = seoDescription;
                model.SEOTypeName = seoTypeName;
                productAssociationHelper.SaveProductAsDraft(pimProductId);
                return model;
            }
            catch (Exception ex)
            {
                string msg = $"Error message - {Admin_Resources.ErrorSEODetailsInsert}, Exception - {ex.Message}";
                ZnodeLogging.LogMessage(msg, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return null;
            }
        }

        //This method is used to add/update the seoDetails for all portal Ids.
        private ZnodeCMSSEODetail SaveSEOForAllStore(SEODetailsModel model)
        {
            ZnodeCMSSEODetail seoDetail = new ZnodeCMSSEODetail();
            int? CMSSEOTypeId = _seoTypeRepository.Table.FirstOrDefault(seoType => seoType.Name.Equals(ZnodeConstant.Product))?.CMSSEOTypeId;

            ZnodeLogging.LogMessage("CMSSEOTypeId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, CMSSEOTypeId);
            if (model.CMSSEOTypeId == CMSSEOTypeId)
            {
                List<PortalModel> portals;
                portals = GetService<IPortalService>().GetPortalList(new NameValueCollection(), new FilterCollection(), new NameValueCollection(), new NameValueCollection())?.PortalList;
                if (IsNotNull(portals))
                {
                    foreach (var portal in portals)
                    {
                        model.PortalId = Convert.ToInt32(portal.PortalId);
                        var seoDetailsList = _seoDetailRepository.Table.Where(d => d.PortalId == model.PortalId && d.SEOId == model.SEOId && d.CMSSEOTypeId == model.CMSSEOTypeId).ToList();
                        if (seoDetailsList.Count == 0)
                            seoDetail = AddSEODetails(model);
                        else
                        {
                            foreach (var detail in seoDetailsList)
                            {
                                model.CMSSEODetailId = detail.CMSSEODetailId;
                                model.OldSEOURL = detail.SEOUrl;
                                UpdateSEODetails(model);
                            }
                        }
                    }
                }
            }
            return seoDetail;
        }

        //Add SEO details call to save the data in ZnodeCMSSEODetail and ZnodeCMSSEODetailLocale
        private ZnodeCMSSEODetail AddSEODetails(SEODetailsModel model)
        {
            if (IsNull(model.PublishStateId) || model.PublishStateId == 0)
            {
                model.PublishStateId = (byte)ZnodePublishStatesEnum.DRAFT;
            }
            ZnodeLogging.LogMessage("Input Parameter model having PublishStateId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { model.PublishStateId });

            ZnodeCMSSEODetail seoDetail = new ZnodeCMSSEODetail();
            if (IsSEOHasRequiredDetails(model))
            {
                seoDetail = _seoDetailRepository.Insert(model.ToEntity<ZnodeCMSSEODetail>());

                ZnodeLogging.LogMessage(String.Format("Seo detail inserted with the Id{0} ", model.CMSSEODetailId), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            }

            //If inserted successfully.
            if (seoDetail?.CMSSEODetailId > 0)
            {
                //Maps the CMSSEODetailId in model.
                model.CMSSEODetailId = seoDetail.CMSSEODetailId;

                //Save the data in locale table.
                SaveInSEODetailsLocale(model);

                ZnodeLogging.LogMessage(String.Format(Admin_Resources.SuccessSEODetailsInsert, model.CMSSEODetailId), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            }
            else
            {
                seoDetail.SEOCode = model.SEOCode;
                seoDetail.PortalId = model.PortalId;
                seoDetail.CMSSEOTypeId = model.CMSSEOTypeId;
            }
            return seoDetail;
        }

        //Update seo details.
        public virtual bool UpdateSEODetails(SEODetailsModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter model having SEOUrl,CMSSEODetailId,PortalId", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { model.SEOUrl, model.CMSSEODetailId, model.PortalId });
            if (IsNull(model))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorSEODetailModelNull);
            //skip the validation if IsAllStore true.
            if (!model.IsAllStore && IsSeoUrlExistsOnUpdate(model.SEOUrl, model.CMSSEODetailId, model.PortalId))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorSEOURLExists);
            model.SEOUrl = model.SEOUrl?.Trim();
            model.PublishStateId = (byte)ZnodePublishStatesEnum.DRAFT;

            bool isUpdated = _seoDetailRepository.Update(model.ToEntity<ZnodeCMSSEODetail>());
            ZnodeLogging.LogMessage(isUpdated ? String.Format(Admin_Resources.SuccessSEODetailUpdate, model.CMSSEODetailId) : String.Format(Admin_Resources.ErrorSEODetailUpdate, model.CMSSEODetailId), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            //Save the data in locale table.
            SaveInSEODetailsLocale(model);

            if(isUpdated)
                productAssociationHelper.SaveProductAsDraft(Convert.ToInt32(model?.PimProductId));

            if (model.IsRedirect.GetValueOrDefault() && !Equals(model.SEOUrl, model.OldSEOURL))
                SEORedirectUrlHelper.CreateUrlRedirect(model);

            return isUpdated;
        }

        /// <summary>
        /// <summary>
        /// Delete Seo Detail by using seoTypeId, portalId and seoCode.
        /// </summary>
        /// <param name="seoTypeId">seoType Id</param>
        /// <param name="portalId">portal Id</param>
        /// /// <param name="seoCode">seoCode</param>
        /// <returns>Returns true if record deleted successfully, else false.</returns>.
        public virtual bool DeleteSeo(int seoTypeId, int portalId, string seoCode)
        {
            int status = 0;
            ZnodeLogging.LogMessage("Execution started:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("SEOCode:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, seoCode);
            ZnodeLogging.LogMessage("CMSSEOTypeId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, seoTypeId);

            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("SEOCode", seoCode, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("CMSSEOTypeId", seoTypeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Status", status, ParameterDirection.Output, DbType.Int32);
            
            IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteCMSSeoDetails @SEOCode,@CMSSEOTypeId,@PortalId,@Status OUT", 1, out status);
            ZnodeLogging.LogMessage("deleteResult count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { deleteResult?.Count });
            if (status==1)
            {
                ZnodeLogging.LogMessage(Admin_Resources.DeleteSeoDetail, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return true;
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorDeleteSeo, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return false;
            }
        }

        #endregion
        #endregion

        #region Private Methods
        //Returns true if url is present in SEO Details table.
        private bool IsSeoUrlExists(string url, int portalId)
              => _seoDetailRepository.Table.Any(x => x.SEOUrl != null && x.SEOUrl == url.Trim() && x.PortalId == portalId);

        //Returns true if url is present in SEO Details table.
        private bool IsSeoUrlExistsOnUpdate(string url, int? id, int portalId)
        {
            ZnodeLogging.LogMessage("Input Parameter url,id,portalId", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { url, id, portalId });

            List<ZnodeCMSSEODetail> entityList = _seoDetailRepository.Table.Where(x => x.SEOUrl != null && x.SEOUrl == url.Trim() && x.PortalId == portalId)?.Select(x => x)?.ToList();
            ZnodeLogging.LogMessage("entityList list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, entityList?.Count());
            //If the url not exits.
            if (IsNull(entityList) || entityList?.Count == 0) return false;

            if (entityList.Any(x => x.CMSSEODetailId == id)) return false;

            return true;
        }

        //Get expands and add them to navigation properties
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (IsNotNull(expands) && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    string value = expands.Get(key);
                    if (Equals(value, Constants.ExpandKeys.SEOUrl.ToLower())) { SetExpands(ZnodeCMSSEODetailEnum.SEOUrl.ToString(), navigationProperties); }
                    if (Equals(value, ZnodeCMSSEODetailEnum.ZnodeCMSSEODetailLocales.ToString().ToLower())) { SetExpands(ZnodeCMSSEODetailEnum.ZnodeCMSSEODetailLocales.ToString(), navigationProperties); }
                }
            }
            return navigationProperties;
        }

        //Save the data into SEO details locale.
        private void SaveInSEODetailsLocale(SEODetailsModel seoDetailsmodel)
        {
            List<int> localeIds = new List<int>();
            if (seoDetailsmodel.LocaleId <= 0)
                localeIds = GetActiveLocaleList().Select(x => x.LocaleId).ToList();
            else
                localeIds.Add(seoDetailsmodel.LocaleId);
            ZnodeLogging.LogMessage("localeIds list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, localeIds?.Count());
            foreach (var localeId in localeIds)
            {
                seoDetailsmodel.LocaleId = localeId;
                //Get the SEO details locale.
                List<ZnodeCMSSEODetailLocale> seoDetailsLocales = _seoDetailLocaleRepository.Table.Where(x => x.CMSSEODetailId == seoDetailsmodel.CMSSEODetailId && x.LocaleId == (int?)localeId)?.ToList();

                if (seoDetailsLocales?.Count > 0)
                {
                    foreach (var seoDetailsLocale in seoDetailsLocales)
                    {
                        ZnodeLogging.LogMessage(_seoDetailLocaleRepository.Update(GetSEODetailsLocaleEntity(seoDetailsmodel, seoDetailsLocale.CMSSEODetailLocaleId))
                            ? String.Format(Admin_Resources.SuccessSEODetailUpdate, seoDetailsmodel.CMSSEODetailId) : String.Format(Admin_Resources.ErrorSEODetailUpdate, seoDetailsmodel.CMSSEODetailId), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                    }
                }
                else
                {
                    if (seoDetailsmodel.LocaleId == 0)
                        seoDetailsmodel.LocaleId = GetDefaultLocaleId();
                    ZnodeLogging.LogMessage(_seoDetailLocaleRepository.Insert(GetSEODetailsLocaleEntity(seoDetailsmodel, 0))?.CMSSEODetailLocaleId > 0
                            ? String.Format(Admin_Resources.SuccessSEODetailUpdate, seoDetailsmodel.CMSSEODetailId) : String.Format(Admin_Resources.ErrorSEODetailUpdate, seoDetailsmodel.CMSSEODetailId), ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

                }
            }

        }

        //Get the SEO details locale entity.
        private static ZnodeCMSSEODetailLocale GetSEODetailsLocaleEntity(SEODetailsModel seoDetailsmodel, int cmsSEODetailLocaleId)
            => new ZnodeCMSSEODetailLocale
            {
                LocaleId = seoDetailsmodel.LocaleId,
                CMSSEODetailId = seoDetailsmodel.CMSSEODetailId,
                SEODescription = seoDetailsmodel.SEODescription,
                SEOKeywords = seoDetailsmodel.SEOKeywords,
                SEOTitle = seoDetailsmodel.SEOTitle,
                CMSSEODetailLocaleId = cmsSEODetailLocaleId,
                CanonicalURL = seoDetailsmodel.CanonicalURL,
                RobotTag = seoDetailsmodel.RobotTag
            };

        //Get the name of seo type item.
        private string GetSeoTypeItemName(int itemId, int seoTypeId, int localeId, int portalId)
        {
            ZnodeLogging.LogMessage("Input Parameter itemId,seoTypeId,localeId,portalId", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { itemId, seoTypeId, localeId, portalId });

            //On the basis of seoTypeId get the data of SeoType item.
            IPublishProductService _publishedProductService = GetService<IPublishProductService>();
            IPublishCategoryService _publishedCategoryService = GetService<IPublishCategoryService>();
            IZnodeRepository<ZnodeCMSContentPage> _contentPageRepository = new ZnodeRepository<ZnodeCMSContentPage>();

            //Returns the name of seo type item on the basis of items name and seoTypeId.
            switch (seoTypeId)
            {
                case (int)SEODetailsEnum.Product:
                    {
                        PublishProductModel product = _publishedProductService.GetPublishProduct(itemId, GetPublishFilters(portalId, localeId), null);
                        return (product?.IsConfigurableProduct).GetValueOrDefault() ? product?.ParentConfiguarableProductName : product?.Name;
                    }
                case (int)SEODetailsEnum.Category:
                    return _publishedCategoryService.GetPublishCategory(itemId, GetPublishFilters(portalId, localeId), null)?.Name;
                case (int)SEODetailsEnum.Content_Page:
                    return _contentPageRepository.Table.Where(x => x.CMSContentPagesId == itemId)?.FirstOrDefault()?.PageName;
                default:
                    break;
            }
            return null;
        }

        protected virtual string GetSeoTypeItemName(string seoCode, int seoTypeId, int localeId, int portalId)
        {
            ZnodeLogging.LogMessage("Input Parameter seoCode,seoTypeId,localeId,portalId", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { seoCode, seoTypeId, localeId, portalId });
            //Returns the name of seo type item on the basis of items name and seoTypeId.
            switch (seoTypeId)
            {
                case (int)SEODetailsEnum.Product:
                    return GetProductName(seoCode, localeId, portalId);
                case (int)SEODetailsEnum.Category:
                    return GetCategoryName(seoCode, localeId);
                case (int)SEODetailsEnum.Content_Page:
                    return new ZnodeRepository<ZnodeCMSContentPage>().Table.Where(x => x.PageName == seoCode && x.PortalId == portalId)?.FirstOrDefault()?.PageName;
            }
            return null;
        }

        //Get Category name by seoCode.
        protected virtual string GetCategoryName(string seoCode, int localeId)
        {
            ZnodeLogging.LogMessage("Input Parameter seoCode,localeId", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { seoCode, localeId });
            string CategoryName = "";
            //Get category name by locale id and categorycode.s
            ExecuteSpHelper executeSpHelper = new ExecuteSpHelper();
            executeSpHelper.GetParameter("@LocaleId", localeId, ParameterDirection.Input, SqlDbType.Int);
            executeSpHelper.GetParameter("@CategoryCode", seoCode, ParameterDirection.Input, SqlDbType.NVarChar);
            DataSet data = executeSpHelper.GetSPResultInDataSet("Znode_GetCategoryName");

            DataTable dataTable = data?.Tables[0];
            if (dataTable?.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable?.Rows)
                {
                    CategoryName = Convert.ToString(row["CategoryName"]);
                }
            }
            ZnodeLogging.LogMessage("CategoryName:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { CategoryName });
            return CategoryName;
        }

        //Get Product name by seoCode.
        protected virtual string GetProductName(string seoCode, int localeId, int portalId)
        {
            ZnodeLogging.LogMessage("Input Parameter seoCode,portalId,localeId", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { seoCode, portalId, localeId });
            string productName = "";
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(new FilterCollection() { { new FilterTuple(FilterKeys.Sku, FilterOperators.Is, seoCode) } }, new NameValueCollection(), new NameValueCollection());
            ZnodeLogging.LogMessage("pageListModel to generated ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            pageListModel.PagingLength = 1;
            bool pimProductIdsIn = false;
            DataSet dsProduct = GetService<IProductService>().GetXmlProductsDataSet(new FilterCollection() { { new FilterTuple(FilterKeys.Sku, FilterOperators.Is, seoCode) } }, pageListModel, string.Empty, ref pimProductIdsIn);

            if (!Equals(dsProduct, null) && dsProduct.Tables.Count > 0 && dsProduct.Tables[0].Rows.Count > 0)
            {
                var xml = Convert.ToString(dsProduct.Tables[0]?.Rows[0]["ProductXML"]);
                if (!string.IsNullOrEmpty(xml))
                {
                    productName = XElement.Parse(xml).Descendants("ProductName").Single().Value;
                    return productName;
                }
            }
            return productName;
        }

        //Get filters to get publish data.
        private FilterCollection GetPublishFilters(int portalId, int localeId)
        {
            ZnodeLogging.LogMessage("Input Parameter portalId,localeId", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { portalId, localeId });
            string publishCatalogId = Convert.ToString(_portalCatalogRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.PublishCatalogId);
            ZnodeLogging.LogMessage("publishCatalogId:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { publishCatalogId });
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, portalId.ToString()));
            filters.Add(new FilterTuple(FilterKeys.LocaleId, FilterOperators.Equals, localeId.ToString()));
            filters.Add(new FilterTuple(WebStoreEnum.ZnodeCatalogId.ToString(), FilterOperators.Equals, publishCatalogId));
            return filters;
        }

     

    

        //Publish seo details
        public virtual PublishedModel Publish(string seoCode, int portalId, int localeId, int seoTypeId, string targetPublishState, bool takeFromDraftFirst)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter seoCode,portalId,localeId,targetPublishState,takeFromDraftFirst", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { seoCode, portalId, localeId, targetPublishState, takeFromDraftFirst });

            //Check SEO type
            CheckSeoPublishType(seoTypeId, out bool isProductOrCategory);

            bool result = false;

            //replace the ASCII value with its equivalent character
            seoCode = seoCode.Replace("038", "&");

            result = GetService<IPublishPortalDataService>().PublishSEO(seoCode, portalId, localeId, seoTypeId, targetPublishState);

            if (!isProductOrCategory && result)
                GetService<ICMSPageSearchService>().CreateIndexForPortalCMSPages(portalId, Convert.ToString(targetPublishState));

            if (result)
                ClearCacheHelper.EnqueueEviction(new PortalPublishEvent()
                {
                    Comment = $"From SEO with code '{seoCode}' publishing.",
                    PortalIds = new int[] { portalId }
                });

            return new PublishedModel { IsPublished = result, ErrorMessage = result ? String.Empty : Api_Resources.GenericExceptionMessageDuringPublish };
        }

        private void CheckSeoPublishType(int seoTypeId, out bool isProductOrCategory)
        {
            ZnodeCMSSEOType znodeCMSSEOType = _seoTypeRepository.Table?.FirstOrDefault(x=>x.CMSSEOTypeId == seoTypeId);
            isProductOrCategory = IsNotNull(znodeCMSSEOType) ? ((znodeCMSSEOType.Name.Equals(ZnodeConstant.Product,StringComparison.CurrentCultureIgnoreCase))
                || (znodeCMSSEOType.Name.Equals(ZnodeConstant.Category, StringComparison.CurrentCultureIgnoreCase))) : false;

            if (isProductOrCategory)
                //Added addition logic to check whether any catalog is in progress or not to prevent DB deadlock 
                CheckCatalogPublishAlreadyInProgress(isProductOrCategory, IsNotNull(znodeCMSSEOType) ? znodeCMSSEOType.Name : string.Empty);
        }

        public virtual string CheckPublishState(string targetPublishState)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter targetPublishState:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { targetPublishState });
            if (string.IsNullOrEmpty(targetPublishState) || targetPublishState == "NONE")
            {
                targetPublishState = GetDefaultPublishState().ToString();
            }

            return targetPublishState;
        }

        //Check catalog publish already in progress or not. if yes we will throw exception. 
        private void CheckCatalogPublishAlreadyInProgress(bool isProductOrCategory, string cmsSEOType)
        {
            IZnodeRepository<ZnodePublishCatalogLog> _publishCatalogLogRepository = new ZnodeRepository<ZnodePublishCatalogLog>();
            bool isCatalogCategoryPublishInProgress = _publishCatalogLogRepository.Table.Any(x => x.IsCatalogPublished == null);
            bool isCheckExceptionType = cmsSEOType.Equals(ZnodeConstant.Product, StringComparison.CurrentCultureIgnoreCase) ? true : false;

            if (isCatalogCategoryPublishInProgress)
                throw new ZnodeException(isCheckExceptionType ? ErrorCodes.ProductSeoPublishError : ErrorCodes.CategorySeoPublishError,
                    isCheckExceptionType ? PIM_Resources.ErrorProductSeoPublish : PIM_Resources.ErrorCategorySeoPublish);
        }

       
      
       
      
        //Update the status on publish
        private void UpdateSeoPublishStatusBySEOCode(int cmsSeoDetailId, int seoTypeId, string seoCode, string metaInformation, int portalId, string seoUrl, bool isPublished)
        {
            ZnodeLogging.LogMessage("Input Parameter portalId,metaInformation,cmsSeoDetailId,seoTypeId,seoUrl", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { portalId, metaInformation, cmsSeoDetailId, seoTypeId, seoUrl });
            _seoDetailRepository.Update(new ZnodeCMSSEODetail
            {
                CMSSEODetailId = cmsSeoDetailId,
                CMSSEOTypeId = seoTypeId,
                SEOCode = seoCode,
                MetaInformation = metaInformation,
                PortalId = portalId,
                SEOUrl = seoUrl,
                IsPublish = isPublished,
            });
        }

        [Obsolete("To be discontinued in one of the upcoming versions.")]
        private void UpdateSeoPublishStatus(int cmsSeoDetailId, int seoTypeId, int itemId, string metaInformation, int portalId, string seoUrl, bool isPublished)
        {
            ZnodeLogging.LogMessage("Input Parameter portalId,metaInformation,cmsSeoDetailId,seoTypeId,seoUrl", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { portalId, metaInformation, cmsSeoDetailId, seoTypeId, seoUrl });
            _seoDetailRepository.Update(new ZnodeCMSSEODetail
            {
                CMSSEODetailId = cmsSeoDetailId,
                CMSSEOTypeId = seoTypeId,
                SEOId = itemId,
                MetaInformation = metaInformation,
                PortalId = portalId,
                SEOUrl = seoUrl,
                IsPublish = isPublished
            });
        }

        public virtual PublishProductListModel GetProductsForSEO(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            string localeId = filters.Find(x => string.Equals(x.FilterName, ZnodeCMSContentPageGroupLocaleEnum.LocaleId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3;

            string portalId = filters.Find(x => string.Equals(x.FilterName, ZnodePortalEnum.PortalId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Item3;
            ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { localeId = localeId, portalId = portalId });

            filters.RemoveAll(x => x.FilterName == FilterKeys.PortalId);
            filters.RemoveAll(x => x.FilterName == FilterKeys.LocaleId);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate publishProducts list ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IList<PublishProductModel> publishProducts = GetProductListForSEO(localeId, portalId, pageListModel);
            ZnodeLogging.LogMessage("publishProducts list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, publishProducts?.Count());
            PublishProductListModel publishProductListModel = new PublishProductListModel() { PublishProducts = publishProducts.ToList() };

            //Map pagination parameters
            publishProductListModel.BindPageListModel(pageListModel);

            return publishProductListModel;
        }

        protected virtual IList<PublishProductModel> GetProductListForSEO(string localeId, string portalId, PageListModel pageListModel)
        {
            ZnodeLogging.LogMessage("Input Parameter localeId,portalId", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { localeId, portalId });

            IZnodeViewRepository<PublishProductModel> objStoredProc = new ZnodeViewRepository<PublishProductModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", null, ParameterDirection.Output, DbType.Int32);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);

            IList<PublishProductModel> publishProducts = objStoredProc.ExecuteStoredProcedureList("Znode_GetCatalogProductSEODetail @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId,@PortalId", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("publishProducts list count:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, publishProducts?.Count());
            return publishProducts;
        }
        //Update the status on publish
        private void UpdateSeoPublishStatus(int cmsSeoDetailId, int seoTypeId, string seoCode, string metaInformation, int portalId, string seoUrl, bool isPublished, ZnodePublishStatesEnum targetPublishState, bool? isRedirect = false)
        {
            ZnodeLogging.LogMessage("Input Parameter seoCode,portalId,metaInformation,cmsSeoDetailId,seoTypeId,seoUrl", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new object[] { seoCode, portalId, metaInformation, cmsSeoDetailId, seoTypeId, seoUrl });
            _seoDetailRepository.Update(new ZnodeCMSSEODetail
            {
                CMSSEODetailId = cmsSeoDetailId,
                CMSSEOTypeId = seoTypeId,
                SEOCode = seoCode,
                MetaInformation = metaInformation,
                PortalId = portalId,
                SEOUrl = seoUrl,
                IsRedirect = isRedirect,
                PublishStateId = isPublished ? (byte)targetPublishState : (byte)ZnodePublishStatesEnum.DRAFT
            });
        }

      
      
        //Check if SEO details to be inserted in DB or not        
        private bool IsSEOHasRequiredDetails(SEODetailsModel model)
        {
            if ((model?.SEOTitle != null) || (model?.SEODescription != null) || (model?.SEOKeywords != null) || (model?.SEOUrl != null) || (model?.CanonicalURL != null) || (model?.RobotTag != "None"))
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
