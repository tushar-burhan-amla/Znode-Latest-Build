using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ITemplateService
    {
        /// <summary>
        /// Get the templates list.
        /// </summary>
        /// <param name="expands">Expands.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sort.</param>
        /// <param name="page">Pagination.</param>
        /// <returns>Returns templates list.</returns>
        TemplateListModel GetTemplates(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Create the template.
        /// </summary>
        /// <param name="templateModel">Template model to create.</param>
        /// <returns>Returns created model.</returns>
        TemplateModel CreateTemplate(TemplateModel templateModel);

        /// <summary>
        /// Gets the existing template.
        /// </summary>
        /// <param name="cmsTemplateId">Template id.</param>
        /// <param name="expands">Expands.</param>
        /// <returns>Returns template.</returns>
        TemplateModel GetTemplate(int cmsTemplateId, NameValueCollection expands);

        /// <summary>
        /// Update the existing template.
        /// </summary>
        /// <param name="templateModel">Template model to update.</param>
        /// <returns>Returns updated model.</returns>
        bool UpdateTemplate(TemplateModel templateModel);

        /// <summary>
        /// Delete the templates.
        /// </summary>
        /// <param name="cmsTemplateIds">Template ids to delete.</param>
        /// <returns>Returns true if updated successfully.</returns>
        bool DeleteTemplate(ParameterModel cmsTemplateIds);
    }
}
