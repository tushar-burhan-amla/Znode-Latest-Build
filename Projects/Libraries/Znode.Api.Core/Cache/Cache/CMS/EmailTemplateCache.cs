using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
namespace Znode.Engine.Api.Cache
{
    public class EmailTemplateCache : BaseCache, IEmailTemplateCache
    {
        private readonly IEmailTemplateService _service;
        public EmailTemplateCache(IEmailTemplateService emailTemplateService)
        {
            _service = emailTemplateService;
        }

        //Get email template list. 
        public virtual string GetEmailTemplates(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            //If Data From Cache Is Null Then Give Service call
            if (string.IsNullOrEmpty(data))
            {
                EmailTemplateListModel model = _service.GetEmailTemplateList(Expands, Filters, Sorts, Page);
                if (model.EmailTemplatesList.Count > 0)
                {
                    EmailTemplateListResponse response = new EmailTemplateListResponse { EmailTemplates = model.EmailTemplatesList };
                    response.MapPagingDataFromModel(model);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }

            return data;
        }

        //Get email template by emailTemplateId.
        public virtual string GetTemplatePage(int emailTemplateId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                EmailTemplateModel templatePage = _service.GetEmailTemplate(emailTemplateId, Filters);
                if (!Equals(templatePage, null))
                {
                    EmailTemplateResponse response = new EmailTemplateResponse { EmailTemplate = templatePage };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get email template tokens.
        public virtual string GetEmailTemplateTokens(string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                EmailTemplateModel templatePage = _service.GetEmailTemplateTokens();
                if (!Equals(templatePage, null))
                {
                    EmailTemplateResponse response = new EmailTemplateResponse { EmailTemplate = templatePage };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get the Email Template Area List
        public virtual string GetEmailTemplateAreaList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            //If Data From Cache Is Null Then Give Service call
            if (string.IsNullOrEmpty(data))
            {
                EmailTemplateAreaListModel model = _service.GetEmailTemplateAreaList(Expands, Filters, Sorts, Page);
                if (model.EmailTemplatesAreaList.Count > 0)
                {
                    EmailTemplateAreaListResponse response = new EmailTemplateAreaListResponse { EmailTemplateAreas = model.EmailTemplatesAreaList };
                    response.MapPagingDataFromModel(model);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
    }
}