using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services
{
    public partial class StoreLocatorService
    {
        #region Public Methods
        //Gets store locator list. 
        public virtual WebStoreLocatorListModel GetWebStoreLocatorList(FilterCollection filters, NameValueCollection sorts)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, null);
            ZnodeLogging.LogMessage("pageListModel generated to set SP parameters to get store locator list: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            string stateCode = filters.Exists(x => x.FilterName.Equals(ZnodeAddressEnum.StateName.ToString(), StringComparison.InvariantCultureIgnoreCase)) ? filters.Where(x => x.FilterName.Equals(ZnodeAddressEnum.StateName.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FirstOrDefault()?.FilterValue : "";
            ZnodeLogging.LogMessage("stateCode generated: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, stateCode);

            filters.RemoveAll(x => x.FilterName.Equals(ZnodeAddressEnum.StateName.ToString(), StringComparison.InvariantCultureIgnoreCase));

            IZnodeViewRepository<StoreLocatorDataModel> objStoredProc = new ZnodeViewRepository<StoreLocatorDataModel>();
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@StateCode", stateCode, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<StoreLocatorDataModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetStoreDetail @WhereClause,@Rows,@PageNo,@Order_By,@StateCode,@RowCount OUT", 5, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("StoreLocatorDataModel list count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, list?.Count);

            WebStoreLocatorListModel listModel = new WebStoreLocatorListModel { StoreLocators = list?.ToList() };

            listModel.StoreLocators.ForEach(x => x.MapQuestURL = GetMapQuestURL(x, x.CountryName));

            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return listModel;
        }
        #endregion

    }
}
