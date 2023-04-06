using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class TimeZoneListModel : BaseListModel
    {
        public List<TimeZoneModel> TimeZones { get; set; }

        public TimeZoneListModel()
        {
            TimeZones = new List<TimeZoneModel>();
        }
    }
}
