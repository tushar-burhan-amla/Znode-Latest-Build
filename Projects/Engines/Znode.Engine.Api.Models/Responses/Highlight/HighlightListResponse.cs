using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class HighlightListResponse : BaseListResponse
    {
        public List<HighlightModel> Highlights { get; set; }
        public List<PIMAttributeDefaultValueModel> HighlightCodes { get; set; }
    }
}
