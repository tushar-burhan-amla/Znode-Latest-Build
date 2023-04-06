using System.Linq;
using System.Web;
using System.Web.Routing;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore
{
    public interface ISEOUrlRouteData
    {
        //
        // Summary:
        //     Gets or sets a dictionary of expressions that specify valid values for a URL
        //     parameter.
        //
        // Returns:
        //     An object that contains the parameter names and expressions.
        RouteValueDictionary Constraints { get; set; }
        //
        // Summary:
        //     Gets or sets the values to use if the URL does not contain all the parameters.
        //
        // Returns:
        //     An object that contains the parameter names and default values.
        RouteValueDictionary Defaults { get; set; }
        #region Methods
        /// <summary>
        /// Returns modified information about the route.
        /// </summary>
        /// <param name="httpContext">An object that encapsulates information about the HTTP request.</param>
        /// <returns>
        /// An object that contains the values from the route definition.
        /// </returns>
        RouteData GetRouteData(HttpContextBase httpContext);
        #endregion
    }
}