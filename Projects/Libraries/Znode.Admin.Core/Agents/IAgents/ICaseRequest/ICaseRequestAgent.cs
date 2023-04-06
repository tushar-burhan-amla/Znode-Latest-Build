using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface ICaseRequestAgent
    {
        /// <summary>
        /// To get CaseRequest list
        /// </summary>
        /// <param name="filters">FilterCollection filters</param>
        /// <param name="sortCollection">SortCollection sortCollection</param>
        /// <param name="pageIndex">int? pageIndex</param>
        /// <param name="recordPerPage">int? recordPerPage</param>
        /// <returns>returns list of CaseRequest</returns>
        CaseRequestListViewModel GetCaseRequests(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// To get CaseRequests by id
        /// </summary>
        /// <param name="caseRequestId">int caseRequestId</param>
        /// <returns>returns CaseRequest model</returns>
        CaseRequestViewModel GetCaseRequest(int caseRequestId);

        /// <summary>
        /// To Save Case Request
        /// </summary>
        /// <param name="model">CaseRequestViewModel model</param>
        /// <returns>returns CaseRequestViewModel</returns>
        CaseRequestViewModel CreateCaseRequest(CaseRequestViewModel model);

        /// <summary>
        /// To update Case Request
        /// </summary>
        /// <param name="model">CaseRequestViewModel model</param>
        /// <returns>returns CaseRequestViewModel</returns>
        CaseRequestViewModel UpdateCaseRequest(CaseRequestViewModel model);

        /// <summary>
        /// To bind page dropdown for create/edit
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        CaseRequestViewModel BindPageDropdown(CaseRequestViewModel model);

        /// <summary>
        /// Get Note list from database.
        /// </summary>
        /// <param name="filters">Filters collection.</param>
        /// <param name="sorts">Sort collection.</param>
        /// <param name="page">Page Number.</param>
        /// <returns>Returns NoteListViewModel</returns>
        NoteListViewModel GetNotes(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// To save Case Requests notes
        /// </summary>
        /// <param name="model">NotesViewModel model</param>
        /// <returns>returns NotesViewModel</returns>
        NoteViewModel SaveCaseRequestsNotes(NoteViewModel model);

        /// <summary>
        /// Sets filter for case requestId.
        /// </summary>
        /// <param name="filters">Filter collection.</param>
        /// <param name="caseRequestId">caseRequestId.</param>
        void SetFiltersForCaseRequestId(FilterCollection filters, int caseRequestId);

        /// <summary>
        /// To ReplyCustomer
        /// </summary>
        /// <param name="model">CaseRequestViewModel model</param>
        /// <param name="ErrorMessage">out string ErrorMessage</param>
        /// <returns>returns true/false</returns>
        CaseRequestViewModel ReplyCustomer(CaseRequestViewModel model);

        /// <summary>
        ///  Get mail history list to customer.
        /// </summary>
        /// <param name="filters">FilterCollection filters</param>
        /// <param name="sortCollection">SortCollection sortCollection</param>
        /// <param name="pageIndex">int? pageIndex</param>
        /// <param name="recordPerPage">int? recordPerPage</param>
        /// <returns>returns mail history list of CaseRequest</returns>
        CaseRequestListViewModel GetCaseRequestMailHistory(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);
    }
}