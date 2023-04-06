namespace Znode.Engine.Api.Cache
{
    public interface IWebStoreMessageCache
    {
        /// <summary>
        /// Get message by Message Key, Area and Portal Id.
        /// </summary>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Returns Message.</returns>
        string GetMessage(string routeUri, string routeTemplate);

        /// <summary>
        /// Get messages by Area and Portal Id.
        /// </summary>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <param name="localeId">Current Locale Id.</param>
        /// <returns>Returns list of Messages.</returns>
        string GetMessages(string routeUri, string routeTemplate, int localeId);
    }
}
