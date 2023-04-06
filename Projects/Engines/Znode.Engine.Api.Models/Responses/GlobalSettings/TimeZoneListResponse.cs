using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class TimeZoneListResponse : BaseListResponse
    {
        public List<TimeZoneModel> TimeZones { get; set; }
    }
}
