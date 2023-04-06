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
    public class ProductReviewStateService : BaseService,IProductReviewStateService
    {
        #region Private Variable
        private readonly IZnodeRepository<ZnodeProductReviewState> _productReviewStateRepository;
        #endregion

        #region Constructor
        public ProductReviewStateService()
        {
            _productReviewStateRepository = new ZnodeRepository<ZnodeProductReviewState>();
        }
        #endregion

        #region Public Method
        public virtual ProductReviewStateListModel GetProductReviewStates(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            //Set Paging Parameters
            int pagingStart = 0;
            int pagingLength = 0;
            int totalCount = 0;
            SetPaging(page,out pagingStart,out pagingLength);

            //Get the OrderBy Clause
            string orderBy = DynamicClauseHelper.GenerateDynamicOrderByClause(sorts);
            ZnodeLogging.LogMessage("orderBy: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, orderBy);
            //gets the where clause with filter Values.              
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            ZnodeLogging.LogMessage("whereClauseModel to generate productReviewStateListEntity list ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, whereClauseModel);
            ProductReviewStateListModel listModel = new ProductReviewStateListModel();

            var productReviewStateListEntity = _productReviewStateRepository.GetPagedList(whereClauseModel.WhereClause, orderBy, whereClauseModel.FilterValues, pagingStart, pagingLength, out totalCount);
            ZnodeLogging.LogMessage("productReviewStateListEntity: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, productReviewStateListEntity);
            foreach (ZnodeProductReviewState productReviewState in productReviewStateListEntity)
            {
                listModel.ProductReviewStates.Add(ProductReviewStateMap.ToModel(productReviewState));
            }

            listModel.PageIndex = pagingStart;
            listModel.PageSize = pagingLength;
            listModel.TotalResults = totalCount;
            ZnodeLogging.LogMessage("Executed: ", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return listModel;
        }
        #endregion
    }
}
