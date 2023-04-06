namespace Znode.Engine.Api.Cache
{
    public interface IAccountQuoteCache 
    {
        /// <summary>
        /// Get Account Quote on the basis of omsQuoted.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns Account Quote.</returns>
        string GetAccountQuote(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Account Quote list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns Account Quote list.</returns>
        string GetAccountQuoteList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get template list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns template list.</returns>
        string GetTemplateList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get the list of user approvers.
        /// </summary>
        /// <param name="routeUri">Route uri.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>Returns list of user approvers.</returns>
        string GetUserApproverList(string routeUri, string routeTemplate);

        /// <summary>
        /// Method to check if the current user is an approver to any other user and has approvers itself.
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <param name="routeUri">Route uri.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>Returns model containing data about whether the user is an approver to someone and has approvers itself.</returns>
        string UserApproverDetails(int userId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get pending payments and pending orders count for showing account menus
        /// </summary>
        /// <param name="routeUri">Route uri.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>Returns model containing data about pending payments and pending orders count</returns>
        string GetUserDashboardPendingOrderDetailsCount(string routeUri, string routeTemplate);

        #region Template
        /// <summary>
        /// Get account template.
        /// </summary>
        /// <param name="omsTemplateId">OmsTemplateId</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns Account template model.</returns>
        string GetAccountTemplate(int omsTemplateId, string routeUri, string routeTemplate);
        #endregion
    }
}
