using System;
using System.Diagnostics;

using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.WebStore;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using Znode.WebStore.Core.ViewModels;

namespace Znode.WebStore.Core.Agents
{
    public class SearchReportAgent : BaseAgent, ISearchReportAgent
    {
        #region Private Variable
        private readonly ISearchReportClient _searchReportClient;
        #endregion

        #region Public Constructor
        public SearchReportAgent(ISearchReportClient searchReportClient)
        {
            _searchReportClient = GetClient<ISearchReportClient>(searchReportClient);
        }
        #endregion

        #region Public Methods
        public virtual SearchReportViewModel SaveSearchReport(SearchReportViewModel viewModel)
        {
            try
            {
                if (HelperUtility.IsNotNull(viewModel))
                {
                    ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

                    SearchReportModel model = _searchReportClient.SaveSearchReport(viewModel.ToModel<SearchReportModel>());

                    ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

                    return HelperUtility.IsNotNull(model) ? model.ToViewModel<SearchReportViewModel>() : new SearchReportViewModel();
                }
                return new SearchReportViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                return new SearchReportViewModel { HasError = true, ErrorMessage = ex.ErrorMessage };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                return (SearchReportViewModel)GetViewModelWithErrorMessage(viewModel, WebStore_Resources.ErrorFailedToSaveSearchReport);
            }
        }
        #endregion
    }
}
