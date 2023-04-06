using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

namespace Znode.Engine.Services
{
    public class UrlRedirectService : BaseService, IUrlRedirectService
    {
        #region Private Variable
        private readonly IZnodeRepository<ZnodeCMSUrlRedirect> _urlRedirectRepository;
        private readonly IZnodeRepository<ZnodeCMSSEODetail> _seoDetailRepository;
        #endregion

        #region Constructor
        public UrlRedirectService()
        {
            _urlRedirectRepository = new ZnodeRepository<ZnodeCMSUrlRedirect>();
            _seoDetailRepository = new ZnodeRepository<ZnodeCMSSEODetail>();
        }
        #endregion

        #region Public Methods

        //Gets the list of Url Redirects.
        public virtual UrlRedirectListModel GetUrlRedirectList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            int portalId = Convert.ToInt32(filters.Find(x => string.Equals(x.FilterName, ZnodePortalEnum.PortalId.ToString(), StringComparison.CurrentCultureIgnoreCase))?.FilterValue);
            ZnodeLogging.LogMessage("portalId generated from filters:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, portalId);

            //Check for User Portal Access.
            CheckUserPortalAccess(portalId);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel generated to get url redirects list: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IList<ZnodeCMSUrlRedirect> list = _urlRedirectRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("Url redirects list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, list?.Count);

            UrlRedirectListModel listModel = new UrlRedirectListModel();
            listModel.UrlRedirects = list?.Count > 0 ? list.ToModel<UrlRedirectModel>().ToList() : null;
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Get url redirect.
        public virtual UrlRedirectModel GetUrlRedirect(FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (filters?.Count < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorFiltersEmpty);

            //gets the where clause with filter Values.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated to get url redirect: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, whereClauseModel.WhereClause);

            return _urlRedirectRepository.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.ToModel<UrlRedirectModel>();
        }

        //Create Url Redirect.
        public virtual UrlRedirectModel Create(UrlRedirectModel urlRedirectModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNull(urlRedirectModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorURLRedirectModelNull);

            if (IsNotValidURL(urlRedirectModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.EnterValidData);

            List<ZnodeCMSUrlRedirect> urlRedirectList = _urlRedirectRepository.GetEntityList(string.Empty).ToList();
            ZnodeLogging.LogMessage("Url redirect list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, urlRedirectList?.Count);

            if (CheckLoopStatus(urlRedirectModel, urlRedirectList))
                throw new ZnodeException(ErrorCodes.NotPermitted, Admin_Resources.ErrorURLRedirectLoop);

            ZnodeCMSUrlRedirect urlRedirect = _urlRedirectRepository.Insert(urlRedirectModel.ToEntity<ZnodeCMSUrlRedirect>());
            if (urlRedirect?.CMSUrlRedirectId > 0)
            {
                urlRedirectModel.CMSUrlRedirectId = urlRedirect.CMSUrlRedirectId;
                ZnodeLogging.LogMessage(Admin_Resources.SuccessUrlRedirectCreated, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            }
            else
                ZnodeLogging.LogMessage(Admin_Resources.ErrorUrlRedirectCreate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage("urlRedirectModel to be returned: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, urlRedirectModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            return urlRedirectModel;
        }

        //Update url redirect.
        public virtual bool Update(UrlRedirectModel urlRedirectModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNull(urlRedirectModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorURLRedirectModelNull);
            ZnodeLogging.LogMessage("Input parameter urlredirectModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, urlRedirectModel);
            if (IsNotValidURL(urlRedirectModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.EnterValidData);

            List<ZnodeCMSUrlRedirect> urlRedirectList = _urlRedirectRepository.GetEntityList(string.Empty).ToList();
            ZnodeLogging.LogMessage("Url redirect list count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, urlRedirectList?.Count);

            if (CheckLoopStatus(urlRedirectModel, urlRedirectList))
                throw new ZnodeException(ErrorCodes.NotPermitted, Admin_Resources.ErrorURLRedirectLoop);

            bool isUpdated = _urlRedirectRepository.Update(urlRedirectModel.ToEntity<ZnodeCMSUrlRedirect>());
          
            ZnodeLogging.LogMessage(isUpdated ? string.Format(Admin_Resources.SuccessUrlRedirectUpdateWithId, urlRedirectModel.CMSUrlRedirectId)
                : Admin_Resources.ErrorUrlRedirectUpdate, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return isUpdated;
        }

        //Delete url redirect.
        public virtual bool Delete(ParameterModel urlRedirectIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            if (Equals(urlRedirectIds, null) || string.IsNullOrEmpty(urlRedirectIds.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorUrlRedirectIdsNull);

            ZnodeLogging.LogMessage("Input parameter urlredirectIds: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, urlRedirectIds?.Ids);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeCMSUrlRedirectEnum.CMSUrlRedirectId.ToString(), ProcedureFilterOperators.In, urlRedirectIds.Ids.ToString()));

            EntityWhereClauseModel entityWhereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated to get url redirect: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, entityWhereClauseModel.WhereClause);

            bool isDeleted = _urlRedirectRepository.Delete(entityWhereClauseModel.WhereClause);

            ZnodeLogging.LogMessage(isDeleted ? string.Format(Admin_Resources.SuccessUrlRedirectDeleted, urlRedirectIds.Ids)  : Admin_Resources.ErrorUrlRedirectDelete, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return isDeleted;
        }
        #endregion

        #region Private Methods
        // Check whether the From URL is repeated or not.
        //Returns true if the URL creates cycle else return false.
        private bool CheckLoopStatus(UrlRedirectModel urlredirectModel, List<ZnodeCMSUrlRedirect> urlRedirectList)
        {
            if (HelperUtility.IsNotNull(urlRedirectList))
                return urlRedirectList.Any(x =>
                System.Convert.ToBoolean((x.RedirectFrom?.Equals(urlredirectModel.RedirectTo, System.StringComparison.InvariantCultureIgnoreCase) &
                                         !Equals(x.CMSUrlRedirectId, urlredirectModel.CMSUrlRedirectId) & Equals(x.PortalId, urlredirectModel.PortalId)) |
                                         ((x.RedirectTo?.Equals(urlredirectModel.RedirectFrom, System.StringComparison.InvariantCultureIgnoreCase) |
                                            x.RedirectFrom?.Equals(urlredirectModel.RedirectFrom, System.StringComparison.InvariantCultureIgnoreCase)) &
                                         !Equals(x.CMSUrlRedirectId, urlredirectModel.CMSUrlRedirectId) & Equals(x.PortalId, urlredirectModel.PortalId))));
            return false;
        }
        // Checks whether is not a valid URL.
        // Returns true if the URL is not a valid URL else return false.
        private bool IsNotValidURL(UrlRedirectModel urlredirectModel)
            => Equals(urlredirectModel.RedirectFrom.ToLower(), urlredirectModel.RedirectTo.ToLower());

        #endregion
    }
}
