using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class SliderListResponse : BaseListResponse
    {
        public List<SliderModel> Sliders { get; set; }
    }
}
