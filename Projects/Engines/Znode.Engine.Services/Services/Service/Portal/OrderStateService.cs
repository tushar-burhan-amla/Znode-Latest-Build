using System.Collections.Specialized;
using System.Diagnostics;
using Znode.Engine.Api.Models;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services
{
    public class OrderStateService : BaseService, IOrderStateService
    {
        #region Private Member
        private readonly IZnodeRepository<ZnodeOmsOrderState> _orderStateRepository;
        #endregion

        #region Constructor
        public OrderStateService()
        {
            _orderStateRepository = new ZnodeRepository<ZnodeOmsOrderState>();
        }
        #endregion

        #region Public Method
        public virtual OrderStateListModel GetOrderStates(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", string.Empty, TraceLevel.Info);

            //Set Paging Parameters
            int pagingStart = 0;
            int pagingLength = 0;
            int totalCount = 0;
            SetPaging(page, out pagingStart, out pagingLength);

            //Gets the OrderBy Clause
            string orderBy = DynamicClauseHelper.GenerateDynamicOrderByClause(sorts);

            //gets the where clause with filter Values.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("Where clause generated:", string.Empty, TraceLevel.Verbose, whereClauseModel.WhereClause);
            OrderStateListModel listModel = new OrderStateListModel();

            var orderStateListEntity = _orderStateRepository.GetPagedList(whereClauseModel.WhereClause, orderBy, whereClauseModel.FilterValues, pagingStart, pagingLength, out totalCount);
            ZnodeLogging.LogMessage("orderStateListEntity:", string.Empty, TraceLevel.Verbose, orderStateListEntity);
            foreach (var orderState in orderStateListEntity)
            {
                listModel.OrderStates.Add(OrderStateMap.ToModel(orderState));
            }

            listModel.TotalResults = totalCount;
            listModel.PageIndex = pagingStart;
            listModel.PageSize = pagingLength;
            ZnodeLogging.LogMessage("Executed.", string.Empty, TraceLevel.Info);

            return listModel;
        }
        #endregion
    }
}
