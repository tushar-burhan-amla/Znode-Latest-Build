using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services
{
    public partial class SEOService
    {
        #region Public Method
        //Get SEO setting by seo type.
        public virtual List<SEODetailsModel> GetSeoSettingList(string seoTypeName, int portalId, int localeId = 0)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters seoTypeName, portalId and localeId: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new object[] { seoTypeName, portalId, localeId });

            IZnodeViewRepository<SEODetailsModel> objStoredProc = new ZnodeViewRepository<SEODetailsModel>();
            objStoredProc.SetParameter("@SeoId", string.Empty, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@SeoType", seoTypeName, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            List<SEODetailsModel> SeoSettings = objStoredProc.ExecuteStoredProcedureList("Znode_GetSeoDetails  @SeoId,@SeoType,@LocaleId,@PortalId")?.ToList();
            ZnodeLogging.LogMessage("SeoSettings list count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, SeoSettings?.Count);
            SeoSettings?.ForEach(x => x.LocaleId = localeId);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return SeoSettings;
        }

        public virtual List<ZnodePublishSeoEntity> GetPublishSEOSettingList(string seoTypeName, int? portalId, int localeId = 0, int? versionId = 0)
        {

            FilterCollection filters = GetFiltersForSEO(portalId, localeId, seoTypeName);

            if (seoTypeName == ZnodeConstant.Category || seoTypeName == ZnodeConstant.Product || seoTypeName == ZnodeConstant.Brand)
            {
                versionId = !Equals(versionId, 0) && HelperUtility.IsNotNull(versionId) ? versionId : GetCatalogVersionId();
                filters.Add(new FilterTuple("VersionId", FilterOperators.Equals, versionId.ToString()));
            }
            else if (seoTypeName == ZnodeConstant.ContentPage || seoTypeName == ZnodeConstant.BlogNews)
                filters.Add(new FilterTuple("VersionId", FilterOperators.Equals, WebstoreVersionId.ToString()));

            List<ZnodePublishSeoEntity> publishSEOList = ZnodeDependencyResolver.GetService<IPublishedPortalDataService>().GetSEOSettings(filters);
            return publishSEOList;
        }

        public List<ZnodeCMSSEODetail> GetSEODetailsList(int portalId, string seoType)
        {
            List<ZnodeCMSSEODetail> znodeCMSSEODetails = (from seoDetails in _seoDetailRepository.Table
                                                          join seoTypeRepository in _seoTypeRepository.Table on seoDetails.CMSSEOTypeId equals seoTypeRepository.CMSSEOTypeId
                                                          where seoDetails.PortalId == portalId && seoTypeRepository.Name == seoType
                                                          select seoDetails).ToList();
            ZnodeLogging.LogMessage("znodeCMSSEODetails count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, znodeCMSSEODetails?.Count);

            return znodeCMSSEODetails;
        }

        //Get SEO Setting
        public SEODetailsModel GetSeoSetting(int seoId, string seoTypeName, int portalId, int localeId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters seoId, seoTypeName, portalId and localeId: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new object[] { seoId, seoTypeName, portalId, localeId });
            IZnodeViewRepository<SEODetailsModel> objStoredProc = new ZnodeViewRepository<SEODetailsModel>();
            objStoredProc.SetParameter("@SeoId", seoId.ToString(), ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@SeoType", seoTypeName, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@LocaleId", localeId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            SEODetailsModel seoDetails = objStoredProc.ExecuteStoredProcedureList("Znode_GetSeoDetails  @SeoId,@SeoType,@LocaleId,@PortalId")?.FirstOrDefault();
            ZnodeLogging.LogMessage("ItemName, CMSSEODetailId, SEOId and SEOCode properties of SEODetailsModel to be returned: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { ItemName = seoDetails?.ItemName, CMSSEODetailId = seoDetails?.CMSSEODetailId, SEOId = seoDetails?.SEOId, SEOCode = seoDetails?.SEOCode });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return seoDetails;
        }

        public ZnodePublishSeoEntity GetPublishSeoSetting(string seoCode, string seoTypeName, int portalId, int localeId)
          => ZnodeDependencyResolver.GetService<IPublishedPortalDataService>().GetSEOEntityByCode(seoCode, seoTypeName, portalId, localeId);



        //Get SEO default setting by portal id.
        public virtual ZnodeCMSPortalSEOSetting GetPortalSeoDefaultSetting(string portalId)
             => _portalSEOSettingRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(PortalFilter(portalId).ToFilterDataCollection()).WhereClause).FirstOrDefault();
        #endregion

        #region Private Method

        //Get filters for SEO
        protected virtual FilterCollection GetFiltersForSEO(int? portalId, int localeId, string seoTypeName)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple("SEOTypeName", FilterOperators.Is, seoTypeName));
            filters.Add(new FilterTuple("LocaleId", FilterOperators.Equals, localeId.ToString()));
            if (HelperUtility.IsNotNull(portalId))
                filters.Add(new FilterTuple("PortalId", FilterOperators.Equals, portalId.ToString()));
            return filters;
        }

        //Generate filters for Portal Id.
        private static FilterCollection PortalFilter(string portalId)
            => new FilterCollection() { new FilterTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId) };
        #endregion
    }
}
