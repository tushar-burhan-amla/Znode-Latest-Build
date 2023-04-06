namespace Znode.Engine.Api.Cache
{
    public interface IWebStoreCaseRequestCache
    {
        /// <summary>
        /// Get the list of case request.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>String Data.</returns>
        string GetCaseRequests(string routeUri, string routeTemplate);

        /// <summary>
        /// Get case request on the basis of caseRequestId.
        /// </summary>
        /// <param name="caseRequestId">caseRequestId.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns case Request.</returns>
        string GetCaseRequest(int caseRequestId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get the list of case priority.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>String Data.</returns>
        string GetCasePriorityList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get the list of case status.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>String Data.</returns>
        string GetCaseStatusList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get the list of case type.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>String Data.</returns>
        string GetCaseTypeList(string routeUri, string routeTemplate);
    }
}