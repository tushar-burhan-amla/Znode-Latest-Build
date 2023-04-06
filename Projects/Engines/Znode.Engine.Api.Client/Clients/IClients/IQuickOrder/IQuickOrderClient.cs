using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IQuickOrderClient : IBaseClient
    {
        /// <summary>
        /// This method return quick order products based on parameter value
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="parameters"></param>
        /// <returns>returns quick order product list</returns>
        QuickOrderProductListModel GetQuickOrderProductList(FilterCollection filters, QuickOrderParameterModel parameters);


        /// <summary>
        /// This method return random quick order product basic details
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        QuickOrderProductListModel GetDummyQuickOrderProductList(FilterCollection filters, int? pageIndex, int? pageSize);
    }
}
