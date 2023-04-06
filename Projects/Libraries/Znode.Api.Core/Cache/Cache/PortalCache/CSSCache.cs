using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class CSSCache : BaseCache, ICSSCache
    {
        //to do
        #region Private Variable
        private readonly ICSSService _service;
        #endregion

        #region Constructor
        public CSSCache(ICSSService service)
        {
            _service = service;
        }
        #endregion

        public virtual string GetCssListByThemeId(int cmsThemeId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);

            if (string.IsNullOrEmpty(data))
            {
                CSSListModel list = _service.GetCssListByThemeId(cmsThemeId, Filters, Sorts, Page);
                if (list?.CSSs?.Count > 0 || !string.IsNullOrEmpty(list?.CMSThemeName))
                {
                    CSSListResponse response = new CSSListResponse { CSSs = list?.CSSs, CMSThemeName = list?.CMSThemeName };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
    }
}