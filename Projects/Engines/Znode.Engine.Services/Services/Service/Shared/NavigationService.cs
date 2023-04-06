using System;
using System.Data;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using System.Diagnostics;
using Znode.Libraries.Framework.Business;
namespace Znode.Engine.Services
{
    public class NavigationService : BaseService, INavigationService
    {
        public NavigationModel GetNavigationDetails(NavigationParamModel model)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);
            NavigationModel navigationModel = new NavigationModel();
            IZnodeViewRepository<View_EntityDetailRowCount> objStoredProc = new ZnodeViewRepository<View_EntityDetailRowCount>();
            objStoredProc.SetParameter("TableName", model.EntityName, ParameterDirection.InputOutput, DbType.String);
            objStoredProc.SetParameter("Id", model.ID, ParameterDirection.InputOutput, DbType.String);
            ZnodeLogging.LogMessage("SP parameters: ", string.Empty, TraceLevel.Verbose, new object[] { model?.EntityName, model?.ID });
            var data = objStoredProc.ExecuteStoredProcedureList("ZnodeEntityDetailRowCount @TableName,@Id");

            navigationModel.TotalCount = Convert.ToInt32(data.FirstOrDefault().RowCount);
            navigationModel.CurrentIndex = Convert.ToInt32(data.FirstOrDefault().IndexId);
            navigationModel.NextRecordId = data.FirstOrDefault().Leadvalue;
            navigationModel.PreviousRecordId = data.FirstOrDefault().Lagvalue;
            ZnodeLogging.LogMessage("Execution done.", string.Empty, TraceLevel.Info);
            return navigationModel;
        }
    }
}
