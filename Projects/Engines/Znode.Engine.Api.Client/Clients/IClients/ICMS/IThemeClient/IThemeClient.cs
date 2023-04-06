using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IThemeClient : IBaseClient
    {
        /// <summary>
        /// Gets Theme list.
        /// </summary>
        /// <param name="filters">Filter Collection for theme List.</param>
        /// <param name="sorts">Sort collection of Theme List.</param>
        /// <param name="pageIndex">Index of requesting page</param>
        /// <param name="pageSize">Record per page</param>
        /// <returns>List of themes</returns>
        ThemeListModel GetThemes(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create new theme
        /// </summary>
        /// <param name="model">Theme Model</param>
        /// <returns>Theme Model</returns>
        ThemeModel CreateTheme(ThemeModel model);

        /// <summary>
        /// Get theme on the basis of cmsThemeId.
        /// </summary>
        /// <param name="cmsThemeId">cmsThemeId to get theme details.</param>
        /// <returns>Returns ThemeModel.</returns>
        ThemeModel GetTheme(int cmsThemeId);

        /// <summary>
        /// Update theme data.
        /// </summary>
        /// <param name="themeModel">Theme model to update.</param>
        /// <returns>Returns updated theme model.</returns>
        ThemeModel UpdateTheme(ThemeModel themeModel);

        /// <summary>
        /// Delete Theme
        /// </summary>
        /// <param name="themeId">Theme ID to delete Theme</param>
        /// <returns>boolean value true/false</returns>
        bool DeleteTheme(string themeId);

        #region Associate Store

        /// <summary>
        /// Associate price to store.
        /// </summary>
        /// <param name="listModel">PricePortalListModel</param>
        /// <returns>Returns true if portal associated successfully else return false.</returns>
        bool AssociateStore(PricePortalListModel listModel);

        /// <summary>
        /// Remove associated portals from CMS theme.
        /// </summary>
        /// <param name="model">ParameterModel.</param>
        /// <returns>Returns true if portals unassociated successfully else return false.</returns>
        bool RemoveAssociatedStores(ParameterModel cmsPortalThemeId);
        #endregion

        #region CMS Widgets

        /// <summary>
        /// Gets CMS Area list.
        /// </summary>
        /// <param name="filters">Filter Collection for theme Area List.</param>
        /// <param name="sorts">Sort collection of Theme Area List.</param>
        /// <param name="pageIndex">Index of requesting page</param>
        /// <param name="pageSize">Record per page</param>
        /// <returns>CMS Area list</returns>
        CMSAreaListModel GetAreas(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        #endregion
    }
}
