using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class TagManagerCache : BaseCache,ITagManagerCache
    {
        #region Private Variable
        private readonly ITagManagerService _tagManagerService;
        #endregion

        #region Constructor
        public TagManagerCache(ITagManagerService tagManagerService)
        {
            _tagManagerService = tagManagerService;
        }
        #endregion

        //Get tag manager data by portalId.
        public virtual string GetTagManager(int portalId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //get tag manager details by portal id.
                TagManagerModel model = _tagManagerService.GetTagManager(portalId, Expands);
                if (IsNotNull(model))
                {
                    TagManagerResponse response = new TagManagerResponse { TagManagerModel = model };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
    }
}
