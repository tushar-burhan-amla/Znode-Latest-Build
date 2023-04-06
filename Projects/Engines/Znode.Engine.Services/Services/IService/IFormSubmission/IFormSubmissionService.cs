using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IFormSubmissionService
    {
        /// <summary>
        /// Get the list of form submission.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with form submission list.</param>
        /// <param name="filters">Filters to be applied on form submission list.</param>
        /// <param name="sorts">Sorting to be applied on form submission list.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Returns list of form submission.</returns>
        FormSubmissionListModel GetFormSubmissionList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get Form Submission Details.
        /// </summary>
        /// <param name="formSubmitId">int formSubmitId</param>
        /// <returns>returns FormBuilderAttributeGroupModel</returns>
        FormBuilderAttributeGroupModel GetFormSubmitDetails(int formSubmitId);

        /// <summary>
        /// Get All Forms List For Export.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with form submission list.</param>
        /// <param name="filters">Filters to be applied on form submission list.</param>
        /// <param name="sorts">Sorting to be applied on form submission list.</param>
        /// <param name="page">Page index.</param>
        /// <param name="exportFileTypeId">export File Type Id</param>
        /// <returns>returns list of form submission</returns>
        ExportModel GetAllFormsListForExport(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page, string exportType);

        /// <summary>
        /// Get Form Submission Export Data Set Response.
        /// </summary>
        /// <param name="filters">Filters to be applied on form submission list.</param>
        /// <param name="pageListModel">page List Model.</param>
        /// <param name="exportFileTypeId">export File Type Id.</param>
        /// <returns>returns dataset response.</returns>
        ExportModel GetFormSubmissionExportDataSetResponse(FilterCollection filters, PageListModel pageListModel, string exportType);

    }
}
