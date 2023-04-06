namespace Znode.Engine.Api.Cache
{
    public interface ITemplateCache
    {
        /// <summary>
        /// Get the templates list.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Returns templates list.</returns>
        string GetTemplates(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets the existing template.
        /// </summary>
        /// <param name="cmsTemplateId">Template id.</param>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Returns template.</returns>
        string GetTemplate(int cmsTemplateId, string routeUri, string routeTemplate);
    }
}
