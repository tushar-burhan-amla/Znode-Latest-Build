using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CityListResponse : BaseListResponse
    {
        public List<CityModel> Cities { get; set; }
    }
}
