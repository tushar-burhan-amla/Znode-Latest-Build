namespace Znode.Engine.Api.Cache
{
    /// <summary>
    ///This is the root Interface EmailTemplateCache.
    /// </summary>
    public interface IEmailTemplateCache
    {
        /// <summary>
        /// Get Email Template List Details.
        /// </summary>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>return template details.</returns>
        string GetEmailTemplates(string routeUri, string routeTemplate);

        /// <summary>
        /// Get the Email Template details on template Id.
        /// </summary>
        ///<param name="emailTemplateId">Email Template ID of the template page.</param>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>return page content.</returns>
        string GetTemplatePage(int emailTemplateId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get the Email Template tokens.
        /// </summary>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>return email template tokens.</returns>
        string GetEmailTemplateTokens(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Email Template Area List.
        /// </summary>
        /// <param name="routeUri">routeUri</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns>return template area details.</returns>
        string GetEmailTemplateAreaList(string routeUri, string routeTemplate);

    }
}