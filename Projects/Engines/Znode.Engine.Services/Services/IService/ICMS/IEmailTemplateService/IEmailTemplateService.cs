using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IEmailTemplateService
    {
        /// <summary>
        ///Gets the list of email templates.
        /// </summary>
        /// <param name="expands"> Expands for email template list.</param>
        /// <param name="filters">Filters for email template list.</param>
        /// <param name="sorts">Sorts for email template list.</param>
        /// <param name="page">Paging information about email template list.</param>
        /// <returns>Email template list model.</returns>
        EmailTemplateListModel GetEmailTemplateList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Creates an email template.
        /// </summary>
        /// <param name="emailTemplateModel">Email template model to be created.</param>
        /// <returns>Created email template.</returns>
        EmailTemplateModel CreateTemplatePage(EmailTemplateModel model);

        /// <summary>
        /// Gets an email template according to ID.
        /// </summary>
        /// <param name="emailTemplateId">ID of the email Template.</param>
        /// <param name="filters">Filter to pass.</param>
        /// <returns>Email template of the specified ID.</returns>
        EmailTemplateModel GetEmailTemplate(int emailTemplateId, FilterCollection filters);

        /// <summary>
        /// Updates an email template.
        /// </summary>
        /// <param name="emailTemplateModel">Email template model to be updated.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdateTemplatePage(EmailTemplateModel emailTemplateModel);

        /// <summary>
        /// Delete an email template.
        /// </summary>
        /// <param name="emailTemplateIds">Email Template Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteTemplatePage(ParameterModel emailTemplateIds);

        /// <summary>
        /// Gets an email template tokens.
        /// </summary>
        /// <returns>Email template tokens.</returns>
        EmailTemplateModel GetEmailTemplateTokens();

        /// <summary>
        ///Gets the list of email template areas.
        /// </summary>
        /// <param name="expands"> Expands for email template list.</param>
        /// <param name="filters">Filters for email template list.</param>
        /// <param name="sorts">Sorts for email template list.</param>
        /// <param name="page">Paging information about email template list.</param>
        /// <returns>Email template Area list model.</returns>
        EmailTemplateAreaListModel GetEmailTemplateAreaList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets the list of email template area Mapper.
        /// </summary>
        /// <param name="portalId">Portal Id.</param>
        /// <returns>Email template Area Mapper list model.</returns>
        EmailTemplateAreaMapperListModel GetEmailTemplateAreaMapperList(int portalId);

        /// <summary>
        /// Delete Email Template Area Configuration.
        /// </summary>
        /// <param name="parameterModel">model with ids</param>
        /// <returns>true/false</returns>
        bool DeleteEmailTemplateAreaConfiguration(ParameterModel parameterModel);

        /// <summary>
        /// Save the Email Template Area Configuration Details.
        /// </summary>
        /// <param name="model">Model values for the Email Template Area Configuration</param>
        /// <returns>Return true or false</returns>
        bool SaveEmailTemplateAreaConfiguration(EmailTemplateAreaMapperModel model);

    }
}
