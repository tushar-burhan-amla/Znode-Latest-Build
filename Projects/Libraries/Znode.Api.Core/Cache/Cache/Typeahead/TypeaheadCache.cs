using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class TypeaheadCache : BaseCache, ITypeaheadCache
    {
        #region Private Variable

        private readonly ITypeaheadService _typeaheadService;

        #endregion Private Variable

        #region Constructor
        public TypeaheadCache(ITypeaheadService typeaheadService)
        {
            _typeaheadService = typeaheadService;
        }
        #endregion        
    }
}
