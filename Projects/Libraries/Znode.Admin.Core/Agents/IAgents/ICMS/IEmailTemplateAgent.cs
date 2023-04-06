using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.ViewModels;
using System.Collections.Generic;

namespace Znode.Engine.Admin.Agents
{
    public interface IEmailTemplateAgent
    {
        /// <summary>
        /// Gets the list of all email templates.
        /// </summary>
        /// <param name="filters">Filters for email templates.</param>
        /// <param name="sortCollection">Sorts for email template.</param>
        /// <param name="pageIndex">Start page index of email template list.</param>
        /// <param name="recordPerPage">Record per page of email templates.</param>
        /// <returns>List of email templates.</returns>
        EmailTemplateListViewModel EmailTemplates(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get an email template.
        /// </summary>
        /// <param name="emailTemplateId">Email template ID of model.</param>
        /// <param name="localeId">Locale id.</param>
        /// <returns>Email Template view model.</returns>
        EmailTemplateViewModel GetEmailTemplate(int emailTemplateId,int localeId);

        /// <summary>
        /// Create an Email template.
        /// </summary>
        /// <param name="emailTemplateViewModel">View model of the email template to be created.</param>
        /// <returns>Created email template.</returns>
        EmailTemplateViewModel CreateEmailTemplate(EmailTemplateViewModel emailTemplateViewModel);

        /// <summary>
        /// Update an email template.
        /// </summary>
        /// <param name="emailTemplateViewModel">Update values of email template.</param>
        /// <returns>Updated Email template view model.</returns>
        EmailTemplateViewModel UpdateEmailTemplate(EmailTemplateViewModel emailTemplateViewModel);

        /// <summary>
        /// Delete an email template.
        /// </summary>
        /// <param name="emailtemplateId">Id of the email template to be deleted.</param>      
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteEmailTemplate(string emailtemplateId);

        /// <summary>
        /// Get an email template tokens.
        /// </summary>
        /// <returns>Email Template tokens.</returns>
        string GetEmailTemplateTokens();

        /// <summary>
        /// Get All Mapped Email Template Area Details.
        /// </summary>
        /// <param name="portalId">Portal id.</param>
        /// <returns>Return the Mapped Email Template Area Details.</returns>
        EmailTemplateAreaDataViewModel GetMappedEmailTemplateArea(int portalId);

        /// <summary>
        /// Delete Email Template Area Mapping.
        /// </summary>
        /// <param name="areaMappingId">ids of Email Template Area Mapping.</param>
        /// <param name="message"></param>
        /// <returns>true/false</returns>
        bool DeleteEmailTemplateAreaMapping(string areaMappingId);

        /// <summary>
        /// Save the Email Template Area Configuration Details.
        /// </summary>
        /// <param name="model">Model values for the Email Template Area Configuration</param>
        /// <returns>Return true or false</returns>
        bool SaveEmailTemplateAreaConfiguration(EmailTemplateAreaMapperViewModel model);

        /// <summary>
        /// Get Available Email Template Area.
        /// </summary>
        /// <param name="portalId">Portal Id.</param>
        /// <returns>Return Mapped Email Template Area in Model</returns>
        EmailTemplateAreaMapperViewModel GetAvailableTemplateArea(int portalId);

        /// <summary>
        /// Get Email Template list by search Key. 
        /// </summary>
        /// <param name="searchTerm">Search value to get in list.</param>
        /// <returns>Returns Email Template List.</returns>
        List<EmailTemplateViewModel> GetEmailTemplateListByName(string searchTerm);

        /// <summary>
        /// Get Email Template Token.
        /// </summary>
        /// <param name="emailTemplateViewModel">EmailTemplateViewModel.</param>
        void GetEmailTemplateToken(EmailTemplateViewModel emailTemplateViewModel);
    }
}
