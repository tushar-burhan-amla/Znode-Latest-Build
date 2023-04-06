
using System.Web.Mvc;

using Znode.Engine.WebStore.Controllers;
using Znode.WebStore.Core.Agents;
using Znode.WebStore.Core.ViewModels;

namespace Znode.WebStore.Core.Controllers
{
    public class SearchReportController : BaseController
    {
        #region Public Variable
        private readonly ISearchReportAgent _searchReportAgent;
        #endregion

        #region Public Constructor
        public SearchReportController(ISearchReportAgent searchReportAgent)
        {
            _searchReportAgent = searchReportAgent;
        }
        #endregion

        #region Public Methods
        //Save Search Report Data.
        [HttpPost]
        public virtual void SaveSearchReportData(SearchReportViewModel model)
        {
            _searchReportAgent.SaveSearchReport(model);
        }
        #endregion
    }
}
