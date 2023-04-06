using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class DynamicContentCache : BaseCache, IDynamicContentCache
    {
        #region Private Variable
        private readonly IDynamicContentService _dynamicContentService;
        #endregion

        #region Constructor
        public DynamicContentCache(IDynamicContentService dynamicContentService)
        {
            _dynamicContentService = dynamicContentService;
        }
        #endregion

        #region Public methods
        public string GetEditorFormats(int portalId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                EditorFormatListModel modelList = _dynamicContentService.GetEditorFormats(portalId);
                if (IsNotNull(modelList))
                    data = InsertIntoCache(routeUri, routeTemplate, new EditorFormatsResponse { EditorFormatList = modelList });
            }
            return data;
        }
        #endregion
    }
}
