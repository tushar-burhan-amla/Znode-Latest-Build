using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IApplicationSettingsService
    {
        /// <summary>
        /// gets the filter settings
        /// </summary>
        /// <param name="itemName">item name</param>
        /// <returns>returns the application setting data model</returns>
        ApplicationSettingDataModel GetFilterConfigurationXML(string itemName, int? userId = null);

        ViewModel CreateNewView(ViewModel model);

        #region XML Editor 

        /// <summary>
        /// Get Xml configuration list from database.
        /// </summary>
        /// <param name="expands">Expands collections</param>
        /// <param name="filters">Filters collection</param>
        /// <param name="sorts">Sort collection</param>
        /// <param name="page">Page Number</param>
        /// <returns>Returns ApplicationSettingListModel</returns>
        ApplicationSettingListModel GetApplicationSettings(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get Column list from database.
        /// </summary>
        /// <param name="entityType">Type of object</param>
        /// <param name="entityName">Name of Object</param>
        /// <returns></returns>
        ApplicationSettingListModel GetColumnList(string entityType, string entityName);

        /// <summary>
        /// Save Xml configuration in ApplicationSetting Table.
        /// </summary>
        /// <param name="model">ApplicationSettingDataModel</param>
        /// <returns>Returns true/false</returns>
        bool SaveXmlConfiguration(ApplicationSettingDataModel model);

        /// <summary>
        /// Delete view by ListViewId.
        /// </summary>
        /// <param name="model">ApplicationSettingDataModel</param>
        /// <returns>Returns true/false</returns>
        bool DeleteView(ParameterModel listViewId);

        /// <summary>
        /// Delete selected of all view by applicationSettingId.
        /// </summary>
        /// <param name="applicationSettingId">applicationSettingId</param>
        /// <returns>Returns true/false</returns>
        bool UpdateViewSelectedStatus(int applicationSettingId);

        /// <summary>
        /// Get view by ListViewId.
        /// </summary>
        /// <param name="itemViewId">List ViewId</param>
        /// <returns></returns>
        ViewModel GetView(int itemViewId);
        #endregion
    }
}
