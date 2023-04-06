using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IEmailTemplateClient : IBaseClient
    {
        /// <summary>
        ///Gets the list of email templates.
        /// </summary>
        /// <param name="expands"> Expands for email template list.</param>
        /// <param name="filters">Filters for email template list.</param>
        /// <param name="sorts">Sorts for email template list.</param>
        /// <returns>Email template list model.</returns>
        EmailTemplateListModel GetTemplates(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        ///Gets the list of email templates.
        /// </summary>
        /// <param name="expands"> Expands for email template list.</param>
        /// <param name="filters">Filters for email template list.</param>
        /// <param name="sorts">Sorts for email template list.</param>
        /// <param name="pageIndex">Start page index for email template list.</param>
        /// <param name="pageSize">Page size of email template list.</param>
        /// <returns>Email template list model.</returns>
        EmailTemplateListModel GetTemplates(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Creates and email template.
        /// </summary>
        /// <param name="emailTemplateModel">Email template model to be created.</param>
        /// <returns>Created email template.</returns>
        EmailTemplateModel CreateTemplatePage(EmailTemplateModel emailTemplateModel);

        /// <summary>
        /// Gets an email template according to emailTemplateId.
        /// </summary>
        /// <param name="emailTemplateId">ID of the email Template.</param>
        /// <param name="filters">Filter to pass.</param>
        /// <returns>Email template of the specified ID.</returns>
        EmailTemplateModel GetTemplatePage(int emailTemplateId, FilterCollection filters);

        /// <summary>
        /// Updates an email template.
        /// </summary>
        /// <param name="emailTemplateModel">Email template model to be updated.</param>
        /// <returns>True if email template is updated else False.</returns>
        EmailTemplateModel UpdateTemplatePage(EmailTemplateModel emailTemplateModel);

        /// <summary>
        /// Deletes an email template.
        /// </summary>
        /// <param name="emailTemplateId">emailTemplateId to be deleted.</param>
        /// <returns>True if email template is deleted else False.</returns>
        bool DeleteTemplatePage(ParameterModel emailTemplateId);

        /// <summary>
        /// Gets an email template tokens.
        /// </summary>
        /// <returns>Email template tokens.</returns>
        EmailTemplateModel GetEmailTemplateTokens();

        /// <summary>
        ///Gets the list of Email Templates Areas List.
        /// </summary>
        /// <param name="filters">Filters for email template list.</param>
        /// <param name="sorts">Sorts for email template list.</param>
        /// <returns>Email template list model.</returns>
        EmailTemplateAreaListModel GetEmailTemplateAreaList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets the list of Email Templates Areas Mapper Details List.
        /// </summary>
        /// <param name="portalId">Portal Id.</param>
        /// <returns>Returns Email Templates Areas Mapper Details List.</returns>
        EmailTemplateAreaMapperListModel GetEmailTemplateAreaMapperList(int portalId);

        /// <summary>
        /// Delete Email Template Area Mapping.
        /// </summary>
        /// <param name="areaMappingId">model with ids</param>
        /// <returns>true/false</returns>
        bool DeleteEmailTemplateAreaMapping(ParameterModel areaMappingId);

        /// <summary>
        /// Save the Email Template Area Configuration Details.
        /// </summary>
        /// <param name="model">Model values for the Email Template Area Configuration</param>
        /// <returns>Return true or false</returns>
        bool SaveEmailTemplateAreaConfiguration(EmailTemplateAreaMapperModel model);
    }
}
