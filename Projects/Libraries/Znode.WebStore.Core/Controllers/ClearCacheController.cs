using System.Web.Http;
using Znode.Engine.WebStore.Agents;

namespace Znode.Engine.WebStore.Controllers
{
    public class ClearCacheController : ApiController
    {
        #region Private Variables
        private readonly IClearCacheAgent _clearCacheAgent;
        #endregion

        #region Constructor
        public ClearCacheController(IClearCacheAgent clearCacheAgent)
        {
            _clearCacheAgent = clearCacheAgent;
        }
        #endregion

        #region Public Methods

        //This method will clear the cache of Search and Category page.
        [HttpGet]
        public virtual string ClearAllCache()
         => _clearCacheAgent.ClearCache();

        #endregion
    }
}
