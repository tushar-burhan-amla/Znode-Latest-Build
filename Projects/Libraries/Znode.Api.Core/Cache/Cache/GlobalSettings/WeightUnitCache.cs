using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class WeightUnitCache : BaseCache, IWeightUnitCache
    {
        #region Private Variables
        private readonly IWeightUnitService _service;
        #endregion

        #region Constructor
        public WeightUnitCache(IWeightUnitService weightUnitService)
        {
            _service = weightUnitService;
        }
        #endregion

        #region Public Methods

        //Get a list of all WeightUnits.
        public virtual string GetWeightUnits(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get WeightUnit List.
                WeightUnitListModel list = _service.GetWeightUnits(Expands, Filters, Sorts, Page);
                if (list?.WeightUnits?.Count > 0)
                {
                    //Create response.
                    WeightUnitListResponse response = new WeightUnitListResponse { WeightUnits = list.WeightUnits };

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