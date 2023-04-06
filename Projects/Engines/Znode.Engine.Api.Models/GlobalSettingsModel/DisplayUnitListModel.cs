using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class DisplayUnitListModel :  BaseListModel
    {
        public List<DisplayUnitModel> DisplayUnits { get; set; }

        public DisplayUnitListModel()
        {
            DisplayUnits = new List<DisplayUnitModel>();
        }
    }
}
