using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IWebStoreCaseRequestService
    {
        /// <summary>
        /// Create CaseRequest.
        /// </summary>
        /// <param name="caseRequestModel">CaseRequest Model.</param>
        /// <returns>Returns created CaseRequest Model.</returns>
        WebStoreCaseRequestModel CreateContactUs(WebStoreCaseRequestModel caseRequestModel);

        /// <summary>
        /// Get the list of case request.
        /// </summary>
        /// <param name="filters">List of filter tuples</param>
        /// <param name="sorts">Collection of sorting parameters</param>
        /// <param name="page">Collection of paging parameters</param>
        /// <returns>List of case request.</returns>
        WebStoreCaseRequestListModel GetCaseRequests(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Create Case Request.
        /// </summary>
        /// <param name="webStoreCaseRequestModel">WebStoreCaseRequestModel</param>
        /// <returns>WebStoreCaseRequestModel</returns>
        WebStoreCaseRequestModel CreateCaseRequest(WebStoreCaseRequestModel webStoreCaseRequestModel);

        /// <summary>
        /// Get case request on the basis of caseRequestId.
        /// </summary>
        /// <param name="caseRequestId">caseRequestId.</param>
        /// <returns>Returns WebStore CaseRequest Model.</returns>
        WebStoreCaseRequestModel GetCaseRequest(int caseRequestId);

        /// <summary>
        /// Update CaseRequest data.
        /// </summary>
        /// <param name="webStoreCaseRequestModel">WebStoreCaseRequestModel to update.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdateCaseRequest(WebStoreCaseRequestModel webStoreCaseRequestModel);

        /// <summary>
        /// Create Case Request.
        /// </summary>
        /// <param name="webStoreCaseRequestModel">WebStoreCaseRequestModel</param>
        /// <returns>WebStoreCaseRequestModel</returns>
        bool ReplyCustomer(WebStoreCaseRequestModel webStoreCaseRequestModel);

        /// <summary>
        /// Get the list of case request.
        /// </summary>
        /// <returns>List of case request.</returns>
        CaseTypeListModel GetCaseTypeList();

        /// <summary>
        /// Get the list of case request.
        /// </summary>
        /// <returns>List of case request.</returns>
        CaseStatusListModel GetCaseStatusList();

        /// <summary>
        /// Get the list of case request.
        /// </summary>
        /// <returns>List of case request.</returns>
        CasePriorityListModel GetCasePriorityList();
    }
}
