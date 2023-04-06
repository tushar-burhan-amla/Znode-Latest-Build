using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Cache
{
    interface IQuickOrderCache
    {
        /// <summary>
        /// This method return quick order products based on parameter value
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetQuickOrderProductList(QuickOrderParameterModel parameters, string routeUri, string routeTemplate);

        /// <summary>
        /// This method return random quick order product basic details
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns></returns>
        string GetDummyQuickOrderProductList(string routeUri, string routeTemplate);
    }
}
