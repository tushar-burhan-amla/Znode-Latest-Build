using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IQuickOrderService
    {
        /// <summary>
        /// This method return products based on parameter values
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        QuickOrderProductListModel GetQuickOrderProductList(QuickOrderParameterModel parameters, FilterCollection filters);

        /// <summary>
        /// This method return random quick order product basic details
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        QuickOrderProductListModel GetDummyQuickOrderProductList(FilterCollection filters, NameValueCollection page);
    }
}
