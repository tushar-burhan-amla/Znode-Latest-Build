using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface ITemplateClient : IBaseClient
    {
        /// <summary>
        /// Get the templates list.
        /// </summary>
        /// <param name="expands">Expands.</param>
        /// <param name="filters">Filters.</param>
        /// <param name="sorts">Sort.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>Returns templates list.</returns>
        TemplateListModel GetTemplates(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create the template.
        /// </summary>
        /// <param name="model">Template model to create.</param>
        /// <returns>Returns created model.</returns>
        TemplateModel CreateTemplate(TemplateModel model);

        /// <summary>
        /// Gets the existing template.
        /// </summary>
        /// <param name="cmsTemplateId">Template id.</param>
        /// <param name="expands">Expands.</param>
        /// <returns>Returns template.</returns>
        TemplateModel GetTemplate(int cmsTemplateId, ExpandCollection expands);

        /// <summary>
        /// Update the existing template.
        /// </summary>
        /// <param name="templateModel">Template model to update.</param>
        /// <returns>Returns updated model.</returns>
        TemplateModel UpdateTemplate(TemplateModel templateModel);

        /// <summary>
        /// Delete the templates.
        /// </summary>
        /// <param name="cmsTemplateIds">Template ids to delete.</param>
        /// <returns>Returns true if updated successfully.</returns>
        bool DeleteTemplate(ParameterModel cmsTemplateIds);
    }
}
