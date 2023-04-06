using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Constants;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class AccessPermissionService : BaseService, IAccessPermissionService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeAccountPermission> _accountPermissionRepository;
        private readonly IZnodeRepository<ZnodeAccountPermissionAccess> _accountPermissionAccessRepository;
        private readonly IZnodeRepository<ZnodeAccessPermission> _accessPermissionRepository;
        #endregion

        #region Constructor
        public AccessPermissionService()
        {
            _accountPermissionRepository = new ZnodeRepository<ZnodeAccountPermission>();
            _accountPermissionAccessRepository = new ZnodeRepository<ZnodeAccountPermissionAccess>();
            _accessPermissionRepository = new ZnodeRepository<ZnodeAccessPermission>();
        }
        #endregion

        #region Public Methods
        public virtual AccessPermissionListModel AccountPermissionList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("AccountPermissionList method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to set SP parameter to get accessPermissionsList: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IZnodeViewRepository<View_GetAccountAccessPermission> objStoredProc = new ZnodeViewRepository<View_GetAccountAccessPermission>();
            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", null, ParameterDirection.Output, DbType.Int32);

            IList<View_GetAccountAccessPermission> accessPermissionsList = objStoredProc.ExecuteStoredProcedureList("Znode_GetAccountAccessPermission @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("accessPermissionsList count: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, accessPermissionsList?.Count());

            AccessPermissionListModel list = new AccessPermissionListModel();
            if (accessPermissionsList?.Count > 0)
            {
                list.AccountPermissions = new List<AccessPermissionModel>();
                list.AccountPermissions = accessPermissionsList.ToModel<AccessPermissionModel>().ToList();
            }

            //Set for pagination.
            list.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("AccountPermissionList method execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return list;
        }

        public virtual AccessPermissionModel GetAccountPermission(NameValueCollection expands, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("GetAccountPermission method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated to get accessPermisssion: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, whereClause?.WhereClause);

            ZnodeAccountPermission accessPermisssion = _accountPermissionRepository.GetEntity(whereClause.WhereClause, GetExpands(expands), whereClause.FilterValues);
            if (HelperUtility.IsNotNull(accessPermisssion))
            {
                AccessPermissionModel accessPermissionModel = accessPermisssion.ToModel<AccessPermissionModel>();
                accessPermissionModel.AccountPermissionAccessList = accessPermisssion?.ZnodeAccountPermissionAccesses?.ToModel<AccountPermissionAccessModel>().ToList();
                ZnodeLogging.LogMessage("AccessPermissionId value of accessPermissionModel: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, accessPermissionModel?.AccessPermissionId);
                return accessPermissionModel;
            }
            return null;
        }

        public virtual AccessPermissionListModel AccessPermissionList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("AccessPermissionList method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to get ZnodeAccessPermission list: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());

            IList<ZnodeAccessPermission> list = _accessPermissionRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("AccessPermission list count: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, list?.Count());

            AccessPermissionListModel listModel = new AccessPermissionListModel();
            listModel.AccountPermissions = list?.Count > 0 ? list.ToModel<AccessPermissionModel>().ToList() : null;

            //Set for pagination.
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("AccessPermissionList method execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return listModel;
        }

        public virtual bool DeleteAccountPermission(ParameterModel ids)
        {
            ZnodeLogging.LogMessage("DeleteAccountPermission method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            if (Equals(ids, null) || string.IsNullOrEmpty(ids.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorAccountPermissionIdNull);

            FilterTuple filters = new FilterTuple(ZnodeAccountPermissionEnum.AccountPermissionId.ToString(), ProcedureFilterOperators.Equals, ids.Ids.ToString());
            FilterCollection filtersList = new FilterCollection();
            filtersList.Add(filters);

            EntityWhereClauseModel whereClauseForDelete = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersList.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated for delete: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, whereClauseForDelete?.WhereClause);
            _accountPermissionAccessRepository.Delete(whereClauseForDelete.WhereClause, whereClauseForDelete.FilterValues);
            return _accountPermissionRepository.Delete(whereClauseForDelete.WhereClause, whereClauseForDelete.FilterValues);
        }

        public virtual AccessPermissionModel CreateAccountPermission(AccessPermissionModel accessPermissionModel)
        {
            ZnodeLogging.LogMessage("CreateAccountPermission method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            if (Equals(accessPermissionModel, null))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorAccountPermissionModelNull);

            ZnodeAccountPermission znodeAccountPermission = _accountPermissionRepository.Insert(new ZnodeAccountPermission { AccountId = accessPermissionModel.AccountId, AccountPermissionName = accessPermissionModel.AccountPermissionName });
            ZnodeLogging.LogMessage("Inserted account permission with AccountId and AccountPermissionName: ", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, new { AccountId = accessPermissionModel?.AccountId, AccountPermissionName = accessPermissionModel?.AccountPermissionName });

            if (znodeAccountPermission?.AccountPermissionId > 0)
            {
                accessPermissionModel.AccountPermissionId = znodeAccountPermission.AccountPermissionId;
                accessPermissionModel.AccessPermissionId = Convert.ToInt32(_accountPermissionAccessRepository.Insert(new ZnodeAccountPermissionAccess { AccountPermissionId = znodeAccountPermission.AccountPermissionId, AccessPermissionId = accessPermissionModel.AccessPermissionId })?.AccessPermissionId);
            }
            else
                throw new ZnodeException(ErrorCodes.ExceptionalError, Admin_Resources.ErrorCreateAccountPermission);
            ZnodeLogging.LogMessage("CreateAccountPermission method execution done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return accessPermissionModel;
        }

        public virtual bool UpdateAccountPermission(AccessPermissionModel accessPermissionModel)
        {
            ZnodeLogging.LogMessage("UpdateAccountPermission method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            if (Equals(accessPermissionModel, null))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorAccountPermissionModelNull);

            ZnodeAccountPermission accountPermission = accessPermissionModel.ToEntity<ZnodeAccountPermission>();

            _accountPermissionRepository.Update(accountPermission);
            ZnodeLogging.LogMessage("Inserted AccountPermission with id: ", ZnodeLogging.Components.Warehouse.ToString(), TraceLevel.Verbose, accountPermission?.AccountPermissionId);

            FilterTuple filters = new FilterTuple(ZnodeAccountPermissionAccessEnum.AccountPermissionId.ToString(), ProcedureFilterOperators.Equals, accessPermissionModel.AccountPermissionId.ToString());
            FilterCollection filtersList = new FilterCollection();
            filtersList.Add(filters);

            EntityWhereClauseModel whereClauseForDelete = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersList.ToFilterDataCollection());
            ZnodeLogging.LogMessage("WhereClause generated for delete: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, whereClauseForDelete?.WhereClause);

            ZnodeAccountPermissionAccess entityToUpdate = _accountPermissionAccessRepository.GetEntity(whereClauseForDelete.WhereClause, whereClauseForDelete.FilterValues);

            if (HelperUtility.IsNotNull(entityToUpdate))
            {
                if (entityToUpdate.AccessPermissionId == accessPermissionModel.AccessPermissionId)
                    return true;
                entityToUpdate.AccessPermissionId = accessPermissionModel.AccessPermissionId;
                return _accountPermissionAccessRepository.Update(entityToUpdate);
            }
            return false;
        }
        #endregion

        #region Private Methods
        //Gets the list of expands.
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (!Equals(expands, null) && expands.HasKeys())
            {
                foreach (string key in expands.Keys)
                {
                    var value = expands.Get(key);
                    if (Equals(value, ExpandKeys.ZnodeAccountPermissionAccesses)) { SetExpands(ZnodeAccountPermissionEnum.ZnodeAccountPermissionAccesses.ToString(), navigationProperties); }
                }
            }
            return navigationProperties;
        }
        #endregion
    }
}
