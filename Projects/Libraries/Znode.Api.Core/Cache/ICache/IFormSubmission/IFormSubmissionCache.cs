using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;

namespace Znode.Engine.Api.Cache
{
    public interface IFormSubmissionCache
    {
        /// <summary>
        /// Get form submission list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns form submission list.</returns>
        string GetFormSubmissionList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Form Submission Details
        /// </summary>
        /// <param name="formSubmitId">int form submit Id</param>
        /// <param name="routeUri">string routeUri</param>
        /// <param name="routeTemplate">string routeTemplate</param>
        /// <returns>returns form submit details</returns>
        string GetFormSubmitDetails(int formSubmitId, string routeUri, string routeTemplate);

        /// <summary>
        ///  Get Export Response Message for Form Submission.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <param name="exportFileTypeId">export File Type Id.</param>
        /// <returns>returns response message for form submission list.</returns>
        string GetFormSubmissionListforExport(string routeUri, string routeTemplate, string exportType);
    }
}
