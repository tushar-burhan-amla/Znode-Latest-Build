using System.Collections.Generic;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IApplicationSettingsClient : IBaseClient
    {
        /// <summary>
        /// Get dynamic grid configuration XML 
        /// </summary>
        /// <param name="itemName">string item Name</param>
        /// <returns>return application settings data model</returns>
        ApplicationSettingDataModel GetFilterConfigurationXML(string itemName, int? userId = null);

        ViewModel CreateNewView(ViewModel model);

        /// <summary>
        /// Deletes a View by itemViewId.
        /// </summary>
        /// <param name="parameterModel">itemViewId to delete view.</param>
        /// <returns>True/False value according the status of delete operation.</returns>
        bool DeleteView(ParameterModel parameterModel);

        #region XML Editor
        /// <summary>
        /// Get Xml configuration List
        /// </summary>
        /// <param name="filters">Filter collection</param>
        /// <param name="sorts">Sort collection</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Record per page</param>
        /// <returns>Returns list of ApplicationSetting</returns>
        ApplicationSettingListModel GetApplicationSettings(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get Column names list
        /// </summary>
        /// <param name="entityType">Type of entity</param>
        /// <param name="entityName">Name of entity</param>
        /// <returns></returns>
        List<EntityColumnModel> GetColumnList(string entityType, string entityName);

        /// <summary>
        /// Insert /Update /Delete Xml configuration
        /// </summary>
        /// <param name="model">Application Setting Data Model</param>
        /// <returns>Returns true/false</returns>
        bool SaveXmlConfiguration(ApplicationSettingDataModel model);

        /// <summary>
        /// Get View by itemViewId
        /// </summary>
        /// <param name="itemViewId">ItemviewId</param>
        ViewModel GetView(int itemViewId);

        /// <summary>
        /// Remove Selected of all View by applicationSettingId
        /// </summary>
        /// <param name="applicationSettingId">applicationSettingId</param>
        bool UpdateViewSelectedStatus(int applicationSettingId);
        #endregion
    }
}
