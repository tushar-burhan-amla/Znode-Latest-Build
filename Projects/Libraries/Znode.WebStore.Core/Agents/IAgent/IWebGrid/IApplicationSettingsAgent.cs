using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.ViewModels;

namespace Znode.Engine.WebStore.Agents
{
    public interface IApplicationSettingsAgent
    {
        /// <summary>
        /// Get dynamic grid configuration XML 
        /// </summary>
        /// <param name="nameOfXML">Name of XML</param>
        /// <returns>Returns XMl string data model </returns>
        ApplicationSettingDataModel GetFilterConfigurationXML(string nameOfXML,int? userId=null);

        #region XMLEditor
        /// <summary>
        /// To get list of column names of corresponding table/view/stored procedure.
        /// </summary>
        /// <param name="entityType">It can be table/view/stored procedure.</param>
        /// <param name="entityName">Names of corresponding table/view/stored procedure.</param>
        /// <returns>Returns DataSet </returns>
        List<EntityColumnModel> GetObjectColumnList(string entityType, string entityName);

        /// <summary>
        /// To get xml configuration from database into application setting model.
        /// </summary>
        /// <param name="ApplicationId"></param>
        /// <returns>Returns ApplicationSettingListViewModel</returns>
        ApplicationSettingListViewModel ApplicationsSettingList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Save xml configuration into database.
        /// </summary>
        /// <param name="xmlSTring">String of XML</param>
        /// <param name="viewOptions">View option</param>
        /// <param name="entityType">Type of Entity</param>
        /// <param name="entityName">Name of entity</param>
        /// <param name="frontPageName">Front Page Name</param>
        /// <param name="frontObjectName">Front Object Name</param>
        /// <param name="id">Configuration Id</param>
        /// <returns>Returns true/false</returns>
        bool SaveXmlConfiguration(string xmlSTring, string viewOptions, string entityType, string entityName, string frontPageName, string frontObjectName, int id);

        /// <summary>
        /// Update xml configuration into database.
        /// </summary>
        /// <param name="xmlSTring">String of XML</param>
        /// <param name="viewOptions">View option</param>
        /// <param name="entityType">Type of Entity</param>
        /// <param name="entityName">Name of entity</param>
        /// <param name="frontPageName">Front Page Name</param>
        /// <param name="frontObjectName">Front Object Name</param>
        /// <param name="id">Configuration Id</param>
        /// <returns>Returns true/false</returns>
        bool UpdateXmlConfiguration(string xmlSTring, string viewOptions, string entityType, string entityName, string frontPageName, string frontObjectName, int id);

        /// <summary>
        /// Delete xml configuration from database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>boolen value </returns>
        bool DeleteXmlConfiguration(int id);

        /// <summary>
        /// Get column names list
        /// </summary>
        /// <param name="entityType">Type of entity</param>
        /// <param name="entityName">Name of entity</param>
        /// <returns>Returns list of SelectListItem</returns>
        List<SelectListItem> GetEntityColumnList(string entityType, string entityName);

        /// <summary>
        /// Create XMl file from WebGridColumnModel list.
        /// </summary>
        /// <param name="list">List of WebGridColumnModel</param>
        /// <returns>Returns XMl string</returns>
        string CreateXMLFile(List<WebGridColumnViewModel> list);

        /// <summary>
        /// Convert XML string to WebGridColumnModel list.
        /// </summary>
        /// <param name="xmlString">String of XMl</param>
        /// <returns>Returns WebGridColumnModel List</returns>
        List<WebGridColumnViewModel> GetListFromXMLString(string xmlString);

        /// <summary>
        /// Get list of entity names for autocomplete
        /// </summary>
        /// <param name="term"></param>
        /// <param name="entityType">Type of entity</param>
        /// <returns>Returns IEnumerable SelectListItem</returns>
        IEnumerable<SelectListItem> GetEntityNames(string term = "", string entityType = "");
        #endregion
    }
}
