using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IThemeService
    {
        /// <summary>
        /// Get the list of all themes with pagination
        /// </summary>
        /// <param name="filters">List of filter tuples</param>
        /// <param name="sorts">Collection of sorting parameters</param>
        /// <param name="page">Collection of paging parameters</param>
        /// <returns>List of Themes</returns>
        ThemeListModel GetThemes(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Create new theme
        /// </summary>
        /// <param name="themeModel">Theme model</param>
        /// <returns>Theme Model</returns>
        ThemeModel CreateTheme(ThemeModel themeModel);

        /// <summary>
        /// Get theme by themeId
        /// </summary>
        /// <param name="themeId">Get theme through themeId</param>
        /// <returns>Theme Model</returns>
        ThemeModel GetTheme(int themeId);

        /// <summary>
        /// Update theme data.
        /// </summary>
        /// <param name="themeModel">Theme model to update.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdateTheme(ThemeModel themeModel);

        /// <summary>
        /// Delete Theme
        /// </summary>
        /// <param name="themeId">Theme ID to delete Theme</param>
        /// <returns>boolean value true/false</returns>
        bool DeleteTheme(ParameterModel themeId);

        #region Associate Store
        /// <summary>
        ///  Get associated store list for cms theme.
        /// </summary>
        /// <param name="expands">Expands for Portal cms theme.</param>
        /// <param name="filters">Filters for Portal cms theme.</param>
        /// <param name="sorts">Sorts for for Portal cms theme.</param>
        /// <param name="page">Page size.</param>
        /// <returns>Returns list of Portal cms theme.</returns>
        PricePortalListModel GetAssociatedStoreList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get list of unassociated stores.
        /// </summary>
        /// <param name="filters">Filters for Portal.</param>
        /// <param name="sorts">Sorts for for Portal.</param> <param name="page"></param>
        /// <returns>Returns list of unassociated store list.</returns>
        PortalListModel GetUnAssociatedStoreList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Associate store to cms theme.
        /// </summary>
        /// <param name="pricePortalModel">PricePortalListModel</param>
        /// <returns>Returns true if store associated successfully else return false.</returns>
        bool AssociateStore(PricePortalListModel pricePortalModel);

        /// <summary>
        /// Remove associated stores from cms theme.
        /// </summary>
        /// <param name="cmsPortalThemeId">cmsPortalThemeId to unassociate store.</param>
        /// <returns>Returns true if stores unassociated successfully else return false.</returns>
        bool RemoveAssociatedStores(ParameterModel cmsPortalThemeId);
        #endregion

        #region CMS Widgets    
        /// <summary>
        /// Get the list of all cms areas with pagination
        /// </summary>
        /// <param name="filters">List of filter tuples</param>
        /// <param name="sorts">Collection of sorting parameters</param>
        /// <param name="page">Collection of paging parameters</param>
        /// <returns>List of Themes</returns>
        CMSAreaListModel GetAreas(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);
        #endregion
    }
}
