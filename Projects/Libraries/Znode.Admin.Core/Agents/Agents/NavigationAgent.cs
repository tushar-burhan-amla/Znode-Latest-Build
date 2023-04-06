using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.Admin.Agents
{
    public class NavigationAgent : BaseAgent, INavigationAgent
    {
        #region Private Variables
        private readonly INavigationClient _navigationClient;
        #endregion

        #region Constructor
        public NavigationAgent(INavigationClient navigationClient)
        {
            _navigationClient = GetClient<INavigationClient>(navigationClient);
        }
        #endregion

        public virtual NavigationViewModel GetNavigationDetails(string Id, string controllerName, string entity, string queryParameter, string areaName = null, string editAction = null, string deleteAction = null, string detailAction = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
            NavigationParamModel parameter = new NavigationParamModel();
            parameter.ID = Id;
            parameter.EntityName = entity;
            var viewModel = NavigationViewModelMap.ToViewModel(_navigationClient.GetNavigationDetails(parameter), Id, controllerName, areaName, editAction, deleteAction, detailAction);
            viewModel.PreviousQueryString = $"{queryParameter}={viewModel.PreviousID}";
            viewModel.NextQueryString = $"{queryParameter}={viewModel.NextID}";
            viewModel.EditDeleteQueryString = $"{queryParameter}={viewModel.ID}";
            ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
            return viewModel;
        }
    }
}