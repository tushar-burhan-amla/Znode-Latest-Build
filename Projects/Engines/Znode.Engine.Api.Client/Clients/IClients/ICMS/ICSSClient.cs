using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface ICSSClient : IBaseClient
    {

        /// <summary>
        /// Create new CSS.
        /// </summary>
        /// <param name="model">CSS Model to create new CSS</param>
        /// <returns>return Created CSS Model</returns>
        CSSModel CreateCSS(CSSModel model);

        /// <summary>
        /// Delete CSS
        /// </summary>
        /// <param name="cssId">CSS Ids to delete CSS</param>
        /// <returns>boolean value true id css deleted else returns false</returns>
        bool DeleteCSS(string cssIds);

        /// <summary>
        /// Get the list of Css associated to theme.
        /// </summary>
        /// <param name="cmsThemeId">Id of theme.</param>
        /// <param name="filters">FilterCollection</param>
        /// <param name="sorts">SortCollection</param>
        /// <param name="pageIndex">index of current page</param>
        /// <param name="pageSize">Total records display in grid</param>
        /// <returns>CSSListModel</returns>
        CSSListModel GetCssListByThemeId(int cmsThemeId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);
    }
}
