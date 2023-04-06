using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IThemeAgent
    {
        /// <summary>
        /// Get the list of themes.
        /// </summary>
        /// <param name="filters">Filters for Portal cms theme.</param>
        /// <param name="sorts">Sorts for for Portal cms theme.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>List of Themes.</returns>
        ThemeListViewModel GetThemeList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get the list of parent themes.
        /// </summary>        
        /// <returns>List of Themes.</returns>
        List<SelectListItem> GetParentThemeList();

        /// <summary>
        /// Get the list of revision themes.
        /// </summary>
        /// <param name="filters">Filters for Portal cms theme.</param>
        /// <param name="sorts">Sorts for for Portal cms theme.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>List of revision themes.</returns>
        ThemeListViewModel GetThemeRevisionList(int CMSThemeId, string themeName, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create new theme.
        /// </summary>
        /// <param name="themeViewModel">ThemeViewModel</param>
        /// <returns>Returns ThemeViewModel.</returns>
        ThemeViewModel CreateTheme(ThemeViewModel themeViewModel);

        /// <summary>
        /// Get theme by cms theme id.
        /// </summary>
        /// <param name="cmsThemeId">cms Theme Id</param>
        /// <returns>Returns ThemeViewModel.</returns>
        ThemeViewModel GetTheme(int cmsThemeId);

        /// <summary>
        /// Update theme.
        /// </summary>
        /// <param name="themeViewModel">Theme view model to update.</param>
        /// <returns>Returns updated theme view model.</returns>
        ThemeViewModel UpdateTheme(ThemeViewModel themeViewModel);

        /// <summary>
        /// Check Whether the theme Name is already exists.
        /// </summary>
        /// <param name="themeName">themename</param>
        /// <param name="cmsThemeId">id for the theme</param>
        /// <returns>return the status in true or false</returns>
        bool CheckThemeNameExist(string themeName, int cmsThemeId);

        /// <summary>
        /// Update revised theme.
        /// </summary>
        /// <param name="themeName">themeName</param>
        /// <param name="cmsThemeId">cmsThemeId</param>
        /// <returns>Returns updated theme view model.</returns>
        bool UpdateRevisedTheme(int cmsThemeId, string themeName);

        /// <summary>
        ///Delete theme which is not associated to store. 
        /// </summary>
        /// <param name="themeId">Id to delete theme</param>
        /// <param name="cmsThemeName">Name of theme</param>
        /// <param name="errorMessage">message if error occurred while deleting theme.</param>
        /// <returns>True if deleted successfully else false.</returns>
        bool DeleteTheme(string themeId, string cmsThemeName, out string errorMessage);

        /// <summary>
        ///Delete revised theme. 
        /// </summary>
        /// <param name="cmsThemeName">Name of theme</param>
        /// <param name="errorMessage">message if error occurred while deleting theme.</param>
        /// <returns>True if deleted successfully else false.</returns>
        bool DeleteRevisedTheme(string cmsThemeName, out string errorMessage);

        /// <summary>
        /// Download Theme.
        /// </summary>
        /// <param name="themeName">themeName</param>
        /// <returns>Returns path.</returns>
        string DownloadTheme(string themeName);

        /// <summary>
        /// Get the contents of zip file of theme.
        /// </summary>
        /// <param name="filePath">path of theme folder.</param>
        /// <param name="themeName">Name of theme.</param>
        /// <returns>Array of byte.</returns>
        byte[] GetZipFile(string filePath, string themeName);

        #region Associate Store

        /// <summary>
        /// Associate store to cms theme.
        /// </summary>
        /// <param name="cmsThemeId">Id of cms theme list.</param>
        /// <param name="storeIds">Store Ids to be associated.</param>
        /// <returns>Returns true if store associated successfully else return false.</returns>
        bool AssociateStore(int cmsThemeId, string storeIds);

        /// <summary>
        /// Remove associated stores from price.
        /// </summary>
        /// <param name="cmsPortalThemeId">cmsPortalThemeIds to unassociate store from cms theme.</param>
        /// <returns>Returns true if store unassociated successfully else return false.</returns>
        bool RemoveAssociatedStores(string cmsPortalThemeId);
        #endregion

        /// <summary>
        /// Get the list of css associated to theme
        /// </summary>
        /// <param name="themeId">Id of theme</param>
        /// <param name="filters">FilterCollection</param>
        /// <param name="sorts">SortCollection</param>
        /// <param name="pageIndex">Index of page</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>CSSListViewModel</returns>
        CSSListViewModel GetCssList(int themeId, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize);

        /// <summary>
        /// Create new Css
        /// </summary>
        /// <param name="cssViewModel">CSSViewModel</param>
        /// <returns>CSSViewModel</returns>
        CSSViewModel CreateCSS(CSSViewModel cssViewModel);

        /// <summary>
        /// Delete css.
        /// </summary>
        /// <param name="cssId">Id of css.</param>
        /// <param name="cssName">name of css.</param>
        /// <param name="errorMessage">error message.</param>
        /// <returns>true if deleted else false.</returns>
        bool DeleteCss(string cssId, string cssName, string themeName, out string errorMessage);

        /// <summary>
        /// Download Css.
        /// </summary>
        /// <param name="CMSThemeId">CMSThemeId</param>
        /// <param name="CssName">cssName</param>
        /// <param name="themeName">themeName</param>
        /// <returns>Returns path.</returns>
        string DownloadCss(int CMSThemeId, string cssName, string themeName);
    }
}
