using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IWebStoreCaseRequestClient : IBaseClient
    {
        /// <summary>
        /// Create CreateContactUs.
        /// </summary>
        /// <param name="WebStoreCaseRequestModel">WebStoreCaseRequestModel Model.</param>
        /// <returns>Returns created CaseRequest Model.</returns>
        WebStoreCaseRequestModel CreateContactUs(WebStoreCaseRequestModel webStoreCaseRequestModel);

        /// Get the list of case requests.
        /// </summary>
        /// <param name="expands">Expand collection for case request list.</param>
        /// <param name="filters">Filter Collection for case request list.</param>
        /// <param name="sorts">Sort collection of case request list.</param>
        /// <param name="pageIndex">Index of requesting page</param>
        /// <param name="pageSize">Record per page</param>
        /// <returns>List of slider</returns>
        WebStoreCaseRequestListModel GetCaseRequests(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create new Case Request.
        /// </summary>
        /// <param name="caseRequestModel">WebStoreCaseRequestModel</param>
        /// <returns>WebStoreCaseRequestModel</returns>
        WebStoreCaseRequestModel CreateCaseRequest(WebStoreCaseRequestModel caseRequestModel);

        /// <summary>
        /// Get case request on the basis of caseRequestId.
        /// </summary>
        /// <param name="caseRequestId">caseRequestId to get case request details.</param>
        /// <returns>Returns CaseRequestModel.</returns>
        WebStoreCaseRequestModel GetCaseRequest(int caseRequestId);

        /// <summary>
        /// Update case request data.
        /// </summary>
        /// <param name="caseRequestModel">CaseRequest model to update.</param>
        /// <returns>Returns updated WebStore CaseRequest Model.</returns>
        WebStoreCaseRequestModel UpdateCaseRequest(WebStoreCaseRequestModel caseRequestModel);

        /// <summary>
        /// To get case status list
        /// </summary>
        /// <returns>Return case status list</returns>
        CaseStatusListModel GetCaseStatusList();

        /// <summary>
        /// To get case priority list.
        /// </summary>
        /// <returns>Return case Priority List</returns>
        CasePriorityListModel GetCasePriorityList();

        /// <summary>
        /// To get case type list.
        /// </summary>
        /// <returns>Return case type List</returns>
        CaseTypeListModel GetCaseTypeList();

        /// <summary>
        /// To Reply Customer
        /// </summary>
        /// <param name="model">WebStoreCaseRequestModel model</param>
        /// <param name="ErrorMessage">out string ErrorMessage</param>
        /// <returns>returns model.</returns>
        WebStoreCaseRequestModel ReplyCustomer(WebStoreCaseRequestModel model);
    }
}
