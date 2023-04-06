using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;

using Znode.Engine.Api.Models;
using Znode.Engine.ERPConnector;
using Znode.Engine.Exceptions;
using Znode.Engine.Hangfire;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;


using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Engine.Services.Helper;
using Znode.Libraries.Caching.Events;

namespace Znode.Engine.Services
{
    public class ERPConfiguratorService : BaseService, IERPConfiguratorService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeERPConfigurator> _eRPConfiguratorRepository;
        private readonly IZnodeRepository<ZnodeERPTaskScheduler> _erpTaskSchedulerRepository;
        private readonly IERPJobs _eRPJobs;
        #endregion

        #region Constructor
        public ERPConfiguratorService()
        {
            _eRPConfiguratorRepository = new ZnodeRepository<ZnodeERPConfigurator>();
            _erpTaskSchedulerRepository = new ZnodeRepository<ZnodeERPTaskScheduler>();
            _eRPJobs = GetService<IERPJobs>();
        }
        #endregion

        #region Public Methods

        // Get ERPConfigurator Classes List from Class.
        public virtual ERPConfiguratorListModel GetERPConfiguratorClassesList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("PageListModel to generate ERPConfiguratorClassesList: ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            int eRPConfiguratorId = Convert.ToInt32(filters.Where(x => x.FilterName.Equals(ZnodeERPConfiguratorEnum.ERPConfiguratorId.ToString().ToLower()))?.Select(y => y.FilterValue)?.FirstOrDefault());
            filters.RemoveAll(x => x.Item1 == ZnodeERPConfiguratorEnum.ERPConfiguratorId.ToString().ToLower());

            string associatedERPConfiguratorIds = GetERPConfiguratorId(eRPConfiguratorId);
            ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info,new{ associatedERPConfiguratorIds= associatedERPConfiguratorIds, eRPConfiguratorId = eRPConfiguratorId});
            if (!string.IsNullOrEmpty(associatedERPConfiguratorIds))
            {
                filters.Add(new FilterTuple(ZnodeERPConfiguratorEnum.ERPConfiguratorId.ToString(), FilterOperators.NotIn, associatedERPConfiguratorIds));
            }
            //gets the where clause with filter Values.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClauseModel to generate ERPConfiguratorClassesList: ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            ERPConfiguratorListModel eRPConfiguratorListModel = new ERPConfiguratorListModel();
            IList<ZnodeERPConfigurator> ERPConfiguratorClassesList = _eRPConfiguratorRepository.GetPagedList(whereClauseModel.WhereClause, pageListModel.OrderBy, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("ERPConfiguratorClassesList list count:", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, ERPConfiguratorClassesList?.Count());
            foreach (ZnodeERPConfigurator ERPConfiguratorClasses in ERPConfiguratorClassesList)
                eRPConfiguratorListModel.ERPConfiguratorList.Add(ERPConfiguratorClasses.ToModel<ERPConfiguratorModel>());

            eRPConfiguratorListModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return eRPConfiguratorListModel;
        }

        // Get ERPConfigurator list from database.
        public virtual ERPConfiguratorListModel GetERPConfiguratorList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("PageListModel to generate eRPConfiguratorList: ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //maps the entity list to model
            IList<ZnodeERPConfigurator> eRPConfiguratorList = _eRPConfiguratorRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("eRPConfiguratorList list count:", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, eRPConfiguratorList?.Count());
            ERPConfiguratorListModel listModel = new ERPConfiguratorListModel();
            listModel.ERPConfiguratorList = eRPConfiguratorList?.Count > 0 ? eRPConfiguratorList.ToModel<ERPConfiguratorModel>().ToList() : new List<ERPConfiguratorModel>();
            foreach (var item in listModel.ERPConfiguratorList)
            {
                item.Status = item.IsActive;
                item.IsActive = !item.IsActive;
            }
            //Set for pagination
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return listModel;
        }

        // Get all ERP Configurator Classes which are not present in database.
        public virtual ERPConfiguratorListModel GetAllERPConfiguratorClassesNotInDatabase()
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            ERPConfiguratorListModel ERPConfiguratorList = new ERPConfiguratorListModel();

            //Get names of classes from assembly.
            Assembly assembly = Assembly.Load("Znode.Engine.ERPConnector");
            List<string> libraryClasslist = assembly?.GetTypes()?.Where(type => type.IsSubclassOf(typeof(BaseERP))).Select(x => x.Name).ToList();
            libraryClasslist?.Remove("SalesOrderMapper");
            libraryClasslist?.Remove("AuthenticationHelper");
            ZnodeLogging.LogMessage("libraryClasslist list count:", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, libraryClasslist?.Count());
            //Get names of classes from database.
            ERPConfiguratorListModel availableERPConfiguratorClasses = GetERPConfiguratorClassesList(new FilterCollection(), new NameValueCollection(), new NameValueCollection());
            var dbClassName = availableERPConfiguratorClasses?.ERPConfiguratorList?.Select(x => x.ClassName).ToList();
            ZnodeLogging.LogMessage("dbClassName:", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info, dbClassName);
            if (IsNotNull(dbClassName))
                foreach (var Class in dbClassName)
                    libraryClasslist?.Remove(Class);

            if (IsNotNull(libraryClasslist))
                foreach (var LibraryClass in libraryClasslist)
                    ERPConfiguratorList.ERPConfiguratorList.Add(ERPConfiguratorMap.AddClassNameToModel(LibraryClass));
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return ERPConfiguratorList;
        }

        //Create eRPConfigurator.
        public virtual ERPConfiguratorModel Create(ERPConfiguratorModel eRPConfiguratorModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            if (IsNull(eRPConfiguratorModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            //Create new eRPConfigurator and return it.
            ZnodeERPConfigurator eRPConfigurator = _eRPConfiguratorRepository.Insert(eRPConfiguratorModel.ToEntity<ZnodeERPConfigurator>());

            ClearCacheHelper.EnqueueEviction(new StaleApiRoutesEvent()
            {
                Comment = $"Clear the ActiveERPConfiguration cache key on creating the ERP connector",
                RouteTemplateKeys = new string[] { CachedKeys.ActiveERPConfiguration }
            });

            ZnodeLogging.LogMessage("Inserted ERPConfigurator with Name ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, eRPConfigurator?.ERPName);
            if (eRPConfigurator?.ERPConfiguratorId > 0)
            {
                if (eRPConfiguratorModel.IsActive)
                {
                    List<ZnodeERPConfigurator> znodeERPConfigurator = _eRPConfiguratorRepository.Table.Where(x => x.ERPConfiguratorId != eRPConfigurator.ERPConfiguratorId && x.IsActive).ToList();
                    znodeERPConfigurator?.ForEach(item =>
                    {
                        item.IsActive = false;
                        _eRPConfiguratorRepository.Update(item);
                    });
                    ZnodeLogging.LogMessage("znodeERPConfigurator list count:", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, znodeERPConfigurator?.Count());
                }
                eRPConfiguratorModel = eRPConfigurator.ToModel<ERPConfiguratorModel>();
            }

            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return eRPConfiguratorModel;
        }

        //Get eRPConfigurator by eRPConfigurator id.
        public virtual ERPConfiguratorModel GetERPConfigurator(int eRPConfiguratorId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Paramters eRPConfiguratorId:", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info, eRPConfiguratorId);
            if (eRPConfiguratorId <= 0)
                return null;
            //Get the eRPConfigurator Details based on id.
            ZnodeERPConfigurator eRPConfigurator = _eRPConfiguratorRepository.Table.FirstOrDefault(x => x.ERPConfiguratorId == eRPConfiguratorId);
            ERPConfiguratorModel eRPConfiguratorModel = eRPConfigurator.ToModel<ERPConfiguratorModel>();
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return eRPConfiguratorModel;
        }

        //Update eRPConfigurator.
        public virtual bool Update(ERPConfiguratorModel eRPConfiguratorModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            if (IsNull(eRPConfiguratorModel))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelNotNull);
            if (eRPConfiguratorModel.ERPConfiguratorId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.IdCanNotBeLessThanOne);

            bool isERPConfiguratorUpdated = false;
            if (eRPConfiguratorModel?.ERPConfiguratorId > 0)
            {
                //Update eRPConfigurator
                isERPConfiguratorUpdated = _eRPConfiguratorRepository.Update(eRPConfiguratorModel.ToEntity<ZnodeERPConfigurator>());

                ClearCacheHelper.EnqueueEviction(new StaleApiRoutesEvent()
                {
                    Comment = $"Clear the ActiveERPConfiguration cache key on updating the ERP connector",
                    RouteTemplateKeys = new string[] { CachedKeys.ActiveERPConfiguration }
                });

                if (isERPConfiguratorUpdated && eRPConfiguratorModel.IsActive)
                {
                    List<ZnodeERPConfigurator> znodeERPConfigurator = _eRPConfiguratorRepository.Table.Where(x => x.ERPConfiguratorId != eRPConfiguratorModel.ERPConfiguratorId && x.IsActive).ToList();
                    znodeERPConfigurator?.ForEach(item =>
                    {
                        item.IsActive = false;
                        _eRPConfiguratorRepository.Update(item);
                    });
                    ZnodeLogging.LogMessage("znodeERPConfigurator list count:", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, znodeERPConfigurator?.Count());
                }
            }

            ZnodeLogging.LogMessage(isERPConfiguratorUpdated ? Admin_Resources.SuccessERPConfiguratorUpdate : Admin_Resources.ErrorERPConfiguratorUpdate, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return isERPConfiguratorUpdated;
        }

        //Delete eRPConfigurator.
        public virtual bool Delete(ParameterModel eRPConfiguratorIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            if (IsNull(eRPConfiguratorIds) || string.IsNullOrEmpty(eRPConfiguratorIds.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ERPClassNotLessThanOne);

            if (Array.IndexOf(eRPConfiguratorIds.Ids.Split(','), GetActiveERPClassId().ToString()) > -1)
                throw new ZnodeException(ErrorCodes.NotDeleteActiveRecord, Admin_Resources.SuccessERPClassDelete);

            int[] eRPConfiguratorIdsArray = eRPConfiguratorIds.Ids.Split(',').Select(int.Parse).ToArray();
            var TaskSchedulerNames = _erpTaskSchedulerRepository.Table.Where(item => eRPConfiguratorIdsArray.Contains((int)item.ERPConfiguratorId)).Select(x => x.SchedulerName).ToList();
            ZnodeLogging.LogMessage("TaskSchedulerNames list count:", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, TaskSchedulerNames?.Count());
            bool schedulerStatus = (TaskSchedulerNames?.Count) <= 0 || _eRPJobs.RemoveJobs(TaskSchedulerNames);

            if (schedulerStatus)
            {
                int status = 0;
                IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
                objStoredProc.SetParameter(ZnodeERPConfiguratorEnum.ERPConfiguratorId.ToString(), eRPConfiguratorIds.Ids, ParameterDirection.Input, DbType.String);
                objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                objStoredProc.ExecuteStoredProcedureList("Znode_DeleteERPConfigurator @ERPConfiguratorId,  @Status OUT", 1, out status);
                if (status == 1)
                {
                    ZnodeLogging.LogMessage(Admin_Resources.SuccessERPConfigDelete, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                    return true;
                }
                else
                {
                    ZnodeLogging.LogMessage(Admin_Resources.ErrorERPConfigDelete, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                    return false;
                }
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorERPConfigDelete, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                return false;
            }
        }

        //Method to Activate or Deactivate a ERPConfigurator.
        public virtual bool EnableDisableERPConfigurator(string eRPConfiguratorId, bool isActive)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            if (Convert.ToInt32(eRPConfiguratorId) < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.IdCanNotBeLessThanOne);

            bool isERPConfiguratorUpdated = false;
            if (Convert.ToInt32(eRPConfiguratorId) > 0)
            {
                ERPConfiguratorModel eRPConfiguratorModel = GetERPConfigurator(Convert.ToInt32(eRPConfiguratorId));
                ZnodeLogging.LogMessage("Erp Id and Status:", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, new object[] { eRPConfiguratorModel.ERPConfiguratorId, eRPConfiguratorModel.IsActive });
                //Update eRPConfigurator
                eRPConfiguratorModel.IsActive = isActive;
                isERPConfiguratorUpdated = _eRPConfiguratorRepository.Update(eRPConfiguratorModel.ToEntity<ZnodeERPConfigurator>());

                ClearCacheHelper.EnqueueEviction(new StaleApiRoutesEvent()
                {
                    Comment = $"Clear the ActiveERPConfiguration cache key on Enable and Disable the ERP connector",
                    RouteTemplateKeys = new string[] { CachedKeys.ActiveERPConfiguration }
                });

                if (isERPConfiguratorUpdated && eRPConfiguratorModel.IsActive)
                {
                    List<ZnodeERPConfigurator> znodeERPConfigurator = _eRPConfiguratorRepository.Table.Where(x => x.ERPConfiguratorId != eRPConfiguratorModel.ERPConfiguratorId && x.IsActive).ToList();
                    znodeERPConfigurator?.ForEach(item =>
                    {
                        item.IsActive = false;
                        _eRPConfiguratorRepository.Update(item);
                    });
                    ZnodeLogging.LogMessage("znodeERPConfigurator list count:", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, znodeERPConfigurator?.Count());

                }
            }

            //Enable Disable task Scheduler
            EnableDisableScheduler(eRPConfiguratorId, isActive);

            if (isActive)
                ZnodeLogging.LogMessage(isERPConfiguratorUpdated ? Admin_Resources.SuccessERPConfiguratorEnable : Admin_Resources.ErrorERPConfiguratorEnable, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            else
                ZnodeLogging.LogMessage(isERPConfiguratorUpdated ? Admin_Resources.SuccessERPConfiguratorDisable : Admin_Resources.ErrorERPConfiguratorDisable, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return isERPConfiguratorUpdated;
        }


        //Method for class name of active ERP. 
        public virtual string GetActiveERPClassName()
        {
            return GetActiveERPConfigurationFromCache()?.ClassName;
        }

        //Method for Active class Id. 
        public virtual int GetActiveERPClassId() 
            => HelperUtility.IsNull(GetActiveERPConfigurationFromCache()) ? 0 : GetActiveERPConfigurationFromCache().ERPConfiguratorId;

        //Method for class name of active ERP. 
        public virtual string GetERPClassName() 
            => GetActiveERPConfigurationFromCache()?.ERPName;

        #endregion

        #region Private Methods
        // Get eRPConfiguratorId on the basis of eRPConfigurator id.
        private string GetERPConfiguratorId(int eRPConfiguratorId)
        {
            FilterCollection eRPConfiguratorClassFilterCollection = new FilterCollection { new FilterTuple(ZnodeERPConfiguratorEnum.ERPConfiguratorId.ToString(), FilterOperators.Equals, eRPConfiguratorId.ToString()) };
            EntityWhereClauseModel ERPConfiguratorWhereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(eRPConfiguratorClassFilterCollection.ToFilterDataCollection());

            return string.Join(",", _eRPConfiguratorRepository.GetEntityList(ERPConfiguratorWhereClauseModel.WhereClause).Select(x => x.ERPConfiguratorId).ToArray());
        }

        //Method for Enable Disable task Scheduler
        private void EnableDisableScheduler(string eRPConfiguratorId, bool isActive)
        {
            int erpConfiguratorId = Convert.ToInt32(eRPConfiguratorId);
            List<ZnodeERPTaskScheduler> znodeERPTaskSchedulerList = _erpTaskSchedulerRepository.Table.Where(x => x.ERPConfiguratorId == erpConfiguratorId).ToList();
            ZnodeLogging.LogMessage("znodeERPTaskSchedulerList list count:", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, znodeERPTaskSchedulerList?.Count());
            if (znodeERPTaskSchedulerList?.Count > 0)
            {
                znodeERPTaskSchedulerList?.ForEach(item =>
                {
                    new ERPTaskSchedulerService().EnableDisableTaskScheduler(item.ERPTaskSchedulerId, isActive);
                });

            }
        }

        // Get active erp configurator list from cache.
        private ZnodeERPConfigurator GetActiveERPConfigurationFromCache()
        {
            string cacheKey = "ActiveERPConfiguration";            
            if (HelperUtility.IsNull(HttpRuntime.Cache.Get(cacheKey)) || IsCacheRefresh())
            {
                ZnodeERPConfigurator znodeERPConfigurator = _eRPConfiguratorRepository.Table.Where(x => x.IsActive).FirstOrDefault();

                if (HelperUtility.IsNotNull(znodeERPConfigurator))
                {
                    HttpRuntime.Cache[cacheKey] = znodeERPConfigurator;
                }
                else
                {
                    HttpRuntime.Cache[cacheKey] = new ZnodeERPConfigurator();
                }
            }
            ZnodeERPConfigurator cachedERPConfigurator = (ZnodeERPConfigurator)HttpRuntime.Cache.Get(cacheKey);
            if (cachedERPConfigurator.ERPConfiguratorId == 0)
            {
                return null;
            }
            return cachedERPConfigurator;

        }
        #endregion
    }
}
