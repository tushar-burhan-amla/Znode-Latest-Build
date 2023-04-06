using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class CMSWidgetsCache : BaseCache, ICMSWidgetsCache
    {
        #region Private Variable
        private readonly ICMSWidgetsService _service;
        #endregion

        #region Constructor
        public CMSWidgetsCache(ICMSWidgetsService _cmsWidgetService)
        {
            _service = _cmsWidgetService;
        }
        #endregion

        #region Public Methods
        //Get CMS Widgets list.
        public virtual string List(string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get the Widget List from the database.
                CMSWidgetsListModel list = _service.List(Filters, Sorts, Page);
                if (IsNotNull(list))
                {
                    //Create Response
                    CMSWidgetsListResponse response = new CMSWidgetsListResponse { CMSWidgetsList = list?.CMSWidgetsList };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}
