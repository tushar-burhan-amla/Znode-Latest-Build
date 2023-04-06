namespace Znode.Engine.Api.Cache
{
    public interface IImportCache
    {
        /// <summary>
        /// gets te Import type list
        /// </summary>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>string data</returns>
        string GetImportTypeList(string routeUri, string routeTemplate);

        /// <summary>
        /// get all templates with respect to Import Type Id
        /// </summary>
        /// <param name="importTypeId">Import Type id</param>
        /// <param name="familyId">Family id</param>
        /// <param name="promotionTypeId">Promotion Type Id</param>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>string data</returns>
        string GetAllTemplates(int importTypeId, int familyId, string routeUri, string routeTemplate, int promotionTypeId = 0);

        /// <summary>
        /// gets the list of template data with respect to template id
        /// </summary>
        /// <param name="templateId">Template Id</param>
        /// <param name="importHeadId">Import Head Id</param>
        /// <param name="familyId">Family Id</param>
        /// <param name="promotionTypeId">Promotion Type Id</param>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>string data</returns>
        string GetTemplateData(int templateId, int importHeadId, int familyId, string routeUri, string routeTemplate, int promotionTypeId = 0);

        /// <summary>
        /// Downloads the columns for import depending upon ImportHead Id
        /// </summary>
        /// <param name="importHeadId">Import Head Id</param>
        /// <param name="familyId">Family Id</param>
        /// <param name="promotionTypeId">Promotion Type Id</param>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>string data</returns>
        string DownLoadTemplate(int importHeadId, int familyId, string routeUri, string routeTemplate, int promotionTypeId = 0);

        /// <summary>
        /// Get import logs
        /// </summary>
        /// <param name="routeUri">Route Uri</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>string</returns>
        string GetImportLogs(string routeUri, string routeTemplate);

        /// <summary>
        /// Get import log details
        /// </summary>
        /// <param name="importProcessLogId">Import process Log Id</param>
        /// <param name="routeUri">Route uri</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>string</returns>
        string GetImportLogDetails(int importProcessLogId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get the import log status
        /// </summary>
        /// <param name="importProcessLogId">importProcessLogId</param>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>string</returns>
        string GetLogStatus(int importProcessLogId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get all families for Product Import
        /// </summary>
        /// <param name="isCategory">isCategory</param>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns></returns>
        string GetFamilies(bool isCategory, string routeUri, string routeTemplate);

        /// <summary>
        /// Check import status
        /// </summary>
        /// <returns></returns>
        string CheckImportStatus(string routeUri, string routeTemplate);

        /// <summary>
        /// Get default template for import data.
        /// </summary>
        /// <param name="importType"></param>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <param name="templateName"></param>
        /// <returns></returns>
        string GetDefaultTemplate(string templateName, string routeUri, string routeTemplate);

        ///// <summary>
        ///// Get custom import template list. It will not return the system defined import template.
        ///// </summary>
        ///// <param name="routeUri">Route Uri</param>
        ///// <param name="routeTemplate">Route Template</param>
        ///// <returns>string</returns>
        string GetCustomImportTemplateList(string routeUri, string routeTemplate);
    
    string GetErrorListforImport(string fileType, int importProcessLogId, string routeUri, string routeTemplate);

    }
}
