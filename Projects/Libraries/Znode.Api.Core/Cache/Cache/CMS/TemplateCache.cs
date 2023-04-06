using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class TemplateCache : BaseCache, ITemplateCache
    {
        #region Private Variable
        private readonly ITemplateService _service;
        #endregion

        #region Constructor
        public TemplateCache(ITemplateService templateService)
        {
            _service = templateService;
        }
        #endregion

        #region Public Methods

        //Get the list of Template.
        public virtual string GetTemplates(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                TemplateListModel list = _service.GetTemplates(Expands, Filters, Sorts, Page);
                if (list?.Templates?.Count > 0)
                {
                    TemplateListResponse response = new TemplateListResponse { Templates = list.Templates };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Get Template by cmsTemplateId.
        public virtual string GetTemplate(int cmsTemplateId, string routeUri, string routeTemplate)
        {
            //Get data from Cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response.
                TemplateModel templateModel = _service.GetTemplate(cmsTemplateId, Expands);
                if (HelperUtility.IsNotNull(templateModel))
                {
                    TemplateListResponse response = new TemplateListResponse { Template = templateModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion
    }
}