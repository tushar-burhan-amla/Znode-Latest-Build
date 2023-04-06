using System.Collections.Generic;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Admin.Models;
namespace Znode.Engine.Admin.Agents
{
    public interface IFormSubmissionAgent
    {
        /// <summary>
        /// Gets the list of Form Submission.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Form Submission list.</param>
        /// <param name="filters">Filters to be applied on Form Submission list.</param>
        /// <param name="sorts">Sorting to be applied on Form Submission list.</param>
        /// <param name="pageIndex">Start page index of Form Submission list.</param>
        /// <param name="pageSize">Page size of Form Submission list.</param>
        /// <returns>Returns Form Submission list.</returns>
        FormSubmissionListViewModel GetFormSubmissionList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get form submission details and groups.
        /// </summary>
        /// <param name="formSubmitId">Int FormSubmitId</param>
        /// <returns>returns Form Builder Attribute Group ViewModel</returns>
        FormBuilderAttributeGroupViewModel GetFormSubmitDetails(int formSubmitId);

        /// <summary>
        /// Get Response message for Export Form Submission.
        /// </summary>
        /// <param name="exportFileTypeId">export File Type Id.</param>
        /// <param name="filters">Filters to be applied on Form Submission list.</param>
        /// <param name="sorts">Sorting to be applied on Form Submission list.</param>
        /// <param name="pageIndex">Start page index of Form Submission list.</param>
        /// <param name="pageSize">Page size of Form Submission list.</param>
        /// <returns>returns Export Form Submission List.</returns>
        ExportResponseMessageModel GetExportFormSubmissionList(string exportType, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null);
    }
}
