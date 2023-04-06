using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class ContainerTemplateCache: BaseCache, IContainerTemplateCache
    {
        #region Private Variable
        private readonly IContainerTemplateService _service;
        #endregion

        #region Constructor
        public ContainerTemplateCache(IContainerTemplateService containerTemplateService)
        {
            _service = containerTemplateService;
        }
        #endregion

        public virtual string List(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ContainerTemplateListModel list = _service.GetContainerTemplateList(Expands, Filters, Sorts, Page);
                if (list?.ContainerTemplates?.Count > 0)
                {
                    ContainerTemplateListResponse response = new ContainerTemplateListResponse { ContainerTemplates = list.ContainerTemplates };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Get Container Template
        public virtual string GetContainerTemplate(string templateCode, string routeUri, string routeTemplate)
        {
            //Get data from Cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response.
                ContainerTemplateModel model = _service.GetContainerTemplate(templateCode);
                if (HelperUtility.IsNotNull(model))
                {
                    ContainerTemplateListResponse response = new ContainerTemplateListResponse { ContainerTemplate = model };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
    }
}
