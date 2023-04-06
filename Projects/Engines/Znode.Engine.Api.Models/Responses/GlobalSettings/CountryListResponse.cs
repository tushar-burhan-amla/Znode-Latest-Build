using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class CountryListResponse: BaseListResponse
    {
        public List<CountryModel> Countries { get; set; }
    }
}
