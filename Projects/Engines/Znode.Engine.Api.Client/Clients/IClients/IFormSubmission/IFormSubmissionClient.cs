using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IFormSubmissionClient : IBaseClient
    {
        /// <summary>
        /// Gets the list of Form Submission.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Form Submission list.</param>
        /// <param name="filters">Filters to be applied on Form Submission list.</param>
        /// <param name="sorts">Sorting to be applied on Form Submission list.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>Returns FormSubmission list.</returns>
        FormSubmissionListModel GetFormSubmissionList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get Form submission details
        /// </summary>
        /// <param name="FormSubmitId">int FormSubmitId</param>
        /// <returns>returns FormBuilderAttributeGroupModel</returns>
        FormBuilderAttributeGroupModel GetFormSubmitDetails(int formSubmitId);

        /// <summary>
        /// Get form submission details for export 
        /// </summary>
        /// <param name="exportFileTypeId">export File Type Id</param>
        /// <param name="expands">Expands to be retrieved along with Form Submission list.</param>
        /// <param name="filters">Filters to be applied on Form Submission list.</param>
        /// <param name="sorts">Sorting to be applied on Form Submission list.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>Returns FormSubmission list response.</returns>

        ExportModel GetExportFormSubmissionList(string exportType, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

    }
}
