using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface ITemplateAgent
    {
        /// <summary>
        /// Get the templates list.
        /// </summary>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sort.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>Returns templates list.</returns>
        TemplateListViewModel GetTemplates(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null);

        /// <summary>
        /// Create the template.
        /// </summary>
        /// <param name="templateViewModel">Template view model to create.</param>
        /// <returns>Returns created view model.</returns>
        TemplateViewModel CreateTemplate(TemplateViewModel templateViewModel);

        /// <summary>
        /// Gets the existing template.
        /// </summary>
        /// <param name="cmsTemplateId">Template id.</param>
        /// <returns>Returns template.</returns>
        TemplateViewModel GetTemplate(int cmsTemplateId);

        /// <summary>
        /// Update the existing template.
        /// </summary>
        /// <param name="templateViewModel">Template view model to update.</param>
        /// <returns>Returns updated view model.</returns>
        TemplateViewModel UpdateTemplate(TemplateViewModel templateViewModel);

        /// <summary>
        /// Delete the templates.
        /// </summary>
        /// <param name="cmsTemplateId">>Template ids to delete.</param>
        /// <param name="fileName">File name to delete.</param>
        /// <param name="errorMessage">Error Message.</param>
        /// <returns>Returns true if updated successfully.</returns>
        bool DeleteTemplate(string cmsTemplateId, string fileName, out string errorMessage);

        /// <summary>
        /// Copy the template.
        /// </summary>
        /// <param name="templateViewModel">Template id.</param>
        /// <returns>Returns copied model.</returns>
        TemplateViewModel CopyTemplate(TemplateViewModel templateViewModel);

        /// <summary>
        /// Get the details of template to download.
        /// </summary>
        /// <param name="cmsTemplateId">Id of template.</param>
        /// <param name="fileName">Name of template file.</param>
        /// <returns>Returns path of template file.</returns>
        string DownloadTemplate(int cmsTemplateId, string fileName);

        /// <summary>
        /// Check duplicate template name.
        /// </summary>
        /// <param name="templateName"> templateName</param>
        /// <returns>true/false</returns>
        bool CheckTemplateName(string templateName);
    }
}
