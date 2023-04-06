using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
namespace Znode.Engine.Api.Cache
{
    public class HighlightCache : BaseCache, IHighlightCache
    {
        private readonly IHighlightService _service;
        public HighlightCache(IHighlightService highlightService)
        {
            _service = highlightService;
        }

        //Get highlight list. 
        public virtual string GetHighlights(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            //If Data From Cache Is Null Then Give Service call
            if (string.IsNullOrEmpty(data))
            {
                HighlightListModel listmodel = _service.GetHighlightList(Expands, Filters, Sorts, Page);
                if (listmodel.HighlightList.Count > 0)
                {
                    HighlightListResponse response = new HighlightListResponse { Highlights = listmodel.HighlightList };
                    response.MapPagingDataFromModel(listmodel);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get highlight by highlightId.
        public virtual string GetHighlight(int highlightId, int productId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                HighlightModel highlight = _service.GetHighlight(highlightId, productId, Filters);
                if (!Equals(highlight, null))
                {
                    HighlightResponse response = new HighlightResponse { Highlight = highlight };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get highlight by highlightCode.
        public virtual string GetHighlightByCode(string highLightCode, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                HighlightModel highlight = _service.GetHighlightByCode(highLightCode, Filters);
                if (!Equals(highlight, null))
                {
                    HighlightResponse response = new HighlightResponse { Highlight = highlight };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get highlight code list.
        public virtual string GetHighlightCodeList(string attributeCode, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //highlight code list
                HighlightListModel list = _service.GetAvailableHighlightCodes(attributeCode);
                if (list?.HighlightCodes?.Count > 0)
                {
                    //Get response and insert it into cache.
                    HighlightListResponse response = new HighlightListResponse { HighlightCodes = list.HighlightCodes };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #region Highlight Type

        //Get highlight type list.
        public virtual string GetHighlightTypeList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Highlight type list.
                HighlightTypeListModel highlightTypeList = _service.GetHighlightTypeList(Filters, Sorts);
                if (highlightTypeList?.HighlightTypes?.Count > 0)
                {
                    //Get response and insert it into cache.
                    HighlightTypeListResponse response = new HighlightTypeListResponse { HighlightTypeList = highlightTypeList.HighlightTypes, TemplateTokens = highlightTypeList.TemplateTokens };
                    response.MapPagingDataFromModel(highlightTypeList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion      
    }
}