using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class DisplayUnitListResponse :  BaseListResponse
    {
        public List<DisplayUnitModel> DisplayUnits { get; set; }
    }
}
