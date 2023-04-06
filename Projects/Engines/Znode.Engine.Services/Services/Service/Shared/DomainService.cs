using Microsoft.Web.Administration;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Helper;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Services
{
    public class DomainService : BaseService, IDomainService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeDomain> _domainRepository;
        #endregion

        #region Constructor
        public DomainService()
        {
            _domainRepository = new ZnodeRepository<ZnodeDomain>();
        }
        #endregion

        #region Public Methods

        //Method to get domain by domain name.
        public ZnodeDomain GetDomain(string domainName)
            => _domainRepository.Table.Where(x => Equals(x.DomainName, domainName.ToLower()))?.FirstOrDefault();

        //Method to get domain list.
        public DomainListModel GetDomains(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            //Replace filter values for domain.
            ReplaceFilterValuesForDomain(ref filters);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            DomainListModel listModel = new DomainListModel();
            if (CheckCloudflareEnabled(filters))
            {
                filters.RemoveAll(x => x.Item1.Equals(FilterKeys.applicationType, StringComparison.InvariantCultureIgnoreCase));
                pageListModel = new PageListModel(filters, sorts, page);
                
                IZnodeViewRepository<DomainModel> objStoredProc = new ZnodeViewRepository<DomainModel>();
                //SP parameters
                objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
                objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("@RowsCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

                listModel.Domains = objStoredProc.ExecuteStoredProcedureList("Znode_GetDomainList @WhereClause,@Rows,@PageNo,@Order_By,@RowsCount OUT", 4, out pageListModel.TotalRowCount).ToList();
            }
            else
            {
                ZnodeLogging.LogMessage("pageListModel to get domainListEntity: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
                List<ZnodeDomain> domainListEntity = _domainRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, new List<string>() { ZnodeDomainEnum.ZnodePortal.ToString() }, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.ToList();
                listModel.Domains = domainListEntity?.Count > 0 ? domainListEntity.ToModel<DomainModel>().ToList() : new List<DomainModel>();
            }
            ZnodeLogging.LogMessage("Domains count: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, listModel?.Domains?.Count);
            //Set for pagination
            listModel.BindPageListModel(pageListModel);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Method to get domain by domain id.
        public DomainModel GetDomain(int domainId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (domainId > 0)
            {
                FilterCollection filters = new FilterCollection();
                filters.Add(ZnodeDomainEnum.DomainId.ToString(), FilterOperators.Equals, domainId.ToString());
                ZnodeLogging.LogMessage("domainId to get Domain: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, domainId);
                return _domainRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause, new List<string>() { ZnodeDomainEnum.ZnodePortal.ToString() })?.ToModel<DomainModel>();
            }

            return null;
        }

        //Method to Create domain.
        public DomainModel CreateDomain(DomainModel domainModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (IsNull(domainModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);

            //Detached protocol from domain.
            DetachedProtocolFromDomain(domainModel);

            if (IsDomainNameExists(domainModel.DomainName, domainModel.DomainId))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorDomainNameAlreadyExists);

            //Create new domain and return it.
            ZnodeDomain domain = _domainRepository.Insert(domainModel.ToEntity<ZnodeDomain>());

            if (domainModel.ApplicationType == ZnodeConstant.WebStore || domainModel.ApplicationType == ZnodeConstant.WebstorePreview)
            {
                BindURL(domainModel);
                //Refreshing Site and Domain config caches at Business Framework's config manager.
                ZnodeConfigManager.RefreshConfiguration();
            }

            ZnodeLogging.LogMessage((domain?.DomainId > 0) ? string.Format(Admin_Resources.SuccessDomainInsert, domain?.DomainId) : Admin_Resources.ErrorDomainInsert, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            if (IsNotNull(domain))
                return domain.ToModel<DomainModel>();

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return domainModel;
        }

        //Method to update domain
        public bool UpdateDomain(DomainModel domainModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (IsNull(domainModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelCanNotBeNull);

            if (domainModel.DomainId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.IdCanNotBeLessThanOne);

            if (IsDomainNameExists(domainModel.DomainName, domainModel.DomainId))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorDomainNameAlreadyExists);

            //Detached protocol from domain.
            DetachedProtocolFromDomain(domainModel);

            //set 'IsDefault' property to 'False' of records for which application type and portal id are match with input values
            if (domainModel.IsDefault)
                UpdateIsDefaultOfDomain(domainModel);

            //Update domain Information
            bool isDomainUpdated = _domainRepository.Update(domainModel.ToEntity<ZnodeDomain>());
            ZnodeLogging.LogMessage(isDomainUpdated ? string.Format(Admin_Resources.SuccessDomainUpdate, domainModel.DomainId) : Admin_Resources.ErrorDomainUpdate, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return isDomainUpdated;
        }

        //Method to delete domain
        public bool DeleteDomain(ParameterModel domainIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (IsNull(domainIds) || string.IsNullOrEmpty(domainIds.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorDomainIdLessThanOne);

            int[] DomainIds = StringToInt(domainIds.Ids);
            List<string> domainNames = null; List<DomainModel> DefaultDomainlist = null;
            List<DomainModel> Domainlist = _domainRepository.Table.Where(x => DomainIds.Contains(x.DomainId)).ToModel<DomainModel>().ToList();
            if (IsNotNull(Domainlist))
            {
                DefaultDomainlist = Domainlist?.Where(x => Equals(x.IsDefault, true)).ToList();
                if (DefaultDomainlist?.Count == 1)
                {
                    throw new ZnodeException(ErrorCodes.SetDefaultDataError, Admin_Resources.ErrorDefaultDomain);
                }
                domainNames = (Domainlist?.Where(x => Equals(x.IsDefault, false)).Select(x => x.DomainName)).ToList();
                domainIds.Ids = string.Join(",", (Domainlist?.Where(x => Equals(x.IsDefault, false)).Select(x => x.DomainId)).ToArray());
            }

            //Generates filter clause for multiple domainIds.
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeDomainEnum.DomainId.ToString(), ProcedureFilterOperators.In, domainIds.Ids));


            ZnodeLogging.LogMessage("DomainIds to get domain names: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, DomainIds);

            //Returns true if domain deleted successfully else return false.
            bool IsDeleted = _domainRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage(IsDeleted ? string.Format(Admin_Resources.SuccessDomainDelete, DomainIds) : Admin_Resources.ErrorDomainDelete, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            // Remove URL binding from hosted site.
            RemoveHostName(domainNames);

            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (IsDeleted && DefaultDomainlist?.Count > 0)
                throw new ZnodeException(ErrorCodes.NonDefaultUrlDeleteError, Admin_Resources.DeleteNonDefaultDomain);
            return IsDeleted;
        }

        //Convert comma seperated string ids to Integer array
        public int[] StringToInt(string Ids)
        {
            List<int> DomainIds = new List<int>();
            if (Ids.Contains(','))
            {
                var stringDomainIds = Ids.Split(',');
                for (int index = 0; index < stringDomainIds.Length; index++)
                {
                    int num;
                    if (int.TryParse(stringDomainIds[index], out num))
                        DomainIds.Add(num);
                }
            }
            else
                DomainIds.Add(Convert.ToInt32(Ids));

            return DomainIds.ToArray();
        }

        //Method to enable disable domain.
        public bool EnableDisableDomain(DomainModel domainModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            List<ZnodeDomain> list = GetDomain(domainModel);
            ZnodeLogging.LogMessage("Domain list count and IsActive: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { list?.Count, domainModel?.IsActive });
            if (domainModel.IsActive)
                SetActive(list);
            else
                SetDeActive(list);

            list?.ForEach(x => _domainRepository.Update(x));
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return true;
        }

        //Method to enable domain.
        private static void SetActive(List<ZnodeDomain> domainList) => domainList?.ForEach(x => x.IsActive = true);

        //Method to disable domain.
        private static void SetDeActive(List<ZnodeDomain> domainList) => domainList?.ForEach(x => x.IsActive = false);

        //Get Account details of various account ids.
        private List<ZnodeDomain> GetDomain(DomainModel domainModel)
        {
            FilterCollection filterList = new FilterCollection();

            filterList.Add(new FilterTuple(ZnodeDomainEnum.DomainId.ToString(), ProcedureFilterOperators.In, domainModel.DomainIds.ToString()));
            return _domainRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterList.ToFilterDataCollection()).WhereClause).ToList();
        }

        #endregion

        #region Private Methods
        //Check for Existing Domain Names
        private bool IsDomainNameExists(string name, int domainId)
        {
            if (domainId > 0)
            {
                ZnodeLogging.LogMessage("Check domain name exists status based on name and domainId: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, new object[] { name, domainId });
                //Check for Domain Exists or not.
                ZnodeDomain domain = _domainRepository.Table.FirstOrDefault(x => x.DomainId == domainId);
                if (Equals(domain?.DomainName.ToLower(), name.ToLower()))
                    return false;
            }
            return _domainRepository.Table.Any(x => x.DomainName.ToLower() == name.ToLower());
        }

        //Detached protocol from domain.
        private DomainModel DetachedProtocolFromDomain(DomainModel domainModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            // Remove "http://" and "https://" from domain name.
            if (domainModel.DomainName.Contains("http://") || domainModel.DomainName.Contains("https://"))
                domainModel.DomainName = domainModel.DomainName.Replace("http://", string.Empty).Replace("https://", string.Empty);

            // Strip off the port number so we won't conflict with debuggers and other services that may specify a different port.
            if (domainModel.DomainName.Contains(":"))
                domainModel.DomainName = domainModel.DomainName.Substring(0, domainModel.DomainName.IndexOf(":"));

            // Remove any trailing "/"
            if (domainModel.DomainName.EndsWith("/"))
                domainModel.DomainName = domainModel.DomainName.Remove(domainModel.DomainName.Length - 1);

            ZnodeLogging.LogMessage("DomainName property of DomainModel: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, domainModel?.DomainName);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return domainModel;
        }
        /// <summary>
        /// set 'IsDefault' property to 'False' of records for which application type and portal id are match with input values
        /// </summary>
        /// <param name="domainModel"></param>
        private void UpdateIsDefaultOfDomain(DomainModel domainModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            FilterCollection filterList = new FilterCollection();
            filterList.Add(new FilterTuple(ZnodeDomainEnum.DomainId.ToString(), ProcedureFilterOperators.NotEquals, domainModel.DomainId.ToString()));
            filterList.Add(new FilterTuple(ZnodeDomainEnum.PortalId.ToString(), ProcedureFilterOperators.Equals, domainModel.PortalId.ToString()));
            filterList.Add(new FilterTuple(ZnodeDomainEnum.ApplicationType.ToString(), ProcedureFilterOperators.Is, domainModel.ApplicationType.ToString()));
            filterList.Add(new FilterTuple(ZnodeDomainEnum.IsDefault.ToString(), ProcedureFilterOperators.Equals, "True"));
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filterList.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause to get domain list: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, whereClauseModel?.WhereClause);
            List<ZnodeDomain> list = _domainRepository.GetEntityList(whereClauseModel.WhereClause, whereClauseModel.FilterValues).ToList();
            if (domainModel.ApplicationType.Equals(ApplicationTypesEnum.WebstorePreview.ToString()))
                list = list?.Where(s => s.ApplicationType.Equals(ApplicationTypesEnum.WebstorePreview.ToString())).ToList();
            else
                list = list?.Where(s => s.ApplicationType.Equals(ApplicationTypesEnum.WebStore.ToString())).ToList();
            ZnodeLogging.LogMessage("Domain list count: ", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Verbose, list?.Count);
            list?.ForEach(x => { x.IsDefault = false; _domainRepository.Update(x); });
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
        }
        //Replace the value name of filters to get filtered data from db
        private void ReplaceFilterValuesForDomain(ref FilterCollection filters)
        {
            foreach (FilterTuple tuple in filters)
                if (tuple.Item1 == ZnodeDomainEnum.DomainName.ToString().ToLower() || tuple.Item1.Contains($"{ZnodeDomainEnum.DomainName.ToString().ToLower()}|"))
                    ReplaceFilterValueName(ref filters, tuple.Item1);
        }

        /// <summary>
        /// Replace filter value name 
        /// </summary>
        /// <param name="filters">Reference of filter collection</param>
        /// <param name="valueName">value Name</param>
        private void ReplaceFilterValueName(ref FilterCollection filters, string valueName)
        {
            FilterCollection tempCollection = new FilterCollection();
            tempCollection = filters;
            FilterCollection newCollection = new FilterCollection();

            foreach (var tuple in filters)
            {
                if (Equals(tuple.Item1.ToLower(), valueName.ToLower()))
                    newCollection.Add(new FilterTuple(tuple.Item1, tuple.Item2, tuple.Item3.Replace("http://", "").Replace("https://", "")));
            }
            foreach (var temp in tempCollection)
            {
                if (!Equals(temp.Item1.ToLower(), valueName.ToLower()))
                    newCollection.Add(temp);
            }
            filters = newCollection;
        }

        //Method to add URL binding for hosted site.
        private void BindURL(DomainModel domainModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage(Admin_Resources.EnterBindURLMethod, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            try
            {
                ServerManager serverManager = new ServerManager();
                ZnodeLogging.LogMessage(Admin_Resources.CreatedServerManagerClassObject, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

                //Get side to for which binding need to add
                Site site = serverManager.Sites.FirstOrDefault(s => s.Name == ZnodeApiSettings.WebstoreWebsiteName.ToString());
                ZnodeLogging.LogMessage(Admin_Resources.HostedWebstoreSiteName + site, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

                if (!Equals(site, null))
                {
                    //Get port id for binding going to add
                    int port = site.Bindings.Select(e => e.EndPoint.Port).FirstOrDefault();
                    ZnodeLogging.LogMessage(Admin_Resources.HostedWebstorePortId + port, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

                    //Add binding to site
                    site.Bindings.Add("*:" + port + ":" + domainModel.DomainName, "http");

                    // Commit the changes
                    serverManager.CommitChanges();
                }
                else
                    ZnodeLogging.LogMessage(string.Format(Admin_Resources.HostedWebstoreNameNotFound, ZnodeApiSettings.WebstoreWebsiteName), ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
        }

        //Method to Remove URL binding from hosted site.
        public void RemoveHostName(List<string> domainNames)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);

            try
            {
                if (!string.IsNullOrEmpty(ZnodeApiSettings.WebstoreWebsiteName))
                {
                    ServerManager serverMgr = new ServerManager();
                    Site mySite = serverMgr.Sites[ZnodeApiSettings.WebstoreWebsiteName];

                    if (!Equals(mySite, null))
                    {
                        foreach (var hostname in domainNames)
                        {
                            for (int index = 0; index < mySite.Bindings.Count; index++)
                            {
                                if (mySite.Bindings[index].Host == hostname)
                                {
                                    mySite.Bindings.RemoveAt(index);
                                    break;
                                }
                            }
                        }
                        serverMgr.CommitChanges();
                    }
                }
                else
                {
                    ZnodeLogging.LogMessage("WebstoreWebsiteName is blank.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                }
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
        }
        #endregion

        #region Protected Methods
        //check IsCloudflare Enabled.
        protected bool CheckCloudflareEnabled(FilterCollection filters)
        {
            return filters.Contains(new FilterTuple(FilterKeys.applicationType.ToLower(), "eq", ApplicationCacheTypeEnum.CloudflareCache.ToString().ToLower()));
        }
        #endregion
    }
}
