using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services
{
    public class StateService : BaseService, IStateService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeState> _stateRepository;
        #endregion

        #region Constructor
        public StateService()
        {
            _stateRepository = new ZnodeRepository<ZnodeState>();
        }
        #endregion

        #region Public Methods

        //Get a list of states
        public virtual StateListModel GetStateList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate stateListEntity list ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            StateListModel stateList = new StateListModel();

            //Gets the entity list according to where clause, order by clause and pagination
            IList<ZnodeState> stateListEntity = _stateRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("stateListEntity list count:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, stateListEntity?.Count());
            //maps the entity list to model
            stateList.States = stateListEntity?.Count > 0 ? stateListEntity.ToModel<StateModel>().ToList() : new List<StateModel>();

            stateList?.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return stateList;
        }

        #endregion
    }
}
