using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class StateCache : BaseCache, IStateCache
    {
        #region Private Variables
        private readonly IStateService _service;
        #endregion

        #region Constructor
        public StateCache(IStateService stateService)
        {
            _service = stateService;
        }
        #endregion

        #region Public Methods

        //Get a list of all states.
        public virtual string GetStateList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                StateListModel list = _service.GetStateList(Filters, Sorts, Page);
                if (list?.States?.Count > 0)
                {
                    //Create response.
                    StateListResponse response = new StateListResponse { States = list.States };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion
    }
}